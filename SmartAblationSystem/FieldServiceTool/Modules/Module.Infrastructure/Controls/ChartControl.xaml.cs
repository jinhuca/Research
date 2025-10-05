using System.Drawing;
using System.Windows;
using System.Windows.Forms.DataVisualization.Charting;


namespace Module.Infrastructure.Controls
{
	public partial class ChartControl
	{
		public ChartControl()
		{
			InitializeComponent();
			WinChart.ChartAreas["myChartArea"].InnerPlotPosition.Y = 10;
			WinChart.ChartAreas["myChartArea"].InnerPlotPosition.Height = 75;
			WinChart.ChartAreas["myChartArea"].InnerPlotPosition.Width = 95;
			WinChart.ChartAreas["myChartArea"].AxisX.Enabled = AxisEnabled.True;
			WinChart.ChartAreas["myChartArea"].AxisY.Enabled = AxisEnabled.False;
			WinChart.ChartAreas["myChartArea"].AxisY2.Enabled = AxisEnabled.True;
		}

		public double YAxisMaximum
		{
			get => (double)GetValue(YAxisMaximumProperty);
			set => SetValue(YAxisMaximumProperty, value);
		}

		public static readonly DependencyProperty YAxisMaximumProperty = DependencyProperty.Register(
			nameof(YAxisMaximum),
			typeof(double),
			typeof(ChartControl),
			new PropertyMetadata(OnYAxisMaximumChanged));

		private static void OnYAxisMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var chartControl_ = d as ChartControl;
			chartControl_.OnYAxisMaximumChanged(e);
		}

		private void OnYAxisMaximumChanged(DependencyPropertyChangedEventArgs e)
		{
			WinChartSeries.Points.Clear();
			YAxis.Maximum = (double)e.NewValue;
			Y2Axis.Maximum = (double)e.NewValue;
		}

		public static readonly DependencyProperty XAxisMaximumProperty = DependencyProperty.Register(
			nameof(XAxisMaximum),
			typeof(double),
			typeof(ChartControl),
			new PropertyMetadata(OnXAxisMaximumChanged));

		public double XAxisMaximum
		{
			get => (double)GetValue(XAxisMaximumProperty);
			set => SetValue(XAxisMaximumProperty, value);
		}

