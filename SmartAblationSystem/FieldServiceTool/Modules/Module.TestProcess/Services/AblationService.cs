using Module.Console.Interfaces;
using Module.FlowMeterComm.Services;
using Module.Infrastructure;
using Module.Infrastructure.AppLog;
using Module.Infrastructure.Constants;
using Module.Infrastructure.Controls;
using Module.Infrastructure.PubSubEvents;
using Module.Infrastructure.TestEntities;
using Module.Infrastructure.TestInterfaces;
using Module.Infrastructure.TestResults.Interfaces;
using Module.SystemParameters.Interfaces;
using Module.TestProcess.Helpers;
using Module.TestProcess.Models.Tests;
using Module.TestProcess.Views.Dialogs;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using Module.TestProcess.Constants;
using static Communication.CanBusMessageDefinition;
using static Communication.CanBusMessageDefinition.MessageStateId;
using static Module.Infrastructure.Constants.AblationTestState;
using static Module.Infrastructure.Constants.Strings;
using static Module.Infrastructure.Constants.UserCommand;
using static Module.Infrastructure.Helpers.ThreadHelpers;
using static Module.Infrastructure.SessionStatus;
using static Module.TestProcess.Constants.TestProcessMessages;
using static Module.TestProcess.Services.ServiceConstants;
using static System.DateTime;
using static System.Reactive.Linq.Observable;
using static System.Threading.Tasks.Task;
using static System.TimeSpan;

namespace Module.TestProcess.Services
{
	public class AblationService : BindableBase, IAblationService
	{
		#region Fields

		private readonly IContainerProvider _containerProvider;
		private readonly IMachineModel _machineModel;
		private readonly ISensorParameters _sensorParameters;
    private readonly ICatheterVerificationService _catheterVerificationService;

    private readonly IEventAggregator _eventAggregator;
		private readonly IDialogService _dialogService;
		private readonly ISubject<MessageStateId> _systemStateSubject;
		private readonly ISubject<double> _balloonTemperatureSubject;
		// private readonly AutoResetEvent _consoleSwitchState = new AutoResetEvent(false);
		private readonly ISubject<SessionStatus> _sessionStatusSubject;
		private ISessionModel _sessionModel;

		private readonly Dictionary<int, IAblationConfiguration> _ablationConfigurationDictionary;
		private readonly int _ablationCount;
		private readonly List<bool> _ablationTestStatusList = new List<bool>();
		private IbpAblationValidator _ibpAblationValidator;
		private Pwm2AblationValidator _pwm2AblationValidator;
		private double _ObpInReady;

		private readonly List<Task> _dataAnalysisTasks = new List<Task>();

		private Task _inflationDataAnalysisTask;
		private Task _ablationDataAnalysisTask;
		private Task _thawingDataAnalysisTask;

		private CancellationTokenSource _samplingSensorDataCancellationTokenSource = new CancellationTokenSource();
		private CancellationToken _samplingSensorDataCancellationToken;
		private CancellationTokenSource _samplingAblationDetailsCancellationTokenSource = new CancellationTokenSource();
		private CancellationToken _samplingAblationDetailsCancellationToken;

		private string _excelFileGenerated = string.Empty;
		private readonly IXlsxService _xlsxService;

		private bool _needToPerformFlowMeterCheck;
		private IFlowMeterDataManager _flowMeterDataManager;
		private string _validationMsg = string.Empty;
		private readonly SerialDisposable _sessionStatusObserverDisposable = new SerialDisposable();
    private FlowMeterValidationResult _flowMeterValidationResult;
		#endregion Fields

		#region Properties

		public bool? Passed { get; set; }
		public IAblationConfiguration Configuration { get; set; }
		public IAblationTestResult Result { get; set; }
		public int AblationCount { get; set; }
		public IAblationDataManagement DataManagement { get; set; }
		public ITestInfo Info { get; set; }

		// Enable/Disable Flow Meter check from user input 
		public bool FlowMeterCheckEnabled { get; set; }
		#endregion Properties

		public AblationService(
			IContainerProvider containerProvider,
			IMachineModel machineModel,
			ISensorParameters sensorParameters,
      ICatheterVerificationService catheterVerificationService,
      IAblationConfiguration configuration,
			IAblationDataManagement dataManagement,
			IAblationTestResult ablationTestAblationResult,
			ITestInfo info,
			IDialogService dialogService,
			Dictionary<int, IAblationConfiguration> ablationConfigurationDictionary,
			IXlsxService xlsxService,
			IEventAggregator eventAggregator)
		{
			_containerProvider = containerProvider;
			_machineModel = machineModel;
			_sensorParameters = sensorParameters;
      _catheterVerificationService = catheterVerificationService;

      _systemStateSubject = new BehaviorSubject<MessageStateId>(CAN_ID_STATE_UNKNOWN);
			_sessionStatusSubject = new BehaviorSubject<SessionStatus>(Unknown);
			_balloonTemperatureSubject = new BehaviorSubject<double>(_machineModel.TC1Reading);
			_ablationConfigurationDictionary = ablationConfigurationDictionary;
			_xlsxService = xlsxService;
			_eventAggregator = eventAggregator;
			_dialogService = dialogService;
			Result = ablationTestAblationResult;

			Configuration = configuration;
			DataManagement = dataManagement;
			Info = info;
			Info.Entity = TestEntity.AblationTestsEntity;

			FromEventPattern<PropertyChangedEventArgs>(_machineModel, nameof(_machineModel.PropertyChanged))
				.Where(arg => arg.EventArgs.PropertyName == nameof(_machineModel.SystemState))
				.Subscribe(_ => _systemStateSubject?.OnNext(_machineModel.SystemState));

			FromEventPattern<PropertyChangedEventArgs>(_machineModel, nameof(_machineModel.PropertyChanged))
				.Where(arg => arg.EventArgs.PropertyName == nameof(_machineModel.TC1Reading))
				.Subscribe(_ => _balloonTemperatureSubject?.OnNext(_machineModel.TC1Reading));

			_samplingSensorDataCancellationToken = _samplingSensorDataCancellationTokenSource.Token;
			_samplingAblationDetailsCancellationToken = _samplingAblationDetailsCancellationTokenSource.Token;
			_machineModel.Console.DeflateAfterThaw = true;

			InitializeConsole();
			InitializeConfiguration();
			InitializeDataManagement();
			_ablationCount = _ablationConfigurationDictionary.Count;
			AblationCount = _ablationCount;
      _flowMeterValidationResult = null; 

#if DEBUG
      _machineModel.CatheterID = 1;
			_machineModel.CatheterLot = 128;
			_machineModel.CatheterSerialNumber = 182;
#endif
		}

