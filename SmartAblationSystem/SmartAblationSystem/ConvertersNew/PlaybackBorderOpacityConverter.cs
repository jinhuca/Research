using System;
using System.Globalization;
using System.Windows.Data;

namespace SmartAblationSystem.ConvertersNew
{
	internal class PlaybackBorderOpacityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if(value != null && bool.TryParse(value.ToString(), out var b_))
			{
				return b_ ? 0 : 1;
			}

			throw new NotSupportedException();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
