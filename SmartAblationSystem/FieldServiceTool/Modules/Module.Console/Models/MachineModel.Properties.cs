using Console;
using Module.Console.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using static Communication.CanBusMessageDefinition;

namespace Module.Console.Models
{
	/// <summary>
	/// Partial class for MachineModel - Public Properties.
	/// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
	/// </summary>
	public partial class MachineModel
	{
		/// <summary>
		/// Get the instance of Machine which defines many methods to interact with console  
		/// </summary>
		public Machine Console => _machine;

		#region Version Properties

		/// <summary>
		/// Gets or sets the central microController bootLoader firmware version. CMCUBoot
		/// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public int CentralMicroControllerBootLoaderFirmwareVersion
		{
			get => centralMicroControllerBootLoaderFirmwareVersion;
			set => SetProperty(ref centralMicroControllerBootLoaderFirmwareVersion, value);
		}

		/// <summary>
		/// Gets or sets the Central Micro Controller Firmware Version value.
		/// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public int CentralMicroControllerFirmwareVersion
		{
			get => centralMicroControllerFirmwareVersion;
			set => SetProperty(ref centralMicroControllerFirmwareVersion, value);
		}

		/// <summary>
		/// Gets or sets the CPLD bootLoader firmware version.
		/// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public int CpldFirmwareVersion
		{
			get => cpldFirmwareVersion;
			set => SetProperty(ref cpldFirmwareVersion, value);
		}

		/// <summary>
		/// Gets or sets the Patient Micro Controller Firmware Version value.
		/// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public int PatientMicroControllerFirmwareVersion
		{
			get => patientMicroControllerFirmwareVersion;
			set => SetProperty(ref patientMicroControllerFirmwareVersion, value);
		}

		/// <summary>
		/// Gets or sets the patient bootLoader firmware version.
		/// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public int PatientMicroControllerBootLoaderFirmwareVersion
		{
			get => patientMicroControllerBootLoaderFirmwareVersion;
			set => SetProperty(ref patientMicroControllerBootLoaderFirmwareVersion, value);
		}

		/// <summary>
		/// This property gets/sets the RemoteControlFirmwareDBVersion value.
		/// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public int RemoteControlFirmwareDBVersion
		{
			get => remoteControlFirmwareDBVersion;
			set => SetProperty(ref remoteControlFirmwareDBVersion, value);
		}


		/// <summary>
		/// Gets or sets the Repeater Firmware value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public int RepeaterFirmware
		{
			get => repeaterFirmware;
			set => SetProperty(ref repeaterFirmware, value);
		}

		/// <summary>
		/// Gets or sets the ICB Firmware value.
		/// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public int ICBFirmware
		{
			get => iCBFirmware;
			set => SetProperty(ref iCBFirmware, value);
		}


		/// <summary>
		/// Gets or sets  the repeater bootLoader firmware version
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public int RepeaterBootLoaderFirmware
		{
			get => repeaterBootLoaderFirmware;
			set => SetProperty(ref repeaterBootLoaderFirmware, value);
		}

		/// <summary>
		/// Gets or sets the ICB bootloader firmware version
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public int ICBBootLoaderFirmwareVersion
		{
			get => iCBBootLoaderFirmwareVersion;
			set => SetProperty(ref iCBBootLoaderFirmwareVersion, value);
		}

		/// <summary>
		/// Gets or sets  the remote control firmware version
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public int RemoteControlFirmware
		{
			get => remoteControlFirmware;
			set => SetProperty(ref remoteControlFirmware, value);
		}


		/// <summary>
		/// Gets or sets  the remote control bootLoader firmware version
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public int RemoteControlBootLoaderFirmwareVersion
		{
			get => remoteControlBootLoaderFirmwareVersion;
			set
			{
				if (RemoteControlFirmware == 4098)
					SetProperty(ref remoteControlBootLoaderFirmwareVersion, 0);
				else
					SetProperty(ref remoteControlBootLoaderFirmwareVersion, value);
			}
		}

		#endregion Version Properties

		public RemoteControlFSM RemoteControlFSM
		{
			get => remoteControlFSM;
			set => SetProperty(ref remoteControlFSM, value);
		}

		/// <summary>
		/// Gets or sets a value indicating whether the Catheter is valid or not.
		/// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsCatheterValid
		{
			get => isCatheterValid;
			set => SetProperty(ref isCatheterValid, value);
		}

		/// <summary>
		/// Gets or sets a value indicating the Catheter cable is connected or not.
		/// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsCatheterCableConnected
		{
			get => isCatheterCableConnected;
			set => SetProperty(ref isCatheterCableConnected, value);
		}


		/// <summary>
		/// Gets or sets a value indicating the vacuum is connected or not.
		/// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsVacuumDisconnected
		{
			get => isVacuumDisconnected;
			set => SetProperty(ref isVacuumDisconnected, value);
		}

		/// <summary>
		/// Gets or sets  a value indicating whether used for Engineering
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsUsedForEngineering
		{
			get => isUsedForEngineering;
			set => SetProperty(ref isUsedForEngineering, value);
		}

		/// <summary>
		/// Gets or sets  a value indicating whether DAS Ballon is using or not
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsSystemUsingDASBalloon
		{
			get => isSystemUsingDASBalloon;
			set => SetProperty(ref isSystemUsingDASBalloon, value);
		}

		/// <summary>
		/// Gets or sets an int value for Engineering catheter signature
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public int EngineeringCatheterSignature => _machine.ServiceDevices.EngineeringCatheterSignature;

		/// <summary>
		/// Gets or sets or the boot loader data.
		/// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public byte[] BootLoaderData
		{
			get => bootLoaderData;
			set => bootLoaderData = value;
		}

		/// <summary>
		/// Gets or sets the blood detector impedance
		/// . Safety classification: Death or SERIOUS INJURY is possible (IEC 62304 Class C).
		/// </summary>
		/// <id>SF-SDS-0003</id>
		public int BloodDetecorImValue
		{
			get => bloodDetecorImValue;
			set => SetProperty(ref bloodDetecorImValue, value);
		}

		public Stopwatch ICBStopWatchDisconnection
		{
			get => iCBStopWatchDisconnection;
			set => iCBStopWatchDisconnection = value;
		}

