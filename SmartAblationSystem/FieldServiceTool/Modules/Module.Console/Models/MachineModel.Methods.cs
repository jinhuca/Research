using Module.Infrastructure.AppLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Communication;
using Console;
using DataAccessLayer;
using MicroLibrary;
using Module.Console.Helpers;
using Unity;
using static Communication.CanBusMessageDefinition;

namespace Module.Console.Models
{
	/// <summary>
	/// Methods for <see cref="MachineModel"/>.
	/// </summary>
	public partial class MachineModel
	{

		#region BalloonDAS methods

    public async Task SendBalloonPressureSetPointAsync(bool isDasEnabled)
    {
      await Task.Run(() => SendBalloonPressureSetPoint(isDasEnabled));
    }

    private void SendBalloonPressureSetPoint(bool isDasEnabled)
		{
			foreach (var stateId in Enum.GetValues(typeof(MessageStateId))
								 .OfType<MessageStateId>()
								 .Where(s => s != MessageStateId.CAN_ID_STATE_UNKNOWN && s != MessageStateId.CAN_ID_STATE_EXCEPTION))
			{
				int state = 0;
				state = Data.MessageStateIdToStateDict.ContainsKey(stateId)
					? Data.MessageStateIdToStateDict[stateId]
					: 1;

				var balloonParameters = data.DataAccess.GetDASBalloonParameterByStateId(state);

				var balloonTargetInjectionFlow = isDasEnabled
															? balloonParameters.HighFlowSetPoint ?? 0d
															: balloonParameters.LowFlowSetPoint ?? 0d;

				var balloonTargetInjectionPressure = isDasEnabled
															? balloonParameters.HighTargetInjectionPressure
															: balloonParameters.LowTargetInjectionPressure;

				var balloonPressure = isDasEnabled
															? balloonParameters.HighPressureSetPoint ?? 0d
															: balloonParameters.LowPressureSetpoint ?? 0d;

				// Update Das values
				Console.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachine[stateId].TargetInjectionFlow = balloonTargetInjectionFlow;
				Console.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachine[stateId].TargetInjectionPressure = balloonTargetInjectionPressure;
				Console.PatientMicroControllerBalloonPressureRegulatorValueAccordingToTheStateMachine[stateId].TargetBalloonPressure = balloonPressure;

				// Send message to Console via CanBus
				Console.WriteFromMicroController((MessageStateId)state, CatheterInfoIds[1]);
				System.Threading.Thread.Sleep(30);

				Console.WriteFromMicroController((MessageStateId)state, CentralMicroControllerTargetInjectionFlow);
				System.Threading.Thread.Sleep(30);
			}
		}

		#endregion BalloonDAS methods

		#region Private Methods

		private void InitializeMachine()
		{
			ReadAppSettings();
			ResolveObjects();
			_machine.GUIIsReady = true;

			// Initialize Data Access
      var tankMetalWeight = data.DataAccess.GetCurrentTankMetalWeight();
      Console.Tank.MetalWeight = tankMetalWeight;

			SubscribeTransducerEvents();
			SubscribeRegisterEvents();

			SubscribeConsoleMonitorEvents();

			ValidateCatheter();

			InitializeRegisterIDSDynamicTables();
			InitializeAckRegistersTable();

			ackTimer.Interval = TimeSpan.FromMilliseconds(2000);
			ackTimer.Tick += ackTimerTimer_tick;

			PropertyChanged += MachineModel_PropertyChanged;
		}

		private void MachineModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
		}

		/// <summary>
		/// Occurs when the Ack Timer tick event is raised
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="sender">The Ack Timer object.</param>
		/// <param name="e">Represents the base class for classes that contain timer event data.</param>
		private void ackTimerTimer_tick(object sender, EventArgs e)
		{
			lock (_myRegister_Lock)
			{
				List<int> localpatientMicroControllerRegisterIDSDynamicTable = new List<int>();
				localpatientMicroControllerRegisterIDSDynamicTable = PatientMicroControllerRegisterIDSDynamicTable;
				List<int> localcentralMicroControllerRegisterIDSDynamicTable = new List<int>();
				localcentralMicroControllerRegisterIDSDynamicTable = CentralMicroControllerRegisterIDSDynamicTable;
			}
		}

		private void ReadAppSettings()
		{
			catheterMaximumTimeDisconnection = Convert.ToInt64(ConfigurationManager.AppSettings["CMTD"]);
		}

		private void ResolveObjects()
		{
			_catheterConnectedTimer = container.Resolve<MicroTimer>();
			_catheterValidator = container.Resolve<CatheterValidator>();
		}

		public void ReadFirmwareVersions()
		{
			_machine.ReadFromMicroController(MessageStateId.CAN_ID_STATE_IDLE, CentralMicroControllerFirmwareVersionId);
			_machine.ReadFromMicroController(MessageStateId.CAN_ID_STATE_IDLE, PatientMicroControllerFirmwareVersionId);
			_machine.ReadFromMicroController(MessageStateId.CAN_ID_STATE_IDLE, CatheterFirmwareVersionId);
			_machine.ReadFromMicroControllerOnCanTwo(MessageStateId.CAN_ID_STATE_IDLE, RepeaterFirmwareAndICBFirmwareId);
			_machine.ReadFromMicroControllerOnCanTwo(MessageStateId.CAN_ID_STATE_IDLE, RemoteFirmwareId);
			Thread.Sleep(50);
		}

    private void ValidateCatheter()
		{
			_catheterConnectedTimer.Interval = 500000;
			_catheterConnectedTimer.MicroTimerElapsed += CatheterConnectedTimer_MicroTimerElapsed;
		}

		public void ResetCanOneStopWatch()
		{
			if (CanOneStopWatchCommunicationLost?.IsRunning == true)
			{
				CanOneStopWatchCommunicationLost.Restart();
			}
		}

		private void SendSystemStateToRemoteControl(MessageStateId _systemState)
		{
			byte[] data = new byte[8];
			Array.Clear(data, 0, 8);

			switch ((int)_systemState)
			{
				case (int)MessageStateId.CAN_ID_STATE_IDLE:
					data[0] = 1;
					break;

				case (int)MessageStateId.CAN_ID_STATE_READY:
					data[0] = 2;
					break;

				case (int)MessageStateId.CAN_ID_STATE_INFLATION:
					data[0] = 3;

					break;
				case (int)MessageStateId.CAN_ID_STATE_TRANSITION:
					data[0] = 4;
					break;

				case (int)MessageStateId.CAN_ID_STATE_ABLATION:
					data[0] = 5;
					break;

				case (int)MessageStateId.CAN_ID_STATE_THAWING:
					data[0] = 6;
					break;

				case (int)MessageStateId.CAN_ID_STATE_EXCEPTION:
					data[0] = 7;
					break;
			}

			_machine.SendStateToRemoteCotrol(_systemState, data);
		}

		/// <summary>
		/// Gets Sole valves status.
		/// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		private void GetSolenoidValvesStatus(Int64 cMCUValvesStatus)
		{
			if ((cMCUValvesStatus & (Int64)CMCUValvesStatus.SolenoidValve1ON) == (Int64)CMCUValvesStatus.SolenoidValve1ON)
			{
				IsSolenoidValve1ON = true;
			}
			else
			{
				IsSolenoidValve1ON = false;
			}

			if ((cMCUValvesStatus & (Int64)CMCUValvesStatus.SolenoidValve2ON) == (Int64)CMCUValvesStatus.SolenoidValve2ON)
			{
				IsSolenoidValve2ON = true;
			}
			else
			{
				IsSolenoidValve2ON = false;
			}

			if ((cMCUValvesStatus & (Int64)CMCUValvesStatus.SolenoidValve3ON) == (Int64)CMCUValvesStatus.SolenoidValve3ON)
			{
				IsSolenoidValve3ON = true;
			}
			else
			{
				IsSolenoidValve3ON = false;
			}

			if ((cMCUValvesStatus & (Int64)CMCUValvesStatus.SolenoidValve4ON) == (Int64)CMCUValvesStatus.SolenoidValve4ON)
			{
				IsSolenoidValve4ON = true;
			}
			else
			{
				IsSolenoidValve4ON = false;
			}

			if ((cMCUValvesStatus & (Int64)CMCUValvesStatus.SolenoidValve5ON) == (Int64)CMCUValvesStatus.SolenoidValve5ON)
			{
				IsSolenoidValve5ON = true;
			}
			else
			{
				IsSolenoidValve5ON = false;
			}

			if ((cMCUValvesStatus & (Int64)CMCUValvesStatus.SolenoidValve6ON) == (Int64)CMCUValvesStatus.SolenoidValve6ON)
			{
				IsSolenoidValve6ON = true;
			}
			else
			{
				IsSolenoidValve6ON = false;
			}

			if ((cMCUValvesStatus & (Int64)CMCUValvesStatus.SolenoidValve7ON) == (Int64)CMCUValvesStatus.SolenoidValve7ON)
			{
				IsSolenoidValve7ON = true;
			}
			else
			{
				IsSolenoidValve7ON = false;
			}

			if ((cMCUValvesStatus & (Int64)CMCUValvesStatus.SolenoidValve8ON) == (Int64)CMCUValvesStatus.SolenoidValve8ON)
			{
				IsSolenoidValve8ON = true;
			}
			else
			{
				IsSolenoidValve8ON = false;
			}

			if ((cMCUValvesStatus & (Int64)CMCUValvesStatus.SolenoidValve9ON) == (Int64)CMCUValvesStatus.SolenoidValve9ON)
			{
				IsSolenoidValve9ON = true;
			}
			else
			{
				IsSolenoidValve9ON = false;
			}
		}

