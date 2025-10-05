using Communication;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Module.TestProcess.Converters
{
	[ValueConversion(typeof(CanBusMessageDefinition.MessageStateId), typeof(string))]
	public class SystemStateToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string result = string.Empty;
			if (value is null) return result;
			
			switch ((CanBusMessageDefinition.MessageStateId)value)
			{
				case CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_UNKNOWN:
					result = "Unknown";
					break;
				case CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE:
					result = "IDLE";
					break;
				case CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY:
					result = "READY";
					break;
				case CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_INFLATION:
					result = "INFLATION";
					break;
				case CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION:
					result = "TRANSITION";
					break;
				case CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_ABLATION:
					result = "ABLATION";
					break;
				case CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING:
					result = "THAWING";
					break;
				case CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_EXCEPTION:
					result = "EXCEPTION";
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