		public async Task<ITestInfo> Start(CancellationToken cancellationToken, ISessionModel sessionModel)
		{
			_sessionModel = sessionModel;

      CheckSessionStatus_(-1);
			if(cancellationToken.IsCancellationRequested || sessionModel.Status == Stopped)
			{
				Info.Status = TestStatus.Aborted;
				return await Task.FromResult(Info);
			}

			_sessionStatusObserverDisposable.Disposable = FromEventPattern<PropertyChangedEventArgs>(sessionModel, nameof(PropertyChanged))
				.Where(evt => evt.EventArgs.PropertyName == nameof(sessionModel.Status))
				.ObserveOn(TaskPoolScheduler.Default)
				.Subscribe(_ => _sessionStatusSubject.OnNext(sessionModel.Status));

			Result.Initialize(AblationCount);
			_ibpAblationValidator = GetIbpRule();
			_pwm2AblationValidator = GetPwm2Rule();
			Info.Status = TestStatus.Inprogress;

			PromptToInstallFlowMeterOrSkip();
			_needToPerformFlowMeterCheck = FlowMeterCheckEnabled;
      _flowMeterValidationResult = null;

      CheckBalloonTemperature(sessionModel, 1);
			if(Info.Status == TestStatus.Aborted || sessionModel.Status == Stopped)
			{
				Info.Status = TestStatus.Aborted;
				return Info;
			}

		restart:
			if(Info.Status == TestStatus.Retry)
			{
				ResetSamplingDetails();
				ResetDataCancellationSRC();
				Result.Reset(AblationCount);

				if(File.Exists(_excelFileGenerated))
				{
					try
					{
						File.Delete(_excelFileGenerated);
					}
					catch(IOException ioe)
					{
						FieldServiceTrace.LogException(ioe);
					}
				}
				Info.Status = TestStatus.Inprogress;
			}

			_excelFileGenerated = _xlsxService.GenerateXlsxFileName(sessionModel.StartTime?.ToString(ReportDateTimeFormat));
			_ablationTestStatusList.Clear();

			var createdExcelFile_ = await _xlsxService.CreateExcelFile(_excelFileGenerated);
			if(!createdExcelFile_)
			{
				await FromException<ITestInfo>(new IOException());
			}

			try
			{
				await _xlsxService.AddSummaryToWorksheet(Result, _excelFileGenerated);
			}
			catch(Exception e)
			{
				DisplayExceptionMessage(e);
				FieldServiceTrace.LogException(e);
				await FromException<ITestInfo>(new Exception());
			}

			if(_needToPerformFlowMeterCheck)
			{
				InitializeFlowMeterDataManagement();
			}

			foreach(var ablationConfig in _ablationConfigurationDictionary)
      {
				// Always start with small balloon for a single ablation, only enable DAS balloon in inflation state when needed 
        EnableDASBalloon(false);
        
        CheckSessionStatus_(ablationConfig.Key);
				if(cancellationToken.IsCancellationRequested || sessionModel.Status == Stopping || sessionModel.Status == Stopped)
				{
					StopStateMachine();
					Info.Status = TestStatus.Aborted;
					Info.Entity.Description = $"{Step3TestCaption}{ablationConfig.Key}{OfText}{_ablationCount}{Ablations}{StopMessage}";
					break;
				}

				Configuration = _ablationConfigurationDictionary[ablationConfig.Key];

        try
        {
          StartSingleAblationTest(ablationConfig.Key, cancellationToken, sessionModel);
        }
        catch (Exception ex)
        {
          Info.Status = TestStatus.Aborted;
          FieldServiceTrace.LogException(ex);
          break;
        }
        finally
        {
					// Leave System with small balloon when finish ablation, no matter succeeded or failed   
					EnableDASBalloon(false);
        }

				if(cancellationToken.IsCancellationRequested ||
					 Info.Status == TestStatus.Aborted ||
					 sessionModel.Status == Stopped)
				{
					break;
				}

				// check if there is Flow Meter communication error when flow meter check is enabled
				if(_needToPerformFlowMeterCheck && (_flowMeterDataManager?.IsConnectionLost ?? false))
				{
					// prompt to ask retry or skip
					RetryOrSkipFlowMeterCheck(FlowMeterCommErrorMessage);
					// if Skip was pressed, the _needToPerformFlowMeterCheck would be set to false 
					if(_needToPerformFlowMeterCheck)
					{
						Info.Status = TestStatus.Retry;
						goto restart;
					}
				}

				switch(CheckFMSmoothness_(ablationConfig.Key))
				{
					case true:
						Info.Status = TestStatus.Inprogress;
						break;
					case false:
						InvokeRetryRationaleDialog(Result.RetryRationale);
						Info.Status = TestStatus.Retry;
						goto restart;
					case null: // Stop
						Info.Status = TestStatus.Aborted;
						break;
						void InvokeRetryRationaleDialog(string rationale)
						{
							var parameters = new DialogParameters
							{
								{ DialogTitleKey, RetryRationaleTitle },
								{ DialogMessageKey, rationale },
								{ ParamIdKey, RetryTitleAblationTest }
							};
							Application.Current.Dispatcher.Invoke(() =>
							{
								_dialogService.ShowDialog(nameof(RationaleDialog), parameters, null);
							});
						}
				}

				Info.Entity.Description = $"{Step3TestCaption}{ablationConfig.Key}{OfText}{_ablationCount}{Ablations}{SavingDataMessage}";
				var saved_ = await _xlsxService.AddAblationDetailToWorksheet(ablationConfig.Key, Result, _excelFileGenerated);

        // Update _flowMeterValidationResult if it is required. 
        if (_needToPerformFlowMeterCheck)
        {
          _flowMeterValidationResult = _flowMeterDataManager.ValidateFlowMeter(); 
        }

				if (_flowMeterValidationResult != null && ablationConfig.Key == 1)
          saved_ &= await _xlsxService.AddFlowMeterDetailToWorksheet(ablationConfig.Key, _flowMeterValidationResult.DataCollection, _excelFileGenerated);

        if (Info.Status == TestStatus.Aborted || sessionModel.Status == Stopped)
				{
					break;
				}

				if(_needToPerformFlowMeterCheck)
				{
					// disable flow meter check after first ablation
					DisposeFlowMeterDataManager();
          ValidateFlowMeterResult(_flowMeterValidationResult);
          // saved_ &= await _xlsxService.AddFlowMeterDetailToWorksheet(ablationConfig.Key, _flowMeterValidationResult.DataCollection, _excelFileGenerated);

          if (!_flowMeterValidationResult?.IsValid??false)
					{
						Info.Status = TestStatus.Retry;
						goto restart;
					}
        }

        Info.Entity.Description = saved_
					? $"{Step3TestCaption}{ablationConfig.Key}{OfText}{_ablationCount}{Ablations}{SavedDataMessage}"
					: $"{Step3TestCaption}{ablationConfig.Key}{OfText}{_ablationCount}{Ablations}{FailedSavingDataMessage}";

				try
				{
					var valid_ = ValidateSingleAblation_(ablationConfig.Key);

					if(valid_ == false)
					{
						InvokeAblationFailureDialog(ablationConfig.Key, _validationMsg);
						if(Info.Status == TestStatus.Retry)
						{
							ResetSamplingDetails();
							ResetDataCancellationSRC();
							var parameters_ = new DialogParameters()
						{
							{ DialogTitleKey, RetryRationaleTitle },
							{ DialogMessageKey, Result.RetryRationale },
							{ ParamIdKey, RetryTitleAblationTest }
						};
							Application.Current.Dispatcher.Invoke(() =>
							{
								_dialogService.ShowDialog(nameof(RationaleDialog), parameters_, null);
							});
							goto restart;
						}
						_ablationTestStatusList.Add(false);
						if(Info.Status == TestStatus.Aborted)
						{
							break;
						}
					}
					else
					{
						_ablationTestStatusList.Add(true);
					}
				}
				catch(Exception ex)
				{
					DisplayExceptionMessage(ex);
					FieldServiceTrace.LogException(ex);
					Info.Status = TestStatus.Aborted;
					break;
				}
			}

			StopStateMachine();
			if(Info.Status != TestStatus.Aborted)
			{
				Info.Status = TestStatus.Finished;
			}
			else
			{
				sessionModel.Status = Stopped;
				_eventAggregator.GetEvent<SessionStatusEvent>().Publish((Stopped, Now));
			}

			Info.Entity.Description = $"{Step3TestCaption}{Ablations}{SavingDataMessage}";
			var successful_ = await _xlsxService.AddSummaryToWorksheet(Result, _excelFileGenerated);
			Info.Entity.Description = successful_
				? $"{Step3TestCaption}{Ablations}{SavedDataMessage}"
				: $"{Step3TestCaption}{Ablations}{FailedSavingDataMessage}";

			Result.Passed = _ablationTestStatusList.All(status_ => status_);

			_sessionStatusObserverDisposable.Disposable?.Dispose();
			return Info;

			void CheckSessionStatus_(int ablationId)
			{
				using(var _waitSignalEvent = new ManualResetEvent(false))
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
						Info.Entity.Description = $"{Step3TestCaption}{ablationId}{OfText}{_ablationCount}{Ablations}{TestPausedMessage}";
					}

					if(sessionModel.Status == Pausing)
					{
						sessionModel.Status = Paused;
						_eventAggregator.GetEvent<SessionStatusEvent>().Publish((sessionModel.Status, Now));
						Info.Entity.Description = $"{Step3TestCaption}{ablationId}{OfText}{_ablationCount}{Ablations}{sessionModel.Status}.";
					}

					var pause_ = _waitSignalEvent.WaitOne();
				}
			}

