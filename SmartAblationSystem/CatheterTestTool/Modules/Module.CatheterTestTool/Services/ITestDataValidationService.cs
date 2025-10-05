
using System.Collections.Generic;
using Module.CatheterTestTool.Models;

namespace Module.CatheterTestTool.Services
{
    public interface ITestDataValidationService
    {
        TestReportData ValidateTestResult(CatheterTestData testData);
    }
}
