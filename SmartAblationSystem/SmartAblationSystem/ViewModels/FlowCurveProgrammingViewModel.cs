using Communication;
using Console;
using DataAccessLayer;
using FileSerializer;
using MicroLibrary;
using Prism.Mvvm;
using SmartAblationSystem.Helpers;
using SmartAblationSystem.Models;
using SmartAblationSystem.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Management;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using Prism.Commands;
using static Communication.CanBusMessageDefinition;

namespace SmartAblationSystem.ViewModels
{
    /// <summary>
    /// This class is the Flow Curve Programming View Model
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class FlowCurveProgrammingViewModel : BindableBase
    {
        #region Command
        public ICommand WriteCommand { get; private set; }
        public ICommand ReadCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand IncrementSystemStateCommand { get; private set; }
        public ICommand IncreaseTimeCommand { get; private set; }
        public ICommand DecreaseTimeCommand { get; private set; }
        public ICommand ConnectCommand { get; private set; }
        public ICommand StartCommand { get; private set; }
        public ICommand StopCommand { get; private set; }

        public ICommand FlowCurveQuadraticFormulaDocumentCommand { get; private set; }

        #endregion

        public event EventHandler<EventArgs> PT2PT3FM1IbPEvent;

        private DispatcherTimer timer = new DispatcherTimer();
        private MicroTimer loggingTimer = new MicroTimer();
        private MicroTimer ablationTimer = new MicroTimer();

        private string selectedState = string.Empty;

        private bool isCatheterConnected = false;
        private bool isCatheterTubeConnected = false;
        private bool isProgrammingFlow = false;

        private int cryoTherapyTime;
        private bool cryoTherapyTimeVisible;
        private short refrigerantLevelUnit = 0;
        private bool isStatusAbllationBallonVisible = false;
        private N2OFlowCalculator n2OFlowCalculator;


        private DataAccess dataAccess;

        private CommonViewModel localCommonViewModel = CommonViewModel.Current;

        private MessageStateId PreviousSystemState = MessageStateId.CAN_ID_STATE_UNKNOWN;
        private MessageStateId PreviousSystemStateForExpectedFlow = MessageStateId.CAN_ID_STATE_UNKNOWN;

        private bool catheterIsConnecting = false;

        private Helpers.Enumeration.TankWeight gasState = Helpers.Enumeration.TankWeight.THE_TANK_WEIGHT_IS_IN_BOUNDS;

        #region Const
        private const int Fm1LowOffsetID = 27;
        private const int Fm1LowFitAndFm1LowCeilingID = 28;
        private const int maxWritingTime = 8;

        #endregion

        /// <summary>
        /// This constructor initializes the PIDs View Model's properties, commands and data access
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public FlowCurveProgrammingViewModel()
        {
            localCommonViewModel.PropertyChanged += Current_PropertyChanged;

            this.WriteCommand = new DelegateCommand<object>(this.OnWriteCommand, this.CanWriteCommand);
            this.ReadCommand = new DelegateCommand<object>(this.OnReadCommand, this.CanReadCommand);
            this.SaveCommand = new DelegateCommand<object>(this.OnSaveCommand, this.CanSaveCommand);
            this.ConnectCommand = new DelegateCommand<object>(this.OnConnectCommand, this.CanConnectCommand);
            this.StartCommand = new DelegateCommand<object>(this.OnStartCommand, this.CanStartCommand);
            this.StopCommand = new DelegateCommand<object>(this.OnStopCommand, this.CanStopCommand);

            this.IncreaseTimeCommand = new DelegateCommand<object>(this.OnIncreaseTimeCommand, this.CanIncreaseTimeCommand);
            this.DecreaseTimeCommand = new DelegateCommand<object>(this.OnDecreaseTimeCommand, this.CanDecreaseTimeCommand);
            this.FlowCurveQuadraticFormulaDocumentCommand = new DelegateCommand<object>(this.OnFlowCurveQuadraticFormulaDocumentCommand, this.CanFlowCurveQuadraticFormulaDocumentCommand);

            this.IncrementSystemStateCommand = new DelegateCommand<object>(this.OnIncrementSystemStateCommand, this.CanIncrementSystemStateCommand);

            this.dataAccess = CommonViewModel.Current.Data.DataAccess;

            n2OFlowCalculator = new N2OFlowCalculator();

            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();

            loggingTimer.Interval = 5000000; // we are using 5000ms inteval
            loggingTimer.MicroTimerElapsed += new MicroLibrary.MicroTimer.MicroTimerElapsedEventHandler(loggingTimer_tick);
            loggingTimer.Stop();

            ablationTimer.Interval = 1000000;
            ablationTimer.MicroTimerElapsed += new MicroLibrary.MicroTimer.MicroTimerElapsedEventHandler(ablationTimer_tick);
            ablationTimer.Stop();
        }

        /// <summary>
        /// Returns if  can invoke flow curve quadratic formula document command.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private bool CanFlowCurveQuadraticFormulaDocumentCommand(object arg)
        {
            return true;
        }

        /// <summary>
        /// show documentviewer window dialog
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void OnFlowCurveQuadraticFormulaDocumentCommand(object obj)
        {
            DocumentViewerWindow document = new DocumentViewerWindow("N2O FLOW CURVE MODEL.xps", "N2O FLOW CURVE MODEL");
            document.ShowDialog();
        }

        /// <summary>
        /// This property gets/sets the Cryotherapy time value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int CryoTherapyTime
        {
            get
            {
                return cryoTherapyTime;
            }

            set
            {
                cryoTherapyTime = value;
                RaisePropertyChanged("CryoTherapyTime");
            }
        }

        /// <summary>
        /// This property gets/sets the Relative error value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double RelativeError
        {
            get
            {
                // User need only the diffrence:
                return Math.Abs((FM1Reading - ExpectedFlow));

                //return FM1Reading != 0 ? Math.Abs((FM1Reading - ExpectedFlow) / FM1Reading * 100 ) : 0;
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
        /// This property gets/sets the Is Status Ablation Balloon visible value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsStatusAbllationBallonVisible
        {
            get
            {
                return isStatusAbllationBallonVisible;
            }

            set
            {
      
                isStatusAbllationBallonVisible = value;
                RaisePropertyChanged("IsStatusAbllationBallonVisible");
            }
        }

        /// <summary>
        /// This property gets/sets the Cryotherapy time visible value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool CryoTherapyTimeVisible
        {
            get
            {
                return cryoTherapyTimeVisible;
            }

            set
            {
                cryoTherapyTimeVisible = value;
                RaisePropertyChanged("CryoTherapyTimeVisible");
            }
        }

        /// <summary>
        /// This property gets/sets the Gas State value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public Helpers.Enumeration.TankWeight GasState
        {
            get
            {
                return CommonViewModel.Current.GasState; ;
            }
            set
            {
                gasState = value;
                RaisePropertyChanged("GasState");
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
                return CommonViewModel.Current.LC1Reading;
            }
            set
            {
                CommonViewModel.Current.LC1Reading = value;
                RaisePropertyChanged("LC1Reading");
            }
        }

        public bool DASBalloonEnabled { get; set; } = false;

        public bool IsLowFlowActivated { get; set; } = false;

        /// <summary>
        /// This property gets/sets the Refrigerant Level Unit value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public short RefrigerantLevelUnit
        {
            get
            {
                return refrigerantLevelUnit;
            }
            set
            {
                this.refrigerantLevelUnit = value;
                RaisePropertyChanged("RefrigerantLevelUnit");

                //Here we want to get the new reading to make the conversion
                RaisePropertyChanged("LC1Reading");
            }
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
        /// This function invoke the PT2/PT3/FM1/Ibp event when PT2/PT3/FM1/IBP change has been trigerred from
        /// an external source
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">The View Model that sent the event.</param>
        /// <param name="e">The parameter's name that has changed.</param>
        protected virtual void OnPT2PT3FM1IbPChanged(object sender, EcgEventArgs e)
        {
            PT2PT3FM1IbPEvent?.Invoke(sender, e);
        }

        /// <summary>
        /// This function is invoked by the timer at each tick and triggers PT2/PT3/FM1/IBP event change
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">The View Model that sent the event.</param>
        /// <param name="e">The event arguments.</param>
        private void timer_Tick(object sender, EventArgs e)
        {
            //Dictionary<MessageStateId, FlowMeterOne> _FlowMeterOneValueAccordingToTheStateMachine = localCommonViewModel.Console.FlowMeterOneValueAccordingToTheStateMachine;
            //MessageStateId _SystemState = localCommonViewModel.SystemState;

            //if(_SystemState != MessageStateId.CAN_ID_STATE_UNKNOWN && _SystemState != MessageStateId.CAN_ID_STATE_EXCEPTION)
            //N2OFlowCalculator.ExpectedFlow(_SystemState, _FlowMeterOneValueAccordingToTheStateMachine[_SystemState].FlowMeterLowRangeLimit,
            //                                             _FlowMeterOneValueAccordingToTheStateMachine[_SystemState].FlowMeterHighRangelimit,
            //                                             _FlowMeterOneValueAccordingToTheStateMachine[_SystemState].FlowMeterThresholLowlimit);




            OnPT2PT3FM1IbPChanged(CommonViewModel.Current, null);
        }

        /// <summary>
        /// This function is invoked by the loggingTimer.  It stops the timer and adds current data to the database
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">The View Model that sent the event (not used in this function).</param>
        /// <param name="e">The event arguments.</param>
        private void loggingTimer_tick(object sender, EventArgs e)
        {
            try
            {
                loggingTimer.Stop();

                //this.dataAccess.AddCMCUPIDLoga(FM1Reading, PT2Reading, TargetInjectionFlow, TargetInjectionPressure, TargetInjectionFlow - FM1Reading
                //    , TargetInjectionPressure - PT2Reading, PatientPGain, PatientIGain, PatientDGain, PatientPIDOffset);

                //this.dataAccess.AddPMCUPIDLoga(PT3Reading, CP1Reading, CP2Reading, PIDDutyCycle, PatientPIDDutyCycle, TargetBalloonPressure,
                //    TargetBalloonPressure - CP1Reading, PGain, IGain, DGain, PIDOffset);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        /// <summary>
        /// This function is invoked by the ablationTimer
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">The View Model that sent the event (not used in this function).</param>
        /// <param name="e">The event arguments.</param>
        private void ablationTimer_tick(object sender, EventArgs e)
        {
            try
            {
                CryoTherapyTime++;

                if (this.CryoTherapyTime == RequiredAblationTime &&
                    CommonViewModel.Current.SystemState != Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING)
                {
                    //Stop the ablation procedure, but keep the ablation timer running.
                    StopAblationProcedure();
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }


        /// <summary>
        /// Stop ablation timer
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public void StopAblationTimer()
        {
            ablationTimer.Stop();
        }


        /// <summary>
        /// This read-only property returns the system's States List
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public List<string> StatesList
        {
            get
            {
                List<string> convertedState = new List<string>();
                Communication.CanBusMessageDefinition.MessageStateId[] states = (Communication.CanBusMessageDefinition.MessageStateId[])Enum.GetValues(typeof(Communication.CanBusMessageDefinition.MessageStateId));

                foreach (Communication.CanBusMessageDefinition.MessageStateId element in states)
                {
                    convertedState.Add(element.ToString().Replace("CAN_ID_STATE_", string.Empty));
                }
                return convertedState;
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
                localCommonViewModel.Console.InjectionFlowValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].TargetInjectionFlow = localCommonViewModel.TargetInjectionFlow;
                return localCommonViewModel.TargetInjectionFlow;
            }

            set
            {
                try
                {
                    localCommonViewModel.TargetInjectionFlow = value;
                    localCommonViewModel.Console.InjectionFlowValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].TargetInjectionFlow = value;
                    RaisePropertyChanged("TargetInjectionFlow");
                }
                catch { }
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
                try
                {
                    localCommonViewModel.TargetBalloonPressure = value;
                    localCommonViewModel.Console.PatientMicroControllerBalloonPressureRegulatorValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].TargetBalloonPressure = value;
                    RaisePropertyChanged("TargetBalloonPressure");
                }
                catch { }
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
                localCommonViewModel.Console.InjectionPressureValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].TargetInjectionPressure = localCommonViewModel.TargetInjectionPressure;
                return localCommonViewModel.TargetInjectionPressure;
            }

            set
            {
                try
                {
                    localCommonViewModel.TargetInjectionPressure = value;
                    localCommonViewModel.Console.InjectionPressureValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].TargetInjectionPressure = value;
                    RaisePropertyChanged("TargetInjectionPressure");
                }
                catch { }
            }
        }

        /// <summary>
        /// This property gets/sets the system's state value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public MessageStateId SystemState
        {
            get
            {
                CryoTherapyTimeVisible = false;

                if (CommonViewModel.Current.SystemState == Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY)
                {
                    IsCatheterConnectedAndInIReadyState = true;
                }
                else
                {
                    IsCatheterConnectedAndInIReadyState = false;
                }

                //Only display the ablation timer when in Ablation
                if (CommonViewModel.Current.SystemState == Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_ABLATION ||
                    CommonViewModel.Current.SystemState == Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION)
                {
                    CryoTherapyTimeVisible = true;
                }
                else
                {
                    CryoTherapyTimeVisible = false;
                }

                //Start the Ablation Timer when the system state falls in Ablation
                if (PreviousSystemState != Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION &&
                    CommonViewModel.Current.SystemState == Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION)
                {
                    //Start the Ablation Timer
                    CryoTherapyTime = 0;
                    ablationTimer.Start();
                }
                else if (
                         CommonViewModel.Current.SystemState == Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_INFLATION ||
                         CommonViewModel.Current.SystemState == Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE ||
                         CommonViewModel.Current.SystemState == Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY)
                {
                    //When not in ablation, stop the ablation timer and reset the cryotherapy time.
                    if (ablationTimer.Enabled)
                    {
                        ablationTimer.Stop();
                    }

                    CryoTherapyTime = 0;
                }
                else if (CommonViewModel.Current.SystemState == MessageStateId.CAN_ID_STATE_TRANSITION && PreviousSystemState != MessageStateId.CAN_ID_STATE_TRANSITION)
                {
                    loggingTimer.Start();
                }

                PreviousSystemState = localCommonViewModel.SystemState;

                return localCommonViewModel.SystemState;
            }

            set
            {
                localCommonViewModel.SystemState = value;
                RaisePropertyChanged("SystemState");
            }
        }

        /// <summary>
        /// This function manages the Procedure Stop (console and flags update)
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void StopAblationProcedure()
        {
            CommonViewModel.Current.IsAblationProcedureStarted = false;
            CommonViewModel.Current.IsAblationProcedureEnded = true;
            CommonViewModel.Current.Console.Stop();
        }

        /// <summary>
        /// This function activates the catheter if the conditions are verified
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
                // Here we activate the ouputs
                //localCommonViewModel.Console.IinjectionEnable();
                //localCommonViewModel.Console.VacuumEnable();
            }
        }

        /// <summary>
        /// This property gets/sets Catheter connected flag
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
                SetProperty(ref this.isCatheterConnected, value);
            }
        }

        /// <summary>
        /// This property gets/sets Catheter cable connected flag
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

            set
            {
                // SetProperty(ref this.isCatheterCableConnected, value);
                // SetProperty(ref this.catheterCableConnectedStatusImage,  "");
                RaisePropertyChanged("IsCatheterConnected");  //Not sure if needed any more
                RaisePropertyChanged("IsCatheterCableConnected");
            }
        }

        /// <summary>
        /// This property gets/sets Catheter tube connected flag
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsCatheterTubeConnected
        {
            get
            {
                return localCommonViewModel.IsCatheterTubeConnected;
            }
            set
            {
                SetProperty(ref this.isCatheterTubeConnected, value);
                RaisePropertyChanged("IsCatheterTubeConnected");
            }
        }

        /// <summary>
        /// This property gets/sets Catheter electrically connected and in idle state flag
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsCatheterElectricallyConnectedAndInIdleState
        {
            get
            {
                bool isCatheterElectricallyConnectedAndInIdleState = (IsCatheterCableConnected &&
                        localCommonViewModel.SystemState == Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE && CommonViewModel.Current.IsCatheterValid
                        && CommonViewModel.Current.IsCMCUReady && CommonViewModel.Current.IsPMCUReady);

                if (!IsCatheterCableConnected || isCatheterElectricallyConnectedAndInIdleState)
                {
                    //CatheterIsConnecting = false;
                }

                else
                {

                }

                return isCatheterElectricallyConnectedAndInIdleState;
            }
            set
            {
                RaisePropertyChanged("IsCatheterElectricallyConnectedAndInIdleState");
            }
        }

        /// <summary>
        /// Gets/sets a value indicating whether catheter is connecting or not
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
        /// This property gets/sets Catheter connected and in ready state flag
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsCatheterConnectedAndInIReadyState
        {
            get
            {
                return (localCommonViewModel.SystemState == Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY);
            }
            set
            {
                RaisePropertyChanged("IsCatheterConnectedAndInIReadyState");
            }
        }

        /// <summary>
        /// Gets/sets expected flow value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double ExpectedFlow
        {
            get
            {
                Dictionary<MessageStateId, FlowMeterOne> _FlowMeterOneValueAccordingToTheStateMachine = localCommonViewModel.Console.FlowMeterOneValueAccordingToTheStateMachine;
                MessageStateId _SystemState = localCommonViewModel.SystemState;

                if (PreviousSystemStateForExpectedFlow != _SystemState && _SystemState != MessageStateId.CAN_ID_STATE_EXCEPTION && _SystemState != MessageStateId.CAN_ID_STATE_UNKNOWN)
                {
                    PreviousSystemStateForExpectedFlow = _SystemState;
                    OnReadCommand("UpdateThreshold");
                }





                if (_SystemState != MessageStateId.CAN_ID_STATE_UNKNOWN && _SystemState != MessageStateId.CAN_ID_STATE_EXCEPTION)
                    return N2OFlowCalculator.ExpectedFlow(_SystemState, _FlowMeterOneValueAccordingToTheStateMachine[_SystemState].FlowMeterLowRangeLimit,
                                                                  _FlowMeterOneValueAccordingToTheStateMachine[_SystemState].FlowMeterHighRangelimit,
                                                                  _FlowMeterOneValueAccordingToTheStateMachine[_SystemState].FlowMeterThresholLowlimit);
                return 0;

            }
        }

        /// <summary>
        /// This property gets/sets the system's selected state
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
                    //case 0:

                    //    ConsoleFiniteStateMachine.CurrentState = Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_UNKNOWN;

                    case 1:

                        ConsoleFiniteStateMachine.CurrentState = Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE;

                        break;

                    case 2:

                        ConsoleFiniteStateMachine.CurrentState = Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY;

                        break;

                    case 3:

                        ConsoleFiniteStateMachine.CurrentState = Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_INFLATION;

                        break;

                    case 4:

                        ConsoleFiniteStateMachine.CurrentState = Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION;

                        break;

                    case 5:

                        ConsoleFiniteStateMachine.CurrentState = Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_ABLATION;

                        break;

                    case 6:

                        ConsoleFiniteStateMachine.CurrentState = Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING;

                        break;

                    case 7:

                        ConsoleFiniteStateMachine.CurrentState = Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_EXCEPTION;

                        break;
                }
                SetProperty(ref this.selectedState, value);
            }
        }

        /// <summary>
        /// Function that returns if the system can invoke the Write command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command parameter (not used in this function).</param>
        /// <returns>Boolean value if the system can invoke the command.</returns>
        private bool CanWriteCommand(object arg)
        {
            // To do
            return true;
        }

        /// <summary>
        /// Function/Command that handles writing when the command is invoked
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command's parameter.</param>
        private void OnWriteCommand(object arg)
        {
            for (int i = 0; i < maxWritingTime; i++)
            {
                int state = 0;

                state = Array.IndexOf(Enum.GetValues(typeof(MessageStateId)), ConsoleFiniteStateMachine.CurrentState);

                localCommonViewModel.Console.WriteFromMicroController((MessageStateId)state, Fm1LowOffsetID);
                System.Threading.Thread.Sleep(50);
                localCommonViewModel.Console.WriteFromMicroController((MessageStateId)state, Fm1LowFitAndFm1LowCeilingID);
                System.Threading.Thread.Sleep(50);
            }
        }

        /// <summary>
        /// Function/Command that handles connection/disconnection when the Connect
        /// command is invoked
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command's parameter (not used in this function).</param>
        private void OnConnectCommand(object arg)
        {
            if (localCommonViewModel.SystemState == Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY)
            {
                localCommonViewModel.Console.Disconnect();
                //IsCatheterConnected = false;
            }
            else if (IsCatheterCableConnected)
            {
                localCommonViewModel.Console.Connect();
                //IsCatheterConnected = true;
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
            localCommonViewModel.Console.Start();
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
        /// Function/Command that handles the console stop when the Stop command is invoked
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command's parameter (not used in this function).</param>
        private void OnStopCommand(object arg)
        {
            localCommonViewModel.Console.Stop();
        }

        /// <summary>
        /// Function that returns if the system can invoke the Stop command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command parameter (not used in this function)</param>
        /// <returns>Boolean value if the system can invoke the command.</returns>
        private bool CanStopCommand(object arg)
        {
            return true;
        }

        /// <summary>
        /// Function/Command that handles the reading when the command is invoked
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command's parameter (central or patient micro controller).</param>
        private void OnReadCommand(object arg)
        {
            int state = 0;
            MessageStateId _SystemState;

            if (arg?.ToString() == "UpdateThreshold")
            {
                _SystemState = localCommonViewModel.SystemState;
                state = Array.IndexOf(Enum.GetValues(typeof(MessageStateId)), _SystemState);

                for (int i = 0; i < maxWritingTime; i++)
                {
                    for (int id = 27; id < 29; id++)
                    {
                        localCommonViewModel.Console.ReadFromMicroController((MessageStateId)state, id);
                        System.Threading.Thread.Sleep(20);
                    }
                }
            }

            else
            {

                state = Array.IndexOf(Enum.GetValues(typeof(MessageStateId)), ConsoleFiniteStateMachine.CurrentState);

                for (int i = 0; i < maxWritingTime; i++)
                {
                    for (int id = 27; id < 29; id++)
                    {
                        localCommonViewModel.Console.ReadFromMicroController((MessageStateId)state, id);
                        System.Threading.Thread.Sleep(20);
                    }
                }
            }


        }

        /// <summary>
        /// Function that returns if the system can Read from MicroController command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command parameter (not used in this function)</param>
        /// <returns>Boolean value if the system can invoke the command.</returns>
        private bool CanReadCommand(object arg)
        {
            return true;
        }

        /// <summary>
        /// Function/Command that handles the saving when the command is invoked
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command's parameter (central or patient micro controller).</param>
        private void OnSaveCommand(object arg)
        {
            int state = 0;

            state = Array.IndexOf(Enum.GetValues(typeof(MessageStateId)), ConsoleFiniteStateMachine.CurrentState);
            CatheterType catheterType = this.dataAccess.GetCatheterAccordingToCatheterId(localCommonViewModel.CatheterID);

            if (catheterType != null)
                this.dataAccess.UpdateFlowCurveParameters(state, ThresholdFM1Low, ThresholdFM1High, FM1LowRange, FM1HighRange, catheterType.ID);
        }

        /// <summary>
        /// Function that returns if the system can save command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command parameter (not used in this function)</param>
        /// <returns>Boolean value if the system can invoke the command.</returns>
        private bool CanSaveCommand(object arg)
        {
            return true;
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

        #region Curve parameters

        /// <summary>
        /// This property gets/sets the FM1 Low Range value. it is used for Fm1 Low Fit (the part --- A --- of the Quadratic formula.)
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
                try
                {
                    localCommonViewModel.FM1LowRange = value;
                    localCommonViewModel.Console.FlowMeterOneValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].FlowMeterLowRangeLimit = value;
                    RaisePropertyChanged("FM1LowRange");
                }
                catch { }
            }
        }


        /// <summary>
        /// This property gets/sets the FM1 High Range value. it is used for Fm1 Low Ceiling (the part --- B --- of the Quadratic formula.)
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
                try
                {
                    localCommonViewModel.FM1HighRange = value;
                    localCommonViewModel.Console.FlowMeterOneValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].FlowMeterHighRangelimit = value;
                    RaisePropertyChanged("FM1HighRange");
                }
                catch { }
            }
        }

        /// <summary>
        /// This property gets/sets the Threshold FM1 Low value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A). it is used for Fm1 Low Offset (the part --- C --- of the Quadratic formula.)
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
                try
                {
                    localCommonViewModel.ThresholdFM1Low = value;
                    localCommonViewModel.Console.FlowMeterOneValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].FlowMeterThresholLowlimit = value;
                    RaisePropertyChanged("ThresholdFM1Low");
                }
                catch { }
            }
        }

        /// <summary>
        /// This property gets/sets the Threshold FM1 High value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A). is us used to Th FM1 High
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
                try
                {
                    localCommonViewModel.ThresholdFM1High = value;
                    localCommonViewModel.Console.FlowMeterOneValueAccordingToTheStateMachine[ConsoleFiniteStateMachine.CurrentState].FlowMeterThresholHighlimit = value;
                    RaisePropertyChanged("ThresholdFM1High");
                }
                catch { }
            }
        }

        /// <summary>
        /// N2O flow calculator
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public N2OFlowCalculator N2OFlowCalculator { get => n2OFlowCalculator; set => n2OFlowCalculator = value; }




        #endregion

        /// <summary>
        /// Function that returns if the system can invoke the Increment System State command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command parameter (not used in this function).</param>
        /// <returns>Boolean value if the system can invoke the command.</returns>
        private bool CanIncrementSystemStateCommand(object arg)
        {
            return true;
        }

        /// <summary>
        /// Function/Command that handles the System's state incrementation when the Increment System State
        /// command is invoked.  This function shall only be called when building in Simulator Mode
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="obj">The command's parameter (not used in this function).</param>
        private void OnIncrementSystemStateCommand(object obj)
        {
            if (CommonViewModel.Current.SystemState == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_UNKNOWN)
            {
                CommonViewModel.Current.SystemState = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE;
            }
            else if (CommonViewModel.Current.SystemState == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE)
            {
                CommonViewModel.Current.SystemState = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY;
            }
            else if (CommonViewModel.Current.SystemState == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY)
            {
                CommonViewModel.Current.SystemState = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_INFLATION;
            }
            else if (CommonViewModel.Current.SystemState == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_INFLATION)
            {
                CommonViewModel.Current.SystemState = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION;
            }
            else if (CommonViewModel.Current.SystemState == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION)
            {
                CommonViewModel.Current.SystemState = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_ABLATION;
            }
            else if (CommonViewModel.Current.SystemState == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_ABLATION)
            {
                CommonViewModel.Current.SystemState = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING;
            }
            else if (CommonViewModel.Current.SystemState == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING)
            {
                CommonViewModel.Current.SystemState = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE;
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

            if (IsProgrammingFlow)
            {
                switch (e.PropertyName)
                {
                    case "TC1Reading":
                        RaisePropertyChanged("TC1Reading");
                        break;

                    case "CP2Reading":
                        RaisePropertyChanged("CP2Reading");
                        break;

                    case "CP1Reading":
                        RaisePropertyChanged("CP1Reading");
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
                        RaisePropertyChanged("ExpectedFlow");
                        RaisePropertyChanged("RelativeError");
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

                    case "PT2Reading":
                        RaisePropertyChanged("PT2Reading");
                        break;

                    case "PT3Reading":
                        RaisePropertyChanged("PT3Reading");
                        break;

                    case "IBPReading":
                        RaisePropertyChanged("IBPReading");
                        break;

                    case "DeflateAfterThaw":
                        RaisePropertyChanged("DeflateAfterThaw");
                        break;

                    case "RequiredAblationTime":
                        RaisePropertyChanged("RequiredAblationTime");
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

                    case "ThresholdForCP2High":
                        RaisePropertyChanged("ThresholdForCP2High");
                        break;

                    case "ThresholdForCP1High":
                        RaisePropertyChanged("ThresholdForCP1High");
                        break;

                    case "ThresholdForCTC1High":
                        RaisePropertyChanged("ThresholdForCTC1High");
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

                    #endregion Errors

                    #region catheter ready

                    case "IsCMCUReady":
                    case "IsPMCUReady":
                        ActivateCatheterIfConditionsApply();
                        break;

                        #endregion
                }
            }
        }


        /// <summary>
        /// Gets/sets the value indicating whether Lock the foot switch or not
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
        /// Gets or sets whether a user is programming the flow curve
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsProgrammingFlow
        {
            get => isProgrammingFlow;
            set => isProgrammingFlow = value;
        }
    }
}