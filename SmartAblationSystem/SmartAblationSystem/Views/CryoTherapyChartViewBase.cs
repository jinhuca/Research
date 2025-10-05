
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Forms.Integration;
using System.Windows.Threading;
using Communication;
using Console;
using FileSerializer;
using SmartAblationSystem.Helpers;
using SmartAblationSystem.ViewModels;

using MouseEventArgs = System.Windows.Forms.MouseEventArgs;
using static Communication.CanBusMessageDefinition;
using static LogSystem.LogService;
using SmartAblationSystem.Models;
using System.Windows.Forms;
using Chart = System.Windows.Forms.DataVisualization.Charting.Chart;
using UserControl = System.Windows.Controls.UserControl;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using System.Reactive.Concurrency;
using System.Windows.Input;

namespace SmartAblationSystem.Views
{
  using Shared;

  public abstract class CryoTherapyChartViewBase : UserControl
  {
    protected readonly Color GRID_LINES_COLOR = Color.Gray;
    protected readonly Color ECG_GRID_LINES_COLOR = Color.DimGray;
    protected readonly Color SERIES_COLOR = ColorTranslator.FromHtml("#00AFEF");
    protected readonly Color DIAPHRAGM_DOT_SERIES_COLOR = Color.White;
    protected readonly Color SERIES_COLOR_THRESHOLD_EXCEEDED = Color.Red;
    protected readonly Color SERIES_COLOR_ISOLATED_VEIN = ColorTranslator.FromHtml("#9ac466");
    protected readonly Color SERIES_COLOR_ABLATION_FAIL = ColorTranslator.FromHtml("#FF0000");
    protected readonly Color SERIES_COLOR_BLOOD_PRESSURE = ColorTranslator.FromHtml("#38B4B4");
    protected readonly Color SERIES_COLOR_BLOOD_PRESSURE_SCAN_LINE = ColorTranslator.FromHtml("#000000");

    private const int SERIE_A = 0;
    private const int SERIE_B = 1;

    protected const double TEMPERATURE_MIN_VALUE = -80;
    protected const double TEMPERATURE_MAX_VALUE = 40;

    protected readonly ChartDashStyle TEMPERATURE_GRID_DASH_STYLE = ChartDashStyle.Dot;
    protected readonly ChartDashStyle ECG_GRID_DASH_STYLE = ChartDashStyle.Solid;

    protected CryoTherapyViewModel cryoTherapyViewModel;

    private int X_AXIS_BLOOD_PRESSURE = 1000;
    private int bloodPressurePointDisplayIndex = 0;

    private Color doctorPreferenceColor;

    private int _runtimeTemperatureSeries = 0;

    protected Series SerieTemperature;
    protected Series SerieVeinIsolationDuration;
    protected Series SerieAblationFail;

    private Series SerieBloodPressure;
    private Series SerieBloodPressureScanLine;
    private bool enableOcclusionPressureDisplay;

    private System.Timers.Timer ecgTimer = new System.Timers.Timer(50);

    protected Chart ChartTemperature;
    protected Chart ChartBloodPressure;

    protected bool _isEventSubscribed;
    protected int errorTimeToPixelInterval = 5;

    private readonly SerialDisposable _updateShadowingGraphDisposable = new SerialDisposable();

    private Stopwatch playBackStopWatch = new Stopwatch();

    private long playBackMMinimumTime = 1000;

    protected CryoTherapyChartViewBase()
    {
      ecgTimer.AutoReset = true;
      ecgTimer.Elapsed += ecgTimeElapsed;
    }

    protected virtual void SubscribeEventHandlers()
    {
      if (_isEventSubscribed)
      {
        return;
      }

      cryoTherapyViewModel = DataContext as CryoTherapyViewModel;
      cryoTherapyViewModel.SystemStateEvent += CryoTherapyViewModel_SystemStateEvent;
      cryoTherapyViewModel.StopAblation += CryoTherapyViewModel_StopAblation;
      CommonViewModel.Current.AblationTimerChangedEvent += CryoTherapyViewModel_TimerUpdatedEvent;
      cryoTherapyViewModel.ReadyStateEvent += CryoTherapyViewModel_ReadyStateEvent;
      cryoTherapyViewModel.InflationStateEvent += CryoTherapyViewModel_InflationStateEvent;
      cryoTherapyViewModel.PlaybackModeEvent += CryoTherapyViewModel_PlaybackModeEvent;
      cryoTherapyViewModel.ResetTherapyEvent += CryoTherapyViewModel_ResetTherapyEvent;
      cryoTherapyViewModel.TemperatureChartTypeChangedEvent += CryoTherapyViewModel_TemperatureChartTypeChangedEvent;
      cryoTherapyViewModel.OcclusionPressureGraphAxisYChangedEvent += CryoTherapyViewModel_OcclusionPressureGraphAxisYChangedEvent;
      cryoTherapyViewModel.OcclusionPressureGraphSweepSpeedChangedEvent += CryoTherapyViewModel_OcclusionPressureGraphSweepSpeedChangedEvent;
      cryoTherapyViewModel.ClearOcclusionPressureGraphRequestEvent += cryoTherapyViewModelOnClearOcclusionPressureGraphRequestEvent;

      _updateShadowingGraphDisposable.Disposable =
        cryoTherapyViewModel.UpdateShadowTemperatureGraphObservable
          .ObserveOnDispatcher()
          .Subscribe(HandleUpdateShadowGraphRequest);

      _isEventSubscribed = true;
    }

    protected virtual void UnsubscribeEventHandlers()
    {
      _updateShadowingGraphDisposable.Disposable = null;
      _mouseUpDisposable.Disposable = null;
      _mouseDownDisposable.Disposable = null;

      cryoTherapyViewModel.SystemStateEvent -= CryoTherapyViewModel_SystemStateEvent;
      cryoTherapyViewModel.StopAblation -= CryoTherapyViewModel_StopAblation;
      CommonViewModel.Current.AblationTimerChangedEvent -= CryoTherapyViewModel_TimerUpdatedEvent;
      cryoTherapyViewModel.ReadyStateEvent -= CryoTherapyViewModel_ReadyStateEvent;
      cryoTherapyViewModel.InflationStateEvent -= CryoTherapyViewModel_InflationStateEvent;
      cryoTherapyViewModel.ResetTherapyEvent -= CryoTherapyViewModel_ResetTherapyEvent;
      cryoTherapyViewModel.PlaybackModeEvent -= CryoTherapyViewModel_PlaybackModeEvent;
      cryoTherapyViewModel.TemperatureChartTypeChangedEvent -= CryoTherapyViewModel_TemperatureChartTypeChangedEvent;
      cryoTherapyViewModel.OcclusionPressureGraphAxisYChangedEvent -= CryoTherapyViewModel_OcclusionPressureGraphAxisYChangedEvent;
      cryoTherapyViewModel.OcclusionPressureGraphSweepSpeedChangedEvent -= CryoTherapyViewModel_OcclusionPressureGraphSweepSpeedChangedEvent;
      cryoTherapyViewModel.ClearOcclusionPressureGraphRequestEvent -= cryoTherapyViewModelOnClearOcclusionPressureGraphRequestEvent;

      _isEventSubscribed = false;
    }

    protected virtual void ControlLoaded()
    {
      cryoTherapyViewModel = DataContext as CryoTherapyViewModel;

      if (cryoTherapyViewModel == null)
        return;

      SubscribeEventHandlers();

      Task.Delay(2000).ContinueWith(t => enableOcclusionPressureDisplay = true);

      playBackStopWatch.Start();
      ClearCharts();
      ecgTimer?.Start();
      cryoTherapyViewModel.IsConsoleUsingDeflateAfterThawing = CommonViewModel.Current.Console.EnableDefalteAfterThaw;
    }

    protected virtual void ControlUnloaded()
    {
      try
      {
        Task.Delay(2000).ContinueWith(t => enableOcclusionPressureDisplay = false);
        playBackStopWatch.Reset();
        playBackStopWatch.Stop();

        if (ecgTimer?.Enabled ?? false)
        {
          ecgTimer.Stop();
        }

        ClearCharts();
      }
      catch (Exception ex)
      {
        LogException(ex);
      }

      //AppTrace.Log("Unloaded CryoTherapy.", LogLevel.Debug, Thread.CurrentThread.ManagedThreadId.ToString(), nameof(CryoTherapy), nameof(UserControl_Unloaded));

    }

