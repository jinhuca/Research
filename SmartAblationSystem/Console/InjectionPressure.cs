namespace Console
{
    /// <summary>
    /// Represents the injection pressure class
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class InjectionPressure
    {
        private int injectionPressureId;
        private double targetInjectionPressure;

        /// <summary>
        /// Gets or sets the Injection pressure id
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int InjectionPressureId
        {
            get
            {
                return injectionPressureId;
            }

            set
            {
                injectionPressureId = value;
            }
        }

        /// <summary>
        /// Gets or sets the target injection pressure
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
    }
}