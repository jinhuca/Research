using Xunit;
using CrystalMonitor.Hardware;

namespace CrystalMonitorTests.HardwareTests;

public class HardwareTypeTests {
  // -------------------------------------------------------------------------
  // Ordinal values
  // -------------------------------------------------------------------------

  [Theory]
  [InlineData(HardwareType.Motherboard, 0)]
  [InlineData(HardwareType.SuperIO, 1)]
  [InlineData(HardwareType.Cpu, 2)]
  [InlineData(HardwareType.Memory, 3)]
  [InlineData(HardwareType.GpuNvidia, 4)]
  [InlineData(HardwareType.GpuAmd, 5)]
  [InlineData(HardwareType.GpuIntel, 6)]
  [InlineData(HardwareType.Storage, 7)]
  [InlineData(HardwareType.Network, 8)]
  [InlineData(HardwareType.Cooler, 9)]
  [InlineData(HardwareType.EmbeddedController, 10)]
  [InlineData(HardwareType.Psu, 11)]
  [InlineData(HardwareType.Battery, 12)]
  [InlineData(HardwareType.PowerMonitor, 13)]
  public void HardwareType_OrdinalValue_IsCorrect(HardwareType type, int expected) {
    Assert.Equal(expected, (int)type);
  }

  // -------------------------------------------------------------------------
  // Member count
  // -------------------------------------------------------------------------

  [Fact]
  public void HardwareType_HasExactly14Members() {
    Assert.Equal(14, Enum.GetValues<HardwareType>().Length);
  }

  // -------------------------------------------------------------------------
  // IsDefined
  // -------------------------------------------------------------------------

  [Theory]
  [InlineData(HardwareType.Motherboard)]
  [InlineData(HardwareType.SuperIO)]
  [InlineData(HardwareType.Cpu)]
  [InlineData(HardwareType.Memory)]
  [InlineData(HardwareType.GpuNvidia)]
  [InlineData(HardwareType.GpuAmd)]
  [InlineData(HardwareType.GpuIntel)]
  [InlineData(HardwareType.Storage)]
  [InlineData(HardwareType.Network)]
  [InlineData(HardwareType.Cooler)]
  [InlineData(HardwareType.EmbeddedController)]
  [InlineData(HardwareType.Psu)]
  [InlineData(HardwareType.Battery)]
  [InlineData(HardwareType.PowerMonitor)]
  public void HardwareType_AllMembers_AreDefined(HardwareType type) {
    Assert.True(Enum.IsDefined(type));
  }

  [Fact]
  public void HardwareType_UndefinedValue_IsNotDefined() {
    Assert.False(Enum.IsDefined((HardwareType)999));
  }

  // -------------------------------------------------------------------------
  // Cast from int
  // -------------------------------------------------------------------------

  [Theory]
  [InlineData(0, HardwareType.Motherboard)]
  [InlineData(1, HardwareType.SuperIO)]
  [InlineData(2, HardwareType.Cpu)]
  [InlineData(3, HardwareType.Memory)]
  [InlineData(4, HardwareType.GpuNvidia)]
  [InlineData(5, HardwareType.GpuAmd)]
  [InlineData(6, HardwareType.GpuIntel)]
  [InlineData(7, HardwareType.Storage)]
  [InlineData(8, HardwareType.Network)]
  [InlineData(9, HardwareType.Cooler)]
  [InlineData(10, HardwareType.EmbeddedController)]
  [InlineData(11, HardwareType.Psu)]
  [InlineData(12, HardwareType.Battery)]
  [InlineData(13, HardwareType.PowerMonitor)]
  public void HardwareType_CastFromInt_ReturnsCorrectMember(int value, HardwareType expected) {
    Assert.Equal(expected, (HardwareType)value);
  }

  [Fact]
  public void HardwareType_CastFromUndefinedInt_DoesNotThrow() {
    var ex = Record.Exception(() => _ = (HardwareType)999);
    Assert.Null(ex);
  }

  // -------------------------------------------------------------------------
  // Parsing
  // -------------------------------------------------------------------------

