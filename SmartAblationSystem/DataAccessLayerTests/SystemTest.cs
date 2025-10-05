using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartAblationSystem.ViewModels;
using SmartAblationSystem.Helpers;
using SmartAblationSystem.Models;
using System.IO;
using System.Collections.Generic;
using Communication;
using Console;
using DataAccessLayer;
using Moq;
using RS232Communication;
using Unity;
using static Communication.CanBusMessageDefinition;

namespace DataAccessLayerTests
{
    [TestClass]
    public class SystemTest
    {
        double freeSapce = 0;
        private readonly VitalParametersAlerts vitalParametersAlerts = new VitalParametersAlerts();
        private CommonViewModel commonViewModel; 

        [TestInitialize]
        public void Setup()
        {
            commonViewModel = new CommonViewModel(new Machine(new CanBusCommunication(), new GeneralPurposeInputOutput()), new SerialPortManager());

            CommonViewModel.Current.Console.GUIInMaintenanceMode = true;

                foreach (DriveInfo drive in DriveInfo.GetDrives())
                {
                    
                    if(drive.Name == "C:\\")
                    freeSapce = (long)Math.Round((double)drive.TotalFreeSpace / Math.Pow(1024,2), 0);
               }
            
        }

        [TestMethod]
        public void UpperBloodThresholdTest()
        {
            short expectedValue = 75;

            //Set the Upper Blood Threshold
            CommonViewModel.Current.UpperBloodThreshold = 75;

            //Get the Upper Blood Threshold

            short result = CommonViewModel.Current.UpperBloodThreshold;
            Assert.AreEqual(expectedValue, result);


        }


        [TestMethod]
        public void IsBloodDetectorwireOpenTest()
        {
            bool expectedValue = true;

            //Set the blood detector wire to open
            CommonViewModel.Current.IsBloodDetectorwireOpen = true;

            //Get the blood detector wire state

            bool result = CommonViewModel.Current.IsBloodDetectorwireOpen;
            Assert.AreEqual(expectedValue, result);

            expectedValue = false;

            //Set the blood detector wire to close
            CommonViewModel.Current.IsBloodDetectorwireOpen = false;

            //Get the blood detector wire state

            result = CommonViewModel.Current.IsBloodDetectorwireOpen;
            Assert.AreEqual(expectedValue, result);


        }

        [TestMethod]
        public void BloodDetecorImValueTest()
        {
            int expectedValue = 100;

            //Set the blood detecor Im Value
            CommonViewModel.Current.BloodDetecorImValue = 100;

            //Get the blood detecor Im Value

            int result = CommonViewModel.Current.BloodDetecorImValue;
            Assert.AreEqual(expectedValue, result);
            

        }

        [TestMethod]
        public void BloodDetectionTypeTest()
        {
            int expectedValue = 0; // to wire

            //Set the blood detection type
            CommonViewModel.Current.BloodDetectionType = 0;

            //Get the blood detection type

            int result = CommonViewModel.Current.BloodDetectionType;
            Assert.AreEqual(expectedValue, result);

            expectedValue = 1; // to blood

            //Set the blood detection type
            CommonViewModel.Current.BloodDetectionType = 1;

            //Get the blood detection type

            result = CommonViewModel.Current.BloodDetectionType;
            Assert.AreEqual(expectedValue, result);


        }

        [TestMethod]
        public void LowerBloodThresholdTest()
        {
            int expectedValue = 14;

            //Set the lower blood threshold
            CommonViewModel.Current.LowerBloodThreshold = 14;

            //Get the lower blood threshold

            int result = CommonViewModel.Current.LowerBloodThreshold;
            Assert.AreEqual(expectedValue, result);

        }

