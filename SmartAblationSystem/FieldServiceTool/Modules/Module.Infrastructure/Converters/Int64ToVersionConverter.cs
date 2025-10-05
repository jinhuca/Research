using System;
using System.Globalization;
using System.Windows.Data;

namespace Module.Infrastructure
{
	[ValueConversion(typeof(Int64), typeof(string))]
	public class Int64ToVersionConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string valueConverted = System.Convert.ToInt64(value).ToString("X");
			if (valueConverted != null)
			{
				int length = valueConverted.Length;
				switch (valueConverted.Length)
				{
					case 4:
						valueConverted = valueConverted.Insert(3, ".").Insert(2, ".").Insert(1, ".");
						break;
					case 3:
						valueConverted = valueConverted.Insert(length, ".").Insert(length - 1, ".").Insert(length - 2, ".") + "0";
						break;
					case 2:
						valueConverted = valueConverted.Insert(length - 1, ".") + "0.0";
						break;
					case 1:
						valueConverted = valueConverted + ".0.0.0";
						break;
				}
			}
			return valueConverted;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
