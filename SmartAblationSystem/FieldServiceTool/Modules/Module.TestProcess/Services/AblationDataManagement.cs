using Module.Console.Interfaces;
using Module.Infrastructure.Constants;
using Module.Infrastructure.TestResults.Implementation;
using Module.Infrastructure.TestResults.Interfaces;
using Module.SystemParameters.Interfaces;
using Module.TestProcess.Helpers;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using Unity;
using static System.DateTime;
using static System.Double;
using static Communication.CanBusMessageDefinition;
using static Communication.CanBusMessageDefinition.MessageStateId;
using static Module.Infrastructure.Constants.Strings;
using static Module.Infrastructure.Helpers.ThreadHelpers;
using static Module.TestProcess.Services.ServiceConstants;
using static System.Reactive.Linq.Observable;
using static System.TimeSpan;

namespace Module.TestProcess.Services
{
  public class AblationDataManagement : IAblationDataManagement
  {
    private IAblationTestResult _ablationTestResult;
    private List<IAblationDetails> _ablationDetailList;
    private readonly IMachineModel _machineModel;
    private readonly ISensorParameters _sensorParameters;
    private readonly ISubject<double> _ibpSubject;
    private readonly ISubject<MessageStateId> _systemStateSubject;
    private readonly IContainerProvider _containerProvider;

    private readonly ISubject<AblationDetails> _ablationDetailSubject = new BehaviorSubject<AblationDetails>(new AblationDetails() { State = AblationTestState.UNKNOWN });

    public AblationDataManagement(
      IUnityContainer container,
      IMachineModel machineModel,
      ISensorParameters sensorParameters,
      List<IAblationDetails> ablationDetailList,
      IContainerProvider containerProvider)
    {
      _machineModel = machineModel;
      _sensorParameters = sensorParameters;
      _ablationDetailList = ablationDetailList;
      _containerProvider = containerProvider;
      _ibpSubject = new BehaviorSubject<double>(_sensorParameters.IBP);
      _systemStateSubject = new BehaviorSubject<MessageStateId>(CAN_ID_STATE_UNKNOWN);

      FromEventPattern<PropertyChangedEventArgs>(_sensorParameters, nameof(_sensorParameters.PropertyChanged))
        .Where(arg => arg.EventArgs.PropertyName == nameof(_sensorParameters.IBP))
        .Subscribe(_ => _ibpSubject?.OnNext(_sensorParameters.IBP));

      FromEventPattern<PropertyChangedEventArgs>(_machineModel, nameof(_machineModel.PropertyChanged))
        .Where(arg => arg.EventArgs.PropertyName == nameof(_machineModel.SystemState))
        .Subscribe(_ => _systemStateSubject?.OnNext(_machineModel.SystemState));
    }

    public void RecordInflationData(int id, IAblationConfiguration ablationConfiguration, CancellationToken cancellationToken, int inflationTime = 0)
    {
      var sensorDataDictionaryById_ = _ablationTestResult.GetSensorDataDictionaryById(id);
#if DEBUG
      var rnd_ = new Random();
      var ibpInitialValue_ = -14.0d;
      while(!cancellationToken.IsCancellationRequested && _machineModel.SystemState == CAN_ID_STATE_INFLATION)
      {
        sensorDataDictionaryById_[TestParameter.Inflation_IBP].Add(ibpInitialValue_ + 0.3d);
        sensorDataDictionaryById_[TestParameter.Inflation_PT2].Add(Math.Round(rnd_.Next(120, 180) * 1.1d, RoundOneDigit));
        sensorDataDictionaryById_[TestParameter.Inflation_FM1].Add(Math.Round(rnd_.Next(600, 1200) * 1.1d, RoundOneDigit));
        if(id == 1)
        {
          sensorDataDictionaryById_[TestParameter.Inflation_OBP].Add(-13.5d);
        }
        WaitFor(ablationConfiguration.InflationRecordingIntervalMillisecond/1000.0);
      }
#else
			using(_ablationDetailSubject.Subscribe(data =>
						{
							if(data.State != AblationTestState.INFLATION) return;

							sensorDataDictionaryById_[TestParameter.Inflation_IBP].Add(data.IBP);
							sensorDataDictionaryById_[TestParameter.Inflation_PT2].Add(data.PT2);
							sensorDataDictionaryById_[TestParameter.Inflation_FM1].Add(data.FM1);
							if(id == 1)
							{
								sensorDataDictionaryById_[TestParameter.Inflation_OBP].Add(data.OBP);
							}
						}))
			{
				while(!cancellationToken.IsCancellationRequested)
				{
					WaitFor(0.1);
				}
			}
#endif
    }

