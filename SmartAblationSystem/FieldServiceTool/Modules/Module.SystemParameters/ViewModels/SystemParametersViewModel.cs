using Module.SystemParameters.Models;
using Prism.Mvvm;

namespace Module.SystemParameters.ViewModels
{
	/// <summary>
	/// Partial class definition for <see cref="SystemParametersViewModel"/>.
	/// </summary>
	public partial class SystemParametersViewModel : BindableBase
	{
		private readonly ISystemParameters _systemParameterModel;
		public SystemParametersViewModel(ISystemParameters systemParameterModel)
		{
			_systemParameterModel = systemParameterModel;
      InitializeSensorParameterValues();
      InitializeVersions();
			_systemParameterModel.SensorParameters.PropertyChanged += SensorParameters_PropertyChanged;
      _systemParameterModel.VersionTestResult.PropertyChanged += VersionTestResult_PropertyChanged;
    }

		private void InitializeVersions()
		{
			CMCUBoot = _systemParameterModel.VersionTestResult.CMCUBootVersion;
			CMCU = _systemParameterModel.VersionTestResult.CMCUVersion;
			PMCUBoot = _systemParameterModel.VersionTestResult.PMCUBootVersion;
			PMCU = _systemParameterModel.VersionTestResult.PMCUVersion;
			RMCUBoot = _systemParameterModel.VersionTestResult.RMCUBootVersion;
			RMCU = _systemParameterModel.VersionTestResult.RMCUVersion;
			ICBBootVersion = _systemParameterModel.VersionTestResult.ICBBootVersion;
			ICBVersion = _systemParameterModel.VersionTestResult.ICBVersion;
			RCMCUBoot = _systemParameterModel.VersionTestResult.RCMCUBootVersion;
			RCMCUVersion = _systemParameterModel.VersionTestResult.RCMCUVersion;
			CPLD = _systemParameterModel.VersionTestResult.CPLDVersion;
			GUIVersion = _systemParameterModel.VersionTestResult.GUIVersion;
			DBVersion = _systemParameterModel.VersionTestResult.DBVersion;
		}

    private void InitializeSensorParameterValues()
    {
      Temperature = _systemParameterModel.SensorParameters.Temperature;
      FM1 = _systemParameterModel.SensorParameters.FM1;
      IBP = _systemParameterModel.SensorParameters.IBP;
      OBP = _systemParameterModel.SensorParameters.OBP;
      LC = _systemParameterModel.SensorParameters.LC;
      PWM1 = _systemParameterModel.SensorParameters.PWM1;
      PWM2 = _systemParameterModel.SensorParameters.PWM2;
      PT1 = _systemParameterModel.SensorParameters.PT1;
      PT2 = _systemParameterModel.SensorParameters.PT2;
      PT3 = _systemParameterModel.SensorParameters.PT3;
      PT4 = _systemParameterModel.SensorParameters.PT4;
      PT5 = _systemParameterModel.SensorParameters.PT5;
      TS1 = _systemParameterModel.SensorParameters.TS1;
      PGain = _systemParameterModel.SensorParameters.PGain;
      IGain = _systemParameterModel.SensorParameters.IGain;
      DGain = _systemParameterModel.SensorParameters.DGain;
      PatientPGain = _systemParameterModel.SensorParameters.PatientPGain;
      PatientIGain = _systemParameterModel.SensorParameters.PatientIGain;
      PatientDGain = _systemParameterModel.SensorParameters.PatientDGain;
      PIDOffset = _systemParameterModel.SensorParameters.PIDOffset;
      PatientPIDOffset = _systemParameterModel.SensorParameters.PatientPIDOffset;
    }
  }
}
