using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Reflection;
using System.Threading;

namespace RS232Communication
{
  /// <summary>
  /// Represents the serial port manager
  ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
  /// </summary>
  public class SerialPortManager : IDisposable, ISerialPortManager
  {
    private static string DEFAULT_COM_PORT = "COM1";

    public LSPROEnumeartion LSPROEnumeartion => new LSPROEnumeartion();

    private DateTime timestampLastSend = DateTime.Now;
    //List<int> TestList = new List<int> { 1, 2, 3, 4 , 5, 6, 3, 9};
    List<int> listOfCTRL_SEQIndex = new List<int>();
    LSPROCRC32 LSPROCRC32 = new LSPROCRC32();

    int IncrementIndex = 0;

    private bool isLsproInitialized = false;

    public SerialPortManager()
    {
      try
      {
        // Finding installed serial ports on hardware
        _currentSerialSettings.PortNameCollection = SerialPort.GetPortNames();
        _currentSerialSettings.PropertyChanged += _currentSerialSettings_PropertyChanged;

        // If serial ports is found, we select the first found
        if (_currentSerialSettings.PortNameCollection.Length > 0)
          _currentSerialSettings.PortName = DEFAULT_COM_PORT;
      }
      catch (Exception ex)
      {
        IsLsproInitialized = false;
      }
    }

    public void SetPortName(string comPortName)
    {
      // If serial ports is found, we select the first found
      if (_currentSerialSettings.PortNameCollection.Length > 0)
        _currentSerialSettings.PortName = comPortName;
    }

    public void InitializeLSPROCRC32AndStart()
    {
      try
      {
        LSPROCRC32.Initialize();

        StartListening();

        IsLsproInitialized = true;
      }
      catch (IOException)
      {
        IsLsproInitialized = false;
      }
    }

    /// <summary>
    /// Dispose the serila port manager 
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    ~SerialPortManager()
    {
      Dispose(false);
    }

    #region Fields

    private SerialPort _serialPort;
    private SerialSettings _currentSerialSettings = new SerialSettings();
    private string _latestRecieved = String.Empty;

    public event EventHandler<SerialDataEventArgs> NewSerialDataRecieved;

    #endregion Fields

    #region Properties

    /// <summary>
    /// Gets or sets the current serial port settings
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public SerialSettings CurrentSerialSettings
    {
      get { return _currentSerialSettings; }
      set { _currentSerialSettings = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the LSPRO is initialized
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsLsproInitialized
    {
      get => isLsproInitialized;
      set => isLsproInitialized = value;
    }

    #endregion Properties

    #region Event handlers

    /// <summary>
    /// Occurs when current serial settings property changed
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="sender">Serial port</param>
    /// <param name="e">Event</param>
    private void _currentSerialSettings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      // if serial port is changed, a new baud query is issued
      if (e.PropertyName.Equals("PortName"))
        UpdateBaudRateCollection();
    }

    /// <summary>
    /// Occurs when serial port data received
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="sender">Serial port</param>
    /// <param name="e">Event</param>
    private void _serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
      if (_serialPort == null || !_serialPort.IsOpen) return;
      int dataLength = _serialPort.BytesToRead;
      byte[] data = new byte[dataLength];
      int nbrDataRead = _serialPort.Read(data, 0, dataLength);
      if (nbrDataRead == 0)
        return;

      // Send data to whom ever interested
      if (NewSerialDataRecieved != null)
        NewSerialDataRecieved(this, new SerialDataEventArgs(data));
    }

    #endregion Event handlers

    #region Methods

    /// <summary>
    /// Start serila port listening
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public void StartListening()
    {
      // Closing serial port if it is open
      if (_serialPort != null && _serialPort.IsOpen)
        _serialPort.Close();

      // Setting serial port settings
      _serialPort = new SerialPort(
          _currentSerialSettings.PortName,
          _currentSerialSettings.BaudRate,
          _currentSerialSettings.Parity,
          _currentSerialSettings.DataBits,
          _currentSerialSettings.StopBits);

      //The core million
      _serialPort.DtrEnable = true;
      _serialPort.RtsEnable = true;


      try
      {
        // Subscribe to event and open serial port for data
        _serialPort.DataReceived += _serialPort_DataReceived;
        _serialPort.Open();
      }
      catch (Exception ex)
      {
        _serialPort.DataReceived -= _serialPort_DataReceived;
        _serialPort = null;
        throw;
      }
    }

    /// <summary>
    /// Closes the serial port
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public void StopListening()
    {
      _serialPort?.Close();
      _serialPort = null;
    }


