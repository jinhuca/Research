namespace Console
{
    /// <summary>
    /// Represents the patient microController balloon pressure regulator class
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class PatientMicroControllerBalloonPressureRegulator
    {
        private double targetBalloonPressure;

        /// <summary>
        /// Gets or sets the target balloon pressure
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double TargetBalloonPressure
        {
            get
            {
                return targetBalloonPressure;
            }

            set
            {
                targetBalloonPressure = value;
            }
        }
    }
}