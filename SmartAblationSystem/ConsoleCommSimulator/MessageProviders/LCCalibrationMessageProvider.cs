using Communication;
using ConsoleCommSimulator.Interfaces;
using ConsoleCommSimulator.Data;
using DataAccessLayer;
using Prism.Events;
using System;

namespace ConsoleCommSimulator.MessageProviders
{

  public class LCCalibrationMessageProvider : MessageProviderBase
  {
    private static readonly uint MessageId = 62;

    private readonly DataAccess _dataAccess;

    private double _currentCalibrationFactor = 2.0d;

    private double _currentCalibrationOffset; 

    public LCCalibrationMessageProvider(IEventAggregator eventAggregator, ISimulatorConfiguration configuration, DataAccess dataAccess)
      : base(eventAggregator, configuration)
    {
      _dataAccess = dataAccess;
      NodeId = 1; 
    }

    public override void Initialize()
    {
      base.Initialize();
      _currentCalibrationFactor = this._dataAccess.LoadCellCalibrationFactor();
      _currentCalibrationOffset = 1.5d;
    }

    public override void UpdateParameters(CanBusMessageParameters parameters)
    {
      base.UpdateParameters(parameters);
      var messageElements = SplitCanBusMessageId(parameters.MessageId);

      if (messageElements.Item2 == MessageId)
      {
        var data = parameters.Data; 
        if (data != null && data.Length > 0 && (data[0] != 0x0 || data[1] != 0x0))
        {
          this._currentCalibrationFactor = ConvertDecimalDataWithFactor(data, 2, 10000);
        }

        PublishLCCalibrationMessage(this._currentCalibrationFactor, this._currentCalibrationOffset);
      }
    }

    public void PublishLCCalibrationMessage(double factor, double offset)
    {
      var data = BuildLCCalibrationMessageData(factor, offset);

      var message = new CanBusMessage()
                        {

                          Id = CanBusId.CanBus1,
                          CanBusEventArgs = new CanBusEventArgs()
                                              { Cob = 0, Falgs = 4, Id = CreateMessageId(CurrentStateId, MessageId), Length = 8, Data = data}
                        };

      PublishCanBusMessage(message);
    }

    private static double ConvertDecimalDataWithFactor(byte[] data, int index, double factor)
    {
      return (((data[index] * 256 + data[index + 1]))) / factor;
    }

    private static byte[] BuildLCCalibrationMessageData(double factor, double offset)
    {
      var data = new byte[8];
      Array.Clear(data, 0, 8);
      uint componentId = 0x02;  // CMCU_Load_Cell
      data[0] = (byte)(componentId >> 8);
      data[1] = (byte)(componentId);

      var calibrationFactor = (int)(factor * 10000); 
      data[2] = (byte)(calibrationFactor >> 8);
      data[3] = (byte)(calibrationFactor);

      var calibrationOffset = (int)(offset * 10);
      data[4] = (byte)(calibrationOffset >> 8);
      data[5] = (byte)(calibrationOffset);
        
      return data;
    }
  }
}
