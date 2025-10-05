using DataAccessLayer;
using LogSystem;
using Prism.Commands;
using Prism.Mvvm;
using RijndaelCryptography;
using SmartAblationSystem.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using UniversalLoginManager;
using static SmartAblationSystem.Helpers.Enumeration;
using static SmartAblationSystem.Models.Languages;
using static SmartAblationSystem.ViewModels.CommonViewModel;
using static SmartAblationSystem.Views.MessagePopup;

namespace SmartAblationSystem.ViewModels
{
  public class ManageUsersViewModel : BindableBase, INotifyDataErrorInfo
  {
    public ICommand AddDoctorCommand { get; private set; }
    public ICommand AddUserCommand { get; private set; }
    public ICommand DeleteUserCommand { get; private set; }
    public ICommand ReturnToSettingsCommand { get; private set; }

    public ManageUsersViewModel()
    {
      AddDoctorCommand = new DelegateCommand<object>(OnAddDoctorCommand, _ => true);
      AddUserCommand = new DelegateCommand<object>(OnAddUserCommand, _ => true);
      DeleteUserCommand = new DelegateCommand<object>(OnDeleteUserCommand, _ => true);
      ReturnToSettingsCommand = new DelegateCommand<object>(OnReturnToSettingsCommand, _ => true);
      IsDialogInvoked = false;
    }

    private void ResetUserNamePassword()
    {
      Username = string.Empty;
      Password = string.Empty;
      ConfirmPassword = string.Empty;
      CurrentPassword = string.Empty;
      DrFirstName = string.Empty;
      DrLastName = string.Empty;
    }

    private User _selectedUser;
    public User SelectedUser
    {
      get => _selectedUser;
      set => SetProperty(ref _selectedUser, value);
    }

    private bool _isEnteredUserNameValid;
    public bool IsEnteredUserNameValid
    {
      get => _isEnteredUserNameValid;
      set
      {
        SetProperty(ref _isEnteredUserNameValid, value);
        RaisePropertyChanged(nameof(IsAddUserInformValid));
        RaisePropertyChanged(nameof(IsAddDoctorInformValid));
        RaisePropertyChanged(nameof(IsEditUserInformationValid));
        RaisePropertyChanged(nameof(IsEditDoctorInformationValid));
      }
    }

    private bool _isEnteredDrFirstNameValid;
    public bool IsEnteredDrFirstNameValid
    {
      get => _isEnteredDrFirstNameValid;
      set
      {
        SetProperty(ref _isEnteredDrFirstNameValid, value);
        RaisePropertyChanged(nameof(IsAddDoctorInformValid));
        RaisePropertyChanged(nameof(IsEditDoctorInformationValid));
      }
    }

    private bool _isEnteredDrLastNameValid;
    public bool IsEnteredDrLastNameValid
    {
      get => _isEnteredDrLastNameValid;
      set
      {
        SetProperty(ref _isEnteredDrLastNameValid, value);
        RaisePropertyChanged(nameof(IsAddDoctorInformValid));
        RaisePropertyChanged(nameof(IsEditDoctorInformationValid));
      }
    }

    private bool _isEnteredPasswordValid;
    public bool IsEnteredPasswordValid
    {
      get => _isEnteredPasswordValid;
      set
      {
        SetProperty(ref _isEnteredPasswordValid, value);
        RaisePropertyChanged(nameof(IsAddUserInformValid));
        RaisePropertyChanged(nameof(IsAddDoctorInformValid));
        RaisePropertyChanged(nameof(IsEditUserInformationValid));
        RaisePropertyChanged(nameof(IsEditDoctorInformationValid));
      }
    }

    private bool _isConfirmPasswordMatched;
    public bool IsConfirmPasswordMatched
    {
      get => _isConfirmPasswordMatched;
      set
      {
        SetProperty(ref _isConfirmPasswordMatched, value);
        RaisePropertyChanged(nameof(IsAddUserInformValid));
        RaisePropertyChanged(nameof(IsAddDoctorInformValid));
        RaisePropertyChanged(nameof(IsEditUserInformationValid));
        RaisePropertyChanged(nameof(IsEditDoctorInformationValid));
      }
    }

