namespace Console
{
    /// <summary>
    /// Represents the blood detector interface
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public interface IBloodDetector
    {
        int ID { get; set; }
    }
}