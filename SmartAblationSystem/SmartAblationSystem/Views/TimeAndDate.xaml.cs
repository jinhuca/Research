using System.Windows;
using SmartAblationSystem.ViewModels;

namespace SmartAblationSystem.Views
{
  /// <summary>
  /// Interaction logic for TimeAndDate.xaml
  /// </summary>
  public partial class TimeAndDate 
  {
    public TimeAndDate()
    {
      InitializeComponent();
    }

    private void TimeAndDate_OnLoaded(object sender, RoutedEventArgs e)
    {
      var viewModel = this.DataContext as TimeAndDateViewModel;
      viewModel?.RefreshDisplay();
    }
  }
}
