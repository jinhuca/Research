
using System.Reactive.Subjects;

namespace CustomControls.UserControls
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.Linq;
  using System.Reactive.Linq;
  using System.Windows;
  using System.Windows.Input;

  using Prism.Commands;

  /// <summary>
  /// Interaction logic for CryoCalendar.xaml
  /// </summary>
  public partial class CryoCalendar : INotifyPropertyChanged
  {
    private static readonly IDictionary<int, string> _monthIntToStringDict = new Dictionary<int, string>()
                              {
                                { 1, "Jan" }, { 2, "Feb" }, { 3, "Mar" }, { 4, "Apr" }, { 5, "May" }, { 6, "June" },
                                { 7, "July" }, { 8, "Aug" }, { 9, "Sept" }, { 10, "Oct" }, { 11, "Nov" }, { 12, "Dec" }
                              };

    private static readonly int _oldestYear = 1900;
    private static readonly int _newestYear = DateTime.Now.Year;

    private static readonly DateTime _today = DateTime.Now;
    
    public event PropertyChangedEventHandler PropertyChanged;

    private readonly ISubject<bool> _updateSelectedDateSubject = new Subject<bool>();

    public static readonly DependencyProperty SelectedDateProperty = DependencyProperty.Register(nameof(SelectedDate),
      typeof(DateTime?), typeof(CryoCalendar),
      new FrameworkPropertyMetadata(null,
        FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedDateChanged));

    public ICommand GoToPreviousMonth { get; } 
    public ICommand GoToPreviousDay { get; } 
    public ICommand GoToPreviousYear { get; } 

    public ICommand GoToNextMonth { get; } 
    public ICommand GoToNextDay { get; } 
    public ICommand GoToNextYear { get; } 

    public DateTime? DefaultDisplayDate { get; set; }

    public CryoCalendar()
    {
      InitializeComponent();

      GoToPreviousMonth = new DelegateCommand(ExecuteGoPreviousMonth, ()=>true);  
      GoToPreviousDay  = new DelegateCommand(ExecuteGoPreviousDay, ()=>true);  
      GoToPreviousYear  = new DelegateCommand(ExecuteGoPreviousYear).ObservesCanExecute(()=>CanNavigateYearDown);  

      GoToNextMonth = new DelegateCommand(ExecuteGoNextMonth, ()=>true);  
      GoToNextDay = new DelegateCommand(ExecuteGoNextDay, ()=>true);  
      GoToNextYear = new DelegateCommand(ExecuteGoNextYear).ObservesCanExecute(()=>CanNavigateYearUp);

      IsVisibleChanged += (s, e) =>
      {
        if ((bool)e.NewValue)
        {
          if (!SelectedDate.HasValue && DefaultDisplayDate.HasValue)
          {
            var date = DefaultDisplayDate.Value;
            UpdateMonthsDisplay(date.Month, false);
            UpdateDaysDisplay(date.Day, false);
            UpdateYearsDisplay(date.Year, false);
          }
        }
      };

      _updateSelectedDateSubject
        .Where(e=> e)
        .ObserveOnDispatcher()
        .Subscribe(
          _ =>
            {
              SelectedDate = new DateTime(this._selectedYear, this._selectedMonth, this._selectedDay);
            });
    }

    protected void RaisePropertyChanged(string propertyName)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public DateTime? SelectedDate
    {
      get => (DateTime?) this.GetValue(SelectedDateProperty);
      set => this.SetValue(SelectedDateProperty, (object) value);
    }

    private static void OnSelectedDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var isDateHasValue = (e.NewValue as DateTime?).HasValue;
      var control = d as CryoCalendar;
      var date = e.NewValue as DateTime? ?? control?.DefaultDisplayDate ?? DateTime.Today;

      if (!isDateHasValue && control != null)
      {
        control.SelectedDate = null;
      }

      control?.UpdateMonthsDisplay(date.Month, isDateHasValue);
      control?.UpdateDaysDisplay(date.Day, isDateHasValue);
      control?.UpdateYearsDisplay(date.Year, isDateHasValue);
    }

    private int _selectedDay=_today.Day;

    public int SelectedDay
    {
      get => _selectedDay;
      set
      {
        if (value != _selectedDay)
        {
          this._selectedDay = value;
          RaisePropertyChanged(nameof(SelectedDay));
        }
      }
    }

    private bool _ignoreYearConstrain = false;
    public bool IgnoreYearConstrain
    {
      get => _ignoreYearConstrain;  
      set
      {
        _ignoreYearConstrain = value;
        RaisePropertyChanged(nameof(IgnoreYearConstrain));
      }
    } 

    private bool _showSelectDateCaption = true;
    public bool ShowSelectDateCaption
    {
      get => _showSelectDateCaption; 
      set
      {
        _showSelectDateCaption = value;
        RaisePropertyChanged(nameof(ShowSelectDateCaption));
      }
    } 

    private int _selectedMonth = _today.Month;

    public string SelectedMonth
    {
      get => _monthIntToStringDict[_selectedMonth];
      set
      {
        this._selectedMonth = _monthIntToStringDict.FirstOrDefault(m => m.Value == value).Key;
        RaisePropertyChanged(nameof(SelectedMonth));
      }
    }

    private int _selectedYear=_today.Year;

    public int SelectedYear
    {
      get => _selectedYear;
      set
      {
        if (value != _selectedYear)
        {
          this._selectedYear = value;
          RaisePropertyChanged(nameof(SelectedYear));
        }
      }
    }

    private int _previousDay = _today.Day - 1;

    public int PreviousDay
    {
      get => _previousDay;
      set
      {
        if (value != _previousDay)
        {
          this._previousDay = value;
          RaisePropertyChanged(nameof(PreviousDay));
        }
      }
    }

    private int _previousMonth = _today.Month - 1;

    public string PreviousMonth
    {
      get => _monthIntToStringDict[_previousMonth];
      set
      {
          this._previousMonth = _monthIntToStringDict.FirstOrDefault(m => m.Value == value).Key;
          RaisePropertyChanged(nameof(PreviousMonth));
      }
    }

    private int _previousYear = _today.Year - 1;

    public int PreviousYear
    {
      get => _previousYear;
      set
      {
        if (value != _previousYear)
        {
          this._previousYear = value;
          RaisePropertyChanged(nameof(PreviousYear));
        }
      }
    }

    private int _nextDay = _today.Day + 1;

    public int NextDay
    {
      get => _nextDay;
      set
      {
        if (value != _nextDay)
        {
          this._nextDay = value;
          RaisePropertyChanged(nameof(NextDay));
        }
      }
    }

    private int _nextMonth = _today.Month + 1; //_monthIntToStringDict[7];

    public string NextMonth
    {
      get => _monthIntToStringDict[_nextMonth];
      set
      {
        this._nextMonth = _monthIntToStringDict.FirstOrDefault(m => m.Value == value).Key;
        RaisePropertyChanged(nameof(NextMonth));
      }
    }

    private int _nextYear = _today.Year + 1;

    public int NextYear
    {
      get => _nextYear;
      set
      {
        if (value != _nextYear)
        {
          this._nextYear = value;
          RaisePropertyChanged(nameof(NextYear));
        }
      }
    }

    private void UpdateMonthsDisplay(int selectedMonth, bool raisePropertyChanged = true)
    {
      this._selectedMonth = selectedMonth;
      this._previousMonth = selectedMonth - 1 == 0 ? 12 : selectedMonth - 1;
      this._nextMonth = selectedMonth + 1 > 12 ? 1 : selectedMonth + 1;

      RaisePropertyChanged(nameof(PreviousMonth));
      RaisePropertyChanged(nameof(SelectedMonth));
      RaisePropertyChanged(nameof(NextMonth));

      _updateSelectedDateSubject.OnNext(raisePropertyChanged);
    }

    private void UpdateDaysDisplay(int selectedDay, bool raisePropertyChanged = true)
    {
      this._selectedDay = selectedDay;
      this._previousDay = selectedDay - 1 == 0 ? 31 : selectedDay - 1;
      this._nextDay = selectedDay + 1 > 31 ? 1 : selectedDay + 1;

      RaisePropertyChanged(nameof(PreviousDay));
      RaisePropertyChanged(nameof(SelectedDay));
      RaisePropertyChanged(nameof(NextDay));
      _updateSelectedDateSubject.OnNext(raisePropertyChanged);
    }

    private bool _canNavigateYearUp = true;

    public bool CanNavigateYearUp
    {
      get => this._canNavigateYearUp;
      set
      {
        this._canNavigateYearUp = value;
        RaisePropertyChanged(nameof(CanNavigateYearUp));
      }
    }

    private bool _canNavigateYearDown = true;

    public bool CanNavigateYearDown
    {
      get => this._canNavigateYearDown;
      set
      {
        if (value == this._canNavigateYearDown) return;
        this._canNavigateYearDown = value;
        RaisePropertyChanged(nameof(CanNavigateYearDown));
      }
    }

    private void UpdateYearsDisplay(int selectedYear, bool raisePropertyChanged = true)
    {
      this._selectedYear = selectedYear;
      this._previousYear = selectedYear - 1;
      this._nextYear = selectedYear + 1;

      CanNavigateYearDown = IgnoreYearConstrain || PreviousYear >= _oldestYear;
      CanNavigateYearUp = IgnoreYearConstrain || SelectedYear < _newestYear;

      RaisePropertyChanged(nameof(PreviousYear));
      RaisePropertyChanged(nameof(SelectedYear));
      RaisePropertyChanged(nameof(NextYear));
      _updateSelectedDateSubject.OnNext(raisePropertyChanged);
    }

    private void ExecuteGoPreviousMonth()
    {
      this.GetAndUpdateValidDayDown(this.SelectedYear, this._previousMonth, this._selectedDay);
      this.UpdateMonthsDisplay(this._previousMonth);
    }

    private void ExecuteGoPreviousDay()
    {
      var validDay = this.GetAndUpdateValidDayDown(this.SelectedYear, this._selectedMonth, this._previousDay);
      this.UpdateDaysDisplay(validDay);
    }

    private void ExecuteGoPreviousYear()
    {
      this.GetAndUpdateValidDayDown(this._previousYear, this._selectedMonth, this._selectedDay);
      this.UpdateYearsDisplay(this._previousYear);
    }

    private void ExecuteGoNextMonth()
    {
      this.GetAndUpdateValidDayDown(this.SelectedYear, this._nextMonth, this._selectedDay);
      this.UpdateMonthsDisplay(this._nextMonth);
    }

    private void ExecuteGoNextDay()
    {
      var nextDay = _nextDay;
      while (!IsSelectedDateValid(this._selectedYear, this._selectedMonth, nextDay))
      {
        nextDay = ++nextDay > 31 ? 1 : nextDay;
      }

      this.UpdateDaysDisplay(nextDay);
    }

    private void ExecuteGoNextYear()
    {
      if (!IgnoreYearConstrain && this._selectedYear >= _newestYear) return;

      this.GetAndUpdateValidDayDown(this._nextYear, this._selectedMonth, this._selectedDay);
      this.UpdateYearsDisplay(this._nextYear);
    }

    private int GetAndUpdateValidDayDown(int year, int month, int day)
    {
      while (!IsSelectedDateValid(year, month, day))
      {
        day = --day == 0 ? 31 : day;
      }

      SelectedDay = day;
      return day;
    }

    private bool IsSelectedDateValid(int year, int month, int day)
    {
      if (!IgnoreYearConstrain && (year > _newestYear || year < _oldestYear))
        return false;

      if (month < 1 || month > 12)
        return false;

      return day > 0 && day <= DateTime.DaysInMonth(year, month);
    }

    private void OnCurrentDateMouseTouchDown(object sender, InputEventArgs e)
    {
      _updateSelectedDateSubject.OnNext(true);
    }
  }
}
