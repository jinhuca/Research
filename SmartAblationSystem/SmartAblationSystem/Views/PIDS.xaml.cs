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
    /// Interaction logic for PIDS.xaml
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public partial class PIDS : UserControl
    {
        private Color gridLinesColor = Color.Gray;
        private PIDSViewModel pIDSViewModel;

        private WindowsFormsHost PT2ChartHost;
        private WindowsFormsHost PT3IBPChartHost;
        private WindowsFormsHost FM1ChartHost;
        private WindowsFormsHost PWMChartHost;

        private Series SeriePT2;
        private Series SeriePT3;
        private Series SerieFM1;
        private Series SerieIBP;
        private Series SeriePWMInjection;
        private Series SeriePWMBallon;

        private Color CHART_GRID_LINES_COLOR = Color.DimGray;
        private ChartDashStyle CHART_GRID_DASH_STYLE = ChartDashStyle.Solid;
        private Color SERIES_COLOR = ColorTranslator.FromHtml("#00AFEF");

        private MicroTimer PIDTimer = new MicroTimer();

        /// <summary>
        /// Initializes PIDS components.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public PIDS()
        {
            InitializeComponent();
            this.pIDSViewModel = this.DataContext as PIDSViewModel;

            PIDTimer.Interval = 500000; // we are using 500ms inteval
            PIDTimer.MicroTimerElapsed += new MicroTimer.MicroTimerElapsedEventHandler(PIDTimer_tick);
            PIDTimer.Stop();

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

            pIDSViewModel.StopAblationTimer();

            pIDSViewModel.IsTuningPid = false;

            pIDSViewModel.IsLoggingActivated = false;

            pIDSViewModel.EnableOrDisablePIDManualMode = false;
            CommonViewModel.Current.Console.EnableOrDisablePIDManualMode = false;

            pIDSViewModel.EnableOrDisablePressureFlowMode = false;
            CommonViewModel.Current.Console.EnableOrDisablePressureFlowMode = false;

            //The CPLD state Machine  need two stops to leave Ablation state
            CommonViewModel.Current.Console.Stop();
            CommonViewModel.Current.Console.InjectionDisable();
            Thread.Sleep(10);
            CommonViewModel.Current.Console.Stop();

            CommonViewModel.Current.ChangeBalloonTypeFSM.DASBalloonEnabled = false;

            
            PIDTimer.Stop();
        }

        /// <summary>
        /// Loads and initialize charts, their properties and series.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void LoadCharts()
        {
            try
            {
                if (PT2ChartHost == null)
                {
                    PT2ChartHost = new WindowsFormsHost() { VerticalAlignment = VerticalAlignment.Stretch, HorizontalAlignment = HorizontalAlignment.Stretch };
                    var chartPT2 = new Chart() { Name = "PT2Chart", BackColor = System.Drawing.Color.Transparent, Height = 230 };

                    Thickness margin = PT2ChartHost.Margin;
                    margin.Left = 0;
                    margin.Top = -10;
                    margin.Right = 0;
                    margin.Bottom = 0;
                    PT2ChartHost.Margin = margin;

                    PT2ChartHost.Child = chartPT2;
                    ChartPT2.Children.Add(PT2ChartHost);

                    InitializePT2Graphic();
                }

                if (FM1ChartHost == null)
                {
                    FM1ChartHost = new WindowsFormsHost() { VerticalAlignment = VerticalAlignment.Stretch, HorizontalAlignment = HorizontalAlignment.Stretch };
                    var chartFM1 = new Chart() { Name = "FM1Chart", BackColor = System.Drawing.Color.Transparent, Height = 230 };

                    Thickness margin = FM1ChartHost.Margin;
                    margin.Left = 0;
                    margin.Top = -10;
                    margin.Right = 0;
                    margin.Bottom = 0;
                    FM1ChartHost.Margin = margin;

                    FM1ChartHost.Child = chartFM1;
                    ChartFM1.Children.Add(FM1ChartHost);

                    InitializeFM1Graphic();
                }

                if (PT3IBPChartHost == null)
                {
                    PT3IBPChartHost = new WindowsFormsHost() { VerticalAlignment = VerticalAlignment.Stretch, HorizontalAlignment = HorizontalAlignment.Stretch };
                    var chartPT3IBP = new Chart() { Name = "PT3Chart", BackColor = System.Drawing.Color.Transparent, Height = 230 };

                    Thickness margin = PT3IBPChartHost.Margin;
                    margin.Left = 0;
                    margin.Top = -10;
                    margin.Right = 0;
                    margin.Bottom = 0;
                    PT3IBPChartHost.Margin = margin;

                    PT3IBPChartHost.Child = chartPT3IBP;
                    ChartPT3.Children.Add(PT3IBPChartHost);

                    InitializePT3IBPGraphic();
                }

                if (PWMChartHost == null)
                {
                    PWMChartHost = new WindowsFormsHost() { VerticalAlignment = VerticalAlignment.Stretch, HorizontalAlignment = HorizontalAlignment.Stretch };
                    var chartPWM = new Chart() { Name = "PWMChart", BackColor = System.Drawing.Color.Transparent, Height = 230 };

                    Thickness margin = PWMChartHost.Margin;
                    margin.Left = 0;
                    margin.Top = -10;
                    margin.Right = 0;
                    margin.Bottom = 0;
                    PWMChartHost.Margin = margin;

                    PWMChartHost.Child = chartPWM;
                    ChartPWM.Children.Add(PWMChartHost);

                    InitializePWMGraphic();
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        /// <summary>
        /// Initializes PT2 chart, its properties and serie.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void InitializePT2Graphic()
        {
            try
            {
                Chart chart = (Chart)PT2ChartHost.Child;

                InitializeCommonGraphic(chart);
                InitializeCommonSerie(chart.Series, "PT2");
                SeriePT2 = chart.Series[0];

                chart.ChartAreas[0].AxisX.Minimum = 0;
                chart.ChartAreas[0].AxisX.Maximum = 20;
                chart.ChartAreas[0].AxisX.MajorGrid.Interval = 5;

                chart.ChartAreas[0].AxisY.Minimum = 0;
                chart.ChartAreas[0].AxisY.Maximum = 1000;
                chart.ChartAreas[0].AxisY.MajorGrid.Interval = 100;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }

        /// <summary>
        /// Initializes PT# and IBP charts, their properties and series.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void InitializePT3IBPGraphic()
        {
            try
            {
                Chart chart = (Chart)PT3IBPChartHost.Child;

                InitializeCommonGraphic(chart);
                InitializeCommonSerie(chart.Series, "PT3");
                InitializeCommonSerie(chart.Series, "IBP");
                SeriePT3 = chart.Series[0];
                SerieIBP = chart.Series[1];
                SerieIBP.Color = Color.Yellow;

                chart.ChartAreas[0].AxisX.Minimum = 0;
                chart.ChartAreas[0].AxisX.Maximum = 20;
                chart.ChartAreas[0].AxisX.MajorGrid.Interval = 5;

                chart.ChartAreas[0].AxisY.Minimum = 0;
                chart.ChartAreas[0].AxisY.Maximum = 30;
                chart.ChartAreas[0].AxisY.MajorGrid.Interval = 5;
            }
            catch (Exception ex)
            {
                // TODO
                ex.ToString();
                throw;
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
                InitializeCommonSerie(chart.Series, "FM1");
                SerieFM1 = chart.Series[0];

                chart.ChartAreas[0].AxisX.Minimum = 0;
                chart.ChartAreas[0].AxisX.Maximum = 20;
                chart.ChartAreas[0].AxisX.MajorGrid.Interval = 5;

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
        /// Initializes PWM charts, its properties and serie.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void InitializePWMGraphic()
        {
            try
            {
                Chart chart = (Chart)PWMChartHost.Child;

                InitializeCommonGraphic(chart);
                InitializeCommonSerie(chart.Series, "PWMInj");
                InitializeCommonSerie(chart.Series, "PWMBal");
                SeriePWMInjection = chart.Series[0];
                SeriePWMBallon = chart.Series[1];
                SeriePWMBallon.Color = Color.Yellow;

                chart.ChartAreas[0].AxisX.Minimum = 0;
                chart.ChartAreas[0].AxisX.Maximum = 20;
                chart.ChartAreas[0].AxisX.MajorGrid.Interval = 5;

                chart.ChartAreas[0].AxisY.Minimum = 0;
                chart.ChartAreas[0].AxisY.Maximum = 100;
                chart.ChartAreas[0].AxisY.MajorGrid.Interval = 10;
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
        private void InitializeCommonSerie(SeriesCollection series, string name)
        {
            Series serie = null;

            try
            {
                if (series != null)
                {
                    serie = series.Add(name);
                    serie.ChartType = SeriesChartType.FastLine;
                    serie.BorderWidth = 2;
                    serie.IsVisibleInLegend = false;
                    serie.Color = SERIES_COLOR;

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
        private void PIDTimer_tick(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                CommonViewModel commonViewModel = CommonViewModel.Current;

                if (SeriePT2 == null || SerieFM1 == null || SeriePT3 == null || SerieIBP == null ||
                    SeriePWMInjection == null || SeriePWMBallon == null)
                {
                    LoadCharts();
                }

                if (SeriePT2 != null)
                {
                    SeriePT2.Points.AddY(commonViewModel.PT2Reading);

                    if (SeriePT2.Points.Count >= 20)
                    {
                        SeriePT2.Points.RemoveAt(0);
                    }
                }

                if (SerieFM1 != null)
                {
                    SerieFM1.Points.AddY(commonViewModel.FM1Reading);

                    if (SerieFM1.Points.Count >= 20)
                    {
                        SerieFM1.Points.RemoveAt(0);
                    }
                }

                if (SeriePT3 != null)
                {
                    SeriePT3.Points.AddY(commonViewModel.PT3Reading);

                    if (SeriePT3.Points.Count >= 20)
                    {
                        SeriePT3.Points.RemoveAt(0);
                    }
                }

                if (SerieIBP != null)
                {
                    SerieIBP.Points.AddY(commonViewModel.CP1Reading);

                    if (SerieIBP.Points.Count >= 20)
                    {
                        SerieIBP.Points.RemoveAt(0);
                    }
                }

                if (SeriePWMInjection != null)
                {
                    SeriePWMInjection.Points.AddY(commonViewModel.PIDDutyCycle);

                    if (SeriePWMInjection.Points.Count >= 20)
                    {
                        SeriePWMInjection.Points.RemoveAt(0);
                    }
                }

                if (SeriePWMBallon != null)
                {
                    SeriePWMBallon.Points.AddY(commonViewModel.PatientPIDDutyCycle);

                    if (SeriePWMBallon.Points.Count >= 20)
                    {
                        SeriePWMBallon.Points.RemoveAt(0);
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
            pIDSViewModel.IsTuningPid = true;
            pIDSViewModel.SystemState = CommonViewModel.Current.SystemState;
            pIDSViewModel.DASBalloonEnabled = false;
            pIDSViewModel.LockTheFootSwitch = false;
            CommonViewModel.Current.DeflateAfterThaw = false;
            
            pIDSViewModel.RefreshInflationSpeedMode();

            PIDTimer.Start();
        }
    }
}