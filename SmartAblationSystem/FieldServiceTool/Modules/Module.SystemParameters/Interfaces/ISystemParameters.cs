using System.ComponentModel;
using Module.Infrastructure.TestResults.Interfaces;
using Module.SystemParameters.Interfaces;

namespace Module.SystemParameters.Models
{
	public interface ISystemParameters : INotifyPropertyChanged
	{
		ISensorParameters SensorParameters { get; set; }
		IVersionTestResult VersionTestResult { get; set; }
	}
}