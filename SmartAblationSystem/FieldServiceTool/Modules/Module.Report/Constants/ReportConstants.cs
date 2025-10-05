namespace Module.Report.Constants
{
	public static class ReportConstants
	{
		public const string TestResultTitle = "Test Report ";
		public const string PageFieldText = "Page";
		public const string NullTestResultMessage = "No test report generated due to Test Result is null";
		public const string TestText = "Test";
		public const string ExpectedText = "Expected Result";
		public const string ActualText = "Actual Result";
		public const string ResultText = "Pass/Fail";
		public const string PassedMessage = "Pass";
		public const string FailedMessage = "Failed";
		public const string PassedImage = @"\Passed.png";
		public const string FailedImage = @"\Failed.png";
		public const string NotAvailableMessage = "N/A";

		public const string ElementTypeCover2 = "Cover2";
		public const string ElementTypeTable = "table";
		public const string ElementTypeTableBig = "tableBig";
		public const string ElementTypeTableSmall = "tableSmall";
		public const string ElementTypeTableImage = "tableimage";
		public const string ElementTypeTableImageResult = "tableimageresult";
		public const string ElementTypeTableBigNewPage = "tableBigNewPage";
		public const string ElementTypeB = "tableBig2-b";
		public const string ElementTypeNewPage = "NEWPAGE";

		public const string ConsoleText = "Console ";
		public const string TesterNameText = "Tester Name: ";
		public const string DateTimeFormat = "MMMM dd, yyyy";

		public const string SiteText = "Hospital Name:";
		public const string ConsoleSnText = "Console S/N: ";
		public const string TesterFullNameText = "Tester Name: ";
		public const string TestDateTimeText = "Test Date/Time: ";
		public const string FstVersion = "Service Tool Application Version: ";
		public const string OverallResultText = "Overall Test Result: ";
		public const string DashWithSpace = " - ";
		public const string StartMsg = "Start ";
		public const string FinishMsg = " Finish ";
		public const string ManualTestsText = "1. Manual Test Result: ";

		public const string VersionVerificationText = "1.1 Version Verification Result: ";

		public const string CMCUBootLoaderText = "Control MCU Boot Loader";     // (0)  CMCUBootVersion
		public const string CMCUApplicationText = "Control MCU Application";    // (1)  CMCUVersion
		public const string CPLDText = "CPLD";                                  // (2)  CPLDVersion
		public const string PMCUBootLoaderText = "Patient MCU Boot Loader";     // (3)  PMCUBootVersion
		public const string PMCUApplicationText = "Patient MCU Application";    // (4)  PMCUVersion
		public const string RMCUBootLoaderText = "Repeater MCU Boot Loader";    // (5)  RMCUBootVersion
		public const string RMCUText = "Repeater MCU Application";              // (6)  RMCUVersion
		public const string ICBBootLoaderText = "ICB MCU Boot Loader";          // (7)  ICBBootVersion
		public const string ICBApplicationText = "ICB MCU Application";         // (8)  ICBVersion
		public const string RCMCUBootLoaderText = "Remote MCU Boot Loader";     // (9)  RCMCUBootVersion
		public const string RCMCUText = "Remote MCU Application";               // (10) RCMCUVersion
		public const string GUIText = "SmartAblation Application";              // (11) GUIVersion
		public const string DBText = "SmartAblation Database";                  // (12) DBVersion

		public const string InputTestText = "1.2 Input Test Result: ";
		public const string StartPushButtonText = "Start Pushbutton";
		public const string StopPushButtonText = "Stop Pushbutton";
		public const string StartFootSwitchText = "Start Foot Switch";
		public const string StopFootSwitchText = "Stop Foot Switch";
		public const string OnText = "ON";
		public const string OffText = "OFF";

		public const string VisualTestText = "1.3 Visual Test Result: ";
		public const string ConsoleLEDsText = "Console LEDs";
		public const string OnOffText = "ON / Flashing";
		public const string DisplayTestText = "Display Clear";
		public const string DisplayMessageText = "Console monitor is clear and displays from edge to edge";

		public const string AudibleTestText = "1.4 Audible Test Result: ";
		public const string SpeakerText = "Speaker";
		public const string AudibleMessage = "Sound is heard";

		public const string ParameterCheckText = "2. Parameter Check Result: ";

		public const string IdleCheckText = "2.1 Idle State Test Result: ";
		public const string AvgFM1Text = "Average Flow (FM1)";
		public const string FM1ThresholdText = "<= 40";
		public const string AvgPT1Text = "Average Tank Pressure (PT1)";
		public const string PT1ThresholdText = "N/A";
		public const string PT1Result = "N/A";
		public const string AvgLC1Text = "Average Remaining N2O (LC1)";
		public const string AvgIBPText = "Average Inner Balloon Pressure (IBP)";
		public const string LC1ThresholdText = "N/A";
		public const string AvgPT3Text = "Average Return Pressure (PT3) (Atmospheric)";
		public const string PT3Threshold = "N/A";
		public const string PT3Result = "N/A";
		public const string AvgTS1Text = "Average Sub-Cooler Temperature (TS1)";
		public const string AvgTS1ThresholdText = "<= -25";

		public const string ReadyCheckText = "2.2 Ready State Test Result: ";
		
		public const string MaxOBPText = "Maximum Outer Balloon Pressure (OBP)";
		public const string PerformanceTestText = "3. Performance Test Result: ";
		public const string AblationTestsText = "3.1 Ablation Tests Result: ";
		public const string InflationTestTitleText = "3.1.1 Inflation";

		public const string InflationIndexText = "Inflation #";
		public const string AblationIndexText = "Ablation #";
		public const string ThawingIndexText = "Thawing #";

		public const string InflationIBPText = "Test Average Inner Balloon Pressure (IBP)";
		public const string InflationOBPText = "Test Maximum Outer Balloon Pressure (OBP)";

		public const string AblationTestTitleText = "3.1.2 Ablation";
		public const string AblationTimeInTransitionText = "Time in Transition";
		public const string AblationFM1Text = "Flow Meter (FM1)";
		
		public const string AblationPT2Text = "Injection Pressure (PT2)";
		
		public const string AblationIBPText = "Inner Balloon Pressure (IBP)";
		
		public const string AblationOBPText = "Outer Balloon Pressure (OBP)";
		public const string AblationOBPThresholdText = "<= -13.3";
		public const string AblationLowestTC1Text = "Lowest Balloon Temperature (TC1)";
		public const string AblationTimeTo50Text = "Time to -50°C Balloon Temperature";
		public const string AblationPWM1Text = "Injection PWM (PWM1)";
		public const string AblationPWM2Text = "Balloon PWM (PWM2)";
		public const string AblationPT3Text = "Return Pressure (PT3)";
		public const string AblationPT4Text = "Vacuum Pressure (PT4)";
		public const string AblationPT5Text = "Scavenging Pressure (PT5)";
		public const string AblationTS1Text = "Sub Cooler Temperature (TS1)";
		
		public const string SmoothnessCheckText = "Smoothness Check";
		public const string SmoothnessCheckTestText = AblationIndexText + "1";
		
		public const string FlowMeterCheckText = "Flow Meter Check";
		public const string FlowMeterCheckTestText = AblationIndexText + "1";
		public const string FlowMeterCheckSkippedText = "Skipped";

		public const string ThawingTestTitleText = "3.1.3 Thawing";
		public const string ThawingPT3Text = "Test Return Pressure (PT3)";
		public const string ThawingPT4Text = "Test Vacuum Pressure (PT4)";
		public const string ThawingPT5Text = "Test Scavenging Pressure (PT5)";
		public const string ThawingPWM1Text = "Test Injection PWM (PWM1)";
		public const string ThawingPWM2Text = "Test Balloon PWM (PWM2)";

		public const string RetryRationaleTestText = "Retry Rationale";

		public const string ErrorMessageText = "Error Messages";

		public const string TestNameText = "Test Name";
		public const string RationaleText = "Rationale";
		public const string ErrorSummary = "Error Summary";
		public const string ErrorText = "Error Description";
		
		public const string ErrorMessageForGeneratingExcelSheet = "Error in generating sensor data sheets.";

		public const string BinFolderName = "bin";
		public const string ImageFolderName = "Images";
	}
}
