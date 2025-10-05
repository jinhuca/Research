using DataAccessLayer;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Prism.Commands;
using static SmartAblationSystem.Helpers.Enumeration;

namespace SmartAblationSystem.ViewModels
{
    /// <summary>
    /// This class is the PID Quicksort Algorithm View Model
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    internal class PIDQuicksortAlgorithmViewModel : BindableBase
    {
        private ObservableCollection<CMCUPIDLogs> cMCUPIDLogs;

        private ObservableCollection<PMCUPIDLogs> pMCUPIDLogs;

        private CmcuSorting CmcuCurrentSort = CmcuSorting.Unknown;
        private PmcuSorting PmcuCurrentSort = PmcuSorting.Unknown;

        public ICommand TargetFlowErrorSortCommand { get; private set; }
        public ICommand ErrorSortCommand { get; private set; }

        /// <summary>
        /// This constructor initializes the PID Quicksort Algorithm View Model commands
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public PIDQuicksortAlgorithmViewModel()
        {
            this.TargetFlowErrorSortCommand = new DelegateCommand<object>(this.OnTargetFlowErrorSortCommand, this.CanTargetFlowErrorSortCommand);
            this.ErrorSortCommand = new DelegateCommand<object>(this.OnErrorSortCommand, this.CanErrorSortCommand);
        }

        /// <summary>
        /// This property gets/sets CMCU PID Logs value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public ObservableCollection<CMCUPIDLogs> CMCUPIDLogs
        {
            get
            {
                if (CmcuCurrentSort == CmcuSorting.TargetFlowError)
                    return CommonViewModel.Current.Data.DataAccess.GetAllCMCUPIDLogsAccordingToTargetFlowError();
                else if (CmcuCurrentSort == CmcuSorting.TargetInjectionPressureError)
                    return CommonViewModel.Current.Data.DataAccess.GetAllCMCUPIDLogsAccordingToTargetInjectionPressureError();

                return CommonViewModel.Current.Data.DataAccess.GetAllCMCUPIDLogs();
            }

            set
            {
                cMCUPIDLogs = value;
                RaisePropertyChanged("CMCUPIDLogs");
            }
        }

        /// <summary>
        /// This property gets/sets PMCU PID Logs value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public ObservableCollection<PMCUPIDLogs> PMCUPIDLogs
        {
            get
            {
                if (PmcuCurrentSort == PmcuSorting.TargetBallonPressureError)
                    return CommonViewModel.Current.Data.DataAccess.GetAllPMCUPIDLogsAccordingToTargetBallonPressureError();

                return CommonViewModel.Current.Data.DataAccess.GetAllPMCUPIDLogs();
            }

            set
            {
                pMCUPIDLogs = value;
                RaisePropertyChanged("PMCUPIDLogs");
            }
        }

        /// <summary>
        /// Function that returns if the system can invoke the Target Flow Error Sort command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command parameter (not used in this function).</param>
        /// <returns>Boolean value if the system can invoke the command.</returns>
        private bool CanTargetFlowErrorSortCommand(object arg)
        {
            return true;
        }

        /// <summary>
        /// Function/Command that handles the CMCU current sort (depending on the argument) when the
        /// Target Flow Error Sort command is invoked
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command's parameter to be used to determine which is the CMCU current sort.</param>
        private void OnTargetFlowErrorSortCommand(object arg)
        {
            if (arg.ToString() == "TargetFlowError")
            {
                CmcuCurrentSort = CmcuSorting.TargetFlowError;
            }
            else if (arg.ToString() == "TargeInjectionPressureError")
            {
                CmcuCurrentSort = CmcuSorting.TargetInjectionPressureError;
            }

            RaisePropertyChanged("CMCUPIDLogs");
        }

        /// <summary>
        /// Function that returns if the system can invoke the Error Sort command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command parameter (not used in this function).</param>
        /// <returns>Boolean value if the system can invoke the command.</returns>
        private bool CanErrorSortCommand(object arg)
        {
            return true;
        }

        /// <summary>
        /// Function/Command that handles the PMCU current sort when the Error Sort
        /// command is invoked
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command's parameter (not used in this function).</param>
        private void OnErrorSortCommand(object arg)
        {
            PmcuCurrentSort = PmcuSorting.TargetBallonPressureError;
            RaisePropertyChanged("PMCUPIDLogs");
        }
    }
}