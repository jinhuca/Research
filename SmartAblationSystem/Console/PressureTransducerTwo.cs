namespace Console
{
    /// <summary>
    /// Represents the pressure transducer two  class
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class PressureTransducerTwo : IPressureTransducer
    {
        private int iD = 2;

        /// <summary>
        /// Creates the pressure transducer two  class
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public PressureTransducerTwo() : base()
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
        /// Gets or sets the current pressure
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double CurrentPressure
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the pressure ID
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