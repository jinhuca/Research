using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Module.Console.Converters
{
	[ValueConversion(typeof(bool), typeof(SolidColorBrush))]
	public class VacuumBrushConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			BrushConverter brushConverter = new BrushConverter();
			//var vacuumOnBrush = brushConverter.ConvertFromString("#FF99EE33");
			var vacuumOffBrush = brushConverter.ConvertFromString("#FFE0E0E0");
			return vacuumOffBrush;
			//return bool.TryParse(value.ToString(), out var boolValue) ? boolValue ? vacuumOnBrush : vacuumOffBrush : vacuumOffBrush;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
