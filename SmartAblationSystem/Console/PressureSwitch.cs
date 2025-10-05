namespace Console
{
    /// <summary>
    /// Represents the pressure switch class
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class PressureSwitch : IPressureSwitch
    {
        private int iD;
        private double pressureThresholdHighLimit;
        private double pressureLowRangeLimit;
        private double pressureHighRangeLimit;

        /// <summary>
        /// Creates  the pressure switch  class
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public PressureSwitch()
        {
        }

        /// <summary>
        /// Gets or sets the pressure switch Id
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
        /// Gets or sets pressure high range limit
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