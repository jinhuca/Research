
using System;
using System.Reactive.Linq;
using System.Windows;

namespace Module.CatheterTestTool.Views
{
  /// <summary>
  /// Interaction logic for TemperatureGraphControl.xaml
  /// </summary>
  public partial class TemperatureGraphControl
  {
    private static readonly int DefaultDisplayCount = 60;
    private static readonly string TemperatureSeries = "temperatureSeries";

    public static readonly DependencyProperty DisplayCountProperty =
      DependencyProperty.Register("DisplayCount", typeof(int), typeof(TemperatureGraphControl),
        new PropertyMetadata(DefaultDisplayCount, 
          (o, e) => 
            ((TemperatureGraphControl)o).chartTemperature.ChartAreas["temperatureArea"].AxisX.Maximum = (int)e.NewValue));

    public static readonly DependencyProperty DisplayValueProperty =
      DependencyProperty.Register("DisplayValue", typeof(double), typeof(TemperatureGraphControl),
        new PropertyMetadata(0.0d));

    public static readonly DependencyProperty ClearGraphProperty =
      DependencyProperty.RegisterAttached("ClearGraph", typeof(bool), typeof(TemperatureGraphControl),
        new PropertyMetadata(false, HandleClearGraphChanged));

    public TemperatureGraphControl()
    {
      InitializeComponent();
      Observable.Interval(TimeSpan.FromSeconds(1.0))
        .ObserveOnDispatcher()
        .Subscribe(_ => UpdateTemperatures());
    }

    public int DisplayCount
    {
      get => (int)GetValue(DisplayCountProperty);
      set => SetValue(DisplayCountProperty, value);
    }

    public double DisplayValue
    {
      get => (double)GetValue(DisplayValueProperty);
      set => SetValue(DisplayValueProperty, value);
    }

    public static bool GetClearGraph(DependencyObject d)
    {
      return (bool)d.GetValue(ClearGraphProperty);
    }

    public static void SetClearGraph(DependencyObject d, bool value)
    {
      d.SetValue(ClearGraphProperty, value);
    }

    private void UpdateTemperatures()
    {
      if (chartTemperature.Series[TemperatureSeries].Points.Count >= DisplayCount)
      {
        chartTemperature.Series[TemperatureSeries].Points.RemoveAt(0);
        foreach (var pt in chartTemperature.Series[TemperatureSeries].Points)
        {
          pt.XValue -= 1;
        }
      }
      chartTemperature.Series[TemperatureSeries].Points.AddXY(chartTemperature.Series[TemperatureSeries].Points.Count - 1, DisplayValue);
    }

    private static void HandleClearGraphChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if (!(bool)e.NewValue) return;

      ((TemperatureGraphControl)sender)?.chartTemperature.Series[TemperatureSeries].Points.Clear();
    }

  }
}
