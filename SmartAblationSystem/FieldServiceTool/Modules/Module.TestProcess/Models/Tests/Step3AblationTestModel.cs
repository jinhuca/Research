using Module.Infrastructure;
using Module.Infrastructure.PubSubEvents;
using Module.Infrastructure.TestEntities;
using Module.Infrastructure.TestInterfaces;
using Module.Infrastructure.TestResults.Interfaces;
using Module.TestProcess.Services;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using static System.DateTime;
using static Module.Infrastructure.Constants.AblationTestState;
using static Module.Infrastructure.SessionStatus;
using static Module.TestProcess.Constants.TestProcessMessages;
using static System.Reactive.Linq.Observable;

namespace Module.TestProcess.Models.Tests
{
	public class Step3AblationTestModel : BindableBase, ITestModel
	{
		private readonly IAblationService _ablationService;
		private readonly IEventAggregator _eventAggregator;
		private readonly ISubject<SessionStatus> _sessionStatusSubject;

		private ITestInfo _info;
		public ITestInfo Info
		{
			get => _info;
			set => SetProperty(ref _info, value);
		}

		private IAblationTestResult _result;
		public IAblationTestResult Result
		{
			get => _result;
			set => SetProperty(ref _result, value);
		}

		public Step3AblationTestModel(
			ITestInfo testInfo,
			IAblationTestResult testResult,
			IAblationService ablationService,
			IEventAggregator eventAggregator)
		{
			_ablationService = ablationService;
			_eventAggregator = eventAggregator;
			_sessionStatusSubject = new BehaviorSubject<SessionStatus>(Unknown);
			Info = testInfo;
			Info.Entity = TestEntity.AblationTestsEntity;
			Result = testResult;
		}

		public async Task<ITestInfo> Start(CancellationToken cancellationToken, ISessionModel sessionModel)
		{
			CheckSessionStatus_();
			if(cancellationToken.IsCancellationRequested || sessionModel.Status == Stopped)
			{
				Info.Status = TestStatus.Failed;
				Info.Entity.Description = Ablations + StopMessage;
				return await Task.FromResult(Info);
			}

			Info.StartTime = Now;
			Info.Status = TestStatus.Inprogress;
			var result = await Task.Run(() => _ablationService.Start(cancellationToken, sessionModel));
			
			Info.FinishTime = Now;
			Info.Status = EvaluateTestStatus();
			Result = GenerateTestResult(Info);

			if(Info.Status == TestStatus.Aborted)
			{
				Info.Entity.Description = $"{Step3TestCaption}{AblationTestTitle}{TestStoppedMessage}";
				Result.Passed = false;
				return await Task.FromResult(result);
			}

			Info.Entity.Description = $"{Step3TestCaption}{AblationTestTitle}{TestFinishedMessage}";
			return await Task.FromResult(result);

			void CheckSessionStatus_()
			{
				var _waitSignalEvent = new ManualResetEvent(false);

				FromEventPattern<PropertyChangedEventArgs>(sessionModel, nameof(PropertyChanged))
					.Where(evt => evt.EventArgs.PropertyName == nameof(sessionModel.Status))
					.Subscribe(_ => _sessionStatusSubject.OnNext(sessionModel.Status));

				using(_sessionStatusSubject.Subscribe(status_ =>
				{
					if(status_ != Paused && status_ != Pausing)
					{
						var pause_ = _waitSignalEvent.Set();
					}
				}))
				{
					if(sessionModel.Status == Paused)
					{
						Info.Entity.Description = $"{Step3TestCaption}{AblationTestTitle}{TestPausedMessage}";
						_eventAggregator.GetEvent<SessionStatusEvent>().Publish((sessionModel.Status, Now));
					}

					if(sessionModel.Status == Pausing)
					{
						Info.Entity.Description = $"{Step3TestCaption}{AblationTestTitle}{TestPausingMessage}";
					}

					var pause_ = _waitSignalEvent.WaitOne();
				}
			}
		}

    private TestStatus EvaluateTestStatus()
    {
      var testResult_ = _ablationService.DataManagement.GetTestResult();
      bool[] testPassedArray_ = new bool[3];
      int stateIndex_ = 0;
      foreach(var key_ in testResult_.AblationResult.Keys)
      {
        var result_ = testResult_
          .AblationResult[key_]
          .Values
          .All(item => item.TrueForAll(value => value.Passed != false));

        if(key_ == INFLATION || key_ == ABLATION || key_ == THAWING)
        {
          testPassedArray_[stateIndex_] = result_;
          stateIndex_++;
        }
      }

      var smoothness_ = testResult_.Smoothness;
      var parameterPassed_ = Array.TrueForAll(testPassedArray_, passed_ => passed_);
      var ablationCompleted_ = _ablationService.AblationCount == testResult_.AblationSummaryList.Count;

      if (!ablationCompleted_)
      {
        return TestStatus.Aborted;
      }

      return parameterPassed_ && smoothness_ && ablationCompleted_ ? TestStatus.Passed : TestStatus.Failed;
    }

		private IAblationTestResult GenerateTestResult(ITestInfo testInfo)
		{
			var testResult_ = _ablationService.DataManagement.GetTestResult();
			testResult_.Passed = _ablationService.Result.Passed;
			return testResult_;
		}
	}
}
