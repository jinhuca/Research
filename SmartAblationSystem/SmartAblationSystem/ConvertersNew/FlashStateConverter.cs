using System;
using System.Globalization;
using System.Windows.Data;
using static Communication.CanBusMessageDefinition;

namespace SmartAblationSystem.ConvertersNew
{
	internal class FlashStateConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if(!(value is MessageStateId id_))
				throw new ArgumentNullException(nameof(value));

			return id_ == MessageStateId.CAN_ID_STATE_TRANSITION || id_ == MessageStateId.CAN_ID_STATE_ABLATION;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
