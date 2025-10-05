namespace FileSerializer
{
  using Shared;

  /// <summary>
  /// This class contains properties for Ablation Data details.
  ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
  /// </summary>
  public class AblationDataDetails : AblationData
  {
    /// <summary>
    /// Gets or sets the ablation site
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int AblationSite { get; set; }

    /// <summary>
    /// Gets or sets the temperature rate
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double TemperatureRate { get; set; }

    /// <summary>
    /// Gets or sets the maximum temperature rate
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double MaxTemperatureRate { get; set; }

    /// <summary>
    /// Gets or sets the time in ablation
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int TimeInAblation { get; set; }

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

    /// <summary>
    /// Gets or sets the time to vein isolation
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int TimeToVeinIsolation { get; set; }

    public int TimeSinceVeinIsolation { get; set; }

    public int TemperatureAtIsolation { get; set; }

    public int TotalThawingTime { get; set; }

    /// <summary>
    /// Gets or sets the Exception State Time
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int ExceptionStateTime { get; set; }

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
    /// Gets or sets the Catheter Extended Container Tag string 
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
    /// Gets or sets the TC1 Reading value
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double TC1Reading { get; set; }

    /// <summary>
    /// Gets or sets the time in seconds
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int TimeInSecondIndex { get; set; }

    /// <summary>
    /// Gets or sets the PMCU CJ Reading value
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double PMCUCJReading { get; set; }

    /// <summary>
    /// Gets or sets the PT1 Reading value
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double PT1Reading { get; set; }

    /// <summary>
    /// Gets or sets the PT2 Reading value
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double PT2Reading { get; set; }

    /// <summary>
    /// Gets or sets the PT3 Reading value
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double PT3Reading { get; set; }

    /// <summary>
    /// Gets or sets the PT4 Reading value
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double PT4Reading { get; set; }

    /// <summary>
    /// Gets or sets the PT5 Reading value
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double PT5Reading { get; set; }

    /// <summary>
    /// Gets or sets the PS1 Reading value
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double PS1Reading { get; set; }

    /// <summary>
    /// Gets or sets the FM1 Reading value
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double FM1Reading { get; set; }

    /// <summary>
    /// Gets or sets the TS1 Reading value
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double TS1Reading { get; set; }

    /// <summary>
    /// Gets or sets the TN2O Reading value
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double TN2OReading { get; set; }

    /// <summary>
    /// Gets or sets the LC1 Reading value
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double LC1Reading { get; set; }

    /// <summary>
    /// Gets or sets the TIP Reading value
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double TIPReading { get; set; }

    /// <summary>
    /// Gets or sets the inner balloon pressure Reading value
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double CP1Reading { get; set; }//IBP

    /// <summary>
    /// Gets or sets the outer balloon pressure Reading value
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double CP2Reading { get; set; }//OBP

    /// <summary>
    /// Gets or sets the CIMP1 Reading value
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double CIMP1Reading { get; set; }

    /// <summary>
    /// Gets or sets the pulse Width Modulation for the injection
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double PWMINJ { get; set; }

    /// <summary>
    /// Gets or sets the Pulse Width Modulation for the balloon
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double PWMBAL { get; set; }

    /// <summary>
    /// Gets or sets whether thaw temperature is reached
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsThawTemperatureReached { get; set; }

    /// <summary>
    /// Gets or sets whether target temperature is reached
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsTargetTemperatureReached { get; set; }

    /// <summary>
    /// Gets or sets the procedure ID
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int ProcedureId { get; set; }
    /// <summary>
    /// Gets or sets the skin to skin duration
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int SkinToSkinDuration { get; set; }

    /// <summary>
    /// Gets or sets the value indicting whether is data edited or not
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsDataEdited { get; set; } = false;

    /// <summary>
    /// Gets or sets value of CMCUCJ reading
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double CMCUCJReading { get; set; }

    //Hardware Information

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
    /// Gets or sets value of Ecg Channel 1 And 2 Reading ( Blood Pressure (mmHg) )
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double EcgChannel1And2Reading { get; set; }

    /// <summary>
    /// Gets or sets value of Ecg Channel 3 And 4 Reading ( DMS Value (G) )
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double EcgChannel3And4Reading { get; set; }

    /// <summary>
    /// Gets or sets value of Ecg Channel 7 And 8 Reading ( DMS Value (%)  )
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double EcgChannel7And8Reading { get; set; }

    /// <summary>
    /// Gets or sets value of Ecg Channel 5 And 6 Reading  (ESO Temp)
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int EcgChannel5And6Reading { get; set; }


    /// <summary>
    /// Gets or sets value of Blood Detecor Im Value  (BDI)
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int BloodDetecorImValue { get; set; }

    /// <summary>
    /// Gets or sets value of the Remote Firmware
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public string RemoteFirmware { get; set; }

    public double PressureSetPoint { get; set; }

    public double EtsSensor1 { get; set; }

    public double EtsSensor2 { get; set; }

    public double EtsSensor3 { get; set; }

    public double EtsSensor4 { get; set; }

    public double EtsSensor5 { get; set; }

    public double EtsSensor6 { get; set; }

    public double EtsSensor7 { get; set; }

    public double EtsSensor8 { get; set; }

    public double EtsSensor9 { get; set; }

    public double EtsSensor10 { get; set; }

    public double EtsSensor11 { get; set; }

    public double EtsSensor12 { get; set; }

    public double EtsSensor13 { get; set; }

    public bool ISTTISelected { get; set; }

    // Transferred from AblationECGData, which is removed according to Jira PLX-1265 Reduce the File Size for JSON Files  

    /// <summary>
    /// Gets or sets the Esophagus Temperature Threshold Reached value
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool EsophagusTemperatureThresholdReached { get; set; }

    /// <summary>
    /// Gets or sets the Esophagus Temperature value
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int EsophagusTemperature { get; set; }

    /// <summary>
    /// Gets or sets the Is Diaphragm Movement Detected value
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsDiaphragmMovementDetected { get; set; }

    /// <summary>
    /// Gets or sets the Diaphragm Amplitude value
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int DiaphragmAmplitude { get; set; }

    /// <summary>
    /// Gets or sets the Diaphragm Amplitude Threshold Reached value
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool DiaphragmAmplitudeThresholdReached { get; set; }

    /// <summary>
    /// Gets or sets the Ignore Minimum Diaphragm Movement value
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IgnoreMinimumDiaphragmMovement { get; set; }

    /// <summary>
    /// Gets or sets the diaphragm sensor gain value
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int DiaphragmSensorGain { get; set; }

    /// <summary>
    /// Gets or sets the value indicating whether system monitoring diaphram is alert or not
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsSystemMonitoringDiaphragmAlert { get; set; }

    public string BalloonSize { get; set; }

    public bool IsLowFlowActivated { get; set; }

    public AblationDataDetails UpdateBalloonSizeIfEmpty()
    {
      if (string.IsNullOrEmpty(BalloonSize))
      {
        BalloonSize = SharedConstants.BalloonSizeFromPressureSetPoint(PressureSetPoint); 
      }

      return this;
    }

  }
}