using FileSerializer;
using SmartAblationSystem.Helpers;
using SmartAblationSystem.Models;
using SmartAblationSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Input;
using static Communication.CanBusMessageDefinition;
using static LogSystem.LogService;

namespace SmartAblationSystem.Views
{
  /// <summary>
  /// Code behind for TreatmentRecords.xaml
  /// </summary>
  public partial class TreatmentRecords : UserControl, IDisposable
  {
    #region Constants
    private const double TEMPERATURE_MIN_VALUE = -80;
    private const double TEMPERATURE_MAX_VALUE = 60;
    private const int ErrorTimeToPixelInterval = 5;
    private const string TemperatureChartName = "ChartTemperature";
    private const string Temperature = "Temperature";
    private const string TemperatureTitleFont = "Arial";
    private const float TemperatureLabelFontSize = 10.0f;
    private const string TemperatureAreaName = "TemperatureArea";

    private readonly ChartDashStyle TEMPERATURE_GRID_DASH_STYLE = ChartDashStyle.Solid;
    private readonly Color GRID_LINES_COLOR = ColorTranslator.FromHtml("#60606e");
    private readonly Color LABEL_COLOR = ColorTranslator.FromHtml("#cec3e6");
    private readonly Color SERIES_COLOR = ColorTranslator.FromHtml("#4aa5e2");
    private readonly Color SERIES_COLOR_ISOLATED_VEIN = ColorTranslator.FromHtml("#9ac466");
    private readonly Color SERIES_COLOR_ABLATION_FAIL = ColorTranslator.FromHtml("#FF0000");
    private readonly Color CHART_AREA_COLOR_1 = ColorTranslator.FromHtml("#212126");
    private readonly Color CHART_AREA_COLOR_2 = ColorTranslator.FromHtml("#00212126");

    #endregion Constants
    #region Fields
    private readonly SerialDisposable _playbackModeEventDisposable = new SerialDisposable();

    private TreatmentRecordsViewModel _treatmentRecordsViewModel;
    private Series _temperatureSeries;
    private Series _veinIsolationDurationSeries;
    private Series _ablationFailSeries;
    //private WindowsFormsHost _temperatureChartHost;
    private Chart _temperatureChart;

    #endregion Fields


    public TreatmentRecords()
    {
      InitializeComponent();
      Setup();
      this.Loaded += TreatmentRecords_Loaded;
      this.Unloaded += TreatmentRecords_Unloaded;
    }

    #region Initialization

    private void Setup()
    {
      _treatmentRecordsViewModel = DataContext as TreatmentRecordsViewModel;
      _treatmentRecordsViewModel.PropertyChanged += _treatmentRecordsViewModel_PropertyChanged;
      _playbackModeEventDisposable.Disposable = Observable
        .FromEventPattern<EventArgs>(_treatmentRecordsViewModel, nameof(TreatmentRecordsViewModel.PlaybackModeEvent))
        .Throttle(TimeSpan.FromMilliseconds(200))
        .ObserveOnDispatcher()
        .Subscribe(_ => TreatmentRecordsViewModel_PlaybackModeEvent());

      StartInitializeChart();
    }

    private void _treatmentRecordsViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      switch(e.PropertyName)
      {
        case "NavigatedProcedureRecords":
          if (_treatmentRecordsViewModel.NavigatedProcedureRecords == null)
          {
            ClearTemperatureChart();
            _treatmentRecordsViewModel?.SingleAblationDatasList.Clear();
          }
          break;
      }
    }

    private async void StartInitializeChart()
    {
      await InitializeChart();
    }

    private async Task InitializeChart()
    {
      _temperatureChart = new Chart
      {
        BackColor = Color.Transparent,
        Name = TemperatureChartName,
        Dock = System.Windows.Forms.DockStyle.Fill,
        Enabled = true
      };
      await Task.Run(() => InitializeTemperatureGraphic(_temperatureChart));
      AddChartToHost(_temperatureChart);
      _treatmentRecordsViewModel?.InitializeProcedureListsAsync();
    }

    private void AddChartToHost(Chart chart)
    {
      _temperatureChart = chart;
      _temperatureSeries = chart.Series[0];
      _temperatureSeries.Points.Add(0, 0);
      _veinIsolationDurationSeries = chart.Series[1];
      _ablationFailSeries = chart.Series[2];
      ChartHost.Child = chart;
    }

