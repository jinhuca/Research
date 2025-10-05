namespace CustomControls.UserControls
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Windows.Controls;
  using System.Windows.Shapes;

  /// <summary>
  /// Interaction logic for ETSCircaSensorControl.xaml
  /// </summary>
  public partial class ETSCircaSensorControl
  {
    public ETSCircaSensorControl()
    {
      this.InitializeComponent();
    }

    protected override void UpdateETSSensorDataControls(IList<double> etsSensorData, IList<bool> channelStatus)
    {
      if (etsSensorData == null || channelStatus == null)
      {
        return;
      }

      var controlList = new List<(Ellipse, TextBlock)>()
                          {
                            (this.PART_Sensor_EllipseP, this.PART_Sensor_TextP),
                            (this.PART_Sensor_Ellipse1, this.PART_Sensor_Text1),
                            (this.PART_Sensor_Ellipse2, this.PART_Sensor_Text2),
                            (this.PART_Sensor_Ellipse3, this.PART_Sensor_Text3),
                            (this.PART_Sensor_Ellipse4, this.PART_Sensor_Text4),
                            (this.PART_Sensor_Ellipse5, this.PART_Sensor_Text5),
                            (this.PART_Sensor_Ellipse6, this.PART_Sensor_Text6),
                            (this.PART_Sensor_Ellipse7, this.PART_Sensor_Text7),
                            (this.PART_Sensor_Ellipse8, this.PART_Sensor_Text8),
                            (this.PART_Sensor_Ellipse9, this.PART_Sensor_Text9),
                            (this.PART_Sensor_Ellipse10, this.PART_Sensor_Text10),
                            (this.PART_Sensor_Ellipse11, this.PART_Sensor_Text11),
                            (this.PART_Sensor_Ellipse12, this.PART_Sensor_Text12)
                          };
      
      var count = Math.Min(etsSensorData.Count, controlList.Count);
      var minValue = this.MinTemperature <= 0 ? Math.Max(0, etsSensorData.Min()) : this.MinTemperature; 
      for (int i = 0; i < count; ++i)
      {
        var isSensorBroken = channelStatus[i]; 
        var sensorData = isSensorBroken ? 0 : etsSensorData[i];
        controlList[i].Item1.Fill = ETSSensorDataToColorConverter(sensorData, minValue, this.EtsTemperature);
        controlList[i].Item2.Text = isSensorBroken 
                                      ? _brokenSensorText 
                                      : i != 0 
                                        ? i.ToString()
                                        : "P";
      }
    }
  }
}
