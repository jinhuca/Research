using Prism.Mvvm;
using SmartAblationSystem.Helpers;
using SmartAblationSystem.Views;
using System.Windows.Controls;
using System.Windows.Input;
using Prism.Commands;

namespace SmartAblationSystem.ViewModels
{
    /// <summary>
    /// This class is the Service View Model
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    internal class ServiceViewModel : BindableBase
    {
        private UserControl currentServiceView;
        private UserControl electricalSignalMonitoringView;
        private UserControl flowCurveProgrammingView;
        private UserControl mechanicalMonitoringView;
        private UserControl tCsAndLCCalibrationView;
        private UserControl pIDSView;
        private UserControl catheterDatabaseView;
        private UserControl pIDQuicksortAlgorithmView;
        private UserControl siteSetupView;
        private UserControl SystemFilesView;

        private bool isElectricalSignalMonitoringSelected;
        private bool isMechanicalMonitoringSelected;
        private bool isFlowCurveProgrammingSelected;
        private bool isTCsAndLCCalibrationSelected;
        private bool isPIDSRegulationSelected;
        private bool isCatheterDatabaseSelected;
        private bool isPIDQuicksortAlgorithmSelected;
        private bool isSiteSetupSelected;
        private bool isSystemFilesSelected;

        private CommonViewModel localCommonViewModel = CommonViewModel.Current;
        public ICommand NavigateToViewCommand { get; private set; }

        /// <summary>
        /// This constructor initializes the Service View Model's properties and commands
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public ServiceViewModel()
        {
            VacuumOff();
            this.ElectricalSignalMonitoringView = new ElectricalSignalMonitoring();
            this.CurrentServiceView = this.ElectricalSignalMonitoringView;
            CommonViewModel.Current.ScreenName = "Electrical Monitoring";

            localCommonViewModel.PropertyChanged += Current_PropertyChanged;

            this.NavigateToViewCommand = new DelegateCommand<object>(this.OnNavigateToView, this.CanNavigateToView);
        }

        /// <summary>
        /// This property gets/sets the Current Service View value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public UserControl CurrentServiceView
        {
            get
            {
                return this.currentServiceView;
            }

            set
            {
                SetProperty(ref this.currentServiceView, value);
            }
        }

        /// <summary>
        /// This property gets/sets the Electrical Signal Monitoring View value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public UserControl ElectricalSignalMonitoringView
        {
            get
            {
                return electricalSignalMonitoringView;
            }

            set
            {
                SetProperty(ref this.electricalSignalMonitoringView, value);
            }
        }

        /// <summary>
        /// This property gets/sets the Flow Curve Programming View value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public UserControl FlowCurveProgrammingView
        {
            get
            {
                return flowCurveProgrammingView;
            }

            set
            {
                SetProperty(ref this.flowCurveProgrammingView, value);
            }
        }

        /// <summary>
        /// This property gets/sets the Mechanical Monitoring View User Control
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public UserControl MechanicalMonitoringView
        {
            get
            {
                return mechanicalMonitoringView;
            }

            set
            {
                SetProperty(ref this.mechanicalMonitoringView, value);
            }
        }

        /// <summary>
        /// This property gets/sets the Electrical Signal Monitoring Selected boolean flag
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsElectricalSignalMonitoringSelected
        {
            get
            {
                return isElectricalSignalMonitoringSelected;
            }

            set
            {
                SetProperty(ref this.isElectricalSignalMonitoringSelected, value);
            }
        }

        /// <summary>
        /// This property gets/sets the Mechanical Monitoring Selected flag
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsMechanicalMonitoringSelected
        {
            get
            {
                return isMechanicalMonitoringSelected;
            }

            set
            {
                SetProperty(ref this.isMechanicalMonitoringSelected, value);
            }
        }

        /// <summary>
        /// This property gets/sets the Flow Curve Programming Selected flag
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsFlowCurveProgrammingSelected
        {
            get
            {
                return isFlowCurveProgrammingSelected;
            }

            set
            {
                SetProperty(ref this.isFlowCurveProgrammingSelected, value);
            }
        }

        /// <summary>
        /// This property gets/sets the TC and LC Calibration View
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public UserControl TCsAndLCCalibrationView
        {
            get
            {
                return tCsAndLCCalibrationView;
            }

            set
            {
                SetProperty(ref this.tCsAndLCCalibrationView, value);
            }
        }

        /// <summary>
        /// This property gets/sets the TC and LC Calibration selected flag
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsTCsAndLCCalibrationSelected
        {
            get
            {
                return isTCsAndLCCalibrationSelected;
            }

            set
            {
                SetProperty(ref this.isTCsAndLCCalibrationSelected, value);
            }
        }

        /// <summary>
        /// This property gets/sets the PIDS Regulation selected flag
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsPIDSRegulationSelected
        {
            get
            {
                return isPIDSRegulationSelected;
            }

            set
            {
                SetProperty(ref this.isPIDSRegulationSelected, value);
            }
        }

        /// <summary>
        /// This property gets/sets the PIDS View User Control
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public UserControl PIDSView
        {
            get
            {
                return pIDSView;
            }

            set
            {
                pIDSView = value;
            }
        }

        /// <summary>
        /// This property gets/sets the Catheter Database selected flag
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsCatheterDatabaseSelected
        {
            get
            {
                return isCatheterDatabaseSelected;
            }

            set
            {
                SetProperty(ref this.isCatheterDatabaseSelected, value);
            }
        }

        /// <summary>
        /// This property gets/sets the Catheter Database View User Control
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public UserControl CatheterDatabaseView
        {
            get
            {
                return catheterDatabaseView;
            }

            set
            {
                catheterDatabaseView = value;
            }
        }

        /// <summary>
        /// This property gets/sets the PID Quicksort Algorithm View User Control
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public UserControl PIDQuicksortAlgorithmView
        {
            get
            {
                return pIDQuicksortAlgorithmView;
            }

            set
            {
                pIDQuicksortAlgorithmView = value;
            }
        }

        /// <summary>
        /// This property gets/sets the PID Quicksort Algorithm selected flag
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsPIDQuicksortAlgorithmSelected
        {
            get
            {
                return isPIDQuicksortAlgorithmSelected;
            }

            set
            {
                SetProperty(ref this.isPIDQuicksortAlgorithmSelected, value);
            }
        }

        /// <summary>
        /// This property gets/sets the Site Setup View User Control
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public UserControl SiteSetupView
        {
            get
            {
                return siteSetupView;
            }

            set
            {
                siteSetupView = value;
            }
        }

        /// <summary>
        /// This property gets/sets the Site Setup selected flag
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsSiteSetupSelected
        {
            get
            {
                return isSiteSetupSelected;
            }

            set
            {
                SetProperty(ref this.isSiteSetupSelected, value);
            }
        }

        /// <summary>
        /// This property gets/sets the System Files selected flag
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsSystemFilesSelected
        {
            get
            {
                return isSystemFilesSelected;
            }

            set
            {
                SetProperty(ref this.isSystemFilesSelected, value);
            }
        }

        /// <summary>
        /// Gets or sets  a value indicating whether the boot loader updating firmware
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summar
        public bool IsBootLoaderUpdatingFirmware
        {
            get
            {
                return CommonViewModel.Current.IsBootLoaderUpdatingFirmware; 
            }
            set
            {
                CommonViewModel.Current.IsBootLoaderUpdatingFirmware = value;
                RaisePropertyChanged("IsBootLoaderUpdatingFirmware");
            }
        }

        /// <summary>
        /// Function that sets the vacuum off
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void VacuumOff()
        {
            CommonViewModel.Current.Console.Disconnect();
            CommonViewModel.Current.IsVacuumDisconnected = true;
        }

        /// <summary>
        /// Function that returns if the system can invoke the Navigate to View command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command parameter (not used in this function).</param>
        /// <returns>Boolean value if the system can invoke the command.</returns>
        private bool CanNavigateToView(object arg)
        {
            return true;
        }

        /// <summary>
        /// Function/Command that handles the view navigation switch and resets properties
        /// when Navigate to View Command is invoked
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command parameter that holds the destination view name.</param>
        private void OnNavigateToView(object arg)
        {
            if (CommonViewModel.Current.CanOneStopWatchCommunicationLost != null && CommonViewModel.Current.CanOneStopWatchCommunicationLost.IsRunning)
                CommonViewModel.Current.CanOneStopWatchCommunicationLost.Restart();

            if (CommonViewModel.Current.CanTwoStopWatchCommunicationLost != null)
                CommonViewModel.Current.CanTwoStopWatchCommunicationLost.Restart();

            CommonViewModel.Current.IsMaintenanceModeScreenSelected = false;

            if (arg.ToString() == "MechanicalMonitoring")
            {
                CommonViewModel.Current.IsMaintenanceModeScreenSelected = true;

                if (this.MechanicalMonitoringView == null)
                    this.MechanicalMonitoringView = new MechanicalMonitoring();

                VacuumOff();
                CommonViewModel.Current.ScreenName = "Mechanical Monitoring";

                this.CurrentServiceView = this.MechanicalMonitoringView;
                this.IsMechanicalMonitoringSelected = true;
                this.IsElectricalSignalMonitoringSelected = false;
                this.IsTCsAndLCCalibrationSelected = false;
                this.IsPIDSRegulationSelected = false;
                this.IsCatheterDatabaseSelected = false;
                this.IsPIDQuicksortAlgorithmSelected = false;
                this.IsSiteSetupSelected = false;
                this.IsSystemFilesSelected = false;
                this.IsFlowCurveProgrammingSelected = false;

                CommonViewModel.Current.LogUserAction(Enumeration.Actions.AccessMechanicalMonitoring);
            }
            else if (arg.ToString() == "FlowCurveProgramming")
            {
                CommonViewModel.Current.IsMaintenanceModeScreenSelected = true;

                if (this.FlowCurveProgrammingView == null)
                    this.FlowCurveProgrammingView = new FlowCurveProgramming();

                VacuumOff();
                CommonViewModel.Current.ScreenName = "Flow Curve Programming";

                this.CurrentServiceView = this.FlowCurveProgrammingView;
                this.IsMechanicalMonitoringSelected = false;
                this.IsElectricalSignalMonitoringSelected = false;
                this.IsTCsAndLCCalibrationSelected = false;
                this.IsPIDSRegulationSelected = false;
                this.IsCatheterDatabaseSelected = false;
                this.IsPIDQuicksortAlgorithmSelected = false;
                this.IsSiteSetupSelected = false;
                this.IsSystemFilesSelected = false;
                this.IsFlowCurveProgrammingSelected = true;

                CommonViewModel.Current.LogUserAction(Enumeration.Actions.AccessFlowCurveProgramming);
            }
            else if (arg.ToString() == "ElectricalSignalMonitoring")
            {
                CommonViewModel.Current.IsMaintenanceModeScreenSelected = true;

                if (this.ElectricalSignalMonitoringView == null)
                    this.ElectricalSignalMonitoringView = new ElectricalSignalMonitoring();

                VacuumOff();
                CommonViewModel.Current.ScreenName = "Electrical Signal";

                this.CurrentServiceView = this.ElectricalSignalMonitoringView;
                this.IsMechanicalMonitoringSelected = false;
                this.IsElectricalSignalMonitoringSelected = true;
                this.IsTCsAndLCCalibrationSelected = false;
                this.IsPIDSRegulationSelected = false;
                this.IsCatheterDatabaseSelected = false;
                this.IsPIDQuicksortAlgorithmSelected = false;
                this.IsSiteSetupSelected = false;
                this.IsSystemFilesSelected = false;
                this.IsFlowCurveProgrammingSelected = false;

                CommonViewModel.Current.LogUserAction(Enumeration.Actions.AccessElectricalMonitoring);
            }
            else if (arg.ToString() == "TCsAndLCCalibration")
            {
                //Resets the display each time it is called
                this.TCsAndLCCalibrationView = new TCsAndLCCalibration();

                VacuumOff();
                CommonViewModel.Current.ScreenName = "Calibration";

                this.CurrentServiceView = this.TCsAndLCCalibrationView;
                this.IsMechanicalMonitoringSelected = false;
                this.IsElectricalSignalMonitoringSelected = false;
                this.IsTCsAndLCCalibrationSelected = true;
                this.IsPIDSRegulationSelected = false;
                this.IsCatheterDatabaseSelected = false;
                this.IsPIDQuicksortAlgorithmSelected = false;
                this.IsSiteSetupSelected = false;
                this.IsSystemFilesSelected = false;
                this.IsFlowCurveProgrammingSelected = false;

                CommonViewModel.Current.LogUserAction(Enumeration.Actions.AccessLoadCellCalibration);
            }
            else if (arg.ToString() == "PIDSRegulation")
            {
                CommonViewModel.Current.IsMaintenanceModeScreenSelected = true;

                if (this.PIDSView == null)
                    this.PIDSView = new PIDS();

                VacuumOff();
                CommonViewModel.Current.ScreenName = "PIDS";

                this.CurrentServiceView = this.PIDSView;
                this.IsMechanicalMonitoringSelected = false;
                this.IsElectricalSignalMonitoringSelected = false;
                this.IsTCsAndLCCalibrationSelected = false;
                this.IsPIDSRegulationSelected = true;
                this.IsCatheterDatabaseSelected = false;
                this.IsPIDQuicksortAlgorithmSelected = false;
                this.IsSiteSetupSelected = false;
                this.IsSystemFilesSelected = false;
                this.IsFlowCurveProgrammingSelected = false;

                CommonViewModel.Current.LogUserAction(Enumeration.Actions.AccessPIDControl);
            }
            else if (arg.ToString() == "CatheterDatabase")
            {
                if (this.CatheterDatabaseView == null)
                    this.CatheterDatabaseView = new CatheterDatabase();

                VacuumOff();
                CommonViewModel.Current.ScreenName = "Catheter Database";

                this.CurrentServiceView = this.CatheterDatabaseView;
                this.IsMechanicalMonitoringSelected = false;
                this.IsElectricalSignalMonitoringSelected = false;
                this.IsTCsAndLCCalibrationSelected = false;
                this.IsPIDSRegulationSelected = false;
                this.IsCatheterDatabaseSelected = true;
                this.IsPIDQuicksortAlgorithmSelected = false;
                this.IsSiteSetupSelected = false;
                this.IsSystemFilesSelected = false;
                this.IsFlowCurveProgrammingSelected = false;

                CommonViewModel.Current.LogUserAction(Enumeration.Actions.AccessCatheterDatabase);
            }
            else if (arg.ToString() == "PIDQuicksortAlgorithm")
            {
                if (this.PIDQuicksortAlgorithmView == null)
                    this.PIDQuicksortAlgorithmView = new PIDQuicksortAlgorithm();

                VacuumOff();
                CommonViewModel.Current.ScreenName = "PID Quicksort Algo";

                this.CurrentServiceView = this.PIDQuicksortAlgorithmView;
                this.IsMechanicalMonitoringSelected = false;
                this.IsElectricalSignalMonitoringSelected = false;
                this.IsTCsAndLCCalibrationSelected = false;
                this.IsPIDSRegulationSelected = false;
                this.IsCatheterDatabaseSelected = false;
                this.IsPIDQuicksortAlgorithmSelected = true;
                this.IsSiteSetupSelected = false;
                this.IsSystemFilesSelected = false;
                this.IsFlowCurveProgrammingSelected = false;
            }
            else if (arg.ToString() == "SiteSetup")
            {
                if (this.SiteSetupView == null)
                    this.SiteSetupView = new SiteSetup();

                VacuumOff();
                CommonViewModel.Current.ScreenName = "Site Setup";

                this.CurrentServiceView = this.SiteSetupView;
                this.IsMechanicalMonitoringSelected = false;
                this.IsElectricalSignalMonitoringSelected = false;
                this.IsTCsAndLCCalibrationSelected = false;
                this.IsPIDSRegulationSelected = false;
                this.IsCatheterDatabaseSelected = false;
                this.IsPIDQuicksortAlgorithmSelected = false;
                this.IsSiteSetupSelected = true;
                this.IsSystemFilesSelected = false;
                this.IsFlowCurveProgrammingSelected = false;

                CommonViewModel.Current.LogUserAction(Enumeration.Actions.AccessSiteSetup);
            }
            else if (arg.ToString() == "SystemFiles")
            {
                if (this.SystemFilesView == null)
                    this.SystemFilesView = new SystemFiles();

                VacuumOff();
                CommonViewModel.Current.ScreenName = "System Files";

                this.CurrentServiceView = this.SystemFilesView;
                this.IsMechanicalMonitoringSelected = false;
                this.IsElectricalSignalMonitoringSelected = false;
                this.IsTCsAndLCCalibrationSelected = false;
                this.IsPIDSRegulationSelected = false;
                this.IsCatheterDatabaseSelected = false;
                this.IsPIDQuicksortAlgorithmSelected = false;
                this.IsSiteSetupSelected = false;
                this.IsSystemFilesSelected = true;
                this.IsFlowCurveProgrammingSelected = false;

                CommonViewModel.Current.LogUserAction(Enumeration.Actions.AccessSystemFiles);
            }

            CommonViewModel.Current.MaintenanceScreenName = CommonViewModel.Current.ScreenName;
        }

        /// <summary>
        /// This function handles the sender's PropertyChanged event
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">The View Model that sent the event.</param>
        /// <param name="e">The property changed arguments.</param>
        private void Current_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsBootLoaderUpdatingFirmware":
                    RaisePropertyChanged("IsBootLoaderUpdatingFirmware");
                    break;

            }
        }
    }
}