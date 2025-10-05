using DataAccessLayer;
using Prism.Mvvm;
using SmartAblationSystem.Helpers;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Prism.Commands;
using SmartAblationSystem.Models;
using static LogSystem.LogService;
using System.ComponentModel;
using System.Reactive.Linq;

using static SmartAblationSystem.Helpers.Enumeration;

namespace SmartAblationSystem.ViewModels
{
  public class NotificationsViewModel : BindableBase
  {
    private static readonly int _volumeIncrement = 10;
    private static readonly int _minVolume = 0; 
    private static readonly int _maxVolume = 100;

    private static readonly double _expectedTimerIncremental = 10; 
    private static readonly double _ablationTimerIncremental = 30; 

    private readonly ICryoTherapyViewModel _cryoTherapyViewModel;

    // Temperature settings
    private int _requiredTargetTemperature;
    private int _thawTimerToTemperature;
    private int _lowAblationTemperatureAlarm;
    private int _highAblationTemperatureAlarm;

    // DMS related properties 
    private int _esophagusTemperature;
    private int _diaphragmAmplitude;
    private double _dmsDetectionThreshold;
    private int _dmsDetectionThresholdValue;
    private int _diaphragmSensorGain;
    private bool _ignoreMinimumDiaphragmMovementValue;
    private bool _isUsingAudioAlert;
    private bool _isDMSDetectionThresholdValid;

    // System settings
    private InflationSpeedMode _inflationSpeedMode;
    private bool _enableEnhancedAudio;
    private bool _isUsingAutoPlayback;
    private Enumeration.RefrigerantUnit _refrigerantLevelUnit;
    private bool _deflateAfterThaw;

    private Enumeration.CurveStyle _curveStyle;
    private bool _canDisplayShadowGraph = false;
    private uint _requiredVolume;

    // Ablation Timer settings 
    private Enumeration.AblationDurationType _ablationDurationType;
    private bool _isFixedTimerSelected;
    private bool _isTTIFixedTimerSelected;
    private bool _isTTIDurationTimerSelected;

    private readonly HashSet<string> _settingPropertiesNames = new HashSet<string>()
    {
      nameof(RequiredTargetTemperature),
      nameof(ThawTimerToTemperature),
      nameof(LowAblationTemperatureAlarm),
      nameof(HighAblationTemperatureAlarm),
      nameof(EsophagusTemperature),
      nameof(DiaphragmAmplitude),
      nameof(DMSDetectionThreshold),
      nameof(DMSDetectionThresholdValue),
      nameof(DiaphragmSensorGain),
      nameof(IgnoreMinimumDiaphragmMovementValue),
      nameof(IsUsingAudioAlert),
      nameof(InflationSpeedMode),
      nameof(EnableEnhancedAudio),
      nameof(IsUsingAutoPlayback),
      nameof(CurveStyle),
      nameof(RefrigerantLevelUnit),
      nameof(DeflateAfterThaw),
      nameof(CanDisplayShadowGraph),
      nameof(RequiredVolume),
      nameof(AblationDurationType),
      nameof(ExpectedTimeToVeinIsolation),
      nameof(AblationTimerTTIFixed),
      nameof(NewAblationTimerTTIFixed),
      nameof(DurationExpectedVeinIsolationTime),
      nameof(AblationTimerTTI),
      nameof(NewAblationTimerTTI),
      nameof(AblationTimer)
    };

    public NotificationsViewModel(ICryoTherapyViewModel cryoTherapyViewModel)
    {
      _cryoTherapyViewModel = cryoTherapyViewModel;
      ResetLSProCommand = new DelegateCommand(ExecuteResetLSProCommunication, () => true);

      IncreaseVolumeCommand = new DelegateCommand(ExecuteIncreaseVolume, () => true);
      DecreaseVolumeCommand = new DelegateCommand(ExecuteDecreaseVolume, () => true);

      InitializeSettings();

      Observable.FromEventPattern<PropertyChangedEventArgs>(this, "PropertyChanged")
        .Where(e => !IsInitializing && _settingPropertiesNames.Contains(e.EventArgs.PropertyName))
        .Subscribe(e =>
        {
          IsSettingsDirtyFromUI = true;
          IsSettingsDirtyFromDB = true;
          CanLoadDefault = IsUserAllowedToChangeAblationTimers;
        });
    }