    /// <summary>
    /// Initializes the Temperature Graphic properties.
    /// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="chartTemperature">A Chart object that represents the Temperature Chart.</param>
    protected Chart InitializeTemperatureGraphic()
    {
      //AppTrace.Log("Initializing Temperature Graph...", LogLevel.Debug, Thread.CurrentThread.ManagedThreadId.ToString(), nameof(CryoTherapy), nameof(InitializeTemperatureGraphic));
      var chartTemperature = new Chart() { BackColor = Color.Transparent, Name = "ChartTemperature", Dock = DockStyle.Fill, Enabled = false };
      if (chartTemperature?.ChartAreas != null)
      {
        chartTemperature.ChartAreas.Add("TemperatureArea");
        chartTemperature.ChartAreas[0].BackColor = Color.Transparent;
        chartTemperature.ChartAreas[0].AxisX.MinorGrid.Enabled = false;
        chartTemperature.ChartAreas[0].AxisX.Minimum = 0;
        chartTemperature.ChartAreas[0].AxisX.Maximum = 240;
        chartTemperature.ChartAreas[0].AxisX.Interval = 30;
        chartTemperature.ChartAreas[0].AxisX.MajorGrid.Enabled = true;
        chartTemperature.ChartAreas[0].AxisX.IsStartedFromZero = true;
        chartTemperature.ChartAreas[0].AxisX.MajorGrid.LineColor = GRID_LINES_COLOR;
        chartTemperature.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = TEMPERATURE_GRID_DASH_STYLE;
        chartTemperature.ChartAreas[0].AxisX.LineColor = GRID_LINES_COLOR;
        chartTemperature.ChartAreas[0].AxisX.LineDashStyle = TEMPERATURE_GRID_DASH_STYLE;
        chartTemperature.ChartAreas[0].AxisX.LabelStyle.ForeColor = GRID_LINES_COLOR;
        chartTemperature.ChartAreas[0].AxisX.LabelStyle.Font = new Font("Courrier New", 10.0f, System.Drawing.FontStyle.Bold);
        chartTemperature.ChartAreas[0].AxisX.LabelStyle.Format = "F0";

        chartTemperature.ChartAreas[0].AxisY.MajorGrid.LineColor = GRID_LINES_COLOR;
        chartTemperature.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = TEMPERATURE_GRID_DASH_STYLE;
        chartTemperature.ChartAreas[0].AxisY.LineColor = GRID_LINES_COLOR;
        chartTemperature.ChartAreas[0].AxisY.LabelStyle.ForeColor = GRID_LINES_COLOR;
        chartTemperature.ChartAreas[0].AxisY.LabelStyle.Font = new Font("Courrier New", 10.0f, System.Drawing.FontStyle.Bold);
        chartTemperature.ChartAreas[0].AxisY.LabelStyle.Format = "F0";
        chartTemperature.ChartAreas[0].AxisY.Minimum = TEMPERATURE_MIN_VALUE;
        chartTemperature.ChartAreas[0].AxisY.Maximum = TEMPERATURE_MAX_VALUE;
        chartTemperature.ChartAreas[0].AxisY.Interval = 20;

        //To make the X-axis appear on Y axis 0.
        chartTemperature.ChartAreas[0].AxisX.Crossing = 0;
        chartTemperature.ChartAreas[0].AxisY.Crossing = 0;
        UpdateLabelAxisX(chartTemperature);
      }

      if (chartTemperature?.Series != null)
      {
        chartTemperature.Series.Clear();
        _runtimeTemperatureSeries = 0;

        chartTemperature.Series.Add("Temperature");
        chartTemperature.Series[_runtimeTemperatureSeries].ChartType = SeriesChartType.FastLine;
        chartTemperature.Series[_runtimeTemperatureSeries].BorderWidth = 3;
        chartTemperature.Series[_runtimeTemperatureSeries].IsVisibleInLegend = false;
        chartTemperature.Series[_runtimeTemperatureSeries].Color = SERIES_COLOR;
        _runtimeTemperatureSeries++;

        //Add isolation vein duration series
        chartTemperature.Series.Add("VeinIsolationDuration");
        chartTemperature.Series[_runtimeTemperatureSeries].ChartType = SeriesChartType.Point;
        chartTemperature.Series[_runtimeTemperatureSeries].MarkerColor = ColorTranslator.FromHtml("#FF2A2A32");
        chartTemperature.Series[_runtimeTemperatureSeries].MarkerStyle = MarkerStyle.Circle;
        chartTemperature.Series[_runtimeTemperatureSeries].MarkerSize = 9;
        chartTemperature.Series[_runtimeTemperatureSeries].MarkerBorderColor = SERIES_COLOR_ISOLATED_VEIN;
        chartTemperature.Series[_runtimeTemperatureSeries].MarkerBorderWidth = 3;
        _runtimeTemperatureSeries++;

        chartTemperature.Series.Add("AblationFail");
        chartTemperature.Series[_runtimeTemperatureSeries].ChartType = SeriesChartType.Point;
        chartTemperature.Series[_runtimeTemperatureSeries].MarkerColor = ColorTranslator.FromHtml("#FF2A2A32");
        chartTemperature.Series[_runtimeTemperatureSeries].MarkerStyle = MarkerStyle.Triangle;
        chartTemperature.Series[_runtimeTemperatureSeries].MarkerBorderColor = SERIES_COLOR_ABLATION_FAIL;
        chartTemperature.Series[_runtimeTemperatureSeries].MarkerSize = 10;
        chartTemperature.Series[_runtimeTemperatureSeries].MarkerBorderWidth = 3;
      }

      if (chartTemperature != null)
      {
        chartTemperature.AntiAliasing = AntiAliasingStyles.None;
        chartTemperature.TextAntiAliasingQuality = TextAntiAliasingQuality.Normal;
        chartTemperature.MouseClick += ChartTemperature_MouseClick;
      }

      return chartTemperature;
    }

    protected void SetupTemperatureChart(WindowsFormsHost host, Chart chart)
    {
      SerieTemperature = chart.Series[0];
      SerieTemperature.Points.Add(0, 0);
      SerieVeinIsolationDuration = chart.Series[1];
      SerieAblationFail = chart.Series[2];

      ChartTemperature = chart;
      host.Child = ChartTemperature;
      host.TabIndex = 2;
    }

    /// <summary>
    /// Clears the temperature chart.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    protected void ClearTemperatureChart()
    {
      if (Dispatcher.CheckAccess())
      {
        ClearTemperatureChartAction();
      }
      else
      {
        Dispatcher.Invoke(ClearTemperatureChartAction);
      }
    }

    private void ClearTemperatureChartAction()
    {
      try
      {
        //AppTrace.Log("Clearing Temperature Chart ...", LogLevel.Debug, Thread.CurrentThread.ManagedThreadId.ToString(), nameof(CryoTherapy), nameof(ClearTemperatureChart));

        if (SerieTemperature != null && SerieTemperature.Points != null && SerieTemperature.Points.Count != 0 && ChartTemperature != null)
        {
          // we disable the graph selection when we are drawing in real time:
          ChartTemperature.Enabled = false;

          ChartTemperature.Annotations.Clear();
          SerieTemperature.Points.Clear();
          SerieTemperature.Points.Add(0, 0);
          SerieTemperature.Points[0].IsEmpty = true;  //Display the grids when there is no data

          //Remove the vein isolation duration bubble
          SerieVeinIsolationDuration?.Points?.Clear();

          //Remove the Ablation Fail shape
          SerieAblationFail?.Points?.Clear();

          if (!cryoTherapyViewModel.IsTreatmentNumberAndPlayBackVisible) // && !cryoTherapyViewModel.WasAblationTimeManuallyChanged)
          {
            if (cryoTherapyViewModel.IsFixedTimerSelected)
            {
              cryoTherapyViewModel.RefreshModeldata();
            }

            if (cryoTherapyViewModel.ISTTIFixedTimerSelected)
            {
              cryoTherapyViewModel.RequiredAblationTime = cryoTherapyViewModel.NewAblationTimerTTIFixed;
              cryoTherapyViewModel.ISTTISelected = true;
            }
            else if (cryoTherapyViewModel.ISTTIDurationTimerSelected)
            {
              cryoTherapyViewModel.RequiredAblationTime = Constants.MaximumTTIDurationTimer;
              cryoTherapyViewModel.ISTTISelected = true;
            }
          }

          switch (cryoTherapyViewModel.NotificationModel.CurrentPhysician.preference.CurveColor)
          {
            case (int)Enumeration.SerieColor.Blue:

              doctorPreferenceColor = ColorTranslator.FromHtml("#00AFEF");

              break;

            case (int)Enumeration.SerieColor.Yellow:

              doctorPreferenceColor = ColorTranslator.FromHtml("#0077BE");

              break;

            case (int)Enumeration.SerieColor.Green:
              doctorPreferenceColor = ColorTranslator.FromHtml("#0000FF");

              break;

            case (int)Enumeration.SerieColor.White:

              doctorPreferenceColor = Color.White;

              break;
          }

          switch (cryoTherapyViewModel.TemperatureChartType)
          {
            case (int)Enumeration.CurveStyle.Line:

              SerieTemperature.ChartType = SeriesChartType.Line;
              break;

            case (int)Enumeration.CurveStyle.Area:

              SerieTemperature.ChartType = SeriesChartType.Area;
              break;

            default:

              ChartTemperature.Series[0].ChartType = SeriesChartType.Line;
              break;
          }
        }

        if (!cryoTherapyViewModel.IsTreatmentNumberAndPlayBackVisible && ChartTemperature?.ChartAreas != null)
        {
          var axisX = ChartTemperature.ChartAreas[0].AxisX;
          if (axisX == null) return;
          axisX.Maximum = 240;
          axisX.Interval = 30;
        }

        //AppTrace.Log("Cleared Temperature Chart.", LogLevel.Debug, Thread.CurrentThread.ManagedThreadId.ToString(), nameof(CryoTherapy), nameof(ClearTemperatureChart));
      }
      catch (Exception ex)
      {
        LogException(ex);
      }

    }

