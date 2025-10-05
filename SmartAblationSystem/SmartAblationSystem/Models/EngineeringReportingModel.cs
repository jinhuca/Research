using FileSerializer;
using MicroLibrary;
using SmartAblationSystem.ViewModels;
using System;
using System.Threading;
using static Communication.CanBusMessageDefinition;

namespace SmartAblationSystem.Helpers
{
    /// <summary>
    /// This class for is the Engineering Reporting Model
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class EngineeringReportingModel
    {
        private static EngineeringReportingModel instance;

        private static MessageStateId currentState = MessageStateId.CAN_ID_STATE_UNKNOWN;
        private static MessageStateId previousState = MessageStateId.CAN_ID_STATE_UNKNOWN;
        private static bool stopAcquisition = false;
        private static bool starAcquisition = false;
        private static bool isLoggingActivated = false;
        private static MicroTimer loggingTimer = new MicroTimer();

        private EngineeringData engineeringData;

        private static ThreadStart DataThreadStart;
        private static Thread DataThread;

        /// <summary>
        /// Initializes a new instance of the Engineering Reporting Model class and its properties
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private EngineeringReportingModel()
        {
            DataThreadStart = new ThreadStart(LogData);            //Create a new thread
            DataThread = new Thread(DataThreadStart);
            DataThread.Priority = ThreadPriority.Normal;

            StartDataThread();
        }

        /// <summary>
        /// Returns an EngineeringReportingModel object instance
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static EngineeringReportingModel Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EngineeringReportingModel();
                }

