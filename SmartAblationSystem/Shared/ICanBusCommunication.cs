using System;

namespace Communication
{
	public interface ICanBusCommunication
	{
		event EventHandler<CanBusEventArgs> MessageReceivedOne;

		/// <summary>
		/// Gets or sets events from CAN 1
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		CanBusEventArgs CanBusOneEventArgs { get; set; }

		/// <summary>
		/// Gets or sets events from CAN 1
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		CanBusEventArgs CanBusTwoEventArgs { get; set; }

		event EventHandler<CanBusEventArgs> MessageReceivedTwo;

		/// <summary>
		/// Frees the unmanaged resources.
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		void Dispose();

		/// <summary>
		/// Sends data to CAN 1
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="messageId">message id</param>
		/// <param name="dataToSend">data to send</param>
		/// <param name="messageframeformats">message frame formats</param>
		void SendDataToCanBus(uint messageId, byte[] dataToSend, bool messageframeformats, bool ReadingFirmwareVersion = false);

		/// <summary>
		/// Sends data to CAN 2
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="messageId">message Id</param>
		/// <param name="dataToSend">data to send</param>
		/// <param name="messageframeformats">message frame formats</param>
		void SendDataToCanBusTwo(uint messageId, byte[] dataToSend, bool messageframeformats);
	}
}