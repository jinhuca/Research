using System;
using System.ComponentModel;
using System.Management;
using Communication;
using Module.CatheterTestTool.PubSubEvents;
using Module.Infrastructure.AppLog;

namespace Module.CatheterTestTool.ViewModels
{
  public partial class CatheterTestMainWindowViewModel
  {
    private void HandleMachineModelPropertyChanged(PropertyChangedEventArgs args)
    {
      switch (args.PropertyName)
      {
        case nameof(_machineModel.IsCatheterCableConnected):
          RaisePropertyChanged(nameof(IsCatheterCableConnected));
          break;
        case nameof(_machineModel.IsVacuumDisconnected):
          RaisePropertyChanged(nameof(IsVacuumOn));
          break;
        case nameof(_machineModel.SystemState):
          RaisePropertyChanged(nameof(SystemStateString));
          break;
        case nameof(_machineModel.CatheterID):
          CatheterInfo.ID = _machineModel.CatheterID;
          RaisePropertyChanged(nameof(CatheterInfo));
          break;
        case nameof(_machineModel.CatheterSerialNumber):
          CatheterInfo.SerialNumber = _machineModel.CatheterSerialNumber;
          RaisePropertyChanged(nameof(CatheterInfo));
          RaisePropertyChanged(nameof(IsCatheterSNAvailable));
          break;
        case nameof(_machineModel.CatheterLot):
          CatheterInfo.Lot = _machineModel.CatheterLot;
          RaisePropertyChanged(nameof(CatheterInfo));
          break;
        case nameof(_machineModel.CatheterFirmwareVersion):
          CatheterInfo.FirmwareVersion = _machineModel.CatheterFirmwareVersion;
          RaisePropertyChanged(nameof(CatheterInfo));
          break;
        case nameof(_machineModel.CatheterExpirationDate):
          CatheterInfo.CatheterExpirationDate = _machineModel.CatheterExpirationDate;
          RaisePropertyChanged(nameof(CatheterInfo));
          break;
        case nameof(_machineModel.CatheterLastUseDate):
          CatheterInfo.LastUseDate = _machineModel.CatheterLastUseDate;
          RaisePropertyChanged(nameof(CatheterInfo));
          break;
        case nameof(_machineModel.CMCUSystemStatusError):
          UpdateCMCUStatus(_machineModel.CMCUSystemStatusError);
          RaisePropertyChanged(nameof(CmcuStatusCode));
          break;
        case nameof(_machineModel.PMCUSystemStatusErrorCode):
          UpdatePMCUStatus(_machineModel.PMCUSystemStatusErrorCode);
          RaisePropertyChanged(nameof(PmcuStatusCode));
          break;
        default:
          break;
      }
    }

    private void UpdateCMCUStatus(long cmcuSystemStatusCode)
    {
      FieldServiceTrace.Log($"CMCUSystemStatusCode is updated with code : {cmcuSystemStatusCode:X8}");

      IsCMCUReady = (cmcuSystemStatusCode & (Int64)CanBusMessageDefinition.CMCUStatusError.CMCUReady)
                    == (Int64)CanBusMessageDefinition.CMCUStatusError.CMCUReady;

      RaisePropertyChanged(nameof(IsCatheterCableConnected));
    }

    private void UpdatePMCUStatus(long pmcuSystemStatusCode)
    {
      FieldServiceTrace.Log($"CMCUSystemStatusCode is updated with code : {pmcuSystemStatusCode:X8}");

      _machineModel.IsCatheterCableConnected = (pmcuSystemStatusCode & (Int64)CanBusMessageDefinition.PMCUStatusError.CatheterCableConnected)
                                               == (Int64)CanBusMessageDefinition.PMCUStatusError.CatheterCableConnected;

      if (!_machineModel.IsCatheterCableConnected)
      {
        _machineModel.CatheterSerialNumber = 0;
        _machineModel.CatheterID = 0;
        _machineModel.CatheterLot = 0;
        _machineModel.TC1Reading = 0d;
      }

      IsPMCUReady = (pmcuSystemStatusCode & (Int64)CanBusMessageDefinition.PMCUStatusError.PMCUReady)
                                == (Int64)CanBusMessageDefinition.PMCUStatusError.PMCUReady;

      RaisePropertyChanged(nameof(IsCatheterCableConnected));
    }

    private void USBDriveConnectionEventArrived(object sender, EventArrivedEventArgs e)
    {
      try
      {
        USBDriveList = _usbDriveConnectionManager.GetUSBDriveList();
        USBDriveConnected = USBDriveList != null && USBDriveList.Count > 0;

        _eventAggregator.GetEvent<USBConnectionEvent>().Publish(USBDriveConnected ? USBDriveList[0].Name : string.Empty);
      }
      catch (Exception ex)
      {
        FieldServiceTrace.LogException(ex);
      }
    }

    private async void UpdateDasBalloonSettings()
    {
      // Das balloon is only available for Catheter Type 2
      if (_machineModel.CatheterID == 2)
      {
        await _machineModel.SendBalloonPressureSetPointAsync(true);
      }
    }
  }
}
