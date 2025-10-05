using Module.CatheterTestTool.Models;
using System;
using System.Diagnostics;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Module.CatheterTestTool.Configuration;
using Module.Console.Interfaces;
using Module.Infrastructure.AppLog;
using Module.SystemParameters.Interfaces;
using Prism.Ioc;

using static Communication.CanBusMessageDefinition;

namespace Module.CatheterTestTool.Services
{
  public class CatheterTestService : CatheterTestServiceBase, ICatheterTestService
  {
    private readonly IContainerProvider _containerProvider;
    private readonly ISensorParameters _sensorParameters;
    private readonly ICatheterTestConfiguration _catheterTestConfiguration;
     
    private readonly System.Timers.Timer _ablationTimer;
    private readonly Stopwatch _ablationRecordingDataStopWatch = new Stopwatch();
    private readonly TestStatus _currentTestState = new TestStatus { CountdownTimer = CatheterTestConstants.ABLATION_TIME };

    private readonly SerialDisposable _exceptionStateMonitoringDisposable = new SerialDisposable();

    private ITestDataManager _testDataManager;
    private ISubject<TestStatus> _testStatusSubject;
    private CatheterInfoData _currentCatheterInfo; 

    public CatheterTestService(IContainerProvider containerProvider,
        IMachineModel machineModel,
        ISensorParameters sensorParameters,
        ICatheterTestConfiguration catheterTestConfiguration) : base(machineModel)
    {
      _containerProvider = containerProvider;
      _sensorParameters = sensorParameters;
      _catheterTestConfiguration = catheterTestConfiguration; 

      // Initialize the DeflateAfterThaw to true, so it will deflate automatically
      // when temperature reaches target value during thawing state
      machineModel.Console.DeflateAfterThaw = true;

      _ablationTimer = new System.Timers.Timer(1000);
      _ablationTimer.Elapsed += AblationTimerTick;
      _ablationTimer.Enabled = false;

      InitializeConsole();
    }

    public IObservable<MessageStateId> SystemStateObservable => SystemStateSubject;

    public MessageStateId SystemState => MachineModel.SystemState;

    public void CancelTest()
    {
      if (IsCancelEventSet)
        return;
      CancelTestProcess();
    }

    private void CancelTestProcess()
    {
      CancelTestSessionEvent.Set();

      // Notify the waiting thread to stop with cancel
      SystemStateSubject?.OnNext(MachineModel.SystemState);

      _currentTestState.CurrentTestStep = 0;
      _currentTestState.Description = "Cancelling test ...";
      _testStatusSubject?.OnNext(_currentTestState);

      StopTestProcess();
    }

    public TestReportData GetTestResultData()
    {
      return _testDataManager?.GetTestResult();
    }

    public IObservable<TestStatus> StartTest(string tester, CatheterInfoData catheterInfo)
    {
      FieldServiceTrace.Log("Start Testing ...");
      //Reset Cancel Event
      CancelTestSessionEvent.Reset();
      // Resolve a test data manager per session
      _testDataManager = _containerProvider.Resolve<ITestDataManager>();

      _testStatusSubject = new BehaviorSubject<TestStatus>(_currentTestState);
      _currentCatheterInfo = catheterInfo;

      // set the tester name and CatheterInfo 
      _testDataManager.SetTesterName(tester);
      _testDataManager.SetCatheterInfo(catheterInfo);

      _currentTestState.CurrentTestStep = 1;
      _currentTestState.TestProgressInPercentage = 0;
      _currentTestState.Step1Progress = 100;
      _currentTestState.Step2Progress = 0;
      _currentTestState.Step3Progress = 0;
      _currentTestState.IsTestCompleted = false;
      _currentTestState.CountdownTimer = CatheterTestConstants.ABLATION_TIME;
      _currentTestState.Description = "Checking console Catheter information and status ...";
      _testStatusSubject?.OnNext(_currentTestState);

#if !DEBUG
      //Handle the case that console switches to Exception state
      _exceptionStateMonitoringDisposable.Disposable =
          SystemStateSubject?.Where(s => s == MessageStateId.CAN_ID_STATE_EXCEPTION)
              .ObserveOn(TaskPoolScheduler.Default)
              .Subscribe(s =>
              {
                TestOnError("The console encountered an error.");
              });
#endif
      // Step 0: Wait for water temperature reaching desired value (>= 30); 
      // Step 1: Turn on vacuum and wait for State to Ready  
      // When this method is called, the system should be in Ready state (catheter is connected)
      // Step 2: Start Inflating and wait until system is transited to Inflation state (Console.Start() from Ready state)
      //          and IBP is stable between 2.5+/- 0.1 psi with 5 seconds  
      // Step 3: Switch to Ablation state, and wait the system is transited to Ablation state
      //          and last to 60 seconds (Console.Start() from Ablation State)
      // Step 4: Start recording data for 10 seconds and stop recording
      // 
      // Step 5: Stop Ablation and wait until system is transited to Thawing state 
      // (Console.Stop() to stop ablation)         

      WaitForWaterTemperature();

      return _testStatusSubject;
    }

