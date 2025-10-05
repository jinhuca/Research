using System;

namespace Module.Infrastructure.Constants
{
  public static class Strings
  {
    public const string FSTZipName = "ServiceTool.zip";
    public const string DialogTitleKey = "title";
    public const string DialogName = "Logon";
    public const string DialogMessageKey = "message";
    public const string MessageDialogTypeKey = "DialogType";
    public const string Fm1TransitionKey = "Fm1Transition";
    public const string SmoothnessVerificationTitle = "Smoothness Verification";
    public const string Tab = "\t";
    public const string SessionStatusParameterKey = "SessionStatusParam";

    public const string SmoothnessMessage =
      "Verify the PID Curve during transition and Ablation are smooth, not erratic and does not alarm.";

    public const string SmoothnessFailureTitle = "Smoothness Verification Failure";

    public const string SmoothnessFailureMessage =
      "The smoothness verification failed. Continue to the next step, retry this step, or stop the test?";

    public const string FirstNameKey = "FirstName";
    public const string LastNameKey = "LastName";
    public static string ParamYes = "Yes";
    public static string ParamNo = "No";

    public const string ReportDateTimeFormat = "yyyyMMdd-HHmmss";
    public const string ReportHeader = "FST-";
    public const string TestReportPrefix = "TestReport-";
    public const string SensorDataPrefix = "SensorData-";
    public const string WhiteSpace = " ";
    public const string Underscore = "_";
    public const string Dash = "-";
    public const string Comma = ",";
    public const string Colon = ":";
    public const string Period = ".";
    public const string LeftParenthesis = "(";
    public const string RightParenthesis = ")";
    public const string CsvExtension = ".csv";
    public const string XlsxExtension = ".xlsx";
    public const string txtExtension = ".txt";
    public const string pdfExtension = ".pdf";
    public const string Id = "Id";
    public const string ChartSeriesName = "WinChartSeries";
    public const string ErrorListParameterKey = "ErrorList";
    public const string CurrentVolumeParameterKey = "CurrentVolume";
    public const string UpdateVolumeActionParameterKey = "UpdateVolumeAction";

    public const string VersionTitle = "Version Verification";
    public const string VersionParameters = "VersionParams";

    public const string StartVersionTestMessage =
      "Start firmware and software version verification tests after click the OK button on this dialog.";

    public const string InputTitle = "Console Input Test";
    public const string InputTestDescription = "Test Start and Stop PushButtons and Foot Switch.";

    public const string StartInputTestMessage =
      "Start Input Tests for Push Buttons on Console Front Panel and Foot Switches:\n\n(1) Start Push Button\n(2) Stop Push Button\n(3) Start Foot Switch\n(4) Stop Foot Switch\n\nClick the OK to Start.";

    public const string InputFailureTitle = "Console Input Test Failure";
    public const string IdleStateCheckFailureTitle = "Idle State Check Failure";
    public const string IdleStateCheckWarnTitle = "Idle State Check Warning";
    public const string ReadyStateCheckFailureTitle = "Ready State Check Failure";
    public const string ReadyStateCheckWarnTitle = "Ready State Check Warning";
    public const string ChangeTankDialogTitle = "Change Tank Warning";
    public const string ChangeTankMsg = "Tank pressure measured low, consider change tank.";
    public const string AblationFailureMsg = "Ablation Test Failure";

    public const string StartPushButtonTestMessage = "Press and hold the Start pushbutton on the console, then press the OK button.";

    public const string StartPushButtonSuccessMessage = "Start Push Button Test Suceeded.\n";
    public const string StartPushButtonFailureMessage = "Start Push Button Test Failed.\n";

    public const string StopPushButtonTestMessage ="Release the Start pushbutton then press and hold the Stop pushbutton on the console, then press the OK button.";

    public const string StopPushButtonSuccessMessage = "Stop Push Button Test Succeeded.\n";
    public const string StopPushButtonFailureMessage = "Stop Push Button Test Failed.\n";

    public const string StartFootSwitchMessage = "Release the Stop pushbutton then press and hold the Start Foot Switch, then press the OK button.";
    public const string StartFootSwitchSuccessMessage = "Start Foot Switch Test Succeeded.\n";
    public const string StartFootSwitchFailureMessage = "Start Foot Switch Test Failed.\n";

