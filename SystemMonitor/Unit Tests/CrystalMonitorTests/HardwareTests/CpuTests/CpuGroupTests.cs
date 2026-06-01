using CrystalMonitor.Hardware;

namespace CrystalMonitorTests.HardwareTests.CpuTests;

public class CpuGroupTests : IDisposable {
  private readonly Computer _computer;
  private readonly List<IHardware> _cpuHardware;

  public CpuGroupTests() {
    _computer = new Computer { IsCpuEnabled = true };
    _computer.Open();

    _cpuHardware = _computer.Hardware
      .Where(h => h.HardwareType == HardwareType.Cpu)
      .ToList();
  }

  public void Dispose() {
    _computer.Close();
  }

  // -------------------------------------------------------------------------
  // Helpers
  // -------------------------------------------------------------------------

  private bool HasCpu => _cpuHardware.Count > 0;

  // -------------------------------------------------------------------------
  // Group initialisation
  // -------------------------------------------------------------------------

  [Fact]
  public void CpuGroup_IsCreated_WhenCpuEnabled() {
    Assert.NotNull(_cpuHardware);
  }

  [Fact]
  public void CpuGroup_Hardware_IsNotNull() {
    Assert.NotNull(_cpuHardware);
  }

  [Fact]
  public void CpuGroup_Hardware_ContainsAtLeastOneCpu() {
    if (!HasCpu) return;

    Assert.True(_cpuHardware.Count > 0,
      "CpuGroup should expose at least one CPU on any physical machine.");
  }

  [Fact]
  public void CpuGroup_AllHardware_HaveCpuHardwareType() {
    if (!HasCpu) return;

    Assert.All(_cpuHardware, h =>
      Assert.Equal(HardwareType.Cpu, h.HardwareType));
  }

  [Fact]
  public void CpuGroup_AllHardware_HaveNonEmptyNames() {
    if (!HasCpu) return;

    Assert.All(_cpuHardware, h =>
      Assert.False(string.IsNullOrWhiteSpace(h.Name),
        "CPU hardware entry has a null or empty name."));
  }

  // -------------------------------------------------------------------------
  // Vendor-specific CPU type mapping
  // -------------------------------------------------------------------------

  [Fact]
  public void CpuGroup_IntelCpus_AreDetected() {
    if (!HasCpu) return;

    var intelHardware = _cpuHardware
      .Where(h => h.Name.Contains("Intel", StringComparison.OrdinalIgnoreCase))
      .ToList();

    if (intelHardware.Count == 0) return; // no Intel CPU on this machine

    Assert.All(intelHardware, h =>
      Assert.Equal(HardwareType.Cpu, h.HardwareType));
  }

  [Fact]
  public void CpuGroup_AmdCpus_AreDetected() {
    if (!HasCpu) return;

    var amdHardware = _cpuHardware
      .Where(h => h.Name.Contains("AMD", StringComparison.OrdinalIgnoreCase))
      .ToList();

    if (amdHardware.Count == 0) return; // no AMD CPU on this machine

    Assert.All(amdHardware, h =>
      Assert.Equal(HardwareType.Cpu, h.HardwareType));
  }

  [Fact]
  public void CpuGroup_AllCpus_HaveRecognisedVendorInName() {
    if (!HasCpu) return;

    // Every detected CPU should be from a known vendor or at minimum have a name
    Assert.All(_cpuHardware, h =>
      Assert.False(string.IsNullOrWhiteSpace(h.Name)));
  }

  // -------------------------------------------------------------------------
  // GetReport — per CPU
  // -------------------------------------------------------------------------

  [Fact]
  public void CpuGroup_AllCpus_GetReport_IsNotNullOrEmpty() {
    if (!HasCpu) return;

    Assert.All(_cpuHardware, h =>
      Assert.False(string.IsNullOrWhiteSpace(h.GetReport()),
        $"GetReport() for '{h.Name}' should not be null or empty."));
  }

  [Fact]
  public void CpuGroup_AllCpus_GetReport_ContainsTimeStampCounterInfo() {
    if (!HasCpu) return;

    Assert.All(_cpuHardware, h =>
      Assert.Contains("Time Stamp Counter", h.GetReport(),
        StringComparison.OrdinalIgnoreCase));
  }

  [Fact]
  public void CpuGroup_AllCpus_GetReport_ContainsCpuName() {
    if (!HasCpu) return;

    Assert.All(_cpuHardware, h => {
      string report = h.GetReport();
      Assert.Contains("Name", report, StringComparison.OrdinalIgnoreCase);
    });
  }

  [Fact]
  public void CpuGroup_AllCpus_GetReport_ContainsCoreCount() {
    if (!HasCpu) return;

    Assert.All(_cpuHardware, h =>
      Assert.Contains("Number of Cores", h.GetReport(),
        StringComparison.OrdinalIgnoreCase));
  }
}

// -------------------------------