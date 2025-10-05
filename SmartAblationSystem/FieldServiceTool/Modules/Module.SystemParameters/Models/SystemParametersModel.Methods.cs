using System;
using System.Threading.Tasks;
using Module.SystemParameters.Extensions;

namespace Module.SystemParameters.Models
{
	/// <summary>
	/// Methods for <see cref="SystemParametersModel"/>.
	/// </summary>
	public partial class SystemParametersModel
	{
		private const string EmptyVersion = "0.0.0.0";
		public string GetConsoleSerialNum()
		{
			return _dataAccess.GetConsoleSerialNumber();
		}

    private void UpdateVersionResult()
		{
			var versions = _dataAccess.GetLatestVersion();
      
      // Only ICB and Remote Control can be Disconnected. We reset the versions, in case they are disconnected.  
      ResetIcbAndRemoteControlVersions(); 
			_machineModel.ReadFirmwareVersions();

      VersionTestResult.GUIVersion = string.IsNullOrEmpty(versions.Software) ? EmptyVersion : versions.Software;
      VersionTestResult.DBVersion = string.IsNullOrEmpty(versions.DataBaseVersion) ? EmptyVersion : versions.DataBaseVersion;

      Task.Delay(500).ContinueWith( _ =>
      {
        VersionTestResult.CMCUBootVersion = _machineModel.CentralMicroControllerBootLoaderFirmwareVersion.ToVersionString();
        VersionTestResult.CMCUVersion = _machineModel.CentralMicroControllerFirmwareVersion.ToVersionString();
        VersionTestResult.CPLDVersion = _machineModel.CpldFirmwareVersion.ToString("X");
        VersionTestResult.PMCUBootVersion = _machineModel.PatientMicroControllerBootLoaderFirmwareVersion.ToVersionString();
        VersionTestResult.PMCUVersion = _machineModel.PatientMicroControllerFirmwareVersion.ToVersionString();
        VersionTestResult.RMCUBootVersion = _machineModel.RepeaterBootLoaderFirmware.ToVersionString();
        VersionTestResult.RMCUVersion = _machineModel.RepeaterFirmware.ToVersionString();

        VersionTestResult.RCMCUBootVersion = _machineModel.RemoteControlBootLoaderFirmwareVersion.ToVersionString();
        VersionTestResult.RCMCUVersion = _machineModel.RemoteControlFirmware.ToVersionString();
        VersionTestResult.ICBBootVersion = _machineModel.ICBBootLoaderFirmwareVersion.ToVersionString();
        VersionTestResult.ICBVersion = _machineModel.ICBFirmware.ToVersionString();
      });
    }

    private void ResetIcbAndRemoteControlVersions()
    {
      _machineModel.RemoteControlBootLoaderFirmwareVersion = 0;
      _machineModel.RemoteControlFirmware = 0;
      _machineModel.ICBBootLoaderFirmwareVersion = 0;
      _machineModel.ICBFirmware = 0;
		}

    private void InitializeSensorParameterValues()
    {
      SensorParameters.Temperature = _machineModel.TC1Reading; 
      SensorParameters.FM1 = _machineModel.FM1Reading;
      SensorParameters.IBP = _machineModel.CP1Reading;
      SensorParameters.OBP = _machineModel.CP2Reading;
      SensorParameters.LC = _machineModel.LC1Reading;
      SensorParameters.PT1 = _machineModel.PT1Reading;
      SensorParameters.PT2 = _machineModel.PT2Reading;
      SensorParameters.PT3 = _machineModel.PT3Reading;
      SensorParameters.PT4 = _machineModel.PT4Reading;
      SensorParameters.PT5 = _machineModel.PT5Reading;
      SensorParameters.TS1 = _machineModel.TS1Reading;
      SensorParameters.PWM1 = _machineModel.PIDDutyCycle;
      SensorParameters.PWM2 = _machineModel.PatientPIDDutyCycle;
      SensorParameters.PGain = _machineModel.PGain;
      SensorParameters.IGain = _machineModel.IGain;
      SensorParameters.DGain = _machineModel.DGain;
      SensorParameters.PIDOffset = _machineModel.PIDOffset;
      SensorParameters.PatientPGain = _machineModel.PatientPGain;
      SensorParameters.PatientIGain = _machineModel.PatientIGain;
      SensorParameters.PatientDGain = _machineModel.PatientDGain;
      SensorParameters.PatientPIDOffset = _machineModel.PatientPIDOffset;
    }
  }
}
