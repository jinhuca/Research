using DevExpress.Xpf.Gauges;

namespace SmartAblationSystem.Converters
{
    /// <summary>
    /// This class provides Segments States
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class SegmentsStatesProvider
    {
        private StatesMaskConverter converter = new StatesMaskConverter();

        /// <summary>
        /// Gets a Round Segment Mapping StatesMask value.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public StatesMask RoundSegmentsMappingMask
        {
            get
            {
                return (StatesMask)converter.ConvertFromString(@"1 1 1 1 1 1 1 1 1 1 1 1 1
                                                                                                            1 1 1 1 1 1 1 1 1 1 1 1 1
                                                                                                            1 1 1 1 1 1 1 1 1 1 1 1 1
                                                                                                            1 1 1 1 1 1 1 1 1 1 1 1 1
                                                                                                            1 1 1 1 1 1 1 1 1 1 1 1 1
                                                                                                            1 1 1 1 1 1 1 1 1 1 1 1 1
                                                                                                            1 1 1 1 1 1 1 1 1 1 1 1 1
                                                                                                            1 1 1 1 1 1 1 1 1 1 1 1 1
                                                                                                            1 1 1 1 1 1 1 1 1 1 1 1 1
                                                                                                            1 1 1 1 1 1 1 1 1 1 1 1 1
                                                                                                            1 1 1 1 1 1 1 1 1 1 1 1 1
                                                                                                            1 1 1 1 1 1 1 1 1 1 1 1 1
                                                                                                            1 1 1 1 1 1 1 1 1 1 1 1 1");
            }
        }

        /// <summary>
        /// Gets an Arrow Segment Mapping StatesMask value.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public StatesMask ArrowSegmentsMappingMask
        {
            get
            {
                return (StatesMask)converter.ConvertFromString(@"0 0 0 0 0 0 0 0 0 0 0 0 0
                                                                                                            0 0 0 0 0 0 0 1 0 0 0 0 0
                                                                                                            0 0 0 0 0 0 0 1 1 0 0 0 0
                                                                                                            0 0 0 0 0 0 0 1 1 1 0 0 0
                                                                                                            1 1 1 1 1 1 1 1 1 1 1 0 0
                                                                                                            1 1 1 1 1 1 1 1 1 1 1 1 0
                                                                                                            1 1 1 1 1 1 1 1 1 1 1 1 1
                                                                                                            1 1 1 1 1 1 1 1 1 1 1 1 0
                                                                                                            1 1 1 1 1 1 1 1 1 1 1 0 0
                                                                                                            0 0 0 0 0 0 0 1 1 1 0 0 0
                                                                                                            0 0 0 0 0 0 0 1 1 0 0 0 0
                                                                                                            0 0 0 0 0 0 0 1 0 0 0 0 0
                                                                                                            0 0 0 0 0 0 0 0 0 0 0 0 0");
            }
        }
    }
}