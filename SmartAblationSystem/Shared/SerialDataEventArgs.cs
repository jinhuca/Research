using System;

namespace RS232Communication
{
  /// <summary>
  /// EventArgs used to send bytes recieved on serial port
  /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
  /// </summary>
  public class SerialDataEventArgs : EventArgs
  {
    /// <summary>
    /// Serial data Event Args
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="dataInByteArray">Data in byte array</param>
    public SerialDataEventArgs(byte[] dataInByteArray)
    {
      Data = dataInByteArray;
    }

    /// <summary>
    /// Byte array containing data from serial port
    /// </summary>
    public byte[] Data;
  }
}