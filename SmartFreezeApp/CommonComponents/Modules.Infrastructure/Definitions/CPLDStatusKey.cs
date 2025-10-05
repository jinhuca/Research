namespace Modules.Infrastructure.Definitions;

/// <summary>
/// CPLD and CMCU status key
///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
/// </summary>
public enum CPLDStatusKey : Int64
{
  CMCUPASS = 44033,  //AC01 the data are inverted 
  CMCUPASSINTERMEDAIREITERMEDIARYPASS = 44034, //AC02 the data are inverted 
  CMCUANDCPLDPASS = 44035, //AC03 the data are inverted 
  CMCUFAIL = 48385, //0xBD01
  INTERMEDAIREITERMEDIARYFAIL = 48386, //0xBD02
  CMCUANDCPLDFAIL = 48387, //0xBD03

}