using System.ComponentModel;

namespace Module.SystemParameters.Interfaces
{
	public interface IVersionParameters : INotifyPropertyChanged
	{
		int CMCUBootVersion { get; set; }
		int PMCUBootVersion { get; set; }
		int RMCUBootVersion { get; set; }
		int ICBBootVersion { get; set; }
		int RCMCUBootVersion { get; set; }
		int CMCUVersion { get; set; }
		int CPLDVersion { get; set; }
		int PMCUVersion { get; set; }
		int RMCUVersion { get; set; }
		int ICBVersion { get; set; }
		int RCMCUVersion { get; set; }
		string DBVersion { get; }
		string GUIVersion { get; }
	}
}
