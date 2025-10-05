using SmartAblationSystem.Helpers;
using SmartAblationSystem.Models;
using SmartAblationSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Forms.Integration;
using System.Windows.Threading;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using static LogSystem.LogService;
using Color = System.Drawing.Color;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using System.Drawing;

using SeriesCollection = System.Windows.Forms.DataVisualization.Charting.SeriesCollection;
using Series = System.Windows.Forms.DataVisualization.Charting.Series;

namespace SmartAblationSystem.Views
{
  public partial class CryoTherapyNormalView
  {
    private enum ChartIndex
    {
      TemperatureChart = 0,
      // TipPressureChart,
      DiaphragmMovementChart,
      // EcgTemperatureChart,
      OcclusionPressureChart
    }

    private static readonly Color DiaphragmMovementAnnotationLineColor = ColorTranslator.FromHtml("#ff707070");
    private static readonly Color DiaphragmMovementSeriesLineColor = Color.White;

    private const int HIGN_RESOLUTION_DMS_GRAPH_X_AXIS_MAX = 1000;

    private const int NORMAL_DMS_GRAPH_X_AXIS_MAX = 100;

    private Series SerieDiaphragmMovement_A;
    private Series SerieEsophagusTemperature_A;
    private Series SerieEsophagusTemperature_B;

    private const int TIP_PRESSURE_ECG_CHANNEL = 5;
    private const int DIAPHRAGM_MOVEMENT_ECG_CHANNEL = 0;

    private const double TipPressureMinValue = -15;
    private const double TipPressureMaxValue = 15;
    private const double BalloonPressureMaxValue = 10;

    private const double DiaphragmMovementMinValue = -1;
    private const double DiaphragmMovementMaxValue = 1;

    private const double EsophagusTemperatureMinValue = 0;
    private const double EsophagusTemperatureMaxValue = 50;

    private WindowsFormsHost TemperatureHost;
    private WindowsFormsHost BloodPressureHost;

    private WindowsFormsHost TipPressureHost;
    private WindowsFormsHost DiaphragmMovementHost;
    private WindowsFormsHost EsophagusTemperatureHost;

    private BackgroundWorker bw;

    private int _xAxisMaximum = NORMAL_DMS_GRAPH_X_AXIS_MAX;

    private readonly SerialDisposable _highResDetectedDisposable = new SerialDisposable();

    public CryoTherapyNormalView()
    {
      InitializeComponent();
      if (bw == null)
      {
        bw = new BackgroundWorker();
        bw.DoWork += bw_DoWork;
        bw.RunWorkerCompleted += bw_RunWorkerCompleted;
      }
    }


    private void UpdateDmsMovementChartXAxisMax(bool isHighResDms)
    {
      _xAxisMaximum = isHighResDms ? HIGN_RESOLUTION_DMS_GRAPH_X_AXIS_MAX : NORMAL_DMS_GRAPH_X_AXIS_MAX;
      var dmsMovementChart = (Chart)DiaphragmMovementHost?.Child;
      if (dmsMovementChart != null)
      {
        SerieDiaphragmMovement_A.Points.Clear();
        dmsMovementChart.ChartAreas[0].AxisX.Maximum = _xAxisMaximum;
      }
    }

    private SerialDisposable _isPlaybackModeVisibleDisposable = new SerialDisposable();

    protected override void SubscribeEventHandlers()
    {
      if (_isEventSubscribed)
      {
        return;
      }

      base.SubscribeEventHandlers();

      cryoTherapyViewModel.TipOrBalloonPressureSelectionChangedEvent += CryoTherapyViewModel_TipOrBalloonPressureSelectionChangedEvent;
      cryoTherapyViewModel.DiaphragmMovementUnitChangedEvent += CryoTherapyViewModel_DiaphragmMovementUnitChangedEvent;
      cryoTherapyViewModel.DiaphragmSensorGainChangedEvent += CryoTherapyViewModel_DiaphragmSensorGainChangedEvent;

      _highResDetectedDisposable.Disposable = Observable
        .FromEventPattern<PropertyChangedEventArgs>(cryoTherapyViewModel, "PropertyChanged")
        .Where(e => e.EventArgs.PropertyName == nameof(cryoTherapyViewModel.HighResDmsSignalDetected)
                    && !cryoTherapyViewModel.IsTreatmentNumberAndPlayBackVisible)
        .Select(_ => cryoTherapyViewModel.HighResDmsSignalDetected)
        .ObserveOnDispatcher()
        .Subscribe(UpdateDmsMovementChartXAxisMax);

      _isPlaybackModeVisibleDisposable.Disposable = Observable
        .FromEventPattern<PropertyChangedEventArgs>(cryoTherapyViewModel, "PropertyChanged")
        .Where(e => e.EventArgs.PropertyName == nameof(cryoTherapyViewModel.IsTreatmentNumberAndPlayBackVisible))
        .Select(e => cryoTherapyViewModel.IsTreatmentNumberAndPlayBackVisible)
        .ObserveOnDispatcher()
        .Subscribe(HandlePlaybackModeUpdate);

      UpdateDmsMovementChartXAxisMax(cryoTherapyViewModel.HighResDmsSignalDetected);

      ObserveTTIButton(VeinIsolationButton);

      // _isEventSubscribed = true;
    }

