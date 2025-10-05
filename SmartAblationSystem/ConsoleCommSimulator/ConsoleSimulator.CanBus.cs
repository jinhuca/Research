using ConsoleCommSimulator.Data;

namespace ConsoleCommSimulator
{
  public partial class ConsoleSimulator
  {
    public void SendDataToCanBus(uint messageId, byte[] dataToSend, bool messageframeformats, bool ReadingFirmwareVersion = false)
    {
      UpdateProviderParameters(messageId, dataToSend);
    }

    public void SendDataToCanBusTwo(uint messageId, byte[] dataToSend, bool messageframeformats)
    {
      UpdateProviderParameters(messageId, dataToSend);
    }

    private void UpdateProviderParameters(uint messageId, byte[] dataToSend)
    {
      if (_canBusMessageProviders == null) return;
      var parameters = new CanBusMessageParameters { MessageId = messageId, Data = dataToSend };
      foreach (var provider in _canBusMessageProviders)
      {
        provider.UpdateParameters(parameters);
      }
    }
  }
}
