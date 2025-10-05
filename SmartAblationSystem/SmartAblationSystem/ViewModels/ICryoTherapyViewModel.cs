using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using Communication;
using Console;
using DataAccessLayer;
using FileSerializer;
using Shared;
using SmartAblationSystem.Helpers;
using SmartAblationSystem.Models;

namespace SmartAblationSystem.ViewModels
{
  public interface ICryoTherapyViewModel
  {
    /// <summary>
    /// This property gets/sets the list of Ablation Data Details for a single Ablation
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    List<AblationDataDetails> SingleAblationDatasList { get; set; }

    /// <summary>
    /// This property gets/sets the Gas State value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    Helpers.Enumeration.TankWeight GasState { get; set; }

    /// <summary>
    /// This property gets/sets the Start The Timer boolean flag value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool CanStartTheTimer { get; set; }

    /// <summary>
    /// Gets or sets the blood pressure maximum value during one second.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    double BloodPressureMaximumValueDuringOneSecond { get; set; }

    /// <summary>
    /// This property gets/sets the Cryotherapy value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    int CryoTherapyTime { get; set; }

    /// <summary>
    /// Gets or sets last cryoTherapy time
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    int LastCryoTherapyTime { get; set; }

    /// <summary>
    /// This property gets/sets the Total Cryotherapy Time value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    int TotalCryoTherapyTime { get; set; }

    /// <summary>
    /// This property gets/sets the Elapsed Time value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    int ElapsedTime { get; set; }

    /// <summary>
    /// Gets or sets elapsed time last value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    int ElapsedTimeLastValue { get; set; }

    /// <summary>
    /// Gets or sets a value for Last elapsed time for flow reading
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    int ElapsedTimeLastValueForFlowReading { get; set; }

    /// <summary>
    /// Gets or sets a value for Last elapsed time for IBP reading
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    int ElapsedTimeLastValueForIBPReading { get; set; }

    /// <summary>
    /// This property gets/sets the ECG Time value.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    int EcgTime { get; set; }

    /// <summary>
    /// This property gets/sets the Is Visible boolean flag value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool IsVisible { get; set; }

    /// <summary>
    /// This property gets/sets the Ablation Number value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    int AblationNumber { get; set; }

    /// <summary>
    /// This property gets/sets the Procedure Ended boolean flag value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool IsTheProcedureEnded { get; set; }

    /// <summary>
    /// This property gets/sets the Catheter Connected boolean flag value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool IsCatheterConnected { get; set; }

    /// <summary>
    /// This property gets/sets the Required Ablation Time Blue Margin value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    int RequiredAblationTimePlueMargin { get; set; }

    /// <summary>
    /// This property gets/sets the Required Target Temperature value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    int RequiredTargetTemperature { get; set; }

    /// <summary>
    /// This property gets/sets the Low Ablation Temperature Alarm value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    int LowAblationTemperatureAlarm { get; set; }

    /// <summary>
    /// This property gets/sets the High Ablation Temperature Alarm value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    int HighAblationTemperatureAlarm { get; set; }

    /// <summary>
    /// This property gets/sets the Thaw Timer To Temperature value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    int ThawTimerToTemperature { get; set; }

    /// <summary>
    /// This property gets/sets the Esophagus Temperature value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    int EsophagusTemperature { get; set; }

    /// <summary>
    /// This property gets/sets the Diaphragm Amplitude value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    int DiaphragmAmplitude { get; set; }

    double DMSDetectionThreshold { get; set; }
    int DMSDetectionThresholdValue { get; set; }

    /// <summary>
    /// This property gets/sets the Diaphragm Sensor Gain value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    int DiaphragmSensorGain { get; set; }

    /// <summary>
    /// This property gets/sets the Error value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    string Error { get; set; }

    /// <summary>
    /// This property gets/sets the Temperature Rate value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    double TemperatureRate { get; set; }

    /// <summary>
    /// This property gets/sets the Max Temperature Rate value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    double MaxTemperatureRate { get; set; }

    /// <summary>
    /// This property gets/sets the Time To Target Temperature value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    int TimeToTargetTemperature { get; set; }

    /// <summary>
    /// This property gets/sets the Treatment Number value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    int TreatmentNumber { get; set; }

