using System;
using System.Globalization;
using System.Windows.Data;
using static Communication.CanBusMessageDefinition;
using static Communication.CanBusMessageDefinition.MessageStateId;

namespace SmartAblationSystem.ConvertersNew
{
	internal class SystemStateToBrushConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if(!(value is MessageStateId id_))
			{
				throw new ArgumentNullException(nameof(value));
			}

			switch (id_)
			{
				case CAN_ID_STATE_UNKNOWN:
					return new System.Windows.Media.BrushConverter().ConvertFromString(DefinedStateColors.UnknownStateColor);
				case CAN_ID_STATE_IDLE:
					return new System.Windows.Media.BrushConverter().ConvertFromString(DefinedStateColors.IdleStateColor); 
				case CAN_ID_STATE_READY:
					return new System.Windows.Media.BrushConverter().ConvertFromString(DefinedStateColors.ReadyStateColor); 
				case CAN_ID_STATE_INFLATION:
					return new System.Windows.Media.BrushConverter().ConvertFromString(DefinedStateColors.InflationStateColor);
				case CAN_ID_STATE_TRANSITION:
					return new System.Windows.Media.BrushConverter().ConvertFromString(DefinedStateColors.TransitionStateColor);
				case CAN_ID_STATE_ABLATION:
					return new System.Windows.Media.BrushConverter().ConvertFromString(DefinedStateColors.AblationStateColor);
				case CAN_ID_STATE_THAWING:
					return new System.Windows.Media.BrushConverter().ConvertFromString(DefinedStateColors.ThawingStateColor);
				case CAN_ID_STATE_EXCEPTION:
					return new System.Windows.Media.BrushConverter().ConvertFromString(DefinedStateColors.ExceptionStateColor);
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
