
namespace CustomControls.UserControls
{
  using System.Windows;

  /// <summary>
  /// Interaction logic for ProgressSettingBarControl.xaml
  /// </summary>
  public partial class ProgressSettingBarControl
  {
    public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register("MinValue",
      typeof(double), typeof(ProgressSettingBarControl),
      new PropertyMetadata(0d, MinValueChanged));

    public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue",
      typeof(double), typeof(ProgressSettingBarControl),
      new PropertyMetadata(100d, MaxValueChanged));

    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", 
      typeof(double), typeof(ProgressSettingBarControl),
      new PropertyMetadata(0d, ValueChanged));

    public ProgressSettingBarControl()
    {
      this.InitializeComponent();
    }

    public double MinValue
    {
      get => (double)this.GetValue(MinValueProperty);
      set => this.SetValue(MinValueProperty, value);
    }

    public double MaxValue
    {
      get => (double)this.GetValue(MaxValueProperty);
      set => this.SetValue(MaxValueProperty, value);
    }

    public double Value
    {
      get => (double)this.GetValue(ValueProperty); 
      set => this.SetValue(ValueProperty, value);
    }

    private static void MinValueChanged(DependencyObject dp,DependencyPropertyChangedEventArgs e)
    {
      var control = dp as ProgressSettingBarControl;
      if (control == null) return;

      UpdateProgress(control, control.Value);
    }

    private static void MaxValueChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
    {
      var control = dp as ProgressSettingBarControl;
      if (control == null) return;

      UpdateProgress(control, control.Value);
    }

    private static void ValueChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
    {
      var control = dp as ProgressSettingBarControl;
      if (control == null) return;

      UpdateProgress(control, (double)e.NewValue);
    }

    private static void UpdateProgress(ProgressSettingBarControl control, double value)
    {

      var borderWidth = control._border1.ActualWidth;
      var totalLength = borderWidth == 0 ? control._mainGrid.Width : borderWidth;
      value = value < control.MaxValue 
                ? value > control.MinValue 
                    ? value : control.MinValue 
                : control.MaxValue;
      var progressWidth = ((value - control.MinValue) * totalLength) / (control.MaxValue - control.MinValue);
      control._valueProgressBar.Width = progressWidth;

      control._valueProgressBar.CornerRadius = value >= control.MaxValue 
                                                 ? new CornerRadius(5)
                                                 : new CornerRadius(5, 0, 0, 5); 
    }
  }
}
