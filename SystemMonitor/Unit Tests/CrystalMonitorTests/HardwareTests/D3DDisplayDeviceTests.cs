using CrystalMonitor.Hardware;

namespace CrystalMonitorTests.HardwareTests;

public class D3DDisplayDeviceTests {
  // -------------------------------------------------------------------------
  // Helpers
  // -------------------------------------------------------------------------

  private static bool IsWindows =>
    System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(
      System.Runtime.InteropServices.OSPlatform.Windows);

  // -------------------------------------------------------------------------
  // GetActualDeviceIdentifier — pure string transformation, no hardware needed
  // -------------------------------------------------------------------------

  [Fact]
  public void GetActualDeviceIdentifier_NullInput_ReturnsNull() {
    var result = D3DDisplayDevice.GetActualDeviceIdentifier(null);
    Assert.Null(result);
  }

  [Fact]
  public void GetActualDeviceIdentifier_EmptyInput_ReturnsEmpty() {
    var result = D3DDisplayDevice.GetActualDeviceIdentifier(string.Empty);
    Assert.Equal(string.Empty, result);
  }

  [Theory]
  [InlineData(
    @"\\?\ROOT#BasicRender#0000#{1ca05180-a699-450a-9a0c-de4fbe3ddd89}",
    @"ROOT\BasicRender\0000")]
  [InlineData(
    @"\\?\PCI#VEN_1002&DEV_731F&SUBSYS_57051682&REV_C4#6&e539058&0&00000019#{1ca05180-a699-450a-9a0c-de4fbe3ddd89}",
    @"PCI\VEN_1002&DEV_731F&SUBSYS_57051682&REV_C4\6&e539058&0&00000019")]
  public void GetActualDeviceIdentifier_StripsPrefix_AndGuid_AndReplacesHash(
    string input, string expected) {
    var result = D3DDisplayDevice.GetActualDeviceIdentifier(input);
    Assert.Equal(expected, result);
  }

  [Fact]
  public void GetActualDeviceIdentifier_StripsPrefixOnly_WhenNoGuidSuffix() {
    // Input has \\?\ prefix but no trailing GUID
    var result = D3DDisplayDevice.GetActualDeviceIdentifier(@"\\?\ROOT#BasicRender#0000");
    Assert.Equal(@"ROOT\BasicRender\0000", result);
  }

