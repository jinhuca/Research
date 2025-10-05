using Communication;
using ConsoleCommSimulator.Configuration;
using ConsoleCommSimulator.Data;
using ConsoleCommSimulator.Interfaces;
using Prism.Events;
using System.Collections.Generic;
using Log = LogSystem.LogService;
using System;
using MicroLibrary;

namespace ConsoleCommSimulator.MessageProviders
{
  // blood pressure
  public class BloodPressureMessageProvider : MessageProviderBase
  {
    private static string BLOOD_SENSOR_NODE = "BloodPressureConfig";
    private int _index = 0;
    private static uint _messageId = 7;
    private readonly double[] _data = new double[]
    {
      // 0, -0.01, -0.03, 0.03, -0.06, -0.08, -0.08, -0.04, -0.12, -0.14, -0.16, -0.11, -0.19, -0.21, -0.22, -0.17, -0.25, -0.26, -0.28, -0.24, 
      // -0.31, -0.32, -0.34, -0.29, -0.37, -0.38, -0.39, -0.35, -0.42, -0.43, -0.44, -0.4, -0.47, -0.48, -0.49, -0.46, -0.51, -0.52, -0.53, -0.5, 
      // -0.55, -0.56, -0.56, -0.54, -0.59, -0.59, -0.6, -0.58, -0.61, -0.62, -0.62, -0.6, -0.63, -0.64, -0.64, -0.63, -0.64, -0.64, -0.65, -0.64, 
      // -0.65, -0.65, -0.65, -0.65, -0.65, -0.65, -0.64, -0.65, -0.64, -0.63, -0.63, -0.64, -0.62, -0.62, -0.62, -0.63, -0.61, -0.6, -0.6, -0.62, 
      // -0.58, -0.57, -0.57, -0.59, -0.54, -0.53, -0.52, -0.56, -0.51, -0.5, -0.49, -0.52, -0.46, -0.44, -0.43, -0.47, -0.41, -0.39, -0.38, -0.42, 
      // -0.35, -0.34, -0.33, -0.37, -0.3, -0.29, -0.27, -0.31, -0.24, -0.22, -0.21, -0.26, -0.18, -0.17, -0.15, -0.19, -0.11, -0.09, -0.08, -0.13, 
      // -0.04, -0.03, -0.01, -0.06, 0.02, 0.04, 0.06, 0.01, 0.09, 0.11, 0.12, 0.07, 0.16, 0.17, 0.19, 0.14, 0.22, 0.24, 0.25, 0.2, 0.28, 0.3, 0.31, 
      // 0.27, 0.34, 0.35, 0.37, 0.31, 0.4, 0.41, 0.41, 0.38, 0.45, 0.45, 0.47, 0.43, 0.48, 0.5, 0.5, 0.48, 0.52, 0.54, 0.54, 0.52, 0.57, 0.57, 0.58, 
      // 0.55, 0.59, 0.61, 0.61, 0.58, 0.62, 0.63, 0.63, 0.62, 0.63, 0.64, 0.64, 0.62, 0.64, 0.65, 0.64, 0.64, 0.65, 0.65, 0.65, 0.65, 0.65, 0.65, 0.64, 
      // 0.65, 0.64, 0.63, 0.63, 0.64, 0.62, 0.61, 0.61, 0.63, 0.6, 0.59, 0.59, 0.61, 0.58, 0.56, 0.55, 0.58, 0.55, 0.54, 0.53, 0.55, 0.5, 0.5, 0.49, 
      // 0.52, 0.47, 0.45, 0.44, 0.48, 0.42, 0.41, 0.39, 0.43, 0.37, 0.35, 0.34, 0.38, 0.31, 0.29, 0.28, 0.32, 0.24, 0.23, 0.22, 0.26, 0.18, 0.17, 0.15, 
      // 0.2, 0.12, 0.1, 0.09, 0.14, 0.06, 0.04, 0.02, 0.07, 0.03, 0.01  

      // 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34,
      // 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67,
      // 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 89, 88, 87, 86, 85, 84, 83, 82, 81, 80,
      // 79, 78, 77, 76, 75, 74, 73, 72, 71, 70, 69, 68, 67, 66, 65, 64, 63, 62, 61, 60, 59, 58, 57, 56, 55, 54, 53, 52, 51, 50, 49, 48, 47,
      // 46, 45, 44, 43, 42, 41, 40, 39, 38, 37, 36, 35, 34, 33, 32, 31, 30, 29, 28, 27, 26, 25, 24, 23, 22, 21, 20, 19, 18, 17, 16, 15, 14,
      // 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1
      
      31, 31.1, 31.2, 31.5, 32, 32.5, 33, 33.5, 34, 35, 36, 37, 39, 41, 42, 44, 45, 47, 50, 52, 55, 57, 61, 65, 70, 72, 73, 73.5, 74, 74.4,
      74.7, 75, 75, 75, 74, 73, 72, 69, 65, 62, 57, 52, 47, 42, 37, 32, 30, 27, 24, 19, 16, 15, 14, 13, 12.5, 12.2, 12.1, 12.1, 12.5, 12.8, 13, 14,
      15, 16, 17, 18, 19, 20, 20.4, 20.7, 20.9, 21, 21.2, 21.5, 22, 23, 24, 25, 26, 27, 28, 28.5, 29, 29.4, 29.8, 30.2, 30.5, 30.7, 30.9, 31,
      31, 31, 31.2, 31.4, 31.7, 32, 33, 34, 35, 36, 37, 38, 38.3, 38.5, 38.5, 38.3, 38, 37, 36, 35, 34, 33, 32.5, 32, 31.8, 31.6, 31.5,
      31.4, 31.3, 31.2, 31.1, 31, 31, 31, 31, 31, 31, 31, 31, 31, 31, 31, 31
    };


