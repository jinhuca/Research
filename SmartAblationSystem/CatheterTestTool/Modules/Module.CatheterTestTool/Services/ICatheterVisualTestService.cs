using System;

namespace Module.CatheterTestTool.Services
{
  public interface ICatheterVisualTestService
  {
    IObservable<TestStatus> StartTest();
    void CancelTest();
    void CompleteTest(bool isCompleted);
  }
}