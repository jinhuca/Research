namespace Console
{
    /// <summary>
    /// Represents the load cell class
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class LoadCell : ILoadCell
    {
        private int iD;
        private double loadCellThresholdWarning;
        private double loadCellThresholdFail;
        private double loadCellLowRangeLimit;
        private double loadCellHighRangeLimit;

        /// <summary>
        /// Gets or sets the load cell Id
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