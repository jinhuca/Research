using System.Windows;

namespace CustomControls.UserControls
{
  /// <summary>
  /// Interaction logic for VolumeBarCtrl.xaml
  /// </summary>
  public partial class VolumeBarCtrl
  {
    public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register(
      nameof(MinValue), 
      typeof(double), 
      typeof(VolumeBarCtrl), 
      new PropertyMetadata(0d, MinValueChanged));

    public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(
      nameof(MaxValue), 
      typeof(double), 
      typeof(VolumeBarCtrl),
      new PropertyMetadata(100d, MaxValueChanged));

    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
      nameof(Value), 
      typeof(double), 
      typeof(VolumeBarCtrl),
      new PropertyMetadata(0d, ValueChanged));

    public VolumeBarCtrl()
    {
      InitializeComponent();
    }

    public double MinValue
    {
      get => (double)GetValue(MinValueProperty);
      set => SetValue(MinValueProperty, value);
    }

    public double MaxValue
    {
      get => (double)GetValue(MaxValueProperty);
      set => SetValue(MaxValueProperty, value);
    }

    public double Value
    {
      get => (double)GetValue(ValueProperty);
      set => SetValue(ValueProperty, value);
    }

    private static void MinValueChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
    {
      if(!(dp is VolumeBarCtrl control_)) return;

      UpdateProgress(control_, control_.Value);
    }

    private static void MaxValueChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
    {
      if(!(dp is VolumeBarCtrl control_)) return;

      UpdateProgress(control_, control_.Value);
    }

    private static void ValueChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
    {
      if(!(dp is VolumeBarCtrl control_)) return;

      UpdateProgress(control_, (double)e.NewValue);
    }

    private static void UpdateProgress(VolumeBarCtrl control, double value)
    {
      var borderWidth_ = control._border1.ActualWidth;
      var totalLength_ = borderWidth_ == 0 ? control._mainGrid.Width : borderWidth_;
      value = value < control.MaxValue 
        ? value > control.MinValue ? value : control.MinValue
        : control.MaxValue;
      var progressWidth_ = ((value - control.MinValue) * totalLength_) / (control.MaxValue - control.MinValue);
      control._valueProgressBar.Width = progressWidth_;
      control._valueProgressBar.CornerRadius = value >= control.MaxValue 
        ? new CornerRadius(5) 
        : new CornerRadius(5, 0, 0, 5);
    }
  }
}