		public bool IsCanOneInError
		{
			get => isCanOneInError;
			set => isCanOneInError = value;
		}

		public bool IsCanTwoInError
		{
			get => isCanTwoInError;
			set => SetProperty(ref isCanTwoInError, value);
		}

		public bool IsBootLoaderUpdatingFirmware
		{
			get => isBootLoaderUpdatingFirmware;
			set => SetProperty(ref isBootLoaderUpdatingFirmware, value);
		}

		/// <summary>
		/// Gets or sets the ECG Channel 1 and 2 Reading value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double EcgChannel1And2Reading
		{
			get => ecgChannel1And2Reading;
			set => SetProperty(ref ecgChannel1And2Reading, value);
		}

		/// <summary>
		/// Gets or sets the Dms detection threshold
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		private double _DmsDetectionThreshold = 0.003;

		/// <summary>
		/// Gets or sets the Minimum Diaphragm Movement value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double DMSDetectionThreshold
		{
			get => _DmsDetectionThreshold;
			set => SetProperty(ref _DmsDetectionThreshold, value);
		}

		/// <summary>
		/// Gets or sets the ECG Channel 3 and 4 Reading value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double EcgChannel3And4Reading
		{
			get => ecgChannel3And4Reading;

			set
			{
				SetProperty(ref ecgChannel3And4Reading, value);

				if ((PreviousSystemState == MessageStateId.CAN_ID_STATE_INFLATION || PreviousSystemState == MessageStateId.CAN_ID_STATE_THAWING) &&
						(SystemState == MessageStateId.CAN_ID_STATE_TRANSITION || SystemState == MessageStateId.CAN_ID_STATE_ABLATION))
				{
					diaphragmMovementTable.Clear();
					diaphragmMovementTablePeakToPeak.Clear();

				}

				if (ecgChannel3And4Reading < DMSDetectionThreshold)
				{
					diaphragmMovementTable.Add(ecgChannel3And4Reading);
				}
				else
				{
					diaphragmMovementTable.Clear();
				}

				if (SystemState != MessageStateId.CAN_ID_STATE_EXCEPTION)
				{
					PreviousSystemState = SystemState;
				}

				if (SystemState == MessageStateId.CAN_ID_STATE_TRANSITION || SystemState == MessageStateId.CAN_ID_STATE_ABLATION)
				{

					diaphragmMovementTablePeakToPeak.Add(ecgChannel3And4Reading);

					if (diaphragmMovementTablePeakToPeak.Count > diaphragmMovementCompterOneSecondeValuePeakToPeak)
					{
						double diaphragmMovementTableFisrtPeak = 0;
						double diaphragmMovementTableSecondPeak = 0;

						foreach (double _element in diaphragmMovementTablePeakToPeak)
						{
							if (_element > diaphragmMovementTableFisrtPeak)
							{
								if (diaphragmMovementTableFisrtPeak != diaphragmMovementTableSecondPeak)
								{
									diaphragmMovementTableSecondPeak = diaphragmMovementTableFisrtPeak;
								}
								diaphragmMovementTableFisrtPeak = _element;
							}
						}

						if (diaphragmMovementTableSecondPeak == 0)
						{
							diaphragmMovementTableSecondPeak = diaphragmMovementTableFisrtPeak;
						}

						double PeakToPeakAverageValue = (diaphragmMovementTableSecondPeak + diaphragmMovementTableFisrtPeak) / 2;

						if (PeakToPeakAverageValue > MaximumAveragePacingLevel && !DiaphragmConditioning.IsDiaphragmReseting)
							MaximumAveragePacingLevel = PeakToPeakAverageValue;

						diaphragmMovementTablePeakToPeak.Clear();
					}
				}

				//1 second is 25 * 40ms
				if (diaphragmMovementTable.Count > diaphragmMovementCompterOneSecondeValue)
				{
					diaphragmMovementTable.Clear();
				}

				if (ecgChannel3And4Reading > MaxEcgChannel3And4Reading)
				{
					MaxEcgChannel3And4Reading = ecgChannel3And4Reading;
				}

				if (Ecgs3And4StopWatch != null && Ecgs3And4StopWatch.ElapsedMilliseconds >= ecg3An4RefreshTime)
				{
					RaisePropertyChanged(nameof(MaxEcgChannel3And4Reading));
					Ecgs3And4StopWatch.Restart();
				}

				if (SystemState != MessageStateId.CAN_ID_STATE_TRANSITION && SystemState != MessageStateId.CAN_ID_STATE_ABLATION)
					MaximumAveragePacingLevel = 0;
			}
		}

		public bool IsBloodPressureSensorConnected
		{
			get => isBloodPressureSensorConnected;
			set => SetProperty(ref isBloodPressureSensorConnected, value);
		}

		public double[] BloodPressureValue
		{
			get
			{
#if Simulator
        return new double[] { 35, 35, 35, 35 };
#endif
				{ lock (_bloodPressure_Lock) return bloodPressureValue; }
			}
			set
			{
				{ lock (_bloodPressure_Lock) SetProperty(ref bloodPressureValue, value); }
			}
		}

		/// <summary>
		/// Gets or sets the Max ECG Channel 3 and 4 Reading value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double MaxEcgChannel3And4Reading
		{
			get => maxEcgChannel3And4Reading;
			set
			{
				if (SensorReadingManager.AreSensorsConnected)
					SetProperty(ref maxEcgChannel3And4Reading, value);
			}
		}

		/// <summary>
		/// Gets or sets the Maximum Average Pacing Level value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <id>SF-SDS-0105</id>
		public double MaximumAveragePacingLevel
		{
			get
			{
				if (DiaphragmConditioning.IsDiaphragmReseting)
					return 0;
				return maximumAveragePacingLevel;
			}

			set
			{

				if (value > PacingLevelMaxvalue)
				{
					maximumAveragePacingLevel = PacingLevelMaxvalue;
				}

				else
				{
					maximumAveragePacingLevel = value;
				}

				RaisePropertyChanged();
			}
		}

