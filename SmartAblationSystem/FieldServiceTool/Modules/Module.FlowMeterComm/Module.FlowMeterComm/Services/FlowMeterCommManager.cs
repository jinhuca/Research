using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Module.FlowMeterComm.Models;
using Module.FlowMeterComm.Utilities;
using Module.Infrastructure.AppLog;

namespace FlowMeterComm
{
  public class FlowMeterCommManager : IFlowMeterCommManager
  {
    private static readonly int _minReadingIntervalInMilliSec = 50;
    private static readonly int _readingRequestTiemoutInMilliSec = 1000; 
    private static readonly byte _preembleByteValue = 0xff;
    private static readonly byte[] _preemblesBytes = Enumerable.Repeat(_preembleByteValue, 5).ToArray();
    private static readonly byte[] _broadcastAddress = { 0x80, 0x00, 0x00, 0x00, 0x00 };
    private static readonly byte _shortAddressFrameDelimiterMaster = 0x02;
    private static readonly byte _longAddressFrameDelimiterMaster = 0x82;
    private static readonly byte _shortAddressFrameDelimiterSlave = 0x06;
    private static readonly byte _longAddressFrameDelimiterSlave = 0x86;

    private static readonly int _defaultBaudRate = 19200;
    private static readonly Parity _defaultParity = Parity.Odd;
    private static readonly StopBits _defaultStopBits = StopBits.One;
    private static readonly int _defaultDataBits = 8; 

    private readonly AutoResetEvent _requestHasBeenProcessed = new AutoResetEvent(false);

    private readonly ISubject<string> _commMessageObservable = new BehaviorSubject<string>("");
    private readonly ISubject<bool> _messageReceivedObservable = new Subject<bool>();

    private readonly SerialDisposable _readingFlowRateSubscription = new SerialDisposable();
    private IDisposable _messageReceivedTimeout;
    private SerialPort _serialPort;
    private string _portName = "COM5";

    private byte[] _deviceLongAddress = _broadcastAddress;
    private byte[] _readingPackgeBytes; 

    public FlowMeterCommManager(IFlowMeterParameters parameters)
    {
      FlowMeterParameters = parameters;
    }

    #region properties

    public event EventHandler<SerialComErrorEventArgs> FlowMeterCommErrorEvent;  

    public IFlowMeterParameters FlowMeterParameters { get; }

    public IObservable<string> CommMessageObservable => _commMessageObservable;

    #endregion properties

    public bool InitCommunication(string comPortName)
    {
      _portName = comPortName;
      OpenPort();
      _requestHasBeenProcessed.Reset();
      return true;
    }

    public string[] GetComNames()
    {
      return SerialPort.GetPortNames();
    }

    public bool ConnectToDevice(string serialNum = "")
    {
      int retryCount = 3;
      bool connected; 
      do
      {
        if (string.IsNullOrEmpty(serialNum))
          RequestToQueryDeviceId();
        else
          RequestToQueryDeviceIdBySerialNum(serialNum);

        connected = _requestHasBeenProcessed.WaitOne(_readingRequestTiemoutInMilliSec); 
      } while (!connected && --retryCount > 0);

      return connected; 
    }   

    public void StartReading(int interval)
    {
      // Make sure the reading interval is larger than minimum interval (50ms) 
      var appliedInterval = Math.Max(_minReadingIntervalInMilliSec, interval);
      _readingFlowRateSubscription.Disposable = Observable
        .Interval(TimeSpan.FromMilliseconds(appliedInterval))
        .Subscribe(_ => RequestToReadFlowRate());
        // .Subscribe(_ => RequestToReadAllVariables());

      _messageReceivedTimeout = _messageReceivedObservable
        .Timeout(TimeSpan.FromMilliseconds(_readingRequestTiemoutInMilliSec * 3 + appliedInterval))
        .Subscribe(
          _ => { },
          _ =>
          {
            StopReading();
            FlowMeterCommErrorEvent?.Invoke(this, new SerialComErrorEventArgs()
            {
              ErrorType = SerialCommErrors.DeviceNoResponse, 
              ErrorMessage = FlowMeterCommConstants.DEVICE_NO_RESPONSE_ERROR_MSG
            });
          }
        );
    }

    public void StopReading()
    {
      _readingFlowRateSubscription.Disposable?.Dispose();
      _messageReceivedTimeout?.Dispose();
      _messageReceivedTimeout = null;
    }

    public void Close()
    {
      StopReading();
      // Closing serial port if it is open
      if (_serialPort != null && _serialPort.IsOpen)
      {
        _serialPort.DataReceived -= FlowDataReceived;
        _serialPort.Close();
        _serialPort = null;
      } 
    }

    public void RequestToQueryDeviceIdBySerialNum(string serialNum)
    {
      var tagName = new string(serialNum.GetLast(8).ToArray());
      byte[] commandData = GeneratePackedASCIIMessage(tagName);
      var messageToSend = BuildSendMessage(FlowMeterCommand.ReadUniqueIdentifier, _broadcastAddress, commandData);
      SendRequestMessage(messageToSend);
    }

