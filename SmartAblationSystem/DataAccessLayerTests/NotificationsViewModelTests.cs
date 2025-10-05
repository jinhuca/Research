
using System.Windows.Input;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SmartAblationSystem.Helpers;
using SmartAblationSystem.ViewModels;

namespace DataAccessLayerTests
{
  [TestClass]
  public class NotificationsViewModelTests
  {

    [TestMethod]
    public void ResetLSProCommand_Test()
    {
      var therapyViewModelMock = new Mock<ICryoTherapyViewModel>();
      var resetLSProCommandMock = new Mock<ICommand>();
      therapyViewModelMock.Setup(x => x.ResetLSPROCommand).Returns(resetLSProCommandMock.Object);

      var notificationsViewModel = new NotificationsViewModel(therapyViewModelMock.Object);
      notificationsViewModel.ResetLSProCommand.Execute(null);

      therapyViewModelMock.Verify(x => x.ResetLSPROCommand.Execute(null), Times.Once); 
    }

    #region tests for updating IsSettingsDirtyFromUI when any setting changed 

    [TestMethod]
    public void IsSettingsDirtyFromUI_IgnoreMinimumDiaphragmMovementValue_Test()
    {
      var therapyViewModelMock = new Mock<ICryoTherapyViewModel>();
      therapyViewModelMock.Setup(x => x.IgnoreMinimumDiaphragmMovementValue).Returns(false);
      therapyViewModelMock.Setup(x => x.IsUserAllowedToChangeAblationTimers).Returns(true);
      var notificationsViewModel = new NotificationsViewModel(therapyViewModelMock.Object);

      Assert.IsFalse(notificationsViewModel.IgnoreMinimumDiaphragmMovementValue);
      Assert.IsFalse(notificationsViewModel.IsSettingsDirtyFromUI);

      notificationsViewModel.IgnoreMinimumDiaphragmMovementValue = true;
      Assert.IsTrue(notificationsViewModel.IsSettingsDirtyFromUI);

      Assert.IsTrue(notificationsViewModel.CanSavePreferences);
      Assert.IsTrue(notificationsViewModel.CanLoadDefault);
    }

    [TestMethod]
    public void IsSettingsDirtyFromUI_AblationDurationType_Test()
    {
      var therapyViewModelMock = new Mock<ICryoTherapyViewModel>();
      var initAblationDurationType = Enumeration.AblationDurationType.FixedTimer;
      therapyViewModelMock.Setup(x => x.AblationDurationType).Returns(initAblationDurationType);
      therapyViewModelMock.Setup(x => x.IsUserAllowedToChangeAblationTimers).Returns(true);
      var notificationsViewModel = new NotificationsViewModel(therapyViewModelMock.Object);

      Assert.AreEqual(initAblationDurationType, notificationsViewModel.AblationDurationType);
      Assert.IsFalse(notificationsViewModel.IsSettingsDirtyFromUI);

      notificationsViewModel.AblationDurationType = Enumeration.AblationDurationType.TTIDurationTimer;
      Assert.IsTrue(notificationsViewModel.IsSettingsDirtyFromUI);
      Assert.AreEqual(Enumeration.AblationDurationType.TTIDurationTimer, notificationsViewModel.AblationDurationType);

      Assert.IsTrue(notificationsViewModel.CanSavePreferences);
      Assert.IsTrue(notificationsViewModel.CanLoadDefault);
    }

    [TestMethod]
    public void IsSettingsDirtyFromUI_CanDisplayShadowGraph_Test()
    {
      var therapyViewModelMock = new Mock<ICryoTherapyViewModel>();
      therapyViewModelMock.Setup(x => x.CanDisplayShadowGraph).Returns(true);
      therapyViewModelMock.Setup(x => x.IsUserAllowedToChangeAblationTimers).Returns(true);
      var notificationsViewModel = new NotificationsViewModel(therapyViewModelMock.Object);

      Assert.IsTrue(notificationsViewModel.CanDisplayShadowGraph);
      Assert.IsFalse(notificationsViewModel.IsSettingsDirtyFromUI);

      notificationsViewModel.CanDisplayShadowGraph = false;
      Assert.IsTrue(notificationsViewModel.IsSettingsDirtyFromUI);
      Assert.IsFalse(notificationsViewModel.CanDisplayShadowGraph);

      Assert.IsTrue(notificationsViewModel.CanSavePreferences);
      Assert.IsTrue(notificationsViewModel.CanLoadDefault);
    }