		/// <summary>
		/// Gets or sets the ECG Channel 5 and 6 Reading value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double EcgChannel5And6Reading
		{
			get
			{
				if (IsMultiEtsSesnorConnected)
					return MinimumTemperature;
				return ecgChannel5And6Reading;
			}
			set
			{
				SetProperty(ref ecgChannel5And6Reading, value);

#if Simulator
        if (IsMultiEtsSesnorConnected && !AreSensorsInPlayBackMode)
          AnalyseEsophagusTemperature();
#endif

			}
		}

		public bool IsMultiEtsSesnorConnected
		{
			get =>
				//#if Simulator
				//                return  true;   //false; //
				//#endif
				isMultiEtsSesnorConnected;
			set => SetProperty(ref isMultiEtsSesnorConnected, value);
		}

		/// <summary>
		/// This property gets/sets the Time The ETS sesnor value
		/// . Safety classification: Non-SERIOUS INJURY is possible (IEC 62304 Class B).
		/// </summary>
		/// <id>SF-SDS-0047</id>
		public double EtsSesnor13
		{
			get => etsSesnor13;
			set => SetProperty(ref etsSesnor13, value);
		}

		/// <summary>
		/// Gets or sets a value indicating whether the Sensors are in playback mode or not
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool AreSensorsInPlayBackMode
		{
			get => areSensorsInPlayBackMode;
			set
			{
				try
				{
					SetProperty(ref areSensorsInPlayBackMode, value);
				}
				catch (Exception ex)
				{
					// TODO
					ex.ToString();
				}
			}
		}

		/// <summary>
		/// This property gets/sets the Time The ETS sesnor value
		/// . Safety classification: Non-SERIOUS INJURY is possible (IEC 62304 
		/// .
		/// </summary>
		public double EtsSesnor1
		{
			get => etsSesnor1;
			set => SetProperty(ref etsSesnor1, value);
		}

		/// <summary>
		/// This property gets/sets the Time The ETS sesnor value
		/// . Safety classification: Non-SERIOUS INJURY is possible (IEC 62304 Class B).
		/// </summary>
		/// <id>SF-SDS-0036</id>
		public double EtsSesnor2
		{
			get => etsSesnor2;
			set => SetProperty(ref etsSesnor2, value);
		}

		/// <summary>
		/// This property gets/sets the Time The ETS sesnor value
		/// . Safety classification: Non-SERIOUS INJURY is possible (IEC 62304 Class B).
		/// </summary>
		/// <id>SF-SDS-0037</id>
		public double EtsSesnor3
		{
			get => etsSesnor3;
			set => SetProperty(ref etsSesnor3, value);
		}

		/// <summary>
		/// This property gets/sets the Time The ETS sensor value
		/// . Safety classification: Non-SERIOUS INJURY is possible (IEC 62304 Class B).
		/// </summary>
		/// <id>SF-SDS-0038</id>
		public double EtsSesnor4
		{
			get => etsSesnor4;
			set => SetProperty(ref etsSesnor4, value);
		}

		/// <summary>
		/// This property gets/sets the Time The ETS sensor value
		/// . Safety classification: Non-SERIOUS INJURY is possible (IEC 62304 Class B).
		/// </summary>
		/// <id>SF-SDS-0039</id>
		public double EtsSesnor5
		{
			get => etsSesnor5;
			set => SetProperty(ref etsSesnor5, value);
		}

		/// <summary>
		/// This property gets/sets the Time The ETS sensor value
		/// . Safety classification: Non-SERIOUS INJURY is possible (IEC 62304 Class B).
		/// </summary>
		/// <id>SF-SDS-0040</id>
		public double EtsSesnor6
		{
			get => etsSesnor6;
			set => SetProperty(ref etsSesnor6, value);
		}

		/// <summary>
		/// This property gets/sets the Time The ETS sensor value
		/// . Safety classification: Non-SERIOUS INJURY is possible (IEC 62304 Class B).
		/// </summary>
		/// <id>SF-SDS-0041</id>
		public double EtsSesnor7
		{
			get => etsSesnor7;
			set => SetProperty(ref etsSesnor7, value);
		}

		/// <summary>
		/// This property gets/sets the Time The ETS sensor value
		/// . Safety classification: Non-SERIOUS INJURY is possible (IEC 62304 Class B).
		/// </summary>
		/// <id>SF-SDS-0042</id>
		public double EtsSesnor8
		{
			get => etsSesnor8;
			set => SetProperty(ref etsSesnor8, value);
		}

		/// <summary>
		/// This property gets/sets the Time The ETS sensor value
		/// . Safety classification: Non-SERIOUS INJURY is possible (IEC 62304 Class B).
		/// </summary>
		/// <id>SF-SDS-0043</id>
		public double EtsSesnor9
		{
			get => etsSesnor9;
			set => SetProperty(ref etsSesnor9, value);
		}

		/// <summary>
		/// This property gets/sets the Time The ETS sensor value
		/// . Safety classification: Non-SERIOUS INJURY is possible (IEC 62304 Class B).
		/// </summary>
		/// <id>SF-SDS-0044</id>
		public double EtsSesnor10
		{
			get => etsSesnor10;
			set => SetProperty(ref etsSesnor10, value);
		}

		/// <summary>
		/// This property gets/sets the Time The ETS sensor value
		/// . Safety classification: Non-SERIOUS INJURY is possible (IEC 62304 Class B).
		/// </summary>
		/// <id>SF-SDS-0045</id>
		public double EtsSesnor11
		{
			get => etsSesnor11;
			set => SetProperty(ref etsSesnor11, value);
		}

		/// <summary>
		/// This property gets/sets the Time The ETS sensor value
		/// . Safety classification: Non-SERIOUS INJURY is possible (IEC 62304 Class B).
		/// </summary>
		/// <id>SF-SDS-0046</id>
		public double EtsSesnor12
		{
			get => etsSesnor12;
			set => SetProperty(ref etsSesnor12, value);
		}

		/// <summary>
		/// This property gets/sets the list of sensors state value
		/// . Safety classification: Non-SERIOUS INJURY is possible (IEC 62304 Class B).
		/// </summary>
		/// <id>SF-SDS-0049</id>
		public List<int> ListOfSesnorsState
		{
			get => listOfSesnorsState;
			set => SetProperty(ref listOfSesnorsState, value);
		}