  [Fact]
  public void GetActualDeviceIdentifier_ReplacesHashWithBackslash() {
    var result = D3DDisplayDevice.GetActualDeviceIdentifier(@"\\?\ROOT#BasicRender#0000");
    Assert.DoesNotContain("#", result);
    Assert.Contains(@"\", result);
  }

  [Fact]
  public void GetActualDeviceIdentifier_NoPrefix_ReplacesHashOnly() {
    // No \\?\ prefix — only hash replacement should apply
    var result = D3DDisplayDevice.GetActualDeviceIdentifier(@"ROOT#BasicRender#0000");
    Assert.Equal(@"ROOT\BasicRender\0000", result);
  }

  [Fact]
  public void GetActualDeviceIdentifier_NoHashNoGuid_ReturnsUnchanged() {
    const string input = "ROOTDEVICE";
    var result = D3DDisplayDevice.GetActualDeviceIdentifier(input);
    Assert.Equal(input, result);
  }

  [Fact]
  public void GetActualDeviceIdentifier_GuidSuffix_IsRemoved() {
    const string input = @"\\?\PCI#VEN_10DE#{1ca05180-a699-450a-9a0c-de4fbe3ddd89}";
    var result = D3DDisplayDevice.GetActualDeviceIdentifier(input);
    Assert.DoesNotContain("{", result);
    Assert.DoesNotContain("}", result);
  }

  [Fact]
  public void GetActualDeviceIdentifier_DoesNotEndWithBackslash_AfterGuidRemoval() {
    const string input = @"\\?\ROOT#BasicRender#0000#{1ca05180-a699-450a-9a0c-de4fbe3ddd89}";
    var result = D3DDisplayDevice.GetActualDeviceIdentifier(input);
    Assert.False(result.EndsWith(@"\"),
      $"Result should not end with backslash, got: '{result}'");
  }

  // -------------------------------------------------------------------------
  // GetDeviceIdentifiers — integration (Windows only)
  // -------------------------------------------------------------------------

  [Fact]
  public void GetDeviceIdentifiers_OnWindows_DoesNotThrow() {
    if (!IsWindows) return;

    var ex = Record.Exception(() => D3DDisplayDevice.GetDeviceIdentifiers());
    Assert.Null(ex);
  }

  [Fact]
  public void GetDeviceIdentifiers_OnWindows_ReturnsNullOrNonEmptyArray() {
    if (!IsWindows) return;

    var result = D3DDisplayDevice.GetDeviceIdentifiers();

    // null means CM_Get_Device_Interface_List_Size failed (e.g. no GPU)
    // non-null must have at least one entry
    if (result != null)
      Assert.True(result.Length > 0,
        "GetDeviceIdentifiers returned an empty non-null array.");
  }

  [Fact]
  public void GetDeviceIdentifiers_OnWindows_AllEntriesAreNonEmpty() {
    if (!IsWindows) return;

    var result = D3DDisplayDevice.GetDeviceIdentifiers();
    if (result == null) return;

    Assert.All(result, id =>
      Assert.False(string.IsNullOrWhiteSpace(id),
        "Device identifier should not be null or empty."));
  }

  [Fact]
  public void GetDeviceIdentifiers_OnWindows_AllEntriesStartWithDevicePrefix() {
    if (!IsWindows) return;

    var result = D3DDisplayDevice.GetDeviceIdentifiers();
    if (result == null || result.Length == 0) return;

    // All raw device interface paths start with \\?\
    Assert.All(result, id =>
      Assert.StartsWith(@"\\?\", id,
        StringComparison.OrdinalIgnoreCase));
  }

  [Fact]
  public void GetDeviceIdentifiers_OnWindows_AllEntriesContainGuid() {
    if (!IsWindows) return;

    var result = D3DDisplayDevice.GetDeviceIdentifiers();
    if (result == null || result.Length == 0) return;

    // Device interface paths end with a GUID in braces
    Assert.All(result, id =>
      Assert.Contains("{", id, StringComparison.Ordinal));
  }

  // -------------------------------------------------------------------------
  // GetDeviceInfoByIdentifier — integration (Windows only)
  // -------------------------------------------------------------------------

  [Fact]
  public void GetDeviceInfoByIdentifier_OnWindows_DoesNotThrow() {
    if (!IsWindows) return;

    var ids = D3DDisplayDevice.GetDeviceIdentifiers();
    if (ids == null || ids.Length == 0) return;

    var ex = Record.Exception(() =>
      D3DDisplayDevice.GetDeviceInfoByIdentifier(ids[0], out _));
    Assert.Null(ex);
  }

  [Fact]
  public void GetDeviceInfoByIdentifier_OnWindows_ReturnsTrueForRealDevice() {
    if (!IsWindows) return;

    var ids = D3DDisplayDevice.GetDeviceIdentifiers();
    if (ids == null || ids.Length == 0) return;

    bool found = false;
    foreach (var id in ids) {
      if (D3DDisplayDevice.GetDeviceInfoByIdentifier(id, out _)) {
        found = true;
        break;
      }
    }

    Assert.True(found,
      "Expected at least one device identifier to return a valid D3DDeviceInfo.");
  }

  [Fact]
  public void GetDeviceInfoByIdentifier_OnWindows_PopulatesNodes() {
    if (!IsWindows) return;

    var ids = D3DDisplayDevice.GetDeviceIdentifiers();
    if (ids == null || ids.Length == 0) return;

    foreach (var id in ids) {
      if (!D3DDisplayDevice.GetDeviceInfoByIdentifier(id, out var info)) continue;

      Assert.NotNull(info.Nodes);
      Assert.True(info.Nodes.Length > 0,
        "D3DDeviceInfo.Nodes should contain at least one node.");
      return;
    }
  }

  [Fact]
  public void GetDeviceInfoByIdentifier_OnWindows_NodeNamesAreNonEmpty() {
    if (!IsWindows) return;

    var ids = D3DDisplayDevice.GetDeviceIdentifiers();
    if (ids == null || ids.Length == 0) return;

    foreach (var id in ids) {
      if (!D3DDisplayDevice.GetDeviceInfoByIdentifier(id, out var info)) continue;

      Assert.All(info.Nodes, node =>
        Assert.False(string.IsNullOrWhiteSpace(node.Name),
          $"Node {node.Id} should have a non-empty name."));
      return;
    }
  }

  [Fact]
  public void GetDeviceInfoByIdentifier_OnWindows_NodeNamesStartWithD3D() {
    if (!IsWindows) return;

    var ids = D3DDisplayDevice.GetDeviceIdentifiers();
    if (ids == null || ids.Length == 0) return;

    foreach (var id in ids) {
      if (!D3DDisplayDevice.GetDeviceInfoByIdentifier(id, out var info)) continue;

      Assert.All(info.Nodes, node =>
        Assert.StartsWith("D3D", node.Name, StringComparison.Ordinal));
      return;
    }
  }

  [Fact]
  public void GetDeviceInfoByIdentifier_OnWindows_NodeQueryTimesAreRecent() {
    if (!IsWindows) return;

    var ids = D3DDisplayDevice.GetDeviceIdentifiers();
    if (ids == null || ids.Length == 0) return;

    foreach (var id in ids) {
      if (!D3DDisplayDevice.GetDeviceInfoByIdentifier(id, out var info)) continue;

      DateTime before = DateTime.Now;
      Assert.All(info.Nodes, node =>
        Assert.True(node.QueryTime <= before,
          $"Node {node.Id} QueryTime {node.QueryTime} should not be in the future."));
      return;
    }
  }

  [Fact]
  public void GetDeviceInfoByIdentifier_OnWindows_MemoryLimitsAreNonNegative() {
    if (!IsWindows) return;

    var ids = D3DDisplayDevice.GetDeviceIdentifiers();
    if (ids == null || ids.Length == 0) return;

    foreach (var id in ids) {
      if (!D3DDisplayDevice.GetDeviceInfoByIdentifier(id, out var info)) continue;

      Assert.True(info.GpuVideoMemoryLimit >= 0,
        $"GpuVideoMemoryLimit should be non-negative, got {info.GpuVideoMemoryLimit}.");
      Assert.True(info.GpuSharedLimit >= 0,
        $"GpuSharedLimit should be non-negative, got {info.GpuSharedLimit}.");
      Assert.True(info.GpuDedicatedLimit >= 0,
        $"GpuDedicatedLimit should be non-negative, got {info.GpuDedicatedLimit}.");
      return;
    }
  }

  [Fact]
  public void GetDeviceInfoByIdentifier_OnWindows_UsedDoesNotExceedMax() {
    if (!IsWindows) return;

    var ids = D3DDisplayDevice.GetDeviceIdentifiers();
    if (ids == null || ids.Length == 0) return;

    foreach (var id in ids) {
      if (!D3DDisplayDevice.GetDeviceInfoByIdentifier(id, out var info)) continue;

      if (info.GpuSharedMax > 0)
        Assert.True(info.GpuSharedUsed <= info.GpuSharedMax,
          $"GpuSharedUsed {info.GpuSharedUsed} exceeds GpuSharedMax {info.GpuSharedMax}.");

      if (info.GpuDedicatedMax > 0)
        Assert.True(info.GpuDedicatedUsed <= info.GpuDedicatedMax,
          $"GpuDedicatedUsed {info.GpuDedicatedUsed} exceeds GpuDedicatedMax {info.GpuDedicatedMax}.");
      return;
    }
  }

  // -------------------------------------------------------------------------
  // D3DDeviceInfo struct
  // -------------------------------------------------------------------------

  [Fact]
  public void D3DDeviceInfo_DefaultConstruction_DoesNotThrow() {
    var ex = Record.Exception(() => new D3DDisplayDevice.D3DDeviceInfo());
    Assert.Null(ex);
  }

  [Fact]
  public void D3DDeviceInfo_DefaultValues_AreZeroOrNull() {
    var info = new D3DDisplayDevice.D3DDeviceInfo();

    Assert.Equal(0ul, info.GpuSharedLimit);
    Assert.Equal(0ul, info.GpuDedicatedLimit);
    Assert.Equal(0ul, info.GpuVideoMemoryLimit);
    Assert.Equal(0ul, info.GpuSharedUsed);
    Assert.Equal(0ul, info.GpuDedicatedUsed);
    Assert.Equal(0ul, info.GpuSharedMax);
    Assert.Equal(0ul, info.GpuDedicatedMax);
    Assert.Null(info.Nodes);
    Assert.False(info.Integrated);
  }

  // -------------------------------------------------------------------------
  // D3DDeviceNodeInfo struct
  // -------------------------------------------------------------------------

  [Fact]
  public void D3DDeviceNodeInfo_DefaultConstruction_DoesNotThrow() {
    var ex = Record.Exception(() => new D3DDisplayDevice.D3DDeviceNodeInfo());
    Assert.Null(ex);
  }

  [Fact]
  public void D3DDeviceNodeInfo_DefaultValues_AreZeroOrNull() {
    var node = new D3DDisplayDevice.D3DDeviceNodeInfo();

    Assert.Equal(0ul, node.Id);
    Assert.Null(node.Name);
    Assert.Equal(0L, node.RunningTime);
    Assert.Equal(default(DateTime), node.QueryTime);
  }

  [Fact]
  public void D3DDeviceNodeInfo_CanBeAssigned() {
    var now = DateTime.Now;
    var node = new D3DDisplayDevice.D3DDeviceNodeInfo {
      Id = 1,
      Name = "D3D 3D",
      RunningTime = 12345L,
      QueryTime = now
    };

    Assert.Equal(1ul, node.Id);
    Assert.Equal("D3D 3D", node.Name);
    Assert.Equal(12345L, node.RunningTime);
    Assert.Equal(now, node.QueryTime);
  }
}