    public ObservableCollection<User> UserList
    {
      get
      {
        //Admin user must not be able to see the list of Cryterion users.
        if(Current.IsAdminUser || Current.IsCryterionUser || Current.IsBSCADMINUser)
        {
          var userList_ = Current.LoginManager.Users;
          var userWithoutCryterion_ = new ObservableCollection<User>();
          bool isCryterion_ = false;

          //Create a new observable collection with only the user that are not Cryterion type.
          foreach(var user in userList_)
          {
            User user_ = user;
            isCryterion_ = false;
            foreach(var userType_ in user_.Types)
            {
              if(userType_.Id == (int)LoginManager.AccessControlType.CRYTERION || userType_.Id == (int)LoginManager.AccessControlType.BSCADMIN)
              {
                isCryterion_ = true;
              }
            }
            if(!isCryterion_)
            {
              userWithoutCryterion_.Add(user_);
            }
          }
          return userWithoutCryterion_;
        }
        return Current.LoginManager.Users;
      }
    }

    private string _hospitalName;
    public string HospitalName
    {
      get => _hospitalName;
      set => SetProperty(ref _hospitalName, value);
    }

    public bool IsBSCUser => Current.IsCryterionUser || Current.IsBSCADMINUser;

    private string _username = string.Empty;
    public string Username
    {
      get => _username;
      set
      {
        SetProperty(ref _username, value);
        ValidateUserName(value);
        IsEnteredUserNameValid = GetErrors(nameof(Username)) == null;
        RaisePropertyChanged(nameof(IsEditUserInformationValid));
        RaisePropertyChanged(nameof(IsEditDoctorInformationValid));
      }
    }

    private string _drFirstName = string.Empty;
    public string DrFirstName
    {
      get => _drFirstName;
      set
      {
        SetProperty(ref _drFirstName, value);
        ValidateDrFirstName(value);
        IsEnteredDrFirstNameValid = GetErrors(nameof(DrFirstName)) == null;
        RaisePropertyChanged(nameof(IsEnteredDrFirstNameValid));
        RaisePropertyChanged(nameof(IsEditDoctorInformationValid));
      }
    }

    private string _drLastName = string.Empty;
    public string DrLastName
    {
      get => _drLastName;
      set
      {
        SetProperty(ref _drLastName, value);
        ValidateDrLastName(value);
        IsEnteredDrLastNameValid = GetErrors(nameof(DrLastName)) == null;
        RaisePropertyChanged(nameof(IsEnteredDrLastNameValid));
        RaisePropertyChanged(nameof(IsEditDoctorInformationValid));
      }
    }

    private preference _preference;

    public preference Preferences
    {
      get => _preference;
      set => SetProperty(ref _preference, value);
    }

    private string _password = string.Empty;
    public string Password
    {
      get => _password;
      set
      {
        ValidatePassword(value);
        IsEnteredPasswordValid = GetErrors(nameof(Password)) == null;
        ValidateConfirmPassword(ConfirmPassword);
        SetProperty(ref _password, value);
      }
    }

    private string _confirmPassword = string.Empty;
    public string ConfirmPassword
    {
      get => _confirmPassword;
      set
      {
        ValidateConfirmPassword(value);
        IsConfirmPasswordMatched = GetErrors(nameof(ConfirmPassword)) == null;
        SetProperty(ref _confirmPassword, value);
      }
    }

    private bool _adminSelected;
    public bool AdminSelected
    {
      get => _adminSelected;
      set
      {
        SetProperty(ref _adminSelected, value);
        RaisePropertyChanged(nameof(IsEditUserInformationValid));
      }
    }

    private string _currentPassword = string.Empty;
    public string CurrentPassword
    {
      get => _currentPassword;
      set => SetProperty(ref _currentPassword, value);
    }

    public bool IsAddUserInformValid =>
      IsEnteredUserNameValid &&
      IsEnteredPasswordValid &&
      IsConfirmPasswordMatched;

    public bool IsAddDoctorInformValid =>
      IsEnteredUserNameValid &&
      IsEnteredDrFirstNameValid &&
      IsEnteredDrLastNameValid &&
      IsEnteredPasswordValid &&
      IsConfirmPasswordMatched;