    private void InitializeTemperatureGraphic(Chart ChartTemperature)
    {
      ChartTemperature.ChartAreas.Add(TemperatureAreaName);

      // background color
      var chartArea = ChartTemperature.ChartAreas[0];

      chartArea.BackGradientStyle = GradientStyle.TopBottom;
      chartArea.BackColor = this.CHART_AREA_COLOR_1;
      chartArea.BackSecondaryColor = this.CHART_AREA_COLOR_2;
      chartArea.BorderDashStyle = this.TEMPERATURE_GRID_DASH_STYLE;
      chartArea.BorderColor = this.GRID_LINES_COLOR;

      var axisX = chartArea.AxisX;
      axisX.MinorGrid.Enabled = false;
      axisX.Minimum = 0;
      axisX.Maximum = 240;
      axisX.Interval = 30;
      axisX.MajorGrid.Enabled = true;
      axisX.IsStartedFromZero = true;
      axisX.MajorGrid.LineColor = this.GRID_LINES_COLOR;
      axisX.MajorGrid.LineDashStyle = this.TEMPERATURE_GRID_DASH_STYLE;
      axisX.LineColor = this.GRID_LINES_COLOR;
      axisX.LineDashStyle = this.TEMPERATURE_GRID_DASH_STYLE;
      axisX.LabelStyle.ForeColor = LABEL_COLOR;
      axisX.LabelStyle.Font = new Font(TemperatureTitleFont, TemperatureLabelFontSize, System.Drawing.FontStyle.Regular);
      axisX.MajorTickMark.Enabled = false;

      var axisY = chartArea.AxisY;
      axisY.MajorGrid.LineColor = this.GRID_LINES_COLOR;
      axisY.MajorGrid.LineDashStyle = this.TEMPERATURE_GRID_DASH_STYLE;
      axisY.LineColor = this.GRID_LINES_COLOR;
      axisY.LabelStyle.ForeColor = LABEL_COLOR;
      axisY.LabelStyle.Font = new Font(TemperatureTitleFont, TemperatureLabelFontSize, System.Drawing.FontStyle.Regular);
      axisY.Minimum = TEMPERATURE_MIN_VALUE;
      axisY.Maximum = TEMPERATURE_MAX_VALUE;
      axisY.Interval = 20;
      axisY.MajorTickMark.Enabled = false;
      //To make the X-axis appear on Y axis 0.
      axisX.Crossing = 0;
      axisY.Crossing = 0;

      //// Set Antialiasing mode
      ////this can be set lower if there are any performance issues!
      ChartTemperature.AntiAliasing = AntiAliasingStyles.None;
      ChartTemperature.TextAntiAliasingQuality = TextAntiAliasingQuality.SystemDefault;

      ChartTemperature.Series.Clear();
      ChartTemperature.Series.Add(Temperature);
      var tempSeries = ChartTemperature.Series[0];
      tempSeries.ChartType = SeriesChartType.FastLine;
      tempSeries.BorderWidth = 2;
      tempSeries.IsVisibleInLegend = false;
      tempSeries.Color = this.SERIES_COLOR;

      //Add isolation vein duration series
      ChartTemperature.Series.Add("VeinIsolationDuration");
      var veinIsolationSeries = ChartTemperature.Series[1];
      veinIsolationSeries.ChartType = SeriesChartType.Point;
      veinIsolationSeries.MarkerStyle = MarkerStyle.Circle;
      veinIsolationSeries.MarkerSize = 9;
      veinIsolationSeries.MarkerBorderWidth = 3;
      veinIsolationSeries.IsVisibleInLegend = false;
      veinIsolationSeries.MarkerColor = ColorTranslator.FromHtml("#FF2A2A32"); 
      veinIsolationSeries.MarkerBorderColor = this.SERIES_COLOR_ISOLATED_VEIN;

      ChartTemperature.Series.Add("AblationFail");
      var ablationFailSeries = ChartTemperature.Series[2];
      ablationFailSeries.ChartType = SeriesChartType.Bubble;
      ablationFailSeries.BorderWidth = 1;
      ablationFailSeries.IsVisibleInLegend = false;
      ablationFailSeries.Color = this.SERIES_COLOR_ABLATION_FAIL;
      ablationFailSeries.MarkerStyle = MarkerStyle.Triangle;
      ablationFailSeries["BubbleMinSize"] = "5";
      ablationFailSeries["BubbleMaxSize"] = "5";

      ChartTemperature.MouseClick += ChartTemperature_MouseClick;
            UpdateLabelAxisX(ChartTemperature);
    }