		private void UpdateCatheterInfo(byte[] data)
		{
			// Update Catheter Info
			CatheterID = data[0]; //CanBusMessageConverter.ConverteInfoData(data, 0);
			CatheterSerialNumber = data[1]; //CanBusMessageConverter.ConverteInfoData(data, 2);
			CatheterLot = CanBusMessageConverter.ConverteCatheterInfoData(data, 2);

			// we are using exception datetime data because there is nodate time in C
			CatheterExpirationMonth = data[4];
			CatheterExpirationDay = data[5];
			CatheterExpirationYear = CanBusMessageConverter.ConverteInfoData(data, 6);

			try
			{
				CatheterExpirationDate = new DateTime(CatheterExpirationYear, CatheterExpirationMonth, CatheterExpirationDay);
			}
			catch (Exception ex)
			{
				FieldServiceTrace.LogException(ex);
				CatheterExpirationDate = inavalidCatheterExpirationDate;
			}
		}

		private void ManageRTRCatheterMessage(byte[] data, ICanBusCommunication communicationData, int iD, bool needUpdateCatheterInfo = true)
		{
			if (needUpdateCatheterInfo)
			{
				UpdateCatheterInfo(data);
			}

			// Update the Catheter Last used date
			try
			{
				CatheterLastUseDate = new DateTime(CatheterLastUseYear, CatheterLastUseMonth, CatheterLastUseDay, CatheterLastUseHour, 0, 0, 0);
			}
			catch (Exception ex)
			{
				FieldServiceTrace.LogException(ex);
				CatheterLastUseDate = DateTime.Now;
			}

			// TODO:: Should validate Catheter here, We will do it later
			// TODO:: Assume the Catheter is valid now 
			IsCatheterValid = true;

			// Send Catheter RTR Acknowledge message 
			if (IsCatheterValid)
			{
				//here it is important that all catheter use a serial number
				InitializeRegistersAccordingToCatheterID(CatheterID);
				SendRequestedDataAsync(communicationData.CanBusOneEventArgs.Id, (uint)iD, true, true);
			}
			else
			{
				SendRequestedDataAsync(communicationData.CanBusOneEventArgs.Id, (uint)iD, true, false);
			}
		}

		/// <summary>
		/// Initializes the registers according to a catheter ID
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="catheterID">An integer representing a Catheter Id.</param>
		/// <id>SF-SDS-0104</id>
		private void InitializeRegistersAccordingToCatheterID(int catheterID)
		{
			InitializeCatheterInfo();

			//Target Ballon pressure
			CatheterType catheterType = data.DataAccess.GetCatheterByCatheterId(catheterID & (~EngineeringCatheterSignature));
			//_machine.Balloon.TargetBalloonPressure = catheterType.TargetBalloonPressure;

			InitializePMCURegisterValues(catheterType.ID);

			InitializeCMCURegisterValues(catheterType.ID);

			InitializeDASBalloonRegisters();
		}

		private void InitializeCatheterInfo()
		{
			//Serial Number and Number Of Injections
			FieldServiceTrace.Log($"Initialize Catheter Info : SN={CatheterSerialNumber}; " +
														$"Injection#={NumberOfInjections}; ExpirationDate={CatheterExpirationDate}; LastUsedDate={CatheterExpirationDate}. ");

			_machine.Catheter.SerialNumber = CatheterSerialNumber;
			_machine.Catheter.NumberOfInjections = NumberOfInjections;

			//Catheter Expiration Date
			if (CatheterExpirationDate != null)
			{
				_machine.Catheter.CatheterExpirationYear = CatheterExpirationDate.Year;
				_machine.Catheter.CatheterExpirationMonth = CatheterExpirationDate.Month;
				_machine.Catheter.CatheterExpirationDay = CatheterExpirationDate.Day;
			}

			//Last Use Date
			if (CatheterLastUseDate != null)
			{
				_machine.Catheter.CatheterLastUseYear = CatheterLastUseDate.Year;
				_machine.Catheter.CatheterLastUseMonth = CatheterLastUseDate.Month;
				_machine.Catheter.CatheterLastUseDay = CatheterLastUseDate.Day;
				_machine.Catheter.CatheterLastUseHour = CatheterLastUseDate.Hour;
			}
		}

		private void InitializePMCURegisterValues(int catheterId)
		{
			IEnumerable<PMCRegisterValue> pMCRegisterValues = data.DataAccess.GetPMCRegisterValuesByCatheterID(catheterId);
			//FieldServiceTrace.Log($"Initialize PMCU Register Values , number of parameters loaded: {pMCRegisterValues.Count()}");
			// TODO:: need to understand what need to do with ChangeBalloonTypeFSM
			// if ((catheterID & (~EngineeringCatheterSignature)) == (int)Enumeration.CatheterType.Plus)
			// {
			//     ChangeBalloonTypeFSM.CatheterType = Enumeration.CatheterType.Plus;
			//     IsSystemUsingDASBalloon = true;
			//
			//
			// }
			// else if ((catheterID & (~EngineeringCatheterSignature)) == (int)Enumeration.CatheterType.ID28mm)
			// {
			//     ChangeBalloonTypeFSM.CatheterType = Enumeration.CatheterType.ID28mm;
			//     IsSystemUsingDASBalloon = false;
			// }

			// Initialize Patient Micro Controller Register. the traget ballon pressure is state independent.
			foreach (PMCRegisterValue pMCRegisterValue in pMCRegisterValues.Where(p => p.StateID != 7))
			{
				MessageStateId mid = Data.StateToMessageStateIdDict.ContainsKey(pMCRegisterValue.StateID)
						? Data.StateToMessageStateIdDict[pMCRegisterValue.StateID]
						: MessageStateId.CAN_ID_STATE_IDLE;

				_machine.PatientPressureTransducerOneValueAccordingToTheStateMachine[mid].PressureThresholdHighLimit = pMCRegisterValue.CP1PressureThresholdHighLimit;

				if (pMCRegisterValue.StateID == 1)
				{
					_machine.PatientPressureTransducerOneValueAccordingToTheStateMachine[mid].PressureLowRangeLimit = pMCRegisterValue.CP1PressureLowRangeLimit;
				}
				else
				{
					double localPressureLowRangeLimit = OuterBalloonPressureThreshold.GetThreshold(PT3Reading);

					if (localPressureLowRangeLimit < -12)
					{
						_machine.PatientPressureTransducerOneValueAccordingToTheStateMachine[mid].PressureLowRangeLimit = -12;
					}
					else if (localPressureLowRangeLimit > -6)
					{
						_machine.PatientPressureTransducerOneValueAccordingToTheStateMachine[mid].PressureLowRangeLimit = -6;
					}

					else if (!(localPressureLowRangeLimit < -12) && !(localPressureLowRangeLimit > -6))
					{
						_machine.PatientPressureTransducerOneValueAccordingToTheStateMachine[mid].PressureLowRangeLimit = localPressureLowRangeLimit;
					}
				}

				_machine.PatientPressureTransducerOneValueAccordingToTheStateMachine[mid].PressureHighRangeLimit = pMCRegisterValue.CP1PressureHighRangeLimit;

				//CP2
				_machine.PatientPressureTransducerTwoValueAccordingToTheStateMachine[mid].PressureThresholdHighLimit = pMCRegisterValue.CP2PressureThresholdHighLimit;
				_machine.PatientPressureTransducerTwoValueAccordingToTheStateMachine[mid].PressureLowRangeLimit = pMCRegisterValue.CP2PressureLowRangeLimit;
				_machine.PatientPressureTransducerTwoValueAccordingToTheStateMachine[mid].PressureHighRangeLimit = pMCRegisterValue.CP2PressureHighRangeLimit;

				//TC1
				_machine.ThermocoupleOneValueAccordingToTheStateMachine[mid].ThawingTemperature = pMCRegisterValue.TC1ThawingTemperature;

				//Thawing Temperature Set Point
				_machine.ThermocoupleOneValueAccordingToTheStateMachine[mid].ThawingTemperatureSetPoint = pMCRegisterValue.ThawingTemperatureSetPoint;
				ThawingTemperatureSetPoint = pMCRegisterValue.ThawingTemperatureSetPoint;

				//Patient Micro Controller PID
				_machine.PatientMicroControllerPIDValueAccordingToTheStateMachine[mid].PGain = pMCRegisterValue.Pgain;
				_machine.PatientMicroControllerPIDValueAccordingToTheStateMachine[mid].IGain = pMCRegisterValue.Igain;
				_machine.PatientMicroControllerPIDValueAccordingToTheStateMachine[mid].DGain = pMCRegisterValue.Dgain;
				_machine.PatientMicroControllerPIDValueAccordingToTheStateMachine[mid].Offset = pMCRegisterValue.Offset;

				//Target Balloon Pressure
				_machine.PatientMicroControllerBalloonPressureRegulatorValueAccordingToTheStateMachine[mid].TargetBalloonPressure = pMCRegisterValue.TargetBalloonPressure;

				//Blood detector
				_machine.BloodDetectorValueAccordingToTheStateMachine[mid].LowerBloodThreshold = pMCRegisterValue.LowerBloodThreshold;
				_machine.BloodDetectorValueAccordingToTheStateMachine[mid].UpperBloodThreshold = pMCRegisterValue.UpperBloodThreshold;
			}
		}

