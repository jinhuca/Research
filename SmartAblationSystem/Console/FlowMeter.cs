namespace Console
{
    /// <summary>
    ///  Represents the flow meter class
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    internal class FlowMeter : IFlowMeter
    {
        private int iD;
        private double flowMeterThresholLowlimit;
        private double flowMeterThresholHighlimit;
        private double flowMeterLowRangeLimit;
        private double flowMeterHighRangelimit;

        /// <summary>
        /// Gets or sets the flow metter ID
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
        /// Gets or sets the flow meter threshold low limit
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double FlowMeterThresholLowlimit
        {
            get
            {
                return flowMeterThresholLowlimit;
            }

            set
            {
                flowMeterThresholLowlimit = value;
            }
        }

        /// <summary>
        /// Gets or sets the flow Meter threshold high limit
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double FlowMeterThresholHighlimit
        {
            get
            {
                return flowMeterThresholHighlimit;
            }

            set
            {
                flowMeterThresholHighlimit = value;
            }
        }

        /// <summary>
        /// Gets or sets the flow meter low range limit
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double FlowMeterLowRangeLimit
        {
            get
            {
                return flowMeterLowRangeLimit;
            }

            set
            {
                flowMeterLowRangeLimit = value;
            }
        }

        /// <summary>
        /// Gets or sets the flow meter high range limit
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double FlowMeterHighRangelimit
        {
            get
            {
                return flowMeterHighRangelimit;
            }

            set
            {
                flowMeterHighRangelimit = value;
            }
        }
    }
}