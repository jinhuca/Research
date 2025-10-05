namespace Module.Infrastructure.Constants
{
	public static class MonitorConstants
	{
		public const string FSTNameIdentity = "ServiceToolApp";
		public const string FSTPathOnConsole = @"C:\Program Files\BSC\Smart Ablation System\ServiceTool";
		public const string FSTPathOnConsoleWithParentheses = "\"" + FSTPathOnConsole + "\"";

		public const string MonitorAppIdentity = "MonitorApp.exe";
		public const string MonitorAppFolder = TempFolderPath + @"\MonitorAppFolder";

		public const string CmdNameIdentity = "cmd.exe";
		public const string MonitorFolderPath = TempFolderPath + @"\MonitorAppFolder";
		public const string OnHomeBatchPath = TempFolderPath + @"\FSTOnHome.bat";
		public const string OnShutdownBatchPath = TempFolderPath + @"\FSTShutdown.bat";
		public const string DeleteOnHomeBatchCmd = @"DEL " + OnHomeBatchPath;
		public const string DeleteOnShutdownBatchCmd = @"DEL " + OnShutdownBatchPath;
		public const string TempFolderPath = TempPublicPath + @"\Stuffs";
		public const string DeleteFSTCmd = @"RD /S /Q " + FSTPathOnConsoleWithParentheses;

		public const string TempPublicPath = @"C:\Users\Public";
		public const string DeleteStuffs = @"RD /S /Q " + TempFolderPath;
	}
}
