namespace Console
{
    /// <summary>
    /// Represents  the central micro controller PID
    /// </summary>
    public class CentralMicroControllerPID : IPID
    {
        private double pGain;
        private double iGain;
        private double dGain;
        private double offset;

        /// <summary>
        /// Gets or sets the proportional gain
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PGain
        {
            get
            {
                return pGain;
            }

            set
            {
                pGain = value;
            }
        }

        /// <summary>
        /// Gets or sets the integral gain
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double IGain
        {
            get
            {
                return iGain;
            }

            set
            {
                iGain = value;
            }
        }

        /// <summary>
        /// Gets or sets the derivative gain
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double DGain
        {
            get
            {
                return dGain;
            }

            set
            {
                dGain = value;
            }
        }

        /// <summary>
        /// Gets or sets the offset
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double Offset
        {
            get
            {
                return offset;
            }

            set
            {
                offset = value;
            }
        }
    }
}