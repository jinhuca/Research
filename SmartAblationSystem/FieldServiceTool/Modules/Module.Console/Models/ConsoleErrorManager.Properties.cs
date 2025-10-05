
using static Communication.CanBusMessageDefinition;

namespace Module.Console.Models
{
  public partial class ConsoleErrorManager
  {
    private const double SYSTEM_ERROR_RESET_TIMEOUT = 5;
    private const double CMCU_WARNING_RESET_TIMEOUT = 60;

    private volatile bool _isErrorMessageDialogShowing = false;
    private volatile bool _systemHasError = false;
    private volatile bool _systemHasWarning = false;
    private bool _isSystemRested;

    public bool IsSystemRested
    {
      get => _isSystemRested;
      set => SetProperty(ref _isSystemRested, value);
    }

    private long _cmcuAllErrorsFlag = (long)(CMCUStatusError.ExceptionType5 |
                                             CMCUStatusError.CPLDWatchDogTimerError |
                                             CMCUStatusError.TwoMultiplexReadingDoesNotMatch |
                                             CMCUStatusError.FlowTooHigh |
                                             CMCUStatusError.FlowTooLow |
                                             CMCUStatusError.FlowReadingOutOfRange |
                                             CMCUStatusError.LoadCellWeightFail |
                                             CMCUStatusError.LoadCellReadingOutOfRange |
                                             CMCUStatusError.PressurePT1InTankIsTooHigh |
                                             CMCUStatusError.PressurePT1InTankReadingOutOfRange |
                                             CMCUStatusError.PressurePT2AfterCatheterButBeforeReturnLineTooHigh |
                                             CMCUStatusError.PT2ReadingOutOfRange |
                                             CMCUStatusError.ReturnPressurePT3TooHigh |
                                             CMCUStatusError.ReturnPressurePT3OutOfRange |
                                             CMCUStatusError.VacuumPressurePT4TooHigh |
                                             CMCUStatusError.VacuumPressurePT4OutOfRange |
                                             CMCUStatusError.SubCoolerTemperatureOutOfRange |
                                             CMCUStatusError.InjectionVentPressureIsHigh |
                                             CMCUStatusError.ScavengingPressureIsHigh |
                                             CMCUStatusError.SelfTestFail
                                             );

    private long _cmcuAllWarningFlag = (long)(CMCUStatusError.LoadCellWeightWarning |
                                              CMCUStatusError.PressureInTankIsHighFanToBeOn |
                                              CMCUStatusError.PressurePT1InTankIsLow |
                                              CMCUStatusError.SubCoolerTemperatureIsHigh |
                                              CMCUStatusError.InjectionVentPressureOutOfRange
                                              );

    private long _pmcuAllErrors = (long)(PMCUStatusError.CPLDWatchDogTimerError |
                                         PMCUStatusError.SelfTestFail |
                                         PMCUStatusError.InnerBalloonPressureTooHigh |
                                         PMCUStatusError.InnerBalloonPressureTooLow |
                                         PMCUStatusError.OuterBalloonPressureTooHigh |
                                         PMCUStatusError.OuterBalloonPressureReadingOutOrRange |
                                         PMCUStatusError.BalloonTipPressureTooHigh |
                                         PMCUStatusError.BalloonTipPressureTooLow |
                                         PMCUStatusError.BalloonTipPressurePeadingOutOfRange |
                                         PMCUStatusError.BalloonTemperatureTooHigh |
                                         PMCUStatusError.ThawingTemperatureTooHigh |
                                         PMCUStatusError.ThawingTemperatureTooLow |
                                         PMCUStatusError.BloodDetectedInCatheter |
                                         PMCUStatusError.BloodDetectorOpenWires
                                         );

    #region CMCU Error Status Properties
    private bool _isCMCUExceptionType5;
    public bool IsCMCUExceptionType5
    {
      get => _isCMCUExceptionType5;
      set => SetProperty(ref _isCMCUExceptionType5, value);
    }

    private bool _isVacuumDisconnected;
    public bool IsVacuumDisconnected
    {
      get => _isVacuumDisconnected;
      set => SetProperty(ref _isVacuumDisconnected, value, nameof(IsVacuumDisconnected));
    }

    private bool _isCPLDWatchdogError;
    public bool IsCPLDWatchdogError
    {
      get => _isCPLDWatchdogError;
      set => SetProperty(ref _isCPLDWatchdogError, value);
    }

    private bool _isCMCUTwoMultiplexReadingsDoNotMatch;
    public bool IsCMCUTwoMultiplexReadingsDoNotMatch
    {
      get => _isCMCUTwoMultiplexReadingsDoNotMatch;
      set => SetProperty(ref _isCMCUTwoMultiplexReadingsDoNotMatch, value);
    }

    private bool _isCMCUFlowTooHigh;
    public bool IsCMCUFlowTooHigh
    {
      get => _isCMCUFlowTooHigh;
      set => SetProperty(ref _isCMCUFlowTooHigh, value);
    }

