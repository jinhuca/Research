using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SmartAblationSystem.Converters
{
	public class CountToVisibilityConverter : IValueConverter
	{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var count_ = (int)value;
      var step_ = (string)parameter;
      var result_ = Visibility.Hidden;

      switch(count_)
      {
        case 0:
          if(step_ == "Step1")
          {
            result_ = Visibility.Visible;
          }
          break;
        case 1:
          if(step_ == "Step1" || step_ == "Step2")
          {
            result_ = Visibility.Visible;
          }
          break;
        case 2:
          result_ = Visibility.Visible;
          break;
      }
      return result_;
    }

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