    public ICommand ResetLSProCommand { get; private set; }

    public ICommand IncreaseVolumeCommand { get; }
    public ICommand DecreaseVolumeCommand { get; }

    /// <summary>
    /// This property gets/sets the Required Target Temperature value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <id>SF-SDS-0107</id>
    public int RequiredTargetTemperature
    {
      get => _requiredTargetTemperature;
      set => SetProperty(ref _requiredTargetTemperature, value);
    }

    /// <summary>
    /// This property gets/sets the Thaw Timer To Temperature value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <id>SF-SDS-0108</id> 
    public int ThawTimerToTemperature
    {
      get => _thawTimerToTemperature;
      set => SetProperty(ref _thawTimerToTemperature, value);
    }

    /// <summary>
    /// This property gets/sets the Low Ablation Temperature Alarm value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <id>SF-SDS-0109</id>
    public int LowAblationTemperatureAlarm
    {
      get => _lowAblationTemperatureAlarm;
      set => SetProperty(ref _lowAblationTemperatureAlarm, value);
    }

    /// <summary>
    /// This property gets/sets the High Ablation Temperature Alarm value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <id>SF-SDS-0110</id>
    public int HighAblationTemperatureAlarm
    {
      get => _highAblationTemperatureAlarm;
      set => SetProperty(ref _highAblationTemperatureAlarm, value);
    }

    /// <summary>
    /// This property gets/sets the Esophagus Temperature value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <id>SF-SDS-0111</id>
    public int EsophagusTemperature
    {
      get => _esophagusTemperature;
      set => SetProperty(ref _esophagusTemperature, value);
    }

    /// <summary>
    /// This property gets/sets the Diaphragm Amplitude value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <id>SF-SDS-0112</id>
    public int DiaphragmAmplitude
    {
      get => _diaphragmAmplitude;
      set => SetProperty(ref _diaphragmAmplitude, value);
    }

    /// <summary>
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <id>SF-SDS-0113</id>

    public double DMSDetectionThreshold
    {
      get => _dmsDetectionThreshold;
      set => SetProperty(ref _dmsDetectionThreshold, value);
    }

    public bool IsDMSDetectionThresholdValid
    {
      get => _isDMSDetectionThresholdValid;
      set => SetProperty(ref _isDMSDetectionThresholdValid, value);
    }
    /// <summary>
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <id>SF-SDS-0114</id>

    public int DMSDetectionThresholdValue
    {
      get => _dmsDetectionThresholdValue;
      set => SetProperty(ref _dmsDetectionThresholdValue, value);
    }

