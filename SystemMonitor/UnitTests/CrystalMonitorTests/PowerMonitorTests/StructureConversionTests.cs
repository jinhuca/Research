using System.Collections.Generic;
using CrystalMonitor.Interop.PowerMonitor;
using Xunit;

namespace CrystalMonitorTests.InteropTests.PowerMonitorTests;

public class StructureConversionTests {
  // ---------------------------------------------------------------------
  // Sample fixture builders. Every field gets a distinct, recognizable
  // value so a transposed-field bug (e.g. swapping two same-typed
  // properties in the production mapping) would show up as a failure
  // rather than passing by coincidence.
  // ---------------------------------------------------------------------

  private static byte[] SampleFriendlyName() {
    byte[] name = new byte[32];
    byte[] text = System.Text.Encoding.ASCII.GetBytes("WireView Pro 2 Test Device");
    System.Array.Copy(text, name, text.Length);
    return name;
  }

  private static FanConfigStruct SampleFanConfig() => new FanConfigStruct {
    Mode = FanMode.FanModeCurve,
    TempSource = TempSource.TempSourceTsOut,
    DutyMin = 20,
    DutyMax = 100,
    TempMin = 300,
    TempMax = 700
  };

  private static UiConfigStructV1 SampleUiV1(Theme theme) => new UiConfigStructV1 {
    CurrentScale = CurrentScale.CurrentScale15A,
    PowerScale = PowerScale.PowerScale600W,
    Theme = theme,
    DisplayRotation = DisplayRotation.DisplayRotation180,
    TimeoutMode = TimeoutMode.TimeoutModeCycle,
    CycleScreens = 0b0001_0111,
    CycleTime = 5,
    Timeout = 30
  };

  private static DeviceConfigStructV1 SampleConfigV1(Theme theme = Theme.ThemeTg1) => new DeviceConfigStructV1 {
    Crc = 0xABCD,
    Version = 3,
    FriendlyName = SampleFriendlyName(),
    FanConfig = SampleFanConfig(),
    BacklightDuty = 80,
    FaultDisplayEnable = 1,
    FaultBuzzerEnable = 0,
    FaultSoftPowerEnable = 1,
    FaultHardPowerEnable = 0,
    TsFaultThreshold = 950,
    OcpFaultThreshold = 65,
    WireOcpFaultThreshold = 15,
    OppFaultThreshold = 700,
    CurrentImbalanceFaultThreshold = 25,
    CurrentImbalanceFaultMinLoad = 8,
    ShutdownWaitTime = 12,
    LoggingInterval = 4,
    Ui = SampleUiV1(theme)
  };

  private static DeviceConfigStructV2 SampleConfigV2(Theme theme = Theme.ThemeTg2, AVG avg = AVG.AVG_354MS) => new DeviceConfigStructV2 {
    Crc = 0x1357,
    Version = 4,
    FriendlyName = SampleFriendlyName(),
    FanConfig = SampleFanConfig(),
    BacklightDuty = 60,
    FaultDisplayEnable = 0,
    FaultBuzzerEnable = 1,
    FaultSoftPowerEnable = 0,
    FaultHardPowerEnable = 1,
    TsFaultThreshold = 880,
    OcpFaultThreshold = 70,
    WireOcpFaultThreshold = 18,
    OppFaultThreshold = 600,
    CurrentImbalanceFaultThreshold = 15,
    CurrentImbalanceFaultMinLoad = 6,
    ShutdownWaitTime = 9,
    LoggingInterval = 3,
    Average = avg,
    Ui = SampleUiV1(theme)
  };