    public void RecordAblationData(int id, CancellationToken cancellationToken, int ablationTime)
    {
      var sensorDataDictionaryById_ = _ablationTestResult.GetSensorDataDictionaryById(id);

      var stopwatch_ = new Stopwatch();
      stopwatch_.Start();
#if DEBUG
      var rnd_ = new Random();
      while(!cancellationToken.IsCancellationRequested && 
            (_machineModel.SystemState == CAN_ID_STATE_ABLATION || _machineModel.SystemState == CAN_ID_STATE_TRANSITION))
      {
        sensorDataDictionaryById_[TestParameter.Ablation_FM1].Add(Math.Round(rnd_.Next(7500, 8100) * 1.0d, RoundOneDigit));
        sensorDataDictionaryById_[TestParameter.Ablation_PT2].Add(Math.Round(rnd_.Next(400, 650) * 1.0d, RoundOneDigit));
        sensorDataDictionaryById_[TestParameter.Ablation_IBP].Add(Math.Round(rnd_.Next(2, 3) * 2.0d, RoundOneDigit));
        sensorDataDictionaryById_[TestParameter.Ablation_OBP].Add(-13.5d);
        sensorDataDictionaryById_[TestParameter.Ablation_TC1].Add(_machineModel.TC1Reading);
        sensorDataDictionaryById_[TestParameter.Ablation_PWM1].Add(Math.Round(rnd_.Next(25, 35) * 1.1d, RoundOneDigit));
        sensorDataDictionaryById_[TestParameter.Ablation_PWM2].Add(Math.Round(rnd_.Next(55, 65) * 1.0d, RoundOneDigit));
        sensorDataDictionaryById_[TestParameter.Ablation_PT3].Add(Math.Round(rnd_.Next(8, 11) * 1.1, RoundOneDigit));
        sensorDataDictionaryById_[TestParameter.Ablation_PT4].Add(Math.Round(rnd_.Next(3, 4) * 1.1d, RoundOneDigit));
        sensorDataDictionaryById_[TestParameter.Ablation_PT5].Add(Math.Round(rnd_.Next(14, 16) * 1.1d, RoundOneDigit));
        sensorDataDictionaryById_[TestParameter.Ablation_TS1].Add(Math.Round(rnd_.Next(-40, -10) * 1.0d, RoundOneDigit));

        sensorDataDictionaryById_[TestParameter.Transition_Time].Add(13.3);
        sensorDataDictionaryById_[TestParameter.FiftyDegree_Time].Add(11.2);
        WaitFor(0.1);
      }
#else
			using(_ablationDetailSubject.Subscribe(data =>
						{
							if(data.State != AblationTestState.ABLATION && data.State != AblationTestState.TRANSITION) return;

							if(data.State == AblationTestState.ABLATION)
							{
								sensorDataDictionaryById_[TestParameter.Ablation_FM1].Add(data.FM1);
								sensorDataDictionaryById_[TestParameter.Ablation_TS1].Add(data.TS1);
								sensorDataDictionaryById_[TestParameter.Ablation_PT3].Add(data.PT3);
								sensorDataDictionaryById_[TestParameter.Ablation_PT4].Add(data.PT4);
								sensorDataDictionaryById_[TestParameter.Ablation_PT5].Add(data.PT5);

								sensorDataDictionaryById_[TestParameter.Ablation_IBP].Add(data.IBP);
								sensorDataDictionaryById_[TestParameter.Ablation_OBP].Add(data.OBP);
							}
							sensorDataDictionaryById_[TestParameter.Ablation_PT2].Add(data.PT2);
							sensorDataDictionaryById_[TestParameter.Ablation_TC1].Add(data.TC1);
							sensorDataDictionaryById_[TestParameter.Ablation_PWM1].Add(data.IPWM);
							sensorDataDictionaryById_[TestParameter.Ablation_PWM2].Add(data.BPWM);
						}))
			{
				while(!cancellationToken.IsCancellationRequested)
				{
					WaitFor(0.1);
				}
			}
#endif
      stopwatch_.Stop();

      var timeInTransition_ = RetrieveTransitionTime();
      sensorDataDictionaryById_[TestParameter.Transition_Time].Add(timeInTransition_);

      var timeToMinus50_ = RetrieveTimeToMinus50();
      sensorDataDictionaryById_[TestParameter.FiftyDegree_Time].Add(timeToMinus50_);

      double RetrieveTransitionTime()
      {
        var timestamps = _ablationDetailList.Where(x => x.State == AblationTestState.TRANSITION).Select(x => x.Timestamp);
        return (timestamps.LastOrDefault() - timestamps.FirstOrDefault()).TotalSeconds;
      }

      double RetrieveTimeToMinus50()
      {
        if (_ablationDetailList.Any(x => x.State == AblationTestState.ABLATION && x.TC1 <= Minus50Celsius))
        {
          return (_ablationDetailList.First(x => x.State == AblationTestState.ABLATION && x.TC1 <= Minus50Celsius).Timestamp 
            - _ablationDetailList.FirstOrDefault(x => x.State == AblationTestState.TRANSITION).Timestamp).TotalSeconds;
        }
        return NaN;
      }
    }