    private readonly MicroTimer _messageUpdateTimer;
    private static CanBusMessageDefinition.MessageStateId DEFAULT_STATE = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE;
    private IDictionary<string, StateToICBValue> _statesDictionary;
    private BloodPressureMessageConfig _BloodSensorMessageConfig;
    private readonly int _dataLength;

    public BloodPressureMessageProvider(IEventAggregator eventAggregator, ISimulatorConfiguration configuration) 
      : base(eventAggregator, configuration)
    {
      NodeId = 3;
      _messageUpdateTimer = new MicroTimer(32_000);
      _messageUpdateTimer.MicroTimerElapsed += (_, __) => PublishMessage();
      _dataLength = _data.Length;
    }

    public override void Initialize()
    {
      base.Initialize();
      _BloodSensorMessageConfig = new BloodPressureMessageConfig();
      var loadConfig = _BloodSensorMessageConfig.Parse(GetConfigurationNode(BLOOD_SENSOR_NODE));
      if (loadConfig)
      {

        _statesDictionary = _BloodSensorMessageConfig.StateToBloodPressureMap;
        
        _messageUpdateTimer.Start();
      }
      else
      {
        Log.LogInfo("Parsing configuration failed");
      }

    }

    protected override void DisposeMessageProvider()
    {
      // Stop Timer
      _messageUpdateTimer?.Stop();
    }

    private double GetNextData()
    {
      if (_index >= _dataLength - 1) _index = 0;
      return (_data[_index++]);
    }

    private (byte, byte) GetDataBytes(double value)
    {
      var intData = (short)(value * 100);
      var bytes = BitConverter.GetBytes(intData);
      return (bytes[1], bytes[0]);
    }

    private void PublishMessage()
    {
      var d1 = GetDataBytes(GetNextData());
      var d2 = GetDataBytes(GetNextData());
      var d3 = GetDataBytes(GetNextData());
      var d4 = GetDataBytes(GetNextData());

      var message = new CanBusMessage()
      {

        Id = CanBusId.CanBus2,
        CanBusEventArgs = new CanBusEventArgs()
          { 
            Cob = 0, 
            Falgs = 4, 
            Id = CreateMessageId(CurrentStateId, _messageId), Length = 8, 
            Data = new byte[] { d1.Item1, d1.Item2, d2.Item1, d2.Item2, d3.Item1, d3.Item2, d4.Item1, d4.Item2 }
          }
      };

      PublishCanBusMessage(message);

    }
  }
}
