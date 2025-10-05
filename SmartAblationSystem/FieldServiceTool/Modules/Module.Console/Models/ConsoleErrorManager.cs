using DataAccessLayer;
using Module.Console.Helpers;
using Module.Console.Interfaces;
using Module.Infrastructure.Constants;
using Module.Infrastructure.Controls;
using Module.Infrastructure.PubSubEvents;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using static Communication.CanBusMessageDefinition;

namespace Module.Console.Models
{
	public partial class ConsoleErrorManager : BindableBase
	{
		private readonly IMachineModel _machineModel;
		private readonly ICacheableDataAccess _dataAccesser;
		private readonly IEventAggregator _eventAggregator;
		private readonly IDialogService _dialogService;
    private bool _ignoreCmcuExceptionType5 = false;

		public ConsoleErrorManager(IMachineModel machineModel, ICacheableDataAccess dataAccesse, IEventAggregator eventAggregator, IDialogService dialogService)
		{
			_machineModel = machineModel;
			_dataAccesser = dataAccesse;
			_eventAggregator = eventAggregator;
			_dialogService = dialogService;

			_machineModel.PropertyChanged += HandleMachineErrorStatus;
      _eventAggregator.GetEvent<UserCommandEvent>().Subscribe(OnReceiveUserCommand); 
    }

		private void HandleMachineErrorStatus(object sender, PropertyChangedEventArgs args)
		{
			// only process CMCUStatusCode and PMCUStatusCode property changes 
			switch(args.PropertyName)
			{
				case nameof(_machineModel.CMCUSystemStatusError):
					_ = ProcessCMCUErrorStatusCodeAsync(_machineModel.CMCUSystemStatusError);
					break;
				case nameof(_machineModel.PMCUSystemStatusErrorCode):
					_ = ProcessPMCUErrorStatusCodeAsync(_machineModel.PMCUSystemStatusErrorCode);
					break;
				default:
					return;
			}
		}

		private async Task ProcessCMCUErrorStatusCodeAsync(long cmcuStatusCode)
		{
			await Task.Run(() => ProcessCMCUErrorStatusCode(cmcuStatusCode));
		}

