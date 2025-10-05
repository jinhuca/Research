using System.Timers;
using Communication;
using ConsoleCommSimulator.Configuration;
using ConsoleCommSimulator.Data;
using ConsoleCommSimulator.Interfaces;
using Prism.Events;
using System.Collections.Generic;
using Log = LogSystem.LogService;
using System;

namespace ConsoleCommSimulator.MessageProviders
{
  // ECG78 is pacing: for idle, ready, inflation, thawing, -1 is normal
  // for transition and ablation, real DMS data is sent 
  // look in DMSLogic.cs
  public class DMSSensorMessageProvider : MessageProviderBase
  {
    private static string ICB_SENSOR_NODE = "DMSConfig";
    private static uint _messageId = 8;
    private static uint _messageId1 = 1;

    private static double DEFAULT_ECG12 = 36.09;
    private static double DEFAULT_ESOTEMP = 25.0;
    private static double DEFAULT_ECG78 = 76.3;

    private static double SPEED_RATIO = 1; // there's a 36.09 so we can't use small speed

    private Timer _messageUpdateTimer;
    private static CanBusMessageDefinition.MessageStateId DEFAULT_STATE = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE;
    private IDictionary<string, StateToDMSValue> _statesDictionary;
    private int _pointIndex = 0; // index of where we are on the list of points for dms wave
    private List<double> _points = new List<double>();
    private DMSSensorMessageConfig _DMSSensorMessageConfig;
    private int NumberOfPositions = 70; // used to calculate step of the sine wave, the lower the more frequent

    private double _ecg12 = DEFAULT_ECG12;
    private double _esotemp = DEFAULT_ESOTEMP;
    private double _ecg78 = DEFAULT_ECG78;

    private double _ecg12Goal;
    private double _esotempGoal;
    private double _ecg78Goal;


    public DMSSensorMessageProvider(IEventAggregator eventAggregator, ISimulatorConfiguration configuration) 
      : base(eventAggregator, configuration)
    {
      NodeId = 3; 
    }

    public override void Initialize()
    {
      base.Initialize();
      _DMSSensorMessageConfig = new DMSSensorMessageConfig();
      var loadConfig = _DMSSensorMessageConfig.Parse(GetConfigurationNode(ICB_SENSOR_NODE));
      if (loadConfig)
      {

        _statesDictionary = _DMSSensorMessageConfig.StateToDMSMap;

        _messageUpdateTimer = new Timer(_DMSSensorMessageConfig.ECGInterval <= 0 ? 100 : _DMSSensorMessageConfig.ECGInterval);
        // now generate the points based on the config
        UpdateICBGoal(DEFAULT_STATE);
        UpdatePoints(DEFAULT_STATE);
        _messageUpdateTimer.Elapsed += PublishMessage;
        _messageUpdateTimer.Start();
      }
      else
      {
        Log.LogInfo("Parsing configuration failed");
      }


    }

    protected override void DisposeMessageProvider()
    {
      // Stop Timer
      _messageUpdateTimer?.Stop();
    }
    protected override void HandleSystemStateUpdate(ConsoleStateMessage message)
    {
      base.HandleSystemStateUpdate(message);
      UpdateICBGoal(message.State);
      UpdatePoints(message.State);

    }
    private void UpdateICBGoal(CanBusMessageDefinition.MessageStateId stateNumber)
    {
      string location = ConvertStateNumberToString(stateNumber);

      _ecg12Goal = _statesDictionary[location].ECG12;
      _esotempGoal = _statesDictionary[location].ESOTEMP;
      _ecg78Goal = _statesDictionary[location].ECG78;
    }

    private void UpdatePoints(CanBusMessageDefinition.MessageStateId state)
    {
      // draws a sine wave
      string stateString = ConvertStateNumberToString(state);
      var tempDiaphragmGraph = _statesDictionary[stateString].DiaphragmGraph;
      // this is the amplitude centered at 0 
      double xStep = 2 * Math.PI / NumberOfPositions;
      _points.Clear();
      for (double x = 0; x < 2 * Math.PI; x += xStep)
      {
        // y = a sin(bx) where a is the amplitude
        double yValue = tempDiaphragmGraph * Math.Sin(x);
        _points.Add(yValue);
      }
    }

    private void PublishMessage(object sender, ElapsedEventArgs e)
    {
      // 0D76000005DCFFF6
      // 05dc == 15 
      // update _pointIndex every time publish message is called
      if (_pointIndex == NumberOfPositions-1)
      {
        _pointIndex = 0;
      }
      else
      {
        _pointIndex++;
      }
     
      var bytesarray = BitConverter.GetBytes((int)(_points[_pointIndex]));

      _ecg12 = IncrementDecimal(_ecg12, _ecg12Goal, SPEED_RATIO);
      _esotemp = 30; //IncrementDecimal(_esotemp, _esotempGoal, 0.1);
      _ecg78 = 85; //IncrementDecimal(_ecg78, _ecg78Goal, SPEED_RATIO);

      var ecg12 = (int)(_ecg12 * 100);
      var esotemp = (int)(_esotemp * 10);
      var ecg78 = (int)(_ecg78 * 10); // this one uses convert negative decimal instead of ecg
      var ECG12byte1 = (byte)((ecg12 >> 8) & 0xFF); 
      var ECG12byte2 = (byte)(ecg12 & 0xFF);
      var diaphragmByte1 = bytesarray[1];   
      var diaphragmByte2 = bytesarray[0];
      var esoByte1 = (byte)((esotemp >> 8) & 0xFF); 
      var esoByte2 = (byte)(esotemp & 0xFF); 
      var pacingByte1 = (byte)((ecg78 >> 8) & 0xFF); 
      var pacingByte2 = (byte)(ecg78 & 0xFF);

      var message = new CanBusMessage()
      {

        Id = CanBusId.CanBus2,
        CanBusEventArgs = new CanBusEventArgs()
          { Cob = 0, Falgs = 4, Id = CreateMessageId(CurrentStateId, _messageId), Length = 8, Data = new byte[] { ECG12byte1, ECG12byte2, diaphragmByte1, diaphragmByte2, esoByte1, esoByte2, pacingByte1, pacingByte2 } }
      };

      // var messageConnected = new CanBusMessage()
      // {
      //   // looks like 0300000000000000
      //   Id = CanBusId.CanBus2,
      //   CanBusEventArgs = new CanBusEventArgs()
      //   { Cob = 0, Falgs = 4, Id = CreateMessageId(CurrentStateId, _messageId1), Length = 8, Data = _DMSSensorMessageConfig.DMSSetting }
      // };

      PublishCanBusMessage(message);
      // PublishCanBusMessage(messageConnected);

    }
  }
}
