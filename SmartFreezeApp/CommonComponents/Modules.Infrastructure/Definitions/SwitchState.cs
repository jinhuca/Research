namespace Modules.Infrastructure.Definitions;

/// <summary>
/// Switch State enumeration
/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
/// </summary>
public enum SwitchState
{
  Unknown = 0,
  SwitchStateDeactivated = 255,
  AblationTimerDecrement = 254,
  AblationTimerIncrement = 253,
  AblationSiteLeft = 251,
  StartButton = 247,
  StopButton = 239,
  AblationSiteRight = 223,
  BalloonDiameterIncrease = 191,
  BalloonDiameterDecrease = 127

}