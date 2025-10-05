using System.ComponentModel;

namespace Module.SystemParameters.Interfaces
{
	public interface ISensorParameters : INotifyPropertyChanged
	{
		double Temperature { get; set; }
		double FM1 { get; set; }
		double IBP { get; set; }
		double OBP { get; set; }
		double LC { get; set; }
		double PT1 { get; set; }
		double PT2 { get; set; }
		double PT3 { get; set; }
		double PT4 { get; set; }
		double PT5 { get; set; }
    double TS1 { get; set; }
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
	}
}
