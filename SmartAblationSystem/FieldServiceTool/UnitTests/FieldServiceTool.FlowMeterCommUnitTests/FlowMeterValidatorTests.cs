using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Module.FlowMeterComm.Models;
using Module.FlowMeterComm.Services;

namespace FieldServiceTool.FlowMeterCommUnitTests
{
  [TestClass]
  public class FlowMeterValidatorTests
  {
    [TestMethod]
    public void ValidateFlowMeterResult_MoreThan120Data_Valid()
    {
      var acceptance = 0.02;
      var expectedOffset = 0.01;
      var testData = GenerateRandomFlowDataCollection(150, expectedOffset); 

      var result = FlowMeterValidator.ValidateFlowMeterResult(testData, acceptance, 1000);

      Assert.IsTrue(result.IsValid);
      Assert.AreEqual(expectedOffset, Math.Round(result.AverageOffset, 2));
    }

    [TestMethod]
    public void ValidateFlowMeterResult_LessThan120Data_Valid()
    {
      var acceptance = 0.02;
      var expectedOffset = 0.01;
      var testData = GenerateRandomFlowDataCollection(50, expectedOffset);

      var result = FlowMeterValidator.ValidateFlowMeterResult(testData, acceptance, 1000);

      Assert.IsTrue(result.IsValid);
      Assert.AreEqual(expectedOffset, Math.Round(result.AverageOffset, 2));
    }

    [TestMethod]
    public void ValidateFlowMeterResult_MoreThan120Data_Invalid()
    {
      var acceptance = 0.02;
      var expectedOffset = 0.03;
      var testData = GenerateRandomFlowDataCollection(150, expectedOffset);

      var result = FlowMeterValidator.ValidateFlowMeterResult(testData, acceptance, 1000);

      Assert.IsFalse(result.IsValid);
      Assert.AreEqual(expectedOffset, Math.Round(result.AverageOffset, 2));
    }

    [TestMethod]
    public void ValidateFlowMeterResult_LessThan120Data_Invalid()
    {
      var acceptance = 0.02;
      var expectedOffset = 0.03;
      var testData = GenerateRandomFlowDataCollection(50, expectedOffset);

      var result = FlowMeterValidator.ValidateFlowMeterResult(testData, acceptance, 1000);

      Assert.IsFalse(result.IsValid);
      Assert.AreEqual(expectedOffset, Math.Round(result.AverageOffset, 2));
    }

    private IList<FlowRateData> GenerateRandomFlowDataCollection(int count, double offset)
    {
      var direction = 1d;
      var random = new Random();

      List<FlowRateData> testData = new List<FlowRateData>();
      for (int i = 0; i < count; i++)
      {
        var fm1 = random.NextDouble() * 1000d;
        direction = (i % 5) == 0 ? -1 : 1;
        testData.Add(new FlowRateData
        {
          Index = i,
          FM1 = fm1,
          FMExt = fm1 * (1 + offset * direction)
        });
      }

      return testData;
    }
  }
}
