using SmartAblationSystem.Helpers;
using SmartAblationSystem.Models;
using SmartAblationSystem.Views;
using System;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Shared;

using static Communication.CanBusMessageDefinition;
using BindableBase = Prism.Mvvm.BindableBase;

namespace SmartAblationSystem.ViewModels
{
  /// <summary>
  /// This class is the Main Window View Model.
  /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
  /// </summary>
  internal class MainWindowViewModel : BindableBase
  {
    private UserControl currentView;
    private UserControl settingsView;
    private UserControl manageUsers;
    private UserControl timeAndDate;
    private UserControl userManual;
    private UserControl serviceView;
    private UserControl homeView;
    private UserControl mainCryoTherapy;
    private UserControl changeTank;
    private UserControl mainTreatmentRecord;
    private UserControl actionLog;
    private UserControl errorLog;

    public ICommand HomeCommand { get; private set; }

    public ICommand DecreaseVolumeCommand { get; private set; }

    public ICommand IncreaseVolumeCommand { get; private set; }

    public ICommand DisplayWarningCommand { get; private set; }

    public ICommand WarningMessagesCommand { get; private set; }

    public ICommand LogoutCommand { get; private set; }

    public ICommand UserManualCommand { get; private set; }

    public ICommand ViewChangeCommand { get; set; }
    public ICommand ChangeTankCommand { get; private set; }

    #region monitoring from the main windows

    private Thread heartbeatThread;




    #endregion

    bool isUserManualLoading = false;

    /// <summary>
    /// This property gets/sets Window Loaded value.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool WindowLoaded
    {
      get { return commonViewModel.IsWindowLoaded; }
      set { commonViewModel.IsWindowLoaded = value; }
    }


    private MaliciousDataChangeModel maliciousDataChangeModel = MaliciousDataChangeModel.Instance;

    // autoMonitor The Main Thread
    private Stopwatch playBackStopWatch = new Stopwatch();

    private CommonViewModel commonViewModel;

    private readonly IDisplayConfigurationMonitor displayConfigurationMonitor;

    /// <summary>
    /// This constructor initializes the Main Window View Model's properties and commands
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public MainWindowViewModel(CommonViewModel commonViewModel_, IDisplayConfigurationMonitor displayConfigurationMonitor_)
    {
      // First we build the views to display some thing for the user
      this.CurrentView = new Home(); //Home();
      this.homeView = this.CurrentView;

      commonViewModel = commonViewModel_;
      commonViewModel.ViewChanged += CommonViewModel_viewChanged;
      commonViewModel.PropertyChanged += Current_PropertyChanged;
      this.HomeCommand = new Prism.Commands.DelegateCommand<object>(this.OnHome, this.CanHome);
      this.WarningMessagesCommand = new Prism.Commands.DelegateCommand<object>(this.OnDisplayWarningCommand, this.CanDisplayWarningCommand);

      // Create also the main cryo view because laoding is heavy
      this.mainCryoTherapy = new MainCryoTherapy();

      this.IncreaseVolumeCommand = new Prism.Commands.DelegateCommand<object>(this.OnIncreaseVolumeCommand, this.CanIncreaseVolumeCommand);

      this.DecreaseVolumeCommand = new Prism.Commands.DelegateCommand<object>(this.OnDecreaseVolumeCommand, this.CanDecreaseVolumeCommand);

      this.DisplayWarningCommand = new Prism.Commands.DelegateCommand<object>(this.OnDisplayWarningCommand, this.CanDisplayWarningCommand);

      this.LogoutCommand = new Prism.Commands.DelegateCommand<object>(this.OnLogoutCommand, this.CanLogoutCommand);

      this.UserManualCommand = new Prism.Commands.DelegateCommand<object>(this.OnUserManualCommand, this.CanUserManualCommand);

      ViewChangeCommand = new Prism.Commands.DelegateCommand<object>(OnViewChangeCommand, CanChangeView);

      this.ChangeTankCommand = new Prism.Commands.DelegateCommand<object>(this.OnChangeTankCommand, this.CanChangeTankCommand);

      //set the volume to 100% at beginning
      //RequiredVolume = 100;

      Task.Delay(3000).ContinueWith(t => commonViewModel.StartCanOneStopWatchCommunicationMonitoring());
      Task.Delay(3000).ContinueWith(t => commonViewModel.StartCanTwoStopWatchCommunicationMonitoring());

      commonViewModel.ResetSystem();

      heartbeatThread = new Thread(new ThreadStart(StartHeartbeatThread));
      heartbeatThread.Start();

#if RELEASE
			displayConfigurationMonitor = displayConfigurationMonitor_;
			displayConfigurationMonitor?.DisplayMonitoringSubscription();
#endif
    }

