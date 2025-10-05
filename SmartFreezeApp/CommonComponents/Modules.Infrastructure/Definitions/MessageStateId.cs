namespace Modules.Infrastructure.Definitions;

/// <summary>
/// Console state
///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
/// </summary>
public enum MessageStateId
{
  CAN_ID_STATE_UNKNOWN = 0,
  CAN_ID_STATE_IDLE = 256,
  CAN_ID_STATE_READY = 512,
  CAN_ID_STATE_INFLATION = 768,
  CAN_ID_STATE_TRANSITION = 1024,
  CAN_ID_STATE_ABLATION = 1280,
  CAN_ID_STATE_THAWING = 1536,
  CAN_ID_STATE_EXCEPTION = 1792
}