using CrystalMonitor.Hardware;

namespace CrystalMonitorTests.HardwareTests;

/// <summary>
/// Unit tests for SMBIOS-related enums and classes.
/// Tests enum values, encoding, and data parsing patterns.
/// </summary>
public class SmBiosTests {
  // =========================================================================
  // SystemEnclosureType Tests
  // =========================================================================

  [Theory]
  [InlineData(SystemEnclosureType.Other, 1)]
  [InlineData(SystemEnclosureType.Unknown, 2)]
  [InlineData(SystemEnclosureType.Desktop, 3)]
  [InlineData(SystemEnclosureType.LowProfileDesktop, 4)]
  [InlineData(SystemEnclosureType.Tower, 7)]
  [InlineData(SystemEnclosureType.Laptop, 9)]
  [InlineData(SystemEnclosureType.RackMountChassis, 23)]
  public void SystemEnclosureType_OrdinalValue_IsCorrect(SystemEnclosureType type, int expected) {
    Assert.Equal(expected, (int)type);
  }

  [Fact]
  public void SystemEnclosureType_HasMultipleMembers() {
    var count = Enum.GetValues<SystemEnclosureType>().Length;
    Assert.True(count > 20, $"Expected more than 20 members, got {count}");
  }

  [Theory]
  [InlineData(SystemEnclosureType.Desktop)]
  [InlineData(SystemEnclosureType.Laptop)]
  [InlineData(SystemEnclosureType.Tablet)]
  public void SystemEnclosureType_CommonTypes_AreDefined(SystemEnclosureType type) {
    Assert.True(Enum.IsDefined(type));
  }

  // =========================================================================
  // SystemEnclosureState Tests
  // =========================================================================

  [Theory]
  [InlineData(SystemEnclosureState.Other, 1)]
  [InlineData(SystemEnclosureState.Unknown, 2)]
  [InlineData(SystemEnclosureState.Safe, 3)]
  [InlineData(SystemEnclosureState.Warning, 4)]
  [InlineData(SystemEnclosureState.Critical, 5)]
  [InlineData(SystemEnclosureState.NonRecoverable, 6)]
  public void SystemEnclosureState_OrdinalValue_IsCorrect(SystemEnclosureState state, int expected) {
    Assert.Equal(expected, (int)state);
  }

  [Fact]
  public void SystemEnclosureState_HasExactly6Members() {
    Assert.Equal(6, Enum.GetValues<SystemEnclosureState>().Length);
  }

  [Theory]
  [InlineData(SystemEnclosureState.Safe)]
  [InlineData(SystemEnclosureState.Warning)]
  [InlineData(SystemEnclosureState.Critical)]
  public void SystemEnclosureState_HealthStates_AreDefined(SystemEnclosureState state) {
    Assert.True(Enum.IsDefined(state));
  }

  // =========================================================================
  // SystemEnclosureSecurityStatus Tests
  // =========================================================================

  [Theory]
  [InlineData(SystemEnclosureSecurityStatus.Other, 1)]
  [InlineData(SystemEnclosureSecurityStatus.Unknown, 2)]
  [InlineData(SystemEnclosureSecurityStatus.None, 3)]
  [InlineData(SystemEnclosureSecurityStatus.ExternalInterfaceLockedOut, 4)]
  [InlineData(SystemEnclosureSecurityStatus.ExternalInterfaceEnabled, 5)]
  public void SystemEnclosureSecurityStatus_OrdinalValue_IsCorrect(
    SystemEnclosureSecurityStatus status, int expected) {
    Assert.Equal(expected, (int)status);
  }

  [Fact]
  public void SystemEnclosureSecurityStatus_HasExactly5Members() {
    Assert.Equal(5, Enum.GetValues<SystemEnclosureSecurityStatus>().Length);
  }

  [Fact]
  public void SystemEnclosureSecurityStatus_OtherIsMinValue() {
    var minValue = Enum.GetValues<SystemEnclosureSecurityStatus>().Min();
    Assert.Equal(SystemEnclosureSecurityStatus.Other, minValue);
  }

  // =========================================================================
  // ProcessorFamily Tests
  // =========================================================================

