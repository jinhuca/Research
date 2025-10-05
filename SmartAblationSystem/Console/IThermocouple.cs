namespace Console
{
    /// <summary>
    /// Represents the thermocouple interface
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public interface IThermocouple
    {
        /// <summary>
        /// Gets or sets the thermocouple ID
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        int ID { get; set; }

        /// <summary>
        /// Gets or sets the thermocouple temperature
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        double Temperature { get; set; }

        /// <summary>
        /// Gets or sets the thawing temperature
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        double ThawingTemperature { get; set; }
    }
}