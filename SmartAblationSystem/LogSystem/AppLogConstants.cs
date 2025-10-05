namespace LogSystem
{
  public static class AppLogConstants
  {
    public const string LogTitle = "SmartFreezeLog_";
    public const string Underscore = "_";
    public const string LogExtension = ".log";
    public const string LogFolderName = "Logs";
    public const string DateTimeFormatString = "MMM/dd/yyyy HH:mm:ss.fff";
    public const string mscorlibName = "mscorlib.dll";
    public const string DestinationLogFolder = @"Logs\";

    public const string WinEvtType = "Application";
    public const string WinEvtLogTitle = "WinEvt_";
    public const string WinEvtMetaDataFolder = "LocaleMetaData";
    public const string WinEvtLogExtension = ".evtx";
    public const string WinEvtQuery = "*";
    public const string WinEvtQuery_Critical = "*[System/Level=1 or System/Level=2]";
    public const string WinEvtTimeString = "yyyyMMdd_HHmmss";
    public const string ZipExtension = ".zip";

    public const int PastHours = 24 * 30;
    public const long TimeDurationFactorInMilliseconds = 60 * 60 * 1000;

    public const double TestLogExpirationInSecond = 100;
    public const double TestCleanUpPeriodInSecond = 10;

    public const double OneDayInSecond = 86_400;										            // 1 day in second
    public const double OneWeekInSecond = OneDayInSecond * 7;						        // 1 week in second
    public const double ThirtyDaysInSecond = OneDayInSecond * 30;				        // 30 day in second

    public const double FirstCleanupTimeInSecond = 10;
    public const double CleanUpPeriodInSecond = OneDayInSecond;                 // 1 days
    public const double LogExpirationTimeInSecond = ThirtyDaysInSecond;         // 30 days
  }
}