    public void RecordThawingData(int id, CancellationToken cancellationToken, int thawingTime)
    {
      var sensorDataDictionaryById_ = _ablationTestResult.GetSensorDataDictionaryById(id);

      using(_ablationDetailSubject.Subscribe(data =>
      {
        if(data.State != AblationTestState.THAWING) return;
        sensorDataDictionaryById_[TestParameter.Thawing_PT3].Add(data.PT3);
        sensorDataDictionaryById_[TestParameter.Thawing_PT4].Add(data.PT4);
        sensorDataDictionaryById_[TestParameter.Thawing_PT5].Add(data.PT5);
        sensorDataDictionaryById_[TestParameter.Thawing_PWM1].Add(data.IPWM);
        sensorDataDictionaryById_[TestParameter.Thawing_PWM2].Add(data.BPWM);
      }))
      {
        while(!cancellationToken.IsCancellationRequested)
        {
          WaitFor(0.1);
        }
      }
    }

    public void SampleAblationDetails(int id, IAblationConfiguration ablationConfiguration, CancellationToken cancellationToken)
    {
      _ablationDetailList = new List<IAblationDetails>();
      var time_ = 1;
      var SensorSamplingIntervalForInflationMillisecond_ = ablationConfiguration.InflationRecordingIntervalMillisecond;
      while(!cancellationToken.IsCancellationRequested &&
            (_machineModel.SystemState == CAN_ID_STATE_INFLATION
             || _machineModel.SystemState == CAN_ID_STATE_ABLATION
             || _machineModel.SystemState == CAN_ID_STATE_THAWING
             || _machineModel.SystemState == CAN_ID_STATE_TRANSITION))
      {
				var ablationDetails = new AblationDetails
        {
          Timestamp = Now,
          Time = time_++,
          ID = id,
          State = ConvertStateId(_machineModel.SystemState),
          TC1 = _machineModel.TC1Reading,
          PT1 = _machineModel.PT1Reading,
          PT2 = _machineModel.PT2Reading,
          PT3 = _machineModel.PT3Reading,
          PT4 = _machineModel.PT4Reading,
          PT5 = _machineModel.PT5Reading,
          FM1 = _machineModel.FM1Reading,
          TS1 = _machineModel.TS1Reading,
          LC1 = _machineModel.LC1Reading,
          IBP = _machineModel.CP1Reading,
          OBP = _machineModel.CP2Reading,
          IPWM = _machineModel.PIDDutyCycle,
          BPWM = _machineModel.PatientPIDDutyCycle
        };

        _ablationDetailSubject.OnNext(ablationDetails);
        _ablationDetailList.Add(ablationDetails);

        SpinWait.SpinUntil(() => false, _machineModel.SystemState == CAN_ID_STATE_INFLATION
	        ? FromMilliseconds(SensorSamplingIntervalForInflationMillisecond_)
	        : FromMilliseconds(SensorSamplingIntervalForAblationMillisecond));
      }
      
      _ablationDetailList.RemoveAll(x => x.State == AblationTestState.UNKNOWN);
      _ablationTestResult.AblationDetailsList = _ablationDetailList;

      // Reset the _ablationDetailSubject after completing AblationDetails sampling,
      // to avoid having invalid Inflation data if error occurs in previous inflation state  
      _ablationDetailSubject.OnNext(new AblationDetails { State = AblationTestState.UNKNOWN });

      AblationTestState ConvertStateId(MessageStateId messageId)
      {
        var result_ = AblationTestState.UNKNOWN;
        switch(messageId)
        {
          case CAN_ID_STATE_INFLATION:
            result_ = AblationTestState.INFLATION;
            break;
          case CAN_ID_STATE_TRANSITION:
            result_ = AblationTestState.TRANSITION;
            break;
          case CAN_ID_STATE_ABLATION:
            result_ = AblationTestState.ABLATION;
            break;
          case CAN_ID_STATE_THAWING:
            result_ = AblationTestState.THAWING;
            break;
          case CAN_ID_STATE_UNKNOWN:
          case CAN_ID_STATE_IDLE:
          case CAN_ID_STATE_READY:
          case CAN_ID_STATE_EXCEPTION:
          default:
            break;
        }
        return result_;
      }
    }

