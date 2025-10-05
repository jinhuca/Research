using System.Windows;

namespace SmartAblationSystem.Views
{
  public partial class ShutdownWindow
  {
    public ShutdownWindow()
    {
      InitializeComponent();
    }

    private void CancelClick(object sender, RoutedEventArgs e)
    {
      DialogResult = false;
      Close();
    }

    private void ShutdownClick(object sender, RoutedEventArgs e)
    {
      DialogResult = true;
      Close();
    }
  }
}
