using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using static Communication.CanBusMessageDefinition;

namespace Module.TestProcess.Converters
{
	[ValueConversion(typeof(MessageStateId), typeof(SolidColorBrush))]
	public class SystemStateToBrushConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			BrushConverter brushConverter = new BrushConverter();

			var unknownBrush = brushConverter.ConvertFromString("#FF909090");
			var idleBrush = brushConverter.ConvertFromString("#FFB5E61D");
			var readyBrush = brushConverter.ConvertFromString("#FF33EEFF");
			var inflationBrush = brushConverter.ConvertFromString("#FFFF8040");
			var transitionBrush = brushConverter.ConvertFromString("#FFFF0080");
			var ablationBrush = brushConverter.ConvertFromString("#FFFF8000");
			var thawingBrush = brushConverter.ConvertFromString("#FFFF1010");
			var exceptionBrush = brushConverter.ConvertFromString("#FFED1C24");

			switch ((MessageStateId)value)
			{
				case MessageStateId.CAN_ID_STATE_IDLE:
					return idleBrush;
				case MessageStateId.CAN_ID_STATE_READY:
					return readyBrush;
				case MessageStateId.CAN_ID_STATE_INFLATION:
					return inflationBrush;
				case MessageStateId.CAN_ID_STATE_TRANSITION:
					return transitionBrush;
				case MessageStateId.CAN_ID_STATE_ABLATION:
					return ablationBrush;
				case MessageStateId.CAN_ID_STATE_THAWING:
					return thawingBrush;
				case MessageStateId.CAN_ID_STATE_EXCEPTION:
					return exceptionBrush;
				case MessageStateId.CAN_ID_STATE_UNKNOWN:
				default:
					return unknownBrush;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
