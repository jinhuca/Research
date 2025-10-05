namespace Console
{
    /// <summary>
    /// Represents the injection flow class
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class InjectionFlow
    {
        private int injectionFlowId;
        private double targetInjectionFlow;

        /// <summary>
        /// Gets or sets the injection flow ID
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int InjectionFlowId
        {
            get
            {
                return injectionFlowId;
            }

            set
            {
                injectionFlowId = value;
            }
        }

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
    }
}