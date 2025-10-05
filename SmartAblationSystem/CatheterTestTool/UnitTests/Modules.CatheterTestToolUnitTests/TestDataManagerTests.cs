using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Module.CatheterTestTool.Services;
using Module.SystemParameters.Interfaces;
using Moq;

namespace Module.CatheterTestToolUnitTest
{
  [TestClass]
  public class TestDataManagerTests
  {
    private TestDataManager _testDataManager;
    private ISensorParameters _sensorParameters;

    [TestInitialize]
    public void Initialize()
    {
      _sensorParameters = new MockSensorParameters();
      var testValidationServiceMoq = new Mock<ITestDataValidationService>();
      var fileManagerMoq = new Mock<ITestDataFileManager>();
      _testDataManager = new TestDataManager(_sensorParameters, testValidationServiceMoq.Object, fileManagerMoq.Object);
    }

    [TestCleanup]
    public void Cleanup()
    {
    }

    [TestMethod]
    public void RecordTestData_Test()
    {
      double fm1 = 0;
      double tc = 5;
      double pt2 = 5;
      double pt3 = 10;
      double pt4 = 20;
      double ibp = 0;
      _sensorParameters.FM1 = fm1;
      _sensorParameters.Temperature = tc;
      _sensorParameters.IBP = ibp;
      _sensorParameters.PT2 = pt2;
      _sensorParameters.PT3 = pt3;
      _sensorParameters.PT4 = pt4;
      _testDataManager.StartRecordTestData();

      for (int i = 0; i < 20; ++i)
      {
        _sensorParameters.FM1 += 1;
        _sensorParameters.Temperature += 5;
        _sensorParameters.IBP += 4;
        _sensorParameters.PT2 += 1;
        _sensorParameters.PT3 += 2;
        _sensorParameters.PT4 += 3;
        Thread.Sleep(200);
      }

      _testDataManager.CompleteRecord();

      var testData = _testDataManager.GetTestData();

      Assert.AreEqual(10d, testData.FM1);
      Assert.AreEqual(40d, testData.IBP);
      Assert.AreEqual(15d, testData.PT2);
      Assert.AreEqual(30d, testData.PT3);
      Assert.AreEqual(50d, testData.PT4);
      Assert.AreEqual(55d, testData.TC);
    }

    [TestMethod]
    public void GetTestDetailData_Test()
    {
      double fm1 = 0;
      double tc = 5;
      double pt2 = 5;
      double pt3 = 10;
      double pt4 = 20;
      double ibp = 0;
      _sensorParameters.FM1 = fm1;
      _sensorParameters.Temperature = tc;
      _sensorParameters.IBP = ibp;
      _sensorParameters.PT2 = pt2;
      _sensorParameters.PT3 = pt3;
      _sensorParameters.PT4 = pt4;

      _testDataManager.StartRecordTestData();

      int i;
      for (i = 0; i < 40; ++i)
      {
        _sensorParameters.FM1 += 1;
        _sensorParameters.Temperature += 5;
        _sensorParameters.IBP += 4;
        _sensorParameters.PT2 += 1;
        _sensorParameters.PT3 += 2;
        _sensorParameters.PT4 += 3;
        Task.Delay(250).Wait();
      }

      _testDataManager.CompleteRecord();

      // Recording time 10 sec, and record data every 500ms. We should expect 20~22 data recorded
      var detailData = _testDataManager.GetTestDetailData();
      Assert.IsTrue(detailData.Count >= 20 && detailData.Count <= 22);
    }

    class MockSensorParameters : ISensorParameters
    {
      private double _temperature;
      private double _fm1;
      private double _ibp;
      private double _pt2;
      private double _pt3;
      private double _pt4;

      public event PropertyChangedEventHandler PropertyChanged;


      public double Temperature
      {
        get { return _temperature; }
        set
        {
          _temperature = value;
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Temperature)));
        }
      }

      public double FM1
      {
        get { return _fm1; }
        set
        {
          _fm1 = value;
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FM1)));
        }
      }

      public double IBP
      {
        get { return _ibp; }
        set
        {
          _ibp = value;
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IBP)));
        }
      }

      public double PT2
      {
        get { return _pt2; }
        set
        {
          _pt2 = value;
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PT2)));
        }
      }

      public double PT3
      {
        get { return _pt3; }
        set
        {
          _pt3 = value;
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PT3)));
        }
      }

      public double PT4
      {
        get { return _pt4; }
        set
        {
          _pt4 = value;
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PT4)));
        }
      }

      public double OBP { get; set; }
      public double LC { get; set; }
      public double PT1 { get; set; }
      public double PT5 { get; set; }
      public double TS1 { get; set; }
      public double PWM1 { get; set; }
      public double PWM2 { get; set; }
      public double PGain { get; set; }
      public double IGain { get; set; }
      public double DGain { get; set; }
      public double PIDOffset { get; set; }
      public double PatientPGain { get; set; }
      public double PatientIGain { get; set; }
      public double PatientDGain { get; set; }
      public double PatientPIDOffset { get; set; }
    }

  }
}