    public void SetAblationSummary(int id)
    {
      var treatment_ = _containerProvider.Resolve<ITreatment>();
      treatment_.Id = id;
      treatment_.CatheterID = _machineModel.CatheterID;
      treatment_.CatheterLotNum = _machineModel.CatheterLot;
      treatment_.CatheterSN = _machineModel.CatheterSerialNumber;
      treatment_.InflationSpeed = _ablationTestResult.GetSensorDataDictionaryById(id)[TestParameter.Inflation_Speed].FirstOrDefault();
      _ablationTestResult.AblationSummaryList.Add(treatment_);
    }

    public void InitializeDataManagement(IAblationTestResult testResult)
    {
      _ablationTestResult = testResult;
    }

    public IAblationTestResult GetTestResult()
    {
      return _ablationTestResult;
    }

		public void ProcessInflationData(int id, IAblationConfiguration ablationConfiguration, double obpInReady)
    {
      var sensorDataDictionary_ = _ablationTestResult.GetSensorDataDictionaryById(id);
      var resultDictionary_ = _ablationTestResult.AblationResult[AblationTestState.INFLATION];

      var skipSeconds = ablationConfiguration.EnableDASBalloon ? SkipSecondsForDASBalloon : SkipSeconds;
      var firstTimestamp = _ablationDetailList.First(ab => ab.State == AblationTestState.INFLATION).Timestamp; 
      var skip_ = _ablationDetailList.Count(ab => (ab.Timestamp - firstTimestamp).TotalMilliseconds <= skipSeconds*1000);

      foreach(var parameter in resultDictionary_.Keys)
      {
        switch(parameter)
        {
          case TestParameter.Inflation_Speed:
            var startTimestamp_ = _ablationDetailList?
		          .FirstOrDefault()?
		          .Timestamp;
	          var endTimestamp_ = _ablationDetailList?
		          .First(x => x.State == AblationTestState.INFLATION && x.IBP >= InflationSpeedIBPThreshold)
		          .Timestamp;
	          var value_ = 0.0d;
	          if(!startTimestamp_.HasValue || !endTimestamp_.HasValue)
	          {
		          value_ = NaN;
	          }
	          else
	          {
              // Requested by test team:  covert the datetime to string and parse it,
              // to make sure getting exactly the same value from the stored data in Excel 
              var dt1 = DateTime.Parse(startTimestamp_.Value.ToString(TimestampFormatString));
              var dt2 = DateTime.Parse(endTimestamp_.Value.ToString(TimestampFormatString));

              value_ = Math.Round((dt2 - dt1).TotalSeconds, RoundThreeDigits);
            }
            _ablationTestResult.GetSensorDataDictionaryById(id)[TestParameter.Inflation_Speed].Add(value_);

            var expected_ = 0.00d;
            var passed_ = false;

            if(ablationConfiguration.IsFastInflation)
            {
              passed_ = value_ <= FastInflationSpeedUpper;
              expected_ = 0.00d;
            }
            else
            {
              if(id > 1)
              {
                var previousResult_ = _ablationTestResult?.GetSensorDataDictionaryById(id - 1);
                if(previousResult_?[TestParameter.Inflation_Speed]?.Count > 0)
                {
                  var previousSpeed_ = previousResult_[TestParameter.Inflation_Speed].FirstOrDefault();
                  expected_ = previousSpeed_ + SlowInflationDelta;
                }
                else
                {
                  expected_ = SlowInflationSpeedLower;
                }
                passed_ = value_ > expected_ && value_ < SlowInflationSpeedUpper;
              }
              else
              {
                passed_ = value_ > SlowInflationSpeedLower && value_ < SlowInflationSpeedUpper;
                expected_ = SlowInflationSpeedLower;
              }
            }
            resultDictionary_[parameter].Add((value_, passed_, expected_));
            break;
          case TestParameter.Inflation_IBP:
            var toCalculateIBPData_ = Math.Round(sensorDataDictionary_[parameter].Count > skip_
              ? sensorDataDictionary_[parameter].Skip(skip_).Average()
              : sensorDataDictionary_[parameter].Average(), RoundTwoDigits);
						var ibpInflationLower = ablationConfiguration.EnableDASBalloon ? IBPDASBalloonInflationLower : IBPInflationLower;
            var ibpInflationUpper = ablationConfiguration.EnableDASBalloon ? IBPDASBalloonInflationUpper : IBPInflationUpper;

            resultDictionary_[parameter].Add((toCalculateIBPData_, toCalculateIBPData_ >= ibpInflationLower && toCalculateIBPData_ <= ibpInflationUpper, ablationConfiguration.EnableDASBalloon ? 1d : -1d ));
            break;
          case TestParameter.Inflation_OBP:
            if(id == 1)
            {
              var obpInflationThreshold_ = Math.Round(obpInReady + OBPDeltaInflationState, RoundOneDigit);
              var max_ = Math.Round(sensorDataDictionary_[parameter].Max(), RoundOneDigit);
              resultDictionary_[parameter].Add((max_, max_ <= obpInflationThreshold_, obpInflationThreshold_));
            }
            break;
          case TestParameter.Inflation_PT2:
            var toCalculatePT2Data_ = Math.Round(sensorDataDictionary_[parameter].Count > skip_
              ? sensorDataDictionary_[parameter].Skip(skip_).Average()
              : sensorDataDictionary_[parameter].Average(), RoundTwoDigits);
            resultDictionary_[parameter].Add((toCalculatePT2Data_, toCalculatePT2Data_ <= PT2InflationUpper && toCalculatePT2Data_ >= PT2InflationLower, null));
            break;
          case TestParameter.Inflation_FM1:
            var toCalculateFM1Data_ = Math.Round(sensorDataDictionary_[parameter].Count > skip_
              ? sensorDataDictionary_[parameter].Skip(skip_).Average()
              : sensorDataDictionary_[parameter].Average(), RoundOneDigit);
            resultDictionary_[parameter].Add((toCalculateFM1Data_, toCalculateFM1Data_ <= FM1InflationUpper && toCalculateFM1Data_ >= FM1InflationLower, null));
            break;
        }
      }
    }

