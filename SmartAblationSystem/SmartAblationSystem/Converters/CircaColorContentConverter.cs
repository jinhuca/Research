using SmartAblationSystem.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace SmartAblationSystem.Converters
{
    internal class CircaColorContentConverter : IValueConverter
    {
        bool isSensorBroken = false;
        Color color = (Color)ColorConverter.ConvertFromString("#FF00afef");

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
           // ViewModels.CommonViewModel localCommonViewModel = ViewModels.CommonViewModel.Current;


            if (parameter != null && value !=null)
            {
                try
                {
                    string[] PArray = parameter.ToString().Split('-');
                    //string Ptype = parameter.ToString()[0].ToString();
                    //string Pvalue = parameter.ToString().Substring(1, parameter.ToString().Length - 1);

                    bool IsLabel = false;
                    if (PArray.Length > 1)
                        IsLabel = true;
                    string Ptype = PArray[0].ToString()[0].ToString();
                    string Pvalue = PArray[0].ToString().Substring(1, PArray[0].ToString().Length - 1);
                    isSensorBroken = ETSdataSortingAndStatus.ChannelStatus[int.Parse(Pvalue)];

                    List<int> lowestTempChannelNums = (List<int>)value;

                    if (Ptype == "C") //C for circle
                    {
                        
                        if (IsLabel)
                        { 
                            //In case all sesnor are broken 
                            if (lowestTempChannelNums.Count == 0 && isSensorBroken)
                                return new SolidColorBrush(Colors.Transparent );

                            foreach (int v in lowestTempChannelNums)
                            {
                                if (v.ToString() == Pvalue)
                                {
                                    return new SolidColorBrush(Colors.Transparent); //System.Windows.Media.Colors.Yellow;
                                }
                                else if (isSensorBroken)
                                {
                                    return new SolidColorBrush(Colors.Transparent);
                                }
                            }
                            return new SolidColorBrush(Colors.White);
                        }
                        else
                        {
                            if (lowestTempChannelNums.Count == 0 && isSensorBroken)
                                return new SolidColorBrush(Colors.Red);

                            foreach (int v in lowestTempChannelNums)
                            {
                                if (v.ToString() == Pvalue)
                                {
                                    return new SolidColorBrush(color); 
                                }
                                else if (isSensorBroken)
                                {
                                    return new SolidColorBrush(Colors.Red);
                                }
                            }
                            return new SolidColorBrush(Colors.Transparent);
                        }

                    }
                    else if (Ptype == "P") //T for texte
                    {
                        foreach (int v in lowestTempChannelNums)
                        {
                            if (v.ToString() == Pvalue)
                            {
                                if (Pvalue == "0")
                                    return "P";
                                else
                                    return v.ToString();
                            }
                            else if (isSensorBroken)
                            {
                                if (Pvalue == "0")
                                    return "P";
                                else
                                    return Pvalue;
                            }
                        }
                        return string.Empty;
                        //return (Pvalue != "0" ? Pvalue : "P");
                    }

                    else if (Ptype == "T") //T for texte
                    {
                        foreach (int v in lowestTempChannelNums)
                        {
                            if (v.ToString() == Pvalue)
                            {
                                if (Pvalue == "0")
                                    return "P";
                                else
                                    return v.ToString();
                            }
                            else if (isSensorBroken)
                            {
                                if (Pvalue == "0")
                                    return "P";
                                else
                                    return Pvalue;
                            }
                        }
                        return string.Empty;
                        //return (Pvalue != "0" ? Pvalue : "P");
                    }
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }
            }
            return "";
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
            return (value == Brushes.Green) ? true : false;
        }
    }
}