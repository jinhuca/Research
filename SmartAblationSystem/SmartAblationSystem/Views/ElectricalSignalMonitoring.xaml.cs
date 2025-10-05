using SmartAblationSystem.ViewModels;
using System.Threading;
using System.Windows.Controls;

namespace SmartAblationSystem.Views
{
    /// <summary>
    /// Interaction logic for ElectricalSignalMonitoring.xaml
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public partial class ElectricalSignalMonitoring : UserControl
    {
        private ElectricalSignalMonitoringViewModel electricalSignalMonitoringViewModel;

        /// <summary>
        /// Initializes the Electrical Signal Monitoring components.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public ElectricalSignalMonitoring()
        {
            InitializeComponent();
            electricalSignalMonitoringViewModel = this.DataContext as ElectricalSignalMonitoringViewModel;
        }

        /// <summary>
        /// Occurs when the UserControl_Unloaded event is raised.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">The component that raised the event.</param>
        /// <param name="e">Represents the base class for classes that contain user control event data</param>
        private void UserControl_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            //Always disable maintenance mode when quitting except for these screens :
            // Mechanical Panel, Electrical Panel, Flow Curve Programming and PID.
            if (!CommonViewModel.Current.IsMaintenanceModeScreenSelected)
            {
                electricalSignalMonitoringViewModel.EnableOrDisableMaintenanceMode = false;
                CommonViewModel.Current.IsMaintenanceModeScreenSelected = false;
            }

            CommonViewModel.Current.Console.Stop();
            CommonViewModel.Current.Console.InjectionDisable();
            Thread.Sleep(10);
            CommonViewModel.Current.Console.Stop();
        }

        /// <summary>
        /// Occurs when the UserControl_loaded event is raised.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            electricalSignalMonitoringViewModel.SystemState = (int)CommonViewModel.Current.SystemState;

            //force the refresh of Maintenance mode button
            electricalSignalMonitoringViewModel.EnableOrDisableMaintenanceMode = electricalSignalMonitoringViewModel.EnableOrDisableMaintenanceMode;
            electricalSignalMonitoringViewModel.LockTheFootSwitch = true;
            electricalSignalMonitoringViewModel.RefreshInflationSpeedMode();
        }
    }
}