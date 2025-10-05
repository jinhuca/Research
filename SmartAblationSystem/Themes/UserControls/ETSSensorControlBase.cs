using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace CustomControls.UserControls
{
  using System.Collections.Generic;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Media;

  public abstract class ETSSensorControlBase : UserControl
  {
    protected static readonly double _invalidMinTemperature = -100;
    protected static readonly string _brokenSensorText = "X"; 

    protected static readonly IList<double> _defaultSensorData = new List<double>(){0,0,0,0,0,0,0,0,0,0,0,0,0};
    protected static readonly IList<bool> _defaultChannelStatus = new List<bool>(){false, 
                                                                    false, false, false, false, 
                                                                    false, false, false, false, 
                                                                    false, false, false, false };

    protected static readonly Color _defaultETSSensorDataColor = (Color)ColorConverter.ConvertFromString("#ffffffff");
    protected static readonly Color _minETSSensorDataColor = (Color)ColorConverter.ConvertFromString("#ff73b6e4");
    protected static readonly Color _warnETSSensorDataColor = (Color)ColorConverter.ConvertFromString("#ffff9d57");

    private readonly ISubject<bool> _sensorDataUpdateSubject = new Subject<bool>();

    protected ETSSensorControlBase()
    {
      _sensorDataUpdateSubject
        .Throttle(TimeSpan.FromMilliseconds(10))
        .ObserveOnDispatcher()
        .Subscribe(_ =>
        {
          if (IsVisible) UpdateETSSensorDataControls(ETSSensorData, ChannelStatus);
        });
    }

    #region DependencyProperty registrations

    public static readonly DependencyProperty ETSSensorDataProperty = DependencyProperty.Register(
      "ETSSensorData",
      typeof(IList<double>), 
      typeof(ETSSensorControlBase),
      new PropertyMetadata(_defaultSensorData, OnETSSensorDataChanged, ETSSensorDataCoerceValueCallback));
    
    public static readonly DependencyProperty EtsTemperatureProperty = DependencyProperty.Register(
      "EtsTemperature",
      typeof(double),
      typeof(ETSSensorControlBase),
      new PropertyMetadata(0d, OnETSTemperatureChanged));

    public static readonly DependencyProperty MinTemperatureProperty = DependencyProperty.Register(
      "MinTemperature",
      typeof(double), 
      typeof(ETSSensorControlBase),
      new PropertyMetadata(_invalidMinTemperature, OnMinTemperatureChanged));

    public static readonly DependencyProperty ChannelStatusProperty = DependencyProperty.Register(
      "ChannelStatus",
      typeof(IList<bool>), 
      typeof(ETSSensorControlBase),
      new PropertyMetadata(_defaultChannelStatus, OnETSChannelStatusChanged, ETSChannelStatusCoerceValueCallback));

    #endregion DependencyProperty registration

    #region DependencyProperty definitions
    public IList<double> ETSSensorData
    {
      get => (IList<double>)this.GetValue(ETSSensorDataProperty);
      set => this.SetValue(ETSSensorDataProperty, value);
    }

    public double EtsTemperature
    {
      get => (double)this.GetValue(EtsTemperatureProperty);
      set => this.SetValue(EtsTemperatureProperty, value);
    }

    public double MinTemperature
    {
      get => (double)this.GetValue(MinTemperatureProperty);
      set => this.SetValue(MinTemperatureProperty, value);
    }

    public IList<bool> ChannelStatus
    {
      get => (IList<bool>)this.GetValue(ChannelStatusProperty);
      set => this.SetValue(ChannelStatusProperty, value);
    }

    #endregion DependencyProperty definitions

    #region DependencyProperty Update Handlers

    protected static object ETSSensorDataCoerceValueCallback(DependencyObject dp, object value)
    {
      // if the Sensor Data is reference equal, invoke the update.
      // Otherwise let PropertyChanged callback handle it. 
      var etsSensorData_ = ((ETSSensorControlBase)dp).ETSSensorData;
      if (etsSensorData_ != null && value != null && etsSensorData_.Equals(value))
      {
        var control = (ETSSensorControlBase)dp;
        // control?.UpdateETSSensorDataControls((IList<double>)value, control?.ChannelStatus);
        control?._sensorDataUpdateSubject.OnNext(true);
      }

      return value;
    }

    protected static void OnETSSensorDataChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e) 
    {
      var etsSensorData = (IList<double>)e.NewValue ?? new List<double>(_defaultSensorData);
      var control = (ETSSensorControlBase)dp;
      // control?.UpdateETSSensorDataControls(etsSensorData, control?.ChannelStatus); 
      control?._sensorDataUpdateSubject.OnNext(true);
    }

    protected static void OnETSChannelStatusChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
    {
      var etsChannelStatus = (IList<bool>)e.NewValue ?? _defaultChannelStatus;
      var control = (ETSSensorControlBase)dp;
      // control?.UpdateETSSensorDataControls(control?.ETSSensorData, etsChannelStatus); 
      control?._sensorDataUpdateSubject.OnNext(true);
    }

    protected static object ETSChannelStatusCoerceValueCallback(DependencyObject dp, object value)
    {
      var etsChannelStatus_ = ((ETSSensorControlBase)dp).ChannelStatus;
      if (etsChannelStatus_ != null && value != null && etsChannelStatus_.Equals(value))
      {
        var control = (ETSSensorControlBase)dp;
        // control?.UpdateETSSensorDataControls(control?.ETSSensorData, (IList<bool>)value);
        control?._sensorDataUpdateSubject.OnNext(true);
      }

      return value;
    }

    protected abstract void UpdateETSSensorDataControls(IList<double> etsSensorData, IList<bool> channelStatus);

    protected static void OnETSTemperatureChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
    {
      //int etsTemperature
      var etsTemperature = (double)e.NewValue; 
      var control = (ETSSensorControlBase)dp;
      control?.ETSTemperatureChanged(etsTemperature);
    } 

    protected virtual void ETSTemperatureChanged(double etsTemperature){}

    protected static void OnMinTemperatureChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
    {
      var control = (ETSSensorControlBase) dp;
      // control?.UpdateETSSensorDataControls(control?.ETSSensorData, control?.ChannelStatus);
      control?._sensorDataUpdateSubject.OnNext(true);
    }

    #endregion DependencyProperty Update Handlers

    protected static SolidColorBrush ETSSensorDataToColorConverter(double sensorData_, double minValue_, double settingValue_)
    {
      var sensorData = Convert.ToInt32(sensorData_); 
      var settingValue = Convert.ToInt32(settingValue_);
      var minValue = Convert.ToInt32(minValue_);

      return sensorData >= settingValue
               ? sensorData > minValue
                   ? new SolidColorBrush(_defaultETSSensorDataColor)
                   : new SolidColorBrush(_minETSSensorDataColor)
               : new SolidColorBrush(_warnETSSensorDataColor);
    }
  }
}
