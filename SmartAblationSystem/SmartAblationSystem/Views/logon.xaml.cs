using SmartAblationSystem.ViewModels;
using System.Windows;

namespace SmartAblationSystem.Views
{
  public partial class logon
  {
    public logon(object dataContext)
    {
      InitializeComponent();
      this.DataContext = dataContext as MainWindowViewModel;
    }

    private void LoginClick(object sender, RoutedEventArgs e)
    {
      DialogResult = true;
      Close();
    }

    private void CancelClick(object sender, RoutedEventArgs e)
    {
      DialogResult = false;
      Close();
    }

    private void User_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
      if(TxtUser.Text.ToUpper() == "BSC" || TxtUser.Text.ToUpper() == "BSCADMIN")
      {
        StackPasswordCode.Visibility = Visibility.Visible;
      }
      else
      {
        StackPasswordCode.Visibility = Visibility.Hidden;
      }
    }
  }
}
