using System;

namespace Console
{
    /// <summary>
    /// Represents the pressure transducer  class
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class PressureTransducer : IPressureTransducer
    {
        private double pressureHighlimit;
        private double pressureLowerLimit;
        private double pressureUpperlimit;

        /// <summary>
        /// Creates the pressure transducer  class
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public PressureTransducer()
        {
        }

        /// <summary>
        /// Gets or sets the pressure ID.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int ID
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
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
        /// Gets the current pressure
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double CurrentPressure
        {
            get;
            set;
        }
    }
}