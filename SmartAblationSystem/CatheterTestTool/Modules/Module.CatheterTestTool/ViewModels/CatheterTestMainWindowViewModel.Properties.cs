using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Module.CatheterTestTool.Data;
using Module.CatheterTestTool.Models;
using Module.CatheterTestTool.Services;
using Module.SystemParameters.Interfaces;

using static Communication.CanBusMessageDefinition;

namespace Module.CatheterTestTool.ViewModels
{
  public partial class CatheterTestMainWindowViewModel
  {
    private TestStatus _testStatus = new TestStatus { CountdownTimer = CatheterTestConstants.ABLATION_TIME };
    private bool _isWaterTemperatureInRange;

    public bool IsCatheterCableConnected => _machineModel.IsCatheterCableConnected && IsCMCUReady && IsPMCUReady;

    public bool IsVacuumOn => !_machineModel.IsVacuumDisconnected;

    public string SystemStateString => _systemStateToStringDict.ContainsKey(_machineModel.SystemState)
        ? _systemStateToStringDict[_machineModel.SystemState]
        : _systemStateToStringDict[MessageStateId.CAN_ID_STATE_UNKNOWN];

    public ISensorParameters SensorParameters { get; }

    public string Title => CatheterTestConstants.CATHETER_TEST_TOOL_TITLE;

    public TesterInfoData TesterInfo { get; }
    public string TesterName => $"{TesterInfo.FirstName} {TesterInfo.LastName}";

    public CatheterInfoData CatheterInfo { get; } = new CatheterInfoData();

    public bool IsCatheterSNAvailable => CatheterInfo != null && CatheterInfo.SerialNumber > 0;

    public TestStatus TestStatusProgress
    {
      get => _testStatus;
      set
      {
        _testStatus = value;
        RaisePropertyChanged(nameof(TestStatusProgress));
      }
    }

    public bool IsWaterTemperatureInRange
    {
      get => _isWaterTemperatureInRange;
      set => SetProperty(ref _isWaterTemperatureInRange, value);
    }

    private TestDataValidationResults _testDataValidationResults;

    public TestDataValidationResults TestDataValidationResults
    {
      get => _testDataValidationResults;
      set => SetProperty(ref _testDataValidationResults, value);
    }

    private bool _isTestStarted;

    public bool IsTestStarted
    {
      get => _isTestStarted;
      set
      {
        SetProperty(ref _isTestStarted, value);
        RaisePropertyChanged(nameof(CanStartTest));
        RaisePropertyChanged(nameof(CanExportToUSB));
      }
    }

    public bool CanStartTest => !_isTestStarted && IsCatheterCableConnected;

    public bool IsCMCUReady { get; private set; }

    public bool IsPMCUReady { get; private set; }

    public IList<DriveInfo> USBDriveList { get; set; }

    private bool _usbDriveConnected;

    public bool USBDriveConnected
    {
      get => _usbDriveConnected;
      set
      {
        SetProperty(ref _usbDriveConnected, value); 
        RaisePropertyChanged(nameof(CanExportToUSB));
      }
    }

    public bool CanExportToUSB => CanStartTest && USBDriveConnected;

    private bool _resetTemperatureDisplay;
    public bool ResetTemperatureDisplay
    {
      get => _resetTemperatureDisplay;
      set => SetProperty(ref _resetTemperatureDisplay, value);
    }

    private string _testSummaryNotes;
    public string TestSummaryNotes
    {
      get => _testSummaryNotes; 
      set => SetProperty(ref _testSummaryNotes, value);
    }

    public string SoftwareVersion => $"V{Assembly.GetEntryAssembly()?.GetName().Version}";

    #region CMCU/PMCU Status Code (Temporary for Debugging)
    public long CmcuStatusCode
    {
      get => _machineModel.CMCUSystemStatusError;
    }

    public long PmcuStatusCode
    {
      get => _machineModel.PMCUSystemStatusErrorCode;
    }

    #endregion

    private void ResetTestProgress()
    {
      TestStatusProgress.CurrentTestStep = 0;
      TestStatusProgress.TestProgressInPercentage = 0;
      TestStatusProgress.Step1Progress = 0;
      TestStatusProgress.Step2Progress = 0;
      TestStatusProgress.Step3Progress = 0;
      TestStatusProgress.Description = "";
      TestStatusProgress.IsTestCompleted = false;
      TestStatusProgress.CountdownTimer = CatheterTestConstants.ABLATION_TIME;
    }
  }
}
