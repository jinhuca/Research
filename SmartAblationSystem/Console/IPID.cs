namespace Console
{
    /// <summary>
    /// Represents the PID interface
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public interface IPID
    {
        /// <summary>
        /// Gets or sets the PID derivative value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        double DGain { get; set; }

        /// <summary>
        /// Gets or sets the PID integral value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        double IGain { get; set; }

        /// <summary>
        /// Gets or sets the PID offset value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        double Offset { get; set; }

        /// <summary>
        /// Gets or sets the  PID proportional value 
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        double PGain { get; set; }
    }
}