    protected override void UnsubscribeEventHandlers()
    {
      base.UnsubscribeEventHandlers();

      _highResDetectedDisposable.Disposable = null;
      _isPlaybackModeVisibleDisposable.Disposable = null;

      cryoTherapyViewModel.TipOrBalloonPressureSelectionChangedEvent -= CryoTherapyViewModel_TipOrBalloonPressureSelectionChangedEvent;
      cryoTherapyViewModel.DiaphragmMovementUnitChangedEvent -= CryoTherapyViewModel_DiaphragmMovementUnitChangedEvent;
      cryoTherapyViewModel.DiaphragmSensorGainChangedEvent -= CryoTherapyViewModel_DiaphragmSensorGainChangedEvent;

      // _isEventSubscribed = false;
    }

    protected override void ClearCharts()
    {
      base.ClearCharts();
      ClearECGCharts();
      cryoTherapyViewModel?.ClearDmsData();
    }

    /// <summary>
    /// Occurs when the bw_DoWork event is raised.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="sender">The component that raised the event.</param>
    /// <param name="e">A DoWorkEventArgs that contains the event data.</param>
    private void bw_DoWork(object sender, DoWorkEventArgs e)
    {
      // we  are resting the can two because the first load can take more time than expected 
      CommonViewModel.Current.ResetCanTwoStopWatch();

      BackgroundWorker worker = sender as BackgroundWorker;
      List<Chart> chartList = new List<Chart>();

      // ChartIndex.TemperatureChart
      // var chart = new Chart() { BackColor = Color.Transparent, Name = "ChartTemperature", Dock = DockStyle.Fill, Enabled = false };
      var chart = InitializeTemperatureGraphic();
      chartList.Add(chart);

      // chart = new Chart() { Name = "TipPressureChart", BackColor = Color.Transparent };
      // InitializeTipPressureGraphic(chart);
      // chartList.Add(chart);

      // ChartIndex.DiaphragmMovementChart

      chart = InitializeDiaphragmMovementGraphic();
      chartList.Add(chart);

      // chart = new Chart() { Name = "EsophagusTemperatureChart", BackColor = Color.Transparent };
      // InitializeEsophagusTemperatureGraphic(chart);
      // chartList.Add(chart);

      // ChartIndex.OcclusionPressureChart

      chart = InitializeBloodPressureGraphic();
      chartList.Add(chart);

      e.Result = chartList;
    }

    /// <summary>
    /// Occurs when the bw_RunWorkerCompleted event is raised.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="sender">The component that raised the event.</param>
    /// <param name="e">A RunWorkerCompletedEventArgs that contains the event data.</param>
    private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      Thickness margin;

