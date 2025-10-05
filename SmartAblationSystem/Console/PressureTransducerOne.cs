namespace Console
{
    /// <summary>
    /// Represents the pressure transducer one class
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class PressureTransducerOne : IPressureTransducer
    {
        //PT1_F
        private double tankPressureTooHigh = 1000;

        //PT1_L
        private double tankPressureLow = 600;

        private int iD = 1;

        /// <summary>
        /// Creates the pressure transducer one class
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public PressureTransducerOne() : base()
        {
        }

        /// <summary>
        /// Gets or sets the pressure high range limit
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PressureHighRangeLimit
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the pressure threshold high limit
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PressureThresholdHighLimit
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the pressure low range limit
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PressureLowRangeLimit
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the tank pressure low
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double TankPressureLow
        {
            get
            {
                return tankPressureLow;
            }

            set
            {
                tankPressureLow = value;
            }
        }

        /// <summary>
        /// Gets or sets the current pressure
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double CurrentPressure
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the tank pressure too high
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double TankPressureTooHigh
        {
            get
            {
                return tankPressureTooHigh;
            }

            set
            {
                tankPressureTooHigh = value;
            }
        }

        /// <summary>
        /// Gets or sets the pressure tank ID
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
    }
}