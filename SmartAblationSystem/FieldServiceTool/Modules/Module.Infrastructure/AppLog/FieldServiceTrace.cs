using Module.Infrastructure.Helpers;
using Module.Infrastructure.Properties;
using System;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using static System.DateTime;
using static Module.Infrastructure.Constants.Strings;

namespace Module.Infrastructure.AppLog
{
	public static class FieldServiceTrace
	{
		private static readonly string LogTitle = Resources.LogTitle;
		private static readonly string Underscore = Resources.Underscore;
		private static readonly string LogExtension = Resources.LogExtension;
		private static readonly string LogFolderName = Resources.LogFolderName;
		private static readonly string TagTitle = Resources.TagTitle;
		private static readonly string DateTimeFormatString = Resources.DateTimeFormatString;
		private static readonly string Delimiter = Resources.Delimiter;
		private static readonly string SemiColonDelimiter = Resources.SemiColonDelimiter;
		private static readonly USBManager _usbManager = new USBManager(USBArrivedEventHandler);
		private static long Id;

		static FieldServiceTrace() => SetupTrace();

		private static void SetupTrace()
		{
			Trace.Listeners.Clear();
			string logFileName = LogTitle + Underscore + Now.Date.Year + "_" + Now.Date.Month + Now.Date.Day + "_" + Now.Ticks + LogExtension;
			string exeFilePath = Assembly.GetExecutingAssembly().Location;
			string directoryName = Path.GetDirectoryName(exeFilePath);

			if(directoryName != null)
			{
				var pathLogFile = Path.Combine(directoryName, LogFolderName);
				if(!Directory.Exists(pathLogFile))
				{
					Directory.CreateDirectory(pathLogFile);
				}
			}

			string[] path = { directoryName, LogFolderName, logFileName };
			var pathString = Path.Combine(path);

			if(pathString != null)
			{
				var fileListener = new TextWriterTraceListener(pathString);
				Trace.Listeners.Add(fileListener);
			}

			Trace.AutoFlush = true;
			Trace.WriteLine(TagTitle, Now.ToString(DateTimeFormatString));
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public static void Log(string msg, Level level = Level.Info, [CallerMemberName] string methodName = null, [CallerLineNumber] int lineNumber = 0)
		{
			Trace.Write($"{Id++,-8}");
			Trace.Write($"{Now.ToString(DateTimeFormatString),-26}");
			Trace.Write($"{level,-10}");
			var tid = $"Thread {Thread.CurrentThread.ManagedThreadId,3}";
			Trace.Write($"{tid,-14}");
			var file = $"Class: {NameOfCallingClass()}";
			Trace.Write($"{file,-40}");
			Trace.Write($"Member: {methodName,-48}");
			Trace.Write($"Line#: {lineNumber,-12}");
			Trace.WriteLineIf(!string.IsNullOrEmpty(msg), $"Msg: {msg}");
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public static void LogException(Exception exception)
		{
			Trace.Write(Id++ + Delimiter);
			Trace.Write(Now.ToString(DateTimeFormatString) + SemiColonDelimiter);
			Trace.Write(" " + Level.Exception + Delimiter);
			Trace.Write($"Message: {exception.Message}");
			Trace.WriteLine($"{Delimiter}StackTrace: {exception.StackTrace}");
			Trace.WriteLineIf(exception.InnerException != null, $"{Delimiter}Inner Exception: {exception.InnerException?.StackTrace}");
		}

		private static string NameOfCallingClass()
		{
			string fullName;
			Type declaringType;
			int skipFrames = 2;
			do
			{
				MethodBase method = new StackFrame(skipFrames, false).GetMethod();
				declaringType = method.DeclaringType;
				if(declaringType == null)
				{
					return method.Name;
				}
				skipFrames++;
				fullName = declaringType.Name;
			}
			while(declaringType.Module.Name.Equals("mscorlib.dll", StringComparison.OrdinalIgnoreCase));
			return fullName;
		}

		private static string GetUSBLocation()
		{
			return _usbManager.DriveInfos == null || _usbManager.DriveInfos.Count == 0
				? string.Empty
				: _usbManager?.DriveInfos[0]?.Name;
		}

		private static void USBArrivedEventHandler(object sender, EventArrivedEventArgs e)
		{
			var USBDriveList_ = _usbManager.GetUSBDriveList();
			var USBDriveConnected_ = USBDriveList_ != null && USBDriveList_.Count > 0;
			var IsServiceToolAvailable_ = USBDriveConnected_ && File.Exists(USBDriveList_[0].Name + FSTZipName);
		}
	}
}
