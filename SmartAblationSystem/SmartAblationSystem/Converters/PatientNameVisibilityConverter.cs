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
	public class PatientNameVisibilityConverter : IMultiValueConverter
	{
    // 
    private readonly IValueConverter _visibilityConverter = new PWVisibilityConverter();
    private readonly IValueConverter _stringToMaskConverter = new StringToMaskConverter();
    private readonly IValueConverter _procedureToStringConverter = new ProcedureToStringConverter();

    public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
		{
      if (!(value[0] is DataAccessLayer.Patient currentPatient))
      {
        throw new ArgumentException("Invalid argument");
      }
      if (!(value[1] is bool patientCheckboxChecked))
      {
        throw new ArgumentException("Invalid argument");
      }
      Visibility isVisible = (Visibility)_visibilityConverter.Convert(patientCheckboxChecked, targetType, parameter, culture);
			if (isVisible == Visibility.Visible && parameter is string fullNameParameter)
			{
        return _procedureToStringConverter.Convert(currentPatient, targetType, fullNameParameter, culture);
      }
			else
			{
				return _stringToMaskConverter.Convert(currentPatient, targetType, parameter, culture);
			}
		}

		public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

}
