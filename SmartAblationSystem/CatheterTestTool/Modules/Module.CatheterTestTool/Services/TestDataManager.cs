using Module.CatheterTestTool.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Module.SystemParameters.Interfaces;

namespace Module.CatheterTestTool.Services
{
  public class TestDataManager : ITestDataManager
  {
    private readonly ISensorParameters _sensorParameters;
    private readonly ITestDataValidationService _testDataValidationService;
    private readonly ITestDataFileManager _testDataFileManager;

    private readonly IObservable<EventPattern<PropertyChangedEventArgs>> _sensorParametersUpdateObserable;
    private readonly SerialDisposable _systemParametersUpdatedHandler = new SerialDisposable();
    private readonly SerialDisposable _collectingDetailDataDisposable = new SerialDisposable();

    private readonly IList<ISubject<double>> _subjects = new List<ISubject<double>>();

    private IList<CatheterTestData> _testDetailData;
    private ISubject<double> _temperatureSubject;
    private ISubject<double> _fm1Subject;
    private ISubject<double> _pt2Subject;
    private ISubject<double> _pt3Subject;
    private ISubject<double> _pt4Subject;
    private ISubject<double> _ibpSubject;
    private ISubject<double> _obpSubject;

    private readonly CatheterTestData _testData;
    private volatile bool _recordingDataStarted = false;

    public TestDataManager(ISensorParameters sensorParameters, ITestDataValidationService testDataValidationService, ITestDataFileManager testDataFileManager)
    {
      _sensorParameters = sensorParameters;
      _testDataValidationService = testDataValidationService;
      _testDataFileManager = testDataFileManager; 

      _sensorParametersUpdateObserable =
          Observable.FromEventPattern<PropertyChangedEventArgs>(_sensorParameters, "PropertyChanged");
      _testData = new CatheterTestData();
    }

    public bool IsRecordingData => _recordingDataStarted;

    public void StartRecordTestData()
    {
      if (_recordingDataStarted) return;

      _recordingDataStarted = true;
      CreateAndInitStartValues();
      _testDetailData = new List<CatheterTestData>();
      _systemParametersUpdatedHandler.Disposable = _sensorParametersUpdateObserable
                                                      .Subscribe(pattern => HandleSensorParametersUpdated(pattern.EventArgs));

      _collectingDetailDataDisposable.Disposable = Observable.Interval(TimeSpan.FromMilliseconds(500))
          .ObserveOn(TaskPoolScheduler.Default)
          .Subscribe(_ => GetTestDataSnapshot());

      GetTestDataSnapshot();
    }

    public void CompleteRecord()
    {
      _systemParametersUpdatedHandler.Disposable?.Dispose();
      _collectingDetailDataDisposable.Disposable?.Dispose();

      CompleteAndUpdateTestData();
      _recordingDataStarted = false;
    }

    public void SetTesterName(string testerName)
    {
      _testData.TesterName = testerName;
    }

    public void SetCatheterInfo(CatheterInfoData catheterInfo)
    {
      _testData.CatheterInfo = catheterInfo;
    }

    public CatheterTestData GetTestData()
    {
      return (CatheterTestData)_testData.Clone();
    }

    public IList<CatheterTestData> GetTestDetailData()
    {
      return _testDetailData;
    }

    public TestReportData GetTestResult()
    {
      return _testDataValidationService?.ValidateTestResult(_testData);
    }

    public bool SaveTestData()
    {
      return _testDataFileManager?.SaveTestData(GetTestResult(), _testDetailData)??false; 
    }

    private void HandleSensorParametersUpdated(PropertyChangedEventArgs eventArgs)
    {
      switch (eventArgs.PropertyName)
      {
        case nameof(ISensorParameters.FM1):
          _fm1Subject?.OnNext(_sensorParameters.FM1);
          break;
        case nameof(ISensorParameters.Temperature):
          _temperatureSubject?.OnNext(_sensorParameters.Temperature);
          break;
        case nameof(ISensorParameters.PT2):
          _pt2Subject?.OnNext(_sensorParameters.PT2);
          break;
        case nameof(ISensorParameters.PT3):
          _pt3Subject?.OnNext(_sensorParameters.PT3);
          break;
        case nameof(ISensorParameters.PT4):
          _pt4Subject?.OnNext(_sensorParameters.PT4);
          break;
        case nameof(ISensorParameters.IBP):
          _ibpSubject?.OnNext(_sensorParameters.IBP);
          break;
        case nameof(ISensorParameters.OBP):
          _obpSubject?.OnNext(_sensorParameters.OBP);
          break;
        default:
          break;
      }
    }

    private void CompleteAndUpdateTestData()
    {
      foreach (var subject in _subjects)
      {
        subject.OnCompleted();
      }
    }

    private void SubscribeWithAverage(ISubject<double> subject, Action<double> action)
    {
      subject?.Average().Subscribe(action);
      _subjects.Add(subject);
    }

    private void CreateAndInitStartValues()
    {
      _subjects.Clear();

      _temperatureSubject = new BehaviorSubject<double>(_sensorParameters.Temperature);
      SubscribeWithAverage(_temperatureSubject, tc => _testData.TC = tc);

      _fm1Subject = new BehaviorSubject<double>(_sensorParameters.FM1);
      SubscribeWithAverage(_fm1Subject, fm => _testData.FM1 = fm);

      _pt2Subject = new BehaviorSubject<double>(_sensorParameters.PT2);
      SubscribeWithAverage(_pt2Subject, pt => _testData.PT2 = pt);

      _pt3Subject = new BehaviorSubject<double>(_sensorParameters.PT3);
      SubscribeWithAverage(_pt3Subject, pt => _testData.PT3 = pt);

      _pt4Subject = new BehaviorSubject<double>(_sensorParameters.PT4);
      SubscribeWithAverage(_pt4Subject, pt => _testData.PT4 = pt);

      _ibpSubject = new BehaviorSubject<double>(_sensorParameters.IBP);
      SubscribeWithAverage(_ibpSubject, ibp => _testData.IBP = ibp);

      _obpSubject = new BehaviorSubject<double>(_sensorParameters.OBP);
      SubscribeWithAverage(_obpSubject, obp => _testData.OBP = obp);
    }

    private void GetTestDataSnapshot()
    {
      _testDetailData?.Add(new CatheterTestData()
      {
        CatheterInfo = _testData?.CatheterInfo,
        TesterName = _testData?.TesterName,
        FM1 = _sensorParameters.FM1,
        IBP = _sensorParameters.IBP,
        OBP = _sensorParameters.OBP,
        TC = _sensorParameters.Temperature,
        PT2 = _sensorParameters.PT2,
        PT3 = _sensorParameters.PT3,
        PT4 = _sensorParameters.PT4
      });
    }
  }
}
