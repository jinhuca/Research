
namespace SmartAblationSystem.ConvertersNew
{
  using System;
  using System.Globalization;
  using System.Windows.Data;

  public class ETSTemperatureToStringConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var temperature = (double)value;
      return (temperature < 0 || temperature > 50) ? "-" : temperature.ToString("00");
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return Binding.DoNothing;
    }
  }
}