    public const string StopFootSwitchMessage = "Release the Start Foot Switch then press and hold the Stop Foot Switch, then press the OK button.";
    public const string StopFootSwitchSuccessMessage = "Release the Stop Foot Switch, then press the Ok button to continue.\n";
    public const string StopFootSwitchFailureMessage = "Stop Foot Switch Test Failed.\n";

    public const string LEDsTestTitle = "LEDs Test";

    public const string LEDsParameters =
      "Verify that: \n\n(1) The Start pushbutton LED is flashing blue and green\n(2) The Stop pushbutton LED is on and white\n(3) The Warning LED is on and yellow\n(4) The Fault LED is on and red\n(5) The Power Ring LED is on and blue.";

    public const string StartLEDsTestMessage =
      "Verify the system LEDs on the Console are illuminated or flashing after clicked the OK button on this dialog:\n\n(1) Start LED\n(2) Stop LED\n(3) Warning LED\n(4) Fault LED";

    public const string ScreenTestTitle = "Screen Test";
    public const string ScreenParameters = "Verify the console Monitor is clear and displays from edge to edge.";
    public const string StartScreenTestMessage = "Verify the console screen is clear from edge to edge (no blurry).";

    public const string AudibleTitle = "Audible Test";
    public const string AudibleParameters = "Verify that the system speaker can be heard.";

    public const string StartAudibleTestMessage =
      "Notice that the console speaker sounds after clicked the OK button on this dialog.";

    public const string ConfirmationTitle = "Test Failure";

    public const string ConfirmationMessage = "The test step failed. Continue to the next step, retry this step, or stop the test?";

    public const string ReportFileTitle = "Field Service Tool Report";
    public const string ReportFileMessage = "The report file was successfully saved to the USB drive.";

    public const string DialogYesButtonTextKey = "YesButtonKey";
    public const string DialogNoButtonTextKey = "NoButtonKey";

    public const string ParamIdKey = "TestId";
    public const string FailText = "Fail";
    public const string PassText = "Pass";
    public const string OKText = "OK";
    public const string QuitText = "Quit";
    public const string ContinueText = "Continue";
    public const string YesText = "Yes";
    public const string CancelText = "Cancel";
    public const string NoText = "No";

    public const string RetryButtonTextKey = "RetryButtonTextKey";
    public const string RetryText = "Retry";
    public const string StopText = "Stop";
    public const string RetryRationaleTitle = "Retry Rationale";
    public static string NewLine = Environment.NewLine;

    public const string OKButtonText = "OK";
    public const string AtMsg = " at ";
    public const string RetryTitleVersionVerification = "Step 1 - Manual Tests - Version Verification";
    public const string RetryTitleLEDsTest = "Step 1 - Manual Tests - LEDs Test";
    public const string RetryTitleScreenTest = "Step 1 - Manual Tests - Screen Test";
    public const string RetryTitleAudibleTest = "Step 1 - Manual Tests - Audible Test";
    public const string RetryTitleStartPushButtonInputTest = "Step 1 - Manual Tests - Start Pushbutton Input Test";
    public const string RetryTitleStopPushButtonInputTest = "Step 1 - Manual Tests - Stop Pushbutton Input Test";
    public const string RetryTitleStartFootSwitchInputTest = "Step 1 - Manual Tests - Start Foot Switch Input Test";
    public const string RetryTitleStopFootSwitchInputTest = "Step 1 - Manual Tests - Stop Foot Switch Input Test";
    public const string RetryTitleIdleStateCheck = "Step 2 - Parameter Check - Idle State Check";
    public const string RetryTitleReadyStateCheck = "Step 2 - Parameter Check - Ready State Check";
    public const string RetryTitleAblationTest = "Step 3 - Performance Tests - Ablation Tests";

    public const string StartingTestMsg = "Starting test session ...";
    public const string StartTestMsg = "Tests started";

    public const string PausingTestMsg = "Pausing test session ...";
    public const string PauseTestMsg = "Tests paused";

    public const string ResumingTestMsg = "Resuming test session ...";
    public const string ResumeTestMsg = "Tests resumed";

    public const string StoppingTestMsg = "Stopping test session ...";
    public const string StopTestMsg = "Tests stopped";

    public const double OBPAdjustment = 0.1d;
    public const double BalloonPressureThreshold = 4.7d;
    public const double OBPFactor = 2.0d;
    public const string LessEqualText = " <= ";
    public const int RoundOneDigit = 1;
    public const int RoundTwoDigits = 2;
    public const int RoundThreeDigits = 3;
    public const string NAText = "N/A";