		/// <summary>
		/// Gets or sets the ECG Channel 7 and 8 Reading value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double EcgChannel7And8Reading
		{
			get => ecgChannel7And8Reading;
			set
			{
				SetProperty(ref ecgChannel7And8Reading, value);

				if (!IgnoreMinimumDiaphragmMovementValue)
					IsDiaphragmMovementDetected = DMSLogic.GetDMSState(ecgChannel7And8Reading, systemState);
				else
					IsDiaphragmMovementDetected = true;


			}
		}
		/// <summary>
		/// Gets or sets the Channel Tip value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double ChannelTipReading
		{
			get => channelTipReading;

			set => channelTipReading = value;
		}

		/// <summary>
		/// Gets or sets the Channel Accelerometer Reading value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double ChannelAccelerometerReading
		{
			get => channelAccelerometerReading;

			set => channelAccelerometerReading = value;
		}

		/// <summary>
		/// Gets or sets the ECG Channel 9 and 10 Readings List
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public List<double> EcgChannel9And10Readings
		{
			get
			{
				{ lock (_myVar_Lock) return ecgChannel9And10Readings; }
			}

			set
			{
				{ lock (_myVar_Lock) ecgChannel9And10Readings = value; }
			}
		}

		/// <summary>
		/// Gets or sets the Channel Tip Readings List
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public List<double> ChannelTipReadings
		{
			get
			{
				{ lock (_myVar_Lock) return channelTipReadings; }
			}

			set
			{
				{ lock (_myVar_Lock) channelTipReadings = value; }
			}
		}

		/// <summary>
		/// Gets or sets the Channel Accelerometer Readings List
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public List<double> ChannelAccelerometerReadings
		{
			get
			{
				{ lock (_myVar_Lock) return channelAccelerometerReadings; }
			}

			set
			{
				{ lock (_myVar_Lock) channelAccelerometerReadings = value; }
			}
		}

		/// <summary>
		/// Gets or sets the ECG Channel 9 and 10 Reading value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double EcgChannel9And10Reading
		{
			get => ecgChannel9And10Reading;
			set => ecgChannel9And10Reading = value;
		}

		/// <summary>
		/// Gets or sets Is Diaphragm Movement Detected value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsDiaphragmMovementDetected
		{
			get => isDiaphragmMovementDetected;

			set => SetProperty(ref isDiaphragmMovementDetected, value);
		}

		/// <summary>
		/// Gets or sets a value indicating whether ignore minimum diaphragm movement or not
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IgnoreMinimumDiaphragmMovementValue
		{
			get => ignoreMinimumDiaphragmMovementValue;

			set => SetProperty(ref ignoreMinimumDiaphragmMovementValue, value);
		}

		/// <summary>
		/// This property gets/sets the minimum temperature value.
		/// Safety classification: Non-SERIOUS INJURY is possible (IEC 62304 Class B).
		/// </summary>
		/// <id>SF-SDS-0050</id>
		public double MinimumTemperature
		{
			get => minimumTemperature;
			set => minimumTemperature = value;
		}

		/// <summary>
		/// Gets or sets the blood detection type.
		/// Safety classification: Death or SERIOUS INJURY is possible (IEC 62304 Class C).
		/// </summary>
		/// <id>SF-SDS-0004</id>
		public int BloodDetectionType
		{
			get => bloodDetectionType;
			set => SetProperty(ref bloodDetectionType, value);
		}

		public int CPLDErrorRegister
		{
			get => cPLDErrorRegister;
			set => SetProperty(ref cPLDErrorRegister, value);
		}

		/// <summary>
		/// Gets or sets the Catheter Last use hour value.
		/// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public int CatheterLastUseHour
		{
			get => catheterLastUseHour;
			set => catheterLastUseHour = value;
		}

		/// <summary>
		/// Gets or sets the Catheter Last Use Day value.
		/// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public int CatheterLastUseDay
		{
			get => catheterLastUseDay;
			set => SetProperty(ref catheterLastUseDay, value);
		}

		/// <summary>
		/// Gets or sets the Catheter Last Use Month value.
		/// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public int CatheterLastUseMonth
		{
			get => catheterLastUseMonth;
			set => SetProperty(ref catheterLastUseMonth, value);
		}

		/// <summary>
		/// Gets or sets the Catheter Last Use Year value.
		/// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public int CatheterLastUseYear
		{
			get => catheterLastUseYear;
			set => SetProperty(ref catheterLastUseYear, value);
		}

		/// <summary>
		/// Gets or sets sent catheter last use hour
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public int SentCatheterLastUseHour
		{
			get => sentCatheterLastUseHour;
			set => sentCatheterLastUseHour = value;
		}

		/// <summary>
		/// Gets or sets the Catheter Last Use Date value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public DateTime CatheterLastUseDate
		{
			get => catheterLastUseDate;
			set => SetProperty(ref catheterLastUseDate, value);
		}

		/// <summary>
		/// Gets or sets the Number of Injections value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public int NumberOfInjections
		{
			get => numberOfInjections;
			set => SetProperty(ref numberOfInjections, value);
		}

		/// <summary>
		/// Gets or sets the Target Balloon Pressure value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double TargetBalloonPressure
		{
			get => targetBalloonPressure;
			set => SetProperty(ref targetBalloonPressure, value);
		}

		/// <summary>
		/// Gets or sets the List of Patient Micro controller register IDS Dynamic table's integer values
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public List<int> PatientMicroControllerRegisterIDSDynamicTable
		{
			get
			{
				lock (_myRegister_Lock) return patientMicroControllerRegisterIDSDynamicTable;
			}
			set
			{
				lock (_myRegister_Lock) patientMicroControllerRegisterIDSDynamicTable = value;
			}
		}

		/// <summary>
		/// Gets or sets the Dictionary of Patient Micro controller Ack Register Table values
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public Dictionary<int, bool> PatientMicroControllerAckRegistersTable
		{
			get
			{
				lock (_myRegister_Lock) return patientMicroControllerackRegistersTable;
			}
			set
			{
				lock (_myRegister_Lock) patientMicroControllerackRegistersTable = value;
			}
		}