  private static DeviceConfigStructV3 SampleConfigV3(byte backgroundBitmapId, AVG avg = AVG.AVG_177MS) => new DeviceConfigStructV3 {
    Crc = 0x2468,
    Version = 5,
    FriendlyName = SampleFriendlyName(),
    FanConfig = SampleFanConfig(),
    BacklightDuty = 45,
    FaultDisplayEnable = 1,
    FaultBuzzerEnable = 1,
    FaultSoftPowerEnable = 0,
    FaultHardPowerEnable = 0,
    TsFaultThreshold = 720,
    OcpFaultThreshold = 55,
    WireOcpFaultThreshold = 10,
    OppFaultThreshold = 500,
    CurrentImbalanceFaultThreshold = 18,
    CurrentImbalanceFaultMinLoad = 4,
    ShutdownWaitTime = 8,
    LoggingInterval = 2,
    Average = avg,
    Ui = new UiConfigStructV2 {
      DefaultScreen = Screen.ScreenStatus,
      CurrentScale = CurrentScale.CurrentScale20A,
      PowerScale = PowerScale.PowerScale300W,
      DisplayRotation = DisplayRotation.DisplayRotation0,
      TimeoutMode = TimeoutMode.TimeoutModeSleep,
      CycleScreens = 0b0000_1111,
      CycleTime = 7,
      Timeout = 20,
      PrimaryColor = 0x11111111,
      SecondaryColor = 0x22222222,
      HighlightColor = 0x33333333,
      BackgroundColor = 0x44444444,
      BackgroundBitmapId = backgroundBitmapId,
      FanBitmapId = 0x64,
      DisplayInversion = DISPLAY_INVERSION.DISPLAY_INVERSION_ON
    }
  };

  private static void AssertFanConfigEqual(FanConfigStruct expected, FanConfigStruct actual) {
    Assert.Equal(expected.Mode, actual.Mode);
    Assert.Equal(expected.TempSource, actual.TempSource);
    Assert.Equal(expected.DutyMin, actual.DutyMin);
    Assert.Equal(expected.DutyMax, actual.DutyMax);
    Assert.Equal(expected.TempMin, actual.TempMin);
    Assert.Equal(expected.TempMax, actual.TempMax);
  }

  private static void AssertUiV1Equal(UiConfigStructV1 expected, UiConfigStructV1 actual) {
    Assert.Equal(expected.CurrentScale, actual.CurrentScale);
    Assert.Equal(expected.PowerScale, actual.PowerScale);
    Assert.Equal(expected.Theme, actual.Theme);
    Assert.Equal(expected.DisplayRotation, actual.DisplayRotation);
    Assert.Equal(expected.TimeoutMode, actual.TimeoutMode);
    Assert.Equal(expected.CycleScreens, actual.CycleScreens);
    Assert.Equal(expected.CycleTime, actual.CycleTime);
    Assert.Equal(expected.Timeout, actual.Timeout);
  }

  // =======================================================================
  // ConvertConfigV1ToV2
  // =======================================================================

  [Fact]
  public void ConvertConfigV1ToV2_CopiesAllSharedFields() {
    DeviceConfigStructV1 v1 = SampleConfigV1(Theme.ThemeTg1);
    DeviceConfigStructV2 v2 = StructureConversion.ConvertConfigV1ToV2(v1);

    Assert.Equal(v1.Crc, v2.Crc);
    Assert.Equal(v1.Version, v2.Version);
    Assert.Equal(v1.FriendlyName, v2.FriendlyName);
    AssertFanConfigEqual(v1.FanConfig, v2.FanConfig);
    Assert.Equal(v1.BacklightDuty, v2.BacklightDuty);
    Assert.Equal(v1.FaultDisplayEnable, v2.FaultDisplayEnable);
    Assert.Equal(v1.FaultBuzzerEnable, v2.FaultBuzzerEnable);
    Assert.Equal(v1.FaultSoftPowerEnable, v2.FaultSoftPowerEnable);
    Assert.Equal(v1.FaultHardPowerEnable, v2.FaultHardPowerEnable);
    Assert.Equal(v1.TsFaultThreshold, v2.TsFaultThreshold);
    Assert.Equal(v1.OcpFaultThreshold, v2.OcpFaultThreshold);
    Assert.Equal(v1.WireOcpFaultThreshold, v2.WireOcpFaultThreshold);
    Assert.Equal(v1.OppFaultThreshold, v2.OppFaultThreshold);
    Assert.Equal(v1.CurrentImbalanceFaultThreshold, v2.CurrentImbalanceFaultThreshold);
    Assert.Equal(v1.CurrentImbalanceFaultMinLoad, v2.CurrentImbalanceFaultMinLoad);
    Assert.Equal(v1.ShutdownWaitTime, v2.ShutdownWaitTime);
    Assert.Equal(v1.LoggingInterval, v2.LoggingInterval);
    AssertUiV1Equal(v1.Ui, v2.Ui);
  }

