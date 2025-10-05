using Module.SystemParameters.Interfaces;
using Prism.Mvvm;

namespace Module.SystemParameters.Models
{
	public class SensorParametersModel : BindableBase, ISensorParameters
	{
		private double _temperature;
		public double Temperature
		{
			get => _temperature;
			set => SetProperty(ref _temperature, value);
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

		private double _PWM1;
		public double PWM1
		{
			get => _PWM1;
			set => SetProperty(ref _PWM1, value);
		}

		private double _PWM2;
		public double PWM2
		{
			get => _PWM2;
			set => SetProperty(ref _PWM2, value);
		}

		private double _PGain;
		public double PGain
		{
			get => _PGain;
			set => SetProperty(ref _PGain, value);
		}

		private double _IGain;
		public double IGain
		{
			get => _IGain;
			set => SetProperty(ref _IGain, value);
		}

		private double _DGain;
		public double DGain
		{
			get => _DGain;
			set => SetProperty(ref _DGain, value);
		}

		private double _PIDOffset;
		public double PIDOffset
		{
			get => _PIDOffset;
			set => SetProperty(ref _PIDOffset, value);
		}

		private double _PatientPGain;
		public double PatientPGain
		{
			get => _PatientPGain;
			set => SetProperty(ref _PatientPGain, value);
		}

		private double _PatientIGain;
		public double PatientIGain
		{
			get => _PatientIGain;
			set => SetProperty(ref _PatientIGain, value);
		}

		private double _PatientDGain;
		public double PatientDGain
		{
			get => _PatientDGain;
			set => SetProperty(ref _PatientDGain, value);
		}

		private double _PatientPIDOffset;
		public double PatientPIDOffset
		{
			get => _PatientPIDOffset;
			set => SetProperty(ref _PatientPIDOffset, value);
		}
	}
}
