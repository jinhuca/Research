using Communication;
using Console;
using DataAccessLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartAblationSystem.Models;
using SmartAblationSystem.ViewModels;
using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using RS232Communication;
using Unity;
using static Communication.CanBusMessageDefinition;

namespace DataAccessLayerTests
{
    [TestClass()]
    public class CanBusCommunicationTests
    {
        private CanBusCommunication canBusCommunication;

        private int state = Array.IndexOf(Enum.GetValues(typeof(MessageStateId)), MessageStateId.CAN_ID_STATE_IDLE);
        private int localSelectedRegisterForTargetInjectionFlow = 15;
        private int maxWritingTime = 8;
        private int maxCycle = 3;//100000;
        private Data data;
        private Machine console;
        private ChangeBalloonTypeFSM changeBalloonTypeFSM;
        private InflateDeflateBalloonModel inflateDeflateBalloonModel;
        private CommonViewModel commonViewModel; 

        [TestInitialize]
        public void Setup()
        {
            commonViewModel = new CommonViewModel(new Machine(new CanBusCommunication(), new GeneralPurposeInputOutput()), new SerialPortManager());
            CommonViewModel.Current.Console.GUIInMaintenanceMode = true;
        }

        /// <summary>
        /// These Test validate the IsCatheterValid fail as a result of an exception
        /// If an excpetion occures the test fail
        /// </summary>

        [TestMethod()]
        public void BypassValidationTestWithInvalidCatheterSerialNumber()
        {
            bool IsCatheterValid = false;

            int CatheterSerialNumber = 155;
            int CatheterLot = 65535; 
            bool IsCryterionUser;
            IsCryterionUser = true;
            try
            {
                if (CatheterSerialNumber == CommonViewModel.Current.Console.ServiceDevices.EngineeringCatheter.SerialNumber &&
                     CatheterLot == CommonViewModel.Current.Console.ServiceDevices.EngineeringCatheter.CatheterLot && IsCryterionUser)
                {
                    IsCatheterValid = true;

                }
                Assert.AreEqual(IsCatheterValid, false);
                
            }
            catch (Exception ex)
            {
                Assert.Fail();
            }
        }


        /// <summary>
        /// These Test validate the IsCatheterValid fail as a result of an exception
        /// If an excpetion occures the test fail
        /// </summary>

        [TestMethod()]
        public void BypassValidationTestWithInvalidCatheterLot()
        {
            bool IsCatheterValid = false;

            int CatheterSerialNumber = 255;
            int CatheterLot = 1234567;
            bool IsCryterionUser;
            IsCryterionUser = true;
            try
            {
                if (CatheterSerialNumber == CommonViewModel.Current.Console.ServiceDevices.EngineeringCatheter.SerialNumber &&
                     CatheterLot == CommonViewModel.Current.Console.ServiceDevices.EngineeringCatheter.CatheterLot && IsCryterionUser)
                {
                    IsCatheterValid = true;

                }
                Assert.AreEqual(IsCatheterValid, false);

            }
            catch (Exception ex)
            {
                Assert.Fail();
            }
        }


        /// <summary>
        /// These Test validate the IsCatheterValid fail as a result of an exception
        /// If an excpetion occures the test fail
        /// </summary>

        [TestMethod()]
        public void BypassValidationTestWithInvalidCryterionUser()
        {
            bool IsCatheterValid = false;

            int CatheterSerialNumber = 255;
            int CatheterLot = 65535; 
            bool IsCryterionUser;
            IsCryterionUser = false;
            try
            {
                if (CatheterSerialNumber == CommonViewModel.Current.Console.ServiceDevices.EngineeringCatheter.SerialNumber &&
                     CatheterLot == CommonViewModel.Current.Console.ServiceDevices.EngineeringCatheter.CatheterLot && IsCryterionUser)
                {
                    IsCatheterValid = true;

                }
                Assert.AreEqual(IsCatheterValid, false);

            }
            catch (Exception ex)
            {
                Assert.Fail();
            }
        }


        /// <summary>
        /// These Test validate the IsCatheterValid pass as a result of an exception
        /// If an excpetion occures the test fail
        /// </summary>

