using Module.Accessories.Models;
using Module.Accessories.Properties;
using Prism.Mvvm;
using static Communication.CanBusMessageDefinition;

namespace Module.Accessories.ViewModels
{
	public class AccessoriesViewModel : BindableBase
	{
		public string DmsTitle { get; } = Resources.DmsChartTitle;
		public string EtsTitle { get; } = Resources.EtsChartTitle;
		public string TemperatureTitle { get; } = Resources.TemperatureChartTitle;
		public string FlowTitle { get; } = Resources.FlowChartTitle;
		public string TemperatureSeries => Constants.TemperatureSeriesName;
		public string FlowSeries => Constants.FlowSeriesName;
		public string DmsSeries => Constants.DmsSeriesName;
		public string EtsSeries => Constants.EtsSeriesName;

		public double TempMin => _model.TemperatureMin;
		public double TempMax => _model.TemperatureMax;
		public double Temperature => _model.Temperature;
		public double FM1 => _model.FM1;
		public double Ets => _model.Ets;

		private MessageStateId _SystemState;
		public MessageStateId SystemState
		{
			get => _SystemState;
			set => SetProperty(ref _SystemState, value);
		}

		private readonly AccessoriesModel _model;
		public AccessoriesViewModel(AccessoriesModel model)
		{
			_model = model;
			_model.PropertyChanged += _model_PropertyChanged;
		}

		private void _model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case nameof(_model.SystemState):
					SystemState = _model.SystemState;
					break;
			}
		}
	}
}