    /// <summary>
    /// Display other graphs, e.g. DMS graph if needed, depends on current selected time on temperature graph.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="time">The component that raised the event.</param>
    protected virtual void DisplayOtherGraphsVsTime(int time)
    {
    }

    /// <summary>
    /// Occurs when the ChartTemperature_MouseClick event is raised.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="sender">The component that raised the event.</param>
    /// <param name="e">A MouseEventArgs that contains the event data.</param>
    protected virtual void ChartTemperature_MouseClick(object sender, MouseEventArgs e)
    {
      var screenPosition = e.Location;
      if (ChartTemperature != null && cryoTherapyViewModel?.SingleAblationDatasList?.Count > 0)
      {
        var results = ChartTemperature.HitTest(screenPosition.X, screenPosition.Y, false, ChartElementType.PlottingArea);
        foreach (var result in results)
        {
          if (result.ChartElementType == ChartElementType.PlottingArea)
          {
            int time = (int)(Math.Round(result.ChartArea.AxisX.PixelPositionToValue(screenPosition.X)));

            //Give the user some feedback by displaying a vertical line where the chart has been touched/clicked
            ChartTemperature.Annotations.Clear();
            var verticalLine = new VerticalLineAnnotation();
            verticalLine.AxisX = ChartTemperature.ChartAreas[0].AxisX;
            verticalLine.AllowMoving = true;
            verticalLine.IsInfinitive = true;
            verticalLine.ClipToChartArea = verticalLine.Name;
            verticalLine.Name = "myVerticalLine";
            verticalLine.LineColor = ColorTranslator.FromHtml("#00afef");
            verticalLine.LineWidth = 2;
            verticalLine.X = time;
            verticalLine.ClipToChartArea = ChartTemperature.ChartAreas[0].Name;

            ChartTemperature.Annotations.Add(verticalLine);

            DisplayTemperatureVsTime(Convert.ToInt32(time));

            // Display other graphs if needed
            DisplayOtherGraphsVsTime(time);
          }
        }
      }
    }

    /// <summary>
    /// Initializes the Common properties of charts.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="chart">A Chart representing a common chart.</param>
    protected void InitializeCommonGraphic(Chart chart)
    {
      //AppTrace.Log("Initializing Common Graph ...", LogLevel.Debug, Thread.CurrentThread.ManagedThreadId.ToString(), nameof(CryoTherapy), nameof(InitializeCommonGraphic));

      try
      {
        if (chart != null)
        {
          //chart.Dock = System.Windows.Forms.DockStyle.Fill;
          chart.BackColor = Color.Transparent;
          chart.ChartAreas.Add("ChartArea");
          chart.ChartAreas[0].BackColor = Color.Transparent;
          chart.ChartAreas[0].AxisX.IsStartedFromZero = true;
          chart.ChartAreas[0].AxisX.LineDashStyle = ChartDashStyle.NotSet;
          chart.ChartAreas[0].AxisX.LineColor = ECG_GRID_LINES_COLOR;
          chart.ChartAreas[0].AxisX.LabelStyle.Enabled = false;  //hides the axis labels
          chart.ChartAreas[0].AxisX.MinorGrid.Enabled = false;
          chart.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
          chart.ChartAreas[0].AxisX.MajorGrid.LineColor = ECG_GRID_LINES_COLOR;
          chart.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ECG_GRID_DASH_STYLE;
          chart.ChartAreas[0].AxisX.MajorTickMark.Enabled = false;  //hides marks on the axis labels

          chart.ChartAreas[0].AxisY.LineDashStyle = ChartDashStyle.NotSet;
          chart.ChartAreas[0].AxisY.LineColor = ECG_GRID_LINES_COLOR;
          chart.ChartAreas[0].AxisY.LabelStyle.Enabled = false;  //hides the axis labels
          chart.ChartAreas[0].AxisY.MinorGrid.Enabled = false;
          chart.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
          chart.ChartAreas[0].AxisY.MajorGrid.LineColor = ECG_GRID_LINES_COLOR;
          chart.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ECG_GRID_DASH_STYLE;
          chart.ChartAreas[0].AxisY.MajorTickMark.Enabled = false;  //hides marks on the axis labels

          // Set Anti aliasing mode
          // this can be set lower if there are any performance issues!
          chart.AntiAliasing = AntiAliasingStyles.None;
          chart.TextAntiAliasingQuality = TextAntiAliasingQuality.Normal;
        }
      }
      catch (Exception ex)
      {
        LogException(ex);
        throw;
      }

      //AppTrace.Log("Common Graph Initialized", LogLevel.Debug, Thread.CurrentThread.ManagedThreadId.ToString(), nameof(CryoTherapy), nameof(InitializeCommonGraphic));
    }

    /// <summary>
    /// Initializes a common serie.
    /// </summary>
    /// <param name="series">A SeriesCollection representing chart series.</param>
    /// <param name="name">A string representing a chart name</param>
    protected void InitializeCommonSerie(SeriesCollection series, string name)
    {
      //AppTrace.Log("Initializing Common Series ...", LogLevel.Debug, Thread.CurrentThread.ManagedThreadId.ToString(), nameof(CryoTherapy), nameof(InitializeCommonSerie));

      try
      {
        if (series != null)
        {
          series.Clear();
          series.Add(name + "_A");
          series[SERIE_A].ChartType = SeriesChartType.FastLine;
          series[SERIE_A].BorderWidth = 2;
          series[SERIE_A].IsVisibleInLegend = false;
          series[SERIE_A].Color = SERIES_COLOR;

          series.Add(name + "_B");
          series[SERIE_B].ChartType = SeriesChartType.FastLine;
          series[SERIE_B].BorderWidth = 2;
          series[SERIE_B].IsVisibleInLegend = false;
          series[SERIE_B].Color = SERIES_COLOR;
        }
      }
      catch (Exception ex)
      {
        LogException(ex);
        throw;
      }

      //AppTrace.Log("Common Series Initialized", LogLevel.Debug, Thread.CurrentThread.ManagedThreadId.ToString(), nameof(CryoTherapy), nameof(InitializeCommonSerie));
    }

