using Module.Console.Interfaces;
using Prism.Mvvm;
using System;
using System.Timers;
using static Communication.CanBusMessageDefinition;
using System.ComponentModel;

namespace Module.Accessories.Models
{
	public class AccessoriesModel : BindableBase
	{
		private readonly IMachineModel _machineModel;
		private Random rnd = new Random();

		public AccessoriesModel(IMachineModel machineModel)
		{
			_machineModel = machineModel;
			_machineModel.PropertyChanged += _machineModel_PropertyChanged;
		}

		private void _machineModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch(e.PropertyName)
			{
				case nameof(_machineModel.SystemState):
					SystemState = _machineModel.SystemState;
					break;
			}
		}

		private MessageStateId _SystemState;
		public MessageStateId SystemState
		{
			get => _SystemState;
			set => SetProperty(ref _SystemState, value);
		}

		public double TemperatureMin = Constants.TemperatureMinValue;
		public double TemperatureMax = Constants.TemperatureMaxValue;

		private double _temperature;
		public double Temperature
		{
			get
			{
#if DEBUG
				_temperature = rnd.Next(-20, 20);
#else
				_temperature = _machineModel.TC1Reading;
#endif
				return _temperature;
			}
			set => SetProperty(ref _temperature, value);
		}

		private double _fm1;
		public double FM1
		{
			get
			{
#if DEBUG
				switch(SystemState)
				{
					case MessageStateId.CAN_ID_STATE_UNKNOWN:
						_fm1 = rnd.Next(30, 50);
						break;
					case MessageStateId.CAN_ID_STATE_IDLE:
						_fm1 = rnd.Next(30, 50);
						break;
					case MessageStateId.CAN_ID_STATE_READY:
						_fm1 = rnd.Next(30, 50);
						break;
					case MessageStateId.CAN_ID_STATE_INFLATION:
						_fm1 = rnd.Next(1000, 3000);
						break;
					case MessageStateId.CAN_ID_STATE_TRANSITION:
						_fm1 = rnd.Next(4000, 6000);
						break;
					case MessageStateId.CAN_ID_STATE_ABLATION:
						_fm1 = rnd.Next(7000, 8000);
						break;
					case MessageStateId.CAN_ID_STATE_THAWING:
						_fm1 = rnd.Next(7000, 8000);
						break;
					case MessageStateId.CAN_ID_STATE_EXCEPTION:
						_fm1 = 0;
						break;
				}
#else
				_fm1 = _machineModel.FM1Reading;
#endif
				return _fm1;
			}
			set => SetProperty(ref _fm1, value);
		}

		private double _ets;

		public double Ets
		{
			get
			{
#if DEBUG
				_ets = rnd.Next(20, 40);
#else
				_ets = _machineModel.EtsSesnor1;
#endif
				return _ets;
			}
			set => SetProperty(ref _ets, value);
		}
	}
}