    [TestMethod]
    public void IsSettingsDirtyFromUI_AblationTimer_Test()
    {
      var therapyViewModelMock = new Mock<ICryoTherapyViewModel>();
      var initAblationTimer = 100;
      therapyViewModelMock.Setup(x => x.AblationTimer).Returns(initAblationTimer);
      therapyViewModelMock.Setup(x => x.IsUserAllowedToChangeAblationTimers).Returns(true);
      var notificationsViewModel = new NotificationsViewModel(therapyViewModelMock.Object);

      Assert.AreEqual(initAblationTimer, notificationsViewModel.AblationTimer);
      Assert.IsFalse(notificationsViewModel.IsSettingsDirtyFromUI);

      notificationsViewModel.AblationTimer = 200;
      Assert.IsTrue(notificationsViewModel.IsSettingsDirtyFromUI);
      Assert.AreEqual(200, notificationsViewModel.AblationTimer);

      Assert.IsTrue(notificationsViewModel.CanSavePreferences);
      Assert.IsTrue(notificationsViewModel.CanLoadDefault);
    }

    [TestMethod]
    public void IsSettingsDirtyFromUI_NewAblationTimerTTI_Test()
    {
      var therapyViewModelMock = new Mock<ICryoTherapyViewModel>();
      var initTimer = 100;
      therapyViewModelMock.Setup(x => x.NewAblationTimerTTI).Returns(initTimer);
      therapyViewModelMock.Setup(x => x.IsUserAllowedToChangeAblationTimers).Returns(true);
      var notificationsViewModel = new NotificationsViewModel(therapyViewModelMock.Object);

      Assert.AreEqual(initTimer, notificationsViewModel.NewAblationTimerTTI);
      Assert.IsFalse(notificationsViewModel.IsSettingsDirtyFromUI);

      notificationsViewModel.NewAblationTimerTTI = 200;
      Assert.IsTrue(notificationsViewModel.IsSettingsDirtyFromUI);
      Assert.AreEqual(200, notificationsViewModel.NewAblationTimerTTI);

      Assert.IsTrue(notificationsViewModel.CanSavePreferences);
      Assert.IsTrue(notificationsViewModel.CanLoadDefault);
    }

    [TestMethod]
    public void IsSettingsDirtyFromUI_AblationTimerTTI_Test()
    {
      var therapyViewModelMock = new Mock<ICryoTherapyViewModel>();
      var initTimer = 100;
      therapyViewModelMock.Setup(x => x.AblationTimerTTI).Returns(initTimer);
      therapyViewModelMock.Setup(x => x.IsUserAllowedToChangeAblationTimers).Returns(true);
      var notificationsViewModel = new NotificationsViewModel(therapyViewModelMock.Object);

      Assert.AreEqual(initTimer, notificationsViewModel.AblationTimerTTI);
      Assert.IsFalse(notificationsViewModel.IsSettingsDirtyFromUI);

      notificationsViewModel.AblationTimerTTI = 200;
      Assert.IsTrue(notificationsViewModel.IsSettingsDirtyFromUI);
      Assert.AreEqual(200, notificationsViewModel.AblationTimerTTI);

      Assert.IsTrue(notificationsViewModel.CanSavePreferences);
      Assert.IsTrue(notificationsViewModel.CanLoadDefault);
    }

    [TestMethod]
    public void IsSettingsDirtyFromUI_DurationExpectedVeinIsolationTime_Test()
    {
      var therapyViewModelMock = new Mock<ICryoTherapyViewModel>();
      var initTimer = 100;
      therapyViewModelMock.Setup(x => x.DurationExpectedVeinIsolationTime).Returns(initTimer);
      therapyViewModelMock.Setup(x => x.IsUserAllowedToChangeAblationTimers).Returns(true);
      var notificationsViewModel = new NotificationsViewModel(therapyViewModelMock.Object);

      Assert.AreEqual(initTimer, notificationsViewModel.DurationExpectedVeinIsolationTime);
      Assert.IsFalse(notificationsViewModel.IsSettingsDirtyFromUI);

      notificationsViewModel.DurationExpectedVeinIsolationTime = 200;
      Assert.IsTrue(notificationsViewModel.IsSettingsDirtyFromUI);
      Assert.AreEqual(200, notificationsViewModel.DurationExpectedVeinIsolationTime);

      Assert.IsTrue(notificationsViewModel.CanSavePreferences);
      Assert.IsTrue(notificationsViewModel.CanLoadDefault);
    }

