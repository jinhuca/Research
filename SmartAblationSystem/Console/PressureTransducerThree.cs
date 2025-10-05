namespace Console
{
    /// <summary>
    /// Represents pressure transducer three class
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class PressureTransducerThree : IPressureTransducer
    {
        private int iD = 3;

        /// <summary>
        /// Creates pressure transducer three class
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public PressureTransducerThree()
        {
        }

        /// <summary>
        /// Gets or sets the pressure high Range Limit
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