			bool? CheckFMSmoothness_(int ablationId)
			{
				bool? result_ = null;

				if(ablationId != 1)
				{
					return true;
				}

				var fm1Transition_ = Result.AblationDetailsList
					.Where(item_ => item_.State == TRANSITION)
					.Select(value_ => value_.FM1).ToList();

				var fm1Ablation_ = Result.AblationDetailsList
					.Where(item_ => item_.State == ABLATION)
					.Select(value_ => value_.FM1)
					.Take(10).ToList();

				fm1Transition_.AddRange(fm1Ablation_);

				var parameters_ = new DialogParameters
				{
					{ DialogTitleKey, SmoothnessVerificationTitle },
					{ DialogMessageKey, SmoothnessMessage },
					{ Fm1TransitionKey, fm1Transition_ }
				};
				Application.Current.Dispatcher.Invoke(() =>
				{
					_dialogService.ShowDialog(nameof(SmoothnessVerificationDialog), parameters_, SmoothnessVerificationCallback_);
				});

				return result_;

				void SmoothnessVerificationCallback_(IDialogResult dialogResult)
				{
					switch(dialogResult.Result)
					{
						case ButtonResult.Yes:
							Result.Smoothness = true;
							result_ = true;
							break;
						case ButtonResult.No:
							result_ = false;
							Result.Smoothness = false;
							InvokeSmoothFailDialog();
							break;
						case ButtonResult.Abort:
							result_ = false;
							break;
						case ButtonResult.Cancel:
						case ButtonResult.Ignore:
						case ButtonResult.None:
						case ButtonResult.Retry:
						case ButtonResult.OK:
						default:
							result_ = false;
							break;
					}
				}

				void InvokeSmoothFailDialog()
				{
					var failDialogParameters_ = new DialogParameters
					{
						{ DialogTitleKey, SmoothnessFailureTitle },
						{ DialogMessageKey, SmoothnessFailureMessage }
					};
					Application.Current.Dispatcher.Invoke(() =>
					{
						_dialogService.ShowDialog(nameof(ConfirmationDialog), failDialogParameters_, failCallback_);
					});

					void failCallback_(IDialogResult dialogResult)
					{
						switch(dialogResult.Result)
						{
							case ButtonResult.Ignore:
								result_ = true;
								break;
							case ButtonResult.Retry:
								result_ = false;
								break;
							case ButtonResult.Abort:
								result_ = null;
								break;
							case ButtonResult.Cancel:
							case ButtonResult.No:
							case ButtonResult.None:
							case ButtonResult.OK:
							case ButtonResult.Yes:
							default:
								break;
						}
					}
				}
			}

