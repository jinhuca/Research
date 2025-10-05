using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Prism.Mvvm;
using System.Runtime.InteropServices;
using System.Windows.Input;
using Prism.Commands;
using SmartAblationSystem.Helpers;

namespace SmartAblationSystem.ViewModels
{
  /// <summary>
  /// This class is the time and date View Model
  /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
  /// </summary>
  public class TimeAndDateViewModel : BindableBase
  {
    [StructLayout(LayoutKind.Sequential)]
    public struct SYSTEMTIME
    {
      public short wYear;
      public short wMonth;
      public short wDayOfWeek;
      public short wDay;
      public short wHour;
      public short wMinute;
      public short wSecond;
      public short wMilliseconds;
    }


    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool SetSystemTime([In] ref SYSTEMTIME st);

    public ICommand SetDateCommand { get; private set; }
    public ICommand ResetDateCommand { get; private set; }
    public ICommand SetTimeCommand { get; private set; }
    public ICommand ResetTimeCommand { get; private set; }
    
    public ICommand ReturnToSettingsCommand { get; private set; }

    private readonly CommonViewModel _localCommonViewModel;
    private readonly Subject<bool> _setUsingDaylightSavingSubject = new Subject<bool>();
    private bool _lastUsingDaylightSavingStatus; 

    public TimeAndDateViewModel(CommonViewModel commonViewModel)
    {
      _localCommonViewModel = commonViewModel;

      SetDateCommand = new DelegateCommand(OnSetDateCommand, () => true);
      ResetDateCommand = new DelegateCommand(OnResetDateCommand, () => true);
      SetTimeCommand = new DelegateCommand(OnSetTimeCommand, () => true);
      ResetTimeCommand = new DelegateCommand(OnResetTimeCommand, () => true);

      this.ReturnToSettingsCommand = new DelegateCommand(this.OnReturnToSettingsCommand, () => true);

      HospitalName = _localCommonViewModel.Data.DataAccess.GetHospitalName();
      _lastUsingDaylightSavingStatus = _localCommonViewModel.IsUsingDaylightSavingTime; 
      _setUsingDaylightSavingSubject
        .Throttle(TimeSpan.FromMilliseconds(500))
        .Subscribe(UpdateUsingDaylightSaving);
    }

    public void RefreshDisplay()
    {
      LastModifyDate = DateTime.Today;
      LastModifyTime = DateTime.Now;
      CurrentTimeZoneInfo = TimeZoneInfo.Local;
      HospitalName = _localCommonViewModel.Data.DataAccess.GetHospitalName();

      RaisePropertyChanged(nameof(IsUserAllowedToModifyDateTime));
      RaisePropertyChanged(nameof(IsUsingDaylightSavingTime));
    }

    private string _hospitalName = string.Empty;

