using Modules.Infrastructure.Definitions;

namespace Modules.CanBusCommunication;

public interface ICanBusCommunication
{
  event EventHandler<CanBusEventArgs> MessageReceivedOne;

  /// <summary>
  /// Gets or sets events from CAN 1
  /// </summary>
  CanBusEventArgs CanBusOneEventArgs { get; set; }

  /// <summary>
  /// Gets or sets events from CAN 1
  /// </summary>
  CanBusEventArgs CanBusTwoEventArgs { get; set; }

  event EventHandler<CanBusEventArgs> MessageReceivedTwo;

  /// <summary>
  /// Frees the unmanaged resources.
  /// </summary>
  void Dispose();

  /// <summary>
  /// Sends data to CAN 1
  /// </summary>
  /// <param name="messageId">message id</param>
  /// <param name="dataToSend">data to send</param>
  /// <param name="messageframeformats">message frame formats</param>
  void SendDataToCanBus(uint messageId, byte[] dataToSend, bool messageframeformats, bool ReadingFirmwareVersion = false);

  /// <summary>
  /// Sends data to CAN 2
  /// </summary>
  /// <param name="messageId">message Id</param>
  /// <param name="dataToSend">data to send</param>
  /// <param name="messageframeformats">message frame formats</param>
  void SendDataToCanBusTwo(uint messageId, byte[] dataToSend, bool messageframeformats);
}