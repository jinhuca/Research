using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using Communication;
using ConsoleCommSimulator.Configuration;
using ConsoleCommSimulator.Data;
using ConsoleCommSimulator.Interfaces;
using ConsoleCommSimulator.Validation;
using Prism.Events;
using Log = LogSystem.LogService;

namespace ConsoleCommSimulator.MessageProviders
{
  public class TCMessageProvider : MessageProviderBase
  {
    private static string TC_CONFIG_NODE_ID = "TCConfig";
    private static uint TC_MESSAGE_ID = 40;
    private static uint DEFLATE_MESSAGE_ID = 63;
    private static uint DEFLATE_NODE_ID = 2;
    private static byte AUTO_DEFLATE_BYTE = 0x04;
    private static CanBusMessageDefinition.MessageStateId DEFAULT_STATE = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE;
    private static int DEFAULT_INTERVAL = 500;
    private static double DEFAULT_TC1 = 36.0;
    private static double DEFAULT_TC2 = 36.0;
    private static double DEFAULT_PMCUCJ = 26.0;
    private static double DEFAULT_TEMP_READY = 20.0; // auto switch temp from thaw to ready
    private static double SPEED_RATIO = 0.2;
    private static string DEFAULT_TC_NAME = "default";
    private int _tcInterval = DEFAULT_INTERVAL;
    private Timer _messageUpdateTimer;
    private TCMessageConfig _tcMessageConfig;
    private IDictionary<string, StateToTCValue> _tcStates = new Dictionary<string, StateToTCValue>();
    public IDictionary<string, List<double>> _tcCurvesByName = new Dictionary<string, List<double>>();
    // current PS
    private double _tc1 = DEFAULT_TC1;
    private double _tc2 = DEFAULT_TC2;
    private double _pmcucj = DEFAULT_PMCUCJ;
    private double _tc1Goal;
    private double _tc2Goal;
    private double _pmcucjGoal;
    // tc curve from xml
    private List<double> _tcCurveData = new List<double>();
    private List<double> _tcFITCurveData = new List<double>();
    private int _dasEnabledPressure = 75;
    private int _dasDisabledPressure = 25;
    private int _tcDataInterval = 1000; //by default
    private int _thawingSlopeIndex = 0; // for thawing
    private bool _isDASEnabled = false;
    private CanBusMessageDefinition.MessageStateId _canBusMessageStateId;
    private Stopwatch _stopwatch = new Stopwatch();
    private byte[] _tcData = new byte[8];
    private bool _DeflateAfterThaw = false;
    private readonly Random _random = new Random(DateTime.Now.Millisecond);
    private TC1ThresholdValidation _tc1ThresholdValidation;

    bool _skipVerification = false;

    public TCMessageProvider(IEventAggregator eventAggregator, ISimulatorConfiguration configuration
      , TC1ThresholdValidation tC1ThresholdValidation) :
      base(eventAggregator, configuration)
    {
      NodeId = ConvertElementToNodeOne(TC_MESSAGE_ID);
      _tc1ThresholdValidation = tC1ThresholdValidation;
    }

