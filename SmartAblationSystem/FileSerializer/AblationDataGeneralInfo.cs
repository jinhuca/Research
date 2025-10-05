
namespace FileSerializer
{
  public class AblationDataGeneralInfo
  {
    /// <summary>
    /// Gets or sets the hospital name
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public string Hospital { get; set; }

    /// <summary>
    /// Gets or sets Database Version
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int DatabaseVersion { get; set; }

    /// <summary>
    /// Gets or sets GUI Version
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public string GUIVersion { get; set; }

    /// <summary>
    /// Gets or sets the time to target temperature
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int TimeToTargetTemperature { get; set; }

    /// <summary>
    /// Gets or sets the required ablation time
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int RequiredAblationTime { get; set; }

    public int TotalThawingTime { get; set; }

    public int? TimeSinceIsolation { get; set; }

    public int? TemperatureAtIsolation { get; set; }

    /// <summary>
    /// Gets or sets the time to vein isolation
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int TimeToVeinIsolation { get; set; }
    /// <summary>
    /// Gets or sets the required target temperature
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int RequiredTargetTemperature { get; set; }

    /// <summary>
    /// Gets or sets the time to thaw
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int TimeToThaw { get; set; }

    /// <summary>
    /// Gets or sets the Catheter ID
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int CatheterId { get; set; }

    /// <summary>
    /// Gets or sets the Catheter Lot
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int CatheterLot { get; set; }


    /// <summary>
    /// Gets or sets the Catheter Serial Number
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int CatheterSerialNumber { get; set; }

    /// <summary>
    /// Gets or sets the Catheter extended Container tag
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public string CatheterContainer { get; set; }

    /// <summary>
    /// Gets or sets the Catheter Type Id
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsUsedForEngineering { get; set; }

    /// <summary>
    /// Gets or sets the Thaw Timer To Temperature
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int ThawTimerToTemperature { get; set; }

    /// <summary>
    /// Gets or sets value of the CMCU Firmware
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public string CMCUFirmware { get; set; }

    /// <summary>
    /// Gets or sets value of the PMCU Firmware
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public string PMCUFirmware { get; set; }

    /// <summary>
    /// Gets or sets value of the Repeater Firmware
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public string RepeaterFirmware { get; set; }

    /// <summary>
    /// Gets or sets value of the ICB Firmware
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public string ICBFirmware { get; set; }

    /// <summary>
    /// Gets or sets value of the Catheter Firmware
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public string CatheterFirmware { get; set; }

    /// <summary>
    /// Gets or sets value of the CPLD Firmware
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public string CPLDFirmware { get; set; }

    /// <summary>
    /// Gets or sets value of the Console Serial Number
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public string ConsoleSerialNumber { get; set; }

    /// <summary>
    /// Gets or sets value of the Remote Firmware
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public string RemoteFirmware { get; set; }

    /// <summary>
    /// Gets or sets the ablation data ID
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int AblationID { get; set; } //Treatment Number

    /// <summary>
    /// Gets or sets the Pressure set point
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double PressureSetPoint { get; set; }

    /// <summary>
    /// Gets or sets the value indicting whether is data edited or not
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsDataEdited { get; set; } = false;

    /// <summary>
    /// Gets or sets the procedure ID
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int ProcedureId { get; set; }

    public string BalloonSize { get; set; }
  }
}
