
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Module.CatheterTestTool.Models;
using Module.CatheterTestTool.Services;
using Module.CatheterTestTool.Views;
using Module.Infrastructure.AppLog;
using Module.Infrastructure.Controls;
using Prism.Commands;
using Prism.Services.Dialogs;
using static Prism.Services.Dialogs.ButtonResult;
using static Module.CatheterTestTool.Models.CatheterTestConstants;
using Dialog = Prism.Services.Dialogs.Dialog;
using static Module.Infrastructure.Constants.Strings;

namespace Module.CatheterTestTool.ViewModels
{
  public partial class CatheterTestMainWindowViewModel
  {
    private ICommand _startTestCommand;
    private ICommand _stopTestCommand;
    private ICommand _homeCommand;
    private ICommand _exportToUsbCommand;

    public ICommand StartTestCommand
    {
      get => _startTestCommand = _startTestCommand ?? new DelegateCommand(StartTest).ObservesCanExecute(() => CanStartTest);
    }

    public ICommand StopTestCommand
    {
      get => _stopTestCommand = _stopTestCommand ?? new DelegateCommand(StopTest).ObservesCanExecute(() => IsTestStarted);
    }

    public ICommand HomeCommand
    {
      get => _homeCommand = _homeCommand ?? new DelegateCommand(ExecuteHomeCommand);
    }

    public ICommand ExportToUSBCommand
    {
      get => _exportToUsbCommand = _exportToUsbCommand ?? new DelegateCommand(ExecuteExportToUsbCommand).ObservesCanExecute(() => CanExportToUSB);
    }

    private void StopTest()
    {
      _catheterTestService?.CancelTest();
    }

    private void RequestToClearTemperatureGraph()
    {
      ResetTemperatureDisplay = false;
      Task.Delay(10).Wait();
      ResetTemperatureDisplay = true;
      Task.Delay(20).ContinueWith((_) => ResetTemperatureDisplay = false);
    }

    private void StartTest()
    {
      IsTestStarted = true;

      ResetTestProgress();
      TestSummaryNotes = String.Empty;
      TestDataValidationResults = null;

      _testStatusSubscriptionDisposable.Disposable = _catheterVisualService?.StartTest()
        .Subscribe(ProcessCatheterVisualCheck,
                    VisualTestOnError,
                    VisualTestCompleted);
    }

    private void ProcessCatheterVisualCheck(TestStatus status)
    {
      TestStatusProgress = status;
      if (!status.NeedUserInput) return;

      // Display Message Dialog for Pass/Fail when receives NeedUSerInput == true
      var parameters = new DialogParameters()
        {
          { POPUP_DIALOG_TITLE_KEY, CATHETER_VISUAL_CHECK_TITLE},
          { POPUP_DIALOG_MESSAGE_KEY, CATHETER_VISUAL_CHECK_MESSAGE },
          { POPUP_DIALOG_ISPASSFAIL_DIALOG_KEY, true},
          { POPUP_DIALOG_OKBUTTON_TEXT_KEY, PassText},
          { POPUP_DIALOG_CANCELBUTTON_TEXT_KEY, FailText}
        };

      Application.Current.Dispatcher.Invoke(() =>
        _dialogService.ShowDialog(nameof(CatheterTestPopupMessageView),
          parameters,
          (result) => _catheterVisualService?.CompleteTest(result.Result == OK)
          ));
    }

    private void VisualTestOnError(Exception exception)
    {
      TestOnError(exception);
    }

    private void VisualTestCompleted()
    {
      // Verify if it is Pass or Fail. Fail ---> End of test  
      // Pass ---> Show Message that guides user to put the catheter into water tank
      // and press continue or cancel  
      if (TestStatusProgress?.IsTestCompleted ?? false)
      {
        var parameters = new DialogParameters()
        {
          { POPUP_DIALOG_TITLE_KEY, CATHETER_TEST_TOOL_TITLE },
          { POPUP_DIALOG_MESSAGE_KEY,  CONTINUE_CATHETER_TEST_MESSAGE },
          { POPUP_DIALOG_ISPASSFAIL_DIALOG_KEY, false },
          { POPUP_DIALOG_OKBUTTON_TEXT_KEY, POPUP_DIALOG_CONTINUE_BUTTON_TEXT },
          { POPUP_DIALOG_CANCELBUTTON_TEXT_KEY, POPUP_DIALOG_CANCEL_BUTTON_TEXT }
        };

        Application.Current.Dispatcher.Invoke(() =>
          _dialogService.ShowDialog(nameof(CatheterTestPopupMessageView),
            parameters,
            (result) => HandleCatheterAblationTestRequest(result.Result == OK)
          ));
      }
      else
      {
        TestCompleted("Catheter Visual Check failed.");
      }
    }