        [TestMethod]
        public void SetReadyModelParametersTest()
        {
          var unityContainerMock = new Mock<IUnityContainer>();
            CryoTherapyViewModel cryoTherapyViewModel = new CryoTherapyViewModel(unityContainerMock.Object);
            //cryoTherapyViewModel.IsSystemInReady = true;

            CommonViewModel.Current.SystemState = Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY;
            CommonViewModel.Current.IsCatheterValid = true;
            SensorReadingMananger.AreSensorsConnected = true;


            PrivateObject cryoTherapyViewModelObject = new PrivateObject(cryoTherapyViewModel);

            var retVal = cryoTherapyViewModelObject.Invoke("SetReadyModelParameters");

            Assert.AreEqual(cryoTherapyViewModel.IsCatheterConnectedAndInIReadyState, true);

            Assert.AreEqual(cryoTherapyViewModel.IsDiaphragmMovementVisible, true);

            Assert.AreEqual(cryoTherapyViewModel.IsEsophagusTemperatureVisible, true);

            Assert.AreEqual(cryoTherapyViewModel.IsIsolatingVein, false);

            Assert.AreEqual(cryoTherapyViewModel.IsSystemMonitoringDiaphragmAlert, false);

            Assert.AreEqual(cryoTherapyViewModel.AllowPSPChangeDuringThawing, false);

            Assert.AreEqual(cryoTherapyViewModel.IsSystemInReady, true);

            Assert.AreEqual(cryoTherapyViewModel.IsEsophagusTemperatureConditionAlertsMeet, false);

            Assert.AreEqual(cryoTherapyViewModel.DASBalloonEnabled, false);


        }


        [TestMethod]
        public void GetTotalFreeSpaceTest()
        {
            double expectedValue = 0;

            expectedValue = DrivesInformation.GetTotalFreeSpace();

            Assert.AreEqual(expectedValue, freeSapce);

        }


        /// <summary>
        /// here we verify that the blood pressure value are converted correctely
        /// </summary>
        [TestMethod]
        public void BloodPressureFactorTest()
        {

            double[] BloodPressureValueExpectedValue = { 3, 6, 8, 20 };

            byte[] data = new byte[8] {  1, 44, 2, 88, 3, 32, 7, 208 };


            double[] bloodPressureValue = new double[4];
            CanBusMessageConverter.ConverteBloodPressureData(data, out bloodPressureValue);

            Assert.AreEqual(BloodPressureValueExpectedValue[0], bloodPressureValue[0]);
            Assert.AreEqual(BloodPressureValueExpectedValue[1], bloodPressureValue[1]);
            Assert.AreEqual(BloodPressureValueExpectedValue[2], bloodPressureValue[2]);
            Assert.AreEqual(BloodPressureValueExpectedValue[3], bloodPressureValue[3]);


        }


        /// <summary>
        /// here we verify that the remote control Membrane
        /// </summary>
        [TestMethod]
        public void RemoteControlMembraneChangedTest()
        {
            //To complete this integration test we need to use a reel hardware. this function is tested during the DV
            

            try
            {
                CommonViewModel localCommonViewModel = CommonViewModel.Current;

                PrivateObject obj = new PrivateObject(CommonViewModel.Current);

            
                var retVal = obj.Invoke("RemoteControlMembraneChanged");
            }

            catch (Exception ex)
            {
                ex.ToString();


            }

            finally
            {
                //if there is no exception the test is passing
            }
        }



        [TestMethod]
        public void OnEnableDefalteAfterThawCommandTest()
        {
            SiteSetupViewModel siteSetupViewModel = new SiteSetupViewModel();

            siteSetupViewModel.EnableDefalteAfterThaw = true;

            siteSetupViewModel.OnEnableDefalteAfterThawCommand("Test true");

            Assert.AreEqual(CommonViewModel.Current.Console.DeflateAfterThaw, false);

            siteSetupViewModel.EnableDefalteAfterThaw = false;

            siteSetupViewModel.OnEnableDefalteAfterThawCommand("Test false");

            Assert.AreEqual(CommonViewModel.Current.Console.DeflateAfterThaw, true);

        }


        [TestMethod]
        public void ThawingTemperatureSetPointTest()
        {
            double expectedValue = 50;

            //Set the Thawing Temperature Set Point
            CommonViewModel.Current.ThawingTemperatureSetPoint = 50;

            //Get the Thawing Temperature Set Point

            double result = CommonViewModel.Current.ThawingTemperatureSetPoint;
            Assert.AreEqual(expectedValue, result);

        }

        [TestMethod]
        public void ResetDiaphragmReferenceTest()
        {
            double expectedValue = 0;
            CommonViewModel.Current.ResetDiaphragmReference();

            double result = CommonViewModel.Current.MaximumAveragePacingLevel;

            Assert.AreEqual(expectedValue, result);


        }