  [Theory]
  [InlineData(Theme.ThemeTg1)]
  [InlineData(Theme.ThemeTg2)]
  [InlineData(Theme.ThemeTg3)]
  public void ConvertConfigV1ToV2_AlwaysSetsDefaultAverage_RegardlessOfV1Content(Theme theme) {
    // DeviceConfigStructV1 has no Average field at all, so V2's Average is
    // always synthesized to a fixed default rather than derived from V1.
    DeviceConfigStructV1 v1 = SampleConfigV1(theme);
    DeviceConfigStructV2 v2 = StructureConversion.ConvertConfigV1ToV2(v1);
    Assert.Equal(AVG.AVG_1417MS, v2.Average);
  }

  // =======================================================================
  // ConvertConfigV2ToV1
  // =======================================================================

  [Fact]
  public void ConvertConfigV2ToV1_CopiesAllSharedFields() {
    DeviceConfigStructV2 v2 = SampleConfigV2();
    DeviceConfigStructV1 v1 = StructureConversion.ConvertConfigV2ToV1(v2);

    Assert.Equal(v2.Crc, v1.Crc);
    Assert.Equal(v2.Version, v1.Version);
    Assert.Equal(v2.FriendlyName, v1.FriendlyName);
    AssertFanConfigEqual(v2.FanConfig, v1.FanConfig);
    Assert.Equal(v2.BacklightDuty, v1.BacklightDuty);
    Assert.Equal(v2.FaultDisplayEnable, v1.FaultDisplayEnable);
    Assert.Equal(v2.FaultBuzzerEnable, v1.FaultBuzzerEnable);
    Assert.Equal(v2.FaultSoftPowerEnable, v1.FaultSoftPowerEnable);
    Assert.Equal(v2.FaultHardPowerEnable, v1.FaultHardPowerEnable);
    Assert.Equal(v2.TsFaultThreshold, v1.TsFaultThreshold);
    Assert.Equal(v2.OcpFaultThreshold, v1.OcpFaultThreshold);
    Assert.Equal(v2.WireOcpFaultThreshold, v1.WireOcpFaultThreshold);
    Assert.Equal(v2.OppFaultThreshold, v1.OppFaultThreshold);
    Assert.Equal(v2.CurrentImbalanceFaultThreshold, v1.CurrentImbalanceFaultThreshold);
    Assert.Equal(v2.CurrentImbalanceFaultMinLoad, v1.CurrentImbalanceFaultMinLoad);
    Assert.Equal(v2.ShutdownWaitTime, v1.ShutdownWaitTime);
    Assert.Equal(v2.LoggingInterval, v1.LoggingInterval);
    AssertUiV1Equal(v2.Ui, v1.Ui);
  }

  // =======================================================================
  // V1 <-> V2 round trip
  // =======================================================================

