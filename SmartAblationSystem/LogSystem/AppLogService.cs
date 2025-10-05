using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Timers;
using static LogSystem.AppLogConstants;

namespace LogSystem
{
	public static class AppLogService
	{
		private static readonly System.Timers.Timer _logTimer = new System.Timers.Timer
		{
			Interval = TimeSpan.FromSeconds(MaxTimeForLogFileInSecond).TotalMilliseconds,
			Enabled = true,
			AutoReset = true
		};

		private static readonly System.Timers.Timer _cleanUpTimer = new System.Timers.Timer
		{
			Interval = TimeSpan.FromSeconds(CleanUpPeriodInSecond).TotalMilliseconds,
			Enabled = true,
			AutoReset = true
		};

		private static long _logStartIndex;
		private static string _logFileName = string.Empty;
		private static readonly IDisposable _createLogDisposable;
		private static readonly IDisposable _cleanUpLogDisposable;

		/// <summary>
		/// Static constructor for creating and starting log timers.
		/// </summary>
		static AppLogService()
		{
			_createLogDisposable = Observable.FromEventPattern<ElapsedEventArgs>(_logTimer, nameof(_logTimer.Elapsed))
				.ObserveOn(TaskPoolScheduler.Default)
				.Subscribe(e => CreateLogFile());

			_cleanUpLogDisposable = Observable.FromEventPattern<ElapsedEventArgs>(_cleanUpTimer, nameof(_cleanUpTimer.Elapsed))
				.ObserveOn(TaskPoolScheduler.Default)
				.Subscribe(e => CleanUpLogs());

			_logTimer.Start();
			_cleanUpTimer.Start();

			CleanUpLogs();
			CreateLogFile();
		}

		/// <summary>
		/// Method for cleaning up old log files
		/// </summary>
		/// <param name="e"></param>
		private static void CleanUpLogs()
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
			foreach(var file_ in di_.GetFiles())
			{
				if(file_.LastAccessTime >= DateTime.Now.AddSeconds(-LogExpirationInSecond))
				{
					continue;
				}

				try
				{
					file_.Delete();
				}
				catch(IOException ioe)
				{
					Trace.WriteLine(ioe.Message);
					break;
				}
			}
		}

		/// <summary>
		/// Create log file based on the current date and time.
		/// </summary>
		private static void CreateLogFile()
		{
			var directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			if(directoryName == null)
			{
				return;
			}

			var logFileName = LogTitle + DateTime.Now.Date.Year + Underscore +
												DateTime.Now.Date.Month + DateTime.Now.Date.Day + Underscore +
												DateTime.Now.Ticks + LogExtension;

			var LogFolderName_ = Path.Combine(directoryName, LogFolderName);

			if(!Directory.Exists(LogFolderName_))
			{
				try
				{
					Directory.CreateDirectory(LogFolderName_);
				}
				catch(Exception ex)
				{
					Trace.WriteLine(ex.Message);
					return;
				}
			}
			_logFileName = Path.Combine(LogFolderName_, logFileName);
			_logStartIndex = 0;
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
			var logInstance = new LogInfo
			{
				Id = _logStartIndex++,
				ThreadId = Thread.CurrentThread.ManagedThreadId,
				ClassName = NameOfCallingClass(),
				MethodName = methodName,
				LineNumber = lineNumber,
				Timestamp = DateTime.Now,
				Message = msg,
				Level = logLevel
			};

			using(var writer_ = new StreamWriter(_logFileName, true))
			{
				var log_ = JsonConvert.SerializeObject(logInstance, Formatting.Indented);
				if(log_ != null)
				{
					writer_.WriteLine(log_);
				}
			}
		}

		/// <summary>
		/// Log Exception.
		/// </summary>
		/// <param name="ex"></param>
		/// <param name="methodName"></param>
		/// <param name="lineNumber"></param>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public static void LogException(ExceptionInfo ex, [CallerMemberName] string methodName = "", [CallerLineNumber] int lineNumber = 0)
		{
			using(var writer_ = new StreamWriter(_logFileName, true))
			{
				ex.Id = _logStartIndex++;
				ex.ThreadId = Thread.CurrentThread.ManagedThreadId;
				ex.Level = LogLevel.Exception;
				ex.ClassName = NameOfCallingClass();
				ex.MethodName = methodName;
				ex.LineNumber = lineNumber;
				ex.Timestamp = DateTime.Now;
				var log_ = JsonConvert.SerializeObject(ex, Formatting.Indented);

				if(log_ != null)
				{
					writer_.WriteLine(log_);
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
					file_.MoveTo(Path.Combine(destinationLogFolder_, file_.Name));
				}
				catch(IOException ioe)
				{
					Trace.WriteLine(ioe.Message);
					break;
				}
			}
		}

		/// <summary>
		/// Close AppLogService by disposing observables.
		/// </summary>
		public static void Close()
		{
			_createLogDisposable.Dispose();
			_cleanUpLogDisposable.Dispose();
		}

		/// <summary>
		/// Get calling class name by reflection.
		/// </summary>
		/// <returns></returns>
		private static string NameOfCallingClass()
		{
			string fullName;
			Type declaringType;
			int skipFrames = 2;
			do
			{
				var method = new StackFrame(skipFrames, false).GetMethod();
				declaringType = method.DeclaringType;
				if(declaringType == null)
				{
					return method.Name;
				}
				skipFrames++;
				fullName = declaringType.Name;
			}
			while(declaringType.Module.Name.Equals(mscorlibName, StringComparison.OrdinalIgnoreCase));
			return fullName;
		}
	}
}
