using MicroLibrary;
using SmartAblationSystem.ViewModels;
using System;
using System.Drawing;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Forms.Integration;

namespace SmartAblationSystem.Views
{
    /// <summary>
    /// Interaction logic for FlowCurveProgramming.xaml
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public partial class FlowCurveProgramming : UserControl
    {
        private FlowCurveProgrammingViewModel flowCurveProgrammingViewModel;

        private Color gridLinesColor = Color.Gray;

        private WindowsFormsHost FM1ChartHost;

        private Series SerieExpected;
        private Series SerieReal;
        private Series SerieLimit;

        private Color CHART_GRID_LINES_COLOR = Color.DimGray;
        private ChartDashStyle CHART_GRID_DASH_STYLE = ChartDashStyle.Solid;
        private Color SERIES_EXPECTED_COLOR = ColorTranslator.FromHtml("#ffff00");      //yellow
        private Color SERIES_LIMIT_COLOR = ColorTranslator.FromHtml("#ff0000");         //Red
        private Color SERIES_REAL_COLOR = ColorTranslator.FromHtml("#00a300");          //green

        private MicroTimer Timer = new MicroTimer();

        /// <summary>
        /// Initializes Flow Curve Programming components.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public FlowCurveProgramming()
        {
            InitializeComponent();
            this.flowCurveProgrammingViewModel = this.DataContext as FlowCurveProgrammingViewModel;

            Timer.Interval = 100000; // we are using 100ms inteval
            Timer.MicroTimerElapsed += new MicroTimer.MicroTimerElapsedEventHandler(Timer_tick);
            Timer.Stop();

#if Simulator
            BtnStateSimulator.Visibility = Visibility.Visible;
#endif
        }

        /// <summary>
        /// Occurs when the UserControl_Unloaded event is raised.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">The component that raised the event.</param>
        /// <param name="e">Contains state information and event data associated with a routed event.</param>
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            //Always disable maintenance mode when quitting except for these screens :
            // Mechanical Panel, Electrical Panel, Flow Curve Programming and PID.
            if (!CommonViewModel.Current.IsMaintenanceModeScreenSelected)
            {
                CommonViewModel.Current.Console.GUIInMaintenanceMode = false;
                CommonViewModel.Current.IsMaintenanceModeScreenSelected = false;
            }

            //pIDSViewModel.IsLoggingActivated = false;

            //pIDSViewModel.EnableOrDisablePIDManualMode = false;
            //CommonViewModel.Current.Console.EnableOrDisablePIDManualMode = false;

            //pIDSViewModel.EnableOrDisablePressureFlowMode = false;
            CommonViewModel.Current.Console.EnableOrDisablePressureFlowMode = false;

            ////The CPLD state Machine  need two stops to leave Ablation state
            //CommonViewModel.Current.Console.Stop();
            //CommonViewModel.Current.Console.InjectionDisable();
            //Thread.Sleep(10);
            //CommonViewModel.Current.Console.Stop();

            flowCurveProgrammingViewModel.IsProgrammingFlow = false;
            flowCurveProgrammingViewModel.StopAblationTimer();

            Timer.Stop();
        }

