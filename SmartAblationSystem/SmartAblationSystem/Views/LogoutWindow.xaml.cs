using SmartAblationSystem.ViewModels;
using System.Threading.Tasks;
using System.Windows;

namespace SmartAblationSystem.Views
{
  public partial class LogoutWindow
  {
    public LogoutWindow()
    {
      InitializeComponent();
    }

    private void CancelClick(object sender, RoutedEventArgs e)
    {
      DialogResult = false;
      Close();
    }

    private void ConfirmClick(object sender, RoutedEventArgs e)
    {
      DialogResult = true;
      Close();
      // should set IsLoggedIn to false!

      if(CommonViewModel.Current.IsCanOneWasInError)
      {
        CommonViewModel.Current.IsCanOneReseted = true;
#if !DEBUG
                // Reset CAN1
                CommonViewModel.Current.ResetCanOneStopWatch();
#endif
        // Prevents CAN1 Error Message to start stacking the same error message
        if(CommonViewModel.Current.ErrorIdMessageAndSolutionList != null && CommonViewModel.Current.ErrorIdMessageAndSolutionList.Count != 0)
        {
          CommonViewModel.Current.ErrorIdMessageAndSolutionList.Clear();
        }

        Task.Delay(3000).ContinueWith(t => CommonViewModel.Current.IsCanOneWasInError = false);
      }

      if(!CommonViewModel.Current.IsCanOneWasInError && CommonViewModel.Current.IsCanOneReseted)
      {
        CommonViewModel.Current.IsCanOneReseted = false;
      }

      /** CAN2 **/
      if(CommonViewModel.Current.IsCanTwoWasInError)
      {
        if(CommonViewModel.Current.ErrorIdMessageAndSolutionList != null && CommonViewModel.Current.ErrorIdMessageAndSolutionList.Count != 0)
        {
          // Prevents CAN2 Error Message to start stacking the same error message
          CommonViewModel.Current.ErrorIdMessageAndSolutionList.Clear();
        }

        Task.Delay(3000).ContinueWith(t => CommonViewModel.Current.IsCanTwoWasInError = false);
      }

    }
  }
}