    private bool IfWaterTemperatureIsInRange()
    {
      return _sensorParameters.Temperature >= CatheterTestConstants.MINIMUM_WATER_TEMPERATURE_REQUIRED;
    }

    private async void WaitForWaterTemperature()
    {
      if (!IfWaterTemperatureIsInRange())
      {
        await Task.Run(() =>
        {
          _currentTestState.WaitingWaterTemperature = true;
          _testStatusSubject.OnNext(_currentTestState);

          while (!IfWaterTemperatureIsInRange() && !IsCancelEventSet)
          {
            Task.Delay(100).Wait(); 
          }

          _currentTestState.WaitingWaterTemperature = false;
          _testStatusSubject.OnNext(_currentTestState);
          
          if (!IsCancelEventSet)
            SwitchToReadyState(); 
        });
      }
      else 
        await Task.Run(SwitchToReadyState);
    }

    protected override bool SwitchToReadyState()
    {
      FieldServiceTrace.Log("SwitchToReadyState");
      bool succeeded = base.SwitchToReadyState(); 

      if (IsCancelEventSet)
      {
        //cancelled 
      }
      else if (!succeeded)
      {
        // State transiting timeout 
        TestOnError("Timeout while waiting for Console switching to READY state.");
      }
      else
      {
        //Go to next step (Switch To Inflation) if it is not cancelled
        succeeded = SwitchToInflation();
      }

      return succeeded && !IsCancelEventSet;
    }

    protected override bool SwitchToInflation()
    {

      // Update progress
      UpdateTestProgress(2, 5, "Switching to Inflation ...");

      bool succeeded = base.SwitchToInflation(); 

      if (IsCancelEventSet)
      {
        //cancelled
      }
      else if (!succeeded)
      {
        // State transiting timeout 
        TestOnError("Timeout while waiting for Console switching to INFLATION state.");
      }
      else
      {
        //Go to next step (Wait IBP stablization) if it is not cancelled
        ValidateIBPStablization(CatheterTestConstants.IBP_STABLIZATION_TIME_IN_SEC,
                                                            CatheterTestConstants.IBP_STABLIZATION_TIMEOUT_IN_SEC);
      }

      return succeeded && !IsCancelEventSet; 
    }

    private void ValidateIBPStablization(int timeInSec, int timeoutInSec)
    {
      bool isTimeout = false;

      UpdateTestProgress(2, 8, "Waiting for IBP value to stabilize ...");
      var stopWatcher = new Stopwatch();
      stopWatcher.Start();
      double startElapsed = stopWatcher.Elapsed.TotalSeconds;

      var ibpTargetSetting = _catheterTestConfiguration.GetInflationIBPSetting(_currentCatheterInfo?.ID??1); 

      while (!IsCancelEventSet)
      {
        var elapsedTime = stopWatcher.Elapsed.TotalSeconds;
        if (elapsedTime >= timeoutInSec)
        {
          isTimeout = true;
          break;
        }

        var ibpValue = _sensorParameters.IBP;

        // reset the start counting time if the value out of range 
        if (!ibpValue.AreDoubleValuesEqual(ibpTargetSetting.TargetValue, ibpTargetSetting.Offset))
        {
          startElapsed = elapsedTime;
        }

        if (elapsedTime - startElapsed >= timeInSec)
        {
          break;
        }

        // Run this every 0.5 second
        Thread.Sleep(500);
      }

      stopWatcher.Stop();

      if (IsCancelEventSet)
      {

      }
      else if (isTimeout)
      {
        //Timeout
        TestOnError("Timeout while waiting for IBP value stable.");
      }
      else
      {
        // Start Ablation 
        UpdateTestProgress(2, 10, "IBP value is stable.");
        StartAblation();
      }
    }

