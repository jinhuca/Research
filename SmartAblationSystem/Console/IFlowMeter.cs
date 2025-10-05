namespace Console
{
    /// <summary>
    /// Represents the flow meter interface
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public interface IFlowMeter
    {
        double FlowMeterThresholLowlimit { get; set; }
        double FlowMeterThresholHighlimit { get; set; }
        double FlowMeterLowRangeLimit { get; set; }
        double FlowMeterHighRangelimit { get; set; }
        int ID { get; set; }
    }
}