    /// <summary>
    /// Initializes the blood pressure graphic properties
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="chartBloodPressure">A Chart object that represents the Blood pressure Chart.</param>
    protected Chart InitializeBloodPressureGraphic()
    {
      //AppTrace.Log("Initializing Blood Pressure Graph...", LogLevel.Debug, Thread.CurrentThread.ManagedThreadId.ToString(), nameof(CryoTherapy), nameof(InitializeBloodPressureGraphic));
      ChartBloodPressure = new Chart() { BackColor = Color.Transparent, Name = "BloodPressureHost", Dock = DockStyle.Fill, Enabled = false };
      X_AXIS_BLOOD_PRESSURE = calculateOcclusionPressureGraphAxisXMaximum();

      ChartBloodPressure.ChartAreas.Add("BloodPressure");
      ChartBloodPressure.ChartAreas[0].BackColor = Color.Transparent;
      ChartBloodPressure.ChartAreas[0].AxisX.Minimum = 0;
      ChartBloodPressure.ChartAreas[0].AxisX.Maximum = X_AXIS_BLOOD_PRESSURE; //calculateOcclusionPressureGraphAxisXMaximum();
      ChartBloodPressure.ChartAreas[0].AxisX.Interval = 1; // depending wich id we are using this value can be set to 10
      ChartBloodPressure.ChartAreas[0].AxisX.IsStartedFromZero = true;
      ChartBloodPressure.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.Red;
      ChartBloodPressure.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Solid;
      ChartBloodPressure.ChartAreas[0].AxisX.LineColor = GRID_LINES_COLOR;
      ChartBloodPressure.ChartAreas[0].AxisX.LineDashStyle = ChartDashStyle.Solid;
      ChartBloodPressure.ChartAreas[0].AxisX.LabelStyle.ForeColor = GRID_LINES_COLOR;
      ChartBloodPressure.ChartAreas[0].AxisX.LabelStyle.Font = new Font("Courrier New", 10.0f, System.Drawing.FontStyle.Bold);

      // use these code to make quick change to the GUI
      ChartBloodPressure.ChartAreas[0].AxisX.MajorGrid.Enabled = false; // DO not display the vertical line
      ChartBloodPressure.ChartAreas[0].AxisX.MinorGrid.Enabled = false;
      ChartBloodPressure.ChartAreas[0].AxisX.LabelStyle.Enabled = false; // Do not display X legend 

      ChartBloodPressure.ChartAreas[0].AxisY.Minimum = cryoTherapyViewModel.OcclusionPressureGraphAxisYMinimum;
      ChartBloodPressure.ChartAreas[0].AxisY.Maximum = cryoTherapyViewModel.OcclusionPressureGraphAxisYMaximum;
      ChartBloodPressure.ChartAreas[0].AxisY.Interval = 10;
      ChartBloodPressure.ChartAreas[0].AxisY.IntervalOffset = (ChartBloodPressure.ChartAreas[0].AxisY.Interval - ChartBloodPressure.ChartAreas[0].AxisY.Minimum) % ChartBloodPressure.ChartAreas[0].AxisY.Interval;
      ChartBloodPressure.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.White; //GRID_LINES_COLOR;
      ChartBloodPressure.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
      ChartBloodPressure.ChartAreas[0].AxisY.LineColor = GRID_LINES_COLOR;
      ChartBloodPressure.ChartAreas[0].AxisY.LineDashStyle = ChartDashStyle.Solid;
      ChartBloodPressure.ChartAreas[0].AxisY.LabelStyle.ForeColor = GRID_LINES_COLOR;
      ChartBloodPressure.ChartAreas[0].AxisY.LabelStyle.Font = new Font("Courrier New", 10.0f, System.Drawing.FontStyle.Bold);

      // Make quick change
      ChartBloodPressure.ChartAreas[0].AxisY.MajorGrid.Enabled = true; // DO not display the horizontal line line
      ChartBloodPressure.ChartAreas[0].AxisY.MinorGrid.Enabled = false;
      ChartBloodPressure.ChartAreas[0].AxisY.LabelStyle.Enabled = true; // Do not display Y legend 

      ChartBloodPressure.ChartAreas[0].BorderDashStyle = ChartDashStyle.Solid;
      ChartBloodPressure.ChartAreas[0].BorderWidth = 1;
      ChartBloodPressure.ChartAreas[0].BorderColor = Color.White;

      //To make the X-axis appear on Y axis 0.
      ChartBloodPressure.ChartAreas[0].AxisX.Crossing = 0;
      ChartBloodPressure.ChartAreas[0].AxisY.Crossing = 0;

      //// Set Anti-aliasing mode
      //// this can be set lower if there are any performance issues!
      ChartBloodPressure.AntiAliasing = AntiAliasingStyles.None;
      ChartBloodPressure.TextAntiAliasingQuality = TextAntiAliasingQuality.Normal;

      ChartBloodPressure.Series.Clear();
      ChartBloodPressure.Series.Add("Blood Pressure");
      ChartBloodPressure.Series[0].ChartType = SeriesChartType.FastLine;
      ChartBloodPressure.Series[0].BorderWidth = 1;
      ChartBloodPressure.Series[0].IsVisibleInLegend = false;
      ChartBloodPressure.Series[0].Color = SERIES_COLOR_BLOOD_PRESSURE;
      SerieBloodPressure = ChartBloodPressure.Series[0];

      ChartBloodPressure.Series.Add("Blood Pressure Scan Line");
      ChartBloodPressure.Series[1].ChartType = SeriesChartType.FastLine;
      ChartBloodPressure.Series[1].BorderWidth = 8;
      ChartBloodPressure.Series[1].IsVisibleInLegend = false;
      ChartBloodPressure.Series[1].Color = SERIES_COLOR_BLOOD_PRESSURE_SCAN_LINE;
      SerieBloodPressureScanLine = ChartBloodPressure.Series[1];

      ChartBloodPressure.MouseClick += ChartBloodPressure_MouseClick;

      return ChartBloodPressure;
    }

    protected void SetupOcclusionPressureGraph(WindowsFormsHost host, Chart chart)
    {
      SerieBloodPressure = chart.Series[0];
      SerieBloodPressure.Points.Add(0, 0);
      SerieBloodPressureScanLine = chart.Series[1];
      SerieBloodPressureScanLine.Points.AddXY(0, 0);

      //blood pressure child
      host.Child = ChartBloodPressure;
      host.TabIndex = 2; // we can change that after testing 
    }

    /* 10 levels of Sweep Speed, 1 being 1000 points displayed, 2 being 900, 3 being 800....10 being 100 */
    private int calculateOcclusionPressureGraphAxisXMaximum()
    {
      return (-100 * cryoTherapyViewModel.OcclusionPressureGraphSweepSpeed + 1100);
    }

    /// <summary>
    /// Occurs when chart blood pressure mouse click.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="sender">The component that raised the event.</param>
    /// <param name="e">Represents the base class for classes that contain user control event data.</param>
    private void ChartBloodPressure_MouseClick(object sender, MouseEventArgs e)
    {

    }

    /// <summary>
    /// Occurs when the CryoTherapyViewModel_TemperatureChartTypeChangedEvent event is raised.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="sender">The component that raised the event.</param>
    /// <param name="e">Represents the base class for classes that contain user control event data.</param>
    private void CryoTherapyViewModel_TemperatureChartTypeChangedEvent(object sender, EventArgs e)
    {
      if (sender != null && sender is short &&
          ChartTemperature != null && ChartTemperature.Series != null && ChartTemperature.Series.Count > 0)
      {
        if ((short)sender == (int)Enumeration.CurveStyle.Line)
        {
          SerieTemperature.ChartType = SeriesChartType.Line;
        }
        else if ((short)sender == (int)Enumeration.CurveStyle.Area)
        {
          SerieTemperature.ChartType = SeriesChartType.Area;
        }
        else
        {
          SerieTemperature.ChartType = SeriesChartType.Line;
        }
      }
    }

    /// <summary>
    /// Occurs when the CryoTherapyViewModel_OcclusionPressureGraphAxisYChangedEvent event is raised.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="sender">The component that raised the event.</param>
    /// <param name="e">Represents the base class for classes that contain user control event data.</param>
    private void CryoTherapyViewModel_OcclusionPressureGraphAxisYChangedEvent(object sender, OcclusionPressureGraphAxisYEventArgs e)
    {
      //AppTrace.Log("Updating Occlusion Pressure Graph Y-Axis Value Change Event ...", LogLevel.Debug, Thread.CurrentThread.ManagedThreadId.ToString(), nameof(CryoTherapy), nameof(CryoTherapyViewModel_OcclusionPressureGraphAxisYChangedEvent));

      Dispatcher.Invoke(() =>
        {

          if (ChartBloodPressure == null)
          {
            return;
          }

          if (ChartBloodPressure != null)
          {
            if (e.LimitID == "Maximum")
              ChartBloodPressure.ChartAreas[0].AxisY.Maximum = cryoTherapyViewModel.OcclusionPressureGraphAxisYMaximum;
            else if (e.LimitID == "Minimum")
            {
              ChartBloodPressure.ChartAreas[0].AxisY.Minimum = cryoTherapyViewModel.OcclusionPressureGraphAxisYMinimum;
              ChartBloodPressure.ChartAreas[0].AxisY.IntervalOffset = (ChartBloodPressure.ChartAreas[0].AxisY.Interval - ChartBloodPressure.ChartAreas[0].AxisY.Minimum) % ChartBloodPressure.ChartAreas[0].AxisY.Interval;
            }
          }

        }
      );

      //AppTrace.Log("Updating Occlusion Pressure Graph Y-Axis Value Change Event Ended", LogLevel.Debug, Thread.CurrentThread.ManagedThreadId.ToString(), nameof(CryoTherapy), nameof(CryoTherapyViewModel_OcclusionPressureGraphAxisYChangedEvent));
    }