    /// <summary>
    /// This property gets/sets the Diaphragm Sensor Gain value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <id>SF-SDS-0115</id>
    public int DiaphragmSensorGain
    {
      get => _diaphragmSensorGain;
      set => SetProperty(ref _diaphragmSensorGain, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the diaphragm movement is monitored
    /// . Safety classification: No injury or damage to health is possible(IEC 62304 Class A).
    /// </summary>
    /// <id>SF-SDS-0116</id>
    public bool IgnoreMinimumDiaphragmMovementValue
    {
      get => _ignoreMinimumDiaphragmMovementValue;
      set => SetProperty(ref _ignoreMinimumDiaphragmMovementValue, value);
    }

    /// <summary>
    /// This property gets/sets the Is Using Audi Alert value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <id>SF-SDS-0117</id>
    public bool IsUsingAudioAlert
    {
      get => _isUsingAudioAlert;
      set => SetProperty(ref _isUsingAudioAlert, value);
    }

    /// <summary>
    /// This property gets/sets the Enable Slow Inflation Mode value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <id>SF-SDS-0118</id>
    public Enumeration.InflationSpeedMode InflationSpeedMode
    {
      get => _inflationSpeedMode;
      set => SetProperty(ref _inflationSpeedMode, value);
    }

    /// <summary>
    /// Gets/sets the value indicating whether enable enhanced audio.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <id>SF-SDS-0119</id>
    public bool EnableEnhancedAudio
    {
      get => _enableEnhancedAudio;
      set => SetProperty(ref _enableEnhancedAudio, value);
    }

    /// <summary>
    /// This property gets/sets the Is Using Auto Playback value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <id>SF-SDS-0120</id>
    public bool IsUsingAutoPlayback
    {
      get => _isUsingAutoPlayback;
      set => SetProperty(ref _isUsingAutoPlayback, value);
    }
    /// <summary>
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <id>SF-SDS-0121</id>
    public Enumeration.CurveStyle CurveStyle
    {
      get => _curveStyle;
      set => SetProperty(ref _curveStyle, value);
    }

    /// <summary>
    /// This property gets/sets the Refrigerant Level Unit value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <id>SF-SDS-0122</id>
    public Enumeration.RefrigerantUnit RefrigerantLevelUnit
    {
      get => _refrigerantLevelUnit;
      set => SetProperty(ref _refrigerantLevelUnit, value);
    }

    /// <summary>
    /// This property gets/sets the Deflate After Thaw boolean flag value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <id>SF-SDS-0123</id>
    public bool DeflateAfterThaw
    {
      get => _deflateAfterThaw;
      set => SetProperty(ref _deflateAfterThaw, value);
    }

    private bool _isSiteUsingDefalteAfterThaw;
    public bool IsSiteUsingDefalteAfterThaw
    {
      get => _isSiteUsingDefalteAfterThaw;
      set => SetProperty(ref _isSiteUsingDefalteAfterThaw, value);
    }
    /// <summary>
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <id>SF-SDS-0124</id>

    public bool CanDisplayShadowGraph
    {
      get => _canDisplayShadowGraph;
      set => SetProperty(ref _canDisplayShadowGraph, value);
    }
    /// <summary>
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <id>SF-SDS-0125</id>

    public uint RequiredVolume
    {
      get => _requiredVolume;
      set => SetProperty(ref _requiredVolume, value);
    }

    /// <summary>
    /// Gets or sets the database ablation duration type value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <id>SF-SDS-0126</id>
    public Enumeration.AblationDurationType AblationDurationType
    {
      get => _ablationDurationType;
      set
      {
        this.SetProperty(ref this._ablationDurationType, value);
        IsFixedTimerSelected = value == AblationDurationType.FixedTimer; // _cryoTherapyViewModel.IsFixedTimerSelected;
        ISTTIFixedTimerSelected = value == AblationDurationType.TTIFixedTimer; //_cryoTherapyViewModel.ISTTIFixedTimerSelected;
        ISTTIDurationTimerSelected = value == AblationDurationType.TTIDurationTimer; //_cryoTherapyViewModel.ISTTIDurationTimerSelected;
      }
    }

    public bool IsFixedTimerSelected
    {
      get => _isFixedTimerSelected;
      set => SetProperty(ref _isFixedTimerSelected, value);
    }

    public bool ISTTIFixedTimerSelected
    {
      get => _isTTIFixedTimerSelected;
      set => SetProperty(ref _isTTIFixedTimerSelected, value);
    }

    public bool ISTTIDurationTimerSelected
    {
      get => _isTTIDurationTimerSelected;
      set => SetProperty(ref _isTTIDurationTimerSelected, value);
    }

    private int _requiredAblationTime;
    public int RequiredAblationTime
    {
      get => _requiredAblationTime;
      set => SetProperty(ref _requiredAblationTime, value);
    }
    /// <summary>
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <id>SF-SDS-0127</id>

    private int _ablationTimer;
    public int AblationTimer
    {
      get => _ablationTimer;
      set => SetProperty(ref _ablationTimer, value);
    }

    /// <summary>
    /// This property gets/sets the Temporary Ablation Time value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int TemporaryManualAblationTime
    {
      get;
      set;
    }
    
    private double RoundTimerWithIncremental(double value, double incremental)
    {
      return incremental * Math.Floor(value / incremental);
    }
    
    /// <summary>
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <id>SF-SDS-0128</id>
    private int _expectedTimeToVeinIsolation;
    public int ExpectedTimeToVeinIsolation
    {
      get => _expectedTimeToVeinIsolation;
      set
      {
        this.SetProperty(ref this._expectedTimeToVeinIsolation, value);
        MinTTIFixedAblationTimer1 = Math.Max(RoundTimerWithIncremental(value + _ablationTimerIncremental, _ablationTimerIncremental), 60d);
      }
    }

    private int _ablationTimerTTIFixed;
    /// <summary>
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <id>SF-SDS-0129</id>
    public int AblationTimerTTIFixed
    {
      get => _ablationTimerTTIFixed;
      set
      {
        this.SetProperty(ref this._ablationTimerTTIFixed, value);
        MaxFixedTTIExpectedTimer = Math.Min(value - _expectedTimerIncremental, 200d);
        MinTTIFixedAblationTimer2 = Math.Max(RoundTimerWithIncremental(value + _ablationTimerIncremental, _ablationTimerIncremental), 90);
        NewAblationTimerTTIFixed = (int)Math.Max(NewAblationTimerTTIFixed, MinTTIFixedAblationTimer2);
      }
    }
    private int _newAblationTimerTTIFixed;
    /// <summary>
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <id>SF-SDS-0130</id>
    public int NewAblationTimerTTIFixed
    {
      get => _newAblationTimerTTIFixed;
      set => SetProperty(ref _newAblationTimerTTIFixed, value);
    }

    private int _durationExpectedVeinIsolationTime;
    /// <summary>
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <id>SF-SDS-0131</id>
    public int DurationExpectedVeinIsolationTime
    {
      get => _durationExpectedVeinIsolationTime; 
      set => SetProperty(ref _durationExpectedVeinIsolationTime, value);
    }


    private int _ablationTimerTTI;
    /// <summary>
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <id>SF-SDS-0132</id>
    public int AblationTimerTTI
    {
      get => _ablationTimerTTI;
      set
      {
        this.SetProperty(ref this._ablationTimerTTI, value);
        MinTTIDurationAblationTimer2 = Math.Max(RoundTimerWithIncremental(value + _ablationTimerIncremental, _ablationTimerIncremental), 60);
        NewAblationTimerTTI = (int)Math.Max(NewAblationTimerTTI, MinTTIDurationAblationTimer2);
      }
    }

    private int _newAblationTimerTTI;
    /// <summary>
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <id>SF-SDS-0133</id>
    public int NewAblationTimerTTI
    {
      get => _newAblationTimerTTI; 
      set => SetProperty(ref _newAblationTimerTTI, value);
    }

    private bool _canSavePreferences;
    public bool CanSavePreferences
    {
      get => _canSavePreferences;
      set => SetProperty(ref _canSavePreferences, value);
    }

    private double _maxFixedTTIExpectedTimer = 30;

    public double MaxFixedTTIExpectedTimer //
    {
      get => this._maxFixedTTIExpectedTimer;
      set => SetProperty(ref _maxFixedTTIExpectedTimer, value);
    }

    private double _minTTIFixedAblationTimer1; //ExpectedTimeToVeinIsolation
    public double MinTTIFixedAblationTimer1
    {
      get => this._minTTIFixedAblationTimer1;
      set => SetProperty(ref this._minTTIFixedAblationTimer1, value);
    }

    private double _minTTIFixedAblationTimer2; //AblationTimerTTIFixed
    public double MinTTIFixedAblationTimer2
    {
      get => this._minTTIFixedAblationTimer2;
      set => SetProperty(ref this._minTTIFixedAblationTimer2, value);
    }

    private double _minTTIDurationAblationTimer2;

    public double MinTTIDurationAblationTimer2 //AblationTimerTTI
    {
      get => this._minTTIDurationAblationTimer2;
      set => SetProperty(ref _minTTIDurationAblationTimer2, value);
    }

    // MinTTIFixedAblationTimer2
    // private double _maxTTIFixedAblationTimer1; //ExpectedTimeToVeinIsolation
    // public double MaxTTIFixedAblationTimer1
    // {
    //   get => this._maxTTIFixedAblationTimer1;
    //   set => SetProperty(ref this._maxTTIFixedAblationTimer1, value);
    // }

    /// <summary>
    /// Gets the value indicating whether user is allowed to change ablation timers or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <id>SF-SDS-0134</id>
    public bool IsUserAllowedToChangeAblationTimers => _cryoTherapyViewModel.IsUserAllowedToChangeAblationTimers;

    /// <summary>
    /// Gets the value indicating whether user is allowed to change cooling and thaw to temperature or not
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <id>SF-SDS-0135</id>
    public bool IsUserAllowedToChangeCoolingAndThawToTemperature => _cryoTherapyViewModel.IsUserAllowedToChangeCoolingAndThawToTemperature; 

    private bool _isSettingsDirtyFromDB;
    public bool IsSettingsDirtyFromDB
    {
      get => _isSettingsDirtyFromDB;
      set
      {
        SetProperty(ref _isSettingsDirtyFromDB, value);
        CanSavePreferences = value; 
      }
    }

    private bool _isSettingsDirtyFromUI;
    public bool IsSettingsDirtyFromUI
    {
      get => _isSettingsDirtyFromUI;
      set => SetProperty(ref _isSettingsDirtyFromUI, value);
    }

    private bool _canLoadDefault;
    public bool CanLoadDefault
    {
      get => _canLoadDefault;
      set => SetProperty(ref _canLoadDefault, value); 
    }

    public bool IsInitializing { get; set; }

    public void ApplySettings()
    {
      _cryoTherapyViewModel.RequiredTargetTemperature = RequiredTargetTemperature;
      _cryoTherapyViewModel.ThawTimerToTemperature = ThawTimerToTemperature;
      _cryoTherapyViewModel.LowAblationTemperatureAlarm = LowAblationTemperatureAlarm;
      _cryoTherapyViewModel.HighAblationTemperatureAlarm = HighAblationTemperatureAlarm;

      _cryoTherapyViewModel.EsophagusTemperature = EsophagusTemperature;
      _cryoTherapyViewModel.DiaphragmAmplitude = DiaphragmAmplitude;
      _cryoTherapyViewModel.DMSDetectionThreshold = DMSDetectionThreshold;
      _cryoTherapyViewModel.DMSDetectionThresholdValue = DMSDetectionThresholdValue;
      _cryoTherapyViewModel.DiaphragmSensorGain = DiaphragmSensorGain;
      _cryoTherapyViewModel.IgnoreMinimumDiaphragmMovementValue = IgnoreMinimumDiaphragmMovementValue;
      _cryoTherapyViewModel.IsDMSDetectionThresholdValid = IsDMSDetectionThresholdValid;

      _cryoTherapyViewModel.IsUsingAudioAlertSetting = IsUsingAudioAlert;
      _cryoTherapyViewModel.EnableFastInflationMode = InflationSpeedMode == InflationSpeedMode.Fast;
      _cryoTherapyViewModel.EnabaleEnhancedAudio = EnableEnhancedAudio;
      _cryoTherapyViewModel.IsUsingAutoPlayback = IsUsingAutoPlayback;
      _cryoTherapyViewModel.TemperatureChartType = (short)CurveStyle;
      _cryoTherapyViewModel.RefrigerantLevelUnit = (short)RefrigerantLevelUnit;
      _cryoTherapyViewModel.DeflateAfterThaw = DeflateAfterThaw;
      _cryoTherapyViewModel.IsSiteUsingDefalteAfterThaw = IsSiteUsingDefalteAfterThaw;
      _cryoTherapyViewModel.CanDisplayShadowGraph = CanDisplayShadowGraph;
      _cryoTherapyViewModel.RequiredVolume = RequiredVolume;

      if (IsUserAllowedToChangeAblationTimers)
      {
        _cryoTherapyViewModel.AblationDurationType = AblationDurationType;
        _cryoTherapyViewModel.IsFixedTimerSelected = IsFixedTimerSelected;
        _cryoTherapyViewModel.ISTTIFixedTimerSelected = ISTTIFixedTimerSelected;
        _cryoTherapyViewModel.ISTTIDurationTimerSelected = ISTTIDurationTimerSelected;
        _cryoTherapyViewModel.RequiredAblationTime = RequiredAblationTime;
        _cryoTherapyViewModel.TemporaryManualAblationTime = TemporaryManualAblationTime;

        _cryoTherapyViewModel.ExpectedTimeToVeinIsolation = ExpectedTimeToVeinIsolation;
        _cryoTherapyViewModel.AblationTimerTTIFixed = AblationTimerTTIFixed;
        _cryoTherapyViewModel.NewAblationTimerTTIFixed = NewAblationTimerTTIFixed;

        _cryoTherapyViewModel.DurationExpectedVeinIsolationTime = DurationExpectedVeinIsolationTime;
        _cryoTherapyViewModel.AblationTimerTTI = AblationTimerTTI;
        _cryoTherapyViewModel.NewAblationTimerTTI = NewAblationTimerTTI;

        _cryoTherapyViewModel.AblationTimer = AblationTimer;
      }

      _cryoTherapyViewModel.IsSettingsDirty = IsSettingsDirtyFromDB; 
    }

    public void ResetSettingsFromPreferences(preference preference_)
    {
      if (preference_ == null) return;

      RequiredTargetTemperature = (int)preference_.CoolingRequiredTargetTemperature;
      ThawTimerToTemperature = (int)preference_.ThawTimerToTemperature;

      LowAblationTemperatureAlarm = (int)preference_.LowAblationTemperatureAlarm;
      HighAblationTemperatureAlarm = (int)preference_.HighAblationTemperatureAlarm;
      EsophagusTemperature = (int)preference_.EsophagusTemperature;
      DiaphragmAmplitude = (int)preference_.DiaphragmAmplitude;

      DMSDetectionThreshold = Math.Max(preference_.DMSDetectionThreshold, Constants.MaxDMSDetectionThreshold);
      DMSDetectionThresholdValue = ConvertTheDMSTOTenBase(DMSDetectionThreshold);
      // IsDMSDetectionThresholdValid = preference_.IsDMSDetectionThresholdValid;

      DiaphragmSensorGain = Math.Min(preference_.DiaphragmSensorGain, Constants.MaxDiaphragmSensorGain);
      IgnoreMinimumDiaphragmMovementValue = preference_.IgnoreDiaphragmMovement;
      IsUsingAudioAlert = preference_.IsUsingAudioAlert;
      InflationSpeedMode = preference_.IsUsingInflationFastSpeed ? InflationSpeedMode.Slow : InflationSpeedMode.Fast; // For historical reason, inflation speed is saved reversed in DB
      EnableEnhancedAudio = preference_.EnabaleEnhancedAudio;
      IsUsingAutoPlayback = preference_.IsUsingAutoPlayback;

      CurveStyle = (Enumeration.CurveStyle)preference_.CurveStyle;
      RefrigerantLevelUnit = (Enumeration.RefrigerantUnit)preference_.RefrigerantLevelUnit;

      DeflateAfterThaw = IsSiteUsingDefalteAfterThaw || preference_.IsUsingAutoDeflation;

      CanDisplayShadowGraph = preference_.IsUsingShadowing;
      RequiredVolume = (uint)preference_.VolumeLevel;

      AblationDurationType = (Enumeration.AblationDurationType)preference_.AblationDurationType;

      IsFixedTimerSelected = AblationDurationType == Enumeration.AblationDurationType.FixedTimer;
      ISTTIFixedTimerSelected = AblationDurationType == Enumeration.AblationDurationType.TTIFixedTimer;
      ISTTIDurationTimerSelected = AblationDurationType == Enumeration.AblationDurationType.TTIDurationTimer;

      RequiredAblationTime = preference_.AblationTimer;
      if (ISTTIFixedTimerSelected)
      {
        RequiredAblationTime = preference_.NewAblationTimerTTIFixed;
      }
      else if (ISTTIDurationTimerSelected)
      {
        RequiredAblationTime = 240;
      }

      ExpectedTimeToVeinIsolation = preference_.ExpectedVeinIsolationTime;
      AblationTimerTTIFixed = preference_.AblationTimerTTIFixed;
      NewAblationTimerTTIFixed = preference_.NewAblationTimerTTIFixed;

      DurationExpectedVeinIsolationTime = preference_.DurationExpectedVeinIsolationTime;
      AblationTimerTTI = preference_.AblationTimerTTI;
      NewAblationTimerTTI = preference_.NewAblationTimerTTI;

      TemporaryManualAblationTime = RequiredAblationTime;
      AblationTimer = preference_.AblationTimer;

      IsSettingsDirtyFromDB = false;
      CanLoadDefault = false; 
    }

    public void SaveSettingsToPreference()
    {
      var preference = NotificationModel.Instance.CurrentPhysician.preference; 

      if (preference == null) return;

      preference.CoolingRequiredTargetTemperature = RequiredTargetTemperature;
      preference.ThawTimerToTemperature = ThawTimerToTemperature;

      preference.LowAblationTemperatureAlarm = LowAblationTemperatureAlarm;
      preference.HighAblationTemperatureAlarm = HighAblationTemperatureAlarm;
      preference.EsophagusTemperature = EsophagusTemperature;
      preference.DiaphragmAmplitude = DiaphragmAmplitude;
      
      DMSDetectionThreshold = ConvertTheTenBaseTODMS(DMSDetectionThresholdValue); 
      preference.DMSDetectionThreshold = DMSDetectionThreshold;

      preference.DiaphragmSensorGain = (short)DiaphragmSensorGain;
      preference.IgnoreDiaphragmMovement = IgnoreMinimumDiaphragmMovementValue;

      preference.IsUsingAudioAlert = IsUsingAudioAlert;
      preference.IsUsingInflationFastSpeed = InflationSpeedMode != InflationSpeedMode.Fast; // For historical reason, inflation speed is saved reversed in DB
      preference.EnabaleEnhancedAudio = EnableEnhancedAudio;
      preference.IsUsingAutoPlayback = IsUsingAutoPlayback;

      preference.CurveStyle = (short)CurveStyle;
      preference.RefrigerantLevelUnit = (short)RefrigerantLevelUnit;

      if (!IsSiteUsingDefalteAfterThaw)
        preference.IsUsingAutoDeflation = DeflateAfterThaw;

      preference.IsUsingShadowing = CanDisplayShadowGraph;
      preference.VolumeLevel = (short)RequiredVolume;
      preference.AblationDurationType = (short)AblationDurationType;

      preference.AblationTimer = (short)AblationTimer;

      preference.ExpectedVeinIsolationTime = ExpectedTimeToVeinIsolation;
      preference.AblationTimerTTIFixed = AblationTimerTTIFixed;
      preference.NewAblationTimerTTIFixed = NewAblationTimerTTIFixed;

      preference.DurationExpectedVeinIsolationTime = DurationExpectedVeinIsolationTime;
      preference.AblationTimerTTI = AblationTimerTTI;
      preference.NewAblationTimerTTI = NewAblationTimerTTI;

      NotificationModel.Instance.SaveNotification();

      IsSettingsDirtyFromDB = false;
      _cryoTherapyViewModel.IsSettingsDirty = false; 
    }

    private void InitializeSettings()
    {
      IsInitializing = true;
      RequiredTargetTemperature = _cryoTherapyViewModel.RequiredTargetTemperature;
      ThawTimerToTemperature = _cryoTherapyViewModel.ThawTimerToTemperature;
      LowAblationTemperatureAlarm = _cryoTherapyViewModel.LowAblationTemperatureAlarm;
      HighAblationTemperatureAlarm = _cryoTherapyViewModel.HighAblationTemperatureAlarm;
      EsophagusTemperature = _cryoTherapyViewModel.EsophagusTemperature;
      DiaphragmAmplitude = _cryoTherapyViewModel.DiaphragmAmplitude;

      DMSDetectionThreshold = Math.Max(_cryoTherapyViewModel.DMSDetectionThreshold, Constants.MaxDMSDetectionThreshold);
      DMSDetectionThresholdValue = _cryoTherapyViewModel.DMSDetectionThresholdValue;
      IsDMSDetectionThresholdValid = _cryoTherapyViewModel.IsDMSDetectionThresholdValid;

      DiaphragmSensorGain = Math.Min(_cryoTherapyViewModel.DiaphragmSensorGain, Constants.MaxDiaphragmSensorGain);
      IgnoreMinimumDiaphragmMovementValue = _cryoTherapyViewModel.IgnoreMinimumDiaphragmMovementValue;
      IsUsingAudioAlert = _cryoTherapyViewModel.IsUsingAudioAlertSetting;
      InflationSpeedMode = _cryoTherapyViewModel.EnableFastInflationMode ? InflationSpeedMode.Fast : InflationSpeedMode.Slow;
      EnableEnhancedAudio = _cryoTherapyViewModel.EnabaleEnhancedAudio;
      IsUsingAutoPlayback = _cryoTherapyViewModel.IsUsingAutoPlayback;
      CurveStyle = (Enumeration.CurveStyle)_cryoTherapyViewModel.TemperatureChartType;
      RefrigerantLevelUnit = (Enumeration.RefrigerantUnit)_cryoTherapyViewModel.RefrigerantLevelUnit;
      DeflateAfterThaw = _cryoTherapyViewModel.DeflateAfterThaw;
      IsSiteUsingDefalteAfterThaw = _cryoTherapyViewModel.IsSiteUsingDefalteAfterThaw;
      CanDisplayShadowGraph = _cryoTherapyViewModel.CanDisplayShadowGraph;
      RequiredVolume = _cryoTherapyViewModel.RequiredVolume;

      AblationDurationType = _cryoTherapyViewModel.AblationDurationType;

      RequiredAblationTime = _cryoTherapyViewModel.RequiredAblationTime;
      TemporaryManualAblationTime = _cryoTherapyViewModel.TemporaryManualAblationTime;

      ExpectedTimeToVeinIsolation = _cryoTherapyViewModel.ExpectedTimeToVeinIsolation;
      AblationTimerTTIFixed = _cryoTherapyViewModel.AblationTimerTTIFixed;
      NewAblationTimerTTIFixed = _cryoTherapyViewModel.NewAblationTimerTTIFixed;

      DurationExpectedVeinIsolationTime = _cryoTherapyViewModel.DurationExpectedVeinIsolationTime;
      AblationTimerTTI = _cryoTherapyViewModel.AblationTimerTTI;
      NewAblationTimerTTI = _cryoTherapyViewModel.NewAblationTimerTTI;
      AblationTimer = _cryoTherapyViewModel.AblationTimer;

      IsSettingsDirtyFromDB = _cryoTherapyViewModel.IsSettingsDirty;
      IsInitializing = false;

      CanLoadDefault = IsSettingsDirtyFromDB && IsUserAllowedToChangeAblationTimers; 
    }

    #region static methods

    /// <summary>
    /// Convert the DMS to ten base
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="DMSDetectionThreshold"> the value to convert</param>
    /// <returns> to dms base value</returns>
    private  static int ConvertTheDMSTOTenBase(double DMSDetectionThreshold)
    {
      double dMSTOTenBase = -100 * DMSDetectionThreshold + 11;

      return (int)Math.Round(dMSTOTenBase, 0);
    }

    private static double ConvertTheTenBaseTODMS(int value)
    {
      double TenBase = (double)(11 - value) / 100;
      return Math.Round(TenBase, 2);
    }

    #endregion static methods

    private void ExecuteResetLSProCommunication()
    {
      try
      {
        _cryoTherapyViewModel.ResetLSPROCommand.Execute(null);
      }
      catch (Exception ex) 
      {
        LogException(ex);
      }
    }

    private void ExecuteIncreaseVolume()
    {
      RequiredVolume = (uint)Math.Min(RequiredVolume + _volumeIncrement, _maxVolume); 
    }

    private void ExecuteDecreaseVolume()
    {
      RequiredVolume = (uint)Math.Max(RequiredVolume - _volumeIncrement, _minVolume); 
    }
  }
}