    public void RecordFlowMeterCheckResult(int id, (double Value, bool? Passed, double?) result)
    {
      var ablationResultDictionary = _ablationTestResult.AblationResult[AblationTestState.ABLATION];
      ablationResultDictionary[TestParameter.Ablation_FlowMeterCheck].Add(result);
    }

		public void ProcessAblationData(int id, IAblationConfiguration ablationConfiguration, IbpAblationValidator ibpAblationValidator, Pwm2AblationValidator pwm2AblationValidator, double obpInReady)
    {
      var sensorDataDictionary_ = _ablationTestResult.GetSensorDataDictionaryById(id);
      var ablationResultDictionary_ = _ablationTestResult.AblationResult[AblationTestState.ABLATION];

      foreach(var parameter in ablationResultDictionary_.Keys)
      {
        if(sensorDataDictionary_[parameter] != null && sensorDataDictionary_[parameter].Count > 0)
        {
          var first_ = Math.Round(sensorDataDictionary_[parameter].FirstOrDefault(), RoundTwoDigits);
          var last150Avg_ = Math.Round(sensorDataDictionary_[parameter].TakeLast(150).Average(), RoundTwoDigits);
          var max_ = Math.Round(sensorDataDictionary_[parameter].Max(), RoundTwoDigits);
          var min_ = Math.Round(sensorDataDictionary_[parameter].Min(), RoundTwoDigits);

          switch(parameter)
          {
            case TestParameter.Transition_Time:
              ablationResultDictionary_[parameter].Add((first_, first_ >= 5.0 && first_ <= 20.0, null));
              break;
            case TestParameter.FiftyDegree_Time:
              ablationResultDictionary_[parameter].Add((first_, null, null));
              break;
            case TestParameter.Ablation_FM1:
              var fm1AblationUpper = ablationConfiguration.EnableDASBalloon ? FM1DASBalloonAblationUpper : FM1AblationUpper;
              var fm1AblationLower = ablationConfiguration.EnableDASBalloon ? FM1DASBalloonAblationLower : FM1AblationLower;
              ablationResultDictionary_[parameter].Add((last150Avg_, 
								last150Avg_ <= fm1AblationUpper && last150Avg_ >= fm1AblationLower, ablationConfiguration.EnableDASBalloon ? 1d : -1d ));
              break;
            case TestParameter.Ablation_PT2:
              ablationResultDictionary_[parameter].Add((last150Avg_,
                last150Avg_ <= PT2AblationUpper && last150Avg_ >= PT2AblationLower, null));
              break;
            case TestParameter.Ablation_IBP:
              switch(ibpAblationValidator)
              {
                case IbpAblationValidator.Rule1:
                  var ibpInflationLower = ablationConfiguration.EnableDASBalloon ? IBPDASBalloonInflationLower : IBPInflationLower;
                  var ibpInflationUpper = ablationConfiguration.EnableDASBalloon ? IBPDASBalloonInflationUpper : IBPInflationUpper;
                  ablationResultDictionary_[parameter].Add((last150Avg_,
										last150Avg_ <= ibpInflationUpper && last150Avg_ >= ibpInflationLower, ablationConfiguration.EnableDASBalloon ? 1d : -1d));
                  break;
                case IbpAblationValidator.Rule2:
                  ablationResultDictionary_[parameter].Add((last150Avg_, null, null));
                  break;
                case IbpAblationValidator.Rule3:
                  break;
                default:
                  throw new ArgumentOutOfRangeException(nameof(ibpAblationValidator), ibpAblationValidator, null);
              }
              break;
            case TestParameter.Ablation_OBP:
#if DEBUG
              obpInReady = -13.4;
              max_ = -13.5;
#endif
              var obpAblationThreshold_ = Math.Round(obpInReady + OBPDeltaAblationState, RoundOneDigit);
              ablationResultDictionary_[parameter].Add((max_, max_ <= obpAblationThreshold_, obpAblationThreshold_));
              break;
            case TestParameter.Ablation_TC1:
              ablationResultDictionary_[parameter].Add((min_, null, null));
              break;
            case TestParameter.Ablation_PWM1:
              ablationResultDictionary_[parameter].Add((last150Avg_, null, null));
              break;
            case TestParameter.Ablation_PWM2:
              switch(pwm2AblationValidator)
              {
                case Pwm2AblationValidator.Rule1:
                  ablationResultDictionary_[parameter].Add((last150Avg_, last150Avg_ <= PWM2AblationThreshold, null));
                  break;
                case Pwm2AblationValidator.Rule2:
                  ablationResultDictionary_[parameter].Add((last150Avg_, null, null));
                  break;
                case Pwm2AblationValidator.Rule3:
                  break;
                default:
                  throw new ArgumentOutOfRangeException(nameof(pwm2AblationValidator), pwm2AblationValidator, null);
              }
              break;
            case TestParameter.Ablation_PT3:
              ablationResultDictionary_[parameter].Add((last150Avg_, null, null));
              break;
            case TestParameter.Ablation_PT4:
              ablationResultDictionary_[parameter].Add((last150Avg_, null, null));
              break;
            case TestParameter.Ablation_PT5:
              ablationResultDictionary_[parameter].Add((last150Avg_, null, null));
              break;
            case TestParameter.Ablation_TS1:
              ablationResultDictionary_[parameter].Add((max_, max_ <= TS1AblationThreshold, null));
              break;
          }
        }
      }
    }

