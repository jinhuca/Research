using Module.Console.Interfaces;
using Module.Infrastructure.PubSubEvents;
using Module.Infrastructure.TestInterfaces;
using Prism.Services.Dialogs;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System;
using Module.Infrastructure;
using Module.Infrastructure.Controls;
using Module.Infrastructure.TestResults.Interfaces;

using static Communication.CanBusMessageDefinition;
using static Module.Infrastructure.SessionStatus;
using static Module.Infrastructure.Constants.Strings;
using static Module.Infrastructure.Constants.UserCommand;
using static Module.TestProcess.Constants.TestProcessMessages;
using System.ComponentModel;
using static System.Reactive.Linq.Observable;
using Prism.Events;

namespace Module.TestProcess.Services
{
  public class CatheterVerificationService : ICatheterVerificationService
  {
    private readonly IMachineModel _machineModel;
    private readonly IEventAggregator _eventAggregator;
    private readonly IDialogService _dialogService;

    private readonly ISubject<bool> _catheterStateSubject;
    private readonly AutoResetEvent _catheterReadyEvent = new AutoResetEvent(false);

    public CatheterVerificationService(IMachineModel machineModel,
      IEventAggregator eventAggregator,
      IDialogService dialogService)
    {
      _machineModel = machineModel;
      _eventAggregator = eventAggregator;
      _dialogService = dialogService;

      _catheterStateSubject = new BehaviorSubject<bool>(IsCatheterReady);
      IsCMCUReady = (_machineModel.CMCUSystemStatusError & (long)CMCUStatusError.CMCUReady) ==
                    (long)CMCUStatusError.CMCUReady;
      IsPMCUReady = (_machineModel.PMCUSystemStatusErrorCode & (long)PMCUStatusError.PMCUReady) ==
                    (long)PMCUStatusError.PMCUReady;

      FromEventPattern<PropertyChangedEventArgs>(_machineModel, nameof(_machineModel.PropertyChanged))
        .Where(e => e.EventArgs.PropertyName == nameof(_machineModel.CMCUSystemStatusError)
                    || e.EventArgs.PropertyName == nameof(_machineModel.PMCUSystemStatusErrorCode)
                    || e.EventArgs.PropertyName == nameof(_machineModel.CatheterSerialNumber))
        .ObserveOn(TaskPoolScheduler.Default)
        .Subscribe(e => HandleCMCUPMCUSystemStatusError(e.EventArgs));
    }

    public bool IsCatheterReady
    {
	    get
	    {
#if DEBUG
		    return true;
#endif
        return IsCMCUReady && IsPMCUReady && _machineModel.CatheterSerialNumber != 0;
      }
    }
    
    protected bool IsCMCUReady { get; private set; }
    protected bool IsPMCUReady { get; private set; }

    protected bool IsCatheterIdValid
    {
	    get
	    {
#if DEBUG
		    return true;
#endif
        return (_machineModel.CatheterID & ~_machineModel.EngineeringCatheterSignature) == ServiceConstants.POLARxFITCatheterId;
      }
    }
	    

    public bool VerifyCatheterIsReadyAndValid(CancellationToken cancellationToken, ISessionModel sessionModel, ITestInfo info)
    {
      if (cancellationToken.IsCancellationRequested || sessionModel.Status == Stopping)
      {
        return false;
      }

      while (WaitAndVerifyCatheterReady(sessionModel, info))
      {
        if (VerifyCatheterType(sessionModel, info) || info.Status == TestStatus.Aborted)
        {
          break;
        }

        Task.Delay(TimeSpan.FromSeconds(3)).Wait(cancellationToken);
      }

      return info.Status != TestStatus.Aborted;
    }

