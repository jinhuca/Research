namespace Modules.Infrastructure.Definitions;

/// <summary>
/// Patient microcontroller errors
///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
/// </summary>
[Flags]
public enum PMCUStatusError : Int64
{
  ExceptionType1 = 536870912, //0x20000000

  ExceptionType2 = 1073741824, // 0x40000000

  ExceptionType3 = 1610612736, // 0x60000000

  ExceptionType4 = 2147483648,  // 0x80000000

  ExceptionType5 = 268435456,  // 0x10000000

  CPLDWatchDogTimerError = 1, // 0x00000001

  SelfTestFail = 2,  //0x00000002

  InnerBalloonPressureTooHigh = 4, // 0x00000004

  InnerBalloonPressureTooLow = 8, //0x00000008 

  //InnerBalloonPressureReadingOutOfRange = 8, //0x00000008

  BalloonTemperatureLowWarning = 16, //0x00000010

  OuterBalloonPressureTooHigh = 32, //0x00000020

  OuterBalloonPressureReadingOutOrRange = 64, //0x00000040

  BalloonTipPressureTooHigh = 128, //0x00000080

  BalloonTipPressureTooLow = 256, //0x00000100

  BalloonTipPressurePeadingOutOfRange = 512, //0x00000200

  ThawingTemperatureTooHigh = 1024, //0x00000400

  ThawingTemperatureTooLow = 2048, //0x00000800

  BalloonTemperatureTooHigh = 4096, // 0x0001000

  BloodDetectedInCatheter = 16384, // 0x0004000

  BloodDetectorOpenWires = 32768, // 0x00008000

  CatheterCableConnected = 16777216, //0x01000000

  //SelfTestFail = 67108864,  // 0x04000000

  PMCUReady = 134217728, // 0x08000000
}