			bool? ValidateSingleAblation_(int ablationId)
			{
				Task.WaitAll(_dataAnalysisTasks.ToArray());
				_validationMsg = string.Empty;
				bool? isValid_ = null;

				var inflationValid_ = true;
				var inflationSpeedValid_ = true;
				var inflationObpValid_ = true;
				var inflationPt2Valid_ = true;
				var inflationFm1Valid_ = true;
				var inflationIbpValid_ = true;

				var ablationValid_ = true;
				var transitionTimeValid_ = true;
				var ablationFm1Valid_ = true;
				var ablationPt2Valid_ = true;
				var ablationIbpValid_ = true;
				var ablationObpValid_ = true;
				var ablationPwm2Valid_ = true;
				var ablationTs1Valid_ = true;
				var ablationSmooth_ = true;

				foreach(var item in Result.AblationResult)
				{
					var index_ = ablationId - 1;
					switch(item.Key)
					{
						case INFLATION:
							var inflationResult_ = new List<(string, string, string)>();

							if(item.Value[TestParameter.Inflation_Speed][index_].Passed == false)
							{
								inflationSpeedValid_ = false;
                var lowerBound_ = string.Empty;

                if (Configuration.IsFastInflation)
                {
                  lowerBound_ = string.Empty;
                }
                else
                {
                  if (index_ > 0)
                  {
                    var previousResult_ = Result?.GetSensorDataDictionaryById(index_);
                    if (previousResult_?[TestParameter.Inflation_Speed]?.Count > 0)
                    {
                      var previousSpeed_ = previousResult_[TestParameter.Inflation_Speed].FirstOrDefault();
                      lowerBound_ = (previousSpeed_ + SlowInflationDelta).ToString(TwoDecimalPlace, CultureInfo.CurrentCulture);
                    }
                    else
                    {
                      lowerBound_ = SlowInflationSpeedLower.ToString(TwoDecimalPlace, CultureInfo.CurrentCulture);
                    }
                  }
                }

                var expectedInflationSpeed_ = string.IsNullOrWhiteSpace(lowerBound_) ? InflationSpeedExpectedText2 : $"{lowerBound_}{InflationSpeedExpectedText1}";
                var testedInflationSpeed = item.Value[TestParameter.Inflation_Speed][index_].Value.ToString(ThreeDecimalPlace, CultureInfo.InvariantCulture);
								inflationResult_.Add((SpeedText, testedInflationSpeed, expectedInflationSpeed_));
							}

							if(ablationId == 1 && item.Value[TestParameter.Inflation_OBP][0].Passed == false)
							{
								inflationObpValid_ = false;
								var expectedInflationObp_ = LessEqualText + Math.Round(_ObpInReady + OBPAdjustment, RoundOneDigit).ToString(OneDecimalPlace, CultureInfo.InvariantCulture);
								var testedInflationObp_ = item.Value[TestParameter.Inflation_OBP][0].Value.ToString(OneDecimalPlace, CultureInfo.InvariantCulture);
								inflationResult_.Add((OBPText, testedInflationObp_, expectedInflationObp_));
							}

							if(item.Value[TestParameter.Inflation_PT2][index_].Passed == false)
							{
								inflationPt2Valid_ = false;
								var expectedInflationPt2_ = InflationPT2ExpectedText;
								var testedInflationPt2_ = item.Value[TestParameter.Inflation_PT2][index_].Value.ToString(TwoDecimalPlace, CultureInfo.InvariantCulture);
								inflationResult_.Add((PT2Text, testedInflationPt2_, expectedInflationPt2_));
							}

							if(item.Value[TestParameter.Inflation_FM1][index_].Passed == false)
							{
								inflationFm1Valid_ = false;
								var expectedInflationFm1_ = InflationFM1Threshold;
								var testedInflationFm1_ = item.Value[TestParameter.Inflation_FM1][index_].Value.ToString(OneDecimalPlace, CultureInfo.InvariantCulture);
								inflationResult_.Add((FM1Text, testedInflationFm1_, expectedInflationFm1_));
							}

							if(item.Value[TestParameter.Inflation_IBP][index_].Passed == false)
							{
								inflationIbpValid_ = false;
								var expectedInflationIbp_ = Configuration.EnableDASBalloon ? InflationIBPDASBalloonThreshold : InflationIBPThreshold;
								var testedInflationIBP_ = item.Value[TestParameter.Inflation_IBP][index_].Value.ToString(TwoDecimalPlace, CultureInfo.InstalledUICulture);
								inflationResult_.Add((IBPText, testedInflationIBP_, expectedInflationIbp_));
							}

							inflationValid_ = inflationSpeedValid_
																&& inflationObpValid_
																&& inflationPt2Valid_
																&& inflationFm1Valid_
																&& inflationIbpValid_;

							if(!inflationValid_)
							{
								var padding_ = 21;
								var char_ = ' ';

								_validationMsg += $"{InflationTitle.PadRight(padding_, char_)}{ActualValueTitle.PadRight(padding_, char_)}{ExpectedValueTitle.PadRight(padding_, char_)}{Environment.NewLine}";
								foreach(var tuple in inflationResult_)
								{
									_validationMsg += $"{WhiteSpace}{tuple.Item1.PadRight(padding_, char_)}{tuple.Item2.PadRight(padding_, char_)}{tuple.Item3.PadRight(padding_, char_)}{Environment.NewLine}";
								}
								_validationMsg += Environment.NewLine;
							}
							break;
						case TRANSITION:
							break;
						case ABLATION:
							var ablationResult_ = new List<(string, string, string)>();

							if(item.Value[TestParameter.Ablation_FM1][index_].Passed == false)
							{
								ablationFm1Valid_ = false;
								var expectedAblationFm1_ = Configuration.EnableDASBalloon ? AblationFM1DASBalloonThresholdText : AblationFM1ThresholdText;
								var testedAblationFm1_ = item.Value[TestParameter.Ablation_FM1][index_].Value.ToString(OneDecimalPlace, CultureInfo.InvariantCulture);
								ablationResult_.Add((FM1Text, testedAblationFm1_, expectedAblationFm1_));
							}

							if(item.Value[TestParameter.Transition_Time][index_].Passed == false)
							{
								transitionTimeValid_ = false;
								var expectedTransitionTime_ = TransitionTimeRangeText;
								var testedTransitionTime_ = item.Value[TestParameter.Transition_Time][index_].Value.ToString(TwoDecimalPlace, CultureInfo.InstalledUICulture);
								ablationResult_.Add((TransitionTimeText, testedTransitionTime_, expectedTransitionTime_));
							}

							if(item.Value[TestParameter.Ablation_PT2][index_].Passed == false)
							{
								ablationPt2Valid_ = false;
								var expectedAblationPt2_ = AblationPT2ThresholdText;
								var testedAblationPt2_ = item.Value[TestParameter.Ablation_PT2][index_].Value.ToString(ThreeDecimalPlace, CultureInfo.InvariantCulture);
								ablationResult_.Add((PT2Text, testedAblationPt2_, expectedAblationPt2_));
							}

							if(item.Value[TestParameter.Ablation_IBP]?.Count > 0 &&
							   item.Value[TestParameter.Ablation_IBP][index_].Passed != null && 
							   item.Value[TestParameter.Ablation_IBP][index_].Passed == false)
							{
								ablationIbpValid_ = false;
								var expectedAblationIbp_ = Configuration.EnableDASBalloon ? AblationIBPDASBalloonThresholdText : AblationIBPThresholdText;
								var testedAblationIbp_ = item.Value[TestParameter.Ablation_IBP][index_].Value.ToString(TwoDecimalPlace, CultureInfo.InvariantCulture);
								ablationResult_.Add((IBPText, testedAblationIbp_, expectedAblationIbp_));
							}

							if(item.Value[TestParameter.Ablation_OBP][index_].Passed == false)
							{
								ablationObpValid_ = false;
								var expectedAblationObp_ = item.Value[TestParameter.Ablation_OBP][index_].Expected.HasValue
									? item.Value[TestParameter.Ablation_OBP][index_].Expected.Value.ToString(OneDecimalPlace, CultureInfo.InvariantCulture)
									: NAText;
								if(expectedAblationObp_ != NAText)
								{
									expectedAblationObp_ = LessEqualText + expectedAblationObp_;
								}
								var testedAblationObp_ = item.Value[TestParameter.Ablation_OBP][index_].Value.ToString(OneDecimalPlace, CultureInfo.InvariantCulture);
								ablationResult_.Add((OBPText, testedAblationObp_, expectedAblationObp_));
							}

							if(item.Value[TestParameter.Ablation_PWM2]?.Count > 0 &&
							   item.Value[TestParameter.Ablation_PWM2][index_].Passed != null && 
							   item.Value[TestParameter.Ablation_PWM2][index_].Passed == false)
							{
								ablationPwm2Valid_ = false;
								var expectedAblationPwm2_ = AblationPWM2TextRule1;
								var testedAblationPwm2_ = item.Value[TestParameter.Ablation_PWM2][index_].Value.ToString(TwoDecimalPlace, CultureInfo.InvariantCulture);
								ablationResult_.Add((PWM2Text, testedAblationPwm2_, expectedAblationPwm2_));
							}

							if(item.Value[TestParameter.Ablation_TS1][index_].Passed == false)
							{
								ablationTs1Valid_ = false;
								var expectedAblationTs1_ = AblationTS1ThresholdText;
								var testedAblationTs1_ = item.Value[TestParameter.Ablation_TS1][index_].Value.ToString(OneDecimalPlace, CultureInfo.InvariantCulture);
								ablationResult_.Add((TS1Text, testedAblationTs1_, expectedAblationTs1_));
							}

							if (!Result.Smoothness && ablationId == 1)
							{
								ablationSmooth_ = false;
								ablationResult_.Add(("Smoothness", TestResult.Fail.ToString(), TestResult.Pass.ToString()));
							}

							ablationValid_ = ablationFm1Valid_
															 && transitionTimeValid_
															 && ablationPt2Valid_
															 && ablationIbpValid_
															 && ablationObpValid_
															 && ablationPwm2Valid_
															 && ablationTs1Valid_
															 && ablationSmooth_;

							if(!ablationValid_)
							{
								var padding_ = 21;
								var char_ = ' ';

								_validationMsg += $"{AblationTitle.PadRight(padding_, char_)}{ActualValueTitle.PadRight(padding_, char_)}{ExpectedValueTitle.PadRight(padding_, char_)}{Environment.NewLine}";
								foreach(var tuple in ablationResult_)
								{
									_validationMsg += $"{WhiteSpace}{tuple.Item1.PadRight(padding_, char_)}{tuple.Item2.PadRight(padding_, char_)}{tuple.Item3.PadRight(padding_, char_)}{Environment.NewLine}";
								}
							}
							break;
						case THAWING:
							break;
						case UNKNOWN:
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
				}

				isValid_ = inflationValid_ && ablationValid_;

				return isValid_;
			}

			void InvokeAblationFailureDialog(int ablationId, string paramValue)
			{
				var title_ = "#" + ablationId + WhiteSpace + AblationFailureMsg;
				var parameters = new DialogParameters()
				{
					{ DialogTitleKey, title_ },
					{ DialogMessageKey, paramValue }
				};
				Application.Current.Dispatcher.Invoke(() =>
				{
					_dialogService.ShowDialog(nameof(Step3AblationCheckDialog), parameters, ConfirmationDialogCallback_);
				});
			}

			void ConfirmationDialogCallback_(IDialogResult confirmation)
			{
				switch(confirmation.Result)
				{
					case ButtonResult.Abort:
						Info.Status = TestStatus.Aborted;
						break;
					case ButtonResult.Ignore:
						Info.Status = TestStatus.Failed;
						break;
					case ButtonResult.Retry:
						Info.Status = TestStatus.Retry;
						break;
					case ButtonResult.Cancel:
					case ButtonResult.No:
					case ButtonResult.None:
					case ButtonResult.OK:
					case ButtonResult.Yes:
					default:
						break;
				}
			}

			IbpAblationValidator GetIbpRule()
			{
				// For DASBalloon test, always do Pass/Fail check
        if (Configuration.EnableDASBalloon)
        {
          return IbpAblationValidator.Rule1;
        }

        var pt3Idle_ = (sessionModel?.Steps[StepId.Step2]?.Tests[TestId.IdleStateCheck] as Step2IdleCheckModel).Result.PT3Avg;

				if(pt3Idle_.Value >= PT3IdleLower && pt3Idle_.Value <= PT3IdleUpper)
				{
					return IbpAblationValidator.Rule1;
				}
				return pt3Idle_.Value < PT3IdleLower ? IbpAblationValidator.Rule2 : IbpAblationValidator.Rule3;
			}

			Pwm2AblationValidator GetPwm2Rule()
			{
				var pt3Idle_ = (sessionModel?.Steps[StepId.Step2]?.Tests[TestId.IdleStateCheck] as Step2IdleCheckModel).Result.PT3Avg;

				if(pt3Idle_.Value >= PT3IdleLower && pt3Idle_.Value <= PT3IdleUpper)
				{
					return Pwm2AblationValidator.Rule1;
				}
				return pt3Idle_.Value < PT3IdleLower ? Pwm2AblationValidator.Rule2 : Pwm2AblationValidator.Rule3;
			}
		}

		#region State Machine