    private void TreatmentRecordsViewModel_PlaybackModeEvent()
    {
      LoadTemperatureChart();
    }

    private void SetTemperatureChartInterval(int xAxisTimeValue)
    {
      if(!(_temperatureChart?.ChartAreas?.Count > 0) || _temperatureChart.ChartAreas[0].AxisX == null)
      {
        _temperatureChart.ChartAreas[0].AxisX.Interval = 5;
      }
      else
      {
        if(xAxisTimeValue <= 30)
        {
          _temperatureChart.ChartAreas[0].AxisX.Interval = 5;
        }
        else if(xAxisTimeValue <= 60)
        {
          _temperatureChart.ChartAreas[0].AxisX.Interval = 10;
        }
        else if(xAxisTimeValue <= 150)
        {
          _temperatureChart.ChartAreas[0].AxisX.Interval = 20;
        }
        else if(xAxisTimeValue <= 240)
        {
          _temperatureChart.ChartAreas[0].AxisX.Interval = 30;
        }
        else if(xAxisTimeValue <= 480)
        {
          _temperatureChart.ChartAreas[0].AxisX.Interval = 50;
        }
        else
        {
          _temperatureChart.ChartAreas[0].AxisX.Interval = Math.Ceiling(_temperatureChart.ChartAreas[0].AxisX.Maximum / 10);
        }
      }
    }

    private void ChartTemperature_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
    {
      Chart ChartTemperature = (Chart)sender;
      if(ChartTemperature == null) return;
      var screenPosition = e.Location;
      var results = ChartTemperature.HitTest(screenPosition.X, screenPosition.Y, false, ChartElementType.PlottingArea);
      if (btnBMIInfo?.ToolTip != null)
      {
        ((ToolTip)btnBMIInfo.ToolTip).IsOpen = false;
      }
      foreach(var result in results)
      {
        if(result.ChartElementType == ChartElementType.PlottingArea)
        {
          var timeHighResolution = result.ChartArea.AxisX.PixelPositionToValue(screenPosition.X);
          int time = (int)(Math.Round(result.ChartArea.AxisX.PixelPositionToValue(screenPosition.X)));

          //Give the user some feedback by displaying a vertical line where the chart has been touched/clicked
          ChartTemperature.Annotations.Clear();
          var verticalLine = GetVerticalAnnotationLine(time, ChartTemperature.ChartAreas[0]);
          if(verticalLine != null)
          {
            ChartTemperature.Annotations.Add(verticalLine);
          }
          DisplayTemperatureVsTime(time);
        }
      }
    }

