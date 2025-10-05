namespace Console
{
    /// <summary>
    /// Represents the tank class
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class Tank
    {
        /// <summary>
        /// Tank type
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public enum TankType
        {
            Unknown = 0,
            TENN_POUND_TANK = 1,
            FIFTEEN_POUND_TANK = 2
        }

        private int iD = 1;
        private double maximumWeight = 0;
        private double minimumWeight = 0;
        private double metalWeight = 18;
        private double n2OWeight = 0;

        /// <summary>
        /// Gets or sets the tank maximum weight
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double MaximumWeight
        {
            get
            {
                return maximumWeight;
            }

            set
            {
                maximumWeight = value;
            }
        }

        /// <summary>
        /// Gets or sets the tank minimum weight
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double MinimumWeight
        {
            get
            {
                return minimumWeight;
            }

            set
            {
                minimumWeight = value;
            }
        }

        /// <summary>
        /// Gets or sets the tank metal weight
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double MetalWeight
        {
            get
            {
                return metalWeight;
            }

            set
            {
                metalWeight = value;
            }
        }

        /// <summary>
        /// Gets or sets the nitrous oxide weight
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double N2OWeight
        {
            get
            {
                return n2OWeight;
            }

            set
            {
                n2OWeight = value;
            }
        }

        /// <summary>
        /// Gets or sets the tank ID
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

        /// <summary>
        /// Creates the tank class
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public Tank()
        {
        }
    }
}