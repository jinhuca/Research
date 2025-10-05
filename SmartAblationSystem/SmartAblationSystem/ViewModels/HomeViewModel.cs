using DataAccessLayer;
using Prism.Mvvm;
using SmartAblationSystem.Helpers;
using SmartAblationSystem.Views;
using System;
using System.Diagnostics;
using System.Windows.Input;
using Prism.Commands;

namespace SmartAblationSystem.ViewModels
{
    /// <summary>
    /// This class is for the Home View Model
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    internal class HomeViewModel : BindableBase
    {
        private ViewsEventArgs viewsEvent;
        private int selectedItem = 2;

        private double changeTankOpacity;
        private double shutDownOpacity;
        private double cryoTherapyOpacity;
        private double recordsButtonOpacity;
        private double maintenanceButtonOpacity;

        private bool isAUserAwareThatTheDiskHealthStatusIsWarningState = false;
        private bool isAUserAwareThatTheDiskHealthStatusIsFailingState = false;

        public ICommand ServiceCommand { get; private set; }
        public ICommand CryoTherapyCommand { get; private set; }
        public ICommand ShutDownCommand { get; private set; }
        public ICommand TankCommand { get; private set; }
        public ICommand RecordCommand { get; private set; }
        public ICommand LoadedCommand { get; private set; }
        public ICommand LogoutCommand { get; private set; }

        /// <summary>
        /// This constructor initializes the Home View Model's properties and commands
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public HomeViewModel()
        {
            this.ServiceCommand = new DelegateCommand<object>(this.OnService, this.CanService);
            this.CryoTherapyCommand = new DelegateCommand<object>(this.OnCryoTherapy, this.CanCryoTherapy);
            this.ShutDownCommand = new DelegateCommand<object>(this.OnShutDown, this.CanShutDown);
            this.TankCommand = new DelegateCommand<object>(this.OnTankCommand, this.CanTankCommand);
            this.RecordCommand = new DelegateCommand<object>(this.OnRecordCommand, this.CanRecordCommand);
            this.LoadedCommand = new DelegateCommand<object>(this.OnLoadedCommand, this.CanLoadedCommand);
            LogoutCommand = new DelegateCommand<object>(OnLogoutCommand).ObservesCanExecute(() => IsLoggedIn);

            SetOpacity(2);
            viewsEvent = new ViewsEventArgs();
        }

        /// <summary> 
        /// This property gets/sets the Select Item value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int SelectedItem
        {
            get
            {
                return this.selectedItem;
            }

            set
            {
                SetProperty(ref this.selectedItem, value);
                SetOpacity(value);
            }
        }

        /// <summary>
        /// This property gets/sets the Change Tank Opacity value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double ChangeTankOpacity
        {
            get
            {
                return changeTankOpacity;
            }

            set
            {
                SetProperty(ref this.changeTankOpacity, value);
            }
        }

        /// <summary>
        /// This property gets/sets the Shudown Opacity value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double ShutDownOpacity
        {
            get
            {
                return shutDownOpacity;
            }

            set
            {
                SetProperty(ref this.shutDownOpacity, value);
            }
        }

        /// <summary>
        /// This property gets/sets the Cryotherapy opacity value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double CryoTherapyOpacity
        {
            get
            {
                return cryoTherapyOpacity;
            }

            set
            {
                SetProperty(ref this.cryoTherapyOpacity, value);
            }
        }

        /// <summary>
        /// This property gets/sets the Record Button Opacity value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double RecordsButtonOpacity
        {
            get
            {
                return recordsButtonOpacity;
            }

            set
            {
                SetProperty(ref this.recordsButtonOpacity, value);
            }
        }

        /// <summary>
        /// This property gets/sets the Maintenance Button Opacity value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double MaintenanceButtonOpacity
        {
            get
            {
                return maintenanceButtonOpacity;
            }

            set
            {
                SetProperty(ref this.maintenanceButtonOpacity, value);
            }
        }

        /// <summary>
        /// This read-only property returns the Hospital Name value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string HospitalName
        {
            get
            {
                return CommonViewModel.Current.Data.DataAccess.GetHospitalName();
            }
        }
        
        private string _userName = CommonViewModel.Current.CurrentUser != null ? CommonViewModel.Current.CurrentUser.UserName : string.Empty;

        public string UserName
        {
          get => _userName;
          set => SetProperty( ref _userName, value);
        }

        private bool _isLoggedOn = CommonViewModel.Current.CurrentUser != null;

        public bool IsLoggedIn
        {
          get => _isLoggedOn;
          set => SetProperty(ref _isLoggedOn, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user is aware that the disk health status is in warning state
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsAUserAwareThatTheDiskHealthStatusIsWarningState
        {
            get
            {
                return isAUserAwareThatTheDiskHealthStatusIsWarningState;
            }

            set
            {
                isAUserAwareThatTheDiskHealthStatusIsWarningState = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user is aware that the disk health status is in failing state
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsAUserAwareThatTheDiskHealthStatusIsFailingState
        {
            get
            {
              return   isAUserAwareThatTheDiskHealthStatusIsFailingState;
            }
            set
            {
                isAUserAwareThatTheDiskHealthStatusIsFailingState = value;
            }
        }

        /// <summary>
        /// Function that sets the opacity of the item (control) received in parameter
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="selectedItem">The menu item selected.</param>
        private void SetOpacity(int selectedItem)
        {
            switch (selectedItem)
            {
                case 0:
                    ChangeTankOpacity = 1;
                    ShutDownOpacity = 0.75;
                    CryoTherapyOpacity = 0.75;
                    RecordsButtonOpacity = 0.75;
                    MaintenanceButtonOpacity = 0.75;
                    break;

                case 1:
                    ChangeTankOpacity = 0.75;
                    ShutDownOpacity = 1;
                    CryoTherapyOpacity = 0.75;
                    RecordsButtonOpacity = 0.75;
                    MaintenanceButtonOpacity = 0.75;
                    break;

                case 2:
                    ChangeTankOpacity = 0.75;
                    ShutDownOpacity = 0.75;
                    CryoTherapyOpacity = 1;
                    RecordsButtonOpacity = 0.75;
                    MaintenanceButtonOpacity = 0.75;
                    break;

                case 3:
                    ChangeTankOpacity = 0.75;
                    ShutDownOpacity = 0.75;
                    CryoTherapyOpacity = 0.75;
                    RecordsButtonOpacity = 1;
                    MaintenanceButtonOpacity = 0.75;
                    break;

                case 4:
                    ChangeTankOpacity = 0.75;
                    ShutDownOpacity = 0.75;
                    CryoTherapyOpacity = 0.75;
                    RecordsButtonOpacity = 0.75;
                    MaintenanceButtonOpacity = 1;
                    break;
            }
        }

        /// <summary>
        /// Function that returns if the system can invoke the Service command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command parameter (not used in this function).</param>
        /// <returns>Boolean value if the system can invoke the command.</returns>
        private bool CanService(object arg)
        {
            return true;
        }

        /// <summary>
        /// Function/Command that handles the view change when the Service
        /// command is invoked
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command's parameter (not used in this function).</param>
        private void OnService(object arg)
        {
            //if (SelectedItem == (int)Helpers.Enumeration.ScreenID.MAINTENANCE)
            //{
                viewsEvent.ViewName = "Settings";
                CommonViewModel.Current.OnViewchanged(viewsEvent);
            //}
        }

        /// <summary>
        /// Function that returns if the system can invoke the Tank command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command parameter (not used in this function).</param>
        /// <returns>Boolean value if the system can invoke the command.</returns>
        private bool CanTankCommand(object arg)
        {
            return true;
        }

        /// <summary>
        /// Function/Command that handles the view change when the Tank
        /// command is invoked
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command's parameter (not used in this function).</param>
        private void OnTankCommand(object arg)
        {
            //if (SelectedItem == (int)Helpers.Enumeration.ScreenID.CHANGE_TANK)
            //{
                viewsEvent.ViewName = "Tank";
                CommonViewModel.Current.OnViewchanged(viewsEvent);
            //}
        }

        /// <summary>
        /// Function that returns if the system can invoke the Record command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command parameter (not used in this function).</param>
        /// <returns>Boolean value if the system can invoke the command.</returns>
        private bool CanRecordCommand(object arg)
        {
            return true;
        }

        /// <summary>
        /// Function/Command that handles the view change when the Record
        /// command is invoked
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command's parameter (not used in this function).</param>
        private void OnRecordCommand(object arg)
        {
            //if (SelectedItem == (int)Helpers.Enumeration.ScreenID.RECORDS)
            //{
                viewsEvent.ViewName = "MainTreatmentRecord";
                CommonViewModel.Current.OnViewchanged(viewsEvent);
                HomePageOpacity = 1.0;
                //}
        }

        /// <summary>
        /// Function that returns if the system can invoke the Loaded command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command parameter (not used in this function).</param>
        /// <returns>Boolean value if the system can invoke the command.</returns>
        private bool CanLoadedCommand(object arg)
        {
            return true;
        }

        /// <summary>
        /// Function/Command that handles the view change when the Loaded command is invoked
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command's parameter (not used in this function).</param>
        private void OnLoadedCommand(object arg)
        {
            RaisePropertyChanged("HospitalName");
        }

        private void OnLogoutCommand(object arg)
        {
          var confirmationWindow_ = new LogoutWindow();
          
          HomePageOpacity = 0.2;
          
          if ((bool)confirmationWindow_.ShowDialog())
          {
            if ((CommonViewModel.Current.IsCryterionUser || CommonViewModel.Current.IsBSCADMINUser) &&
                CommonViewModel.Current.IsUsedForEngineering)
            {
              CommonViewModel.Current.IsCatheterValid = false;
            }

            CommonViewModel.Current.LoginManager.CurrentUser = null;
            UserName = null;
            IsLoggedIn = false;
          }

          HomePageOpacity = 1.0;
        }

        private double _homePageOpacity = 1;

        public double HomePageOpacity
    {
          get => _homePageOpacity;
          set => SetProperty(ref _homePageOpacity, value);
        }

        /// <summary>
        /// Function that returns if the system can invoke the Cryotherapy command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command parameter (not used in this function).</param>
        /// <returns>Boolean value if the system can invoke the command.</returns>
        private bool CanCryoTherapy(object arg)
        {
            return true;
        }

        /// <summary>
        /// Function/Command that handles the view change when the Cryotherapy command is invoked
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command's parameter (not used in this function).</param>
        private void OnCryoTherapy(object arg)
        {
	        DrivesInformation.GetTotalFreeSpace();

            if (SelectedItem == (int)Helpers.Enumeration.ScreenID.CRYO_THERAPY)
            {
                //Here we display the Hard drive message
                // if (DrivesInformation.HardDiskHealthStatus != DrivesInformation.HealthStatus.Fail)  //Emily Test
               if (DrivesInformation.HardDiskHealthStatus == DrivesInformation.HealthStatus.Fail) 
                {
                    Tuple<long, string, string, string> genericMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID89, (int)Enumeration.ErrorTypes.GUI);

                    string hexValue = "Error - " + genericMessage.Item1.ToString("X");
                
                    CommonViewModel.Current.SaveError((int)Enumeration.ErrorTypes.GUI, hexValue, int.Parse(genericMessage.Item1.ToString()), CommonViewModel.Current.SystemState);
                    MessagePopup dialogPopup = new MessagePopup(genericMessage, MessagePopup.MessageType.ErrorMessage, MessagePopup.ButtonType.Ok, "", true);
                    dialogPopup.ShowDialog();
                    
                    IsAUserAwareThatTheDiskHealthStatusIsFailingState = true;
                    return;
                }

                else if (DrivesInformation.HardDiskHealthStatus == DrivesInformation.HealthStatus.Warning && !IsAUserAwareThatTheDiskHealthStatusIsWarningState)
                {
                    Tuple<long, string, string, string> genericMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID91, (int)Enumeration.ErrorTypes.GUI);
                    
                    string hexValue = "Warning - " + genericMessage.Item1.ToString("X");
                    CommonViewModel.Current.SaveError((int)Enumeration.ErrorTypes.GUI, hexValue, int.Parse(genericMessage.Item1.ToString()), CommonViewModel.Current.SystemState);
                    
                    MessagePopup dialogPopup = new MessagePopup(genericMessage, MessagePopup.MessageType.WarningMessage, MessagePopup.ButtonType.Ok, "", true);
                    dialogPopup.ShowDialog();


                    IsAUserAwareThatTheDiskHealthStatusIsWarningState = true;
                }

                if (ConsolePowerAndState.NumberOfLogingToTherapy == 0 && !CommonViewModel.Current.IsCanOneInError)
                {
                    CommonViewModel.Current.ReadTheFirmwareVersions();
                }

                viewsEvent.ViewName = "MainCryoTherapy";
                CommonViewModel.Current.OnViewchanged(viewsEvent);

             
            }
        }

        /// <summary>
        /// Function that returns if the system can invoke the Shutdown command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command parameter (not used in this function).</param>
        /// <returns>Boolean value if the system can invoke the command.</returns>
        private bool CanShutDown(object arg)
        {
            return true;
        }

        /// <summary>
        /// Function/Command that handles the application shutdown operation when the Shutdown
        /// command is invoked
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command's parameter (not used in this function).</param>
        private void OnShutDown(object arg)
        {
          var dialog_ = new ShutdownWindow();
          HomePageOpacity = 0.2;
          if (dialog_.ShowDialog() == false)
          {
            HomePageOpacity = 1.0;
            return;
          }

          var fileAction_ = new FileAction(); 
          fileAction_.RemovePDFFile();
          if (!CommonViewModel.Current.IsCanOneInError) 
          {
            CommonViewModel.Current.Console.PowerOffMessage();
          }
          System.Threading.Thread.Sleep(500);
          Process.Start("shutdown", "/s /t 0");
        }

        private void RemoveTempFiles()
        {

        }

        /// <summary>
        /// Function that notifies a listener when the Hospital Name changed
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public void GethospitalName()
        {
            RaisePropertyChanged("HospitalName");
        }
    }
}