                return instance;
            }
        }

        /// <summary>
        /// Gets or sets a MessageStateId representing the current state
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static MessageStateId CurrentState
        {
            get
            {
                return currentState;
            }
            set
            {
                if (value != currentState)
                    currentState = value;
            }
        }

        /// <summary>
        /// Gets or sets a MessageStateId representing the previous state
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static MessageStateId PreviousState
        {
            get
            {
                return previousState;
            }
            set
            {
                if (value != previousState)
                {
                    previousState = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the data acquisition has stopped or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static bool StopAcquisition
        {
            get
            {
                return stopAcquisition;
            }
            set
            {
                if (value != stopAcquisition)
                    stopAcquisition = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the data acquisition has started or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static bool StarAcquisition
        {
            get
            {
                return starAcquisition;
            }

            set
            {
                if (value != starAcquisition)
                    starAcquisition = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the logging is activated or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static bool IsLoggingActivated
        {
            get
            {
                return isLoggingActivated;
            }

            set
            {
                if (value != isLoggingActivated)
                {
                    isLoggingActivated = value;
                }
            }
        }

        /// <summary>
        /// Initializes the instance of EnginneringReporing Model
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static void initialize()
        {
            instance = new EngineeringReportingModel();
        }

        /// <summary>
        /// Adds an engineering data to the array of Engineering Datas
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public void AddEngineeringData()
        {
            if (engineeringData == null)
                engineeringData = new FileSerializer.EngineeringData();

            if (CommonViewModel.Current != null)
            {
                CommonViewModel localCommonViewModel = CommonViewModel.Current;
                EngineeringDataDetails engineeringDataDetails = new EngineeringDataDetails();

                engineeringDataDetails.TimeStamp = DateTime.Now.ToString("hh.mm.ss.fff");
                engineeringDataDetails.SystemState = (int)localCommonViewModel.SystemState;

                engineeringDataDetails.TimeToTargetTemperature = 0; //?
                engineeringDataDetails.RequiredTargetTemperature = 0; //?
                engineeringDataDetails.TimeToThaw = 0; //?
                engineeringDataDetails.TC1Reading = localCommonViewModel.TC1Reading;
                engineeringDataDetails.TimeInSecondIndex = 0; //?
                engineeringDataDetails.PMCUCJReading = localCommonViewModel.PMCUCJReading;
                engineeringDataDetails.PT1Reading = localCommonViewModel.PT1Reading;
                engineeringDataDetails.PT2Reading = localCommonViewModel.PT2Reading;
                engineeringDataDetails.PT3Reading = localCommonViewModel.PT3Reading;
                engineeringDataDetails.PT4Reading = localCommonViewModel.PT4Reading;
                engineeringDataDetails.PT5Reading = localCommonViewModel.PT5Reading;
                engineeringDataDetails.PS1Reading = localCommonViewModel.PS1Reading;
                engineeringDataDetails.FM1Reading = localCommonViewModel.FM1Reading;
                engineeringDataDetails.TS1Reading = localCommonViewModel.TS1Reading;
                engineeringDataDetails.TN2OReading = localCommonViewModel.TN2OReading;
                engineeringDataDetails.LC1Reading = localCommonViewModel.LC1Reading;
                engineeringDataDetails.TIPReading = localCommonViewModel.TIPReading;
                engineeringDataDetails.CP1Reading = localCommonViewModel.CP1Reading;
                engineeringDataDetails.CP2Reading = localCommonViewModel.CP2Reading;
                engineeringDataDetails.CIMP1Reading = 0; //?
                engineeringDataDetails.PWMINJ = localCommonViewModel.PIDDutyCycle;
                engineeringDataDetails.PWMBAL = localCommonViewModel.PatientPIDDutyCycle;

                engineeringDataDetails.EcgChannel1And2Reading = localCommonViewModel.EcgChannel1And2Reading;
                engineeringDataDetails.EcgChannel3And4Reading = localCommonViewModel.EcgChannel3And4Reading;
                engineeringDataDetails.EcgChannel5And6Reading = localCommonViewModel.EcgChannel5And6Reading;
                engineeringDataDetails.EcgChannel7And8Reading = localCommonViewModel.EcgChannel7And8Reading;

                engineeringData.EngineeringDataDetails.Add(engineeringDataDetails);
            }
        }

        /// <summary>
        /// Starts the data thread
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private static void StartDataThread()
        {
            if (DataThread != null)
                DataThread.Start();
        }

        /// <summary>
        /// Stops the data thread
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private static void StopDataThread()
        {
            //if (DataThread != null)
            //    DataThread.Join();
        }

        /// <summary>
        /// Logs the data to a file
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void LogData()
        {
            try
            {
                while (true)
                {
                    // We are not able to predict the CPLD  State so we will write  when good Transition Appear
                    if (IsLoggingActivated)
                    {
                        if (this.engineeringData == null)
                        {
                            this.engineeringData = new FileSerializer.EngineeringData();
                        }

                        CurrentState = CommonViewModel.Current.SystemState;

                        if (StarAcquisition)
                            AddEngineeringData();

                        if (CurrentState == MessageStateId.CAN_ID_STATE_EXCEPTION && previousState != MessageStateId.CAN_ID_STATE_EXCEPTION)
                        {
                            //The Ablation is finished we will save the data
                            if (StarAcquisition)
                            {
                                StopAcquisition = true;
                                StarAcquisition = false;

                                this.engineeringData.WriteToJson(engineeringData);
                            }

                            previousState = CommonViewModel.Current.SystemState;
                        }

                        if (CurrentState != MessageStateId.CAN_ID_STATE_THAWING && previousState == MessageStateId.CAN_ID_STATE_THAWING)
                        {
                            //The Ablation is finished we will save the data
                            if (!StopAcquisition)
                            {
                                StopAcquisition = true;
                                StarAcquisition = false;

                                this.engineeringData.WriteToJson(engineeringData);
                            }

                            previousState = CommonViewModel.Current.SystemState;
                        }

                        if (CurrentState == MessageStateId.CAN_ID_STATE_IDLE && previousState == MessageStateId.CAN_ID_STATE_INFLATION)
                        {
                            //The Ablation is finished we will save the data
                            if (StarAcquisition)
                            {
                                StopAcquisition = true;
                                StarAcquisition = false;

                                this.engineeringData.WriteToJson(engineeringData);
                            }

                            previousState = CommonViewModel.Current.SystemState;
                        }

                        if (CurrentState == MessageStateId.CAN_ID_STATE_INFLATION && previousState != MessageStateId.CAN_ID_STATE_INFLATION)
                        {
                            //Here we start the ablation and Write to the JSON File
                            if (!StarAcquisition)
                            {
                                StopAcquisition = false;
                                StarAcquisition = true;
                            }

                            previousState = CommonViewModel.Current.SystemState;
                        }

                        if (CurrentState == MessageStateId.CAN_ID_STATE_TRANSITION && previousState != MessageStateId.CAN_ID_STATE_TRANSITION)
                        {
                            //Here we start the ablation
                            if (!StarAcquisition)
                            {
                                StopAcquisition = false;
                                StarAcquisition = true;
                            }

                            previousState = CommonViewModel.Current.SystemState;
                        }

                        if (CurrentState == MessageStateId.CAN_ID_STATE_ABLATION && previousState != MessageStateId.CAN_ID_STATE_ABLATION)
                        {
                            //Here we start the ablation
                            if (!StarAcquisition)
                            {
                                StopAcquisition = false;
                                StarAcquisition = true;
                            }

                            previousState = CommonViewModel.Current.SystemState;
                        }

                        //be sure to not miss the previous state
                        previousState = CommonViewModel.Current.SystemState;
                    }

                    Thread.Sleep(75);
                }
            }
            catch
            {
            }
        }
    }
}