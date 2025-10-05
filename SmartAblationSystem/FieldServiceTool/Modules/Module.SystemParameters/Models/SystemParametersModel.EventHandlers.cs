namespace Module.SystemParameters.Models
{
	/// <summary>
	/// Event Handlers for <see cref="SystemParametersModel"/>.
	/// </summary>
	public partial class SystemParametersModel
	{
		private void _machineModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case nameof(_machineModel.TC1Reading):
					SensorParameters.Temperature = _machineModel.TC1Reading;
					break;
				case nameof(_machineModel.FM1Reading):
					SensorParameters.FM1 = _machineModel.FM1Reading;
					break;
				case nameof(_machineModel.CP1Reading):
					SensorParameters.IBP = _machineModel.CP1Reading;
					break;
				case nameof(_machineModel.CP2Reading):
					SensorParameters.OBP = _machineModel.CP2Reading;
					break;
				case nameof(_machineModel.LC1Reading):
					SensorParameters.LC = _machineModel.LC1Reading;
					break;
				case nameof(_machineModel.PT1Reading):
					SensorParameters.PT1 = _machineModel.PT1Reading;
					break;
				case nameof(_machineModel.PT2Reading):
					SensorParameters.PT2 = _machineModel.PT2Reading;
					break;
				case nameof(_machineModel.PT3Reading):
					SensorParameters.PT3 = _machineModel.PT3Reading;
					break;
				case nameof(_machineModel.PT4Reading):
					SensorParameters.PT4 = _machineModel.PT4Reading;
					break;
				case nameof(_machineModel.PT5Reading):
					SensorParameters.PT5 = _machineModel.PT5Reading;
					break;
        case nameof(_machineModel.TS1Reading):
          SensorParameters.TS1 = _machineModel.TS1Reading;
          break;
				case nameof(_machineModel.PIDDutyCycle):
					SensorParameters.PWM1 = _machineModel.PIDDutyCycle;
					break;
				case nameof(_machineModel.PatientPIDDutyCycle):
					SensorParameters.PWM2 = _machineModel.PatientPIDDutyCycle;
					break;
				case nameof(_machineModel.PGain):
					SensorParameters.PGain = _machineModel.PGain;
					break;
				case nameof(_machineModel.IGain):
					SensorParameters.IGain = _machineModel.IGain;
					break;
				case nameof(_machineModel.DGain):
					SensorParameters.DGain = _machineModel.DGain;
					break;
				case nameof(_machineModel.PIDOffset):
					SensorParameters.PIDOffset = _machineModel.PIDOffset;
					break;
				case nameof(_machineModel.PatientPGain):
					SensorParameters.PatientPGain = _machineModel.PatientPGain;
					break;
				case nameof(_machineModel.PatientIGain):
					SensorParameters.PatientIGain = _machineModel.PatientIGain;
					break;
				case nameof(_machineModel.PatientDGain):
					SensorParameters.PatientDGain = _machineModel.PatientDGain;
					break;
				case nameof(_machineModel.PatientPIDOffset):
					SensorParameters.PatientPIDOffset = _machineModel.PatientPIDOffset;
					break;
			}
		}
	}
}