    public bool IsEditUserInformationValid => ResetPasswordSelected
      ? IsEnteredUserNameValid &&
        IsEnteredPasswordValid &&
        IsConfirmPasswordMatched
      : IsEnteredUserNameValid;

    public bool IsEditDoctorInformationValid => ResetPasswordSelected
      ? IsEnteredUserNameValid &&
        IsEnteredDrFirstNameValid &&
        IsEnteredDrLastNameValid &&
        IsEnteredPasswordValid &&
        IsConfirmPasswordMatched
      : IsEnteredUserNameValid &&
        IsEnteredDrFirstNameValid &&
        IsEnteredDrLastNameValid;

    private bool _resetPasswordSelected;
    public bool ResetPasswordSelected
    {
      get => _resetPasswordSelected;
      set
      {
        SetProperty(ref _resetPasswordSelected, value);
        if(value)
        {
          Password = string.Empty;
          ConfirmPassword = string.Empty;
        }
        RaisePropertyChanged(nameof(IsEditDoctorInformationValid));
        RaisePropertyChanged(nameof(IsEditUserInformationValid));
      }
    }

    private bool _isDialogInvoked;
    public bool IsDialogInvoked
    {
      get => _isDialogInvoked;
      set => SetProperty(ref _isDialogInvoked, value);
    }

