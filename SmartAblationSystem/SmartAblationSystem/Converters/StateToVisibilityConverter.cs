using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using static Communication.CanBusMessageDefinition;

namespace SmartAblationSystem.Converters
{
  /// <summary>
  /// This class converts a State To Visibility
  /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
  /// </summary>
  internal class StateToVisibilityConverter : IValueConverter
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
      Visibility isVisible = Visibility.Hidden;

      if (parameter != null)
      {
        if (parameter.ToString() == "START")
        {
          switch ((int)value)
          {
            case (int)MessageStateId.CAN_ID_STATE_READY:
            case (int)MessageStateId.CAN_ID_STATE_INFLATION:
            case (int)MessageStateId.CAN_ID_STATE_THAWING:
              isVisible = Visibility.Visible;
              break;

            case (int)MessageStateId.CAN_ID_STATE_IDLE:
            case (int)MessageStateId.CAN_ID_STATE_ABLATION:
              isVisible = Visibility.Hidden;
              break;
          }
        }
        else if (parameter.ToString() == "STOP")
        {
          switch ((int)value)
          {
            case (int)MessageStateId.CAN_ID_STATE_READY:
            case (int)MessageStateId.CAN_ID_STATE_INFLATION:
            case (int)MessageStateId.CAN_ID_STATE_TRANSITION:
            case (int)MessageStateId.CAN_ID_STATE_ABLATION:
            case (int)MessageStateId.CAN_ID_STATE_THAWING:
              isVisible = Visibility.Visible;
              break;

            case (int)MessageStateId.CAN_ID_STATE_IDLE:
              isVisible = Visibility.Hidden;
              break;
          }
        }
        else if (parameter.ToString() == "VACUUM")
        {
          switch ((int)value)
          {
            case (int)MessageStateId.CAN_ID_STATE_READY:
            case (int)MessageStateId.CAN_ID_STATE_ABLATION:
            case (int)MessageStateId.CAN_ID_STATE_THAWING:
              isVisible = Visibility.Visible;
              break;

            case (int)MessageStateId.CAN_ID_STATE_IDLE:
            case (int)MessageStateId.CAN_ID_STATE_INFLATION:
              isVisible = Visibility.Hidden;
              break;
          }
        }
        else if (parameter.ToString() == "TABS")
        {
          isVisible = Visibility.Visible;
        }
        else if (parameter.ToString() == "TREATMENT")
        {
          isVisible = Visibility.Visible;
        }
        else if (parameter.ToString() == "HOME" || parameter.ToString() == "UserManual")
        {
          switch ((int)value)
          {
            case (int)MessageStateId.CAN_ID_STATE_IDLE:
            case (int)MessageStateId.CAN_ID_STATE_READY:
            case (int)MessageStateId.CAN_ID_STATE_EXCEPTION:
            case (int)MessageStateId.CAN_ID_STATE_UNKNOWN:

              isVisible = Visibility.Visible;
              break;

            case (int)MessageStateId.CAN_ID_STATE_INFLATION:
            case (int)MessageStateId.CAN_ID_STATE_TRANSITION:
            case (int)MessageStateId.CAN_ID_STATE_ABLATION:
            case (int)MessageStateId.CAN_ID_STATE_THAWING:

              isVisible = Visibility.Hidden;
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
              isVisible = Visibility.Visible;
              break;
            case (int)MessageStateId.CAN_ID_STATE_INFLATION:
            case (int)MessageStateId.CAN_ID_STATE_TRANSITION:
            case (int)MessageStateId.CAN_ID_STATE_ABLATION:
            case (int)MessageStateId.CAN_ID_STATE_THAWING:
              isVisible = Visibility.Collapsed;
              break;
          }
        }
        else if (parameter.ToString() == "ABLATION_TIMER" ||
                 parameter.ToString() == "TARGET_TEMPERATURE" ||
                 parameter.ToString() == "ABLATION_SITE" ||
                 parameter.ToString() == "TREATMENT_NOTES" ||
                 parameter.ToString() == "CATHETER_TYPE" ||
                 parameter.ToString() == "VOLUME")
        {
          isVisible = Visibility.Visible;
        }
        else if (parameter.ToString() == "TIME_TO_THAW")
        {
          switch ((int)value)
          {
            case (int)MessageStateId.CAN_ID_STATE_IDLE:
            case (int)MessageStateId.CAN_ID_STATE_INFLATION:
            case (int)MessageStateId.CAN_ID_STATE_ABLATION:
              isVisible = Visibility.Hidden;
              break;

            case (int)MessageStateId.CAN_ID_STATE_THAWING:
              isVisible = Visibility.Visible;
              break;
          }
        }
        else if (parameter.ToString() == "TIME_TO_TEMPERATURE")
        {
          switch ((int)value)
          {
            case (int)MessageStateId.CAN_ID_STATE_IDLE:
            case (int)MessageStateId.CAN_ID_STATE_INFLATION:
              isVisible = Visibility.Hidden;
              break;

            case (int)MessageStateId.CAN_ID_STATE_THAWING:
            case (int)MessageStateId.CAN_ID_STATE_ABLATION:
              isVisible = Visibility.Visible;
              break;
          }
        }

        else if (parameter.ToString() == "CatheterAndBallonState")
        {
          switch ((int)value)
          {
            case (int)MessageStateId.CAN_ID_STATE_IDLE:
            case (int)MessageStateId.CAN_ID_STATE_READY:
            case (int)MessageStateId.CAN_ID_STATE_INFLATION:
            case (int)MessageStateId.CAN_ID_STATE_THAWING:
              isVisible = Visibility.Visible;
              break;

            case (int)MessageStateId.CAN_ID_STATE_TRANSITION:
            case (int)MessageStateId.CAN_ID_STATE_ABLATION:
              isVisible = Visibility.Hidden;
              break;
          }
        }

        else if (parameter.ToString() == "CatheterTube")
        {
          switch ((int)value)
          {

            case (int)MessageStateId.CAN_ID_STATE_EXCEPTION:
            case (int)MessageStateId.CAN_ID_STATE_UNKNOWN:

              isVisible = Visibility.Hidden;
              break;

            default:
              isVisible = Visibility.Visible;
              break;
          }
        }

        else if (parameter.ToString() == "LowFlow")
        {
          switch ((int)value)
          {
            case (int)MessageStateId.CAN_ID_STATE_INFLATION:
            case (int)MessageStateId.CAN_ID_STATE_ABLATION:
              isVisible = Visibility.Visible;
              break;

            case (int)MessageStateId.CAN_ID_STATE_IDLE:
            case (int)MessageStateId.CAN_ID_STATE_READY:
            case (int)MessageStateId.CAN_ID_STATE_TRANSITION:
            case (int)MessageStateId.CAN_ID_STATE_THAWING:
            case (int)MessageStateId.CAN_ID_STATE_EXCEPTION:
              isVisible = Visibility.Hidden;
              break;
          }
        }
        //else if (parameter.ToString() == "ABLATION_TIMER" ||
        //        parameter.ToString() == "TARGET_TEMPERATURE" ||
        //        parameter.ToString() == "ABLATION_SITE" ||
        //        parameter.ToString() == "TREATMENT_NOTES" ||
        //        parameter.ToString() == "CATHETER_TYPE" ||
        //        parameter.ToString() == "VOLUME")
        //{
        //    isVisible = Visibility.Visible;
        //}

        else if (parameter.ToString() == "BASE_LINE")
        {
          switch ((int)value)
          {

            case (int)MessageStateId.CAN_ID_STATE_ABLATION:
            case (int)MessageStateId.CAN_ID_STATE_TRANSITION:
            case (int)MessageStateId.CAN_ID_STATE_THAWING:
              isVisible = Visibility.Visible;
              break;

            case (int)MessageStateId.CAN_ID_STATE_IDLE:
            case (int)MessageStateId.CAN_ID_STATE_READY:
            case (int)MessageStateId.CAN_ID_STATE_INFLATION:
            case (int)MessageStateId.CAN_ID_STATE_EXCEPTION:
              isVisible = Visibility.Hidden;
              break;
          }
        }

        else if (parameter.ToString() == "BASE_LINE2")
        {
          switch ((int)value)
          {
            case (int)MessageStateId.CAN_ID_STATE_INFLATION:
            case (int)MessageStateId.CAN_ID_STATE_ABLATION:
            case (int)MessageStateId.CAN_ID_STATE_TRANSITION:
            case (int)MessageStateId.CAN_ID_STATE_THAWING:
              isVisible = Visibility.Visible;
              break;

            case (int)MessageStateId.CAN_ID_STATE_IDLE:
            case (int)MessageStateId.CAN_ID_STATE_READY:
            case (int)MessageStateId.CAN_ID_STATE_EXCEPTION:
              isVisible = Visibility.Hidden;
              break;
          }
        }

      }

      return isVisible;
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