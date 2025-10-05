using Modules.Infrastructure.Definitions;

namespace Modules.CanBusCommunication;

public class CanBusCommunication : ICanBusCommunication, IModule
{
  public CanBusEventArgs CanBusOneEventArgs { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
  public CanBusEventArgs CanBusTwoEventArgs { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

  public event EventHandler<CanBusEventArgs> MessageReceivedOne;
  public event EventHandler<CanBusEventArgs> MessageReceivedTwo;

  public void Dispose()
  {
    throw new NotImplementedException();
  }

  public void SendDataToCanBus(uint messageId, byte[] dataToSend, bool messageframeformats, bool ReadingFirmwareVersion = false)
  {
    throw new NotImplementedException();
  }

  public void SendDataToCanBusTwo(uint messageId, byte[] dataToSend, bool messageframeformats)
  {
    throw new NotImplementedException();
  }

  public void RegisterTypes(IContainerRegistry containerRegistry)
  {
    throw new NotImplementedException();
  }

  public void OnInitialized(IContainerProvider containerProvider)
  {
    throw new NotImplementedException();
  }
}