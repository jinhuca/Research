using System.Collections.Generic;
using System.Timers;
using Communication;
using ConsoleCommSimulator.Configuration;
using ConsoleCommSimulator.Data;
using ConsoleCommSimulator.Interfaces;
using Prism.Events;
using Log = LogSystem.LogService;

namespace ConsoleCommSimulator.MessageProviders
{
  public class ETSMessageProvider : MessageProviderBase
  {
    private static string ETS_CONFIG_NODE_ID = "ETSConfig";
    private static uint ETS_MESSAGE_ID_5 = 5; 
    private static uint ETS_MESSAGE_ID_6 = 6; 
    private static CanBusMessageDefinition.MessageStateId DEFAULT_STATE = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE;
    private static int DEFAULT_INTERVAL = 50;

    private Timer _messageUpdateTimer;
    private ETSMessageConfig _etsMessageConfig;
    private IDictionary<string, StateToETSValue> _etsStates = new Dictionary<string, StateToETSValue>();

    private byte[] _etsData5 = new byte[8];
    private byte[] _etsData6 = new byte[8];

    public ETSMessageProvider(IEventAggregator eventAggregator, ISimulatorConfiguration configuration) :
      base(eventAggregator, configuration)
    {
      NodeId = ConvertElementToNodeTwo(ETS_MESSAGE_ID_5);
    }

    public override void Initialize()
    {
      base.Initialize();
      _etsMessageConfig = new ETSMessageConfig();
      var loadConfig = _etsMessageConfig.Parse(GetConfigurationNode(ETS_CONFIG_NODE_ID));
      if (loadConfig)
      {
        _messageUpdateTimer = new Timer(_etsMessageConfig.Interval <= 0 ? DEFAULT_INTERVAL : _etsMessageConfig.Interval);
        _etsStates = _etsMessageConfig.StateToETSMap;
        _messageUpdateTimer.Elapsed += PublishETS5Message;
        _messageUpdateTimer.Start();
      }
      else
      {
        Log.LogInfo("Parsing configuration failed");
      }

    }

    protected override void HandleSystemStateUpdate(ConsoleStateMessage message)
    {
      base.HandleSystemStateUpdate(message);
      UpdateETSGoal(message.State);
    }
    private void UpdateETSGoal(CanBusMessageDefinition.MessageStateId stateNumber)
    {
      string etslocation = ConvertStateNumberToString(stateNumber);
      _etsData5[0] = (byte)(_etsStates[etslocation].Channel0 & 0xFF);
      _etsData5[1] = (byte)(_etsStates[etslocation].Channel1 & 0xFF);
      _etsData5[2] = (byte)(_etsStates[etslocation].Channel2 & 0xFF);
      _etsData5[3] = (byte)(_etsStates[etslocation].Channel3 & 0xFF);
      _etsData5[4] = (byte)(_etsStates[etslocation].Channel4 & 0xFF);
      _etsData5[5] = (byte)(_etsStates[etslocation].Channel5 & 0xFF);
      _etsData5[6] = (byte)(_etsStates[etslocation].Channel6 & 0xFF);
      _etsData5[7] = (byte)(_etsStates[etslocation].Channel7 & 0xFF);

      _etsData6[0] = (byte)(_etsStates[etslocation].Channel8 & 0xFF);
      _etsData6[1] = (byte)(_etsStates[etslocation].Channel9 & 0xFF);
      _etsData6[2] = (byte)(_etsStates[etslocation].Channel10 & 0xFF);
      _etsData6[3] = (byte)(_etsStates[etslocation].Channel11 & 0xFF);

    }

    protected override void DisposeMessageProvider()
    {
      // Stop Timer
      _messageUpdateTimer?.Stop();
    }
    private void PublishETS5Message(object sender, ElapsedEventArgs e)
    {
      var message = new CanBusMessage()
      {
        Id = CanBusId.CanBus2,
        CanBusEventArgs = new CanBusEventArgs()
        { Cob = 0, Falgs = 4, Id = CreateMessageId(CurrentStateId, ETS_MESSAGE_ID_5), Length = 8, Data = _etsData5 }
      };

      PublishCanBusMessage(message);
 
      PublishETS6Message(sender, e);
    }

    private void PublishETS6Message(object sender, ElapsedEventArgs e)
    {
      var message = new CanBusMessage()
      {
        Id = CanBusId.CanBus2,
        CanBusEventArgs = new CanBusEventArgs()
        { Cob = 0, Falgs = 4, Id = CreateMessageId(CurrentStateId, ETS_MESSAGE_ID_6), Length = 8, Data = _etsData6 }
      };

      PublishCanBusMessage(message);
    }


  }
}