    private void OnAddDoctorCommand(object obj)
    {
      var addDoctorWindow_ = new AddDoctorWindow(this);
      IsDialogInvoked = true;
      ResetUserNamePassword();

      var dialogResult_ = addDoctorWindow_.ShowDialog();
      if(dialogResult_.HasValue && dialogResult_.Value)
      {
        if(!Current.Data.DataAccess.DoesUserNameAlreadyExists(Username))
        {
          if(!Current.Data.DataAccess.DoesDoctorFullNameAlreadyExists(DrFirstName, DrLastName))
          {
            var passwordEntered_ = AncestralPasswordEncrypter.EncryptPassword(Password);
            var physicianId_ = Current.Data.DataAccess.AddNewUser(
              userName: Username,
              userPassword: passwordEntered_,
              userType: (int)LoginManager.AccessControlType.DOCTOR,
              lastname: DrLastName,
              firstname: DrFirstName);

            if(Preferences == null)
            {
              Preferences = new preference
              {
                CoolingRequiredTargetTemperature = -30,
                ThawTimerToTemperature = 0,
                LowAblationTemperatureAlarm = -45.0,
                HighAblationTemperatureAlarm = 30.0,
                EsophagusTemperature = 20.0,
                DiaphragmAmplitude = 80,
                DiaphragmAmplitudeType = 0,
                BalloonPressureSelected = false,
                TipPressureSelected = false,
                AblationTimer = 240,
                IsUsingAutoDeflation = true,
                VolumeLevel = 50,
                CurveStyle = 2,
                CurveColor = 0,
                VeinIsolationDuration = 0,
                RequestedAblationTime = 0,
                DMSDetectionThreshold = 0.04,
                RefrigerantLevelUnit = 0,
                DiaphragmSensorGain = 100,
                IsUsingInflationFastSpeed = false,
                IsUsingAudioAlert = true
              };
            }

            Current.Data.DataAccess.AddPhysician(
              username: Username,
              physicianId: physicianId_,
              firstname: DrFirstName,
              lastname: DrLastName,
              Preferences.CoolingRequiredTargetTemperature,
              Preferences.ThawTimerToTemperature,
              Preferences.LowAblationTemperatureAlarm,
              Preferences.HighAblationTemperatureAlarm,
              Preferences.EsophagusTemperature,
              Preferences.DiaphragmAmplitude,
              Preferences.DiaphragmAmplitudeType,
              Preferences.BalloonPressureSelected,
              Preferences.TipPressureSelected,
              Preferences.AblationTimer,
              Preferences.IsUsingAutoDeflation,
              Preferences.VolumeLevel,
              Preferences.CurveStyle,
              Preferences.CurveColor,
              background: "Black",
              Preferences.VeinIsolationDuration,
              Preferences.RequestedAblationTime,
              Preferences.DMSDetectionThreshold,
              Preferences.RefrigerantLevelUnit,
              Preferences.DiaphragmSensorGain,
              Preferences.IsUsingInflationFastSpeed,
              Preferences.IsUsingAudioAlert);

            RaisePropertyChanged(nameof(UserList));
            Current.PhysicianList = Current.Data.DataAccess.GetAllActivePhysicians();
            Current.LogUserAction(Actions.CreateUser);
          }
          else
          {
            var physicianNameMessage_ = ErrorsAndCryterionSolutionTranslations((int)GUIMessages.ID21, (int)ErrorTypes.GUI);
            var alreadyExistsMessage_ = ErrorsAndCryterionSolutionTranslations((int)GUIMessages.ID17, (int)ErrorTypes.GUI);
            var usernameExistsMessage_ = ErrorsAndCryterionSolutionTranslations((int)GUIMessages.ID18, (int)ErrorTypes.GUI);

            var messagePopup_ = new MessagePopup(
              physicianNameMessage_.Item2 + " " + DrFirstName + " " + DrLastName + " " + alreadyExistsMessage_.Item2,
              MessageType.WarningMessage, 
              ButtonType.Ok, 
              string.Empty);
            
            messagePopup_.ShowDialog();
          }
        }
        else
        {
          //The User already exists
          var user_ = Current.Data.DataAccess.GetUser(Username);

          if(user_ != null)
          {
            var usernameMessage_ = ErrorsAndCryterionSolutionTranslations((int)GUIMessages.ID16, (int)ErrorTypes.GUI);

            //The user already exists and is ACTIVE.
            if(user_.Status)
            {
              var physicianNameMessage_ = ErrorsAndCryterionSolutionTranslations((int)GUIMessages.ID21, (int)ErrorTypes.GUI);
              var alreadyExistsMessage_ = ErrorsAndCryterionSolutionTranslations((int)GUIMessages.ID17, (int)ErrorTypes.GUI);
              var usernameExistsMessage_ = ErrorsAndCryterionSolutionTranslations((int)GUIMessages.ID18, (int)ErrorTypes.GUI);

              var messagePopup_ = new MessagePopup(
                physicianNameMessage_.Item2 + " " + Username + " " + alreadyExistsMessage_.Item2,
                MessageType.WarningMessage, 
                ButtonType.Ok, 
                usernameExistsMessage_.Item2);

              messagePopup_.ShowDialog();
            }
            else
            {
              var inactiveMessage_ = ErrorsAndCryterionSolutionTranslations((int)GUIMessages.ID19, (int)ErrorTypes.GUI);
              var reactivateMessage_ = ErrorsAndCryterionSolutionTranslations((int)GUIMessages.ID20, (int)ErrorTypes.GUI);

              //The existing user is INACTIVE, ask if we want to reactivate the user.
              //If so, change its password.
              var messagePopup_ = new MessagePopup(
                usernameMessage_.Item2 + " " + Username + " " + inactiveMessage_.Item2,
                MessageType.SystemMessage, 
                ButtonType.YesNo,
                reactivateMessage_.Item2);

              if((bool)messagePopup_.ShowDialog())
              {
                //Reactivate the user and change its password.
                user_.Status = true;
                user_.Password = Password;
                Current.Data.DataAccess.UpdateUser(user_);
                RaisePropertyChanged(nameof(UserList));
              }
            }
          }
        }
      }

      IsDialogInvoked = false;
    }