  [Theory]
  [InlineData(Theme.ThemeTg1)]
  [InlineData(Theme.ThemeTg2)]
  [InlineData(Theme.ThemeTg3)]
  public void V1ToV2ToV1_RoundTrip_PreservesAllOriginalFields(Theme theme) {
    // V1 -> V2 only adds a field (Average); it never discards anything, so
    // converting back to V1 should reproduce the original exactly.
    DeviceConfigStructV1 original = SampleConfigV1(theme);
    DeviceConfigStructV1 roundTripped = StructureConversion.ConvertConfigV2ToV1(StructureConversion.ConvertConfigV1ToV2(original));

    Assert.Equal(original.Crc, roundTripped.Crc);
    Assert.Equal(original.Version, roundTripped.Version);
    Assert.Equal(original.FriendlyName, roundTripped.FriendlyName);
    AssertFanConfigEqual(original.FanConfig, roundTripped.FanConfig);
    Assert.Equal(original.BacklightDuty, roundTripped.BacklightDuty);
    Assert.Equal(original.TsFaultThreshold, roundTripped.TsFaultThreshold);
    Assert.Equal(original.OppFaultThreshold, roundTripped.OppFaultThreshold);
    Assert.Equal(original.ShutdownWaitTime, roundTripped.ShutdownWaitTime);
    Assert.Equal(original.LoggingInterval, roundTripped.LoggingInterval);
    AssertUiV1Equal(original.Ui, roundTripped.Ui);
  }

  // =======================================================================
  // ConvertConfigV2ToV3 — theme-derived colors and bitmap IDs
  // =======================================================================

  public static IEnumerable<object[]> ThemeColorAndBitmapCases() {
    yield return new object[] {
      Theme.ThemeTg1,
      WireViewPro2Constants.THEME_PRIMARY_COLOR_TG1, WireViewPro2Constants.THEME_SECONDARY_COLOR_TG1,
      WireViewPro2Constants.THEME_HIGHLIGHT_COLOR_TG1, WireViewPro2Constants.THEME_BACKGROUND_COLOR_TG1,
      (byte)THEME_BACKGROUND.ThermalGrizzlyOrange, (byte)THEME_FAN.ThermalGrizzlyOrange
    };
    yield return new object[] {
      Theme.ThemeTg2,
      WireViewPro2Constants.THEME_PRIMARY_COLOR_TG2, WireViewPro2Constants.THEME_SECONDARY_COLOR_TG2,
      WireViewPro2Constants.THEME_HIGHLIGHT_COLOR_TG2, WireViewPro2Constants.THEME_BACKGROUND_COLOR_TG2,
      (byte)THEME_BACKGROUND.ThermalGrizzlyDark, (byte)THEME_FAN.ThermalGrizzlyDark
    };
    yield return new object[] {
      // Anything other than Tg1/Tg2 falls through to the Tg3 branch,
      // exercised here with the actual remaining defined enum value.
      Theme.ThemeTg3,
      WireViewPro2Constants.THEME_PRIMARY_COLOR_TG3, WireViewPro2Constants.THEME_SECONDARY_COLOR_TG3,
      WireViewPro2Constants.THEME_HIGHLIGHT_COLOR_TG3, WireViewPro2Constants.THEME_BACKGROUND_COLOR_TG3,
      (byte)THEME_BACKGROUND.Disabled, (byte)THEME_FAN.ThermalGrizzlyBlackWhite
    };
  }

  [Theory]
  [MemberData(nameof(ThemeColorAndBitmapCases))]
  public void ConvertConfigV2ToV3_DerivesCorrectColorsAndBitmapsForTheme(
      Theme theme, uint expectedPrimary, uint expectedSecondary, uint expectedHighlight, uint expectedBackground,
      byte expectedBackgroundBitmap, byte expectedFanBitmap) {
    DeviceConfigStructV2 v2 = SampleConfigV2(theme);
    DeviceConfigStructV3 v3 = StructureConversion.ConvertConfigV2ToV3(v2);

    Assert.Equal(expectedPrimary, v3.Ui.PrimaryColor);
    Assert.Equal(expectedSecondary, v3.Ui.SecondaryColor);
    Assert.Equal(expectedHighlight, v3.Ui.HighlightColor);
    Assert.Equal(expectedBackground, v3.Ui.BackgroundColor);
    Assert.Equal(expectedBackgroundBitmap, v3.Ui.BackgroundBitmapId);
    Assert.Equal(expectedFanBitmap, v3.Ui.FanBitmapId);
  }

