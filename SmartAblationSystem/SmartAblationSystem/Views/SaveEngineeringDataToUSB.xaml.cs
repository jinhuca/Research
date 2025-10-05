using SmartAblationSystem.ViewModels;
using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SmartAblationSystem.Views
{
    /// <summary>
    /// Interaction logic for SaveEngineeringDataToUSB.xaml
    /// </summary>
    public partial class SaveEngineeringDataToUSB : Window
    {
        public SaveEngineeringDataToUSB(object dataContext)
        {
            InitializeComponent();
            this.DataContext = dataContext as ConsoleErrorLogViewModel;
            //ConsoleErrorLog tv = (ConsoleErrorLog)this.DataContext;
            
            
        }

        /// <summary>
        /// Occurs when the Ok_Click event is raised.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">The component that raised the event.</param>
        /// <param name="e">Contains state information and event data associated with a routed event.</param>
        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DialogResult = true;
                ConsoleErrorLogViewModel tv = (ConsoleErrorLogViewModel)this.DataContext;
                       
                tv.FilePassword = passwordBox.Text;

                this.Close();
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }


        /// <summary>
        /// Occurs when the No_Click event is raised.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">The component that raised the event.</param>
        /// <param name="e">Contains state information and event data associated with a routed event.</param>
        private void No_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        /// <summary>
        /// Occurs when the Password changed event is raised.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">The component that raised the event.</param>
        /// <param name="e">Contains state information and event data associated with a routed event.</param>
        private void PasswordChangedHandler(object sender, RoutedEventArgs e)
        {
            Regex objAlphaNumericPattern = new Regex("^[a-zA-Z0-9]*$"); //^[a-zA-Z0-9_.-]*$

            if (objAlphaNumericPattern.IsMatch(passwordBox.Text))
            {
                lblValidationMessage.Visibility = Visibility.Hidden;
                if (passwordBox.Text.Length > 0 && (CSVRadioButton.IsChecked == true || PDFRadioButton.IsChecked == true || JSONRadioButton.IsChecked == true ))
                    Yes.IsEnabled = true;
                else
                    Yes.IsEnabled = false;
            }
            else
            {
                lblValidationMessage.Visibility = Visibility.Visible;
                Yes.IsEnabled = false;
            }
        }

        /// <summary>
        /// Occurs when the JSon selected event is raised.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">The component that raised the event.</param>
        /// <param name="e">Contains state information and event data associated with a routed event.</param>
        private void JSonSelected(object sender, RoutedEventArgs e)
        {
            ConsoleErrorLog tv = (ConsoleErrorLog)this.DataContext;
            try
            {
                //if (tv.IsCryterionUser || tv.IsBSCADMINUser)
                //{
                //    if (JSONRadioButton.IsChecked == true && CSVRadioButton.IsChecked == false && PDFRadioButton.IsChecked == false && ReoprtRadioButton.IsChecked == false)
                //    {
                //        Yes.IsEnabled = true;
                //        PasswordPanel.Visibility = Visibility.Hidden;
                //        passwordBox.DataContext = "aaaa";
                //    }
                //    else if (JSONRadioButton.IsChecked == true && passwordBox.Text.Length > 0)
                //        Yes.IsEnabled = true;
                //}
                //else
                //{
                //    if (passwordBox.Text.Length == 0) Yes.IsEnabled = false;
                //    else if (CSVRadioButton.IsChecked == false && PDFRadioButton.IsChecked == false && JSONRadioButton.IsChecked == false && ReoprtRadioButton.IsChecked == false)
                //        Yes.IsEnabled = false;
                //    else Yes.IsEnabled = true;

                //    PasswordPanel.Visibility = Visibility.Visible;
                //    passwordBox.DataContext = "";
                //}
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        /// <summary>
        /// Occurs when the JSon Not Selected event is raised.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">The component that raised the event.</param>
        /// <param name="e">Contains state information and event data associated with a routed event.</param>
        private void JSonNotSelected(object sender, RoutedEventArgs e)
        {
            try
            {
                if (passwordBox.Text.Length == 0) Yes.IsEnabled = false;
                else if (CSVRadioButton.IsChecked == false && PDFRadioButton.IsChecked == false && JSONRadioButton.IsChecked == false)
                    Yes.IsEnabled = false;
                else Yes.IsEnabled = true;

                PasswordPanel.Visibility = Visibility.Visible;
                passwordBox.DataContext = "";
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

    }
}