    private void OnAddUserCommand(object obj)
    {
      IsDialogInvoked = true;
      ResetUserNamePassword();
      var addUserWindow_ = new AddUserWindow(this);

      try
      {
        var dialog_ = addUserWindow_.ShowDialog();
        if(!dialog_.HasValue || !dialog_.Value)
        {
          return;
        }

        if(Current.Data.DataAccess.DoesUserNameAlreadyExists(Username))
        {
          var user_ = Current.Data.DataAccess.GetUser(Username);

          if(user_ != null)
          {
            var (_, item2_, _, _) = ErrorsAndCryterionSolutionTranslations(
              errorId: (int)GUIMessages.ID16,
              errorType: (int)ErrorTypes.GUI);

            if(user_.Status)
            {
              var alreadyExistsMessage_ = ErrorsAndCryterionSolutionTranslations((int)GUIMessages.ID17, (int)ErrorTypes.GUI);
              var usernameExistsMessage_ = ErrorsAndCryterionSolutionTranslations((int)GUIMessages.ID18, (int)ErrorTypes.GUI);
              var messagePopup_ = new MessagePopup(
                item2_ + " " + Username + " " + alreadyExistsMessage_.Item2,
                MessageType.WarningMessage, ButtonType.Ok,
                usernameExistsMessage_.Item2);
              
              messagePopup_.ShowDialog();
            }
            else
            {
              var inactiveMessage_ = ErrorsAndCryterionSolutionTranslations((int)GUIMessages.ID19, (int)ErrorTypes.GUI);
              var reactivateMessage_ = ErrorsAndCryterionSolutionTranslations((int)GUIMessages.ID20, (int)ErrorTypes.GUI);
              var messagePopup_ = new MessagePopup(
                item2_ + " " + Username + " " + inactiveMessage_.Item2,
                MessageType.SystemMessage, 
                ButtonType.YesNo,
                reactivateMessage_.Item2);
              
              if((bool)messagePopup_.ShowDialog())
              {
                user_.Status = true;
                user_.Password = Password;
                Current.Data.DataAccess.UpdateUser(user_);
                RaisePropertyChanged(nameof(UserList));
              }
            }
          }
        }
        else
        {
          var passwordEntered_ = AncestralPasswordEncrypter.EncryptPassword(Password);
          Current.Data.DataAccess.AddNewUser(
            userName: Username,
            userPassword: passwordEntered_,
            userType: AdminSelected ? (int)LoginManager.AccessControlType.ADMIN : (int)LoginManager.AccessControlType.USER,
            lastname: UIConstants.TempName,
            firstname: UIConstants.TempName);
          Current.LogUserAction(Actions.CreateUser);
          RaisePropertyChanged(nameof(UserList));
        }
      }
      catch(Exception ex_)
      {
        LogService.LogException(ex_);
      }
      finally
      {
        IsDialogInvoked = false;
      }
    }

    public bool EditUser()
    {
      if(SelectedUser == null) return false;

      if (Current.Data.DataAccess.DoesUserNameAlreadyExists(Username) &&
          SelectedUser.UserName != Username)
      {
        var usernameMessage_ = ErrorsAndCryterionSolutionTranslations((int)GUIMessages.ID16, (int)ErrorTypes.GUI);
        var alreadyExistsMessage_ = ErrorsAndCryterionSolutionTranslations((int)GUIMessages.ID17, (int)ErrorTypes.GUI);
        var usernameExistsMessage_ = ErrorsAndCryterionSolutionTranslations((int)GUIMessages.ID18, (int)ErrorTypes.GUI);

        var messagePopup_ = new MessagePopup(
          message: usernameMessage_.Item2 + " " + Username + " " + alreadyExistsMessage_.Item2,
          messageType: MessageType.WarningMessage, 
          buttonType: ButtonType.Ok,
          messageTitle: usernameExistsMessage_.Item2);

        messagePopup_.ShowDialog();
        return false;
      }

      var originalUserName_ = SelectedUser.UserName;
      SelectedUser.UserName = Username;

      var selectedUserType_ = new DataAccessLayer.Type
      {
        Id = AdminSelected ? (int)LoginManager.AccessControlType.ADMIN : (int)LoginManager.AccessControlType.USER
      };

      if(ResetPasswordSelected)
      {
        ResetPassword();
      }
      else
      {
        KeepPassword(originalUserName_);
      }

      bool editResult_;
      try
      {
        Current.Data.DataAccess.UpdateUser(SelectedUser, selectedUserType_.Id);
        Current.LogUserAction(Actions.EditUser);
        editResult_ = true;
      }
      catch(Exception ex_)
      {
        editResult_ = false;
        LogService.LogException(ex_);
      }
      RaisePropertyChanged(nameof(UserList));
      return editResult_;
    }

