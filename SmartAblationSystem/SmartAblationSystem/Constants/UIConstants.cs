namespace SmartAblationSystem
{
	internal static class UIConstants
	{
		public const string UnknownState = "UNKNOWN";
		public const string ExceptionState = "EXCEPTION";
		public const string IdleState = "IDLE";
		public const string ReadyState = "READY";
		public const string InflationState = "INFLATION";
		public const string TransitionState = "TRANSITION";
		public const string AblationState = "ABLATION";
		public const string ThawingState = "THAWING";
		public const string PlayBack = "PlayBack";

    public const string RegularUser = "USER";
    public const string DoctorUser = "DOCTOR";
    public const string AdminUser = "ADMIN";
    public const string BSCUser = "BSC";
    public const string BSCADMINUser = "BSCADMIN";
    public const string TempName = "temp";

    public const string BSCDataTemplate = "BSCDataTemplate_";
    public const string DoctorDataTemplate = "DoctorDataTemplate_";

    public const string SimpleBSCDataTemplate = "SimpleBSCDataTemplate_";
    public const string SimpleDoctorDataTemplate = "SimpleDoctorDataTemplate_";

    public const string NormalView = "Full View";
    public const string SimpleView = "Simple View";

    public const double OpacityDisabled = 0.3;
		public const double OpacityEnabled = 1.0;

    public const string UserNameEmptyErrorMessage = "User name cannot be empty.";
    public const string DrFirstNameEmptyErrorMessage = "First name cannot be empty.";
    public const string DrLastNameEmptyErrorMessage = "Last name cannot be empty.";
    public const string PasswordEmptyErrorMessage = "Password cannot be empty.";

    public const string UserNameInvalidMessage =
      "Alphanumeric, space, underscore, hyphen, comma only.";

    public const string DrFirstNameInvalidMessage =
      "Alphanumeric, space, underscore, hyphen, comma only.";

    public const string DrLastNameInvalidMessage =
      "Alphanumeric, space, underscore, hyphen, comma only.";

    public const string PasswordNotMatchMessage = "Passwords don't match!";
    public const string PasswordInvalidMessage = "Must be 8 to 16 characters with at least 1 digit.";

    public const string PatientFirstName = "First Name";
    public const string PatientLastName = "Last Name";
    public const string PatientGender = "Gender";
    public const string PatientBirthDate = "Date of Birth";
    public const string PatientHeight = "Height ";
    public const string PatientWeight = "Weight ";
    public const string PatientBMI = "BMI";

    public const string DoubleDash = "--";
  }
}
