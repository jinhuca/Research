using SmartAblationSystem.ViewModels;
using System.Windows.Controls;

namespace SmartAblationSystem.Views
{
    /// <summary>
    /// Interaction logic for TCsAndLCCalibration.xaml
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public partial class TCsAndLCCalibration : UserControl
    {
        private TCsAndLCCalibrationViewModel tCsAndLCCalibrationViewModel;
        /// <summary>
        /// Initializes TCsAndLCCalibration components.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public TCsAndLCCalibration()
        {
            InitializeComponent();

            tCsAndLCCalibrationViewModel = this.DataContext as TCsAndLCCalibrationViewModel;
        }


        /// <summary>
        /// Occurs when UserControl_Loaded event is raised.
        /// IEC 62304 Class A
        /// </summary>
        /// <param name="sender">The component that raised the event.</param>
        /// <param name="e">Contains state information and event data associated with a routed event.</param>
        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            CommonViewModel.Current.Console.GUIInMaintenanceMode = true;
            tCsAndLCCalibrationViewModel.IsLoadCellEnabled = false;
            tCsAndLCCalibrationViewModel.ResetDisplay();
            tCsAndLCCalibrationViewModel.ReadBackCalibrationFactorCommand.Execute(null);
        }

        /// <summary>
        /// Occurs when UserControl_UnLoaded event is raised.
        /// IEC 62304 Class A
        /// </summary>
        /// <param name="sender">The component that raised the event.</param>
        /// <param name="e">Contains state information and event data associated with a routed event.</param>
        private void UserControl_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
             CommonViewModel.Current.Console.GUIInMaintenanceMode = false;
        }


		/// <summary>
        /// Frontend field validation.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void FieldValidation(object sender, System.EventArgs e)
        {
            string txtValue = "";
            double numValue;
            txtValue = txtCalibrationFactorValue.Text;
           
            if (txtValue.Trim() == "")
            {
                Factorbutton.IsEnabled = false;
                lblMessage.Content = "Value is required";
            }
            else if (!double.TryParse(txtValue, out numValue))
            {
                Factorbutton.IsEnabled = false;
                lblMessage.Content = "Invalid input";
            }
            else if (numValue<=1 || numValue >=3)
            {
                Factorbutton.IsEnabled = false;
                lblMessage.Content = "Invalid value range";
            }
            else
            {
                Factorbutton.IsEnabled = true;
                lblMessage.Content = "";
                //txtValue = numValue.ToString("F", CultureInfo.InvariantCulture);   //("F", CultureInfo.CurrentCulture);
            }
        }

        /// <summary>
        /// Reset Value.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void ResetValue(object sender, System.EventArgs e)
        {
            txtCalibrationFactorValue.Text = "";
            lblMessage.Content = "";
            Factorbutton.IsEnabled = true;
        }
    }
}