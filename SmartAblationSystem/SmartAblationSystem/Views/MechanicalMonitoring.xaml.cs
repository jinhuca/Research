using SmartAblationSystem.ViewModels;
using System.Windows.Controls;
using DevExpress.Mvvm.POCO;

namespace SmartAblationSystem.Views
{
    /// <summary>
    /// Interaction logic for MechanicalMonitoringView.xaml
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public partial class MechanicalMonitoring : UserControl
    {
        private MechanicalMonitoringViewModel mechanicalMonitoringViewModel;
        /// <summary>
        /// Initializes MechanicalMonitoring components.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public MechanicalMonitoring()
        {
            InitializeComponent();
            this.mechanicalMonitoringViewModel = this.DataContext as MechanicalMonitoringViewModel;
        }
        /// <summary>
        /// Occurs when the UserControl_Loaded event is raised.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            mechanicalMonitoringViewModel.SystemState = CommonViewModel.Current.SystemState;
            mechanicalMonitoringViewModel.RefreshInflationSpeedMode();
        }

        /// <summary>
        /// Occurs when the UserControl_Unloaded event is raised.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void UserControl_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            //Always disable maintenance mode when quitting except for these screens :
            // Mechanical Panel, Electrical Panel, Flow Curve Programming and PID.
            if (!CommonViewModel.Current.IsMaintenanceModeScreenSelected)
            {
                CommonViewModel.Current.Console.GUIInMaintenanceMode = false;
                CommonViewModel.Current.IsMaintenanceModeScreenSelected = false;
            }
        }
    }
}