namespace Console
{
    /// <summary>
    /// Represents the pressure switch two class
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class PressureSwitchTwo : IPressureSwitch
    {
        private int iD = 2;
        private double pressureThresholdHighLimit;
        private double pressureLowRangeLimit;
        private double pressureHighRangeLimit;

        /// <summary>
        /// Gets or sets the pressure switch two
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
        /// Gets or sets the pressure threshold high limit
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PressureThresholdHighLimit
        {
            get
            {
                return pressureThresholdHighLimit;
            }

            set
            {
                pressureThresholdHighLimit = value;
            }
        }

        /// <summary>
        /// Gets or sets pressure low range limit
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PressureLowRangeLimit
        {
            get
            {
                return pressureLowRangeLimit;
            }

            set
            {
                pressureLowRangeLimit = value;
            }
        }

        /// <summary>
        /// Gets or sets the pressure high range limit
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PressureHighRangeLimit
        {
            get
            {
                return pressureHighRangeLimit;
            }

            set
            {
                pressureHighRangeLimit = value;
            }
        }
    }
}