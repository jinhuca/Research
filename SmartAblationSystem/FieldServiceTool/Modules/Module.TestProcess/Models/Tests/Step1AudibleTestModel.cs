using Module.Console.Interfaces;
using Module.Infrastructure;
using Module.Infrastructure.Controls;
using Module.Infrastructure.PubSubEvents;
using Module.Infrastructure.TestEntities;
using Module.Infrastructure.TestInterfaces;
using Module.Infrastructure.TestResults.Interfaces;
using Module.TestProcess.Views;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using static System.DateTime;
using static Module.Infrastructure.Constants.Strings;
using static Module.Infrastructure.Constants.UserCommand;
using static Module.Infrastructure.Helpers.ThreadHelpers;
using static Module.Infrastructure.SessionStatus;
using static Module.TestProcess.Constants.TestProcessMessages;
using static Module.TestProcess.Services.ServiceConstants;
using static System.Reactive.Linq.Observable;

namespace Module.TestProcess.Models.Tests
{
	public class Step1AudibleTestModel : BindableBase, ITestModel
	{
		public Step1AudibleTestModel(
			IDialogService dialogService,
			ITestInfo testInfo,
			IAudibleTestResult testResult,
			IMachineModel machineModel,
			IEventAggregator eventAggregator)
		{
			_dialogService = dialogService;
			_machineModel = machineModel;
			_eventAggregator = eventAggregator;
			_sessionStatusSubject = new BehaviorSubject<SessionStatus>(Unknown);
			Info = testInfo;
			Info.Entity = TestEntity.AudibleTestEntity;
			Result = testResult;
		}

		private readonly IEventAggregator _eventAggregator;
		private readonly IDialogService _dialogService;
		private readonly IMachineModel _machineModel;
		private readonly ISubject<SessionStatus> _sessionStatusSubject;

		private ITestInfo _info;
		public ITestInfo Info
		{
			get => _info;
			set => SetProperty(ref _info, value);
		}

		private IAudibleTestResult _result;
		public IAudibleTestResult Result
		{
			get => _result;
			set => SetProperty(ref _result, value);
		}

		private string _rationale = string.Empty;
		public string Rationale
		{
			get => _rationale;
			set => SetProperty(ref _rationale, value);
		}

