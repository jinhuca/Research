namespace Console
{
    /// <summary>
    /// Represents the pressure transducer interface
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public interface IPressureTransducer
    {
        //When there is no lower limit value.By convention i will use int min value
        //When there is no higher limit value. By convention i will use int max value

        /// <summary>
        /// Gets or sets the pressure high range limit
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        double PressureHighRangeLimit { get; set; }

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
        /// Gets or sets the current pressure
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        double CurrentPressure { get; set; }

        /// <summary>
        /// Gets or sets the pressure transducer ID
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        int ID { get; set; }
    }
}