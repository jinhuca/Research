using Module.Accessories.ViewModels;
using System;
using System.Timers;
using System.Windows;
using System.Windows.Forms.DataVisualization.Charting;
using static Module.Accessories.Models.Constants;
using static Module.Infrastructure.Constants.Strings;

namespace Module.Accessories.Views
{
	public partial class AccessoriesView
	{
		private readonly AccessoriesViewModel _viewModel;
		public string TemperatureSeries { get; }
		public string FlowSeries { get; }
		public string DmsSeries { get; }
		public string EtsSeries { get; }
		public AccessoriesView(Timer timer, AccessoriesViewModel viewModel)
		{
			InitializeComponent();
			_viewModel = viewModel;
			TemperatureSeries = _viewModel.TemperatureSeries;
			FlowSeries = _viewModel.FlowSeries;
			DmsSeries = _viewModel.DmsSeries;
			EtsSeries = _viewModel.EtsSeries;

			var timer_ = timer;
			timer_.AutoReset = true;
			timer_.Enabled = true;
			timer_.Interval = 1000;
			timer_.Elapsed += TimerOnElapsed;
		}

		private void TimerOnElapsed(object sender, ElapsedEventArgs e)
		{
			Application.Current.Dispatcher.BeginInvoke((Action)(() =>
			{
				UpdateTemperatures();
				UpdateFlowMeters();
			}));

			void UpdateTemperatures()
			{
				Series myser = TemperatureChart.FindName(ChartSeriesName) as Series;
				if(myser.Points.Count >= TemperatureDisplayCount)
				{
					myser.Points.RemoveAt(0);
					foreach(var pt in myser.Points)
					{
						pt.XValue -= 1;
					}
				}
				myser.Points.AddXY(myser.Points.Count - 1, _viewModel.Temperature);
			}
			void UpdateFlowMeters()
			{
				Series myser = FlowChart.FindName(ChartSeriesName) as Series;
				if(myser.Points.Count > FlowMeterDisplayCount)
				{
					myser.Points.RemoveAt(0);
					foreach(var pt in myser.Points)
					{
						pt.XValue -= 1;
					}
				}
				myser.Points.AddXY(myser.Points.Count - 1, _viewModel.FM1);
			}
		}
	}
}
