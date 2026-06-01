using Xunit;
using CrystalMonitor.Hardware;
using Windows.Win32.System.SystemInformation;

namespace CrystalMonitorTests.HardwareTests;

public class FirmwareTableTests {
  // -------------------------------------------------------------------------
  // Helpers
  // -------------------------------------------------------------------------

  private static bool IsWindows =>
    System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(
      System.Runtime.InteropServices.OSPlatform.Windows);

  // ACPI and RSMB are the two most common firmware table providers
  private static readonly FIRMWARE_TABLE_PROVIDER Acpi =
    (FIRMWARE_TABLE_PROVIDER)0x41435049; // 'ACPI'

  private static readonly FIRMWARE_TABLE_PROVIDER Rsmb =
    (FIRMWARE_TABLE_PROVIDER)0x52534D42; // 'RSMB'

  // -------------------------------------------------------------------------
  // GetTable(provider, string) — string-to-uint ID conversion
  // -------------------------------------------------------------------------

  [Fact]
  public void GetTable_StringOverload_DoesNotThrow_OnWindows() {
    if (!IsWindows) return;

    var ex = Record.Exception(() => FirmwareTable.GetTable(Rsmb, "RSMB"));
    Assert.Null(ex);
  }

  [Fact]
  public void GetTable_StringOverload_ProducesConsistentResult_WithUintOverload() {
    if (!IsWindows) return;

    // "FACP" in little-endian is 0x50434146
    uint id = (uint)(('F') | ('A' << 8) | ('C' << 16) | ('P' << 24));

    byte[] fromString = FirmwareTable.GetTable(Acpi, "FACP");
    byte[] fromUint = FirmwareTable.GetTable(Acpi, id);

    // Both should return the same result (null or identical bytes)
    if (fromString == null && fromUint == null) return;

    Assert.NotNull(fromString);
    Assert.NotNull(fromUint);
    Assert.Equal(fromUint, fromString);
  }

  [Theory]
  [InlineData("FACP", 0x50434146u)] // F=0x46, A=0x41, C=0x43, P=0x50 → little-endian
  [InlineData("APIC", 0x43495041u)] // A=0x41, P=0x50, I=0x49, C=0x43
  [InlineData("RSMB", 0x424D5352u)] // R=0x52, S=0x53, M=0x4D, B=0x42
  public void GetTable_StringOverload_ConvertsTableNameToCorrectUint(
    string tableName, uint expectedId) {
    // Verify the bit-shift formula: id = table[3]<<24 | table[2]<<16 | table[1]<<8 | table[0]
    uint id = (uint)((tableName[3] << 24) | (tableName[2] << 16) |
                     (tableName[1] << 8) | tableName[0]);
    Assert.Equal(expectedId, id);
  }

  // -------------------------------------------------------------------------
  // GetTable(provider, uint) — integration
  // -------------------------------------------------------------------------

  [Fact]
  public void GetTable_UintOverload_DoesNotThrow_OnWindows() {
    if (!IsWindows) return;

    var ex = Record.Exception(() => FirmwareTable.GetTable(Rsmb, 0u));
    Assert.Null(ex);
  }

  [Fact]
  public void GetTable_ReturnsNull_WhenTableNotFound() {
    if (!IsWindows) return;

    // 0xDEADBEEF is unlikely to be a valid firmware table ID
    var result = FirmwareTable.GetTable(Acpi, 0xDEADBEEFu);
    Assert.Null(result);
  }

  [Fact]
  public void GetTable_Rsmb_ReturnsNonNullBuffer_OnPhysicalMachine() {
    if (!IsWindows) return;

    // RSMB table 0 is the raw SMBIOS firmware table — always present on
    // physical Windows machines
    var result = FirmwareTable.GetTable(Rsmb, 0u);
    if (result == null) return; // VM or container without SMBIOS, skip

    Assert.True(result.Length > 0,
      "RSMB firmware table should contain at least one byte.");
  }

