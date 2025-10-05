using Module.SystemParameters.Properties;

namespace Module.SystemParameters.ViewModels
{
	/// <summary>
	/// Properties for <see cref="SystemParametersViewModel"/>.
	/// </summary>
	public partial class SystemParametersViewModel
	{
		#region System Parameter Text

		public string Title { get; } = Resources.ModuleTitle;
		public string AccessoriesTitle { get; } = Resources.AccessoriesTitle;
		public string GraphsTitle { get; } = Resources.GraphsTitle;

		#endregion System Parameter Text

		#region Sensor Text

		public string SensorsTitle { get; } = Resources.SensorsTitle;
		public string TemperatureText { get; } = Resources.TemperatureText;
		public string FM1Text { get; } = Resources.FM1Text;
		public string IBPText { get; } = Resources.IBPText;
		public string OBPText { get; } = Resources.OBPText;
		public string LCText { get; } = Resources.LCText;
		public string PT1Text { get; } = Resources.PT1Text;
		public string PT2Text { get; } = Resources.PT2Text;
		public string PT3Text { get; } = Resources.PT3Text;
		public string PT4Text { get; } = Resources.PT4Text;
		public string PT5Text { get; } = Resources.PT5Text;
    public string TS1Text { get; } = Resources.TS1Text;
		public string PWM1Text { get; } = Resources.PWM1Text;
		public string PWM2Text { get; } = Resources.PWM2Text;
		public string I_P_GainText { get; } = Resources.I_P_GainText;
		public string I_I_GainText { get; } = Resources.I_I_GainText;
		public string I_D_GainText { get; } = Resources.I_D_GainText;
		public string I_OffsetText { get; } = Resources.I_OffsetText;
		public string B_P_GainText { get; } = Resources.B_P_GainText;
		public string B_I_GainText { get; } = Resources.B_I_GainText;
		public string B_D_GainText { get; } = Resources.B_D_GainText;
		public string B_OffsetText { get; } = Resources.B_OffsetText;

		#endregion Sensor Text

		#region Version Text

		public string BootloaderText { get; } = Resources.BootloaderTitle;
		public string ApplicationText { get; } = Resources.ApplicationTitle;
		public string FirmwareText { get; } = Resources.FirmwareTitle;
		public string VersionsTitle { get; } = Resources.VersionsTitle;
		public string CMCUBootText { get; } = Resources.CMCUBooTVersionText;
		public string CPLDText { get; } = Resources.CPLDVersionText;
		public string PMCUText { get; } = Resources.PMCUVersionText;
		public string PMCUBootText { get; } = Resources.PMCUBootVersionText;
		public string RMCUText { get; } = Resources.RMCUVersionText;
		public string RMCUBootText { get; } = Resources.RMCUBootVersionText;
		public string ICBText { get; } = Resources.ICBVersionText;
		public string ICBBootText { get; } = Resources.ICBBootVersionText;
		public string RCMCUBootText { get; } = Resources.RCMCUBootText;
		public string CMCUVersionText { get; } = Resources.CMCUVersionText;
		public string RCMCUVersionText { get; } = Resources.RCMCUVersionText;
		public string GUIVersionText { get; } = Resources.GUIVersionText;
		public string DBVersionText { get; } = Resources.DBVersionText;

		#endregion Version Text

		#region System Parameter Values

		private double _Temperature;
		public double Temperature
		{
			get => _Temperature;
			set => SetProperty(ref _Temperature, value);
		}

		private double _FM1;
		public double FM1
		{
			get => _FM1;
			set => SetProperty(ref _FM1, value);
		}

		private double _IBP;
		public double IBP
		{
			get => _IBP;
			set => SetProperty(ref _IBP, value);
		}

		private double _OBP;
		public double OBP
		{
			get => _OBP;
			set => SetProperty(ref _OBP, value);
		}

		private double _LC;
		public double LC
		{
			get => _LC;
			set => SetProperty(ref _LC, value);
		}

		private double _PT1;
		public double PT1
		{
			get => _PT1;
			set => SetProperty(ref _PT1, value);
		}

		private double _PT2;
		public double PT2
		{
			get => _PT2;
			set => SetProperty(ref _PT2, value);
		}

