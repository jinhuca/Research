namespace SmartAblationSystem.Views
{
  using System;

  using SmartAblationSystem.Models;
  using SmartAblationSystem.ViewModels;
  using System.Windows;

  using DataAccessLayer;

  /// <summary>
  /// Interaction logic for Notifications.xaml
  /// </summary>
  public partial class Notifications
  {
    private NotificationsViewModel _viewModel;

    private NotificationModel notificationModel = NotificationModel.Instance;
    private preference _preference;

    public Notifications()
    {
      this.InitializeComponent();

      _preference = notificationModel.CurrentPhysician.preference;
      _viewModel = this.DataContext as NotificationsViewModel;
    }

    /// <summary>
    /// Occurs when the Apply_Click event is raised.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="sender">The component that raised the event.</param>
    /// <param name="e">Contains state information and event data associated with a routed event.</param>
    private void Apply_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        _viewModel.ApplySettings();

        DialogResult = true;
        this.Close();
      }
      catch (Exception ex)
      {
        // TODO
        ex.ToString();
      }
    }

    /// <summary>
    /// Occurs when the No_Click event is raised.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="sender">The component that raised the event.</param>
    /// <param name="e">Contains state information and event data associated with a routed event.</param>
    private void No_Click(object sender, RoutedEventArgs e)
    {
      DialogResult = false;
      this.Close();
    }


    private void SaveToDBBtn_OnClick(object sender, RoutedEventArgs e)
    {
      try
      {
        _viewModel.SaveSettingsToPreference();
        _viewModel.ApplySettings();
      }
      catch (Exception ex)
      {
        LogSystem.LogService.LogException(ex);
      }
      finally
      {
        DialogResult = true;
        this.Close();
      }
    }

    private void ResetFromDBBtn_OnClick(object sender, RoutedEventArgs e)
    { 
      _viewModel.ResetSettingsFromPreferences(_preference);
    }
  }
}