    public void ProcessThawingData(int id)
    {
      var sensorDataDictionary_ = _ablationTestResult.GetSensorDataDictionaryById(id);
      var thawingResultDictionary_ = _ablationTestResult.AblationResult[AblationTestState.THAWING];

      foreach(var parameter in thawingResultDictionary_.Keys)
      {
        if(sensorDataDictionary_[parameter] != null && sensorDataDictionary_[parameter].Count > 0)
        {
          var toCalculateDataAvg_ = Math.Round(sensorDataDictionary_[parameter].Count > SkipSeconds
            ? sensorDataDictionary_[parameter].Skip(SkipSeconds).Average()
            : sensorDataDictionary_[parameter].Average(), RoundTwoDigits);

          switch(parameter)
          {
            case TestParameter.Thawing_PT3:
              thawingResultDictionary_[parameter].Add((toCalculateDataAvg_, null, null));
              break;
            case TestParameter.Thawing_PT4:
              thawingResultDictionary_[parameter].Add((toCalculateDataAvg_, null, null));
              break;
            case TestParameter.Thawing_PT5:
              thawingResultDictionary_[parameter].Add((toCalculateDataAvg_, null, null));
              break;
            case TestParameter.Thawing_PWM1:
              thawingResultDictionary_[parameter].Add((Math.Round(toCalculateDataAvg_, RoundTwoDigits), null, null));
              break;
            case TestParameter.Thawing_PWM2:
              thawingResultDictionary_[parameter].Add((Math.Round(toCalculateDataAvg_, RoundTwoDigits), null, null));
              break;
          }
        }
      }
    }
  }
}