    /// <summary>
    /// Gets/sets previous treatment number value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    int PreviousTreatmentNumber { get; set; }

    /// <summary>
    /// This property gets/sets the Total Treatment Number value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    int TotalTreatmentNumber { get; set; }

    /// <summary>
    /// Gets or sets a value for thawing elapsed time
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    int ThawingElapsedTime { get; set; }

    /// <summary>
    /// This property gets/sets the Ablation Timer value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    int AblationTimer { get; set; }

    /// <summary>
    /// Gets/sets total ablation duration value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    int TotalAblationDuration { get; set; }

    /// <summary>
    /// This property gets/sets the Vein Isolation Duration value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    int VeinIsolationDuration { get; set; }

    /// <summary>
    /// Gets or sets last vein isolation duration value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    int LastVeinIsolationDuration { get; set; }

    /// <summary>
    /// This property gets/sets the Expected Time To Vein Isolation value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    int ExpectedTimeToVeinIsolation { get; set; }

    /// <summary>
    /// This property gets/sets the Exceeded Expected Time To Vein Isolation value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    int NewAblationTimer { get; set; }

    /// <summary>
    /// This property gets/sets the Vein Isolation Start Time value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    int VeinIsolationStratTime { get; set; }

    /// <summary>
    /// This property gets/sets the Vein Isolation End Time value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    int VeinIsolationEndTime { get; set; }

    /// <summary>
    /// This property gets/sets the Exception State Time value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    int ExceptionStateTime { get; set; }

    /// <summary>
    /// Gets or sets duration expected vein isolation time value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    int DurationExpectedVeinIsolationTime { get; set; }

    /// <summary>
    /// Gets or sets ablation timer TTI value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    int AblationTimerTTI { get; set; }

    /// <summary>
    /// Gets or sets new ablation timer TTI value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    int NewAblationTimerTTI { get; set; }

    /// <summary>
    /// Gets or sets ablation timer TTI fixed value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    int AblationTimerTTIFixed { get; set; }

    /// <summary>
    /// Gets or sets new ablation timer TTI fixed value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    int NewAblationTimerTTIFixed { get; set; }

    /// <summary>
    /// Gets or sets the database ablation duration type value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    Enumeration.AblationDurationType AblationDurationType { get; set; }

    /// <summary>
    /// Gets/sets required ablation time according to state value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    int RequiredAblationTimeAccordingToState { get; set; }

    /// <summary>
    /// Gets or sets the time previous refrence
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    int TimePreviousRefrence { get; set; }

    /// <summary>
    /// Gets or sets the timing filter value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    int TimingFiliter { get; set; }

    /// <summary>
    /// Gets or sets a value for decreasing compter
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    int DecreasingCompter { get; set; }

    /// <summary>
    /// Gets or sets a value for previous TC1 reading
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    double PreviousTC1Reading { get; set; }

    /// <summary>
    /// This property gets/sets the Previous Generic Error value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    string PreviousGenericError { get; set; }

    /// <summary>
    /// This property gets/sets the Esophagus Temperature Threshold Reached boolean flag value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool EsophagusTemperatureThresholdReached { get; set; }

    /// <summary>
    /// This property gets/sets the Diaphragm Amplitude Threshold Reached boolean flag value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool DiaphragmAmplitudeThresholdReached { get; set; }

    /// <summary>
    /// This property gets/sets the Is Diaphragm movement detected value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool IsDiaphragmMovementDetected { get; set; }

    /// <summary>
    /// This property gets/sets the Time To Target Temperature Visible boolean flag value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool IsTimeToTargetTemperatureVisible { get; set; }

    /// <summary>
    /// This property gets/sets the Snow Flake Visible boolean flag value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool IsSnowFlakeVisible { get; set; }

    /// <summary>
    /// This property gets/sets the Treatment Number And Playback Visible boolean flag value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool IsTreatmentNumberAndPlayBackVisible { get; set; }

    /// <summary>
    /// This property gets/sets the Last Ablation Data Loaded boolean flag value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool IsLastAblationDataLoaded { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether display thawing ballon or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool DisplayThawingBallon { get; set; }

    /// <summary>
    /// This property gets/sets the Diaphragm Movement Percentage Selected value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool DiaphragmMovementPercentageSelected { get; set; }