    [TestMethod]
    public void IsSettingsDirtyFromUI_NewAblationTimerTTIFixed_Test()
    {
      var therapyViewModelMock = new Mock<ICryoTherapyViewModel>();
      var initTimer = 100;
      therapyViewModelMock.Setup(x => x.NewAblationTimerTTIFixed).Returns(initTimer);
      therapyViewModelMock.Setup(x => x.IsUserAllowedToChangeAblationTimers).Returns(true);
      var notificationsViewModel = new NotificationsViewModel(therapyViewModelMock.Object);

      Assert.AreEqual(initTimer, notificationsViewModel.NewAblationTimerTTIFixed);
      Assert.IsFalse(notificationsViewModel.IsSettingsDirtyFromUI);

      notificationsViewModel.NewAblationTimerTTIFixed = 200;
      Assert.IsTrue(notificationsViewModel.IsSettingsDirtyFromUI);
      Assert.AreEqual(200, notificationsViewModel.NewAblationTimerTTIFixed);

      Assert.IsTrue(notificationsViewModel.CanSavePreferences);
      Assert.IsTrue(notificationsViewModel.CanLoadDefault);
    }

    [TestMethod]
    public void IsSettingsDirtyFromUI_AblationTimerTTIFixed_Test()
    {
      var therapyViewModelMock = new Mock<ICryoTherapyViewModel>();
      var initTimer = 100;
      therapyViewModelMock.Setup(x => x.AblationTimerTTIFixed).Returns(initTimer);
      therapyViewModelMock.Setup(x => x.IsUserAllowedToChangeAblationTimers).Returns(true);
      var notificationsViewModel = new NotificationsViewModel(therapyViewModelMock.Object);

      Assert.AreEqual(initTimer, notificationsViewModel.AblationTimerTTIFixed);
      Assert.IsFalse(notificationsViewModel.IsSettingsDirtyFromUI);

      notificationsViewModel.AblationTimerTTIFixed = 200;
      Assert.IsTrue(notificationsViewModel.IsSettingsDirtyFromUI);
      Assert.AreEqual(200, notificationsViewModel.AblationTimerTTIFixed);

      Assert.IsTrue(notificationsViewModel.CanSavePreferences);
      Assert.IsTrue(notificationsViewModel.CanLoadDefault);
    }

    [TestMethod]
    public void IsSettingsDirtyFromUI_ExpectedTimeToVeinIsolation_Test()
    {
      var therapyViewModelMock = new Mock<ICryoTherapyViewModel>();
      var initTimer = 100;
      therapyViewModelMock.Setup(x => x.ExpectedTimeToVeinIsolation).Returns(initTimer);
      therapyViewModelMock.Setup(x => x.IsUserAllowedToChangeAblationTimers).Returns(true);
      var notificationsViewModel = new NotificationsViewModel(therapyViewModelMock.Object);

      Assert.AreEqual(initTimer, notificationsViewModel.ExpectedTimeToVeinIsolation);
      Assert.IsFalse(notificationsViewModel.IsSettingsDirtyFromUI);

      notificationsViewModel.ExpectedTimeToVeinIsolation = 200;
      Assert.IsTrue(notificationsViewModel.IsSettingsDirtyFromUI);
      Assert.AreEqual(200, notificationsViewModel.ExpectedTimeToVeinIsolation);

      Assert.IsTrue(notificationsViewModel.CanSavePreferences);
      Assert.IsTrue(notificationsViewModel.CanLoadDefault);
    }

    [TestMethod]
    public void IsSettingsDirtyFromUI_RequiredVolume_Test()
    {
      var therapyViewModelMock = new Mock<ICryoTherapyViewModel>();
      var initTimer = 10u;
      therapyViewModelMock.Setup(x => x.RequiredVolume).Returns(initTimer);
      therapyViewModelMock.Setup(x => x.IsUserAllowedToChangeAblationTimers).Returns(true);
      var notificationsViewModel = new NotificationsViewModel(therapyViewModelMock.Object);

      Assert.AreEqual(initTimer, notificationsViewModel.RequiredVolume);
      Assert.IsFalse(notificationsViewModel.IsSettingsDirtyFromUI);

      notificationsViewModel.RequiredVolume = 20;
      Assert.IsTrue(notificationsViewModel.IsSettingsDirtyFromUI);
      Assert.AreEqual(20u, notificationsViewModel.RequiredVolume);

      Assert.IsTrue(notificationsViewModel.CanSavePreferences);
      Assert.IsTrue(notificationsViewModel.CanLoadDefault);
    }