  [Fact]
  public void ConvertConfigV2ToV3_AlwaysSetsDefaultScreenAndDisplayInversionToDefaults() {
    DeviceConfigStructV2 v2 = SampleConfigV2(Theme.ThemeTg3);
    DeviceConfigStructV3 v3 = StructureConversion.ConvertConfigV2ToV3(v2);

    Assert.Equal(Screen.ScreenMain, v3.Ui.DefaultScreen);
    Assert.Equal(DISPLAY_INVERSION.DISPLAY_INVERSION_OFF, v3.Ui.DisplayInversion);
  }

  [Fact]
  public void ConvertConfigV2ToV3_PassesThroughCoreAndNonThemeUiFields() {
    DeviceConfigStructV2 v2 = SampleConfigV2(Theme.ThemeTg1);
    DeviceConfigStructV3 v3 = StructureConversion.ConvertConfigV2ToV3(v2);

    Assert.Equal(v2.Crc, v3.Crc);
    Assert.Equal(v2.Version, v3.Version);
    Assert.Equal(v2.FriendlyName, v3.FriendlyName);
    AssertFanConfigEqual(v2.FanConfig, v3.FanConfig);
    Assert.Equal(v2.BacklightDuty, v3.BacklightDuty);
    Assert.Equal(v2.FaultDisplayEnable, v3.FaultDisplayEnable);
    Assert.Equal(v2.FaultBuzzerEnable, v3.FaultBuzzerEnable);
    Assert.Equal(v2.FaultSoftPowerEnable, v3.FaultSoftPowerEnable);
    Assert.Equal(v2.FaultHardPowerEnable, v3.FaultHardPowerEnable);
    Assert.Equal(v2.TsFaultThreshold, v3.TsFaultThreshold);
    Assert.Equal(v2.OcpFaultThreshold, v3.OcpFaultThreshold);
    Assert.Equal(v2.WireOcpFaultThreshold, v3.WireOcpFaultThreshold);
    Assert.Equal(v2.OppFaultThreshold, v3.OppFaultThreshold);
    Assert.Equal(v2.CurrentImbalanceFaultThreshold, v3.CurrentImbalanceFaultThreshold);
    Assert.Equal(v2.CurrentImbalanceFaultMinLoad, v3.CurrentImbalanceFaultMinLoad);
    Assert.Equal(v2.ShutdownWaitTime, v3.ShutdownWaitTime);
    Assert.Equal(v2.LoggingInterval, v3.LoggingInterval);
    Assert.Equal(v2.Average, v3.Average);
    Assert.Equal(v2.Ui.CurrentScale, v3.Ui.CurrentScale);
    Assert.Equal(v2.Ui.PowerScale, v3.Ui.PowerScale);
    Assert.Equal(v2.Ui.DisplayRotation, v3.Ui.DisplayRotation);
    Assert.Equal(v2.Ui.TimeoutMode, v3.Ui.TimeoutMode);
    Assert.Equal(v2.Ui.CycleScreens, v3.Ui.CycleScreens);
    Assert.Equal(v2.Ui.CycleTime, v3.Ui.CycleTime);
    Assert.Equal(v2.Ui.Timeout, v3.Ui.Timeout);
  }

  // =======================================================================
  // ConvertConfigV3ToV2 — best-effort theme inference from bitmap ID
  // =======================================================================

