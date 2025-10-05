using Communication;
using Prism.Mvvm;
using SmartAblationSystem.Helpers;
using SmartAblationSystem.Models;
using SmartAblationSystem.Views;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FileSerializer;
using Prism.Commands;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using MicroLibrary;

namespace SmartAblationSystem.ViewModels
{
    /// <summary>
    /// This class is the SimulatorPopup View Model
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    internal class SimulationModeViewModel : BindableBase
    {
        private UserControl cryoTherapyView;

        public ICommand IncrementSystemStateCommand { get; private set; }
        public ICommand SetUnknownSystemStateCommand { get; private set; }
        public ICommand SetExceptionSystemStateCommand { get; private set; }
        public ICommand IncrementTemperatureCommand { get; private set; }
        public ICommand DecrementTemperatureCommand { get; private set; }
        public ICommand ErrorCommand { get; private set; }
        public ICommand IncreaseBloodPressureCommand { get; private set; }
        public ICommand DecreaseBloodPressureCommand { get; private set; }
        public ICommand ProxIncreaseCommand { get; private set; }
        public ICommand ProxDecreaseCommand { get; private set; }

        /* ICB Commands */
        public ICommand ToggleETSCommand { get; private set; }
        public ICommand ToggleDMSCommand { get; private set; }
        public ICommand TogglePressureSensorCommand { get; private set; }
        public ICommand ToggleRemoteControlCommand { get; private set; }

        /* ETS Commands */
        public ICommand ToggleETSTypeCommand { get; private set; }
        public ICommand CopyMultiETSTipToAllCommand { get; private set; }
        public ICommand UpdateETSDataCommand { get; private set; }

        /* DMS Commands */
        public ICommand SimulateSlowDMSCommand { get; private set; }
        public ICommand SimulateMediumDMSCommand { get; private set; }
        public ICommand SimulateFastDMSCommand { get; private set; }

        /* Remote Control Commands */
        public ICommand RemoteControlStartCommand { get; private set; }
        public ICommand RemoteControlStopCommand { get; private set; }
        public ICommand RemoteControlAblationSiteLeftCommand { get; private set; }
        public ICommand RemoteControlAblationSiteRightCommand { get; private set; }
        public ICommand RemoteControlTimeIncreaseCommand { get; private set; }
        public ICommand RemoteControlTimeDecreaseCommand { get; private set; }
        public ICommand RemoteControlPressureIncreaseCommand { get; private set; }
        public ICommand RemoteControlPressureDecreaseCommand { get; private set; }


        bool valueReseted = false;
        double[] bloodPressureSimValue = { 35, 35, 35, 35 };
        double singleETSTemperature = 0;
        double multiETSSensor1Temperature = 0;
        double multiETSSensor2Temperature = 0;
        double multiETSSensor3Temperature = 0;
        double multiETSSensor4Temperature = 0;
        double multiETSSensor5Temperature = 0;
        double multiETSSensor6Temperature = 0;
        double multiETSSensor7Temperature = 0;
        double multiETSSensor8Temperature = 0;
        double multiETSSensor9Temperature = 0;
        double multiETSSensor10Temperature = 0;
        double multiETSSensor11Temperature = 0;
        double multiETSSensor12Temperature = 0;
        double multiETSSensor13Temperature = 0;

        bool icbETSConnection = false;
        bool icbDMSConnection = false;
        bool icbCatheterConnection = false;
        bool icbPressureSensorConnection = false;
        bool icbRemoteControlConnection = false;

        bool slowDMS = false;
        bool mediumDMS = false;
        bool fastDMS = false;

        double singlePressureSensorValue = 35;

        private MicroTimer SimulationElapsedTime = new MicroTimer();
        int simulationDataIndex = 0;
        List<SimulatedDMSData> simulatedDMSData;

        /// <summary>
        /// This constructor initializes the SimulatorPopup View Model's properties and commands
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public SimulationModeViewModel()
        {
            this.IncrementSystemStateCommand = new DelegateCommand<object>(this.OnIncrementSystemStateCommand, this.CanIncrementSystemStateCommand);
            this.SetUnknownSystemStateCommand = new DelegateCommand<object>(this.OnSetUnknownSystemStateCommand, this.CanSetUnknownSystemStateCommand);
            this.SetExceptionSystemStateCommand = new DelegateCommand<object>(this.OnSetExceptionSystemStateCommand, this.CanSetExceptionSystemStateCommand);
            this.IncrementTemperatureCommand = new DelegateCommand<object>(this.OnIncrementTemperatureCommand, this.CanChangeTemperatureCommand);
            this.DecrementTemperatureCommand = new DelegateCommand<object>(this.OnDecrementTemperatureCommand, this.CanChangeTemperatureCommand);
            this.ErrorCommand = new DelegateCommand<object>(this.OnErrorCommand, this.CanErrorCommand);
            this.TogglePressureSensorCommand = new DelegateCommand<object>(this.OnTogglePressureSensorCommand, this.CanTogglePressureSensorCommand);
            this.IncreaseBloodPressureCommand = new DelegateCommand<object>(this.OnIncreaseBloodPressureCommand, this.CanIncreaseBloodPressureCommand);
            this.DecreaseBloodPressureCommand = new DelegateCommand<object>(this.OnDecreaseBloodPressureCommand, this.CanDecreaseBloodPressureCommand);



            this.ProxIncreaseCommand = new DelegateCommand<object>(this.OnProxIncreaseCommand, this.CanProxIncreaseCommand);
            this.ProxDecreaseCommand = new DelegateCommand<object>(this.OnProxDecreaseCommand, this.CanProxDecreaseCommand);

            this.ToggleETSCommand = new DelegateCommand<object>(this.OnToggleETSCommand, this.CanToggleETSCommand);
            this.ToggleDMSCommand = new DelegateCommand<object>(this.OnToggleDMSCommand, this.CanToggleDMSCommand);
            this.TogglePressureSensorCommand = new DelegateCommand<object>(this.OnTogglePressureSensorCommand, this.CanTogglePressureSensorCommand);
            this.ToggleRemoteControlCommand = new DelegateCommand<object>(this.OnToggleRemoteControlCommand, this.CanToggleRemoteControlCommand);

            this.ToggleETSTypeCommand = new DelegateCommand<object>(this.OnToggleETSTypeCommand, this.CanToggleETSTypeCommand);
            this.UpdateETSDataCommand = new DelegateCommand<object>(this.OnUpdateETSDataCommand, this.CanUpdateETSDataCommand);
            this.CopyMultiETSTipToAllCommand = new DelegateCommand<object>(this.OnCopyMultiETSTipToAllCommand, this.CanCopyMultiETSTipToAllCommand);

            this.SimulateSlowDMSCommand = new DelegateCommand<object>(this.OnSimulateSlowDMSCommand, this.CanSimulateSlowDMSCommand);
            this.SimulateMediumDMSCommand = new DelegateCommand<object>(this.OnSimulateMediumDMSCommand, this.CanSimulateMediumDMSCommand);
            this.SimulateFastDMSCommand = new DelegateCommand<object>(this.OnSimulateFastDMSCommand, this.CanSimulateFastDMSCommand);

            this.RemoteControlStartCommand = new DelegateCommand<object>(this.OnRemoteControlStartCommand, this.CanRemoteControlStartCommand);
            this.RemoteControlStopCommand = new DelegateCommand<object>(this.OnRemoteControlStopCommand, this.CanRemoteControlStopCommand);
            this.RemoteControlAblationSiteLeftCommand = new DelegateCommand<object>(this.OnRemoteControlAblationSiteLeftCommand, this.CanRemoteControlAblationSiteLeftCommand);
            this.RemoteControlAblationSiteRightCommand = new DelegateCommand<object>(this.OnRemoteControlAblationSiteRightCommand, this.CanRemoteControlAblationSiteRightCommand);
            this.RemoteControlTimeIncreaseCommand = new DelegateCommand<object>(this.OnRemoteControlTimeIncreaseCommand, this.CanRemoteControlTimeIncreaseCommand);
            this.RemoteControlTimeDecreaseCommand = new DelegateCommand<object>(this.OnRemoteControlTimeDecreaseCommand, this.CanRemoteControlTimeDecreaseCommand);
            this.RemoteControlPressureIncreaseCommand = new DelegateCommand<object>(this.OnRemoteControlPressureIncreaseCommand, this.CanRemoteControlPressureIncreaseCommand);
            this.RemoteControlPressureDecreaseCommand = new DelegateCommand<object>(this.OnRemoteControlPressureDecreaseCommand, this.CanRemoteControlPressureDecreaseCommand);

            loadSimulatedDMSData();

            SimulationElapsedTime.Interval = 40000;
            SimulationElapsedTime.MicroTimerElapsed += new MicroTimer.MicroTimerElapsedEventHandler(simulationElapsedTime_Tick);
            SimulationElapsedTime.Start();
        }

        private void loadSimulatedDMSData()
        {
            if (!File.Exists(@"C:\Users\abbouda\Documents\SmartFreeze\branches\CryoTherapyV5\SmartAblationSystem\results.json"))
            {
                throw new FileNotFoundException();
            }
            else
            {
                using (StreamReader r = new StreamReader(@"C:\Users\abbouda\Documents\SmartFreeze\branches\CryoTherapyV5\SmartAblationSystem\results.json"))
                {
                    string json = r.ReadToEnd();
                    simulatedDMSData = JsonConvert.DeserializeObject<List<SimulatedDMSData>>(json);
                }
            }
        }

        private async void simulationElapsedTime_Tick(object sender, EventArgs e)
        {
            await Task.Run(() => {
                if (ICBDMSConnection)
                {
                    if (slowDMS)
                    {
                        CommonViewModel.Current.EcgChannel3And4Reading = simulatedDMSData[simulationDataIndex].Slow / 100;
                        CommonViewModel.Current.EcgChannel7And8Reading = simulatedDMSData[simulationDataIndex].Slow / 100;
                    }
                    else if (mediumDMS)
                    {
                        CommonViewModel.Current.EcgChannel3And4Reading = simulatedDMSData[simulationDataIndex].Medium / 100;
                        CommonViewModel.Current.EcgChannel7And8Reading = simulatedDMSData[simulationDataIndex].Medium / 100;
                    }
                    else if (fastDMS)
                    {
                        CommonViewModel.Current.EcgChannel3And4Reading = simulatedDMSData[simulationDataIndex].Fast / 100;
                        CommonViewModel.Current.EcgChannel7And8Reading = simulatedDMSData[simulationDataIndex].Fast / 100;
                    }
                    
                    if (simulationDataIndex == 700)
                    {
                        simulationDataIndex = 0;
                    }
                    else
                    {
                        simulationDataIndex++;
                    }
                }     
            });
        }

        #region Commands

        #region ICB Commands
        private bool CanToggleETSCommand(object arg)
        {
            return true;
        }

        private void OnToggleETSCommand(object obj)
        {
            if (ICBETSConnection)
                ICBETSConnection = false;
            else
                ICBETSConnection = true;
        }
        private bool CanToggleDMSCommand(object arg)
        {
            return true;
        }

        private void OnToggleDMSCommand(object obj)
        {
            if (ICBDMSConnection)
                ICBDMSConnection = false;
            else
                ICBDMSConnection = true;
        }

        private bool CanToggleRemoteControlCommand(object arg)
        {
            return true;
        }

        private void OnToggleRemoteControlCommand(object obj)
        {
            if (ICBRemoteControlConnection)
                ICBRemoteControlConnection = false;
            else
                ICBRemoteControlConnection = true;
        }
        #endregion

        #region ETS Commands
        private bool CanToggleETSTypeCommand(object arg)
        {
            return true;
        }

        private void OnToggleETSTypeCommand(object obj)
        {
            if((bool)obj)
            {
                CommonViewModel.Current.IsMultiEtsSesnorConnected = true;
            }
            else
            {
                CommonViewModel.Current.IsMultiEtsSesnorConnected = false;
            }
            OnUpdateETSDataCommand(obj);
        }

        private bool CanUpdateETSDataCommand(object arg)
        {
            return true;
        }

        private void OnUpdateETSDataCommand(object obj)
        {
            if ((bool)obj)
            {
                
                CommonViewModel.Current.EtsSesnor1 = MultiETSSensor1Temperature;
                CommonViewModel.Current.EtsSesnor2 = MultiETSSensor2Temperature;
                CommonViewModel.Current.EtsSesnor3 = MultiETSSensor3Temperature;
                CommonViewModel.Current.EtsSesnor4 = MultiETSSensor4Temperature;
                CommonViewModel.Current.EtsSesnor5 = MultiETSSensor5Temperature;
                CommonViewModel.Current.EtsSesnor6 = MultiETSSensor6Temperature;
                CommonViewModel.Current.EtsSesnor7 = MultiETSSensor7Temperature;
                CommonViewModel.Current.EtsSesnor8 = MultiETSSensor8Temperature;
                CommonViewModel.Current.EtsSesnor9 = MultiETSSensor9Temperature;
                CommonViewModel.Current.EtsSesnor10 = MultiETSSensor10Temperature;
                CommonViewModel.Current.EtsSesnor11 = MultiETSSensor11Temperature;
                CommonViewModel.Current.EtsSesnor12 = MultiETSSensor12Temperature;
                CommonViewModel.Current.EtsSesnor13 = MultiETSSensor13Temperature;

                if (MultiETSSensor13Temperature == -100)
                {
                    MultiETSSensor13Temperature = 100;
                }
                CommonViewModel.Current.EcgChannel5And6Reading = MultiETSSensor13Temperature;
            }
            else
            {
                CommonViewModel.Current.EtsSesnor13 = SingleETSTemperature;
                
                if (SingleETSTemperature == -100)
                {
                    SingleETSTemperature = 100;
                }
                CommonViewModel.Current.EcgChannel5And6Reading = SingleETSTemperature;
            }
        }

        private bool CanCopyMultiETSTipToAllCommand(object arg)
        {
            return true;
        }

        private void OnCopyMultiETSTipToAllCommand(object obj)
        {
            MultiETSSensor1Temperature = MultiETSSensor13Temperature;
            MultiETSSensor2Temperature = MultiETSSensor13Temperature;
            MultiETSSensor3Temperature = MultiETSSensor13Temperature;
            MultiETSSensor4Temperature = MultiETSSensor13Temperature;
            MultiETSSensor5Temperature = MultiETSSensor13Temperature;
            MultiETSSensor6Temperature = MultiETSSensor13Temperature;
            MultiETSSensor7Temperature = MultiETSSensor13Temperature;
            MultiETSSensor8Temperature = MultiETSSensor13Temperature;
            MultiETSSensor9Temperature = MultiETSSensor13Temperature;
            MultiETSSensor10Temperature = MultiETSSensor13Temperature;
            MultiETSSensor11Temperature = MultiETSSensor13Temperature;
            MultiETSSensor12Temperature = MultiETSSensor13Temperature;
        }
        #endregion

        #region DMS Commands
        private bool CanSimulateSlowDMSCommand(object arg)
        {
            return true;
        }

        private void OnSimulateSlowDMSCommand(object obj)
        {
            slowDMS = true;
            mediumDMS = false;
            fastDMS = false;
        }

        private bool CanSimulateMediumDMSCommand(object arg)
        {
            return true;
        }

        private void OnSimulateMediumDMSCommand(object obj)
        {
            slowDMS = false;
            mediumDMS = true;
            fastDMS = false;
        }

        private bool CanSimulateFastDMSCommand(object arg)
        {
            return true;
        }

        private void OnSimulateFastDMSCommand(object obj)
        {
            slowDMS = false;
            mediumDMS = false;
            fastDMS = true;
        }
        #endregion

        private bool CanTogglePressureSensorCommand(object arg)
        {
            return true;
        }

        private void OnTogglePressureSensorCommand(object obj)
        {
#if Simulator
            if (CommonViewModel.Current.IsBloodPressureSensorConnected)
                CommonViewModel.Current.IsBloodPressureSensorConnected = false;
            else
            {
                CommonViewModel.Current.IsBloodPressureSensorConnected = true;
                CommonViewModel.Current.writeBloodPressureValueBuffer(bloodPressureSimValue);
            }

            if (ICBPressureSensorConnection)
                ICBPressureSensorConnection = false;
            else
                ICBPressureSensorConnection = true;
#endif
        }

        private bool CanIncreaseBloodPressureCommand(object arg)
        {
            return true;
        }

        private void OnIncreaseBloodPressureCommand(object obj)
        {
#if Simulator
            /*double[] _bloodPressureValue = new double[4];
            for (int i = 0; i < 4; i++)
            {
                pres_cnt = ((pres_cnt + 1) % Maxpres);
                _bloodPressureValue[i] = (double)(pres_cnt + 5);
            }
            CommonViewModel.Current.BloodPressureValue = _bloodPressureValue;
            ((CryoTherapyViewModel)cryoTherapyView.DataContext).EcgChannel1And2Reading = _bloodPressureValue[3];*/
            double[] dataBloodPressureValue = bloodPressureSimValue;
            for (int index = 0; index < dataBloodPressureValue.Length; index++)
            {
                if (dataBloodPressureValue[index] < 99)
                {
                    dataBloodPressureValue[index] = dataBloodPressureValue[index] + 1;
                }
            }
            CommonViewModel.Current.writeBloodPressureValueBuffer(dataBloodPressureValue);
            //CommonViewModel.Current.BloodPressureValue = dataBloodPressureValue;
            CommonViewModel.Current.EcgChannel1And2Reading = dataBloodPressureValue[3];
            SinglePressureSensorValue = dataBloodPressureValue[3];
#endif
        }
        //int pres_cnt = 0;
        //int Maxpres = 80;
        private bool CanDecreaseBloodPressureCommand(object arg)
        {
            return true;
        }

        private void OnDecreaseBloodPressureCommand(object obj)
        {
#if Simulator
            double[] dataBloodPressureValue = bloodPressureSimValue;
            for (int index = 0; index < dataBloodPressureValue.Length; index++)
            {
                if (dataBloodPressureValue[index] > 0)
                {
                    dataBloodPressureValue[index] = dataBloodPressureValue[index] - 1;
                }
            }
            CommonViewModel.Current.writeBloodPressureValueBuffer(dataBloodPressureValue);
            //CommonViewModel.Current.BloodPressureValue = dataBloodPressureValue;
            CommonViewModel.Current.EcgChannel1And2Reading = dataBloodPressureValue[3];
            SinglePressureSensorValue = dataBloodPressureValue[3];
#endif      
        }

        private bool CanProxDecreaseCommand(object arg)
        {
            return true;
        }

        private void OnProxDecreaseCommand(object obj)
        {
            CommonViewModel.Current.EcgChannel5And6Reading--;
        }

        private bool CanProxIncreaseCommand(object arg)
        {
            return true;
        }

        private void OnProxIncreaseCommand(object obj)
        {
            CommonViewModel.Current.EcgChannel5And6Reading++;
            // CommonViewModel.Current.RCWarningTimerControlPopupMessage();
        }

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
        /// Function that returns if the system can invoke the Change Temperature command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command parameter (not used in this function).</param>
        /// <returns>Boolean value if the system can invoke the command.</returns>
        private bool CanChangeTemperatureCommand(object arg)
        {
            return true;
        }

        /// <summary>
        /// Function/Command that handles the System State Incrementation when the Increment System State
        /// command is invoked.  This command shall only be invoked by the DebugWithSimulator solution
        /// if configuration is selected
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
                //CommonViewModel.Current.SystemState = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION;
                CommonViewModel.Current.SystemState = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE;
                Task.Delay(3000).ContinueWith(t => CommonViewModel.Current.SystemState = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY);

            }
            else if (CommonViewModel.Current.SystemState == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_EXCEPTION)
            {
                CommonViewModel.Current.SystemState = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE;
            }