    public override void Initialize()
    {
      base.Initialize();
      _tcMessageConfig = new TCMessageConfig();
      var loadConfig = _tcMessageConfig.Parse(GetConfigurationNode(TC_CONFIG_NODE_ID));

      if (loadConfig)
      {
        _tcCurvesByName = _tcMessageConfig.ThawingTCMap;
        LoadNewTCGraph(DEFAULT_TC_NAME);
        _tcInterval = _tcMessageConfig.Interval <= 0 ? DEFAULT_INTERVAL : _tcMessageConfig.Interval;
        _messageUpdateTimer = new Timer(_tcInterval);
        _tcStates = _tcMessageConfig.StateToTCMap;
        UpdateTCGoal(DEFAULT_STATE);
        _messageUpdateTimer.Elapsed += PublishTCMessage;
        _messageUpdateTimer.Start();
      }

      else
      {
        Log.LogInfo("Parsing configuration failed");
      }

    }
    public void LoadNewTCGraph(string name)
    {
      // use this to load new TC by name from control panel 
      var loadTCXmlConfig = _tcMessageConfig.LoadTCXML(GetConfigurationNode("TCData", name));
      var loadFITTCXmlConfig = _tcMessageConfig.LoadTCXML(GetConfigurationNode("TCFITData", name));
      var tcInterval = _tcMessageConfig.LoadTCInterval(GetConfigurationNode("TCDataInterval", name));
      if (loadTCXmlConfig != null && loadFITTCXmlConfig != null && tcInterval != -1)
      {
        _tcCurveData = loadTCXmlConfig;
        _tcFITCurveData = loadFITTCXmlConfig;
        _tcDataInterval = tcInterval;
      }
      else
      {
        Log.LogInfo("Parsing new TC graph configuration failed");
      }
    }
    public override void UpdateParameters(CanBusMessageParameters parameters)
    {
      base.UpdateParameters(parameters);
      var messageElements = SplitCanBusMessageId(parameters.MessageId);

      // check if message 63 has been sent for auto deflate
      // checking the correct byte of msg63
      if (messageElements.Item1 == DEFLATE_NODE_ID && messageElements.Item2 == DEFLATE_MESSAGE_ID)
      {
        if ((parameters.Data[1] & AUTO_DEFLATE_BYTE) != 0)
        {
          //flag is raised
          _DeflateAfterThaw = true;
        }
        else if ((parameters.Data[1] & AUTO_DEFLATE_BYTE) == 0)
        {
          //flag is lowered
          _DeflateAfterThaw = false;
        }

      }
      if (messageElements != null && messageElements.Item2 == 52) // check if 7.5 or 2.5
      {
        byte[] cp1InBytes = new[] { parameters.Data[1], parameters.Data[0] };
        int cp1Target = BitConverter.ToInt16(cp1InBytes, 0);
        _isDASEnabled = _dasEnabledPressure == cp1Target;
      }
    }

    protected override void HandleSystemStateUpdate(ConsoleStateMessage message)
    {
      base.HandleSystemStateUpdate(message);
      _skipVerification = false;
      _canBusMessageStateId = message.State;
      _thawingSlopeIndex = 0;
      UpdateTCGoal(message.State);
      if (message.State == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION)
      {
        if (!_stopwatch.IsRunning)
        {
          _stopwatch.Restart();
        }
      }
      else if (message.State != CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_ABLATION)
      {
        if (_stopwatch.IsRunning)
        {
          _stopwatch.Stop();
          _stopwatch.Reset();
        }
      } 

    }
    protected override void DisposeMessageProvider()
    {
      // Stop Timer
      _messageUpdateTimer?.Stop();
    }

    private double GetNextData(List<double> data)
    {
      int dataLength = data.Count;
      if (_thawingSlopeIndex >= dataLength - 1) _thawingSlopeIndex = dataLength - 2; // keep repeating last one if not stopped
      return _tc1 + data[_thawingSlopeIndex++];
    }
    private double InterpolateValue(double publishedInterval, TimeSpan currentTimeElapsed, List<double> data)
    {
      if (currentTimeElapsed <= TimeSpan.Zero)
        return data[0];

      double numPublishedIntervalsElapsed = currentTimeElapsed.TotalMilliseconds / publishedInterval;
      int lowerIndex = (int)numPublishedIntervalsElapsed;
      int upperIndex = lowerIndex + 1;
      // return directly if reached the end of data
      if (lowerIndex >= data.Count)
      {
        // biased so swings are less frequent
        var rv = Math.Pow(_random.NextDouble(), 5) * 2 - 1;
        return Math.Round(rv + data[data.Count - 1], 0);
      }
      double lowerElement = data[lowerIndex];
      double upperElement = upperIndex < data.Count ? data[upperIndex] : data[data.Count - 1];

      // calculate distance between lower index and current time
      double indexDelta = (numPublishedIntervalsElapsed - lowerIndex);
       
      return Math.Round((lowerElement * (1 - indexDelta) + upperElement * indexDelta), 0);
    }

