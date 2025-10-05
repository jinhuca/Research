using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using static Communication.CanBusMessageDefinition;
using static Communication.CanBusMessageDefinition.MessageStateId;

namespace Module.Console.Converters
{
	[ValueConversion(typeof(MessageStateId), typeof(SolidColorBrush))]
	public class SystemStateToBrushConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			BrushConverter brushConverter = new BrushConverter();

			var unknownBrush = brushConverter.ConvertFromString("#FFE0E0E0");
			/*var idleBrush = brushConverter.ConvertFromString("#FF11FF11");
			var readyBrush = brushConverter.ConvertFromString("#FF00AFAF");
			var inflationBrush = brushConverter.ConvertFromString("#FFFF2266");
			var transitionBrush = brushConverter.ConvertFromString("#FFEEEEEE");
			var ablationBrush = brushConverter.ConvertFromString("#FFFF8800");
			var thawingBrush = brushConverter.ConvertFromString("#FFCCFFFF");
			var exceptionBrush = brushConverter.ConvertFromString("#FFFF0022");

			switch ((MessageStateId)value)
			{
				case CAN_ID_STATE_IDLE:
					return idleBrush;
				case CAN_ID_STATE_READY:
					return readyBrush;
				case CAN_ID_STATE_INFLATION:
					return inflationBrush;
				case CAN_ID_STATE_TRANSITION:
					return transitionBrush;
				case CAN_ID_STATE_ABLATION:
					return ablationBrush;
				case CAN_ID_STATE_THAWING:
					return thawingBrush;
				case CAN_ID_STATE_EXCEPTION:
					return exceptionBrush;
				case CAN_ID_STATE_UNKNOWN:
				default:
					return unknownBrush;
			}*/
			return unknownBrush;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
