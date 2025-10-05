namespace Modules.Infrastructure.Definitions;

/// <summary>
/// Module keys
///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
/// </summary>
public enum ModuleKeys : Int64
{
  CMCUKey = 49374, //0xC0DE
  CPLDKey = 50398, //0xC4DE
  PMCUKey = 50910, //0xC6DE 
  RMCUKey = 51422, //0xC8DE
  RCMCUKey = 51934, // 0xCADE
  BMCUKey = 52446, // 0xCCDE     
  CMCUREBOOT = 45057, //B001
  PMCUREBOOT = 45058, //B002
  RMCUREBOOT = 45059, //B003
  BMCUREBOOT = 45060 // B004
}