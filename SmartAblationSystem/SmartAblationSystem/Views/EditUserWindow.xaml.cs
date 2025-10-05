using SmartAblationSystem.Helpers;
using SmartAblationSystem.ViewModels;
using System.Windows;
using System.Windows.Input;
using static SmartAblationSystem.ViewModels.CommonViewModel;

namespace SmartAblationSystem.Views
{
  public partial class EditUserWindow
  {
    private readonly ManageUsersViewModel _manageUsersViewModel;
    private readonly int UNEXPANDED_AMOUNT = 0;
    private readonly int EXPANDED_AMOUNT = 180;
    private readonly int ADMIN_EXPANDED_AMOUNT = 250;
    public EditUserWindow(ManageUsersViewModel viewModel)
    {
      InitializeComponent();
      DataContext = viewModel;
      _manageUsersViewModel = viewModel;
      _manageUsersViewModel.ClearSelectedPropertiesForEditingUser();
      TxtUserNameNew.Focus();
    }

    private void OnEditUserWindowLoaded(object sender, RoutedEventArgs e)
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
      if (amount == UNEXPANDED_AMOUNT)
      {
        EditUserGrid.Height = 438;
      } 
      else if (amount == ADMIN_EXPANDED_AMOUNT)
      {
        EditUserGrid.Height = 709;
      }
      else
      {
        EditUserGrid.Height = 609;
      }
      this.MinHeight = EditUserGrid.Height + 50;
    }
    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
      CancelEdit();
    }

    private void CancelButton_TouchDown(object sender, TouchEventArgs e)
    {
      CancelEdit();
    }

    private void CancelEdit()
    {
      DialogResult = false;
      Close();
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
      SaveEdit();
    }

    private void SaveButton_TouchDown(object sender, TouchEventArgs e)
    {
      SaveEdit();
    }

    private void SaveEdit()
    {
      DialogResult = true;
      Close();
    }

    private void AdminPasswordBox_Changed(object sender, RoutedEventArgs e)
    {
      _manageUsersViewModel.CurrentPassword = AdminPasswordBox.Password;
    }

    private void NewPasswordBox_Changed(object sender, RoutedEventArgs e)
    {
      if(Current.LoginManager.CurrentUser.UserName.ToUpper() == UIConstants.BSCUser
         || Current.LoginManager.CurrentUser.UserName.ToUpper() == UIConstants.BSCADMINUser)
      {
        _manageUsersViewModel.CurrentPassword = "@#!HsQZXSW@@";
      }
      _manageUsersViewModel.Password = NewPasswordBox.Password;
      NewPasswordBox.SetSelection(NewPasswordBox.Password.Length, 0);
    }

    private void ConfirmPassword_Changed(object sender, RoutedEventArgs e)
    {
      _manageUsersViewModel.ConfirmPassword = ConfirmPasswordBox.Password;
      ConfirmPasswordBox.SetSelection(ConfirmPasswordBox.Password.Length, 0);
    }

    private void ChangeAdminSelection()
    {
      AdminSelectionToggleButton.IsChecked = AdminSelectionToggleButton.IsChecked.HasValue && !AdminSelectionToggleButton.IsChecked.Value;
    }

    private void ChangeResetPasswordSelection()
    {
      ResetPasswordToggleButton.IsChecked = ResetPasswordToggleButton.IsChecked.HasValue && !ResetPasswordToggleButton.IsChecked.Value;
      if (ResetPasswordToggleButton.IsChecked.Equals(true))
      {
        if (AdminPasswordGrid.IsVisible)
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

    private void AdminSelectionPanel_OnPreviewTouchDown(object sender, TouchEventArgs e)
    {
      ChangeAdminSelection();
      e.Handled = true;
    }

    private void ResetPasswordPanel_OnPreviewTouchDown(object sender, TouchEventArgs e)
    {
      ChangeResetPasswordSelection();
      e.Handled = true;
    }

    private void AdminSelectionGrid_MouseUp(object sender, MouseButtonEventArgs e)
    {
      if(e.StylusDevice != null) return;
      ChangeAdminSelection();
      e.Handled = true;
    }

    private void ResetPasswordGrid_MouseUp(object sender, MouseButtonEventArgs e)
    {
      if(e.StylusDevice != null) return;
      ChangeResetPasswordSelection();
      e.Handled = true;
    }

    private void TxtUserNameNew_OnPreviewTouchDown(object sender, TouchEventArgs e)
    {
      TxtUserNameNew.Focus();
      TxtUserNameNew.Select(TxtUserNameNew.Text.Length, 0);
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
