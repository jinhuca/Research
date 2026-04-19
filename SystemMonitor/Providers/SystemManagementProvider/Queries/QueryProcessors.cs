using System.Management;
using static SystemManagementProvider.Constants.Win32_Processor;

namespace SystemManagementProvider.Queries;

public class QueryProcessors {
  private ManagementObjectSearcher _searcher;
  private static Dictionary<string, (string, string)> info = [];

  public QueryProcessors(ManagementObjectSearcher searcher) {
    if(searcher is null)
      throw new ArgumentNullException(nameof(searcher));
    _searcher = searcher;
  }

  public (string, string) Query(string queryString) {
    (string, string) result_ = new();
    try {
      if (_searcher == null) {
        return (string.Empty, string.Empty);
      }
      foreach (var obj in _searcher.Get()) {
        result_ = (queryString, Convert.ToString(obj[queryString])!);
      }
    }
    catch (ManagementException ex) {
      Console.WriteLine(ex.Message);
    }
    return result_;
  }

  public void Invoke() {
    try {
      if (_searcher == null) {
        return;
      }
      foreach (var obj in _searcher.Get()) {
        info.Add(AddressWidthKey, (Convert.ToString(obj[AddressWidthKey]), AddressWidthDesc));
      }
    }
    catch (ManagementException ex) {
      Console.WriteLine(ex.Message);
    }
  }

  public (string key, string value, string desc) GetOneInfo(string queryText) {
    if (info.Count <= 0) {
      throw new InvalidOperationException("Get Processor Info");
    }
    if (info.ContainsKey(queryText)) {
      var result = info[queryText];
      var key = queryText;
      var value = result.Item1;
      var desc = result.Item2;
      return (key, value, desc);
    }
    else {
      return (string.Empty, string.Empty, string.Empty);
    }
  }