    private bool _isCMCUFlowTooLow;
    public bool IsCMCUFlowTooLow
    {
      get => _isCMCUFlowTooLow;
      set => SetProperty(ref _isCMCUFlowTooLow, value);
    }

    private bool _isCMCUFlowReadingOutOfRange;
    public bool IsCMCUFlowReadingOutOfRange
    {
      get => _isCMCUFlowReadingOutOfRange;
      set => SetProperty(ref _isCMCUFlowReadingOutOfRange, value);
    }

    private bool _isCMCULoadCellWeightFail;
    public bool IsCMCULoadCellWeightFail
    {
      get => _isCMCULoadCellWeightFail;
      set => SetProperty(ref _isCMCULoadCellWeightFail, value);
    }

    private bool _isCMCULoadCellReadingOutOfRange;
    public bool IsCMCULoadCellReadingOutOfRange
    {
      get => _isCMCULoadCellReadingOutOfRange;
      set => SetProperty(ref _isCMCULoadCellReadingOutOfRange, value);
    }

    private bool _isCMCUPressurePT1InTankIsTooHigh;
    public bool IsCMCUPressurePT1InTankIsTooHigh
    {
      get => _isCMCUPressurePT1InTankIsTooHigh;
      set => SetProperty(ref _isCMCUPressurePT1InTankIsTooHigh, value);
    }

    private bool _isCMCUPressurePT1InTankReadingOutOfRange;
    public bool IsCMCUPressurePT1InTankReadingOutOfRange
    {
      get => _isCMCUPressurePT1InTankReadingOutOfRange;
      set => SetProperty(ref _isCMCUPressurePT1InTankReadingOutOfRange, value);
    }

    private bool _isCMCUPressurePT2AfterCatheterButBeforeReturnLineTooHigh;
    private bool IsCMCUPressurePT2AfterCatheterButBeforeReturnLineTooHigh
    {
      get => _isCMCUPressurePT2AfterCatheterButBeforeReturnLineTooHigh;
      set => SetProperty(ref _isCMCUPressurePT2AfterCatheterButBeforeReturnLineTooHigh, value);
    }

    private bool _isCMCUPT2ReadingOutOfRange;
    public bool IsCMCUPT2ReadingOutOfRange
    {
      get => _isCMCUPT2ReadingOutOfRange;
      set => SetProperty(ref _isCMCUPT2ReadingOutOfRange, value);
    }

    private bool _isCMCUReturnPressurePT3TooHigh;
    public bool IsCMCUReturnPressurePT3TooHigh
    {
      get => _isCMCUReturnPressurePT3TooHigh;
      set => SetProperty(ref _isCMCUReturnPressurePT3TooHigh, value);
    }

    private bool _isCMCUReturnPressurePT3OutOfRange;
    public bool IsCMCUReturnPressurePT3OutOfRange
    {
      get => _isCMCUReturnPressurePT3OutOfRange;
      set => SetProperty(ref _isCMCUReturnPressurePT3OutOfRange, value);
    }

    private bool _isCMCUVacuumPressurePT4TooHigh;
    public bool IsCMCUVacuumPressurePT4TooHigh
    {
      get => _isCMCUVacuumPressurePT4TooHigh;
      set => SetProperty(ref _isCMCUVacuumPressurePT4TooHigh, value);
    }

    private bool _isCMCUVacuumPressurePT4OutOfRange;
    public bool IsCMCUVacuumPressurePT4OutOfRange
    {
      get => _isCMCUVacuumPressurePT4OutOfRange;
      set => SetProperty(ref _isCMCUVacuumPressurePT4OutOfRange, value);
    }

    private bool _isCMCUSubCoolerTemperatureOutOfRange;
    public bool IsCMCUSubCoolerTemperatureOutOfRange
    {
      get => _isCMCUSubCoolerTemperatureOutOfRange;
      set => SetProperty(ref _isCMCUSubCoolerTemperatureOutOfRange, value);
    }

    private bool _isCMCUInjectionVentPressureIsHigh;
    public bool IsCMCUInjectionVentPressureIsHigh
    {
      get => _isCMCUInjectionVentPressureIsHigh;
      set => SetProperty(ref _isCMCUInjectionVentPressureIsHigh, value);
    }

    private bool _isCMCUInjectionVentPressureOutOfRange;
    public bool IsCMCUInjectionVentPressureOutOfRange
    {
      get => _isCMCUInjectionVentPressureOutOfRange;
      set => SetProperty(ref _isCMCUInjectionVentPressureOutOfRange, value);
    }

    private bool _isCMCUScavengingPressureIsHigh;
    public bool IsCMCUScavengingPressureIsHigh
    {
      get => _isCMCUScavengingPressureIsHigh;
      set => SetProperty(ref _isCMCUScavengingPressureIsHigh, value);
    }

    private bool _isCMCUSelfTestFail;
    public bool IsCMCUSelfTestFail
    {
      get => _isCMCUSelfTestFail;
      set => SetProperty(ref _isCMCUSelfTestFail, value);
    }

