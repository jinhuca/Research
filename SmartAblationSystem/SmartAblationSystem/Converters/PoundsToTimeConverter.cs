using SmartAblationSystem.Helpers;
using SmartAblationSystem.ViewModels;
using System;
using System.Globalization;
using System.Windows.Data;
using static Communication.CanBusMessageDefinition;

namespace SmartAblationSystem.Converters
{
  using Type = System.Type;

  /// <summary>
  /// This class converts pounds to time and back
  /// Ex: A true boolean value will converts to a non-visible visibility value.
  /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
  /// </summary>
  public class PoundsToTimeConverter : IMultiValueConverter
  {
    private const double FLOW_SETPOINT_BASE = 7800d; 
    private const double GAS_CONSUMPTION_PER_MIN = 0.0313;
    
    #region IValueConverter Members

    /// <summary>
    /// Converts a value to a target type depending on the object received in parameter
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="values">An object to convert.</param>
    /// <param name="targetType">A Type representing the conversion target type.</param>
    /// <param name="parameter">An object representing the conversion's parameter.</param>
    /// <param name="culture">Provides information about a specific culture (called a locale for unmanaged).</param>
    /// <returns>An object converted to the target type.</returns>
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      var lc1Reading = (double)values[0];
      var refrigerantUnit = (short)values[1];
      var isDasBalloonEnabled = (bool)values[2];
      var isLowFlowActivated = (bool)values[3];
      var ablationDuration = (int)values[4];

      // Pounds to Time 
      if (refrigerantUnit == 1 || refrigerantUnit == 2)
      {
        var localCommonViewModel = CommonViewModel.Current;
        var targetInjectionLowFlow = localCommonViewModel.Console.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachine[MessageStateId.CAN_ID_STATE_ABLATION].TargetInjectionLowFlow;
        var dasLowFlow = localCommonViewModel.Console.PatientMicroControllerCryoBalloonConfigurationValueAccordingToTheStateMachine[MessageStateId.CAN_ID_STATE_ABLATION].DASLowFlow;
        var targetInjectionFlow = isDasBalloonEnabled 
                                    ? localCommonViewModel.InflateDeflateBalloonModel.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsActivated[MessageStateId.CAN_ID_STATE_ABLATION].TargetInjectionFlow
                                    : localCommonViewModel.InflateDeflateBalloonModel.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsNotActivated[MessageStateId.CAN_ID_STATE_ABLATION].TargetInjectionFlow;

        var flowSetPoint = isLowFlowActivated 
                             ? isDasBalloonEnabled ? dasLowFlow : targetInjectionLowFlow
                             : targetInjectionFlow;
        var remainingTimeInMin = GetRemainingTimeInMinutes(lc1Reading, flowSetPoint);

        return refrigerantUnit == 1
                 ? System.Convert.ToInt32(remainingTimeInMin).ToString()
                 : System.Convert.ToInt32(remainingTimeInMin * 60 / ablationDuration).ToString();
      }

      return Math.Round(Scale.CurrentWeightUnit == Enumeration.WeightUnit.Lbs ? lc1Reading : Scale.ConvertLbToKg(lc1Reading), 1).ToString();
    }

    /// <summary>
    /// Converts back an object to a target type depending on the object received in parameter
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="value">An object to convert back.</param>
    /// <param name="targetTypes">A Type representing the conversion target type.</param>
    /// <param name="parameter">An object representing the conversion's parameter.</param>
    /// <param name="culture">Provides information about a specific culture (called a locale for unmanaged).</param>
    /// <returns>An object array converted to the target type.</returns>
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      return new [] {Binding.DoNothing, Binding.DoNothing, Binding.DoNothing, Binding.DoNothing};
    }

    #endregion IValueConverter Members

    private static double GetRemainingTimeInMinutes(double LC1Value, double flowSetPointInAblation)
    {
      var loadCellThresholdFail = CommonViewModel.Current.Console.LoadCellOneValueAccordingToTheStateMachine[MessageStateId.CAN_ID_STATE_ABLATION].LoadCellThresholdFail -
                              CommonViewModel.Current.Console.Tank.MetalWeight;

      //Adding more robusteness for time to lBS. when the catheter is not connected the threshold are not updated 
      if (loadCellThresholdFail < 0)
        return 0;

      var gasConsumptionPerMin = GAS_CONSUMPTION_PER_MIN * flowSetPointInAblation / FLOW_SETPOINT_BASE;
      var time = (LC1Value - loadCellThresholdFail) / gasConsumptionPerMin;
      return time > 0 ? time : 0;
    }
  }
}
