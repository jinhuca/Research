using System;
using System.Collections.Generic;
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
  public class LCMessageProvider : MessageProviderBase
  {
    private static string LC_CONFIG_NODE_ID = "LCConfig";
    private static uint LC_MESSAGE_ID = 4; 
    private static CanBusMessageDefinition.MessageStateId DEFAULT_STATE = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE;
    private static int DEFAULT_INTERVAL = 50;
    private static int DEFAULT_LC_INTERVAL = 60000;
    private static double DEFAULT_LC1 = 0;
    private static double SPEED_RATIO = 0.2;
    private Timer _messageUpdateTimer;
    private Timer _lcTankTimer;
    private LCMessageConfig _lcMessageConfig;
    private double _lc1 = (int)DEFAULT_LC1;
    private double _lcUsedOffsetAmount = 0.0313; // per min as defined in pounds to time converter 
    private LC1ThresholdValidation _lc1ThresholdValidation;
    private double _lc1Goal;

    private int _lcUsedInterval; // default is 60000 (60 seconds)
    private byte[] _lcData = new byte[8];

    public LCMessageProvider(IEventAggregator eventAggregator, ISimulatorConfiguration configuration,
      LC1ThresholdValidation lc1ThresholdValidation) :
      base(eventAggregator, configuration)
    {
      NodeId = ConvertElementToNodeOne(LC_MESSAGE_ID);
      _lc1ThresholdValidation = lc1ThresholdValidation;
    }

    public override void Initialize()
    {
      base.Initialize();
      _lcMessageConfig = new LCMessageConfig();
      var loadConfig = _lcMessageConfig.Parse(GetConfigurationNode(LC_CONFIG_NODE_ID));
      if (loadConfig)
      {
        _messageUpdateTimer = new Timer(_lcMessageConfig.Interval <= 0 ? DEFAULT_INTERVAL : _lcMessageConfig.Interval);
        _lcUsedInterval = _lcMessageConfig.LCInterval <= 0 ? DEFAULT_LC_INTERVAL : _lcMessageConfig.LCInterval;
        _lc1Goal = _lcMessageConfig.LC1Value;
        _messageUpdateTimer.Elapsed += PublishPSMessage;
        _messageUpdateTimer.Start();
      }
      else
      {
        Log.LogInfo("Parsing configuration failed");
      }
    }

    private void DecrementLCTank(object sender, ElapsedEventArgs e)
    {
      _lc1Goal -= _lcUsedOffsetAmount;
    }

    protected override void HandleSystemStateUpdate(ConsoleStateMessage message)
    {
      base.HandleSystemStateUpdate(message);
      if (message.State == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION || 
        message.State == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_ABLATION)
      { //start at transition or ablation if not started
        if (_lcTankTimer == null || !_lcTankTimer.Enabled)
        {
          _lcTankTimer = new Timer(_lcUsedInterval); // call it every minute by default, 1 second for testing
          _lcTankTimer.Elapsed += DecrementLCTank;
          _lcTankTimer.Start();
        }
      }
      else if (message.State == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING ||
        message.State == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_EXCEPTION)
      {
        if (_lcTankTimer != null && _lcTankTimer.Enabled)
        {
          // stop at thawing
          _lcTankTimer.Stop();
        }

      }
    }
    protected override void DisposeMessageProvider()
    {
      // Stop Timer
      _messageUpdateTimer?.Stop();
      _lcTankTimer?.Stop();
    }
    private void BuildLC()
    {
      _lc1 = IncrementDecimal(_lc1, _lc1Goal, SPEED_RATIO);

      var lc1 = (int)(_lc1 * 10);
 
      _lcData[0] = (byte)((lc1 >> 8) & 0xFF);
      _lcData[1] = (byte)(lc1 & 0xFF);

    }

    private void PublishPSMessage(object sender, ElapsedEventArgs e)
    {
      BuildLC();
      _lc1ThresholdValidation.ValidateThresholds(_lc1, CurrentStateId);
      var message = new CanBusMessage()
      {
        Id = CanBusId.CanBus1,
        CanBusEventArgs = new CanBusEventArgs()
        { Cob = 0, Falgs = 4, Id = CreateMessageId(CurrentStateId, LC_MESSAGE_ID), Length = 2, Data = _lcData }
      };

      PublishCanBusMessage(message);

    }

  }
}