        [TestMethod]
        public void ConvertLbToKgTest()
        {
            double weight = 130;
            double expected = 58.5;

            double result = result = Scale.ConvertLbToKg(weight);
            Assert.AreEqual(expected, result, 0.001, "Weight not converted properly");
        }


        [TestMethod]
        public void ConvertKgToLbTest()
        {
            double weight = 63;
            double expected = 140;

            double result = result = Scale.ConvertKgToLb(weight);
            Assert.AreEqual(expected, result, 0.001, "Weight not converted properly");
        }


        [TestMethod]
        public void EtsSesnor1Test()
        {
            double expectedValue = 37;

            //Set Ets Sesnor 1
            CommonViewModel.Current.EtsSesnor1 = 37;

            //Get Ets Sesnor 1

            double result = CommonViewModel.Current.EtsSesnor1;
            Assert.AreEqual(expectedValue, result);
        }


        [TestMethod]
        public void EtsSesnor2Test()
        {
            double expectedValue = 37;

            //Set Ets Sesnor 2
            CommonViewModel.Current.EtsSesnor2 = 37;

            //Get Ets Sesnor 2

            double result = CommonViewModel.Current.EtsSesnor2;
            Assert.AreEqual(expectedValue, result);
        }


        [TestMethod]
        public void EtsSesnor3Test()
        {
            double expectedValue = 37;

            //Set Ets Sesnor 3
            CommonViewModel.Current.EtsSesnor3 = 37;

            //Get Ets Sesnor 3

            double result = CommonViewModel.Current.EtsSesnor3;
            Assert.AreEqual(expectedValue, result);
        }


        [TestMethod]
        public void EtsSesnor4Test()
        {
            double expectedValue = 37;

            //Set Ets Sesnor 4
            CommonViewModel.Current.EtsSesnor4 = 37;

            //Get Ets Sesnor 4

            double result = CommonViewModel.Current.EtsSesnor4;
            Assert.AreEqual(expectedValue, result);
        }


        [TestMethod]
        public void EtsSesnor5Test()
        {
            double expectedValue = 37;

            //Set Ets Sesnor 5
            CommonViewModel.Current.EtsSesnor5 = 37;

            //Get Ets Sesnor 5

            double result = CommonViewModel.Current.EtsSesnor5;
            Assert.AreEqual(expectedValue, result);
        }


        [TestMethod]
        public void EtsSesnor6Test()
        {
            double expectedValue = 37;

            //Set Ets Sesnor 6
            CommonViewModel.Current.EtsSesnor6 = 37;

            //Get Ets Sesnor 6

            double result = CommonViewModel.Current.EtsSesnor6;
            Assert.AreEqual(expectedValue, result);
        }


        [TestMethod]
        public void EtsSesnor7Test()
        {
            double expectedValue = 37;

            //Set Ets Sesnor 7
            CommonViewModel.Current.EtsSesnor7 = 37;

            //Get Ets Sesnor 7

            double result = CommonViewModel.Current.EtsSesnor7;
            Assert.AreEqual(expectedValue, result);
        }


        [TestMethod]
        public void EtsSesnor8Test()
        {
            double expectedValue = 37;

            //Set Ets Sesnor 8
            CommonViewModel.Current.EtsSesnor8 = 37;

            //Get Ets Sesnor 8

            double result = CommonViewModel.Current.EtsSesnor8;
            Assert.AreEqual(expectedValue, result);
        }


        [TestMethod]
        public void EtsSesnor9Test()
        {
            double expectedValue = 37;

            //Set Ets Sesnor 9
            CommonViewModel.Current.EtsSesnor9 = 37;

            //Get Ets Sesnor 9

            double result = CommonViewModel.Current.EtsSesnor9;
            Assert.AreEqual(expectedValue, result);
        }


        [TestMethod]
        public void EtsSesnor10Test()
        {
            double expectedValue = 37;

            //Set Ets Sesnor 10
            CommonViewModel.Current.EtsSesnor10 = 37;

            //Get Ets Sesnor 10

            double result = CommonViewModel.Current.EtsSesnor10;
            Assert.AreEqual(expectedValue, result);
        }