    public bool EditDoctor()
    {
      if(SelectedUser == null) return false;
      var editDoctorResult_ = false;
      var originalDoctorName_ = SelectedUser.UserName;

      if(ResetPasswordSelected)
      {
        ResetPassword();
      }
      else
      {
        KeepPassword(originalDoctorName_);
      }

      try
      {
        var isDoctorFullNameExists_ = false;
        var physician_ = Current.Data.DataAccess.GetPhysician(SelectedUser.UserName);

        if(!Current.Data.DataAccess.DoesUserNameAlreadyExists(SelectedUser, Username))
        {
          if((physician_.LastName != DrLastName || physician_.FirstName != DrFirstName) && SelectedUser.UserName == Username)
          {
            if(!Current.Data.DataAccess.DoesDoctorFullNameAlreadyExists(DrFirstName, DrLastName, Username))
            {
              physician_.LastName = DrLastName;
              physician_.FirstName = DrFirstName;
            }
            else
            {
              isDoctorFullNameExists_ = true;
            }
          }
          else if((physician_.LastName != DrLastName || physician_.FirstName != DrFirstName) && SelectedUser.UserName != Username)
          {
            if(!Current.Data.DataAccess.DoesDoctorFullNameAlreadyExists(DrFirstName, DrLastName))
            {
              physician_.LastName = DrLastName;
              physician_.FirstName = DrFirstName;
            }
            else
            {
              isDoctorFullNameExists_ = true;
            }
          }
          var userType_ = new DataAccessLayer.Type();
          if(!isDoctorFullNameExists_)
          {
            SelectedUser.UserName = Username;
            SelectedUser.LastName = DrLastName;
            SelectedUser.FirstName = DrFirstName;
            userType_.Id = (int)LoginManager.AccessControlType.DOCTOR;
            Current.Data.DataAccess.UpdateUser(SelectedUser, userType_.Id);

            if(physician_ != null)
            {
              physician_.Name = Username;
              Current.Data.DataAccess.UpdatePhysician(physician_);
            }

            Current.PhysicianList = Current.Data.DataAccess.GetAllActivePhysicians();
            Current.LogUserAction(Actions.EditUser);
            RaisePropertyChanged(nameof(UserList));
          }
          else
          {
            var usernameMessage_ = ErrorsAndCryterionSolutionTranslations((int)GUIMessages.ID21, (int)ErrorTypes.GUI);
            var alreadyExistsMessage_ = ErrorsAndCryterionSolutionTranslations((int)GUIMessages.ID17, (int)ErrorTypes.GUI);
            var usernameExistsMessage_ = ErrorsAndCryterionSolutionTranslations((int)GUIMessages.ID18, (int)ErrorTypes.GUI);

            var messagePopup_ = new MessagePopup(
              message: usernameMessage_.Item2 + " " + DrFirstName + " " + DrLastName + " " + alreadyExistsMessage_.Item2,
              messageType: MessageType.WarningMessage, 
              buttonType: ButtonType.Ok,
              "");
            
            messagePopup_.ShowDialog();
          }
        }
        else
        {
          var usernameMessage_ = ErrorsAndCryterionSolutionTranslations((int)GUIMessages.ID21, (int)ErrorTypes.GUI);
          var alreadyExistsMessage_ = ErrorsAndCryterionSolutionTranslations((int)GUIMessages.ID17, (int)ErrorTypes.GUI);
          var usernameExistsMessage_ = ErrorsAndCryterionSolutionTranslations((int)GUIMessages.ID18, (int)ErrorTypes.GUI);

          var messagePopup_ = new MessagePopup(
            usernameMessage_.Item2 + " " + Username + " " + alreadyExistsMessage_.Item2,
            MessageType.WarningMessage, 
            ButtonType.Ok,
            usernameExistsMessage_.Item2);
          
          messagePopup_.ShowDialog();
        }

        editDoctorResult_ = true;
      }
      catch(Exception ex_)
      {
        editDoctorResult_ = false;
        LogService.LogException(ex_);
      }

      return editDoctorResult_;
    }