		public void StartSingleAblationTest(int ablationId, CancellationToken cancellationToken, ISessionModel sessionModel)
		{
			_dataAnalysisTasks.Clear();
			_systemStateSubject.OnNext(_machineModel.SystemState);
			Info.Entity.Description = $"{Step3TestCaption}{ablationId}{OfText}{_ablationCount}{Ablations}{TestInProgressMessage}";
			Info.Status = TestStatus.Inprogress;

      if (!_catheterVerificationService.VerifyCatheterIsReadyAndValid(cancellationToken, sessionModel, Info))
        return;

			CheckBalloonTemperature(sessionModel, WaitForTemperatureAfterInflationInSeconds);
			if(cancellationToken.IsCancellationRequested || Info.Status == TestStatus.Aborted)
			{
				return;
			}

			// Set console to Idle state before starting a new ablation
			InitializeConsole();
			// Set the Inflation mode before start ablation test (leave console time to process it when starts inflation)
			SetInflationMode_();
			WaitFor(DelayForAblationTest);

			VerifySystemInReadyState(ablationId, cancellationToken, sessionModel);

			return;

			void SetInflationMode_()
			{
				_machineModel.Console.EnableFastInflationMode = Configuration.IsFastInflation;
				var speedMode_ = Configuration.IsFastInflation ? InflationSpeedFast : InflationSpeedSlow;
				Info.Entity.Description = $"{Step3TestCaption}{ablationId}{OfText}{_ablationCount}{Ablations}{MessageInflationSpeedMode}{speedMode_}";
			}
		}

		private void VerifySystemInReadyState(int ablationId, CancellationToken cancellationToken, ISessionModel sessionModel)
		{
			Info.Entity.Description = $"{Step3TestCaption}{ablationId}{OfText}{_ablationCount}{Ablations}{VerifyReadyStateMessage}";
#if DEBUG
			Task.Delay(TimeSpan.FromSeconds(1)).ContinueWith(_sensorParameters => _machineModel.SystemState = CAN_ID_STATE_READY);
#endif
			// TODO:: Might have issue here if user disconnects the catheter before this point.
			// TODO:: Might need to verify catheter before turning on vacuum (FUTURE IMPROVEMENT)
			using(var consoleSwitchState = new AutoResetEvent(false))
			using(_systemStateSubject.Subscribe(state_ =>
					 {
						 if(state_ == CAN_ID_STATE_READY ||
								state_ == CAN_ID_STATE_EXCEPTION ||
								_sessionModel.Status == Stopped)
						 {
							 consoleSwitchState.Set();
						 }
					 }))
			using(_sessionStatusSubject.Subscribe(status_ =>
			{
				if(status_ == Stopped)
					_systemStateSubject.OnNext(_machineModel.SystemState);
			}))
			{
				SetVacuum(true);
				consoleSwitchState.WaitOne(FromSeconds(TimeoutForIdleToReadySwitchInSecond));
			}

			if(_machineModel.SystemState == CAN_ID_STATE_READY)
			{
				ProcessInReadyState(ablationId, cancellationToken, sessionModel);
			}
			else
			{
				// Rare case that system won't switch to READY state
				// TODO:: prompt for error 
				Info.Status = TestStatus.Aborted;
				var msg_ = $"{Step3TestCaption}{ablationId}{OfText}{_ablationCount}{Ablations}{IdleToReadySwitchErrorMessage}";
				Info.Entity.Description = msg_;
				_eventAggregator.GetEvent<TimeOutEvent>().Publish(msg_);
				FieldServiceTrace.Log(Info.Entity.Description);
			}
		}

		private void ProcessInReadyState(int ablationId, CancellationToken cancellationToken, ISessionModel sessionModel)
		{
#if DEBUG
			_machineModel.CP2Reading = -13.4;
#endif
			var obpValues_ = new List<double>();
			var stopwatch_ = new Stopwatch();
			stopwatch_.Start();
			while(stopwatch_.Elapsed.TotalSeconds < SamplingPeriodInReadyState)
			{
				WaitFor(SensorSamplingIntervalForReadyInSecond);
				obpValues_.Add(_machineModel.CP2Reading);
			}
			stopwatch_.Stop();
			_ObpInReady = Math.Round(obpValues_.Max(), RoundTwoDigits);
			TransitionFromReadyStateToInflationState(ablationId, cancellationToken, sessionModel);
		}

		private void TransitionFromReadyStateToInflationState(int ablationId, CancellationToken cancellationToken, ISessionModel sessionModel)
		{
			var isInInflationState_ = false;
			Info.Entity.Description = $"{Step3TestCaption}{ablationId}{OfText}{_ablationCount}{Ablations}{MessageFromReadyToInflation}";
#if DEBUG
			_machineModel.CP1Reading = -4.3;
			_machineModel.CP2Reading = -13.5;
			Task.Delay(TimeSpan.FromSeconds(0.5)).ContinueWith(_ => _machineModel.SystemState = CAN_ID_STATE_INFLATION);

			Observable.Interval(TimeSpan.FromMilliseconds(100))
				.Take(10)
				.Subscribe(_ =>
				{
					if(_machineModel.CP1Reading <= 2.8)
						_machineModel.CP1Reading += 1;
				},
				() => _machineModel.CP1Reading = 2.5);
#endif
			VerifyInflationState_();

			if(isInInflationState_)
			{
				ProcessInInflationState(ablationId, cancellationToken, sessionModel);
			}
			else
			{
				var msg_ = $"{Step3TestCaption}{ablationId}{OfText}{_ablationCount}{Ablations}{ReadyToInflationSwitchErrorMessage}";
				TestOnError(msg_, ablationId);
				_machineModel.Console.Stop();
				_eventAggregator.GetEvent<TimeOutEvent>().Publish(msg_);
				FieldServiceTrace.Log(msg_);
			}

			return;

			void VerifyInflationState_()
			{
				using(var consoleSwitchState = new AutoResetEvent(false))
				using(_systemStateSubject.Subscribe(state_ =>
							{
								if(state_ == CAN_ID_STATE_INFLATION || state_ == CAN_ID_STATE_EXCEPTION ||
									 Info.Status == TestStatus.Aborted || sessionModel.Status == Stopped)
								{
									consoleSwitchState.Set();
								}
							}))
				using(_sessionStatusSubject.Subscribe(status_ =>
			 {
				 if(status_ == Stopped) _systemStateSubject.OnNext(_machineModel.SystemState);
			 }))
				{
					_machineModel.Console.Start();
					isInInflationState_ = consoleSwitchState.WaitOne(FromSeconds(TimeoutReadyToInflationInSecond));
					isInInflationState_ &= _machineModel.SystemState == CAN_ID_STATE_INFLATION;
				}
			}
		}

		private void ProcessInInflationState(int ablationId, CancellationToken cancellationToken, ISessionModel sessionModel)
		{
			StartSampleAblationDetails_();
			StartRecordingData_();
      EnableDASBalloonIfNeeded_();

      Inflate_();

			if(cancellationToken.IsCancellationRequested || Info.Status == TestStatus.Aborted)
			{
				FinishSingleAblationTest(ablationId);
				return;
			}

			TransitionFromInflationToAblation(ablationId, cancellationToken, sessionModel);
			return;

			void StartSampleAblationDetails_()
			{
				Run(() => DataManagement.SampleAblationDetails(ablationId, Configuration, _samplingAblationDetailsCancellationToken));
			}
			void StartRecordingData_()
			{
				Run(() => DataManagement.RecordInflationData(ablationId, Configuration, _samplingSensorDataCancellationToken, Configuration.InflationTimeInSecond));
			}

      void EnableDASBalloonIfNeeded_()
      {
        if (Configuration.EnableDASBalloon)
        {
          Run(() => WaitAndEnableDASBalloon(Configuration.InflationTimeInSecond, cancellationToken));
        }
      }

			void Inflate_()
			{
				var inflationTime_ = Configuration.InflationTimeInSecond;
				StartCount(ablationId, CAN_ID_STATE_INFLATION, inflationTime_, InflationTimeText, MessageInInflationState);
				CountdownWithCancellationInSecond(inflationTime_, cancellationToken);
			}
		}