    [TestMethod]
    public void IsSettingsDirtyFromUI_DeflateAfterThaw_Test()
    {
      var therapyViewModelMock = new Mock<ICryoTherapyViewModel>();
      var initValue = true;
      therapyViewModelMock.Setup(x => x.DeflateAfterThaw).Returns(initValue);
      therapyViewModelMock.Setup(x => x.IsUserAllowedToChangeAblationTimers).Returns(true);
      var notificationsViewModel = new NotificationsViewModel(therapyViewModelMock.Object);

      Assert.AreEqual(initValue, notificationsViewModel.DeflateAfterThaw);
      Assert.IsFalse(notificationsViewModel.IsSettingsDirtyFromUI);

      notificationsViewModel.DeflateAfterThaw = false;
      Assert.IsTrue(notificationsViewModel.IsSettingsDirtyFromUI);
      Assert.AreEqual(false, notificationsViewModel.DeflateAfterThaw);

      Assert.IsTrue(notificationsViewModel.CanSavePreferences);
      Assert.IsTrue(notificationsViewModel.CanLoadDefault);
    }

    [TestMethod]
    public void IsSettingsDirtyFromUI_RefrigerantLevelUnit_Test()
    {
      var therapyViewModelMock = new Mock<ICryoTherapyViewModel>();
      var initValue = Enumeration.RefrigerantUnit.Lbs;
      therapyViewModelMock.Setup(x => x.RefrigerantLevelUnit).Returns((short)initValue);
      therapyViewModelMock.Setup(x => x.IsUserAllowedToChangeAblationTimers).Returns(true);
      var notificationsViewModel = new NotificationsViewModel(therapyViewModelMock.Object);

      Assert.AreEqual(initValue, notificationsViewModel.RefrigerantLevelUnit);
      Assert.IsFalse(notificationsViewModel.IsSettingsDirtyFromUI);

      notificationsViewModel.RefrigerantLevelUnit = Enumeration.RefrigerantUnit.Minute;
      Assert.IsTrue(notificationsViewModel.IsSettingsDirtyFromUI);
      Assert.AreEqual(Enumeration.RefrigerantUnit.Minute, notificationsViewModel.RefrigerantLevelUnit);

      Assert.IsTrue(notificationsViewModel.CanSavePreferences);
      Assert.IsTrue(notificationsViewModel.CanLoadDefault);
    }

    [TestMethod]
    public void IsSettingsDirtyFromUI_CurveStyle_Test()
    {
      var therapyViewModelMock = new Mock<ICryoTherapyViewModel>();
      var initValue = Enumeration.CurveStyle.Line;
      therapyViewModelMock.Setup(x => x.TemperatureChartType).Returns((short)initValue);
      therapyViewModelMock.Setup(x => x.IsUserAllowedToChangeAblationTimers).Returns(true);
      var notificationsViewModel = new NotificationsViewModel(therapyViewModelMock.Object);

      Assert.AreEqual(initValue, notificationsViewModel.CurveStyle);
      Assert.IsFalse(notificationsViewModel.IsSettingsDirtyFromUI);

      notificationsViewModel.CurveStyle = Enumeration.CurveStyle.Area;
      Assert.IsTrue(notificationsViewModel.IsSettingsDirtyFromUI);
      Assert.AreEqual(Enumeration.CurveStyle.Area, notificationsViewModel.CurveStyle);

      Assert.IsTrue(notificationsViewModel.CanSavePreferences);
      Assert.IsTrue(notificationsViewModel.CanLoadDefault);
    }