		/// <summary>
		/// Gets or sets the Threshold For CP1 High value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double ThresholdForCP1High
		{
			get => thresholdForCP1High;
			set => SetProperty(ref thresholdForCP1High, value);
		}

		/// <summary>
		/// Gets or sets the Threshold For Outer Balloon Pressure value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double ThresholdForOuterBallonPressure
		{
			get => thresholdForOuterBallonPressure;
			set => SetProperty(ref thresholdForOuterBallonPressure, value);
		}

		/// <summary>
		/// Gets or sets the Threshold For Inner Balloon Pressure Low value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double ThresholdForInnerBallonPressureLow
		{
			get => thresholdForInnerBallonPressureLow;
			set => SetProperty(ref thresholdForInnerBallonPressureLow, value);
		}

		/// <summary>
		/// Gets or sets  the upgrade status
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double UpgradeStatus
		{
			get => upgradeStatus;
			set => SetProperty(ref upgradeStatus, value);
		}

		/// <summary>
		/// Gets or sets the Threshold For CTC 1 High value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double ThresholdForCTC1High
		{
			get => thresholdForCTC1High;
			set => SetProperty(ref thresholdForCTC1High, value);
		}

		/// <summary>
		/// Gets or sets the Threshold For CTC2 High value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double ThresholdForCTC2High
		{
			get => thresholdForCTC2High;
			set => SetProperty(ref thresholdForCTC2High, value);
		}

		/// <summary>
		/// Gets or sets the thawing temperature set point.
		/// . Safety classification: Death or SERIOUS INJURY is possible (IEC 62304 Class C).
		/// </summary>
		/// <id>SF-SDS-0007</id>
		public double ThawingTemperatureSetPoint
		{
			get => thawingTemperatureSetPoint;
			set => SetProperty(ref thawingTemperatureSetPoint, value);
		}

		/// <summary>
		/// Gets or sets the blood detection lower threshold
		/// . Safety classification: Death or SERIOUS INJURY is possible (IEC 62304 Class C).
		/// </summary>
		/// <id>SF-SDS-0005</id>
		public short LowerBloodThreshold
		{
			get => lowerBloodThreshold;
			set => SetProperty(ref lowerBloodThreshold, value);
		}

		/// <summary>
		/// Gets or sets the blood detection upper threshold
		/// . Safety classification: Death or SERIOUS INJURY is possible (IEC 62304 Class C).
		/// </summary>
		/// <id>SF-SDS-0001</id>
		public short UpperBloodThreshold
		{
			get => upperBloodThreshold;
			set => SetProperty(ref upperBloodThreshold, value);
		}

		/// <summary>
		/// Gets or sets the Patient P Gain value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double PatientPGain
		{
			get => patientPGain;
			set => SetProperty(ref patientPGain, value);
		}

		/// <summary>
		/// Gets or sets the Patient I Gain value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double PatientIGain
		{
			get => patientIGain;
			set => SetProperty(ref patientIGain, value);
		}

		/// <summary>
		/// Gets or sets the Patient D Gain value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double PatientDGain
		{
			get => patientDGain;
			set => SetProperty(ref patientDGain, value);
		}

		/// <summary>
		/// Gets or sets the Patient PID Offset value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double PatientPIDOffset
		{
			get => patientPIDOffset;
			set => SetProperty(ref patientPIDOffset, value);
		}

		/// <summary>
		/// Gets or sets the Catheter Firmware Version value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public int CatheterFirmwareVersion
		{
			get => catheterFirmwareVersion;
			set => SetProperty(ref catheterFirmwareVersion, value);
		}



		#region Communication Properties

		/// <summary>
		/// Gets or sets the Target Injection Flow value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double TargetInjectionFlow
		{
			get => targetInjectionFlow;
			set => SetProperty(ref targetInjectionFlow, value);
		}

		/// <summary>
		/// Gets or sets the Target Injection Pressure value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double TargetInjectionPressure
		{
			get => targetInjectionPressure;
			set => SetProperty(ref targetInjectionPressure, value);
		}

		/// <summary>
		/// Gets or sets the Can One Stopwatch communication lost value.
		/// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public Stopwatch CanOneStopWatchCommunicationLost { get; set; } = new Stopwatch();

		/// <summary>
		/// Gets or sets the Can Two Stopwatch communication lost value.
		/// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public Stopwatch CanTwoStopWatchCommunicationLost { get; set; } = new Stopwatch();

		/// <summary>
		/// Gets or sets the Threshold For PT1 High value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double ThresholdForPT1High
		{
			get => thresholdForPT1High;
			set => SetProperty(ref thresholdForPT1High, value);
		}

		/// <summary>
		/// Gets or sets the PT1 Low Range value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double PT1LowRange
		{
			get => pT1LowRange;
			set => SetProperty(ref pT1LowRange, value);
		}

		/// <summary>
		/// Gets or sets the PT1 High Range value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double PT1HighRange
		{
			get => pT1HighRange;
			set => SetProperty(ref pT1HighRange, value);
		}

		/// <summary>
		/// Gets or sets the Threshold for PT1 Low value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double ThresholdForPT1Low
		{
			get => thresholdForPT1Low;
			set => SetProperty(ref thresholdForPT1Low, value);
		}

		/// <summary>
		/// Gets or sets the Threshold for PT1 Fail value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double ThresholdForPT1Fail
		{
			get => thresholdForPT1Fail;
			set => SetProperty(ref thresholdForPT1Fail, value);
		}

		/// <summary>
		/// Gets or sets the Threshold PT2 High value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double ThresholdPT2High
		{
			get => thresholdPT2High;
			set => SetProperty(ref thresholdPT2High, value);
		}

		/// <summary>
		/// Gets or sets the PT2 Low Range value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double PT2LowRange
		{
			get => pT2LowRange;
			set => SetProperty(ref pT2LowRange, value);
		}

		/// <summary>
		/// Gets or sets the PT2 High Range value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double PT2HighRange
		{
			get => pT2HighRange;
			set => SetProperty(ref pT2HighRange, value);
		}

