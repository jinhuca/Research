
using System;
using System.ComponentModel;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Module.Console.Interfaces;
using Module.SystemParameters.Models;

namespace Module.CatheterTestTool.Models
{
    public class CatheterTestSensorParametersModel : SensorParametersModel
    {
        private readonly IMachineModel _machineModel;
        public CatheterTestSensorParametersModel(IMachineModel machineModel)
        {
            _machineModel = machineModel;
            
            Observable.FromEventPattern<PropertyChangedEventArgs>(_machineModel, "PropertyChanged")
                .ObserveOn(TaskPoolScheduler.Default)
                .Subscribe(e => HandleMachineModelPropertyChanged(e.EventArgs));
		}

		private void HandleMachineModelPropertyChanged(PropertyChangedEventArgs args)
		{
			switch (args.PropertyName)
			{
				case nameof(_machineModel.TC1Reading):
					Temperature = _machineModel.TC1Reading;
					break;
				case nameof(_machineModel.FM1Reading):
					FM1 = _machineModel.FM1Reading;
					break;
				case nameof(_machineModel.CP1Reading):
					IBP = _machineModel.CP1Reading;
					break;
				case nameof(_machineModel.CP2Reading):
					OBP = _machineModel.CP2Reading;
					break;
				case nameof(_machineModel.LC1Reading):
					LC = _machineModel.LC1Reading;
					break;
				case nameof(_machineModel.PT1Reading):
					PT1 = _machineModel.PT1Reading;
					break;
				case nameof(_machineModel.PT2Reading):
					PT2 = _machineModel.PT2Reading;
					break;
				case nameof(_machineModel.PT3Reading):
					PT3 = _machineModel.PT3Reading;
					break;
				case nameof(_machineModel.PT4Reading):
					PT4 = _machineModel.PT4Reading;
					break;
				case nameof(_machineModel.PT5Reading):
					PT5 = _machineModel.PT5Reading;
					break;
                case nameof(_machineModel.PIDDutyCycle):
                    PWM1 = _machineModel.PIDDutyCycle;
                    break;
				case nameof(_machineModel.PatientPIDDutyCycle):
					PWM2 = _machineModel.PatientPIDDutyCycle;
					break;
				case nameof(_machineModel.PGain):
					PGain = _machineModel.PGain;
					break;
				case nameof(_machineModel.IGain):
					IGain = _machineModel.IGain;
					break;
				case nameof(_machineModel.DGain):
					DGain = _machineModel.DGain;
					break;
				case nameof(_machineModel.PIDOffset):
					PIDOffset = _machineModel.PIDOffset;
					break;
				case nameof(_machineModel.PatientPGain):
					PatientPGain = _machineModel.PatientPGain;
					break;
				case nameof(_machineModel.PatientIGain):
					PatientIGain = _machineModel.PatientIGain;
					break;
				case nameof(_machineModel.PatientDGain):
					PatientDGain = _machineModel.PatientDGain;
					break;
				case nameof(_machineModel.PatientPIDOffset):
					PatientPIDOffset = _machineModel.PatientPIDOffset;
					break;
				default:
                    break;
			}
		}
	}
}
