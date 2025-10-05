using SmartAblationSystem.Helpers;
using SmartAblationSystem.ViewModels;
using System;
using System.Globalization;
using System.Windows.Data;
using static System.Globalization.CultureInfo;
using static Communication.CanBusMessageDefinition;

namespace SmartAblationSystem.ConvertersNew
{
	internal class GasLevelConverter : IMultiValueConverter
	{
		private const double FLOW_SETPOINT_BASE = 7800d;
		private const double GAS_CONSUMPTION_PER_MIN = 0.0313;

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			var refrigerantUnit = (string)parameter;
			if (refrigerantUnit == null)
			{
				throw new ArgumentException();
			}

			var lc1Reading = (double)values[0];
			var isDasBalloonEnabled = (bool)values[1];
			var isLowFlowActivated = (bool)values[2];
			var ablationDuration = (int)values[3];

			if(refrigerantUnit == "Min" || refrigerantUnit == "Ablation")
			{
				var localCommonViewModel = CommonViewModel.Current;
				var targetInjectionLowFlow = localCommonViewModel.Console.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachine[MessageStateId.CAN_ID_STATE_ABLATION].TargetInjectionLowFlow;
				var dasLowFlow = localCommonViewModel.Console.PatientMicroControllerCryoBalloonConfigurationValueAccordingToTheStateMachine[MessageStateId.CAN_ID_STATE_ABLATION].DASLowFlow;
				var targetInjectionFlow = localCommonViewModel.InflateDeflateBalloonModel.CurrentFlowAndPressureRegulatorValueAccordingToTheStateMachine[MessageStateId.CAN_ID_STATE_ABLATION].TargetInjectionFlow;

				var flowSetPoint = isLowFlowActivated
					? isDasBalloonEnabled ? dasLowFlow : targetInjectionLowFlow
					: targetInjectionFlow;
				var remainingTimeInMin = GetRemainingTimeInMinutes(lc1Reading, flowSetPoint);

				return refrigerantUnit == "Min"
					? System.Convert.ToInt32(remainingTimeInMin).ToString()
					: System.Convert.ToInt32(remainingTimeInMin * 60 / ablationDuration).ToString();
			}

			return Math.Round(Scale.CurrentWeightUnit == Enumeration.WeightUnit.Lbs ? lc1Reading : Scale.ConvertLbToKg(lc1Reading), 1).ToString(InvariantCulture);
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		private static double GetRemainingTimeInMinutes(double LC1Value, double flowSetPointInAblation)
		{
			var loadCellThresholdFail = CommonViewModel.Current.Console.LoadCellOneValueAccordingToTheStateMachine[MessageStateId.CAN_ID_STATE_ABLATION].LoadCellThresholdFail -
			                            CommonViewModel.Current.Console.Tank.MetalWeight;

			//Adding more robusteness for time to lBS. when the catheter is not connected the threshold are not updated 
			if(loadCellThresholdFail < 0)
				return 0;

			var gasConsumptionPerMin = GAS_CONSUMPTION_PER_MIN * flowSetPointInAblation / FLOW_SETPOINT_BASE;
			var time = (LC1Value - loadCellThresholdFail) / gasConsumptionPerMin;
			return time > 0 ? time : 0;
		}
	}
}
