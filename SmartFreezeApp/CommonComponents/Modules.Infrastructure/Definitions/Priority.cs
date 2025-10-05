namespace Modules.Infrastructure.Definitions;

/// <summary>
/// Message priority
///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
/// </summary>
public enum Priority
{
  error = 0,
  warning = 1,
  attention = 2,
  normal = 3
}