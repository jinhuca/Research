using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SmartAblationSystem.ConvertersNew
{
	internal class EmptyAblationTimeConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			// SystemState
			// EnabledIsBloodPressureSensorConnected
			// IsMonitoringBloodPressure
			var state_ = values[0].ToString();
			var enabled_ = System.Convert.ToBoolean(values[1]);
			var isMonitoring_ = System.Convert.ToBoolean(values[2]);

			return (state_ == "CAN_ID_STATE_IDLE" || state_ == "CAN_ID_STATE_READY" || state_ == "CAN_ID_STATE_INFLATION") && !enabled_ && isMonitoring_
				? Visibility.Visible
				: Visibility.Collapsed;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
