using static Communication.CanBusMessageDefinition;

namespace Module.Console.Helpers
{
  /// <summary>
  /// This class handles the ID To MachineState conversion
  /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
  /// </summary>
  public static class IdToMachineState
  {
    /// <summary>
    /// Converts an ID to a State ID
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="id">A integer representing an ID to convert.</param>
    /// <returns>An integer representing a converted state ID.</returns>
    public static int ConvertIdToSate(uint id)
    {
      int stateId = (((int)id & (int)Mask.CAN_ID_STATE_MASK)) >> 8;
      return stateId;
    }
  }
}