		private void InitializeCMCURegisterValues(int catheterId)
		{
			IEnumerable<CMCRegisterValue> cMCRegisterValues = data.DataAccess.GetCMCRegisterValuesByCatheterID(catheterId);

			//FieldServiceTrace.Log($"Initialize CMCU Register Values, number of parameters loaded: {cMCRegisterValues.Count()}");

			foreach (CMCRegisterValue cMCRegisterValue in cMCRegisterValues.Where(c => c.StateID != 7))
			{
				MessageStateId mid = Data.StateToMessageStateIdDict.ContainsKey(cMCRegisterValue.StateID)
						? Data.StateToMessageStateIdDict[cMCRegisterValue.StateID]
						: MessageStateId.CAN_ID_STATE_IDLE;

				//Target Injection Flow
				_machine.InjectionFlowValueAccordingToTheStateMachine[mid].TargetInjectionFlow = cMCRegisterValue.TargetInjectionFlow;

				//target Injection Pressure
				// TODO : change the EDMX file and update the code
				//_machine.InjectionPressureValueAccordingToTheStateMachine[mid].TargetInjectionPressure = cMCRegisterValue

				//Central Micro Controller PID CentralMicroControllerPIDValueAccordingToTheStateMachine
				_machine.CentralMicroControllerPIDValueAccordingToTheStateMachine[mid].PGain = cMCRegisterValue.PGain;
				_machine.CentralMicroControllerPIDValueAccordingToTheStateMachine[mid].IGain = cMCRegisterValue.IGain;
				_machine.CentralMicroControllerPIDValueAccordingToTheStateMachine[mid].DGain = cMCRegisterValue.DGain;
				_machine.CentralMicroControllerPIDValueAccordingToTheStateMachine[mid].Offset = cMCRegisterValue.Offset;

				//PT1
				_machine.PressureTransducerOneValueAccordingToTheStateMachine[mid].TankPressureLow = cMCRegisterValue.PT1TankPressureLow;
				_machine.PressureTransducerOneValueAccordingToTheStateMachine[mid].PressureThresholdHighLimit = cMCRegisterValue.PT1PressureThresholdHighLimit;
				_machine.PressureTransducerOneValueAccordingToTheStateMachine[mid].TankPressureTooHigh = cMCRegisterValue.PT1TankPressureTooHigh;
				_machine.PressureTransducerOneValueAccordingToTheStateMachine[mid].PressureLowRangeLimit = cMCRegisterValue.PT1PressureLowRangeLimit;
				_machine.PressureTransducerOneValueAccordingToTheStateMachine[mid].PressureHighRangeLimit = cMCRegisterValue.PT1PressureHighRangeLimit;

				//PT2
				_machine.PressureTransducerTwoValueAccordingToTheStateMachine[mid].PressureThresholdHighLimit = cMCRegisterValue.PT2PressureThresholdHighLimit;
				_machine.PressureTransducerTwoValueAccordingToTheStateMachine[mid].PressureLowRangeLimit = cMCRegisterValue.PT2PressureLowRangeLimit;
				_machine.PressureTransducerTwoValueAccordingToTheStateMachine[mid].PressureHighRangeLimit = cMCRegisterValue.PT2PressureHighRangeLimit;

				//PT3
				_machine.PressureTransducerThreeValueAccordingToTheStateMachine[mid].PressureThresholdHighLimit = cMCRegisterValue.PT3PressureThresholdHighLimit;
				_machine.PressureTransducerThreeValueAccordingToTheStateMachine[mid].PressureLowRangeLimit = cMCRegisterValue.PT3PressureLowRangeLimit;
				_machine.PressureTransducerThreeValueAccordingToTheStateMachine[mid].PressureHighRangeLimit = cMCRegisterValue.PT3PressureHighRangeLimit;

				//PT4
				_machine.PressureTransducerFourValueAccordingToTheStateMachine[mid].PressureThresholdHighLimit = cMCRegisterValue.PT4PressureThresholdHighLimit;
				_machine.PressureTransducerFourValueAccordingToTheStateMachine[mid].PressureLowRangeLimit = cMCRegisterValue.PT4PressureLowRangeLimit;
				_machine.PressureTransducerFourValueAccordingToTheStateMachine[mid].PressureHighRangeLimit = cMCRegisterValue.PT4PressureHighRangeLimit;

				//TS1
				_machine.TemperatureSensorOneValueAccordingToTheStateMachine[mid].TemperatureThresholdHighLimit = cMCRegisterValue.TS1TemperatureThresholdHighLimit;
				_machine.TemperatureSensorOneValueAccordingToTheStateMachine[mid].TemperatureLowRangeLimit = cMCRegisterValue.TS1TemperatureLowRangeLimit;
				_machine.TemperatureSensorOneValueAccordingToTheStateMachine[mid].TemperatureHighRangeLimit = cMCRegisterValue.TS1TemperatureHighRangeLimit;

				//FM1
				_machine.FlowMeterOneValueAccordingToTheStateMachine[mid].FlowMeterThresholLowlimit = cMCRegisterValue.FM1FlowMeterThresholLowlimit;
				_machine.FlowMeterOneValueAccordingToTheStateMachine[mid].FlowMeterThresholHighlimit = cMCRegisterValue.FM1FlowMeterThresholHighlimit;
				_machine.FlowMeterOneValueAccordingToTheStateMachine[mid].FlowMeterLowRangeLimit = cMCRegisterValue.FM1FlowMeterLowRangeLimit;
				_machine.FlowMeterOneValueAccordingToTheStateMachine[mid].FlowMeterHighRangelimit = cMCRegisterValue.FM1FlowMeterHighRangelimit;

				//PS1
				_machine.PressureSwitchOneValueAccordingToTheStateMachine[mid].PressureThresholdHighLimit = cMCRegisterValue.PS1PressureThresholdHighLimit;
				_machine.PressureSwitchOneValueAccordingToTheStateMachine[mid].PressureLowRangeLimit = cMCRegisterValue.PS1PressureLowRangeLimit;
				_machine.PressureSwitchOneValueAccordingToTheStateMachine[mid].PressureHighRangeLimit = cMCRegisterValue.PS1PressureHighRangeLimit;

				//PS2
				_machine.PressureSwitchTwoValueAccordingToTheStateMachine[mid].PressureThresholdHighLimit = cMCRegisterValue.PS2PressureThresholdHighLimit;
				_machine.PressureSwitchTwoValueAccordingToTheStateMachine[mid].PressureLowRangeLimit = cMCRegisterValue.PS2PressureLowRangeLimit;
				_machine.PressureSwitchTwoValueAccordingToTheStateMachine[mid].PressureHighRangeLimit = cMCRegisterValue.PS2PressureHighRangeLimit;

				//LC1
				// we need to add the metal tank
				double localMetalTank = _machine.Tank.MetalWeight;
				_machine.LoadCellOneValueAccordingToTheStateMachine[mid].LoadCellThresholdWarning = cMCRegisterValue.LC1LoadCellThresholdWarning + localMetalTank;
				_machine.LoadCellOneValueAccordingToTheStateMachine[mid].LoadCellThresholdFail = cMCRegisterValue.LC1LoadCellThresholdFail + localMetalTank;
				_machine.LoadCellOneValueAccordingToTheStateMachine[mid].LoadCellLowRangeLimit = cMCRegisterValue.LC1LoadCellLowRangeLimit + localMetalTank;
				_machine.LoadCellOneValueAccordingToTheStateMachine[mid].LoadCellHighRangeLimit = cMCRegisterValue.LC1LoadCellHighRangeLimit + localMetalTank;

				//Target Injection Flow, Target Injection Pressure
				_machine.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachine[mid].TargetInjectionFlow = cMCRegisterValue.TargetInjectionFlow;
				_machine.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachine[mid].TargetInjectionPressure = cMCRegisterValue.TargetInjectionPressure;

				if (mid == MessageStateId.CAN_ID_STATE_ABLATION)
					baseTargetInjectionFlow = cMCRegisterValue.TargetInjectionFlow;

				//Low Flow Value
				_machine.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachine[mid].TargetInjectionLowFlow = cMCRegisterValue.LowFlow;
			}
		}

		private void InitializeDASBalloonRegisters()
		{

			// List<BalloonParameters> ballonParameters = data.DataAccess.GetDASBallonParameters();
			foreach (MessageStateId stateId in Enum.GetValues(typeof(MessageStateId)))
			{
				if (stateId == MessageStateId.CAN_ID_STATE_UNKNOWN || stateId == MessageStateId.CAN_ID_STATE_EXCEPTION)
					continue;

				MessageStateId mid = stateId;

				int state = Data.MessageStateIdToStateDict.ContainsKey(stateId)
						? Data.MessageStateIdToStateDict[stateId]
						: 1;

				var balloonParameters = data.DataAccess.GetDASBalloonParameterByStateId(state);

				//FieldServiceTrace.Log($"Initialize DAS Balloon Registers, number of parameters loaded: StateId={balloonParameters.StateID}; DASLowFlow={balloonParameters.DASLowFlow} .");


				//Ballon Rum up and ramp dow timing 
				_machine.PatientMicroControllerCryoBalloonConfigurationValueAccordingToTheStateMachine[mid].RampUpTimeByStep = (double)balloonParameters.RampUpTimeByStep;
				_machine.PatientMicroControllerCryoBalloonConfigurationValueAccordingToTheStateMachine[mid].PressureRampUpValue = (double)balloonParameters.PressureRampUpValue;
				_machine.PatientMicroControllerCryoBalloonConfigurationValueAccordingToTheStateMachine[mid].RampDownTimeByStep = (double)balloonParameters.RampDownTimeByStep;
				_machine.PatientMicroControllerCryoBalloonConfigurationValueAccordingToTheStateMachine[mid].PressureRampDownValue = (double)balloonParameters.PressureRampDownValue;

				_machine.PatientMicroControllerCryoBalloonConfigurationValueAccordingToTheStateMachine[mid].DASLowFlow = balloonParameters.DASLowFlow;
			}
		}

