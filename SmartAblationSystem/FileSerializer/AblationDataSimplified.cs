
namespace FileSerializer
{
  /// <summary>
  /// This class contains properties for simplified Ablation Data details.
  ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
  /// </summary>
  public class AblationDataSimplified
  {
    public AblationDataSimplified() { }

    public AblationDataSimplified(AblationDataDetails ablationDataDetails)
    {
      AblationSite = ablationDataDetails.AblationSite;
      TemperatureRate = ablationDataDetails.TemperatureRate;
      MaxTemperatureRate = ablationDataDetails.MaxTemperatureRate;
      TimeInAblation = ablationDataDetails.TimeInAblation;
      ExceptionStateTime = ablationDataDetails.ExceptionStateTime;
      TC1Reading = ablationDataDetails.TC1Reading;

      PMCUCJReading = ablationDataDetails.PMCUCJReading;
      PT1Reading = ablationDataDetails.PT1Reading;
      PT2Reading = ablationDataDetails.PT2Reading;
      PT3Reading = ablationDataDetails.PT3Reading;
      PT4Reading = ablationDataDetails.PT4Reading;
      PT5Reading = ablationDataDetails.PT5Reading;
      PS1Reading = ablationDataDetails.PS1Reading;
      FM1Reading = ablationDataDetails.FM1Reading;
      TS1Reading = ablationDataDetails.TS1Reading;
      TN2OReading = ablationDataDetails.TN2OReading;
      LC1Reading = ablationDataDetails.LC1Reading;
      TIPReading = ablationDataDetails.TIPReading;
      CP1Reading = ablationDataDetails.CP1Reading;
      CP2Reading = ablationDataDetails.CP2Reading;
      CIMP1Reading = ablationDataDetails.CIMP1Reading;
      PWMINJ = ablationDataDetails.PWMINJ;
      PWMBAL = ablationDataDetails.PWMBAL;
      IsThawTemperatureReached = ablationDataDetails.IsThawTemperatureReached;
      IsTargetTemperatureReached = ablationDataDetails.IsTargetTemperatureReached;

      // ProcedureId = ablationDataDetails.ProcedureId;
      SkinToSkinDuration = ablationDataDetails.SkinToSkinDuration;

      // IsDataEdited = ablationDataDetails.IsDataEdited;
      CMCUCJReading = ablationDataDetails.CMCUCJReading;
      EcgChannel1And2Reading = ablationDataDetails.EcgChannel1And2Reading;
      EcgChannel3And4Reading = ablationDataDetails.EcgChannel3And4Reading;
      EcgChannel5And6Reading = ablationDataDetails.EcgChannel5And6Reading;
      EcgChannel7And8Reading = ablationDataDetails.EcgChannel7And8Reading;
      BloodDetecorImValue = ablationDataDetails.BloodDetecorImValue;

      EtsSensor1 = ablationDataDetails.EtsSensor1;
      EtsSensor2 = ablationDataDetails.EtsSensor2;
      EtsSensor3 = ablationDataDetails.EtsSensor3;
      EtsSensor4 = ablationDataDetails.EtsSensor4;
      EtsSensor5 = ablationDataDetails.EtsSensor5;
      EtsSensor6 = ablationDataDetails.EtsSensor6;
      EtsSensor7 = ablationDataDetails.EtsSensor7;
      EtsSensor8 = ablationDataDetails.EtsSensor8;
      EtsSensor9 = ablationDataDetails.EtsSensor9;
      EtsSensor10 = ablationDataDetails.EtsSensor10;
      EtsSensor11 = ablationDataDetails.EtsSensor11;
      EtsSensor12 = ablationDataDetails.EtsSensor12;
      EtsSensor13 = ablationDataDetails.EtsSensor13;

      ISTTISelected = ablationDataDetails.ISTTISelected;

      TimeStamp = ablationDataDetails.TimeStamp;
      ID = ablationDataDetails.ID;
      SystemState = ablationDataDetails.SystemState;
      Error = ablationDataDetails.Error;
      MinimumDiaphragmMovementValue = ablationDataDetails.MinimumDiaphragmMovementValue;
      MinimumEsophagusTemperatureValue = ablationDataDetails.MinimumEsophagusTemperatureValue;
      EsophagusTemperatureThresholdReached = ablationDataDetails.EsophagusTemperatureThresholdReached;
      EsophagusTemperature = ablationDataDetails.EsophagusTemperature;
      IsDiaphragmMovementDetected = ablationDataDetails.IsDiaphragmMovementDetected;
      DiaphragmAmplitude = ablationDataDetails.DiaphragmAmplitude;
      DiaphragmAmplitudeThresholdReached = ablationDataDetails.DiaphragmAmplitudeThresholdReached;
      IgnoreMinimumDiaphragmMovement = ablationDataDetails.IgnoreMinimumDiaphragmMovement;
      DiaphragmSensorGain = ablationDataDetails.DiaphragmSensorGain;
      IsSystemMonitoringDiaphragmAlert = ablationDataDetails.IsSystemMonitoringDiaphragmAlert;
    }

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
    /// Gets or sets the Exception State Time
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int ExceptionStateTime { get; set; }
    /// <summary>
    /// Gets or sets the TC1 Reading value
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double TC1Reading { get; set; }

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

    /// <summary>
    /// Gets or sets the ablation time stamp
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public string TimeStamp { get; set; }

    /// <summary>
    /// Gets or sets the ablation data ID
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int ID { get; set; } // we are using the ID as a time

    /// <summary>
    /// Gets or sets the ablation data system state
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int SystemState { get; set; }

    /// <summary>
    /// Gets or sets the error
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public string Error { get; set; }

    /// <summary>
    /// Gets or sets the Minimum Diaphragm Movement Value
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int MinimumDiaphragmMovementValue { get; set; } = 100;

    /// <summary>
    /// Gets or sets Minimum Esophagus Temperature Value
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int MinimumEsophagusTemperatureValue { get; set; } = 100;

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
  }
}