    private VerticalLineAnnotation GetVerticalAnnotationLine(int xValue, ChartArea chartArea)
    {
      return new VerticalLineAnnotation
      {
        AxisX = chartArea.AxisX,
        AllowMoving = true,
        IsInfinitive = true,
        Name = "myVerticalLine",
        LineColor = ColorTranslator.FromHtml("#00afef"),
        LineWidth = 2,
        X = xValue,
        ClipToChartArea = chartArea.Name
      };
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


        private void LoadTemperatureChart()
    {
      ClearTemperatureChart();
      double previousYValue = double.MinValue;
      bool isContinuousLine = false;

      int lastVeinIsolationDuration_ = 0;

      List<AblationDataDetails> ablationDataListItems = _treatmentRecordsViewModel.SingleAblationDatasList;

      for(var i = 0; i < ablationDataListItems.Count; i++)
      {
        if(Math.Abs(ablationDataListItems[i].TC1Reading - previousYValue) > 0.001)
        {
          if(isContinuousLine)
          {
            _temperatureSeries?.Points.AddXY(ablationDataListItems[i - 1].ID, ablationDataListItems[i - 1].TC1Reading);
            isContinuousLine = false;
          }

          _temperatureSeries?.Points.AddXY(ablationDataListItems[i].ID, ablationDataListItems[i].TC1Reading);
        }
        else
        {
          if(i == ablationDataListItems.Count - 1)
          {
            _temperatureSeries?.Points.AddXY(ablationDataListItems[i].ID, ablationDataListItems[i].TC1Reading);
          }

          isContinuousLine = true;
        }
        previousYValue = ablationDataListItems[i].TC1Reading;

        try
        {
          if(ablationDataListItems[i].TimeToVeinIsolation != lastVeinIsolationDuration_)
          {
            _veinIsolationDurationSeries?.Points.Clear();
          }
        }
        catch(Exception ex)
        {
          LogException(ex);
        }

        if(ablationDataListItems[i].TimeToVeinIsolation != 0 &&
           _veinIsolationDurationSeries?.Points.Count == 0 &&
           ablationDataListItems[i].TimeToVeinIsolation != lastVeinIsolationDuration_)
        {
          lastVeinIsolationDuration_ = ablationDataListItems[i].TimeToVeinIsolation;
          _veinIsolationDurationSeries?.Points.AddXY(ablationDataListItems[i].TimeToVeinIsolation, ablationDataListItems[i].TC1Reading);
        }

        //Adding Time Exception triangle on a new series.
        if(ablationDataListItems[i].ExceptionStateTime != 0 && _ablationFailSeries?.Points.Count == 0)
        {
          _ablationFailSeries?.Points.AddXY(ablationDataListItems[i].ID, ablationDataListItems[i].TC1Reading);
        }
      }

      //Select the first X-axis value and update related values
      _temperatureChart?.Annotations.Clear();
      if(_temperatureSeries?.Points.Count > 1)
      {
        var firstTimeFrame = 0;
        var verticalLine = GetVerticalAnnotationLine(firstTimeFrame, _temperatureChart.ChartAreas[0]);
        if(verticalLine != null)
        {
          _temperatureChart?.Annotations.Clear();
          _temperatureChart?.Annotations.Add(verticalLine);
          DisplayTemperatureVsTime(firstTimeFrame, false);
        }
      }

      if(ablationDataListItems.Count > 30)
      {
        _temperatureChart.ChartAreas[0].AxisX.Maximum = ablationDataListItems.Count + 10; //give some place for user's finger
        SetTemperatureChartInterval(ablationDataListItems.Count);
      }
      else
      {
        _temperatureChart.ChartAreas[0].AxisX.Maximum = 30;
        SetTemperatureChartInterval(30);
      }
      UpdateLabelAxisX(_temperatureChart);
    }

    private void DisplayTemperatureVsTime(int time, bool screenTouched = true)
    {
      int errorIndex = 0;
      var ablationDataListItems_ = _treatmentRecordsViewModel?.SingleAblationDatasList?.FindAll(a => a.SystemState != (int)MessageStateId.CAN_ID_STATE_INFLATION);

      if(ablationDataListItems_?.Count > 0)
      {
        errorIndex = ablationDataListItems_.Count - 1;
      }
      else
      {
        return;
      }
      bool timeFound = false;
      var timeInAblationState_ = ablationDataListItems_
          .Count(s => s.SystemState == (int)MessageStateId.CAN_ID_STATE_TRANSITION || s.SystemState == (int)MessageStateId.CAN_ID_STATE_ABLATION);

      if(ablationDataListItems_.Count <= 0)
      {
        return;
      }

      try
      {
        int veinIsolation = ablationDataListItems_[ablationDataListItems_.Count - 1].TimeToVeinIsolation;

        ablationDataListItems_.ForEach(DetailItem_ =>
        {
          //If the EXACT time cannot be found, display the next temperature value
          //this can be the case when the user selected an ECG chart time value -> it is displayed at 40ms vs 1000ms for temperature
          //therefore, an exact temperature time can not always be found.
          if(timeFound || (DetailItem_.ID != time && DetailItem_.ID <= time)) return;
          _treatmentRecordsViewModel.CryoTherapyTime = DetailItem_.ID;
          _treatmentRecordsViewModel.TC1Reading = DetailItem_.TC1Reading;
          _treatmentRecordsViewModel.TemperatureRate = DetailItem_.TemperatureRate;
          _treatmentRecordsViewModel.MaxTemperatureRate = DetailItem_.MaxTemperatureRate;
          _treatmentRecordsViewModel.RequiredTargetTemperature = DetailItem_.RequiredTargetTemperature;
          _treatmentRecordsViewModel.SystemState = (MessageStateId) DetailItem_.SystemState;
          // FM1Reading
          _treatmentRecordsViewModel.FM1Reading = DetailItem_.FM1Reading;
          _treatmentRecordsViewModel.PT2Reading = DetailItem_.PT2Reading;
          _treatmentRecordsViewModel.CP2Reading = DetailItem_.CP2Reading;
          _treatmentRecordsViewModel.CP1Reading = DetailItem_.CP1Reading;

          _treatmentRecordsViewModel.IsTargetTemperatureReached = DetailItem_.IsTargetTemperatureReached;
          _treatmentRecordsViewModel.TimeToTargetTemperature = DetailItem_.TimeToTargetTemperature;

          //Only display the last time to vein isolation
          _treatmentRecordsViewModel.VeinIsolationDuration = veinIsolation == DetailItem_.TimeToVeinIsolation ? DetailItem_.TimeToVeinIsolation : 0;

          if(DetailItem_.TimeToVeinIsolation == 0)
          {
            _treatmentRecordsViewModel.TemperatureAtTTI = 0;
            _treatmentRecordsViewModel.TimeSinceTTI = 0;
          }
          else
          {
            var selectedRecord_ = ablationDataListItems_.FirstOrDefault(record => record.TimeToVeinIsolation != 0);
            if(selectedRecord_ != null)
            {
              _treatmentRecordsViewModel.TemperatureAtTTI = (int)selectedRecord_.TC1Reading;
              var ttiPoint_ = selectedRecord_.ID;
              var selectedPoint_ = DetailItem_.ID;
              _treatmentRecordsViewModel.TimeSinceTTI = selectedPoint_ > timeInAblationState_
                          ? timeInAblationState_ - ttiPoint_
                          : selectedPoint_ - ttiPoint_;
            }
          }

          _treatmentRecordsViewModel.IsThawTemperatureReached = DetailItem_.IsThawTemperatureReached;
          _treatmentRecordsViewModel.TimeToThawTemperature = DetailItem_.IsThawTemperatureReached ? DetailItem_.TimeToThaw : 0;
          _treatmentRecordsViewModel.ThawTimerToTemperature = DetailItem_.ThawTimerToTemperature;
          _treatmentRecordsViewModel.TimeToTargetTemperature = DetailItem_.TimeToTargetTemperature;

          timeFound = true;

          _treatmentRecordsViewModel.PWMBAL = DetailItem_.PWMBAL;
          _treatmentRecordsViewModel.PWMINJ = DetailItem_.PWMINJ;
          _treatmentRecordsViewModel.PT1Reading = DetailItem_.PT1Reading;
          _treatmentRecordsViewModel.PT3Reading = DetailItem_.PT3Reading;
          _treatmentRecordsViewModel.PT4Reading = DetailItem_.PT4Reading;
          _treatmentRecordsViewModel.PT5Reading = DetailItem_.PT5Reading;
          _treatmentRecordsViewModel.TS1Reading = DetailItem_.TS1Reading;
          _treatmentRecordsViewModel.LC1Reading = DetailItem_.LC1Reading;
          _treatmentRecordsViewModel.BloodDetecorImValue = DetailItem_.BloodDetecorImValue;

          // ECG data
          _treatmentRecordsViewModel.EcgChannel1And2Reading = DetailItem_.EcgChannel1And2Reading;
          _treatmentRecordsViewModel.EcgChannel3And4Reading = DetailItem_.EcgChannel3And4Reading;
          _treatmentRecordsViewModel.EcgChannel5And6Reading = DetailItem_.EcgChannel5And6Reading;
          _treatmentRecordsViewModel.EcgChannel7And8Reading = DetailItem_.EcgChannel7And8Reading;

          _treatmentRecordsViewModel.EsophagusTemperature = DetailItem_.EsophagusTemperature;
          _treatmentRecordsViewModel.EsophagusTemperatureThresholdReached = DetailItem_.EsophagusTemperatureThresholdReached;
          _treatmentRecordsViewModel.IsDiaphragmMovementDetected = DetailItem_.IsDiaphragmMovementDetected;
          _treatmentRecordsViewModel.DiaphragmAmplitude = DetailItem_.DiaphragmAmplitude;
          _treatmentRecordsViewModel.DiaphragmAmplitudeThresholdReached = DetailItem_.DiaphragmAmplitudeThresholdReached;
        }
        );
      }
      catch(Exception ex)
      {
        LogException(ex);
      }

      if(ErrorTimeToPixelInterval >= Math.Abs(ablationDataListItems_.Count - time) && 
         (ablationDataListItems_[errorIndex].Error.Contains("Error")  || ablationDataListItems_[errorIndex].Error.Contains("problem")))
      {
        string errorWithNewLine = ablationDataListItems_[errorIndex].Error.Replace("+", Environment.NewLine);
        var genericMessage = Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID81, (int)Enumeration.ErrorTypes.GUI);
        //Display error message when the user touched the chart.  Do not show the error when the chart has just load (when an error is detected in the first seconds).
        if(screenTouched)
        {
          MessagePopup messagePopup = new MessagePopup(errorWithNewLine, MessagePopup.MessageType.ErrorMessage, MessagePopup.ButtonType.Ok, genericMessage.Item2);
          messagePopup.ShowDialog();
        }
      }
    }

