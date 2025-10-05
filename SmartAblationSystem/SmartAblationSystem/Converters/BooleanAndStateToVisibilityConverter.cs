using SmartAblationSystem.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace SmartAblationSystem.Converters
{
	/// <summary>
	/// Converts a value to a target type depending on the object received in parameter
	/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
	/// </summary>
	class BooleanAndStateToVisibilityConverter : IMultiValueConverter
	{
		bool _SensorBoolValue = false;
		string state = string.Empty;
		bool playBackBoolValue = false;
		bool isIncryscreen = false;
		bool isVeinIsolated = false;

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
			if(parameter?.ToString() == "VerifyState")
			{
				_SensorBoolValue = System.Convert.ToBoolean(values[0]);
				state = values[1].ToString();
				playBackBoolValue = System.Convert.ToBoolean(values[2]);

				if((_SensorBoolValue && state != "CAN_ID_STATE_IDLE" && state != "CAN_ID_STATE_READY" && state != "CAN_ID_STATE_INFLATION") || playBackBoolValue)
				{
					return Visibility.Visible;
				}

				else
				{
					return Visibility.Collapsed;
				}

			}

			if(parameter?.ToString() == "TimeToTargetTemperature")
			{
				_SensorBoolValue = System.Convert.ToBoolean(values[0]);
				state = values[1].ToString();

				if(_SensorBoolValue || state == "CAN_ID_STATE_IDLE")
				{
					return Visibility.Visible;
				}

				else
				{
					return Visibility.Collapsed;
				}

			}

			if(parameter?.ToString() == "UserManual")
			{
				_SensorBoolValue = System.Convert.ToBoolean(values[0]);
				state = values[1].ToString();

				if(!_SensorBoolValue &&
						(state == "CAN_ID_STATE_IDLE" ||
						state == "CAN_ID_STATE_READY" ||
						state == "CAN_ID_STATE_EXCEPTION" ||
						state == "CAN_ID_STATE_UNKNOWN"))
				{
					return Visibility.Visible;
				}

				else if(state == "CAN_ID_STATE_INFLATION" ||
						state == "CAN_ID_STATE_TRANSITION" ||
						state == "CAN_ID_STATE_ABLATION" ||
						state == "CAN_ID_STATE_THAWING" || _SensorBoolValue)
				{
					return Visibility.Hidden;
				}

			}

			if(parameter?.ToString() == "OcclusionPressureShow")
			{
				state = values[0].ToString();


				if(((state == "CAN_ID_STATE_IDLE") || (state == "CAN_ID_STATE_READY") || (state == "CAN_ID_STATE_INFLATION")) && values[1].ToString().ToLower() == "true" && values[2].ToString().ToLower() == "true")
				{
					return Visibility.Visible;
				}

				else
				{
					return Visibility.Collapsed;
				}

			}

			if(parameter?.ToString() == "OcclusionPressureInflationShow")
			{
				state = values[0].ToString();


				if(((state == "CAN_ID_STATE_IDLE") || (state == "CAN_ID_STATE_READY") || (state == "CAN_ID_STATE_INFLATION")) && values[1].ToString().ToLower() == "true" && values[2].ToString().ToLower() == "true")
				{
					return Visibility.Visible;
				}

				else
				{
					return Visibility.Collapsed;
				}

			}
			if(parameter?.ToString() == "TEMPTTI" || parameter?.ToString() == "TTI")
			{
				_SensorBoolValue = System.Convert.ToBoolean(values[0]);
				state = values[1].ToString();
				playBackBoolValue = System.Convert.ToBoolean(values[2]);
				// isVeinIsolated = System.Convert.ToBoolean(values[3]);
				int veinIsolationDuration = System.Convert.ToInt32(values[3]);
				if(veinIsolationDuration > 0 && (state == "CAN_ID_STATE_TRANSITION" || state == "CAN_ID_STATE_ABLATION" || state == "CAN_ID_STATE_THAWING") && _SensorBoolValue)
				{
					return Visibility.Visible;
				}
				else if(playBackBoolValue && veinIsolationDuration > 0)
				{
					return Visibility.Visible;
				}
				else
				{
					return Visibility.Collapsed;
				}

			}

			try
			{
				_SensorBoolValue = System.Convert.ToBoolean(values[0]);
				state = values[1].ToString();
				playBackBoolValue = System.Convert.ToBoolean(values[2]);
				isIncryscreen = System.Convert.ToBoolean(values[3]);

				if(state == "CAN_ID_STATE_INFLATION" ||
						state == "CAN_ID_STATE_TRANSITION" ||
						state == "CAN_ID_STATE_ABLATION" ||
						state == "CAN_ID_STATE_THAWING"
					 )
				{

					return Visibility.Hidden;
				}

				else if(!_SensorBoolValue && playBackBoolValue && isIncryscreen && AblationInformation.IsThereAbltionHistoricalData)
				{
					return Visibility.Visible;
				}
			}

			catch(Exception ex)
			{
				ex.ToString();
				//avoid exception 
			}



			return Visibility.Hidden;
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