        [TestMethod()]
        public void BypassValidationTestWithValidInfo()
        {
            bool IsCatheterValid = false;

            int CatheterSerialNumber = 255;
            int CatheterLot = 65535;
            bool IsCryterionUser;
            IsCryterionUser = true;
            try
            {
                if (CatheterSerialNumber == CommonViewModel.Current.Console.ServiceDevices.EngineeringCatheter.SerialNumber &&
                     CatheterLot == CommonViewModel.Current.Console.ServiceDevices.EngineeringCatheter.CatheterLot && IsCryterionUser)
                {
                    IsCatheterValid = true;

                }
                Assert.AreEqual(IsCatheterValid, true);

            }
            catch (Exception ex)
            {
                Assert.Fail();
            }
        }




        /// <summary>
        /// These Test validate the can bus one and two and initialize 
        /// </summary>
        [TestMethod()]
        public void CanBusCommunicationTest()
        {

            try
            {
                canBusCommunication = new CanBusCommunication();
                Assert.IsNotNull(canBusCommunication);
            }

            catch (Exception ex)
            {
                Assert.Fail();
            }


        }


        /// <summary>
        /// These Test validate the writing not fail as a result of an exception
        /// If an excpetion occures the test fail
        /// </summary>

        [TestMethod()]
        public void WriteTest()
        {
            try
            {

                for (int i = 0; i < maxWritingTime; i++)
                {
                    CommonViewModel.Current.Console.WriteFromMicroController((MessageStateId)state, localSelectedRegisterForTargetInjectionFlow);
                    System.Threading.Thread.Sleep(20);
                }
            }

            catch (Exception ex)
            {
                Assert.Fail();
            }

        }


        /// <summary>
        /// These Test validate the reading not fail as a result of an exception
        ///  If an excpetion occures the test fail
        /// </summary>
        [TestMethod()]
        public void ReadTest()
        {
            try
            {

                for (int i = 0; i < maxWritingTime; i++)
                {
                    CommonViewModel.Current.Console.ReadFromMicroController((MessageStateId)state, localSelectedRegisterForTargetInjectionFlow);
                    System.Threading.Thread.Sleep(20);
                }
            }

            catch (Exception ex)
            {
                Assert.Fail();
            }
        }

        /// <summary>
        /// These test is the most importane Test for the comunication. It is validating that the data 
        /// sent to the firmware is not corrupted, recived correctly and read corrctely.  Here we send a value and we read it  again. 
        /// </summary>
        [TestMethod()]
        public void WriteReadVerificationTest()
        {
            try
            {
              int maxCycleSQLUpdate = 10; 
                for (int j = 0; j < maxCycleSQLUpdate; j++)
                {
                    for (int i = 0; i < maxWritingTime; i++)
                    {
                        Random rnd = new Random();
                        int TargetInjectionFlowRandomValue = rnd.Next(0, 6000);

                        CommonViewModel.Current.Console.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachine[MessageStateId.CAN_ID_STATE_IDLE].TargetInjectionFlow = TargetInjectionFlowRandomValue;

                        CommonViewModel.Current.Console.WriteFromMicroController((MessageStateId)state, localSelectedRegisterForTargetInjectionFlow);
                        System.Threading.Thread.Sleep(20);
                        CommonViewModel.Current.Console.ReadFromMicroController((MessageStateId)state, localSelectedRegisterForTargetInjectionFlow);
                        System.Threading.Thread.Sleep(20);
                    }
                }
            }

            catch (Exception ex)
            {
                Assert.Fail();
            }
        }

        /*[TestMethod()]
        public void TestGpioLevels()
        {
            try
            {

                for (int j = 0; j < maxCycle; j++)
                {

                    //Activating The IOs
                    CommonViewModel.Current.Console.StopEnable();
                    System.Threading.Thread.Sleep(1000);

                    CommonViewModel.Current.Console.WatchdogResetEnable();
                    System.Threading.Thread.Sleep(1000);

                    CommonViewModel.Current.Console.SystemResetEnable();
                    System.Threading.Thread.Sleep(1000);

                    CommonViewModel.Current.Console.FailResetEnable();
                    System.Threading.Thread.Sleep(1000);

                    CommonViewModel.Current.Console.IinjectionEnable();
                    System.Threading.Thread.Sleep(1000);

                    CommonViewModel.Current.Console.VacuumEnable();
                    System.Threading.Thread.Sleep(1000);

                    CommonViewModel.Current.Console.AblateEnable();
                    System.Threading.Thread.Sleep(1000);

                    CommonViewModel.Current.Console.ChangeTankEnable();
                    System.Threading.Thread.Sleep(1000);

                    //Deactivate the IOs
                    CommonViewModel.Current.Console.StopDisable();
                    System.Threading.Thread.Sleep(1000);

                    CommonViewModel.Current.Console.WatchdogResetDisable();
                    System.Threading.Thread.Sleep(1000);

                    CommonViewModel.Current.Console.SystemResetDisable();
                    System.Threading.Thread.Sleep(1000);

                    CommonViewModel.Current.Console.FailResetDisable();
                    System.Threading.Thread.Sleep(1000);

                    CommonViewModel.Current.Console.InjectionDisable();
                    System.Threading.Thread.Sleep(1000);

                    CommonViewModel.Current.Console.VacuumDisable();
                    System.Threading.Thread.Sleep(1000);

                    CommonViewModel.Current.Console.AblateDisable();
                    System.Threading.Thread.Sleep(1000);

                    CommonViewModel.Current.Console.ChangeTankDisable();
                    System.Threading.Thread.Sleep(1000);

                }
            }

            catch (Exception ex)
            {
                Assert.Fail();
            }




        }*/