    /// <summary>
    /// This function starts the heart beat thread.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private void StartHeartbeatThread()
    {
      // //AppTrace.Log($"Starting Heart Beat thread", LogLevel.Info, Thread.CurrentThread.ManagedThreadId.ToString(), nameof(MainWindowViewModel), nameof(StartHeartbeatThread));
      commonViewModel.Console.GUIIsReady = true;

      while (true && Application.Current != null)
      {
        if (commonViewModel.GUIIsRunning)
        {
          commonViewModel.Console.SendHeartbeat();

          if (playBackStopWatch.IsRunning)
          {
            playBackStopWatch.Reset();

          }
        }
        else
        {
          if (!playBackStopWatch.IsRunning)
            playBackStopWatch.Start();

          if (playBackStopWatch.ElapsedMilliseconds > 5000)
          {
            //Debug
          }
          else
          {
            commonViewModel.Console.SendHeartbeat();
          }


        }
      }
    }

    /// <summary>
    /// This property gets/sets the Screen Name value.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public string ScreenName
    {
      get
      {
        return commonViewModel.ScreenName;
      }
      set

      {
        commonViewModel.ScreenName = value;
        RaisePropertyChanged("ScreenName");
      }
    }

    /// <summary>
    /// This property gets/sets the Password Code value.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public string PasswordCode
    {
      get
      {
        return commonViewModel.LoginManager.CryUserCode.ToString();
      }
    }

