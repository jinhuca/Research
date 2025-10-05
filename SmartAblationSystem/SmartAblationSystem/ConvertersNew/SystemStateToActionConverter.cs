using Communication;
using System;
using System.Globalization;
using System.Windows.Data;

namespace SmartAblationSystem.ConvertersNew
{
	internal class SystemStateToActionConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if(!(value is CanBusMessageDefinition.MessageStateId id_))
				throw new ArgumentNullException(nameof(value));

			if(!(parameter is string parameter_))
				throw new ArgumentException(nameof(parameter));

			switch (id_)
			{
				case CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_UNKNOWN:
					return false;
				case CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE:
					return parameter_ == ActionConstants.VacuumOn;
				case CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY:
					return false;
					
				case CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_INFLATION:
					return false;
				case CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION:
					return false;
				case CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_ABLATION:
					return false;
				case CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING:
					return false;
				case CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_EXCEPTION:
					return false;
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