    private bool WaitAndVerifyCatheterReady(ISessionModel sessionModel, ITestInfo info)
    {
      _catheterReadyEvent.Reset();
      using (_catheterStateSubject.ObserveOn(TaskPoolScheduler.Default)
               .Subscribe(isReady =>
               {
                 if (isReady)
                   _catheterReadyEvent.Set();
               }))
      {
        while (!IsCatheterReady)
        {
          if (!_machineModel.IsCatheterCableConnected)
            InvokeCatheterConnectionDialog();
          else
          {
            InvokeRetryStopDialog(info);
            if (info.Status == TestStatus.Aborted)
            {
              break;
            }
          }

          _catheterReadyEvent.WaitOne(TimeSpan.FromSeconds(10));

          if (sessionModel.Status == Stopped)
          {
            info.Status = TestStatus.Aborted;
            return false;
          }
        }
      }

      return IsCatheterReady;
    }

    private bool VerifyCatheterType(ISessionModel sessionModel, ITestInfo info)
    {
      if (!IsCatheterIdValid)
      {
        DisplayInvalidCatheterIdDialog(info);
        return false;
      }

      return true;
    }

    private void InvokeCatheterConnectionDialog()
    {
      var parameters = new DialogParameters {
        { DialogTitleKey, ConnectCatheterDialogTitle },
        { DialogMessageKey, ConnectCatheterMessage }
      };
      Application.Current.Dispatcher.Invoke(() =>
      {
        _dialogService.ShowDialog(nameof(MessageDialog), parameters, null);
      });
    }

    private void InvokeRetryStopDialog(ITestInfo info)
    {
      var parameters = new DialogParameters
      {
        { DialogTitleKey, CatheterInvalidDialogTitle },
        { DialogMessageKey, CatheterConnectionFailureMessage }
      };
      Application.Current.Dispatcher.Invoke(() =>
      {
        _dialogService.ShowDialog(nameof(RetryStopDialog), parameters, (r) => RetryStopCallback(r, info));
      });
    }

    private void RetryStopCallback(IDialogResult dialogResult, ITestInfo info)
    {
      switch (dialogResult.Result)
      {
        case ButtonResult.Retry:
          info.Status = TestStatus.Inprogress;
          break;
        case ButtonResult.Abort:
          info.Status = TestStatus.Aborted;
          _eventAggregator.GetEvent<UserCommandEvent>().Publish((StopTest, DateTime.Now));
          info.Entity.Description = $"{Step2TestCaption}{ReadyStateCheckFailureTitle}{WhiteSpace}{StopMessage}";
          break;
        default:
          info.Status = TestStatus.Inprogress;
          break;
      }
    }

    private void DisplayInvalidCatheterIdDialog(ITestInfo info)
    {
      var parameters = new DialogParameters
      {
        { DialogTitleKey, CatheterIdVerificationDialogTitle },
        { DialogMessageKey, POLARxFITCatheterIsExpectedMessage },
        { RetryButtonTextKey, ContinueText }
      };
      Application.Current.Dispatcher.Invoke(() =>
      {
        _dialogService.ShowDialog(nameof(RetryStopDialog), parameters, (r) => RetryStopCallback(r, info));
      });
    }

    private void HandleCMCUPMCUSystemStatusError(PropertyChangedEventArgs e)
    {
      switch (e.PropertyName)
      {
        case nameof(_machineModel.PMCUSystemStatusErrorCode):
          _machineModel.IsCatheterCableConnected = (_machineModel.PMCUSystemStatusErrorCode & (long)PMCUStatusError.CatheterCableConnected) == (long)PMCUStatusError.CatheterCableConnected;
          if (!_machineModel.IsCatheterCableConnected)
          {
            _machineModel.CatheterSerialNumber = 0;
            _machineModel.CatheterID = 0; 
          }
          IsPMCUReady = (_machineModel.PMCUSystemStatusErrorCode & (long)PMCUStatusError.PMCUReady) == (long)PMCUStatusError.PMCUReady;
          _catheterStateSubject.OnNext(IsCatheterReady);
          break;
        case nameof(_machineModel.CMCUSystemStatusError):
          IsCMCUReady = (_machineModel.CMCUSystemStatusError & (long)CMCUStatusError.CMCUReady) == (long)CMCUStatusError.CMCUReady;
          _catheterStateSubject.OnNext(IsCatheterReady);
          break;
        case nameof(_machineModel.CatheterSerialNumber):
          _catheterStateSubject.OnNext(IsCatheterReady);
          break;
        default:
          break;
      }
    }
  }
}