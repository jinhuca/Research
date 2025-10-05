using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace SmartAblationSystem.Converters
{
	public class PatientStyleVisibilityConverter : IMultiValueConverter
	{
    public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (values[0] is bool isChecked && values[1] is Style infoStyle && values[2] is Style maskedStyle)
      {
        return isChecked ? infoStyle : maskedStyle;
      }
      return null;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
	}

}