    private void OnDeleteUserCommand(object obj)
    {
      if(SelectedUser == null)
      {
        return;
      }

      switch(obj)
      {
        case EditUserWindow userWin_:
          userWin_.Opacity = UIConstants.OpacityDisabled;
          break;
        case EditDoctorWindow doctorWin_:
          doctorWin_.Opacity = UIConstants.OpacityDisabled;
          break;
      }

      Username = SelectedUser.UserName;
      DeleteUserMessage = $"User {Username} will be permanently deleted.";

      var deleteWindow_ = new DeleteUserWindow(this);
      var result_ = deleteWindow_.ShowDialog();

      if(result_.HasValue && result_.Value == false)
      {
        DeleteUserMessage = string.Empty;
      }
      else
      {
        Current.Data.DataAccess.DeleteUser(SelectedUser);
        Current.LogUserAction(Actions.DeleteUser);
        Current.PhysicianList = Current.Data.DataAccess.GetAllActivePhysicians();
        Username = string.Empty;
        RaisePropertyChanged(nameof(UserList));

        switch(obj)
        {
          case EditUserWindow userWin_:
            userWin_.Close();
            break;
          case EditDoctorWindow doctorWin_:
            doctorWin_.Close();
            break;
        }
      }

      DeleteUserMessage = string.Empty;
      switch(obj)
      {
        case EditUserWindow userWin_:
          userWin_.Opacity = UIConstants.OpacityEnabled;
          break;
        case EditDoctorWindow doctorWin_:
          doctorWin_.Opacity = UIConstants.OpacityEnabled;
          break;
      }
    }

    private string _deleteUserMessage;

    public string DeleteUserMessage
    {
      get => _deleteUserMessage;
      set => SetProperty(ref _deleteUserMessage, value);
    }

    public void ResetPassword()
    {
      if(SelectedUser == null)
      {
        return;
      }

      ResetPasswordSelected = false;
      var loginPassword_ = AncestralPasswordEncrypter.DecryptPassword(Current.CurrentUser.Password);
      if(loginPassword_ == CurrentPassword || Current.IsCryterionUser || Current.IsBSCADMINUser)
      {
        SelectedUser.Password = AncestralPasswordEncrypter.EncryptPassword(Password);
        Current.Data.DataAccess.UpdateUser(SelectedUser);
        Current.LogUserAction(Actions.ResetPassword);
        RaisePropertyChanged(nameof(UserList));
      }
      else
      {
        var genericMessage_ = ErrorsAndCryterionSolutionTranslations((int)GUIMessages.ID25, (int)ErrorTypes.GUI);
        var isNotValidMessage_ = ErrorsAndCryterionSolutionTranslations((int)GUIMessages.ID26, (int)ErrorTypes.GUI);
        var passwordInvalidMessage_ = ErrorsAndCryterionSolutionTranslations((int)GUIMessages.ID27, (int)ErrorTypes.GUI);
        var messagePopup_ = new MessagePopup(
          message: genericMessage_.Item2 + " user " + isNotValidMessage_.Item2,
          messageType: MessageType.WarningMessage,
          buttonType: ButtonType.Ok,
          messageTitle: passwordInvalidMessage_.Item2);
        messagePopup_.ShowDialog();
      }
    }

    public void KeepPassword(string originUserName)
    {
      if(SelectedUser == null)
      {
        return;
      }

      try
      {
        SelectedUser.Password = Current.Data.DataAccess.GetUser(originUserName).Password;
      }
      catch(Exception ex_)
      {
        LogService.LogException(ex_);
      }
    }

    private void OnReturnToSettingsCommand(object obj)
    {
      var viewsEvent_ = new ViewsEventArgs { ViewName = "BackToSettings" };
      Current.OnViewchanged(viewsEvent_);
    }

    #region INotifyDataErrorInfo Interface

    public IEnumerable GetErrors(string propertyName)
      => _errorsByPropertyName.ContainsKey(propertyName) ? _errorsByPropertyName[propertyName] : null;

    public bool HasErrors => _errorsByPropertyName.Any();

    public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

    #endregion INotifyDataErrorInfo Interface

    #region INotifyDataErrorInfo Implementation

    private readonly Dictionary<string, List<string>> _errorsByPropertyName = new Dictionary<string, List<string>>();

    private readonly Regex _userNameValidationRegex = new Regex("^[a-zA-Z0-9 _,-]*$", RegexOptions.Compiled);

    private readonly Regex _passwordValidationRegex = new Regex("^(?=.*[0-9]).{8,16}$", RegexOptions.Compiled);

