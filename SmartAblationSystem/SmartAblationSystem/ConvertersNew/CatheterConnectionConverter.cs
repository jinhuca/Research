using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using static Communication.CanBusMessageDefinition;

namespace SmartAblationSystem.ConvertersNew
{
	internal class CatheterConnectionConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if(!(value is MessageStateId id_))
				throw new ArgumentNullException(nameof(value));

			if(!(parameter is string parameter_))
				throw new ArgumentException(nameof(parameter));

			if (parameter_ == "CatheterMechanicalConnection")
			{
				return id_ != MessageStateId.CAN_ID_STATE_IDLE;
			}
			else
			{
				return false;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
