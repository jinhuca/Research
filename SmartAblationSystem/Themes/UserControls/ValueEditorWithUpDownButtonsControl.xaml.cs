
namespace CustomControls.UserControls
{
  using System;
  using System.Reactive.Disposables;
  using System.Reactive.Linq;
  using System.Reactive.Subjects;
  using System.Threading.Tasks;
  using System.Windows;
  using System.Windows.Input;

  using Prism.Commands;

  /// <summary>
  /// Interaction logic for ValueEditorWithUpDownButtonsControl.xaml
  /// </summary>
  public partial class ValueEditorWithUpDownButtonsControl
  {
    #region DependencyProperties Register
    public static readonly DependencyProperty IsEditableProperty = DependencyProperty.Register("IsEditable",
      typeof(bool), typeof(ValueEditorWithUpDownButtonsControl),
      new PropertyMetadata());

    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value",
      typeof(double), typeof(ValueEditorWithUpDownButtonsControl),
      new PropertyMetadata());

    public static readonly DependencyProperty UnitProperty = DependencyProperty.Register("Unit",
      typeof(string), typeof(ValueEditorWithUpDownButtonsControl),
      new PropertyMetadata());

    public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register("MinValue",
      typeof(double), typeof(ValueEditorWithUpDownButtonsControl),
      new PropertyMetadata(0d));

    public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue",
      typeof(double), typeof(ValueEditorWithUpDownButtonsControl),
      new PropertyMetadata(100d));

    public static readonly DependencyProperty IncrementalProperty = DependencyProperty.Register("Incremental",
      typeof(double), typeof(ValueEditorWithUpDownButtonsControl),
      new PropertyMetadata(10d));

    public static readonly DependencyProperty AutoExitEditProperty = DependencyProperty.Register("AutoExitEdit",
      typeof(bool), typeof(ValueEditorWithUpDownButtonsControl),
      new PropertyMetadata(true));

    #endregion DependencyProperties Register

    private readonly ISubject<bool> _editableObserver = new Subject<bool>();
    private readonly SerialDisposable _editableDisposible = new SerialDisposable();

    public ValueEditorWithUpDownButtonsControl()
    {
      this.InitializeComponent();
      this.Focusable = true;
      this.IncrementalCommand = new DelegateCommand(ExecuteIncrementValue, () => true);
      this.DecrementalCommand = new DelegateCommand(ExecuteDecrementValue, () => true);
    }

    public ICommand IncrementalCommand { get; }
    public ICommand DecrementalCommand { get; }

    #region DependencyProperty Definitions
    public bool IsEditable
    {
      get => (bool)this.GetValue(IsEditableProperty);
      set
      {
        this.SetValue(IsEditableProperty, value);
      }
    }

    public double Value
    {
      get => (double)this.GetValue(ValueProperty);
      set => this.SetValue(ValueProperty, value);
    }

    public string Unit
    {
      get => (string)this.GetValue(UnitProperty);
      set => this.SetValue(UnitProperty, value);
    }

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

    public double Incremental
    {
      get => (double)this.GetValue(IncrementalProperty);
      set => this.SetValue(IncrementalProperty, value);
    }

    public bool AutoExitEdit
    {
      get => (bool)this.GetValue(AutoExitEditProperty);
      set => this.SetValue(AutoExitEditProperty, value);
    }

    #endregion DependencyProperty Definitions

    private void ExecuteIncrementValue()
    {
      this._editableObserver.OnNext(false);
      if (this.Value >= this.MaxValue) return;

      this.Value = Math.Min(this.Value + this.Incremental, this.MaxValue);
    }

    private void ExecuteDecrementValue()
    {
      this._editableObserver.OnNext(false);
      if (this.Value <= this.MinValue) return;

      this.Value = Math.Max(this.Value - this.Incremental, this.MinValue);
    }

    private void ShowPopup()
    {
      IsEditable = true;

      if (_editableDisposible.Disposable == null)
      {
        _editableDisposible.Disposable = _editableObserver.Throttle(TimeSpan.FromSeconds(3)).Subscribe(
          _ => this.CloseEditPopup());
      }
      _editableObserver.OnNext(true);
    }

    private void CloseEditPopup()
    {
      this.Dispatcher.Invoke(
        () =>
          {
            if (AutoExitEdit)
            {
              IsEditable = false;
              _editableDisposible.Disposable?.Dispose();
              _editableDisposible.Disposable = null;
              Focus();
            }
          });
    }

    private void Popup_OnClosed(object sender, EventArgs e)
    {
      this.CloseEditPopup();
    }

    private void DelayShowPopup()
    {
      Task.Delay(TimeSpan.FromMilliseconds(10)).ContinueWith(_ => Dispatcher.Invoke(() => ShowPopup()));
    }

    private void _valueTextBox_OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
      this.DelayShowPopup();
    }
  }
}
