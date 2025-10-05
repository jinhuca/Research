using System;
using System.Globalization;
using System.Windows.Data;
using static Communication.CanBusMessageDefinition;

namespace SmartAblationSystem.ConvertersNew
{
	internal class SystemStateToBoolConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if(!(value is MessageStateId id_))
			{
				throw new ArgumentNullException(nameof(value));
			}

			if(!(parameter is string parameter_))
			{
				throw new ArgumentException(nameof(parameter));
			}

			if(parameter_ == UIConstants.AblationState 
			   && (id_ == MessageStateId.CAN_ID_STATE_TRANSITION || id_ == MessageStateId.CAN_ID_STATE_ABLATION))
			{
				return true;
			}

			return SystemStateToStringConverter.Convert(id_) == parameter_;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
