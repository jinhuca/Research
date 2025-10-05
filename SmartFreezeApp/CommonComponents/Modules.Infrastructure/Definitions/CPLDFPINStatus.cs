namespace Modules.Infrastructure.Definitions;

/// <summary>
/// Central microcontroller valves status
///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
/// </summary>
[Flags]
public enum CPLDFPINStatus : byte
{
  NStopFootSwitch = 1, //0x01

  StopFootSwitch = 2, //0x02

  NStartFootSwitch = 4, //0x04

  StartFootSwitch = 8, //0x08

  NStopButton = 16,   //0x10 

  StopButton = 32,    //0x20

  NStartButton = 64,  //0x40

  StartButton = 128,  //0x80
}