namespace Module.TestProcess.Services
{
	public interface IAblationConfiguration
	{
		bool IsFastInflation { get; set; }
		int InflationTimeInSecond { get; set; }
		int InflationRecordingIntervalMillisecond { get; set; }

		int AblationTimeInSecond { get; set; }
		int ThawingTimeInSecond { get; set; }
    bool EnableDASBalloon { get; set; }
  }
}