    /// <summary>
    /// This function handles the Common View Model's View Changed event.  It allows
    /// the current view to change to the one recieved in parameter depending on the
    /// User's permissions.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="sender">The View Model that sent the event.</param>
    /// <param name="e">The property changed arguments.</param>
    private void CommonViewModel_viewChanged(object sender, ViewsEventArgs e)
    {
      if (commonViewModel.CanOneStopWatchCommunicationLost != null && commonViewModel.CanOneStopWatchCommunicationLost.IsRunning)
        commonViewModel.CanOneStopWatchCommunicationLost.Restart();

      if (commonViewModel.CanTwoStopWatchCommunicationLost != null)
        commonViewModel.CanTwoStopWatchCommunicationLost.Restart();

      if (e.ViewName == "Home")
      {
	      if (this.homeView == null)
		      this.homeView = new Home();// HomeNewLook(); // Home();

				this.CurrentView = this.homeView;
        ProcedureLogModel.CanReloadProcudreInformation = false;
        SensorReadingMananger.AllowRemoteControl = false;

        ScreenName = Languages.GuiFieldTranslation.ContainsKey("HomeLabel") ? Languages.GuiFieldTranslation["HomeLabel"] : "Home";
        return;
      }
      else if (e.ViewName == "ManageUsers")
      {
        if (this.manageUsers == null)
          this.manageUsers = new ManageUsers();

        this.CurrentView = this.manageUsers;
        ScreenName = Languages.GuiFieldTranslation.ContainsKey("ManageUsersLabel") ? Languages.GuiFieldTranslation["ManageUsersLabel"] : "Manage Users";

        commonViewModel.LogUserAction(Enumeration.Actions.AccessManageUsers);
      }
      else if (e.ViewName == "TimeAndDate")
      {
        if (this.timeAndDate == null)
          this.timeAndDate = new TimeAndDate();

        this.CurrentView = this.timeAndDate;
        ScreenName = Languages.GuiFieldTranslation.ContainsKey("DateTimeLabel") ? Languages.GuiFieldTranslation["DateTimeLabel"] : "Time & Date";

        //Log Time And Date
        commonViewModel.LogUserAction(Enumeration.Actions.AccessDateAndTime);
      }
      else if (e.ViewName == "UserManual")
      {
        if (this.userManual == null)
          this.userManual = new UserManual();

        this.CurrentView = this.userManual;
        ScreenName = Languages.GuiFieldTranslation.ContainsKey("UserManualLabel") ? Languages.GuiFieldTranslation["UserManualLabel"] : "UserManual";
      }
      else if (e.ViewName == "ActionLog")
      {
        if (this.actionLog == null)
          this.actionLog = new ActionLog();

        this.CurrentView = this.actionLog;
        ScreenName = Languages.GuiFieldTranslation.ContainsKey("ActionLogLabel") ? Languages.GuiFieldTranslation["ActionLogLabel"] : "Action Log";
      }
      else if (e.ViewName == "ViewErrorLog")
      {
        if (this.errorLog == null)
          this.errorLog = new ConsoleErrorLog();

        this.CurrentView = this.errorLog;
        ScreenName = Languages.GuiFieldTranslation.ContainsKey("ErrorLogLabel") ? Languages.GuiFieldTranslation["ErrorLogLabel"] : "Error Log";
      }

      else if (e.ViewName == "Service")
      {
        if (this.serviceView == null)
        {
          this.serviceView = new Service();
        }
        else
        {
          //When coming back on the Maintenance screen for the second time (and more), the screen is already loaded.
          //Vacuum OFF shall be called anyway.
          commonViewModel.Console.Disconnect();
          commonViewModel.IsVacuumDisconnected = true;
        }

        this.CurrentView = this.serviceView;
        ScreenName = commonViewModel.MaintenanceScreenName;

        commonViewModel.LogUserAction(Enumeration.Actions.AccessMaintenance);
      }
      else if (e.ViewName == "WarningMessages")
      {
        WarningMessages warningMessage = new WarningMessages();
        warningMessage.ShowDialog();
      }
      else if (e.ViewName == "BackToSettings")
      {
        if (this.settingsView == null)
          this.settingsView = new Settings();

        this.CurrentView = this.settingsView;
        ScreenName = "Settings";
      }
      else
      {
        logon login = null;

        var home_ = ((CurrentView as Home)?.DataContext as HomeViewModel);
        if (home_ != null)
        {
          home_.HomePageOpacity = 0.2;
        }

        //Generate a random 8 digit pass code for the CRY user.
        commonViewModel.LoginManager.GeneratePassCode();

        if (commonViewModel.CurrentUser == null)
        {
          //1st time login and Settings screens requires user login before display
          login = new logon(this);
          login.WrongUsernameOrPasswordLabel.Visibility = System.Windows.Visibility.Hidden;
          login.ShowDialog();

          while ((login.DialogResult == true && !commonViewModel.LoginManager.LoginUser(login.TxtUser.Text, login.TxtPassword.Password)))
          {

#if Simulator
						//On simulator with CRY user, by-pass the Pass Code.
						if(login.TxtUser.Text.ToUpper() == "BSC")
						{
							commonViewModel.LoginManager.CurrentUser = commonViewModel.Data.DataAccess.ConnectUserCry();
							break;
						}

						if(login.TxtUser.Text.ToUpper() == "BSCADMIN")
						{
							commonViewModel.LoginManager.CurrentUser = commonViewModel.Data.DataAccess.ConnectBSCADMINUser();
							break;
						}
#endif
            string username = login.TxtUser.Text;

            login.WrongUsernameOrPasswordLabel.Visibility = System.Windows.Visibility.Visible;

            login = new logon(this);
            login.TxtUser.Text = username;  //keep the username displayed
            login.TxtPassword.Password= "";
            login.ShowDialog();
          }

          //Log the user login action
          if (login.DialogResult == true && commonViewModel.CurrentUser != null)
          {
            commonViewModel.LogUserAction(Enumeration.Actions.Login);
            if (((currentView as Home)?.DataContext) is HomeViewModel homeViewModel_)
            {
              homeViewModel_.UserName = commonViewModel.CurrentUser.UserName;
              homeViewModel_.IsLoggedIn = true;
            }
          }
          else if (login.DialogResult == false)
          {
            //The user hit Cancel button at the login screen
            if(home_ != null)
            {
              home_.HomePageOpacity = 1.0;
            }
            return;
          }
        }

        //A user was already logged-in or has just logged-in using the login dialog window

        if (commonViewModel.CurrentUser != null)
        {
          // //AppTrace.Log($"Login as {commonViewModel.CurrentUser.UserName}", LogLevel.Info, Thread.CurrentThread.ManagedThreadId.ToString(), nameof(MainWindowViewModel), nameof(CommonViewModel_viewChanged));
          if (e.ViewName == "Settings")
          {
            if ((!commonViewModel.IsCryterionUser && !commonViewModel.IsAdminUser && !commonViewModel.IsBSCADMINUser))
            {
              Tuple<long, string, string, string> genericMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID12, (int)Enumeration.ErrorTypes.GUI);

              MessagePopup dialogPopup = new MessagePopup(genericMessage, MessagePopup.MessageType.WarningMessage, MessagePopup.ButtonType.Ok);
              dialogPopup.ShowDialog();
              home_.HomePageOpacity = 1.0;
              return;
            }

            if (this.settingsView == null)
              this.settingsView = new Settings();

            this.CurrentView = this.settingsView;

            if (Models.Languages.GuiFieldTranslation.ContainsKey("SettingsLabel"))
            {
              ScreenName = Models.Languages.GuiFieldTranslation["SettingsLabel"];
            }

            //Log Setting Action
            commonViewModel.LogUserAction(Enumeration.Actions.AccessSettings);
          }
          else if (e.ViewName == "MainCryoTherapy")
          {
            if (this.mainCryoTherapy == null)
              this.mainCryoTherapy = new MainCryoTherapy();

            this.CurrentView = this.mainCryoTherapy;

            //Make sure the "End Procedure", "Complete Procedure" and "Return to Procedure" buttons
            //are refreshed when showing the screen.
            ((MainCryoTherapyViewModel)this.mainCryoTherapy.DataContext).RefreshButtonsVisibility();

            if (Models.Languages.GuiFieldTranslation.ContainsKey("CryoTherapyLabel"))
            {
              //ScreenName = Models.Languages.GuiFieldTranslation["CryoTherapyLabel"];
              ScreenName = "Patient Info";
            }
          }
          else if (e.ViewName == "Tank")
          {
            this.changeTank = new ChangeTank();  //Allow the view to initialize its controls each time it is called
            commonViewModel.LogUserAction(Enumeration.Actions.AccessChangeTank);
            this.CurrentView = this.changeTank;
            if (Models.Languages.GuiFieldTranslation.ContainsKey("ChangeTankLabel"))
            {
              ScreenName = Models.Languages.GuiFieldTranslation["ChangeTankLabel"];
            }

            // Log Tank Access Change Tank
            commonViewModel.LogUserAction(Enumeration.Actions.AccessChangeTank);
          }
          else if (e.ViewName == "Summary Report")
          {
            if (Models.Languages.GuiFieldTranslation.ContainsKey("SummaryReportLabel"))
            {
              //Converters.FieldToTextConverter fieldtotextValue = new Converters.FieldToTextConverter();
              // ScreenName = fieldtotextValue.Convert("SummaryReportLabel",null, "TITLECASE", null).ToString();
              ScreenName = "Report";

            }
          }
          else if (e.ViewName == "MainTreatmentRecord")
          {


            if (!commonViewModel.IsCryterionUser && !commonViewModel.IsDoctor && !commonViewModel.IsBSCADMINUser && !commonViewModel.IsAdminUser)
            {
              Tuple<long, string, string, string> genericMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID86, (int)Enumeration.ErrorTypes.GUI);

              MessagePopup dialogPopup = new MessagePopup(genericMessage, MessagePopup.MessageType.WarningMessage, MessagePopup.ButtonType.Ok);
              dialogPopup.ShowDialog();
              return;
            }


            //Generate the list of procedures each time the screen is accessed.
            this.mainTreatmentRecord = new MainTreatmentRecord();

            commonViewModel.LogUserAction(Enumeration.Actions.AccessRecord);
            this.CurrentView = this.mainTreatmentRecord;
            if (Models.Languages.GuiFieldTranslation.ContainsKey("RecordsLabel"))
            {
              ScreenName = Models.Languages.GuiFieldTranslation["RecordsLabel"];
            }

            MaliciousDataChangeModel.IsMaliciousDataChangeModelActivated = true;
          }
          else if (e.ViewName == "Records")
          {

            commonViewModel.LogUserAction(Enumeration.Actions.AccessRecord);
            this.CurrentView = this.mainTreatmentRecord;
            if (Models.Languages.GuiFieldTranslation.ContainsKey("RecordsLabel"))
            {
              ScreenName = Models.Languages.GuiFieldTranslation["RecordsLabel"];
            }
          }

          //IsViewChangeVisible = e.ViewName == "MainCryoTherapy";
        }