  [Theory]
  [InlineData(THEME_BACKGROUND.ThermalGrizzlyOrange, Theme.ThemeTg1)]
  [InlineData(THEME_BACKGROUND.ThermalGrizzlyDark, Theme.ThemeTg2)]
  [InlineData(THEME_BACKGROUND.Disabled, Theme.ThemeTg3)]
  public void ConvertConfigV3ToV2_InfersThemeFromBackgroundBitmapId(THEME_BACKGROUND bitmap, Theme expectedTheme) {
    DeviceConfigStructV3 v3 = SampleConfigV3((byte)bitmap);
    DeviceConfigStructV2 v2 = StructureConversion.ConvertConfigV3ToV2(v3);
    Assert.Equal(expectedTheme, v2.Ui.Theme);
  }

  [Fact]
  public void ConvertConfigV3ToV2_UnrecognizedBitmapId_FallsBackToThemeTg3() {
    // Only ThermalGrizzlyOrange (1) and ThermalGrizzlyDark (2) are checked
    // explicitly; any other byte value — including ones that don't
    // correspond to any defined THEME_BACKGROUND member — falls back to
    // Tg3 as a best-effort default.
    DeviceConfigStructV3 v3 = SampleConfigV3(backgroundBitmapId: 99);
    DeviceConfigStructV2 v2 = StructureConversion.ConvertConfigV3ToV2(v3);
    Assert.Equal(Theme.ThemeTg3, v2.Ui.Theme);
  }

  [Fact]
  public void ConvertConfigV3ToV2_PassesThroughCoreAndSharedUiFields() {
    DeviceConfigStructV3 v3 = SampleConfigV3((byte)THEME_BACKGROUND.ThermalGrizzlyDark);
    DeviceConfigStructV2 v2 = StructureConversion.ConvertConfigV3ToV2(v3);

    Assert.Equal(v3.Crc, v2.Crc);
    Assert.Equal(v3.Version, v2.Version);
    Assert.Equal(v3.FriendlyName, v2.FriendlyName);
    AssertFanConfigEqual(v3.FanConfig, v2.FanConfig);
    Assert.Equal(v3.BacklightDuty, v2.BacklightDuty);
    Assert.Equal(v3.FaultDisplayEnable, v2.FaultDisplayEnable);
    Assert.Equal(v3.FaultBuzzerEnable, v2.FaultBuzzerEnable);
    Assert.Equal(v3.FaultSoftPowerEnable, v2.FaultSoftPowerEnable);
    Assert.Equal(v3.FaultHardPowerEnable, v2.FaultHardPowerEnable);
    Assert.Equal(v3.TsFaultThreshold, v2.TsFaultThreshold);
    Assert.Equal(v3.OcpFaultThreshold, v2.OcpFaultThreshold);
    Assert.Equal(v3.WireOcpFaultThreshold, v2.WireOcpFaultThreshold);
    Assert.Equal(v3.OppFaultThreshold, v2.OppFaultThreshold);
    Assert.Equal(v3.CurrentImbalanceFaultThreshold, v2.CurrentImbalanceFaultThreshold);
    Assert.Equal(v3.CurrentImbalanceFaultMinLoad, v2.CurrentImbalanceFaultMinLoad);
    Assert.Equal(v3.ShutdownWaitTime, v2.ShutdownWaitTime);
    Assert.Equal(v3.LoggingInterval, v2.LoggingInterval);
    Assert.Equal(v3.Average, v2.Average);
    Assert.Equal(v3.Ui.CurrentScale, v2.Ui.CurrentScale);
    Assert.Equal(v3.Ui.PowerScale, v2.Ui.PowerScale);
    Assert.Equal(v3.Ui.DisplayRotation, v2.Ui.DisplayRotation);
    Assert.Equal(v3.Ui.TimeoutMode, v2.Ui.TimeoutMode);
    Assert.Equal(v3.Ui.CycleScreens, v2.Ui.CycleScreens);
    Assert.Equal(v3.Ui.CycleTime, v2.Ui.CycleTime);
    Assert.Equal(v3.Ui.Timeout, v2.Ui.Timeout);
  }

