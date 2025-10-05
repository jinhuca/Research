namespace Modules.Infrastructure.Definitions;

/// <summary>
/// Central microcontroller errors
///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
/// </summary>
[Flags]
public enum CMCUStatusError : Int64
{
  ExceptionType1 = 536870912, //0x20000000

  ExceptionType2 = 1073741824, // 0x40000000

  ExceptionType3 = 1610612736, // 0x60000000

  ExceptionType4 = 2147483648,  // 0x80000000

  ExceptionType5 = 268435456,  // 0x10000000

  CPLDWatchDogTimerError = 1, // 0x00000001

  TwoMultiplexReadingDoesNotMatch = 2, // 0x00000002

  FlowTooHigh = 4, // 0x00000004

  FlowTooLow = 8, // 0x00000008

  FlowReadingOutOfRange = 16, // 0x00000010

  LoadCellWeightWarning = 32, // 0x00000020

  LoadCellWeightFail = 64, // 0x00000040

  LoadCellReadingOutOfRange = 128, // 0x00000080

  PressureInTankIsHighFanToBeOn = 256, // 0x00000100

  PressurePT1InTankIsLow = 512, // 0x00000200

  PressurePT1InTankIsTooHigh = 1024, // 0x00000400

  PressurePT1InTankReadingOutOfRange = 2048, // 0x00000800

  PressurePT2AfterCatheterButBeforeReturnLineTooHigh = 4096, // 0x00001000

  PT2ReadingOutOfRange = 8192, //0x00002000

  ReturnPressurePT3TooHigh = 16384, //0x00004000

  ReturnPressurePT3OutOfRange = 32768, //0x00008000

  VacuumPressurePT4TooHigh = 65536, //0x00010000

  VacuumPressurePT4OutOfRange = 131072, //0x00020000

  SubCoolerTemperatureIsHigh = 262144, //0x00040000

  SubCoolerTemperatureOutOfRange = 524288, // 0x00080000

  InjectionVentPressureIsHigh = 1048576, // 0x00100000

  InjectionVentPressureOutOfRange = 2097152, // 0x00200000

  ScavengingPressureIsHigh = 4194304, // 0x00400000

  CatheterTubeConnected = 33554432, // 0x02000000

  SelfTestFail = 67108864,  // 0x04000000

  FootSwitchLock = 8388608, // 0x00800000

  VeinIsolated = 16777216,               //0x01000000

  CMCUReady = 134217728, // 0x08000000
}