      try
      {
        // we  are resting the can two because the first load can take more time than expected 
        CommonViewModel.Current.ResetCanTwoStopWatch();
        List<Chart> chartList = (List<Chart>)e.Result;

        //work is done!
        TemperatureHost = new WindowsFormsHost
        {
          Height = 460,
          Width = 1120,
          Visibility = Visibility.Visible,
          HorizontalAlignment = HorizontalAlignment.Left,
          Margin = new Thickness(-55, 5, 10, 0)
        };
        //Assign the Temperature and Vein isolation series and pressure
        var temperatureChart = (int)ChartIndex.TemperatureChart;
        SetupTemperatureChart(TemperatureHost, chartList[temperatureChart]);
        StackPanelTemperature.Children.Clear();
        StackPanelTemperature.Children.Add(TemperatureHost);

        //Blood pressure
        BloodPressureHost = new WindowsFormsHost
        {
          Height = 460,
          Width = 1120,
          Visibility = Visibility.Visible,
          HorizontalAlignment = HorizontalAlignment.Left,
          Margin = new Thickness(-55, 5, 10, 0)
        };

        //Blood pressure serie
        var occlusionChart = (int)ChartIndex.OcclusionPressureChart;
        SetupOcclusionPressureGraph(BloodPressureHost, chartList[occlusionChart]);

        StackPanelBloodPressure.Children.Clear();
        StackPanelBloodPressure.Children.Add(BloodPressureHost);

        DiaphragmMovementHost = new WindowsFormsHost() { Height = 62, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Stretch, Visibility = Visibility.Visible, Width = 220 };
        margin = DiaphragmMovementHost.Margin;
        DiaphragmMovementHost.Margin = margin;
        var diaphragmMovementChart = (int)ChartIndex.DiaphragmMovementChart;
        SetupDiaphragmMovementGraph(DiaphragmMovementHost, chartList[diaphragmMovementChart]);

        StackPanelDiaphragmMovement.Children.Clear();
        StackPanelDiaphragmMovement.Children.Add(DiaphragmMovementHost);

        // Only initialize Charts once 
        if (bw != null)
        {
          bw.DoWork -= bw_DoWork;
          bw.RunWorkerCompleted -= bw_RunWorkerCompleted;
          bw = null;
        }
      }
      catch (Exception ex)
      {
        LogException(ex);
        Tuple<long, string, string, string> genericMessage78 = Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID78, (int)Enumeration.ErrorTypes.GUI);

        Tuple<long, string, string, string> genericMessage77 = Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID77, (int)Enumeration.ErrorTypes.GUI);