    /// <summary>
    /// Occurs when the CryoTherapyViewModel_OcclusionPressureGraphSweepSpeedChangedEvent event is raised.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="sender">The component that raised the event.</param>
    /// <param name="e">Represents the base class for classes that contain user control event data.</param>
    private void CryoTherapyViewModel_OcclusionPressureGraphSweepSpeedChangedEvent(object sender, EventArgs e)
    {
      //AppTrace.Log("Updating Occlusion Pressure Graph Sweep Speed Value Change Event ...", LogLevel.Debug, Thread.CurrentThread.ManagedThreadId.ToString(), nameof(CryoTherapy), nameof(CryoTherapyViewModel_OcclusionPressureGraphSweepSpeedChangedEvent));

      Dispatcher.Invoke(() =>
      {

        if (ChartBloodPressure == null)
        {
          return;
        }

        if (ChartBloodPressure != null)
        {
          int BloodPressureAxisXMaximum = calculateOcclusionPressureGraphAxisXMaximum();

          /* Calculate how many extra points will have to be removed in the chart series */
          int extraPoints = X_AXIS_BLOOD_PRESSURE - BloodPressureAxisXMaximum;

          X_AXIS_BLOOD_PRESSURE = BloodPressureAxisXMaximum;
          /* Update the graph X-Axis maximum */
          ChartBloodPressure.ChartAreas[0].AxisX.Maximum = X_AXIS_BLOOD_PRESSURE;

          /* If any extra points in the series, remove them */
          if (extraPoints > 0)
          {
            if (SerieBloodPressure.Points.Count < extraPoints)
            {
              SerieBloodPressure.Points.Clear();
              bloodPressurePointDisplayIndex = 0;
            }
            else
            {
              for (int i = 0; i < extraPoints; i++)
              {
                SerieBloodPressure.Points.RemoveAt(0);
                bloodPressurePointDisplayIndex--;
                if (bloodPressurePointDisplayIndex == -1)
                {
                  bloodPressurePointDisplayIndex = X_AXIS_BLOOD_PRESSURE;
                }
              }
            }
          }
          SerieBloodPressureScanLine.Points.Clear();
          SerieBloodPressureScanLine.Points.AddXY(bloodPressurePointDisplayIndex + 1, cryoTherapyViewModel.OcclusionPressureGraphAxisYMinimum);
          SerieBloodPressureScanLine.Points.AddXY(bloodPressurePointDisplayIndex + 1, cryoTherapyViewModel.OcclusionPressureGraphAxisYMaximum);
        }

      }
      );

      //AppTrace.Log("Updating Occlusion Pressure Graph Sweep Speed Value Change Event Ended", LogLevel.Debug, Thread.CurrentThread.ManagedThreadId.ToString(), nameof(CryoTherapy), nameof(CryoTherapyViewModel_OcclusionPressureGraphSweepSpeedChangedEvent));
    }
    /// <summary>
    /// Occurs when the ecgOcclusionPressureTimer_tick event is raised.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="sender">The component that raised the event.</param>
    /// <param name="e">Represents the base class for classes that contain user control event data.</param>
    private void UpdateEcgOcclusionPressure(double[] bloodPressureValue)
    {
      //Blood pressure data injection 
      if (SerieBloodPressure != null && SerieBloodPressureScanLine != null
          && SensorReadingMananger.AreSensorsConnected && cryoTherapyViewModel.IsMonitoringBloodPressure)
      {
        DispatcherBeginInvoke(DispatcherPriority.Normal, () => UpdateOcclusionPressureGraph(bloodPressureValue));
      }
    }

    private void UpdateOcclusionPressureGraph(double[] bloodPressureValues)
    {
      // This prevents index out of range if the user changes the slider values very quickly.
      if (bloodPressurePointDisplayIndex > SerieBloodPressure.Points.Count)
        bloodPressurePointDisplayIndex = SerieBloodPressure.Points.Count;

      // Update the vertical scan line's position
      SerieBloodPressureScanLine.Points.Clear();

      // Insert Occlusion Pressure data in SeriesBloodPressure
      for (int bloodIndex = 0; bloodIndex < bloodPressureValues.Length; ++bloodIndex)
      {
        // If display index is at the end of the Series, insert new point at the end.
        if (bloodPressurePointDisplayIndex == SerieBloodPressure.Points.Count)
          SerieBloodPressure.Points.InsertY(bloodPressurePointDisplayIndex, bloodPressureValues[bloodIndex]);
        // If display index is not at the end of the Series, overwrite current point.
        else
        {
          SerieBloodPressure.Points.RemoveAt(bloodPressurePointDisplayIndex);
          SerieBloodPressure.Points.InsertY(bloodPressurePointDisplayIndex, bloodPressureValues[bloodIndex]);
        }

        // Increment display index
        ++bloodPressurePointDisplayIndex;

        // If display index reaches the maximum x_axis, reset to 0
        if (bloodPressurePointDisplayIndex >= X_AXIS_BLOOD_PRESSURE)
        {
          bloodPressurePointDisplayIndex = 0;
        }
      }

      SerieBloodPressureScanLine.Points.AddXY(bloodPressurePointDisplayIndex, cryoTherapyViewModel.OcclusionPressureGraphAxisYMinimum);
      SerieBloodPressureScanLine.Points.AddXY(bloodPressurePointDisplayIndex, cryoTherapyViewModel.OcclusionPressureGraphAxisYMaximum);
    }

    protected virtual void ClearCharts()
    {
      ClearTemperatureChart();
      ClearBloodPressureGraph();
      cryoTherapyViewModel?.ClearBloodPressureData();
    }

    /// <summary>
    /// Occurs when the CryoTherapyViewModel_ResetTherapyEvent event is raised.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="sender">The component that raised the event.</param>
    /// <param name="e">Represents the base class for classes that contain user control event data.</param>
    private void CryoTherapyViewModel_ResetTherapyEvent(object sender, EventArgs e)
    {
      try
      {
        if (NotificationModel.Instance.CurrentPhysician != null &&
          NotificationModel.Instance.CurrentPhysician.preference != null)
        {
          cryoTherapyViewModel.CryoTherapyTime = 0;
          cryoTherapyViewModel.LastCryoTherapyTime = 0;
        }

        ClearCharts();
      }
      catch (Exception ex)
      {
        LogException(ex);
        Tuple<long, string, string, string> genericMessage = Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID113, (int)Enumeration.ErrorTypes.GUI);
        MessagePopup messagePopup = new MessagePopup(genericMessage, MessagePopup.MessageType.ErrorMessage, MessagePopup.ButtonType.Ok);
      }
    }

    /// <summary>
    /// Occurs when the CryoTherapyViewModel_PlaybackModeEvent event is raised.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="sender">The component that raised the event.</param>
    /// <param name="e">Represents the base class for classes that contain user control event data.</param>
    private void CryoTherapyViewModel_PlaybackModeEvent(object sender, EventArgs e)
    {
      //Note right now the arguments are null, but we can add to it in the future if required. We should  probably set a flag for the mode
      //this.InitializeTemperatureGraphic();
      //Clear and load the temperature grid
      //Load/populate the chart
      if (playBackStopWatch.ElapsedMilliseconds > playBackMMinimumTime || cryoTherapyViewModel.IsReloadingPreviuosProcdure)
      {
        playBackStopWatch.Restart();

        if (cryoTherapyViewModel.IsUsingAutoPlayback || !cryoTherapyViewModel.IsPlayBackModeDeactivted)
        {
          CommonViewModel.Current.GUIIsRunning = false;
          LoadTemperatureChart();
        }
        else
        {
          ClearTemperatureChart();
        }
      }
    }

    /// <summary>
    /// Occurs when the CryoTherapyViewModel_StopAblation event is raised.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="sender">The component that raised the event.</param>
    /// <param name="e">Represents the base class for classes that contain user control event data.</param>
    protected virtual void CryoTherapyViewModel_StopAblation(object sender, EventArgs e)
    {
      ClearTemperatureChart();
    }

    /// <summary>
    /// Occurs when the CryoTherapyViewModel_TimerUpdatedEvent event is raised.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="sender">The component that raised the event.</param>
    /// <param name="e">A AblationTimerEventArgs that contains the event data.</param>
    protected virtual void CryoTherapyViewModel_TimerUpdatedEvent(object sender, AblationTimerEventArgs e)
    {

      if (ChartTemperature == null)
      {
        return;
      }

      DispatcherBeginInvoke(DispatcherPriority.Normal, () =>
      {
        if (ChartTemperature?.ChartAreas[0]?.AxisX != null)
        {
          ChartTemperature.BeginInit();
          var timer = (int)e.Seconds;

          if (cryoTherapyViewModel.IsTreatmentNumberAndPlayBackVisible)
          {
            if (cryoTherapyViewModel.TotalAblationDuration > timer && (cryoTherapyViewModel.PreviousTreatmentNumber != cryoTherapyViewModel.TreatmentNumber))
            {
              int remainder = (cryoTherapyViewModel.TotalAblationDuration % 30);
              timer = cryoTherapyViewModel.TotalAblationDuration + (30 - remainder);
            }
          }

          if (!cryoTherapyViewModel.IsTreatmentNumberAndPlayBackVisible)
          {
            ChartTemperature.ChartAreas[0].AxisX.Maximum = 240;
          }

          ChartTemperature.ChartAreas[0].AxisX.Interval = 30;
        }

        if (ChartTemperature != null)
          ChartTemperature.EndInit();
      }
      );
      // }
    }

    /// <summary>
    /// Sets the Temperature Chart interval.
    /// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="xAxisTimeValue">An integer representing X axis time value.</param>
    /// <param name="isPresetValue">An boolean representing a preset value.</param>
    private void SetTemperatureChartInterval(int xAxisTimeValue, bool isPresetValue = false)
    {
      if (isPresetValue)
      {
        //Preset values set by pressing the timer increase/decrease arrows.
        switch (xAxisTimeValue)
        {
          case 30:
            ChartTemperature.ChartAreas[0].AxisX.Interval = 5;
            break;

          case 60:
            ChartTemperature.ChartAreas[0].AxisX.Interval = 10;
            break;

          case 90:
          case 120:
          case 150:
            ChartTemperature.ChartAreas[0].AxisX.Interval = 20;
            break;

          case 180:
          case 210:
          case 240:
            ChartTemperature.ChartAreas[0].AxisX.Interval = 30;
            break;

          case 270:
          case 300:
          case 330:
          case 360:
          case 390:
          case 420:
          case 480:
            ChartTemperature.ChartAreas[0].AxisX.Interval = 50;
            break;
        }
      }
      else
      {
        //x-axis time value used to increase the chart max value when it exceed the preset value.
        //Example : The preset value is 420 (chart max x-axis value is 420) but the timer reaches 421 seconds,
        //          the chart max x-axis value needs to increase so all data is visible.
      }
    }


