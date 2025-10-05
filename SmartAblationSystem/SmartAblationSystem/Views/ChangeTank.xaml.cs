using SmartAblationSystem.ViewModels;
using System;
using System.Windows;

namespace SmartAblationSystem.Views
{
  /// <summary>
  /// Interaction logic for ChangeTank.xaml
  /// </summary>
  public partial class ChangeTank
  {
    public ChangeTank()
    {
      InitializeComponent();

      VideoPlayer.MediaEnded += VideoPlayer_MediaEnded;
      VideoPlayer.Play();
    }

    /// <summary>
    /// Occurs when the VideoPlayer_MediaEnded event is raised.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private void VideoPlayer_MediaEnded(object sender, RoutedEventArgs e)
    {
      VideoPlayer.Position = TimeSpan.Zero;
      VideoPlayer.Play();
    }
    /// <summary>
    /// Occurs when the UserControl_Loaded event is raised.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="sender">The component that raised the event.</param>
    /// <param name="e">Represents the base class for classes that contain user control event data</param>
    private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
      CommonViewModel.Current.CanChangeTank = true;
      CommonViewModel.Current.Console.GUIInMaintenanceMode = true;
    }

    /// <summary>
    /// Occurs when the UserControl_Unloaded event is raised.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="sender">The component that raised the event.</param>
    /// <param name="e">Represents the base class for classes that contain user control event data</param>
    private void UserControl_Unloaded(object sender, System.Windows.RoutedEventArgs e)
    {
      CommonViewModel.Current.CanChangeTank = false;
      CommonViewModel.Current.Console.GUIInMaintenanceMode = false;

      var viewModel = DataContext as ChangeTankViewModel; 
      viewModel?.ResetCommand.Execute(null);
    }
  }
}
