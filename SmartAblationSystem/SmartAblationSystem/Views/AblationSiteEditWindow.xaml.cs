using SmartAblationSystem.ViewModels;
using System;
using System.Windows;
using Shared;
using static LogSystem.LogService;

namespace SmartAblationSystem.Views
{
  public partial class AblationSiteEditWindow
  {
    private readonly AblationSiteEnum _currentAblationSiteEnum;
    private readonly IAblationSiteAware _ablationSiteAware;
    private readonly AblationSiteEditWindowViewModel _viewModel;

    public AblationSiteEditWindow(IAblationSiteAware ablationSiteAware)
    {
      InitializeComponent();
      _ablationSiteAware = ablationSiteAware;
      _viewModel = DataContext as AblationSiteEditWindowViewModel;

      _viewModel.SelectedAblationSite = _ablationSiteAware.AblationSite;
      _currentAblationSiteEnum = _ablationSiteAware.AblationSite;

      //display warning when in playback mode
      _viewModel.DisplayAblationSiteWarning = _ablationSiteAware.DisplayAblationSiteWarning;
      _ablationSiteAware.AblationSite = AblationSiteEnum.UNKNOWN;
    }

    private void Ok_Click(object sender, RoutedEventArgs e)
    {
      HandleYesButtonInvocation();
      e.Handled = true;
    }

    private void HandleYesButtonInvocation()
    {
      try
      {
        _ablationSiteAware.AblationSite = _viewModel.SelectedAblationSite;
        _ablationSiteAware.UpdateAblationSiteChanged(_viewModel.SelectedAblationSite);

        DialogResult = false;
        Close();
      }
      catch(Exception ex)
      {
        LogException(ex);
      }
    }

    private void No_Click(object sender, RoutedEventArgs e)
    {
      HandleCancelButtonInvocation();
      e.Handled = true;
    }

    private void HandleCancelButtonInvocation()
    {
      _ablationSiteAware.AblationSite = _currentAblationSiteEnum;
      DialogResult = false;
      Close();
    }
  }
}