  [Theory]
  [InlineData(ProcessorFamily.Other, 1)]
  [InlineData(ProcessorFamily.Intel8086, 3)]
  [InlineData(ProcessorFamily.Intel80286, 4)]
  [InlineData(ProcessorFamily.IntelPentium, 11)]
  [InlineData(ProcessorFamily.AmdDuron, 24)]
  [InlineData(ProcessorFamily.AmdK5, 25)]
  [InlineData(ProcessorFamily.IntelAtom, 43)]
  public void ProcessorFamily_OrdinalValue_IsCorrect(ProcessorFamily family, int expected) {
    Assert.Equal(expected, (int)family);
  }

  [Fact]
  public void ProcessorFamily_HasManyMembers() {
    var count = Enum.GetValues<ProcessorFamily>().Length;
    Assert.True(count > 70, $"Expected more than 70 members, got {count}");
  }

  [Theory]
  [InlineData(ProcessorFamily.Intel386)]
  [InlineData(ProcessorFamily.Intel486)]
  [InlineData(ProcessorFamily.IntelPentium)]
  [InlineData(ProcessorFamily.AmdAthlon)]
  public void ProcessorFamily_CommonProcessors_AreDefined(ProcessorFamily family) {
    Assert.True(Enum.IsDefined(family));
  }

  [Fact]
  public void ProcessorFamily_CanParseIntelProcessors() {
    var intelFamilies = Enum.GetValues<ProcessorFamily>()
      .Where(f => f.ToString().StartsWith("Intel"))
      .ToList();

    Assert.NotEmpty(intelFamilies);
    Assert.Contains(ProcessorFamily.IntelPentium, intelFamilies);
  }

  [Fact]
  public void ProcessorFamily_CanParseAmdProcessors() {
    var amdFamilies = Enum.GetValues<ProcessorFamily>()
      .Where(f => f.ToString().StartsWith("Amd"))
      .ToList();

    Assert.NotEmpty(amdFamilies);
    Assert.Contains(ProcessorFamily.AmdAthlon, amdFamilies);
  }

  // =========================================================================
  // ProcessorCharacteristics Tests (Flags Enum)
  // =========================================================================

  [Fact]
  public void ProcessorCharacteristics_None_IsZero() {
    Assert.Equal(0, (int)ProcessorCharacteristics.None);
  }

  [Theory]
  [InlineData(ProcessorCharacteristics._64BitCapable, 1)]
  [InlineData(ProcessorCharacteristics.MultiCore, 2)]
  [InlineData(ProcessorCharacteristics.HardwareThread, 4)]
  [InlineData(ProcessorCharacteristics.ExecuteProtection, 8)]
  public void ProcessorCharacteristics_FlagValues_AreCorrect(
    ProcessorCharacteristics flag, int expected) {
    Assert.Equal(expected, (int)flag);
  }

  [Fact]
  public void ProcessorCharacteristics_CanCombineFlags() {
    var combined = ProcessorCharacteristics._64BitCapable | ProcessorCharacteristics.MultiCore;
    Assert.True((combined & ProcessorCharacteristics._64BitCapable) != 0);
    Assert.True((combined & ProcessorCharacteristics.MultiCore) != 0);
  }

  [Fact]
  public void ProcessorCharacteristics_CanCheckFlag() {
    var characteristics = ProcessorCharacteristics._64BitCapable | ProcessorCharacteristics.MultiCore;

    Assert.True(characteristics.HasFlag(ProcessorCharacteristics._64BitCapable));
    Assert.True(characteristics.HasFlag(ProcessorCharacteristics.MultiCore));
    Assert.False(characteristics.HasFlag(ProcessorCharacteristics.HardwareThread));
  }

  [Fact]
  public void ProcessorCharacteristics_IsFlags() {
    Assert.True(typeof(ProcessorCharacteristics).GetCustomAttributes(typeof(FlagsAttribute), false).Length > 0);
  }

  // =========================================================================
  // CacheAssociativity Tests
  // =========================================================================

  [Theory]
  [InlineData(CacheAssociativity.Other, 1)]
  [InlineData(CacheAssociativity.Unknown, 2)]
  [InlineData(CacheAssociativity.DirectMapped, 3)]
  [InlineData(CacheAssociativity._2Way, 4)]
  [InlineData(CacheAssociativity.FullyAssociative, 6)]
  public void CacheAssociativity_OrdinalValue_IsCorrect(CacheAssociativity assoc, int expected) {
    Assert.Equal(expected, (int)assoc);
  }

