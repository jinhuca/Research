using System;
using System.Globalization;
using System.Windows.Data;

namespace SmartAblationSystem.ConvertersNew
{
	internal class TTIValueConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			var convertedString_ = "-";
			if (!int.TryParse(values[0]?.ToString(), out int value_) ||
			    !bool.TryParse(values[1]?.ToString(), out bool isVeinIsolated_) ||
			    !bool.TryParse(values[3]?.ToString(), out bool isPlayBackMode_)) return convertedString_;

			var systemState_ = values[2]?.ToString();
			if ((systemState_ == "CAN_ID_STATE_IDLE" || systemState_ == "CAN_ID_STATE_READY" || systemState_ == "CAN_ID_STATE_INFLATION") 
			    && !isPlayBackMode_)
			{
				convertedString_ = "-";
			}

			else if (isVeinIsolated_)
			{
				convertedString_ = value_.ToString();
			}

			return convertedString_;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
