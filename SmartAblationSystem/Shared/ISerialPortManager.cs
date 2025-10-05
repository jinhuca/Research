using System;

namespace RS232Communication
{
  public interface ISerialPortManager
  {

    void SetPortName(string comPortName);
    void InitializeLSPROCRC32AndStart();

    LSPROEnumeartion LSPROEnumeartion { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the LSPRO is initialized
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool IsLsproInitialized { get; set; }

    /// <summary>
    /// Gets or sets the current serial port settings
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    SerialSettings CurrentSerialSettings { get; set; }

    event EventHandler<SerialDataEventArgs> NewSerialDataRecieved;

    /// <summary>
    /// Start serila port listening
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    void StartListening();

    /// <summary>
    /// Closes the serial port
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    void StopListening();

    /// <summary>
    /// Write to serila port
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="data">Data to send.</param>
    void Write(string data);

    /// <summary>
    /// send a packet
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="command">Command</param>
    /// <param name="data">Data</param>
    /// <param name="count">Count</param>
    void SendPacket(CCMPCommand command, byte[] data, byte[] count = null);
  }
}