    private bool _isCMCULoadCellWeightWarning;
    public bool IsCMCULoadCellWeightWarning
    {
      get => _isCMCULoadCellWeightWarning;
      set => SetProperty(ref _isCMCULoadCellWeightWarning, value);
    }

    private bool _isCMCUPressureInTankIsHighFanToBeOn;
    public bool IsCMCUPressureInTankIsHighFanToBeOn
    {
      get => _isCMCUPressureInTankIsHighFanToBeOn;
      set => SetProperty(ref _isCMCUPressureInTankIsHighFanToBeOn, value);
    }

    private bool _isCMCUPressurePT1InTankIsLow;

    public bool IsCMCUPressurePT1InTankIsLow
    {
      get => _isCMCUPressurePT1InTankIsLow;
      set => SetProperty(ref _isCMCUPressurePT1InTankIsLow, value);
    }

    private bool _isCMCUSubCoolerTemperatureIsHigh;
    public bool IsCMCUSubCoolerTemperatureIsHigh
    {
      get => _isCMCUSubCoolerTemperatureIsHigh;
      set => SetProperty(ref _isCMCUSubCoolerTemperatureIsHigh, value);
    }

    #endregion  CMCU Error Status Properties

    #region PMCU Error Status Properties

    private bool _isPMCUCPLDWatchDogTimerError;
    public bool IsPMCUCPLDWatchDogTimerError
    {
      get => _isPMCUCPLDWatchDogTimerError;
      set => SetProperty(ref _isPMCUCPLDWatchDogTimerError, value);
    }

    private bool _isInnerBalloonPressureTooHigh;
    public bool IsInnerBalloonPressureTooHigh
    {
      get => _isInnerBalloonPressureTooHigh;
      set => SetProperty(ref _isInnerBalloonPressureTooHigh, value);
    }

    private bool _isOuterBalloonPressureTooHigh;
    public bool IsOuterBalloonPressureTooHigh
    {
      get => _isOuterBalloonPressureTooHigh;
      set => SetProperty(ref _isOuterBalloonPressureTooHigh, value);
    }

    private bool _isOuterBalloonPressureReadingOutOfRange;
    public bool IsOuterBalloonPressureReadingReadingOutOfRange
    {
      get => _isOuterBalloonPressureReadingOutOfRange;
      set => SetProperty(ref _isOuterBalloonPressureReadingOutOfRange, value);
    }

    private bool _isBalloonTipPressureTooHigh;
    public bool IsBalloonTipPressureTooHigh
    {
      get => _isBalloonTipPressureTooHigh;
      set => SetProperty(ref _isBalloonTipPressureTooHigh, value);
    }

    private bool _isBalloonTipPressureTooLow;
    public bool IsBalloonTipPressureTooLow
    {
      get => _isBalloonTipPressureTooLow;
      set => SetProperty(ref _isBalloonTipPressureTooLow, value);
    }

    private bool _isBalloonTipPressureReadingOutOfRange;
    public bool IsBalloonTipPressureReadingOutOfRange
    {
      get => _isBalloonTipPressureReadingOutOfRange;
      set => SetProperty(ref _isBalloonTipPressureReadingOutOfRange, value);
    }

    private bool _isThawingTemperatureTooHigh;
    public bool IsThawingTemperatureTooHigh
    {
      get => _isThawingTemperatureTooHigh;
      set => SetProperty(ref _isThawingTemperatureTooHigh, value);
    }

    private bool _isThawingTemperatureTooLow;
    public bool IsThawingTemperatureTooLow
    {
      get => _isThawingTemperatureTooLow;
      set => SetProperty(ref _isThawingTemperatureTooLow, value);
    }

    private bool _isPMCUSelfTestFail;
    public bool IsPMCUSelfTestFail
    {
      get => _isPMCUSelfTestFail;
      set => SetProperty(ref _isPMCUSelfTestFail, value);
    }

    private bool _isInnerBalloonPressureTooLow;
    public bool IsInnerBalloonPressureTooLow
    {
      get => _isInnerBalloonPressureTooLow;
      set => SetProperty(ref _isInnerBalloonPressureTooLow, value);
    }

    private bool _isBloodDetected;
    public bool IsBloodDetected
    {
      get => _isBloodDetected;
      set => SetProperty(ref _isBloodDetected, value);
    }

    private bool _isBloodDetectorOpenWires;
    public bool IsBloodDetectorOpenWires
    {
      get => _isBloodDetectorOpenWires;
      set => SetProperty(ref _isBloodDetectorOpenWires, value);
    }

    private bool _isBalloonTemperatureTooHigh;
    public bool IsBalloonTemperatureTooHigh
    {
      get => _isBalloonTemperatureTooHigh;
      set => SetProperty(ref _isBalloonTemperatureTooHigh, value);
    }

    #endregion PMCU Error Status Properties

  }
}