    private void ClearTemperatureChart()
    {
      _temperatureChart?.Annotations.Clear();
      //
      if(_temperatureSeries?.Points != null && _temperatureSeries.Points.Count != 0)
      {
        _temperatureSeries.Points.Clear();
        _temperatureSeries.Points.Add(0, 0);
        _temperatureSeries.Points[0].IsEmpty = true;  //Display the grids when there is no data
      }

      //Remove the vein isolation duration bubble
      _veinIsolationDurationSeries?.Points?.Clear();

      //Remove the Exception Triangle
      _ablationFailSeries?.Points?.Clear();
    }

    #endregion Initialization

    #region Event Handlers

    private void ShowToolTip_Click(object sender, RoutedEventArgs e)
    {
      var toolTip = new ToolTip();
      if(!(btnBMIInfo.ToolTip is ToolTip _tip)) return;
      toolTip = _tip;
      toolTip.IsOpen = true;
    }

    #endregion Event Handlers

    #region Loaded and Unloaded

    private void TreatmentRecords_Loaded(object sender, RoutedEventArgs e)
    {
      SensorReadingMananger.AllowPlayback = true;
      SensorReadingMananger.DisconnectSensors();
      ProcedureLogModel.IsUserAccessRecord = true;
      _treatmentRecordsViewModel?.RefreshPropertyChanged();
    }

