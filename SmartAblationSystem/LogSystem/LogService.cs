using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using static LogSystem.AppLogConstants;
using static System.Environment;
using static System.Reactive.Concurrency.Scheduler;
using static System.TimeSpan;

namespace LogSystem
{
  public static class LogService
  {
    private const string DefaultLogFileFolder = @".\Logs";

    private static readonly string DefaultLogFileName = LogTitle + DateTime.Now.Date.Year + Underscore +
                                                         DateTime.Now.Date.Month + DateTime.Now.Date.Day + Underscore +
                                                         DateTime.Now.Ticks + LogExtension;
    private static long _logStartIndex;
    private static string _logFileName = Path.Combine(DefaultLogFileFolder, DefaultLogFileName);

    public static string LogPath
    {
      get
      {
        var directoryName_ = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        return directoryName_ != null ? Path.Combine(directoryName_, LogFolderName) : string.Empty;
      }
    }

    static LogService()
    {
    }

    /// <summary>
    /// Create log file based on the current date and time.
    /// </summary>
    public static void CreateLogFile()
    {
      var directoryName_ = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
      var logFolderName_ = string.IsNullOrEmpty(directoryName_) 
        ? DefaultLogFileFolder
        : Path.Combine(directoryName_, LogFolderName);

      if(!Directory.Exists(logFolderName_))
      {
        try
        {
          Directory.CreateDirectory(logFolderName_);
        }
        catch(Exception ex_)
        {
          Trace.WriteLine(ex_.Message);
        }
      }
      
      _logFileName = Path.Combine(logFolderName_, DefaultLogFileName);
    }

    public static void SubscribeCleanupLog()
    {
      Observable
        .Timer(dueTime: FromSeconds(FirstCleanupTimeInSecond), period: FromSeconds(CleanUpPeriodInSecond), scheduler: Default)
        .Subscribe(_ => CleanUpLogs());
    }

    /// <summary>
    /// Method for cleaning up old log files
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public static void CleanUpLogs()
    {
      var executableDirectoryLocation_ = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
      if(executableDirectoryLocation_ == null)
      {
        return;
      }

      var logFolder_ = Path.Combine(executableDirectoryLocation_, LogFolderName);
      if(!Directory.Exists(logFolder_))
      {
        return;
      }

      var di_ = new DirectoryInfo(logFolder_);

      foreach (var file_ in di_.GetFiles().Where(fileInfo => fileInfo.FullName != _logFileName))
      {
        if (file_.LastWriteTime >= DateTime.Now.AddSeconds(-LogExpirationTimeInSecond))
        {
          continue;
        }

        try
        {
          file_.Delete();
        }
        catch (IOException ioe_)
        {
          Trace.WriteLine(ioe_.Message);
          break;
        }
      }
    }

    /// <summary>
    /// Log execution information. All parameters are optional for logging purpose.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="logLevel"></param>
    /// <param name="methodName"></param>
    /// <param name="lineNumber"></param>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public static void LogInfo(string msg = "", LogLevel logLevel = LogLevel.Info, [CallerMemberName] string methodName = null, [CallerLineNumber] int lineNumber = 0)
    {
      var settings_ = new JsonSerializerSettings
      {
        TypeNameHandling = TypeNameHandling.All
      };
      var logInstance_ = new LogItemInformation
      {
        Id = _logStartIndex++,
        ThreadId = Thread.CurrentThread.ManagedThreadId,
        ClassName = NameOfCallingClass() ?? string.Empty,
        MethodName = methodName ?? string.Empty,
        LineNumber = lineNumber,
        Timestamp = DateTime.Now,
        Message = msg ?? string.Empty,
        Info = msg ?? string.Empty,
        Level = logLevel
      };

      var log_ = string.Concat(JsonConvert.SerializeObject(logInstance_, Formatting.Indented, settings_), ",");
      try
      {
        using (var writer_ = new StreamWriter(_logFileName, true))
        {
          writer_.WriteLine(log_);
        }
      }
      catch (IOException ioe_)
      {
        Trace.WriteLine(ioe_.Message);
      }
    }