		private async Task ProcessPMCUErrorStatusCodeAsync(long pmcuStatusCode)
		{
			await Task.Run(() => ProcessPMCUErrorStatusCode(pmcuStatusCode));
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private void ProcessCMCUErrorStatusCode(long cmcuStatusCode)
		{
			var errorList = new List<ErrorMessageExtender>();

			// Exception5
			var isCMCUExceptionType5 = (cmcuStatusCode & (long)CMCUStatusError.ExceptionType5) == (long)CMCUStatusError.ExceptionType5;

			if (!_ignoreCmcuExceptionType5)
      {
        IsCMCUExceptionType5 = CheckAndUpdateCmcuErrorList(errorList, cmcuStatusCode, IsCMCUExceptionType5,
          CMCUStatusError.ExceptionType5, MessageCategory.Error, false, Enumeration.ErrorTypes.PMCU);
      }
      else if (isCMCUExceptionType5)
      {
        return;
      }

      // CPLD Watchdog 
			IsCPLDWatchdogError = CheckAndUpdateCmcuErrorList(errorList, cmcuStatusCode, IsCPLDWatchdogError,
				CMCUStatusError.CPLDWatchDogTimerError);

			// Two Multiplexer Readings Do Not Match 
			IsCMCUTwoMultiplexReadingsDoNotMatch = CheckAndUpdateCmcuErrorList(errorList, cmcuStatusCode, IsCMCUTwoMultiplexReadingsDoNotMatch,
				CMCUStatusError.TwoMultiplexReadingDoesNotMatch);

			// Flow is too high
			IsCMCUFlowTooHigh = CheckAndUpdateCmcuErrorList(errorList, cmcuStatusCode, IsCMCUFlowTooHigh,
				CMCUStatusError.FlowTooHigh);

			// Flow is too low
			IsCMCUFlowTooLow = CheckAndUpdateCmcuErrorList(errorList, cmcuStatusCode, IsCMCUFlowTooLow,
				CMCUStatusError.FlowTooLow);

			// Flow Reading Out Of Range 
			IsCMCUFlowReadingOutOfRange = CheckAndUpdateCmcuErrorList(errorList, cmcuStatusCode, IsCMCUFlowReadingOutOfRange,
				CMCUStatusError.FlowReadingOutOfRange);

			// Load Cell Weight Fail 
			IsCMCULoadCellWeightFail = CheckAndUpdateCmcuErrorList(errorList, cmcuStatusCode, IsCMCULoadCellWeightFail,
				CMCUStatusError.LoadCellWeightFail);

			// Load Cell Reading Out Of Range
			IsCMCULoadCellReadingOutOfRange = CheckAndUpdateCmcuErrorList(errorList, cmcuStatusCode, IsCMCULoadCellReadingOutOfRange,
				 CMCUStatusError.LoadCellReadingOutOfRange);

			// Pressure PT1 In Tank Is Too High
			IsCMCUPressurePT1InTankIsTooHigh = CheckAndUpdateCmcuErrorList(errorList, cmcuStatusCode, IsCMCUPressurePT1InTankIsTooHigh,
				CMCUStatusError.PressurePT1InTankIsTooHigh);

			// Pressure PT1 In Tank Reading Out Of Range
			IsCMCUPressurePT1InTankReadingOutOfRange = CheckAndUpdateCmcuErrorList(errorList, cmcuStatusCode,
				IsCMCUPressurePT1InTankReadingOutOfRange, CMCUStatusError.PressurePT1InTankReadingOutOfRange);

			//  Pressure PT2 After Catheter But Before Return Line Too High
			IsCMCUPressurePT2AfterCatheterButBeforeReturnLineTooHigh = CheckAndUpdateCmcuErrorList(errorList, cmcuStatusCode,
				IsCMCUPressurePT2AfterCatheterButBeforeReturnLineTooHigh, CMCUStatusError.PressurePT2AfterCatheterButBeforeReturnLineTooHigh);

			// PT2 Reading Out Of Range
			IsCMCUPT2ReadingOutOfRange = CheckAndUpdateCmcuErrorList(errorList, cmcuStatusCode, IsCMCUPT2ReadingOutOfRange,
				CMCUStatusError.PT2ReadingOutOfRange);

			// Return Pressure PT3 Too High
			IsCMCUReturnPressurePT3TooHigh = CheckAndUpdateCmcuErrorList(errorList, cmcuStatusCode,
				IsCMCUReturnPressurePT3TooHigh, CMCUStatusError.ReturnPressurePT3TooHigh);

			// Return Pressure PT3 Out Of Range
			IsCMCUReturnPressurePT3OutOfRange = CheckAndUpdateCmcuErrorList(errorList, cmcuStatusCode,
				IsCMCUReturnPressurePT3OutOfRange, CMCUStatusError.ReturnPressurePT3OutOfRange);

			// Vacuum Pressure PT4 Too High
			IsCMCUVacuumPressurePT4TooHigh = CheckAndUpdateCmcuErrorList(errorList, cmcuStatusCode,
				IsCMCUVacuumPressurePT4TooHigh, CMCUStatusError.VacuumPressurePT4TooHigh);

			// Vacuum Pressure PT4 Out Of Range
			IsCMCUVacuumPressurePT4OutOfRange = CheckAndUpdateCmcuErrorList(errorList, cmcuStatusCode,
				IsCMCUVacuumPressurePT4OutOfRange, CMCUStatusError.VacuumPressurePT4OutOfRange);

			// SubCooler Temperature Out Of Range
			IsCMCUSubCoolerTemperatureOutOfRange = CheckAndUpdateCmcuErrorList(errorList, cmcuStatusCode,
				IsCMCUSubCoolerTemperatureOutOfRange, CMCUStatusError.SubCoolerTemperatureOutOfRange);

			// Injection Vent Pressure Is High
			IsCMCUInjectionVentPressureIsHigh = CheckAndUpdateCmcuErrorList(errorList, cmcuStatusCode,
				IsCMCUInjectionVentPressureIsHigh, CMCUStatusError.InjectionVentPressureIsHigh);

			// Scavenging Pressure Is High
			IsCMCUScavengingPressureIsHigh = CheckAndUpdateCmcuErrorList(errorList, cmcuStatusCode,
				IsCMCUScavengingPressureIsHigh, CMCUStatusError.ScavengingPressureIsHigh);

			// CMCU Self Test Fail
			IsCMCUSelfTestFail = CheckAndUpdateCmcuErrorList(errorList, cmcuStatusCode, IsCMCUSelfTestFail,
				CMCUStatusError.SelfTestFail);

			#region Warnings
			var warningList = new List<ErrorMessageExtender>();
			_systemHasError = _systemHasError || errorList.Any();

			bool forceAddWarnings = _systemHasError;
			// Injection Vent Pressure Out Of Range
			IsCMCUInjectionVentPressureOutOfRange = CheckAndUpdateCmcuErrorList(errorList, cmcuStatusCode,
				IsCMCUInjectionVentPressureOutOfRange, CMCUStatusError.InjectionVentPressureOutOfRange, MessageCategory.Warning, forceAddWarnings);


			IsCMCULoadCellWeightWarning = CheckAndUpdateCmcuErrorList(warningList, cmcuStatusCode, IsCMCULoadCellWeightWarning,
				CMCUStatusError.LoadCellWeightWarning, MessageCategory.Warning, forceAddWarnings);

			IsCMCUPressureInTankIsHighFanToBeOn = CheckAndUpdateCmcuErrorList(warningList, cmcuStatusCode,
				IsCMCUPressureInTankIsHighFanToBeOn, CMCUStatusError.PressureInTankIsHighFanToBeOn, MessageCategory.Warning, forceAddWarnings);

			IsCMCUPressurePT1InTankIsLow = CheckAndUpdateCmcuErrorList(warningList, cmcuStatusCode,
				IsCMCUPressurePT1InTankIsLow, CMCUStatusError.PressurePT1InTankIsLow, MessageCategory.Warning, forceAddWarnings);

			IsCMCUSubCoolerTemperatureIsHigh = CheckAndUpdateCmcuErrorList(warningList, cmcuStatusCode,
				IsCMCUSubCoolerTemperatureIsHigh, CMCUStatusError.SubCoolerTemperatureIsHigh, MessageCategory.Warning, forceAddWarnings);

			#endregion Warnings

			// Has errors or warnings, display ErrorMessage Dialog
			if(errorList.Any() || warningList.Any())
			{
				_systemHasError = _systemHasError || errorList.Any();
				_systemHasWarning = _systemHasWarning || warningList.Any();
				errorList.AddRange(warningList);

				_eventAggregator.GetEvent<ErrorListUpdateEvent>().Publish(errorList);

				if(!_isErrorMessageDialogShowing)
				{
					DisplayErrorMessageDialog(errorList);
				}
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private void ProcessPMCUErrorStatusCode(long pmcuStatusCode)
		{
			var errorList = new List<ErrorMessageExtender>();

			IsPMCUCPLDWatchDogTimerError = CheckAndUpdatePmcuErrorList(errorList, pmcuStatusCode,
				IsPMCUCPLDWatchDogTimerError, PMCUStatusError.CPLDWatchDogTimerError);

			IsPMCUSelfTestFail = CheckAndUpdatePmcuErrorList(errorList, pmcuStatusCode, IsPMCUSelfTestFail,
				PMCUStatusError.SelfTestFail);

			IsInnerBalloonPressureTooHigh = CheckAndUpdatePmcuErrorList(errorList, pmcuStatusCode,
				IsInnerBalloonPressureTooHigh, PMCUStatusError.InnerBalloonPressureTooHigh);

			IsInnerBalloonPressureTooLow = CheckAndUpdatePmcuErrorList(errorList, pmcuStatusCode,
				IsInnerBalloonPressureTooLow, PMCUStatusError.InnerBalloonPressureTooLow);

			IsOuterBalloonPressureTooHigh = CheckAndUpdatePmcuErrorList(errorList, pmcuStatusCode,
				IsOuterBalloonPressureTooHigh, PMCUStatusError.OuterBalloonPressureTooHigh);

			IsOuterBalloonPressureReadingReadingOutOfRange = CheckAndUpdatePmcuErrorList(errorList, pmcuStatusCode,
				IsOuterBalloonPressureReadingReadingOutOfRange, PMCUStatusError.OuterBalloonPressureReadingOutOrRange);

			IsBalloonTipPressureTooHigh = CheckAndUpdatePmcuErrorList(errorList, pmcuStatusCode, IsBalloonTipPressureTooHigh,
				PMCUStatusError.BalloonTipPressureTooHigh);

			IsBalloonTipPressureTooLow = CheckAndUpdatePmcuErrorList(errorList, pmcuStatusCode, IsBalloonTipPressureTooLow,
				PMCUStatusError.BalloonTipPressureTooLow);

			IsBalloonTipPressureReadingOutOfRange = CheckAndUpdatePmcuErrorList(errorList, pmcuStatusCode,
				IsBalloonTipPressureReadingOutOfRange, PMCUStatusError.BalloonTipPressurePeadingOutOfRange);

			IsBalloonTemperatureTooHigh = CheckAndUpdatePmcuErrorList(errorList, pmcuStatusCode, IsBalloonTemperatureTooHigh,
				PMCUStatusError.BalloonTemperatureTooHigh);

			IsThawingTemperatureTooHigh = CheckAndUpdatePmcuErrorList(errorList, pmcuStatusCode, IsThawingTemperatureTooHigh,
				PMCUStatusError.ThawingTemperatureTooHigh);

			IsThawingTemperatureTooLow = CheckAndUpdatePmcuErrorList(errorList, pmcuStatusCode, IsThawingTemperatureTooLow,
				PMCUStatusError.ThawingTemperatureTooLow);


			IsBloodDetected = CheckAndUpdatePmcuErrorList(errorList, pmcuStatusCode, IsBloodDetected,
				PMCUStatusError.BloodDetectedInCatheter);

			IsBloodDetectorOpenWires = CheckAndUpdatePmcuErrorList(errorList, pmcuStatusCode, IsBloodDetectorOpenWires,
				PMCUStatusError.BloodDetectorOpenWires);

			// Has errors or warnings
			if(errorList.Any())
			{
				_systemHasError = true;
				_eventAggregator.GetEvent<ErrorListUpdateEvent>().Publish(errorList);
				if(!_isErrorMessageDialogShowing)
				{
					DisplayErrorMessageDialog(errorList);
				}
			}
		}

		private bool CheckAndUpdatePmcuErrorList(List<ErrorMessageExtender> errorList,
			long pmcuStatusCode, bool currentErrorStatus,
			PMCUStatusError statusErrorMask,
			MessageCategory category = MessageCategory.Error,
			bool forceAddInErrorList = false,
			Enumeration.ErrorTypes errorType = Enumeration.ErrorTypes.PMCU)
		{
			return CheckAndUpdateErrorList(errorList, pmcuStatusCode, currentErrorStatus, (long)statusErrorMask, category, forceAddInErrorList, errorType);
		}

		private bool CheckAndUpdateCmcuErrorList(List<ErrorMessageExtender> errorList,
			long cmcuStatusCode, bool currentErrorStatus,
			CMCUStatusError statusErrorMask,
			MessageCategory category = MessageCategory.Error,
			bool forceAddInErrorList = false,
			Enumeration.ErrorTypes errorType = Enumeration.ErrorTypes.CMCU)
		{
			return CheckAndUpdateErrorList(errorList, cmcuStatusCode, currentErrorStatus, (long)statusErrorMask, category, forceAddInErrorList, errorType);
		}

		private bool CheckAndUpdateErrorList(List<ErrorMessageExtender> errorList,
			long statusCode, bool currentErrorStatus,
			long statusErrorMask,
			MessageCategory messageCategory,
			bool forceAddInErrorList,
			Enumeration.ErrorTypes errorType)
		{
			bool newErrorStatus = (statusCode & statusErrorMask) == statusErrorMask;
			if((!currentErrorStatus || forceAddInErrorList) && newErrorStatus)
			{
				errorList.Add(new ErrorMessageExtender(_dataAccesser.GetErrorMessageWithErrorTypeById((int)statusErrorMask, (int)errorType), messageCategory));
			}

			return newErrorStatus;
		}

		private void DisplayErrorMessageDialog(IList<ErrorMessageExtender> errorList)
		{
			var parameters = new DialogParameters
			{
				{ Strings.ErrorListParameterKey, errorList },
				{ Strings.CurrentVolumeParameterKey, 50 },
				{ Strings.UpdateVolumeActionParameterKey, (Action<int>)UpdateVolume }
			};

			_isErrorMessageDialogShowing = true;

			Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.SystemIdle,
				(ThreadStart)(() =>
				{
					_dialogService.Show(nameof(ErrorMessageDialog), parameters, HandleErrorMessageDialogClosed);
				}));
		}

		private void HandleErrorMessageDialogClosed(IDialogResult result)
		{
			_isErrorMessageDialogShowing = false;

			if(_systemHasError)
			{
				// only reset console if system in error. No need to reset for warning only
				ResetConsoleSystem();
			}

			if(result.Result == ButtonResult.Yes)
			{
				Task.Delay(TimeSpan.FromSeconds(SYSTEM_ERROR_RESET_TIMEOUT)).ContinueWith(_ => ResetSystemErrorProperties());
			}
		}

		private void ResetSystemErrorProperties()
		{
			_machineModel.CMCUSystemStatusError &= ~_cmcuAllErrorsFlag;
			_machineModel.PMCUSystemStatusErrorCode &= ~_pmcuAllErrors;
			_systemHasError = false;
		}

		private void ResetSystemWarningProperties()
		{
			_machineModel.CMCUSystemStatusError &= ~_cmcuAllWarningFlag;
			_systemHasWarning = false;
		}

		private void UpdateVolume(int volume)
		{
			_machineModel.Console.SetAudioLevel((uint)volume);
		}

		private void ResetConsoleSystem(bool reconnectVacuum = false)
		{
			_machineModel?.Console?.FailResetEnable();
			Thread.Sleep(10);
			_machineModel?.Console?.FailResetDisable();
			Thread.Sleep(10);
			_machineModel?.Console?.Disconnect();

			if(reconnectVacuum)
			{
				Thread.Sleep(10);
				_machineModel?.Console?.Connect();
			}

			IsSystemRested = true;
			IsVacuumDisconnected = !reconnectVacuum;
			IsCMCUExceptionType5 = false;
		}

    private void OnReceiveUserCommand((UserCommand, DateTime) userCommandMessage)
    {
      var (command, dateTime) = userCommandMessage;
      switch (command)
      {
				case UserCommand.IgnoreCmcuExceptionType5:
          _ignoreCmcuExceptionType5 = true;
					break;
				case UserCommand.EnableCmcuExceptionType5:
          _ignoreCmcuExceptionType5 = false;
					break;
      }
    }
  }
}