    [TestMethod]
    public void IsSettingsDirtyFromUI_InflationSpeedMode_Test()
    {
      var therapyViewModelMock = new Mock<ICryoTherapyViewModel>();
      var initValue = Enumeration.InflationSpeedMode.Slow;
      therapyViewModelMock.Setup(x => x.EnableFastInflationMode).Returns(false);
      therapyViewModelMock.Setup(x => x.IsUserAllowedToChangeAblationTimers).Returns(true);
      var notificationsViewModel = new NotificationsViewModel(therapyViewModelMock.Object);

      Assert.AreEqual(initValue, notificationsViewModel.InflationSpeedMode);
      Assert.IsFalse(notificationsViewModel.IsSettingsDirtyFromUI);

      notificationsViewModel.InflationSpeedMode = Enumeration.InflationSpeedMode.Fast;
      Assert.IsTrue(notificationsViewModel.IsSettingsDirtyFromUI);
      Assert.AreEqual(Enumeration.InflationSpeedMode.Fast, notificationsViewModel.InflationSpeedMode);

      Assert.IsTrue(notificationsViewModel.CanSavePreferences);
      Assert.IsTrue(notificationsViewModel.CanLoadDefault);
    }

    [TestMethod]
    public void IsSettingsDirtyFromUI_DiaphragmSensorGain_Test()
    {
      var therapyViewModelMock = new Mock<ICryoTherapyViewModel>();
      var initTimer = 100;
      therapyViewModelMock.Setup(x => x.DiaphragmSensorGain).Returns(initTimer);
      therapyViewModelMock.Setup(x => x.IsUserAllowedToChangeAblationTimers).Returns(true);
      var notificationsViewModel = new NotificationsViewModel(therapyViewModelMock.Object);

      Assert.AreEqual(initTimer, notificationsViewModel.DiaphragmSensorGain);
      Assert.IsFalse(notificationsViewModel.IsSettingsDirtyFromUI);

      notificationsViewModel.DiaphragmSensorGain = 200;
      Assert.IsTrue(notificationsViewModel.IsSettingsDirtyFromUI);
      Assert.AreEqual(200, notificationsViewModel.DiaphragmSensorGain);

      Assert.IsTrue(notificationsViewModel.CanSavePreferences);
      Assert.IsTrue(notificationsViewModel.CanLoadDefault);
    }

    [TestMethod]
    public void IsSettingsDirtyFromUI_IsUsingAutoPlayback_Test()
    {
      var therapyViewModelMock = new Mock<ICryoTherapyViewModel>();
      var initValue = false;
      therapyViewModelMock.Setup(x => x.IsUsingAutoPlayback).Returns(initValue);
      therapyViewModelMock.Setup(x => x.IsUserAllowedToChangeAblationTimers).Returns(true);
      var notificationsViewModel = new NotificationsViewModel(therapyViewModelMock.Object);

      Assert.AreEqual(initValue, notificationsViewModel.IsUsingAutoPlayback);
      Assert.IsFalse(notificationsViewModel.IsSettingsDirtyFromUI);

      notificationsViewModel.IsUsingAutoPlayback = true;
      Assert.IsTrue(notificationsViewModel.IsSettingsDirtyFromUI);
      Assert.AreEqual(true, notificationsViewModel.IsUsingAutoPlayback);

      Assert.IsTrue(notificationsViewModel.CanSavePreferences);
      Assert.IsTrue(notificationsViewModel.CanLoadDefault);
    }

    [TestMethod]
    public void IsSettingsDirtyFromUI_EnableEnhancedAudio_Test()
    {
      var therapyViewModelMock = new Mock<ICryoTherapyViewModel>();
      var initValue = false;
      therapyViewModelMock.Setup(x => x.EnabaleEnhancedAudio).Returns(initValue);
      therapyViewModelMock.Setup(x => x.IsUserAllowedToChangeAblationTimers).Returns(true);
      var notificationsViewModel = new NotificationsViewModel(therapyViewModelMock.Object);

      Assert.AreEqual(initValue, notificationsViewModel.EnableEnhancedAudio);
      Assert.IsFalse(notificationsViewModel.IsSettingsDirtyFromUI);

      notificationsViewModel.EnableEnhancedAudio = true;
      Assert.IsTrue(notificationsViewModel.IsSettingsDirtyFromUI);
      Assert.AreEqual(true, notificationsViewModel.EnableEnhancedAudio);

      Assert.IsTrue(notificationsViewModel.CanSavePreferences);
      Assert.IsTrue(notificationsViewModel.CanLoadDefault);
    }

