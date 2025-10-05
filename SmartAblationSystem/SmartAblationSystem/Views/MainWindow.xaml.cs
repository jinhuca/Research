using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SmartAblationSystem.Helpers;
using SmartAblationSystem.Models;
using SmartAblationSystem.ViewModels;

namespace SmartAblationSystem.Views
{
	public partial class MainWindow
	{
    private Visibility volumeControlsVisible = Visibility.Hidden;
    private System.Windows.Forms.Timer volumeTimer;
    private const int VOLUME_CONTROL_TIMER_MS = 3000;
    private MainWindowViewModel _viewModel;

    public MainWindow()
		{
			Loaded += OnLoaded;
			InitializeComponent();

      _viewModel = DataContext is MainWindowViewModel
        ? (MainWindowViewModel)DataContext
        : throw new ArgumentNullException(nameof(MainWindowViewModel));

      VolumeControl.Visibility = Visibility.Visible;
      VolumeDown.Visibility = Visibility.Collapsed;
      VolumeUp.Visibility = volumeControlsVisible;
      VolumeLevelCtrl.Visibility = volumeControlsVisible;

      volumeTimer = new System.Windows.Forms.Timer();
      volumeTimer.Tick += HideVolumeControlEvent;
      volumeTimer.Interval = VOLUME_CONTROL_TIMER_MS;
    }

    private void HideVolumeControlEvent(object sender, EventArgs e)
    {
      SetVolumeControlsVisibility(sender, e);
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
		{
			Activate();
		}

		private void MainWindowNewLook_OnLoaded(object sender, RoutedEventArgs e)
		{
			Task.Delay(10_000).ContinueWith(t => CommonViewModel.Current.ReadTheFirmwareVersions());
			Task.Delay(12_000).ContinueWith(t => InitActions());
		}

		private void InitActions()
		{
			CommonViewModel.Current.IsWindowLoaded = true;
			ConsolePowerAndState.ConsoleVersionReference = CommonViewModel.Current.CreateAConsoleVersion();
			ConsoleStateComparator consoleStateComparator_ = new ConsoleStateComparator();
			consoleStateComparator_.VerifyAndUpdateStatiqueDevices(ConsolePowerAndState.ConsoleVersionReference);
		}

    private void SetVolumeControlsVisibility(object sender, EventArgs e)
    {
      if(volumeControlsVisible == Visibility.Visible)
      {
        volumeTimer.Stop();
        volumeControlsVisible = Visibility.Hidden;
      }
      else
      {
        volumeTimer.Start();
        volumeControlsVisible = Visibility.Visible;
      }

      VolumeControl.Visibility = volumeControlsVisible == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
      VolumeDown.Visibility = volumeControlsVisible == Visibility.Visible ? Visibility.Visible : Visibility.Collapsed;
      VolumeUp.Visibility = volumeControlsVisible;
      VolumeLevelCtrl.Visibility = volumeControlsVisible;
      _viewModel.IsVolumeChangeInvisible = volumeControlsVisible != Visibility.Visible;
    }

    private void ResetVolumeControlTimer(object sender, RoutedEventArgs e)
    {
      volumeTimer.Stop();
      volumeTimer.Start();
    }

    private void PreviewTouchDownOnMainGrid(object sender, TouchEventArgs e)
    {
      var button_ = UIElementHelpers.FindElementByName<Button>(element: MainWindowName, childName: "btnBMIInfo");
      if (button_?.ToolTip is ToolTip tip_)
      {
        tip_.IsOpen = false;
      }
    }
  }
}
