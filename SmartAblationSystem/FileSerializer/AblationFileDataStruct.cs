using System;
using System.Collections.Generic;
using System.Linq;
using static Communication.CanBusMessageDefinition;
using static Shared.SharedConstants; 

namespace FileSerializer
{
  public class AblationFileDataStruct
  {
    public static AblationFileDataStruct ConvertAblationDataDetailsToFileStruct(IList<AblationDataDetails> singleDataDetailsList)
    {
      var data = new AblationFileDataStruct();
      if (singleDataDetailsList == null || singleDataDetailsList.Count == 0)
      {
        return data;
      }

      var lastDetailData = singleDataDetailsList.LastOrDefault();
      var totalThawingTime_ = singleDataDetailsList.Count(x => x.SystemState == (int)MessageStateId.CAN_ID_STATE_THAWING);

      int? temperatureAtTii_;
      int? timeSinceTii_;
      if (singleDataDetailsList.Any(x => x.TimeToVeinIsolation > 0))
      {
        temperatureAtTii_ = (int)singleDataDetailsList.First(x => x.TimeToVeinIsolation > 0).TC1Reading;
        timeSinceTii_ = (int)singleDataDetailsList.Count(x => 
          x.TimeToVeinIsolation > 0 && 
          (x.SystemState == (int)MessageStateId.CAN_ID_STATE_ABLATION || x.SystemState == (int)MessageStateId.CAN_ID_STATE_TRANSITION)) - 1;
      }
      else
      {
        temperatureAtTii_ = null;
        timeSinceTii_ = null;
      }

      data.GeneralInfo = new AblationDataGeneralInfo()
      {
        Hospital = lastDetailData?.Hospital,
        DatabaseVersion = lastDetailData?.DatabaseVersion ?? 0,
        GUIVersion = lastDetailData?.GUIVersion,
        TimeToTargetTemperature = lastDetailData?.TimeToTargetTemperature ?? 0,
        RequiredAblationTime = lastDetailData?.RequiredAblationTime ?? 0,
        TimeToVeinIsolation = lastDetailData?.TimeToVeinIsolation ?? 0,
        TimeSinceIsolation = timeSinceTii_,
        TemperatureAtIsolation = temperatureAtTii_,
        RequiredTargetTemperature = lastDetailData?.RequiredTargetTemperature ?? 0,
        TimeToThaw = lastDetailData?.TimeToThaw ?? 0,
        CatheterId = lastDetailData?.CatheterId ?? 0,
        CatheterLot = lastDetailData?.CatheterLot ?? 0,
        CatheterSerialNumber = lastDetailData?.CatheterSerialNumber ?? 0,
        CatheterContainer = lastDetailData?.CatheterContainer?? string.Empty,  
        IsUsedForEngineering = lastDetailData?.IsUsedForEngineering ?? false,

        ThawTimerToTemperature = lastDetailData?.ThawTimerToTemperature ?? 0,

        CMCUFirmware = lastDetailData?.CMCUFirmware,
        PMCUFirmware = lastDetailData?.PMCUFirmware,
        RepeaterFirmware = lastDetailData?.RepeaterFirmware,

        ICBFirmware = lastDetailData?.ICBFirmware,
        CatheterFirmware = lastDetailData?.CatheterFirmware,
        CPLDFirmware = lastDetailData?.CPLDFirmware,
        ConsoleSerialNumber = lastDetailData?.ConsoleSerialNumber,
        RemoteFirmware = lastDetailData?.RemoteFirmware,
        PressureSetPoint = lastDetailData?.PressureSetPoint ?? 0d,
        AblationID = lastDetailData?.AblationID ?? 0,

        ProcedureId = lastDetailData.ProcedureId,
        IsDataEdited = lastDetailData.IsDataEdited,

        BalloonSize = lastDetailData.BalloonSize,
        TotalThawingTime = totalThawingTime_
      };

      data.AblationDataDetails = singleDataDetailsList
        .Select(ab => new AblationDataSimplified(ab))
        .ToList();

      return data;
    }