            //CommonViewModel.Current.IsCMCULoadCellWeightFail = true;
            //CommonViewModel.Current.IsUserAllowedToChangeTank = true;

            //CommonViewModel.Current.WarningMessageManager.AddMessage("This is a SYSTEM message triggered in simulator mode.  Nothing really happenned!", WarningMessagesManager.WarningMessagesManagerEnumeration.MessageType.SYSTEM);
            //CommonViewModel.Current.WarningMessageManager.AddMessage("This is a WARNING message triggered in simulator mode.  Nothing really happenned!", WarningMessagesManager.WarningMessagesManagerEnumeration.MessageType.WARNING);
            //CommonViewModel.Current.WarningMessageManager.AddMessage("This is an ERROR message triggered in simulator mode.  Nothing really happenned!", WarningMessagesManager.WarningMessagesManagerEnumeration.MessageType.ERROR);
        }

        /// <summary>
        /// Function that returns if the system can invoke the Set Unknown System State command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command parameter (not used in this function).</param>
        /// <returns>Boolean value if the system can invoke the command.</returns>
        private bool CanSetUnknownSystemStateCommand(object arg)
        {
            return true;
        }

        /// <summary>
        /// Function/Command that handles the Unknown System State setting when the
        /// Unknown System State command is invoked
        /// This command shall only be invoked the DebugWithSimulator solution configuration is selected.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="obj">The command's parameter (not used in this function).</param>
        private void OnSetUnknownSystemStateCommand(object obj)
        {
            // CommonViewModel.Current.SystemState = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_UNKNOWN;
        }

        /// <summary>
        /// Function that returns if the system can invoke the Set Exception System State command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command parameter (not used in this function).</param>
        /// <returns>Boolean value if the system can invoke the command.</returns>
        private bool CanSetExceptionSystemStateCommand(object arg)
        {
            return true;
        }

        /// <summary>
        /// Function/Command that handles the Exception System State setting when the
        /// Exception System State command is invoked
        /// This command shall only be invoked the DebugWithSimulator solution configuration is selected.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="obj">The command's parameter (not used in this function).</param>
        private void OnSetExceptionSystemStateCommand(object obj)
        {
            //CommonViewModel.Current.GetCMCUStatusError(8);
            //CommonViewModel.Current.SystemState = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_EXCEPTION;
        }

        /// <summary>
        /// Function/Command that handles the Temperature incrementation when the Increment Temperature
        /// command is invoked.  This command shall only be invoked the DebugWithSimulator solution configuration is selected
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="obj">The command's parameter (not used in this function).</param>
        private void OnIncrementTemperatureCommand(object obj)
        {
            CommonViewModel.Current.TC1Reading += 5;  //Tank weight
            CommonViewModel.Current.LC1Reading += 5;  //Tank gage

            //Tip Pressure, 0 to 15
            if (CommonViewModel.Current.EcgChannel1And2Reading < 240)
                CommonViewModel.Current.EcgChannel1And2Reading += 10;

            //Diaphragm amplitude goes between -2 and 2 G
            if (CommonViewModel.Current.EcgChannel3And4Reading < 2)
                CommonViewModel.Current.EcgChannel3And4Reading += 0.1;

            //Esophagus temperature, set 30 value by default when in simulator mode
            if (CommonViewModel.Current.EcgChannel5And6Reading == 0)
            {
                CommonViewModel.Current.EcgChannel5And6Reading = 35;
            }

            //Esophagus temperature
            //if (CommonViewModel.Current.EcgChannel5And6Reading + 1 < 50)
            CommonViewModel.Current.EcgChannel5And6Reading += 1;

            //Diaphragm movement, set 90 value by default when in simulator mode
            if (CommonViewModel.Current.EcgChannel7And8Reading == 0)
            {
                CommonViewModel.Current.EcgChannel7And8Reading = 90;
            }

            //Diaphragm movement
            if (CommonViewModel.Current.EcgChannel7And8Reading < 100)
                CommonViewModel.Current.EcgChannel7And8Reading += 1;

            //Balloon Pressure, 0 to 10
            if (CommonViewModel.Current.CP1Reading < 10)
                CommonViewModel.Current.CP1Reading += 0.5;

            CommonViewModel.Current.LC1Reading = +1;
            CommonViewModel.Current.BloodDetecorImValue++;

            //CommonViewModel.Current.IsVeinIsolated = true;

            // CommonViewModel.Current.IsDiaphragmMovementDetected = false;

            //CommonViewModel.Current.test();



#if Simulator
            //CommonViewModel.Current.testForSimulation();


            if (!valueReseted)
            {
                ResetDataForSimulation();
                valueReseted = true;
            }
#endif
        }

        /// <summary>
        /// Function/Command that handles the Temperature decrementation when the Decrement Temperature
        /// command is invoked.  This command shall only be invoked the DebugWithSimulator solution configuration is selected
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="obj">The command's parameter (not used in this function).</param>
        private void OnDecrementTemperatureCommand(object obj)
        {
            CommonViewModel.Current.TC1Reading -= 5; //Tank weight
            CommonViewModel.Current.CatheterTemperature -= 5;
            //Tip Pressure, 0 to 15
            if (CommonViewModel.Current.EcgChannel1And2Reading > 0)
                CommonViewModel.Current.EcgChannel1And2Reading -= 10;

            //Diaphragm amplitude goes between -2 and 2 G
            if (CommonViewModel.Current.EcgChannel3And4Reading > -2)
                CommonViewModel.Current.EcgChannel3And4Reading -= 0.1;

            //Esophagus temperature, set 30 value by default when in simulator mode
            if (CommonViewModel.Current.EcgChannel5And6Reading == 0)
            {
                CommonViewModel.Current.EcgChannel5And6Reading = 30;
            }

            //Esophagus temperature
            //if (CommonViewModel.Current.EcgChannel5And6Reading > 0)
            CommonViewModel.Current.EcgChannel5And6Reading -= 1;

            //Diaphragm movement, set 90 value by default when in simulator mode
            if (CommonViewModel.Current.EcgChannel7And8Reading == 0)
            {
                CommonViewModel.Current.EcgChannel7And8Reading = 90;
            }

            //Diaphragm movement
            if (CommonViewModel.Current.EcgChannel7And8Reading >= 0)
                CommonViewModel.Current.EcgChannel7And8Reading -= 1;

            //Balloon Pressure, 0 to 10
            if (CommonViewModel.Current.CP1Reading > 0)
                CommonViewModel.Current.CP1Reading -= 0.5;

            CommonViewModel.Current.LC1Reading = -1;
            CommonViewModel.Current.BloodDetecorImValue--;

            //CommonViewModel.Current.DeacreaeSesnorSimulation();
        }

        int errorNumber = 0;

        /// <summary>
        /// Function/Command that handles the Error display.
        /// command is invoked.  This command shall only be invoked the DebugWithSimulator solution configuration is selected
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="obj">The command's parameter (not used in this function).</param>
        private void OnErrorCommand(object obj)
        {

            //CommonViewModel.Current.DisplayException5Message();

            //CommonViewModel.Current.GetCMCUStatusError(544); //544 //512

            Task.Delay(10000).ContinueWith(t => CommonViewModel.Current.GetCMCUStatusError(8));

            //CommonViewModel.Current.GetCMCUStatusError(1568);
            //List<Tuple<long, string, string, string>> errors = new List<Tuple<long, string, string, string>>();

            //errors.Add(new Tuple<long, string, string, string>(1, "Cable not connected!", "Connect Cable!", "A generic Cryterion technical system notification"));
            //errors.Add(new Tuple<long, string, string, string>(2, "Catheter connection lost!", "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.", ""));
            //errors.Add(new Tuple<long, string, string, string>(3, "Generic issue 1", "Fix generic issue 1!  No video available.  Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.", ""));

            //MessagePopup messagePopup = new MessagePopup(errors,
            //                                             MessagePopup.MessageType.ErrorMessage,
            //                                             MessagePopup.ButtonType.Ok,
            //                                             IsActionRequired: true);

            //if ((bool)messagePopup.ShowDialog())
            //{
            //}

            //messagePopup = new MessagePopup("Test Single message", MessagePopup.MessageType.SystemMessage);
            //if ((bool)messagePopup.ShowDialog())
            //{
            //}

            switch (errorNumber)
            {
                case 0:

                    //CommonViewModel.Current.GetCMCUStatusError(512);
                    break;

                case 1:

                    // CommonViewModel.Current.GetPMCUStatusError(16384);
                    break;

                case 2:

                    //CommonViewModel.Current.GetPMCUStatusError(16);
                    break;

                case 3:

                    //CommonViewModel.Current.GetPMCUStatusError(32);
                    break;

                case 4:

                    //   CommonViewModel.Current.GetPMCUStatusError(64);
                    break;

                case 5:

                    //  CommonViewModel.Current.GetPMCUStatusError(128);
                    break;

                case 6:

                    //  CommonViewModel.Current.GetPMCUStatusError(256);
                    break;

                case 7:

                    // CommonViewModel.Current.GetPMCUStatusError(512);
                    break;

                case 8:

                    // CommonViewModel.Current.GetPMCUStatusError(1024);
                    break;

                case 9:

                    //CommonViewModel.Current.GetPMCUStatusError(2048);
                    break;

                case 10:

                    //CommonViewModel.Current.GetPMCUStatusError(4096);
                    break;

                case 11:

                    //CommonViewModel.Current.GetPMCUStatusError(16384);
                    errorNumber = 0;
                    break;
            }

            errorNumber++;


        }


        /// <summary>
        /// Function that returns if the system can invoke the Error command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command parameter (not used in this function).</param>
        /// <returns>Boolean value if the system can invoke the command.</returns>
        private bool CanErrorCommand(object arg)
        {
            return true;
        }

        #region Remote Control Commands
        /// <summary>
        /// Function that returns if the system can invoke the Remote Control Start command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command parameter (not used in this function).</param>
        /// <returns>Boolean value if the system can invoke the command.</returns>
        private bool CanRemoteControlStartCommand(object arg)
        {
            return true;
        }

        /// <summary>
        /// Function/Command that handles the System State setting when the Remote Control Start command is invoked
        /// This command shall only be invoked the DebugWithSimulator solution configuration is selected.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="obj">The command's parameter (not used in this function).</param>
        private void OnRemoteControlStartCommand(object obj)
        {
            if (CommonViewModel.Current.SystemState == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_UNKNOWN)
            {
                CommonViewModel.Current.SystemState = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE;
            }
            else if (CommonViewModel.Current.SystemState == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE)
            {
                CommonViewModel.Current.IsVacuumDisconnected = false;
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
                if (CommonViewModel.Current.IsVeinIsolated)
                {
                    CommonViewModel.Current.IsVeinIsolated = false;
                    CommonViewModel.Current.IsVeinIsolated = true;
                }
                else
                {
                    CommonViewModel.Current.IsVeinIsolated = true;
                }
                Task.Delay(2000).ContinueWith(t => CommonViewModel.Current.SystemState = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_ABLATION);
            }
            else if (CommonViewModel.Current.SystemState == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_ABLATION)
            {
                if (CommonViewModel.Current.IsVeinIsolated)
                {
                    CommonViewModel.Current.IsVeinIsolated = false;
                    CommonViewModel.Current.IsVeinIsolated = true;
                }
                else
                {
                    CommonViewModel.Current.IsVeinIsolated = true;
                }
            }
            else if (CommonViewModel.Current.SystemState == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING)
            {
                CommonViewModel.Current.SystemState = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION;
            }
            else
            {
                CommonViewModel.Current.WasAblationTimeManuallyChanged = false;
            }
        }

        /// <summary>
        /// Function that returns if the system can invoke the Remote Control Start command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command parameter (not used in this function).</param>
        /// <returns>Boolean value if the system can invoke the command.</returns>
        private bool CanRemoteControlStopCommand(object arg)
        {
            return true;
        }

        /// <summary>
        /// Function/Command that handles the System State setting when the Remote Control Start command is invoked
        /// This command shall only be invoked the DebugWithSimulator solution configuration is selected.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="obj">The command's parameter (not used in this function).</param>
        private void OnRemoteControlStopCommand(object obj)
        {
            if (CommonViewModel.Current.SystemState == Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY)
            {
                CommonViewModel.Current.IsVacuumDisconnected = true;
                CommonViewModel.Current.SystemState = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE;
            }
            else if (CommonViewModel.Current.SystemState == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_INFLATION)
            {
                CommonViewModel.Current.SystemState = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE;
            }
            else if (CommonViewModel.Current.SystemState == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION)
            {
                CommonViewModel.Current.SystemState = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING;
            }
            else if (CommonViewModel.Current.SystemState == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_ABLATION)
            {
                CommonViewModel.Current.SystemState = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING;
            }
            else if (CommonViewModel.Current.SystemState == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING)
            {
                CommonViewModel.Current.SystemState = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE;
                Task.Delay(3000).ContinueWith(t => CommonViewModel.Current.SystemState = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY);
            }
        }

        /// <summary>
        /// Function that returns if the system can invoke the Remote Control Start command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command parameter (not used in this function).</param>
        /// <returns>Boolean value if the system can invoke the command.</returns>
        private bool CanRemoteControlAblationSiteLeftCommand(object arg)
        {
            return true;
        }

        /// <summary>
        /// Function/Command that handles the System State setting when the Remote Control Start command is invoked
        /// This command shall only be invoked the DebugWithSimulator solution configuration is selected.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="obj">The command's parameter (not used in this function).</param>
        private void OnRemoteControlAblationSiteLeftCommand(object obj)
        {
            AblationSiteCarousselModel.MoveAblationSiteToTheLeft();
            if (CommonViewModel.Current.AreSensorsInPlayBackMode)
            {
                CommonViewModel.Current.UpdateAblationSite(CommonViewModel.Current.TreatmentNumber, AblationSiteCarousselModel.CurrentAblationSite);
                CommonViewModel.Current.GenerateAblationSummary();
            }
            CommonViewModel.Current.AblationSite = AblationSiteCarousselModel.CurrentAblationSite;
        }

        /// <summary>
        /// Function that returns if the system can invoke the Remote Control Start command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command parameter (not used in this function).</param>
        /// <returns>Boolean value if the system can invoke the command.</returns>
        private bool CanRemoteControlAblationSiteRightCommand(object arg)
        {
            return true;
        }

        /// <summary>
        /// Function/Command that handles the System State setting when the Remote Control Start command is invoked
        /// This command shall only be invoked the DebugWithSimulator solution configuration is selected.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="obj">The command's parameter (not used in this function).</param>
        private void OnRemoteControlAblationSiteRightCommand(object obj)
        {
            AblationSiteCarousselModel.MoveAblationSiteToTheRight();
            if (CommonViewModel.Current.AreSensorsInPlayBackMode)
            {
                CommonViewModel.Current.UpdateAblationSite(CommonViewModel.Current.TreatmentNumber, AblationSiteCarousselModel.CurrentAblationSite);
                CommonViewModel.Current.GenerateAblationSummary();
            }
            CommonViewModel.Current.AblationSite = AblationSiteCarousselModel.CurrentAblationSite;
        }
        /// <summary>
        /// Function that returns if the system can invoke the Remote Control Start command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command parameter (not used in this function).</param>
        /// <returns>Boolean value if the system can invoke the command.</returns>
        private bool CanRemoteControlTimeIncreaseCommand(object arg)
        {
            return true;
        }

        /// <summary>
        /// Function/Command that handles the System State setting when the Remote Control Start command is invoked
        /// This command shall only be invoked the DebugWithSimulator solution configuration is selected.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="obj">The command's parameter (not used in this function).</param>
        private void OnRemoteControlTimeIncreaseCommand(object obj)
        {
            if (CommonViewModel.Current.SystemState != Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING && !CommonViewModel.Current.AreSensorsInPlayBackMode)
            {
                CommonViewModel.Current.RequiredAblationTime += 30;
                CommonViewModel.Current.TemporaryManualAblationTime = CommonViewModel.Current.RequiredAblationTime;
                CommonViewModel.Current.WasAblationTimeManuallyChanged = true;

                CommonViewModel.Current.ISTTISelected = false;
                CommonViewModel.Current.ISTTIDurationTimerSelected = false;
                CommonViewModel.Current.ISTTIFixedTimerSelected = false;
                CommonViewModel.Current.IsFixedTimerSelected = true;
            }
        }

        /// <summary>
        /// Function that returns if the system can invoke the Remote Control Start command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command parameter (not used in this function).</param>
        /// <returns>Boolean value if the system can invoke the command.</returns>
        private bool CanRemoteControlTimeDecreaseCommand(object arg)
        {
            return true;
        }

        /// <summary>
        /// Function/Command that handles the System State setting when the Remote Control Start command is invoked
        /// This command shall only be invoked the DebugWithSimulator solution configuration is selected.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="obj">The command's parameter (not used in this function).</param>
        private void OnRemoteControlTimeDecreaseCommand(object obj)
        {
            if (CommonViewModel.Current.SystemState != Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING && (CommonViewModel.Current.RequiredAblationTime - 30 > CommonViewModel.Current.CryoTherapyTime) && !CommonViewModel.Current.AreSensorsInPlayBackMode)
            {
                CommonViewModel.Current.RequiredAblationTime -= 30;
                CommonViewModel.Current.TemporaryManualAblationTime = CommonViewModel.Current.RequiredAblationTime;
                CommonViewModel.Current.WasAblationTimeManuallyChanged = true;

                CommonViewModel.Current.ISTTISelected = false;
                CommonViewModel.Current.ISTTIDurationTimerSelected = false;
                CommonViewModel.Current.ISTTIFixedTimerSelected = false;
                CommonViewModel.Current.IsFixedTimerSelected = true;
            }
        }

        /// <summary>
        /// Function that returns if the system can invoke the Remote Control Start command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command parameter (not used in this function).</param>
        /// <returns>Boolean value if the system can invoke the command.</returns>
        private bool CanRemoteControlPressureIncreaseCommand(object arg)
        {
            return true;
        }

        /// <summary>
        /// Function/Command that handles the System State setting when the Remote Control Start command is invoked
        /// This command shall only be invoked the DebugWithSimulator solution configuration is selected.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="obj">The command's parameter (not used in this function).</param>
        private void OnRemoteControlPressureIncreaseCommand(object obj)
        {
            if (CommonViewModel.Current.SystemState == Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_INFLATION || CommonViewModel.Current.SystemState == Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING)
            {
                if (CommonViewModel.Current.SystemState == Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING && CommonViewModel.Current.TC1Reading < 20)
                    return;

                if (!CommonViewModel.Current.IsBalloonDiameterIncreased && !CommonViewModel.Current.ChangeBalloonTypeFSM.DASBalloonEnabled && CommonViewModel.Current.IsSystemUsingDASBalloon)
                {
                    CommonViewModel.Current.IsBalloonDiameterIncreased = true;
                    CommonViewModel.Current.IsBalloonDiameterDecreased = false;
                }
            }
        }

        /// <summary>
        /// Function that returns if the system can invoke the Remote Control Start command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command parameter (not used in this function).</param>
        /// <returns>Boolean value if the system can invoke the command.</returns>
        private bool CanRemoteControlPressureDecreaseCommand(object arg)
        {
            return true;
        }

        /// <summary>
        /// Function/Command that handles the System State setting when the Remote Control Start command is invoked
        /// This command shall only be invoked the DebugWithSimulator solution configuration is selected.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="obj">The command's parameter (not used in this function).</param>
        private void OnRemoteControlPressureDecreaseCommand(object obj)
        {
            if (CommonViewModel.Current.SystemState == Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_INFLATION || CommonViewModel.Current.SystemState == Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING)
            {
                if (CommonViewModel.Current.SystemState == Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING && CommonViewModel.Current.TC1Reading < 20)
                    return;

                if (BalloonRampDown.IsBalloonRampDownActivated && !CommonViewModel.Current.IsBalloonDiameterDecreased && CommonViewModel.Current.ChangeBalloonTypeFSM.DASBalloonEnabled)
                {
                    CommonViewModel.Current.IsBalloonDiameterDecreased = true;
                    CommonViewModel.Current.IsBalloonDiameterIncreased = false;
                }
            }
        }
        #endregion

        #endregion

        private void ResetDataForSimulation()
        {
            CommonViewModel.Current.EcgChannel5And6Reading = 37;

            CommonViewModel.Current.EtsSesnor1 = 37;
            CommonViewModel.Current.EtsSesnor2 = 37;
            CommonViewModel.Current.EtsSesnor3 = 37;
            CommonViewModel.Current.EtsSesnor4 = 37;
            CommonViewModel.Current.EtsSesnor5 = 37;
            CommonViewModel.Current.EtsSesnor6 = 37;
            CommonViewModel.Current.EtsSesnor7 = 37;
            CommonViewModel.Current.EtsSesnor8 = 37;
            CommonViewModel.Current.EtsSesnor9 = 37;
            CommonViewModel.Current.EtsSesnor10 = 37;
            CommonViewModel.Current.EtsSesnor11 = 37;
            CommonViewModel.Current.EtsSesnor12 = 37;
            CommonViewModel.Current.EtsSesnor13 = 37;
            CommonViewModel.Current.TIP = 37;

            CommonViewModel.Current.MinimumTemperature = 37;


        }

        

        /// <summary>
        /// This property gets/sets CryoTherapy View value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public UserControl CryoTherapyView
        {
            get
            {
                return cryoTherapyView;
            }

            set
            {
                SetProperty(ref this.cryoTherapyView, value);
            }
        }

        #region Properties

        #region ICB Properties

        public bool ICBETSConnection
        {
            get
            {
                return icbETSConnection;
            }

            set
            {
                SetProperty(ref this.icbETSConnection, value);
                RaisePropertyChanged("ICBETSConnection");
            }
        }

        public bool ICBDMSConnection
        {
            get
            {
                return icbDMSConnection;
            }

            set
            {
                SetProperty(ref this.icbDMSConnection, value);
                RaisePropertyChanged("ICBDMSConnection");
            }
        }

        public bool ICBCatheterConnection
        {
            get
            {
                return icbCatheterConnection;
            }

            set
            {
                SetProperty(ref this.icbCatheterConnection, value);
                RaisePropertyChanged("ICBCatheterConnection");
            }
        }

        public bool ICBPressureSensorConnection
        {
            get
            {
                return icbPressureSensorConnection;
            }

            set
            {
                SetProperty(ref this.icbPressureSensorConnection, value);
                RaisePropertyChanged("ICBPressureSensorConnection");
            }
        }

        public bool ICBRemoteControlConnection
        {
            get
            {
                return icbRemoteControlConnection;
            }

            set
            {
                SetProperty(ref this.icbRemoteControlConnection, value);
                RaisePropertyChanged("ICBRemoteControlConnection");
            }
        }

        #endregion

        #region ETS Properties
        public double SingleETSTemperature
        {
            get
            {
                return singleETSTemperature;
            }
            set
            {
                singleETSTemperature = value;
                RaisePropertyChanged("SingleETSTemperature");
            }
        }

        public double MultiETSSensor1Temperature
        {
            get
            {
                return multiETSSensor1Temperature;
            }
            set
            {
                multiETSSensor1Temperature = value;
                RaisePropertyChanged("MultiETSSensor1Temperature");
            }
        }

        public double MultiETSSensor2Temperature
        {
            get
            {
                return multiETSSensor2Temperature;
            }
            set
            {
                multiETSSensor2Temperature = value;
                RaisePropertyChanged("MultiETSSensor2Temperature");
            }
        }

        public double MultiETSSensor3Temperature
        {
            get
            {
                return multiETSSensor3Temperature;
            }
            set
            {
                multiETSSensor3Temperature = value;
                RaisePropertyChanged("MultiETSSensor3Temperature");
            }
        }

        public double MultiETSSensor4Temperature
        {
            get
            {
                return multiETSSensor4Temperature;
            }
            set
            {
                multiETSSensor4Temperature = value;
                RaisePropertyChanged("MultiETSSensor4Temperature");
            }
        }

        public double MultiETSSensor5Temperature
        {
            get
            {
                return multiETSSensor5Temperature;
            }
            set
            {
                multiETSSensor5Temperature = value;
                RaisePropertyChanged("MultiETSSensor5Temperature");
            }
        }

        public double MultiETSSensor6Temperature
        {
            get
            {
                return multiETSSensor6Temperature;
            }
            set
            {
                multiETSSensor6Temperature = value;
                RaisePropertyChanged("MultiETSSensor6Temperature");
            }
        }

        public double MultiETSSensor7Temperature
        {
            get
            {
                return multiETSSensor7Temperature;
            }
            set
            {
                multiETSSensor7Temperature = value;
                RaisePropertyChanged("MultiETSSensor7Temperature");
            }
        }

        public double MultiETSSensor8Temperature
        {
            get
            {
                return multiETSSensor8Temperature;
            }
            set
            {
                multiETSSensor8Temperature = value;
                RaisePropertyChanged("MultiETSSensor8Temperature");
            }
        }

        public double MultiETSSensor9Temperature
        {
            get
            {
                return multiETSSensor9Temperature;
            }
            set
            {
                multiETSSensor9Temperature = value;
                RaisePropertyChanged("MultiETSSensor9Temperature");
            }
        }

        public double MultiETSSensor10Temperature
        {
            get
            {
                return multiETSSensor10Temperature;
            }
            set
            {
                multiETSSensor10Temperature = value;
                RaisePropertyChanged("MultiETSSensor10Temperature");
            }
        }

        public double MultiETSSensor11Temperature
        {
            get
            {
                return multiETSSensor11Temperature;
            }
            set
            {
                multiETSSensor11Temperature = value;
                RaisePropertyChanged("MultiETSSensor11Temperature");
            }
        }

        public double MultiETSSensor12Temperature
        {
            get
            {
                return multiETSSensor12Temperature;
            }
            set
            {
                multiETSSensor12Temperature = value;
                RaisePropertyChanged("MultiETSSensor12Temperature");
            }
        }

        public double MultiETSSensor13Temperature
        {
            get
            {
                return multiETSSensor13Temperature;
            }
            set
            {
                multiETSSensor13Temperature = value;
                RaisePropertyChanged("MultiETSSensor13Temperature");
            }
        }

        #endregion

        #region Pressure Sensor Properties
        public double SinglePressureSensorValue
        {
            get
            {
                return singlePressureSensorValue;
            }
            set
            {
                singlePressureSensorValue = value;
                RaisePropertyChanged("SinglePressureSensorValue");
            }
        }
        #endregion

        #endregion
    }
}
