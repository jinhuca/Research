using SmartAblationSystem.ViewModels;
using System;
using System.Windows;

namespace SmartAblationSystem.Views
{
    /// <summary>
    /// Interaction logic for AddEditUser.xaml
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public partial class UpdateVeinIsolationDuration : Window
    {
        //private ManageUsersViewModel dt;

        /// <summary>
        /// Initializes Add/Edit User components.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="dataContext">An object representing the Data Context.</param>
        public UpdateVeinIsolationDuration(int currentDuration, int maxDuration)
        {
            InitializeComponent();

            ((UpdateVeinIsolationDurationViewModel)this.DataContext).SetCurrentAndMaxDuration(currentDuration, maxDuration);
        }

        /// <summary>
        /// Occurs when the OK_Click event is raised.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">The component that raised the event.</param>
        /// <param name="e">Contains state information and event data associated with a routed event.</param>
        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (((UpdateVeinIsolationDurationViewModel)this.DataContext).IsInfoValid)
                {
                    DialogResult = true;
                    this.Close();
                }
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
        
    }
}