namespace Console
{
    //FM1
    /// <summary>
    ///  Represents the flow one (FM1) meter class
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class FlowMeterOne : IFlowMeter
    {
        private int iD = 1;
        private double flowMeterThresholLowlimit = 4000;
        private double flowMeterThresholHighlimit = 8000;
        private double flowMeterLowRangeLimit;
        private double flowMeterHighRangelimit = 10000;

        /// <summary>
        /// Gets or sets the flow meter one ID
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
        /// Gets or sets the value of flow meter threshold low limit
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
        /// Gets or sets the flow meter threshold high limit
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
        /// Gets or sets flow meter high range limit
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