		private static void OnXAxisMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var chartControl_ = d as ChartControl;
			chartControl_.OnXAxisMaximumChanged(e);
		}

		private void OnXAxisMaximumChanged(DependencyPropertyChangedEventArgs e)
		{
			WinChartSeries.Points.Clear();
			XAxis.Maximum = (double)e.NewValue;
		}

		public double XAxisInterval
		{
			get => (double)GetValue(XAxisIntervalProperty);
			set => SetValue(XAxisIntervalProperty, value);
		}

		public static readonly DependencyProperty XAxisIntervalProperty = DependencyProperty.Register(
			nameof(XAxisInterval),
			typeof(double),
			typeof(ChartControl),
			new PropertyMetadata(OnXAxisIntervalChanged));

		private static void OnXAxisIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var chartControl_ = d as ChartControl;
			chartControl_.OnXAxisIntervalChanged(e);
		}

		private void OnXAxisIntervalChanged(DependencyPropertyChangedEventArgs e)
		{
			XAxisStyle.Interval = (double)e.NewValue;
		}

		public bool XMinorGridEnabled
		{
			get => (bool)GetValue(XMinorGridEnabledProperty);
			set => SetValue(XMinorGridEnabledProperty, value);
		}

		public static readonly DependencyProperty XMinorGridEnabledProperty = DependencyProperty.Register(
			nameof(XMinorGridEnabled),
			typeof(bool),
			typeof(ChartControl),
			new PropertyMetadata(OnXMinorGridChanged));

		private static void OnXMinorGridChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var chartControl_ = d as ChartControl;
			chartControl_.OnXMinorChanged(e);
		}

		private void OnXMinorChanged(DependencyPropertyChangedEventArgs e)
		{
			XMinorGrid.Enabled = (bool)e.NewValue;
		}

		public double YAxisInterval
		{
			get => (double)GetValue(YAxisIntervalProperty);
			set => SetValue(YAxisIntervalProperty, value);
		}

		public static readonly DependencyProperty YAxisIntervalProperty = DependencyProperty.Register(
			nameof(YAxisInterval),
			typeof(double),
			typeof(ChartControl),
			new PropertyMetadata(OnYAxisIntervalChanged));

		private static void OnYAxisIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var chartControl_ = d as ChartControl;
			chartControl_.OnYAxisIntervalChanged(e);
		}

		private void OnYAxisIntervalChanged(DependencyPropertyChangedEventArgs e)
		{
			YAxisStyle.Interval = (double)e.NewValue;
			Y2AxisStyle.Interval = (double)e.NewValue;
		}

		public string SeriesName
		{
			get => (string)GetValue(SeriesNameProperty);
			set => SetValue(SeriesNameProperty, value);
		}

		public static readonly DependencyProperty SeriesNameProperty = DependencyProperty.Register(
			nameof(SeriesName),
			typeof(string),
			typeof(ChartControl),
			new PropertyMetadata(OnSeriesNamesChanged));

		private static void OnSeriesNamesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var chartControl_ = d as ChartControl;
			chartControl_.OnSeriesNameChanged(e);
		}

		private void OnSeriesNameChanged(DependencyPropertyChangedEventArgs e)
		{
			WinChartSeries.Name = (string)e.NewValue;
		}

		public Color DataPointsColor
		{
			get => (Color)GetValue(DataPointsColorProperty);
			set => SetValue(DataPointsColorProperty, value);
		}

		public static readonly DependencyProperty DataPointsColorProperty = DependencyProperty.Register(
			nameof(DataPointsColor),
			typeof(Color),
			typeof(ChartControl),
			new PropertyMetadata(OnDataPointsColorChanged));

		private static void OnDataPointsColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var chartControl_ = d as ChartControl;
			chartControl_.OnDataPointsColorChanged(e);
		}

		private void OnDataPointsColorChanged(DependencyPropertyChangedEventArgs e)
		{
			WinChartSeries.Color = (Color)e.NewValue;
		}

		public bool StartsFromZero
		{
			get => (bool)GetValue(StartsFromZeroProperty);
			set => SetValue(StartsFromZeroProperty, value);
		}

		public static readonly DependencyProperty StartsFromZeroProperty = DependencyProperty.Register(
			nameof(StartsFromZero),
			typeof(bool),
			typeof(ChartControl),
			new PropertyMetadata(OnStartFromZeroChanged));

		private static void OnStartFromZeroChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var chartControl_ = d as ChartControl;
			chartControl_.OnStartFromZeroChanged(e);
		}

		private void OnStartFromZeroChanged(DependencyPropertyChangedEventArgs e)
		{
			XAxis.IsStartedFromZero = (bool)e.NewValue;
		}

		public double XAxisMinimum
		{
			get => (double)GetValue(XAxisMinimumProperty);
			set => SetValue(XAxisMinimumProperty, value);
		}

		public static readonly DependencyProperty XAxisMinimumProperty = DependencyProperty.Register(
			nameof(XAxisMinimum),
			typeof(double),
			typeof(ChartControl),
			new PropertyMetadata(OnXAxisMinimumChanged));

		private static void OnXAxisMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var chartControl_ = d as ChartControl;
			chartControl_.OnXAxisMinimumChanged(e);
		}

		private void OnXAxisMinimumChanged(DependencyPropertyChangedEventArgs e)
		{
			XAxis.Minimum = (double)e.NewValue;
			XAxis.MinorGrid.Interval = 1;
		}

		public double YAxisMinimum
		{
			get => (double)GetValue(YAxisMinimumProperty);
			set => SetValue(YAxisMinimumProperty, value);
		}

		public static readonly DependencyProperty YAxisMinimumProperty = DependencyProperty.Register(
			nameof(YAxisMinimum),
			typeof(double),
			typeof(ChartControl),
			new PropertyMetadata(OnYAxisMinimumChanged));

		private static void OnYAxisMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var chartControl_ = d as ChartControl;
			chartControl_.OnYAxisMinimumChanged(e);
		}

		private void OnYAxisMinimumChanged(DependencyPropertyChangedEventArgs e)
		{
			YAxis.Minimum = (double)e.NewValue;
			Y2Axis.Minimum = (double)e.NewValue;
		}
	}
}
