using SmartAblationSystem.ViewModels;
using System;
using System.Windows;
using Shared;
using static LogSystem.LogService;

namespace SmartAblationSystem.Views
{
  public partial class AblationSiteWindow
  {
    private readonly AblationSiteEnum _currentAblationSiteEnum;
    private readonly IAblationSiteAware _ablationSiteAware;
    private readonly AblationSiteWindowViewModel _viewModel;

    public AblationSiteWindow(IAblationSiteAware ablationSiteAware)
    {
      InitializeComponent();
      _ablationSiteAware = ablationSiteAware;
      _viewModel = DataContext as AblationSiteWindowViewModel;

      if (_viewModel != null)
      {
        _viewModel.SelectedAblationSite = _ablationSiteAware.AblationSite;
        _currentAblationSiteEnum = _ablationSiteAware.AblationSite;

        //display warning when in playback mode
        _ablationSiteAware.AblationSite = AblationSiteEnum.UNKNOWN;
        _viewModel.DisplayAblationSiteWarning = _ablationSiteAware.DisplayAblationSiteWarning;
      }
    }

    private void Ok_Click(object sender, RoutedEventArgs e)
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
      _ablationSiteAware.AblationSite = _currentAblationSiteEnum;
      DialogResult = false;
      Close();
    }
  }
}