using System.Timers;
using Communication;
using ConsoleCommSimulator.Data;
using ConsoleCommSimulator.Interfaces;
using Prism.Events;

namespace ConsoleCommSimulator.MessageProviders
{
  public class CanBus2SensorStatusMessageProvider : MessageProviderBase
  {
    private static uint _messageId = 1; 
    private Timer _messageUpdateTimer;

    public CanBus2SensorStatusMessageProvider(IEventAggregator eventAggregator, ISimulatorConfiguration configuration) :
      base(eventAggregator, configuration)
    {
      NodeId = 3; 
    }

    public override void Initialize()
    {
      base.Initialize();
      _messageUpdateTimer = new Timer(1000d);
      _messageUpdateTimer.Elapsed += PublishMessage;
      _messageUpdateTimer.Start();
    }

    protected override void DisposeMessageProvider()
    {
      // Stop Timer
      _messageUpdateTimer?.Stop();
    }

    private void PublishMessage(object sender, ElapsedEventArgs e)
    {
      var message = new CanBusMessage()
      {

        Id = CanBusId.CanBus2,
        CanBusEventArgs = new CanBusEventArgs()
          { Cob = 0, Falgs = 4, Id = CreateMessageId(CurrentStateId, _messageId), Length = 1, Data = new byte[]{0x0f, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}  }
      };

      PublishCanBusMessage(message);
    }

  }
}