  // =======================================================================
  // V2 <-> V3 round trip
  // =======================================================================

  [Theory]
  [InlineData(Theme.ThemeTg1)]
  [InlineData(Theme.ThemeTg2)]
  [InlineData(Theme.ThemeTg3)]
  public void V2ToV3ToV2_RoundTrip_PreservesThemeAndSharedFields(Theme theme) {
    // Theme is encoded into BackgroundBitmapId going to V3, then decoded
    // back from BackgroundBitmapId going to V2 — for all three defined
    // Theme values this mapping happens to be exactly invertible, even
    // though the derived color/bitmap fields themselves are one-way.
    DeviceConfigStructV2 original = SampleConfigV2(theme);
    DeviceConfigStructV2 roundTripped = StructureConversion.ConvertConfigV3ToV2(StructureConversion.ConvertConfigV2ToV3(original));

    Assert.Equal(original.Crc, roundTripped.Crc);
    Assert.Equal(original.Average, roundTripped.Average);
    Assert.Equal(original.Ui.Theme, roundTripped.Ui.Theme);
    Assert.Equal(original.Ui.CurrentScale, roundTripped.Ui.CurrentScale);
    Assert.Equal(original.Ui.PowerScale, roundTripped.Ui.PowerScale);
    Assert.Equal(original.Ui.DisplayRotation, roundTripped.Ui.DisplayRotation);
    Assert.Equal(original.Ui.TimeoutMode, roundTripped.Ui.TimeoutMode);
    Assert.Equal(original.Ui.CycleScreens, roundTripped.Ui.CycleScreens);
    Assert.Equal(original.Ui.CycleTime, roundTripped.Ui.CycleTime);
    Assert.Equal(original.Ui.Timeout, roundTripped.Ui.Timeout);
  }

  // =======================================================================
  // ConvertConfigV1ToV3 / ConvertConfigV3ToV1 — composition of the above
  // =======================================================================

  [Fact]
  public void ConvertConfigV1ToV3_ComposesV1ToV2ThenV2ToV3() {
    DeviceConfigStructV1 v1 = SampleConfigV1(Theme.ThemeTg2);
    DeviceConfigStructV3 v3 = StructureConversion.ConvertConfigV1ToV3(v1);

    // Average always becomes the V1->V2 default, since V1 never carried
    // one to begin with.
    Assert.Equal(AVG.AVG_1417MS, v3.Average);
    Assert.Equal(v1.Crc, v3.Crc);
    Assert.Equal(v1.FriendlyName, v3.FriendlyName);
    Assert.Equal(v1.Ui.CurrentScale, v3.Ui.CurrentScale);
    Assert.Equal(v1.Ui.Timeout, v3.Ui.Timeout);
    Assert.Equal(Screen.ScreenMain, v3.Ui.DefaultScreen);
    Assert.Equal(WireViewPro2Constants.THEME_PRIMARY_COLOR_TG2, v3.Ui.PrimaryColor);
    Assert.Equal((byte)THEME_BACKGROUND.ThermalGrizzlyDark, v3.Ui.BackgroundBitmapId);
  }

  [Fact]
  public void ConvertConfigV3ToV1_ComposesV3ToV2ThenV2ToV1() {
    DeviceConfigStructV3 v3 = SampleConfigV3((byte)THEME_BACKGROUND.ThermalGrizzlyOrange);
    DeviceConfigStructV1 v1 = StructureConversion.ConvertConfigV3ToV1(v3);

    Assert.Equal(v3.Crc, v1.Crc);
    Assert.Equal(v3.FriendlyName, v1.FriendlyName);
    Assert.Equal(Theme.ThemeTg1, v1.Ui.Theme);
    Assert.Equal(v3.Ui.CurrentScale, v1.Ui.CurrentScale);
    Assert.Equal(v3.Ui.Timeout, v1.Ui.Timeout);
  }
}