    /// <summary>
    /// Write to serila port
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="data">Data to send.</param>
    public void Write(string data)
    {
      byte[] bytestosend = { 0xD };
      if (_serialPort != null)
      {
        try
        {
          if (data == "R")
            _serialPort.Write(data);
          if (data == "X")
            _serialPort.Write(data);
          if (data == "Y")
            _serialPort.Write(data);
          if (data == "J")
            _serialPort.Write(data);



          else
          {
            string dataWithoutFirstLetter = data.Remove(0, 1);


            // PID ONE
            if (data.Contains("P"))
            {
              _serialPort.Write("P");
            }

            if (data.Contains("I"))
            {
              _serialPort.Write("I");
            }

            if (data.Contains("D"))
            {
              _serialPort.Write("D");
            }


            //PID 2
            if (data.Contains("E"))
            {
              _serialPort.Write("E");
            }

            if (data.Contains("F"))
            {
              _serialPort.Write("F");
            }

            if (data.Contains("G"))
            {
              _serialPort.Write("G");
            }

            if (data.Contains("H"))
            {
              _serialPort.Write("H");
            }


            // Advanced settings
            if (data.Contains("O"))
            {
              _serialPort.Write("O");
            }

            if (data.Contains("B"))
            {
              _serialPort.Write("B");
            }

            if (data.Contains("S"))
            {
              _serialPort.Write("S");
            }

            // valve and mode
            if (data.Contains("V"))
            {
              _serialPort.Write("V");
            }

            if (data.Contains("M"))
            {
              _serialPort.Write("M");
            }

            if (data.Contains("A"))
            {
              _serialPort.Write("A");
            }

            if (data.Contains("C"))
            {
              _serialPort.Write("C");
            }



            Thread.Sleep(100);
            _serialPort.Write(dataWithoutFirstLetter);

            Thread.Sleep(100);
            _serialPort.Write(bytestosend, 0, bytestosend.Length);
          }
        }
        catch (Exception ex)
        {
        }
      }
    }

    /// <summary>
    /// Update baud rate collection
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private void UpdateBaudRateCollection()
    {
      try
      {
        _serialPort = new SerialPort(_currentSerialSettings.PortName);
        _serialPort.Open();
        object p = _serialPort.BaseStream.GetType()
          .GetField("commProp", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(_serialPort.BaseStream);
        Int32 dwSettableBaud = (Int32)p.GetType().GetField("dwSettableBaud",
          BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(p);
        _currentSerialSettings.UpdateBaudRateCollection(dwSettableBaud);
      }
      catch (Exception ex)
      {
        IsLsproInitialized = false;
      }
      finally
      {
        _serialPort?.Close();
        _serialPort = null;
      }
    }

    /// <summary>
    /// Dispose the serial port
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public void Dispose()
    {
      Dispose(true);
    }


    /// <summary>
    /// Dispose the serial port
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="disposing"> Is the serial port disposing</param>
    protected virtual void Dispose(bool disposing)
    {
      if (_serialPort == null) return;

      if (disposing)
      {
        _serialPort.DataReceived -= new SerialDataReceivedEventHandler(_serialPort_DataReceived);
      }
      // Releasing serial port (and other unmanaged objects)
      if (_serialPort != null)
      {
        if (_serialPort.IsOpen)
          _serialPort.Close();

        _serialPort.Dispose();
        _serialPort = null;
      }
    }

    #endregion Methods

    /// <summary>
    /// send a packet
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="command">Command</param>
    /// <param name="data">Data</param>
    /// <param name="count">Count</param>
    public void SendPacket(CCMPCommand command, byte[] data, byte[] count = null)
    {
      if (_serialPort == null) return;
      try
      {
        byte[] commandByte = new byte[] { (byte)command };

        if (count != null)
          commandByte = LSPRODataBuilder.AppendDataAtTheEndOfAnArray(commandByte, count);

        //Adding the command
        byte[] appendCommandArray = LSPRODataBuilder.AppendDataAtTheEndOfAnArray(LSPROEnumeartion.CCMPAndversion, commandByte);

        //Adding the data
        byte[] appendDataArray = LSPRODataBuilder.AppendDataAtTheEndOfAnArray(appendCommandArray, data);

        // build the CRC32
        uint dwCRC32 = LSPROCRC32.GetValue(appendDataArray);
        byte[] crcArray = new byte[4];
        crcArray = BitConverter.GetBytes(dwCRC32);

        //Append the CRC
        byte[] appendCRCArray = LSPRODataBuilder.AppendDataAtTheEndOfAnArray(appendDataArray, crcArray);

        AddControlSequenceThenSend(appendCRCArray);
      }

      catch (Exception ex)
      {
        ex.ToString(); // TO do
      }

    }

    /// <summary>
    /// Build LSPro Message With Padding byte (0x00) following byte 0x0f
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="dataWithCRC"></param>
    /// <returns> List of bytes </returns>
    /// <id>SF-SDS-0136</id> 
    private static List<byte> BuildLSProMessageWithPaddingByte(byte[] dataWithCRC)
    {
      var dataWithCRCList = new List<byte>(dataWithCRC);

      for (int i = dataWithCRC.Length - 1; i >= 0 ; --i)
      {
        if (dataWithCRC[i] == (byte)CCMPControlBytes.CTRL_SEQ)
        {
          dataWithCRCList.Insert(i + 1, (byte)CCMPControlBytes.CTRL_SEQ_IGNORE);
        }
      }

      dataWithCRCList.InsertRange(0, new[] { (byte)CCMPControlBytes.CTRL_SEQ, (byte)CCMPControlBytes.CTRL_SEQ_PACKET_BEGIN });
      dataWithCRCList.AddRange(new[] { (byte)CCMPControlBytes.CTRL_SEQ, (byte)CCMPControlBytes.CTRL_SEQ_PACKET_END });

      return dataWithCRCList;
    }

    /// <summary>
    /// Add control sequence then send
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="dataWithCRC"> Data with CRC</param>
   
    private void AddControlSequenceThenSend(byte[] dataWithCRC)
    {
      if (_serialPort == null) return;
      try
      {
        var dataWithCRCList = BuildLSProMessageWithPaddingByte(dataWithCRC);

        _serialPort?.Write(dataWithCRCList.ToArray(), 0, dataWithCRCList.Count);
        timestampLastSend = DateTime.Now;
      }
      catch (Exception ex)
      {

      }

    }
  }
}
