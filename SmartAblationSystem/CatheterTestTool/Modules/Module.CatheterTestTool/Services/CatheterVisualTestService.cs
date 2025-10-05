using System;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Module.CatheterTestTool.Configuration;
using Module.CatheterTestTool.Models;
using Module.Console.Interfaces;
using Module.Infrastructure.AppLog;
using Module.SystemParameters.Interfaces;
using Prism.Ioc;


namespace Module.CatheterTestTool.Services
{
  public class CatheterVisualTestService : CatheterTestServiceBase, ICatheterVisualTestService
  {
    private TestStatus _testStatus;
    private ISubject<TestStatus> _testStatusSubject;
    
    private readonly AutoResetEvent _completeVisualTestEvent = new AutoResetEvent(false);

    private readonly IContainerProvider _containerProvider;
    private readonly ISensorParameters _sensorParameters;
    private readonly ICatheterTestConfiguration _configuration;

    public CatheterVisualTestService(IContainerProvider containerProvider,
      IMachineModel machineModel,
      ISensorParameters sensorParameters,
      ICatheterTestConfiguration catheterTestConfiguration) : base(machineModel)
    {
      _containerProvider = containerProvider;
      _sensorParameters = sensorParameters;
      _configuration = catheterTestConfiguration;

    }

    public IObservable<TestStatus> StartTest()
    {
      FieldServiceTrace.Log("Start Catheter Visual Testing ...");

      //Reset Cancel Event
      CancelTestSessionEvent.Reset();
      _completeVisualTestEvent.Reset();

      _testStatus = new TestStatus()
      {
        CountdownTimer = CatheterTestConstants.ABLATION_TIME,
        Description = "Start Catheter Visual Testing ..."
      };

      _testStatusSubject = new BehaviorSubject<TestStatus>(_testStatus);
      
      // 1. Turn on Vacuum and wait for Ready state
      // 2. Switch to Inflate state and wait ...
      // 3. When User clicks continue, deflate the balloon and complete visual test   
      Task.Run(SwitchToReadyState);
      
      return _testStatusSubject; 
    }

    public void CancelTest()
    {
      CancelTestSessionEvent.Set();
    }

    public void CompleteTest(bool isCompleted)
    {
      if (isCompleted)
        _completeVisualTestEvent.Set();
      else 
        CancelTestSessionEvent.Set();
    }

    protected override bool SwitchToReadyState()
    {
      bool succeeded = base.SwitchToReadyState();
      if (succeeded)
      {
        // Succeeds, and Switch to Inflation
        SwitchToInflation(); 
      }
      else if (IsCancelEventSet)
      {
        //Cancelled
      }
      else // Timeout, switching to Ready state failed
      {

      }

      return succeeded;
    }

    protected override bool SwitchToInflation()
    {
      _testStatus.Description = "Switching to Inflation state ...";
      _testStatusSubject.OnNext(_testStatus);

      var succeeded = base.SwitchToInflation();
      if (succeeded)
      {
        // Notify that console is in Inflation state and waiting for user input 
        _testStatus.NeedUserInput = true;
        _testStatusSubject.OnNext(_testStatus);

        // Wait for user completing Visual test 
        WaitHandle.WaitAny(new WaitHandle[]{_completeVisualTestEvent, CancelTestSessionEvent});
        // Deflate the balloon, when receives continue signal 
        MachineModel.Console.Stop();
        _testStatus.NeedUserInput = false; 

        if (IsCancelEventSet)
        {
          _testStatus.IsTestCompleted = false;
          _testStatus.Description = "Catheter Visual Check failed. Stop current test session.";
          // Cancel the test and turn off the vacuum
          SetVacuum(false); 
        }
        else
        {
          _testStatus.IsTestCompleted = true;
          _testStatus.Description = "Complete Visual test and continue ablation testing";
        }

        _testStatusSubject.OnNext(_testStatus);
        Task.Delay(200).ContinueWith((_) => _testStatusSubject.OnCompleted()); 
      }
      else if (IsCancelEventSet)
      {
        // Cancelled 
      }
      else // Timeout, failed to switch inflation 
      {
        TestOnError("Timeout while waiting for Console switching to INFLATION state.");
      }

      return succeeded;
    }

    private void TestOnError(string errorMessage)
    {
      _testStatus.Description = errorMessage;
      _testStatusSubject?.OnNext(_testStatus);

      _testStatusSubject?.OnError(new InvalidOperationException(errorMessage));

      CancelTestSessionEvent.Set();
    }

  }
}