		private void TransitionFromInflationToAblation(int ablationId, CancellationToken cancellationToken, ISessionModel sessionModel)
		{
			Info.Entity.Description = $"{Step3TestCaption}{ablationId}{OfText}{_ablationCount}{Ablations}{MessageFromInflationToAblation}";
			var isInAblationState_ = false;
#if DEBUG
			Task.Delay(TimeSpan.FromSeconds(0.1)).ContinueWith(_ => _machineModel.SystemState = CAN_ID_STATE_ABLATION);
			isInAblationState_ = true;
#endif
			using(var consoleSwitchState = new AutoResetEvent(false))
			using(_systemStateSubject.Subscribe(state_ =>
		{
			if(state_ == CAN_ID_STATE_TRANSITION || state_ == CAN_ID_STATE_ABLATION || state_ == CAN_ID_STATE_EXCEPTION)
			{
				consoleSwitchState.Set();
			}
		}))
			using(_sessionStatusSubject.Subscribe(status_ =>
		 {
			 if(status_ == Stopped) _systemStateSubject.OnNext(_machineModel.SystemState);
		 }))
			{
				_machineModel.Console.Start();
				isInAblationState_ = consoleSwitchState.WaitOne(FromSeconds(TimeoutSwitchToAblationStateInSecond));
				isInAblationState_ &= (_machineModel.SystemState == CAN_ID_STATE_TRANSITION || _machineModel.SystemState == CAN_ID_STATE_ABLATION);
				ResetDataCancellationSRC();
				StartProcessingInflationData_();
			}
			
			if(isInAblationState_)
			{
				ProcessInAblationState(ablationId, cancellationToken, sessionModel);
			}
			else
			{
				var msg_ = $"{Step3TestCaption}{ablationId}{OfText}{_ablationCount}{Ablations}{TimeoutForSwitchingToAblationMessage}";
				TestOnError(msg_, ablationId);
				_eventAggregator.GetEvent<TimeOutEvent>().Publish(msg_);
				FinishSingleAblationTest(ablationId);
			}

			void StartProcessingInflationData_()
			{
				_inflationDataAnalysisTask = Run(() => DataManagement.ProcessInflationData(ablationId, Configuration, _ObpInReady));
				_dataAnalysisTasks.Add(_inflationDataAnalysisTask);
			}
		}

		private void ProcessInAblationState(int ablationId, CancellationToken cancellationToken, ISessionModel sessionModel)
		{
			InitiateSensorDataRecordingForAblation();

			if(_needToPerformFlowMeterCheck)
			{
				_flowMeterDataManager?.StartCollectingData();
			}

			var ablationTime_ = Configuration.AblationTimeInSecond;
			StartCount(ablationId, CAN_ID_STATE_ABLATION, ablationTime_, AblationTimeText, MessageInAblationState);
			CountdownWithCancellationInSecond(ablationTime_, cancellationToken);

			// Stop collecting Flow Meter data 
			_flowMeterDataManager?.StopCollectingData();
			if(cancellationToken.IsCancellationRequested || Info.Status == TestStatus.Aborted)
			{
				FinishSingleAblationTest(ablationId);
				return;
			}

			TransitionFromAblationStateToThawingState(ablationId, cancellationToken, sessionModel);

			return;

			void InitiateSensorDataRecordingForAblation()
			{
				Run(() => DataManagement.RecordAblationData(ablationId, _samplingSensorDataCancellationToken, Configuration.AblationTimeInSecond));
			}
		}

		private void TransitionFromAblationStateToThawingState(int ablationId, CancellationToken cancellationToken, ISessionModel sessionModel)
		{
			Info.Entity.Description = $"{Step3TestCaption}{ablationId}{OfText}{_ablationCount}{Ablations}{MessageFromAblationToThawing}";
#if DEBUG
			Task.Delay(TimeSpan.FromSeconds(0.5)).ContinueWith(_ => _machineModel.SystemState = CAN_ID_STATE_THAWING);
#endif
			var isInThawingState_ = false;
			using(var consoleSwitchState = new AutoResetEvent(false))
			using(_systemStateSubject.Subscribe(state_ =>
		 {
			 if(state_ == CAN_ID_STATE_THAWING || state_ == CAN_ID_STATE_EXCEPTION ||
					Info.Status == TestStatus.Aborted || sessionModel.Status == Stopped)
			 {
				 consoleSwitchState.Set();
			 }
		 }))
			using(_sessionStatusSubject.Subscribe(status_ =>
		 {
			 if(status_ == Stopped) _systemStateSubject.OnNext(_machineModel.SystemState);
		 }))
			{
				_machineModel.Console.Stop();
				isInThawingState_ = consoleSwitchState.WaitOne(FromSeconds(TimeoutForAblationToThawingInSeconds));
				isInThawingState_ &= _machineModel.SystemState == CAN_ID_STATE_THAWING;
			}

			if(isInThawingState_)
			{
				ProcessInThawingState(ablationId, cancellationToken, sessionModel);
			}
			else
			{
				var msg_ = $"{Step3TestCaption}{ablationId}{OfText}{_ablationCount}{Ablations}{TimeoutForSwitchingToThawingMessage}";
				TestOnError(msg_, ablationId);
				_eventAggregator.GetEvent<TimeOutEvent>().Publish(msg_);
				FinishSingleAblationTest(ablationId);
			}
		}

		private void ProcessInThawingState(int ablationId, CancellationToken cancellationToken, ISessionModel sessionModel)
		{
			ResetDataCancellationSRC();
			_ablationDataAnalysisTask = Run(() => DataManagement.ProcessAblationData(ablationId, Configuration, _ibpAblationValidator, _pwm2AblationValidator, _ObpInReady));
			_dataAnalysisTasks.Add(_ablationDataAnalysisTask);

			Run(() => DataManagement.RecordThawingData(ablationId, _samplingSensorDataCancellationToken, Configuration.ThawingTimeInSecond));
			Info.Entity.Description = $"{Step3TestCaption}{ablationId}{OfText}{_ablationCount}{Ablations}{MessageInThawingState}";
#if DEBUG
			Observable.Interval(TimeSpan.FromSeconds(1)).Take(15).Subscribe(_ =>
			{
				_machineModel.TC1Reading += 8;
			},
				() => _machineModel.SystemState = CAN_ID_STATE_IDLE);
#endif
			// Notes: _machineModel.Console.DeflateAfterThaw = true was set,
			// so, we should just need to wait for the State change (Ready/Idle) when temperature reaches 20C
			using(var consoleSwitchState = new AutoResetEvent(false))
			using(_systemStateSubject.Subscribe(state_ =>
						 {
							 if(state_ == CAN_ID_STATE_READY ||
									 state_ == CAN_ID_STATE_IDLE ||
									 state_ == CAN_ID_STATE_EXCEPTION ||
									 sessionModel.Status == Stopped)
							 {
								 consoleSwitchState.Set();
							 }
						 }))
			using(_sessionStatusSubject.Subscribe(status_ =>
		 {
			 if(status_ == Stopped) _systemStateSubject.OnNext(_machineModel.SystemState);
		 }))
			{
				var stateUpdated = consoleSwitchState.WaitOne(FromSeconds(TimeoutToFinishThawingStateInSecond));
				// Stop Thawing if timeout and state is till in thawing
				if(!stateUpdated && _machineModel.SystemState == CAN_ID_STATE_THAWING)
					_machineModel.Console.Stop();

				// Turn on Vacuum to flush cold air 
				Task.Delay(TimeSpan.FromSeconds(1)).ContinueWith(_ =>
				{
					if(_machineModel.SystemState == CAN_ID_STATE_IDLE)
						SetVacuum(true);
				});
			}

			ResetDataCancellationSRC();

			_thawingDataAnalysisTask = Run(() => DataManagement.ProcessThawingData(ablationId));
			_dataAnalysisTasks.Add(_thawingDataAnalysisTask);

			Info.Entity.Description = $"{Step3TestCaption}{ablationId}{OfText}{_ablationCount}{Ablations}{MessageInReadyState}";

			FinishSingleAblationTest(ablationId);
		}

