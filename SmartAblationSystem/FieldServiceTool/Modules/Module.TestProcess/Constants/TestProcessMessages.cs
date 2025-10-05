namespace Module.TestProcess.Constants
{
	public static class TestProcessMessages
	{
		public const string FieldServiceToolTestTitle = "Field Service Tool Application - ";
		public const string Step1TestCaption = "Step 1 - Manual Tests - ";
		public const string Step2TestCaption = "Step 2 - Parameter Check - ";
		public const string Step3TestCaption = "Step 3 - Performance Tests - ";
		public const string TestInProgressMessage = "Started.";
		public const string TestFinishedMessage = "Finished.";
		public const string TestPausedMessage = "Paused.";
		public const string TestPausingMessage = "Pausing...";
		public const string TestPassedMessage = "Passed.";
		public const string TestFailedMessage = "Failed.";
		public const string TestStoppedMessage = "Stopped.";
		public const string TestStoppedByExceptionMessage = "Stopped by Exception.";
		public const string CurrentTestSessionFinishedMessage = "Test finished.";

		#region Step 1 Version Verification

		public const string VersionVerificationTitle = "Version Verification - ";

		#endregion Step 1 Version Verification

		#region Step 1 Input Test

		public const string InputTestTitle = "Input Test - ";
		public const string StartPushButtonTest = "Start Push Button Test - ";
		public const string StopPushButtonTest = "Stop Push Button Test - ";
		public const string StartFootSwitchTest = "Start Foot Switch Test - ";
		public const string StopFootSwitchTest = "Stop Foot Switch Test - ";

		#endregion Step 1 Input Test

		#region Step 1 Visual Test

		public const string VisualTestTitle = "Visual Test - ";
		public const string LEDTestMsgTitle = "LEDs Test - ";
		public const string ScreenTestMsgTitle = "Screen Test - ";

		#endregion Step 1 Visual Test

		#region Step 1 Audible Test

		public const string AudibleTestTitle = "Audible Test - ";

		#endregion Step 1 Audible Test

		#region Step 2 Idle State

		public const string IdleStateCheckTitle = "Idle State Check - ";
		public const string SamplingIdleStateSensorDataMessage = "Sampling FM1, PT1, LC1, PT3, TS1 ...";
		public const string SamplingIdleSensorDataFinishedMessage = "Finished sampling sensor data.";
		public const string CheckingTankMessage = "Checking LC1 and PT1 ...";
		public const string CheckedTaskMessage = "Finished checking tank status.";
		public const string ValidatingIdleStateSensorDataMessage = "Validating FM1, PT3, TS1 ...";
		public const string ValidatingIdleStateSensorDataFinishedMessage = "Finished validating sensor data.";
    public const string WaitForSubCoolerTemperatureMessage = "Waiting for the average sub-cooler temperature to drop below -25°C.";
    public const string SampleSensorDataForTS1 = "Collecting sensor data for high TS1";
		public const string DataErrorMsg = "Sensor sampled data out of range.";

		#endregion Step 2 Idle State

		#region Step 2 Ready State

		public const string ReadyStateCheckTitle = "Ready State Check - ";
		public const string RetrievingCatheterConnectionMessage = "Retrieving catheter connection information ... Please wait.";
		public const string CatheterConnectionFailureMessage = "Did not detect a valid catheter connection. Please check the catheter electrical connections and then press Retry to try again or Stop to stop the test.";
		public const string CatheterInvalidDialogTitle = "Catheter Connection Check";
    public const string CatheterIdVerificationDialogTitle = "Catheter ID Check";
		public const string VerifySystemStateInIdle = "Verify system state is in IDLE ...";
		public const string ConfirmSystemStateInIdle = "Starting state is IDLE.";
		public const string StartingSystemIsNotIdle = "Starting state is not IDLE.";
		public const string ConnectCatheterDialogTitle = "Connect Catheter";
		public const string ConnectCatheterMessage = "Connect the catheter to the console (electrically and mechanically).";
		public const string ConnectingCatheter = "Check the catheter connection ...";
		public const string ValidatingCatheterMessage = "Validating the connected catheter ...";
		public const string CatheterReadyMessage = "Catheter connected and ready.";
		public const string DetectCatheterConnectionMessage = "Detecting catheter connection ...";
		public const string CheckReadyStateMessage = "Checking Ready state ...";
		public const string SystemIsReadyMessage = "System is in the Ready state.";
		public const string SystemFailsToSwitchReadyMessage = "System fails to switch to Ready state.";
		public const string SampleReadyStateSensorDataMessage = "Sampling sensor data in the Ready state ...";
		public const string ValidateReadyStateSensorsDataMessage = "Validating sensor data for the Ready state ..";
		public const string CatheterConnectionFailureTitle = "Catheter Connection Fail";

    #endregion Step 2 Ready State

    #region Step 3 Ablation Tests

    public const string AblationTestTitle = "Ablation Tests - ";
		public const string AblationName = "Ablation";
		public const string AblationConfigurationFile = @"Configuration/AblationConfig.xml";
		public const string AblationConfigurationFileInDebug = @"Configuration/AblationConfigDebug.xml";
		public const string InitializingConsoleMessage = "Initializing Console ...";
		public const string ConsoleInitializedMessage = "Console initialized sucessfully.";
		public const string InitializingConfigurationMessage = "Initializing Configuration ...";
		public const string ConfigurationInitializedMessage = "Configuration initialized sucessfully.";
		public const string InitializingDataManagement = "Initializing Data Management ...";
		public const string DataManagementInitializedMessage = "Data Management initialized sucessfully.";
		public const string SensorDataFileGenerationGeneratedSuccessfully = "Sensor data file generated successfully.";
		public const string SensorDataFileGenerationFailed = "Sensor data file generation failed.";
		public const string OfText = " of ";
		public const string InflationSpeedSlow = "Slow";
		public const string InflationSpeedFast = "Fast";
		public const string Ablations = " Ablation Tests - ";
		public const string VerifyReadyStateMessage = "Waiting for the system to be in the Ready state ...";
		public const string MessageFromReadyToInflation = "Transitioning to the Inflation state ...";
		public const string MessageInflationSpeedMode = "Inflation speed mode is set to - ";
		public const string MessageFromInflationToAblation = "Transitioning to the Ablation state ...";
		public const string MessageFromAblationToThawing = "Transitioning to the Thawing state ...";
		public const string MessageFromThawingToReady = "Transitioning to the Ready state ...";
		public const string MessageFromReadyToIdle = "Transitioning to the Idle state ...";
		public const string MessageInIdleState = "in Idle state.";
		public const string MessageInReadyState = "in Ready state.";
		public const string MessageInInflationState = "in Inflation state.";
		public const string MessageInAblationState = "in Ablation state.";
		public const string MessageInThawingState = "in Thawing state.";
		public const string MessageInIbpStabilizationState = "Waiting for IBP stabilization ...";
		public const string MessageForIbpStabilization = "IBP is stabilized in the Inflation state.";
		public const string MessageTurningOffVacuum = "Turning off vacuum.";
		public const string MessageTurnedOffVacuum = "Vacuum is off.";
		public const string MessageAnalyzeData = "Analyzing sensor data ... ";
		public const string CheckBalloonTemperatureMessage = "Checking if balloon temperature is ready for ablation ...";
		public const string BalloonTemperatureTooLowTitle = "Balloon Temperature";
		public const string BalloonTemperatureTooLow = "The balloon temperature is too low to start an ablation. Please increase the water bath temperature.";
		public const string BalloonTemperatureTooLowUserMessage = "Waiting for the balloon temperature to rise above 35°C.";
		public const string BalloonTemperatureReadyUserMessage = "The balloon temperature is ready to begin ablations.";
		public const string ConsoleExceptionTitleText = "Console Exception";
		public const string ConsoleExceptionMessage = "The Test stopped - The console is in EXCEPTION state.";
		public const string InitializingConfigurationIOExceptionMessage = " - IO Exception in Initializing Ablation Configuration.";
		public const string InitializingConfigurationParsingExceptionMessage = " - Exception in Parsing Ablation Configuration.";
		public const string IdleToReadySwitchErrorMessage = "Failed to switch to the Ready state.";
		public const string ReadyToInflationSwitchErrorMessage = "Timeout, Console did not enter the Inflation state.";
		public const string IbpStabilizationTimeoutMessage = "Timeout, IBP did not stabilize.";
		public const string TimeoutForSwitchingToAblationMessage = "Timeout, Console did not enter the Ablation state.";
		public const string TimeoutForSwitchingToThawingMessage = "Timeout, Console did not enter the Thawing state.";
		public const string ProcessingDataAnalysis = "Processing data analysis ...";
		public const string SavingDataMessage = "Saving data to USB ...";
		public const string SavedDataMessage = "Saved ablation data to USB successfully.";
		public const string FailedSavingDataMessage = "Failed to save ablation data to USB.";
		public const string FinishedDataAnalysis = "Finished data analysis.";
		public const string TimeSecondText = " seconds.";
		public const string InflationTimeText = " Inflation Time = ";
		public const string AblationTimeText = " Ablation Time = ";
		public const string ThawingTimeText = " Thawing Time = ";
		public const string StopMessage = "Stopped.";
		public const string GeneratingReportText = "Generating test report ...";
		public const string DataAnalysisExceptionMessage = "Can't finish data analysis in ablation tests.";

		#endregion Step 3 Ablation Tests

		#region Step 3 DMS Tests

		public const string DmsTestTitle = "DMS Tests ";

		#endregion Step 3 DMS Tests

		#region Step 3 ETS Tests

		public const string EtsTestTitle = "ETS Tests ";

		#endregion Step 3 ETS Tests

		#region Step 3 OPS Tests

		public const string OpsTestTitle = "OPS Tests ";

    #endregion Step 3 OPS Tests
  }
}
