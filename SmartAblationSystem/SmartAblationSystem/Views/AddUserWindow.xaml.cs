using SmartAblationSystem.Helpers;
using SmartAblationSystem.ViewModels;
using System;
using System.Windows;

namespace SmartAblationSystem.Views
{
  public partial class AddUserWindow
  {
    private readonly ManageUsersViewModel _manageUsersViewModel;

    public AddUserWindow(ManageUsersViewModel viewModel)
    {
      InitializeComponent();
      DataContext = viewModel;
      _manageUsersViewModel = viewModel;
    }
    private void AdjustWindowPosition()
    {
      var screenWidth = SystemParameters.PrimaryScreenWidth;
      var screenHeight = SystemParameters.PrimaryScreenHeight;

      var centerX = (screenWidth - this.Width) / 2;
      var centerY = (screenHeight - this.Height) / 2;

      this.Left = centerX;
      this.Top = centerY - 100;

    }
    private void OnAddUserWindowLoaded(object sender, RoutedEventArgs e)
    {
      AdjustWindowPosition();
      EnteredPasswordBox.Password = string.Empty;
      EnteredConfirmedPassword.Password = string.Empty;
      _manageUsersViewModel.ClearSelectedPropertiesForAddingNewUser();

    }

    private void Ok_Click(object sender, RoutedEventArgs e)
    {
      DialogResult = true;
      Close();
    }

    private void No_Click(object sender, RoutedEventArgs e)
    {
      DialogResult = false;
      Close();
    }

    private void EnteredPasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
    {
      _manageUsersViewModel.Password = EnteredPasswordBox.Password;
      EnteredPasswordBox.SetSelection(EnteredPasswordBox.Password.Length, 0);
    }

    private void EnteredConfirmedPassword_OnPasswordChanged(object sender, RoutedEventArgs e)
    {
      _manageUsersViewModel.ConfirmPassword = EnteredConfirmedPassword.Password;
      EnteredConfirmedPassword.SetSelection(EnteredConfirmedPassword.Password.Length, 0);
    }
  }
}
