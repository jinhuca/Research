namespace Console
{
    /// <summary>
    /// Represents thermocouple class
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    internal class Thermocouple : IThermocouple
    {
        private int id;
        private double temperature;
        private double thawingTemperature = 20;

        /// <summary>
        /// Gets or sets the thermocouple Id
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int ID
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }

        /// <summary>
        /// Gets or sets the temperature
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double Temperature
        {
            get
            {
                return temperature;
            }

            set
            {
                temperature = value;
            }
        }

        /// <summary>
        /// Gets or sets the thawing temperature
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double ThawingTemperature
        {
            get
            {
                return thawingTemperature;
            }

            set
            {
                thawingTemperature = value;
            }
        }
    }
}