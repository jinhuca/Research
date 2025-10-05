using System;
using CustomControls.UserControls;
using MahApps.Metro.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace SmartAblationSystem.Helpers
{
  public static class DatePickerExtensions
  {
    public static readonly DependencyProperty DisabledDatesStartProperty = DependencyProperty.RegisterAttached(
      "DisabledDatesStart", typeof(CalendarDateRange), typeof(DatePickerExtensions), new PropertyMetadata(new CalendarDateRange(), OnDisabledDatesStartChanged));

    public static CalendarDateRange GetDisabledDatesStart(DependencyObject obj)
    {
      return (CalendarDateRange)obj.GetValue(DisabledDatesStartProperty);
    }

    public static void SetDisabledDatesStart(DependencyObject obj, CalendarDateRange value)
    {
      obj.SetValue(DisabledDatesStartProperty, value);
    }

    private static void OnDisabledDatesStartChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      if (obj is DatePicker datePicker)
      {
        if (args.NewValue is CalendarDateRange range)
        {
          datePicker.BlackoutDates.Clear();
          datePicker.BlackoutDates.Add(range);
        }
      }
    }

    public static readonly DependencyProperty DisabledDatesEnd = DependencyProperty.RegisterAttached(
      "DisabledDatesEnd", typeof(CalendarDateRange), typeof(DatePickerExtensions), new PropertyMetadata(new CalendarDateRange(), OnDisabledDatesEndChanged));

    public static CalendarDateRange GetDisabledDatesEnd(DependencyObject obj)
    {
      return (CalendarDateRange)obj.GetValue(DisabledDatesEnd);
    }

    public static void SetDisabledDatesEnd(DependencyObject obj, CalendarDateRange value)
    {
      obj.SetValue(DisabledDatesEnd, value);
    }

    private static void OnDisabledDatesEndChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      if (obj is DatePicker datePicker)
      {
        if (args.NewValue is CalendarDateRange range)
        {
          datePicker.BlackoutDates.Clear();
          datePicker.BlackoutDates.Add(range);
        }
      }
    }

    public static readonly DependencyProperty DefaultDisplayDateProperty = DependencyProperty.RegisterAttached(
      "DefaultDisplayDate", typeof(DateTime?), typeof(DatePickerExtensions),
      new PropertyMetadata(null, OnDefaultSelectedDateChanged));

    public static DateTime? GetDefaultDisplayDate(DependencyObject obj)
    {
      return (DateTime?)obj.GetValue(DefaultDisplayDateProperty);
    }

    public static void SetDefaultDisplayDate(DependencyObject obj, DateTime? value)
    {
      obj.SetValue(DefaultDisplayDateProperty, value);
    }

    private static void OnDefaultSelectedDateChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      if (obj is DatePicker datePicker)
      {
        var popup = datePicker.FindChild<Popup>("PART_Popup");
        if (popup == null)
          datePicker.Loaded += (_, __) => UpdateCalenderDefaultDisplayDate(datePicker, popup, args.NewValue as DateTime?);
        else
          UpdateCalenderDefaultDisplayDate(datePicker, popup, args.NewValue as DateTime?);
      }
    }

    private static void UpdateCalenderDefaultDisplayDate(DatePicker datePicker, Popup popup, DateTime? newDate)
    {
      popup = popup ?? datePicker.FindChild<Popup>("PART_Popup");
      if (popup == null) return;

      var calendar = (Calendar)popup.Child;
      if (calendar != null)
      {
        var cryoCalendar = FindCryoCalendarControl(datePicker, calendar);
        if (cryoCalendar != null)
          cryoCalendar.DefaultDisplayDate = newDate;
        else
        {
          if (newDate.HasValue) calendar.DisplayDate = newDate.Value;
        }
      }
    }

    private static CryoCalendar FindCryoCalendarControl(DatePicker datePicker, Calendar calendar)
    {
      var cryoCalendar = calendar.FindChild<CryoCalendar>("PART_CryoCalendar");
      if (!datePicker.IsDropDownOpen && cryoCalendar == null)
      {
        datePicker.IsDropDownOpen = true;
        cryoCalendar = calendar.FindChild<CryoCalendar>("PART_CryoCalendar");
        datePicker.IsDropDownOpen = false;
      }

      return cryoCalendar;
    }
  }
}