        [TestMethod]
        public void EtsSesnor11Test()
        {
            double expectedValue = 37;

            //Set Ets Sesnor 11
            CommonViewModel.Current.EtsSesnor11 = 37;

            //Get Ets Sesnor 11

            double result = CommonViewModel.Current.EtsSesnor11;
            Assert.AreEqual(expectedValue, result);
        }


        [TestMethod]
        public void EtsSesnor12Test()
        {
            double expectedValue = 37;

            //Set Ets Sesnor 12
            CommonViewModel.Current.EtsSesnor12 = 37;

            //Get Ets Sesnor 12

            double result = CommonViewModel.Current.EtsSesnor12;
            Assert.AreEqual(expectedValue, result);
        }


        [TestMethod]
        public void EtsSesnor13Test()
        {
            double expectedValue = 12;

            //Set Ets Sesnor 13
            CommonViewModel.Current.EtsSesnor13 = 12;

            //Get Ets Sesnor 13

            double result = CommonViewModel.Current.EtsSesnor13;
            Assert.AreEqual(expectedValue, result);
        }


        [TestMethod]
        public void TIPTest()
        {
            double expectedValue = 1000;

            //Set TIP
            CommonViewModel.Current.TIP = 1000;

            //Get TIP

            double result = CommonViewModel.Current.TIP;
            Assert.AreEqual(expectedValue, result);
        }


        [TestMethod]
        public void listOfSesnorsStateTest()
        {
            List<int> expectedValue = new List<int>() { 1, 2, 3, 4, 5, 6, 2, 11 };

            //Set list of sesnors state
            CommonViewModel.Current.ListOfSesnorsState = new List<int>() { 1, 2, 3, 4, 5, 6, 2, 11 };

            //Get list of sesnors state

            var result = CommonViewModel.Current.ListOfSesnorsState;       //listOfSesnorsState.;
            CollectionAssert.AreEqual(expectedValue, result);
        }


        [TestMethod]
        public void MinimumTemperatureTest()
        {
            double expectedValue = 37;

            //Set Minimum Temperature
            CommonViewModel.Current.MinimumTemperature = 37;

            //Get Minimum Temperature

            double result = CommonViewModel.Current.MinimumTemperature;
            Assert.AreEqual(expectedValue, result);
        }


        [TestMethod]
       
        public void ShouldDiaphragmMovementAlertTriggedTest()
        {

            
            bool isDiaphragmMovementDetected = false;
            bool isDiaphragmAmplitudeThresholdReached = true;
            CanBusMessageDefinition.MessageStateId systemState = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION;
            int lastDiaphragmMovementPercentageOrGReadingValue = 50;

            bool expected = true;

            bool result = vitalParametersAlerts.ShouldDiaphragmMovementAlertTrigged(isDiaphragmMovementDetected, isDiaphragmAmplitudeThresholdReached, systemState, lastDiaphragmMovementPercentageOrGReadingValue);
            Assert.AreEqual(expected, result);
        }


        //[TestMethod]
        //public void ShouldEsophagusTemperatureAlertTriggedTest()
        //{
        //   bool isEsophagusTemperatureThresholdReached = true;
        //    CanBusMessageDefinition.MessageStateId systemState = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE;    // 256

        //    bool expected = false;

        //    bool result = vitalParametersAlerts.ShouldEsophagusTemperatureAlertTrigged(isEsophagusTemperatureThresholdReached, systemState);
        //    Assert.AreEqual(expected, result);
        //}

        //Test case parameter PUT HERE also for mock
     