		/// <summary>
		/// Sends requested data to the Console.
		/// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="RTRID">An unsigned-integer RTRID.</param>
		/// <param name="localId">An unsigned-integer Local ID.</param>
		/// <param name="isItAnsweringCatheterValidation">A boolean representing if answering catheter validaiton.</param>
		/// <param name="iscatheterValid">A boolean representing the catheter validity.</param>
		private void SendRequestedData(uint RTRID, uint localId, bool isItAnsweringCatheterValidation = false, bool iscatheterValid = false)
		{
			if (isItAnsweringCatheterValidation)
			{
				MessageStateId stateId = (MessageStateId)IdToMachineState.ConvertIdToSate(RTRID);
				_machine.AnswerForRemoteFrame(stateId, RTRID, localId, true, iscatheterValid);
			}
			else
			{
				MessageStateId stateId = (MessageStateId)IdToMachineState.ConvertIdToSate(RTRID);
				_machine.AnswerForRemoteFrame(stateId, RTRID, localId);
			}
		}

		private async void SendRequestedDataAsync(uint RTRID, uint localId, bool isItAnsweringCatheterValidation = false, bool iscatheterValid = false)
		{
			await Task.Run(() => SendRequestedData(RTRID, localId, isItAnsweringCatheterValidation, iscatheterValid));
		}

