using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Communication;
using ConsoleCommSimulator.Configuration;
using ConsoleCommSimulator.Data;
using ConsoleCommSimulator.Interfaces;
using DataAccessLayer;
using Prism.Events;
using Log = LogSystem.LogService;


namespace ConsoleCommSimulator.MessageProviders
{
  public class CatheterInfoMessageProvider : MessageProviderBase
  {
    private static string CATHETER_CONFIG_NODE_ID = "CatheterConfig";
    private static uint CATHETER_50_MESSAGE_ID = 0x32; // 50
    private static uint CATHETER_51_MESSAGE_ID = 0x33; // 51
    private static uint HIGH_FLOW_CATHETER_ID = 130;

    private Timer _messageUpdateTimer;
    private CatheterInfoMessageConfig _cathetherInfoMessageConfig;
    private readonly DataAccess _dataAccess;
    public CatheterInfoMessageProvider(IEventAggregator eventAggregator, ISimulatorConfiguration configuration, DataAccess dataAccess) :
      base(eventAggregator, configuration)
    {
      // Since both messages are in PMCU the node is the same
      NodeId = ConvertElementToNodeOne(CATHETER_50_MESSAGE_ID);
      _dataAccess = dataAccess;
    }

    public override void Initialize()
    {
      base.Initialize();
      // this only sends when unplug/plug so init, 10 sec timer, send message, then stop. 

      _cathetherInfoMessageConfig = new CatheterInfoMessageConfig();
      var loadConfig = _cathetherInfoMessageConfig.Parse(GetConfigurationNode(CATHETER_CONFIG_NODE_ID));
      if (loadConfig)
      {
        // limit flow if it's not a high flow catheter
        if (_cathetherInfoMessageConfig.CatheterData[0] == HIGH_FLOW_CATHETER_ID)
        {
          FlowCatheterId = 130;
        }

        // if it is not engineering catheter 
        if ((_cathetherInfoMessageConfig.CatheterData[0] & 0x80) == 0 && _cathetherInfoMessageConfig.IsNewCatheter)
        {
          RemoveCatheterIfExists(
            _cathetherInfoMessageConfig.CatheterId,
            _cathetherInfoMessageConfig.CatheterSn, 
            _cathetherInfoMessageConfig.CatheterLot);
        } 

        // default value is published every 10 seconds
        // keep publishing/waiting until it gets acknowledgement
        _messageUpdateTimer = new Timer(100);
        _messageUpdateTimer.Elapsed += PublishCatheterMessage;
        Task.Delay(_cathetherInfoMessageConfig.CatheterWait).ContinueWith( _=> _messageUpdateTimer.Start());
      }
      else
      {
        Log.LogInfo("Parsing configuration failed");
      }

    }

    private void RemoveCatheterIfExists(int catheterId, int serialNum, int lotNum)
    {
      var isEngineering = (catheterId & 0x80) != 0x0; 
      _dataAccess?.RemoveCatheterInformationsAccordingToSerialNumberAndLotIfExists(serialNum, lotNum, catheterId, isEngineering);
    } 

    public override void UpdateParameters(CanBusMessageParameters parameters)
    {
      base.UpdateParameters(parameters);
      var messageElements = SplitCanBusMessageId(parameters.MessageId);

      // Receives Catheter RTR message, then stop sending message  
      if (messageElements.Item1 == NodeId && messageElements.Item2 == CATHETER_50_MESSAGE_ID
          && IsCatheterValid(parameters.Data))
      {
        _messageUpdateTimer?.Stop();
      } 
      else if (messageElements.Item1 == NodeId && messageElements.Item2 == CATHETER_51_MESSAGE_ID)
      {
        lock (_cathetherInfoMessageConfig)
        {
          _cathetherInfoMessageConfig.FirstUseCatheterData = parameters.Data.ToArray();
        }
      } 
      
    }

    protected override void DisposeMessageProvider()
    {
      // Stop Timer
      _messageUpdateTimer?.Stop();
    }

    private bool IsCatheterValid(byte[] data)
    {
      return data[0] != 0 && data[1] != 0;
    }

    private void PublishCatheterMessage(object sender, ElapsedEventArgs e)
    {
      lock (_cathetherInfoMessageConfig)
      {

        var message50 = new CanBusMessage()
        {

          Id = CanBusId.CanBus1,
          CanBusEventArgs = new CanBusEventArgs()
          { Cob = 0, Falgs = 4, Id = CreateMessageId(CurrentStateId, CATHETER_50_MESSAGE_ID), Length = 8, Data = _cathetherInfoMessageConfig.CatheterData }
        };

        var message51 = new CanBusMessage()
        {

          Id = CanBusId.CanBus1,
          CanBusEventArgs = new CanBusEventArgs()
          {
            Cob = 0, Falgs = 4, Id = CreateMessageId(CurrentStateId, CATHETER_51_MESSAGE_ID), Length = 7, Data = _cathetherInfoMessageConfig.FirstUseCatheterData
          }
        };

        PublishCanBusMessage(message50);
        // wait 10 ms
        System.Threading.Thread.Sleep(10);
        PublishCanBusMessage(message51);

        System.Threading.Thread.Sleep(10);
        PublishCanBusMessage(message50);
      }
    }
  }
}