		/// <summary>
		/// Gets or sets the Threshold PT3 High value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double ThresholdPT3High
		{
			get => thresholdPT3High;
			set => SetProperty(ref thresholdPT3High, value);
		}

		/// <summary>
		/// Gets or sets the PT3 Low Range value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double PT3LowRange
		{
			get => pT3LowRange;
			set => SetProperty(ref pT3LowRange, value);
		}

		/// <summary>
		/// Gets or sets the PT3 High Range value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double PT3HighRange
		{
			get => pT3HighRange;
			set => SetProperty(ref pT3HighRange, value);
		}

		/// <summary>
		/// Gets or sets the Threshold PT4 High value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double ThresholdPT4high
		{
			get => thresholdPT4high;
			set => SetProperty(ref thresholdPT4high, value);
		}

		/// <summary>
		/// Gets or sets the PT4 Low Range value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double PT4LowRange
		{
			get => pT4LowRange;
			set => SetProperty(ref pT4LowRange, value);
		}

		/// <summary>
		/// Gets or sets the PT4 High Range value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double PT4HighRange
		{
			get => pT4HighRange;
			set => SetProperty(ref pT4HighRange, value);
		}

		/// <summary>
		/// Gets or sets the Threshold TS1 High value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double ThresholdTS1High
		{
			get => thresholdTS1High;
			set => SetProperty(ref thresholdTS1High, value);
		}

		/// <summary>
		/// Gets or sets the Threshold TS1 Low Range value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double TS1LowRange
		{
			get => tS1LowRange;
			set => SetProperty(ref tS1LowRange, value);
		}

		/// <summary>
		/// Gets or sets the Threshold TS1 High Range value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double TS1HighRange
		{
			get => tS1HighRange;
			set => SetProperty(ref tS1HighRange, value);
		}

		/// <summary>
		/// Gets or sets the Threshold FM1 Low value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double ThresholdFM1Low
		{
			get => thresholdFM1Low;
			set => SetProperty(ref thresholdFM1Low, value);
		}

		/// <summary>
		/// Gets or sets the Threshold FM1 High value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double ThresholdFM1High
		{
			get => thresholdFM1High;
			set => SetProperty(ref thresholdFM1High, value);
		}

		/// <summary>
		/// Gets or sets the FM1 Low Range value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double FM1LowRange
		{
			get => fM1LowRange;
			set => SetProperty(ref fM1LowRange, value);
		}

		/// <summary>
		/// Gets or sets the FM1 High Range value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double FM1HighRange
		{
			get => fM1HighRange;
			set => SetProperty(ref fM1HighRange, value);
		}

		/// <summary>
		/// Gets or sets the Threshold PS1 High value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double ThresholdPS1High
		{
			get => thresholdPS1High;
			set => SetProperty(ref thresholdPS1High, value);
		}

		/// <summary>
		/// Gets or sets the PS1 High Range value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double PS1HighRange
		{
			get => pS1HighRange;
			set => SetProperty(ref pS1HighRange, value);
		}

		/// <summary>
		/// Gets or sets the PS1 Low Range value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double PS1LowRange
		{
			get => pS1LowRange;
			set => SetProperty(ref pS1LowRange, value);
		}

		/// <summary>
		/// Gets or sets the Threshold PS2 High value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double ThresholdPS2High
		{
			get => thresholdPS2High;
			set => SetProperty(ref thresholdPS2High, value);
		}

		/// <summary>
		/// Gets or sets the PS2 Low Range value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double PS2LowRange
		{
			get => pS2LowRange;
			set => SetProperty(ref pS2LowRange, value);
		}

		/// <summary>
		/// Gets or sets the PS2 High Range value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double PS2HighRange
		{
			get => pS2HighRange;
			set => SetProperty(ref pS2HighRange, value);
		}

		/// <summary>
		/// Gets or sets the Threshold LC1 Warning value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double ThresholdLC1Warning
		{
			get => thresholdLC1Warning;
			set => SetProperty(ref thresholdLC1Warning, value);
		}

		/// <summary>
		/// Gets or sets the Threshold LC1 Fail value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double ThresholdLC1Fail
		{
			get => thresholdLC1Fail;
			set => SetProperty(ref thresholdLC1Fail, value);
		}

		/// <summary>
		/// Gets or sets the LC1 Low Range value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double LC1LowRange
		{
			get => lC1LowRange;
			set => SetProperty(ref lC1LowRange, value);
		}

		/// <summary>
		/// Gets or sets the LC1 High Range value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double LC1HighRange
		{
			get => lC1HighRange;
			set => SetProperty(ref lC1HighRange, value);
		}

		/// <summary>
		/// Gets or sets the Previous System State value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public MessageStateId PreviousSystemState
		{
			get => previousSystemState;
			set => SetProperty(ref previousSystemState, value);
		}

		/// <summary>
		/// Gets or sets the CMCU System Status Error value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public Int64 CMCUSystemStatusError
		{
			get => cMCUSystemStatusError;
			set => SetProperty(ref cMCUSystemStatusError, value);
		}

		/// <summary>
		/// Gets or sets a value indicating whether solenoid valve 1 is on or not
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsSolenoidValve1ON
		{
			get => isSolenoidValve1ON;
			set => SetProperty(ref isSolenoidValve1ON, value);
		}