    public void RequestToQueryDeviceId()
    {
      var messageToSend = BuildSendMessage(FlowMeterCommand.ReadUniqueIdentifier, _broadcastAddress, null);
      SendRequestMessage(messageToSend);
    }

    public void RequestToReadAllVariables()
    {
      var messageToSend = BuildSendMessage(FlowMeterCommand.ReadAllVariables, _deviceLongAddress, null);
      SendRequestMessage(messageToSend);
    }

    public void RequestToReadFlowRate()
    {
      var messageToSend = BuildSendMessage(FlowMeterCommand.ReadFlowRate, _deviceLongAddress, null);
      SendRequestMessage(messageToSend);
    }

    public float ReadFlowRateSync()
    {
      RequestToReadFlowRate();
      var dataRead = _requestHasBeenProcessed.WaitOne(_readingRequestTiemoutInMilliSec);
      return dataRead ? FlowMeterParameters.FlowRate : float.NaN;  
    }

    private void OpenPort()
    {
      // Closing serial port if it is open
      if (_serialPort != null && _serialPort.IsOpen)
        Close();

      // Setting serial port settings
      _serialPort = new SerialPort(
        _portName,
        _defaultBaudRate,
        _defaultParity,
        _defaultDataBits,
        _defaultStopBits);

      _serialPort.DtrEnable = true;
      _serialPort.RtsEnable = true;

      // Subscribe to event and open serial port for data
      _serialPort.DataReceived += FlowDataReceived;
      _serialPort.Open();
    }

    private static byte[] GeneratePackedASCIIMessage(string tagName)
    {
      // Construction of Packed - ASCII:
      //  a.Remove bit #7 and bit #6 from each ASCII character.
      //  b.Pack four 6 - bit ASCII bytes into three bytes.
      // Reconstruction of ASCII characters:
      //  a.Unpack the four 6 - bit ASCII characters into four bytes.
      //  b.Place the complement of bit #5 of each unpacked 6-bit ASCII character into bit #6.
      // c.Set bit #7 of each unpacked ASCII to zero.

      if (tagName.Length != 8) return Enumerable.Repeat((byte)0x00, 6).ToArray();

      var upperCasedTag = tagName.ToUpper();
      var tagInFilteredBytes = Encoding.ASCII.GetBytes(upperCasedTag).Select(b => b & 0x3f).ToArray();
      byte[] packedTagBuffer = new byte[6];
      packedTagBuffer[0] = (byte)(tagInFilteredBytes[0] << 2 | tagInFilteredBytes[1] >> 4);
      packedTagBuffer[1] = (byte)(tagInFilteredBytes[1] << 4 | tagInFilteredBytes[2] >> 2);
      packedTagBuffer[2] = (byte)(tagInFilteredBytes[2] << 6 | tagInFilteredBytes[3]);

      packedTagBuffer[3] = (byte)(tagInFilteredBytes[4] << 2 | tagInFilteredBytes[5] >> 4);
      packedTagBuffer[4] = (byte)(tagInFilteredBytes[5] << 4 | tagInFilteredBytes[6] >> 2);
      packedTagBuffer[5] = (byte)(tagInFilteredBytes[6] << 6 | tagInFilteredBytes[7]);

      return packedTagBuffer;
    }

    private void FlowDataReceived(object sender, SerialDataReceivedEventArgs args)
    {
      if (_serialPort == null || !_serialPort.IsOpen)
        return;

      int dataLength = _serialPort.BytesToRead;
      byte[] data = new byte[dataLength];
      int nbrDataRead = _serialPort.Read(data, 0, dataLength);
      if (nbrDataRead == 0)
        return;

      _messageReceivedObservable.OnNext(true);

      var receivedMessageStr = LogDataString(data, "Received data");
      _commMessageObservable.OnNext(receivedMessageStr);

      try
      {
	      ProcessMessage(data);
      }
      catch (Exception ex)
      {
        FieldServiceTrace.LogException(ex);
      }
    }

    private byte[] BuildSendMessage(FlowMeterCommand command, byte[] address, byte[] requestDataBytes)
    {
      // preembles + address Delimiter + address + command + dataCount + data bytes + checksum
      byte addressDelimiter =
        address.Length == 1 ? _shortAddressFrameDelimiterMaster : _longAddressFrameDelimiterMaster;
      byte dataCount = (byte)(requestDataBytes?.Length ?? 0);

      var buffer = _preemblesBytes
        .Concat(new[] { addressDelimiter })
        .Concat(address)
        .Concat(new[] { (byte)command })
        .Concat(new[] { dataCount });

      if (dataCount > 0)
      {
        buffer = buffer.Concat(requestDataBytes);
      }

      // Checksum is xor all bytes from start character (address delimiter)
      byte checkSum = CalculateChecksum(buffer); //

      return buffer
        .Append(checkSum)
        .ToArray();
    }

