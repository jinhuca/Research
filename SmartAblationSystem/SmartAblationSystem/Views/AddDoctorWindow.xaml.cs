using SmartAblationSystem.Helpers;
using SmartAblationSystem.ViewModels;
using System.Windows;

namespace SmartAblationSystem.Views
{
  public partial class AddDoctorWindow
  {
    private readonly ManageUsersViewModel _manageUsersViewModel;

    public AddDoctorWindow(ManageUsersViewModel viewModel)
    {
      InitializeComponent();
      DataContext = viewModel;
      _manageUsersViewModel = viewModel;
    }

    private void OnAddDoctorWindowLoaded(object sender, RoutedEventArgs e)
    {
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
    
    private void NewPasswordChanged(object sender, RoutedEventArgs e)
    {
      _manageUsersViewModel.Password = NewPasswordBox.Password;
      NewPasswordBox.SetSelection(NewPasswordBox.Password.Length, 0);
    }

    private void ConfirmPasswordChanged(object sender, RoutedEventArgs e)
    {
      _manageUsersViewModel.ConfirmPassword = ConfirmPasswordBox.Password;
      ConfirmPasswordBox.SetSelection(ConfirmPasswordBox.Password.Length, 0);
    }
  }
}
