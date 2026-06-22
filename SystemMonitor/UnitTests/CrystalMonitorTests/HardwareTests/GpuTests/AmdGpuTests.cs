using CrystalMonitor.Hardware;
using System.Diagnostics;

namespace CrystalMonitorTests.HardwareTests.GpuTests;

public class AmdGpuTests : IDisposable {
  private readonly Computer _computer;
  private readonly IHardware? _amdGpu;
  private readonly bool _isAmdGpu;

  public AmdGpuTests() {
    _computer = new Computer() { IsGpuEnabled = true };
    _computer.Open();

    _amdGpu = _computer
      .Hardware
      .FirstOrDefault(h => h.HardwareType == HardwareType.GpuAmd && h.Name.Contains("AMD", StringComparison.OrdinalIgnoreCase));

    _isAmdGpu = _amdGpu != null;
    Debug.WriteLine($"Is AMD GPU detected: {_isAmdGpu}");
  }

  public void Dispose() {
    _computer.Close();
  }

  private bool ShouldSkip => !_isAmdGpu;

  [Fact]
  public void AmdGpu_Should_Be_Detected() {
    if (ShouldSkip) {
      Debug.WriteLine("Skipping test: No AMD GPU detected.");
      return;
    }
    Assert.NotNull(_amdGpu);
    Assert.Equal(HardwareType.GpuAmd, _amdGpu!.HardwareType);
  }
}
