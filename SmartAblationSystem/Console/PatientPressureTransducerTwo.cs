namespace Console
{
    //CP2

    /// <summary>
    /// Represents the patient pressure transducer two class
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class PatientPressureTransducerTwo : IPressureTransducer
    {
        private double pressureHighlimit = -2;
        private double pressureLowerLimit = -17;
        private double pressureUpperlimit = 17;
        private int iD = 6;

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
        /// Gets or sets the pressure high range limit
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PressureHighRangeLimit
        {
            get
            {
                return pressureHighlimit;
            }

            set
            {
                pressureHighlimit = value;
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
                return pressureLowerLimit;
            }

            set
            {
                pressureLowerLimit = value;
            }
        }

        /// <summary>
        /// Gets or sets the pressure low range limit
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PressureLowRangeLimit
        {
            get
            {
                return pressureUpperlimit;
            }

            set
            {
                pressureUpperlimit = value;
            }
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