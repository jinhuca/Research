using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Forms.Integration;
using System.Windows.Threading;

namespace CustomControls.UserControls
{
  /// <summary>
  /// Interaction logic for ETSTemperatureGraph.xaml
  /// </summary>
  public partial class ETSTemperatureGraph  
  {
    public enum ChartDisplayMode
    {
      None, 
      Realtime, 
      Playback
    }

    private static readonly Color GRID_LINES_COLOR = ColorTranslator.FromHtml("#A7A7A7");
    private static readonly Color SERIES_COLOR = Color.White;
    private static readonly Color GRID_Y_LINE = ColorTranslator.FromHtml("#1C1C22");

    private static readonly int _axisXExtension = 10;
    private static readonly int _defaultMaxAxisXCount = 240;
    private static readonly int _axisXIncreamental = 30;
    private static readonly int _axisXLimit = 300;
    private static readonly double _minTemp = 0d; 
    private static readonly double _maxTemp = 50d; 

    private int _currentMaxAxisXCount = _defaultMaxAxisXCount; 

    private static readonly int _maxAxisYCount = 40;
    private WindowsFormsHost _ETSGraphHost;
    private Chart _temperatureChart;
    private Series _temperatureSeries;

    private readonly ISubject<double> _temperatureSubject = new BehaviorSubject<double>(0d);
    private readonly SerialDisposable _temperatureDisposable = new SerialDisposable();

    #region DependencyProperty Registrations
    
    public static readonly DependencyProperty TemperatureProperty = DependencyProperty.Register(nameof(Temperature),
      typeof(double), typeof(ETSTemperatureGraph),
      new PropertyMetadata(0d, OnTemperatureChanged, HandleCoerceUpdate));

    public static readonly DependencyProperty GraphModeProperty = DependencyProperty.Register(nameof(GraphMode), 
      typeof(ChartDisplayMode), typeof(ETSTemperatureGraph),
      new PropertyMetadata(ChartDisplayMode.None, OnGraphModeChanged, GraphModeCoerceValueCallback));


    public static readonly DependencyProperty PlaybackDataProperty = DependencyProperty.Register(nameof(PlaybackData), 
      typeof(List<double>), typeof(ETSTemperatureGraph),
      new PropertyMetadata(null, OnPlaybackDataChanged, PlaybackDataCoerceValueCallback));

    #endregion DependencyProperty Registrations

    public ETSTemperatureGraph()
    {
      InitializeComponent();
      InitializeGraphHost();
    }

    #region DependencyProperty Definitions

    public double Temperature
    {
      get => (double)GetValue(TemperatureProperty);
      set => SetValue(TemperatureProperty, value);
    }

    public ChartDisplayMode GraphMode
    {
      get => (ChartDisplayMode)GetValue(GraphModeProperty);
      set => SetValue(GraphModeProperty, value);
    }

    public List<double> PlaybackData
    {
      get => (List<double>) GetValue(PlaybackDataProperty);
      set => SetValue(PlaybackDataProperty, value);
    }

    #endregion DependencyProperty Definitions

    private void InitializeGraphHost()
    {
      // 296,124
      _ETSGraphHost = new WindowsFormsHost { Height = 145, Width = 328, Visibility=Visibility.Visible };
      _temperatureChart = SetupEtsTemperatureChart();
      _ETSGraphHost.Child = _temperatureChart;
      _temperatureChart.Series[0].Points.Add(0, 0);
      _temperatureChart.Series[0].Points[0].IsEmpty = true;

      _etsTemperatureGraphStackPanel.Children.Clear();
      _etsTemperatureGraphStackPanel.Children.Add( _ETSGraphHost );
    }