    [TestMethod]
    public void IsSettingsDirtyFromUI_IsUsingAudioAlert_Test()
    {
      var therapyViewModelMock = new Mock<ICryoTherapyViewModel>();
      var initValue = false;
      therapyViewModelMock.Setup(x => x.IsUsingAudioAlert).Returns(initValue);
      therapyViewModelMock.Setup(x => x.IsUserAllowedToChangeAblationTimers).Returns(true);
      var notificationsViewModel = new NotificationsViewModel(therapyViewModelMock.Object);

      Assert.AreEqual(initValue, notificationsViewModel.IsUsingAudioAlert);
      Assert.IsFalse(notificationsViewModel.IsSettingsDirtyFromUI);

      notificationsViewModel.IsUsingAudioAlert = true;
      Assert.IsTrue(notificationsViewModel.IsSettingsDirtyFromUI);
      Assert.AreEqual(true, notificationsViewModel.IsUsingAudioAlert);

      Assert.IsTrue(notificationsViewModel.CanSavePreferences);
      Assert.IsTrue(notificationsViewModel.CanLoadDefault);
    }

    [TestMethod]
    public void IsSettingsDirtyFromUI_RequiredTargetTemperature_Test()
    {
      var therapyViewModelMock = new Mock<ICryoTherapyViewModel>();
      var initTimer = 100;
      therapyViewModelMock.Setup(x => x.RequiredTargetTemperature).Returns(initTimer);
      therapyViewModelMock.Setup(x => x.IsUserAllowedToChangeAblationTimers).Returns(true);
      var notificationsViewModel = new NotificationsViewModel(therapyViewModelMock.Object);

      Assert.AreEqual(initTimer, notificationsViewModel.RequiredTargetTemperature);
      Assert.IsFalse(notificationsViewModel.IsSettingsDirtyFromUI);

      notificationsViewModel.RequiredTargetTemperature = 200;
      Assert.IsTrue(notificationsViewModel.IsSettingsDirtyFromUI);
      Assert.AreEqual(200, notificationsViewModel.RequiredTargetTemperature);

      Assert.IsTrue(notificationsViewModel.CanSavePreferences);
      Assert.IsTrue(notificationsViewModel.CanLoadDefault);
    }

    [TestMethod]
    public void IsSettingsDirtyFromUI_ThawTimerToTemperature_Test()
    {
      var therapyViewModelMock = new Mock<ICryoTherapyViewModel>();
      var initTimer = 100;
      therapyViewModelMock.Setup(x => x.ThawTimerToTemperature).Returns(initTimer);
      therapyViewModelMock.Setup(x => x.IsUserAllowedToChangeAblationTimers).Returns(true);
      var notificationsViewModel = new NotificationsViewModel(therapyViewModelMock.Object);

      Assert.AreEqual(initTimer, notificationsViewModel.ThawTimerToTemperature);
      Assert.IsFalse(notificationsViewModel.IsSettingsDirtyFromUI);

      notificationsViewModel.ThawTimerToTemperature = 200;
      Assert.IsTrue(notificationsViewModel.IsSettingsDirtyFromUI);
      Assert.AreEqual(200, notificationsViewModel.ThawTimerToTemperature);

      Assert.IsTrue(notificationsViewModel.CanSavePreferences);
      Assert.IsTrue(notificationsViewModel.CanLoadDefault);
    }

    [TestMethod]
    public void IsSettingsDirtyFromUI_LowAblationTemperatureAlarm_Test()
    {
      var therapyViewModelMock = new Mock<ICryoTherapyViewModel>();
      var initTimer = 100;
      therapyViewModelMock.Setup(x => x.LowAblationTemperatureAlarm).Returns(initTimer);
      therapyViewModelMock.Setup(x => x.IsUserAllowedToChangeAblationTimers).Returns(true);
      var notificationsViewModel = new NotificationsViewModel(therapyViewModelMock.Object);

      Assert.AreEqual(initTimer, notificationsViewModel.LowAblationTemperatureAlarm);
      Assert.IsFalse(notificationsViewModel.IsSettingsDirtyFromUI);

      notificationsViewModel.LowAblationTemperatureAlarm = 200;
      Assert.IsTrue(notificationsViewModel.IsSettingsDirtyFromUI);
      Assert.AreEqual(200, notificationsViewModel.LowAblationTemperatureAlarm);

      Assert.IsTrue(notificationsViewModel.CanSavePreferences);
      Assert.IsTrue(notificationsViewModel.CanLoadDefault);
    }

