using System;
using System.Collections.Generic;
using System.Windows.Data;
using SmartAblationSystem.Models;

namespace SmartAblationSystem.Converters
{
    class CatheterIDToNameMult : IValueConverter
    {
        #region IValueConverter Members
        const int ID28mm = (int)Helpers.Enumeration.CatheterType.ID28mm;
        const int plus = (int)Helpers.Enumeration.CatheterType.Plus;

        string[] catheterDescriptionID28mm = Languages.CatheterDescription[ID28mm].Split('-');
        string[] catheterDescriptionplus = Languages.CatheterDescription[plus].Split('-');

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
            string returnvaluestring = "";
            List<Helpers.Enumeration.CatheterType> TypeList = ((List<Helpers.Enumeration.CatheterType>)value);
            if (parameter != null && TypeList!=null)
            {
                    foreach (Helpers.Enumeration.CatheterType item in TypeList)
                    {
                        if (parameter.ToString() == "CatheterDescriptionSplit")
                        {
                            switch ((int)item)
                            {
                                case ID28mm:
                                    returnvaluestring += catheterDescriptionID28mm[0] + " " + catheterDescriptionID28mm[2] + ", ";
                                break;

                                case plus:
                                    returnvaluestring += catheterDescriptionplus[0] + " " + catheterDescriptionplus[2] + " , ";
                                break;
                                default:
                                    returnvaluestring += "Test Catheter, ";
                                    break;
                            }
                          //  return returnvaluestring;
                        }

                    }

                    if (returnvaluestring.Length > 2)
                    {
	                    returnvaluestring = returnvaluestring.Substring(0, returnvaluestring.Length - 2);
                    }
                    return returnvaluestring;
            }
            else
                return "--";
              
              
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
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string strvalue = value as string;

            return System.Convert.ToInt32(strvalue);
        }

        #endregion IValueConverter Members
    }
}
