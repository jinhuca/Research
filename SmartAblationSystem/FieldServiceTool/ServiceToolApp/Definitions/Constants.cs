namespace ServiceToolApp.Definitions
{
	internal static class Constants
	{
		public static string SmartFreezeAppPath = "SmartFreezeAppPath";
		public static string SmartFreezeAppPathInSimulation = "SmartFreezeAppPathInSimulation";
		public static string SmartFreezeFileName = "SmartFreezeAppFileName";
		public static string ServiceToolPath = "ServiceToolPath";
		public const string DialogTitleKey = "title";
		public const string DialogMessageKey = "message";
		public const string LogFolderName = "Logs";
		public const string GoSmartFreezeTitleValue = "Transit";
		public const string GoSmartFreezeMessageValue = "The Service Tool Application will close and the SmartAblation application will launch. Any data not already saved to the USB drive will be lost. Continue?";

		public const string StopTestTitleValue = "Stop";
		public const string StopTestMessageValue = "The test will need to start from the beginning and all previously saved data will be lost. Press Yes to stop or No to continue testing.";

		public const string FinishTestTitleValue = "Finish";
		public const string FinishTestMessageValue = "Current test session finished. Confirm to generate report.";
		
		public const string TurnOffTitleValue = "Shut Down";
		public const string TurnOffMessageValue = "Shutting down the console will delete any data not already saved to the USB drive. Continue?";

		public const string FirstNameEmptyErrorMessage = "First name cannot be empty.";
		public const string FirstNameInvalidMessage = "The first name must have between 2 and 20 characters. \nMust start with a letter, and may only contain letters, numbers, spaces, periods, and underscores.";
		public const string LastNameEmptyErrorMessage = "Last name cannot be empty.";
		public const string LastNameInvalidMessage = "The last name must have between 2 and 20 characters. \nMust start with a letter, and may only contain letters, numbers, spaces, periods and underscores.";

		public const int VolumeIntervalInMillisecond = 3000;
		public const string ShutDownProcessCmd = "shutdown";
		public const string ShutDownProcessArguments = @"/s /t 0";
	}
}
