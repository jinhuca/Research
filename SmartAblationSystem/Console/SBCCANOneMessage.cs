namespace Console
{
    /// <summary>
    /// Represents SBC CAN 1 message class
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class SBCCANOneMessage
    {
        /// <summary>
        /// Message element ID
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public enum ElementId
        {
            Calibration = 62,
            CPLDValve = 1,
            AudioControl = 4,
            WDTWakeUpSignal = 63,
            PowerOffSignal = 16,
        }
    }

    /// <summary>
    /// Represents the SBC CAN 2 message class
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class SBCCANTWOMessage
    {
        /// <summary>
        /// Single board computer CAN 2 message
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public enum ElementId
        {
            ConnectionBoxState = 10
        }
    }
}