    private void _delayed3sTurnOnVerification()
    {
      System.Threading.Thread.Sleep(3000);
      _skipVerification = false ;
    }

    private double GetCurrentData()
    {
      switch (_canBusMessageStateId)
      {

        case CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE:
        case CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY:
        case CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_INFLATION:

          var rv = _random.NextDouble() * 2 - 1;
          return Math.Round(rv + IncrementDecimal(_tc1, _tc1Goal, SPEED_RATIO), 0);
        // should stay around 37 or their value set in xml

        case CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION:
        case CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_ABLATION:
          if (_isDASEnabled) // use FIT curve
          {

            return InterpolateValue(_tcDataInterval, _stopwatch.Elapsed, _tcFITCurveData);

          }
          else // use the curve from xml
          {
            return InterpolateValue(_tcDataInterval, _stopwatch.Elapsed, _tcCurveData);
          }

        case CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING:
          if ((int)_tc1 == -9 || (int)_tc1 == -10 || (int)_tc1 == 5 || (int)_tc1 == 1)
          {
            _thawingSlopeIndex = 0;
          }
          if (_tc1 >= 39)
          { // stop infinite temperature gain
            return 39;
          }
          // plateau
          if ((int)_tc1 >= -10 && (int)_tc1 <= 1)
          {
            return Math.Round(GetNextData(_tcCurvesByName["_plateau"]), 0);

          }
          // regular thawing start gentle curve
          else if ((int)_tc1 >= -70 && (int)_tc1 < -10)
          {
            return Math.Round(GetNextData(_tcCurvesByName["_initial"]), 0);
          }
          else
          {
            // thawing end 
            return Math.Round(GetNextData(_tcCurvesByName["_end"]), 0);
          }
        default:
          return DEFAULT_TC1;
      }
    }
    private void UpdateTCGoal(CanBusMessageDefinition.MessageStateId stateNumber)
    {
      string tclocation = ConvertStateNumberToString(stateNumber);
      _tc1Goal = _tcStates[tclocation].TC1;
      _tc2Goal = _tcStates[tclocation].TC2;
      _pmcucjGoal = _tcStates[tclocation].PMCUCJ;
    }

    private void BuildTC()
    {
      _tc1 = GetCurrentData();
      _tc2 = IncrementDecimal(_tc2, _tc2Goal, SPEED_RATIO);
      _pmcucj = IncrementDecimal(_pmcucj, _pmcucjGoal, SPEED_RATIO);

      var tc1 = (int)(_tc1 * 10);
      var tc2 = (int)(_tc2 * 10);
      var pmcucj = (int)(_pmcucj * 10);

      _tcData[0] = (byte)((tc1 >> 8) & 0xFF);
      _tcData[1] = (byte)(tc1 & 0xFF);
      _tcData[2] = (byte)((tc2 >> 8) & 0xFF);
      _tcData[3] = (byte)(tc2 & 0xFF);
      _tcData[4] = (byte)((pmcucj >> 8) & 0xFF);
      _tcData[5] = (byte)(pmcucj & 0xFF);

    }

    private void PublishTCMessage(object sender, ElapsedEventArgs e)
    {
      BuildTC();
      _tc1ThresholdValidation.ValidateThresholds(_tc1, CurrentStateId);

      var message = new CanBusMessage()
      {
        Id = CanBusId.CanBus1,
        CanBusEventArgs = new CanBusEventArgs()
        { Cob = 0, Falgs = 4, Id = CreateMessageId(CurrentStateId, TC_MESSAGE_ID), Length = 6, Data = _tcData }
      };

      PublishCanBusMessage(message);

      if (CurrentStateId == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING && _tc1 >= DEFAULT_TEMP_READY && _DeflateAfterThaw == true)
      // only auto-switch if the flag was on
      // gui flags handled by msg63
      {
        // check if tc1 reached 20C
        ConsoleStateMessage newStateMessage = new ConsoleStateMessage();
        newStateMessage.State = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY;
        // auto switch state to ready if flag was set
        PublishSystemStateChange(newStateMessage);
      }
    }

  }
}
