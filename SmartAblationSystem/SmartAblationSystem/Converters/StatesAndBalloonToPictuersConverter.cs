using System;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static Communication.CanBusMessageDefinition;

namespace SmartAblationSystem.Converters
{
  /// <summary>
  /// This class converts a value to a target type depending on the object received in parameter
  /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
  /// </summary>
  class StatesAndBalloonToPictuersConverter : IMultiValueConverter
  {

    bool isUsingDASBalloon = false;
    bool iSThePressureSetPointReached = false;
    int state = 0;
    double pressureSetPoint = 0;
    double previousPressureSetPoint = 0;
    const double twentyTwommBalloonPressure = 2.5;
    double CP1Reading = 0;
    readonly bool UseReelTimeInflation = Properties.Settings.Default.UseReelTimeInflation;

    /// <summary>
    /// Converts a value to a target type depending on the object received in parameter
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="value">An object to convert.</param>
    /// <param name="targetType">A Type representing the conversion target type.</param>
    /// <param name="parameter">An object representing the conversion's parameter.</param>
    /// <param name="culture">Provides information about a specific culture (called a locale for unmanaged).</param>
    /// <returns>An object converted to the target type.</returns>
    public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {

      ImageSource imageSource;

      state = System.Convert.ToInt16(values[0]);
      isUsingDASBalloon = System.Convert.ToBoolean(values[1]);
      iSThePressureSetPointReached = System.Convert.ToBoolean(values[2]);
      pressureSetPoint = System.Convert.ToDouble(values[3]);
      CP1Reading = System.Convert.ToDouble(values[4]);
      var isInPlaybackMode = System.Convert.ToBoolean(values[5]);

      if (isInPlaybackMode)
      {
        return  isUsingDASBalloon 
          ? new BitmapImage(new Uri("/Images/Inflation31mm.png", UriKind.Relative)) 
          : new BitmapImage(new Uri("/Images/Inflation28mm.png", UriKind.Relative));
      }

      switch ((int)state)
      {
        case (int)MessageStateId.CAN_ID_STATE_IDLE:
        {
          imageSource = new BitmapImage(new Uri("/Images/Status Idle.png", UriKind.Relative));
        }
        break;

        case (int)MessageStateId.CAN_ID_STATE_READY:
        {
          imageSource = new BitmapImage(new Uri("/Images/Status Ready.png", UriKind.Relative));
        }
        break;

        case (int)MessageStateId.CAN_ID_STATE_INFLATION:
        case (int)MessageStateId.CAN_ID_STATE_THAWING:
        case (int)MessageStateId.CAN_ID_STATE_TRANSITION:
        case (int)MessageStateId.CAN_ID_STATE_ABLATION:

        {
          if (iSThePressureSetPointReached)
          {
            if (isUsingDASBalloon)
            {

              imageSource = new BitmapImage(new Uri("/Images/Inflation31mm.png", UriKind.Relative));
            }
            else
            {
              imageSource = new BitmapImage(new Uri("/Images/Inflation28mm.png", UriKind.Relative));
            }
          }
          else
          {
            imageSource = new BitmapImage(new Uri("/Images/Status Idle.png", UriKind.Relative));

            #region moving  ballon
            if (UseReelTimeInflation)
            {

              if ((CP1Reading / pressureSetPoint) <= 0.10)
                imageSource = new BitmapImage(new Uri("/Images/inf1.png", UriKind.Relative));

              if ((CP1Reading / pressureSetPoint) > 0.10 && (CP1Reading / pressureSetPoint) <= 0.20)
                imageSource = new BitmapImage(new Uri("/Images/inf2.png", UriKind.Relative));

              if ((CP1Reading / pressureSetPoint) > 0.20 && (CP1Reading / pressureSetPoint) <= 0.30)
                imageSource = new BitmapImage(new Uri("/Images/inf3.png", UriKind.Relative));

              if ((CP1Reading / pressureSetPoint) > 0.30 && (CP1Reading / pressureSetPoint) <= 0.40)
                imageSource = new BitmapImage(new Uri("/Images/inf4.png", UriKind.Relative));

              if ((CP1Reading / pressureSetPoint) > 0.40 && (CP1Reading / pressureSetPoint) <= 0.50)
                imageSource = new BitmapImage(new Uri("/Images/inf5.png", UriKind.Relative));

              if ((CP1Reading / pressureSetPoint) > 0.50 && (CP1Reading / pressureSetPoint) <= 0.60)
                imageSource = new BitmapImage(new Uri("/Images/inf6.png", UriKind.Relative));

              if ((CP1Reading / pressureSetPoint) > 0.60 && (CP1Reading / pressureSetPoint) <= 0.70)
                imageSource = new BitmapImage(new Uri("/Images/inf7.png", UriKind.Relative));

              if ((CP1Reading / pressureSetPoint) > 0.70 && (CP1Reading / pressureSetPoint) <= 0.80)
                imageSource = new BitmapImage(new Uri("/Images/inf8.png", UriKind.Relative));

              if ((CP1Reading / pressureSetPoint) > 0.80)
                imageSource = new BitmapImage(new Uri("/Images/inf9.png", UriKind.Relative));
            }

            #endregion


          }
        }
        break;

        //case (int)MessageStateId.CAN_ID_STATE_TRANSITION:
        //case (int)MessageStateId.CAN_ID_STATE_ABLATION:
        //    imageSource = new BitmapImage(new Uri("/Images/Snow Flake.png", UriKind.Relative));
        //    break;

        case (int)MessageStateId.CAN_ID_STATE_EXCEPTION:
          imageSource = new BitmapImage(new Uri("/Images/Background-withoutlogo.jpg", UriKind.Relative));
          break;

        default:
          imageSource = new BitmapImage(new Uri("/Images/Background-withoutlogo.jpg", UriKind.Relative));
          break;
      }

      return imageSource;
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
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotSupportedException("Cannot convert back");
    }
  }
}