    private void TreatmentRecords_Unloaded(object sender, RoutedEventArgs e)
    {
      SensorReadingMananger.AllowPlayback = false;
      SensorReadingMananger.ConnectSensors();

      if(_treatmentRecordsViewModel.NavigatedProcedureRecords != null)
      {
        _treatmentRecordsViewModel.PreviuosPhysicianId = _treatmentRecordsViewModel.NavigatedProcedureRecords.Procedure.PhysicianID;
      }
    }

    #endregion Loaded and Unloaded

    #region IDisposable Implementation

    private bool disposedValue;

    protected virtual void Dispose(bool disposing)
    {
      if(!disposedValue)
      {
        if(disposing)
        {
          // (1) Unsubscribe events
          _playbackModeEventDisposable.Disposable?.Dispose();
          _temperatureChart.MouseClick -= ChartTemperature_MouseClick;

          // (2) dispose managed state (managed objects)
          _temperatureSeries.Dispose();
          _veinIsolationDurationSeries.Dispose();
          _temperatureChart.Dispose();
        }

        // (3) free unmanaged resources (unmanaged objects) and override finalizer
        // (4) set large fields to null
        disposedValue = true;
      }
    }

    ~TreatmentRecords()
    {
      // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
      Dispose(disposing: false);
    }

    public void Dispose()
    {
      // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
      Dispose(disposing: true);
      GC.SuppressFinalize(this);
    }

    #endregion IDisposable Implementation

    private void TreatmentRecords_OnManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
    {
      e.Handled = true;
    }
  }
}