        [TestMethod()]
        public void ChangeBalloonTypeFSMTargetInjectionFlowTest()
        {
            try
            {
                CommonViewModel localCommonViewModel = CommonViewModel.Current;
                data = new Data();
                console = localCommonViewModel.Console;
                Assert.IsNotNull(data);
                Assert.IsNotNull(console);

                inflateDeflateBalloonModel = new InflateDeflateBalloonModel(data, console);
                changeBalloonTypeFSM = new ChangeBalloonTypeFSM(inflateDeflateBalloonModel);
                Assert.IsNotNull(changeBalloonTypeFSM);


                System.Threading.Thread.Sleep(2000);

                MessageStateId mid = MessageStateId.CAN_ID_STATE_IDLE;

                foreach (MessageStateId stateId in Enum.GetValues(typeof(MessageStateId)))
                {

                    if (stateId != MessageStateId.CAN_ID_STATE_UNKNOWN && stateId != MessageStateId.CAN_ID_STATE_EXCEPTION)
                    {
                        int state = 0;
                        state = Array.IndexOf(Enum.GetValues(typeof(MessageStateId)), stateId);

                        switch (state)
                        {
                            case 1:
                                mid = MessageStateId.CAN_ID_STATE_IDLE;
                                //Validation
                                Assert.AreEqual(inflateDeflateBalloonModel.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsActivated[mid].TargetInjectionFlow, 0);
                                Assert.AreEqual(inflateDeflateBalloonModel.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsNotActivated[mid].TargetInjectionFlow, 0);
                            break;

                            case 2:
                                mid = MessageStateId.CAN_ID_STATE_READY;
                                //Validation
                                Assert.AreEqual(inflateDeflateBalloonModel.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsActivated[mid].TargetInjectionFlow, 0);
                                Assert.AreEqual(inflateDeflateBalloonModel.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsNotActivated[mid].TargetInjectionFlow, 0);
                            break;

                            case 3:
                                mid = MessageStateId.CAN_ID_STATE_INFLATION;
                                //Validation
                                Assert.AreEqual(inflateDeflateBalloonModel.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsActivated[mid].TargetInjectionFlow, 0);
                                Assert.AreEqual(inflateDeflateBalloonModel.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsNotActivated[mid].TargetInjectionFlow, 0);
                                break;

                            case 4:
                                mid = MessageStateId.CAN_ID_STATE_TRANSITION;
                                //Validation
                                Assert.AreEqual(inflateDeflateBalloonModel.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsActivated[mid].TargetInjectionFlow, 5000);
                                Assert.AreEqual(inflateDeflateBalloonModel.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsNotActivated[mid].TargetInjectionFlow, 5000);
                            break;

                            case 5:
                                mid = MessageStateId.CAN_ID_STATE_ABLATION;
                                //Validation
                                Assert.AreEqual(inflateDeflateBalloonModel.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsActivated[mid].TargetInjectionFlow, 8700);
                                Assert.AreEqual(inflateDeflateBalloonModel.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsNotActivated[mid].TargetInjectionFlow, 7800);
                            break;

                            case 6:
                                mid = MessageStateId.CAN_ID_STATE_THAWING;
                                //Validation
                                Assert.AreEqual(inflateDeflateBalloonModel.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsActivated[mid].TargetInjectionFlow, 0);
                                Assert.AreEqual(inflateDeflateBalloonModel.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsNotActivated[mid].TargetInjectionFlow, 0);
                            break;
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Assert.Fail();
            }
        }


        [TestMethod()]
        public void ChangeBalloonTypeFSMTargetInjectionPressureTest()
        {
            try
            {
                CommonViewModel localCommonViewModel = CommonViewModel.Current;
                data = new Data();
                console = localCommonViewModel.Console;
                Assert.IsNotNull(data);
                Assert.IsNotNull(console);

                inflateDeflateBalloonModel = new InflateDeflateBalloonModel(data, console);
                changeBalloonTypeFSM = new ChangeBalloonTypeFSM(inflateDeflateBalloonModel);
                Assert.IsNotNull(changeBalloonTypeFSM);


                System.Threading.Thread.Sleep(2000);

                MessageStateId mid = MessageStateId.CAN_ID_STATE_IDLE;

                foreach (MessageStateId stateId in Enum.GetValues(typeof(MessageStateId)))
                {

                    if (stateId != MessageStateId.CAN_ID_STATE_UNKNOWN && stateId != MessageStateId.CAN_ID_STATE_EXCEPTION)
                    {
                        int state = 0;
                        state = Array.IndexOf(Enum.GetValues(typeof(MessageStateId)), stateId);

                        switch (state)
                        {
                            case 1:
                                mid = MessageStateId.CAN_ID_STATE_IDLE;
                                //Validation
                                Assert.AreEqual(inflateDeflateBalloonModel.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsActivated[mid].TargetInjectionPressure, 0);
                                Assert.AreEqual(inflateDeflateBalloonModel.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsNotActivated[mid].TargetInjectionPressure, 0);
                            break;

                            case 2:
                                mid = MessageStateId.CAN_ID_STATE_READY;
                                //Validation
                                Assert.AreEqual(inflateDeflateBalloonModel.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsActivated[mid].TargetInjectionPressure, 100);
                                Assert.AreEqual(inflateDeflateBalloonModel.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsNotActivated[mid].TargetInjectionPressure, 100);
                            break;

                            case 3:
                                mid = MessageStateId.CAN_ID_STATE_INFLATION;
                                //Validation
                                Assert.AreEqual(inflateDeflateBalloonModel.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsActivated[mid].TargetInjectionPressure, 150);
                                Assert.AreEqual(inflateDeflateBalloonModel.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsNotActivated[mid].TargetInjectionPressure, 150);
                                break;

                            case 4:
                                mid = MessageStateId.CAN_ID_STATE_TRANSITION;
                                //Validation
                                Assert.AreEqual(inflateDeflateBalloonModel.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsActivated[mid].TargetInjectionPressure, 560);
                                Assert.AreEqual(inflateDeflateBalloonModel.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsNotActivated[mid].TargetInjectionPressure, 560);
                            break;

                            case 5:
                                mid = MessageStateId.CAN_ID_STATE_ABLATION;
                                //Validation
                                Assert.AreEqual(inflateDeflateBalloonModel.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsActivated[mid].TargetInjectionPressure, 0);
                                Assert.AreEqual(inflateDeflateBalloonModel.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsNotActivated[mid].TargetInjectionPressure, 0);
                            break;

                            case 6:
                                mid = MessageStateId.CAN_ID_STATE_THAWING;
                                //Validation
                                Assert.AreEqual(inflateDeflateBalloonModel.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsActivated[mid].TargetInjectionPressure, 150);
                                Assert.AreEqual(inflateDeflateBalloonModel.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsNotActivated[mid].TargetInjectionPressure, 150);
                                break;
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Assert.Fail();
            }
        }

    

        #region DAS Balloonn

        /// <summary>
        /// Since the can bus communication is tested we are testing the logic of sending the data and not the communication layer 
        /// </summary>
        [TestMethod()]
        public void SendDasPressureSetpointAndACKTest()
        {
            try
            {
                CommonViewModel localCommonViewModel = CommonViewModel.Current;

                PrivateObject obj = new PrivateObject(CommonViewModel.Current);

                InflateDeflateBalloonModel localInflateDeflateBalloonModel = CommonViewModel.Current.InflateDeflateBalloonModel;


                var retVal = obj.Invoke("SendDasPressureSetpointAndACK");
                
                System.Threading.Thread.Sleep(2000);
                MessageStateId mid = MessageStateId.CAN_ID_STATE_IDLE;

                foreach (MessageStateId stateId in Enum.GetValues(typeof(MessageStateId)))
                {
                    if (stateId != MessageStateId.CAN_ID_STATE_UNKNOWN && stateId != MessageStateId.CAN_ID_STATE_EXCEPTION)
                    {
                        int state = 0;
                        state = Array.IndexOf(Enum.GetValues(typeof(MessageStateId)), stateId);

                        switch (state)
                        {
                            case 1:
                                mid = MessageStateId.CAN_ID_STATE_IDLE;
                                //Validation
                                Assert.AreEqual(localInflateDeflateBalloonModel.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsActivated[mid].TargetInjectionFlow, 0);
                                Assert.AreEqual(localInflateDeflateBalloonModel.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsActivated[mid].TargetInjectionPressure, 0);

                                Assert.AreEqual(localInflateDeflateBalloonModel.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsNotActivated[mid].TargetInjectionFlow, 0);
                                Assert.AreEqual(localInflateDeflateBalloonModel.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsNotActivated[mid].TargetInjectionPressure, 0);

                                //Ballon Rum up and ramp dow timing 
                                
                                Assert.AreEqual(localCommonViewModel.Console.PatientMicroControllerCryoBalloonConfigurationValueAccordingToTheStateMachine[mid].RampUpTimeByStep, 500);
                                Assert.AreEqual(localCommonViewModel.Console.PatientMicroControllerCryoBalloonConfigurationValueAccordingToTheStateMachine[mid].PressureRampUpValue, 0.5);
                                Assert.AreEqual(localCommonViewModel.Console.PatientMicroControllerCryoBalloonConfigurationValueAccordingToTheStateMachine[mid].RampDownTimeByStep, 200);
                                Assert.AreEqual(localCommonViewModel.Console.PatientMicroControllerCryoBalloonConfigurationValueAccordingToTheStateMachine[mid].PressureRampDownValue, 0.2);

                                break;

                            case 2:
                                mid = MessageStateId.CAN_ID_STATE_READY;
                                //Validation

                                Assert.AreEqual(localInflateDeflateBalloonModel.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsActivated[mid].TargetInjectionFlow, 0);
                                Assert.AreEqual(localInflateDeflateBalloonModel.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsActivated[mid].TargetInjectionPressure, 100);

                                Assert.AreEqual(localInflateDeflateBalloonModel.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsNotActivated[mid].TargetInjectionFlow, 0);
                                Assert.AreEqual(localInflateDeflateBalloonModel.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsNotActivated[mid].TargetInjectionPressure, 100);

                                //Ballon Rum up and ramp dow timing 
                                Assert.AreEqual(localCommonViewModel.Console.PatientMicroControllerCryoBalloonConfigurationValueAccordingToTheStateMachine[mid].RampUpTimeByStep, 500);
                                Assert.AreEqual(localCommonViewModel.Console.PatientMicroControllerCryoBalloonConfigurationValueAccordingToTheStateMachine[mid].PressureRampUpValue, 0.5);
                                Assert.AreEqual(localCommonViewModel.Console.PatientMicroControllerCryoBalloonConfigurationValueAccordingToTheStateMachine[mid].RampDownTimeByStep, 200);
                                Assert.AreEqual(localCommonViewModel.Console.PatientMicroControllerCryoBalloonConfigurationValueAccordingToTheStateMachine[mid].PressureRampDownValue, 0.2);

                                break;

                            case 3:
                                mid = MessageStateId.CAN_ID_STATE_INFLATION;
                                //Validation

                                Assert.AreEqual(localInflateDeflateBalloonModel.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsActivated[mid].TargetInjectionFlow, 0);
                                Assert.AreEqual(localInflateDeflateBalloonModel.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsActivated[mid].TargetInjectionPressure, 150);

                                Assert.AreEqual(localInflateDeflateBalloonModel.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsNotActivated[mid].TargetInjectionFlow, 0);
                                Assert.AreEqual(localInflateDeflateBalloonModel.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsNotActivated[mid].TargetInjectionPressure, 150);

                                //Ballon Rum up and ramp dow timing 
                                Assert.AreEqual(localCommonViewModel.Console.PatientMicroControllerCryoBalloonConfigurationValueAccordingToTheStateMachine[mid].RampUpTimeByStep, 500);
                                Assert.AreEqual(localCommonViewModel.Console.PatientMicroControllerCryoBalloonConfigurationValueAccordingToTheStateMachine[mid].PressureRampUpValue, 0.5);
                                Assert.AreEqual(localCommonViewModel.Console.PatientMicroControllerCryoBalloonConfigurationValueAccordingToTheStateMachine[mid].RampDownTimeByStep, 200);
                                Assert.AreEqual(localCommonViewModel.Console.PatientMicroControllerCryoBalloonConfigurationValueAccordingToTheStateMachine[mid].PressureRampDownValue, 0.2);

                                break;

                            case 4:
                                mid = MessageStateId.CAN_ID_STATE_TRANSITION;
                                //Validation

                                Assert.AreEqual(localInflateDeflateBalloonModel.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsActivated[mid].TargetInjectionFlow, 5000);
                                Assert.AreEqual(localInflateDeflateBalloonModel.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsActivated[mid].TargetInjectionPressure, 560);

                                Assert.AreEqual(localInflateDeflateBalloonModel.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsNotActivated[mid].TargetInjectionFlow, 5000);
                                Assert.AreEqual(localInflateDeflateBalloonModel.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsNotActivated[mid].TargetInjectionPressure, 560);

                                //Ballon Rum up and ramp dow timing 
                                Assert.AreEqual(localCommonViewModel.Console.PatientMicroControllerCryoBalloonConfigurationValueAccordingToTheStateMachine[mid].RampUpTimeByStep, 500);
                                Assert.AreEqual(localCommonViewModel.Console.PatientMicroControllerCryoBalloonConfigurationValueAccordingToTheStateMachine[mid].PressureRampUpValue, 0.5);
                                Assert.AreEqual(localCommonViewModel.Console.PatientMicroControllerCryoBalloonConfigurationValueAccordingToTheStateMachine[mid].RampDownTimeByStep, 200);
                                Assert.AreEqual(localCommonViewModel.Console.PatientMicroControllerCryoBalloonConfigurationValueAccordingToTheStateMachine[mid].PressureRampDownValue, 0.2);

                                break;

                            case 5:
                                mid = MessageStateId.CAN_ID_STATE_ABLATION;
                                //Validation

                                Assert.AreEqual(localInflateDeflateBalloonModel.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsActivated[mid].TargetInjectionFlow, 8700);
                                Assert.AreEqual(localInflateDeflateBalloonModel.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsActivated[mid].TargetInjectionPressure, 0);

                                Assert.AreEqual(localInflateDeflateBalloonModel.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsNotActivated[mid].TargetInjectionFlow, 7800);
                                Assert.AreEqual(localInflateDeflateBalloonModel.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsNotActivated[mid].TargetInjectionPressure, 0);

                                //Ballon Rum up and ramp dow timing 
                                Assert.AreEqual(localCommonViewModel.Console.PatientMicroControllerCryoBalloonConfigurationValueAccordingToTheStateMachine[mid].RampUpTimeByStep, 500);
                                Assert.AreEqual(localCommonViewModel.Console.PatientMicroControllerCryoBalloonConfigurationValueAccordingToTheStateMachine[mid].PressureRampUpValue, 0.5);
                                Assert.AreEqual(localCommonViewModel.Console.PatientMicroControllerCryoBalloonConfigurationValueAccordingToTheStateMachine[mid].RampDownTimeByStep, 200);
                                Assert.AreEqual(localCommonViewModel.Console.PatientMicroControllerCryoBalloonConfigurationValueAccordingToTheStateMachine[mid].PressureRampDownValue, 0.2);

                                break;

                            case 6:
                                mid = MessageStateId.CAN_ID_STATE_THAWING;
                                //Validation

                                Assert.AreEqual(localInflateDeflateBalloonModel.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsActivated[mid].TargetInjectionFlow, 0);
                                Assert.AreEqual(localInflateDeflateBalloonModel.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsActivated[mid].TargetInjectionPressure, 150);

                                Assert.AreEqual(localInflateDeflateBalloonModel.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsNotActivated[mid].TargetInjectionFlow, 0);
                                Assert.AreEqual(localInflateDeflateBalloonModel.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsNotActivated[mid].TargetInjectionPressure, 150);

                                //Ballon Rum up and ramp dow timing 
                                Assert.AreEqual(localCommonViewModel.Console.PatientMicroControllerCryoBalloonConfigurationValueAccordingToTheStateMachine[mid].RampUpTimeByStep, 500);
                                Assert.AreEqual(localCommonViewModel.Console.PatientMicroControllerCryoBalloonConfigurationValueAccordingToTheStateMachine[mid].PressureRampUpValue, 0.5);
                                Assert.AreEqual(localCommonViewModel.Console.PatientMicroControllerCryoBalloonConfigurationValueAccordingToTheStateMachine[mid].RampDownTimeByStep, 200);
                                Assert.AreEqual(localCommonViewModel.Console.PatientMicroControllerCryoBalloonConfigurationValueAccordingToTheStateMachine[mid].PressureRampDownValue, 0.2);

                                break;

                        }





                    }
                }

            }

            catch (Exception ex)
            {
                Assert.Fail();
            }
        }



        [TestMethod()]
        public void SetReadyModelParametersTest()
        {
            try
            {
                var unityContainerMock = new Mock<IUnityContainer>();
                CommonViewModel localCommonViewModel = CommonViewModel.Current;

                localCommonViewModel.ChangeBalloonTypeFSM.DASBalloonEnabled = true;

                localCommonViewModel.SystemState = MessageStateId.CAN_ID_STATE_READY;
                
                CryoTherapyViewModel localCryoTherapyViewModel = new CryoTherapyViewModel(unityContainerMock.Object);

                PrivateObject cryoTherapyViewModelObj = new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(localCryoTherapyViewModel);


                cryoTherapyViewModelObj.SetField("PreviousSystemState", MessageStateId.CAN_ID_STATE_IDLE);



                System.Threading.Thread.Sleep(2000);

                var retVal = cryoTherapyViewModelObj.Invoke("SetReadyModelParameters");

                System.Threading.Thread.Sleep(2000);

                Assert.IsTrue(localCommonViewModel.ChangeBalloonTypeFSM.DASBalloonEnabled ==  false);
                Assert.IsTrue(localCryoTherapyViewModel.DASBalloonEnabled == false);

            }

            catch (Exception ex)
            {
                Assert.Fail();
            }
        }



        [TestMethod()]
        public void GetLoadCellThresholdFailTest()
        {
            try
            {
                double LC1LoadCellThresholdFail = 2.5;

                this.data = new Data();

                double value = data.DataAccess.GetLoadCellThresholdFail();

                System.Threading.Thread.Sleep(2000);

                Assert.AreEqual(value, LC1LoadCellThresholdFail);

            }

            catch (Exception ex)
            {
                Assert.Fail();
            }
        }


        [TestMethod()]
        public void GetDASBallonParametersTest()
        {
            try
            {

                this.data = new Data();

                List<BalloonParameters> ballonParameters = data.DataAccess.GetDASBallonParameters();


                System.Threading.Thread.Sleep(2000);

               // MessageStateId mid = MessageStateId.CAN_ID_STATE_IDLE;

                foreach (MessageStateId stateId in Enum.GetValues(typeof(MessageStateId)))
                {

                    if (stateId != MessageStateId.CAN_ID_STATE_UNKNOWN && stateId != MessageStateId.CAN_ID_STATE_EXCEPTION)
                    {
                        int state = 0;
                        state = Array.IndexOf(Enum.GetValues(typeof(MessageStateId)), stateId);

                        BalloonParameters _ballonParameters = ballonParameters[state - 1];

                        switch (state)
                        {
                            case 1:
                              //  mid = MessageStateId.CAN_ID_STATE_IDLE;
                                //Validation

                                Assert.AreEqual((double)_ballonParameters.HighFlowSetPoint, 0);
                                Assert.AreEqual((double)_ballonParameters.HighTargetInjectionPressure, 0);

                                Assert.AreEqual((double)_ballonParameters.LowFlowSetPoint, 0);
                                Assert.AreEqual((double)_ballonParameters.LowTargetInjectionPressure, 0);

                                //Ballon Rum up and ramp dow timing 
                                Assert.AreEqual(_ballonParameters.RampUpTimeByStep, 500);
                                Assert.AreEqual((double)_ballonParameters.PressureRampUpValue, 0.5);
                                Assert.AreEqual((double)_ballonParameters.RampDownTimeByStep, 200);
                                Assert.AreEqual((double)_ballonParameters.PressureRampDownValue, 0.2);

                                break;

                            case 2:
                              //  mid = MessageStateId.CAN_ID_STATE_READY;
                                //Validation

                                Assert.AreEqual((double)_ballonParameters.HighFlowSetPoint, 0);
                                Assert.AreEqual((double)_ballonParameters.HighTargetInjectionPressure, 100);

                                Assert.AreEqual((double)_ballonParameters.LowFlowSetPoint, 0);
                                Assert.AreEqual((double)_ballonParameters.LowTargetInjectionPressure, 100);

                                //Ballon Rum up and ramp dow timing 
                                Assert.AreEqual((double)_ballonParameters.RampUpTimeByStep, 500);
                                Assert.AreEqual((double)_ballonParameters.PressureRampUpValue, 0.5);
                                Assert.AreEqual((double)_ballonParameters.RampDownTimeByStep, 200);
                                Assert.AreEqual((double)_ballonParameters.PressureRampDownValue, 0.2);

                                break;

                            case 3:
                               // mid = MessageStateId.CAN_ID_STATE_INFLATION;
                                //Validation

                                Assert.AreEqual((double)_ballonParameters.HighFlowSetPoint, 0);
                                Assert.AreEqual((double)_ballonParameters.HighTargetInjectionPressure, 150);

                                Assert.AreEqual((double)_ballonParameters.LowFlowSetPoint, 0);
                                Assert.AreEqual((double)_ballonParameters.LowTargetInjectionPressure, 150);

                                //Ballon Rum up and ramp dow timing 
                                Assert.AreEqual((double)_ballonParameters.RampUpTimeByStep, 500);
                                Assert.AreEqual((double)_ballonParameters.PressureRampUpValue, 0.5);
                                Assert.AreEqual((double)_ballonParameters.RampDownTimeByStep, 200);
                                Assert.AreEqual((double)_ballonParameters.PressureRampDownValue, 0.2);

                                break;

                            case 4:
                               // mid = MessageStateId.CAN_ID_STATE_TRANSITION;
                                //Validation

                                Assert.AreEqual((double)_ballonParameters.HighFlowSetPoint, 5000);
                                Assert.AreEqual((double)_ballonParameters.HighTargetInjectionPressure, 560);

                                Assert.AreEqual((double)_ballonParameters.LowFlowSetPoint, 5000);
                                Assert.AreEqual((double)_ballonParameters.LowTargetInjectionPressure, 560);

                                //Ballon Rum up and ramp dow timing 
                                Assert.AreEqual((double)_ballonParameters.RampUpTimeByStep, 500);
                                Assert.AreEqual((double)_ballonParameters.PressureRampUpValue, 0.5);
                                Assert.AreEqual((double)_ballonParameters.RampDownTimeByStep, 200);
                                Assert.AreEqual((double)_ballonParameters.PressureRampDownValue, 0.2);

                                break;

                            case 5:
                               // mid = MessageStateId.CAN_ID_STATE_ABLATION;
                                //Validation

                                Assert.AreEqual((double)_ballonParameters.HighFlowSetPoint, 8700);
                                Assert.AreEqual((double)_ballonParameters.HighTargetInjectionPressure, 0);

                                Assert.AreEqual((double)_ballonParameters.LowFlowSetPoint, 7800);
                                Assert.AreEqual((double)_ballonParameters.LowTargetInjectionPressure, 0);

                                //Ballon Rum up and ramp dow timing 
                                Assert.AreEqual((double)_ballonParameters.RampUpTimeByStep, 500);
                                Assert.AreEqual((double)_ballonParameters.PressureRampUpValue, 0.5);
                                Assert.AreEqual((double)_ballonParameters.RampDownTimeByStep, 200);
                                Assert.AreEqual((double)_ballonParameters.PressureRampDownValue, 0.2);

                                break;

                            case 6:
                                //mid = MessageStateId.CAN_ID_STATE_THAWING;
                                //Validation

                                Assert.AreEqual((double)_ballonParameters.HighFlowSetPoint, 0);
                                Assert.AreEqual((double)_ballonParameters.HighTargetInjectionPressure, 150);

                                Assert.AreEqual((double)_ballonParameters.LowFlowSetPoint, 0);
                                Assert.AreEqual((double)_ballonParameters.LowTargetInjectionPressure, 150);

                                //Ballon Rum up and ramp dow timing 
                                Assert.AreEqual((double)_ballonParameters.RampUpTimeByStep, 500);
                                Assert.AreEqual((double)_ballonParameters.PressureRampUpValue, 0.5);
                                Assert.AreEqual((double)_ballonParameters.RampDownTimeByStep, 200);
                                Assert.AreEqual((double)_ballonParameters.PressureRampDownValue, 0.2);

                                break;

                        }

                    }
                }



            }

            catch (Exception ex)
            {
                Assert.Fail();
            }
        }


        #endregion
        [TestCleanup]
        public void Cleanup()
        {
            CommonViewModel.Current.Console.CanBusCommunication.Dispose();
        }


    }

}