		public async Task<ITestInfo> Start(CancellationToken cancellationToken, ISessionModel sessionModel)
		{
			ResetResult_();
			CheckSessionStatus_();
			if (cancellationToken.IsCancellationRequested || sessionModel.Status == Stopped)
			{
				Info.Status = TestStatus.Aborted;
				return await Task.FromResult(Info);
			}

      Info.Status = TestStatus.Inprogress;
			Info.Entity.Description = $"{Step1TestCaption}{AudibleTestTitle}{TestInProgressMessage}";
			Info.StartTime = Now;

      var audibleTestEvent = new AutoResetEvent(false);
    
      var testTask = Test_();
      InvokeResultDialog_();

      await testTask;
      audibleTestEvent.Dispose();

			if (RetryIfNecessary_())
      {
        return await Start(cancellationToken, sessionModel);
			}

      if(Info.Status == TestStatus.Aborted)
      {
	      Info.Entity.Description = $"{Step1TestCaption}{AudibleTestTitle}{TestStoppedMessage}";
	      return await Task.FromResult(Info);
      }

			Info.FinishTime = Now;
			Info.Entity.Description = $"{Step1TestCaption}{AudibleTestTitle}{Info.Status}";
			return await Task.FromResult(Info);

			void CheckSessionStatus_()
			{
        _sessionStatusSubject.OnNext(sessionModel.Status);
				using (var _waitSignalEvent = new ManualResetEvent(false))
				using (FromEventPattern<PropertyChangedEventArgs>(sessionModel, nameof(PropertyChanged))
					.Where(evt => evt.EventArgs.PropertyName == nameof(sessionModel.Status))
					.Subscribe(_ => _sessionStatusSubject.OnNext(sessionModel.Status)))
				using (_sessionStatusSubject.Subscribe(status_ =>
							 {
								 if (status_ != Paused && status_ != Pausing)
								 {
									 var pause_ = _waitSignalEvent.Set();
								 }
							 }))
				{
					if (sessionModel.Status == Paused)
					{
						Info.Entity.Description = $"{Step1TestCaption}{AudibleTestTitle}{TestPausedMessage}";
					}

					if (sessionModel.Status == Pausing)
					{
						sessionModel.Status = Paused;
						_eventAggregator.GetEvent<SessionStatusEvent>().Publish((sessionModel.Status, Now));
					}

					var pause_ = _waitSignalEvent.WaitOne();
				}
			}

			async Task Test_()
      {
        await Task.Run(() =>
        {
          _machineModel.Console.GUIIsReady = false;
          try
          {
					  WaitFor(DelayForConsoleWarningTestInSecond);
            _machineModel.Console.SetAudioLevel(100);
            do
						{
              if (cancellationToken.IsCancellationRequested)
                break;

						} while (!audibleTestEvent.WaitOne(TimeSpan.FromSeconds(1)));
          }
          finally
          {
            _machineModel.Console.GUIIsReady = true;
          }
        });
			}

			void InvokeResultDialog_()
			{
				var parameters = new DialogParameters
				{
					{ DialogTitleKey, AudibleTitle },
					{ DialogMessageKey, AudibleParameters }
				};
				Application.Current.Dispatcher.Invoke(() =>
				{
					_dialogService.ShowDialog(nameof(Step1AudibleTestDialog), parameters, AudibleTestDialogCallback_);
				});
			}

			void AudibleTestDialogCallback_(IDialogResult dialogResult)
			{
        audibleTestEvent.Set(); 
        switch (dialogResult.Result)
				{
					case ButtonResult.Yes:
						Result.Passed = true;
						Info.Status = TestStatus.Passed;
						break;
          case ButtonResult.No:
            {
              Result.Passed = false;
              if (sessionModel.Status == Stopped || sessionModel.Status == SessionStatus.Exception)
              {
								Info.Status = TestStatus.Aborted;
							}
              else
              {
                InvokeConfirmationDialog_();
              }
            }
            break;
        }
      }
			
			void InvokeConfirmationDialog_()
			{
				var confirmationParameters = new DialogParameters
				{
					{ DialogTitleKey, AudibleTitle + WhiteSpace + ConfirmationTitle },
					{ DialogMessageKey, ConfirmationMessage }
				};
				Application.Current.Dispatcher.Invoke(() =>
				{
					_dialogService.ShowDialog(nameof(ConfirmationDialog), confirmationParameters, ConfirmationDialogCallback_);
				});
			}
			
			void ConfirmationDialogCallback_(IDialogResult confirmationResult)
			{
				switch(confirmationResult.Result)
				{
					case ButtonResult.Abort:
						Result.Passed = false;
						Info.Status = TestStatus.Aborted;
						_eventAggregator.GetEvent<UserCommandEvent>().Publish((StopTest, Now));
						break;
					case ButtonResult.Ignore:
						Result.Passed = false;
						Info.Status = TestStatus.Failed;
						break;
					case ButtonResult.Retry:
						Info.Status = TestStatus.Retry;
						break;
				}
			}

			bool RetryIfNecessary_()
			{
				if (sessionModel.Status == Stopped)
        {
          Info.Status = TestStatus.Aborted; 
        }
        
        if (Info.Status == TestStatus.Retry)
				{
					Rationale = string.Empty;
					InvokeRetryRationaleDialog(Rationale);
				}

        return Info.Status == TestStatus.Retry;
      }

			void InvokeRetryRationaleDialog(string rationale)
			{
				var parameters = new DialogParameters
				{
					{ DialogTitleKey, RetryRationaleTitle },
					{ DialogMessageKey, rationale },
					{ ParamIdKey, RetryTitleAudibleTest }
				};
				Application.Current.Dispatcher.Invoke(() =>
				{
					_dialogService.ShowDialog(nameof(RationaleDialog), parameters, null);
				});
			}

			void ResetResult_()
			{
				Result.Passed = null;
			}
		}
	}
}
