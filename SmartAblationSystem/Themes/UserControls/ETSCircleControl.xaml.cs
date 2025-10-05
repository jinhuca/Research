using System.Runtime.CompilerServices;
using System.Windows;
using CustomControls.PubSubEvents;
using Prism.Events;
using Prism.Ioc;
using Prism.Unity;

namespace CustomControls.UserControls
{
  /// <summary>
  /// Interaction logic for ETSCircleControl.xaml
  /// </summary>
  public partial class ETSCircleControl  
  {
    private static readonly string _flashStoryboardSyncGroupName = "AlertGroup";

    public static readonly DependencyProperty SettingValueProperty = DependencyProperty.Register("SettingValue",
      typeof(string), typeof(ETSCircleControl),
      new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value",
      typeof(string), typeof(ETSCircleControl),
      new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty IsAlertProperty = DependencyProperty.Register("IsAlert",
      typeof(bool), typeof(ETSCircleControl),
      new PropertyMetadata(false, OnIsAlertChanged));

    public static readonly DependencyProperty UnitProperty = DependencyProperty.Register("Unit",
      typeof(string), typeof(ETSCircleControl),
      new PropertyMetadata(string.Empty));

    public ETSCircleControl()
    {
      this.InitializeComponent();
    }

    public string SettingValue
    {
      get => (string)this.GetValue(SettingValueProperty);
      set => this.SetValue(SettingValueProperty, value);
    }

    public string Value
    {
      get => (string)this.GetValue(ValueProperty);
      set => this.SetValue(ValueProperty, value);
    }

    public string Unit
    {
      get => (string)this.GetValue(UnitProperty);
      set => this.SetValue(UnitProperty, value);
    }

    public bool IsAlert
    {
      get => (bool)GetValue(IsAlertProperty);
      set => SetValue(IsAlertProperty, value);
    }

    private static void OnIsAlertChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
    {
      var isAlert = (bool)e.NewValue;
      var isControlVisible = (dp as ETSCircleControl)?.IsVisible ?? false; 
      if (isAlert && isControlVisible)
      {
        var container = (Application.Current as PrismApplication)?.Container;
        var eventAggregator = container?.Resolve<IEventAggregator>();
        eventAggregator?.GetEvent<StoryboardSyncEvent>().Publish(_flashStoryboardSyncGroupName);
      }
    }
  }
}
