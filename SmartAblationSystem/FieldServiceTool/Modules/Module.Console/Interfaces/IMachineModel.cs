using Communication;
using Console;
using Module.Console.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Module.Console.Interfaces
{
	public interface IMachineModel
	{
		/// <summary>
		/// Get the instance of Machine which defines many methods to interact with console  
		/// </summary>
		Machine Console { get; }

		/// <summary>
		/// Gets or sets the central microController bootLoader firmware version. CMCUBoot
		/// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		int CentralMicroControllerBootLoaderFirmwareVersion { get; set; }

		/// <summary>
		/// Gets or sets the Central Micro Controller Firmware Version value.
		/// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		int CentralMicroControllerFirmwareVersion { get; set; }

		/// <summary>
		/// Gets or sets the CPLD bootLoader firmware version.
		/// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		int CpldFirmwareVersion { get; set; }

		/// <summary>
		/// Gets or sets the Patient Micro Controller Firmware Version value.
		/// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		int PatientMicroControllerFirmwareVersion { get; set; }

		/// <summary>
		/// Gets or sets the patient bootLoader firmware version.
		/// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		int PatientMicroControllerBootLoaderFirmwareVersion { get; set; }

		/// <summary>
		/// This property gets/sets the RemoteControlFirmwareDBVersion value.
		/// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		int RemoteControlFirmwareDBVersion { get; set; }

		/// <summary>
		/// Gets or sets the Repeater Firmware value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		int RepeaterFirmware { get; set; }

		/// <summary>
		/// Gets or sets the ICB Firmware value.
		/// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		int ICBFirmware { get; set; }

		/// <summary>
		/// Gets or sets  the repeater bootLoader firmware version
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		int RepeaterBootLoaderFirmware { get; set; }

		/// <summary>
		/// Gets or sets the ICB bootloader firmware version
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		int ICBBootLoaderFirmwareVersion { get; set; }

		/// <summary>
		/// Gets or sets  the remote control firmware version
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		int RemoteControlFirmware { get; set; }

		/// <summary>
		/// Gets or sets  the remote control bootLoader firmware version
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		int RemoteControlBootLoaderFirmwareVersion { get; set; }

		RemoteControlFSM RemoteControlFSM { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the Catheter is valid or not.
		/// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		bool IsCatheterValid { get; set; }

		/// <summary>
		/// Gets or sets a value indicating the Catheter cable is connected or not.
		/// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		bool IsCatheterCableConnected { get; set; }

		/// <summary>
		/// Gets or sets a value indicating the vacuum is connected or not.
		/// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		bool IsVacuumDisconnected { get; set; }

		/// <summary>
		/// Gets or sets  a value indicating whether used for Engineering
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		bool IsUsedForEngineering { get; set; }

		/// <summary>
		/// Gets or sets  a value indicating whether DAS Ballon is using or not
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		bool IsSystemUsingDASBalloon { get; set; }

		/// <summary>
		/// Gets or sets an int value for Engineering catheter signature
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		int EngineeringCatheterSignature { get; }

		/// <summary>
		/// Gets or sets or the boot loader data.
		/// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		byte[] BootLoaderData { get; set; }

		/// <summary>
		/// Gets or sets the blood detector impedance
		/// . Safety classification: Death or SERIOUS INJURY is possible (IEC 62304 Class C).
		/// </summary>
		/// <id>SF-SDS-0003</id>
		int BloodDetecorImValue { get; set; }

		Stopwatch ICBStopWatchDisconnection { get; set; }
		bool IsCanOneInError { get; set; }
		bool IsCanTwoInError { get; set; }
		bool IsBootLoaderUpdatingFirmware { get; set; }

		/// <summary>
		/// Gets or sets the ECG Channel 1 and 2 Reading value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double EcgChannel1And2Reading { get; set; }

		/// <summary>
		/// Gets or sets the Minimum Diaphragm Movement value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double DMSDetectionThreshold { get; set; }

		/// <summary>
		/// Gets or sets the ECG Channel 3 and 4 Reading value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double EcgChannel3And4Reading { get; set; }

		bool IsBloodPressureSensorConnected { get; set; }
		double[] BloodPressureValue { get; set; }

		/// <summary>
		/// Gets or sets the Max ECG Channel 3 and 4 Reading value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double MaxEcgChannel3And4Reading { get; set; }

		/// <summary>
		/// Gets or sets the Maximum Average Pacing Level value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <id>SF-SDS-0105</id>
		double MaximumAveragePacingLevel { get; set; }

		/// <summary>
		/// Gets or sets the ECG Channel 5 and 6 Reading value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double EcgChannel5And6Reading { get; set; }

		bool IsMultiEtsSesnorConnected { get; set; }

		/// <summary>
		/// This property gets/sets the Time The ETS sesnor value
		/// . Safety classification: Non-SERIOUS INJURY is possible (IEC 62304 Class B).
		/// </summary>
		/// <id>SF-SDS-0047</id>
		double EtsSesnor13 { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the Sensors are in playback mode or not
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		bool AreSensorsInPlayBackMode { get; set; }

		/// <summary>
		/// This property gets/sets the Time The ETS sesnor value
		/// . Safety classification: Non-SERIOUS INJURY is possible (IEC 62304 
		/// .
		/// </summary>
		double EtsSesnor1 { get; set; }

		/// <summary>
		/// This property gets/sets the Time The ETS sesnor value
		/// . Safety classification: Non-SERIOUS INJURY is possible (IEC 62304 Class B).
		/// </summary>
		/// <id>SF-SDS-0036</id>
		double EtsSesnor2 { get; set; }

		/// <summary>
		/// This property gets/sets the Time The ETS sesnor value
		/// . Safety classification: Non-SERIOUS INJURY is possible (IEC 62304 Class B).
		/// </summary>
		/// <id>SF-SDS-0037</id>
		double EtsSesnor3 { get; set; }

		/// <summary>
		/// This property gets/sets the Time The ETS sensor value
		/// . Safety classification: Non-SERIOUS INJURY is possible (IEC 62304 Class B).
		/// </summary>
		/// <id>SF-SDS-0038</id>
		double EtsSesnor4 { get; set; }

		/// <summary>
		/// This property gets/sets the Time The ETS sensor value
		/// . Safety classification: Non-SERIOUS INJURY is possible (IEC 62304 Class B).
		/// </summary>
		/// <id>SF-SDS-0039</id>
		double EtsSesnor5 { get; set; }

		/// <summary>
		/// This property gets/sets the Time The ETS sensor value
		/// . Safety classification: Non-SERIOUS INJURY is possible (IEC 62304 Class B).
		/// </summary>
		/// <id>SF-SDS-0040</id>
		double EtsSesnor6 { get; set; }

		/// <summary>
		/// This property gets/sets the Time The ETS sensor value
		/// . Safety classification: Non-SERIOUS INJURY is possible (IEC 62304 Class B).
		/// </summary>
		/// <id>SF-SDS-0041</id>
		double EtsSesnor7 { get; set; }

		/// <summary>
		/// This property gets/sets the Time The ETS sensor value
		/// . Safety classification: Non-SERIOUS INJURY is possible (IEC 62304 Class B).
		/// </summary>
		/// <id>SF-SDS-0042</id>
		double EtsSesnor8 { get; set; }

		/// <summary>
		/// This property gets/sets the Time The ETS sensor value
		/// . Safety classification: Non-SERIOUS INJURY is possible (IEC 62304 Class B).
		/// </summary>
		/// <id>SF-SDS-0043</id>
		double EtsSesnor9 { get; set; }

		/// <summary>
		/// This property gets/sets the Time The ETS sensor value
		/// . Safety classification: Non-SERIOUS INJURY is possible (IEC 62304 Class B).
		/// </summary>
		/// <id>SF-SDS-0044</id>
		double EtsSesnor10 { get; set; }

		/// <summary>
		/// This property gets/sets the Time The ETS sensor value
		/// . Safety classification: Non-SERIOUS INJURY is possible (IEC 62304 Class B).
		/// </summary>
		/// <id>SF-SDS-0045</id>
		double EtsSesnor11 { get; set; }

		/// <summary>
		/// This property gets/sets the Time The ETS sensor value
		/// . Safety classification: Non-SERIOUS INJURY is possible (IEC 62304 Class B).
		/// </summary>
		/// <id>SF-SDS-0046</id>
		double EtsSesnor12 { get; set; }

		/// <summary>
		/// This property gets/sets the list of sensors state value
		/// . Safety classification: Non-SERIOUS INJURY is possible (IEC 62304 Class B).
		/// </summary>
		/// <id>SF-SDS-0049</id>
		List<int> ListOfSesnorsState { get; set; }

		/// <summary>
		/// Gets or sets the ECG Channel 7 and 8 Reading value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double EcgChannel7And8Reading { get; set; }

		/// <summary>
		/// Gets or sets the Channel Tip value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double ChannelTipReading { get; set; }

		/// <summary>
		/// Gets or sets the Channel Accelerometer Reading value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double ChannelAccelerometerReading { get; set; }

		/// <summary>
		/// Gets or sets the ECG Channel 9 and 10 Readings List
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		List<double> EcgChannel9And10Readings { get; set; }

		/// <summary>
		/// Gets or sets the Channel Tip Readings List
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		List<double> ChannelTipReadings { get; set; }

		/// <summary>
		/// Gets or sets the Channel Accelerometer Readings List
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		List<double> ChannelAccelerometerReadings { get; set; }

		/// <summary>
		/// Gets or sets the ECG Channel 9 and 10 Reading value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double EcgChannel9And10Reading { get; set; }

		/// <summary>
		/// Gets or sets Is Diaphragm Movement Detected value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		bool IsDiaphragmMovementDetected { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether ignore minimum diaphragm movement or not
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		bool IgnoreMinimumDiaphragmMovementValue { get; set; }

		/// <summary>
		/// This property gets/sets the minimum temperature value.
		/// Safety classification: Non-SERIOUS INJURY is possible (IEC 62304 Class B).
		/// </summary>
		/// <id>SF-SDS-0050</id>
		double MinimumTemperature { get; set; }

		/// <summary>
		/// Gets or sets the blood detection type.
		/// Safety classification: Death or SERIOUS INJURY is possible (IEC 62304 Class C).
		/// </summary>
		/// <id>SF-SDS-0004</id>
		int BloodDetectionType { get; set; }

		int CPLDErrorRegister { get; set; }

		/// <summary>
		/// Gets or sets the Catheter Last use hour value.
		/// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		int CatheterLastUseHour { get; set; }

		/// <summary>
		/// Gets or sets the Catheter Last Use Day value.
		/// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		int CatheterLastUseDay { get; set; }

		/// <summary>
		/// Gets or sets the Catheter Last Use Month value.
		/// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		int CatheterLastUseMonth { get; set; }

		/// <summary>
		/// Gets or sets the Catheter Last Use Year value.
		/// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		int CatheterLastUseYear { get; set; }

		/// <summary>
		/// Gets or sets sent catheter last use hour
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		int SentCatheterLastUseHour { get; set; }

		/// <summary>
		/// Gets or sets the Catheter Last Use Date value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		DateTime CatheterLastUseDate { get; set; }

		/// <summary>
		/// Gets or sets the Number of Injections value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		int NumberOfInjections { get; set; }

		/// <summary>
		/// Gets or sets the Target Balloon Pressure value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double TargetBalloonPressure { get; set; }

		/// <summary>
		/// Gets or sets the List of Patient Micro controller register IDS Dynamic table's integer values
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		List<int> PatientMicroControllerRegisterIDSDynamicTable { get; set; }

		/// <summary>
		/// Gets or sets the Dictionary of Patient Micro controller Ack Register Table values
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		Dictionary<int, bool> PatientMicroControllerAckRegistersTable { get; set; }

		/// <summary>
		/// Gets or sets the Threshold For CP1 High value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double ThresholdForCP1High { get; set; }

		/// <summary>
		/// Gets or sets the Threshold For Outer Balloon Pressure value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double ThresholdForOuterBallonPressure { get; set; }

		/// <summary>
		/// Gets or sets the Threshold For Inner Balloon Pressure Low value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double ThresholdForInnerBallonPressureLow { get; set; }

		/// <summary>
		/// Gets or sets  the upgrade status
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double UpgradeStatus { get; set; }

		/// <summary>
		/// Gets or sets the Threshold For CTC 1 High value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double ThresholdForCTC1High { get; set; }

		/// <summary>
		/// Gets or sets the Threshold For CTC2 High value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double ThresholdForCTC2High { get; set; }

		/// <summary>
		/// Gets or sets the thawing temperature set point.
		/// . Safety classification: Death or SERIOUS INJURY is possible (IEC 62304 Class C).
		/// </summary>
		/// <id>SF-SDS-0007</id>
		double ThawingTemperatureSetPoint { get; set; }

		/// <summary>
		/// Gets or sets the blood detection lower threshold
		/// . Safety classification: Death or SERIOUS INJURY is possible (IEC 62304 Class C).
		/// </summary>
		/// <id>SF-SDS-0005</id>
		short LowerBloodThreshold { get; set; }

		/// <summary>
		/// Gets or sets the blood detection upper threshold
		/// . Safety classification: Death or SERIOUS INJURY is possible (IEC 62304 Class C).
		/// </summary>
		/// <id>SF-SDS-0001</id>
		short UpperBloodThreshold { get; set; }

		/// <summary>
		/// Gets or sets the Patient P Gain value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double PatientPGain { get; set; }

		/// <summary>
		/// Gets or sets the Patient I Gain value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double PatientIGain { get; set; }

		/// <summary>
		/// Gets or sets the Patient D Gain value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double PatientDGain { get; set; }

		/// <summary>
		/// Gets or sets the Patient PID Offset value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double PatientPIDOffset { get; set; }

		/// <summary>
		/// Gets or sets the Catheter Firmware Version value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		int CatheterFirmwareVersion { get; set; }

		/// <summary>
		/// Gets or sets the Target Injection Flow value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double TargetInjectionFlow { get; set; }

		/// <summary>
		/// Gets or sets the Target Injection Pressure value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double TargetInjectionPressure { get; set; }

		/// <summary>
		/// Gets or sets the Can One Stopwatch communication lost value.
		/// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		Stopwatch CanOneStopWatchCommunicationLost { get; set; }

		/// <summary>
		/// Gets or sets the Can Two Stopwatch communication lost value.
		/// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		Stopwatch CanTwoStopWatchCommunicationLost { get; set; }

		/// <summary>
		/// Gets or sets the Threshold For PT1 High value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double ThresholdForPT1High { get; set; }

		/// <summary>
		/// Gets or sets the PT1 Low Range value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double PT1LowRange { get; set; }

		/// <summary>
		/// Gets or sets the PT1 High Range value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double PT1HighRange { get; set; }

		/// <summary>
		/// Gets or sets the Threshold for PT1 Low value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double ThresholdForPT1Low { get; set; }

		/// <summary>
		/// Gets or sets the Threshold for PT1 Fail value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double ThresholdForPT1Fail { get; set; }

		/// <summary>
		/// Gets or sets the Threshold PT2 High value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double ThresholdPT2High { get; set; }

		/// <summary>
		/// Gets or sets the PT2 Low Range value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double PT2LowRange { get; set; }

		/// <summary>
		/// Gets or sets the PT2 High Range value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double PT2HighRange { get; set; }

		/// <summary>
		/// Gets or sets the Threshold PT3 High value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double ThresholdPT3High { get; set; }

		/// <summary>
		/// Gets or sets the PT3 Low Range value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double PT3LowRange { get; set; }

		/// <summary>
		/// Gets or sets the PT3 High Range value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double PT3HighRange { get; set; }

		/// <summary>
		/// Gets or sets the Threshold PT4 High value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double ThresholdPT4high { get; set; }

		/// <summary>
		/// Gets or sets the PT4 Low Range value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double PT4LowRange { get; set; }

		/// <summary>
		/// Gets or sets the PT4 High Range value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double PT4HighRange { get; set; }

		/// <summary>
		/// Gets or sets the Threshold TS1 High value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double ThresholdTS1High { get; set; }

		/// <summary>
		/// Gets or sets the Threshold TS1 Low Range value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double TS1LowRange { get; set; }

		/// <summary>
		/// Gets or sets the Threshold TS1 High Range value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double TS1HighRange { get; set; }

		/// <summary>
		/// Gets or sets the Threshold FM1 Low value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double ThresholdFM1Low { get; set; }

		/// <summary>
		/// Gets or sets the Threshold FM1 High value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double ThresholdFM1High { get; set; }

		/// <summary>
		/// Gets or sets the FM1 Low Range value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double FM1LowRange { get; set; }

		/// <summary>
		/// Gets or sets the FM1 High Range value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double FM1HighRange { get; set; }

		/// <summary>
		/// Gets or sets the Threshold PS1 High value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double ThresholdPS1High { get; set; }

		/// <summary>
		/// Gets or sets the PS1 High Range value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double PS1HighRange { get; set; }

		/// <summary>
		/// Gets or sets the PS1 Low Range value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double PS1LowRange { get; set; }

		/// <summary>
		/// Gets or sets the Threshold PS2 High value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double ThresholdPS2High { get; set; }

		/// <summary>
		/// Gets or sets the PS2 Low Range value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double PS2LowRange { get; set; }

		/// <summary>
		/// Gets or sets the PS2 High Range value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double PS2HighRange { get; set; }

		/// <summary>
		/// Gets or sets the Threshold LC1 Warning value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double ThresholdLC1Warning { get; set; }

		/// <summary>
		/// Gets or sets the Threshold LC1 Fail value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double ThresholdLC1Fail { get; set; }

		/// <summary>
		/// Gets or sets the LC1 Low Range value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double LC1LowRange { get; set; }

		/// <summary>
		/// Gets or sets the LC1 High Range value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double LC1HighRange { get; set; }

		/// <summary>
		/// Gets or sets the Previous System State value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		CanBusMessageDefinition.MessageStateId PreviousSystemState { get; set; }

		/// <summary>
		/// Gets or sets the CMCU System Status Error value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		Int64 CMCUSystemStatusError { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether solenoid valve 1 is on or not
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		bool IsSolenoidValve1ON { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether solenoid valve 2 is on or not
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		bool IsSolenoidValve2ON { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether solenoid valve 3 is on or not
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		bool IsSolenoidValve3ON { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether solenoid valve 4 is on or not
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		bool IsSolenoidValve4ON { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether solenoid valve 5 is on or not
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		bool IsSolenoidValve5ON { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether solenoid valve 6 is on or not
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		bool IsSolenoidValve6ON { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether solenoid valve7 is on or not
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		bool IsSolenoidValve7ON { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether solenoid valve 8 is on or not.
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		bool IsSolenoidValve8ON { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether solenoid valve 9 is on or not
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		bool IsSolenoidValve9ON { get; set; }

		/// <summary>
		/// Gets or sets the PMCU System Status Error Code value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		Int64 PMCUSystemStatusErrorCode { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether allow firm ware reading or not
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		bool AllowFirmwareReading { get; set; }

		/// <summary>
		/// Gets or sets the Catheter Id value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		int CatheterID { get; set; }

		/// <summary>
		/// Gets or sets the Catheter Serial Number value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		int CatheterSerialNumber { get; set; }

		/// <summary>
		/// Gets or sets the catheter lot value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		int CatheterLot { get; set; }

		/// <summary>
		/// Gets or sets the Catheter Expiration Month value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		int CatheterExpirationMonth { get; set; }

		/// <summary>
		/// Gets or sets the Catheter Expiration Day value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		int CatheterExpirationDay { get; set; }

		/// <summary>
		/// Gets or sets the Catheter Expiration Year value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		int CatheterExpirationYear { get; set; }

		/// <summary>
		/// Gets or sets the Catheter Expiration Date value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		DateTime CatheterExpirationDate { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the Catheter last use date is updated or not
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		bool IsCatheterLastUseDateUpdated { get; set; }

		/// <summary>
		/// Gets or sets sent catheter last use day
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		int SentCatheterLastUseDay { get; set; }

		/// <summary>
		/// Gets or sets the PID Duty cycle value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double PIDDutyCycle { get; set; }

		/// <summary>
		/// Gets or sets sent catheter last use month
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		int SentCatheterLastUseMonth { get; set; }

		/// <summary>
		/// Gets or sets sent catheter last use year
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		int SentCatheterLastUseYear { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the Catheter expiration date was updated or not
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		bool IsCatheterExpirationDateUpdated { get; set; }

		/// <summary>
		/// Gets or sets an double value for ramp up time by step
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double RampUpTimeByStep { get; set; }

		/// <summary>
		/// Gets or sets an double value for pressure ramp up
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double PressureRampUpValue { get; set; }

		/// <summary>
		/// Gets or sets an double value for ramp down time by step
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double RampDownTimeByStep { get; set; }

		/// <summary>
		/// Gets or sets an double value for pressure ramp down 
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double PressureRampDownValue { get; set; }

		/// <summary>
		/// Gets or sets the P Gain value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double PGain { get; set; }

		/// <summary>
		/// Gets or sets the D Gain value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double DGain { get; set; }

		/// <summary>
		/// Gets or sets the I Gain value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double IGain { get; set; }

		/// <summary>
		/// Gets or sets the PID Offset value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double PIDOffset { get; set; }

		/// <summary>
		/// Gets or sets the Dictionary of Central Micro controller Ack Register Table values
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		Dictionary<int, bool> CentralMicroControllerAckRegistersTable { get; set; }

		/// <summary>
		/// Gets or sets a value indicating if Reading from micro controller for register validation or not
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		bool IsReadingFromMicroControllerForRegisterValidation { get; set; }

		double TC1Reading // TEMPERATURE.
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the TC2 Reading value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double TC2Reading { get; set; }

		/// <summary>
		/// Gets or sets the PMCU CJ Reading value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double PMCUCJReading { get; set; }

		/// <summary>
		/// Gets or sets the TS1 Reading value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		double TS1Reading { get; set; }

		double CMCUCJReading { get; set; }
		double TN2OReading { get; set; }
		double CatheterTemperature { get; set; }
		double PS1Reading { get; set; }
		double PS2Reading { get; set; }
		double PT1Reading { get; set; }
		double PT2Reading { get; set; }
		double PT3Reading { get; set; }
		double PT4Reading { get; set; }
		double PT5Reading { get; set; }
		double FM1Reading { get; set; }

		/// <summary>
		/// Gets or sets the System State value
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		CanBusMessageDefinition.MessageStateId SystemState { get; set; }
		double ContinuousThawing { get; set; }
		int CPLDValveRegister { get; set; }
		int CPLDSystemRegister { get; set; }
		int AblationTime { get; set; }
		double CP1Reading { get; set; } // IBP
		double CP2Reading { get; set; } //OBP
		double LC1Reading { get; set; }
		double TIPReading { get; set; }
		double PatientPIDDutyCycle { get; set; }
		double ModuleKey { get; set; }
		bool StartButtonPressed { get; set; }
		bool StopButtonPressed { get; set; }
		bool StartFootSwitchOn { get; set; }
		bool StopFootSwitchOn { get; set; }
		byte CPLDFPINStatus { get; set; }
		List<int> CentralMicroControllerRegisterIDSDynamicTable { get; set; }
		void ReadFirmwareVersions();
		void ResetCanOneStopWatch();
    Task SendBalloonPressureSetPointAsync(bool isDasEnabled);
		event EventHandler<AblationTimerEventArgs> AblationTimerChangedEvent;
		Task Terminate();
		event PropertyChangedEventHandler PropertyChanged;
	}
}