using System.Timers;
using Communication;
using ConsoleCommSimulator.Configuration;
using ConsoleCommSimulator.Data;
using ConsoleCommSimulator.Interfaces;
using Prism.Events;
using System.Collections.Generic;
using Log = Serilog.Log;
using System;

namespace ConsoleCommSimulator.MessageProviders
{
  // blood pressure
  public class ICBPressureMessageProvider : MessageProviderBase
  {
    private static string ICB_SENSOR_NODE = "ICBPressureConfig";
    private static uint _messageId = 7;

    private static double DEFAULT_PRESSURE01 = 0.01;
    private static double DEFAULT_PRESSURE23 = 7.26;
    private static double DEFAULT_PRESSURE45 = 23.34;
    private static double DEFAULT_PRESSURE67 = 34.35;
    private static double SPEED_RATIO = 0.5; 

    private Timer _messageUpdateTimer;
    private static CanBusMessageDefinition.MessageStateId DEFAULT_STATE = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE;
    private IDictionary<string, StateToICBValue> _statesDictionary;
    private ICBSensorMessageConfig _ICBSensorMessageConfig;

    private double _p01 = DEFAULT_PRESSURE01;
    private double _p23 = DEFAULT_PRESSURE23;
    private double _p45 = DEFAULT_PRESSURE45;
    private double _p67 = DEFAULT_PRESSURE67;

    private double _p01Goal;
    private double _p23Goal;
    private double _p45Goal;
    private double _p67Goal;


    public ICBPressureMessageProvider(IEventAggregator eventAggregator, ISimulatorConfiguration configuration) 
      : base(eventAggregator, configuration)
    {
      NodeId = 3; 
    }

    public override void Initialize()
    {
      base.Initialize();
      _ICBSensorMessageConfig = new ICBSensorMessageConfig();
      var loadConfig = _ICBSensorMessageConfig.Parse(GetConfigurationNode(ICB_SENSOR_NODE));
      if (loadConfig)
      {

        _statesDictionary = _ICBSensorMessageConfig.StateToICBMap;

        _messageUpdateTimer = new Timer(_ICBSensorMessageConfig.ECGInterval <= 0 ? 100 : _ICBSensorMessageConfig.ECGInterval);
        // now generate the points based on the config
        UpdateICBGoal(DEFAULT_STATE);
        
        _messageUpdateTimer.Elapsed += PublishMessage;
        _messageUpdateTimer.Start();
      }
      else
      {
        Log.Error("Parsing configuration failed");
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

    }
    private void UpdateICBGoal(CanBusMessageDefinition.MessageStateId stateNumber)
    {
      string location = ConvertStateNumberToString(stateNumber);

      _p01Goal = _statesDictionary[location].Pressure01;
      _p23Goal = _statesDictionary[location].Pressure23;
      _p45Goal = _statesDictionary[location].Pressure45;
      _p67Goal = _statesDictionary[location].Pressure67;
    }

    private void PublishMessage(object sender, ElapsedEventArgs e)
    {

      _p01 = IncrementDecimal(_p01, _p01Goal, SPEED_RATIO);
      _p23 = IncrementDecimal(_p23, _p23Goal, SPEED_RATIO);
      _p45 = IncrementDecimal(_p45, _p45Goal, SPEED_RATIO);
      _p67 = IncrementDecimal(_p67, _p67Goal, SPEED_RATIO);

      var p01 = (int)(_p01 * 100);
      var p23 = (int)(_p23 * 100);
      var p45 = (int)(_p45 * 100);
      var p67 = (int)(_p67 * 100);

      var p01byte1 = (byte)((p01 >> 8) & 0xFF); 
      var p01byte2 = (byte)(p01 & 0xFF);
      var p23byte1 = (byte)((p23 >> 8) & 0xFF);
      var p23byte2 = (byte)(p23 & 0xFF);
      var p45byte1 = (byte)(p45 & 0xFF);
      var p45byte2 = (byte)(p45 & 0xFF);
      var p67byte1 = (byte)((p67 >> 8) & 0xFF); 
      var p67byte2 = (byte)(p67 & 0xFF);

      var message = new CanBusMessage()
      {

        Id = CanBusId.CanBus2,
        CanBusEventArgs = new CanBusEventArgs()
          { Cob = 0, Falgs = 4, Id = CreateMessageId(CurrentStateId, _messageId), Length = 8, Data = new byte[] { p01byte1, p01byte2, p23byte1, p23byte2, p45byte1, p45byte2, p67byte1, p67byte2 } }
      };

      PublishCanBusMessage(message);

    }
  }
}
