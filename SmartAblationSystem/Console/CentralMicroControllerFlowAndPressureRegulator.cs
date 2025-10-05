namespace Console
{
    /// <summary>
    /// Represents the central microController flow and pressure regulator
    /// </summary>
    public class CentralMicroControllerFlowAndPressureRegulator
    {
        private double targetInjectionFlow;
        private double targetInjectionPressure;
        private double targetInjectionLowFlow;

        /// <summary>
        /// Gets or sets the target injection flow
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double TargetInjectionFlow
        {
            get
            {
                return targetInjectionFlow;
            }

            set
            {
                targetInjectionFlow = value;
            }
        }

        /// <summary>
        /// Gets or sets target injection pressure
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double TargetInjectionPressure
        {
            get
            {
                return targetInjectionPressure;
            }

            set
            {
                targetInjectionPressure = value;
            }
        }

        /// <summary>
        /// Gets or sets the target injection flow
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double TargetInjectionLowFlow
        {
            get
            {
                return targetInjectionLowFlow;
            }

            set
            {
                targetInjectionLowFlow = value;
            }
        }
    }
}