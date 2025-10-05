namespace Console
{
    /// <summary>
    /// Represents thermocouple two  class
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    internal class ThermocoupleTwo : IThermocouple
    {
        private int iD = 2;

        /// <summary>
        /// Gets or sets the thermocouple two ID
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int ID
        {
            get
            {
                return iD;
            }

            set
            {
                iD = value;
            }
        }

        /// <summary>
        /// Gets or sets the temperature
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double Temperature
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the thawing temperature
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double ThawingTemperature
        {
            get;
            set;
        }
    }
}