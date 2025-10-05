namespace Module.TestProcess.Services
{
	public class AblationConfiguration : IAblationConfiguration
	{
		public AblationConfiguration() { }

		public AblationConfiguration(
			bool inflationSpeedMode, 
			int inflationTimeInSecond, 
			int inflationRecordingIntervalMillisecond,
			int ablationTimeInSecond,
			int thawingTimeInSecond,
      bool enableDASBalloon)
		{
			IsFastInflation = inflationSpeedMode;
			InflationTimeInSecond = inflationTimeInSecond;
			InflationRecordingIntervalMillisecond = inflationRecordingIntervalMillisecond;
			AblationTimeInSecond = ablationTimeInSecond;
			ThawingTimeInSecond = thawingTimeInSecond;
      EnableDASBalloon = enableDASBalloon;
    }

		public bool IsFastInflation { get; set; }
		public int InflationTimeInSecond { get; set; }
		public int InflationRecordingIntervalMillisecond { get; set; }
		public int AblationTimeInSecond { get; set; }
		public int ThawingTimeInSecond { get; set; }
		public bool EnableDASBalloon { get; set; }
	}
}