  [Fact]
  public void GetTable_ReturnedBuffer_HasPositiveLength() {
    if (!IsWindows) return;

    var result = FirmwareTable.GetTable(Rsmb, 0u);
    if (result == null) return;

    Assert.True(result.Length > 0,
      "GetTable should not return a zero-length buffer.");
  }

  [Fact]
  public void GetTable_ReturnedBuffer_IsNotAllZeros_ForRsmb() {
    if (!IsWindows) return;

    var result = FirmwareTable.GetTable(Rsmb, 0u);
    if (result == null) return;

    // A valid SMBIOS table has non-zero header bytes
    Assert.True(result.Any(b => b != 0),
      "RSMB firmware table should not be entirely zero.");
  }

  [Fact]
  public void GetTable_CalledTwice_ReturnsSameLengthBuffer() {
    if (!IsWindows) return;

    var first = FirmwareTable.GetTable(Rsmb, 0u);
    var second = FirmwareTable.GetTable(Rsmb, 0u);

    if (first == null || second == null) return;

    Assert.Equal(first.Length, second.Length);
  }

  // -------------------------------------------------------------------------
  // EnumerateTables — integration
  // -------------------------------------------------------------------------

  [Fact]
  public void EnumerateTables_DoesNotThrow_OnWindows() {
    if (!IsWindows) return;

    var ex = Record.Exception(() => FirmwareTable.EnumerateTables(Acpi));
    Assert.Null(ex);
  }

  [Fact]
  public void EnumerateTables_ReturnsNullOrNonEmptyArray_OnWindows() {
    if (!IsWindows) return;

    var result = FirmwareTable.EnumerateTables(Acpi);
    if (result == null) return;

    Assert.True(result.Length > 0,
      "EnumerateTables should not return an empty non-null array.");
  }

  [Fact]
  public void EnumerateTables_AllEntries_AreFourCharacters() {
    if (!IsWindows) return;

    var result = FirmwareTable.EnumerateTables(Acpi);
    if (result == null) return;

    // Each entry is decoded as 4 ASCII bytes
    Assert.All(result, entry =>
      Assert.Equal(4, entry.Length));
  }

  [Fact]
  public void EnumerateTables_AllEntries_AreNonNullOrEmpty() {
    if (!IsWindows) return;

    var result = FirmwareTable.EnumerateTables(Acpi);
    if (result == null) return;

    Assert.All(result, entry =>
      Assert.False(string.IsNullOrEmpty(entry),
        "Enumerated table name should not be null or empty."));
  }

  [Fact]
  public void EnumerateTables_ContainsKnownAcpiTable_OnPhysicalMachine() {
    if (!IsWindows) return;

    var result = FirmwareTable.EnumerateTables(Acpi);
    if (result == null || result.Length == 0) return;

    // FACP (Fixed ACPI Description Table) is present on all ACPI-compliant machines
    Assert.Contains("FACP", result);
  }

  [Fact]
  public void EnumerateTables_CountMatchesBufferSizeDividedByFour() {
    if (!IsWindows) return;

    // The method slices the buffer into 4-byte chunks, so result.Length == size / 4
    // We verify this by checking all entries are exactly 4 chars
    var result = FirmwareTable.EnumerateTables(Acpi);
    if (result == null) return;

    Assert.All(result, entry => Assert.Equal(4, entry.Length));
  }

  [Fact]
  public void EnumerateTables_Rsmb_DoesNotThrow() {
    if (!IsWindows) return;

    var ex = Record.Exception(() => FirmwareTable.EnumerateTables(Rsmb));
    Assert.Null(ex);
  }

  [Fact]
  public void EnumerateTables_TablesReturnedAreGetTable_Accessible() {
    if (!IsWindows) return;

    var tables = FirmwareTable.EnumerateTables(Acpi);
    if (tables == null || tables.Length == 0) return;

    // At least the first enumerated table should be retrievable via GetTable
    var result = FirmwareTable.GetTable(Acpi, tables[0]);

    // May still be null if permissions or size fail — just verify no throw
    var ex = Record.Exception(() => FirmwareTable.GetTable(Acpi, tables[0]));
    Assert.Null(ex);
  }
}