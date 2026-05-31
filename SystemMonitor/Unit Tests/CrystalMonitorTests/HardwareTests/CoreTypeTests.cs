using Xunit;
using CrystalMonitor.Hardware;

namespace CrystalMonitorTests.HardwareTests;

public class CoreTypeTests {
  // -------------------------------------------------------------------------
  // Defined values
  // -------------------------------------------------------------------------

  [Fact]
  public void CoreType_Unknown_HasValueZero() {
    Assert.Equal(0, (int)CoreType.Unknown);
  }

  [Fact]
  public void CoreType_Performance_HasValue0x40() {
    Assert.Equal(0x40, (int)CoreType.Performance);
  }

  [Fact]
  public void CoreType_Efficient_HasValue0x20() {
    Assert.Equal(0x20, (int)CoreType.Efficient);
  }

  // -------------------------------------------------------------------------
  // Cast from int
  // -------------------------------------------------------------------------

  [Fact]
  public void CoreType_CastFromInt_0_IsUnknown() {
    Assert.Equal(CoreType.Unknown, (CoreType)0);
  }

  [Fact]
  public void CoreType_CastFromInt_0x40_IsPerformance() {
    Assert.Equal(CoreType.Performance, (CoreType)0x40);
  }

  [Fact]
  public void CoreType_CastFromInt_0x20_IsEfficient() {
    Assert.Equal(CoreType.Efficient, (CoreType)0x20);
  }

  [Fact]
  public void CoreType_CastFromInt_UnknownValue_DoesNotThrow() {
    // C# enums allow casting any int — undefined values are valid at runtime
    var ex = Record.Exception(() => _ = (CoreType)0xFF);
    Assert.Null(ex);
  }

  // -------------------------------------------------------------------------
  // IsDefined
  // -------------------------------------------------------------------------

  [Fact]
  public void CoreType_Unknown_IsDefined() {
    Assert.True(Enum.IsDefined(typeof(CoreType), CoreType.Unknown));
  }

  [Fact]
  public void CoreType_Performance_IsDefined() {
    Assert.True(Enum.IsDefined(typeof(CoreType), CoreType.Performance));
  }

  [Fact]
  public void CoreType_Efficient_IsDefined() {
    Assert.True(Enum.IsDefined(typeof(CoreType), CoreType.Efficient));
  }

  [Fact]
  public void CoreType_HasExactlyThreeDefinedValues() {
    Assert.Equal(3, Enum.GetValues(typeof(CoreType)).Length);
  }

  // -------------------------------------------------------------------------
  // Parsing
  // -------------------------------------------------------------------------

  [Theory]
  [InlineData("Unknown", CoreType.Unknown)]
  [InlineData("Performance", CoreType.Performance)]
  [InlineData("Efficient", CoreType.Efficient)]
  public void CoreType_Parse_ReturnsCorrectValue(string name, CoreType expected) {
    var result = Enum.Parse<CoreType>(name);
    Assert.Equal(expected, result);
  }

  [Theory]
  [InlineData("unknown", CoreType.Unknown)]
  [InlineData("performance", CoreType.Performance)]
  [InlineData("efficient", CoreType.Efficient)]
  public void CoreType_Parse_CaseInsensitive_ReturnsCorrectValue(string name, CoreType expected) {
    var result = Enum.Parse<CoreType>(name, ignoreCase: true);
    Assert.Equal(expected, result);
  }

  [Fact]
  public void CoreType_Parse_InvalidName_Throws() {
    Assert.Throws<ArgumentException>(() => Enum.Parse<CoreType>("InvalidValue"));
  }

  // -------------------------------------------------------------------------
  // ToString
  // -------------------------------------------------------------------------

  [Fact]
  public void CoreType_Unknown_ToStringIsUnknown() {
    Assert.Equal("Unknown", CoreType.Unknown.ToString());
  }

  [Fact]
  public void CoreType_Performance_ToStringIsPerformance() {
    Assert.Equal("Performance", CoreType.Performance.ToString());
  }

  [Fact]
  public void CoreType_Efficient_ToStringIsEfficient() {
    Assert.Equal("Efficient", CoreType.Efficient.ToString());
  }

  // -------------------------------------------------------------------------
  // Equality and comparison
  // -------------------------------------------------------------------------

  [Fact]
  public void CoreType_SameValues_AreEqual() {
    CoreType a = CoreType.Performance;
    CoreType b = CoreType.Performance;
    Assert.Equal(a, b);
  }

  [Fact]
  public void CoreType_DifferentValues_AreNotEqual() {
    Assert.NotEqual(CoreType.Performance, CoreType.Efficient);
  }

  [Fact]
  public void CoreType_Performance_IsNotUnknown() {
    Assert.NotEqual(CoreType.Unknown, CoreType.Performance);
  }

  [Fact]
  public void CoreType_Efficient_IsNotUnknown() {
    Assert.NotEqual(CoreType.Unknown, CoreType.Efficient);
  }

  // -------------------------------------------------------------------------
  // Usage in switch (mirrors CpuId constructor logic)
  // -------------------------------------------------------------------------

  [Theory]
  [InlineData(0x40, CoreType.Performance)]
  [InlineData(0x20, CoreType.Efficient)]
  [InlineData(0x00, CoreType.Unknown)]
  [InlineData(0xFF, CoreType.Unknown)]
  public void CoreType_SwitchMapping_MatchesCpuIdLogic(int rawValue, CoreType expected) {
    // Mirrors the switch in CpuId constructor:
    // uint coreType = (Data[0x1A, 0] >> 24) & 0xFF;
    // CoreType = coreType switch {
    //   0x40 => CoreType.Performance,
    //   0x20 => CoreType.Efficient,
    //   _    => CoreType.Unknown
    // };
    CoreType result = rawValue switch {
      0x40 => CoreType.Performance,
      0x20 => CoreType.Efficient,
      _ => CoreType.Unknown
    };
    Assert.Equal(expected, result);
  }
}