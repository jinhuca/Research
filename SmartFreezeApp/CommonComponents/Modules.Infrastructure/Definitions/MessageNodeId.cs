namespace Modules.Infrastructure.Definitions;

/// <summary>
/// Message node ID
/// </summary>
public enum MessageNodeId
{
  // patient MCU
  patientMicrocontroller = 0,

  // central MCU
  mainMicrocontroller = 1, //0x0800

  // Single Board Computer
  singleBoardComputer = 2, // 0x1000

  //From Connection Box CAN node
  canIdNodeConnBus2 = 3,

  //From Single Board Computer CAN2 node
  canIdNodeSbcBus2 = 4
}