  public Dictionary<string, (string, string)> GetInfo() {
    info.Clear();
    try {
      using ManagementObjectCollection objCollection_ = _searcher.Get();
      foreach (var obj in objCollection_.Cast<ManagementObject>()) {
        info.Add(AddressWidthKey, (Convert.ToString(obj[AddressWidthKey]), AddressWidthDesc));
        info.Add(ArchitectureKey, (GetArchitecture(Convert.ToUInt16(obj[ArchitectureKey])), ArchitectureDesc));
        info.Add(AssetTagKey, (Convert.ToString(obj[AvailabilityKey]), AssetTagDesc));
        info.Add(AvailabilityKey, (GetAvailability(Convert.ToUInt16(obj[AvailabilityKey])), AvailabilityDesc));
        info.Add(CaptionKey, (Convert.ToString(obj[CaptionKey]), CaptionDesc));
        info.Add(CharacteristicsKey, (Convert.ToString(obj[CharacteristicsKey]), CharacteristicsDesc));
        info.Add(ConfigManagerErrorCodeKey, (GetConfigManagerErrorCode(Convert.ToUInt32(obj[ConfigManagerErrorCodeKey])), ConfigManagerErrorCodeDesc));
        info.Add(ConfigManagerUserConfigKey, (Convert.ToString(Convert.ToBoolean(obj[ConfigManagerUserConfigKey])), ConfigManagerUserConfigDesc));
        info.Add(CpuStatusKey, (GetCpuStatus(Convert.ToUInt16(obj[CpuStatusKey])), CpuStatusDesc));
        info.Add(CreationClassNameKey, (Convert.ToString(obj[CreationClassNameKey]), CreationClassNameDesc));
        info.Add(CurrentClockSpeedKey, (Convert.ToString((obj[CurrentClockSpeedKey]) + SpeedUnit), CurrentClockSpeedDesc));
        info.Add(CurrentVoltageKey, (GetCurrentVoltage(Convert.ToUInt16(obj[CurrentVoltageKey])), CurrentVoltageDesc));
        info.Add(DataWidthKey, (GetDataWidth(Convert.ToUInt16(obj[DataWidthKey])), DataWidthDesc));
        info.Add(DescriptionKey, (Convert.ToString(obj[DescriptionKey]), DescriptionDesc));
        info.Add(DeviceIDKey, (Convert.ToString(obj[DeviceIDKey]), DeviceIDDesc));
        info.Add(ErrorClearedKey, (Convert.ToString(Convert.ToBoolean(obj[ErrorClearedKey])), ErrorClearedDesc));
        info.Add(ErrorDescriptionKey, (Convert.ToString(obj[ErrorDescriptionKey]), ErrorDescriptionDesc));
        info.Add(ExtClockKey, (Convert.ToString(obj[ExtClockKey]) + SpeedUnit, ExtClockDesc));
        info.Add(FamilyKey, (GetFamilyName(Convert.ToUInt16(obj[FamilyKey])), FamilyDesc));
        info.Add(ProcessorIdKey, (Convert.ToString(obj[ProcessorIdKey]), ProcessorIdDesc));
        info.Add(InstallDateKey, (Convert.ToString(Convert.ToDateTime(obj[InstallDateKey])), InstallDateDesc));
        info.Add(L2CacheSizeKey, (Convert.ToString(Convert.ToUInt32(obj[L2CacheSizeKey])), L2CacheSizeDesc));
        info.Add(L2CacheSpeedKey, (Convert.ToString(Convert.ToUInt32(obj[L2CacheSpeedKey])), L2CacheSpeedDesc));
        info.Add(L3CacheSizeKey, (Convert.ToString(Convert.ToUInt32(obj[L3CacheSizeKey])), L3CacheSizeDesc));
        info.Add(L3CacheSpeedKey, (Convert.ToString(Convert.ToUInt32(obj[L3CacheSpeedKey])), L3CacheSpeedDesc));
        info.Add(LastErrorCodeKey, (Convert.ToString(obj[LastErrorCodeKey]), LastErrorCodeDesc));
        info.Add(LevelKey, (Convert.ToString(obj[LevelKey]), LevelDesc));
        info.Add(LoadPercentageKey, (Convert.ToString(Convert.ToUInt16(obj[LoadPercentageKey])), LoadPercentageDesc));
        info.Add(NameKey, (Convert.ToString(obj[NameKey]), NameDesc));
        info.Add(ManufacturerKey, (Convert.ToString(obj[ManufacturerKey]), ManufacturerDesc));
        info.Add(MaxClockSpeedKey, (Convert.ToString(obj[MaxClockSpeedKey]) + SpeedUnit, MaxClockSpeedDesc));
        info.Add(NumberOfCoresKey, (Convert.ToString(obj[NumberOfCoresKey]), NumberOfCoresDesc));
        info.Add(NumberOfEnabledCoreKey, (Convert.ToString(obj[NumberOfEnabledCoreKey]), NumberOfEnabledCoreDesc));
        info.Add(NumberOfLogicalProcessorsKey, (Convert.ToString(obj[NumberOfLogicalProcessorsKey]), NumberOfLogicalProcessorsDesc));
        info.Add(OtherFamilyDescriptionKey, (Convert.ToString(obj[OtherFamilyDescriptionKey]), OtherFamilyDescriptionDesc));
        info.Add(PartNumberKey, (Convert.ToString(obj[PartNumberKey]), PartNumberDesc));
        info.Add(PNPDeviceIDKey, (Convert.ToString(obj[PNPDeviceIDKey]), PNPDeviceIDKey));
        info.Add(ProcessorTypeKey, (Convert.ToString(obj[ProcessorTypeKey]), ProcessorTypeDesc));
        info.Add(RevisionKey, (Convert.ToString(obj[RevisionKey]), RevisionDesc));
        info.Add(RoleKey, (Convert.ToString(obj[RoleKey]), RoleDesc));
        info.Add(SerialNumberKey, (Convert.ToString(obj[SerialNumberKey]), SerialNumberDesc));
        info.Add(SocketDesignationKey, (Convert.ToString(obj[SocketDesignationKey]), SocketDesignationDesc));
        info.Add(StatusKey, (Convert.ToString(obj[StatusKey]), StatusDesc));
        info.Add(StatusInfoKey, (Convert.ToString(GetStatusInfo(Convert.ToUInt16(obj[StatusInfoKey]))), StatusInfoDesc));
        info.Add(SteppingKey, (Convert.ToString(obj[SteppingKey]), SteppingDesc));
        info.Add(SystemCreationClassNameKey, (Convert.ToString(obj[SystemCreationClassNameKey]), SystemCreationClassNameDesc));
        info.Add(SystemNameKey, (Convert.ToString(obj[SystemNameKey]), SystemNameDesc));
        info.Add(ThreadCountKey, (Convert.ToString(obj[ThreadCountKey]), ThreadCountDesc));
        info.Add(UniqueIdKey, (Convert.ToString(obj[UniqueIdKey]), UniqueIdDesc));
        info.Add(UpgradeMethodKey, (Convert.ToString(GetUpgradeMethod(Convert.ToUInt16(obj[UpgradeMethodKey]))), UpgradeMethodDesc));
        info.Add(VersionKey, (Convert.ToString(obj[VersionKey]), VersionDesc));
        info.Add(VirtualizationFirmwareEnabledKey, (Convert.ToString(Convert.ToBoolean(obj[VirtualizationFirmwareEnabledKey])), VirtualizationFirmwareEnabledDesc));
        info.Add(VMMonitorModeExtensionsKey, (Convert.ToString(Convert.ToBoolean(obj[VMMonitorModeExtensionsKey])), VMMonitorModeExtensionsDesc));
        info.Add(VoltageCapsKey, (GetVoltageCaps(Convert.ToUInt32(obj[VoltageCapsKey])), VoltageCapsDesc));
      }
    }
    catch (ManagementException ex) {
      Console.WriteLine(ex.Message);
    }
    return info;
  }