    public const string GeneralInfoText = "Summary Information";
    public const string GeneralInfoSheetTitle = "Summary Info";
    public const string IBPText = "IBP";
    public const string OBPText = "OBP";
    public const string PT2Text = "PT2";
    public const string FM1Text = "FM1";
    public const string TransitionTimeText = "Transition Time";
    public const string PWM2Text = "PWM2";
    public const string TS1Text = "TS1";
    public const string SpeedText = "Speed";
    public const string AblationDetailsText = "Ablation #";
    public const string IdleStateCheckDetailsText = "Idle";
    public const string ReadyStateCheckDetailsText = "Ready";
    public const string FlowMeterCheckWorksheet = "Flow Meter Check";
    public const string TimestampFormatString = "yyyy/MM/dd HH:mm:ss.fff";
    public const string AblationDetailsTitle = "Ablation Details Information";
    public const string IdleStateDetailsTitle = "Idle State Information";
    public const string ReadyStateDetailsTitle = "Ready State Information";
    public const string FlowMeterCheckTitle = "Flow Meter Check Data";
    public static readonly string[] FlowMeterCheckColumns = {"Index", "Timestamp", "Int. FM1", "Ext. FM1" };

    public const string HospitalNameDescription = "Hospital Name";
    public const string PatientIDDescription = "Patient ID";
    public const string ConsoleSerialDescription = "Console S/N";
    public const string TimeInBodyDescription = "In Body Time (min)";
    public const string ProcedureIDDescription = "Procedure ID";
    public const string GuiVersionDescription = "GUI Version";
    public const string DataBaseVersionDescription = "Database Version";
    public const string ServiceToolVersionDescription = "Service Tool Version";
    public const string CMCUVersionDescription = "CMCU Firmware";
    public const string CPLDVersionDescription = "CPLD Firmware";
    public const string PmcuVersionDescription = "PMCU Firmware";
    public const string RepeaterVersionDescription = "Repeater Firmware";
    public const string ICBVersionDescription = "ICB Firmware";
    public const string CatheterVersionDescription = "Catheter Firmware";
    public const string RemoteVersionDescription = "Remote Firmware";
    public const string CMCUBootLoaderDescription = "Control BootLoader Firmware";
    public const string RmcuBootVersionDescription = "Repeater BootLoader Firmware";
    public const string PmcuBootVersionDescription = "Patient BootLoader Firmware";
    public const string IcbBootVersionDescription = "ICB BootLoader Firmware";
    public const string RcmcuBootVersionDescription = "Remote BootLoader Firmware";
    public const string NullDescriptionString = "--";
    public const string TimestampDescription = "Timestamp";
    public const string TimeDescription = "Time (sec)";
    public const string AblationIdDescription = "Ablation ID";
    public const string SystemStateDescription = "System State";
    public const string TC1Description = "Balloon Temperature (°C)";
    public const string PT1Description = "Tank Pressure (psig)";
    public const string PT2Description = "Injection Pressure (psig)";
    public const string PT3Description = "Return Line Pressure (psia)";
    public const string PT4Description = "Vacuum Line Pressure (psia)";
    public const string PT5Description = "Scavenging Line Pressure (psia)";
    public const string FM1Description = "Flow (sccm)";
    public const string TS1Description = "Sub-Cooler Temperature (°C)";
    public const string LC1Description = "Tank Weight (lbs)";
    public const string IBPDescription = "Inner Balloon Pressure (psig)";
    public const string OBPDescription = "Outer Balloon Pressure (psig)";
    public const string IPWMDescription = "Injection PWM (%)";
    public const string BPWMDescription = "Balloon PWM (%)";

