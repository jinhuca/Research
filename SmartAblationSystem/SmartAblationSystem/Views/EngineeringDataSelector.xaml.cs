using System;
using System.Windows;

namespace SmartAblationSystem.Views
{
    /// <summary>
    /// Interaction logic for EngineeringDataSelector.xaml
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public partial class EngineeringDataSelector : Window
    {
        /// <summary>
        /// Initializes Engineering Data Selector components.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="dataContext">An object representing the Data Context.</param>
        public EngineeringDataSelector(object dataContext)
        {
            InitializeComponent();

            if (dataContext != null)
            {
                this.DataContext = dataContext;
            }
        }

        /// <summary>
        /// Occurs when the Cancel_Click event is raised.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">The component that raised the event.</param>
        /// <param name="e">Contains state information and event data associated with a routed event.</param>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DialogResult = false;
                this.Close();
            }
            catch (Exception ex)
            {
                // TODO
                ex.ToString();
            }
        }

        /// <summary>
        /// Occurs when the OkButton_Click event is raised.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">The component that raised the event.</param>
        /// <param name="e">Contains state information and event data associated with a routed event.</param>
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }
    }
}