  private static string GetArchitecture(ushort arch) {
    return arch switch
    {
      0 => "x86",
      1 => "MIPS",
      2 => "Alpha",
      3 => "PowerPC",
      5 => "ARM",
      6 => "Itanium-based systems",
      9 => "x64",
      12 => "ARM64",
      _ => "Unknown"
    };
  }

  private static string GetAvailability(ushort avail) {
    return avail switch
    {
      1 => "Other",
      2 => "Unknown",
      3 => "Running or Full Power",
      4 => "Warning",
      5 => "In Test",
      6 => "Not Applicable",
      7 => "Power Off",
      8 => "Off Line",
      9 => "Off Duty",
      10 => "Degraded",
      11 => "Not Installed",
      12 => "Install Error",
      13 => "Power Save -Unknown",
      14 => "Power Save -Low Power Mode",
      15 => "Power Save -Standby",
      16 => "Power Cycle",
      17 => "Power Save -Warning",
      18 => "Paused",
      19 => "Not Ready",
      20 => "Not Configured",
      21 => "Quiesced",
      _ => "Unknown"
    };
  }

  private static string GetConfigManagerErrorCode(uint errorCode) {
    return errorCode switch
    {
      0 => "This device is working properly. (0)",
      1 => "This device is not configured correctly. (1)",
      2 => "Windows cannot load the driver for this device. (2)",
      3 => "The driver for this device might be corrupted, or your system may be running low on memory or other resources. (3)",
      4 => "This device is not working properly. One of its drivers or your registry might be corrupted. (4)",
      5 => "The driver for this device needs a resource that Windows cannot manage. (5)",
      6 => "The boot configuration for this device conflicts with other devices. (6)",
      7 => "Cannot filter. (7)",
      8 => "The driver loader for the device is missing. (8)",
      9 => "This device is not working properly because the controlling firmware is reporting the resources for the device incorrectly. (9)",
      10 => "This device cannot start. (10)",
      11 => "This device failed. (11)",
      12 => "This device cannot find enough free resources that it can use. (12)",
      13 => "Windows cannot verify this device's resources. (13)",
      14 => "This device cannot work properly until you restart your computer. (14)",
      15 => "This device is not working properly because there is probably a re - enumeration problem. (15)",
      16 => "Windows cannot identify all the resources this device uses. (16)",
      17 => "This device is asking for an unknown resource type. (17)",
      18 => "Reinstall the drivers for this device. (18)",
      19 => "Failure using the VxD loader. (19)",
      20 => "Your registry might be corrupted. (20)",
      21 => "System failure: Try changing the driver for this device.If that does not work, see your hardware documentation.Windows is removing this device. (21)",
      22 => "This device is disabled. (22)",
      23 => "System failure: Try changing the driver for this device.If that doesn't work, see your hardware documentation. (23)",
      24 => "This device is not present, is not working properly, or does not have all its drivers installed. (24)",
      25 => "Windows is still setting up this device. (25)",
      26 => "Windows is still setting up this device. (26)",
      27 => "This device does not have valid log configuration. (27)",
      28 => "The drivers for this device are not installed. (28)",
      29 => "This device is disabled because the firmware of the device did not give it the required resources. (29)",
      30 => "This device is using an Interrupt Request(IRQ) resource that another device is using. (30)",
      31 => "This device is not working properly because Windows cannot load the drivers required for this device. (31)",
      _ => ""
    };
  }

