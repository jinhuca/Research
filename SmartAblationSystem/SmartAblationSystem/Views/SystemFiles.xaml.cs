using SmartAblationSystem.Helpers;
using SmartAblationSystem.Models;
using SmartAblationSystem.ViewModels;
using System;
using System.Windows.Controls;

namespace SmartAblationSystem.Views
{
    /// <summary>
    /// Interaction logic for SystemFiles.xaml
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public partial class SystemFiles : UserControl
    {
        private SystemFilesViewModel systemFilesViewModel;

        private ConsoleStateComparator ConsoleStateComparator = new ConsoleStateComparator();

        /// <summary>
        /// Initializes SystemFiles components.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public SystemFiles()
        {
            InitializeComponent();

            this.systemFilesViewModel = this.DataContext as SystemFilesViewModel;

        }

        /// <summary>
        /// Occurs when UserControl_Loaded event is raised.
        /// IEC 62304 Class A
        /// </summary>
        /// <param name="sender">The component that raised the event.</param>
        /// <param name="e">Contains state information and event data associated with a routed event.</param>
        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            CommonViewModel localCommonViewModel = CommonViewModel.Current;
            try
            {

                systemFilesViewModel.IsUsingSystemFile = true;

                localCommonViewModel.AllowFirmwareReading = true;
                localCommonViewModel.Console.ConnectTheCanTwo();
                SensorReadingMananger.AllowRemoteControl = false;

                


            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }


        /// <summary>
        /// Occurs when UserControl_UnLoaded event is raised.
        /// IEC 62304 Class A
        /// </summary>
        /// <param name="sender">The component that raised the event.</param>
        /// <param name="e">Contains state information and event data associated with a routed event.</param>
        private void UserControl_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            CommonViewModel localCommonViewModel = CommonViewModel.Current;
            try
            {
                systemFilesViewModel.IsUsingSystemFile = false;

                localCommonViewModel.AllowFirmwareReading = false ;
                localCommonViewModel.Console.DisconnectTheCanTwo();
                CommonViewModel.Current.Console.GUIInMaintenanceMode = false;
                CommonViewModel.Current.Console.HeartbeatActivated = true;
                CommonViewModel.Current.Console.GUIIsReady = true;


                systemFilesViewModel.IsPMCUSelected = false;
                systemFilesViewModel.IsCMCUSelected = false;
                systemFilesViewModel.IsCPLDSelected = false;
                systemFilesViewModel.IsRepeaterSelected = false;
                systemFilesViewModel.IsICBSelected = false;
                systemFilesViewModel.IsRemoteSelected = false;
                systemFilesViewModel.IsFirmwareLoadSelected = false;

                ConsolePowerAndState.ConsoleVersionReference = localCommonViewModel.CreateAConsoleVersion();

                ConsoleStateComparator.VerifyAndUpdateStatiqueDevices(ConsolePowerAndState.ConsoleVersionReference);


            }

            catch (Exception ex)
            {
                ex.ToString();
            }
        }
    }
}