using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console.Configurations
{
    /// <summary>
    /// Represents  the CryoBalloon Configuration
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class CryoBalloonConfiguration
    {
        private double rampUpTimeByStep;
        private double rampDownTimeByStep;
        private double pressureRampUpValue;
        private double pressureRampDownValue;
        private double dASLowFlow;


        /// <summary>
        /// Gets or sets the ramp up time by step
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double RampUpTimeByStep
        {
            get => rampUpTimeByStep;
            set => rampUpTimeByStep = value;
        }

        /// <summary>
        /// Gets or sets the pressure ramp up value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PressureRampUpValue
        {
            get => pressureRampUpValue;
            set => pressureRampUpValue = value;
        }

        /// <summary>
        /// Gets or sets the ramp down time by step
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double RampDownTimeByStep
        {
            get => rampDownTimeByStep;
            set => rampDownTimeByStep = value;
        }

        /// <summary>
        /// Gets or sets the pressure ramp down value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PressureRampDownValue
        {
            get => pressureRampDownValue;
            set => pressureRampDownValue = value;
        }

        /// <summary>
        /// Gets or sets the DAS Low Flow value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double DASLowFlow
        {
            get => dASLowFlow;
            set => dASLowFlow = value;
        }
    }
}