    private void HandleCatheterAblationTestRequest(bool isContinue)
    {
      if (!isContinue) 
      {
        ResetTestProgress();
        TestCompleted(CATHETER_TEST_SESSION_STOPPED);
        return;
      }
      // waiting for water temperature reaching desired value and start Ablation test  
      FieldServiceTrace.Log("Start Catheter Ablation Test.");
      ResetTestProgress();
      RequestToClearTemperatureGraph();
      _testStatusSubscriptionDisposable.Disposable = _catheterTestService?.StartTest(TesterName, CatheterInfo)
          .Subscribe(status => TestStatusProgress = status,
              TestOnError,
              () =>
              {
                var message = TestStatusProgress.IsTestCompleted 
                  ? CATHETER_TEST_SESSION_COMPLETED
                  : CATHETER_TEST_SESSION_STOPPED;
                TestCompleted(message);
              });
    }

    private void TestOnError(Exception exception)
    {
      var errorMessage = $"The current test was stopped with error :\n {exception.Message}";
      ShowMessageDialog(errorMessage, MessageDialogType.Error);

      IsTestStarted = false;
      TestSummaryNotes = errorMessage; 
    }

    private void TestCompleted(string completedMessage)
    {
      // The test is completed, not stopped by user  
      if (TestStatusProgress.IsTestCompleted)
      {
        var resultData = _catheterTestService.GetTestResultData(); 
        var results = resultData
            .Results
            .ToDictionary(r => r.Sensor, r => r);

        TestDataValidationResults = new TestDataValidationResults()
        {
          TC1 = results.ContainsKey(SensorNameTC1) ? results[SensorNameTC1] : null,
          IBP = results.ContainsKey(SensorNameIBP) ? results[SensorNameIBP] : null,
          OBP = results.ContainsKey(SensorNameOBP) ? results[SensorNameOBP] : null,
          PT2 = results.ContainsKey(SensorNamePT2) ? results[SensorNamePT2] : null,
          FM1 = results.ContainsKey(SensorNameFM1) ? results[SensorNameFM1] : null,
          PT3 = results.ContainsKey(SensorNamePT3) ? results[SensorNamePT3] : null,
          PT4 = results.ContainsKey(SensorNamePT4) ? results[SensorNamePT4] : null
        };

        var areAllTestsPassed = resultData.Results.All(r => r.Result == TestResult.PASS);
        var parameters = new DialogParameters() { { CatheterTestConstants.TEST_RESULT_KEY, areAllTestsPassed } }; 
        Application.Current.Dispatcher.Invoke(() => _dialogService.ShowDialog(nameof(CatheterTestResultDialog), parameters, null));
      }
      else
      {
        ShowMessageDialog(completedMessage);
      }

      IsTestStarted = false;
      TestSummaryNotes = completedMessage; 
    }

    private void ShowMessageDialog(string message, MessageDialogType dialogType = MessageDialogType.Information)
    {
      var parameters = new DialogParameters()
            {
                { DialogTitleKey, CATHETER_TEST_TOOL_TITLE},
                { DialogMessageKey, message },
                {MessageDialogTypeKey, dialogType}
            };

      Application.Current.Dispatcher.Invoke(() => _dialogService.ShowDialog(nameof(MessageDialog), parameters, null));
    }

    private void ExecuteHomeCommand()
    {
      var parameters = new DialogParameters { { DialogTitleKey, TurnOffTitleValue }, { DialogMessageKey, TurnOffMessageValue } };
      _dialogService.ShowDialog(nameof(Dialog), parameters, TurnOffDialogCallback);
    }

    private void ExecuteExportToUsbCommand()
    {
      var parameters = new DialogParameters
          {
            { CatheterTestConstants.USB_DRIVE_NAME_PARAM, USBDriveConnected?USBDriveList[0].Name:string.Empty}
          };

      _dialogService.ShowDialog(nameof(TestResultFileSelector), parameters, null);
    }

    private async void TurnOffDialogCallback(IDialogResult dialogResult)
    {
      switch (dialogResult.Result)
      {
        case Yes:
          var terminationTask =  TerminateCatheterTestTool();
          await InvokeSmartFreeze();
          await terminationTask;
          Environment.Exit(0);
          break;
        case No:
        case Abort:
        case Cancel:
        case Ignore:
        case None:
        case OK:
        case Retry:
          return;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    private async Task TerminateCatheterTestTool()
    {
      await _machineModel.Terminate();
    }

    private async Task InvokeSmartFreeze()
    {
      var appLocation = Properties.Settings.Default.SmartFreezeAppPath;
      var appName = Properties.Settings.Default.SmartFreezeAppFileName; 

      var SmartFreezeApp = Path.Combine(appLocation, appName);

      if (!File.Exists(SmartFreezeApp))
      {
        return;
      }

      try
      {
        using (var smProcess = new Process())
        {
          smProcess.StartInfo.FileName = SmartFreezeApp;
          smProcess.StartInfo.WorkingDirectory = Path.GetDirectoryName(SmartFreezeApp) ?? string.Empty;
          smProcess.StartInfo.CreateNoWindow = false;
          await Task.Run(() => smProcess.Start());
        }
      }
      catch (Exception ex)
      {
        FieldServiceTrace.LogException(ex);
      }
    }
  }
}