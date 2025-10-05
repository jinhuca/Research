using Communication;
using System;
using System.Globalization;
using System.Windows.Data;

namespace SmartAblationSystem.ConvertersNew
{
	internal class SystemStateToEnabledConverter : IValueConverter
	{	
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{			
			if (!(value is CanBusMessageDefinition.MessageStateId id_))
				throw new ArgumentNullException(nameof(value));

			if(!(parameter is string parameter_))
				throw new ArgumentException(nameof(parameter));

			switch (parameter_)
			{
				case ActionConstants.Inflate:
					return id_ == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY;
				default:
					return false;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
