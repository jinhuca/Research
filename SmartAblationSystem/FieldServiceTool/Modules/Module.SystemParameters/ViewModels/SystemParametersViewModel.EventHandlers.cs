using System.ComponentModel;
using Module.Infrastructure.AppLog;
using Module.Infrastructure.TestResults.Implementation;

namespace Module.SystemParameters.ViewModels
{
  /// <summary>
  /// Event Handlers for <see cref="SystemParametersViewModel"/>.
  /// </summary>
  public partial class SystemParametersViewModel
  {
    private void SensorParameters_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      switch (e.PropertyName)
      {
        case nameof(_systemParameterModel.SensorParameters.Temperature):
          Temperature = _systemParameterModel.SensorParameters.Temperature;
          break;
        case nameof(_systemParameterModel.SensorParameters.FM1):
          FM1 = _systemParameterModel.SensorParameters.FM1;
          break;
        case nameof(_systemParameterModel.SensorParameters.IBP):
          IBP = _systemParameterModel.SensorParameters.IBP;
          break;
        case nameof(_systemParameterModel.SensorParameters.OBP):
          OBP = _systemParameterModel.SensorParameters.OBP;
          break;
        case nameof(_systemParameterModel.SensorParameters.LC):
          LC = _systemParameterModel.SensorParameters.LC;
          break;
        case nameof(_systemParameterModel.SensorParameters.PWM1):
          PWM1 = _systemParameterModel.SensorParameters.PWM1;
          break;
        case nameof(_systemParameterModel.SensorParameters.PWM2):
          PWM2 = _systemParameterModel.SensorParameters.PWM2;
          break;
        case nameof(_systemParameterModel.SensorParameters.PT1):
          PT1 = _systemParameterModel.SensorParameters.PT1;
          break;
        case nameof(_systemParameterModel.SensorParameters.PT2):
          PT2 = _systemParameterModel.SensorParameters.PT2;
          break;
        case nameof(_systemParameterModel.SensorParameters.PT3):
          PT3 = _systemParameterModel.SensorParameters.PT3;
          break;
        case nameof(_systemParameterModel.SensorParameters.PT4):
          PT4 = _systemParameterModel.SensorParameters.PT4;
          break;
        case nameof(_systemParameterModel.SensorParameters.PT5):
          PT5 = _systemParameterModel.SensorParameters.PT5;
          break;
        case nameof(_systemParameterModel.SensorParameters.TS1):
          TS1 = _systemParameterModel.SensorParameters.TS1;
          break;
        case nameof(_systemParameterModel.SensorParameters.PGain):
          PGain = _systemParameterModel.SensorParameters.PGain;
          break;
        case nameof(_systemParameterModel.SensorParameters.IGain):
          IGain = _systemParameterModel.SensorParameters.IGain;
          break;
        case nameof(_systemParameterModel.SensorParameters.DGain):
          DGain = _systemParameterModel.SensorParameters.DGain;
          break;
        case nameof(_systemParameterModel.SensorParameters.PatientPGain):
          PatientPGain = _systemParameterModel.SensorParameters.PatientPGain;
          break;
        case nameof(_systemParameterModel.SensorParameters.PatientIGain):
          PatientIGain = _systemParameterModel.SensorParameters.PatientIGain;
          break;
        case nameof(_systemParameterModel.SensorParameters.PatientDGain):
          PatientDGain = _systemParameterModel.SensorParameters.PatientDGain;
          break;
        case nameof(_systemParameterModel.SensorParameters.PIDOffset):
          PIDOffset = _systemParameterModel.SensorParameters.PIDOffset;
          break;
        case nameof(_systemParameterModel.SensorParameters.PatientPIDOffset):
          PatientPIDOffset = _systemParameterModel.SensorParameters.PatientPIDOffset;
          break;
      }
    }

    private void VersionTestResult_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      switch (e.PropertyName)
      {
        case nameof(_systemParameterModel.VersionTestResult.CMCUBootVersion):
          CMCUBoot = _systemParameterModel.VersionTestResult.CMCUBootVersion;
          break;
        case nameof(_systemParameterModel.VersionTestResult.CMCUVersion):
          CMCU = _systemParameterModel.VersionTestResult.CMCUVersion;
          break;
        case nameof(_systemParameterModel.VersionTestResult.PMCUBootVersion):
          PMCUBoot = _systemParameterModel.VersionTestResult.PMCUBootVersion;
          break;
        case nameof(_systemParameterModel.VersionTestResult.PMCUVersion):
          PMCU = _systemParameterModel.VersionTestResult.PMCUVersion;
          break;
        case nameof(_systemParameterModel.VersionTestResult.RMCUBootVersion):
          RMCUBoot = _systemParameterModel.VersionTestResult.RMCUBootVersion;
          break;
        case nameof(_systemParameterModel.VersionTestResult.RMCUVersion):
          RMCU = _systemParameterModel.VersionTestResult.RMCUVersion;
          break;
        case nameof(_systemParameterModel.VersionTestResult.ICBBootVersion):
          ICBBootVersion = _systemParameterModel.VersionTestResult.ICBBootVersion;
          break;
        case nameof(_systemParameterModel.VersionTestResult.ICBVersion):
          ICBVersion = _systemParameterModel.VersionTestResult.ICBVersion;
          break;
        case nameof(_systemParameterModel.VersionTestResult.RCMCUBootVersion):
          RCMCUBoot = _systemParameterModel.VersionTestResult.RCMCUBootVersion;
          break;
        case nameof(_systemParameterModel.VersionTestResult.RCMCUVersion):
          RCMCUVersion = _systemParameterModel.VersionTestResult.RCMCUVersion;
          break;
        case nameof(_systemParameterModel.VersionTestResult.CPLDVersion):
          CPLD = _systemParameterModel.VersionTestResult.CPLDVersion;
          break;
        case nameof(_systemParameterModel.VersionTestResult.GUIVersion):
          GUIVersion = _systemParameterModel.VersionTestResult.GUIVersion;
          break;
        case nameof(_systemParameterModel.VersionTestResult.DBVersion):
          DBVersion = _systemParameterModel.VersionTestResult.DBVersion;
          break;
      }
    }

  }
}
