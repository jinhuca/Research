using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using static Communication.CanBusMessageDefinition;
using static Communication.CanBusMessageDefinition.MessageStateId;

namespace SmartAblationSystem.ConvertersNew
{
	[ValueConversion(typeof(MessageNodeId), typeof(Color))]
	internal class SystemStateToColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if(!(value is MessageStateId id_))
				throw new ArgumentNullException(nameof(value));

			switch (id_)
			{
				case CAN_ID_STATE_UNKNOWN:
					return new ColorConverter().ConvertFrom(DefinedStateColors.UnknownStateColor);
				case CAN_ID_STATE_IDLE:
					return new ColorConverter().ConvertFrom(DefinedStateColors.IdleStateColor);
				case CAN_ID_STATE_READY:
					return new ColorConverter().ConvertFrom(DefinedStateColors.ReadyStateColor);
				case CAN_ID_STATE_INFLATION:
					return new ColorConverter().ConvertFrom(DefinedStateColors.InflationStateColor);
				case CAN_ID_STATE_TRANSITION:
					return new ColorConverter().ConvertFrom(DefinedStateColors.TransitionStateColor);
				case CAN_ID_STATE_ABLATION:
					return new ColorConverter().ConvertFrom(DefinedStateColors.AblationStateColor);
				case CAN_ID_STATE_THAWING:
					return new ColorConverter().ConvertFrom(DefinedStateColors.ThawingStateColor);
				case CAN_ID_STATE_EXCEPTION:
					return new ColorConverter().ConvertFrom(DefinedStateColors.ExceptionStateColor);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
