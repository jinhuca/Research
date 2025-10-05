using System.Windows.Input;

namespace SmartAblationSystem.Views
{
  using System.Windows;

  using SmartAblationSystem.ViewModels;

  /// <summary>
  /// Interaction logic for Patient.xaml
  /// </summary>
  public partial class Patient 
  {
    public Patient()
    {
      InitializeComponent();
    }

    private void Patient_OnLoaded(object sender, RoutedEventArgs e)
    {
      var viewModel = DataContext as PatientViewModel;
      viewModel?.ResetPatientInfo();
    }

    private void Patient_OnMouseOrTouchDown(object sender, InputEventArgs e)
    {
      var keyboardFocusedElement = Keyboard.FocusedElement;
      if (txtPatentID.Equals(keyboardFocusedElement)
          || TxtWeight.Equals(keyboardFocusedElement)
          || TxtHeight.Equals(keyboardFocusedElement))
      {
        this.Focus();
      }
    }

    private void WeightHeightLostFocus(object sender, RoutedEventArgs e)
    {
      var viewModel = DataContext as PatientViewModel;
      if (TxtWeight.Equals(sender) && string.IsNullOrEmpty(TxtWeight.Text) && viewModel != null)
      {
        viewModel.Weight = "0";
      } 
      else if (TxtHeight.Equals(sender) && string.IsNullOrEmpty(TxtHeight.Text) && viewModel != null)
      {
        viewModel.Height = "0";
      }
    }
  }
}
