using Module.SystemParameters.Extensions;

namespace Module.TestProcess.Services
{
	public static class ServiceConstants
	{
		public const double DelayForAblationTest = 1.0d;
		public const double DelayFromStartingToStartedInSecond = 2.0d;
    public const int WaitForSystemToReadyInMillisecond = 2000;
		public const double IntervalBetweenTestsInSecond = 2.0d;
		public const double StateSwitchIntervalInSecond = 0.5d;
		public const double DelayForSessionStatusChangeInSecond = 0.2d;
		
		public const double DelayBeforeStateSwitchInSecond = 0.1d;
		public const double ShortDelayInSecond = 0.1d;
		public const double IntervalForLevelChangeInSecond = 0.1d;
		public const double DelayBeforeTestInSecond = 1.0d;
    public const double DelayForConsoleWarningTestInSecond = 0.5d;

		public const double SampleIntervalInMillisecond = 1_000.0d;

		public const double RecordingPeriodInSecond = 16.0d;

    public const double PauseForIbpStabilizationInSecond = 0.5d;
		public const double TimeoutForIdleToReadySwitchInSecond = 10.0d;
		public const double TimeoutReadyToInflationInSecond = 10.0d;

		public const int TimeoutForAblationToThawingInMilliseconds = 5_000;
		public const double TimeoutForAblationToThawingInSeconds = 5.0d;
		public const int TimeoutForThawingToReadyInMilliseconds = 30_000;

		public const double IntervalBetweenInputTestInSecond = 2.0;
		public const double InputTestTimeoutInSecond = 2.0;
		public const double DelayBeforeVisualTest = 1.0;
		public const double IntervalInVisualTestInSecond = 0.1;

		public const int Ibp_Stabilization_In_Second = 5;
		public const int TimeoutForIbpStabilizationInSecond = 25;

    public const double WaitForTemperatureAfterInflationInSeconds = 30.0;
		public const double TC1LowThresholdForInflationSpeedMode = 35.0;
		public const double TC1HighThresholdForInflationSpeedMode = 40.0;
		public const double IBPThresholdForInflationSpeed = 1.5d;
		public const double IbpInflationStableValue = 2.5;
		public const double IbpInflationStableValueVariant = 0.3;
		public const double IbpStabilizationTimeoutInSecond = 12.0d;
		public const double TimeoutSwitchToAblationStateInSecond = 40.0d;
		public const double TimeoutForDataAnalysisTaskInSecond = 20.0d;
		public const double TimeoutForTransitionSwitch = 40.0d;
    public const double TimeoutToFinishThawingStateInSecond = 60.0; 
		public const double Percentage = 100.0d;
		public const double OnePercentage = 1.0d;

		public const double OneThousand = 1_000.0d;
    public const double SlowInflationDelta = 0.50d;
		public const double SlowInflationSpeedLower = 3.00d;
		public const double SlowInflationSpeedUpper = 5.00d;
		public const double FastInflationSpeedUpper = 2.00d;
		public const double InflationSpeedIBPThreshold = 2.0d;
		
		public const int HalfAblationCount = 5;

		public const double OBPInflationThreshold = -13.3d;
		public const double PT2InflationLower = 120.0d;
		public const double PT2InflationUpper = 180.0d;
		public const double FM1InflationLower = 600.0d;
		public const double FM1InflationUpper = 1200.0d;
		public const double IBPInflationLower = 2.0d;
		public const double IBPInflationUpper = 3.0d;

    public const double IBPDASBalloonInflationLower = 7.0d;
    public const double IBPDASBalloonInflationUpper = 8.0d;

    public const double SensorSamplingIntervalForReadyInSecond = 0.2d;
		public const double SensorSamplingIntervalForAblationInSecond = 1.0d;
		public const double SensorSamplingIntervalForThawingInSecond = 1.0d;
		public const int SensorSamplingIntervalForAblationMillisecond = 1000;

		public const double SamplingPeriodInReadyState = 2.0d;

		public const double TimeoutForReachMinus50InSecond = 50.0d;

		public const double FM1AblationLower = 7500.0d;
		public const double FM1AblationUpper = 8100.0d;

    public const double FM1DASBalloonAblationLower = 8400.0d;
    public const double FM1DASBalloonAblationUpper = 9000.0d;

		public const double PT2AblationLower = 350.0d;
		public const double PT2AblationUpper = 650.0d;

		public const double IBPAblationLower = 2.0d;
		public const double IBPAblationUpper = 3.0d;
    public const double IBPTargetForSwitchToDASBalloon = 2.5d;

		public const double OBPDeltaAblationState = 0.3d;
		
		public const double OBPDeltaInflationState = 0.1d;

		public const double PWM2AblationThreshold = 70.0d;

		public const double TS1AblationThreshold = -10.0d;
		public const double Minus50Celsius = -49.99d;

		public const double PT3IdleLower = 13.2d;
		public const double PT3IdleUpper = 15.0d;
		public const double DelayThawingDataCollectionInSecond = 5.0d;
		public const double DelayReadyStateDataCollectionInSecond = 5.0d;

		public const int SkipSeconds = 5;
    public const int SkipSecondsForDASBalloon = 15;

    public const int POLARxFITCatheterId = 2;
  }
}
