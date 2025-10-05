namespace Console
{
    /// <summary>
    /// Represents thermocouple one class
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class ThermocoupleOne : IThermocouple
    {
        private double thawingTemperature = 20;
        private double thawingTemperatureSetPoint = 0;
        private int iD = 1;

        /// <summary>
        /// Gets or sets the creates thermocouple one class
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public ThermocoupleOne() : base()
        {
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
            get
            {
                return thawingTemperature;
            }

            set
            {
                thawingTemperature = value;
            }
        }

        /// <summary>
        /// Gets or sets the thermocouple one ID
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

        public double ThawingTemperatureSetPoint
        {
            get => thawingTemperatureSetPoint;
            set => thawingTemperatureSetPoint = value;
        }
    }
}