    /// <summary>
    /// This property gets/sets the Temperature Chart Type value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    short TemperatureChartType { get; set; }

    /// <summary>
    /// This property gets/sets the Refrigerant Level Unit value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    short RefrigerantLevelUnit { get; set; }

    /// <summary>
    /// This property gets/sets the Is Isolating Vein value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool IsIsolatingVein { get; set; }

    /// <summary>
    /// This property gets/sets the Is Vein Isolation Duration Visible value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool IsVeinIsolationDurationVisible { get; set; }

    /// <summary>
    /// This property gets/sets the Is Status Ablation Balloon visible value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool IsStatusAbllationBallonVisible { get; set; }

    /// <summary>
    /// This property gets/sets the Is Square Visible value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool IsSqaureVisible { get; set; }

    /// <summary>
    /// This property gets/sets the Is Diaphragm Movement Visible value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <id>SF-SDS-0092</id>
    bool IsDiaphragmMovementVisible { get; set; }

    /// <summary>
    /// This property gets/sets the Is Esophagus Temperature Visible value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <id>SF-SDS-0093</id>
    bool IsEsophagusTemperatureVisible { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the esophagus temperature is in range
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool IsEsophagusTemperatureInRange { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether esophagus temperature condition alerts meet or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <id>SF-SDS-0098</id>
    bool IsEsophagusTemperatureConditionAlertsMeet { get; set; }

    /// <summary>
    /// This property gets/sets the Previous Generic Error value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool IsThawTemperatureReached { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether target temperature reached or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool IsTargetTemperatureReached { get; set; }

    bool IsDMSSettingPopupShow { get; set; }
    bool IsBloodPressureSettingsPopupShow { get; set; }

    /// <summary>
    /// This property gets/sets the Notification Model value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    NotificationModel NotificationModel { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether is system in idle or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool IsSystemInIdle { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether is system in ready or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <id>SF-SDS-0097</id>
    bool IsSystemInReady { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether is system in inflation or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool IsSystemInInflation { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether is system in transition or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool IsSystemInTransition { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether is system in ablation or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool IsSystemInAblation { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the system in thawing or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool IsSystemInThawing { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether is system in exception or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool IsSystemInException { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether language changed or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool IsLanguageChanged { get; set; }

    /// <summary>
    /// This property gets/sets display warning value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool DisplayAblationSiteWarning { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether required ablation time is visible or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool IsRequiredAblationTimeVisible { get; set; }

    /// <summary>
    /// Gets/sets the value indicating whether ablation time is visible or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool IsAblationTimeVisibale { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether fixed time selected or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool IsFixedTimerSelected { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether TTI fixed timer selected  or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool ISTTIFixedTimerSelected { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether TTI duration timer selected  or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool ISTTIDurationTimerSelected { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether TTI selected  or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool ISTTISelected { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether cryo duration changed or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool CryoDurationChanged { get; set; }

    /// <summary>
    /// Gets the value indicating whether user is allowed to change ablation timers or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool IsUserAllowedToChangeAblationTimers { get; }

    /// <summary>
    /// Gets the value indicating whether user is allowed to change cooling and thaw to temperature or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool IsUserAllowedToChangeCoolingAndThawToTemperature { get; }

    /// <summary>
    /// Gets or sets alert duration value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    short AlertDurationValue { get; set; }

    /// <summary>
    /// Gets or sets last diaphragm movement percentage or reading value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    double LastDiaphragmMovementPercentageOrGReadingValue { get; set; }

    /// <summary>
    /// Gets or sets a value for last flow reading
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    double LastFlowReadingValue { get; set; }

    /// <summary>
    /// Gets or sets a value for last IBP reading
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    double LastIBPReadingValue { get; set; }

    /// <summary>
    /// This property gets/sets if the DMS Detection Threshold is valid
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool IsDMSDetectionThresholdValid { get; set; }

    /// <summary>
    /// Gets/sets the value indicating whether is system monitoring diaphragm alert or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <id>SF-SDS-0095</id>
    bool IsSystemMonitoringDiaphragmAlert { get; set; }

    /// <summary>
    /// Gets/sets the value indicating whether is ablation loading aborted or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool IsLoadingAbortedAblation { get; set; }

    /// <summary>
    /// Gets/sets the value indicating whether is the pressure set point reached or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool ISThePressureSetPointReached { get; set; }

    /// <summary>
    /// Gets/sets the value indicating whether allow PSP change during thawing or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <id>SF-SDS-0096</id>
    bool AllowPSPChangeDuringThawing { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether is data loading or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool DataLoading { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether skin to skin count started or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool SkinToSkinCountStarted { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the system is allowed to set playBack mode
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool IsAllowedToSetPlayBack { get; set; }

    /// <summary>
    /// Gets or sets the thawing temperature set point.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool IsThawingTemperatureSetPointReached { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether we are monitoring blood pressure.
    ///  . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool IsMonitoringBloodPressure { get; set; }

    bool DisplayBloodPressure { get; set; }

    /// <summary>
    /// Gets or sets the database version value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    int DatabaseVersion { get; set; }

    /// <summary>
    /// Gets or sets the GUI version value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    string GUIVersion { get; set; }

    /// <summary>
    /// This property gets/sets the EnabledIsBloodPressureSensorConnected value.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool EnabledIsBloodPressureSensorConnected { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the ablation site is changed.
    ///  . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool IsAblationSiteChanged { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the can allow a user to use low flow 
    ///  . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool AllowUserToActivateLowFlow { get; set; }

    /// <summary>
    /// This property gets/sets the Is Save to DB value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool IsSavedToDB { get; set; }

    /// <summary>
    /// Gets or sets Max value of Time In Ablation
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    int TimeInAblationMax { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether Circa is using.
    ///  . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool IsUsingCirca { get; set; }

    /// <summary>
    /// Gets or sets the lowest temp channel number.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    List<int> LowestTempChannelNum { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether tip value is using.
    ///  . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool HasTip { get; set; }

    /// <summary>
    /// Gets or sets the invalid Port COM list.
    ///  . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    List<string> InvalidPortComList { get; set; }

    /// <summary>
    /// This property gets/sets the list of sesnors state playback value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    List<int> ListOfSesnorsStatePlayback { get; set; }

    bool IsFromReturnToProcedure { get; set; }
    double[] DmsData { get; }
    double[] HighResDmsData { get; }
    double[] EcgDmsData { get; }
    double[] BloodPressureData { get; }
    ICommand LastAblationCommand { get; }
    ICommand AblationNumberForwardCommand { get; }
    ICommand AblationNumberBackwardCommand { get; }
    ICommand ConnectCommand { get; }
    ICommand StartCommand { get; }
    ICommand StopCommand { get; }
    ICommand NotificationsCommand { get; }
    ICommand NotificationsChangeCommand { get; }
    ICommand OcclusionPressureSettingsChangeCommand { get; }
    ICommand IncreaseTimeCommand { get; }
    ICommand DecreaseTimeCommand { get; }
    ICommand AblationSiteCommand { get; }
    ICommand TreatmentNotesCommand { get; }
    ICommand DeflateAfterThawCommand { get; }
    ICommand VeinIsolatedCommand { get; }
    ICommand UpdateVeinIsolationDurationCommand { get; }
    ICommand ChangeTankCommand { get; }
    ICommand LockTheFootSwitchCommand { get; }
    ICommand EnableDASBallonCommand { get; }
    ICommand ResetLSPROCommand { get; }
    ICommand ActivateLowFlowCommand { get; }
    ICommand ResetDiaphragmCommand { get; }
    ICommand SaveDMSSettingCommand { get; }
    ICommand VolumeControlOnCommand { get; }
    ICommand VolumeControlOffCommand { get; }
    ICommand SaveOcclusionPressureGraphSettingsCommand { get; }
    ICommand TareOcclusionPressureGraphCommand { get; }
    ICommand ResetTareOcclusionPressureGraphCommand { get; }

    /// <summary>
    /// This property gets the Minimum DMS Detection Value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    double DMSDetectionMinValue { get; }

    /// <summary>
    /// This property gets the Minimum DMS Detection Value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    double DMSDetectionMaxValue { get; }

    /// <summary>
    /// This property gets/sets if high resolution DMS signal received 
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool HighResDmsSignalDetected { get; set; }

    /// <summary>
    /// This property gets/sets Current Ablation value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    Ablation CurrentAblation { get; set; }

    /// <summary>
    /// This read-only returns the Ablation Summary value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    AblationSummary AblationSummary { get; }

    /// <summary>
    /// This property gets/sets the Tip Pressure Selected value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool TipPressureSelected { get; set; }

    bool IgnoreMinimumDiaphragmMovementBindingValue { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the diaphragm movement is monitored
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool IgnoreMinimumDiaphragmMovementValue { get; set; }

    /// <summary>
    /// This read-only property returns the Tip or Balloon Pressure Reading value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    double TipOrBalloonPressureReading { get; }

    /// <summary>
    /// This property gets/sets the Diaphragm Movement Percentage or G Reading value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    double DiaphragmMovementPercentageOrGReading { get; set; }

    /// <summary>
    /// This property gets/sets the Diaphragm Maximum movement value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    double MaximumAveragePacingLevel { get; set; }

    /// <summary>
    /// This property gets/sets the Diaphragm Maximum movement value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    double MaximumHRAveragePacingLevel { get; set; }

    double TEMPTTI { get; set; }

    /// <summary>
    /// This property gets/sets the TC1 Reading value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    double TC1Reading { get; set; }

    /// <summary>
    /// This property gets/sets the Catheter is connecting value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool CatheterIsConnecting { get; set; }

    /// <summary>
    /// This property gets/sets the CP1 Reading value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    double CP1Reading { get; set; }

    /// <summary>
    /// This property gets/sets the PT2 Reading value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    double PT2Reading { get; set; }

    /// <summary>
    /// This property gets/sets the FM1 Reading value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    double FM1Reading { get; set; }

    /// <summary>
    /// Gets/sets the CP2 Reading value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    double CP2Reading { get; set; }

    /// <summary>
    /// This property gets/sets the Max Ecg Channel 1 And 2 Reading value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    double MaxEcgChannel1And2Reading { get; set; }

    /// <summary>
    /// This property gets/sets the Ecg Channel 1 And 2 Reading value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    double EcgChannel1And2Reading { get; set; }

    /// <summary>
    /// This property gets/sets the Ecg Channel 3 And 4 Reading value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    double EcgChannel3And4Reading { get; set; }

    /// <summary>
    /// This property gets/sets the Max Ecg Channel 3 And 4 Reading value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    double MaxEcgChannel3And4Reading { get; set; }

    /// <summary>
    /// This property gets/sets the Ecg Channel 5 And 6 Reading value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    double EcgChannel5And6Reading { get; set; }

    List<int> ListOfSesnorsState { get; set; }

    /// <summary>
    /// This property gets/sets the Ecg Channel 7 And 8 Reading value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    double EcgChannel7And8Reading { get; set; }

    /// <summary>
    /// This property gets/sets the Time to Thaw Temperature value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    int TimeToThawTemperature { get; set; }

    /// <summary>
    /// This property gets/sets the Keep Time to Thaw value (used for display purpose only)
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool KeepTimeToThaw { get; set; }

    /// <summary>
    /// This property gets/sets the Keep Time To Temperature value (used for display purpose only)
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool KeepTimeToTemperature { get; set; }

    /// <summary>
    /// This property gets/sets the Esophagus Temperature Threshold Reached boolean flag value in playback mode
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool EsophagusTemperatureThresholdReachedPlayback { get; set; }

    /// <summary>
    /// This property gets/sets the LC1 Reading value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    double LC1Reading { get; set; }

    /// <summary>
    /// This property gets/sets the LC1 Reading value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// NB: These value is reading the TS1 temperature. the firmware junction is changed.
    /// </summary>
    double TN2OReading { get; set; }

    /// <summary>
    /// This property gets/sets the CMCU cold junction Reading value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    double CMCUCJReading { get; set; }

    /// <summary>
    /// This property gets/sets the PMCU cold junction Reading value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    double PMCUCJReading { get; set; }

    /// <summary>
    /// Gets or sets the blood detector impedance
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    int BloodDetecorImValue { get; set; }

    bool IsBloodPressureSensorConnected { get; set; }
    bool IsMultiEtsSesnorConnected { get; set; }

    /// <summary>
    /// This property gets/sets the Ablation Site value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    AblationSiteEnum AblationSite { get; set; }

    /// <summary>
    /// This property gets/sets the previous ablation Site value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    AblationSiteEnum PreviousAblationSite { get; set; }

    /// <summary>
    /// This property gets/sets the CP1 Reading Playback value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    double CP1ReadingPlayback { get; set; }

    /// <summary>
    /// This property gets/sets the TC1 Reading Playback value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    double TC1ReadingPlayback { get; set; }

    /// <summary>
    /// This property gets/sets the CP2 Reading Playback value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    double CP2ReadingPlayback { get; set; }

    /// <summary>
    /// This property gets/sets the FM1 Reading Playback value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    double FM1ReadingPlayback { get; set; }

    /// <summary>
    /// This property gets/sets the PT2 Reading Playback value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    double PT2ReadingPlayback { get; set; }

    /// <summary>
    /// This property gets/sets the LC1 Reading Playback value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    double LC1ReadingPlayback { get; set; }

    /// <summary>
    /// This property gets/sets the Required ablation time Playback value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    int RequiredAblationTimePlayback { get; set; }

    /// <summary>
    /// This property gets/sets the Max ECG Channel 1 and 2 Reading Playback value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    double MaxEcgChannel1And2ReadingPlayback { get; set; }

    /// <summary>
    /// This property gets/sets the Max ECG Channel 3 and 4 Reading Playback value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    double MaxEcgChannel3And4ReadingPlayback { get; set; }

    /// <summary>
    /// This property gets/sets the ECG Channel 1 and 2 Reading Playback value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    double EcgChannel1And2ReadingPlayback { get; set; }

    /// <summary>
    /// This property gets/sets the ECG Channel 3 and 4 Reading Playback value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    double EcgChannel3And4ReadingPlayback { get; set; }

    /// <summary>
    /// This property gets/sets the ECG Channel 5 and 6 Reading Playback value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    double EcgChannel5And6ReadingPlayback { get; set; }

    /// <summary>
    /// This property gets/sets the ECG Channel 7 and 8 Reading Playback value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    double EcgChannel7And8ReadingPlayback { get; set; }

    double PressureSetPointPlayback { get; set; }

    /// <summary>
    /// This property gets/sets the CMCU System Status Error value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    Int64 CMCUSystemStatusError { get; set; }

    /// <summary>
    /// This property gets/sets the System State value.  It manages transition between states : timers,
    /// ablation cycles, cathether connection, playback and display elements value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    CanBusMessageDefinition.MessageStateId SystemState { get; set; }

    /// <summary>
    /// This property gets/sets the Is Playback Mode Deactivated value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool IsPlayBackModeDeactivted { get; set; }

    /// <summary>
    /// This property gets/sets the Elapsed Time in minute value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    int ElapsedTimeMinute { get; }

    int InBodyTime { get; }

    /// <summary>
    /// This property gets/sets the Current Time value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    DateTime CurrentTime { get; }

    /// <summary>
    /// This property gets/sets the Current Patient value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    DataAccessLayer.Patient CurrentPatient { get; set; }

    /// <summary>
    /// This property gets/sets the Catheter Cable Connected boolean flag value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool IsCatheterCableConnected { get; set; }

    /// <summary>
    /// This property gets/sets the Catheter Tube Connected boolean flag value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool IsCatheterTubeConnected { get; set; }

    /// <summary>
    /// This property gets/sets the Required Ablation Time value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>

    int RequiredAblationTime { get; set; }

    /// <summary>
    /// This property gets/sets the Temporary Ablation Time value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    int TemporaryManualAblationTime { get; set; }

    /// <summary>
    /// This property gets/sets the Was Ablation Time Manually Changed value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool WasAblationTimeManuallyChanged { get; set; }

    int RequiredTargetTemperatureBinding { get; set; }
    int ThawTimerToTemperatureBinding { get; set; }
    int EsophagusBindingTemperature { get; set; }
    int DiaphragmBindingAmplitude { get; set; }

    /// <summary>
    /// This property gets/sets the Occlusion Pressure Graph Y-Axis Maximum
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    int OcclusionPressureGraphAxisYMaximum { get; set; }

    /// <summary>
    /// This property gets/sets the Occlusion Pressure Graph Y-Axis Maximum
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    int OcclusionPressureGraphAxisYMinimum { get; set; }

    /// <summary>
    /// This property gets/sets the Blood Pressure Graph Sweep Speed
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    int OcclusionPressureGraphSweepSpeed { get; set; }

    /// <summary>
    /// This property gets/sets the Treatment Number reference value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    int TreatmentNumberRefrence { get; set; }

    /// <summary>
    /// This property gets/sets the Catheter Electrically Connected And In Idle State boolean flag value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool IsCatheterElectricallyConnectedAndInIdleState { get; set; }

    /// <summary>
    /// This property gets/sets the Catheter Connected And In Ready State boolean flag value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <id>SF-SDS-0091</id>
    bool IsCatheterConnectedAndInIReadyState { get; set; }

    /// <summary>
    /// This property gets/sets the Deflate After Thaw boolean flag value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool DeflateAfterThaw { get; set; }

    /// <summary>
    /// This property gets/sets the Enable Slow Inflation Mode value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool EnableFastInflationMode { get; set; }

    uint RequiredVolume { get; set; }
    bool IsUsingAudioAlertSetting { get; set; }

    /// <summary>
    /// This property gets/sets the Is Using Audi Alert value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool IsUsingAudioAlert { get; set; }

    /// <summary>
    /// This property gets/sets the Is Using Auto Playback value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool IsUsingAutoPlayback { get; set; }

    /// <summary>
    /// This property gets/sets the Is Using Audi Alert value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool IsUsingAudioAlertMute { get; set; }

    /// <summary>
    /// Gets or sets the Lock the foot switch boolean value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool LockTheFootSwitch { get; set; }

    /// <summary>
    /// This property gets/sets is the Vein is Isolated value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool IsVeinIsolated { get; }

    /// <summary>
    /// This property gets/sets the IsUsingBloodPressureSensor value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool IsUsingBloodPressureSensor { get; set; }

    object VisibilityValue { get; }

    /// <summary>
    /// This property gets/sets the Target Balloon Pressure value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    double TargetBalloonPressure { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether esophagus temperature condition alerts meet or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool IsEsophagusTemperatureConditionAlertsMeetPlayback { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether is an cryterion user or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool IsCryterionUser { get; }

    bool ISTTISelectedPlayback { get; set; }

    /// <summary>
    /// Gets/sets the previuos total treatment number value 
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    int PreviuosTotalTreatmentNumber { get; set; }

    /// <summary>
    /// Gets/sets the value indicating whether the software is saving ablation data
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool IsWritingDataToFile { get; set; }

    /// <summary>
    /// Gets/sets the value indicating whether the software is saving the ECG data
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool IsWritingECGDataToFile { get; set; }

    /// <summary>
    /// Gets/sets the value indicating whether is system monitoring diaphragm alert playback or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool IsSystemMonitoringDiaphragmAlertPlayback { get; set; }

    /// <summary>
    /// Gets/sets the minimum diaphragm movement last value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    int MinimumDiaphragmMovementLastValue { get; set; }

    /// <summary>
    /// Gets/sets the minimum esophagus temperature last value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    int MinimumEsophagusTemperatureLastValue { get; set; }

    /// <summary>
    /// Gets/sets the value indicating whether is system using DAS balloon or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool IsSystemUsingDASBalloon { get; }

    /// <summary>
    /// Gets/sets the value indicating whether is DAS balloon enabled or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <id>SF-SDS-0099</id>
    bool DASBalloonEnabled { get; set; }

    /// <summary>
    /// Gets/sets the value indicating whether is balloon ramp down activated or not.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool IsBalloonRampDownActivated { get; }

    /// <summary>
    /// Gets/sets the value for pressure set point or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    double PressureSetPoint { get; set; }

    bool IsTTIPopupShow { get; set; }
    bool IsReloadingPreviuosProcdure { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether is used for engineering or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool IsUsedForEngineering { get; set; }

    /// <summary>
    /// Gets or sets a value for skin to skin duration
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    int SkinToSkinDuration { get; set; }

    /// <summary>
    /// Gets or sets a value for catheter type
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    Enumeration.CatheterType CatheterType { get; }

    /// <summary>
    /// Gets or sets a value for CMCUCJ Reading Playback
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    double CMCUCJReadingPlayback { get; set; }

    /// <summary>
    /// Gets or sets a value for PMCUCJ Reading Playback
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    double PMCUCJReadingPlayback { get; set; }

    /// <summary>
    /// Gets or sets a value for blood detecor impedance value in playback
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    int BloodDetecorImValuePlayback { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the blood pressure sensor connected in playback.
    ///  . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool IsBloodPressureSensorConnectedPlayback { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the Multi ETS sensor connected in playback.
    ///  . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool IsMultiEtsSesnorConnectedPlayback { get; set; }

    /// <summary>
    /// Gets or sets the thawing temperature set point.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool IsSiteUsingDefalteAfterThaw { get; set; }

    /// <summary>
    /// Gets/sets the value indicating whether enabale enhanced audio.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool EnabaleEnhancedAudio { get; set; }

    /// <summary>
    /// Gets or sets the port name.
    ///  . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    string PortName { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the low flow is activated
    ///  . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool IsLowFlowActivated { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the console is using low flow
    ///  . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    bool IsUsingLowFlow { get; set; }

    int TTIResetCount { get; set; }
    int MaxDiaphragmSensorGain { get; }
    int MaxDMSDetectionThreshold { get; }
    bool IsSimpleTherapyViewVisible { get; set; }
    bool IsPatientNameVisible { get; set; }
    bool CanDisplayShadowGraph { get; set; }
    IObservable<bool> UpdateShadowTemperatureGraphObservable { get; }
    List<List<AblationDataDetails>> HistoricalAblationData { get; set; }
    bool IsSettingsDirty { get; set; }

    event EventHandler<AblationEventArgs> SystemStateEvent;
    event EventHandler ReadyStateEvent;
    event EventHandler<InflationEventArgs> InflationStateEvent;
    event EventHandler StopAblation;
    event EventHandler PlaybackModeEvent;
    event EventHandler TipOrBalloonPressureSelectionChangedEvent;
    event EventHandler DiaphragmMovementUnitChangedEvent;
    event EventHandler TemperatureChartTypeChangedEvent;
    event EventHandler DiaphragmSensorGainChangedEvent;
    event EventHandler ResetTherapyEvent;
    event EventHandler ChangeTankInCryotherapyEvent;
    event EventHandler<OcclusionPressureGraphAxisYEventArgs> OcclusionPressureGraphAxisYChangedEvent;
    event EventHandler OcclusionPressureGraphSweepSpeedChangedEvent;
    event EventHandler ClearOcclusionPressureGraphRequestEvent;
    void ClearDmsData();
    void ClearBloodPressureData();

    /// <summary>
    /// This property gets the foot switch state.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    void LockAndLockFootSwitch();

    void RefreshTheInBodyTime();

    /// <summary>
    /// Function that Loads the Playback mode.  It loads single ablation data and ECG data lists
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="treatmentNumber"></param>
    void LoadPlaybackMode(int treatmentNumber);

    /// <summary>
    /// Function that resets the cryotherapy counters, properties objects and lists.  It also invokes
    /// the reset cryotherapy event
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    void ResetCryoTherapy();

    /// <summary>
    /// Function that resets the display using the Physician's preferences
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    void ResetDisplayWithPhysicianPreferences();

    /// <summary>
    /// Function that invokes the Reset Therapy event when the Playback data is called
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    void ResetCryoTherapyPlayBackData();

    /// <summary>
    /// Function that invokes the Reset the balloon seize
    ///    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    void ResetDASBalloonSize();

    /// <summary>
    /// Set required ablation time according to state
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    void SetrequiRedAblationTimeAccordingToState(int _redAblationTime);

    /// <summary>
    /// Function Refresh Ablation Time
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    void RefreshModeldata();

    /// <summary>
    /// Refresh the N2O weight and unit(kg/lbs)
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    void RefreshWeightData();

    void OnVeinCmd(bool? moveTTI);
    void UpdateAblationSiteChanged(AblationSiteEnum newAblationSite);
    event PropertyChangedEventHandler PropertyChanged;
  }
}