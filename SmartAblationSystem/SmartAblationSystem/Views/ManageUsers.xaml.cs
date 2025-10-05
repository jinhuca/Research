using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DataAccessLayer;
using LogSystem;
using RijndaelCryptography;
using SmartAblationSystem.ViewModels;
using UniversalLoginManager;
using static SmartAblationSystem.ViewModels.CommonViewModel;

namespace SmartAblationSystem.Views
{
  public partial class ManageUsers
  {
    private ManageUsersViewModel _manageUsersViewModel;

    public ManageUsers()
    {
      InitializeComponent();
    }

    private void ManageUsers_OnLoaded(object sender, RoutedEventArgs e)
    {
      _manageUsersViewModel = (ManageUsersViewModel)DataContext ?? throw new ArgumentException();
      _manageUsersViewModel.HospitalName = Current.Data.DataAccess.GetHospitalName();
    }

    private void EditUser_Click(object sender, RoutedEventArgs e)
    {
      e.Handled = true;

      if(!(e.Source is Button button_))
      {
        return;
      }

      if(!(button_.DataContext is User selectedUser_))
      {
        return;
      }

      InvokeEditUserWindow(selectedUser_);
    }

    private void InvokeEditUserWindow(User selectedUser)
    {
      if(_manageUsersViewModel == null)
      {
        return;
      }

      _manageUsersViewModel.SelectedUser = selectedUser;
      _manageUsersViewModel.Username = selectedUser.UserName;
      var p_ = AncestralPasswordEncrypter.DecryptPassword(selectedUser.Password);
      _manageUsersViewModel.Password = selectedUser.Password;
      _manageUsersViewModel.ConfirmPassword = selectedUser.Password;

      foreach(var type_ in selectedUser.Types)
      {
        if(type_.Id == (int)LoginManager.AccessControlType.ADMIN)
        {
          _manageUsersViewModel.AdminSelected = true;
        }
      }

      var userType_ = selectedUser.Types.FirstOrDefault();
      if(userType_ == null)
      {
        return;
      }

      switch(userType_.Description)
      {
        case UIConstants.DoctorUser:
          try
          {
            var physician_ = Current?.Data?.DataAccess?.GetPhysician(_manageUsersViewModel.Username);
            if(physician_ == null)
            {
              return;
            }
            _manageUsersViewModel.Username = physician_.Name;
            _manageUsersViewModel.DrFirstName = physician_.FirstName;
            _manageUsersViewModel.DrLastName = physician_.LastName;
          }
          catch(Exception ex_)
          {
            LogService.LogException(ex_);
          }
          try
          {
            var doctorEditor_ = new EditDoctorWindow(_manageUsersViewModel)
            {
              WindowStartupLocation = WindowStartupLocation.Manual,
              Top = 100,
              Left = 530
            };
            _manageUsersViewModel.IsDialogInvoked = true;
            var result_ = doctorEditor_.ShowDialog();
            if(result_.HasValue && result_.Value)
            {
              _manageUsersViewModel.EditDoctor();
            }
          }
          catch(Exception ex_)
          {
            LogService.LogException(ex_);
          }
          finally
          {
            _manageUsersViewModel.IsDialogInvoked = false;
          }
          break;
        case UIConstants.RegularUser:
        case UIConstants.AdminUser:
          try
          {
            var userEditor_ = new EditUserWindow(_manageUsersViewModel)
            {
              WindowStartupLocation = WindowStartupLocation.Manual,
              Top = 120,
              Left = 730
            };
            _manageUsersViewModel.IsDialogInvoked = true;
            var result_ = userEditor_.ShowDialog();
            if(result_.HasValue && result_.Value)
            {
              _manageUsersViewModel.EditUser();
            }
          }
          catch(Exception ex_)
          {
            LogService.LogException(ex_);
          }
          finally
          {
            _manageUsersViewModel.IsDialogInvoked = false;
          }
          break;
      }
    }

    private void ManageUsers_OnManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
    {
      e.Handled = true;
    }

    private void ManageUsers_OnUnloaded(object sender, RoutedEventArgs e)
    {
      UserListDataGrid.UnselectAll();
    }
  }
}