  [Fact]
  public void CacheAssociativity_HasManyMembers() {
    var count = Enum.GetValues<CacheAssociativity>().Length;
    Assert.True(count > 10);
  }

  [Theory]
  [InlineData(CacheAssociativity.DirectMapped)]
  [InlineData(CacheAssociativity._2Way)]
  [InlineData(CacheAssociativity._4Way)]
  [InlineData(CacheAssociativity.FullyAssociative)]
  public void CacheAssociativity_CommonAssociativities_AreDefined(CacheAssociativity assoc) {
    Assert.True(Enum.IsDefined(assoc));
  }

  // =========================================================================
  // CacheDesignation Tests
  // =========================================================================

  [Theory]
  [InlineData(CacheDesignation.Other, 0)]
  [InlineData(CacheDesignation.L1, 1)]
  [InlineData(CacheDesignation.L2, 2)]
  [InlineData(CacheDesignation.L3, 3)]
  public void CacheDesignation_OrdinalValue_IsCorrect(CacheDesignation designation, int expected) {
    Assert.Equal(expected, (int)designation);
  }

  [Fact]
  public void CacheDesignation_HasExactly4Members() {
    Assert.Equal(4, Enum.GetValues<CacheDesignation>().Length);
  }

  [Theory]
  [InlineData(CacheDesignation.L1)]
  [InlineData(CacheDesignation.L2)]
  [InlineData(CacheDesignation.L3)]
  public void CacheDesignation_ProcessorCaches_AreDefined(CacheDesignation designation) {
    Assert.True(Enum.IsDefined(designation));
  }

  // =========================================================================
  // MemoryType Tests
  // =========================================================================

  [Theory]
  [InlineData(MemoryType.Other, 0x01)]
  [InlineData(MemoryType.Unknown, 0x02)]
  [InlineData(MemoryType.DRAM, 0x03)]
  [InlineData(MemoryType.SDRAM, 0x0f)]
  [InlineData(MemoryType.DDR, 0x12)]
  [InlineData(MemoryType.DDR2, 0x13)]
  [InlineData(MemoryType.DDR3, 0x18)]
  [InlineData(MemoryType.DDR4, 0x1a)]
  [InlineData(MemoryType.DDR5, 0x22)]
  [InlineData(MemoryType.LPDDR5, 0x23)]
  public void MemoryType_OrdinalValue_IsCorrect(MemoryType type, int expected) {
    Assert.Equal(expected, (int)type);
  }

  [Fact]
  public void MemoryType_HasManyMembers() {
    var count = Enum.GetValues<MemoryType>().Length;
    Assert.True(count > 30, $"Expected more than 30 members, got {count}");
  }

  [Theory]
  [InlineData(MemoryType.DDR)]
  [InlineData(MemoryType.DDR2)]
  [InlineData(MemoryType.DDR3)]
  [InlineData(MemoryType.DDR4)]
  [InlineData(MemoryType.DDR5)]
  public void MemoryType_DdrMemoryTypes_AreDefined(MemoryType type) {
    Assert.True(Enum.IsDefined(type));
  }

  [Theory]
  [InlineData(MemoryType.LPDDR)]
  [InlineData(MemoryType.LPDDR2)]
  [InlineData(MemoryType.LPDDR3)]
  [InlineData(MemoryType.LPDDR4)]
  [InlineData(MemoryType.LPDDR5)]
  public void MemoryType_LowPowerDdrTypes_AreDefined(MemoryType type) {
    Assert.True(Enum.IsDefined(type));
  }

  [Fact]
  public void MemoryType_CanIdentifyDdrGenerations() {
    var ddrTypes = new[] {
      MemoryType.DDR,
      MemoryType.DDR2,
      MemoryType.DDR3,
      MemoryType.DDR4,
      MemoryType.DDR5
    };

    Assert.All(ddrTypes, t => Assert.True(Enum.IsDefined(t)));
  }

  // =========================================================================
  // Integration Tests
  // =========================================================================

  [Fact]
  public void SmBios_EnumTypes_CanBeParsed() {
    var systemEnclosureStr = SystemEnclosureType.Laptop.ToString();
    var parsed = Enum.Parse<SystemEnclosureType>(systemEnclosureStr);

    Assert.Equal(SystemEnclosureType.Laptop, parsed);
  }

