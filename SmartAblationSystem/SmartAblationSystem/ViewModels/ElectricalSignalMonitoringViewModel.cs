using Console;
using Prism.Commands;
using Prism.Mvvm;
using SmartAblationSystem.Helpers;
using SmartAblationSystem.Views;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Controls;
using static Communication.CanBusMessageDefinition;

namespace SmartAblationSystem.ViewModels
{
	/// <summary>
	/// This class is the Electrical Signal Monitoring View Model
	/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
	/// </summary>
	internal class ElectricalSignalMonitoringViewModel : BindableBase
	{
		private bool isGPIO0Activated;
		private bool isGPIO1Activated;
		private bool isGPIO2Activated;
		private bool isGPIO3Activated;
		private bool isGPIO4Activated;
		private bool isGPIO5Activated;
		private bool isGPIO6Activated;
		private bool isGPIO7Activated;
		private uint level;

		//SVs
		private bool isSV0Activated;

		private bool isSV1Activated;
		private bool isSV2Activated;
		private bool isSV3Activated;
		private bool isSV4Activated;
		private bool isSV5Activated;
		private bool isSV6Activated;
		private bool isSV7Activated;
		private bool isSV8Activated;
		private bool isFANActivated;
		private bool isSV10Activated;
		private bool isSV11Activated;

		private uint svLevel1;
		private uint svLevel2;
		private uint svLevel3;
		private uint svLevel4;
		private uint svLevel5;
		private uint svLevel6;
		private uint svLevel7;
		private uint svLevel8;
		private uint svLevel9;
		private uint fanLevel;
		private uint svLevel10;
		private uint svLevel11;
		private uint svLevelPrevious;

		private string selectedState = string.Empty;
		private ComboBoxItem selectedRegister;
		private ComboBoxItem pateintSelectedRegister;

		private bool isClosing;
		private bool isCatheterConnected;

		private int lastUseHour;
		private int lastUseDay;
		private int lastUseMonth;
		private int lastUseYear;
		private int serilaNumber;
		private int lotNumber;

		public DelegateCommand<object> SetGPIOLevelCommand { get; }
		public DelegateCommand<object> SetSVLevelCommand { get; }
		public DelegateCommand<object> MaintenanceCommand { get; }
		public DelegateCommand<object> ConnectCommand { get; }
		public DelegateCommand<object> StartCommand { get; }
		public DelegateCommand<object> StopCommand { get; }
		public DelegateCommand<object> DeflateAfterThawCommand { get; }
		public DelegateCommand<object> WriteConnectionBoxDiaphragmMinMaxCommand { get; }
		public DelegateCommand<object> LockTheFootSwitchCommand { get; }
		public DelegateCommand<object> WriteToMicroControllerCommand { get; }
		public DelegateCommand<object> ReadFromMicroControllerCommand { get; }
		public DelegateCommand<object> CloseCommand { get; }
		public DelegateCommand<object> LogoutCommand { get; }
		public DelegateCommand<object> ProgramCatheterCommand { get; }
		public DelegateCommand<object> FastButtonCommand { get; }
		public DelegateCommand<object> SlowButtonCommand { get; }

		private const int maxWritingTime = 8;

		private string readingArg = string.Empty;

		private bool isReadingFromCMCU;
		private CommonViewModel localCommonViewModel = CommonViewModel.Current;

		private bool catheterIsConnecting;

		private bool _isCommandEnabled = true;

		public bool IsCommandEnabled
		{
			get => _isCommandEnabled;
			set => SetProperty(ref _isCommandEnabled, value);
		}

		/// <summary>
		/// This constructor initializes the Electrical Signal Monitoring View Model's properties and commands
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public ElectricalSignalMonitoringViewModel()
		{
			localCommonViewModel.PropertyChanged += Current_PropertyChanged;
      WriteToMicroControllerCommand = new DelegateCommand<object>(OnWriteToMicroControllerCommand).ObservesCanExecute(()=>IsCommandEnabled);
			ReadFromMicroControllerCommand = new DelegateCommand<object>(OnReadFromMicroControllerCommand).ObservesCanExecute(()=> IsCommandEnabled);
			SetGPIOLevelCommand = new DelegateCommand<object>(OnSetGPIOLevelCommand).ObservesCanExecute(() => IsCommandEnabled);
			SetSVLevelCommand = new DelegateCommand<object>(OnSetSVLevelCommand).ObservesCanExecute(() => IsCommandEnabled);
			CloseCommand = new DelegateCommand<object>(OnCloseCommand).ObservesCanExecute(() => IsCommandEnabled);
			LogoutCommand = new DelegateCommand<object>(OnLogoutCommand).ObservesCanExecute(() => IsCommandEnabled);
			MaintenanceCommand = new DelegateCommand<object>(OnMaintenanceCommand).ObservesCanExecute(() => IsCommandEnabled);
			WriteConnectionBoxDiaphragmMinMaxCommand = new DelegateCommand<object>(OnWriteConnectionBoxDiaphragmMinMaxCommand).ObservesCanExecute(() => IsCommandEnabled);
			ConnectCommand = new DelegateCommand<object>(OnConnectCommand).ObservesCanExecute(() => IsCommandEnabled);
			StartCommand = new DelegateCommand<object>(OnStartCommand).ObservesCanExecute(() => IsCommandEnabled);
			StopCommand = new DelegateCommand<object>(OnStopCommand).ObservesCanExecute(() => IsCommandEnabled);
			LockTheFootSwitchCommand = new DelegateCommand<object>(OnLockTheFootSwitchCommand).ObservesCanExecute(() => IsCommandEnabled);
			DeflateAfterThawCommand = new DelegateCommand<object>(OnDeflateAfterThawCommand).ObservesCanExecute(() => IsCommandEnabled);
			ProgramCatheterCommand = new DelegateCommand<object>(OnProgramCatheterCommand);
			FastButtonCommand = new DelegateCommand<object>(OnFastButtonCommand, (obj) => true);
			SlowButtonCommand = new DelegateCommand<object>(OnSlowButtonCommand, (obj) => true);

			SvLevelPrevious = 0;
			ConnectionBoxDiaphragmMinimumValue = CommonViewModel.Current.ConnectionBox.DiaphragmeMinimumValue;
			ConnectionBoxDiaphragmMaximumValue = CommonViewModel.Current.ConnectionBox.DiaphragmeMaximumValue;
			DMSDetectionThreshold = CommonViewModel.Current.DMSDetectionThreshold;
		}

		private double _connectionBoxMin;

		/// <summary>
		/// This property gets/sets the Connection Box Diaphragm Min value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double ConnectionBoxDiaphragmMinimumValue
		{
			get => _connectionBoxMin;
			set => SetProperty(ref _connectionBoxMin, value);
		}

		public double DMSDetectionThreshold { get; set; }

		private double _connectionBoxDiaphragmMaximumValue;

    public Enumeration.InflationSpeedMode InflationSpeedMode => CommonViewModel.Current.Console.EnableFastInflationMode
                                                                ? Enumeration.InflationSpeedMode.Fast
                                                                : Enumeration.InflationSpeedMode.Slow;

    public void RefreshInflationSpeedMode()
    {
      RaisePropertyChanged(nameof(InflationSpeedMode));
    }

    /// <summary>
    /// This property gets/sets the Connection Box Diaphragm Max value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double ConnectionBoxDiaphragmMaximumValue
		{
			get => _connectionBoxDiaphragmMaximumValue;
			set => SetProperty(ref _connectionBoxDiaphragmMaximumValue, value);
		}

		/// <summary>
		/// This property gets/sets the Ablation Time value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public int AblationTime
		{
			get
			{
				return localCommonViewModel.AblationTime;
			}

			set
			{
				localCommonViewModel.AblationTime = value;
				RaisePropertyChanged("AblationTime");
			}
		}

		/// <summary>
		/// This property gets/sets the Deflate After Thaw value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool DeflateAfterThaw
		{
			get
			{
				return CommonViewModel.Current.DeflateAfterThaw;
			}

			set
			{
				CommonViewModel.Current.DeflateAfterThaw = value;
				RaisePropertyChanged("DeflateAfterThaw");
			}
		}

		/// <summary>
		/// Function/Command that handles deflate after thaw when the Deflate After Thaw
		/// command is invoked
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="arg">The command's parameter (not used in this function).</param>
		private void OnDeflateAfterThawCommand(object arg)
		{
			IsCommandEnabled = false;
			DeflateAfterThaw = !DeflateAfterThaw;
			IsCommandEnabled = true;
		}

		/// <summary>
		/// This property gets/sets the Central Micro Controller Firmware Version value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public int CentralMicroControllerFirmwareVersion
		{
			get
			{
				return localCommonViewModel.CentralMicroControllerFirmwareVersion;
			}

			set
			{
				localCommonViewModel.CentralMicroControllerFirmwareVersion = value;
				RaisePropertyChanged("CentralMicroControllerFirmwareVersion");
			}
		}

		/// <summary>
		/// This property gets/sets the Continuous Thawing value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double ContinuousThawing
		{
			get
			{
				return localCommonViewModel.ContinuousThawing;
			}

			set
			{
				localCommonViewModel.ContinuousThawing = value;
				RaisePropertyChanged("ContinuousThawing");
			}
		}

		/// <summary>
		/// This property gets/sets the CP1 Reading value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double CP1Reading
		{
			get
			{
				return localCommonViewModel.CP1Reading;
			}

			set
			{
				localCommonViewModel.CP1Reading = value;
				RaisePropertyChanged("CP1Reading");
			}
		}

		/// <summary>
		/// This property gets/sets the CP2 Reading value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double CP2Reading
		{
			get
			{
				return localCommonViewModel.CP2Reading;
			}

			set
			{
				localCommonViewModel.CP2Reading = value;
				RaisePropertyChanged("CP2Reading");
			}
		}

		/// <summary>
		/// This property gets/sets the TIP Reading value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double TIPReading
		{
			get
			{
				return localCommonViewModel.TIPReading;
			}

			set
			{
				localCommonViewModel.TIPReading = value;
				RaisePropertyChanged("TIPReading");
			}
		}

		/// <summary>
		/// This property gets/sets the CPLD Error Register value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public int CPLDErrorRegister
		{
			get
			{
				return localCommonViewModel.CPLDErrorRegister;
			}

			set
			{
				localCommonViewModel.CPLDErrorRegister = value;
				RaisePropertyChanged("CPLDErrorRegister");
			}
		}

		/// <summary>
		/// This property gets/sets the CPLD System Register value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public int CPLDSystemRegister
		{
			get
			{
				return localCommonViewModel.CPLDSystemRegister;
			}

			set
			{
				localCommonViewModel.CPLDSystemRegister = value;
				RaisePropertyChanged("CPLDSystemRegister");
			}
		}

		/// <summary>
		/// This property gets/sets the CPLD Valve Register value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public int CPLDValveRegister
		{
			get
			{
				return localCommonViewModel.CPLDValveRegister;
			}

			set
			{
				localCommonViewModel.CPLDValveRegister = value;
				RaisePropertyChanged("CPLDValveRegister");
			}
		}

		/// <summary>
		/// This property gets/sets the D Gain value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double DGain
		{
			get
			{
				localCommonViewModel.Console.CentralMicroControllerPIDValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].DGain = localCommonViewModel.DGain;
				return localCommonViewModel.DGain;
			}

