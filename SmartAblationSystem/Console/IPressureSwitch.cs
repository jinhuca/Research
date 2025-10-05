namespace Console
{
    /// <summary>
    /// Represents the pressure switch interface
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public interface IPressureSwitch
    {
        /// <summary>
        /// Gets or sets the pressure switch ID
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        int ID { get; set; }

        /// <summary>
        /// Gets or sets the pressure threshold high limit
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        double PressureThresholdHighLimit { get; set; }

        /// <summary>
        /// Gets or sets the pressure low range limit
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        double PressureLowRangeLimit { get; set; }

        /// <summary>
        /// Gets or sets the pressure high range limit
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        double PressureHighRangeLimit { get; set; }
    }
}