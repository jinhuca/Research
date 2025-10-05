
namespace CustomControls.UserControls
{
  using System;
  using System.Windows;
  using System.Windows.Input;

  using Prism.Commands;

  /// <summary>
  /// Interaction logic for SettingBarWithButtonsControl.xaml
  /// </summary>
  public partial class SettingBarWithButtonsControl
  {
    public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register("MinValue",
      typeof(double), typeof(SettingBarWithButtonsControl),
      new PropertyMetadata(0d));

    public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue",
      typeof(double), typeof(SettingBarWithButtonsControl),
      new PropertyMetadata(100d));

    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", 
      typeof(double), typeof(SettingBarWithButtonsControl),
      new PropertyMetadata(0d));

    public static readonly DependencyProperty IncrementalProperty = DependencyProperty.Register("Incremental", 
      typeof(double), typeof(SettingBarWithButtonsControl),
      new PropertyMetadata(10d));

    public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register("Description",
      typeof(string), typeof(SettingBarWithButtonsControl),
      new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty UnitProperty = DependencyProperty.Register("Unit",
      typeof(string), typeof(SettingBarWithButtonsControl),
      new PropertyMetadata(string.Empty));

    public SettingBarWithButtonsControl()
    {
      this.InitializeComponent();

      this.IncrementalCommand = new DelegateCommand(ExecuteIncrementValue, () => true);
      this.DecrementalCommand = new DelegateCommand(ExecuteDecrementValue, () => true);
    }

    public ICommand IncrementalCommand { get; }

    public ICommand DecrementalCommand { get; }

    public double MinValue
    {
      get => (double)this.GetValue(MinValueProperty);
      set => this.SetValue(MinValueProperty, value);
    }

    public double MaxValue
    {
      get => (double)this.GetValue(MaxValueProperty);
      set => this.SetValue(MaxValueProperty, value);
    }

    public double Value
    {
      get => (double)this.GetValue(ValueProperty); 
      set => this.SetValue(ValueProperty, value);
    }

    public double Incremental
    {
      get => (double)this.GetValue(IncrementalProperty); 
      set => this.SetValue(IncrementalProperty, value);
    }

    public string Description
    {
      get => (string)this.GetValue(DescriptionProperty);
      set => this.SetValue(DescriptionProperty, value);
    }

    public string Unit
    {
      get => (String)this.GetValue(UnitProperty);
      set => this.SetValue(UnitProperty, value);
    }

    private void ExecuteIncrementValue()
    {
      if (this.Value >= this.MaxValue) return; 

      this.Value = Math.Min(this.Value + this.Incremental, this.MaxValue);
    }

    private void ExecuteDecrementValue()
    {
      if (this.Value <= this.MinValue) return; 

      this.Value = Math.Max(this.Value - this.Incremental, this.MinValue);
    }
  }
}