    /// <summary>
    /// Updates the temperature series color.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="temperature">A double representing the temperature.</param>
    public void UpdateTemperatureSeriesColor(double temperature)
    {
      //AppTrace.Log("Updating Temperature Series Color.", LogLevel.Debug, Thread.CurrentThread.ManagedThreadId.ToString(), nameof(CryoTherapy), nameof(UpdateTemperatureSeriesColor));

      //When in ablation and the current Temperature reading exceeds the min or max threshold,
      //display the series in red.
      if (CommonViewModel.Current.SystemState != MessageStateId.CAN_ID_STATE_THAWING &&
          !CommonViewModel.Current.AreSensorsInPlayBackMode &&
          (temperature >= cryoTherapyViewModel.HighAblationTemperatureAlarm ||
           temperature <= cryoTherapyViewModel.LowAblationTemperatureAlarm))
      {
        if (SerieTemperature.Color != SERIES_COLOR_THRESHOLD_EXCEEDED)
        {
          SerieTemperature.Color = SERIES_COLOR_THRESHOLD_EXCEEDED;
        }
      }
      else
      {
        if (SerieTemperature.Color != doctorPreferenceColor)
        {
          SerieTemperature.Color = doctorPreferenceColor;
        }
      }

      //AppTrace.Log("Updated Temperature Series Color.", LogLevel.Debug, Thread.CurrentThread.ManagedThreadId.ToString(), nameof(CryoTherapy), nameof(UpdateTemperatureSeriesColor));
    }

    private void UpdateLabelAxisX(Chart ChartTemperature)
    {
      ChartTemperature.ChartAreas[0].AxisX.CustomLabels.Clear();

      for (double j = 0; j <= ChartTemperature.ChartAreas[0].AxisX.Maximum; j += ChartTemperature.ChartAreas[0].AxisX.Interval)
      {
        if (j != 0) // Exclude the zero label
        {
          CustomLabel customLabel = new CustomLabel();
          customLabel.FromPosition = j - (ChartTemperature.ChartAreas[0].AxisX.Interval / 2);
          customLabel.ToPosition = j + (ChartTemperature.ChartAreas[0].AxisX.Interval / 2);
          customLabel.Text = j.ToString();
          ChartTemperature.ChartAreas[0].AxisX.CustomLabels.Add(customLabel);
        }
      }
    }

    /// <summary>
    /// Occurs when the CryoTherapyViewModel_SystemStateEvent event is raised.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="sender">The component that raised the event.</param>
    /// <param name="e">A AblationEventArgs that contains the event data.</param>
    private void CryoTherapyViewModel_SystemStateEvent(object sender, AblationEventArgs e)
    {
      if (!IsVisible)
        return;

      DispatcherBeginInvoke(DispatcherPriority.Normal, () =>
      {
        if (SerieTemperature?.Points != null && ChartTemperature != null)
        {
          int time = e.Compter;
          if (time < cryoTherapyViewModel.TimePreviousRefrence && time != 1)
            return;

          cryoTherapyViewModel.TimePreviousRefrence = time;

          if (e.Compter == 1)
          {
            SerieTemperature.Points.Clear();
            ChartTemperature.Annotations.Clear();

            //Remove the vein isolation duration bubble
            SerieVeinIsolationDuration?.Points?.Clear();

            //Remove the SerieAblationFail Triangle
            SerieAblationFail?.Points?.Clear();

            // Reset the timers 

            //if (cryoTherapyViewModel.IsFixedTimerSelected)
            //    cryoTherapyViewModel.RequiredAblationTime = this.cryoTherapyViewModel.NotificationModel.CurrentPhysician.preference.AblationTimer;

            if (cryoTherapyViewModel.ISTTIFixedTimerSelected)
              cryoTherapyViewModel.RequiredAblationTime = cryoTherapyViewModel.NewAblationTimerTTIFixed;

            if (cryoTherapyViewModel.ISTTIDurationTimerSelected)
              cryoTherapyViewModel.RequiredAblationTime = Constants.MaximumTTIDurationTimer;
          }

          SerieTemperature.Points.AddXY(e.Compter, e.Temperature);

          // we verify if new isolation is requested 
          if (cryoTherapyViewModel.LastVeinIsolationDuration != cryoTherapyViewModel.VeinIsolationDuration)
            SerieVeinIsolationDuration?.Points.Clear();

          //Add Vein Isolation bubble when its value is not 0 and
          //when it has not been added yet.
          if (cryoTherapyViewModel.VeinIsolationDuration != 0 && SerieVeinIsolationDuration?.Points?.Count == 0)
          {
            cryoTherapyViewModel.LastVeinIsolationDuration = cryoTherapyViewModel.VeinIsolationDuration;

            if (!cryoTherapyViewModel.ISTTIDurationTimerSelected)
            {
              cryoTherapyViewModel.CryoDurationChanged = true;
            }

            var ablationData = cryoTherapyViewModel.SingleAblationDatasList.FindLast(x => x.ID == cryoTherapyViewModel.VeinIsolationDuration);
            var temperatureTTI = ablationData?.TC1Reading ?? cryoTherapyViewModel.TEMPTTI;

            SerieVeinIsolationDuration?.Points.AddXY(cryoTherapyViewModel.VeinIsolationDuration, temperatureTTI);
          }

          //Add SerieAblationFail when exception state occurs 
          if (cryoTherapyViewModel.SystemState == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_EXCEPTION && SerieAblationFail?.Points?.Count == 0)
          {
            SerieAblationFail.Points.AddXY(e.Compter, e.Temperature);
          }

          //When the counter (X-axis) exceeds the x-axis max value (set by Timer), update the chart x-axis maximum value.
          if (e.Compter >= ChartTemperature?.ChartAreas[0]?.AxisX?.Maximum)
          {
            ////Double the X-axis maximum value
            //ChartTemperature.ChartAreas[0].AxisX.Maximum *= 2;
            ////Update the intervals to have at most 10 ticks (unreadable above that value)
            //ChartTemperature.ChartAreas[0].AxisX.Interval = (int)(ChartTemperature.ChartAreas[0].AxisX.Maximum / 10);

            //New Code
            //Double the X-axis maximum value
            ChartTemperature.ChartAreas[0].AxisX.Maximum += 30;
            UpdateLabelAxisX(ChartTemperature);
          }

          //When the current Temperature reading exceeds the min or max threshold, display the serie in red.
          UpdateTemperatureSeriesColor(e.Temperature);
        }
      });
    }

    /// <summary>
    /// Clear the blood pressure graph
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    protected void ClearBloodPressureGraph()
    {
      DispatcherBeginInvoke(DispatcherPriority.Normal, () =>
      {
        try
        {
          //AppTrace.Log("Clearing Blood Pressure Graph ...", LogLevel.Debug, Thread.CurrentThread.ManagedThreadId.ToString(), nameof(CryoTherapy), nameof(ClearBloodPressureGraph));

          SerieBloodPressure?.Points.Clear();
          SerieBloodPressure?.Points.Add(0, 0);

          if (SerieBloodPressure != null && SerieBloodPressure.Points.Count > 0)
          {
            SerieBloodPressure.Points[0].IsEmpty = true;
          }

          bloodPressurePointDisplayIndex = 0;
          SerieBloodPressureScanLine?.Points.Clear();
          SerieBloodPressureScanLine?.Points.AddXY(bloodPressurePointDisplayIndex + 1, cryoTherapyViewModel.OcclusionPressureGraphAxisYMinimum);
          SerieBloodPressureScanLine?.Points.AddXY(bloodPressurePointDisplayIndex + 1, cryoTherapyViewModel.OcclusionPressureGraphAxisYMaximum);

          if (SerieBloodPressureScanLine != null && SerieBloodPressureScanLine.Points.Count > 0)
          {
            SerieBloodPressureScanLine.Points[0].IsEmpty = true;
          }
        }
        catch (Exception ex)
        {
          LogException(ex);
        }
      });
    }

    private void cryoTherapyViewModelOnClearOcclusionPressureGraphRequestEvent(object o, EventArgs e)
    {
      ClearBloodPressureGraph();
    }

    /// <summary>
    /// Handle occlusion data update
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private void HandleOcclusionDataUpdated(double[] value)
    {
      if (!enableOcclusionPressureDisplay)
        return;

      try
      {
        UpdateEcgOcclusionPressure(value);
      }
      catch (Exception ex)
      {
        LogException(ex);
      }
    }

