using SmartAblationSystem.Helpers;
using SmartAblationSystem.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace SmartAblationSystem.Views
{
  public partial class EditDoctorWindow
  {
    private readonly ManageUsersViewModel _manageUsersViewModel;
    private readonly int UNEXPANDED_AMOUNT = 0;
    private readonly int EXPANDED_AMOUNT = 100;
    private readonly int ADMIN_EXPANDED_AMOUNT = 200;

    public EditDoctorWindow(ManageUsersViewModel viewModel)
    {
      InitializeComponent();
      DataContext = viewModel;
      _manageUsersViewModel = viewModel;
      _manageUsersViewModel.ClearSelectedPropertiesForEditingUser();
      NewUserNameTextBox.Focus();
    }
    private void OnEditDoctorWindowLoaded(object sender, RoutedEventArgs e)
    {
      AdjustWindowPosition(UNEXPANDED_AMOUNT);

    }
    private void AdjustWindowPosition(int amount)
    {
      var screenWidth = SystemParameters.PrimaryScreenWidth;
      var screenHeight = SystemParameters.PrimaryScreenHeight;

      var centerX = (screenWidth - this.Width) / 2;
      var centerY = (screenHeight - this.Height) / 2;

      this.Left = centerX;
      this.Top = centerY - amount;

      // Ensure the window doesn't move off the top of the screen
      if (this.Top < 0)
      {
        this.Top = 0;
      }
      // Update Shadow on the border
      if (amount == UNEXPANDED_AMOUNT)
      {
        EditDoctorWindowBorder.Height = 502;
      }
      else if (amount == ADMIN_EXPANDED_AMOUNT)
      {
        EditDoctorWindowBorder.Height = 669;
      }
      else
      {
        EditDoctorWindowBorder.Height = 579;
      }
      this.MinHeight = EditDoctorWindowBorder.Height + 50;
    
  }
    private void TxtCurrentPassword_TextChanged(object sender, RoutedEventArgs e)
    {
      _manageUsersViewModel.CurrentPassword = AdminPasswordBox.Password;
    }

    private void NewPasswordBox_Changed(object sender, RoutedEventArgs e)
    {
      _manageUsersViewModel.Password = NewPasswordBox.Password;
      NewPasswordBox.SetSelection(NewPasswordBox.Password.Length, 0);
    }

    private void ConfirmPassword_Changed(object sender, RoutedEventArgs e)
    {
      _manageUsersViewModel.ConfirmPassword = ConfirmPasswordBox.Password;
      ConfirmPasswordBox.SetSelection(ConfirmPasswordBox.Password.Length, 0);
    }

    private void ResetPasswordGrid_MouseUp(object sender, MouseButtonEventArgs e)
    {
      if(e.StylusDevice != null) return;
      ChangeResetPasswordSelection();
      e.Handled = true;
    }

    private void ResetPasswordPanel_PreviewTouchDown(object sender, TouchEventArgs e)
    {
      ChangeResetPasswordSelection();
      e.Handled = true;
    }

    private void ChangeResetPasswordSelection()
    {
      ResetPasswordToggleButton.IsChecked = ResetPasswordToggleButton.IsChecked.HasValue && !ResetPasswordToggleButton.IsChecked.Value;
      if (ResetPasswordToggleButton.IsChecked.Equals(true))
      {
        if (DoctorAdminPasswordGrid.IsVisible)
        {
          AdjustWindowPosition(ADMIN_EXPANDED_AMOUNT);
        }
        else
        {

          // move the window more up
          AdjustWindowPosition(EXPANDED_AMOUNT);
        }
      }
      else
      {
        AdjustWindowPosition(UNEXPANDED_AMOUNT);
      }
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
      CancelEdition();
    }

    private void CancelButton_PreviewTouchDown(object sender, TouchEventArgs e)
    {
      e.Handled = true;
      CancelEdition();
    }

    private void CancelEdition()
    {
      DialogResult = false;
      Close();
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
      SaveEdition();
    }
    private void SaveButton_PreviewTouchDown(object sender, TouchEventArgs e)
    {
      e.Handled = true;
      SaveEdition();
    }

    private void SaveEdition()
    {
      DialogResult = true;
      Close();
    }

    private void NewUserNameTextBox_PreviewTouchDown(object sender, TouchEventArgs e)
    {
      NewUserNameTextBox.Focus();
      NewUserNameTextBox.Select(NewUserNameTextBox.Text.Length, 0);
    }

    private void FirstNameTextBox_PreviewTouchDown(object sender, TouchEventArgs e)
    {
      FirstNameTextBox.Focus();
      FirstNameTextBox.Select(FirstNameTextBox.Text.Length, 0);
    }

    private void LastNameTextBox_PreviewTouchDown(object sender, TouchEventArgs e)
    {
      LastNameTextBox.Focus();
      LastNameTextBox.Select(LastNameTextBox.Text.Length, 0);
    }

    private void AdminPasswordBox_PreviewTouchDown(object sender, TouchEventArgs e)
    {
      AdminPasswordBox.Focus();
      AdminPasswordBox.SetSelection(AdminPasswordBox.Password.Length, 0);
    }

    private void NewPasswordBox_PreviewTouchDown(object sender, TouchEventArgs e)
    {
      NewPasswordBox.Focus();
      NewPasswordBox.SetSelection(NewPasswordBox.Password.Length, 0);
    }

    private void ConfirmPasswordBox_PreviewTouchDown(object sender, TouchEventArgs e)
    {
      ConfirmPasswordBox.Focus();
      ConfirmPasswordBox.SetSelection(ConfirmPasswordBox.Password.Length, 0);
    }
  }
}