        if(home_ != null)
        {
          home_.HomePageOpacity = 1.0;
        }
      }
    }

    /// <summary>
    /// This property gets/sets the Current View value.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public UserControl CurrentView
    {
      get
      {
        return currentView;
      }
      set
      {
        currentView = value;
        RaisePropertyChanged("CurrentView");
        RaisePropertyChanged("IsLogoutVisible");
      }
    }

    private string _therapyViewName = string.Empty;
    public string TherapyViewName
    {
      get => _therapyViewName;
      set => SetProperty(ref _therapyViewName, value);
    }

    /// <summary>
    /// This property gets/sets Required Volume value.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public uint RequiredVolume
    {
      get
      {
        return commonViewModel.RequiredVolume;
      }
      set
      {
        commonViewModel.RequiredVolume = value;
        RaisePropertyChanged("RequiredVolume");
      }
    }

    /// <summary>
    /// This property gets/sets the Warning Visible value.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsWarningVisible
    {
      get
      {
        return commonViewModel.IsWarningVisible;
      }
      set
      {
        commonViewModel.IsWarningVisible = value;
        RaisePropertyChanged("IsWarningVisible");
      }
    }

    private bool _isVolumeChangeInvisible = true;
    public bool IsVolumeChangeInvisible
    {
      get=>_isVolumeChangeInvisible;
      set => SetProperty(ref _isVolumeChangeInvisible, value);
    }

    private bool _isViewChangeVisible;

    public bool IsViewChangeVisible
    {
      get => _isViewChangeVisible;
      set => SetProperty(ref _isViewChangeVisible, value);
    }

    /// <summary>
    /// This property gets/sets the Logout Visible value.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsLogoutVisible
    {
      get
      {
        //Only display the Logout button when a user is connected and the current view is HOME.
        return commonViewModel.CurrentUser != null && currentView == homeView;
      }
      set
      {
        RaisePropertyChanged("IsLogoutVisible");
      }
    }

    /// <summary>
    /// This property gets the UserName value.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public string UserName
    {
      get
      {
        return commonViewModel?.CurrentUser?.UserName;
      }
    }

    /// <summary>
    /// This read-only property returns the Software Version value.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public string SoftwareVersion
    {
      get
      {
        try
        {
          Assembly assembly = Assembly.GetExecutingAssembly();
          FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
          string version = fileVersionInfo.ProductVersion;

          return version;
        }
        catch
        {
          //TODO :
          return "0.0.0.0";
        }
      }
      set
      {
        RaisePropertyChanged("SoftwareVersion");
      }
    }

    /// <summary>
    /// Function that sets the Current View to the User Control recieved in parameter.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="us">The User Control to be assigned to the Current View.</param>
    public void changeView(UserControl us)
    {
      this.CurrentView = us;
    }

    /// <summary>
    /// Function that returns if the system can invoke the Home command.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="arg">The command parameter (not used in this function).</param>
    /// <returns>Boolean value if the system can invoke the command.</returns>
    private bool CanHome(object arg)
    {
      return true;
    }

    /// <summary>
    /// This read-only property returns the System State value.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public MessageStateId SystemState
    {
      get
      {
        return commonViewModel.SystemState;
      }
    }

    /// <summary>
    /// Gets the value indicating whether user allowed to change tank.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsUserAllowedToChangeTank
    {
      get
      {
        return (commonViewModel.IsUserAllowedToChangeTank && this.CurrentView == this.mainCryoTherapy);
      }
    }

    /// <summary>
    /// Gets or sets  a value indicating whether the boot loader updating firmware.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summar
    public bool IsBootLoaderUpdatingFirmware
    {
      get
      {
        return commonViewModel.IsBootLoaderUpdatingFirmware;
      }
      set
      {
        commonViewModel.IsBootLoaderUpdatingFirmware = value;
        RaisePropertyChanged("IsBootLoaderUpdatingFirmware");
      }
    }

    /// <summary>
    /// Gets or sets the malicious data change model object.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summar
    internal MaliciousDataChangeModel MaliciousDataChangeModel
    {
      get => maliciousDataChangeModel;
      set => maliciousDataChangeModel = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the user manual is laoding .
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summar
    public bool IsUserManualLoading
    {
      get
      {
        return isUserManualLoading;
      }
      set
      {
        this.isUserManualLoading = value;
        RaisePropertyChanged("IsUserManualLoading");
      }
    }

    /// <summary>
    /// Function/Command that handles the return to Home view when the Home command is invoked.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="arg">The command's parameter (not used in this function).</param>
    private void OnHome(object arg)
    {
      if (ZiPStates.IsZipingFiles)
        return;

      if (CurrentView.GetType() == typeof(MainCryoTherapy))
      {
        MainCryoTherapyViewModel context = CurrentView.DataContext as MainCryoTherapyViewModel;

        if (context.CurrentMainCryoTherapyView.GetType() == typeof(CryoTherapy))
        {
          MessageStateId currentState = commonViewModel.SystemState;
          if (currentState != MessageStateId.CAN_ID_STATE_IDLE &
              currentState != MessageStateId.CAN_ID_STATE_READY &
              currentState != MessageStateId.CAN_ID_STATE_EXCEPTION &
              currentState != MessageStateId.CAN_ID_STATE_UNKNOWN
              )
            return;

          Tuple<long, string, string, string> genericMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID11, (int)Enumeration.ErrorTypes.GUI);

          MessagePopup dialogPopup = new MessagePopup(genericMessage)
          {
            WindowStartupLocation = WindowStartupLocation.Manual,
            Left = 601,
            Top = 490
          };

          if ((bool)dialogPopup.ShowDialog())
          {
            // //AppTrace.Log("Start to Quit Procedure to Home", LogLevel.Debug, Thread.CurrentThread.ManagedThreadId.ToString(), nameof(MainWindowViewModel), nameof(OnHome));
            ManageQuitProcedure(context);
            // //AppTrace.Log("Quit Procedure to Home", LogLevel.Debug, Thread.CurrentThread.ManagedThreadId.ToString(), nameof(MainWindowViewModel), nameof(OnHome));
          }
          else
          {
            return;
          }
        }
        else if (context.CurrentMainCryoTherapyView.GetType() == typeof(Report))
        {
          Tuple<long, string, string, string> genericMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID11, (int)Enumeration.ErrorTypes.GUI);

          MessagePopup dialogPopup = new MessagePopup(genericMessage)
          {
            WindowStartupLocation = WindowStartupLocation.Manual,
            Left = 601,
            Top = 490
          };

          if ((bool)dialogPopup.ShowDialog())
          {
            //End the procedure before going to Home
            context.EndProcedureCommand.Execute("HomeAndEndingProcedure");
            //ManageQuitProcedure(context);
          }
          else
          {
            return;
          }
        }
      }
      else if (CurrentView.GetType() == typeof(ChangeTank) &&
         commonViewModel.AccessedChangeTankFromCryotherapy)
      {
        //When Change Tank was accessed while in Cryotherapy, must return to Cryotherapy screen when
        //HOME is pressed
        commonViewModel.AccessedChangeTankFromCryotherapy = false;

        if (this.mainCryoTherapy != null)
        {
          this.CurrentView = this.mainCryoTherapy;

          if (Languages.GuiFieldTranslation.ContainsKey("CryoTherapyLabel"))
          {
            ScreenName = Models.Languages.GuiFieldTranslation["CryoTherapyLabel"];
          }
        }

        return;  //ensure to quit this function and not execute the code below
      }

      this.CurrentView = homeView;
      ScreenName = "Home";

      commonViewModel.Console.GUIInMaintenanceMode = false;

      commonViewModel.Console.Disconnect();
      commonViewModel.IsVacuumDisconnected = true;
      commonViewModel.DeflateAfterThaw = false;
      MaliciousDataChangeModel.IsMaliciousDataChangeModelActivated = false;
      ProcedureLogModel.CanReloadProcudreInformation = false;
      SensorReadingMananger.AllowRemoteControl = false;

    }

    /// <summary>
    /// Function that handles console and flags reset when quitting a Procedure.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="context">A viewmodel representing the context.</param>
    public void ManageQuitProcedure(MainCryoTherapyViewModel context)
    {
      // //AppTrace.Log($"Before", LogLevel.Debug, Thread.CurrentThread.ManagedThreadId.ToString(), nameof(MainWindowViewModel), nameof(ManageQuitProcedure));

      //Save the skin to skin time
      if (commonViewModel.SkinToSkinDuration != 0 && commonViewModel.CurrentProcedure != null)
      {
        short skinToSkinDuration = (short)commonViewModel.SkinToSkinDuration;

        if (skinToSkinDuration > 0)
        {
          ProcedureLogModel.SkinToSkinDuration = skinToSkinDuration;
          commonViewModel.SkinToSkinAblationTimer.Start();
        }

        commonViewModel.CurrentProcedure.SkinToSkinDuration = skinToSkinDuration;
        commonViewModel.Data.DataAccess.UpdateProcedure(commonViewModel.CurrentProcedure);
      }

      commonViewModel.Console.Disconnect();
      commonViewModel.Console.AblateDisable();
      commonViewModel.Console.ChangeTankDisable();

      commonViewModel.IsAblationProcedureEnded = false; // we have to stop the timer and put the data in the database
      commonViewModel.CanStartTherapy = false;
      CPUTimeWatchdog.IsTimerStarted = false;

      //commonViewModel.CurrentAblation = null; //To be tested
      commonViewModel.CurrentPatient = null;
      commonViewModel.CurrentProcedure = null;
      context.CurrentMainCryoTherapyView = context.PatientView;

      PatientViewModel Patientcontext = context.PatientView.DataContext as PatientViewModel;
      Patientcontext.ResetPatientInfo();

      commonViewModel.SkinToSkinDuration = 0;
      commonViewModel.IsSystemInDataError = false;

      // //AppTrace.Log($"After", LogLevel.Debug, Thread.CurrentThread.ManagedThreadId.ToString(), nameof(MainWindowViewModel), nameof(ManageQuitProcedure));
    }

    /// <summary>
    /// Function/Command that handles the Volume Increase when the Increase Volume
    /// command is invoked.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="arg">The command's parameter (not used in this function).</param>
    private void OnIncreaseVolumeCommand(object arg)
    {
      RequiredVolume += 10;
    }

    /// <summary>
    /// Function that returns if the system can invoke the Increase Volume command.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="arg">The command parameter (not used in this function).</param>
    /// <returns>Boolean value if the system can invoke the command.</returns>
    private bool CanIncreaseVolumeCommand(object arg)
    {
      return true;
    }

    /// <summary>
    /// Function/Command that handles the Volume Decrease when the Decrease Volume
    /// command is invoked.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="arg">The command's parameter (not used in this function).</param>
    private void OnDecreaseVolumeCommand(object arg)
    {
      RequiredVolume -= 10;
    }

    /// <summary>
    /// Function that returns if the system can invoke the Decrease Volume command.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="arg">The command parameter (not used in this function).</param>
    /// <returns>Boolean value if the system can invoke the command.</returns>
    private bool CanDecreaseVolumeCommand(object arg)
    {
      return true;
    }

    /// <summary>
    /// Function/Command that handles the Display Warning view when the Display Warning
    /// command is invoked.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="arg">The command's parameter (not used in this function).</param>
    private void OnDisplayWarningCommand(object arg)
    {
      ViewsEventArgs viewsEvent = new ViewsEventArgs();
      viewsEvent.ViewName = "WarningMessages";
      commonViewModel.OnViewchanged(viewsEvent);
    }

    /// <summary>
    /// Function that returns if the system can invoke the Display Warning command.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="arg">The command parameter (not used in this function).</param>
    /// <returns>Boolean value if the system can invoke the command.</returns>
    private bool CanDisplayWarningCommand(object arg)
    {
      return true;
    }

    /// <summary>
    /// Function/Command that handles the Logout command when the Logout
    /// command is invoked.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="arg">The command's parameter (not used in this function).</param>
    private void OnLogoutCommand(object arg)
    {
      Tuple<long, string, string, string> genericMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID15, (int)Enumeration.ErrorTypes.GUI);

      MessagePopup dialogPopup = new MessagePopup(genericMessage);

      if ((bool)dialogPopup.ShowDialog())
      {
        if ((commonViewModel.IsCryterionUser || commonViewModel.IsBSCADMINUser) && commonViewModel.IsUsedForEngineering)
        {
          commonViewModel.IsCatheterValid = false;
        }
        commonViewModel.LoginManager.CurrentUser = null;
      }
    }

    /// <summary>
    /// Function that returns if the system can invoke the Logout command.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="arg">The command parameter (not used in this function).</param>
    /// <returns>Boolean value if the system can invoke the command.</returns>
    private bool CanLogoutCommand(object arg)
    {
      return true;
    }

    /// <summary>
    /// Function/Command that handles the User Manual view when the User Manual
    /// command is invoked.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="arg">The command's parameter (not used in this function).</param>
    private void OnUserManualCommand(object arg)
    {
      IsUserManualLoading = true;
      DisplayTheUserManual();
      IsUserManualLoading = false;

    }

    private void OnViewChangeCommand(object arg)
    {
      if (CurrentView.DataContext is MainCryoTherapyViewModel vm_ && vm_.CryoTherapyView?.DataContext is CryoTherapyViewModel cryoTherapyViewModel_)
      {
        TherapyViewName = cryoTherapyViewModel_.IsSimpleTherapyViewVisible ? UIConstants.SimpleView : UIConstants.NormalView;
        cryoTherapyViewModel_.IsSimpleTherapyViewVisible = !cryoTherapyViewModel_.IsSimpleTherapyViewVisible;
      }
    }

    private bool CanChangeView(object arg) => true;

    /// <summary>
		/// Function that returns if the system can invoke the User Manual command.
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="arg">The command parameter (not used in this function).</param>
		/// <returns>Boolean value if the system can invoke the command.</returns>
		private bool CanUserManualCommand(object arg)
    {
      return true;
    }
    /// <summary>
    /// Function/Command that handles the change tank.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private void OnChangeTankCommand(object arg)
    {
      commonViewModel.AccessedChangeTankFromCryotherapy = true;

      ViewsEventArgs viewsEvent = new ViewsEventArgs(); ;
      viewsEvent.ViewName = "Tank";
      commonViewModel.OnViewchanged(viewsEvent);
    }

    /// <summary>
    /// Function that returns if the system can invoke change tank command.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private bool CanChangeTankCommand(object arg)
    {
      return true;
    }

    private void DisplayTheUserManual()
    {
      Application.Current.Dispatcher.Invoke((Action)delegate
        {
          try
          {
            DocumentViewerWindow userManualWindow =
              new DocumentViewerWindow(Languages.SelectedUserManualLanguage.UserManualDocument, "USER MANUAL");

            commonViewModel.IsUserManualOpned = true;
            userManualWindow.ShowDialog();
          }
          catch (Exception ex)
          {
            LogSystem.LogService.LogException(ex);
          }
          finally
          {
            commonViewModel.IsUserManualOpned = false;
          }
        });
    }

    /// <summary>
    /// This function handles the sender's PropertyChanged event.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="sender">The View Model that sent the event.</param>
    /// <param name="e">The property changed arguments.</param>
    private void Current_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      CommonViewModel commonviewmodel = sender as CommonViewModel;

      switch (e.PropertyName)
      {
        case "IsWarningVisible":
          RaisePropertyChanged("IsWarningVisible");
          break;

        case "SystemState":
          RaisePropertyChanged("SystemState");
          break;

        case "RequiredVolume":
          RaisePropertyChanged("RequiredVolume");
          break;

        case "ScreenName":
          RaisePropertyChanged("ScreenName");
          //We change the screen we fair an event for the tank
          RaisePropertyChanged("IsUserAllowedToChangeTank");
          if(ScreenName == "Cryo Therapy")
          {
            if(CurrentView.DataContext is MainCryoTherapyViewModel vm_ &&
               vm_.CryoTherapyView?.DataContext is CryoTherapyViewModel cryoTherapyViewModel_)
            {
              TherapyViewName = cryoTherapyViewModel_.IsSimpleTherapyViewVisible ? UIConstants.NormalView : UIConstants.SimpleView;
            }
          }
          break;

        case "CurrentUser":
          RaisePropertyChanged("IsLogoutVisible");
          RaisePropertyChanged("UserName");
          break;

        case "IsUserAllowedToChangeTank":
          RaisePropertyChanged("IsUserAllowedToChangeTank");
          break;

        case "IsBootLoaderUpdatingFirmware":
          RaisePropertyChanged("IsBootLoaderUpdatingFirmware");
          break;
      }
    }
  }
}