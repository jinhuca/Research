namespace Console
{
    /// <summary>
    /// Represents the load cell interface
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public interface ILoadCell
    {
        int ID { get; set; }
        double LoadCellThresholdWarning { get; set; }
        double LoadCellThresholdFail { get; set; }
        double LoadCellLowRangeLimit { get; set; }
        double LoadCellHighRangeLimit { get; set; }
    }
}