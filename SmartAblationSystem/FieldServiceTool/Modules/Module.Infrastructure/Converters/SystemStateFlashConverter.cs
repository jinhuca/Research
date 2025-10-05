using System;
using System.Globalization;
using System.Windows.Data;
using static Communication.CanBusMessageDefinition;

namespace Module.Infrastructure
{
	[ValueConversion(typeof(MessageStateId), typeof(bool))]
	public class SystemStateFlashConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var systemState = (MessageStateId)value;
			switch (systemState)
			{
				case MessageStateId.CAN_ID_STATE_INFLATION:
				case MessageStateId.CAN_ID_STATE_ABLATION:
				case MessageStateId.CAN_ID_STATE_THAWING:
				case MessageStateId.CAN_ID_STATE_UNKNOWN:
				case MessageStateId.CAN_ID_STATE_IDLE:
				case MessageStateId.CAN_ID_STATE_READY:
				case MessageStateId.CAN_ID_STATE_TRANSITION:
				case MessageStateId.CAN_ID_STATE_EXCEPTION:
					return true;
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