  [Fact]
  public void SmBios_ProcessorFamilies_CanBeSorted() {
    var families = new[] {
      ProcessorFamily.IntelPentium,
      ProcessorFamily.AmdDuron,
      ProcessorFamily.Intel80286,
      ProcessorFamily.Other
    };

    var sorted = families.OrderBy(f => f).ToArray();

    Assert.Equal(ProcessorFamily.Other, sorted[0]);
  }

  [Fact]
  public void SmBios_MemoryTypes_CoverCommonTechnologies() {
    var commonMemory = new[] {
      MemoryType.DRAM,
      MemoryType.DDR,
      MemoryType.DDR2,
      MemoryType.DDR3,
      MemoryType.DDR4,
      MemoryType.DDR5,
      MemoryType.SDRAM,
      MemoryType.SRAM
    };

    Assert.All(commonMemory, mt => Assert.True(Enum.IsDefined(mt)));
  }

  [Fact]
  public void SmBios_SystemEnclosureTypes_IncludeComputerForms() {
    var computerForms = new[] {
      SystemEnclosureType.Desktop,
      SystemEnclosureType.Laptop,
      SystemEnclosureType.Tablet,
      SystemEnclosureType.AllInOne
    };

    Assert.All(computerForms, st => Assert.True(Enum.IsDefined(st)));
  }

  [Fact]
  public void SmBios_CacheTypes_RepresentHierarchy() {
    var l1 = CacheDesignation.L1;
    var l2 = CacheDesignation.L2;
    var l3 = CacheDesignation.L3;

    Assert.True((int)l1 < (int)l2);
    Assert.True((int)l2 < (int)l3);
  }

  [Fact]
  public void SmBios_ProcessorCharacteristics_SupportMultipleCombinations() {
    var modern64bit = ProcessorCharacteristics._64BitCapable | ProcessorCharacteristics.MultiCore;
    var with_exec_protection = modern64bit | ProcessorCharacteristics.ExecuteProtection;

    Assert.True(with_exec_protection.HasFlag(ProcessorCharacteristics._64BitCapable));
    Assert.True(with_exec_protection.HasFlag(ProcessorCharacteristics.MultiCore));
    Assert.True(with_exec_protection.HasFlag(ProcessorCharacteristics.ExecuteProtection));
  }

  // =========================================================================
  // Boundary and Edge Cases
  // =========================================================================

  [Fact]
  public void SystemEnclosureType_DesktopAndLaptop_AreCommon() {
    Assert.True(Enum.IsDefined(SystemEnclosureType.Desktop));
    Assert.True(Enum.IsDefined(SystemEnclosureType.Laptop));
    Assert.NotEqual((int)SystemEnclosureType.Desktop, (int)SystemEnclosureType.Laptop);
  }

  [Fact]
  public void ProcessorFamily_OtherIsMinValue() {
    var minValue = Enum.GetValues<ProcessorFamily>().Min();
    Assert.Equal(ProcessorFamily.Other, minValue);
  }

  [Fact]
  public void MemoryType_OtherIsMinValue() {
    var minValue = Enum.GetValues<MemoryType>().Min();
    Assert.Equal(MemoryType.Other, minValue);
  }

  [Fact]
  public void ProcessorCharacteristics_NoneIsZero() {
    Assert.Equal(ProcessorCharacteristics.None, (ProcessorCharacteristics)0);
  }

  // =========================================================================
  // String Representation Scenarios
  // =========================================================================

  [Fact]
  public void SmBios_Enums_ProduceReadableStrings() {
    var enclosureStr = SystemEnclosureType.Laptop.ToString();
    var processorStr = ProcessorFamily.IntelPentium.ToString();
    var memoryStr = MemoryType.DDR4.ToString();

    Assert.NotEmpty(enclosureStr);
    Assert.NotEmpty(processorStr);
    Assert.NotEmpty(memoryStr);
    Assert.Contains("Laptop", enclosureStr);
    Assert.Contains("Pentium", processorStr);
    Assert.Contains("DDR4", memoryStr);
  }

  [Fact]
  public void SmBios_ParseReverseOfToString() {
    foreach (var type in Enum.GetValues<SystemEnclosureType>()) {
      var str = type.ToString();
      var parsed = Enum.Parse<SystemEnclosureType>(str);
      Assert.Equal(type, parsed);
    }
  }
}