    private void StartAblation()
    {
      bool succeeded;
      using (var switchToAblation = SystemStateSubject?
                 .ObserveOn(TaskPoolScheduler.Default)
                 .Subscribe(state =>
                 {
                   if (state == MessageStateId.CAN_ID_STATE_TRANSITION
                             || state == MessageStateId.CAN_ID_STATE_ABLATION
                             || IsCancelEventSet)
                     ConsoleSwitchState.Set();
                 }))
      {
        // Currently, it should be in Inflation State,
        // and sending Start command should notify console to switch to Ablation state
        MachineModel.Console.Start();

        _currentTestState.Description = "Starting Ablation ...";
        _testStatusSubject?.OnNext(_currentTestState);

        succeeded = ConsoleSwitchState.WaitOne(CatheterTestConstants.READY_TO_ABLATION_STATE_TIMEOUT_IN_SEC);
      }

      if (IsCancelEventSet)
      {
        // Cancelled
      }
      else if (!succeeded)
      {
        //Timoeout
        TestOnError("Timeout while waiting for Console switching to ABLATION state.");
      }
      else
      {
        _currentTestState.Description = "Ablation In Progress ...";
        _testStatusSubject?.OnNext(_currentTestState);

        // Start Timer counting down
        _ablationTimer.Start();
        _ablationRecordingDataStopWatch.Restart();
      }
    }

    private void AblationTimerTick(object sender, ElapsedEventArgs args)
    {
      double ablationElapsed = _ablationRecordingDataStopWatch.Elapsed.TotalSeconds;

      // check if needs to start recording data
      if (ablationElapsed >= (CatheterTestConstants.ABLATION_TIME - CatheterTestConstants.RECORDING_TEST_DATA_TIME_IN_SEC)
          && !_testDataManager.IsRecordingData)
      {
        _testDataManager?.StartRecordTestData();
        _currentTestState.Description = "Start collecting data ...";
      }

      // update test progress
      _currentTestState.CountdownTimer = CatheterTestConstants.ABLATION_TIME - (int)ablationElapsed;
      
      UpdateTestProgress(2, 10 + (int)(ablationElapsed * 90 / CatheterTestConstants.ABLATION_TIME));

      _testStatusSubject?.OnNext(_currentTestState);

      if (ablationElapsed > CatheterTestConstants.ABLATION_TIME)
      {
        TestCompleted();
      }
    }

    private void UpdateTestProgress(int step, int percentage, string message = null)
    {
      _currentTestState.CurrentTestStep = step;
      _currentTestState.TestProgressInPercentage = percentage;
      _currentTestState.Step2Progress = percentage;
      
      if (message != null)
        _currentTestState.Description = message;

      _testStatusSubject?.OnNext(_currentTestState);
    }

    private void StopTestProcess()
    {
      _ablationTimer?.Stop();
      _ablationRecordingDataStopWatch?.Stop();

      // Stop ablation
      MachineModel.Console.Stop();
      _testDataManager?.CompleteRecord();

      // Subscribe to turn off vacuum before stopping the console to make sure we can catch the state update
      SubscribeToTurnOffVacuum();

      // Stop monitoring System in EXCEPTION state
      _exceptionStateMonitoringDisposable.Disposable?.Dispose();
    }

    private void TestCompleted()
    {
      _currentTestState.CountdownTimer = 0;
      _currentTestState.CurrentTestStep = 3;
      _currentTestState.Description = "Analyzing the data ...";
      _currentTestState.IsTestCompleted = true;
      _currentTestState.TestProgressInPercentage = 100;
      _currentTestState.Step2Progress = 100;
      _testStatusSubject?.OnNext(_currentTestState);

      StopTestProcess();
      _testDataManager.SaveTestData();

      _currentTestState.Description = "Storing the test result data ...";
      _currentTestState.Step3Progress = 100;
      _testStatusSubject?.OnNext(_currentTestState);
    }

    private void TestOnError(string errorMessage)
    {
      _currentTestState.Description = errorMessage;
      _testStatusSubject?.OnNext(_currentTestState);

      _testStatusSubject?.OnError(new InvalidOperationException(errorMessage));

      CancelTestProcess();
    }

    private void SubscribeToTurnOffVacuum()
    {
      // Turn off vacuum only when the system state is in Idle or Ready
      var subscription = new SingleAssignmentDisposable();
      subscription.Disposable = SystemStateSubject?
          .Where(state => state == MessageStateId.CAN_ID_STATE_READY
                          || state == MessageStateId.CAN_ID_STATE_IDLE
                          || state == MessageStateId.CAN_ID_STATE_EXCEPTION)
          .ObserveOn(TaskPoolScheduler.Default)
          .Subscribe(state =>
          {
            if (subscription.IsDisposed) return;
            SetVacuum(false);
            subscription.Dispose();
            _currentTestState.Description = "Current test completed.";
            _testStatusSubject?.OnNext(_currentTestState);

            Task.Delay(200).ContinueWith((_) => _testStatusSubject?.OnCompleted());
          });
    }

    private void InitializeConsole()
    {
      SetVacuum(false);
    }
  }
}