		/// <summary>
		/// Send initialization data.
		/// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		private void SendInit()
		{
			if (AllowFirmwareReading)
			{
				_machine.SendBootMessage(MessageStateId.CAN_ID_STATE_IDLE, (int)BootLoaderID.CAN_ID_BOOT_INIT, ASCIIToByteConverter.Initdata);
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private void AnalyseEsophagusTemperature()
		{
			List<double> sesnors = new List<double> { Math.Round(ecgChannel5And6Reading), EtsSesnor1, EtsSesnor2, EtsSesnor3, EtsSesnor4,
								EtsSesnor5, EtsSesnor6, EtsSesnor7, EtsSesnor8,
								EtsSesnor9, EtsSesnor10, EtsSesnor11, EtsSesnor12};
			ListOfSesnorsState.Clear();

			ListOfSesnorsState = ETSdataSortingAndStatus.GetMin(sesnors, out eTSMinimumTemperature);

			MinimumTemperature = eTSMinimumTemperature;
		}

		#endregion Private Methods

		#region Event Handlers

		private void SubscribeTransducerEvents()
		{
			_machine.pressureTransducerEvent += PressureChanged;
			_machine.thermocoupleEvent += TemperatureChanged;
			_machine.pressureSwitchEvent += PressureSwitchEvent;
			_machine.flowMeterEvent += FlowMeterEvent;
			_machine.loadCellEvent += LoadCellEvent;
			_machine.bloodDetectorEvent += BloodDetectorEvent;
			_machine.ecgEventArgs += EcgArgsChanged;
			_machine.remoteControlMembraneSwitchStateEventArgs += RemoteControlMembraneSwitchStateEventArgs;
			_machine.bloodPressureSensorStateEventArgs += BloodPressureSensorConnectionChanged;
			_machine.probeEventArgs += ProbeSesnorChanged;
		}

		private void SubscribeRegisterEvents()
		{
			_machine.registerEvent += RegisterChangedEventHandler;
			_machine.canTwoRegisterEvent += CanTwoRegisterChangedEventHandler;
		}

		private void RegisterChangedEventHandler(object sender, RegisterValuesEventArgs e)
		{
			ResetCanOneStopWatch();

			var communicationData = sender as ICanBusCommunication;
			byte[] data = null;

			if (communicationData?.CanBusOneEventArgs.Data != null)
			{
				data = communicationData.CanBusOneEventArgs.Data;
			}

			if (communicationData.CanBusOneEventArgs.Falgs != (int)FrameType.Remote && data != null)
			{
				switch (e.ID)
				{
					case 8:
						CentralMicroControllerFirmwareVersion = CanBusMessageConverter.ConverteInfoData(data, 0);
						CpldFirmwareVersion = CanBusMessageConverter.ConverteInfoData(data, 2);
						CentralMicroControllerBootLoaderFirmwareVersion = CanBusMessageConverter.ConverteInfoData(data, 4);
						break;

					case 9:
						CPLDErrorRegister = CanBusMessageConverter.ConverteInfoData(data, 0);
						break;

					case 10:
						CPLDValveRegister = CanBusMessageConverter.ConverteInfoData(data, 0);
						break;

					case 11:
						CPLDSystemRegister = CanBusMessageConverter.ConverteInfoData(data, 0);
						break;

					case 13:
						AblationTime = CanBusMessageConverter.ConverteInfoData(data, 0);
						break;

					case 14:
						ContinuousThawing = CanBusMessageConverter.ConverteDecimalData(data, 0);
						break;

					case 15:
						TargetInjectionFlow = CanBusMessageConverter.ConverteDecimalDataFM1(data, 0);
						TargetInjectionPressure = CanBusMessageConverter.ConverteDecimalData(data, 2);
						if (IsReadingFromMicroControllerForRegisterValidation)
						{
							if (_machine?.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachine?.ContainsKey(SystemState) == true)
							{
								listOfValues.Add((_machine.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachine[SystemState].TargetInjectionFlow, TargetInjectionFlow));
								listOfValues.Add((_machine.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachine[SystemState].TargetInjectionPressure, TargetInjectionPressure));
							}
							CentralMicroControllerAckRegistersTable[15] = true;
						}
						break;
					case 16:
						PGain = CanBusMessageConverter.ConverteDecimalData(data, 0);
						IGain = CanBusMessageConverter.ConverteDecimalData(data, 2);
						DGain = CanBusMessageConverter.ConverteDecimalData(data, 4);
						PIDOffset = CanBusMessageConverter.ConverteDecimalData(data, 6);
						if (IsReadingFromMicroControllerForRegisterValidation)
						{
							if (_machine?.PatientMicroControllerPIDValueAccordingToTheStateMachine?.ContainsKey(SystemState) == true)
							{
								listOfValues.Add((_machine.PatientMicroControllerPIDValueAccordingToTheStateMachine[SystemState].PGain, PGain));
								listOfValues.Add((_machine.PatientMicroControllerPIDValueAccordingToTheStateMachine[SystemState].IGain, IGain));
								listOfValues.Add((_machine.PatientMicroControllerPIDValueAccordingToTheStateMachine[SystemState].DGain, DGain));
								listOfValues.Add((_machine.PatientMicroControllerPIDValueAccordingToTheStateMachine[SystemState].Offset, PIDOffset));
							}
							CentralMicroControllerAckRegistersTable[16] = true;
						}
						break;

					case 17:
						ThresholdForPT1High = CanBusMessageConverter.ConverteDecimalData(data, 0);
						ThresholdForPT1Fail = CanBusMessageConverter.ConverteDecimalData(data, 2);
						ThresholdForPT1Low = CanBusMessageConverter.ConverteDecimalData(data, 4);
						if (IsReadingFromMicroControllerForRegisterValidation)
						{
							if (_machine?.PressureTransducerOneValueAccordingToTheStateMachine?.ContainsKey(SystemState) == true)
							{
								listOfValues.Add((_machine.PressureTransducerOneValueAccordingToTheStateMachine[SystemState].PressureThresholdHighLimit, ThresholdForPT1High));
								listOfValues.Add((_machine.PressureTransducerOneValueAccordingToTheStateMachine[SystemState].TankPressureTooHigh, ThresholdForPT1Fail));
								listOfValues.Add((_machine.PressureTransducerOneValueAccordingToTheStateMachine[SystemState].TankPressureLow, ThresholdForPT1Low));
							}
							CentralMicroControllerAckRegistersTable[17] = true;
						}
						break;

					case 18:
						PT1LowRange = CanBusMessageConverter.ConverteDecimalData(data, 0);
						PT1HighRange = CanBusMessageConverter.ConverteDecimalData(data, 2);
						if (IsReadingFromMicroControllerForRegisterValidation)
						{
							if (_machine?.PressureTransducerOneValueAccordingToTheStateMachine?.ContainsKey(SystemState) == true)
							{
								listOfValues.Add((_machine.PressureTransducerOneValueAccordingToTheStateMachine[SystemState].PressureLowRangeLimit, PT1LowRange));
								listOfValues.Add((_machine.PressureTransducerOneValueAccordingToTheStateMachine[SystemState].PressureHighRangeLimit, PT1HighRange));
							}
							CentralMicroControllerAckRegistersTable[18] = true;
						}
						break;

					case 19:
						ThresholdPT2High = CanBusMessageConverter.ConverteDecimalData(data, 0);
						if (IsReadingFromMicroControllerForRegisterValidation)
						{
							if (_machine?.PressureTransducerTwoValueAccordingToTheStateMachine?.ContainsKey(SystemState) == true)
							{
								listOfValues.Add((_machine.PressureTransducerTwoValueAccordingToTheStateMachine[SystemState].PressureThresholdHighLimit, ThresholdPT2High));
							}
							CentralMicroControllerAckRegistersTable[19] = true;
						}
						break;

					case 20:
						PT2LowRange = CanBusMessageConverter.ConverteDecimalData(data, 0);
						PT2HighRange = CanBusMessageConverter.ConverteDecimalData(data, 2);
						if (IsReadingFromMicroControllerForRegisterValidation)
						{
							if (_machine?.PressureTransducerTwoValueAccordingToTheStateMachine?.ContainsKey(SystemState) == true)
							{
								listOfValues.Add((_machine.PressureTransducerTwoValueAccordingToTheStateMachine[SystemState].PressureLowRangeLimit, PT2LowRange));
								listOfValues.Add((_machine.PressureTransducerTwoValueAccordingToTheStateMachine[SystemState].PressureHighRangeLimit, PT2HighRange));
							}
							CentralMicroControllerAckRegistersTable[20] = true;
						}
						break;

					case 21:
						ThresholdPT3High = CanBusMessageConverter.ConverteDecimalData(data, 0);
						if (IsReadingFromMicroControllerForRegisterValidation)
						{
							if (_machine?.PressureTransducerThreeValueAccordingToTheStateMachine?.ContainsKey(SystemState) == true)
							{
								listOfValues.Add((_machine.PressureTransducerThreeValueAccordingToTheStateMachine[SystemState].PressureThresholdHighLimit, ThresholdPT3High));
							}
							CentralMicroControllerAckRegistersTable[21] = true;
						}
						break;

					case 22:
						PT3LowRange = CanBusMessageConverter.ConverteDecimalData(data, 0);
						PT3HighRange = CanBusMessageConverter.ConverteDecimalData(data, 2);
						if (IsReadingFromMicroControllerForRegisterValidation)
						{
							if (_machine?.PressureTransducerThreeValueAccordingToTheStateMachine?.ContainsKey(SystemState) == true)
							{
								listOfValues.Add((_machine.PressureTransducerThreeValueAccordingToTheStateMachine[SystemState].PressureLowRangeLimit, PT3LowRange));
								listOfValues.Add((_machine.PressureTransducerThreeValueAccordingToTheStateMachine[SystemState].PressureHighRangeLimit, PT3HighRange));
							}
							CentralMicroControllerAckRegistersTable[22] = true;
						}
						break;

					case 23:
						ThresholdPT4high = CanBusMessageConverter.ConverteDecimalData(data, 0);
						if (IsReadingFromMicroControllerForRegisterValidation)
						{
							if (_machine?.PressureTransducerFourValueAccordingToTheStateMachine?.ContainsKey(SystemState) == true)
							{
								listOfValues.Add((_machine.PressureTransducerFourValueAccordingToTheStateMachine[SystemState].PressureThresholdHighLimit, ThresholdPT4high));
							}
							CentralMicroControllerAckRegistersTable[23] = true;
						}
						break;

					case 24:
						PT4LowRange = CanBusMessageConverter.ConverteDecimalData(data, 0);
						PT4HighRange = CanBusMessageConverter.ConverteDecimalData(data, 2);
						if (IsReadingFromMicroControllerForRegisterValidation)
						{
							if (_machine?.PressureTransducerFourValueAccordingToTheStateMachine?.ContainsKey(SystemState) == true)
							{
								listOfValues.Add((_machine.PressureTransducerFourValueAccordingToTheStateMachine[SystemState].PressureLowRangeLimit, PT4LowRange));
								listOfValues.Add((_machine.PressureTransducerFourValueAccordingToTheStateMachine[SystemState].PressureHighRangeLimit, PT4HighRange));
							}
							CentralMicroControllerAckRegistersTable[24] = true;
						}
						break;

					case 25:
						ThresholdTS1High = CanBusMessageConverter.ConverteNegativDecimalData(data, 0);
						if (IsReadingFromMicroControllerForRegisterValidation)
						{
							if (_machine?.TemperatureSensorOneValueAccordingToTheStateMachine?.ContainsKey(SystemState) == true)
							{
								listOfValues.Add((_machine.TemperatureSensorOneValueAccordingToTheStateMachine[SystemState].TemperatureThresholdHighLimit, ThresholdTS1High));
							}
							CentralMicroControllerAckRegistersTable[25] = true;
						}
						break;

					case 26:
						TS1LowRange = CanBusMessageConverter.ConverteNegativDecimalData(data, 0);
						TS1HighRange = CanBusMessageConverter.ConverteDecimalData(data, 2);
						if (IsReadingFromMicroControllerForRegisterValidation)
						{
							if (_machine?.TemperatureSensorOneValueAccordingToTheStateMachine?.ContainsKey(SystemState) == true)
							{
								listOfValues.Add((_machine.TemperatureSensorOneValueAccordingToTheStateMachine[SystemState].TemperatureLowRangeLimit, TS1LowRange));
								listOfValues.Add((_machine.TemperatureSensorOneValueAccordingToTheStateMachine[SystemState].TemperatureHighRangeLimit, TS1HighRange));
							}
							CentralMicroControllerAckRegistersTable[26] = true;
						}
						break;

					case 27:
						ThresholdFM1Low = CanBusMessageConverter.ConverteFM1NegativDecimalData(data, 0);
						ThresholdFM1High = CanBusMessageConverter.ConverteFM1NegativDecimalData(data, 2);
						if (IsReadingFromMicroControllerForRegisterValidation)
						{
							if (_machine?.FlowMeterOneValueAccordingToTheStateMachine?.ContainsKey(SystemState) == true)
							{
								listOfValues.Add((_machine.FlowMeterOneValueAccordingToTheStateMachine[SystemState].FlowMeterThresholLowlimit, ThresholdFM1Low));
								listOfValues.Add((_machine.FlowMeterOneValueAccordingToTheStateMachine[SystemState].FlowMeterThresholHighlimit, ThresholdFM1High));
							}
							CentralMicroControllerAckRegistersTable[27] = true;
						}
						break;

					case 28:
						FM1LowRange = CanBusMessageConverter.ConverteFM1NegativDecimalData(data, 0);
						FM1HighRange = CanBusMessageConverter.ConverteFM1NegativDecimalData(data, 2);
						if (IsReadingFromMicroControllerForRegisterValidation)
						{
							if (_machine?.FlowMeterOneValueAccordingToTheStateMachine?.ContainsKey(SystemState) == true)
							{
								listOfValues.Add((_machine.FlowMeterOneValueAccordingToTheStateMachine[SystemState].FlowMeterLowRangeLimit, FM1LowRange));
								listOfValues.Add((_machine.FlowMeterOneValueAccordingToTheStateMachine[SystemState].FlowMeterHighRangelimit, FM1HighRange));
							}
							CentralMicroControllerAckRegistersTable[28] = true;
						}
						break;

					case 29:
						ThresholdPS1High = CanBusMessageConverter.ConverteDecimalData(data, 0);
						if (IsReadingFromMicroControllerForRegisterValidation)
						{
							if (_machine?.PressureSwitchOneValueAccordingToTheStateMachine?.ContainsKey(SystemState) == true)
							{
								listOfValues.Add((_machine.PressureSwitchOneValueAccordingToTheStateMachine[SystemState].PressureThresholdHighLimit, ThresholdPS1High));
							}
							CentralMicroControllerAckRegistersTable[29] = true;
						}
						break;

					case 30:
						PS1LowRange = CanBusMessageConverter.ConverteDecimalData(data, 0);
						PS1HighRange = CanBusMessageConverter.ConverteDecimalData(data, 2);
						if (IsReadingFromMicroControllerForRegisterValidation)
						{
							if (_machine?.PressureSwitchOneValueAccordingToTheStateMachine?.ContainsKey(SystemState) == true)
							{
								listOfValues.Add((_machine.PressureSwitchOneValueAccordingToTheStateMachine[SystemState].PressureLowRangeLimit, PS1LowRange));
								listOfValues.Add((_machine.PressureSwitchOneValueAccordingToTheStateMachine[SystemState].PressureHighRangeLimit, PS1HighRange));
							}
							CentralMicroControllerAckRegistersTable[30] = true;
						}
						break;

					case 31:
						ThresholdPS2High = CanBusMessageConverter.ConverteDecimalData(data, 0);
						if (IsReadingFromMicroControllerForRegisterValidation)
						{
							if (_machine?.PressureSwitchTwoValueAccordingToTheStateMachine?.ContainsKey(SystemState) == true)
							{
								listOfValues.Add((_machine.PressureSwitchTwoValueAccordingToTheStateMachine[SystemState].PressureThresholdHighLimit, ThresholdPS2High));
							}
							CentralMicroControllerAckRegistersTable[31] = true;
						}
						break;

					case 32:
						PS2LowRange = CanBusMessageConverter.ConverteDecimalData(data, 0);
						PS2HighRange = CanBusMessageConverter.ConverteDecimalData(data, 2);
						if (IsReadingFromMicroControllerForRegisterValidation)
						{
							if (_machine?.PressureSwitchTwoValueAccordingToTheStateMachine?.ContainsKey(SystemState) == true)
							{
								listOfValues.Add((_machine.PressureSwitchTwoValueAccordingToTheStateMachine[SystemState].PressureLowRangeLimit, PS2LowRange));
								listOfValues.Add((_machine.PressureSwitchTwoValueAccordingToTheStateMachine[SystemState].PressureHighRangeLimit, PS2HighRange));
							}
							CentralMicroControllerAckRegistersTable[32] = true;
						}
						break;

					case 33:
						ThresholdLC1Warning = CanBusMessageConverter.ConverteDecimalData(data, 0);
						ThresholdLC1Fail = CanBusMessageConverter.ConverteDecimalData(data, 2);
						if (IsReadingFromMicroControllerForRegisterValidation)
						{
							if (_machine?.LoadCellOneValueAccordingToTheStateMachine?.ContainsKey(SystemState) == true)
							{
								listOfValues.Add((_machine.LoadCellOneValueAccordingToTheStateMachine[SystemState].LoadCellThresholdWarning, ThresholdLC1Warning));
								listOfValues.Add((_machine.LoadCellOneValueAccordingToTheStateMachine[SystemState].LoadCellThresholdFail, ThresholdLC1Fail));
							}
							CentralMicroControllerAckRegistersTable[33] = true;
						}
						break;

					case 34:
						LC1LowRange = CanBusMessageConverter.ConverteDecimalData(data, 0);
						LC1HighRange = CanBusMessageConverter.ConverteDecimalData(data, 2);
						if (IsReadingFromMicroControllerForRegisterValidation)
						{
							if (_machine?.LoadCellOneValueAccordingToTheStateMachine?.ContainsKey(SystemState) == true)
							{
								listOfValues.Add((_machine.LoadCellOneValueAccordingToTheStateMachine[SystemState].LoadCellLowRangeLimit, LC1LowRange));
								listOfValues.Add((_machine.LoadCellOneValueAccordingToTheStateMachine[SystemState].LoadCellHighRangeLimit, LC1HighRange));
							}
							CentralMicroControllerAckRegistersTable[34] = true;
						}
						break;

					case 35:
						SystemState = (MessageStateId)(Convert.ToInt32(communicationData.CanBusOneEventArgs.Id) & (Int32)Mask.CAN_ID_STATE_MASK);
						remoteControlTimingToFactorIncrement++;
						if (PreviousSystemState != SystemState || (remoteControlTimingToFactorIncrement >= remoteControlTimingToFactor))
						{
							SendSystemStateToRemoteControl(SystemState);
							if (SystemState != MessageStateId.CAN_ID_STATE_EXCEPTION)
							{
								PreviousSystemState = SystemState;
							}
							remoteControlTimingToFactorIncrement = 0;
						}

						CMCUSystemStatusError = CanBusMessageConverter.ConvertStatusErrorData(data);

						if (!DiaphragmConditioning.IsDiaphragmReseting)
						{
							_machine.IsConsoleInAblationState = SystemState == MessageStateId.CAN_ID_STATE_TRANSITION || SystemState == MessageStateId.CAN_ID_STATE_ABLATION;
						}
						break;

					case 36:
						GetSolenoidValvesStatus(CanBusMessageConverter.ConvertValvesStatusData(data));
						CPLDFPINStatus = data[7];
						StartButtonPressed = (CPLDFPINStatus & (byte)CanBusMessageDefinition.CPLDFPINStatus.StartButton) != 0;
						StopButtonPressed = (CPLDFPINStatus & (byte)CanBusMessageDefinition.CPLDFPINStatus.StopButton) == 0;
						StartFootSwitchOn = (CPLDFPINStatus & (byte)CanBusMessageDefinition.CPLDFPINStatus.StartFootSwitch) != 0;
						StopFootSwitchOn = (CPLDFPINStatus & (byte)CanBusMessageDefinition.CPLDFPINStatus.StopFootSwitch) != 0;
						break;

					case 48:
						PatientMicroControllerFirmwareVersion = CanBusMessageConverter.ConverteInfoData(data, 0);
						PatientMicroControllerBootLoaderFirmwareVersion = CanBusMessageConverter.ConverteInfoData(data, 2);
						break;

					case 49:
						PMCUSystemStatusErrorCode = CanBusMessageConverter.ConvertStatusErrorData(data);

            if ((pMCUSystemStatusErrorCode & (Int64)PMCUStatusError.CatheterCableConnected) !=
                (Int64)PMCUStatusError.CatheterCableConnected)
            {
							ResetCatheterInfo();
            }
            
						break;

					case 50:
						if (!AllowFirmwareReading)
						{
							if (data != null)
							{
								UpdateCatheterInfo(data);

								IsCatheterExpirationDateUpdated = true;

								if (IsCatheterLastUseDateUpdated && SentCatheterLastUseDay != 0 && SentCatheterLastUseMonth != 0 
                    && SentCatheterLastUseYear != 0 && _canManageRTRCatheterMessage)
								{
									// Invoke ManageRTRCatheterMessage if the LastUsedDate is Updated (Start Validating Catheter and acknowledge the console)
									// Console has an issue that would not send RTR message if we send multiple Acknowledge messages in 50 ms
                  _canManageRTRCatheterMessage = false;
									ManageRTRCatheterMessage(data, communicationData, e.ID, false);
									// Request to read Catheter Firmware version 
									Console.ReadFromMicroController(MessageStateId.CAN_ID_STATE_IDLE, CatheterFirmwareVersionId);
                  Task.Delay(60).ContinueWith(_ => _canManageRTRCatheterMessage = true);
								}
							}
						}
						break;

					case 51:
						if (!AllowFirmwareReading)
						{
							if (IsCatheterExpirationDateUpdated)
							{
								if (data != null)
								{
									CatheterLastUseHour = data[0];
									CatheterLastUseDay = data[1];
									CatheterLastUseMonth = data[2];
									CatheterLastUseYear = CanBusMessageConverter.ConverteCatheterInfoData(data, 3);

									SentCatheterLastUseHour = data[0];
									SentCatheterLastUseDay = data[1];
									SentCatheterLastUseMonth = data[2];
									SentCatheterLastUseYear = CanBusMessageConverter.ConverteCatheterInfoData(data, 3);

									if (CatheterLastUseDay == 0 || CatheterLastUseMonth == 0 || CatheterLastUseYear == 0)   //Emily changed for SCB-318
									{
										CatheterLastUseDate = DateTime.Now;

										CatheterLastUseHour = CatheterLastUseDate.Hour;
										CatheterLastUseDay = CatheterLastUseDate.Day;
										CatheterLastUseMonth = CatheterLastUseDate.Month;
										CatheterLastUseYear = CatheterLastUseDate.Year;
										CatheterLastUseDate = new DateTime(CatheterLastUseYear, CatheterLastUseMonth, CatheterLastUseDay, CatheterLastUseHour, 0, 0, 0);

										if (_machine?.Catheter != null)
										{
											_machine.Catheter.CatheterLastUseHour = CatheterLastUseHour;
											_machine.Catheter.CatheterLastUseDay = CatheterLastUseDay;
											_machine.Catheter.CatheterLastUseMonth = CatheterLastUseMonth;
											_machine.Catheter.CatheterLastUseYear = CatheterLastUseYear;
										}

										if (communicationData?.CanBusOneEventArgs != null)
										{
											SendRequestedDataAsync(communicationData.CanBusOneEventArgs.Id, (uint)e.ID, true, true);
										}
									}
									NumberOfInjections = CanBusMessageConverter.ConverteCatheterInfoData(data, 5);
									IsCatheterLastUseDateUpdated = true;
									if (_machine?.Catheter != null)
									{
										_machine.Catheter.CatheterLastUseHour = CatheterLastUseHour;
										_machine.Catheter.CatheterLastUseDay = CatheterLastUseDay;
										_machine.Catheter.CatheterLastUseMonth = CatheterLastUseMonth;
										_machine.Catheter.CatheterLastUseYear = CatheterLastUseYear;
									}
								}
							}
						}
						break;

					case 52:
						TargetBalloonPressure = CanBusMessageConverter.ConverteNegativDecimalData(data, 0);
						if (IsReadingFromMicroControllerForRegisterValidation)
						{
							PatientMicroControllerRegisterIDSDynamicTable.Remove(52);
							if (_machine?.PatientMicroControllerBalloonPressureRegulatorValueAccordingToTheStateMachine?.ContainsKey(SystemState) == true)
							{
								listOfValues.Add((_machine.PatientMicroControllerBalloonPressureRegulatorValueAccordingToTheStateMachine[SystemState].TargetBalloonPressure, TargetBalloonPressure));
							}
							PatientMicroControllerAckRegistersTable[52] = true;
						}
						break;

					case 53:
						ThresholdForCP1High = CanBusMessageConverter.ConverteNegativDecimalData(data, 0);
						ThresholdForOuterBallonPressure = CanBusMessageConverter.ConverteNegativDecimalData(data, 2);
						ThresholdForInnerBallonPressureLow = CanBusMessageConverter.ConverteNegativDecimalData(data, 4);
						if (IsReadingFromMicroControllerForRegisterValidation)
						{
							PatientMicroControllerRegisterIDSDynamicTable.Remove(53);

							if (_machine?.PatientMicroControllerPIDValueAccordingToTheStateMachine?.ContainsKey(SystemState) == true)
							{
								// here i am waiting for the threshold these code have to be updated
								listOfValues.Add((_machine.PatientMicroControllerPIDValueAccordingToTheStateMachine[SystemState].PGain, PGain));
								listOfValues.Add((_machine.PatientMicroControllerPIDValueAccordingToTheStateMachine[SystemState].IGain, IGain));
								listOfValues.Add((_machine.PatientMicroControllerPIDValueAccordingToTheStateMachine[SystemState].DGain, DGain));
								listOfValues.Add((_machine.PatientMicroControllerPIDValueAccordingToTheStateMachine[SystemState].Offset, PIDOffset));
							}
							PatientMicroControllerAckRegistersTable[53] = true;
						}
						break;

					case 54:
						ThresholdForCTC1High = CanBusMessageConverter.ConverteNegativDecimalData(data, 0);
						ThresholdForCTC2High = CanBusMessageConverter.ConverteNegativDecimalData(data, 2);
						ThawingTemperatureSetPoint = CanBusMessageConverter.ConverteNegativDecimalData(data, 2);
						LowerBloodThreshold = (short)CanBusMessageConverter.ConverteDecimalData(data, 4);
						UpperBloodThreshold = (short)CanBusMessageConverter.ConverteDecimalData(data, 6);

						if (IsReadingFromMicroControllerForRegisterValidation)
						{
							PatientMicroControllerRegisterIDSDynamicTable.Remove(54);

							if (_machine?.ThermocoupleOneValueAccordingToTheStateMachine?.ContainsKey(SystemState) == true)
							{
								listOfValues.Add((_machine.ThermocoupleOneValueAccordingToTheStateMachine[SystemState].ThawingTemperature, ThresholdForCTC1High));
							}
							PatientMicroControllerAckRegistersTable[54] = true;
						}
						break;

					case 55:
						PatientPGain = CanBusMessageConverter.ConverteDecimalData(data, 0);
						PatientIGain = CanBusMessageConverter.ConverteDecimalData(data, 2);
						PatientDGain = CanBusMessageConverter.ConverteDecimalData(data, 4);
						PatientPIDOffset = CanBusMessageConverter.ConverteDecimalData(data, 6);
						if (IsReadingFromMicroControllerForRegisterValidation)
						{
							PatientMicroControllerRegisterIDSDynamicTable.Remove(55);
							if (_machine?.PatientMicroControllerPIDValueAccordingToTheStateMachine?.ContainsKey(SystemState) == true)
							{
								listOfValues.Add((_machine.PatientMicroControllerPIDValueAccordingToTheStateMachine[SystemState].PGain, PatientPGain));
								listOfValues.Add((_machine.PatientMicroControllerPIDValueAccordingToTheStateMachine[SystemState].IGain, PatientIGain));
								listOfValues.Add((_machine.PatientMicroControllerPIDValueAccordingToTheStateMachine[SystemState].DGain, PatientDGain));
								listOfValues.Add((_machine.PatientMicroControllerPIDValueAccordingToTheStateMachine[SystemState].Offset, PatientPIDOffset));
							}
							PatientMicroControllerAckRegistersTable[55] = true;
						}
						break;

					case 56:
						CatheterFirmwareVersion = CanBusMessageConverter.ConverteInfoData(data, 0);
						break;

					case 57:
						RampUpTimeByStep = CanBusMessageConverter.ConvertRampUpTimeAndRampDownTimeByStepData(data, 0);
						PressureRampUpValue = CanBusMessageConverter.ConvertRampUpPressureAndRampDownPressureByStepData(data, 2);
						RampDownTimeByStep = CanBusMessageConverter.ConvertRampUpTimeAndRampDownTimeByStepData(data, 4);
						PressureRampDownValue = CanBusMessageConverter.ConvertRampUpPressureAndRampDownPressureByStepData(data, 6);
						break;

					case 59:
						SendInit();
						break;

					case 60:
						ModuleKey = CanBusMessageConverter.ConvertModuleKeyData(data);
						UpgradeStatus = CanBusMessageConverter.ConvertUpgradeStatusData(data);
						break;
				}
			}
			else
			{
				HandleRTRMessage(communicationData, e);
			}
		}

    private void ResetCatheterInfo()
    {
      IsCatheterLastUseDateUpdated = false;
      IsCatheterExpirationDateUpdated = false;
      IsCatheterValid = false;

      SentCatheterLastUseHour = 0;
      SentCatheterLastUseDay = 0;
      SentCatheterLastUseMonth = 0;
      SentCatheterLastUseYear = 0;
      _canManageRTRCatheterMessage = true;
    }

		private void HandleRTRMessage(ICanBusCommunication communicationData, RegisterValuesEventArgs e)
		{
			if (communicationData.CanBusOneEventArgs.Falgs != (int)FrameType.Remote)
				return;

			if (e != null && e.ID == 50)
			{
				var data = communicationData.CanBusOneEventArgs.Data;
				ManageRTRCatheterMessage(data, communicationData, e.ID);
			}
			// ID == 51 has been handled in non-RTR message; ID==58 is Boot loader related, don't handle it now    
			else if (IsCatheterValid && (e != null && e.ID != 51 && e.ID != 58))
			{
				// we are using RTR so we have to ansewr with the same ID and to build the data we will use local id
				if (communicationData?.CanBusOneEventArgs != null)
				{
					SendRequestedDataAsync(communicationData.CanBusOneEventArgs.Id, (uint)e.ID);
				}
			}
		}

		private void CanTwoRegisterChangedEventHandler(object sender, RegisterValuesEventArgs e)
		{
			ICanBusCommunication communicationData = sender as ICanBusCommunication;
			byte[] data = null;
			if (communicationData != null && communicationData.CanBusTwoEventArgs.Data != null)
			{
				data = communicationData.CanBusTwoEventArgs.Data;
			}

			switch (e.ID)
			{
				case 11:
					RepeaterFirmware = CanBusMessageConverter.ConverteInfoData(data, 0);
					ICBFirmware = CanBusMessageConverter.ConverteInfoData(data, 2);
					RepeaterBootLoaderFirmware = CanBusMessageConverter.ConverteInfoData(data, 4);
					ICBBootLoaderFirmwareVersion = CanBusMessageConverter.ConverteInfoData(data, 6);
					break;
				case 24:
					RemoteControlFirmware = CanBusMessageConverter.ConverteInfoData(data, 0);
					RemoteControlBootLoaderFirmwareVersion = CanBusMessageConverter.ConverteInfoData(data, 2);
					break;
				case 58:
					AnswerRTRBootDataFORICBOrReapeter(communicationData);
					break;
				case 59:
					SendInitFORICBOrReapeter();
					break;
				case 60:
					ModuleKey = CanBusMessageConverter.ConvertModuleKeyData(data);
					UpgradeStatus = CanBusMessageConverter.ConvertUpgradeStatusData(data);
					break;
			}
		}

		private void PressureChanged(object sender, PressureTransducerEventArgs e)
		{
			ResetCanOneStopWatch();

			var communicationData = sender as ICanBusCommunication;
			if (communicationData?.CanBusOneEventArgs != null)
			{
				byte[] data = communicationData.CanBusOneEventArgs.Data;
				switch (e.Type)
				{
					case PressureTransducerEventArgs.PressureType.TP:
						PT1Reading = CanBusMessageConverter.ConverteDecimalData(data, 0);
						PT2Reading = CanBusMessageConverter.ConverteDecimalData(data, 2);
						PT3Reading = CanBusMessageConverter.ConverteDecimalData(data, 4);
						PT4Reading = CanBusMessageConverter.ConverteDecimalData(data, 6);
						break;
					case PressureTransducerEventArgs.PressureType.CP:
						CP1Reading = CanBusMessageConverter.ConverteNegativDecimalData(data, 0);
						CP2Reading = CanBusMessageConverter.ConverteNegativDecimalData(data, 2);
						TIPReading = CanBusMessageConverter.ConverteNegativDecimalData(data, 4);
						PatientPIDDutyCycle = CanBusMessageConverter.ConverteDecimalData(data, 6);
						break;
				}
			}
		}

		private void TemperatureChanged(object sender, ThermocoupleEventArgs e)
		{
			ResetCanOneStopWatch();

			var communicationData = (ICanBusCommunication)sender;
			if (communicationData?.CanBusOneEventArgs != null && e != null)
			{
				byte[] data = communicationData.CanBusOneEventArgs.Data;
				switch (e.Type)
				{
					case ThermocoupleEventArgs.ThermocoupleType.TC:
						TC1Reading = CanBusMessageConverter.ConverteNegativDecimalData(data, 0);
						TC2Reading = CanBusMessageConverter.ConverteNegativDecimalData(data, 2);
						PMCUCJReading = CanBusMessageConverter.ConverteNegativDecimalData(data, 4);
						break;
					case ThermocoupleEventArgs.ThermocoupleType.TS:
						TS1Reading = CanBusMessageConverter.ConverteNegativDecimalData(data, 4);
						CMCUCJReading = CanBusMessageConverter.ConverteNegativDecimalData(data, 2);
						break;
				}
			}
		}

		private void PressureSwitchEvent(object sender, PressureSwitchEventArgs e)
		{
			ResetCanOneStopWatch();
			if (sender is ICanBusCommunication communicationData && communicationData.CanBusOneEventArgs != null)
			{
				byte[] data = communicationData.CanBusOneEventArgs.Data;
				PS1Reading = CanBusMessageConverter.ConverteDecimalData(data, 0);
				PS2Reading = CanBusMessageConverter.ConverteDecimalData(data, 2);
			}
		}

		private void FlowMeterEvent(object sender, FlowMeterEventArgs e)
		{
			var communicationData = sender as ICanBusCommunication;
			if (communicationData?.CanBusOneEventArgs != null)
			{
				byte[] data = communicationData.CanBusOneEventArgs.Data;
				switch (e.FlowMeter.ID)
				{
					case 0:
						FM1Reading = CanBusMessageConverter.ConverteDecimalDataFM1(data, 0);
						PT5Reading = CanBusMessageConverter.ConverteDecimalData(data, 2);
						PIDDutyCycle = CanBusMessageConverter.ConverteDecimalData(data, 4);
						break;
				}
			}
		}

		/// <summary>
		/// Handler for the Load Changed event is raised.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LoadCellEvent(object sender, LoadCellEventArgs e)
		{
			ResetCanOneStopWatch();
			if (sender is ICanBusCommunication communicationData && communicationData.CanBusOneEventArgs != null)
			{
				byte[] data = communicationData.CanBusOneEventArgs.Data;
				LC1Reading = CanBusMessageConverter.ConverteDecimalData(data, 0);
			}
		}

		/// <summary>
		/// Handler for the blood detection event is raised.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void BloodDetectorEvent(object sender, BloodDetectorEventArgs e)
		{
			ResetCanOneStopWatch();
			if (sender is ICanBusCommunication communicationData && communicationData.CanBusOneEventArgs != null)
			{
				byte[] data = communicationData.CanBusOneEventArgs.Data;

				BloodDetectionType = (int)CanBusMessageConverter.ConverteDecimalData(data, 0);
				BloodDetecorImValue = (int)CanBusMessageConverter.ConverteDecimalData(data, 4);
			}
		}

		private void RemoteControlMembraneSwitchStateEventArgs(object sender, RemoteControlMembraneSwitchStateEventArgs e)
		{
			//FieldServiceTrace.Log("Fired [Remote Control Event].");
		}

		private void BloodPressureSensorConnectionChanged(object sender, BloodPressureSensorEventArgs e)
		{
			ICanBusCommunication communicationData = sender as ICanBusCommunication;

			byte[] data = communicationData.CanBusTwoEventArgs.Data;

			switch (e.ID)
			{
				case 1:

					uint status = data[0];

					if (!IsBloodPressureSensorConnected && (status & (uint)SensorConnectionStatus.Pressure) == (uint)SensorConnectionStatus.Pressure)
					{
						IsBloodPressureSensorConnected = true;
					}
					else if (IsBloodPressureSensorConnected && (status & (uint)SensorConnectionStatus.Pressure) != (uint)SensorConnectionStatus.Pressure)
					{
						IsBloodPressureSensorConnected = false;
					}


					if (!IsMultiEtsSesnorConnected && (status & (uint)SensorConnectionStatus.ETSMulti) == (uint)SensorConnectionStatus.ETSMulti)
					{
						IsMultiEtsSesnorConnected = true;
					}
					else if (IsMultiEtsSesnorConnected && (status & (uint)SensorConnectionStatus.ETSMulti) != (uint)SensorConnectionStatus.ETSMulti)
					{
						IsMultiEtsSesnorConnected = false;
					}
					break;

				case 7:
					double[] _bloodPressureValue = new double[4];
					CanBusMessageConverter.ConverteBloodPressureData(data, out _bloodPressureValue);
					BloodPressureValue = _bloodPressureValue;

					break;
			}
		}

		/// <summary>
		/// Handler for ECG Event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void EcgArgsChanged(object sender, EcgEventArgs e)
		{
			if (CanTwoStopWatchCommunicationLost != null)
				CanTwoStopWatchCommunicationLost.Restart();

			if (ICBStopWatchDisconnection != null)
			{
				ICBStopWatchDisconnection.Restart();
			}

			if (IsCanTwoInError)
			{
				if (CanTwoStopWatchCommunicationLost != null)
				{
					CanTwoStopWatchCommunicationLost.Restart();
				}
				IsCanTwoInError = false;
			}

			ICanBusCommunication communicationData = sender as ICanBusCommunication;

			byte[] data = communicationData.CanBusTwoEventArgs.Data;

			switch (e.ID)
			{
				case 8:

					EcgChannel1And2Reading = CanBusMessageConverter.ConverteECGDecimalData(data, 0, 100.0);

					// THE diaphragm graph 
					EcgChannel3And4Reading = CanBusMessageConverter.ConverteECGDecimalData(data, 2, 100.0);

					//ESO Temp

					double temporayEsoValue = CanBusMessageConverter.ConverteECGDecimalData(data, 4);
					EtsSesnor13 = temporayEsoValue;

					if (temporayEsoValue == -100)
					{
						temporayEsoValue = 100;
					}
					EcgChannel5And6Reading = temporayEsoValue;

					if (IsMultiEtsSesnorConnected && !AreSensorsInPlayBackMode)
						AnalyseEsophagusTemperature();


					//The diaphragm %
					EcgChannel7And8Reading = CanBusMessageConverter.ConverteNegativDecimalData(data, 6);


					break;

				case 9:
					EcgChannel9And10Reading = CanBusMessageConverter.ConverteDecimalData(data, 0);
					ChannelTipReading = CanBusMessageConverter.ConverteDecimalData(data, 2);
					ChannelAccelerometerReading = CanBusMessageConverter.ConverteDecimalData(data, 4);

					EcgChannel9And10Readings.Add(EcgChannel9And10Reading);
					ChannelTipReadings.Add(ChannelTipReading);
					ChannelAccelerometerReadings.Add(ChannelAccelerometerReading);
					break;
			}
		}

		private void ProbeSesnorChanged(object sender, ProbeEventArgs e)
		{
			if (!IsMultiEtsSesnorConnected)
			{
				return;
			}

			var communicationData = (ICanBusCommunication)sender;
			byte[] data = communicationData.CanBusTwoEventArgs.Data;
			switch (e.ID)
			{
				case 5:
					EtsSesnor1 = data[0];
					EtsSesnor2 = data[1];
					EtsSesnor3 = data[2];
					EtsSesnor4 = data[3];
					EtsSesnor5 = data[4];
					EtsSesnor6 = data[5];
					EtsSesnor7 = data[6];
					EtsSesnor8 = data[7];
					break;
				case 6:
					EtsSesnor9 = data[0];
					EtsSesnor10 = data[1];
					EtsSesnor11 = data[2];
					EtsSesnor12 = data[3];
					break;
			}

			if (!AreSensorsInPlayBackMode)
			{
				AnalyseEsophagusTemperature();
			}
		}

		private void CatheterConnectedTimer_MicroTimerElapsed(object sender, MicroTimerEventArgs timerEventArgs)
		{
		}

		#endregion Event Handlers

		private void InitializeRegisterIDSDynamicTables()
		{
			foreach (var item in PatientMicroControllerAckRegistersTable)
			{
				if (PatientMicroControllerAckRegistersTable.Count > item.Key)
					PatientMicroControllerAckRegistersTable[item.Key] = false;
			}

			foreach (var item in CentralMicroControllerAckRegistersTable)
			{
				if (CentralMicroControllerAckRegistersTable.Count > item.Key)
					CentralMicroControllerAckRegistersTable[item.Key] = false;
			}
		}

		/// <summary>
		/// Initializes the Ack Registers table.
		/// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		private void InitializeAckRegistersTable()
		{
			#region CMCU

			CentralMicroControllerAckRegistersTable.Add(15, ackForRegisters15);
			CentralMicroControllerAckRegistersTable.Add(16, aAckForRegisters16);
			CentralMicroControllerAckRegistersTable.Add(17, ackForRegisters17);
			CentralMicroControllerAckRegistersTable.Add(18, ackForRegisters18);
			CentralMicroControllerAckRegistersTable.Add(19, acKForRegisters19);
			CentralMicroControllerAckRegistersTable.Add(20, acKForRegisters20);
			CentralMicroControllerAckRegistersTable.Add(21, acKForRegisters21);
			CentralMicroControllerAckRegistersTable.Add(22, acKForRegisters22);
			CentralMicroControllerAckRegistersTable.Add(23, acKForRegisters23);
			CentralMicroControllerAckRegistersTable.Add(24, acKForRegisters24);
			CentralMicroControllerAckRegistersTable.Add(25, acKForRegisters25);
			CentralMicroControllerAckRegistersTable.Add(26, acKForRegisters26);
			CentralMicroControllerAckRegistersTable.Add(27, acKForRegisters27);
			CentralMicroControllerAckRegistersTable.Add(28, acKForRegisters28);
			CentralMicroControllerAckRegistersTable.Add(29, acKForRegisters29);
			CentralMicroControllerAckRegistersTable.Add(30, acKForRegisters30);
			CentralMicroControllerAckRegistersTable.Add(31, acKForRegisters31);
			CentralMicroControllerAckRegistersTable.Add(32, acKForRegisters32);
			CentralMicroControllerAckRegistersTable.Add(33, acKForRegisters33);
			CentralMicroControllerAckRegistersTable.Add(34, acKForRegisters34);

			#endregion CMCU

			#region PMCU

			PatientMicroControllerAckRegistersTable.Add(52, acKForRegisters52);
			PatientMicroControllerAckRegistersTable.Add(53, acKForRegisters53);
			PatientMicroControllerAckRegistersTable.Add(54, acKForRegisters54);
			PatientMicroControllerAckRegistersTable.Add(55, acKForRegisters55);

			#endregion PMCU
		}

		/// <summary>
		/// Gets or sets the List of Central Micro controller register IDS Dynamic table's integer values
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public List<int> CentralMicroControllerRegisterIDSDynamicTable
		{
			get
			{
				{ lock (_myRegister_Lock) return centralMicroControllerRegisterIDSDynamicTable; }
			}
			set
			{
				{ lock (_myRegister_Lock) centralMicroControllerRegisterIDSDynamicTable = value; }
			}
		}

		/// <summary>
		/// Send initialization data for ICB or reapeter
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		private void SendInitFORICBOrReapeter()
		{
			_machine.SendBootMessageForICBOrReapeter(MessageStateId.CAN_ID_STATE_IDLE, (int)BootLoaderID.CAN_ID_BOOT_INIT, ASCIIToByteConverter.Initdata);
		}

		/// <summary>
		/// Answer RTR Boot Data FOR ICB Or Reapeter
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="communicationData">communication data</param>
		private void AnswerRTRBootDataFORICBOrReapeter(ICanBusCommunication communicationData)
		{

			var asciitobyteconverter = new ASCIIToByteConverter();

			if (asciitobyteconverter.CanSendEndTransmission)
			{
				_machine.SendBootMessageForICBOrReapeter(MessageStateId.CAN_ID_STATE_IDLE, (int)BootLoaderID.CAN_ID_BOOT_END, ASCIIToByteConverter.Initdata);

			}

			else
			{
				for (int i = 0; i < 8; i++)
				{

					Array.Clear(BootLoaderData, 0, 8);
					packetNumber = 0;
					BootLoaderData = asciitobyteconverter.GetPacket(out packetNumber);

					_machine.AnswerRTRBootMessageForICBOrReapeter(packetNumber, (int)communicationData.CanBusTwoEventArgs.Id, BootLoaderData);
				}
			}
		}

		private void SubscribeConsoleMonitorEvents()
		{
			_consoleMonitor.PropertyChanged -= ConsoleMonitorPropertyChanged;
			_consoleMonitor.PropertyChanged += ConsoleMonitorPropertyChanged;
		}

		private void ConsoleMonitorPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case nameof(_consoleMonitor.IsVacuumDisconnected):
					IsVacuumDisconnected = _consoleMonitor.IsVacuumDisconnected;
					break;
				default:
					break;
			}
		}
	}
}
