using System;
using System.Globalization;
using System.Windows.Data;

namespace Module.Infrastructure
{
	[ValueConversion(typeof(CatheterStatus), typeof(bool))]
	public class CatheterStatusFlashConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var catheterStatus = (CatheterStatus)value;
			switch (catheterStatus)
			{
				case CatheterStatus.Disconnected:
				case CatheterStatus.Connected:
				case CatheterStatus.Ready:
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