		private void FinishSingleAblationTest(int ablationId)
		{
			ResetDataCancellationSRC();
			StopSampleAblationDetails_();

			DataManagement.SetAblationSummary(ablationId);
			Info.Entity.Description = $"{Step3TestCaption}{ablationId}{OfText}{_ablationCount}{Ablations}{ProcessingDataAnalysis}";

			if(_dataAnalysisTasks.Count > 0)
			{
				try
				{
					WaitAll(_dataAnalysisTasks.ToArray(), FromSeconds(TimeoutForDataAnalysisTaskInSecond));
				}
				catch(AggregateException ae)
				{
					TestOnError($"{ae.Message}", ablationId);
				}

				Info.Entity.Description = $"{Step3TestCaption}{ablationId}{OfText}{_ablationCount}{Ablations}{FinishedDataAnalysis}";
			}

			void StopSampleAblationDetails_()
			{
				ResetSamplingDetails();
			}
		}

		#endregion State Machine

		#region Helpers

		private void StartCount(int id, MessageStateId state, int time, string stateText, string message)
		{
			Run(() =>
			{
				var msg_ = $"{Step3TestCaption}{id}{OfText}{_ablationCount}{Ablations}{message}{stateText}{time}{TimeSecondText}{Tab}{Tab}{Tab}";
				var start_ = 1;
				using(Interval(FromSeconds(1))
								.Subscribe(x =>
								{
									Info.Entity.Description = msg_ + start_++;
								}))
				{
					SpinWait.SpinUntil(() =>
					{
						if (state == CAN_ID_STATE_INFLATION)
						{
							return _machineModel.SystemState != CAN_ID_STATE_INFLATION;
						}
						return _machineModel.SystemState != CAN_ID_STATE_ABLATION && _machineModel.SystemState != CAN_ID_STATE_TRANSITION;
					});
				}
			});
		}

		private void TestOnError(string errorMsg, int ablationId)
		{
			if(_sessionModel == null ||
				 _sessionModel?.Status == Pausing ||
				 _sessionModel?.Status == Paused ||
				 _sessionModel?.Status == Stopped)
			{
				return;
			}

			//Clark TODO:: Should find a way to stop the current ablation properly

			Info.Entity.Description = $"{Step3TestCaption}{errorMsg}";
			_eventAggregator.GetEvent<UserCommandEvent>().Publish((StopTest, Now));
			Info.Status = TestStatus.Aborted;
		}

		private void InitializeConsole()
		{
			SetVacuum(false);
		}

		private void InitializeConfiguration()
		{
			Info.Entity.Description = $"{Step3TestCaption}{AblationTestTitle}{InitializingConfigurationMessage}";
			try
			{
#if DEBUG
				var ablationConfig = XElement.Load(AblationConfigurationFileInDebug);
#else
				var ablationConfig = XElement.Load(AblationConfigurationFile);
#endif
				foreach(var ablation in ablationConfig.Descendants(AblationName))
				{
					_ablationConfigurationDictionary.Add(
						int.Parse(ablation.Attribute(Id).Value),
						new AblationConfiguration
						{
							IsFastInflation = Convert.ToBoolean(ablation.Elements(nameof(Configuration.IsFastInflation)).First().Value),
							InflationTimeInSecond = int.Parse(ablation.Elements(nameof(Configuration.InflationTimeInSecond)).First().Value),
							InflationRecordingIntervalMillisecond = int.Parse(ablation.Elements(nameof(Configuration.InflationRecordingIntervalMillisecond)).First().Value),
							AblationTimeInSecond = int.Parse(ablation.Elements(nameof(Configuration.AblationTimeInSecond)).First().Value),
							ThawingTimeInSecond = int.Parse(ablation.Elements(nameof(Configuration.ThawingTimeInSecond)).First().Value),
							EnableDASBalloon = Convert.ToBoolean(ablation.Elements(nameof(Configuration.EnableDASBalloon)).First().Value)
            });
				}
			}
			catch(IOException ioe)
			{
				FieldServiceTrace.LogException(ioe);
				Info.Entity.Description = $"{Step3TestCaption}{AblationTestTitle}{InitializingConfigurationIOExceptionMessage}";
			}
			catch(XmlException xe)
			{
				FieldServiceTrace.LogException(xe);
				Info.Entity.Description = $"{Step3TestCaption}{AblationTestTitle}{InitializingConfigurationParsingExceptionMessage}";
			}
			Info.Entity.Description = $"{Step3TestCaption}{AblationTestTitle}{ConfigurationInitializedMessage}";
		}

		private void InitializeDataManagement()
		{
			DataManagement.InitializeDataManagement(Result);
		}

		private void ResetDataCancellationSRC()
		{
			_samplingSensorDataCancellationTokenSource.Cancel();

			WaitFor(0.2);
			if(_samplingSensorDataCancellationToken.IsCancellationRequested)
			{
				_samplingSensorDataCancellationTokenSource.Dispose();
				_samplingSensorDataCancellationTokenSource = new CancellationTokenSource();
				_samplingSensorDataCancellationToken = _samplingSensorDataCancellationTokenSource.Token;
			}
		}

		private void ResetSamplingDetails()
		{
			_samplingAblationDetailsCancellationTokenSource.Cancel();

			WaitFor();
			if(_samplingAblationDetailsCancellationToken.IsCancellationRequested)
			{
				_samplingAblationDetailsCancellationTokenSource.Dispose();
				_samplingAblationDetailsCancellationTokenSource = new CancellationTokenSource();
				_samplingAblationDetailsCancellationToken = _samplingAblationDetailsCancellationTokenSource.Token;
			}
		}

		private void SetVacuum(bool enable)
		{
			if(enable)
			{
				_machineModel.Console.Connect();
			}
			else
			{
				_machineModel.Console.Disconnect();
			}
			_machineModel.IsVacuumDisconnected = !enable;
		}

		private void StopStateMachine()
		{
			_machineModel.Console.Stop();
			SetVacuum(false);
#if DEBUG
			// Console should be in Idle state when Vacuum is off
			_machineModel.SystemState = CAN_ID_STATE_IDLE;
#endif
		}

		private void DisplayExceptionMessage(Exception e)
		{
			Info.Entity.Description = $"{Step3TestCaption}{AblationTestTitle}{e.Message}";
		}

		#endregion Helpers

		#region FlowMeter Check Methods

		private void PromptToInstallFlowMeterOrSkip()
		{
			// prompt to ask user if they want to Skip the flow meter check
			// "Do you want to skip the Flow Meter Check?" => "Yes"/"No"  
			var flowMeterCheckParams = new DialogParameters
			{
				{ DialogTitleKey, FlowMeterCheckMessageTitle },
				{ DialogMessageKey, SkipFlowMeterCheckMessage }
			};

			DisplayDialog(nameof(Infrastructure.Controls.Dialog), flowMeterCheckParams, SkipFlowMeterCheckCallback_);

			if(FlowMeterCheckEnabled)
			{
				// pause the test and and wait for installing external flow meter 
				_eventAggregator.GetEvent<UserCommandEvent>().Publish((HoldException, DateTime.Now));

				// Prompt message to ask user connect the External Flow Meter 
				// "Please connect the External Flow Meter to the console." ==> "Ok"
				var installParameters = new DialogParameters
				{
					{ MessageDialogTypeKey, MessageDialogType.Information },
					{ DialogTitleKey, FlowMeterCheckMessageTitle },
					{ DialogMessageKey, ConnectFlowMeterMessage }
				};

				DisplayDialog(nameof(MessageDialog), installParameters);

				// resume the test
				_eventAggregator.GetEvent<UserCommandEvent>().Publish((ResetHoldException, DateTime.Now));
			}

			void SkipFlowMeterCheckCallback_(IDialogResult dialogResult)
			{
				switch(dialogResult.Result)
				{
					case ButtonResult.Yes:
						FlowMeterCheckEnabled = false;
						UpdateSkipFlowMeterCheckRationale();
						break;
					case ButtonResult.No:
						FlowMeterCheckEnabled = true;
						break;
					default:
						FlowMeterCheckEnabled = true;
						break;
				}
			}
		}

		private void UpdateSkipFlowMeterCheckRationale()
		{
			var parameters = new DialogParameters
			{
				{ DialogTitleKey, FlowMeterSkipRationaleTitle },
				{ DialogMessageKey, string.Empty },
				{ ParamIdKey, SkipFlowMeterCheckId },
				{ MaxTextLengthInTextBoxKey, 500}
			};

			DisplayDialog(nameof(RationaleDialog), parameters);
			//UPDATE TEST RESULT FOR SKIP 
			DataManagement.RecordFlowMeterCheckResult(1, (Double.NaN, null, null));
		}

