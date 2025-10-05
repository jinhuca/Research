using System;
using Module.CatheterTestTool.Models;

using static Communication.CanBusMessageDefinition;

namespace Module.CatheterTestTool.Services
{
  public class TestStatus
  {
    public int CurrentTestStep { get; set; }
    public int TestProgressInPercentage { get; set; }
    public bool NeedUserInput { get; set; }
    public bool WaitingWaterTemperature { get; set; }
    public int Step1Progress { get; set; }
    public int Step2Progress { get; set; }
    public int Step3Progress { get; set; }

    public int CountdownTimer { get; set; }
    public bool IsTestCompleted { get; set; }
    public string Description { get; set; }
  }

  public interface ICatheterTestService
  {
    IObservable<MessageStateId> SystemStateObservable { get; }
    MessageStateId SystemState { get; }
    IObservable<TestStatus> StartTest(string tester, CatheterInfoData catheterInfo);
    void CancelTest();
    TestReportData GetTestResultData();
  }
}