    private Chart SetupEtsTemperatureChart()
    {
      var chartTemperature = new Chart { BackColor = Color.Transparent, Name = "ChartTemperature", Dock = DockStyle.Fill, Enabled = false };
      if (chartTemperature?.ChartAreas != null)
      {
        chartTemperature.ChartAreas.Add("TemperatureArea");
        chartTemperature.ChartAreas[0].BackColor = Color.Transparent;
        chartTemperature.ChartAreas[0].AxisX.Minimum = 0;
        chartTemperature.ChartAreas[0].AxisX.Maximum = _defaultMaxAxisXCount + _axisXExtension;
        _currentMaxAxisXCount = _defaultMaxAxisXCount;
        chartTemperature.ChartAreas[0].AxisX.Interval = 30;
        chartTemperature.ChartAreas[0].AxisX.MajorGrid.Enabled = true;
        chartTemperature.ChartAreas[0].AxisX.IsStartedFromZero = true;

        chartTemperature.ChartAreas[0].AxisX.MinorGrid.Enabled = false;
        chartTemperature.ChartAreas[0].AxisX.MajorGrid.LineColor = GRID_Y_LINE;
        chartTemperature.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Solid;
        chartTemperature.ChartAreas[0].AxisX.LineColor = GRID_LINES_COLOR;
        chartTemperature.ChartAreas[0].AxisX.LineDashStyle = ChartDashStyle.Solid;
        chartTemperature.ChartAreas[0].AxisX.LabelStyle.ForeColor = GRID_LINES_COLOR;
        chartTemperature.ChartAreas[0].AxisX.LabelStyle.Font = new Font("Noto Sans Myanmar", 6.0f, System.Drawing.FontStyle.Regular);
        chartTemperature.ChartAreas[0].AxisX.LabelStyle.Format = "F0";

        chartTemperature.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
        chartTemperature.ChartAreas[0].AxisY.MajorTickMark.LineColor = GRID_LINES_COLOR;
        chartTemperature.ChartAreas[0].AxisY.MinorGrid.Enabled = false;
        chartTemperature.ChartAreas[0].AxisY.LineColor = GRID_LINES_COLOR;
        chartTemperature.ChartAreas[0].AxisY.LabelStyle.ForeColor = GRID_LINES_COLOR;
        chartTemperature.ChartAreas[0].AxisY.LabelStyle.Font = new Font("Noto Sans Myanmar", 6.0f, System.Drawing.FontStyle.Regular);
        chartTemperature.ChartAreas[0].AxisY.LabelStyle.Format = "F0";
        chartTemperature.ChartAreas[0].AxisY.Minimum = 0;
        chartTemperature.ChartAreas[0].AxisY.Maximum = _maxAxisYCount + 5;
        chartTemperature.ChartAreas[0].AxisY.Interval = 10;

        //To make the X-axis appear on Y axis 0.
        chartTemperature.ChartAreas[0].AxisX.Crossing = 0;
        chartTemperature.ChartAreas[0].AxisY.Crossing = 0;
      }

      if (chartTemperature?.Series != null)
      {
        chartTemperature.Series.Clear();

        _temperatureSeries = chartTemperature.Series.Add("Temperature");
        _temperatureSeries.ChartType = SeriesChartType.FastLine;
        _temperatureSeries.BorderWidth = 2;
        _temperatureSeries.IsVisibleInLegend = false;
        _temperatureSeries.Color = SERIES_COLOR;
      }

      if (chartTemperature != null)
      {
        chartTemperature.AntiAliasing = AntiAliasingStyles.None;
        chartTemperature.TextAntiAliasingQuality = TextAntiAliasingQuality.Normal;
      }

      return chartTemperature;
    }

    private static void OnTemperatureChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
    {
      var control = dp as ETSTemperatureGraph;
      if (control == null) return;

      var temperature = (double)e.NewValue;
      control.PublishTemperatureData(temperature);
    }

    private static object HandleCoerceUpdate(DependencyObject dp, object value)
    {
      var control = dp as ETSTemperatureGraph;
      if (control == null) return value; 

      var temperature = (double)value;
      if (temperature == control.Temperature)
      {
        control.PublishTemperatureData(temperature);
      }

      return value;
    }

    private void PublishTemperatureData(double temperature)
    {
      _temperatureSubject.OnNext(temperature);
    }

    private void UpdateGraphSetting(ChartDisplayMode graphMode)
    {
      switch (graphMode)
      {
        case ChartDisplayMode.Realtime:
          ResetGraph(); 
          _temperatureDisposable.Disposable = _temperatureSubject
            .Window(TimeSpan.FromSeconds(1), TaskPoolScheduler.Default)
          .Subscribe(tb =>
            {
              try
              {
                tb.Average().Subscribe(t => UpdateTemperatureGraph(RangeTemperatureValue(t)), _ => { }, () => { });
              }
              catch (Exception ex)
              {
                UpdateTemperatureGraph(0);
              }
            });
          break;
      
      case ChartDisplayMode.Playback :
          _temperatureDisposable.Disposable = null; 
          // Update Temperature Graph with Playback data 
          // ResetGraph(); 
          if (PlaybackData != null && PlaybackData.Any())
          {
            UpdateTemperatureGraph(PlaybackData);
          }
          break;
      case ChartDisplayMode.None:
        _temperatureDisposable.Disposable = null; 
        ResetGraph(); 
        break;
      }
    }

