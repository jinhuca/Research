
namespace CustomControls.UserControls
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Windows.Controls;
  using System.Windows.Media;

  /// <summary>
  /// Interaction logic for ETSProbeBarControl.xaml
  /// </summary>
  public partial class ETSProbeBarControl 
  {
    private static readonly int _maxBarHight = 110;
    private static readonly double _maxTemperature = 40;
    private static readonly double _offset = 12.8; 

    private static readonly Color _defaultIndexTextColor = (Color)ColorConverter.ConvertFromString("#FFCEC3C6");
    private static readonly string _valueTag = @"#VALUE";
    private static string _etsTempPathData = @"M 0 #VALUE L 268 #VALUE"; 

    public ETSProbeBarControl()
    {
      this.InitializeComponent();
    }

    #region DependencyProperty definitions
 
    #endregion DependencyProperty definitions

    #region DependencyProperty Update Handlers

    protected override void UpdateETSSensorDataControls(IList<double> etsSensorData, IList<bool> channelStatus) 
    {
      if (etsSensorData == null || channelStatus == null)
      {
        return;
      }

      var controlList = new List<(Border, TextBlock, TextBlock)>()
                          {
                            (this._etsSensorBarP, this._etsSensorValueP, this._etsSensorIndexP), 
                            (this._etsSensorBar1, this._etsSensorValue1, this._etsSensorIndex1), 
                            (this._etsSensorBar2 , this._etsSensorValue2 , this._etsSensorIndex2), 
                            (this._etsSensorBar3 , this._etsSensorValue3 , this._etsSensorIndex3), 
                            (this._etsSensorBar4 , this._etsSensorValue4 , this._etsSensorIndex4), 
                            (this._etsSensorBar5 , this._etsSensorValue5 , this._etsSensorIndex5), 
                            (this._etsSensorBar6 , this._etsSensorValue6 , this._etsSensorIndex6), 
                            (this._etsSensorBar7 , this._etsSensorValue7 , this._etsSensorIndex7), 
                            (this._etsSensorBar8 , this._etsSensorValue8 , this._etsSensorIndex8), 
                            (this._etsSensorBar9 , this._etsSensorValue9 , this._etsSensorIndex9), 
                            (this._etsSensorBar10 , this._etsSensorValue10 , this._etsSensorIndex10), 
                            (this._etsSensorBar11 , this._etsSensorValue11 , this._etsSensorIndex11), 
                            (this._etsSensorBar12 , this._etsSensorValue12 , this._etsSensorIndex12), 
                          };

      var count = Math.Min(etsSensorData.Count, controlList.Count);
      var minValue = this.MinTemperature <= 0 ? Math.Max(0, etsSensorData.Min()) : this.MinTemperature; 
      for (int i = 0; i < count; ++i)
      {
        var isSensorBroken = channelStatus[i]; 
        var sensorData = isSensorBroken ? 0 : etsSensorData[i];
        
        controlList[i].Item1.Height = (sensorData / _maxTemperature) * _maxBarHight; 
        controlList[i].Item1.Background = ETSSensorDataBarBrushConverter(this, sensorData, this.EtsTemperature); 

        controlList[i].Item2.Text = isSensorBroken ? _brokenSensorText : sensorData.ToString();
        controlList[i].Item2.Foreground = ETSSensorDataToColorConverter(sensorData, minValue, this.EtsTemperature);

        controlList[i].Item3.Foreground = isSensorBroken
                                            ? new SolidColorBrush(_warnETSSensorDataColor)
                                            : ETSSensorDataToColorConverter(sensorData, minValue, this.EtsTemperature);
      }
    }

    protected override void ETSTemperatureChanged(double etsTemperature)
    {
      base.ETSTemperatureChanged(etsTemperature);

      // var height = this._etsSensorValue1.ActualHeight;
      int guideLineHeight = (int)(_maxBarHight + _offset - etsTemperature / _maxTemperature * _maxBarHight);
      var data = _etsTempPathData.Replace(_valueTag, guideLineHeight.ToString()); 
      this._etsTemperatureSettingLine.Data = Geometry.Parse(data);
    }

    #endregion DependencyProperty Update Handlers

    private static Brush ETSSensorDataBarBrushConverter(ETSProbeBarControl control, double sensorData, double settingValue)
    {
      var normalBrush = control.FindResource("etsBarBrush") as Brush;
      var warningBrush = control.FindResource("etsBarWarningBrush") as Brush; 
      return sensorData >= settingValue
               ? normalBrush
               : warningBrush;
    }
  }
}