    private void AddError(string propertyName, string error)
    {
      if(!_errorsByPropertyName.ContainsKey(propertyName))
      {
        _errorsByPropertyName[propertyName] = new List<string>();
      }
      if(!_errorsByPropertyName[propertyName].Contains(error))
      {
        _errorsByPropertyName[propertyName].Add(error);
        RaiseErrorsChanged(propertyName);
      }
    }

    public void ClearErrors(string propertyName)
    {
      if(_errorsByPropertyName.ContainsKey(propertyName))
      {
        _errorsByPropertyName.Remove(propertyName);
        RaiseErrorsChanged(propertyName);
      }
    }

    private void RaiseErrorsChanged(string propertyName)
      => ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));

    private void ValidateUserName(string name)
    {
      ClearErrors(nameof(Username));
      if(string.IsNullOrEmpty(name))
      {
        AddError(nameof(Username), UIConstants.UserNameEmptyErrorMessage);
        return;
      }
      if(!_userNameValidationRegex.IsMatch(name))
      {
        AddError(nameof(Username), UIConstants.UserNameInvalidMessage);
      }
    }

    private void ValidateDrFirstName(string firstName)
    {
      ClearErrors(nameof(DrFirstName));
      if(string.IsNullOrEmpty(firstName))
      {
        AddError(nameof(DrFirstName), UIConstants.DrFirstNameEmptyErrorMessage);
        return;
      }
      if(!_userNameValidationRegex.IsMatch(firstName))
      {
        AddError(nameof(DrFirstName), UIConstants.DrFirstNameInvalidMessage);
      }
    }

    private void ValidateDrLastName(string lastName)
    {
      ClearErrors(nameof(DrLastName));
      if(string.IsNullOrEmpty(lastName))
      {
        AddError(nameof(DrLastName), UIConstants.DrLastNameEmptyErrorMessage);
        return;
      }
      if(!_userNameValidationRegex.IsMatch(lastName))
      {
        AddError(nameof(DrLastName), UIConstants.DrLastNameInvalidMessage);
      }
    }
    
    private void ValidatePassword(string pw)
    {
      ClearErrors(nameof(Password));
      if(string.IsNullOrEmpty(pw))
      {
        AddError(nameof(Password), UIConstants.PasswordEmptyErrorMessage);
        return;
      }
      if(!_passwordValidationRegex.IsMatch(pw))
      {
        AddError(nameof(Password), UIConstants.PasswordInvalidMessage);
        IsEnteredPasswordValid = false;
      }
      else
      {
        IsEnteredPasswordValid = true;
      }
    }

    private void ValidateConfirmPassword(string cpw)
    {
      ClearErrors(nameof(ConfirmPassword));
      if(cpw != Password || !_passwordValidationRegex.IsMatch(Password))
      {
        AddError(nameof(ConfirmPassword), UIConstants.PasswordNotMatchMessage);
        IsConfirmPasswordMatched = false;
      }
      else
      {
        IsConfirmPasswordMatched = true;
      }
    }

    #endregion INotifyDataErrorInfo Implementation

    public void ClearSelectedPropertiesForAddingNewUser()
    {
      Password = string.Empty;
      ConfirmPassword = string.Empty;
      AdminSelected = false;

      ClearErrors(nameof(Username));
      ClearErrors(nameof(DrFirstName));
      ClearErrors(nameof(DrLastName));
      ClearErrors(nameof(Password));
      ClearErrors(nameof(ConfirmPassword));
    }

    public void ClearSelectedPropertiesForEditingUser()
    {
      Password = string.Empty;
      ConfirmPassword = string.Empty;
      ResetPasswordSelected = false;
      AdminSelected = GetUserAdminPermission(SelectedUser);

      ClearErrors(nameof(Username));
      ClearErrors(nameof(DrFirstName));
      ClearErrors(nameof(DrLastName));
      ClearErrors(nameof(Password));
      ClearErrors(nameof(ConfirmPassword));
    }

    private bool GetUserAdminPermission(User userToCheck)
    {
      bool result_ = false;
      foreach(var type_ in userToCheck.Types)
      {
        if(type_.Id == (int)LoginManager.AccessControlType.ADMIN)
        {
          result_ = true;
        }
      }
      return result_;
    }
  }
}