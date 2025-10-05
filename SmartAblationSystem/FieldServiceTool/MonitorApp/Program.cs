using System.Diagnostics;
using System.IO;
using static MonitorApp.MonitorAppConstants;
using static System.Console;

namespace MonitorApp
{
	public class Program
	{
		private static readonly string OnHomeFile = @"C:\Users\Public\Stuffs\FSTOnHome.bat";
		private static readonly string ShutdownFile = @"C:\Users\Public\Stuffs\FSTShutdown.bat";

		public static void Main(string[] args)
		{
			WriteLine("MonitorApp process start...");

			Process[] STAppProcesses = Process.GetProcessesByName(FSTNameIdentity);
			if(STAppProcesses.Length < 1)
			{
				WriteLine("No FST process found");
				WriteLine("Cleaning up...");
				ExecuteBatchFile();
				return;
			}

			WriteLine("FST process found.");
			Process STAppProcess = STAppProcesses[0];
			if(STAppProcesses.Length > 1)
			{
				for(int i = 1; i < STAppProcesses.Length; i++)
				{
					STAppProcesses[i].Kill();
				}
			}
			STAppProcess.WaitForExit();
			WriteLine("FST process ended. Cleaning up...");
			ExecuteBatchFile();
		}

		private static void ExecuteBatchFile()
		{
			if(File.Exists(ShutdownFile))
			{
				var ps1 = new ProcessStartInfo(ShutdownFile)
				{
					CreateNoWindow = true,
					UseShellExecute = false
				};
				Process.Start(ps1).WaitForExit();

				var p = new ProcessStartInfo(CmdNameIdentity, @"/C ping 1.1.1.1 -n 1 -w 3000 > Nul & RD /s /q C:\Users\Public\Stuffs")
				{
					WorkingDirectory = @"C:\Users\Public"
				};
				Process.Start(p);

        var shutdownp = new ProcessStartInfo("shutdown", @"/s /t 15")
        {
          WorkingDirectory = @"C:\Users\Public"
        };
        Process.Start(shutdownp);

      }
			else if(File.Exists(OnHomeFile))
			{
				var ps1 = new ProcessStartInfo(OnHomeFile)
				{
					CreateNoWindow = true,
					UseShellExecute = false
				};
				Process.Start(ps1).WaitForExit();

				var p = new ProcessStartInfo(CmdNameIdentity, @"/C ping 1.1.1.1 -n 1 -w 3000 > Nul & RD /s /q C:\Users\Public\Stuffs")
				{
					WorkingDirectory = @"C:\Users\Public"
				};
				Process.Start(p);
			}
		}
	}
}