    /// <summary>
    /// Log Exception.
    /// </summary>
    /// <param name="ex"></param>
    /// <param name="methodName"></param>
    /// <param name="lineNumber"></param>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public static void LogException(Exception ex, [CallerMemberName] string methodName = "", [CallerLineNumber] int lineNumber = 0)
    {
      if(ex == null)
      {
        return;
      }
      var settings_ = new JsonSerializerSettings
      {
        TypeNameHandling = TypeNameHandling.All
      };
      var logInstance_ = new LogItemException()
      {
        Id = _logStartIndex++,
        ThreadId = Thread.CurrentThread.ManagedThreadId,
        ClassName = NameOfCallingClass() ?? string.Empty,
        MethodName = methodName ?? string.Empty,
        LineNumber = lineNumber,
        Timestamp = DateTime.Now,
        Level = LogLevel.Exception,
        Info = ex.Message,
        ExceptionInstance = ex
      };

      var log_ = string.Concat(JsonConvert.SerializeObject(logInstance_, Formatting.Indented, settings_), ",");

      using(var writer_ = new StreamWriter(_logFileName, true))
      {
        try
        {
          if(log_ != null)
          {
            writer_.WriteLine(log_);
          }
        }
        catch(Exception ioe)
        {
          Trace.WriteLine(ioe.Message);
        }
      }
    }

    /// <summary>
    /// Export current log files under log folder to designated location.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public static void ExportLog(string destination)
    {
      if(string.IsNullOrEmpty(destination))
      {
        return;
      }

      var destinationLogFolder_ = Path.Combine(destination, DestinationLogFolder);
      if(!Directory.Exists(destinationLogFolder_))
      {
        try
        {
          var temp_ = Directory.CreateDirectory(destinationLogFolder_);
          temp_.Attributes = FileAttributes.Normal;
        }
        catch(Exception e)
        {
          Trace.WriteLine(e);
          return;
        }
      }

      var sourceDirectory_ = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
      if(sourceDirectory_ == null)
      {
        return;
      }

      var logFolder_ = Path.Combine(sourceDirectory_, LogFolderName);
      if(!Directory.Exists(logFolder_))
      {
        return;
      }

      var di_ = new DirectoryInfo(logFolder_);
      foreach(var file_ in di_.GetFiles())
      {
        try
        {
          var destinationFileName_ = Path.Combine(destinationLogFolder_, file_.Name);
          if(File.Exists(destinationFileName_))
          {
            return;
          }
          file_.CopyTo(Path.Combine(destinationLogFolder_, file_.Name));
        }
        catch(IOException ioe)
        {
          Trace.WriteLine(ioe.Message);
          break;
        }
      }
    }

    public static void ExtractWinEventLog(string fileLocation)
    {
      var logSession_ = new EventLogSession();
      try
      {
        logSession_.ExportLog(
          WinEvtType,
          PathType.LogName,
          WinEvtQuery,
          $"{fileLocation}{WinEvtLogTitle}{MachineName}_{DateTime.UtcNow.ToString(WinEvtTimeString)}{WinEvtLogExtension}");
      }
      catch(Exception ex)
      {
        LogException(ex);
      }
      finally
      {
        logSession_.Dispose();
      }
    }

    public static void ExtractWinEventLogAndMsg(string fileLocation)
    {
      var logSession_ = new EventLogSession();
			try
      {
        var timePeriodQuery_ = $"*[System/TimeCreated[timediff(@SystemTime) < {PastHours * TimeDurationFactorInMilliseconds}]] and";
        var query_ = timePeriodQuery_ + WinEvtQuery_Critical;

				logSession_.ExportLogAndMessages(
          WinEvtType,
          PathType.LogName,
          query_,
          fileLocation,
          true,
          CultureInfo.CurrentCulture);
      }
      catch(Exception ex)
      {
        LogException(ex);
      }
      finally
      {
        logSession_.Dispose();
      }
    }

    /// <summary>
    /// Get calling class name by reflection.
    /// </summary>
    /// <returns></returns>
    private static string NameOfCallingClass()
    {
      string fullName_;
      Type declaringType_;
      var skipFrames_ = 2;
      do
      {
        var method_ = new StackFrame(skipFrames_, false).GetMethod();
        declaringType_ = method_.DeclaringType;
        if(declaringType_ == null)
        {
          return method_.Name;
        }
        skipFrames_++;
        fullName_ = declaringType_.Name;
      }
      while(declaringType_.Module.Name.Equals(mscorlibName, StringComparison.OrdinalIgnoreCase));
      return fullName_;
    }
  }
}