		/// <summary>
		/// Gets or sets a value indicating whether solenoid valve 2 is on or not
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsSolenoidValve2ON
		{
			get => isSolenoidValve2ON;
			set => SetProperty(ref isSolenoidValve2ON, value);
		}
		/// <summary>
		/// Gets or sets a value indicating whether solenoid valve 3 is on or not
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsSolenoidValve3ON
		{
			get => isSolenoidValve3ON;
			set => SetProperty(ref isSolenoidValve3ON, value);
		}
		/// <summary>
		/// Gets or sets a value indicating whether solenoid valve 4 is on or not
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsSolenoidValve4ON
		{
			get => isSolenoidValve4ON;
			set => SetProperty(ref isSolenoidValve4ON, value);
		}
		/// <summary>
		/// Gets or sets a value indicating whether solenoid valve 5 is on or not
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsSolenoidValve5ON
		{
			get => isSolenoidValve5ON;
			set => SetProperty(ref isSolenoidValve5ON, value);
		}
		/// <summary>
		/// Gets or sets a value indicating whether solenoid valve 6 is on or not
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsSolenoidValve6ON
		{
			get => isSolenoidValve6ON;
			set => SetProperty(ref isSolenoidValve6ON, value);
		}
		/// <summary>
		/// Gets or sets a value indicating whether solenoid valve7 is on or not
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsSolenoidValve7ON
		{
			get => isSolenoidValve7ON;
			set => SetProperty(ref isSolenoidValve7ON, value);
		}
		/// <summary>
		/// Gets or sets a value indicating whether solenoid valve 8 is on or not.
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsSolenoidValve8ON
		{
			get => isSolenoidValve8ON;
			set => SetProperty(ref isSolenoidValve8ON, value);
		}
		/// <summary>
		/// Gets or sets a value indicating whether solenoid valve 9 is on or not
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsSolenoidValve9ON
		{
			get => isSolenoidValve9ON;
			set => SetProperty(ref isSolenoidValve9ON, value);
		}

		/// <summary>
		/// Gets or sets the PMCU System Status Error Code value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public Int64 PMCUSystemStatusErrorCode
		{
			get => pMCUSystemStatusErrorCode;
			set => SetProperty(ref pMCUSystemStatusErrorCode, value);
		}

		/// <summary>
		/// Gets or sets a value indicating whether allow firm ware reading or not
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool AllowFirmwareReading
		{
			get => allowFirmwareReading;
			set => allowFirmwareReading = value;
		}

		/// <summary>
		/// Gets or sets the Catheter Id value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public int CatheterID
		{
			get => catheterID;
			set => SetProperty(ref catheterID, value);
		}

		/// <summary>
		/// Gets or sets the Catheter Serial Number value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public int CatheterSerialNumber
		{
			get => catheterSerialNumber;
			set => SetProperty(ref catheterSerialNumber, value);
		}

		/// <summary>
		/// Gets or sets the catheter lot value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public int CatheterLot
		{
			get => catheterLot;
			set => SetProperty(ref catheterLot, value);
		}

		/// <summary>
		/// Gets or sets the Catheter Expiration Month value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public int CatheterExpirationMonth
		{
			get => catheterExpirationMonth;
			set => SetProperty(ref catheterExpirationMonth, value);
		}

		/// <summary>
		/// Gets or sets the Catheter Expiration Day value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public int CatheterExpirationDay
		{
			get => catheterExpirationDay;
			set => SetProperty(ref catheterExpirationDay, value);
		}

		/// <summary>
		/// Gets or sets the Catheter Expiration Year value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public int CatheterExpirationYear
		{
			get => catheterExpirationYear;
			set => SetProperty(ref catheterExpirationYear, value);
		}

		/// <summary>
		/// Gets or sets the Catheter Expiration Date value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public DateTime CatheterExpirationDate
		{
			get => catheterExpirationDate;
			set => SetProperty(ref catheterExpirationDate, value);
		}

		/// <summary>
		/// Gets or sets a value indicating whether the Catheter last use date is updated or not
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsCatheterLastUseDateUpdated
		{
			get => isCatheterLastUseDateUpdated;
			set => isCatheterLastUseDateUpdated = value;
		}

		/// <summary>
		/// Gets or sets sent catheter last use day
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public int SentCatheterLastUseDay
		{
			get => sentCatheterLastUseDay;
			set => sentCatheterLastUseDay = value;
		}

		/// <summary>
		/// Gets or sets the PID Duty cycle value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double PIDDutyCycle
		{
			get => pIDDutyCycle;
			set => SetProperty(ref pIDDutyCycle, value);
		}

		/// <summary>
		/// Gets or sets sent catheter last use month
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public int SentCatheterLastUseMonth
		{
			get => sentCatheterLastUseMonth;
			set => sentCatheterLastUseMonth = value;
		}

		/// <summary>
		/// Gets or sets sent catheter last use year
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public int SentCatheterLastUseYear
		{
			get => sentCatheterLastUseYear;
			set => sentCatheterLastUseYear = value;
		}

		/// <summary>
		/// Gets or sets a value indicating whether the Catheter expiration date was updated or not
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsCatheterExpirationDateUpdated
		{
			get => isCatheterExpirationDateUpdated;
			set => isCatheterExpirationDateUpdated = value;
		}

		#endregion Communication Properties

		#region System Parameters

		/// <summary>
		/// Gets or sets an double value for ramp up time by step
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double RampUpTimeByStep
		{
			get => rampUpTimeByStep;
			set => SetProperty(ref rampUpTimeByStep, value);
		}

		/// <summary>
		/// Gets or sets an double value for pressure ramp up
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double PressureRampUpValue
		{
			get => pressureRampUpValue;
			set => SetProperty(ref pressureRampUpValue, value);
		}

		/// <summary>
		/// Gets or sets an double value for ramp down time by step
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double RampDownTimeByStep
		{
			get => rampDownTimeByStep;
			set => SetProperty(ref rampDownTimeByStep, value);
		}

		/// <summary>
		/// Gets or sets an double value for pressure ramp down 
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double PressureRampDownValue
		{
			get => pressureRampDownValue;
			set => SetProperty(ref pressureRampDownValue, value);
		}

		/// <summary>
		/// Gets or sets the P Gain value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double PGain
		{
			get => pGain;
			set => SetProperty(ref pGain, value);
		}
		/// <summary>
		/// Gets or sets the D Gain value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double DGain
		{
			get => dGain;
			set => SetProperty(ref dGain, value);
		}

		/// <summary>
		/// Gets or sets the I Gain value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double IGain
		{
			get => iGain;
			set => SetProperty(ref iGain, value);
		}

		/// <summary>
		/// Gets or sets the PID Offset value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double PIDOffset
		{
			get => pIDOffset;
			set => SetProperty(ref pIDOffset, value);
		}