    private void SendRequestMessage(byte[] message)
    {
      if (_serialPort!=null && _serialPort.IsOpen)
      {
        var sendingMessage = LogDataString(message, "Sending data"); //$"Sending data : {{{string.Join(", ", message.Select(b => $"0x{b:x2}"))}}}";
        _commMessageObservable.OnNext(sendingMessage);

        _serialPort?.Write(message, 0, message.Length);
      }
    }

    private byte CalculateChecksum(IEnumerable<byte> data)
    {
      return data.SkipWhile(b => b == _preembleByteValue).Aggregate((f, s) => (byte)(f ^ s));
    }

    private bool ValidateChecksum(byte[] message)
    {
      return message.Aggregate((f, s) => (byte)(f ^ s)) == 0x00;
    }

    private void ProcessMessage(byte[] data)
    {
      // A response message could be split in multiple packages, we combine them together to process 
      _readingPackgeBytes = (_readingPackgeBytes != null && data[0] != _preembleByteValue)  
        ? _readingPackgeBytes.Concat(data).ToArray()
       : data;

      // Skip the 0xff
      var validMessageBuffer = _readingPackgeBytes.SkipWhile(b => b == _preembleByteValue).ToArray();

      if (validMessageBuffer.Length ==0 || !ValidateChecksum(validMessageBuffer))
      {
        var receivedMessage = LogDataString(data, "Received"); 
        // Trace.WriteLine($"Checksum is not valid. {receivedMessage}");
        return;
      }

      var processingMessage = LogDataString(_readingPackgeBytes, "Processing data"); 
      // Trace.WriteLine(processingMessage);

      _readingPackgeBytes = null;

      // If message data does not start with address bytes, skip it 
      if (validMessageBuffer[0] != _longAddressFrameDelimiterSlave &&
          validMessageBuffer[0] != _shortAddressFrameDelimiterSlave)
      {
        return;
      }

      var addressByteCount = (validMessageBuffer[0] == _longAddressFrameDelimiterSlave) ? 5 : 1;

      byte commandId = validMessageBuffer[addressByteCount + 1];
      // Valid message data start after CommandId byte 
      var messageData = validMessageBuffer.Skip(addressByteCount + 2).ToArray();

      // UpdateFlowRate and UpdateFlowRateAndTemp methods should not be async. The message order should be kept. 
      switch ((FlowMeterCommand)commandId)
      {
        case FlowMeterCommand.ReadFlowRate:
          UpdateFlowRate(messageData); 
          break;
        case FlowMeterCommand.ReadAllVariables:
          UpdateFlowRateAndTemp(messageData);
          break;
        case FlowMeterCommand.ReadUniqueIdentifier:
        case FlowMeterCommand.QueryDeviceId:
          UpdateDeviceAddress(messageData);
          break;
        default:
          break;
      }
    }

    private async void UpdateDeviceAddress(byte[] messageBytes)
    {
      await Task.Run(() =>
      {
        try
        {
          var deviceIdMessage = messageBytes.ToStructure<DeviceIdMessage>();
          _deviceLongAddress = new[]
              { (byte)(0x80 | deviceIdMessage.ManufacturerCode), deviceIdMessage.DeviceTypeCode }
            .Concat(deviceIdMessage.DeviceId)
            .ToArray();
          _requestHasBeenProcessed.Set();
        }
        catch (Exception ex)
        {
          Trace.WriteLine(ex);
        }
      });
    }

    private void UpdateFlowRate(byte[] messageBytes)
    {
      try
      {
        // struct FlowRateMessage:
        // 1 byte  messageSize; 2 bytes status; 1 byte unit
        // 4 bytes float value in big endian 
        var floatBytes = messageBytes.Skip(4).Take(4).Reverse().ToArray();
        FlowMeterParameters.FlowRate = BitConverter.ToSingle(floatBytes, 0);
        _requestHasBeenProcessed.Set(); 
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex);
      }
    }

    private void UpdateFlowRateAndTemp(byte[] messageBytes)
    {
      // struct AllVariablesMessage :
      // 1 byte messageSize; 2 bytes status; 4 bytes current voltage;
      // 1 byte flowUnit; 4 bytes flowRate (big endian);
      // 1 byte temperature unit; 4 bytes temperature (big endian)
      try
      {
        var floatBytesFlow = messageBytes.Skip(4 + 4).Take(4).Reverse().ToArray();
        FlowMeterParameters.FlowRate = BitConverter.ToSingle(floatBytesFlow, 0);

        var floatBytesTemp = messageBytes.Skip(8 + 4 + 1).Take(4).Reverse().ToArray();
        // FlowMeterParameters.Temperature = BitConverter.ToSingle(floatBytesTemp, 0);

        _requestHasBeenProcessed.Set(); 
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex);
      }
    }

    private string LogDataString(byte[] data, string customizedStr)
    {
      return
        $"{DateTime.Now.ToString("hh:mm:ss.fff")} {customizedStr} : {{{string.Join(", ", data.Select(b => $"0x{b:x2}"))}}}";
    }
  }
}