		private void InitializeFlowMeterDataManagement()
		{
			// Resolve data manager
			_flowMeterDataManager = _containerProvider.Resolve<IFlowMeterDataManager>();

			DetectAndConnectToFlowMeter();
		}

		private void DisposeFlowMeterDataManager()
		{
			_flowMeterDataManager?.CloseConnection();
			_needToPerformFlowMeterCheck = false;
		}

		private void ValidateFlowMeterResult(FlowMeterValidationResult result)
		{
			// Pause test and wait to replace/disconnect the flow meter, in case it would trigger console exception
			_eventAggregator.GetEvent<UserCommandEvent>().Publish((HoldException, DateTime.Now));

			if(!result?.IsValid ?? false)
			{
				var parameters = new DialogParameters
				{
					{MessageDialogTypeKey, MessageDialogType.Information},
					{ DialogTitleKey, FlowMeterCheckMessageTitle },
					{ DialogMessageKey, FlowMeterTestExceedToleranceMessage }
				};

				DisplayDialog(nameof(MessageDialog), parameters);

				// Publish Flow Meter changed RetryRationale event; 
				var avgOffset = Math.Round((double)(result?.AverageOffset * 100d), 2).ToString();
				var flowMeterChangedMessage = FlowMeterChangedSummaryMessage.Replace(AVG_OFFSET_KEY, avgOffset);
				_eventAggregator.GetEvent<RetryRationaleEvent>().Publish((FlowMeterCheckMessageTitle, flowMeterChangedMessage));

				// Create TestResult with Actual Result = N/A, and Status = Skipped
				DataManagement.RecordFlowMeterCheckResult(1, (Double.NaN, null, null));
			}
			else
			{
				// The validate is passed, need to record it to report. 
				// Create TestResult with Actual Result = AverageOffset, and Status = Pass
				var actualValue = result?.AverageOffset != null
					? Math.Round((double)(result?.AverageOffset * 100d), 2)
					: Double.NaN;

				DataManagement.RecordFlowMeterCheckResult(1, (actualValue, true, null));

				// Popup message box to allow user uninstall the external flow meter
				var parameters = new DialogParameters
				{
					{ MessageDialogTypeKey, MessageDialogType.Information },
					{ DialogTitleKey, FlowMeterCheckMessageTitle },
					{ DialogMessageKey, DisconnectFlowMeterMessage}
				};

				DisplayDialog(nameof(MessageDialog), parameters);
			}

			// Resume the test
			_eventAggregator.GetEvent<UserCommandEvent>().Publish((ResetHoldException, DateTime.Now));
		}

		private void DetectAndConnectToFlowMeter()
		{
			while(!_flowMeterDataManager.ConnectToFlowMeter())
			{
				// Close the connection and retry
				_flowMeterDataManager.CloseConnection();
				// If failed to detect the device, prompt to ask user to check the connection and continue  
				RetryOrSkipFlowMeterCheck(CouldNotDetectFlowMeterMessage);
				// Break if skip
				if(!_needToPerformFlowMeterCheck)
					break;
			}
		}

		private void RetryOrSkipFlowMeterCheck(string retryMessage)
		{
			// set _needToCheckFlowMeter = false, if user select Skip 
			var parameters = new DialogParameters
			{
				{ DialogTitleKey, FlowMeterCheckMessageTitle },
				{ DialogMessageKey,  retryMessage }
			};

			DisplayDialog(nameof(Infrastructure.Controls.Dialog), parameters, RetryOrSkipCallback_);

			void RetryOrSkipCallback_(IDialogResult dialogResult)
			{
				switch(dialogResult.Result)
				{
					case ButtonResult.No:
						FlowMeterCheckEnabled = false;
						_needToPerformFlowMeterCheck = false;
						// Display Rationale dialog
						UpdateSkipFlowMeterCheckRationale();
						break;
				}
			}
		}

		private void DisplayDialog(string dialog, DialogParameters parameters, Action<IDialogResult> callBack = null)
		{
			Application.Current.Dispatcher.Invoke(() =>
			{
				_dialogService.ShowDialog(dialog, parameters, callBack);
			});
		}
		#endregion FlowMeter Check Methods

		private void DisplayWaitForBalloonTemperatureDialog(double watertemperature)
		{
			if (watertemperature >= TC1LowThresholdForInflationSpeedMode && watertemperature <= TC1HighThresholdForInflationSpeedMode)
      {
				return;
      }
			string WaterTemperatureMessage = watertemperature > TC1HighThresholdForInflationSpeedMode ? WaterTemperatureTooHighMessage : WaterTemperatureTooLowMessage;
			var balloonTempCheckParams = new DialogParameters
			{
				{ DialogTitleKey,  "Ablation Test" },
				{ DialogMessageKey,  WaterTemperatureMessage + ContinueOrStopMessage },
				{ DialogYesButtonTextKey, OKText },
				{ DialogNoButtonTextKey, StopText }
			};

			DisplayDialog(nameof(Infrastructure.Controls.Dialog), balloonTempCheckParams, BalloonTempCheckCallback_);

			void BalloonTempCheckCallback_(IDialogResult dialogResult)
			{
				switch(dialogResult.Result)
				{
					case ButtonResult.No:
						Info.Status = TestStatus.Aborted;
						_eventAggregator.GetEvent<UserCommandEvent>().Publish((StopTest, Now));
						break;
				}
			}
		}

		private void CheckBalloonTemperature(ISessionModel sessionModel, double waitingPeriodInSeconds)
		{
			var waitingTimer = new Stopwatch();
			long waitingPeriodInMs = (long)(waitingPeriodInSeconds * 1000d);

			waitingTimer.Start();

			while((_sensorParameters.Temperature < TC1LowThresholdForInflationSpeedMode || _sensorParameters.Temperature > TC1HighThresholdForInflationSpeedMode) 
				&& Info.Status != TestStatus.Aborted && sessionModel.Status != Stopped)
			{
				double temperature = _sensorParameters.Temperature;
				if(waitingTimer.ElapsedMilliseconds <= waitingPeriodInMs)
				{
					WaitFor();
					continue;
				}

				if(waitingTimer.IsRunning) waitingTimer.Stop();

				if(!CheckAndUpdateSessionStatus(sessionModel))
				{
					Info.Status = TestStatus.Aborted;
					break;
				}

				DisplayWaitForBalloonTemperatureDialog(temperature);

				WaitFor(2.0);
#if DEBUG
				_sensorParameters.Temperature = 37.5;
#endif
			}
		}

		private bool CheckAndUpdateSessionStatus(ISessionModel sessionModel)
		{
			using(var _waitSignalEvent = new ManualResetEvent(false))
			using(_sessionStatusSubject.Subscribe(status_ =>
						{
							if(status_ != Paused && status_ != Pausing)
							{
								var pause_ = _waitSignalEvent.Set();
							}
						}))
			{
				if(sessionModel.Status == Pausing)
				{
					sessionModel.Status = Paused;
					_eventAggregator.GetEvent<SessionStatusEvent>().Publish((sessionModel.Status, Now));
				}

				var pause_ = _waitSignalEvent.WaitOne();
			}

			return sessionModel.Status != Stopped;
		}

    private void WaitAndEnableDASBalloon(int timeoutInSec, CancellationToken cancellationToken)
    {
      using (var inflated = new ManualResetEventSlim(false))
      using (FromEventPattern<PropertyChangedEventArgs>(_machineModel, nameof(_machineModel.PropertyChanged))
               .Where(arg => arg.EventArgs.PropertyName == nameof(_machineModel.CP1Reading))
               .Select(_ => _machineModel.CP1Reading)
               .Subscribe(ibp =>
               {
                 if (ibp >= IBPTargetForSwitchToDASBalloon)
                 {
                   inflated.Set();
                 }
               }))
      {
        if (inflated.Wait(TimeSpan.FromSeconds(timeoutInSec), cancellationToken))
        {
          EnableDASBalloon();
        }
      }
    }

    private async void EnableDASBalloon(bool enable = true)
    {
			await _machineModel.SendBalloonPressureSetPointAsync(enable);
    }
  }
}