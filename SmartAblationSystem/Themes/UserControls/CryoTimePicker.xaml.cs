using Prism.Commands;
using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Input;

namespace CustomControls.UserControls
{
  /// <summary>
  /// Interaction logic for CryoTimePicker.xaml
  /// </summary>
  public partial class CryoTimePicker : INotifyPropertyChanged 
  {
    public static readonly DependencyProperty SelectedTimeProperty = DependencyProperty.Register(nameof(SelectedTime),
      typeof(DateTime?), typeof(CryoTimePicker), new FrameworkPropertyMetadata(null,  FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedTimeChanged));

    public event PropertyChangedEventHandler PropertyChanged;

    private readonly ISubject<bool> _updateSelectedTimeSubject = new Subject<bool>();

    public CryoTimePicker()
    {
      InitializeComponent();

      GoToPreviousHour = new DelegateCommand(ExecuteGoPreviousHourCommand, () => true);
      GoToPreviousMinute = new DelegateCommand(ExecuteGoPreviousMinuteCommand, () => true);
      GoToNextHour = new DelegateCommand(ExecuteGoNextHourCommand, () => true);
      GoToNextMinute = new DelegateCommand(ExecuteGoNextMinuteCommand, () => true);

      _updateSelectedTimeSubject
        .Where(e => e)
        .ObserveOnDispatcher()
        .Subscribe( _ =>
            {
              var today = DateTime.Today;
              SelectedTime = new DateTime(today.Year, today.Month, today.Day, _currentHour, _currentMinute, 0);
            });
    }

    public ICommand GoToPreviousHour { get; } 
    public ICommand GoToPreviousMinute { get; } 

    public ICommand GoToNextHour { get; } 
    public ICommand GoToNextMinute { get; } 

    public DateTime? SelectedTime
    {
      get => (DateTime?)GetValue(SelectedTimeProperty);
      set => SetValue(SelectedTimeProperty, value);
    }

    private int _currentHour;
    public int CurrentHour
    {
      get => _currentHour;
      set 
      {
        if (_currentHour == value) return;
        _currentHour = value;
        RaisePropertyChanged(nameof(CurrentHour));
      }
    }

    private int _currentMinute;
    public int CurrentMinute
    {
      get => _currentMinute;
      set
      {
        if (value == _currentMinute) return;
        _currentMinute = value;
        RaisePropertyChanged(nameof(CurrentMinute));
      }
    }

    protected void RaisePropertyChanged(string propertyName)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void ExecuteGoPreviousHourCommand()
    {
      CurrentHour = (_currentHour -1) < 0 ? 23 : _currentHour - 1;
      _updateSelectedTimeSubject.OnNext(true);
    }

    private void ExecuteGoPreviousMinuteCommand()
    {
      CurrentMinute = (_currentMinute - 1) < 0 ? 59 : _currentMinute - 1;
      _updateSelectedTimeSubject.OnNext(true);
    }

    private void ExecuteGoNextHourCommand()
    {
      CurrentHour = (CurrentHour + 1) % 24;
      _updateSelectedTimeSubject.OnNext(true);
    }

    private void ExecuteGoNextMinuteCommand()
    {
      CurrentMinute = (CurrentMinute + 1) % 60;
      _updateSelectedTimeSubject.OnNext(true);
    }

    private static void OnSelectedTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var control = d as CryoTimePicker; 
      var newTime = (DateTime?)e.NewValue ?? DateTime.Now;
      if (control != null)
      {
        control.CurrentHour = newTime.Hour;
        control.CurrentMinute = newTime.Minute;
      }
    }
  }
}