        /// <summary>
        /// Loads and initialize charts, their properties and series.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void LoadCharts()
        {
            try
            {
                if (FM1ChartHost == null)
                {
                    FM1ChartHost = new WindowsFormsHost() { VerticalAlignment = VerticalAlignment.Stretch, HorizontalAlignment = HorizontalAlignment.Stretch };
                    var chartFM1 = new Chart() { Name = "FM1Chart", BackColor = System.Drawing.Color.Transparent, Height = 500 };

                    Thickness margin = FM1ChartHost.Margin;
                    margin.Left = -80;
                    margin.Top = -10;
                    margin.Right = 0;
                    margin.Bottom = 0;
                    FM1ChartHost.Margin = margin;

                    FM1ChartHost.Child = chartFM1;
                    ChartFM1.Children.Add(FM1ChartHost);

                    InitializeFM1Graphic();
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        /// <summary>
        /// Initializes FM1 charts, its properties and serie.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void InitializeFM1Graphic()
        {
            try
            {
                Chart chart = (Chart)FM1ChartHost.Child;

                InitializeCommonGraphic(chart);

                InitializeCommonSerie(chart.Series, "Limit", SERIES_LIMIT_COLOR);
                InitializeCommonSerie(chart.Series, "Expected", SERIES_EXPECTED_COLOR);
                InitializeCommonSerie(chart.Series, "Real", SERIES_REAL_COLOR);

                SerieLimit = chart.Series[0];
                SerieExpected = chart.Series[1];
                SerieReal = chart.Series[2];

                chart.ChartAreas[0].AxisX.Minimum = 0;
                chart.ChartAreas[0].AxisX.Maximum = 100;
                chart.ChartAreas[0].AxisX.MajorGrid.Interval = 10;

                chart.ChartAreas[0].AxisY.Minimum = 0;
                chart.ChartAreas[0].AxisY.Maximum = 10000;
                chart.ChartAreas[0].AxisY.MajorGrid.Interval = 1000;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }
        
        /// <summary>
        /// Initializes properties that must be the same among several charts.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="chart">A Chart representing a chart to initialize.</param>
        private void InitializeCommonGraphic(Chart chart)
        {
            try
            {
                if (chart != null)
                {
                    chart.BackColor = System.Drawing.Color.Transparent;
                    chart.ChartAreas.Add("ChartArea");
                    chart.ChartAreas[0].BackColor = System.Drawing.Color.Transparent;
                    chart.ChartAreas[0].AxisX.IsStartedFromZero = true;
                    chart.ChartAreas[0].AxisX.LabelStyle.ForeColor = System.Drawing.Color.White;
                    chart.ChartAreas[0].AxisX.LineDashStyle = ChartDashStyle.NotSet;
                    chart.ChartAreas[0].AxisX.LabelStyle.Enabled = true;  //show the axis labels
                    chart.ChartAreas[0].AxisX.MinorGrid.Enabled = false;
                    chart.ChartAreas[0].AxisX.MajorGrid.Enabled = true;
                    chart.ChartAreas[0].AxisX.MajorGrid.LineColor = CHART_GRID_LINES_COLOR;
                    chart.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = CHART_GRID_DASH_STYLE;
                    chart.ChartAreas[0].AxisX.MajorTickMark.Enabled = true;  //show marks on the axis labels

                    chart.ChartAreas[0].AxisY.LineDashStyle = ChartDashStyle.NotSet;
                    chart.ChartAreas[0].AxisY.LabelStyle.Enabled = true;  //show the axis labels
                    chart.ChartAreas[0].AxisY.LabelStyle.ForeColor = System.Drawing.Color.White;
                    chart.ChartAreas[0].AxisY.MinorGrid.Enabled = false;
                    chart.ChartAreas[0].AxisY.MajorGrid.Enabled = true;
                    chart.ChartAreas[0].AxisY.MajorGrid.LineColor = CHART_GRID_LINES_COLOR;
                    chart.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = CHART_GRID_DASH_STYLE;
                    chart.ChartAreas[0].AxisY.MajorTickMark.Enabled = true;  //show marks on the axis labels

                    // Set Antialiasing mode
                    // this can be set lower if there are any performance issues!
                    chart.AntiAliasing = AntiAliasingStyles.None;
                    chart.TextAntiAliasingQuality = TextAntiAliasingQuality.Normal;
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }

        /// <summary>
        /// Initializes series that must be the same among several charts.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="series">A SeriesCollection representing a collection of series to be initialized.</param>
        /// <param name="name">A string representing a serie name.</param>
        private void InitializeCommonSerie(SeriesCollection series, string name, Color color)
        {
            Series serie = null;

            try
            {
                if (series != null)
                {
                    serie = series.Add(name);
                    serie.ChartType = SeriesChartType.FastLine;
                    serie.BorderWidth = 5;
                    serie.IsVisibleInLegend = false;
                    serie.Color = color;

                    //Display chart even when no value has been received
                    serie.Points.Add(0, 0);
                    serie.Points[0].IsEmpty = true;
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }

        /// <summary>
        /// Occurs when the PIDTimer_tick event is raised.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">The component that raised the event.</param>
        /// <param name="e">Contains state information and event data associated with a routed event.</param>
        private void Timer_tick(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                CommonViewModel commonViewModel = CommonViewModel.Current;

                if (SerieExpected == null || SerieLimit == null || SerieReal == null)
                {
                    LoadCharts();
                }

                if (SerieExpected != null)
                {
                    SerieExpected.Points.AddY(((FlowCurveProgrammingViewModel)this.DataContext).ExpectedFlow);

                    if (SerieExpected.Points.Count >= 100)
                    {
                        SerieExpected.Points.RemoveAt(0);
                    }
                }

                if (SerieLimit != null)
                {
                    SerieLimit.Points.AddY(commonViewModel.ThresholdFM1High);

                    if (SerieLimit.Points.Count >= 100)
                    {
                        SerieLimit.Points.RemoveAt(0);
                    }
                }

                if (SerieReal != null)
                {
                    SerieReal.Points.AddY(commonViewModel.FM1Reading);

                    if (SerieReal.Points.Count >= 100)
                    {
                        SerieReal.Points.RemoveAt(0);
                    }
                }
            }
                );
        }

        /// <summary>
        /// Occurs when the UserControl_Loaded event is raised.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">The component that raised the event.</param>
        /// <param name="e">Contains state information and event data associated with a routed event.</param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            flowCurveProgrammingViewModel.SystemState = CommonViewModel.Current.SystemState;
            flowCurveProgrammingViewModel.LockTheFootSwitch = false;
            flowCurveProgrammingViewModel.IsProgrammingFlow = true;
            Timer.Start();
        }
    }
}