    [TestMethod]
    public void IsSettingsDirtyFromUI_HighAblationTemperatureAlarm_Test()
    {
      var therapyViewModelMock = new Mock<ICryoTherapyViewModel>();
      var initTimer = 100;
      therapyViewModelMock.Setup(x => x.HighAblationTemperatureAlarm).Returns(initTimer);
      therapyViewModelMock.Setup(x => x.IsUserAllowedToChangeAblationTimers).Returns(true);
      var notificationsViewModel = new NotificationsViewModel(therapyViewModelMock.Object);

      Assert.AreEqual(initTimer, notificationsViewModel.HighAblationTemperatureAlarm);
      Assert.IsFalse(notificationsViewModel.IsSettingsDirtyFromUI);

      notificationsViewModel.HighAblationTemperatureAlarm = 200;
      Assert.IsTrue(notificationsViewModel.IsSettingsDirtyFromUI);
      Assert.AreEqual(200, notificationsViewModel.HighAblationTemperatureAlarm);

      Assert.IsTrue(notificationsViewModel.CanSavePreferences);
      Assert.IsTrue(notificationsViewModel.CanLoadDefault);
    }

    [TestMethod]
    public void IsSettingsDirtyFromUI_EsophagusTemperature_Test()
    {
      var therapyViewModelMock = new Mock<ICryoTherapyViewModel>();
      var initTimer = 100;
      therapyViewModelMock.Setup(x => x.EsophagusTemperature).Returns(initTimer);
      therapyViewModelMock.Setup(x => x.IsUserAllowedToChangeAblationTimers).Returns(true);
      var notificationsViewModel = new NotificationsViewModel(therapyViewModelMock.Object);

      Assert.AreEqual(initTimer, notificationsViewModel.EsophagusTemperature);
      Assert.IsFalse(notificationsViewModel.IsSettingsDirtyFromUI);

      notificationsViewModel.EsophagusTemperature = 200;
      Assert.IsTrue(notificationsViewModel.IsSettingsDirtyFromUI);
      Assert.AreEqual(200, notificationsViewModel.EsophagusTemperature);

      Assert.IsTrue(notificationsViewModel.CanSavePreferences);
      Assert.IsTrue(notificationsViewModel.CanLoadDefault);
    }

    [TestMethod]
    public void IsSettingsDirtyFromUI_DiaphragmAmplitude_Test()
    {
      var therapyViewModelMock = new Mock<ICryoTherapyViewModel>();
      var initTimer = 100;
      therapyViewModelMock.Setup(x => x.DiaphragmAmplitude).Returns(initTimer);
      therapyViewModelMock.Setup(x => x.IsUserAllowedToChangeAblationTimers).Returns(true);
      var notificationsViewModel = new NotificationsViewModel(therapyViewModelMock.Object);

      Assert.AreEqual(initTimer, notificationsViewModel.DiaphragmAmplitude);
      Assert.IsFalse(notificationsViewModel.IsSettingsDirtyFromUI);

      notificationsViewModel.DiaphragmAmplitude = 200;
      Assert.IsTrue(notificationsViewModel.IsSettingsDirtyFromUI);
      Assert.AreEqual(200, notificationsViewModel.DiaphragmAmplitude);

      Assert.IsTrue(notificationsViewModel.CanSavePreferences);
      Assert.IsTrue(notificationsViewModel.CanLoadDefault);
    }

    [TestMethod]
    public void IsSettingsDirtyFromUI_DMSDetectionThreshold_Test()
    {
      var therapyViewModelMock = new Mock<ICryoTherapyViewModel>();
      var initTimer = 100;
      therapyViewModelMock.Setup(x => x.DMSDetectionThreshold).Returns(initTimer);
      therapyViewModelMock.Setup(x => x.IsUserAllowedToChangeAblationTimers).Returns(true);
      var notificationsViewModel = new NotificationsViewModel(therapyViewModelMock.Object);

      Assert.AreEqual(initTimer, notificationsViewModel.DMSDetectionThreshold);
      Assert.IsFalse(notificationsViewModel.IsSettingsDirtyFromUI);

      notificationsViewModel.DMSDetectionThreshold = 200;
      Assert.IsTrue(notificationsViewModel.IsSettingsDirtyFromUI);
      Assert.AreEqual(200, notificationsViewModel.DMSDetectionThreshold);

      Assert.IsTrue(notificationsViewModel.CanSavePreferences);
      Assert.IsTrue(notificationsViewModel.CanLoadDefault);
    }