  private static string GetCpuStatus(ushort status) {
    return status switch
    {
      0 => "Unknown(0)",
      1 => "CPU Enabled(1)",
      2 => "CPU Disabled by User via BIOS Setup(2)",
      3 => "CPU Disabled By BIOS (POST Error)(3)",
      4 => "CPU is Idle(4)",
      5 => "Reserved (5)",
      6 => "Reserved(6)",
      7 => "Other (7)",
      _ => "Unknown"
    };
  }

  private static string GetCurrentVoltage(ushort voltageRaw) {
    if ((voltageRaw & 0x80) != 0) {
      double voltage = (voltageRaw & 0x7F) / 10.0;
      return Convert.ToString(voltage) + "V";
    }
    else {
      return Convert.ToString(voltageRaw) + "V";
    }
  }

  private static string GetDataWidth(ushort dataWidth) {
    return dataWidth switch
    {
      32 => "32-bit",
      64 => "64-bit",
      _ => "Unknown"
    };
  }

  private static string GetStatusInfo(ushort info) {
    return info switch
    {
      1 => "Other(1)",
      2 => "Unknown (2)",
      3 => "Enabled(3)",
      4 => "Disabled (4)",
      5 => "Not Applicable (5)",
      _ => "Unknown"
    };
  }

  private static string GetUpgradeMethod(ushort upgradeMethod) {
    return upgradeMethod switch
    {
      1 => "Other(1)",
      2 => "Unknown (2)",
      3 => "Daughter Board (3)",
      4 => "ZIF Socket (4)",
      5 => "Replacement / Piggy Back(5)",
      6 => "None(6)",
      7 => "LIF Socket (7)",
      8 => "Slot 1(8)",
      9 => "Slot 2(9)",
      10 => "370 Pin Socket(10)",
      11 => "Slot A(11)",
      12 => "Slot M (12)",
      13 => "Socket 423(13)",
      14 => "Socket A (Socket 462)(14)",
      15 => "Socket 478(15)",
      16 => "Socket 754(16)",
      17 => "Socket 940(17)",
      18 => "Socket 939(18)",
      _ => "Unknown"
    };
  }

  private static string GetVoltageCaps(UInt32 caps) {
    return caps switch
    {
      1 => "5V",
      2 => "3.3V",
      4 => "2.9V",
      _ => ""
    };
  }

