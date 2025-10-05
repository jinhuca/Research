using System;

namespace Module.CatheterTestTool.Models
{
  public static class CatheterTestConstants
  {
    public static readonly string CATHETER_TEST_TOOL_TITLE = "POLARx Test Tool";
    public const string TurnOffTitleValue = "Shut Down";
    public const string TurnOffMessageValue = "Are you sure you want to close the Catheter Test Tool?";

    public const string USB_DRIVE_NAME_PARAM = "USBDriveName";
    public const string TEST_RESULT_KEY = "TestResultKey";

    public const string CATHETER_TEST_SESSION_COMPLETED = "The current test session is completed.";
    public const string CATHETER_TEST_SESSION_STOPPED = "User stopped the current test session.";

    public const string POPUP_DIALOG_TITLE_KEY = "PopupDialogTitle";
    public const string POPUP_DIALOG_MESSAGE_KEY = "PopupDialogMessage";
    public const string POPUP_DIALOG_OKBUTTON_TEXT_KEY = "PopupDialogOkButtonText";
    public const string POPUP_DIALOG_CANCELBUTTON_TEXT_KEY = "PopupDialogCancelButtonText";
    public const string POPUP_DIALOG_ISPASSFAIL_DIALOG_KEY = "PopupDialogIsPassFailDialog";
    public const string POPUP_DIALOG_CONTINUE_BUTTON_TEXT = "Continue";
    public const string POPUP_DIALOG_CANCEL_BUTTON_TEXT = "Cancel";

    public const string CATHETER_VISUAL_CHECK_TITLE = "Catheter Visual Check";
    public const string CATHETER_VISUAL_CHECK_MESSAGE = "Perform Catheter Balloon Visual Check.\n\n" +
                                                        "Press 'Pass' to continue test.\n" +
                                                        "Press 'Fail' to stop the test. ";

    public const string CONTINUE_CATHETER_TEST_MESSAGE = "Please put catheter into water tank.\n\n" +
                                                        "Press 'Continue' to start Catheter Ablation Run test.\n" +
                                                        "Press 'Cancel' to stop the test. ";

    public const double MINIMUM_WATER_TEMPERATURE_REQUIRED = 30.0;
    public const int IDLE_TO_READY_STATE_TIMEOUT_IN_SEC = 10000;
    public const int READY_TO_INFLATION_STATE_TIMEOUT_IN_SEC = 30000;
    public const int READY_TO_ABLATION_STATE_TIMEOUT_IN_SEC = 40000;
    public const int IBP_STABLIZATION_TIME_IN_SEC = 5;
    public const int IBP_STABLIZATION_TIMEOUT_IN_SEC = 25;
    public const int RECORDING_TEST_DATA_TIME_IN_SEC = 10;
#if DEBUG
    public const int ABLATION_TIME = 30;
#else
        public const int ABLATION_TIME = 70;
#endif

    public const string SensorNameTC1 = "TEMP";
    public const string SensorNameIBP = "IBP";
    public const string SensorNameOBP = "OBP";
    public const string SensorNamePT2 = "PT2";
    public const string SensorNamePT3 = "PT3";
    public const string SensorNamePT4 = "PT4";
    public const string SensorNameFM1 = "FM1";

    public const string STATE_UNKNOWN = "UNKNOWN";
    public const string STATE_IDLE = "IDLE";
    public const string STATE_READY = "READY";
    public const string STATE_INFLATION = "INFLATION";
    public const string STATE_ABLATION = "ABLATION";
    public const string STATE_TRANSITION = "TRANSITION";
    public const string STATE_THAWING = "THAWING";
    public const string STATE_EXCEPTION = "EXCEPTION";

    public const string SMART_FREEZE_APP_PATH = "SmartFreezeAppPath";
    public const string SMART_FREEZE_FILE_NAME = "SmartFreezeAppFileName";

    public static bool AreDoubleValuesEqual(this double initialValue, double value, double bias)
    {
      return Math.Abs(initialValue - value) <= bias;
    }
  }
}