    private void ResetGraph()
    {
      DispatcherBeginInvoke(() =>
        {
          _currentMaxAxisXCount = _defaultMaxAxisXCount;
          _temperatureChart.ChartAreas[0].AxisX.Maximum = _defaultMaxAxisXCount + _axisXExtension;
          _temperatureSeries.Points.Clear();
          _temperatureSeries.Points.AddXY(0, 0);
          _temperatureSeries.Points[0].IsEmpty = true;
        });
    }

    private void UpdateTemperatureGraph(double temperature)
    {
      DispatcherBeginInvoke(
        () =>
          {
            var points = _temperatureSeries.Points;
            if ( points.Count >= _currentMaxAxisXCount && points.Count < _axisXLimit)
            {
              _currentMaxAxisXCount += _axisXIncreamental;
              _temperatureChart.ChartAreas[0].AxisX.Maximum = _currentMaxAxisXCount + _axisXExtension;
            }

            if (points.Count == _currentMaxAxisXCount)
            {
              points.RemoveAt(0);
              foreach (var pt in points)
              {
                pt.XValue--;
              }
            }

            points.AddXY(points.Count - 1, temperature);
          });
    }

    private static void OnGraphModeChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
    {
      var control = dp as ETSTemperatureGraph;
      
      if (control == null)
        return;

      control.UpdateGraphSetting((ChartDisplayMode)e.NewValue);
    }

    private static object GraphModeCoerceValueCallback(DependencyObject dp, object value)
    {
      var control = dp as ETSTemperatureGraph; 
      if (control == null) 
        return value;

      if (value != null && control.GraphMode.Equals(value) && control.GraphMode != ChartDisplayMode.Realtime)
      {
        control.UpdateGraphSetting((ChartDisplayMode)value);
      } 

      return value;
    }

    private static void OnPlaybackDataChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
    {
      var control = dp as ETSTemperatureGraph; 
      if (control == null) return;

      var data = e.NewValue as List<double>;
      if (control.GraphMode == ChartDisplayMode.Playback)
        control?.UpdateTemperatureGraph(data); 
    }

    protected static object PlaybackDataCoerceValueCallback(DependencyObject dp, object value)
    {
      var control = dp as ETSTemperatureGraph; 
      if (control == null) 
        return value;

      if (control.GraphMode == ChartDisplayMode.Playback && control.PlaybackData != null && value != null && control.PlaybackData.Equals(value))
      {
        control.UpdateTemperatureGraph(value as List<double>);
      }

      return value; 
    }

    private void UpdateTemperatureGraph(List<double> temperatureData)
    {
      // Don't update if null or empty 
      if (!temperatureData?.Any() ?? false) 
        return;

      var pointCount = temperatureData.Count;
      var inc = (int)Math.Ceiling((double)(pointCount - _defaultMaxAxisXCount) / _axisXIncreamental);
      _currentMaxAxisXCount = inc > 0 ? _defaultMaxAxisXCount + inc * _axisXIncreamental : _defaultMaxAxisXCount;

      DispatcherBeginInvoke(() =>
      {
        _temperatureChart.ChartAreas[0].AxisX.Maximum =  _currentMaxAxisXCount + _axisXExtension;

        var maxGraphPoints = Math.Min(pointCount, _currentMaxAxisXCount); 
        var points = _temperatureSeries.Points;
        points.Clear();

        for (int i = 0; i < maxGraphPoints; i++)
        {
          points.AddXY(i, RangeTemperatureValue(temperatureData[i]));
        }
      });
    }

    private void DispatcherBeginInvoke(Action action)
    {
      Dispatcher.BeginInvoke(DispatcherPriority.Normal, action);
    }

    private double RangeTemperatureValue(double temperature)
    {
      if (Double.IsInfinity(temperature) || Double.IsNaN(temperature))
      {
        temperature = _minTemp; 
      }
      else if (temperature >= _maxTemp)
      {
        temperature = _maxTemp; 
      }
      else if (temperature <= _minTemp)
      {
        temperature = _minTemp;
      }

      return temperature;
    }
  }
}
