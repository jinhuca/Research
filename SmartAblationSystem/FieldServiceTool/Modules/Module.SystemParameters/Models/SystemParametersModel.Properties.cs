using Module.Infrastructure.TestResults.Interfaces;
using Module.SystemParameters.Interfaces;

namespace Module.SystemParameters.Models
{
	/// <summary>
	/// Properties for <see cref="SystemParametersModel"/>.
	/// </summary>
	public partial class SystemParametersModel
	{
		private ISensorParameters _SensorParameters;
		public ISensorParameters SensorParameters
		{
			get => _SensorParameters;
			set => SetProperty(ref _SensorParameters, value);
		}

		private IVersionTestResult _versionTestResult;
		public IVersionTestResult VersionTestResult
		{
			get => _versionTestResult;
			set => SetProperty(ref _versionTestResult, value);
		}
	}
}