        [TestMethod]
        public void OBPThresholdLeftBound()
        {
            PrivateObject commonViewModelObject = new PrivateObject(commonViewModel);
            object[] args = new object[1] {1};

            commonViewModel.PT3Reading = 20; //Calculates a value of -15.3 < -12
            var expected = -12;
            commonViewModelObject.Invoke("initializeRegistersAccordingToCatheterID",args);
            var result = commonViewModel.Console.PatientPressureTransducerOneValueAccordingToTheStateMachine[MessageStateId.CAN_ID_STATE_READY].PressureLowRangeLimit;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void OBPThresholdMin()
        {
            PrivateObject commonViewModelObject = new PrivateObject(commonViewModel);
            object[] args = new object[1] { 1 };

            commonViewModel.PT3Reading = 16.7; //Calculates a value of -12
            var expected = -12;
            commonViewModelObject.Invoke("initializeRegistersAccordingToCatheterID", args);
            var result = commonViewModel.Console.PatientPressureTransducerOneValueAccordingToTheStateMachine[MessageStateId.CAN_ID_STATE_READY].PressureLowRangeLimit;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void OBPThresholdNormal()
        {
            PrivateObject commonViewModelObject = new PrivateObject(commonViewModel);
            object[] args = new object[1] { 1 };

            commonViewModel.PT3Reading = 16; //Calculates a value of -11.3
            var expected = -11.3;
            commonViewModelObject.Invoke("initializeRegistersAccordingToCatheterID", args);
            var result = commonViewModel.Console.PatientPressureTransducerOneValueAccordingToTheStateMachine[MessageStateId.CAN_ID_STATE_READY].PressureLowRangeLimit;
            Assert.AreEqual(expected, result);
        }
        [TestMethod]
        public void OBPThresholdMax()
        {
            PrivateObject commonViewModelObject = new PrivateObject(commonViewModel);
            object[] args = new object[1] { 1 };

            commonViewModel.PT3Reading = 10.7; //Calculates a value of -6
            var expected = -6;
            commonViewModelObject.Invoke("initializeRegistersAccordingToCatheterID", args);
            var result = commonViewModel.Console.PatientPressureTransducerOneValueAccordingToTheStateMachine[MessageStateId.CAN_ID_STATE_READY].PressureLowRangeLimit;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void OBPThresholdRightBound()
        {
            PrivateObject commonViewModelObject = new PrivateObject(commonViewModel);
            object[] args = new object[1] { 1 };

            commonViewModel.PT3Reading = 10; //Calculates a value of -5.3 > 6
            var expected = -6;
            commonViewModelObject.Invoke("initializeRegistersAccordingToCatheterID", args);
            var result = commonViewModel.Console.PatientPressureTransducerOneValueAccordingToTheStateMachine[MessageStateId.CAN_ID_STATE_READY].PressureLowRangeLimit;
            Assert.AreEqual(expected, result);
        }
        [TestMethod]
        public void ShouldDMSAlertProduceSoundInTransition()
        {
            VitalParametersAlerts vitalAlerts = new VitalParametersAlerts();
            var result = vitalAlerts.ShouldDiaphragmMovementAlertTrigged(true,true, CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION, 50);
            var expected = true;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ShouldDMSAlertProduceSoundInAblation()
        {
            VitalParametersAlerts vitalAlerts = new VitalParametersAlerts();
            var result = vitalAlerts.ShouldDiaphragmMovementAlertTrigged(true, true, CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_ABLATION, 50);
            var expected = true;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ShouldNotDMSAlertProduceSoundInOther()
        {
            VitalParametersAlerts vitalAlerts = new VitalParametersAlerts();
            var result = vitalAlerts.ShouldDiaphragmMovementAlertTrigged(true, true, CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE, 50);
            var expected = false;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ShouldNotETSAlertProduceSoundInIdle()
        {
            VitalParametersAlerts vitalAlerts = new VitalParametersAlerts();
            var result = vitalAlerts.ShouldEsophagusTemperatureAlertTrigged(true,CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE);
            var expected = false;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ShouldNotETSAlertProduceSoundInReady()
        {
            VitalParametersAlerts vitalAlerts = new VitalParametersAlerts();
            var result = vitalAlerts.ShouldEsophagusTemperatureAlertTrigged(true, CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY);
            var expected = false;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ShouldETSAlertProduceSoundInOtherState()
        {
            VitalParametersAlerts vitalAlerts = new VitalParametersAlerts();
            var result = vitalAlerts.ShouldEsophagusTemperatureAlertTrigged(true, CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_ABLATION);
            var expected = true;
            Assert.AreEqual(expected, result);
        }
        [TestCleanup]
        public void Cleanup()
        {
            CommonViewModel.Current.Console.CanBusCommunication.Dispose();
        }
    }
}
