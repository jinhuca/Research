using System;

namespace Module.FlowMeterComm.Models
{
  public enum SerialCommErrors
  {
    None,
    InitFailed,
    DeviceNoResponse
  }

  public class SerialComErrorEventArgs : EventArgs
  {
    public SerialCommErrors ErrorType { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
  }
}