    /// <summary>
    /// Occurs when the CryoTherapyViewModel_ReadyStateEvent event is raised.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="sender">The component that raised the event.</param>
    /// <param name="e">Represents the base class for classes that contain user control event data.</param>
    private void CryoTherapyViewModel_ReadyStateEvent(object sender, EventArgs e)
    {
    }

    /// <summary>
    /// Occurs when the CryoTherapyViewModel_InflationStateEvent event is raised.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="sender">The component that raised the event.</param>
    /// <param name="e">Represents the base class for classes that contain user control event data.</param>
    private void CryoTherapyViewModel_InflationStateEvent(object sender, InflationEventArgs e)
    {
      CommonViewModel.Current.IsPlayBackModeDeactivted = true;
      ClearTemperatureChart();
    }

    /// <summary>
    /// Loads the Temperature chart properties.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    protected void LoadTemperatureChart()
    {
      if (Dispatcher.CheckAccess())
      {
        LoadTemperatureChartAction();
      }
      else
      {
        DispatcherBeginInvoke(DispatcherPriority.Normal, LoadTemperatureChartAction);
      }
    }

    private void LoadTemperatureChartAction()
    {
      cryoTherapyViewModel.DataLoading = true;
      int lastVeinisolationDuration = 0;
      Stopwatch timeOutStopWatch = new Stopwatch();
      timeOutStopWatch.Start();

      try
      {
        CommonViewModel.Current.GUIIsRunning = true;
        ChartTemperature?.BeginInit();

        if (cryoTherapyViewModel?.PreviousGenericError == string.Empty)
        {
          ClearTemperatureChart();
        }

        List<AblationDataDetails> ablationDataListItems =
          cryoTherapyViewModel.SingleAblationDatasList?.FindAll(a => a.SystemState != (int)MessageStateId.CAN_ID_STATE_INFLATION);

        if (ablationDataListItems?.Count == 0)
          return;

        cryoTherapyViewModel.SetrequiRedAblationTimeAccordingToState(ablationDataListItems[ablationDataListItems.Count - 1].RequiredAblationTime);
        ChartTemperature.ChartAreas[0].AxisX.Maximum = 240;

        cryoTherapyViewModel.PressureSetPoint = ablationDataListItems[0].PressureSetPoint;

        for (var i = 0; i < ablationDataListItems.Count; i++)
        {
          if (timeOutStopWatch.ElapsedMilliseconds > 5000)
          {
            CommonViewModel.Current.GUIIsRunning = false;
          }

          if (ablationDataListItems[i].ID > ChartTemperature?.ChartAreas[0]?.AxisX?.Maximum)
          {
            ChartTemperature.ChartAreas[0].AxisX.Maximum += 30;
            //Update the interval
            ChartTemperature.ChartAreas[0].AxisX.Interval = 30; // ChartTemperature.ChartAreas[0].AxisX.Maximum / 10;
          }

          SerieTemperature.Points.AddXY(ablationDataListItems[i].ID, ablationDataListItems[i].TC1Reading);
          //Adding Vein Isolation Duration bubble as soon as there is a duration value different than 0.
          try
          {
            if (ablationDataListItems[i].TimeToVeinIsolation != lastVeinisolationDuration)
            {
              SerieVeinIsolationDuration?.Points.Clear();
            }
          }
          catch (Exception ex)
          {
            LogException(ex);
          }

          if (ablationDataListItems[i].TimeToVeinIsolation != 0
            && SerieVeinIsolationDuration?.Points?.Count == 0
            && ablationDataListItems[i].TimeToVeinIsolation != lastVeinisolationDuration)
          {
            lastVeinisolationDuration = ablationDataListItems[i].TimeToVeinIsolation;
            SerieVeinIsolationDuration.Points.AddXY(ablationDataListItems[i].TimeToVeinIsolation, ablationDataListItems[i].TC1Reading);
          }

          if (ablationDataListItems[i].ExceptionStateTime != 0 && SerieAblationFail?.Points?.Count == 0)
          {
            SerieAblationFail?.Points?.AddXY(ablationDataListItems[i].ID, ablationDataListItems[i].TC1Reading);
          }
        }

        //When the last Temperature reading exceeds the min or max threshold, display the series in red.
        if (ablationDataListItems.Count > 0)
        {
          UpdateTemperatureSeriesColor(ablationDataListItems[ablationDataListItems.Count - 1].TC1Reading);
        }

      }
      catch (Exception ex)
      {
        LogException(ex);
      }
      finally
      {
        if (ChartTemperature != null)
        {
          ChartTemperature.Enabled = true;
          ChartTemperature.EndInit();
          timeOutStopWatch.Reset();
          cryoTherapyViewModel.DataLoading = false;
        }
      }
    }

    protected void DispatcherBeginInvoke(DispatcherPriority priority, System.Action action)
    {
      Dispatcher.BeginInvoke(priority, action);
    }


    /// <summary>
    /// Displays the temperature VS Time on a chart.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="time">An integer representing a time value.</param>
    private void DisplayTemperatureVsTime(int time)
    {
      //AppTrace.Log("Displaying TemperatureVsTime ...", LogLevel.Debug, Thread.CurrentThread.ManagedThreadId.ToString(), nameof(CryoTherapy), nameof(DisplayTemperatureVsTime));

      int errorIndex = 0;
      double eTSMinimumTemperature = 1000;
      cryoTherapyViewModel.IsTimeToTargetTemperatureVisible = true;
      cryoTherapyViewModel.IsVeinIsolationDurationVisible = true;

      //Do not get the items in inflation
      List<AblationDataDetails> ablationDatasListItems =
        cryoTherapyViewModel.SingleAblationDatasList.FindAll(a => a.SystemState != (int)MessageStateId.CAN_ID_STATE_INFLATION);

      if (ablationDatasListItems?.Count > 0)
      {
        errorIndex = ablationDatasListItems.Count - 1;
      }
      bool timeFound = false;

      //Consider that fingers are not precise enough to get the exact value all the time.
      if (errorTimeToPixelInterval >= Math.Abs(ablationDatasListItems.Count - time) && (ablationDatasListItems[errorIndex].Error.Contains("Error") || ablationDatasListItems[errorIndex].Error.Contains("problem")))
      {
        string errorWithNewLine = ablationDatasListItems[errorIndex].Error.Replace("+", Environment.NewLine);

        Tuple<long, string, string, string> genericMessage = Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID81, (int)Enumeration.ErrorTypes.GUI);

        MessagePopup messagePopup = new MessagePopup(errorWithNewLine, MessagePopup.MessageType.WarningMessage, MessagePopup.ButtonType.Ok, genericMessage.Item2);
        messagePopup.ShowDialog();
        return;
      }