  [Theory]
  [InlineData("Motherboard", HardwareType.Motherboard)]
  [InlineData("SuperIO", HardwareType.SuperIO)]
  [InlineData("Cpu", HardwareType.Cpu)]
  [InlineData("Memory", HardwareType.Memory)]
  [InlineData("GpuNvidia", HardwareType.GpuNvidia)]
  [InlineData("GpuAmd", HardwareType.GpuAmd)]
  [InlineData("GpuIntel", HardwareType.GpuIntel)]
  [InlineData("Storage", HardwareType.Storage)]
  [InlineData("Network", HardwareType.Network)]
  [InlineData("Cooler", HardwareType.Cooler)]
  [InlineData("EmbeddedController", HardwareType.EmbeddedController)]
  [InlineData("Psu", HardwareType.Psu)]
  [InlineData("Battery", HardwareType.Battery)]
  [InlineData("PowerMonitor", HardwareType.PowerMonitor)]
  public void HardwareType_Parse_ReturnsCorrectMember(string name, HardwareType expected) {
    Assert.Equal(expected, Enum.Parse<HardwareType>(name));
  }

  [Theory]
  [InlineData("motherboard", HardwareType.Motherboard)]
  [InlineData("cpu", HardwareType.Cpu)]
  [InlineData("gpunvidia", HardwareType.GpuNvidia)]
  [InlineData("powermonitor", HardwareType.PowerMonitor)]
  public void HardwareType_Parse_CaseInsensitive_ReturnsCorrectMember(
    string name, HardwareType expected) {
    Assert.Equal(expected, Enum.Parse<HardwareType>(name, ignoreCase: true));
  }

  [Fact]
  public void HardwareType_Parse_InvalidName_Throws() {
    Assert.Throws<ArgumentException>(() => Enum.Parse<HardwareType>("InvalidType"));
  }

  // -------------------------------------------------------------------------
  // ToString
  // -------------------------------------------------------------------------

  [Theory]
  [InlineData(HardwareType.Motherboard, "Motherboard")]
  [InlineData(HardwareType.SuperIO, "SuperIO")]
  [InlineData(HardwareType.Cpu, "Cpu")]
  [InlineData(HardwareType.Memory, "Memory")]
  [InlineData(HardwareType.GpuNvidia, "GpuNvidia")]
  [InlineData(HardwareType.GpuAmd, "GpuAmd")]
  [InlineData(HardwareType.GpuIntel, "GpuIntel")]
  [InlineData(HardwareType.Storage, "Storage")]
  [InlineData(HardwareType.Network, "Network")]
  [InlineData(HardwareType.Cooler, "Cooler")]
  [InlineData(HardwareType.EmbeddedController, "EmbeddedController")]
  [InlineData(HardwareType.Psu, "Psu")]
  [InlineData(HardwareType.Battery, "Battery")]
  [InlineData(HardwareType.PowerMonitor, "PowerMonitor")]
  public void HardwareType_ToString_ReturnsCorrectName(HardwareType type, string expected) {
    Assert.Equal(expected, type.ToString());
  }

  // -------------------------------------------------------------------------
  // Uniqueness
  // -------------------------------------------------------------------------

  [Fact]
  public void HardwareType_AllMembers_HaveUniqueValues() {
    var values = Enum.GetValues<HardwareType>().Select(v => (int)v).ToList();
    Assert.Equal(values.Count, values.Distinct().Count());
  }

  // -------------------------------------------------------------------------
  // Equality
  // -------------------------------------------------------------------------

  [Fact]
  public void HardwareType_SameValue_IsEqual() {
    HardwareType a = HardwareType.Cpu;
    HardwareType b = HardwareType.Cpu;
    Assert.Equal(a, b);
  }

  [Fact]
  public void HardwareType_DifferentValues_AreNotEqual() {
    Assert.NotEqual(HardwareType.Cpu, HardwareType.Memory);
  }

  // -------------------------------------------------------------------------
  // GPU grouping
  // -------------------------------------------------------------------------

  [Fact]
  public void HardwareType_GpuMembers_AreContiguous() {
    Assert.Equal((int)HardwareType.GpuNvidia + 1, (int)HardwareType.GpuAmd);
    Assert.Equal((int)HardwareType.GpuAmd + 1, (int)HardwareType.GpuIntel);
  }

  [Fact]
  public void HardwareType_ThreeGpuTypes_AllDefined() {
    var gpuTypes = Enum.GetValues<HardwareType>()
      .Where(t => t.ToString().StartsWith("Gpu", StringComparison.Ordinal))
      .ToList();

    Assert.Equal(3, gpuTypes.Count);
    Assert.Contains(HardwareType.GpuNvidia, gpuTypes);
    Assert.Contains(HardwareType.GpuAmd, gpuTypes);
    Assert.Contains(HardwareType.GpuIntel, gpuTypes);
  }

