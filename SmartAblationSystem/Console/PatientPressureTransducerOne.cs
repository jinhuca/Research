namespace Console
{
    //CP1

    /// <summary>
    /// Represents the patient pressure transducer One class
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class PatientPressureTransducerOne : IPressureTransducer
    {
        private double pressureHighlimit = 15;
        private double pressureLowerLimit = -17;
        private double pressureUpperlimit = 17;
        private int id = 5;

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
        /// Gets or sets the pressure Low range limit
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
        /// Gets or sets the current pressure
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double CurrentPressure
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Pressure ID
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
    }
}