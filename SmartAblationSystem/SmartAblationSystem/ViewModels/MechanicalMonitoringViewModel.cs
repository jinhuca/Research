using Prism.Mvvm;
using System.Windows.Input;
using Prism.Commands;
using SmartAblationSystem.Helpers;
using static Communication.CanBusMessageDefinition;

namespace SmartAblationSystem.ViewModels
{
    /// <summary>
    /// This class is the Mechanical Monitoring View Model
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    internal class MechanicalMonitoringViewModel : BindableBase
    {
        private bool isCatheterConnected = false;
        private bool isCatheterCableConnected = false;
        private bool isCatheterTubeConnected = false;
        public ICommand ConnectCommand { get; private set; }
        public ICommand StartCommand { get; private set; }
        public ICommand StopCommand { get; private set; }
        public ICommand FaultResetCommand { get; private set; }
        public ICommand IncreaseTimeCommand { get; private set; }
        public ICommand DecreaseTimeCommand { get; private set; }
        public ICommand SlowButtonCommand { get; private set; }
        public ICommand FastButtonCommand { get; private set; }

        /// <summary>
        /// This constructor initializes the Mechanical Monitoring View Model's commands
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public MechanicalMonitoringViewModel()
        {
            CommonViewModel.Current.PropertyChanged += Current_PropertyChanged;

            this.IncreaseTimeCommand = new DelegateCommand<object>(this.OnIncreaseTimeCommand, this.CanIncreaseTimeCommand);
            this.DecreaseTimeCommand = new DelegateCommand<object>(this.OnDecreaseTimeCommand, this.CanDecreaseTimeCommand);
            this.ConnectCommand = new DelegateCommand<object>(this.OnConnectCommand, this.CanConnectCommand);
            this.StartCommand = new DelegateCommand<object>(this.OnStartCommand, this.CanStartCommand);
            this.StopCommand = new DelegateCommand<object>(this.OnStopCommand, this.CanStopCommand);
            this.FaultResetCommand = new DelegateCommand<object>(this.OnFaultResetCommand, this.CanFaultResetCommand);
            this.FastButtonCommand = new DelegateCommand<object>(OnFastButtonCommand, (obj) => true);
            this.SlowButtonCommand = new DelegateCommand<object>(OnSlowButtonCommand, (obj) => true);
        }

        /// <summary>
        /// This property gets/sets the Required Ablation Time value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int RequiredAblationTime
        {
            get
            {
                return CommonViewModel.Current.RequiredAblationTime;
            }

            set
            {
                CommonViewModel.Current.RequiredAblationTime = value;
                RaisePropertyChanged("RequiredAblationTime");
            }
        }

        public Enumeration.InflationSpeedMode InflationSpeedMode => CommonViewModel.Current.Console.EnableFastInflationMode
          ? Enumeration.InflationSpeedMode.Fast
          : Enumeration.InflationSpeedMode.Slow;

        public void RefreshInflationSpeedMode()
        {
          RaisePropertyChanged(nameof(InflationSpeedMode));
        }

    /// <summary>
    /// Function/Command that handles the Required Ablation Time incrementation when the
    /// Increase Time command is invoked
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="arg">The command's parameter (not used in this function).</param>
    private void OnIncreaseTimeCommand(object arg)
        {
            RequiredAblationTime += 30;
        }

        /// <summary>
        /// Function that returns if the system can invoke the Increase Time command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command parameter (not used in this function).</param>
        /// <returns>Boolean value if the system can invoke the command.</returns>
        private bool CanIncreaseTimeCommand(object arg)
        {
            return true;
        }

        /// <summary>
        /// Function/Command that handles the Required Ablation Time decrementation when the
        /// Decrease Time command is invoked
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command's parameter (not used in this function).</param>
        private void OnDecreaseTimeCommand(object arg)
        {
            RequiredAblationTime -= 30;
        }

        /// <summary>
        /// Function that returns if the system can invoke the Decrease Time command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command parameter (not used in this function).</param>
        /// <returns>Boolean value if the system can invoke the command.</returns>
        private bool CanDecreaseTimeCommand(object arg)
        {
            return true;
        }

        /// <summary>
        /// This property gets/sets PT1 Reading value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PT1Reading
        {
            get
            {
                return CommonViewModel.Current.PT1Reading;
            }

            set
            {
                CommonViewModel.Current.PT1Reading = value;
                RaisePropertyChanged("PT1Reading");
            }
        }

        /// <summary>
        /// This property gets/sets PT2 Reading value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PT2Reading
        {
            get
            {
                return CommonViewModel.Current.PT2Reading;
            }

            set
            {
                CommonViewModel.Current.PT2Reading = value;
                RaisePropertyChanged("PT2Reading");
            }
        }

        /// <summary>
        /// This property gets/sets PT3 Reading value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PT3Reading
        {
            get
            {
                return CommonViewModel.Current.PT3Reading;
            }

            set
            {
                CommonViewModel.Current.PT3Reading = value;
                RaisePropertyChanged("PT3Reading");
            }
        }

        /// <summary>
        /// This property gets/sets PT4 Reading value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PT4Reading
        {
            get
            {
                return CommonViewModel.Current.PT4Reading;
            }

            set
            {
                CommonViewModel.Current.PT4Reading = value;
                RaisePropertyChanged("PT4Reading");
            }
        }

        /// <summary>
        /// This property gets/sets PT5 Reading value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PT5Reading
        {
            get
            {
                return CommonViewModel.Current.PT5Reading;
            }

            set
            {
                CommonViewModel.Current.PT5Reading = value;
                RaisePropertyChanged("PT5Reading");
            }
        }

        /// <summary>
        /// This property gets/sets PS1 Reading value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PS1Reading
        {
            get
            {
                return CommonViewModel.Current.PS1Reading;
            }

            set
            {
                CommonViewModel.Current.PS1Reading = value;
                RaisePropertyChanged("PS1Reading");
            }
        }

        /// <summary>
        /// This property gets/sets PS2 Reading value.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PS2Reading
        {
            get
            {
                return CommonViewModel.Current.PS2Reading;
            }

            set
            {
                CommonViewModel.Current.PS2Reading = value;
                RaisePropertyChanged("PS2Reading");
            }
        }

        /// <summary>
        /// This property gets/sets FM1 Reading value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double FM1Reading
        {
            get
            {
                return CommonViewModel.Current.FM1Reading;
            }

            set
            {
                CommonViewModel.Current.FM1Reading = value;
                RaisePropertyChanged("FM1Reading");
            }
        }

        /// <summary>
        /// This property gets/sets TS1 Reading value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double TS1Reading
        {
            get
            {
                return CommonViewModel.Current.TS1Reading;
            }

            set
            {
                CommonViewModel.Current.TS1Reading = value;
                RaisePropertyChanged("TS1Reading");
            }
        }

        /// <summary>
        /// This property gets/sets TN2O Reading value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double TN2OReading
        {
            get
            {
                return CommonViewModel.Current.TN2OReading;
            }

            set
            {
                CommonViewModel.Current.TN2OReading = value;
                RaisePropertyChanged("TN2OReading");
            }
        }

        /// <summary>
        /// This property gets/sets CMCU CJ Reading value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double CMCUCJReading
        {
            get
            {
                return CommonViewModel.Current.CMCUCJReading;
            }

            set
            {
                CommonViewModel.Current.CMCUCJReading = value;
                RaisePropertyChanged("CMCUCJReading");
            }
        }

        /// <summary>
        /// This property gets/sets LC1 Reading value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double LC1Reading
        {
            get
            {
                return CommonViewModel.Current.LC1Reading;
            }

            set
            {
                CommonViewModel.Current.LC1Reading = value;
                RaisePropertyChanged("LC1Reading");
            }
        }

        /// <summary>
        /// This property gets/sets TC1 Reading value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double TC1Reading
        {
            get
            {
                return CommonViewModel.Current.TC1Reading;
            }

            set
            {
                CommonViewModel.Current.TC1Reading = value;
                RaisePropertyChanged("TC1Reading");
            }
        }

        /// <summary>
        /// This property gets/sets TC2 Reading value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double TC2Reading
        {
            get
            {
                return CommonViewModel.Current.TC2Reading;
            }

            set
            {
                CommonViewModel.Current.TC2Reading = value;
                RaisePropertyChanged("TC2Reading");
            }
        }

        /// <summary>
        /// This property gets/sets System State value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public MessageStateId SystemState
        {
            get
            {
                return CommonViewModel.Current.SystemState;
            }
            set
            {
                RaisePropertyChanged("SystemState");
            }
        }

        /// <summary>
        /// This property gets/sets the Catheter Connected flag
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsCatheterConnected
        {
            get
            {
                return CommonViewModel.Current.IsCatheterConnected;
            }

            set
            {
                SetProperty(ref this.isCatheterConnected, value);
            }
        }

        /// <summary>
        /// This property gets/sets the Catheter Cable Connected flag
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsCatheterCableConnected
        {
            get
            {
                return CommonViewModel.Current.IsCatheterCableConnected;
            }

            set
            {
                SetProperty(ref this.isCatheterCableConnected, value);
                RaisePropertyChanged("IsCatheterConnected");  //Not sure if needed any more
                RaisePropertyChanged("IsCatheterCableConnected");
            }
        }

        /// <summary>
        /// This property gets/sets the Catheter Tube Connected flag
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsCatheterTubeConnected
        {
            get
            {
                return CommonViewModel.Current.IsCatheterTubeConnected;
            }
            set
            {
                SetProperty(ref this.isCatheterTubeConnected, value);
                RaisePropertyChanged("IsCatheterTubeConnected");
            }
        }

        /// <summary>
        /// This property gets/sets the Catheter Electrically Connected and In Idle State flag
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsCatheterElectricallyConnectedAndInIdleState
        {
            get
            {
                return (IsCatheterCableConnected &&
                        CommonViewModel.Current.SystemState == Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE && CommonViewModel.Current.IsCatheterValid
                        && CommonViewModel.Current.IsCMCUReady && CommonViewModel.Current.IsPMCUReady);
            }
            set
            {
                RaisePropertyChanged("IsCatheterElectricallyConnectedAndInIdleState");
            }
        }

        /// <summary>
        /// This property gets/sets the Catheter Connected And In Ready State flag
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsCatheterConnectedAndInIReadyState
        {
            get
            {
                return (CommonViewModel.Current.SystemState == Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY && CommonViewModel.Current.IsCatheterValid);
            }
            set
            {
                RaisePropertyChanged("IsCatheterConnectedAndInIReadyState");
            }
        }

        /// <summary>
        /// This property gets/sets the PMCUCJ value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double PMCUCJReading
        {
            get
            {
                return CommonViewModel.Current.PMCUCJReading;
            }

            set
            {
                CommonViewModel.Current.PMCUCJReading = value;
                RaisePropertyChanged("PMCUCJReading");
            }
        }

        /// <summary>
        /// This property gets/sets the CP1 reading value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double CP1Reading
        {
            get
            {
                return CommonViewModel.Current.CP1Reading;
            }

            set
            {
                CommonViewModel.Current.CP1Reading = value;
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
                return CommonViewModel.Current.CP2Reading;
            }

            set
            {
                CommonViewModel.Current.CP2Reading = value;
                RaisePropertyChanged("CP2Reading");
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
                return CommonViewModel.Current.PIDDutyCycle;
            }

            set
            {
                CommonViewModel.Current.PIDDutyCycle = value;
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
                return CommonViewModel.Current.PatientPIDDutyCycle;
            }

            set
            {
                CommonViewModel.Current.PatientPIDDutyCycle = value;
                RaisePropertyChanged("PatientPIDDutyCycle");
            }
        }

        /// <summary>
        /// Function that activates the catheter when conditions are met
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void ActivateCatheterIfConditionsApply()
        {
            RaisePropertyChanged("IsCatheterElectricallyConnectedAndInIdleState");

            if (CommonViewModel.Current.IsCatheterCableConnected && CommonViewModel.Current.IsCatheterTubeConnected)
            {
                IsCatheterConnected = true;
                // Here we activate the ouputs
            }
        }

        /// <summary>
        /// Function/Command that handles the console connection/disconnection when Connect
        /// command is invoked
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="OnConnectCommand">The command's parameter (not used in this function).</param>
        private void OnConnectCommand(object OnConnectCommand)
        {
            if (CommonViewModel.Current.SystemState == Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY)
            {
                CommonViewModel.Current.Console.Disconnect();
            }
            else if (IsCatheterCableConnected)
            {
                CommonViewModel.Current.Console.Connect();
            }
        }

        /// <summary>
        /// Function that returns if the system can invoke the Connect command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command parameter (not used in this function).</param>
        /// <returns>Boolean value if the system can invoke the command.</returns>
        private bool CanConnectCommand(object arg)
        {
            return true;
        }

        /// <summary>
        /// Function/Command that handles the console start when the Start command is invoked
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command's parameter (not used in this function).</param>
        private void OnStartCommand(object arg)
        {
            CommonViewModel.Current.Console.Start();
        }

        /// <summary>
        /// Function that returns if the system can invoke the Start command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command parameter (not used in this function).</param>
        /// <returns>Boolean value if the system can invoke the command.</returns>
        private bool CanStartCommand(object arg)
        {
            return true;
        }

        /// <summary>
        /// Function that returns if the system can invoke the Stop command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command parameter (not used in this function).</param>
        /// <returns>Boolean value if the system can invoke the command.</returns>
        private bool CanStopCommand(object arg)
        {
            return true;
        }

        /// <summary>
        /// Function/Command that handles the console stop when the Stop command is invoked
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command's parameter (not used in this function).</param>
        private void OnStopCommand(object arg)
        {
            CommonViewModel.Current.Console.Stop();
        }

        /// <summary>
        /// Function/Command that handles the console reset when the Fault Reset command is invoked
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command's parameter (not used in this function).</param>
        private void OnFaultResetCommand(object arg)
        {
            CommonViewModel.Current.Console.FailResetEnable();
            System.Threading.Thread.Sleep(10);
            CommonViewModel.Current.Console.FailResetDisable();
        }

        /// <summary>
        /// Function that returns if the system can invoke the Fault Reset command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command parameter (not used in this function).</param>
        /// <returns>Boolean value if the system can invoke the command.</returns>
        private bool CanFaultResetCommand(object arg)
        {
            return true;
        }

        private void OnSlowButtonCommand(object arg) 
        {
          CommonViewModel.Current.Console.EnableFastInflationMode = false;
          RefreshInflationSpeedMode();
        }

        private void OnFastButtonCommand(object arg)
        {
          CommonViewModel.Current.Console.EnableFastInflationMode = true;
          RefreshInflationSpeedMode();
        }

        /// <summary>
        /// This property gets/sets the SV1 Activated value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsSV1Activated
        {
            get
            {
                return CommonViewModel.Current.IsSolenoidValve1ON;
            }
        }

        /// <summary>
        /// This property gets/sets the SV2 Activated value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsSV2Activated
        {
            get
            {
                return !CommonViewModel.Current.IsSolenoidValve2ON;
            }
        }

        /// <summary>
        /// This property gets/sets the SV3 Activated value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsSV3Activated
        {
            get
            {
                return CommonViewModel.Current.IsSolenoidValve3ON;
            }
        }

        /// <summary>
        /// This property gets/sets the SV4 Activated value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsSV4Activated
        {
            get
            {
                return !CommonViewModel.Current.IsSolenoidValve4ON;
            }
        }

        /// <summary>
        /// This property gets/sets the SV5 Activated value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsSV5Activated
        {
            get
            {
                return !CommonViewModel.Current.IsSolenoidValve5ON;
            }
        }

        /// <summary>
        /// This property gets/sets the SV6 Activated value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsSV6Activated
        {
            get
            {
                return !CommonViewModel.Current.IsSolenoidValve6ON;
            }
        }

        /// <summary>
        /// This property gets/sets the SV7 Activated value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsSV7Activated
        {
            get
            {
                return CommonViewModel.Current.IsSolenoidValve7ON;
            }
        }

        /// <summary>
        /// This property gets/sets the SV8 Activated value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsSV8Activated
        {
            get
            {
                return CommonViewModel.Current.IsSolenoidValve8ON;
            }
        }

        /// <summary>
        /// This property gets/sets the SV9 Activated value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsSV9Activated
        {
            get
            {
                return CommonViewModel.Current.IsSolenoidValve9ON;
            }
        }

        /// <summary>
        /// This property gets/sets the PV1 Activated value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsPV1Activated
        {
            get
            {
                return PatientPIDDutyCycle > 0;
            }
        }

        /// <summary>
        /// This property gets/sets the PV2 Activated value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsPV2Activated
        {
            get
            {
                return PIDDutyCycle > 0;
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

                case "PS1Reading":
                    RaisePropertyChanged("PS1Reading");
                    break;

                case "PS2Reading":
                    RaisePropertyChanged("PS2Reading");
                    break;

                case "FM1Reading":
                    RaisePropertyChanged("FM1Reading");
                    break;

                case "TS1Reading":
                    RaisePropertyChanged("TS1Reading");
                    break;

                case "TN2OReading":
                    RaisePropertyChanged("TN2OReading");
                    break;

                case "LC1Reading":
                    RaisePropertyChanged("LC1Reading");
                    break;

                case "TC1Reading":
                    RaisePropertyChanged("TC1Reading");
                    break;

                case "TC2Reading":
                    RaisePropertyChanged("TC2Reading");
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

                case "SystemState":
                    RaisePropertyChanged("SystemState");

                    //Always trigger to make sure the VACUUM OFF button gets visible/hidden
                    //There is no System State logic in the mechanical panel (like in CryoTherapyViewModel) to trigger it.
                    RaisePropertyChanged("IsCatheterConnectedAndInIReadyState");
                    break;

                case "IsCatheterCableConnected":
                    ActivateCatheterIfConditionsApply();
                    break;

                case "IsCatheterTubeConnected":
                    ActivateCatheterIfConditionsApply();
                    break;

                case "PIDDutyCycle":
                    RaisePropertyChanged("PIDDutyCycle");
                    RaisePropertyChanged("IsPV2Activated");
                    break;

                case "PatientPIDDutyCycle":
                    RaisePropertyChanged("PatientPIDDutyCycle");
                    RaisePropertyChanged("IsPV1Activated");
                    break;

                case "RequiredAblationTime":
                    RaisePropertyChanged("RequiredAblationTime");
                    break;

                case "IsSolenoidValve1ON":
                    RaisePropertyChanged("IsSV1Activated");
                    break;

                case "IsSolenoidValve2ON":
                    RaisePropertyChanged("IsSV2Activated");
                    break;

                case "IsSolenoidValve3ON":
                    RaisePropertyChanged("IsSV3Activated");
                    break;

                case "IsSolenoidValve4ON":
                    RaisePropertyChanged("IsSV4Activated");
                    break;

                case "IsSolenoidValve5ON":
                    RaisePropertyChanged("IsSV5Activated");
                    break;

                case "IsSolenoidValve6ON":
                    RaisePropertyChanged("IsSV6Activated");
                    break;

                case "IsSolenoidValve7ON":
                    RaisePropertyChanged("IsSV7Activated");
                    break;

                case "IsSolenoidValve8ON":
                    RaisePropertyChanged("IsSV8Activated");
                    break;

                case "IsSolenoidValve9ON":
                    RaisePropertyChanged("IsSV9Activated");
                    break;
                /* Not used 
                case "IsPV1Activated":
                    RaisePropertyChanged("IsPV1Activated");
                    break;

                case "IsPV2Activated":
                    RaisePropertyChanged("IsPV2Activated");
                    break;

                */

                #region catheter ready

                case "IsCMCUReady":
                case "IsPMCUReady":
                    ActivateCatheterIfConditionsApply();
                    break;

                case "IsCatheterValid":
                    ActivateCatheterIfConditionsApply();
                    break;

                    #endregion
            }
        }
    }
}