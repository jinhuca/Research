
using System.Collections.Generic;
using Module.CatheterTestTool.Models;

namespace Module.CatheterTestTool.Services
{
    public interface ITestDataManager
    {
        void StartRecordTestData();
        bool IsRecordingData { get; }
        void CompleteRecord();
        CatheterTestData GetTestData();
        TestReportData GetTestResult();
        IList<CatheterTestData> GetTestDetailData();
        void SetTesterName(string testerName);
        void SetCatheterInfo(CatheterInfoData catheterInfo);
        bool SaveTestData();
    }
}
