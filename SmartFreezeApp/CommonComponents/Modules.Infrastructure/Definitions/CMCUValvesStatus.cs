namespace Modules.Infrastructure.Definitions;

/// <summary>
/// Central microcontroller valves status
///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
/// </summary>
public enum CMCUValvesStatus : Int64
{
  SolenoidValve1ON = 1,

  SolenoidValve2ON = 2,

  SolenoidValve3ON = 4,

  SolenoidValve4ON = 8,

  SolenoidValve5ON = 16,

  SolenoidValve6ON = 32,

  SolenoidValve7ON = 64,

  SolenoidValve8ON = 128,

  SolenoidValve9ON = 256,

}