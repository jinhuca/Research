using System;
using System.Globalization;
using System.Windows.Data;
using static Communication.CanBusMessageDefinition;

namespace SmartAblationSystem.Converters
{
  /// <summary>
  /// This class converts a State to Enable (boolean) value
  /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
  /// </summary>
  internal class StateToEnableConverter : IValueConverter
  {
    #region IValueConverter Members

    /// <summary>
    /// Converts a value to a target type depending on the object received in parameter
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="value">An object to convert.</param>
    /// <param name="targetType">A Type representing the conversion target type.</param>
    /// <param name="parameter">An object representing the conversion's parameter.</param>
    /// <param name="culture">Provides information about a specific culture (called a locale for unmanaged).</param>
    /// <returns>An object converted to the target type.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      bool isEnable = false;

      if (parameter != null)
      {
        if (parameter.ToString() == "START")
        {
          switch ((int)value)
          {
            case (int)MessageStateId.CAN_ID_STATE_READY:
            case (int)MessageStateId.CAN_ID_STATE_INFLATION:
            case (int)MessageStateId.CAN_ID_STATE_THAWING:
              isEnable = true;
              break;

            case (int)MessageStateId.CAN_ID_STATE_IDLE:
            case (int)MessageStateId.CAN_ID_STATE_ABLATION:

              isEnable = false;
              break;
          }
        }
        else if (parameter.ToString() == "STOP")
        {
          switch ((int)value)
          {
            case (int)MessageStateId.CAN_ID_STATE_INFLATION:
            case (int)MessageStateId.CAN_ID_STATE_TRANSITION:
            case (int)MessageStateId.CAN_ID_STATE_ABLATION:
            case (int)MessageStateId.CAN_ID_STATE_THAWING:
              isEnable = true;
              break;

            case (int)MessageStateId.CAN_ID_STATE_READY:
            case (int)MessageStateId.CAN_ID_STATE_IDLE:
              isEnable = false;
              break;
          }
        }
        else if (parameter.ToString() == "VACUUM")
        {
          switch ((int)value)
          {
            case (int)MessageStateId.CAN_ID_STATE_READY:
              isEnable = true;
              break;

            case (int)MessageStateId.CAN_ID_STATE_IDLE:
            case (int)MessageStateId.CAN_ID_STATE_INFLATION:
            case (int)MessageStateId.CAN_ID_STATE_ABLATION:
            case (int)MessageStateId.CAN_ID_STATE_THAWING:
              isEnable = false;
              break;
          }
        }
        else if (parameter.ToString() == "TABS")
        {
          switch ((int)value)
          {
            case (int)MessageStateId.CAN_ID_STATE_IDLE:
            case (int)MessageStateId.CAN_ID_STATE_READY:
              isEnable = true;
              break;

            case (int)MessageStateId.CAN_ID_STATE_INFLATION:
            case (int)MessageStateId.CAN_ID_STATE_ABLATION:
            case (int)MessageStateId.CAN_ID_STATE_THAWING:
              isEnable = false;
              break;
          }
        }
        else if (parameter.ToString() == "TREATMENT")
        {
          switch ((int)value)
          {
            case (int)MessageStateId.CAN_ID_STATE_IDLE:
            case (int)MessageStateId.CAN_ID_STATE_READY:
              isEnable = true;
              break;

            case (int)MessageStateId.CAN_ID_STATE_INFLATION:
            case (int)MessageStateId.CAN_ID_STATE_ABLATION:
            case (int)MessageStateId.CAN_ID_STATE_THAWING:
              isEnable = false;
              break;
          }
        }
        else if (parameter.ToString() == "HOME" || parameter.ToString() == "UserManual")
        {
          switch ((int)value)
          {
            case (int)MessageStateId.CAN_ID_STATE_IDLE:
            case (int)MessageStateId.CAN_ID_STATE_READY:
            case (int)MessageStateId.CAN_ID_STATE_EXCEPTION:
            case (int)MessageStateId.CAN_ID_STATE_UNKNOWN:
              isEnable = true;
              break;

            case (int)MessageStateId.CAN_ID_STATE_INFLATION:
            case (int)MessageStateId.CAN_ID_STATE_ABLATION:
            case (int)MessageStateId.CAN_ID_STATE_THAWING:
              isEnable = false;
              break;
          }
        }
        else if (parameter.ToString() == "VIEWCHANGE")
        {
          switch ((int)value)
          {
            case (int)MessageStateId.CAN_ID_STATE_IDLE:
            case (int)MessageStateId.CAN_ID_STATE_READY:
            case (int)MessageStateId.CAN_ID_STATE_EXCEPTION:
            case (int)MessageStateId.CAN_ID_STATE_UNKNOWN:
              isEnable = true;
              break;
            case (int)MessageStateId.CAN_ID_STATE_INFLATION:
            case (int)MessageStateId.CAN_ID_STATE_ABLATION:
            case (int)MessageStateId.CAN_ID_STATE_THAWING:
              isEnable = false;
              break;
          }
        }
        else if (parameter.ToString() == "TREATMENT_NOTES")
        {
          switch ((int)value)
          {
            case (int)MessageStateId.CAN_ID_STATE_INFLATION:

              isEnable = false;
              break;


            case (int)MessageStateId.CAN_ID_STATE_IDLE:
            case (int)MessageStateId.CAN_ID_STATE_READY:
            case (int)MessageStateId.CAN_ID_STATE_ABLATION:
            case (int)MessageStateId.CAN_ID_STATE_THAWING:
              isEnable = true;
              break;
          }
        }
        else if (parameter.ToString() == "ABLATION_TIMER" ||
                 parameter.ToString() == "TARGET_TEMPERATURE" ||
                 parameter.ToString() == "ABLATION_SITE" ||
                 parameter.ToString() == "CATHETER_TYPE" ||
                 parameter.ToString() == "VOLUME")
        {
          isEnable = true;
        }

        else if (parameter.ToString() == "RequiredTime")
        {
          switch ((int)value)
          {
            case (int)MessageStateId.CAN_ID_STATE_THAWING:

              isEnable = false;
              break;


            case (int)MessageStateId.CAN_ID_STATE_IDLE:
            case (int)MessageStateId.CAN_ID_STATE_READY:
            case (int)MessageStateId.CAN_ID_STATE_INFLATION:
            case (int)MessageStateId.CAN_ID_STATE_TRANSITION:
            case (int)MessageStateId.CAN_ID_STATE_ABLATION:
            case (int)MessageStateId.CAN_ID_STATE_EXCEPTION:
              isEnable = true;
              break;
          }
        }

        else if (parameter.ToString() == "BASE_LINE")
        {
          switch ((int)value)
          {

            case (int)MessageStateId.CAN_ID_STATE_ABLATION:
            case (int)MessageStateId.CAN_ID_STATE_TRANSITION:
            case (int)MessageStateId.CAN_ID_STATE_THAWING:
              isEnable = true;
              break;

            case (int)MessageStateId.CAN_ID_STATE_IDLE:
            case (int)MessageStateId.CAN_ID_STATE_READY:
            case (int)MessageStateId.CAN_ID_STATE_INFLATION:
            case (int)MessageStateId.CAN_ID_STATE_EXCEPTION:
              isEnable = false;
              break;
          }
        }
        else if (parameter.ToString() == "BASE_LINE2")
        {
          switch ((int)value)
          {

            case (int)MessageStateId.CAN_ID_STATE_ABLATION:
            case (int)MessageStateId.CAN_ID_STATE_TRANSITION:
            case (int)MessageStateId.CAN_ID_STATE_INFLATION:
            case (int)MessageStateId.CAN_ID_STATE_THAWING:
              isEnable = true;
              break;

            case (int)MessageStateId.CAN_ID_STATE_IDLE:
            case (int)MessageStateId.CAN_ID_STATE_READY:
            case (int)MessageStateId.CAN_ID_STATE_EXCEPTION:
              isEnable = false;
              break;
          }
        }

      }

      return isEnable;
    }

    /// <summary>
    /// Converts back an object to a target type depending on the object received in parameter
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="value">An object to convert back.</param>
    /// <param name="targetType">A Type representing the conversion target type.</param>
    /// <param name="parameter">An object representing the conversion's parameter.</param>
    /// <param name="culture">Provides information about a specific culture (called a locale for unmanaged).</param>
    /// <returns>An object converted to the target type.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    #endregion IValueConverter Members
  }
}