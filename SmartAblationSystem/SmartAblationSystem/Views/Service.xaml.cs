using SmartAblationSystem.ViewModels;
using System.Windows.Controls;

namespace SmartAblationSystem.Views
{
    /// <summary>
    /// Interaction logic for Service.xaml
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public partial class Service : UserControl
    {
        /// <summary>
        /// Initializes Service components.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public Service()
        {
            InitializeComponent();
        }


        /// <summary>
        /// Occurs when UserControl_Loaded event is raised.
        /// IEC 62304 Class A
        /// </summary>
        /// <param name="sender">The component that raised the event.</param>
        /// <param name="e">Contains state information and event data associated with a routed event.</param>
        private void UserControl_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            //Make sure to disable maintenance mode when exiting Maintenance screens.
            CommonViewModel.Current.IsMaintenanceModeScreenSelected = false;
        }
    }
}