  private static string GetFamilyName(ushort family) {
    return family switch
    {
      1 => "other",
      2 => "Unknown",
      3 => "8086",
      4 => "80286",
      5 => "80386",
      6 => "80486",
      7 => "8087",
      8 => "80287",
      9 => "80387",
      10 => "80487",
      11 => "Pentium(R) brand",
      12 => "Pentium(R) Pro",
      13 => "Pentium(R) II",
      14 => "Pentium(R) processor with MMX(TM) technology",
      15 => "Celeron(TM)",
      16 => "Pentium(R) II Xeon(TM)",
      17 => "Pentium(R) III",
      18 => "M1 Family",
      19 => "M2 Family",
      20 => "Intel(R) Celeron(R)M processor",
      21 => "Intel(R)Pentium(R) 4 HT processor",
      24 => "K5 Family",
      25 => "K6 Family",
      26 => "K6 - 2",
      27 => "K6 - 3",
      28 => "AMD Athlon(TM)Processor Family",
      29 => "AMD(R)Duron(TM) Processor",
      30 => "AMD29000 Family",
      31 => "K6 - 2",
      32 => "Power PC Family",
      33 => "Power PC 601",
      34 => "Power PC 603",
      35 => "Power PC 603 +",
      36 => "Power PC 604",
      37 => "Power PC 620",
      38 => "Power PC X704",
      39 => "Power PC 750",
      40 => "Intel(R) Core(TM)Duo processor",
      41 => "Intel(R)Core(TM) Duo mobile processor",
      42 => "Intel(R) Core(TM)Solo mobile processor",
      43 => "Intel(R) Atom(TM)processor",
      48 => "Alpha Family",
      49 => "Alpha 21064",
      50 => "Alpha 21066",
      51 => "Alpha 21164",
      52 => "Alpha 21164PC",
      53 => "Alpha 21164a",
      54 => "Alpha 21264",
      55 => "Alpha 21364",
      56 => "AMD Turion(TM)II Ultra Dual-Core Mobile M Processor Family",
      57 => "AMD Turion(TM) II Dual-Core Mobile M Processor Family",
      58 => "AMD Athlon(TM) II Dual-Core Mobile M Processor Family",
      59 => "AMD Opteron(TM) 6100 Series Processor",
      60 => "AMD Opteron(TM) 4100 Series Processor",
      64 => "MIPS Family",
      65 => "MIPS R4000",
      66 => "MIPS R4200",
      67 => "MIPS R4400",
      68 => "MIPS R4600",
      69 => "MIPS R10000",
      80 => "SPARC Family",
      81 => "SuperSPARC",
      82 => "microSPARC II",
      83 => "microSPARC IIep",
      84 => "UltraSPARC",
      85 => "UltraSPARC II",
      86 => "UltraSPARC IIi",
      87 => "UltraSPARC III",
      88 => "UltraSPARC IIIi",
      96 => "68040",
      97 => "68xxx Family",
      98 => "68000",
      99 => "68010",
      100 => "68020",
      101 => "68030",
      112 => "Hobbit Family",
      120 => "Crusoe(TM) TM5000 Family",
      121 => "Crusoe(TM) TM3000 Family",
      122 => "Efficeon(TM) TM8000 Family",
      128 => "Weitek",
      130 => "Itanium(TM)Processor",
      131 => "AMD Athlon(TM)64 Processor Family",
      132 => "AMD Opteron(TM)Processor Family",
      133 => "AMD Sempron(TM) Processor Family",
      134 => "AMD Turion(TM)64 Mobile Technology",
      135 => "Dual - Core AMD Opteron(TM) Processor Family",
      136 => "AMD Athlon(TM)64 X2 Dual-Core Processor Family",
      137 => "AMD Turion(TM)64 X2 Mobile Technology",
      138 => "Quad - Core AMD Opteron(TM) Processor Family",
      139 => "Third - Generation AMD Opteron(TM) Processor Family",
      140 => "AMD Phenom(TM)FX Quad - Core Processor Family",
      141 => "AMD Phenom(TM) X4 Quad-Core Processor Family",
      142 => "AMD Phenom(TM)X2 Dual - Core Processor Family",
      143 => "AMD Athlon(TM) X2 Dual-Core Processor Family",
      144 => "PA - RISC Family",
      145 => "PA - RISC 8500",
      146 => "PA - RISC 8000",
      147 => "PA - RISC 7300LC",
      148 => "PA - RISC 7200",
      149 => "PA - RISC 7100LC",
      150 => "PA - RISC 7100",
      160 => "V30 Family",
      161 => "Quad - Core Intel(R) Xeon(R) processor 3200 Series",
      162 => "Dual - Core Intel(R) Xeon(R)processor 3000 Series",
      163 => "Quad - Core Intel(R) Xeon(R) processor 5300 Series",
      164 => "Dual - Core Intel(R) Xeon(R)processor 5100 Series",
      165 => "Dual - Core Intel(R) Xeon(R) processor 5000 Series",
      166 => "Dual - Core Intel(R) Xeon(R)processor LV",
      167 => "Dual - Core Intel(R) Xeon(R)processor ULV",
      168 => "Dual - Core Intel(R) Xeon(R) processor 7100 Series",
      169 => "Quad - Core Intel(R) Xeon(R) processor 5400 Series",
      170 => "Quad - Core Intel(R) Xeon(R)processor",
      171 => "Dual - Core Intel(R) Xeon(R) processor 5200 Series",
      172 => "Dual - Core Intel(R) Xeon(R)processor 7200 Series",
      173 => "Quad - Core Intel(R) Xeon(R) processor 7300 Series",
      174 => "Quad - Core Intel(R) Xeon(R)processor 7400 Series",
      175 => "Multi - Core Intel(R) Xeon(R) processor 7400 Series",
      176 => "Pentium(R)III Xeon(TM)",
      177 => "Pentium(R) III Processor with Intel(R)SpeedStep(TM) Technology",
      178 => "Pentium(R)4",
      179 => "Intel(R) Xeon(TM)",
      180 => "AS400 Family",
      181 => "Intel(R) Xeon(TM)processor MP",
      182 => "AMD Athlon(TM) XP Family",
      183 => "AMD Athlon(TM)MP Family",
      184 => "Intel(R)Itanium(R) 2",
      185 => "Intel(R) Pentium(R) M processor",
      186 => "Intel(R) Celeron(R)D processor",
      187 => "Intel(R)Pentium(R) D processor",
      188 => "Intel(R) Pentium(R)Processor Extreme Edition",
      189 => "Intel(R) Core(TM)Solo Processor",
      190 => "K7",
      191 => "Intel(R) Core(TM)2 Duo Processor",
      192 => "Intel(R) Core(TM)2 Solo processor",
      193 => "Intel(R) Core(TM)2 Extreme processor",
      194 => "Intel(R) Core(TM)2 Quad processor",
      195 => "Intel(R) Core(TM)2 Extreme mobile processor",
      196 => "Intel(R) Core(TM)2 Duo mobile processor",
      197 => "Intel(R) Core(TM)2 Solo mobile processor",
      198 => "Intel(R) Core(TM)i7 processor",
      199 => "Dual -Core Intel(R) Celeron(R)Processor",
      200 => "S/390 and zSeries Family",
      201 => "ESA/390 G4",
      202 => "ESA/390 G5",
      203 => "ESA/390 G6",
      204 => "z/Architectur base",
      205 => "Intel(R) Core(TM)i5 processor",
      206 => "Intel(R)Core(TM) i3 processor",
      207 => "Intel(R) Core(TM)i9 processor",
      210 => "VIA C7(TM) - M Processor Family",
      211 => "VIA C7(TM) - D Processor Family",
      212 => "VIA C7(TM)Processor Family",
      213 => "VIA Eden(TM) Processor Family",
      214 => "Multi - Core Intel(R) Xeon(R) processor",
      215 => "Dual - Core Intel(R) Xeon(R) processor 3xxx Series",
      216 => "Quad - Core Intel(R) Xeon(R) processor 3xxx Series",
      217 => "VIA Nano(TM)Processor Family",
      218 => "Dual -Core Intel(R) Xeon(R)processor 5xxx Series",
      219 => "Quad -Core Intel(R) Xeon(R)processor 5xxx Series",
      221 => "Dual -Core Intel(R) Xeon(R)processor 7xxx Series",
      222 => "Quad -Core Intel(R) Xeon(R)processor 7xxx Series",
      223 => "Multi -Core Intel(R) Xeon(R)processor 7xxx Series",
      224 => "Multi -Core Intel(R) Xeon(R)processor 3400 Series",
      230 => "Embedded AMD Opteron(TM) Quad - Core Processor Family",
      231 => "AMD Phenom(TM) Triple - Core Processor Family",
      232 => "AMD Turion(TM) Ultra Dual-Core Mobile Processor Family",
      233 => "AMD Turion(TM)Dual - Core Mobile Processor Family",
      234 => "AMD Athlon(TM)Dual - Core Processor Family",
      235 => "AMD Sempron(TM)SI Processor Family",
      236 => "AMD Phenom(TM)II Processor Family",
      237 => "AMD Athlon(TM)II Processor Family",
      238 => "Six - Core AMD Opteron(TM) Processor Family",
      239 => "AMD Sempron(TM)M Processor Family",
      250 => "i860",
      251 => "i960",
      260 => "SH - 3",
      261 => "SH - 4",
      280 => "ARM",
      281 => "StrongARM",
      300 => "6x86",
      301 => "MediaGX",
      302 => "MII",
      320 => "WinChip",
      350 => "DSP",
      500 => "Video Processor",
      _ => "Unknown"
    };
  }
}
