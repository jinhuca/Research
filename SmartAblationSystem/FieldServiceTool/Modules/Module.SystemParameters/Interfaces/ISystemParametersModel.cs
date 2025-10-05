using System.ComponentModel;

namespace Module.SystemParameters.Models
{
	public interface ISystemParametersModel
	{
		/// <summary>
		/// Gets or sets the central microController bootLoader firmware version.
		/// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		int CentralMicroControllerBootLoaderFirmwareVersion { get; set; }

		/// <summary>
		/// Gets or sets the CPLD bootLoader firmware version.
		/// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		int CPLDVersion { get; set; }

		/// <summary>
		/// Gets or sets the Patient Micro Controller Firmware Version value.
		/// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		int PMCUVersion { get; set; }

		/// <summary>
		/// Gets or sets the patient bootLoader firmware version.
		/// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		int PMCUBootVersion { get; set; }

		/// <summary>
		/// This property gets/sets the RemoteControlFirmwareDBVersion value.
		/// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		int RMCUVersion { get; set; }

		int RMCUBootVersion { get; set; }

		/// <summary>
		/// Gets or sets the ICBVersion Firmware value.
		/// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		int ICBVersion { get; set; }

		string DBVersion { get; }
		string GUIVersion { get; }
		double Temperature { get; set; }
		double FM1 { get; set; }

		double IBP // IBP
		{
			get;
			set;
		}

		double OBP { get; set; }
		double LC { get; set; }
		double PT1 { get; set; }
		double PT2 { get; set; }
		double PT3 { get; set; }
		double PT4 { get; set; }
		double PT5 { get; set; }
		double PWM1 { get; set; }
		double PWM2 { get; set; }
		double PGain { get; set; }
		double IGain { get; set; }
		double DGain { get; set; }
		double PIDOffset { get; set; }
		double PatientPGain { get; set; }
		double PatientIGain { get; set; }
		double PatientDGain { get; set; }
		double PatientPIDOffset { get; set; }
		event PropertyChangedEventHandler PropertyChanged;
	}
}