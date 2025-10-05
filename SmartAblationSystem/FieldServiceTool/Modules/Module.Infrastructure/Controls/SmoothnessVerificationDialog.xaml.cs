using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms.DataVisualization.Charting;
using static Module.Infrastructure.Constants.Strings;

namespace Module.Infrastructure.Controls
{
	public partial class SmoothnessVerificationDialog
	{
		private SmoothnessVerificationDialogViewModel _viewModel;
		public string FlowSeries { get; }
		public SmoothnessVerificationDialog(SmoothnessVerificationDialogViewModel viewModel)
		{
			_viewModel = viewModel;
			InitializeComponent();
			Loaded += SmoothnessVerificationDialog_Loaded;
		}

		private void SmoothnessVerificationDialog_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			var dc_ = DataContext as SmoothnessVerificationDialogViewModel;
			var transitionSeries_ = SmoothFMChart.FindName(ChartSeriesName) as Series;

#if DEBUG
			var fm1Transition_ = new List<double> {
				715, 754, 797, 812, 832, 881, 932, 1020, 1137, 1236, 1345,
				1524, 1607, 1746, 1823, 1912, 2005, 2017, 2252, 2397, 2588,
				2733, 3038, 3519, 4011, 4120, 4551, 4780, 4988, 5161
			};

			var fm1Ablation_ = new List<double> { 
				5184, 5771, 5912, 6381, 6651, 7202, 7272, 7354, 7507,
				7730, 7753, 7859, 7882, 7624, 7612, 7648, 7788, 7800,
				7859, 7835, 7941, 7953, 7917 }.Take(10);

			fm1Transition_.AddRange(fm1Ablation_);
			dc_.Fm1Transition = fm1Transition_;
#endif
			if(dc_.Fm1Transition == null || dc_.Fm1Transition.Count == 0)
			{
				return;
			}
			SmoothFMChart.XAxisMinimum = 0;
			SmoothFMChart.XAxisMaximum = dc_.Fm1Transition.Count - 1;
			SmoothFMChart.YAxisMaximum = dc_.Fm1Transition.Min();
			SmoothFMChart.YAxisMaximum = dc_.Fm1Transition.Max();
			SmoothFMChart.WinChartSeries.BorderWidth = 3;
			var count_ = dc_.Fm1Transition.Count;
			foreach(var item_ in dc_.Fm1Transition)
			{
				transitionSeries_.Points.AddXY(transitionSeries_.Points.Count - 1, item_);
			}
		}

	}
}
