using System;
using System.Collections.Generic;
using System.Linq;
using Module.CatheterTestTool.Configuration;
using Module.CatheterTestTool.Models;

namespace Module.CatheterTestTool.Services
{
  public class TestDataValidationService : ITestDataValidationService
  {
    // private readonly IDictionary<string, ValidationCriteria> _validationCriteria;
    private readonly ICatheterTestConfiguration _testConfiguration;
    // the validation criteria should be injected from configuration  
    public TestDataValidationService(ICatheterTestConfiguration testConfiguration)
    {
      _testConfiguration = testConfiguration;
    }

    public TestReportData ValidateTestResult(CatheterTestData testData)
    {
      var results = (testData != null)
           ? new List<TestDataValidationResult>
           {
                    GetTestValidatedData(testData.TC, CatheterTestConstants.SensorNameTC1, testData.CatheterInfo.ID),
                    GetTestValidatedData(testData.IBP, CatheterTestConstants.SensorNameIBP, testData.CatheterInfo.ID),
                    GetTestValidatedData(testData.OBP, CatheterTestConstants.SensorNameOBP, testData.CatheterInfo.ID),
                    GetTestValidatedData(testData.PT2, CatheterTestConstants.SensorNamePT2, testData.CatheterInfo.ID),
                    GetTestValidatedData(testData.PT3, CatheterTestConstants.SensorNamePT3, testData.CatheterInfo.ID),
                    GetTestValidatedData(testData.PT4, CatheterTestConstants.SensorNamePT4, testData.CatheterInfo.ID),
                    GetTestValidatedData(testData.FM1, CatheterTestConstants.SensorNameFM1, testData.CatheterInfo.ID)
           }
           : Enumerable.Empty<TestDataValidationResult>().ToList();

      return new TestReportData()
      {
        CatheterInfo = testData?.CatheterInfo,
        TestDate = DateTime.Now,
        TesterName = testData?.TesterName,
        Results = results
      };
    }

    private TestDataValidationResult GetTestValidatedData(double value, string sensor, int catheterId)
    {
      var validationCriteria = _testConfiguration.GetValidationCriteriaMap(catheterId);
      var sensorConfigured = validationCriteria.ContainsKey(sensor);
      var minValue = sensorConfigured ? validationCriteria[sensor].MinValue : 0;
      var maxValue = sensorConfigured ? validationCriteria[sensor].MaxValue : 0;

      return new TestDataValidationResult()
      {
        Sensor = sensor,
        Value = Math.Round(value, 3),
        Expected = new[] { minValue, maxValue },
        Result = IsInRange(value, minValue, maxValue) ? TestResult.PASS : TestResult.FAIL
      };
    }

    private bool IsInRange(double value, double min, double max)
    {
      return value >= min && value <= max;
    }
  }
}