      if (ablationDatasListItems.Count > 0)
      {
        try
        {
          int veinIsolation = ablationDatasListItems[ablationDatasListItems.Count - 1].TimeToVeinIsolation;

          cryoTherapyViewModel.TimeInAblationMax = ablationDatasListItems[ablationDatasListItems.Count - 1].TimeInAblation;
          cryoTherapyViewModel.ActualAblationTime = cryoTherapyViewModel
            .SingleAblationDatasList
            .Count(item =>
              item.SystemState == (int)MessageStateId.CAN_ID_STATE_TRANSITION ||
              item.SystemState == (int)MessageStateId.CAN_ID_STATE_ABLATION);

          if (veinIsolation > 0)
          {
            cryoTherapyViewModel.TEMPTTI = ablationDatasListItems[veinIsolation - 1].TC1Reading;
          }

          ablationDatasListItems.ForEach(x =>
          {
            //If the EXACT time cannot be found, display the next temperature value
            //this can be the case when the user selected an ECG chart time value -> it is displayed at 40ms vs 1000ms for temperature
            //therefore, an exact temperature time can not always be found.
            if (!timeFound && (x.ID == time || x.ID > time))
            {
              cryoTherapyViewModel.CryoTherapyTime = x.ID;
              cryoTherapyViewModel.TC1Reading = x.TC1Reading;
              cryoTherapyViewModel.TemperatureRate = x.TemperatureRate;
              cryoTherapyViewModel.MaxTemperatureRate = x.MaxTemperatureRate;
              cryoTherapyViewModel.RequiredTargetTemperatureBinding = x.RequiredTargetTemperature;
              cryoTherapyViewModel.BloodDetecorImValue = x.BloodDetecorImValue;

              cryoTherapyViewModel.IsTargetTemperatureReached = x.IsTargetTemperatureReached;
              cryoTherapyViewModel.TimeToTargetTemperature = x.TimeToTargetTemperature;

              cryoTherapyViewModel.IsThawTemperatureReached = x.IsThawTemperatureReached;
              cryoTherapyViewModel.TimeToThawTemperature = x.TimeToThaw;
              cryoTherapyViewModel.ThawTimerToTemperatureBinding = x.ThawTimerToTemperature;

              //Only display the last time to vein isolation
              cryoTherapyViewModel.VeinIsolationDuration = veinIsolation == x.TimeToVeinIsolation ? x.TimeToVeinIsolation : 0;
              cryoTherapyViewModel.FM1Reading = x.FM1Reading;
              cryoTherapyViewModel.PT2Reading = x.PT2Reading;
              cryoTherapyViewModel.LC1Reading = x.LC1Reading;
              cryoTherapyViewModel.CP2Reading = x.CP2Reading;
              cryoTherapyViewModel.CP1Reading = x.CP1Reading;
              //cryoTherapyViewModel.RequiredAblationTime = x.RequiredAblationTime;
              cryoTherapyViewModel.PressureSetPoint = x.PressureSetPoint;
              cryoTherapyViewModel.ISTTISelected = x.ISTTISelected;

              // ECG data update 
              cryoTherapyViewModel.EcgChannel1And2Reading = x.EcgChannel1And2Reading;
              cryoTherapyViewModel.EcgChannel3And4Reading = x.EcgChannel3And4Reading;
              cryoTherapyViewModel.EcgChannel5And6Reading = x.EcgChannel5And6Reading;
              cryoTherapyViewModel.EcgChannel7And8Reading = x.EcgChannel7And8Reading;

              // cryoTherapyViewModel.DiaphragmSensorGain = x.DiaphragmSensorGain;
              cryoTherapyViewModel.EsophagusBindingTemperature = x.EsophagusTemperature;
              cryoTherapyViewModel.EsophagusTemperatureThresholdReached = x.EsophagusTemperatureThresholdReached;
              cryoTherapyViewModel.IsEsophagusTemperatureConditionAlertsMeet = x.EsophagusTemperatureThresholdReached;
              cryoTherapyViewModel.IsAblationTimeVisibale = true;
              cryoTherapyViewModel.IsDiaphragmMovementDetected = x.IsDiaphragmMovementDetected;
              cryoTherapyViewModel.DiaphragmBindingAmplitude = x.DiaphragmAmplitude;
              cryoTherapyViewModel.IsSystemMonitoringDiaphragmAlert = x.IsSystemMonitoringDiaphragmAlert;
              cryoTherapyViewModel.DiaphragmAmplitudeThresholdReached = x.DiaphragmAmplitudeThresholdReached;
              cryoTherapyViewModel.IgnoreMinimumDiaphragmMovementBindingValue = x.IgnoreMinimumDiaphragmMovement;
              cryoTherapyViewModel.IsLowFlowActivated = x.IsLowFlowActivated;
              cryoTherapyViewModel.DASBalloonEnabled = SharedConstants.IsDasBalloonEnabledFromSetPoint(x.PressureSetPoint);

              List<double> sensors = new List<double> { x.EtsSensor13, x.EtsSensor1 , x.EtsSensor2, x.EtsSensor3, x.EtsSensor4,
                            x.EtsSensor5, x.EtsSensor6, x.EtsSensor7, x.EtsSensor8,
                            x.EtsSensor9, x.EtsSensor10, x.EtsSensor11, x.EtsSensor12};

              if (cryoTherapyViewModel.ListOfSesnorsState != null)
              {
                cryoTherapyViewModel.ListOfSesnorsState.Clear();
              }

              cryoTherapyViewModel.ListOfSesnorsState = ETSdataSortingAndStatus.GetMin(sensors, out eTSMinimumTemperature);
              cryoTherapyViewModel.EcgChannel5And6Reading = Math.Min(x.EcgChannel5And6Reading, eTSMinimumTemperature);
              cryoTherapyViewModel.EcgSensorData = sensors;
              cryoTherapyViewModel.EcgChannelStatus = new List<bool>();
              timeFound = true;
            }
          }
          );
        }
        catch (Exception ex)
        {
          LogException(ex);
          //do nothing to recover, just avoid a crash and continue
        }

        //AppTrace.Log("Displayed TemperatureVsTime.", LogLevel.Debug, Thread.CurrentThread.ManagedThreadId.ToString(), nameof(CryoTherapy), nameof(DisplayTemperatureVsTime));
      }
    }

    protected virtual void HandleSensorDataUpdate()
    {
      HandleOcclusionDataUpdated(cryoTherapyViewModel.BloodPressureData);
      return;
    }

    private void ecgTimeElapsed(object s, EventArgs e)
    {
      if (!IsVisible) return;

      HandleSensorDataUpdate();
    }

    #region TTI Control

    private IDisposable _pressingCountObservable;
    private readonly SerialDisposable _mouseDownDisposable = new SerialDisposable();
    private readonly SerialDisposable _mouseUpDisposable = new SerialDisposable();
    private bool? _shortPress;
    private const double ShortPressPeriod = 0.3d;
    private const double StepPeriod = 0.5d;
    private const int LongPressCount = 3;
    private bool isProcessedTTIReset;

    protected void ObserveTTIButton(UIElement ttiButton)
    {
      _mouseDownDisposable.Disposable = Observable.FromEventPattern<MouseButtonEventArgs>(ttiButton, nameof(PreviewMouseDown))
        .Throttle(TimeSpan.FromSeconds(ShortPressPeriod))
        .Where(e => e.EventArgs.ButtonState == MouseButtonState.Pressed)
        .Select(e => e.EventArgs)
        .ObserveOn(TaskPoolScheduler.Default)
        .Subscribe(e =>
        {
          if (cryoTherapyViewModel.VeinIsolationDuration == 0)
          {
            _shortPress = true;
            return;
          }
          _shortPress = null;
          cryoTherapyViewModel.IsTTIPopupShow = true;
          _pressingCountObservable = Observable.Interval(TimeSpan.FromSeconds(StepPeriod))
            .ObserveOn(TaskPoolScheduler.Default)
            .Take(LongPressCount)
            .Subscribe(
              x =>
              {
                cryoTherapyViewModel.TTIResetCount++;
              },
              () =>
              {
                if (cryoTherapyViewModel.TTIResetCount != LongPressCount) return;
                cryoTherapyViewModel.IsTTIPopupShow = false;
                isProcessedTTIReset = true;
                cryoTherapyViewModel.OnVeinCmd(false);
              });
        });

      _mouseUpDisposable.Disposable = Observable.FromEventPattern<MouseButtonEventArgs>(ttiButton, nameof(PreviewMouseUp))
        .ObserveOn(TaskPoolScheduler.Default)
        .Subscribe(_ =>
        {
          _pressingCountObservable?.Dispose();
          if (isProcessedTTIReset)
          {
            isProcessedTTIReset = false;
          }
          else
          {
            cryoTherapyViewModel.IsTTIPopupShow = false;
            _shortPress = cryoTherapyViewModel.TTIResetCount < LongPressCount;
            cryoTherapyViewModel.OnVeinCmd(_shortPress);
          }
        });
    }

    #endregion TTI Control

    #region Shadowing Temperature Graph
    private void ClearTemperatureShadowCharts()
    {
      if (ChartTemperature == null)
      {
        return;
      }

      var seriesCount = ChartTemperature?.Series.Count ?? 0;
      if (seriesCount > _runtimeTemperatureSeries + 1)
      {
        for (var i = 0; i < seriesCount - (_runtimeTemperatureSeries + 1); ++i)
        {
          ChartTemperature?.Series.RemoveAt(0);
        }
      }
    }

    private void AddTemperatureShadowChart(int ablationId, IEnumerable<AblationDataDetails> ablationData)
    {
      if (ChartTemperature == null)
      {
        return;
      }

      var series = new Series($"Ablation#{ablationId}");
      // insert the shadow Series in front of runtime temperature Series,
      // to make sure runtime temperature graphs are on top  
      ChartTemperature.Series.Insert(0, series);
      series.ChartType = SeriesChartType.FastLine;
      series.BorderWidth = 2;
      series.IsVisibleInLegend = false;
      series.Color = Color.Gray;

      foreach (var t in ablationData)
      {
        series.Points.AddXY(t.ID, t.TC1Reading);
      }
    }

    private void HandleUpdateShadowGraphRequest(bool update)
    {
      try
      {
        ClearTemperatureShadowCharts();
        if (update)
        {
          foreach (var data in cryoTherapyViewModel.HistoricalAblationData)
          {
            AddTemperatureShadowChart(data[0].AblationID, data);
          }
        }
      }
      catch (Exception ex)
      {
        LogException(ex);
      }
    }
    #endregion Shadowing Temperature Graph

    protected void HandleVisibilityChanged(bool isVisible)
    {
      if (!IsLoaded)
        return;

      if (isVisible)
      {
        ecgTimer?.Start();
        if (CommonViewModel.Current.AreSensorsInPlayBackMode)
        {
          LoadTemperatureChart();
        }
        else
        {
          ClearCharts();
        }
      }
      else
      {
        if (ecgTimer?.Enabled ?? false)
        {
          ecgTimer.Stop();
        }
      }
    }
  }
}
