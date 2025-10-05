using System;
using System.Reactive.Linq;
using Module.Console.Interfaces;
using Module.Infrastructure.TestResults.Interfaces;
using Module.SystemParameters.Interfaces;
using Prism.Mvvm;

namespace Module.SystemParameters.Models
{
	/// <summary>
	/// Partial definitions for <see cref="SystemParametersModel"/>.
	/// </summary>
	public partial class SystemParametersModel : BindableBase, ISystemParameters
	{
		public SystemParametersModel(
			IMachineModel machineModel,
			IVersionTestResult versionTestResult,
			ISensorParameters sensorParameters, 
			DataAccessLayer.DataAccess dataAccess)
		{
			_machineModel = machineModel;
			_dataAccess = dataAccess;
			_machineModel.PropertyChanged += _machineModel_PropertyChanged;

			VersionTestResult = versionTestResult;
			SensorParameters = sensorParameters;
			UpdateVersionResult();
			GetConsoleSerialNum();

      Observable.Interval(TimeSpan.FromSeconds(5))
        .Subscribe(_ => UpdateVersionResult());

      InitializeSensorParameterValues();
    }

		private readonly IMachineModel _machineModel;
		private readonly DataAccessLayer.DataAccess _dataAccess;
	}
}