  // -------------------------------------------------------------------------
  // Switch grouping (mirrors usage in CpuGroup, Computer, etc.)
  // -------------------------------------------------------------------------

  [Theory]
  [InlineData(HardwareType.Cpu, "CPU")]
  [InlineData(HardwareType.GpuNvidia, "GPU")]
  [InlineData(HardwareType.GpuAmd, "GPU")]
  [InlineData(HardwareType.GpuIntel, "GPU")]
  [InlineData(HardwareType.Memory, "Memory")]
  [InlineData(HardwareType.Storage, "Storage")]
  [InlineData(HardwareType.Network, "Network")]
  [InlineData(HardwareType.Motherboard, "Other")]
  [InlineData(HardwareType.SuperIO, "Other")]
  [InlineData(HardwareType.Cooler, "Other")]
  [InlineData(HardwareType.EmbeddedController, "Other")]
  [InlineData(HardwareType.Psu, "Other")]
  [InlineData(HardwareType.Battery, "Other")]
  [InlineData(HardwareType.PowerMonitor, "Other")]
  public void HardwareType_SwitchGrouping_ReturnsExpectedCategory(
    HardwareType type, string expectedCategory) {
    string category = type switch {
      HardwareType.Cpu => "CPU",
      HardwareType.GpuNvidia
        or HardwareType.GpuAmd
        or HardwareType.GpuIntel => "GPU",
      HardwareType.Memory => "Memory",
      HardwareType.Storage => "Storage",
      HardwareType.Network => "Network",
      _ => "Other"
    };

    Assert.Equal(expectedCategory, category);
  }

  // =========================================================================
  // Comparison Operations
  // =========================================================================

  [Fact]
  public void HardwareType_Comparison_LessThan() {
    Assert.True(HardwareType.Motherboard < HardwareType.Cpu);
    Assert.False(HardwareType.Cpu < HardwareType.Motherboard);
  }

  [Fact]
  public void HardwareType_Comparison_GreaterThan() {
    Assert.True(HardwareType.PowerMonitor > HardwareType.Cpu);
    Assert.False(HardwareType.Cpu > HardwareType.PowerMonitor);
  }

  [Fact]
  public void HardwareType_Comparison_LessThanOrEqual() {
    Assert.True(HardwareType.Cpu <= HardwareType.Cpu);
    Assert.True(HardwareType.Motherboard <= HardwareType.Cpu);
    Assert.False(HardwareType.PowerMonitor <= HardwareType.Cpu);
  }

  [Fact]
  public void HardwareType_Comparison_GreaterThanOrEqual() {
    Assert.True(HardwareType.Cpu >= HardwareType.Cpu);
    Assert.True(HardwareType.PowerMonitor >= HardwareType.Cpu);
    Assert.False(HardwareType.Motherboard >= HardwareType.Cpu);
  }

  // =========================================================================
  // Iteration and Enumeration
  // =========================================================================

  [Fact]
  public void HardwareType_GetValues_ReturnsAllMembers() {
    var values = Enum.GetValues<HardwareType>();
    Assert.Equal(14, values.Length);
  }

  [Fact]
  public void HardwareType_GetNames_ReturnsCorrectCount() {
    var names = Enum.GetNames<HardwareType>();
    Assert.Equal(14, names.Length);
  }

  [Fact]
  public void HardwareType_CanEnumerateAllValues() {
    var allTypes = Enum.GetValues<HardwareType>().ToList();
    var stringRepresentations = allTypes.Select(t => t.ToString()).ToList();

    Assert.Equal(14, stringRepresentations.Count);
    Assert.All(stringRepresentations, s => Assert.NotEmpty(s));
  }

  // =========================================================================
  // Specific Type Categories
  // =========================================================================

  [Fact]
  public void HardwareType_ProcessorTypes_IncludesCpuAndGpus() {
    var processorTypes = new[] {
      HardwareType.Cpu,
      HardwareType.GpuNvidia,
      HardwareType.GpuAmd,
      HardwareType.GpuIntel
    };

    Assert.All(processorTypes, pt => Assert.True(Enum.IsDefined(pt)));
  }

