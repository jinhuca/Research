using System;
using Communication;

namespace SmartAblationSystem.ConvertersNew
{
	internal static class SystemStateToStringConverter
	{
		public static string Convert(CanBusMessageDefinition.MessageStateId state)
		{
			switch (state)
			{
				case CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_UNKNOWN:
					return UIConstants.UnknownState;
				case CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE:
					return UIConstants.IdleState;
				case CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY:
					return UIConstants.ReadyState;
				case CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_INFLATION:
					return UIConstants.InflationState;
				case CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION:
					return UIConstants.TransitionState;
				case CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_ABLATION:
					return UIConstants.AblationState;
				case CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING:
					return UIConstants.ThawingState;
				case CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_EXCEPTION:
					return UIConstants.ExceptionState;
				default:
					throw new ArgumentOutOfRangeException(nameof(state), state, null);
			}
		}
	}
}
