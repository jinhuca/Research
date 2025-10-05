using System.Windows;

namespace CustomControls.UserControls
{
  /// <summary>
  /// Interaction logic for ChangeTankStepUserControl.xaml
  /// </summary>
  public partial class ChangeTankStepUserControl
  {
    #region DependencyProperty Definitions 
    public static readonly DependencyProperty StepDescriptionProperty = DependencyProperty.Register(nameof(StepDescription), typeof(string), 
      typeof(ChangeTankStepUserControl), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty StepIdProperty = DependencyProperty.Register(nameof(StepId), typeof(int), 
      typeof(ChangeTankStepUserControl), new PropertyMetadata(0));

    public static readonly DependencyProperty IsCompletedProperty = DependencyProperty.Register(nameof(IsCompleted), typeof(bool), 
      typeof(ChangeTankStepUserControl), new PropertyMetadata(false));

    public static readonly DependencyProperty IsCurrentStepProperty = DependencyProperty.Register(nameof(IsCurrentStep), typeof(bool), 
      typeof(ChangeTankStepUserControl), new PropertyMetadata(false));

    #endregion DependencyProperty Definitions
    public ChangeTankStepUserControl()
    {
      InitializeComponent();
    }

    #region properties
    public string StepDescription
    {
      get => (string)GetValue(StepDescriptionProperty); 
      set => SetValue(StepDescriptionProperty, value);
    }

    public int StepId
    {
      get => (int)GetValue(StepIdProperty);
      set => SetValue(StepIdProperty, value);
    }

    public bool IsCompleted
    {
      get => (bool)GetValue(IsCompletedProperty);
      set => SetValue(IsCompletedProperty, value);
    }

    public bool IsCurrentStep
    {
      get => (bool)GetValue(IsCurrentStepProperty);
      set => SetValue(IsCurrentStepProperty, value);
    }
    #endregion properties
  }
}