        MessagePopup messagePopup = new MessagePopup(genericMessage77.Item2, MessagePopup.MessageType.WarningMessage, MessagePopup.ButtonType.Ok, genericMessage78.Item2);
        messagePopup.ShowDialog();
      }
    }

    /// <summary>
    /// Occurs when the UserControl_Loaded event is raised.  Start the ECG Reading after the screen has been loaded.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="sender">The component that raised the event.</param>
    /// <param name="e">Represents the base class for classes that contain user control event data.</param>
    private void CryoTherapyNormalView_Loaded(object sender, RoutedEventArgs e)
    {
      cryoTherapyViewModel = DataContext as CryoTherapyViewModel;
      bw?.RunWorkerAsync();
      base.ControlLoaded();
    }

    /// <summary>
    /// Occurs when the UserControl_Unloaded event is raised.
    /// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="sender">The component that raised the event.</param>
    /// <param name="e">Represents the base class for classes that contain user control event data.</param>
    private void UserControl_Unloaded(object sender, RoutedEventArgs e)
    {
      ControlUnloaded();
    }

    private void CryoTherapyNormalView_OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if (IsLoaded)
      {
        var isVisible = (bool)e.NewValue;
        HandleVisibilityChanged(isVisible);
        if (isVisible)
        {
          TemperatureHost?.InvalidateMeasure();
          BloodPressureHost?.InvalidateMeasure();
          DiaphragmMovementHost?.InvalidateMeasure();
          //GridTemperatureAblationTime?.InvalidateMeasure();
        }
      }
    }

    #region override methods
    /// <summary>
    /// override method to Update sensor data on UI
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    protected override void DisplayOtherGraphsVsTime(int time)
    {
    }

    private int countIndex = 0; 
    /// <summary>
    /// override method to Update sensor data on UI
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    protected override void HandleSensorDataUpdate()
    {
      if (IsVisible)
      {
        base.HandleSensorDataUpdate();
        countIndex = (++countIndex)%2;
        if (countIndex == 0)
          HandleDMSDataUpdated(cryoTherapyViewModel.DmsData);
      }
    }

    #endregion override methods

    #region DMS graph feature

    /// <summary>
    /// Occurs when the GetDiaphragmMovementYAxisValue event is raised.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="diaphragmSensorGain">The diaphragm sensor gain value.</param>
    /// <param name="e">The converted sensor gain value in term of Y-Axis value.</param>
    public double GetDiaphragmMovementYAxisValue(int diaphragmSensorGain)
    {
      double sensorGain = 0;
      if (diaphragmSensorGain == 100)
      {
        sensorGain = 1;
      }
      else if (diaphragmSensorGain == 200)
      {
        sensorGain = 1;  //0.5;
      }
      else if (diaphragmSensorGain == 300)
      {
        sensorGain = 1;// 0.25;
      }
      else if (diaphragmSensorGain == 400)
      {
        sensorGain = 1; // 0.125;
      }
      return sensorGain;
    }

    /// <summary>
    /// Occurs when the CryoTherapyViewModel_DiaphragmSensorGainChangedEvent event is raised.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="sender">The component that raised the event.</param>
    /// <param name="e">Represents the base class for classes that contain user control event data.</param>
    private void CryoTherapyViewModel_DiaphragmSensorGainChangedEvent(object sender, EventArgs e)
    {
      if (sender is int && DiaphragmMovementHost != null &&
        DiaphragmMovementHost.Child != null && ((Chart)DiaphragmMovementHost.Child).ChartAreas != null)
      {
        int i = (int)sender;
        var scaleMax = GetDiaphragmMovementYAxisValue(i); 
        ((Chart)DiaphragmMovementHost.Child).ChartAreas[0].AxisY.Maximum = scaleMax;
        ((Chart)DiaphragmMovementHost.Child).ChartAreas[0].AxisY.Minimum = -1 * scaleMax;
      }
    }

    /// <summary>
    /// Occurs when the CryoTherapyViewModel_DiaphragmMovementUnitChangedEvent event is raised.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="sender">The component that raised the event.</param>
    /// <param name="e">Represents the base class for classes that contain user control event data.</param>
    private void CryoTherapyViewModel_DiaphragmMovementUnitChangedEvent(object sender, EventArgs e)
    {
      cryoTherapyViewModel.DiaphragmMovementPercentageOrGReading = cryoTherapyViewModel.DiaphragmMovementPercentageOrGReading;
    }

    /// <summary>
    /// Handle DMS data update.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>    
    private void HandleDMSDataUpdated(double[] dmsData)
    {
      if (SerieDiaphragmMovement_A != null &&
          SensorReadingMananger.AreSensorsConnected)
      {
        double gain = (cryoTherapyViewModel.DiaphragmSensorGain / 100);
        double[] ecgValue = dmsData.Select(d => d * gain).ToArray();

        var maxAvgPaceLevel = cryoTherapyViewModel.HighResDmsSignalDetected
          ? cryoTherapyViewModel.MaximumHRAveragePacingLevel
          : cryoTherapyViewModel.MaximumAveragePacingLevel;

        // Using Dispatcher.BeginInvoke to avoid blocking by UI thread 
        DispatcherBeginInvoke(DispatcherPriority.Normal, () => UpdateDMSGraph(ecgValue, maxAvgPaceLevel * gain));
      }
    }

    /// <summary>
    /// Update DMS Graph.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>    
    private void UpdateDMSGraph(double[] dmsData, double maxAvgPaceLevel)
    {
      foreach (var d in dmsData)
      {
        if (SerieDiaphragmMovement_A.Points.Count >= _xAxisMaximum)
        {
          SerieDiaphragmMovement_A.Points.RemoveAt(0);
        }

        SerieDiaphragmMovement_A.Points.AddY(d);
      }

      //Add dot line at the maximum diaphragm movement
      //Add the annotation a first time
      if (((Chart)DiaphragmMovementHost.Child).Annotations.Count == 0)
      {
        HorizontalLineAnnotation horizontalAnnotation = new HorizontalLineAnnotation();
        horizontalAnnotation.LineDashStyle = ChartDashStyle.Dash;
        horizontalAnnotation.AxisX = ((Chart)DiaphragmMovementHost.Child).ChartAreas[0].AxisX;
        horizontalAnnotation.AxisY = ((Chart)DiaphragmMovementHost.Child).ChartAreas[0].AxisY;
        horizontalAnnotation.IsSizeAlwaysRelative = false;
        horizontalAnnotation.IsInfinitive = true;
        horizontalAnnotation.ClipToChartArea = ((Chart)DiaphragmMovementHost.Child).ChartAreas[0].Name;
        horizontalAnnotation.LineColor = DiaphragmMovementAnnotationLineColor; 
        horizontalAnnotation.LineWidth = 1;
        ((Chart)DiaphragmMovementHost.Child).Annotations.Add(horizontalAnnotation);
      }

      ((Chart)DiaphragmMovementHost.Child).Annotations[0].AnchorY = maxAvgPaceLevel;
    }

    /// <summary>
    /// SetupDiaphragmMovementGraph when chart is ready
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private void SetupDiaphragmMovementGraph(WindowsFormsHost host, Chart chart)
    {
      // ChartIndex.DiaphragmMovementChart
      SerieDiaphragmMovement_A = chart.Series[0];
      SerieDiaphragmMovement_A.Points.Add(0, 0);
      SerieDiaphragmMovement_A.Points[0].IsEmpty = true;  //Display the grids when there is no data
      host.Child = chart;
    }

    /// <summary>
    /// Initializes the DiaphragmMovement properties of chart.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="chart">A Chart representing a common chart.</param>
    protected void InitializeDiaphragmMovementChart(Chart chart)
    {
      try
      {
        if (chart != null)
        {
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
    }

    /// <summary>
    /// Initializes a DiaphragmMovement series.
    /// </summary>
    /// <param name="series">A SeriesCollection representing chart series.</param>
    /// <param name="name">A string representing a chart name</param>
    protected void InitializeDiaphragmMovementSeries(SeriesCollection series, string name)
    {
      try
      {
        if (series != null)
        {
          series.Clear();
          series.Add(name + "_A");
          series[0].ChartType = SeriesChartType.FastLine;
          series[0].BorderWidth = 1;
          series[0].IsVisibleInLegend = false;
          series[0].Color = DiaphragmMovementSeriesLineColor;
        }
      }
      catch (Exception ex)
      {
        LogException(ex);
        throw;
      }
    }

    /// <summary>
    /// Initializes the Diaphragm Movement chart.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="chart">A Chart representing the Diaphragm movement chart.</param>
    private Chart InitializeDiaphragmMovementGraphic()
    {
      //AppTrace.Log("Initializing Diaphragm Movement Graph ...", LogLevel.Debug, Thread.CurrentThread.ManagedThreadId.ToString(), nameof(CryoTherapy), nameof(InitializeDiaphragmMovementGraphic));
      var chartDiaphragmMovement = new Chart() { Name = "DiaphragmMovementChart", BackColor = Color.Transparent };
      InitializeDiaphragmMovementChart(chartDiaphragmMovement);
      this.InitializeDiaphragmMovementSeries(chartDiaphragmMovement.Series, "Diaphragm Movement");

      return chartDiaphragmMovement;
    }

    /// <summary>
    /// Displays ECG data vs Time.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="time">An integer representing the time value.</param>
    private void DisplayECGVsTime(int time)
    {
      cryoTherapyViewModel.IsEsophagusTemperatureVisible = true;
      cryoTherapyViewModel.IsDiaphragmMovementVisible = true;
    }

    /// <summary>
    /// Clears the ECG charts.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private void ClearECGCharts()
    {
      Dispatcher.Invoke(() =>
      {
        try
        {
          SerieDiaphragmMovement_A?.Points?.Clear();
          SerieDiaphragmMovement_A?.Points?.Add(0, 0);
          if (SerieDiaphragmMovement_A != null && SerieDiaphragmMovement_A.Points != null &&
            SerieDiaphragmMovement_A.Points.Count > 0)
          {
            SerieDiaphragmMovement_A.Points[0].IsEmpty = true;
          }
        }
        catch (Exception ex)
        {
          LogException(ex);
          throw;
        }
      });
    }

    private void HandlePlaybackModeUpdate(bool isPlaybackModeVisible)
    {
      UpdateDmsMovementChartXAxisMax(!isPlaybackModeVisible && cryoTherapyViewModel.HighResDmsSignalDetected);
    }

    #endregion DMS graph feature

    #region TipOrBalloonPressureSelection

    /// <summary>
    /// Occurs when the CryoTherapyViewModel_TipOrBalloonPressureSelectionChangedEvent event is raised.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="sender">The component that raised the event.</param>
    /// <param name="e">Represents the base class for classes that contain user control event data.</param>
    private void CryoTherapyViewModel_TipOrBalloonPressureSelectionChangedEvent(object sender, EventArgs e)
    {
      Chart chart = null;

      if (TipPressureHost != null && TipPressureHost.Child != null)
      {
        try
        {
          ////AppTrace.Log("Starting TipOrBalloon Pressure Selection Changed Event ...", LogLevel.Debug, Thread.CurrentThread.ManagedThreadId.ToString(), nameof(CryoTherapy), nameof(CryoTherapyViewModel_TipOrBalloonPressureSelectionChangedEvent));
          chart = (Chart)TipPressureHost.Child;
          chart.BeginInit();
          chart.Series[0].Points.Clear();  //Clear previously displayed values

          //Update the Tip / Balloon pressure chart Y axis scale depending of the selected pressure type
          if (cryoTherapyViewModel.TipPressureSelected)
          {
            chart.ChartAreas[0].AxisY.Maximum = TipPressureMaxValue;
          }
          else
          {
            chart.ChartAreas[0].AxisY.Maximum = BalloonPressureMaxValue;
          }
        }
        catch (Exception ex)
        {
          LogException(ex);
          Tuple<long, string, string, string> genericMessage76 = Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID76, (int)Enumeration.ErrorTypes.GUI);
          Tuple<long, string, string, string> genericMessage75 = Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID75, (int)Enumeration.ErrorTypes.GUI);

          MessagePopup messagePopup = new MessagePopup(genericMessage75.Item2, MessagePopup.MessageType.WarningMessage, MessagePopup.ButtonType.Ok, genericMessage76.Item2);
          messagePopup.ShowDialog();
        }
        finally
        {
          if (chart != null)
          {
            chart.EndInit();
          }
        }
        // //AppTrace.Log("TipOrBalloon Pressure Selection Changed Event Ended.", LogLevel.Debug, Thread.CurrentThread.ManagedThreadId.ToString(), nameof(CryoTherapy), nameof(CryoTherapyViewModel_TipOrBalloonPressureSelectionChangedEvent));
      }
    }

    /// <summary>
    /// Initializes the Tip Pressure graph.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="chart">A Chart representing the Tip Pressure chart.</param>
    private void InitializeTipPressureGraphic(Chart chart)
    {
      try
      {
        //AppTrace.Log("Initializing Tip Pressure Graph...", LogLevel.Debug, Thread.CurrentThread.ManagedThreadId.ToString(), nameof(CryoTherapy), nameof(InitializeTipPressureGraphic));

        InitializeCommonGraphic(chart);
        InitializeCommonSerie(chart.Series, "Tip Pressure");

        chart.ChartAreas[0].AxisX.Minimum = 0;
        chart.ChartAreas[0].AxisX.Maximum = 100;

        chart.ChartAreas[0].AxisX.MajorGrid.Interval = 10;
        chart.ChartAreas[0].AxisX.MinorGrid.LineDashStyle = TEMPERATURE_GRID_DASH_STYLE;

        chart.ChartAreas[0].AxisY.Minimum = TipPressureMinValue;
        chart.ChartAreas[0].AxisY.Maximum = TipPressureMaxValue;
        chart.ChartAreas[0].AxisY.MajorGrid.Interval = 5;

        //AppTrace.Log("Tip Pressure Graph Initialized", LogLevel.Debug, Thread.CurrentThread.ManagedThreadId.ToString(), nameof(CryoTherapy), nameof(InitializeTipPressureGraphic));
      }
      catch (Exception ex)
      {
        LogException(ex);
        throw;
      }
    }

    /// <summary>
    /// Initializes the Esophagus Temperature chart.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="chart">A Chart representing the Esophagus Temperature chart.</param>
    private void InitializeEsophagusTemperatureGraphic(Chart chart)
    {
      //AppTrace.Log("Initializing Esophagus Temperature Graph ...", LogLevel.Debug, Thread.CurrentThread.ManagedThreadId.ToString(), nameof(CryoTherapy), nameof(InitializeEsophagusTemperatureGraphic));

      try
      {
        InitializeCommonGraphic(chart);
        InitializeCommonSerie(chart.Series, "Esophagus Temperature");

        chart.ChartAreas[0].AxisX.Minimum = 0;
        chart.ChartAreas[0].AxisX.Maximum = 100;

        chart.ChartAreas[0].AxisX.MajorGrid.Interval = 10;

        chart.ChartAreas[0].AxisY.Minimum = EsophagusTemperatureMinValue;
        chart.ChartAreas[0].AxisY.Maximum = EsophagusTemperatureMaxValue;
        chart.ChartAreas[0].AxisY.MajorGrid.Interval = 5;
      }
      catch (Exception ex)
      {
        LogException(ex);
      }

      //AppTrace.Log("Esophagus Temperature Graph Initialized", LogLevel.Debug, Thread.CurrentThread.ManagedThreadId.ToString(), nameof(CryoTherapy), nameof(InitializeEsophagusTemperatureGraphic));
    }

    #endregion TipOrBalloonPressureSelection
  }
}
