namespace Console
{
    //  it is not a thermocouple. but we use the same interface because we are measrunig a temperature
    //TS1

    /// <summary>
    /// Represents the temperature sensor one class
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class TemperatureSensorOne : IThermocouple
    {
        private int iD = 3;

        private double temperatureThresholdHighLimit = -30;
        private double temperatureLowRangeLimit = -60;
        private double temperatureHighRangeLimit = 40;

        /// <summary>
        /// Gets or sets the temperature sensor one ID
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int ID
        {
            get
            {
                return this.iD;
            }

            set
            {
                this.iD = value;
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
        /// Gets or sets temperature threshold high limit
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double TemperatureThresholdHighLimit
        {
            get
            {
                return temperatureThresholdHighLimit;
            }

            set
            {
                temperatureThresholdHighLimit = value;
            }
        }

        /// <summary>
        /// Gets or sets the temperature low range limit
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double TemperatureLowRangeLimit
        {
            get
            {
                return temperatureLowRangeLimit;
            }

            set
            {
                temperatureLowRangeLimit = value;
            }
        }

        /// <summary>
        /// Gets or sets the temperature high range limit
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double TemperatureHighRangeLimit
        {
            get
            {
                return temperatureHighRangeLimit;
            }

            set
            {
                temperatureHighRangeLimit = value;
            }
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