  [Fact]
  public void HardwareType_StorageAndNetwork_AreDefined() {
    Assert.True(Enum.IsDefined(HardwareType.Storage));
    Assert.True(Enum.IsDefined(HardwareType.Network));
  }

  [Fact]
  public void HardwareType_ControllerTypes_IncludesCoolerAndPowerMonitor() {
    var controllerTypes = new[] {
      HardwareType.Cooler,
      HardwareType.PowerMonitor,
      HardwareType.EmbeddedController
    };

    Assert.All(controllerTypes, ct => Assert.True(Enum.IsDefined(ct)));
  }

  // =========================================================================
  // Case Sensitivity Tests
  // =========================================================================

  [Fact]
  public void HardwareType_Parse_CaseSensitive_FailsForWrongCase() {
    Assert.Throws<ArgumentException>(() => 
      Enum.Parse<HardwareType>("cpu", ignoreCase: false));
  }

  [Fact]
  public void HardwareType_Parse_CaseInsensitive_SucceedsForWrongCase() {
    var result = Enum.Parse<HardwareType>("CPU", ignoreCase: true);
    Assert.Equal(HardwareType.Cpu, result);
  }

  [Theory]
  [InlineData("MOTHERBOARD")]
  [InlineData("superio")]
  [InlineData("CpU")]
  [InlineData("mEmOrY")]
  public void HardwareType_Parse_CaseInsensitive_Succeeds(string input) {
    var ex = Record.Exception(() => Enum.Parse<HardwareType>(input, ignoreCase: true));
    Assert.Null(ex);
  }

  // =========================================================================
  // Ordering and Sorting
  // =========================================================================

  [Fact]
  public void HardwareType_CanBeSorted() {
    var types = new[] {
      HardwareType.PowerMonitor,
      HardwareType.Cpu,
      HardwareType.Motherboard,
      HardwareType.Memory
    };

    var sorted = types.OrderBy(t => t).ToArray();

    Assert.Equal(HardwareType.Motherboard, sorted[0]);
    Assert.Equal(HardwareType.Cpu, sorted[1]);
    Assert.Equal(HardwareType.Memory, sorted[2]);
    Assert.Equal(HardwareType.PowerMonitor, sorted[3]);
  }

  [Fact]
  public void HardwareType_CanBeSortedDescending() {
    var types = new[] {
      HardwareType.Motherboard,
      HardwareType.Memory,
      HardwareType.Cpu,
      HardwareType.PowerMonitor
    };

    var sorted = types.OrderByDescending(t => t).ToArray();

    Assert.Equal(HardwareType.PowerMonitor, sorted[0]);
    Assert.Equal(HardwareType.Memory, sorted[1]);
    Assert.Equal(HardwareType.Cpu, sorted[2]);
    Assert.Equal(HardwareType.Motherboard, sorted[3]);
  }

  // =========================================================================
  // Boundary and Edge Cases
  // =========================================================================

  [Fact]
  public void HardwareType_Motherboard_IsMinValue() {
    var minValue = Enum.GetValues<HardwareType>().Min();
    Assert.Equal(HardwareType.Motherboard, minValue);
  }

  [Fact]
  public void HardwareType_PowerMonitor_IsMaxValue() {
    var maxValue = Enum.GetValues<HardwareType>().Max();
    Assert.Equal(HardwareType.PowerMonitor, maxValue);
  }

  [Fact]
  public void HardwareType_NegativeValue_IsNotDefined() {
    Assert.False(Enum.IsDefined((HardwareType)(-1)));
  }

  [Fact]
  public void HardwareType_ValueBeyondRange_IsNotDefined() {
    Assert.False(Enum.IsDefined((HardwareType)100));
  }

  // =========================================================================
  // ToString Consistency
  // =========================================================================

  [Fact]
  public void HardwareType_ToString_Consistent_AcrossMultipleCalls() {
    var type = HardwareType.Cpu;
    var str1 = type.ToString();
    var str2 = type.ToString();
    var str3 = type.ToString();

    Assert.Equal(str1, str2);
    Assert.Equal(str2, str3);
  }

  [Fact]
  public void HardwareType_ToString_CanBeUsedInHierarchy() {
    var baseStr = HardwareType.Cpu.ToString();
    var parts = baseStr.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

    Assert.Single(parts);
    Assert.Equal("Cpu", parts[0]);
  }
}