		private double _PT3;
		public double PT3
		{
			get => _PT3;
			set => SetProperty(ref _PT3, value);
		}

		private double _PT4;
		public double PT4
		{
			get => _PT4;
			set => SetProperty(ref _PT4, value);
		}

		private double _PT5;
		public double PT5
		{
			get => _PT5;
			set => SetProperty(ref _PT5, value);
		}

    private double _TS1;
    public double TS1
    {
      get => _TS1;
      set => SetProperty(ref _TS1, value);
    }

		private double _PWM1 = 0;
		public double PWM1
		{
			get => _PWM1;
			set => SetProperty(ref _PWM1, value);
		}

		private double _PWM2 = 0;
		public double PWM2
		{
			get => _PWM2;
			set => SetProperty(ref _PWM2, value);
		}

		private double _PGain = 10;
		public double PGain
		{
			get => _PGain;
			set => SetProperty(ref _PGain, value);
		}

		private double _IGain = 10;

		public double IGain
		{
			get => _IGain;
			set => SetProperty(ref _IGain, value);
		}

		private double _DGain = 1.12;
		public double DGain
		{
			get => _DGain;
			set => SetProperty(ref _DGain, value);
		}

		private double _PIDOffset = 10;
		public double PIDOffset
		{
			get => _PIDOffset;
			set => SetProperty(ref _PIDOffset, value);
		}

		private double _PatientPGain = 11;
		public double PatientPGain
		{
			get => _PatientPGain;
			set => SetProperty(ref _PatientPGain, value);
		}

		private double _PatientIGain = 13;
		public double PatientIGain
		{
			get => _PatientIGain;
			set => SetProperty(ref _PatientIGain, value);
		}

		private double _PatientDGain = 14.5;
		public double PatientDGain
		{
			get => _PatientDGain;
			set => SetProperty(ref _PatientDGain, value);
		}

		private double _PatientPIDOffset = 19;
		public double PatientPIDOffset
		{
			get => _PatientPIDOffset;
			set => SetProperty(ref _PatientPIDOffset, value);
		}

		#endregion System Parameter Values

		#region Version Values

		private string _CMCUBoot;
		public string CMCUBoot
		{
			get => _CMCUBoot;
			set => SetProperty(ref _CMCUBoot, value);
		}

		private string _PMCUBoot;
		public string PMCUBoot
		{
			get => _PMCUBoot;
			set => SetProperty(ref _PMCUBoot, value);
		}

		private string _RMCUBoot;
		public string RMCUBoot
		{
			get => _RMCUBoot;
			set => SetProperty(ref _RMCUBoot, value);
		}

		private string _ICBBootVersion;
		public string ICBBootVersion
		{
			get => _ICBBootVersion;
			set => SetProperty(ref _ICBBootVersion, value);
		}

		private string _RCMCUBoot;
		public string RCMCUBoot
		{
			get => _RCMCUBoot;
			set => SetProperty(ref _RCMCUBoot, value);
		}

		private string _CMCU;
		public string CMCU
		{
			get => _CMCU;
			set => SetProperty(ref _CMCU, value);
		}

		private string _CPLD;
		public string CPLD
		{
			get => _CPLD;
			set => SetProperty(ref _CPLD, value);
		}

		private string _PMCU;
		public string PMCU
		{
			get => _PMCU;
			set => SetProperty(ref _PMCU, value);
		}

		private string _RMCU;
		public string RMCU
		{
			get => _RMCU;
			set => SetProperty(ref _RMCU, value);
		}

		private string _ICBVersion;
		public string ICBVersion
		{
			get => _ICBVersion;
			set => SetProperty(ref _ICBVersion, value);
		}

		private string _RCMCUVersion;
		public string RCMCUVersion
		{
			get => _RCMCUVersion;
			set => SetProperty(ref _RCMCUVersion, value);
		}

		private string _dbVersion;
		public string DBVersion
		{
			get => _dbVersion;
			set => SetProperty(ref _dbVersion, value);
		}


		private string _guiVersion;
		public string GUIVersion
		{
			get => _guiVersion;
			set => SetProperty(ref _guiVersion, value);
		}

		#endregion Version Values
	}
}