    public const string InflationOBPTestText = "Inflation #1";
    public const string InflationPT2Text = "Test Injection Pressure (PT2)";
    public const string InflationPT2ExpectedText = "120.0 <= PT2 <= 180.0";
    public const string InflationFM1Text = "Test Flow (FM1)";
    public const string InflationFM1Threshold = "600 <= FM1 <= 1200";
    public const string InflationIBPThreshold = "2.0 <= IBP <= 3.0";
    public const string InflationIBPDASBalloonThreshold = "7.0 <= IBP <= 8.0";
    public const string InflationSpeedText = "Test Inflation Speed";
    public const string InflationSpeedExpectedText1 = " < t < 5.00";
    public const string InflationSpeedExpectedText2 = "t <= 2.00";
    public const string CatheterIDDescription = "Catheter ID";
    public const string CatheterLotNumDescription = "Catheter Lot Number";
    public const string CatheterSNDescription = "Catheter Serial Number";
    public const string InflationSpeedDescription = "Inflation Speed";
    public const string TreatmentText = "Treatment #";
    public const string AblationFM1ThresholdText = "7500 <= FM1 <= 8100";
    public const string AblationFM1DASBalloonThresholdText = "8400 <= FM1 <= 9000";
    public const string AblationPT2ThresholdText = "350.0 <= PT2 <= 650.0";
    public const string AblationIBPThresholdText = "2.0 <= IBP <= 3.0";
    public const string AblationIBPDASBalloonThresholdText = "7.0 <= IBP <= 8.0";
    public const string AblationPWM2TextRule1 = "PWM2 <= 70.0";
    public const string AblationTS1ThresholdText = "<= -10";
    public const string TransitionTimeRangeText = "5.0 <= Time <= 20.0";
    public const string InflationTitle = "Inflation:";
    public const string AblationTitle = "Ablation:";
    public const string ExpectedValueTitle = "Expected Value";
    public const string ActualValueTitle = "Actual Value";
    public const string TimeOutTitle = "Console State Transition TimeOut";
    public const string ZeroDecimalPlace = "f0";
    public const string OneDecimalPlace = "f1";
    public const string TwoDecimalPlace = "f2";
    public const string ThreeDecimalPlace = "f3";

    public const string MaxTextLengthInTextBoxKey = "MaxTextLengthInTextBoxKey";
    public const string ErrorMessageInCreateExcelFile = "Failed to create excel file in Idle State";
    public const string ConnectCatheterMechanicallyMessage = "Please mechanically connect the catheter to the console using the Cryo-Cable.";

    public const string FlowMeterCheckMessageTitle = "Flow Meter Check";
    public const string SkipFlowMeterCheckId = "Skip " + FlowMeterCheckMessageTitle; 
    public const string FlowMeterSkipRationaleTitle = FlowMeterCheckMessageTitle + " Skip Rationale";
    public const string SkipFlowMeterCheckMessage = "Do you want to skip the Flow Meter Check?";
    public const string ConnectFlowMeterMessage = "Please connect the external flow meter to the console.";

    public const string FlowMeterCheckRetryOrSkipMessage = "Do you want to retry? \n" +
                                                           "\nPress \"Yes\" to retry. " +
                                                           "\nPress \"No\" to Skip Flow Meter Check.";
    
    public const string CouldNotDetectFlowMeterMessage = "Could not detect the external flow meter. Please check the connections. \n" +
                                                         FlowMeterCheckRetryOrSkipMessage;
 
    public const string FlowMeterTestExceedToleranceMessage = "The Flow Meter Check Failed. " +
                                                              "Please replace the console flow meter with a calibrated one.\n" +
                                                              "\nOnce the console flow meter change is complete, press the OK button to continue.";

    public const string AVG_OFFSET_KEY = @"{AVG_OFFSET_KEY}";

    public const string FlowMeterChangedSummaryMessage = @"The Flow Meter Check failed with a value of ({AVG_OFFSET_KEY}%) and was changed. The Flow Meter Check was then skipped due to the replacement.";

    public const string FlowMeterCommErrorMessage = "The communication to the external flow meter was lost during the ablation test. Please check the communication cable. \n " + FlowMeterCheckRetryOrSkipMessage;

    public const string DisconnectFlowMeterMessage = "Disconnect the external flow meter from the console (electrically and mechanically) " +
                                                     "and reconnect the console flow meter directly to the vacuum pump.\n" +
                                                     "\nOnce the internal flow meter has been reconnected, press the OK button to continue.";

    public const string WaterTemperatureTooLowMessage = "The balloon temperature is too low to start an ablation. Please increase the water bath temperature.\n";

    public const string WaterTemperatureTooHighMessage = "The balloon temperature is too high to start an ablation. Please decrease the water bath temperature.\n";

    public const string ContinueOrStopMessage = "\n Press OK to continue." + "\n Press Stop to stop the test.";

    public const string POLARxFITCatheterIsExpectedMessage = "This catheter cannot be used for this test. Please connect a POLARx FIT (ID 2 or 130) to continue.";
  }
}