    public string HospitalName
    {
      get => _hospitalName;
      set => SetProperty(ref _hospitalName, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the Daylight Saving feature is activated or not.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool IsUsingDaylightSavingTime
    {
      get
      {
        return _localCommonViewModel.IsUsingDaylightSavingTime;
      }

      set
      {
        if (value != _localCommonViewModel.IsUsingDaylightSavingTime)
        {
          _localCommonViewModel.IsUsingDaylightSavingTime = value;
          RaisePropertyChanged(nameof(IsUsingDaylightSavingTime));
          _setUsingDaylightSavingSubject.OnNext(value);
        }
      }
    }

    public ICollection<TimeZoneInfo> TimeZoneInfoCollection => TimeZoneInfo.GetSystemTimeZones();

    private TimeZoneInfo _currentTimeZoneInfo = TimeZoneInfo.Local;

    public TimeZoneInfo CurrentTimeZoneInfo
    {
      get => _currentTimeZoneInfo;
      set
      {
        if (!_currentTimeZoneInfo.Equals(value))
        {
          SetProperty(ref _currentTimeZoneInfo, value);
          UpdateTimeZone(_currentTimeZoneInfo);
        }
      }
    }

    public bool IsUserAllowedToModifyDateTime
    {
      get => _localCommonViewModel.IsBSCADMINUser || _localCommonViewModel.IsCryterionUser;
    }

    private DateTime _lastModifyDate = DateTime.Today;

    public DateTime LastModifyDate
    {
      get => _lastModifyDate;
      set => SetProperty(ref _lastModifyDate, value);
    }

    private DateTime _lastModifyTime = DateTime.Now;

    public DateTime LastModifyTime
    {
      get => _lastModifyTime;
      set => SetProperty(ref _lastModifyTime, value);
    }

    /// <summary>
    /// Function/Command that handles the change date command
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private void OnSetDateCommand()
    {
      var now = DateTime.Now;
      var newDate = new DateTime(LastModifyDate.Year, LastModifyDate.Month, LastModifyDate.Day, now.Hour, now.Minute,
        now.Second, now.Millisecond);
      var utcDateTime = TimeZoneInfo.ConvertTimeToUtc(newDate, TimeZoneInfo.Local);
      
      UpdateDateTime(utcDateTime);
    }

    private void OnResetDateCommand()
    {
      LastModifyDate = DateTime.Today;
    }

    /// <summary>
    /// Function/Command that handles the change time command
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private void OnSetTimeCommand()
    {
      var today = DateTime.Today;
      var newDateTime = new DateTime(today.Year, today.Month, today.Day, LastModifyTime.Hour, LastModifyTime.Minute, 0, 0);
      var utcDateTime = TimeZoneInfo.ConvertTimeToUtc(newDateTime, TimeZoneInfo.Local);
      
      UpdateDateTime(utcDateTime);
    }

    private void UpdateDateTime(DateTime utcDateTime)
    {
      SYSTEMTIME st = new SYSTEMTIME(); 

      st.wYear = (short)utcDateTime.Year;
      st.wMonth = (short)utcDateTime.Month;
      st.wDay = (short)utcDateTime.Day;

      st.wHour = (short)utcDateTime.Hour;
      st.wMinute = (short)utcDateTime.Minute;
      st.wSecond = (short)utcDateTime.Second;
      st.wMilliseconds = (short)utcDateTime.Millisecond;

      SetSystemTime(ref st);

    }

    private void OnResetTimeCommand()
    {
      LastModifyTime = DateTime.Now;
    }

    /// <summary>
    /// Function/Command that handles the Return to Settings when the Return to Settings view
    /// command is invoked
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="obj">The command's parameter (not used in this function).</param>
    private void OnReturnToSettingsCommand()
    {
      ViewsEventArgs viewsEvent = new ViewsEventArgs();
      viewsEvent.ViewName = "BackToSettings";
      CommonViewModel.Current.OnViewchanged(viewsEvent);
    }

    private void UpdateUsingDaylightSaving(bool isUsingDaylightSaving)
    {

      if (_lastUsingDaylightSavingStatus == isUsingDaylightSaving) 
        return;

      _lastUsingDaylightSavingStatus = isUsingDaylightSaving;
      // Update setting in DB
      _localCommonViewModel?.Data?.DataAccess?.SetLowIsUsingDaylightSavingTimeFlag(isUsingDaylightSaving);
      
      // Update Current DateTime with using daylight saving flag 
      var changeHours = isUsingDaylightSaving ? 1 : -1;
      var newDateTime = DateTime.Now.AddHours(changeHours); 

      var utcDateTime = TimeZoneInfo.ConvertTimeToUtc(newDateTime, TimeZoneInfo.Local);;
      UpdateDateTime(utcDateTime);

      RefreshDisplay();
    }

    private void UpdateTimeZone(TimeZoneInfo timeZoneInfo)
    {
      AdjustTokenPrivilegesFunctionality.EnableSetTimeZonePrivileges();

      TimeZoneManager.SetTimeZone(timeZoneInfo);
      RefreshDisplay();
    }
  }
}
