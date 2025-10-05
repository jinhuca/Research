namespace Console
{
    /// <summary>
    /// Represents LC1 class
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class LoadCellOne : ILoadCell
    {
        private int iD = 1;
        private double loadCellThresholdWarning = 4.5;
        private double loadCellThresholdFail = 3.5;
        private double loadCellLowRangeLimit = 15;
        private double loadCellHighRangeLimit = 26;

        /// <summary>
        /// Gets or sets the LC1 ID
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
        /// Gets or sets the load cell threshold warning
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double LoadCellThresholdWarning
        {
            get
            {
                return loadCellThresholdWarning;
            }

            set
            {
                loadCellThresholdWarning = value;
            }
        }

        /// <summary>
        /// Gets or sets the load cell threshold fail
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double LoadCellThresholdFail
        {
            get
            {
                return loadCellThresholdFail;
            }

            set
            {
                loadCellThresholdFail = value;
            }
        }

        /// <summary>
        /// Gets or sets the load cell low range limit
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double LoadCellLowRangeLimit
        {
            get
            {
                return loadCellLowRangeLimit;
            }

            set
            {
                loadCellLowRangeLimit = value;
            }
        }

        /// <summary>
        /// Gets or sets the load cell high range limit
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double LoadCellHighRangeLimit
        {
            get
            {
                return loadCellHighRangeLimit;
            }

            set
            {
                loadCellHighRangeLimit = value;
            }
        }
    }
}