    public List<AblationDataDetails> ConvertToAblationDataDetails()
    {
      return AblationDataDetails
        .Select(ab =>
        {
          var timeToVeinIsolation = GeneralInfo?.TimeToVeinIsolation ?? 0;
          return new AblationDataDetails()
          {
            // General Information
            Hospital = GeneralInfo?.Hospital,
            DatabaseVersion = GeneralInfo?.DatabaseVersion ?? 0,
            GUIVersion = GeneralInfo?.GUIVersion,
            TimeToTargetTemperature = GeneralInfo?.TimeToTargetTemperature ?? 0,
            RequiredAblationTime = GeneralInfo?.RequiredAblationTime ?? 0,
            TimeToVeinIsolation = ab.ID >= timeToVeinIsolation ? timeToVeinIsolation : 0,
            TimeSinceVeinIsolation = GeneralInfo?.TimeSinceIsolation ?? 0,
            TemperatureAtIsolation = GeneralInfo?.TemperatureAtIsolation ?? 0,
            RequiredTargetTemperature = GeneralInfo?.RequiredTargetTemperature ?? 0,
            TimeToThaw = GeneralInfo?.TimeToThaw ?? 0,

            CatheterId = GeneralInfo?.CatheterId ?? 0,
            CatheterLot = GeneralInfo?.CatheterLot ?? 0,
            CatheterSerialNumber = GeneralInfo?.CatheterSerialNumber ?? 0,
            CatheterContainer = GeneralInfo?.CatheterContainer?? string.Empty, 

            IsUsedForEngineering = GeneralInfo?.IsUsedForEngineering ?? false,

            ThawTimerToTemperature = GeneralInfo?.ThawTimerToTemperature ?? 0,

            CMCUFirmware = GeneralInfo?.CMCUFirmware,
            PMCUFirmware = GeneralInfo?.PMCUFirmware,
            RepeaterFirmware = GeneralInfo?.RepeaterFirmware,

            ICBFirmware = GeneralInfo?.ICBFirmware,
            CatheterFirmware = GeneralInfo?.CatheterFirmware,
            CPLDFirmware = GeneralInfo?.CPLDFirmware,
            ConsoleSerialNumber = GeneralInfo?.ConsoleSerialNumber,
            RemoteFirmware = GeneralInfo?.RemoteFirmware,
            PressureSetPoint = GeneralInfo?.PressureSetPoint ?? 0d,
            AblationID = GeneralInfo?.AblationID ?? 0,
            ProcedureId = GeneralInfo?.ProcedureId ?? 0,
            IsDataEdited = GeneralInfo?.IsDataEdited ?? false,

            // Ablation Details 
            AblationSite = ab.AblationSite,
            TemperatureRate = ab.TemperatureRate,
            MaxTemperatureRate = ab.MaxTemperatureRate,
            TimeInAblation = ab.TimeInAblation,
            ExceptionStateTime = ab.ExceptionStateTime,
            TC1Reading = ab.TC1Reading,

            PMCUCJReading = ab.PMCUCJReading,
            PT1Reading = ab.PT1Reading,
            PT2Reading = ab.PT2Reading,
            PT3Reading = ab.PT3Reading,
            PT4Reading = ab.PT4Reading,
            PT5Reading = ab.PT5Reading,
            PS1Reading = ab.PS1Reading,
            FM1Reading = ab.FM1Reading,
            TS1Reading = ab.TS1Reading,
            TN2OReading = ab.TN2OReading,
            LC1Reading = ab.LC1Reading,
            TIPReading = ab.TIPReading,
            CP1Reading = ab.CP1Reading,
            CP2Reading = ab.CP2Reading,
            CIMP1Reading = ab.CIMP1Reading,
            PWMINJ = ab.PWMINJ,
            PWMBAL = ab.PWMBAL,
            IsThawTemperatureReached = ab.IsThawTemperatureReached,
            IsTargetTemperatureReached = ab.IsTargetTemperatureReached,

            // ProcedureId = ab.ProcedureId,
            SkinToSkinDuration = ab.SkinToSkinDuration,

            // IsDataEdited = ab.IsDataEdited,
            CMCUCJReading = ab.CMCUCJReading,
            EcgChannel1And2Reading = ab.EcgChannel1And2Reading,
            EcgChannel3And4Reading = ab.EcgChannel3And4Reading,
            EcgChannel5And6Reading = ab.EcgChannel5And6Reading,
            EcgChannel7And8Reading = ab.EcgChannel7And8Reading,
            BloodDetecorImValue = ab.BloodDetecorImValue,

            EtsSensor1 = ab.EtsSensor1,
            EtsSensor2 = ab.EtsSensor2,
            EtsSensor3 = ab.EtsSensor3,
            EtsSensor4 = ab.EtsSensor4,
            EtsSensor5 = ab.EtsSensor5,
            EtsSensor6 = ab.EtsSensor6,
            EtsSensor7 = ab.EtsSensor7,
            EtsSensor8 = ab.EtsSensor8,
            EtsSensor9 = ab.EtsSensor9,
            EtsSensor10 = ab.EtsSensor10,
            EtsSensor11 = ab.EtsSensor11,
            EtsSensor12 = ab.EtsSensor12,
            EtsSensor13 = ab.EtsSensor13,

            ISTTISelected = ab.ISTTISelected,

            TimeStamp = ab.TimeStamp,
            ID = ab.ID,
            SystemState = ab.SystemState,
            Error = ab.Error,
            MinimumDiaphragmMovementValue = ab.MinimumDiaphragmMovementValue,
            MinimumEsophagusTemperatureValue = ab.MinimumEsophagusTemperatureValue,

            EsophagusTemperatureThresholdReached = ab.EsophagusTemperatureThresholdReached,
            EsophagusTemperature = ab.EsophagusTemperature,
            IsDiaphragmMovementDetected = ab.IsDiaphragmMovementDetected,
            DiaphragmAmplitude = ab.DiaphragmAmplitude,
            DiaphragmAmplitudeThresholdReached = ab.DiaphragmAmplitudeThresholdReached,
            IgnoreMinimumDiaphragmMovement = ab.IgnoreMinimumDiaphragmMovement,
            DiaphragmSensorGain = ab.DiaphragmSensorGain,
            IsSystemMonitoringDiaphragmAlert = ab.IsSystemMonitoringDiaphragmAlert,
            BalloonSize = string.IsNullOrEmpty(GeneralInfo?.BalloonSize)
                            ? BalloonSizeFromPressureSetPoint(GeneralInfo?.PressureSetPoint ?? 2.5)
                            : GeneralInfo?.BalloonSize,
          };
        })
        .ToList();
    }

    public AblationDataGeneralInfo GeneralInfo { get; set; }
    public IList<AblationDataSimplified> AblationDataDetails { get; set; }
  }
}