    [TestMethod]
    public void IsSettingsDirtyFromUI_DMSDetectionThresholdValue_Test()
    {
      var therapyViewModelMock = new Mock<ICryoTherapyViewModel>();
      var initTimer = 100;
      therapyViewModelMock.Setup(x => x.DMSDetectionThresholdValue).Returns(initTimer);
      therapyViewModelMock.Setup(x => x.IsUserAllowedToChangeAblationTimers).Returns(true);
      var notificationsViewModel = new NotificationsViewModel(therapyViewModelMock.Object);

      Assert.AreEqual(initTimer, notificationsViewModel.DMSDetectionThresholdValue);
      Assert.IsFalse(notificationsViewModel.IsSettingsDirtyFromUI);

      notificationsViewModel.DMSDetectionThresholdValue = 200;
      Assert.IsTrue(notificationsViewModel.IsSettingsDirtyFromUI);
      Assert.AreEqual(200, notificationsViewModel.DMSDetectionThresholdValue);

      Assert.IsTrue(notificationsViewModel.CanSavePreferences);
      Assert.IsTrue(notificationsViewModel.CanLoadDefault);
    }

    #endregion tests for updating IsSettingsDirtyFromUI when any setting changed 

    #region Tests: CanLoadDefault is disabled if IsUserAllowedToChangeAblationTimers == false

    [TestMethod]
    public void CanLoadDefault_When_IsDirtyFromDB_True_Test()
    {
      var therapyViewModelMock = new Mock<ICryoTherapyViewModel>();
      therapyViewModelMock.Setup(x => x.IsSettingsDirty).Returns(true);
      therapyViewModelMock.Setup(x => x.IsUserAllowedToChangeAblationTimers).Returns(true);
      var notificationsViewModel = new NotificationsViewModel(therapyViewModelMock.Object);

      Assert.IsTrue(notificationsViewModel.IsSettingsDirtyFromDB);
      Assert.IsFalse(notificationsViewModel.IsSettingsDirtyFromUI);

      Assert.IsTrue(notificationsViewModel.CanSavePreferences);
      Assert.IsTrue(notificationsViewModel.CanLoadDefault);
    }

    [TestMethod]
    public void CanLoadDefault_When_NotIsDirtyFromDB_False_Test()
    {
      var therapyViewModelMock = new Mock<ICryoTherapyViewModel>();
      therapyViewModelMock.Setup(x => x.IsSettingsDirty).Returns(false);
      therapyViewModelMock.Setup(x => x.IsUserAllowedToChangeAblationTimers).Returns(true);
      var notificationsViewModel = new NotificationsViewModel(therapyViewModelMock.Object);

      Assert.IsFalse(notificationsViewModel.IsSettingsDirtyFromDB);
      Assert.IsFalse(notificationsViewModel.IsSettingsDirtyFromUI);

      Assert.IsFalse(notificationsViewModel.CanSavePreferences);
      Assert.IsFalse(notificationsViewModel.CanLoadDefault);
    }

    [TestMethod]
    public void CanLoadDefault_When_IsDirtyFromDB_But_NotIsUserAllowedToChangeAblationTimers_False_Test()
    {
      var therapyViewModelMock = new Mock<ICryoTherapyViewModel>();
      therapyViewModelMock.Setup(x => x.IsSettingsDirty).Returns(true);
      therapyViewModelMock.Setup(x => x.CanDisplayShadowGraph).Returns(false);
      therapyViewModelMock.Setup(x => x.IsUserAllowedToChangeAblationTimers).Returns(false);
      var notificationsViewModel = new NotificationsViewModel(therapyViewModelMock.Object);

      Assert.IsTrue(notificationsViewModel.IsSettingsDirtyFromDB);
      Assert.IsFalse(notificationsViewModel.IsSettingsDirtyFromUI);

      notificationsViewModel.CanDisplayShadowGraph = true;
      Assert.IsTrue(notificationsViewModel.IsSettingsDirtyFromUI);

      Assert.IsTrue(notificationsViewModel.CanSavePreferences);
      Assert.IsFalse(notificationsViewModel.CanLoadDefault);
    }

    #endregion Tests: CanLoadDefault is disabled if IsUserAllowedToChangeAblationTimers == false

  }
}
