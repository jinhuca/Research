using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CustomControls;

public partial class MeterControl : UserControl {
  public MeterControl() {
    InitializeComponent();
  }

  #region Value DependencyProperty

  private const double MinValue = 0, MaxValue = 100;

  public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
    nameof(Value),
    typeof(double),
    typeof(MeterControl),
    new FrameworkPropertyMetadata(
      MinValue, 
      new PropertyChangedCallback(OnValueChanged),
      new CoerceValueCallback(CoerceValue)));

  public double Value {
    get => (double)GetValue(ValueProperty);
    set => SetValue(ValueProperty, value);
  }

  private static object CoerceValue(DependencyObject d, object baseValue) {
    var control = (MeterControl)d;
    double value = (double)baseValue;
    if (value < MinValue) return MinValue;
    if (value > MaxValue) return MaxValue;
    return value;
  }

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

  public static readonly DependencyProperty NeedleColorProperty = DependencyProperty.Register(
    nameof(NeedleColor),
    typeof(Brush),
    typeof(MeterControl),
    new FrameworkPropertyMetadata(Brushes.Red));

  public Brush NeedleColor {
    get => (Brush)GetValue(NeedleColorProperty);
    set => SetValue(NeedleColorProperty, value);
  }

  public static readonly DependencyProperty GaugeColorProperty = DependencyProperty.Register(
    nameof(GaugeColor),
    typeof(Brush),
    typeof(MeterControl),
    new FrameworkPropertyMetadata(Brushes.LightGray));

  public Brush GaugeColor {
    get => (Brush)GetValue(GaugeColorProperty);
    set => SetValue(GaugeColorProperty, value);
  }
}