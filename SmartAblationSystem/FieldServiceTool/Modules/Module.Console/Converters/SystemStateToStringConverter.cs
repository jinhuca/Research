using System;
using System.Globalization;
using System.Windows.Data;
using static Communication.CanBusMessageDefinition;
using static Communication.CanBusMessageDefinition.MessageStateId;

namespace Module.Console.Converters
{
	[ValueConversion(typeof(MessageStateId), typeof(string))]
	public class SystemStateToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string result = string.Empty;
			if (value is null) return result;
			
			switch ((MessageStateId)value)
			{
				case CAN_ID_STATE_UNKNOWN:
					result = "Unknown";
					break;
				case CAN_ID_STATE_IDLE:
					result = "Idle";
					break;
				case CAN_ID_STATE_READY:
					result = "Ready";
					break;
				case CAN_ID_STATE_INFLATION:
					result = "Inflation";
					break;
				case CAN_ID_STATE_TRANSITION:
					result = "Transition";
					break;
				case CAN_ID_STATE_ABLATION:
					result = "Ablation";
					break;
				case CAN_ID_STATE_THAWING:
					result = "Thawing";
					break;
				case CAN_ID_STATE_EXCEPTION:
					result = "Exception";
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(value), value, null);
			}

			return result;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
