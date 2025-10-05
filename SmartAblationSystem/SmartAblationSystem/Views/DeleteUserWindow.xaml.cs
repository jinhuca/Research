using SmartAblationSystem.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace SmartAblationSystem.Views
{
  public partial class DeleteUserWindow
  {
    private ManageUsersViewModel _manageUsersViewModel;

    public DeleteUserWindow(object dataContext)
    {
      InitializeComponent();
      DataContext = dataContext as ManageUsersViewModel;
      _manageUsersViewModel = (ManageUsersViewModel)DataContext;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
      e.Handled = true;
      CancelDeletion();
    }
    private void CancelButton_PreviewTouchDown(object sender, TouchEventArgs e)
    {
      e.Handled = true;
      CancelDeletion();
    }

    private void Confirm_Click(object sender, RoutedEventArgs e)
    {
      e.Handled = true;
      ConfirmDeletion();
    }

    private void ConfirmButton_PreviewTouchDown(object sender, TouchEventArgs e)
    {
      e.Handled = true;
      ConfirmDeletion();
    }

    private void ConfirmDeletion()
    {
      DialogResult = true;
      Close();
    }

    private void CancelDeletion()
    {
      DialogResult = false;
      Close();
    }
  }
}
