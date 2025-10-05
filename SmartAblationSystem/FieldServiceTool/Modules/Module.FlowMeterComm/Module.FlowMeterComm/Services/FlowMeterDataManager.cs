using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using FlowMeterComm;
using Module.FlowMeterComm.Models;
using Module.SystemParameters.Interfaces;

namespace Module.FlowMeterComm.Services
{
  public class FlowMeterDataManager : IFlowMeterDataManager
  {
    private static readonly string[] IgnoredComPorts = { "COM1", "COM2", "COM3", "COM4" };
    private static readonly int DefaultIntervalForReadingFlowRate = 100;
    private static readonly int DefaultSamplingTime = 100;
    private static readonly double AcceptedOffsetPercentage = 0.02d; 

    private readonly IFlowMeterCommManager _flowMeterCommManager;
    private readonly IFlowMeterParameters _flowMeterParameters;
    private readonly ISensorParameters _sensorParameters;
    
    private readonly ISubject<string> _commMessageSubject = new Subject<string>();

    private bool _isConnectedToDevice;
    private SerialDisposable _flowParamSubscriber = new SerialDisposable();

    private readonly List<FlowRateData> _flowRateDataCollection;
    private int _currentSamplingTime; 

    public FlowMeterDataManager(IFlowMeterCommManager flowMeterCommManager,
      IFlowMeterParameters flowMeterParameters,
      ISensorParameters sensorParameters)
    {
      _flowMeterCommManager = flowMeterCommManager;
      _flowMeterParameters = flowMeterParameters;
      _sensorParameters = sensorParameters;
      _flowRateDataCollection = new List<FlowRateData>();
    }

    public string[] AvailableComPorts
    {
      get
      {
        var allAvailablePortNames = _flowMeterCommManager.GetComNames();
        return allAvailablePortNames?.Except(IgnoredComPorts).ToArray();
      }
    }

    public IObservable<string> CommunicationMessageObserver => _commMessageSubject;

    public bool IsConnectionLost { get; private set; }

    public bool ConnectToFlowMeter(string portName = null)
    {
      var validPortNames = AvailableComPorts; 
      if (validPortNames == null || validPortNames.Length == 0)
        return false;

      try
      {
	      _flowMeterCommManager.InitCommunication(validPortNames[0]);
	      _isConnectedToDevice = _flowMeterCommManager.ConnectToDevice();
      }
      catch (Exception ex)
      {
	      _isConnectedToDevice = false;
        Trace.WriteLine(ex.Message);
      }

      if (_isConnectedToDevice)
      {
        IsConnectionLost = false;
        _flowMeterCommManager.FlowMeterCommErrorEvent -= HandleFlowMeterError;
        _flowMeterCommManager.FlowMeterCommErrorEvent += HandleFlowMeterError;
        _flowMeterCommManager.StartReading(DefaultIntervalForReadingFlowRate);
      }

      return _isConnectedToDevice;
    }

    public void CloseConnection()
    {
      _flowMeterCommManager.StopReading();
      _flowMeterCommManager.FlowMeterCommErrorEvent -= HandleFlowMeterError;
      _flowMeterCommManager.Close();
      _isConnectedToDevice = false;
    }

    public void StartCollectingData(int samplingTime)
    {
      if (!_isConnectedToDevice)
      {
        return;
      }

      _flowRateDataCollection.Clear();
      // Sampling time should not less than default one (100ms)
      _currentSamplingTime = Math.Max(samplingTime, DefaultSamplingTime); 
      _flowParamSubscriber.Disposable = Observable
        .Interval(TimeSpan.FromMilliseconds(_currentSamplingTime))
        .Subscribe(CollectFlowRateData);
    }

    public void StopCollectingData()
    {
      _flowMeterCommManager.StopReading();
      _flowParamSubscriber.Disposable?.Dispose();
    }

    public FlowMeterValidationResult ValidateFlowMeter()
    {
      return FlowMeterValidator.ValidateFlowMeterResult(_flowRateDataCollection, AcceptedOffsetPercentage, _currentSamplingTime); 
    }

    private void CollectFlowRateData(long index)
    {
      _flowRateDataCollection?.Add(new FlowRateData()
      {
        Timestamp = DateTime.Now,
        Index = index, 
        FM1 = _sensorParameters.FM1, 
        FMExt = Math.Round(_flowMeterParameters.FlowRate, 0)
      });
    }

    private void HandleFlowMeterError(object sender, SerialComErrorEventArgs args)
    {
      IsConnectionLost = true;
      _commMessageSubject.OnNext(args.ErrorMessage);
    }
  }
}