			set
			{
				localCommonViewModel.DGain = value;
				localCommonViewModel.Console.CentralMicroControllerPIDValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].DGain = value;
				RaisePropertyChanged("DGain");
			}
		}

		/// <summary>
		/// This property gets/sets the FM1 High Range value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double FM1HighRange
		{
			get
			{
				localCommonViewModel.Console.FlowMeterOneValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].FlowMeterHighRangelimit = localCommonViewModel.FM1HighRange;
				return localCommonViewModel.FM1HighRange;
			}

			set
			{
				localCommonViewModel.FM1HighRange = value;
				localCommonViewModel.Console.FlowMeterOneValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].FlowMeterHighRangelimit = value;
				RaisePropertyChanged("FM1HighRange");
			}
		}

		/// <summary>
		/// This property gets/sets the FM1 Low Range value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double FM1LowRange
		{
			get
			{
				localCommonViewModel.Console.FlowMeterOneValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].FlowMeterLowRangeLimit = localCommonViewModel.FM1LowRange;
				return localCommonViewModel.FM1LowRange;
			}

			set
			{
				localCommonViewModel.FM1LowRange = value;
				localCommonViewModel.Console.FlowMeterOneValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].FlowMeterLowRangeLimit = value;
				RaisePropertyChanged("FM1LowRange");
			}
		}

		/// <summary>
		/// This property gets/sets the FM1 Reading value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double FM1Reading
		{
			get
			{
				return localCommonViewModel.FM1Reading;
			}

			set
			{
				localCommonViewModel.FM1Reading = value;
				RaisePropertyChanged("FM1Reading");
			}
		}

		/// <summary>
		/// This property gets/sets the PID Duty Cycle value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double PIDDutyCycle
		{
			get
			{
				return localCommonViewModel.PIDDutyCycle;
			}

			set
			{
				localCommonViewModel.PIDDutyCycle = value;
				RaisePropertyChanged("PIDDutyCycle");
			}
		}

		/// <summary>
		/// This property gets/sets the Patient PID Duty Cycle value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double PatientPIDDutyCycle
		{
			get
			{
				return localCommonViewModel.PatientPIDDutyCycle;
			}

			set
			{
				localCommonViewModel.PatientPIDDutyCycle = value;
				RaisePropertyChanged("PatientPIDDutyCycle");
			}
		}

		/// <summary>
		/// This property gets/sets the I Gain value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double IGain
		{
			get
			{
				localCommonViewModel.Console.CentralMicroControllerPIDValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].IGain = localCommonViewModel.IGain;
				return localCommonViewModel.IGain;
			}

			set
			{
				localCommonViewModel.IGain = value;
				localCommonViewModel.Console.CentralMicroControllerPIDValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].IGain = value;
				RaisePropertyChanged("IGain");
			}
		}

		/// <summary>
		/// This property gets/sets the LC1 High Range value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double LC1HighRange
		{
			get
			{
				localCommonViewModel.Console.LoadCellOneValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].LoadCellHighRangeLimit = localCommonViewModel.LC1HighRange;
				return localCommonViewModel.LC1HighRange;
			}

			set
			{
				localCommonViewModel.LC1HighRange = value;
				localCommonViewModel.Console.LoadCellOneValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].LoadCellHighRangeLimit = value;
				RaisePropertyChanged("LC1HighRange");
			}
		}

		/// <summary>
		/// This property gets/sets the LC1 Low Range value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double LC1LowRange
		{
			get
			{
				localCommonViewModel.Console.LoadCellOneValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].LoadCellLowRangeLimit = localCommonViewModel.LC1LowRange;
				return localCommonViewModel.LC1LowRange;
			}

			set
			{
				localCommonViewModel.LC1LowRange = value;
				localCommonViewModel.Console.LoadCellOneValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].LoadCellLowRangeLimit = value;
				RaisePropertyChanged("LC1LowRange");
			}
		}

		/// <summary>
		/// This property gets/sets the LC1 Reading value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double LC1Reading
		{
			get
			{
				return localCommonViewModel.LC1Reading;
			}

			set
			{
				localCommonViewModel.LC1Reading = value;
				RaisePropertyChanged("LC1Reading");
			}
		}

		/// <summary>
		/// This property gets/sets the P Gain value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double PGain
		{
			get
			{
				localCommonViewModel.Console.CentralMicroControllerPIDValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].PGain = localCommonViewModel.PGain;
				return localCommonViewModel.PGain;
			}

			set
			{
				localCommonViewModel.PGain = value;
				localCommonViewModel.Console.CentralMicroControllerPIDValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].PGain = value;
				RaisePropertyChanged("PGain");
			}
		}

		/// <summary>
		/// This property gets/sets the PID Offset value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double PIDOffset
		{
			get
			{
				localCommonViewModel.Console.CentralMicroControllerPIDValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].Offset = localCommonViewModel.PIDOffset;
				return localCommonViewModel.PIDOffset;
			}

			set
			{
				localCommonViewModel.PIDOffset = value;
				localCommonViewModel.Console.CentralMicroControllerPIDValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].Offset = value;
				RaisePropertyChanged("PIDOffset");
			}
		}

		/// <summary>
		/// This property gets/sets the PS1 High Range value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double PS1HighRange
		{
			get
			{
				localCommonViewModel.Console.PressureSwitchOneValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].PressureHighRangeLimit = localCommonViewModel.PS1HighRange;
				return localCommonViewModel.PS1HighRange;
			}

			set
			{
				localCommonViewModel.PS1HighRange = value;
				localCommonViewModel.Console.PressureSwitchOneValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].PressureHighRangeLimit = value;
				RaisePropertyChanged("PS1HighRange");
			}
		}

		/// <summary>
		/// This property gets/sets the PS1 Low Range value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double PS1LowRange
		{
			get
			{
				localCommonViewModel.Console.PressureSwitchOneValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].PressureLowRangeLimit = localCommonViewModel.PS1LowRange;
				return localCommonViewModel.PS1LowRange;
			}

			set
			{
				localCommonViewModel.PS1LowRange = value;
				localCommonViewModel.Console.PressureSwitchOneValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].PressureLowRangeLimit = value;
				RaisePropertyChanged("PS1LowRange");
			}
		}

		/// <summary>
		/// This property gets/sets the PS1 Reading value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double PS1Reading
		{
			get
			{
				return localCommonViewModel.PS1Reading;
			}

			set
			{
				localCommonViewModel.PS1Reading = value;
				RaisePropertyChanged("PS1Reading");
			}
		}

		/// <summary>
		/// This property gets/sets the PS2 High Range value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double PS2HighRange
		{
			get
			{
				localCommonViewModel.Console.PressureSwitchTwoValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].PressureHighRangeLimit = localCommonViewModel.PS2HighRange;
				return localCommonViewModel.PS2HighRange;
			}

			set
			{
				localCommonViewModel.PS2HighRange = value;
				localCommonViewModel.Console.PressureSwitchTwoValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].PressureHighRangeLimit = value;
				RaisePropertyChanged("PS2HighRange");
			}
		}

		/// <summary>
		/// This property gets/sets the PS2 Low Range value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double PS2LowRange
		{
			get
			{
				localCommonViewModel.Console.PressureSwitchTwoValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].PressureLowRangeLimit = localCommonViewModel.PS2LowRange;
				return localCommonViewModel.PS2LowRange;
			}

			set
			{
				localCommonViewModel.PS2LowRange = value;
				localCommonViewModel.Console.PressureSwitchTwoValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].PressureLowRangeLimit = value;
				RaisePropertyChanged("PS2LowRange");
			}
		}

		/// <summary>
		/// This property gets/sets the PS2 Reading value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double PS2Reading
		{
			get
			{
				return localCommonViewModel.PS2Reading;
			}

			set
			{
				localCommonViewModel.PS2Reading = value;
				RaisePropertyChanged("PS2Reading");
			}
		}

		/// <summary>
		/// This property gets/sets the PT1 High Range value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double PT1HighRange
		{
			get
			{
				localCommonViewModel.Console.PressureTransducerOneValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].PressureHighRangeLimit = localCommonViewModel.PT1HighRange;
				return localCommonViewModel.PT1HighRange;
			}

			set
			{
				localCommonViewModel.PT1HighRange = value;
				localCommonViewModel.Console.PressureTransducerOneValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].PressureHighRangeLimit = value;
				RaisePropertyChanged("PT1HighRange");
			}
		}

		/// <summary>
		/// This property gets/sets the PT1 Low Range value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double PT1LowRange
		{
			get
			{
				localCommonViewModel.Console.PressureTransducerOneValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].PressureLowRangeLimit = localCommonViewModel.PT1LowRange;
				return localCommonViewModel.PT1LowRange;
			}

			set
			{
				localCommonViewModel.PT1LowRange = value;
				localCommonViewModel.Console.PressureTransducerOneValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].PressureLowRangeLimit = value;
				RaisePropertyChanged("PT1LowRange");
			}
		}

		/// <summary>
		/// This property gets/sets the PT1 Reading value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double PT1Reading
		{
			get
			{
				return localCommonViewModel.PT1Reading;
			}

			set
			{
				localCommonViewModel.PT1Reading = value;
				RaisePropertyChanged("PT1Reading");
			}
		}

		/// <summary>
		/// This property gets/sets the PT2 High Range value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double PT2HighRange
		{
			get
			{
				localCommonViewModel.Console.PressureTransducerTwoValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].PressureHighRangeLimit = localCommonViewModel.PT2HighRange;
				return localCommonViewModel.PT2HighRange;
			}

			set
			{
				localCommonViewModel.PT2HighRange = value;
				localCommonViewModel.Console.PressureTransducerTwoValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].PressureHighRangeLimit = value;
				RaisePropertyChanged("PT2HighRange");
			}
		}

		/// <summary>
		/// This property gets/sets the PT2 Low Range value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double PT2LowRange
		{
			get
			{
				localCommonViewModel.Console.PressureTransducerTwoValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].PressureLowRangeLimit = localCommonViewModel.PT2LowRange;
				return localCommonViewModel.PT2LowRange;
			}

			set
			{
				localCommonViewModel.PT2LowRange = value;
				localCommonViewModel.Console.PressureTransducerTwoValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].PressureLowRangeLimit = value;
				RaisePropertyChanged("PT2LowRange");
			}
		}

		/// <summary>
		/// This property gets/sets the PT2 Reading value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double PT2Reading
		{
			get
			{
				return localCommonViewModel.PT2Reading;
			}

			set
			{
				localCommonViewModel.PT2Reading = value;
				RaisePropertyChanged("PT2Reading");
			}
		}

		/// <summary>
		/// This property gets/sets the PT3 High Range value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double PT3HighRange
		{
			get
			{
				localCommonViewModel.Console.PressureTransducerThreeValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].PressureHighRangeLimit = localCommonViewModel.PT3HighRange;
				return localCommonViewModel.PT3HighRange;
			}

			set
			{
				localCommonViewModel.PT3HighRange = value;
				localCommonViewModel.Console.PressureTransducerThreeValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].PressureHighRangeLimit = value;
				RaisePropertyChanged("PT3HighRange");
			}
		}

		/// <summary>
		/// This property gets/sets the PT3 Low Range value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double PT3LowRange
		{
			get
			{
				localCommonViewModel.Console.PressureTransducerThreeValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].PressureLowRangeLimit = localCommonViewModel.PT3LowRange;
				return localCommonViewModel.PT3LowRange;
			}

			set
			{
				localCommonViewModel.PT3LowRange = value;
				localCommonViewModel.Console.PressureTransducerThreeValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].PressureLowRangeLimit = value;
				RaisePropertyChanged("PT3LowRange");
			}
		}

		/// <summary>
		/// This property gets/sets the PT3 Reading value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double PT3Reading
		{
			get
			{
				return localCommonViewModel.PT3Reading;
			}

			set
			{
				localCommonViewModel.PT3Reading = value;
				RaisePropertyChanged("PT3Reading");
			}
		}

		/// <summary>
		/// This property gets/sets the PT4 High Range value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double PT4HighRange
		{
			get
			{
				localCommonViewModel.Console.PressureTransducerFourValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].PressureHighRangeLimit = localCommonViewModel.PT4HighRange;
				return localCommonViewModel.PT4HighRange;
			}

			set
			{
				localCommonViewModel.PT4HighRange = value;
				localCommonViewModel.Console.PressureTransducerFourValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].PressureHighRangeLimit = value;
				RaisePropertyChanged("PT4HighRange");
			}
		}

		/// <summary>
		/// This property gets/sets the PT4 Low Range value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double PT4LowRange
		{
			get
			{
				localCommonViewModel.Console.PressureTransducerFourValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].PressureLowRangeLimit = localCommonViewModel.PT4LowRange;
				return localCommonViewModel.PT4LowRange;
			}

			set
			{
				localCommonViewModel.PT4LowRange = value;
				localCommonViewModel.Console.PressureTransducerFourValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].PressureLowRangeLimit = value;
				RaisePropertyChanged("PT4LowRange");
			}
		}

		/// <summary>
		/// This property gets/sets the PT4 Reading value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double PT4Reading
		{
			get
			{
				return localCommonViewModel.PT4Reading;
			}

			set
			{
				RaisePropertyChanged("PT4Reading");
			}
		}

		/// <summary>
		/// This property gets/sets the PT5 Reading value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double PT5Reading
		{
			get
			{
				return localCommonViewModel.PT5Reading;
			}

			set
			{
				RaisePropertyChanged("PT5Reading");
			}
		}

		/// <summary>
		/// This property gets/sets the System State value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public int SystemState
		{
			get
			{
				if (CommonViewModel.Current.SystemState == MessageStateId.CAN_ID_STATE_READY)
				{
					IsCatheterConnectedAndInIReadyState = true;
				}
				else
				{
					IsCatheterConnectedAndInIReadyState = false;
				}

				return (int)localCommonViewModel.SystemState;
			}

			set
			{
				RaisePropertyChanged("SystemState");
			}
		}

		/// <summary>
		/// This property gets/sets the CMCU System Status Error value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public Int64 CMCUSystemStatusError
		{
			get
			{
				return localCommonViewModel.CMCUSystemStatusError;
			}

			set
			{
				RaisePropertyChanged("CMCUSystemStatusError");
			}
		}

		/// <summary>
		/// This property gets/sets the Target Injection Flow value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double TargetInjectionFlow
		{
			get
			{
				localCommonViewModel.Console.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].TargetInjectionFlow = localCommonViewModel.TargetInjectionFlow;
				return localCommonViewModel.TargetInjectionFlow;
			}

			set
			{
				localCommonViewModel.TargetInjectionFlow = value;
				localCommonViewModel.Console.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].TargetInjectionFlow = value;
				RaisePropertyChanged("TargetInjectionFlow");
			}
		}

		/// <summary>
		/// This property gets/sets the Target Injection Pressure value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double TargetInjectionPressure
		{
			get
			{
				localCommonViewModel.Console.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].TargetInjectionPressure = localCommonViewModel.TargetInjectionPressure;
				return localCommonViewModel.TargetInjectionPressure;
			}

			set
			{
				localCommonViewModel.TargetInjectionPressure = value;
				localCommonViewModel.Console.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].TargetInjectionPressure = value;
				RaisePropertyChanged("TargetInjectionPressure");
			}
		}

		/// <summary>
		/// This property gets/sets the TC1 Reading value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double TC1Reading
		{
			get
			{
				return localCommonViewModel.TC1Reading;
			}

			set
			{
				localCommonViewModel.TC1Reading = value;
				RaisePropertyChanged("TC1Reading");
			}
		}

		/// <summary>
		/// This property gets/sets the TC2 Reading value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double TC2Reading
		{
			get
			{
				return localCommonViewModel.TC2Reading;
			}

			set
			{
				localCommonViewModel.TC2Reading = value;
				RaisePropertyChanged("TC2Reading");
			}
		}

		/// <summary>
		/// This property gets/sets the PMCU CJ Reading value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double PMCUCJReading
		{
			get
			{
				return localCommonViewModel.PMCUCJReading;
			}

			set
			{
				localCommonViewModel.PMCUCJReading = value;
				RaisePropertyChanged("PMCUCJReading");
			}
		}

		public int BloodDetecorImValue
		{
			get
			{
				return localCommonViewModel.BloodDetecorImValue;
			}

			set
			{
				localCommonViewModel.BloodDetecorImValue = value;
				RaisePropertyChanged("BloodDetecorImValue");
			}
		}

		/// <summary>
		/// This property gets/sets the Threshold FM1 High value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double ThresholdFM1High
		{
			get
			{
				localCommonViewModel.Console.FlowMeterOneValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].FlowMeterThresholHighlimit = localCommonViewModel.ThresholdFM1High;
				return localCommonViewModel.ThresholdFM1High;
			}

			set
			{
				localCommonViewModel.ThresholdFM1High = value;
				localCommonViewModel.Console.FlowMeterOneValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].FlowMeterThresholHighlimit = value;
				RaisePropertyChanged("ThresholdFM1High");
			}
		}

		/// <summary>
		/// This property gets/sets the Threshold FM1 Low value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double ThresholdFM1Low
		{
			get
			{
				localCommonViewModel.Console.FlowMeterOneValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].FlowMeterThresholLowlimit = localCommonViewModel.ThresholdFM1Low;
				return localCommonViewModel.ThresholdFM1Low;
			}

			set
			{
				localCommonViewModel.ThresholdFM1Low = value;
				localCommonViewModel.Console.FlowMeterOneValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].FlowMeterThresholLowlimit = value;
				RaisePropertyChanged("ThresholdFM1Low");
			}
		}

		/// <summary>
		/// This property gets/sets the Threshold For PT1 Fail value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double ThresholdForPT1Fail
		{
			get
			{
				localCommonViewModel.Console.PressureTransducerOneValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].TankPressureTooHigh = localCommonViewModel.ThresholdForPT1Fail;
				return localCommonViewModel.ThresholdForPT1Fail;
			}

			set
			{
				localCommonViewModel.ThresholdForPT1Fail = value;
				localCommonViewModel.Console.PressureTransducerOneValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].TankPressureTooHigh = value;
				RaisePropertyChanged("ThresholdForPT1Fail");
			}
		}

		/// <summary>
		/// This property gets/sets the Threshold For PT1 High value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double ThresholdForPT1High
		{
			get
			{
				localCommonViewModel.Console.PressureTransducerOneValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].PressureThresholdHighLimit = localCommonViewModel.ThresholdForPT1High;
				return localCommonViewModel.ThresholdForPT1High;
			}

			set
			{
				localCommonViewModel.ThresholdForPT1High = value;
				localCommonViewModel.Console.PressureTransducerOneValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].PressureThresholdHighLimit = Convert.ToInt32(value);
				RaisePropertyChanged("ThresholdForPT1High");
			}
		}

		/// <summary>
		/// This property gets/sets the Threshold For PT1 Low value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double ThresholdForPT1Low
		{
			get
			{
				localCommonViewModel.Console.PressureTransducerOneValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].TankPressureLow = localCommonViewModel.ThresholdForPT1Low;
				return localCommonViewModel.ThresholdForPT1Low;
			}

			set
			{
				localCommonViewModel.ThresholdForPT1Low = value;
				localCommonViewModel.Console.PressureTransducerOneValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].TankPressureLow = value;
				RaisePropertyChanged("ThresholdForPT1Low");
			}
		}

		/// <summary>
		/// This property gets/sets the Threshold LC1 Fail value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double ThresholdLC1Fail
		{
			get
			{
				localCommonViewModel.Console.LoadCellOneValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].LoadCellThresholdFail = localCommonViewModel.ThresholdLC1Fail;
				return localCommonViewModel.ThresholdLC1Fail - localCommonViewModel.Console.Tank.MetalWeight;
			}

			set
			{
				localCommonViewModel.ThresholdLC1Fail = value + localCommonViewModel.Console.Tank.MetalWeight;
				localCommonViewModel.Console.LoadCellOneValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].LoadCellThresholdFail = value;
				RaisePropertyChanged("ThresholdLC1Fail");
			}
		}

		/// <summary>
		/// This property gets/sets the Threshold LC1 Warning value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double ThresholdLC1Warning
		{
			get
			{
				localCommonViewModel.Console.LoadCellOneValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].LoadCellThresholdWarning = localCommonViewModel.ThresholdLC1Warning;
				return localCommonViewModel.ThresholdLC1Warning - localCommonViewModel.Console.Tank.MetalWeight;
			}

			set
			{
				localCommonViewModel.ThresholdLC1Warning = value + localCommonViewModel.Console.Tank.MetalWeight;
				localCommonViewModel.Console.LoadCellOneValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].LoadCellThresholdWarning = value;
				RaisePropertyChanged("ThresholdLC1Warning");
			}
		}

		/// <summary>
		/// This property gets/sets the Threshold PS1 High value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double ThresholdPS1High
		{
			get
			{
				localCommonViewModel.Console.PressureSwitchOneValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].PressureThresholdHighLimit = localCommonViewModel.ThresholdPS1High;
				return localCommonViewModel.ThresholdPS1High;
			}

			set
			{
				localCommonViewModel.ThresholdPS1High = value;
				localCommonViewModel.Console.PressureSwitchOneValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].PressureThresholdHighLimit = value;
				RaisePropertyChanged("ThresholdPS1High");
			}
		}

		/// <summary>
		/// This property gets/sets the Threshold PS2 High value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double ThresholdPS2High
		{
			get
			{
				localCommonViewModel.Console.PressureSwitchTwoValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].PressureThresholdHighLimit = localCommonViewModel.ThresholdPS2High;
				return localCommonViewModel.ThresholdPS2High;
			}

			set
			{
				localCommonViewModel.ThresholdPS2High = value;
				localCommonViewModel.Console.PressureSwitchTwoValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].PressureThresholdHighLimit = value;
				RaisePropertyChanged("ThresholdPS2High");
			}
		}

		/// <summary>
		/// This property gets/sets the Threshold PT2 High value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double ThresholdPT2High
		{
			get
			{
				localCommonViewModel.Console.PressureTransducerTwoValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].PressureThresholdHighLimit = localCommonViewModel.ThresholdPT2High;
				return localCommonViewModel.ThresholdPT2High;
			}

			set
			{
				localCommonViewModel.ThresholdPT2High = value;
				localCommonViewModel.Console.PressureTransducerTwoValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].PressureThresholdHighLimit = value;
				RaisePropertyChanged("ThresholdPT2High");
			}
		}

		/// <summary>
		/// This property gets/sets the Threshold PT3 High value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double ThresholdPT3High
		{
			get
			{
				localCommonViewModel.Console.PressureTransducerThreeValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].PressureThresholdHighLimit = localCommonViewModel.ThresholdPT3High;
				return localCommonViewModel.ThresholdPT3High;
			}

			set
			{
				localCommonViewModel.ThresholdPT3High = value;
				localCommonViewModel.Console.PressureTransducerThreeValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].PressureThresholdHighLimit = value;
				RaisePropertyChanged("ThresholdPT3High");
			}
		}

		/// <summary>
		/// This property gets/sets the Threshold PT4 High value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double ThresholdPT4high
		{
			get
			{
				localCommonViewModel.Console.PressureTransducerFourValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].PressureThresholdHighLimit = localCommonViewModel.ThresholdPT4high;
				return localCommonViewModel.ThresholdPT4high;
			}

			set
			{
				localCommonViewModel.ThresholdPT4high = value;
				localCommonViewModel.Console.PressureTransducerFourValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].PressureThresholdHighLimit = value;
				RaisePropertyChanged("ThresholdPT4high");
			}
		}

		/// <summary>
		/// This property gets/sets the Threshold TS1 High value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double ThresholdTS1High
		{
			get
			{
				localCommonViewModel.Console.TemperatureSensorOneValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].TemperatureThresholdHighLimit = localCommonViewModel.ThresholdTS1High;
				return localCommonViewModel.ThresholdTS1High;
			}

			set
			{
				localCommonViewModel.ThresholdTS1High = value;
				localCommonViewModel.Console.TemperatureSensorOneValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].TemperatureThresholdHighLimit = value;
				RaisePropertyChanged("ThresholdTS1High");
			}
		}

		/// <summary>
		/// This property gets/sets the TS1 High Range value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double TS1HighRange
		{
			get
			{
				localCommonViewModel.Console.TemperatureSensorOneValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].TemperatureHighRangeLimit = localCommonViewModel.TS1HighRange;
				return localCommonViewModel.TS1HighRange;
			}

			set
			{
				localCommonViewModel.TS1HighRange = value;
				localCommonViewModel.Console.TemperatureSensorOneValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].TemperatureHighRangeLimit = value;
				RaisePropertyChanged("TS1HighRange");
			}
		}

		/// <summary>
		/// This property gets/sets the TS1 Low Range value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double TS1LowRange
		{
			get
			{
				localCommonViewModel.Console.TemperatureSensorOneValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].TemperatureLowRangeLimit = localCommonViewModel.TS1LowRange;
				return localCommonViewModel.TS1LowRange;
			}

			set
			{
				localCommonViewModel.TS1LowRange = value;
				localCommonViewModel.Console.TemperatureSensorOneValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].TemperatureLowRangeLimit = value;
				RaisePropertyChanged("TS1LowRange");
			}
		}

		/// <summary>
		/// This property gets/sets the TS1 Reading value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double TS1Reading
		{
			get
			{
				return localCommonViewModel.TS1Reading;
			}

			set
			{
				localCommonViewModel.TS1Reading = value;
				RaisePropertyChanged("TS1Reading");
			}
		}

		/// <summary>
		/// This property gets/sets the TN2O Reading value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double TN2OReading
		{
			get
			{
				return localCommonViewModel.TN2OReading;
			}

			set
			{
				localCommonViewModel.TN2OReading = value;
				RaisePropertyChanged("TN2OReading");
			}
		}

		/// <summary>
		/// This property gets/sets the CMCU CJ Reading value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double CMCUCJReading
		{
			get
			{
				return localCommonViewModel.CMCUCJReading;
			}

			set
			{
				localCommonViewModel.CMCUCJReading = value;
				RaisePropertyChanged("CMCUCJReading");
			}
		}

		/// <summary>
		/// This property gets/sets Patient Micro Controller Firmware Version value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public int PatientMicroControllerFirmwareVersion
		{
			get
			{
				return localCommonViewModel.PatientMicroControllerFirmwareVersion;
			}

			set
			{
				localCommonViewModel.PatientMicroControllerFirmwareVersion = value;
				RaisePropertyChanged("PatientMicroControllerFirmwareVersion");
			}
		}

		/// <summary>
		/// This property gets/sets the PMCU System Status Error Code value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public Int64 PMCUSystemStatusErrorCode
		{
			get
			{
				return localCommonViewModel.PMCUSystemStatusErrorCode;
			}

			set
			{
				localCommonViewModel.PMCUSystemStatusErrorCode = value;
				RaisePropertyChanged("PMCUSystemStatusErrorCode");
			}
		}

		/// <summary>
		/// This property gets/sets the Catherer Firmware Version value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public int CatheterFirmwareVersion
		{
			get
			{
				return localCommonViewModel.CatheterFirmwareVersion;
			}

			set
			{
				localCommonViewModel.CatheterFirmwareVersion = value;
				RaisePropertyChanged("CatheterFirmwareVersion");
			}
		}

		/// <summary>
		/// This property gets/sets the Catheter Id value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public int CatheterID
		{
			get
			{
				return localCommonViewModel.CatheterID;
			}

			set
			{
				localCommonViewModel.CatheterID = value;
				RaisePropertyChanged("CatheterID");
			}
		}

		/// <summary>
		/// This property gets/sets the Catheter Serial Number value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public int CatheterSerialNumber
		{
			get
			{
				return localCommonViewModel.CatheterSerialNumber;
			}

			set
			{
				localCommonViewModel.CatheterSerialNumber = value;
				RaisePropertyChanged("CatheterSerialNumber");
			}
		}

    /// <summary>
    /// This property gets/sets the Catheter Container value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public string CatheterContainer
    {
      get => localCommonViewModel.CatheterContainerTag;
      set 
      {
        localCommonViewModel.CatheterContainerTag = value;
        RaisePropertyChanged();
      }
    }

    /// <summary>
    /// This property gets/sets the Catheter Container value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int CatheterLotNumber
    {
      get => localCommonViewModel.CatheterLot;
      set 
      {
        localCommonViewModel.CatheterLot = value;
        RaisePropertyChanged();
      }
    }

		/// <summary>
		/// This property gets/sets the Catheter Expiration Date value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public DateTime CatheterExpirationDate
		{
			get
			{
				return localCommonViewModel.CatheterExpirationDate;
			}

			set
			{
				localCommonViewModel.CatheterExpirationDate = value;
				RaisePropertyChanged("CatheterExpirationDate");
			}
		}

		/// <summary>
		/// This property gets/sets the Catheter Last Use Date value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public DateTime CatheterLastUseDate
		{
			get
			{
				return localCommonViewModel.CatheterLastUseDate;
			}

			set
			{
				localCommonViewModel.CatheterLastUseDate = value;
				RaisePropertyChanged("CatheterLastUseDate");
			}
		}

		/// <summary>
		/// This property gets/sets the Number of Injections value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public int NumberOfInjections
		{
			get
			{
				return localCommonViewModel.NumberOfInjections;
			}

			set
			{
				localCommonViewModel.NumberOfInjections = value;
				RaisePropertyChanged("NumberOfInjections");
			}
		}

		/// <summary>
		/// This property gets/sets the Target Balloon Pressure value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double TargetBalloonPressure
		{
			get
			{
				localCommonViewModel.Console.PatientMicroControllerBalloonPressureRegulatorValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].TargetBalloonPressure = localCommonViewModel.TargetBalloonPressure;
				return localCommonViewModel.TargetBalloonPressure;
			}

			set
			{
				localCommonViewModel.TargetBalloonPressure = value;
				localCommonViewModel.Console.PatientMicroControllerBalloonPressureRegulatorValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].TargetBalloonPressure = value;
				RaisePropertyChanged("TargetBalloonPressure");
			}
		}

		/// <summary>
		/// This property gets/sets the PT2 Low Range value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double ThresholdForCP1High // here we shall rename to IBP
		{
			get
			{
				localCommonViewModel.Console.PatientPressureTransducerOneValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].PressureThresholdHighLimit = localCommonViewModel.ThresholdForCP1High;
				return localCommonViewModel.ThresholdForCP1High;
			}

			set
			{
				localCommonViewModel.ThresholdForCP1High = value;

				localCommonViewModel.Console.PatientPressureTransducerOneValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].PressureThresholdHighLimit = value;
				RaisePropertyChanged("ThresholdForCP1High");
			}
		}

		/// <summary>
		/// This property gets/sets the pressure low threshold for inner ballon
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double ThresholdForInnerBallonPressureLow
		{
			get
			{
				localCommonViewModel.Console.PatientPressureTransducerOneValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].PressureHighRangeLimit = localCommonViewModel.ThresholdForInnerBallonPressureLow;
				return localCommonViewModel.ThresholdForInnerBallonPressureLow;
			}

			set
			{
				localCommonViewModel.ThresholdForInnerBallonPressureLow = value;

				localCommonViewModel.Console.PatientPressureTransducerOneValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].PressureHighRangeLimit = value;
				RaisePropertyChanged("ThresholdForInnerBallonPressureLow");
			}
		}

		/// <summary>
		/// This property gets/sets the Threshold For Outer Balloon Pressure value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double ThresholdForOuterBallonPressure
		{
			get
			{
				localCommonViewModel.Console.PatientPressureTransducerOneValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].PressureLowRangeLimit = localCommonViewModel.ThresholdForOuterBallonPressure;
				return localCommonViewModel.ThresholdForOuterBallonPressure;
			}

			set
			{
				localCommonViewModel.ThresholdForOuterBallonPressure = value;
				localCommonViewModel.Console.PatientPressureTransducerOneValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].PressureLowRangeLimit = value;
				RaisePropertyChanged("ThresholdForOuterBallonPressure");
			}
		}

		/// <summary>
		/// This property gets/sets the Threshold For CTC1 High value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double ThresholdForCTC1High
		{
			get
			{
				return localCommonViewModel.ThresholdForCTC1High;
			}

			set
			{
				localCommonViewModel.ThresholdForCTC1High = value;
				localCommonViewModel.Console.ThermocoupleOneValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].ThawingTemperature = value;
				RaisePropertyChanged("ThresholdForCTC1High");
			}
		}

		public double ThawingTemperatureSetPoint
		{
			get
			{
				return localCommonViewModel.ThawingTemperatureSetPoint;
			}

			set
			{
				localCommonViewModel.ThawingTemperatureSetPoint = value;
				localCommonViewModel.Console.ThermocoupleOneValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].ThawingTemperatureSetPoint = value;
				RaisePropertyChanged("ThawingTemperatureSetPoint");
			}
		}

		/// <summary>
		/// Gets or sets the blood detection lower threshold
		/// . Safety classification: Death or SERIOUS INJURY is possible (IEC 62304 Class C).
		/// </summary>
		/// <Id>SF-SDS-0005</Id>
		public short LowerBloodThreshold
		{
			get
			{
				return localCommonViewModel.LowerBloodThreshold;
			}

			set
			{
				localCommonViewModel.LowerBloodThreshold = value;
				localCommonViewModel.Console.BloodDetectorValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].LowerBloodThreshold = value;
				RaisePropertyChanged("LowerBloodThreshold");
			}
		}

		/// <summary>
		/// Gets or sets the blood detection upper threshold
		/// . Safety classification: Death or SERIOUS INJURY is possible (IEC 62304 Class C).
		/// </summary>
		/// <id>SF-SDS-0001</id>
		public short UpperBloodThreshold
		{
			get
			{
				return localCommonViewModel.UpperBloodThreshold;
			}

			set
			{
				localCommonViewModel.UpperBloodThreshold = value;
				localCommonViewModel.Console.BloodDetectorValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].UpperBloodThreshold = value;
				RaisePropertyChanged("UpperBloodThreshold");
			}
		}

		/// <summary>
		/// This property gets/sets the Threshold For CTC2 High value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double ThresholdForCTC2High
		{
			get
			{
				return localCommonViewModel.ThresholdForCTC2High;
			}

			set
			{
				localCommonViewModel.ThresholdForCTC2High = value;
				//There is no TC2
				RaisePropertyChanged("ThresholdForCTC2High");
			}
		}

		/// <summary>
		/// This property gets/sets the Patient Gain value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double PatientPGain
		{
			get
			{
				localCommonViewModel.Console.PatientMicroControllerPIDValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].PGain = localCommonViewModel.PatientPGain;
				return localCommonViewModel.PatientPGain;
			}

			set
			{
				localCommonViewModel.PatientPGain = value;
				localCommonViewModel.Console.PatientMicroControllerPIDValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].PGain = value;
				RaisePropertyChanged("PatientPGain");
			}
		}

		/// <summary>
		/// This property gets/sets the Patient I Gain value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double PatientIGain
		{
			get
			{
				localCommonViewModel.Console.PatientMicroControllerPIDValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].IGain = localCommonViewModel.PatientIGain;
				return localCommonViewModel.PatientIGain;
			}

			set
			{
				localCommonViewModel.PatientIGain = value;
				localCommonViewModel.Console.PatientMicroControllerPIDValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].IGain = value;
				RaisePropertyChanged("PatientIGain");
			}
		}

		/// <summary>
		/// This property gets/sets the Patient D Gain value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double PatientDGain
		{
			get
			{
				localCommonViewModel.Console.PatientMicroControllerPIDValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].DGain = localCommonViewModel.PatientDGain;
				return localCommonViewModel.PatientDGain;
			}

			set
			{
				localCommonViewModel.PatientDGain = value;
				localCommonViewModel.Console.PatientMicroControllerPIDValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].DGain = value;
				RaisePropertyChanged("PatientDGain");
			}
		}

		/// <summary>
		/// This property gets/sets the Patient PID Offset value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public double PatientPIDOffset
		{
			get
			{
				localCommonViewModel.Console.PatientMicroControllerPIDValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].Offset = localCommonViewModel.PatientPIDOffset;
				return localCommonViewModel.PatientPIDOffset;
			}

			set
			{
				localCommonViewModel.PatientPIDOffset = value;
				localCommonViewModel.Console.PatientMicroControllerPIDValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].Offset = value;
				RaisePropertyChanged("PatientPIDOffset");
			}
		}

		/// <summary>
		/// This property gets/sets the GPIO 0 Activated value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsGPIO0Activated
		{
			get => isGPIO0Activated;
			set => SetProperty(ref isGPIO0Activated, value);
		}

		/// <summary>
		/// This property gets/sets the GPIO 1 Activated value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsGPIO1Activated
		{
			get => isGPIO1Activated;
			set => SetProperty(ref isGPIO1Activated, value);
		}

		/// <summary>
		/// This property gets/sets the GPIO 2 Activated value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsGPIO2Activated
		{
			get
			{
				return isGPIO2Activated;
			}

			set
			{
				SetProperty(ref isGPIO2Activated, value);

				if (value)
					CommonViewModel.Current.ResetCatheterInformation();
			}
		}

		/// <summary>
		/// This property gets/sets the GPIO 3 Activated value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsGPIO3Activated
		{
			get => isGPIO3Activated;
			set => SetProperty(ref isGPIO3Activated, value);
		}

		/// <summary>
		/// This property gets/sets the GPIO 4 Activated value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsGPIO4Activated
		{
			get => isGPIO4Activated;
			set => SetProperty(ref isGPIO4Activated, value);
		}

		/// <summary>
		/// This property gets/sets the GPIO 5 Activated value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsGPIO5Activated
		{
			get => isGPIO5Activated;
			set => SetProperty(ref isGPIO5Activated, value);
		}

		/// <summary>
		/// This property gets/sets the GPIO 6 Activated value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsGPIO6Activated
		{
			get => isGPIO6Activated;
			set => SetProperty(ref isGPIO6Activated, value);
		}

		/// <summary>
		/// This property gets/sets the GPIO 7 Activated value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsGPIO7Activated
		{
			get => isGPIO7Activated;
			set => SetProperty(ref isGPIO7Activated, value);
		}

		/// <summary>
		/// This property gets/sets the Level value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public uint Level
		{
			get
			{
				return level;
			}

			set
			{
				level = value;
			}
		}

		/// <summary>
		/// This function handles the sender's PropertyChanged event
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="sender">The View Model that sent the event.</param>
		/// <param name="e">The property changed arguments.</param>
		private void Current_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			CommonViewModel commonviewmodel = sender as CommonViewModel;

			switch (e.PropertyName)
			{
				case "TC1Reading":
					RaisePropertyChanged("TC1Reading");
					break;

				case "TC2Reading":
					RaisePropertyChanged("TC2Reading");
					break;

				case "TS1Reading":
					RaisePropertyChanged("TS1Reading");
					break;

				case "TN2OReading":
					RaisePropertyChanged("TN2OReading");
					break;

				case "PT1Reading":
					RaisePropertyChanged("PT1Reading");
					break;

				case "PT2Reading":
					RaisePropertyChanged("PT2Reading");
					break;

				case "PT3Reading":
					RaisePropertyChanged("PT3Reading");
					break;

				case "PT4Reading":
					RaisePropertyChanged("PT4Reading");
					break;

				case "PT5Reading":
					RaisePropertyChanged("PT5Reading");
					break;

				case "CP1Reading":
					RaisePropertyChanged("CP1Reading");
					break;

				case "CP2Reading":
					RaisePropertyChanged("CP2Reading");
					break;

				case "TIPReading":
					RaisePropertyChanged("TIPReading");
					break;

				case "PS1Reading":
					RaisePropertyChanged("PS1Reading");
					break;

				case "PS2Reading":
					RaisePropertyChanged("PS2Reading");
					break;

				case "FM1Reading":
					RaisePropertyChanged("FM1Reading");
					break;

				case "PIDDutyCycle":
					RaisePropertyChanged("PIDDutyCycle");
					break;

				case "PatientPIDDutyCycle":
					RaisePropertyChanged("PatientPIDDutyCycle");
					break;

				case "LC1Reading":
					RaisePropertyChanged("LC1Reading");
					break;

				case "PMCUCJReading":
					RaisePropertyChanged("PMCUCJReading");
					break;

				case "DeflateAfterThaw":
					RaisePropertyChanged("DeflateAfterThaw");
					break;

				#region Patient register

				case "PatientMicroControllerFirmwareVersion":
					RaisePropertyChanged("PatientMicroControllerFirmwareVersion");
					break;

				case "PMCUSystemStatusErrorCode":
					RaisePropertyChanged("PMCUSystemStatusErrorCode");
					break;

				case "CatheterFirmwareVersion":
					RaisePropertyChanged("CatheterFirmwareVersion");
					break;

				case "CatheterID":
					RaisePropertyChanged("CatheterID");
					break;

				case "CatheterSerialNumber":
					RaisePropertyChanged("CatheterSerialNumber");
					break;

        case "CatheterLot":
          RaisePropertyChanged(nameof(CatheterLotNumber));
          break;

        case "CatheterContainerTag":
          RaisePropertyChanged(nameof(CatheterContainer));
          break;

				case "CatheterExpirationDate":
					RaisePropertyChanged("CatheterExpirationDate");
					break;

				case "CatheterLastUseDate":
					RaisePropertyChanged("CatheterLastUseDate");
					break;

				case "NumberOfInjections":
					RaisePropertyChanged("NumberOfInjections");
					break;

				case "TargetBalloonPressure":
					RaisePropertyChanged("TargetBalloonPressure");
					break;

				case "ThresholdForOuterBallonPressure":  //it wase ThresholdForCP2High
					RaisePropertyChanged("ThresholdForOuterBallonPressure");
					break;

				case "ThresholdForCP1High": // that one is IBP High
					RaisePropertyChanged("ThresholdForCP1High");
					break;

				case "ThresholdForInnerBallonPressureLow": // that one is IBP Low
					RaisePropertyChanged("ThresholdForInnerBallonPressureLow");
					break;

				case "ThresholdForCTC1High":
					RaisePropertyChanged("ThresholdForCTC1High");
					break;

				case "ThawingTemperatureSetPoint":
					RaisePropertyChanged("ThawingTemperatureSetPoint");
					break;

				case "ThresholdForCTC2High":
					RaisePropertyChanged("ThresholdForCTC2High");
					break;

				case "PatientPGain":
					RaisePropertyChanged("PatientPGain");
					break;

				case "PatientIGain":
					RaisePropertyChanged("PatientIGain");
					break;

				case "PatientDGain":
					RaisePropertyChanged("PatientDGain");
					break;

				case "PatientPIDOffset":
					RaisePropertyChanged("PatientPIDOffset");
					break;

				#endregion Patient register

				#region Central Micro Controller: Register Values

				case "CentralMicroControllerFirmwareVersion":
					RaisePropertyChanged("CentralMicroControllerFirmwareVersion");
					break;

				case "CPLDErrorRegister":
					RaisePropertyChanged("CPLDErrorRegister");
					break;

				case "CPLDValveRegister":
					RaisePropertyChanged("CPLDValveRegister");
					break;

				case "CPLDSystemRegister":
					RaisePropertyChanged("CPLDSystemRegister");
					break;

				case "SystemState":
					RaisePropertyChanged("SystemState");
					break;

				case "AblationTime":
					RaisePropertyChanged("AblationTime");
					break;

				case "ContinuousThawing":
					RaisePropertyChanged("ContinuousThawing");
					break;

				case "TargetInjectionFlow":
					RaisePropertyChanged("TargetInjectionFlow");
					break;

				case "TargetInjectionPressure":
					RaisePropertyChanged("TargetInjectionPressure");
					break;

				case "PGain":
					RaisePropertyChanged("PGain");
					break;

				case "IGain":
					RaisePropertyChanged("IGain");
					break;

				case "DGain":
					RaisePropertyChanged("DGain");
					break;

				case "PIDOffset":
					RaisePropertyChanged("PIDOffset");
					break;

				case "ThresholdForPT1High":
					RaisePropertyChanged("ThresholdForPT1High");
					break;

				case "ThresholdForPT1Fail":
					RaisePropertyChanged("ThresholdForPT1Fail");
					break;

				case "ThresholdForPT1Low":
					RaisePropertyChanged("ThresholdForPT1Low");
					break;

				case "PT1LowRange":
					RaisePropertyChanged("PT1LowRange");
					break;

				case "PT1HighRange":
					RaisePropertyChanged("PT1HighRange");
					break;

				case "ThresholdPT2High":
					RaisePropertyChanged("ThresholdPT2High");
					break;

				case "PT2LowRange":
					RaisePropertyChanged("PT2LowRange");
					break;

				case "PT2HighRange":
					RaisePropertyChanged("PT2HighRange");
					break;

				case "ThresholdPT3High":
					RaisePropertyChanged("ThresholdPT3High");
					break;

				case "PT3LowRange":
					RaisePropertyChanged("PT3LowRange");
					break;

				case "PT3HighRange":
					RaisePropertyChanged("PT3HighRange");
					break;

				case "ThresholdPT4high":
					RaisePropertyChanged("ThresholdPT4high");
					break;

				case "PT4LowRange":
					RaisePropertyChanged("PT4LowRange");
					break;

				case "PT4HighRange":
					RaisePropertyChanged("PT4HighRange");
					break;

				case "ThresholdTS1High":
					RaisePropertyChanged("ThresholdTS1High");
					break;

				case "TS1LowRange":
					RaisePropertyChanged("TS1LowRange");
					break;

				case "TS1HighRange":
					RaisePropertyChanged("TS1HighRange");
					break;

				case "ThresholdFM1Low":
					RaisePropertyChanged("ThresholdFM1Low");
					break;

				case "ThresholdFM1High":
					RaisePropertyChanged("ThresholdFM1High");
					break;

				case "FM1LowRange":
					RaisePropertyChanged("FM1LowRange");
					break;

				case "FM1HighRange":
					RaisePropertyChanged("FM1HighRange");
					break;

				case "ThresholdPS1High":
					RaisePropertyChanged("ThresholdPS1High");
					break;

				case "PS1LowRange":
					RaisePropertyChanged("PS1LowRange");
					break;

				case "PS1HighRange":
					RaisePropertyChanged("PS1HighRange");
					break;

				case "ThresholdPS2High":
					RaisePropertyChanged("ThresholdPS2High");
					break;

				case "PS2LowRange":
					RaisePropertyChanged("PS2LowRange");
					break;

				case "PS2HighRange":
					RaisePropertyChanged("PS2HighRange");
					break;

				case "ThresholdLC1Warning":
					RaisePropertyChanged("ThresholdLC1Warning");
					break;

				case "ThresholdLC1Fail":
					RaisePropertyChanged("ThresholdLC1Fail");
					break;

				case "LC1LowRange":
					RaisePropertyChanged("LC1LowRange");
					break;

				case "LC1HighRange":
					RaisePropertyChanged("LC1HighRange");
					break;

				case "CMCUSystemStatusError":
					RaisePropertyChanged("CMCUSystemStatusError");
					break;

				#endregion Central Micro Controller: Register Values

				#region Errors

				case "IsPMCUExceptionType1":
					RaisePropertyChanged("IsPMCUExceptionType1");
					break;

				case "IsPMCUExceptionType2":
					RaisePropertyChanged("IsPMCUExceptionType2");
					break;

				case "IsPMCUExceptionType3":
					RaisePropertyChanged("IsPMCUExceptionType3");
					break;

				case "IsPMCUExceptionType4":
					RaisePropertyChanged("IsPMCUExceptionType4");
					break;

				case "IsPMCUExceptionType5":
					RaisePropertyChanged("IsPMCUExceptionType5");
					break;

				case "IsPMCUCPLDWatchDogTimerError":
					RaisePropertyChanged("IsPMCUCPLDWatchDogTimerError");
					break;

				case "IsInnerBalloonPressureTooHigh":
					RaisePropertyChanged("IsInnerBalloonPressureTooHigh");
					break;

				case "IsInnerBalloonPressureTooLow":
					RaisePropertyChanged("IsInnerBalloonPressureTooLow");
					break;

				case "IsInnerBalloonPressureReadingOutOfRange":
					RaisePropertyChanged("IsInnerBalloonPressureReadingOutOfRange");
					break;

				case "IsOuterBalloonPressureTooHigh":
					RaisePropertyChanged("IsOuterBalloonPressureTooHigh");
					break;

				case "IsOuterBalloonPressureTooLow":
					RaisePropertyChanged("IsOuterBalloonPressureTooLow");
					break;

				case "IsOuterBalloonPressureReadingOutOrRange":
					RaisePropertyChanged("IsOuterBalloonPressureReadingOutOrRange");
					break;

				case "IsBalloonTipPressureTooHigh":
					RaisePropertyChanged("IsBalloonTipPressureTooHigh");
					break;

				case "IsBalloonTipPressureTooLow":
					RaisePropertyChanged("IsBalloonTipPressureTooLow");
					break;

				case "IsBalloonTipPressurePeadingOutOfRange":
					RaisePropertyChanged("IsBalloonTipPressurePeadingOutOfRange");
					break;

				case "IsThawingTemperatureTooHigh":
					RaisePropertyChanged("IsThawingTemperatureTooHigh");
					break;

				case "IsThawingTemperatureTooLow":
					RaisePropertyChanged("IsThawingTemperatureTooLow");
					break;

				case "IsCatheterCableConnected":
					RaisePropertyChanged("IsCatheterCableConnected");
					break;

				case "IsCatheterTubeConnected":
					ActivateCatheterIfConditionsApply();
					break;

				case "IsCMCUExceptionType1":
					RaisePropertyChanged("IsCMCUExceptionType1");
					break;

				case "IsCMCUExceptionType2":
					RaisePropertyChanged("IsCMCUExceptionType2");
					break;

				case "IsCMCUExceptionType3":
					RaisePropertyChanged("IsCMCUExceptionType3");
					break;

				case "IsCMCUExceptionType4":
					RaisePropertyChanged("IsCMCUExceptionType4");
					break;

				case "IsCMCUExceptionType5":
					RaisePropertyChanged("IsCMCUExceptionType5");
					break;

				case "IsCMCUCPLDWatchDogTimerError":
					RaisePropertyChanged("IsCMCUCPLDWatchDogTimerError");
					break;

				case "IsCMCUTwoMultiplexReadingDoesNotMatch":
					RaisePropertyChanged("IsCMCUTwoMultiplexReadingDoesNotMatch");
					break;

				case "IsCMCUFlowTooHigh":
					RaisePropertyChanged("IsCMCUFlowTooHigh");
					break;

				case "IsCMCUFlowTooLow":
					RaisePropertyChanged("IsCMCUFlowTooLow");
					break;

				case "IsCMCUFlowReadingOutOfRange":
					RaisePropertyChanged("IsCMCUFlowReadingOutOfRange");
					break;

				case "IsCMCULoadCellWeightWarning":
					RaisePropertyChanged("IsCMCULoadCellWeightWarning");
					break;

				case "IsCMCULoadCellWeightFail":
					RaisePropertyChanged("IsCMCULoadCellWeightFail");
					break;

				case "IsCMCULoadCellReadingOutOfRange":
					RaisePropertyChanged("IsCMCULoadCellReadingOutOfRange");
					break;

				case "IsCMCUPressureInTankIsHighFanToBeOn":
					RaisePropertyChanged("IsCMCUPressureInTankIsHighFanToBeOn");
					break;

				case "IsCMCUPressurePT1InTankIsLow":
					RaisePropertyChanged("IsCMCUPressurePT1InTankIsLow");
					break;

				case "IsCMCUPressurePT1InTankIsTooHigh":
					RaisePropertyChanged("IsCMCUPressurePT1InTankIsTooHigh");
					break;

				case "IsCMCUPressurePT1InTankReadingOutOfRange":
					RaisePropertyChanged("IsCMCUPressurePT1InTankReadingOutOfRange");
					break;

				case "IsCMCUPressurePT2AfterCatheterButBeforeReturnLineTooHigh":
					RaisePropertyChanged("IsCMCUPressurePT2AfterCatheterButBeforeReturnLineTooHigh");
					break;

				case "IsCMCUPT2ReadingOutOfRange":
					RaisePropertyChanged("IsCMCUPT2ReadingOutOfRange");
					break;

				case "IsCMCUReturnPressurePT3TooHigh":
					RaisePropertyChanged("IsCMCUReturnPressurePT3TooHigh");
					break;

				case "IsCMCUReturnPressurePT3OutOfRange":
					RaisePropertyChanged("IsCMCUReturnPressurePT3OutOfRange");
					break;

				case "IsCMCUVacuumPressurePT4TooHigh":
					RaisePropertyChanged("IsCMCUVacuumPressurePT4TooHigh");
					break;

				case "IsCMCUVacuumPressurePT4OutOfRange":
					RaisePropertyChanged("IsCMCUVacuumPressurePT4OutOfRange");
					break;

				case "IsCMCUSubCoolerTemperatureIsHigh":
					RaisePropertyChanged("IsCMCUSubCoolerTemperatureIsHigh");
					break;

				case "IsCMCUSubCoolerTemperatureOutOfRange":
					RaisePropertyChanged("IsCMCUSubCoolerTemperatureOutOfRange");
					break;

				case "IsCMCUInjectionVentPressureIsHigh":
					RaisePropertyChanged("IsCMCUInjectionVentPressureIsHigh");
					break;

				case "IsCMCUInjectionVertPressureOutOfRange":
					RaisePropertyChanged("IsCMCUInjectionVertPressureOutOfRange");
					break;

				case "IsCMCUScavengingPressureIsHigh":
					RaisePropertyChanged("IsCMCUScavengingPressureIsHigh");
					break;

				case "IsCMCUScavengingPressureOutOfRange":
					RaisePropertyChanged("IsCMCUScavengingPressureOutOfRange");
					break;

				case "CMCUCJReading":
					RaisePropertyChanged("CMCUCJReading");
					break;

				#endregion Errors

				#region catheter ready

				case "IsCMCUReady":
				case "IsPMCUReady":
					ActivateCatheterIfConditionsApply();
					break;

				case "IsCatheterValid":
					ActivateCatheterIfConditionsApply();
					break;

				#endregion catheter ready

				#region Blood Detector

				case "LowerBloodThreshold":
					RaisePropertyChanged("LowerBloodThreshold");
					break;

				case "UpperBloodThreshold":
					RaisePropertyChanged("UpperBloodThreshold");
					break;

				case "BloodDetecorImValue":
					RaisePropertyChanged("BloodDetecorImValue");
					break;

					#endregion Blood Detector
			}

			if (localCommonViewModel.SystemState == MessageStateId.CAN_ID_STATE_EXCEPTION)
			{
				//localCommonViewModel.DisplayErrorMessage("System Is In Error");

				//MessagePopup messagePopup = new MessagePopup("System Is In Error. Do you want to reset ?",
				//                                              MessagePopup.MessageType.ErrorMessage);

				//if ((bool)messagePopup.ShowDialog())
				//{
				//    localCommonViewModel.Console.FailResetEnable();
				//    System.Threading.Thread.Sleep(10);
				//    localCommonViewModel.Console.FailResetDisable();
				//}
			}
		}

		/// <summary>
		/// Function that activates the catheter when conditions are met
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		private void ActivateCatheterIfConditionsApply()
		{
			RaisePropertyChanged("IsCatheterElectricallyConnectedAndInIdleState");
			RaisePropertyChanged("IsCatheterCableConnected");
			RaisePropertyChanged("IsCatheterTubeConnected");

			if (localCommonViewModel.IsCMCUReady && localCommonViewModel.IsPMCUReady)
				CatheterIsConnecting = false;

			if (localCommonViewModel.IsCatheterCableConnected && localCommonViewModel.IsCatheterTubeConnected)
			{
				IsCatheterConnected = true;
			}
		}

		/// <summary>
		/// This property gets/sets the Catheter Connected boolean flag
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsCatheterConnected
		{
			get
			{
				return localCommonViewModel.IsCatheterConnected;
			}

			set
			{
				SetProperty(ref isCatheterConnected, value);
			}
		}

		/// <summary>
		/// This property gets/sets the Last Use Hour value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public int LastUseHour
		{
			get => lastUseHour;
			set => SetProperty(ref lastUseHour, value);
		}

		/// <summary>
		/// This property gets/sets the Last User Day value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public int LastUseDay
		{
			get => lastUseDay;
			set => SetProperty(ref lastUseDay, value);
		}

		/// <summary>
		/// This property gets/sets the Last Used Month value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public int LastUseMonth
		{
			get => lastUseMonth;
			set => SetProperty(ref lastUseMonth, value);
		}

		/// <summary>
		/// This property gets/sets the Last Used Year value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public int LastUseYear
		{
			get => lastUseYear;
			set => SetProperty(ref lastUseYear, value);
		}

		/// <summary>
		/// Delegate for command to write micro controller when the Write to Micro Controller command is invoked.
		/// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="arg">The command's parameter (not used in this function).</param>
		private async void OnWriteToMicroControllerCommand(object arg)
		{
			IsCommandEnabled = false;
			await OnWriteToMicroController(arg);
			IsCommandEnabled = true;
		}

		/// <summary>
		/// Function/Command that handles writing to micro controller when the Write to Micro Controller
		/// command is invoked
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="arg">The command's parameter (not used in this function).</param>
		private async Task OnWriteToMicroController(object arg)
		{
			if (SelectedRegister == null && arg.ToString() == "CentralMicroController" || PateintSelectedRegister == null && arg.ToString() == "PatientMicroController")
			{
				Tuple<long, string, string, string> genericMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID8, (int)Enumeration.ErrorTypes.GUI);

				MessagePopup dialogPopup = new MessagePopup(genericMessage, messageType: MessagePopup.MessageType.SystemMessage);

				if ((bool)dialogPopup.ShowDialog())
				{
				}
			}
			else
			{
				int state = 0;

				state = Array.IndexOf(Enum.GetValues(typeof(MessageStateId)), ConsoleFiniteStateMachine.CurrentState);

				if (state == 0)
					return;

				#region Patient

				if (arg.ToString() == "PatientMicroController")
				{
					int localPateintSelectedRegister = Convert.ToInt32(PateintSelectedRegister.Tag);
					if (localPateintSelectedRegister == 0)
					{
						for (int id = 48; id < 56; id++)
						{
							await Task.Run(() => localCommonViewModel.Console.WriteFromMicroController((MessageStateId)state, id));
							// Helping yong  to get the data because i am sending the data to fast
							await Task.Delay(TimeSpan.FromMilliseconds(20));
						}
					}
					else
					{
						for (int i = 0; i < maxWritingTime; i++)
						{
							await Task.Run(() => localCommonViewModel.Console.WriteFromMicroController((MessageStateId)state, localPateintSelectedRegister));
							await Task.Delay(TimeSpan.FromMilliseconds(20));
						}
					}
				}

				#endregion Patient

				#region Central;

				else if (arg.ToString() == "CentralMicroController")
				{
					int localSelectedRegister = Convert.ToInt32(SelectedRegister.Tag);
					if (localSelectedRegister == 0)
					{
						for (int id = 8; id < 36; id++)
						{
							await Task.Run(() => localCommonViewModel.Console.WriteFromMicroController((MessageStateId)state, id));
							await Task.Delay(TimeSpan.FromMilliseconds(20));
						}
					}
					else
					{
						for (int i = 0; i < maxWritingTime; i++)
						{
							await Task.Run(() => localCommonViewModel.Console.WriteFromMicroController((MessageStateId)state, localSelectedRegister));
							await Task.Delay(TimeSpan.FromMilliseconds(100));
						}
					}
				}

				#endregion Central;
			}
		}

		/// <summary>
		/// Function/Command that handles catheter programming when the Program Catheter
		/// command is invoked
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="arg">The command's parameter (not used in this function).</param>
		private void OnProgramCatheterCommand(object arg)
		{
			IsCommandEnabled = false;
			logon login = new logon(this);
			login.ShowDialog();

			if (login.TxtUser.Text == "1234" && login.TxtPassword.Password == "1234")
			{
				int state = 0;

				state = Array.IndexOf(Enum.GetValues(typeof(MessageStateId)), ConsoleFiniteStateMachine.CurrentState);

				localCommonViewModel.Console.Catheter.CatheterLot = LotNumber;
				localCommonViewModel.Console.Catheter.SerialNumber = SerilaNumber;

				localCommonViewModel.Console.Catheter.CatheterLastUseHour = LastUseHour;
				localCommonViewModel.Console.Catheter.CatheterLastUseDay = LastUseDay;
				localCommonViewModel.Console.Catheter.CatheterLastUseMonth = LastUseMonth;
				localCommonViewModel.Console.Catheter.CatheterLastUseYear = LastUseHour;

				for (int i = 0; i < maxWritingTime; i++)
				{
					localCommonViewModel.Console.WriteFromMicroController((MessageStateId)state, 51);
					System.Threading.Thread.Sleep(50);
					localCommonViewModel.Console.WriteFromMicroController((MessageStateId)state, 50);
					System.Threading.Thread.Sleep(50);
				}
			}
			else
			{
				//Tuple<long, string, string, string> genericMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.IDXX, (int)Enumeration.ErrorTypes.GUI);

				MessagePopup dialogPopup = new MessagePopup("Y N S D", messageType: MessagePopup.MessageType.SystemMessage);

				if ((bool)dialogPopup.ShowDialog())
				{
				}
			}
			IsCommandEnabled = true;
		}

		/// <summary>
		/// Delegate for command to read from  micro controller when the read from Micro Controller command is invoked.
		/// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="arg">The command's parameter (not used in this function).</param>
		private async void OnReadFromMicroControllerCommand(object arg)
		{
			IsCommandEnabled = false;
			await ReadFromMicroController(arg);
			IsCommandEnabled = true;
		}

		/// <summary>
		/// Asynchronous function that reads values from the micro controller.
		/// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="arg">The function's parameter (not used in this function).</param>
		/// <returns>No return</returns>
		private async Task ReadFromMicroController(object arg)
		{
			if ((IsCatheterCableConnected && localCommonViewModel.IsPMCUReady && localCommonViewModel.IsCMCUReady) || !IsCatheterCableConnected)
			{
				int state = 0;

				state = Array.IndexOf(Enum.GetValues(typeof(MessageStateId)), ConsoleFiniteStateMachine.CurrentState);

				if (arg.ToString() == "PatientMicroController")
				{
					for (int id = 48; id < 56; id++)
					{
						await Task.Run(() => localCommonViewModel.Console.ReadFromMicroController((MessageStateId)state, id));
						await Task.Delay(TimeSpan.FromMilliseconds(20));
					}

					readingArg = "PatientMicroController";
				}
				//To do
				else if (arg.ToString() == "CentralMicroController")
				{
					for (int id = 8; id < 36; id++)  // 8  36
					{
						await Task.Run(() => localCommonViewModel.Console.ReadFromMicroController((MessageStateId)state, id));
						await Task.Delay(TimeSpan.FromMilliseconds(20));
					}

					readingArg = "CentralMicroController";
				}
			}
		}

		/// <summary>
		/// Function/Command that handles set SV Level when the Set SV Level
		/// command is invoked
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="arg">The command's parameter (SV Id).</param>
		private void OnSetSVLevelCommand(object arg)
		{
			IsCommandEnabled = false;
			uint SVId = Convert.ToUInt32(arg);
			uint valuesCombinationTosend = 0;

			switch (SVId)
			{
				case 0:
					SvLevel1 = ((IsSV0Activated == true) ? (uint)CalibrationComponentANDCPLDRegister.CPLDRegisterId.SV1 : (uint)0);
					break;

				case 1:
					SvLevel2 = ((IsSV1Activated == true) ? (uint)CalibrationComponentANDCPLDRegister.CPLDRegisterId.SV2 : (uint)0);
					break;

				case 2:
					SvLevel3 = ((IsSV2Activated == true) ? (uint)CalibrationComponentANDCPLDRegister.CPLDRegisterId.SV3 : (uint)0);
					break;

				case 3:
					SvLevel4 = ((IsSV3Activated == true) ? (uint)CalibrationComponentANDCPLDRegister.CPLDRegisterId.SV4 : (uint)0);
					break;

				case 4:
					SvLevel5 = ((IsSV4Activated == true) ? (uint)CalibrationComponentANDCPLDRegister.CPLDRegisterId.SV5 : (uint)0);
					break;

				case 5:
					SvLevel6 = ((IsSV5Activated == true) ? (uint)CalibrationComponentANDCPLDRegister.CPLDRegisterId.SV6 : (uint)0);
					break;

				case 6:
					SvLevel7 = ((IsSV6Activated == true) ? (uint)CalibrationComponentANDCPLDRegister.CPLDRegisterId.Sv7 : (uint)0);
					break;

				case 7:
					SvLevel8 = ((IsSV7Activated == true) ? (uint)CalibrationComponentANDCPLDRegister.CPLDRegisterId.SV8 : (uint)0);
					break;

				case 8:
					SvLevel9 = ((IsSV8Activated == true) ? (uint)CalibrationComponentANDCPLDRegister.CPLDRegisterId.SV9 : (uint)0);
					break;

				case 9:
					FanLevel = ((IsFANActivated == true) ? (uint)CalibrationComponentANDCPLDRegister.CPLDRegisterId.FAN : (uint)0);
					break;

				case 10:
					SvLevel10 = ((IsSV10Activated == true) ? (uint)CalibrationComponentANDCPLDRegister.CPLDRegisterId.SV10 : (uint)0);
					break;

				case 11:
					SvLevel11 = ((IsSV11Activated == true) ? (uint)CalibrationComponentANDCPLDRegister.CPLDRegisterId.SV11 : (uint)0);
					break;
			}

			valuesCombinationTosend = SvLevel1 | SvLevel2 | SvLevel3 | SvLevel4 | SvLevel5 | SvLevel6 | SvLevel7 | SvLevel8 | SvLevel9 | FanLevel | SvLevel10 | SvLevel11;

			localCommonViewModel.Console.SetCPLDSVLevel(valuesCombinationTosend);
			IsCommandEnabled = true;
		}

		/// <summary>
		/// Function that returns if the system can invoke the Set GPIO Level command
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="arg">The command parameter (not used in this function).</param>
		/// <returns>Boolean value if the system can invoke the command.</returns>
		private bool CanSetGPIOLevelCommand(object arg)
		{
			return true;
		}

		/// <summary>
		/// Function/Command that handles the setting of GPIO Level when the Set GPIO Level
		/// command is invoked
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="arg">The command's parameter (GPIO Id).</param>
		private void OnSetGPIOLevelCommand(object arg)
		{
			IsCommandEnabled = false;
			uint GPIOId = Convert.ToUInt32(arg);

			switch (GPIOId)
			{
				case 0:
					Level = ((IsGPIO0Activated == true) ? (uint)1 : (uint)0);
					break;

				case 1:
					Level = ((IsGPIO1Activated == true) ? (uint)1 : (uint)0);
					break;

				case 2:
					Level = ((IsGPIO2Activated == true) ? (uint)1 : (uint)0);

					break;

				case 3:
					Level = ((IsGPIO3Activated == true) ? (uint)1 : (uint)0);
					break;

				case 4:
					Level = ((IsGPIO4Activated == true) ? (uint)1 : (uint)0);
					break;

				case 5:
					Level = ((IsGPIO5Activated == true) ? (uint)1 : (uint)0);
					break;

				case 6:
					Level = ((IsGPIO6Activated == true) ? (uint)1 : (uint)0);
					break;

				case 7:
					Level = ((IsGPIO7Activated == true) ? (uint)1 : (uint)0);
					break;
			}
			localCommonViewModel.Console.GeneralPurposeInputOutput.SetGPIOLevel(GPIOId, 1, Level);
			IsCommandEnabled = true;
		}

		/// <summary>
		/// Function/Command that handles software closure when the Close
		/// command is invoked
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="arg">The command's parameter (not used in this function).</param>
		private void OnCloseCommand(object arg)
		{
			IsClosing = true;

			Tuple<long, string, string, string> genericMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID9, (int)Enumeration.ErrorTypes.GUI);
			MessagePopup dialogPopup = new MessagePopup(genericMessage, messageType: MessagePopup.MessageType.SystemMessage);

			if ((bool)dialogPopup.ShowDialog())
			{
				IsCommandEnabled = false;
				localCommonViewModel.Console.PowerOffMessage();
				System.Threading.Thread.Sleep(500);
				localCommonViewModel.Console.DeactivateAllIOS();
				System.Threading.Thread.Sleep(500);
				localCommonViewModel.Console.CanBusCommunication.Dispose();
				System.Threading.Thread.Sleep(1000);
				Environment.Exit(0);
			}
			else
			{
				IsClosing = false;
				return;
			}
		}

		/// <summary>
		/// Function/Command that handles software closure when the Close
		/// command is invoked
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="arg">The command's parameter (not used in this function).</param>
		[DllImport("wtsapi32.dll", SetLastError = true)]
		private static extern bool WTSDisconnectSession(IntPtr hServer, int sessionId, bool bWait);

		private const int WTS_CURRENT_SESSION = -1;
		private static readonly IntPtr WTS_CURRENT_SERVER_HANDLE = IntPtr.Zero;

		/// <summary>
		/// Log out command
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="arg">object</param>
		private void OnLogoutCommand(object arg)
		{
			Tuple<long, string, string, string> genericMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID15, (int)Enumeration.ErrorTypes.GUI);

			MessagePopup dialogPopup = new MessagePopup(genericMessage);

			if ((bool)dialogPopup.ShowDialog())
			{
				localCommonViewModel.Console.GUIIsReady = false;
				System.Threading.Thread.Sleep(1000);

				for (int i = 0; i < maxWritingTime; i++)

				{
					localCommonViewModel.RequiredVolume = 0;
					System.Threading.Thread.Sleep(20);
				}

				ManagePowerOff();
			}
		}

		/// <summary>
		/// Power off the system
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		private void ManagePowerOff()
		{
			localCommonViewModel.Console.PowerOffMessage();
			System.Threading.Thread.Sleep(500);
			localCommonViewModel.Console.DeactivateAllIOS();
			System.Threading.Thread.Sleep(500);
			localCommonViewModel.Console.CanBusCommunication.Dispose();
			System.Threading.Thread.Sleep(1000);

			if (!WTSDisconnectSession(WTS_CURRENT_SERVER_HANDLE,
				WTS_CURRENT_SESSION, false))
			{
				throw new System.ComponentModel.Win32Exception();
			}
			Environment.Exit(0);
		}

		/// <summary>
		/// Function/Command that handles maintenance mode when the Maintenance
		/// command is invoked
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="arg">The command's parameter (not used in this function).</param>
		private void OnMaintenanceCommand(object arg)
		{
			IsCommandEnabled = false;
			EnableOrDisableMaintenanceMode = EnableOrDisableMaintenanceMode == false;
			IsCommandEnabled = true;
		}

		/// <summary>
		/// Function/Command that handles writing the connection box diaphragm min and max values when the
		/// command is invoked
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="arg">The command's parameter (not used in this function).</param>
		private void OnWriteConnectionBoxDiaphragmMinMaxCommand(object arg)
		{
			IsCommandEnabled = false;
			CommonViewModel.Current.ConnectionBox.DiaphragmeMinimumValue = ConnectionBoxDiaphragmMinimumValue;
			CommonViewModel.Current.ConnectionBox.DiaphragmeMaximumValue = ConnectionBoxDiaphragmMaximumValue;
			CommonViewModel.Current.DMSDetectionThreshold = DMSDetectionThreshold;
			IsCommandEnabled = true;
		}

		/// <summary>
		/// Function/Command that handles the console connection when the Connect
		/// command is invoked
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="arg">The command's parameter (not used in this function).</param>
		private void OnConnectCommand(object arg)
		{
			IsCommandEnabled = false;
			if (localCommonViewModel.SystemState == MessageStateId.CAN_ID_STATE_READY)
			{
				localCommonViewModel.Console.Disconnect();
			}
			else if (IsCatheterCableConnected)
			{
				localCommonViewModel.Console.Connect();
			}
			IsCommandEnabled = true;
		}

		/// <summary>
		/// Function/Command that handles console start when the Start
		/// command is invoked
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="arg">The command's parameter (not used in this function).</param>
		private void OnStartCommand(object arg)
		{
			IsCommandEnabled = false;
			localCommonViewModel.Console.Start();
			IsCommandEnabled = true;
		}

		/// <summary>
		/// Function/Command that handles the console Stop when the Stop
		/// command is invoked
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="arg">The command's parameter (not used in this function).</param>
		private void OnStopCommand(object arg)
		{
			IsCommandEnabled = false;
			localCommonViewModel.Console.Stop();
			IsCommandEnabled = true;
		}

    private void OnFastButtonCommand(object arg)
    {
      CommonViewModel.Current.Console.EnableFastInflationMode = true;
      RefreshInflationSpeedMode();
    }

    private void OnSlowButtonCommand(object arg)
    {
      CommonViewModel.Current.Console.EnableFastInflationMode = false;
      RefreshInflationSpeedMode();
    }

    /// <summary>
    /// This read-only property gets/sets the State List
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public List<string> StatesList
		{
			get
			{
				List<string> convertedState = new List<string>();
				MessageStateId[] states = (MessageStateId[])Enum.GetValues(typeof(MessageStateId));

				foreach (MessageStateId element in states)
				{
					convertedState.Add(element.ToString().Replace("CAN_ID_STATE_", string.Empty));
				}
				return convertedState;
			}
		}

		/// <summary>
		/// This property gets/sets the Selected State value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public string SelectedState
		{
			get
			{
				return selectedState;
			}

			set
			{
				switch (StatesList.IndexOf(value))
				{
					case 1:

						ConsoleFiniteStateMachine.CurrentState = MessageStateId.CAN_ID_STATE_IDLE;
						break;

					case 2:

						ConsoleFiniteStateMachine.CurrentState = MessageStateId.CAN_ID_STATE_READY;
						break;

					case 3:

						ConsoleFiniteStateMachine.CurrentState = MessageStateId.CAN_ID_STATE_INFLATION;
						break;

					case 4:

						ConsoleFiniteStateMachine.CurrentState = MessageStateId.CAN_ID_STATE_TRANSITION;
						break;

					case 5:

						ConsoleFiniteStateMachine.CurrentState = MessageStateId.CAN_ID_STATE_ABLATION;

						break;

					case 6:

						ConsoleFiniteStateMachine.CurrentState = MessageStateId.CAN_ID_STATE_THAWING;

						break;

					case 7:

						ConsoleFiniteStateMachine.CurrentState = MessageStateId.CAN_ID_STATE_EXCEPTION;

						break;

					default:

						ConsoleFiniteStateMachine.CurrentState = MessageStateId.CAN_ID_STATE_UNKNOWN;

						break;
				}
				SetProperty(ref selectedState, value);
			}
		}

		/// <summary>
		/// Function that loads the Patient and Central microcontroller registers according to the system's state
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="stateID">The system's state ID.</param>
		private void LoadRegistersAccordingToState(MessageStateId stateID)
		{
			Machine console = localCommonViewModel.Console;

			#region Patient register

			//CP1
			ThresholdForCP1High = console.PatientPressureTransducerOneValueAccordingToTheStateMachine[stateID].PressureThresholdHighLimit;

			//CP2
			ThresholdForOuterBallonPressure = console.PatientPressureTransducerTwoValueAccordingToTheStateMachine[stateID].PressureThresholdHighLimit;

			//TC1

			//Patient Micro Controller PID
			PatientPGain = console.PatientMicroControllerPIDValueAccordingToTheStateMachine[stateID].PGain;
			PatientIGain = console.PatientMicroControllerPIDValueAccordingToTheStateMachine[stateID].IGain;
			PatientDGain = console.PatientMicroControllerPIDValueAccordingToTheStateMachine[stateID].DGain;
			PatientPIDOffset = console.PatientMicroControllerPIDValueAccordingToTheStateMachine[stateID].Offset;

			#endregion Patient register

			#region Central microcontroller

			TargetInjectionFlow = console.InjectionFlowValueAccordingToTheStateMachine[stateID].TargetInjectionFlow;

			//Central Micro Controller PID CentralMicroControllerPIDValueAccordingToTheStateMachine
			PGain = console.CentralMicroControllerPIDValueAccordingToTheStateMachine[stateID].PGain;
			IGain = console.CentralMicroControllerPIDValueAccordingToTheStateMachine[stateID].IGain;
			DGain = console.CentralMicroControllerPIDValueAccordingToTheStateMachine[stateID].DGain;
			PIDOffset = console.CentralMicroControllerPIDValueAccordingToTheStateMachine[stateID].Offset;

			//PT1
			ThresholdForPT1High = console.PressureTransducerOneValueAccordingToTheStateMachine[stateID].PressureThresholdHighLimit;
			ThresholdForPT1Fail = console.PressureTransducerOneValueAccordingToTheStateMachine[stateID].TankPressureTooHigh;
			ThresholdForPT1Low = console.PressureTransducerOneValueAccordingToTheStateMachine[stateID].TankPressureLow;
			PT1LowRange = console.PressureTransducerOneValueAccordingToTheStateMachine[stateID].PressureLowRangeLimit;
			PT1HighRange = console.PressureTransducerOneValueAccordingToTheStateMachine[stateID].PressureHighRangeLimit;

			//PT2
			ThresholdPT2High = console.PressureTransducerTwoValueAccordingToTheStateMachine[stateID].PressureThresholdHighLimit;
			PT2LowRange = console.PressureTransducerTwoValueAccordingToTheStateMachine[stateID].PressureLowRangeLimit;
			PT2HighRange = console.PressureTransducerTwoValueAccordingToTheStateMachine[stateID].PressureHighRangeLimit;

			//PT3
			ThresholdPT3High = console.PressureTransducerThreeValueAccordingToTheStateMachine[stateID].PressureThresholdHighLimit;
			PT3LowRange = console.PressureTransducerThreeValueAccordingToTheStateMachine[stateID].PressureLowRangeLimit;
			PT3HighRange = console.PressureTransducerThreeValueAccordingToTheStateMachine[stateID].PressureHighRangeLimit;

			//PT4
			ThresholdPT4high = console.PressureTransducerFourValueAccordingToTheStateMachine[stateID].PressureThresholdHighLimit;
			PT4LowRange = console.PressureTransducerFourValueAccordingToTheStateMachine[stateID].PressureLowRangeLimit;
			PT4HighRange = console.PressureTransducerFourValueAccordingToTheStateMachine[stateID].PressureHighRangeLimit;

			//TS1
			ThresholdTS1High = console.TemperatureSensorOneValueAccordingToTheStateMachine[stateID].TemperatureThresholdHighLimit;
			TS1LowRange = console.TemperatureSensorOneValueAccordingToTheStateMachine[stateID].TemperatureLowRangeLimit;
			TS1HighRange = console.TemperatureSensorOneValueAccordingToTheStateMachine[stateID].TemperatureHighRangeLimit;

			//FM1
			ThresholdFM1Low = console.FlowMeterOneValueAccordingToTheStateMachine[stateID].FlowMeterThresholLowlimit;
			ThresholdFM1High = console.FlowMeterOneValueAccordingToTheStateMachine[stateID].FlowMeterThresholHighlimit;
			FM1LowRange = console.FlowMeterOneValueAccordingToTheStateMachine[stateID].FlowMeterLowRangeLimit;
			FM1HighRange = console.FlowMeterOneValueAccordingToTheStateMachine[stateID].FlowMeterHighRangelimit;

			//PS1
			ThresholdPS1High = console.PressureSwitchOneValueAccordingToTheStateMachine[stateID].PressureThresholdHighLimit;
			PS1LowRange = console.PressureSwitchOneValueAccordingToTheStateMachine[stateID].PressureLowRangeLimit;
			PS1HighRange = console.PressureSwitchOneValueAccordingToTheStateMachine[stateID].PressureHighRangeLimit;

			//PS2
			ThresholdPS2High = console.PressureSwitchTwoValueAccordingToTheStateMachine[stateID].PressureThresholdHighLimit;
			PS2LowRange = console.PressureSwitchTwoValueAccordingToTheStateMachine[stateID].PressureLowRangeLimit;
			PS2HighRange = console.PressureSwitchTwoValueAccordingToTheStateMachine[stateID].PressureHighRangeLimit;

			//LC1
			ThresholdLC1Warning = console.LoadCellOneValueAccordingToTheStateMachine[stateID].LoadCellThresholdWarning;
			ThresholdLC1Fail = console.LoadCellOneValueAccordingToTheStateMachine[stateID].LoadCellThresholdFail;
			LC1LowRange = console.LoadCellOneValueAccordingToTheStateMachine[stateID].LoadCellLowRangeLimit;
			LC1HighRange = console.LoadCellOneValueAccordingToTheStateMachine[stateID].LoadCellHighRangeLimit;

			#endregion Central microcontroller
		}

		/// <summary>
		/// This property gets/sets the Closing boolean flag
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsClosing
		{
			get
			{
				return isClosing;
			}

			set
			{
				isClosing = value;
				RaisePropertyChanged("IsClosing");
			}
		}

		/// <summary>
		/// This property gets/sets the SV0 Activated boolean flag value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsSV0Activated
		{
			get
			{
				return isSV0Activated;
			}

			set
			{
				isSV0Activated = value;
				SetProperty(ref isGPIO4Activated, value);
			}
		}

		/// <summary>
		/// This property gets/sets the SV1 Activated boolean flag value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsSV1Activated
		{
			get
			{
				return isSV1Activated;
			}

			set
			{
				SetProperty(ref isSV1Activated, value);
			}
		}

		/// <summary>
		/// This property gets/sets the SV2 Activated boolean flag value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsSV2Activated
		{
			get
			{
				return isSV2Activated;
			}

			set
			{
				SetProperty(ref isSV2Activated, value);
			}
		}

		/// <summary>
		/// This property gets/sets the SV3 Activated boolean flag value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsSV3Activated
		{
			get
			{
				return isSV3Activated;
			}

			set
			{
				SetProperty(ref isSV3Activated, value);
			}
		}

		/// <summary>
		/// This property gets/sets the SV4 Activated boolean flag value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsSV4Activated
		{
			get
			{
				return isSV4Activated;
			}

			set
			{
				SetProperty(ref isSV4Activated, value);
			}
		}

		/// <summary>
		/// This property gets/sets the SV5 Activated boolean flag value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsSV5Activated
		{
			get
			{
				return isSV5Activated;
			}

			set
			{
				SetProperty(ref isSV5Activated, value);
			}
		}

		/// <summary>
		/// This property gets/sets the SV6 Activated boolean flag value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsSV6Activated
		{
			get
			{
				return isSV6Activated;
			}

			set
			{
				SetProperty(ref isSV6Activated, value);
			}
		}

		/// <summary>
		/// This property gets/sets the SV7 Activated boolean flag value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsSV7Activated
		{
			get
			{
				return isSV7Activated;
			}

			set
			{
				SetProperty(ref isSV7Activated, value);
			}
		}

		/// <summary>
		/// This property gets/sets the Sv Level Previous value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public uint SvLevelPrevious
		{
			get
			{
				return svLevelPrevious;
			}

			set
			{
				svLevelPrevious = value;
			}
		}

		/// <summary>
		/// This read-only property returns the PMCU Exception Type 1 value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsPMCUExceptionType1
		{
			get { return localCommonViewModel.IsPMCUExceptionType1; }
		}

		/// <summary>
		/// This read-only property returns the PMCU Exception Type 2 value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsPMCUExceptionType2
		{
			get { return localCommonViewModel.IsPMCUExceptionType2; }
		}

		/// <summary>
		/// This read-only property returns the PMCU Exception Type 3 value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsPMCUExceptionType3
		{
			get { return localCommonViewModel.IsPMCUExceptionType3; }
		}

		/// <summary>
		/// This read-only property returns the PMCU Exception Type 4 value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsPMCUExceptionType4
		{
			get { return localCommonViewModel.IsPMCUExceptionType4; }
		}

		/// <summary>
		/// This read-only property returns the PMCU Exception Type 5 value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsPMCUExceptionType5
		{
			get { return localCommonViewModel.IsPMCUExceptionType5; }
		}

		/// <summary>
		/// This read-only property returns the PMCU CPLD Watchdog Timer Error value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsPMCUCPLDWatchDogTimerError
		{
			get { return localCommonViewModel.IsPMCUCPLDWatchDogTimerError; }
		}

		/// <summary>
		/// This read-only property returns the Inner Balloon Pressure Too High boolean flag value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsInnerBalloonPressureTooHigh
		{
			get { return localCommonViewModel.IsInnerBalloonPressureTooHigh; }
		}

		/// <summary>
		/// This read-only property returns the Inner Balloon Pressure Too Low boolean flag value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsInnerBalloonPressureTooLow
		{
			get { return localCommonViewModel.IsInnerBalloonPressureTooLow; }
		}

		/// <summary>
		/// This read-only property returns the Inner Balloon Pressure Reading Out of Range flag value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsInnerBalloonPressureReadingOutOfRange
		{
			get { return localCommonViewModel.IsInnerBalloonPressureReadingOutOfRange; }
		}

		/// <summary>
		/// This read-only property returns the Outer Balloon Pressure Too High boolean flag value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsOuterBalloonPressureTooHigh
		{
			get { return localCommonViewModel.IsOuterBalloonPressureTooHigh; }
		}

		/// <summary>
		/// This read-only property returns the Outer Balloon Pressure Too Low boolean flag value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsOuterBalloonPressureTooLow
		{
			get { return localCommonViewModel.IsOuterBalloonPressureTooLow; }
		}

		/// <summary>
		/// This read-only property returns the Outer Balloon Pressure Reading Out Of Range flag value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsOuterBalloonPressureReadingOutOrRange
		{
			get { return localCommonViewModel.IsOuterBalloonPressureReadingOutOrRange; }
		}

		/// <summary>
		/// This read-only property returns the Balloon Tip Pressure Too High boolean flag value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsBalloonTipPressureTooHigh
		{
			get { return localCommonViewModel.IsBalloonTipPressureTooHigh; }
		}

		/// <summary>
		/// This read-only property returns the Balloon Tip Pressure Too Low boolean flag value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsBalloonTipPressureTooLow
		{
			get { return localCommonViewModel.IsBalloonTipPressureTooLow; }
		}

		/// <summary>
		/// This read-only property returns the Balloon Tip Pressure Reading Out of Range boolean flag value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsBalloonTipPressurePeadingOutOfRange
		{
			get { return localCommonViewModel.IsBalloonTipPressurePeadingOutOfRange; }
		}

		/// <summary>
		/// This read-only property returns the Thawing Temperature Too High boolean flag value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsThawingTemperatureTooHigh
		{
			get { return localCommonViewModel.IsThawingTemperatureTooHigh; }
		}

		/// <summary>
		/// This read-only property returns the Thawing Temperature Too Low boolean flag value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsThawingTemperatureTooLow
		{
			get { return localCommonViewModel.IsThawingTemperatureTooLow; }
		}

		/// <summary>
		/// This read-only property returns the Balloon Temperature Too High boolean flag value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsBalloonTemperatureTooHigh
		{
			get { return localCommonViewModel.IsBalloonTemperatureTooHigh; }
		}

		/// <summary>
		/// This read-only property returns the Catheter Cable Connected boolean flag value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsCatheterCableConnected
		{
			get
			{
				bool _IsCatheterCableConnected = CommonViewModel.Current.IsCatheterCableConnected;

				if (_IsCatheterCableConnected && (!CommonViewModel.Current.IsCMCUReady || !CommonViewModel.Current.IsPMCUReady))
					CatheterIsConnecting = true;
				else
					CatheterIsConnecting = false;

				return _IsCatheterCableConnected;
			}
		}

		/// <summary>
		/// This read-only property returns the Catheter Tube Connected boolean flag value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsCatheterTubeConnected
		{
			get { return localCommonViewModel.IsCatheterTubeConnected; }
		}

		/// <summary>
		/// This read-only property returns the CMCU Exception Type 1 boolean flag value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsCMCUExceptionType1
		{
			get { return localCommonViewModel.IsCMCUExceptionType1; }
		}

		/// <summary>
		/// This read-only property returns the CMCU Exception Type 2 boolean flag value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsCMCUExceptionType2
		{
			get { return localCommonViewModel.IsCMCUExceptionType2; }
		}

		/// <summary>
		/// This read-only property returns the CMCU Exception Type 3 boolean flag value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsCMCUExceptionType3
		{
			get { return localCommonViewModel.IsCMCUExceptionType3; }
		}

		/// <summary>
		/// This read-only property returns the CMCU Exception Type 4 boolean flag value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsCMCUExceptionType4
		{
			get { return localCommonViewModel.IsCMCUExceptionType4; }
		}

		/// <summary>
		/// This read-only property returns the CMCU Exception Type 5 boolean flag value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsCMCUExceptionType5
		{
			get { return localCommonViewModel.IsCMCUExceptionType5; }
		}

		/// <summary>
		/// This read-only property returns the CMCU CPLD Watchdog Timer Error boolean flag value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsCMCUCPLDWatchDogTimerError
		{
			get { return localCommonViewModel.IsCMCUCPLDWatchDogTimerError; }
		}

		/// <summary>
		/// This read-only property returns the CMCU Two Multiplex Reading Does Not Match boolean flag value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsCMCUTwoMultiplexReadingDoesNotMatch
		{
			get { return localCommonViewModel.IsCMCUTwoMultiplexReadingDoesNotMatch; }
		}

		/// <summary>
		/// This read-only property returns the CMCU Flow Too High boolean flag value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsCMCUFlowTooHigh
		{
			get { return localCommonViewModel.IsCMCUFlowTooHigh; }
		}

		/// <summary>
		/// This read-only property returns the CMCU Flow Too Low boolean flag value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsCMCUFlowTooLow
		{
			get { return localCommonViewModel.IsCMCUFlowTooLow; }
		}

		/// <summary>
		/// This read-only property returns the CMCU Flow Reading Out of Range boolean flag value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsCMCUFlowReadingOutOfRange
		{
			get { return localCommonViewModel.IsCMCUFlowReadingOutOfRange; }
		}

		/// <summary>
		/// This read-only property returns the CMCU Load Cell Weight Warning boolean flag value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsCMCULoadCellWeightWarning
		{
			get { return localCommonViewModel.IsCMCULoadCellWeightWarning; }
		}

		/// <summary>
		/// This read-only property returns the CMCU Load Cell Weight Fail boolean flag value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsCMCULoadCellWeightFail
		{
			get { return localCommonViewModel.IsCMCULoadCellWeightFail; }
		}

		/// <summary>
		/// This read-only property returns the CMCU Load Cell Reading Out of Range boolean flag value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsCMCULoadCellReadingOutOfRange
		{
			get { return localCommonViewModel.IsCMCULoadCellReadingOutOfRange; }
		}

		/// <summary>
		/// This read-only property returns the CMCU Pressure In Tank Is High Fan To Be On boolean flag value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsCMCUPressureInTankIsHighFanToBeOn
		{
			get { return localCommonViewModel.IsCMCUPressureInTankIsHighFanToBeOn; }
		}

		/// <summary>
		/// This read-only property returns the CMCU Pressure PT1 In Tank is Low boolean flag value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsCMCUPressurePT1InTankIsLow
		{
			get { return localCommonViewModel.IsCMCUPressurePT1InTankIsLow; }
		}

		/// <summary>
		/// This read-only property returns the CMCU Pressure PT1 In Tank is High boolean flag value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsCMCUPressurePT1InTankIsTooHigh
		{
			get { return localCommonViewModel.IsCMCUPressurePT1InTankIsTooHigh; }
		}

		/// <summary>
		/// This read-only property returns the CMCU Pressure PT1 In Tank Reading Out of Range boolean flag value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsCMCUPressurePT1InTankReadingOutOfRange
		{
			get { return localCommonViewModel.IsCMCUPressurePT1InTankReadingOutOfRange; }
		}

		/// <summary>
		/// This read-only property returns the CMCU Pressure PT2 After Catheter but Before Return Line Too High
		/// boolean flag value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsCMCUPressurePT2AfterCatheterButBeforeReturnLineTooHigh
		{
			get { return localCommonViewModel.IsCMCUPressurePT2AfterCatheterButBeforeReturnLineTooHigh; }
		}

		/// <summary>
		/// This read-only property returns the CMCU Pressure PT2 Reading Out of Range boolean flag value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsCMCUPT2ReadingOutOfRange
		{
			get { return localCommonViewModel.IsCMCUPT2ReadingOutOfRange; }
		}

		/// <summary>
		/// This read-only property returns the CMCU Return Pressure PT3 Too High boolean flag value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsCMCUReturnPressurePT3TooHigh
		{
			get { return localCommonViewModel.IsCMCUReturnPressurePT3TooHigh; }
		}

		/// <summary>
		/// This read-only property returns the CMCU Return Pressure PT3 Out of Range boolean flag value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsCMCUReturnPressurePT3OutOfRange
		{
			get { return localCommonViewModel.IsCMCUReturnPressurePT3OutOfRange; }
		}

		/// <summary>
		/// This read-only property returns the CMCU Return Pressure PT4 Too High boolean flag value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsCMCUVacuumPressurePT4TooHigh
		{
			get { return localCommonViewModel.IsCMCUVacuumPressurePT4TooHigh; }
		}

		/// <summary>
		/// This read-only property returns the CMCU Return Pressure PT4 Out of Range boolean flag value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsCMCUVacuumPressurePT4OutOfRange
		{
			get { return localCommonViewModel.IsCMCUVacuumPressurePT4OutOfRange; }
		}

		/// <summary>
		/// This read-only property returns the CMCU Sub Cooler Temperature is High boolean flag value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsCMCUSubCoolerTemperatureIsHigh
		{
			get { return localCommonViewModel.IsCMCUSubCoolerTemperatureIsHigh; }
		}

		/// <summary>
		/// This read-only property returns the CMCU Sub Cooler Temperature Out of Range boolean flag value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsCMCUSubCoolerTemperatureOutOfRange
		{
			get { return localCommonViewModel.IsCMCUSubCoolerTemperatureOutOfRange; }
		}

		/// <summary>
		/// This read-only property returns the CMCU Injection Vent Pressure Is High boolean flag value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsCMCUInjectionVentPressureIsHigh
		{
			get { return localCommonViewModel.IsCMCUInjectionVentPressureIsHigh; }
		}

		/// <summary>
		/// This read-only property returns the CMCU Injection Vert Pressure Out of Range boolean flag value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsCMCUInjectionVertPressureOutOfRange
		{
			get { return localCommonViewModel.IsCMCUInjectionVertPressureOutOfRange; }
		}

		/// <summary>
		/// This read-only property returns the CMCU Scavenging Pressure Is High boolean flag value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsCMCUScavengingPressureIsHigh
		{
			get { return localCommonViewModel.IsCMCUScavengingPressureIsHigh; }
		}

		/// <summary>
		/// This read-only property returns the CMCU Scavenging Pressure Out of Range boolean flag value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		//public bool IsCMCUScavengingPressureOutOfRange
		//{
		//    get { return localCommonViewModel.IsCMCUScavengingPressureOutOfRange; }
		//}

		/// <summary>
		/// This property gets/sets Sv Level 1 value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public uint SvLevel1
		{
			get
			{
				return svLevel1;
			}

			set
			{
				svLevel1 = value;
			}
		}

		/// <summary>
		/// This property gets/sets Sv Level 2 value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public uint SvLevel2
		{
			get
			{
				return svLevel2;
			}

			set
			{
				svLevel2 = value;
			}
		}

		/// <summary>
		/// This property gets/sets Sv Level 3 value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public uint SvLevel3
		{
			get
			{
				return svLevel3;
			}

			set
			{
				svLevel3 = value;
			}
		}

		/// <summary>
		/// This property gets/sets Sv Level 4 value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public uint SvLevel4
		{
			get
			{
				return svLevel4;
			}

			set
			{
				svLevel4 = value;
			}
		}

		/// <summary>
		/// This property gets/sets Sv Level 5 value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public uint SvLevel5
		{
			get
			{
				return svLevel5;
			}

			set
			{
				svLevel5 = value;
			}
		}

		/// <summary>
		/// This property gets/sets Sv Level 6 value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public uint SvLevel6
		{
			get
			{
				return svLevel6;
			}

			set
			{
				svLevel6 = value;
			}
		}

		/// <summary>
		/// This property gets/sets Sv Level 8 value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public uint SvLevel8
		{
			get
			{
				return svLevel8;
			}

			set
			{
				svLevel8 = value;
			}
		}

		/// <summary>
		/// This property gets/sets Sv Level 7 value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public uint SvLevel7
		{
			get
			{
				return svLevel7;
			}

			set
			{
				svLevel7 = value;
			}
		}

		/// <summary>
		/// This property gets/sets Sv Level 9 value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public uint SvLevel9
		{
			get
			{
				return svLevel9;
			}

			set
			{
				svLevel9 = value;
			}
		}

		/// <summary>
		/// This property gets/sets Sv Level 8 value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsSV8Activated
		{
			get
			{
				return isSV8Activated;
			}

			set
			{
				SetProperty(ref isSV8Activated, value);
			}
		}

		/// <summary>
		/// This property gets/sets FAN Activated value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsFANActivated
		{
			get
			{
				return isFANActivated;
			}

			set
			{
				isFANActivated = value;
			}
		}

		/// <summary>
		/// This property gets/sets SV 10 Activated value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsSV10Activated
		{
			get
			{
				return isSV10Activated;
			}

			set
			{
				isSV10Activated = value;
			}
		}

		/// <summary>
		/// This property gets/sets SV 11 Activated value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsSV11Activated
		{
			get
			{
				return isSV11Activated;
			}

			set
			{
				isSV11Activated = value;
			}
		}

		/// <summary>
		/// This property gets/sets the Fan Level value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public uint FanLevel
		{
			get
			{
				return fanLevel;
			}

			set
			{
				fanLevel = value;
			}
		}

		/// <summary>
		/// This property gets/sets SV Level 10 value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public uint SvLevel10
		{
			get
			{
				return svLevel10;
			}

			set
			{
				svLevel10 = value;
			}
		}

		/// <summary>
		/// This property gets/sets SV Level 11 value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public uint SvLevel11
		{
			get
			{
				return svLevel11;
			}

			set
			{
				svLevel11 = value;
			}
		}

		/// <summary>
		/// This property gets/sets the Enable or Disable Maintenance Mode value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool EnableOrDisableMaintenanceMode
		{
			get
			{
				return CommonViewModel.Current.Console.GUIInMaintenanceMode;
			}
			set
			{
				CommonViewModel.Current.Console.GUIInMaintenanceMode = value;
				RaisePropertyChanged("EnableOrDisableMaintenanceMode");
			}
		}

		/// <summary>
		/// This property gets/sets the Selected Register value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public ComboBoxItem SelectedRegister
		{
			get
			{
				return selectedRegister;
			}

			set
			{
				selectedRegister = value;
			}
		}

		/// <summary>
		/// This property gets/sets the Patient Selected Register value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public ComboBoxItem PateintSelectedRegister
		{
			get
			{
				return pateintSelectedRegister;
			}

			set
			{
				pateintSelectedRegister = value;
			}
		}

		/// <summary>
		/// This property gets/sets the Reading From CMCU value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsReadingFromCMCU
		{
			get
			{
				return isReadingFromCMCU;
			}

			set
			{
				isReadingFromCMCU = value;
				RaisePropertyChanged("IsReadingFromCMCU");
			}
		}

		/// <summary>
		/// This property gets/sets the Catheter Connected And In Ready State value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsCatheterConnectedAndInIReadyState
		{
			get
			{
				return (localCommonViewModel.SystemState == MessageStateId.CAN_ID_STATE_READY && CommonViewModel.Current.IsCatheterValid);
			}
			set
			{
				RaisePropertyChanged("IsCatheterConnectedAndInIReadyState");
			}
		}

		/// <summary>
		/// This property gets/sets the Catheter Electrically Connected And In Idle State value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsCatheterElectricallyConnectedAndInIdleState
		{
			get
			{
				return (IsCatheterCableConnected &&
						localCommonViewModel.SystemState == MessageStateId.CAN_ID_STATE_IDLE && CommonViewModel.Current.IsCatheterValid
						&& CommonViewModel.Current.IsCMCUReady && CommonViewModel.Current.IsPMCUReady);
			}
			set
			{
				RaisePropertyChanged("IsCatheterElectricallyConnectedAndInIdleState");
			}
		}

		/// <summary>
		/// This property gets/sets the Serial Number value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public int SerilaNumber
		{
			get
			{
				return serilaNumber;
			}

			set
			{
				SetProperty(ref serilaNumber, value);
			}
		}

		/// <summary>
		/// This property gets/sets the Lot Number value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public int LotNumber
		{
			get
			{
				return lotNumber;
			}

			set
			{
				SetProperty(ref lotNumber, value);
			}
		}

		/// Gets/sets the value indicating whether catheter is connecting or not
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool CatheterIsConnecting
		{
			get
			{
				return catheterIsConnecting;
			}

			set
			{
				if (value != catheterIsConnecting)
				{
					catheterIsConnecting = value;
					RaisePropertyChanged("CatheterIsConnecting");
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether is lock the foot switch or not
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool LockTheFootSwitch
		{
			get
			{
				return CommonViewModel.Current.LockTheFootSwitch;
			}

			set
			{
				CommonViewModel.Current.LockTheFootSwitch = value;
				RaisePropertyChanged("LockTheFootSwitch");
			}
		}

		/// <summary>
		/// Set LockTheFootSwitch parameter value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		private void OnLockTheFootSwitchCommand(object arg)
		{
			IsCommandEnabled = false;
			string parameter = arg?.ToString();
			LockTheFootSwitch = parameter == "LockTheFootSwitch";
			IsCommandEnabled = true;
		}
	}
}