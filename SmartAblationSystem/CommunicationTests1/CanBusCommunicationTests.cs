using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartAblationSystem.ViewModels;
using System;
using static Communication.CanBusMessageDefinition;
using static System.Net.Mime.MediaTypeNames;

namespace Communication.Tests
{
    [TestClass()]
    public class CanBusCommunicationTests
    {
        private CanBusCommunication canBusCommunication;

        private int state = Array.IndexOf(Enum.GetValues(typeof(MessageStateId)), MessageStateId.CAN_ID_STATE_IDLE);
        private int localSelectedRegisterForTargetInjectionFlow = 15;
        private int maxWritingTime = 8;




        [TestInitialize]
        public void Setup()
        {
            CommonViewModel.initialize();
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
            

            for (int i = 0; i < maxWritingTime; i++)
            {
                CommonViewModel.Current.Console.WriteFromMicroController((MessageStateId)state, localSelectedRegisterForTargetInjectionFlow);
                System.Threading.Thread.Sleep(20);
            }


        }


        /// <summary>
        /// These Test validate the reading not fail as a result of an exception
        ///  If an excpetion occures the test fail
        /// </summary>
        [TestMethod()]
        public void ReadTest()
        {
           

            for (int i = 0; i < maxWritingTime; i++)
            {
                CommonViewModel.Current.Console.ReadFromMicroController((MessageStateId)state, localSelectedRegisterForTargetInjectionFlow);
                System.Threading.Thread.Sleep(20);
            }
        }

        /// <summary>
        /// These test is the most importane Test for the comunication. It is validating that the data 
        /// sent to the firmware is not corrupted, recived correctly and read corrctely.  Here we send a value and we read it  again. 
        /// here we are simulating 100 0000 read write  on can bus 
        /// </summary>
        [TestMethod()]
        public void WriteReadVerificationTest()
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

        [TestMethod()]
        public void TestGpioLevels()
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

        [TestCleanup]
        public void Cleanup()
        {
            CommonViewModel.Current.Console.CanBusCommunication.Dispose();
           

        }

        


    }
}