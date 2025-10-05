using System;
using System.ComponentModel;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Communication;
using Module.CatheterTestTool.Data;
using Module.CatheterTestTool.Services;
using Module.Console.Interfaces;
using Module.Infrastructure.PubSubEvents;
using Module.SystemParameters.Interfaces;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace Module.CatheterTestTool.ViewModels
{
  public partial class CatheterTestMainWindowViewModel : BindableBase
  {
    public CatheterTestMainWindowViewModel(
        IContainerProvider containerProvider_,
        IDialogService dialogService_,
        IMachineModel machineModel,
        ISensorParameters sensorParameters,
        ICatheterVisualTestService catheterVisualService,
        ICatheterTestService catheterTestService_)
    {
      _containerProvider = containerProvider_;
      _dialogService = dialogService_;
      _machineModel = machineModel;
      SensorParameters = sensorParameters;
      _catheterVisualService = catheterVisualService;
      _catheterTestService = catheterTestService_;
      TesterInfo = new TesterInfoData();
      _eventAggregator = containerProvider_.Resolve<IEventAggregator>();
      _eventAggregator.GetEvent<TesterInfoEvent>().Subscribe(UpdateTesterInfoData);
      InitializeCatheterInfo();
      InitializeUSBDriveInfo();
      InitializeEventSubscriptions();

#if DEBUG
      //TODO::Remove this line before check in.
      mockSensorParameters();
#endif
    }

    private void InitializeEventSubscriptions()
    {
      Observable.FromEventPattern<PropertyChangedEventArgs>(_machineModel, "PropertyChanged")
        .ObserveOn(TaskPoolScheduler.Default)
        .Subscribe(e => HandleMachineModelPropertyChanged(e.EventArgs));

      // property CanStartTest relies on IsCatheterCableConnected and IsTestStarted  
      Observable.FromEventPattern<PropertyChangedEventArgs>(this, "PropertyChanged")
        .Where(e =>
          e.EventArgs.PropertyName == nameof(IsCatheterCableConnected)
          || e.EventArgs.PropertyName == nameof(IsTestStarted))
        .ObserveOn(TaskPoolScheduler.Default)
        .Subscribe(e => RaisePropertyChanged(nameof(CanStartTest)));

      // Update DAS Balloon SetPoints
      Observable.FromEventPattern<PropertyChangedEventArgs>(this, "PropertyChanged")
        .Where(e =>
          e.EventArgs.PropertyName == nameof(IsCatheterCableConnected))
        .ObserveOn(TaskPoolScheduler.Default)
        .Subscribe(e =>
        {
          if (IsCatheterCableConnected) 
            UpdateDasBalloonSettings();
        });
    }

    private void UpdateTesterInfoData((string, string, DateTime) obj)
    {
      TesterInfo.FirstName = obj.Item1;
      TesterInfo.LastName = obj.Item2;
      TesterInfo.LogonTime = obj.Item3;
      RaisePropertyChanged(nameof(TesterName));
    }

    private void InitializeCatheterInfo()
    {
      CatheterInfo.FirmwareVersion = _machineModel.CatheterFirmwareVersion;
      CatheterInfo.ID = _machineModel.CatheterID;
      CatheterInfo.Lot = _machineModel.CatheterLot;
      CatheterInfo.SerialNumber = _machineModel.CatheterSerialNumber;
    }

    private void InitializeUSBDriveInfo()
    {
      _usbDriveConnectionManager = new USBDriveConnectionManager.USBDriveConnectionManager(USBDriveConnectionEventArrived);
      USBDriveConnectionEventArrived(null, null);
    }

    //TODO:: For test only (TO BE Cleaned once done)
    #region test mock function
    private void mockSensorParameters()
    {
      _machineModel.CatheterSerialNumber = 13958;
      _machineModel.CatheterID = 2;
      _machineModel.CatheterLot = 101; 

      _machineModel.TC1Reading = 20.0d;
      _machineModel.FM1Reading = 10.2d;
      _machineModel.CP1Reading = _machineModel.CatheterID == 1 ? 2.5 : 7.5;
      _machineModel.CP2Reading = 12;
      _machineModel.PIDDutyCycle = 60;
      _machineModel.PatientPIDDutyCycle = 70;

      _machineModel.PT1Reading = 1.2;
      _machineModel.PT2Reading = 2.3;
      _machineModel.PT3Reading = 3.4;
      _machineModel.PT4Reading = 4.5;
      _machineModel.PT5Reading = 5.6;

      _machineModel.SystemState = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY;

      Task.Delay(10000).ContinueWith((_) =>
      {
        _machineModel.CMCUSystemStatusError = 0x0A000000;
        _machineModel.PMCUSystemStatusErrorCode = 0x01000000 | 0x08000000;
      });

      long cmcuNewStatus = 1; 
      double step = 1d;
      Observable.Interval(TimeSpan.FromSeconds(1))
        .Skip(10)
          .ObserveOn(TaskPoolScheduler.Default)
          .Subscribe(t =>
      {
        var state = _machineModel.SystemState;
        if (state == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_EXCEPTION)
        {
          _machineModel.SystemState = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_UNKNOWN;
        }
        else if (state == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_UNKNOWN)
        {
          _machineModel.SystemState = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE;
        }
        // else if (_machineModel.SystemState == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING &&
        //     SensorParameters.Temperature <= 20d)
        // {
        //
        // }
        else
        {
          _machineModel.SystemState += 256;
        }

        if (SensorParameters.FM1 > 20.0d)
        {
          step = -1d;
        }
        else if (SensorParameters.FM1 <= 0d)
        {
          step = 1d;
        }

        SensorParameters.FM1 += step;
        _machineModel.TC1Reading += step*2;

        // _machineModel.CMCUSystemStatusError |= _cmcuAllWarningFlag; 

        // if (_machineModel.CMCUSystemStatusError < 0xfffffff)
        // {
        //   _machineModel.CMCUSystemStatusError |= cmcuNewStatus;
        //   _machineModel.PMCUSystemStatusErrorCode |= cmcuNewStatus;
        //   cmcuNewStatus = cmcuNewStatus << 1;
        // }
        // else
        // {
        //   cmcuNewStatus = 1;
        //   _machineModel.CMCUSystemStatusError = 0;
        //   _machineModel.PMCUSystemStatusErrorCode = 0;
        // }

      });
    }

    #endregion
  }
}