		/// <summary>
		/// Gets or sets the Dictionary of Central Micro controller Ack Register Table values
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public Dictionary<int, bool> CentralMicroControllerAckRegistersTable
		{
			get
			{
				lock (_myRegister_Lock) return centralMicroControllerAckRegistersTable;
			}
			set
			{
				lock (_myRegister_Lock) centralMicroControllerAckRegistersTable = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating if Reading from micro controller for register validation or not
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsReadingFromMicroControllerForRegisterValidation
		{
			get => isReadingFromMicroControllerForRegisterValidation;
			set => isReadingFromMicroControllerForRegisterValidation = value;
		}

		public double TC1Reading    // TEMPERATURE.
		{
			get => tC1Reading;
			set
			{
				if (SensorReadingManager.AreSensorsConnected)
				{
					SetProperty(ref tC1Reading, value);
				}
				CatheterTemperature = value;
			}
		}

		/// <summary>
		/// Gets or sets the TC2 Reading value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double TC2Reading
		{
			get => tC2Reading;
			set => SetProperty(ref tC2Reading, value);
		}

		/// <summary>
		/// Gets or sets the PMCU CJ Reading value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double PMCUCJReading
		{
			get => pMCUCJReading;
			set => SetProperty(ref pMCUCJReading, value);
		}

		/// <summary>
		/// Gets or sets the TS1 Reading value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double TS1Reading
		{
			get => tS1Reading;
			set => SetProperty(ref tS1Reading, value);
		}

		public double CMCUCJReading
		{
			get => cMCUCJReading;
			set => SetProperty(ref cMCUCJReading, value);
		}

		public double TN2OReading
		{
			get => tN2OReading;
			set => SetProperty(ref tN2OReading, value);
		}

		public double CatheterTemperature { get; set; }

		public double PS1Reading
		{
			get => pS1Reading;

			set => SetProperty(ref pS1Reading, value);
		}

		public double PS2Reading
		{
			get => pS2Reading;

			set => SetProperty(ref pS2Reading, value);
		}

		private double pT1Reading;
		public double PT1Reading
		{
			get => pT1Reading;
			set => SetProperty(ref pT1Reading, value);
		}

		private double pT2Reading;
		public double PT2Reading
		{
			get => pT2Reading;
			set => SetProperty(ref pT2Reading, value);
		}

		private double pT3Reading;
		public double PT3Reading
		{
			get => pT3Reading;
			set => SetProperty(ref pT3Reading, value);
		}

		private double pT4Reading;
		public double PT4Reading
		{
			get => pT4Reading;
			set => SetProperty(ref pT4Reading, value);
		}

		private double pT5Reading;
		public double PT5Reading
		{
			get => pT5Reading;
			set => SetProperty(ref pT5Reading, value);
		}

		// Flow meter
		private double fM1Reading;
		public double FM1Reading
		{
			get => fM1Reading;
			set => SetProperty(ref fM1Reading, value);
		}

		/// <summary>
		/// Gets or sets the System State value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public MessageStateId SystemState
		{
			get => systemState;
			set => SetProperty(ref systemState, value);
		}

		/// <summary>
		/// Gets or sets the Continuous Thawing value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double ContinuousThawing
		{
			get => continuousThawing;
			set => SetProperty(ref continuousThawing, value);
		}

		/// <summary>
		/// Gets or sets the CPLD Valve Register value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public int CPLDValveRegister
		{
			get => cPLDValveRegister;
			set => SetProperty(ref cPLDValveRegister, value);
		}

		/// <summary>
		/// Gets or sets the CPLD System Register value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public int CPLDSystemRegister
		{
			get => cPLDSystemRegister;
			set => SetProperty(ref cPLDSystemRegister, value);
		}

		/// <summary>
		/// Gets or sets Ablation Time value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public int AblationTime
		{
			get => ablationTime;
			set => SetProperty(ref ablationTime, value);
		}

		/// <summary>
		/// Gets or sets the inner balloon pressure reading value.
		/// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double CP1Reading
		{
			get => cP1Reading;
			set
			{
				if (SensorReadingManager.AreSensorsConnected)
				{
					SetProperty(ref cP1Reading, value);
				}
			}
		}    // IBP

		/// <summary>
		/// Gets or sets the outer balloon pressure reading value.
		/// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double CP2Reading
		{
			get => cP2Reading;
			set
			{
				if (SensorReadingManager.AreSensorsConnected)
				{
					SetProperty(ref cP2Reading, value);
				}
			}
		}    //OBP

		private double lC1ReadingWithMetalPreviousValue = 1000;
		/// <summary>
		/// Gets or sets the LC1 Reading value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double LC1Reading
		{
			get => lC1Reading;
			set
      {
        if (lC1ReadingWithMetalPreviousValue == value) 
          return;
   		
        lC1Reading = value - Console.Tank.MetalWeight;
        if (lC1Reading < 0) lC1Reading = 0d;

        lC1ReadingWithMetalPreviousValue = value;

				RaisePropertyChanged(nameof(LC1Reading));
      }
		}

		/// <summary>
		/// Gets or sets the TIP Reading value.
		/// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double TIPReading
		{
			get => tIPReading;
			set => SetProperty(ref tIPReading, value);
		}

		/// <summary>
		/// Gets or sets the Patient PID Duty Cycle value.
		/// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double PatientPIDDutyCycle
		{
			get => patientPIDDutyCycle;
			set => SetProperty(ref patientPIDDutyCycle, value);
		}

		/// <summary>
		/// Gets or sets  the module key for the update
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double ModuleKey
		{
			get => moduleKey;
			set => SetProperty(ref moduleKey, value);
		}

		#endregion System Parameters

		public bool StartButtonPressed
		{
			get => _startButtonPressed;
			set => SetProperty(ref _startButtonPressed, value);
		}

		public bool StopButtonPressed
		{
			get => _stopButtonPressed;
			set => SetProperty(ref _stopButtonPressed, value);
		}

		public bool StartFootSwitchOn
		{
			get => _startFootSwitchOn;
			set => SetProperty(ref _startFootSwitchOn, value);
		}

		public bool StopFootSwitchOn
		{
			get => _stopFootSwitchOn;
			set => SetProperty(ref _stopFootSwitchOn, value);
		}

		public byte CPLDFPINStatus
		{
			get => _cpldFPINStatus;
			set => SetProperty(ref _cpldFPINStatus, value);
		}
	}
}
