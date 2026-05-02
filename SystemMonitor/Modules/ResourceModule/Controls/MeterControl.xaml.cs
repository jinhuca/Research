using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ResourceModule.Controls;

public partial class MeterControl : UserControl {
  public MeterControl() {
    InitializeComponent();
  }

  #region Value DependencyProperty

  private const double DefaultMinValue = 0, DefaultMaxValue = 100;
  private const double DefaultMinAngle = -120, DefaultMaxAngle = 120;

  public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
    nameof(Value),
    typeof(double),
    typeof(MeterControl),
    new FrameworkPropertyMetadata(
      DefaultMinValue,
      new PropertyChangedCallback(OnValueChanged)));

  public double Value {
    get => (double)GetValue(ValueProperty);
    set => SetValue(ValueProperty, value);
  }

  //private static object CoerceValue(DependencyObject d, object baseValue) {
  //  var control = (MeterControl)d;
  //  double value = (double)baseValue;
  //  //if (value < DefaultMinValue) 
  //  //  return DefaultMinValue;
  //  //if (value > DefaultMaxValue) 
  //  //  return DefaultMaxValue;
  //  return value;
  //}

  private static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args) {
    MeterControl control = (MeterControl)obj;
    RoutedPropertyChangedEventArgs<double> e = new RoutedPropertyChangedEventArgs<double>(
        (double)args.OldValue, (double)args.NewValue, ValueChangedEvent);
    control.OnValueChanged(e);
  }

  public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent(
    nameof(ValueChanged),
    RoutingStrategy.Bubble,
    typeof(RoutedPropertyChangedEventHandler<double>),
    typeof(MeterControl));

  public event RoutedPropertyChangedEventHandler<double> ValueChanged {
    add { AddHandler(ValueChangedEvent, value); }
    remove { RemoveHandler(ValueChangedEvent, value); }
  }

  protected virtual void OnValueChanged(RoutedPropertyChangedEventArgs<double> args) {
    RaiseEvent(args);
  }

  #endregion Value DependencyProperty

  #region Label DependencyProperty

  public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
    nameof(Label),
    typeof(string),
    typeof(MeterControl),
    new FrameworkPropertyMetadata(string.Empty));

  public string Label {
    get => (string)GetValue(LabelProperty);
    set => SetValue(LabelProperty, value);
  }

  #endregion Label DependencyProperty

  #region Unit DependencyProperty

  public static readonly DependencyProperty UnitProperty = DependencyProperty.Register(
    nameof(Unit),
    typeof(string),
    typeof(MeterControl),
    new FrameworkPropertyMetadata(string.Empty));

  public string Unit {
    get => (string)GetValue(UnitProperty);
    set => SetValue(UnitProperty, value);
  }

  #endregion Unit DependencyProperty

  #region NeedleColor DependencyProperty

  public static readonly DependencyProperty NeedleColorProperty = DependencyProperty.Register(
    nameof(NeedleColor),
    typeof(Brush),
    typeof(MeterControl),
    new FrameworkPropertyMetadata(Brushes.Red));

  public Brush NeedleColor {
    get => (Brush)GetValue(NeedleColorProperty);
    set => SetValue(NeedleColorProperty, value);
  }

  #endregion NeedleColor DependencyProperty

  #region GaugeColor DependencyProperty

  public static readonly DependencyProperty GaugeColorProperty = DependencyProperty.Register(
    nameof(GaugeColor),
    typeof(Brush),
    typeof(MeterControl),
    new FrameworkPropertyMetadata(Brushes.LightGray));

  public Brush GaugeColor {
    get => (Brush)GetValue(GaugeColorProperty);
    set => SetValue(GaugeColorProperty, value);
  }

  #endregion GaugeColor DependencyProperty

  public double MinValue {
    get => (double)GetValue(MinValueProperty);
    set => SetValue(MinValueProperty, value);
  }

  public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register(
    nameof(MinValue),
    typeof(double),
    typeof(MeterControl),
    new FrameworkPropertyMetadata(DefaultMinValue));

  public double MaxValue {
    get => (double)GetValue(MaxValueProperty);
    set => SetValue(MaxValueProperty, value);
  }

  public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(
    nameof(MaxValue),
    typeof(double),
    typeof(MeterControl),
    new FrameworkPropertyMetadata(DefaultMaxValue));

  //public double PredefinedMinAngle {
  //  get => (double)GetValue(MinAngleProperty);
  //  set => SetValue(MinAngleProperty, value);
  //}

  //public static readonly DependencyProperty MinAngleProperty = DependencyProperty.Register(
  //  nameof(PredefinedMinAngle),
  //  typeof(double),
  //  typeof(MeterControl),
  //  new FrameworkPropertyMetadata(DefaultMinAngle));

  //public double PredefinedMaxAngle {
  //  get => (double)GetValue(MaxAngleProperty);
  //  set => SetValue(MaxAngleProperty, value);
  //}

  //public static readonly DependencyProperty MaxAngleProperty = DependencyProperty.Register(
  //  nameof(PredefinedMaxAngle),
  //  typeof(double),
  //  typeof(MeterControl),
  //  new FrameworkPropertyMetadata(DefaultMaxAngle));
}