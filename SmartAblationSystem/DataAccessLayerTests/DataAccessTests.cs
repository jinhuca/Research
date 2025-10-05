using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using static Communication.CanBusMessageDefinition;
using System.Data.SqlClient;
using SmartAblationSystem.ViewModels;
using System.Collections.ObjectModel;

namespace DataAccessLayer.Tests
{
    [TestClass()]
    public class DataAccessTests
    {
        private List<string> pMCRegisterResults;
        private List<string> cMCRegisterResults;
        private bool generateReport = false;
        private int testNumber = 0;

        private bool runningPMC = false;
        DataAccess data;

        [TestInitialize]
        public void Setup()
        {
            data = new DataAccess();

            int catheterId = 1;

            data.AddCatheterInformation(255, 255, 1, new DateTime(2019, 1, 1), new DateTime(2018, 1, 16, 11, 0, 0), 0, catheterId, false, catheterId);
        }

        [TestMethod()]
        public void GetRegisterValues5YearsTest()
        {
            FileSerializer.CSVManager csvManager = new FileSerializer.CSVManager();
            generateReport = true;
            testNumber = 1;

            //Run tests that simulates readings each days for 5 years using 2 catheters per day.
            for (int i = 0; i < 365*5*2; i++)
            {
                runningPMC = true;
                GetPMCRegisterValuesAccordingToCatheterIDTest();
                runningPMC = false;
                GetCMCRegisterValuesAccordingToCatheterIDTest();
                testNumber++;
            }

            //Write reports in a separate file for PMC and CMC registers results.
            string filename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                           "TestResults_PMCRegister_" +
                                           DateTime.Now.Year + "_" +
                                           DateTime.Now.Month + "_" +
                                           DateTime.Now.Day + "_" +
                                           DateTime.Now.Hour + "h_" +
                                           DateTime.Now.Minute + "m_" +
                                           DateTime.Now.Second + "sec");
            csvManager.GenerateAndWriteTestReport(pMCRegisterResults, filename);

            filename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                           "TestResults_CMCRegister_" +
                                           DateTime.Now.Year + "_" +
                                           DateTime.Now.Month + "_" +
                                           DateTime.Now.Day + "_" +
                                           DateTime.Now.Hour + "h_" +
                                           DateTime.Now.Minute + "m_" +
                                           DateTime.Now.Second + "sec");
            csvManager.GenerateAndWriteTestReport(pMCRegisterResults, filename);
        }

        private void RunAssert(int expected, int actual, string testName)
        {
            Assert.AreEqual(expected, actual);
            GenerateReportLine(expected, actual, testName);
        }

        private void RunAssert(double expected, double actual, string testName)
        {
            Assert.AreEqual(expected, actual);
            GenerateReportLine(expected, actual, testName);
        }

        private void RunAssert(DateTime expected, DateTime actual, string testName)
        {
            Assert.AreEqual(expected, actual);
            GenerateReportLine(expected, actual, testName);
        }


        private void RunAssert(bool expected, bool actual, string testName)
        {
            Assert.AreEqual(expected, actual);
            GenerateReportLine(expected, actual, testName);
        }

        private void GenerateReportLine(int expectedValue, int actualValue, string testName)
        {
            string line = "";

            if (generateReport)
            {
                line = testNumber + "," +
                       testName + "," +
                       expectedValue + "," +
                       actualValue + "," +
                       (expectedValue == actualValue ? "PASS" : "FAIL");

                if (runningPMC)
                {
                    if (pMCRegisterResults == null)
                        pMCRegisterResults = new List<string>();

                    pMCRegisterResults.Add(line);
                }
                else
                {
                    if (cMCRegisterResults == null)
                        cMCRegisterResults = new List<string>();

                    cMCRegisterResults.Add(line);
                }
            }
        }


        private void GenerateReportLine(bool expectedValue, bool actualValue, string testName)
        {
            string line = "";

            if (generateReport)
            {
                line = testNumber + "," +
                       testName + "," +
                       expectedValue + "," +
                       actualValue + "," +
                       (expectedValue == actualValue ? "PASS" : "FAIL");

                if (runningPMC)
                {
                    if (pMCRegisterResults == null)
                        pMCRegisterResults = new List<string>();

                    pMCRegisterResults.Add(line);
                }
                else
                {
                    if (cMCRegisterResults == null)
                        cMCRegisterResults = new List<string>();

                    cMCRegisterResults.Add(line);
                }
            }
        }




        private void GenerateReportLine(double expectedValue, double actualValue, string testName)
        {
            string line = "";

            if (generateReport)
            {
                line = testNumber + "," +
                       testName + "," +
                       expectedValue + "," +
                       actualValue + "," +
                       (expectedValue == actualValue ? "PASS" : "FAIL");

                if (runningPMC)
                {
                    if (pMCRegisterResults == null)
                        pMCRegisterResults = new List<string>();

                    pMCRegisterResults.Add(line);
                }
                else
                {
                    if (cMCRegisterResults == null)
                        cMCRegisterResults = new List<string>();

                    cMCRegisterResults.Add(line);
                }
            }
        }

        private void GenerateReportLine(DateTime expectedValue, DateTime actualValue, string testName)
        {
            string line = "";

            if (generateReport)
            {
                line = testNumber + "," +
                       testName + "," +
                       expectedValue + "," +
                       actualValue + "," +
                       (expectedValue.Equals(actualValue) ? "PASS" : "FAIL");

                if (runningPMC)
                {
                    if (pMCRegisterResults == null)
                        pMCRegisterResults = new List<string>();

                    pMCRegisterResults.Add(line);
                }
                else
                {
                    if (cMCRegisterResults == null)
                        cMCRegisterResults = new List<string>();

                    cMCRegisterResults.Add(line);
                }
            }
        }
        [TestMethod()]
        public void GetCatheterIDTest()
        {
            int CatheterID = 1;
            int CatID = 1;
            RunAssert(CatID, data.GetCatheterId(CatheterID), "CatID");
        }


        [TestMethod()]
        public void GetExistingUserTypeTest()
        {
            User user = data.GetUser("BSC");
            RunAssert(3, data.GetUserType(user), "UserType:3");

        }

        [TestMethod()]
        public void GetNotExistingUserTypeTest()
        {
            User user = data.GetUser("NotUser");
            RunAssert(0, data.GetUserType(user), "UserType:0");

        }
        [TestMethod()]
        public void GetHardDriveLimitsTest()
        {

            long Item1;
            long Item2;
            long WarningLimit = 1500;
            long FailLimit = 1000;
            Tuple<Int64, Int64> HardDriveLimits = data.GetHardDriveLimits();

            Item1 = HardDriveLimits.Item1;
            Item2 = HardDriveLimits.Item2;

            RunAssert(WarningLimit, Item1, "WarningLimit");
            RunAssert(FailLimit, Item2, "FailLimit");
        }

        [TestMethod()]
        public void ChangeTankMetalWeightTest()
        {
            int Id = 1;
            double metalweightTest = 99.88;
            double metalweight;
            metalweight = data.GetTankTypes(Id).MetalWeight;
            data.ChangeTankMetalWeight(Id, metalweightTest);
            RunAssert(metalweightTest, data.GetTankTypes(Id).MetalWeight, "TankID");
            data.ChangeTankMetalWeight(Id, metalweight);
        }

        [TestMethod()]
        public void GetDASBallonParametersTest()
        {

            double[] LowPressureSetpoint = { 2.5, 2.5, 2.5, 2.5, 2.5, 2.5 };
            double[] HighPressureSetPoint = { 7.5, 7.5, 7.5, 7.5, 7.5, 7.5 };
            double[] LowFlowSetPoint = { 0, 0, 0, 5000, 7800, 0 };
            double[] HighFlowSetPoint = { 0, 0, 0, 5000, 8700, 0 };
            double[] RampUpTimeByStep = { 500, 500, 500, 500, 500, 500 };
            double[] PressureRampUpValue = { 0.5, 0.5, 0.5, 0.5, 0.5, 0.5 };
            double[] RampDownTimeByStep = { 200, 200, 200, 200, 200, 200 };
            double[] PressureRampDownValue = { 0.2, 0.2, 0.2, 0.2, 0.2, 0.2 };
            double[] TotalRampUpTime = { 10000, 10000, 10000, 10000, 10000, 10000 };
            double[] TotalRampDowntime = { 10000, 10000, 10000, 10000, 10000, 10000 };
            double[] LowTargetInjectionPressure = { 0, 100, 150, 560, 0, 150 };
            double[] HighTargetInjectionPressure = { 0, 100, 150, 560, 0, 150 };
            double[] DASLowFlow = {0,0,0,5000,7800,0 };
            List<BalloonParameters> ballonParameters = data.GetDASBallonParameters();

            foreach (BalloonParameters ballonParameter in ballonParameters)
            {

                int index = ballonParameter.StateID - 1;

                if (ballonParameter.StateID > 0 && ballonParameter.StateID < 7)
                {
                    RunAssert(LowPressureSetpoint[index], (double)ballonParameter.LowPressureSetpoint, "LowPressureSetpoint");
                    RunAssert(HighPressureSetPoint[index], (double)ballonParameter.HighPressureSetPoint, "HighPressureSetPoint");
                    RunAssert(LowFlowSetPoint[index], (double)ballonParameter.LowFlowSetPoint, "LowFlowSetPoint");
                    RunAssert(HighFlowSetPoint[index], (double)ballonParameter.HighFlowSetPoint, "HighFlowSetPointt");
                    RunAssert(RampUpTimeByStep[index], (double)ballonParameter.RampUpTimeByStep, "RampUpTimeByStep");
                    RunAssert(PressureRampUpValue[index], (double)ballonParameter.PressureRampUpValue, "PressureRampUpValue");
                    RunAssert(RampDownTimeByStep[index], (double)ballonParameter.RampDownTimeByStep, "RampDownTimeByStep");
                    RunAssert(PressureRampDownValue[index], (double)ballonParameter.PressureRampDownValue, "PressureRampDownValue");
                    RunAssert(TotalRampUpTime[index], (double)ballonParameter.TotalRampUpTime, "TotalRampUpTime");
                    RunAssert(LowTargetInjectionPressure[index], ballonParameter.LowTargetInjectionPressure, "LowTargetInjectionPressure");
                    RunAssert(HighTargetInjectionPressure[index], ballonParameter.HighTargetInjectionPressure, "HighTargetInjectionPressure");
                    RunAssert(DASLowFlow[index], ballonParameter.DASLowFlow, "DASLowFlow");
                }
            }
        }



        [TestMethod()]
        public void UpdateBalloonParametersValuesTest()
        {
            int stateid = 1;
            double rampUpTimeByStepTest = 500.01;
            double pressureRampUpValueTest = 0.51;
            double rampDownTimeByStepTest = 200.99;
            double pressureRampDownValueTest = 0.22;

            double rampUpTimeByStep;
            double pressureRampUpValue;
            double rampDownTimeByStep;
            double pressureRampDownValue;

            double rampUpTimeByStepTest2;
            double pressureRampUpValueTest2;
            double rampDownTimeByStepTest2;
            double pressureRampDownValueTest2;

            List<BalloonParameters> ballonParameters = data.GetDASBallonParameters();
            rampUpTimeByStep = (double)ballonParameters[0].RampUpTimeByStep;
            pressureRampUpValue = (double)ballonParameters[0].PressureRampUpValue;
            rampDownTimeByStep = (double)ballonParameters[0].RampDownTimeByStep;
            pressureRampDownValue = (double)ballonParameters[0].PressureRampDownValue;

            data.UpdateBalloonParametersValues(stateid, rampUpTimeByStepTest, pressureRampUpValueTest, rampDownTimeByStepTest, pressureRampDownValueTest);

            List<BalloonParameters> ballonParameters2 = data.GetDASBallonParameters();
            rampUpTimeByStepTest2 = (double)ballonParameters2[0].RampUpTimeByStep;
            pressureRampUpValueTest2 = (double)ballonParameters2[0].PressureRampUpValue;
            rampDownTimeByStepTest2 = (double)ballonParameters2[0].RampDownTimeByStep;
            pressureRampDownValueTest2 = (double)ballonParameters2[0].PressureRampDownValue;


            RunAssert(rampUpTimeByStepTest, rampUpTimeByStepTest2, "rampUpTimeByStep");
            RunAssert(pressureRampUpValueTest, pressureRampUpValueTest2, "pressureRampUpValue");
            RunAssert(rampDownTimeByStepTest, rampDownTimeByStepTest2, "rampDownTimeByStep");
            RunAssert(pressureRampDownValueTest, pressureRampDownValueTest2, "pressureRampDownValue");
            data.UpdateBalloonParametersValues(stateid, rampUpTimeByStep, pressureRampUpValue, rampDownTimeByStep, pressureRampDownValue);
        }

        [TestMethod()]
        public void GetPMCRegisterValuesAccordingToCatheterIDTest()
        {
            int CatheterID = 1;  // or CatheterID=2;

            #region value to assert

            //These are the values to be verified.
            double[] CP1PressureThresholdHighLimit = { 10, 10, 10, 10, 10, 10, 10 };
            double[] CP1PressureLowRangeLimit = { 10, 4.7, 4.7, 4.7, 4.7, 4.7, 4.7 };
            double[] CP1PressureHighRangeLimit = { -20, -20, -20, 0, 0, 0, -20 };
            double[] CP2PressureThresholdHighLimit = { 1000, -10, -10, -10, -10, -10, -10 };
            double[] CP2PressureLowRangeLimit = { 0, 0, 0, 0, 0, 0, 0 };
            double[] CP2PressureHighRangeLimit = { 1000, 1000, 1000, 1000, 1000, 1000, 1000 };
            double[] ThawingTemperature = { -70, -70, -70, -70, -70, -70, -70 };
            double[] PGain = { 0, 0, 40, 20, 15, 40, 0 };
            double[] IGain = { 0, 0, 40, 20, 15, 20, 0 };
            double[] DGain = { 0, 0, 0, 0, 0, 0, 0 };
            double[] Offset = { 0, 40, 20, 20, 20, 20, 0 };
            double[] TargetBalloonPressure = { 2.5, 2.5, 2.5, 2.5, 2.5, 2.5, 2.5 };
            Int16[] LowerBloodThreshold = {14, 14, 14, 14, 14, 14, 14 };
            Int16[] UpperBloodThreshold = {75, 75, 75, 75, 75, 75, 75};
            double[] ThawingTemperatureSetPoint = {50, 50, 50, 50, 50, 20, 50 };
            


            CatheterType catheterType = data.GetCatheterAccordingToCatheterId(CatheterID);

            List<PMCRegisterValue> pMCRegisterValues = data.GetPMCRegisterValuesAccordingToCatheterID(catheterType.ID);
            
            // Initialize Patient Micro Controller Register
            foreach (PMCRegisterValue pMCRegisterValue in pMCRegisterValues)
            {
                int index = pMCRegisterValue.StateID - 1;

                if (pMCRegisterValue.StateID > 0 && pMCRegisterValue.StateID < 8)
                {
                    RunAssert(CP1PressureThresholdHighLimit[index], pMCRegisterValue.CP1PressureThresholdHighLimit, "CP1PressureThresholdHighLimit");
                    RunAssert(CP1PressureLowRangeLimit[index], pMCRegisterValue.CP1PressureLowRangeLimit, "CP1PressureLowRangeLimit");
                    RunAssert(CP1PressureHighRangeLimit[index], pMCRegisterValue.CP1PressureHighRangeLimit, "CP1PressureHighRangeLimit");

                    RunAssert(CP2PressureThresholdHighLimit[index], pMCRegisterValue.CP2PressureThresholdHighLimit, "CP2PressureThresholdHighLimit");
                    RunAssert(CP2PressureLowRangeLimit[index], pMCRegisterValue.CP2PressureLowRangeLimit, "CP2PressureLowRangeLimit");
                    RunAssert(CP2PressureHighRangeLimit[index], pMCRegisterValue.CP2PressureHighRangeLimit, "CP2PressureHighRangeLimit");
                    RunAssert(ThawingTemperature[index], pMCRegisterValue.TC1ThawingTemperature, "TC1ThawingTemperature");

                    RunAssert(PGain[index], pMCRegisterValue.Pgain, "Pgain");
                    RunAssert(IGain[index], pMCRegisterValue.Igain, "Igain");
                    RunAssert(DGain[index], pMCRegisterValue.Dgain, "Dgain");
                    RunAssert(Offset[index], pMCRegisterValue.Offset, "Offset");
                    RunAssert(TargetBalloonPressure[index], pMCRegisterValue.TargetBalloonPressure, "TargetBalloonPressure");
                    RunAssert(LowerBloodThreshold[index], pMCRegisterValue.LowerBloodThreshold, "LowerBloodThreshold");
                    RunAssert(UpperBloodThreshold[index], pMCRegisterValue.UpperBloodThreshold, "UpperBloodThreshold");
                    RunAssert(ThawingTemperatureSetPoint[index], pMCRegisterValue.ThawingTemperatureSetPoint, "ThawingTemperatureSetPoint");
                  

                }
            }

            #endregion patient region
        }

        [TestMethod()]
        public void GetCMCRegisterValuesAccordingToCatheterIDTest()
        {
            int CatheterID = 1;

            #region value to assert

            //These are the values to be verified.
            double[] PT1TankPressureLow = { 680, 680, 680, 680, 680, 680 };
            double[] PT1PressureThresholdHighLimit = { 850, 850, 850, 850, 850, 850 };
            double[] PT1TankPressureTooHigh = { 975, 975, 975, 975, 975, 975 };
            double[] PT1PressureLowRangeLimit = { 0, 0, 0, 0, 0, 0 };
            double[] PT1PressureHighRangeLimit = { 0, 0, 0, 0, 0, 0 };

            double[] PT2PressureThresholdHighLimit = { 800, 800, 250, 680, 680, 680 };
            double[] PT2PressureLowRangeLimit = { 0, 0, 0, 0, 0, 0 };
            double[] PT2PressureHighRangeLimit = { 0, 0, 0, 0, 0, 0 };

            double[] PT3PressureThresholdHighLimit = { 30, 7, 25, 25, 25, 25 };
            double[] PT3PressureLowRangeLimit = { 0, 0, 0, 0, 0, 0 };
            double[] PT3PressureHighRangeLimit = { 0, 0, 0, 0, 0, 0 };

            double[] PT4PressureThresholdHighLimit = { 11, 11, 11, 11, 11, 11 };
            double[] PT4PressureLowRangeLimit = { 0, 0, 0, 0, 0, 0 };
            double[] PT4PressureHighRangeLimit = { 0, 0, 0, 0, 0, 0 };

            double[] TS1TemperatureThresholdHighLimit = { -10, -10, -10, -10, -10, -10 };
            double[] TS1TemperatureLowRangeLimit = { 0, 0, 0, 0, 0, 0 };
            double[] TS1TemperatureHighRangeLimit = { 0, 0, 0, 0, 0, 0 };

            double[] FM1FlowMeterThresholLowlimit = { 0, 0, 0, -5100, 3000, 0 };
            double[] FM1FlowMeterThresholHighlimit = { 3000, 3000, 5000, 10000, 10000, 10000 };
            double[] FM1FlowMeterLowRangeLimit = { 0, 0, 0, 34, 0, 0 };
            double[] FM1FlowMeterHighRangelimit = { 0, 0, 0, 2000, 0, 0 };

            double[] PS1PressureThresholdHighLimit = { 100, 100, 10, 10, 10, 10 };
            double[] PS1PressureLowRangeLimit = { 0, 0, 0, 0, 0, 0 };
            double[] PS1PressureHighRangeLimit = { 0, 0, 0, 0, 0, 0 };

            double[] PS2PressureThresholdHighLimit = { 19, 19, 19, 19, 19, 19 };
            double[] PS2PressureLowRangeLimit = { 0, 0, 0, 0, 0, 0 };
            double[] PS2PressureHighRangeLimit = { 0, 0, 0, 0, 0, 0 };

            double[] LC1LoadCellThresholdWarning = { 3.5, 3.5, 3.5, 3.5, 3.5, 3.5 };
            double[] LC1LoadCellThresholdFail = { 2.5, 2.5, 2.5, 2.5, 2.5, 2.5 };
            double[] LC1LoadCellLowRangeLimit = { 0, 0, 0, 0, 0, 0 };
            double[] LC1LoadCellHighRangeLimit = { 0, 0, 0, 0, 0, 0 };

            double[] PGain = { 0, 75, 20, 60, 20, 20 };
            double[] IGain = { 0, 60, 5, 12, 10, 5 };
            double[] DGain = { 0, 0, 0, 0, 0, 0 };
            double[] Offset = { 0, 20, 20, 20, 20, 20 };


            double[] TargetInjectionFlow = { 0, 0, 0, 5000, 7800, 0 };
            double[] TargetInjectionPressure = { 0, 100, 150, 560, 0, 150 };


            CatheterType catheterType = data.GetCatheterAccordingToCatheterId(CatheterID);

            List<CMCRegisterValue> pCMCRegisterValues = data.GetCMCRegisterValuesAccordingToCatheterID(catheterType.ID);

            // Initialize Central Micro Controller Register
            foreach (CMCRegisterValue cMCRegisterValue in pCMCRegisterValues)
            {
                int index = cMCRegisterValue.StateID - 1;

                if (cMCRegisterValue.StateID > 0 && cMCRegisterValue.StateID < 7)
                {
                    RunAssert(PT1TankPressureLow[index], cMCRegisterValue.PT1TankPressureLow, "PT1TankPressureLow");
                    RunAssert(PT1PressureThresholdHighLimit[index], cMCRegisterValue.PT1PressureThresholdHighLimit, "PT1PressureThresholdHighLimit");
                    RunAssert(PT1TankPressureTooHigh[index], cMCRegisterValue.PT1TankPressureTooHigh, "PT1TankPressureTooHigh");
                    RunAssert(PT1PressureLowRangeLimit[index], cMCRegisterValue.PT1PressureLowRangeLimit, "PT1PressureLowRangeLimit");
                    RunAssert(PT1PressureHighRangeLimit[index], cMCRegisterValue.PT1PressureHighRangeLimit, "PT1PressureHighRangeLimit");

                    RunAssert(PT2PressureThresholdHighLimit[index], cMCRegisterValue.PT2PressureThresholdHighLimit, "PT2PressureThresholdHighLimit");
                    RunAssert(PT2PressureLowRangeLimit[index], cMCRegisterValue.PT2PressureLowRangeLimit, "PT2PressureLowRangeLimit");
                    RunAssert(PT2PressureHighRangeLimit[index], cMCRegisterValue.PT2PressureHighRangeLimit, "PT2PressureHighRangeLimit");

                    RunAssert(PT3PressureThresholdHighLimit[index], cMCRegisterValue.PT3PressureThresholdHighLimit, "PT3PressureThresholdHighLimit");
                    RunAssert(PT3PressureLowRangeLimit[index], cMCRegisterValue.PT3PressureLowRangeLimit, "PT3PressureLowRangeLimit");
                    RunAssert(PT3PressureHighRangeLimit[index], cMCRegisterValue.PT3PressureHighRangeLimit, "PT3PressureHighRangeLimit");

                    RunAssert(PT4PressureThresholdHighLimit[index], cMCRegisterValue.PT4PressureThresholdHighLimit, "PT4PressureThresholdHighLimit");
                    RunAssert(PT4PressureLowRangeLimit[index], cMCRegisterValue.PT4PressureLowRangeLimit, "PT4PressureLowRangeLimit");
                    RunAssert(PT4PressureHighRangeLimit[index], cMCRegisterValue.PT4PressureHighRangeLimit, "PT4PressureHighRangeLimit");

                    RunAssert(TS1TemperatureThresholdHighLimit[index], cMCRegisterValue.TS1TemperatureThresholdHighLimit, "TS1TemperatureThresholdHighLimit");
                    RunAssert(TS1TemperatureLowRangeLimit[index], cMCRegisterValue.TS1TemperatureLowRangeLimit, "TS1TemperatureLowRangeLimit");
                    RunAssert(TS1TemperatureHighRangeLimit[index], cMCRegisterValue.TS1TemperatureHighRangeLimit, "TS1TemperatureHighRangeLimit");

                    RunAssert(FM1FlowMeterThresholLowlimit[index], cMCRegisterValue.FM1FlowMeterThresholLowlimit, "FM1FlowMeterThresholLowlimit");
                    RunAssert(FM1FlowMeterThresholHighlimit[index], cMCRegisterValue.FM1FlowMeterThresholHighlimit, "FM1FlowMeterThresholHighlimit");
                    RunAssert(FM1FlowMeterLowRangeLimit[index], cMCRegisterValue.FM1FlowMeterLowRangeLimit, "FM1FlowMeterLowRangeLimit");
                    RunAssert(FM1FlowMeterHighRangelimit[index], cMCRegisterValue.FM1FlowMeterHighRangelimit, "FM1FlowMeterHighRangelimit");

                    RunAssert(PS1PressureThresholdHighLimit[index], cMCRegisterValue.PS1PressureThresholdHighLimit, "PS1PressureThresholdHighLimit");
                    RunAssert(PS1PressureLowRangeLimit[index], cMCRegisterValue.PS1PressureLowRangeLimit, "PS1PressureLowRangeLimit");
                    RunAssert(PS1PressureHighRangeLimit[index], cMCRegisterValue.PS1PressureHighRangeLimit, "PS1PressureHighRangeLimit");

                    RunAssert(PS2PressureThresholdHighLimit[index], cMCRegisterValue.PS2PressureThresholdHighLimit, "PS2PressureThresholdHighLimit");
                    RunAssert(PS2PressureLowRangeLimit[index], cMCRegisterValue.PS2PressureLowRangeLimit, "PS2PressureLowRangeLimit");
                    RunAssert(PS2PressureHighRangeLimit[index], cMCRegisterValue.PS2PressureHighRangeLimit, "PS2PressureHighRangeLimit");

                    RunAssert(LC1LoadCellThresholdWarning[index], cMCRegisterValue.LC1LoadCellThresholdWarning, "LC1LoadCellThresholdWarning");
                    RunAssert(LC1LoadCellThresholdFail[index], cMCRegisterValue.LC1LoadCellThresholdFail, "LC1LoadCellThresholdFail");
                    RunAssert(LC1LoadCellLowRangeLimit[index], cMCRegisterValue.LC1LoadCellLowRangeLimit, "LC1LoadCellLowRangeLimit");
                    RunAssert(LC1LoadCellHighRangeLimit[index], cMCRegisterValue.LC1LoadCellHighRangeLimit, "LC1LoadCellHighRangeLimit");

                    RunAssert(PGain[index], cMCRegisterValue.PGain, "PGain");
                    RunAssert(IGain[index], cMCRegisterValue.IGain, "IGain");
                    RunAssert(DGain[index], cMCRegisterValue.DGain, "DGain");
                    RunAssert(Offset[index], cMCRegisterValue.Offset, "Offset");

                    RunAssert(TargetInjectionFlow[index], cMCRegisterValue.TargetInjectionFlow, "TargetInjectionFlow");
                    RunAssert(TargetInjectionPressure[index], cMCRegisterValue.TargetInjectionPressure, "TargetInjectionPressure");
                }
            }

            #endregion patient region
        }


        [TestMethod()]
        public void UpdateFlowCurveParameterTest()
        {
            int[] state = { 1, 2, 3, 4, 5, 6 };
            double[] ThresholdFM1Low = { 1, 1, 1, -5101, 301, 10 };
            double[] ThresholdFM1High = { 3001, 3001, 5001, 10001, 10001, 10001 };
            double[] FM1LowRange = { 1, 1, 1, 31, 1, 1 };
            double[] FM1HighRange = { 1, 1, 1, 2001, 1, 1 };
            int catheterTypeID = 1;
            CatheterType catheterType = data.GetCatheterAccordingToCatheterId(catheterTypeID);
            for (int i = 0; i < 6; i++)
            {
                data.UpdateFlowCurveParameters(state[i], ThresholdFM1Low[i], ThresholdFM1High[i], FM1LowRange[i], FM1HighRange[i], catheterType.ID);
            }


            List<CMCRegisterValue> CMCRegisterValues = data.GetCMCRegisterValuesAccordingToCatheterID(catheterType.ID);

            foreach (CMCRegisterValue CMCRegisterValue in CMCRegisterValues)
            {
                int index = CMCRegisterValue.StateID - 1;
                RunAssert(ThresholdFM1Low[index], CMCRegisterValue.FM1FlowMeterThresholLowlimit, "ThresholdFM1Low");
                RunAssert(ThresholdFM1High[index], CMCRegisterValue.FM1FlowMeterThresholHighlimit, "ThresholdFM1High");
                RunAssert(FM1LowRange[index], CMCRegisterValue.FM1FlowMeterLowRangeLimit, "FM1LowRange");
                RunAssert(FM1HighRange[index], CMCRegisterValue.FM1FlowMeterHighRangelimit, "FM1HighRange");
            }

            // Put original data back to table.

            int[] stateorg = { 1, 2, 3, 4, 5, 6 };
            double[] ThresholdFM1Loworg = { 0, 0, 0, -5100, 3000, 0 };
            double[] ThresholdFM1Highorg = { 3000, 3000, 5000, 10000, 10000, 10000 };
            double[] FM1LowRangeorg = { 0, 0, 0, 34, 0, 0 };
            double[] FM1HighRangeorg = { 0, 0, 0, 2000, 0, 0 };

            CatheterType catheterTypeorg = data.GetCatheterAccordingToCatheterId(catheterTypeID);
            for (int i = 0; i < 6; ++i)
            {
                data.UpdateFlowCurveParameters(stateorg[i], ThresholdFM1Loworg[i], ThresholdFM1Highorg[i], FM1LowRangeorg[i], FM1HighRangeorg[i], catheterType.ID);
            }



        }

        [TestMethod()]
        public void UpdatePMCUPIDValuesTest()
        {
            int CatheterID = 1;

            #region value to assert

            double[] CP1PressureThresholdHighLimit = { 10, 10, 10, 10, 10, 10, 10 };
            double[] CP1PressureLowRangeLimit = { 10, 4.7, 4.7, 4.7, 4.7, 4.7, 4.7 };
            double[] CP1PressureHighRangeLimit = { -20, -20, -20, 0, 0, 0, -20 };
            double[] CP2PressureThresholdHighLimit = { 1000, -10, -10, -10, -10, -10, -10 };
            double[] CP2PressureLowRangeLimit = { 0, 0, 0, 0, 0, 0, 0 };
            double[] CP2PressureHighRangeLimit = { 1000, 1000, 1000, 1000, 1000, 1000, 1000 };
            double[] ThawingTemperature = { -70, -70, -70, -70, -70, -70, -70 };
            double[] PGain = { 0, 0, 40, 20, 15, 40, 0 };
            double[] IGain = { 0, 0, 40, 20, 15, 20, 0 };
            double[] DGain = { 0, 0, 0, 0, 0, 0, 0 };
            double[] Offset = { 0, 40, 20, 20, 20, 20, 0 };
            double[] TargetBalloonPressure = { 2.5, 2.5, 2.5, 2.5, 2.5, 2.5, 2.5 };
            Int16[] LowerBloodThreshold = { 14, 14, 14, 14, 14, 14, 14 };
            Int16[] UpperBloodThreshold = { 75, 75, 75, 75, 75, 75, 75 };
            double[] ThawingTemperatureSetPoint = { 50, 50, 50, 50, 50, 20, 50 };

            CatheterType catheterType = data.GetCatheterAccordingToCatheterId(CatheterID);

            List<PMCRegisterValue> pMCRegisterValues = data.GetPMCRegisterValuesAccordingToCatheterID(catheterType.ID);

            // Verify initial Patient Micro Controller Register
            foreach (PMCRegisterValue pMCRegisterValue in pMCRegisterValues)
            {
                int index = pMCRegisterValue.StateID - 1;

                if (pMCRegisterValue.StateID > 0 && pMCRegisterValue.StateID < 8)
                {
                    RunAssert(CP1PressureThresholdHighLimit[index], pMCRegisterValue.CP1PressureThresholdHighLimit, "CP1PressureThresholdHighLimit");
                    RunAssert(CP1PressureLowRangeLimit[index], pMCRegisterValue.CP1PressureLowRangeLimit, "CP1PressureLowRangeLimit");
                    RunAssert(CP1PressureHighRangeLimit[index], pMCRegisterValue.CP1PressureHighRangeLimit, "CP1PressureHighRangeLimit");

                    RunAssert(CP2PressureThresholdHighLimit[index], pMCRegisterValue.CP2PressureThresholdHighLimit, "CP2PressureThresholdHighLimit");
                    RunAssert(CP2PressureLowRangeLimit[index], pMCRegisterValue.CP2PressureLowRangeLimit, "CP2PressureLowRangeLimit");
                    RunAssert(CP2PressureHighRangeLimit[index], pMCRegisterValue.CP2PressureHighRangeLimit, "CP2PressureHighRangeLimit");
                    RunAssert(ThawingTemperature[index], pMCRegisterValue.TC1ThawingTemperature, "TC1ThawingTemperature");

                    RunAssert(PGain[index], pMCRegisterValue.Pgain, "Pgain");
                    RunAssert(IGain[index], pMCRegisterValue.Igain, "Igain");
                    RunAssert(DGain[index], pMCRegisterValue.Dgain, "Dgain");
                    RunAssert(Offset[index], pMCRegisterValue.Offset, "Offset");
                    RunAssert(TargetBalloonPressure[index], pMCRegisterValue.TargetBalloonPressure, "TargetBalloonPressure");

                
                }
            }

            //Update Patient Micro Controller Register
            //The new the original value +1.
            for (int i = 1; i < 8; ++i)
            {
                data.UpdatePMCUPIDValues(i, PGain[i - 1] + 1, IGain[i - 1] + 1, DGain[i - 1] + 1, Offset[i - 1] + 1, catheterType.ID);
            }

            // Read and Verify Updated Patient Micro Controller Register
            pMCRegisterValues = data.GetPMCRegisterValuesAccordingToCatheterID(catheterType.ID);
            foreach (PMCRegisterValue pMCRegisterValue in pMCRegisterValues)
            {
                int index = pMCRegisterValue.StateID - 1;

                if (pMCRegisterValue.StateID > 0 && pMCRegisterValue.StateID < 8)
                {
                    RunAssert(CP1PressureThresholdHighLimit[index], pMCRegisterValue.CP1PressureThresholdHighLimit, "CP1PressureThresholdHighLimit");
                    RunAssert(CP1PressureLowRangeLimit[index], pMCRegisterValue.CP1PressureLowRangeLimit, "CP1PressureLowRangeLimit");
                    RunAssert(CP1PressureHighRangeLimit[index], pMCRegisterValue.CP1PressureHighRangeLimit, "CP1PressureHighRangeLimit");

                    RunAssert(CP2PressureThresholdHighLimit[index], pMCRegisterValue.CP2PressureThresholdHighLimit, "CP2PressureThresholdHighLimit");
                    RunAssert(CP2PressureLowRangeLimit[index], pMCRegisterValue.CP2PressureLowRangeLimit, "CP2PressureLowRangeLimit");
                    RunAssert(CP2PressureHighRangeLimit[index], pMCRegisterValue.CP2PressureHighRangeLimit, "CP2PressureHighRangeLimit");
                    RunAssert(ThawingTemperature[index], pMCRegisterValue.TC1ThawingTemperature, "TC1ThawingTemperature");

                    RunAssert(PGain[index] + 1, pMCRegisterValue.Pgain, "Pgain");
                    RunAssert(IGain[index] + 1, pMCRegisterValue.Igain, "Igain");
                    RunAssert(DGain[index] + 1, pMCRegisterValue.Dgain, "Dgain");
                    RunAssert(Offset[index] + 1, pMCRegisterValue.Offset, "Offset");
                    RunAssert(TargetBalloonPressure[index], pMCRegisterValue.TargetBalloonPressure, "TargetBalloonPressure");
                }
            }

            // Revert back to original values Patient Micro Controller Register
            // Update Patient Micro Controller Register
            // The new the original value + 1.
            for (int i = 1; i < 8; ++i)
            {
                data.UpdatePMCUPIDValues(i, PGain[i - 1], IGain[i - 1], DGain[i - 1], Offset[i - 1], catheterType.ID);
            }

            #endregion
        }


        [TestMethod()]
        public void UpdateControlMicrocontrollerPIDValues()
        {
            int CatheterID = 1;

            double PGain = 0;
            double IGain = 0;
            double Dgain = 0;
            double Offset = 0;

            List<CMCRegisterValue> localCMCRegisterValues;
            CMCRegisterValue cmcuRegister;

            #region value to assert



            CatheterType catheterType = data.GetCatheterAccordingToCatheterId(CatheterID);

            List<CMCRegisterValue> cMCRegisterValues = data.GetCMCRegisterValuesAccordingToCatheterID(catheterType.ID);

            List<CMCRegisterValue> CopyofMCRegisterValues = cMCRegisterValues;

            // Verify initial Patient Micro Controller Register
            foreach (CMCRegisterValue cMCRegisterValue in cMCRegisterValues)
            {
                MessageStateId mid = MessageStateId.CAN_ID_STATE_IDLE;

                switch (cMCRegisterValue.StateID)
                {
                    case 1:
                        mid = MessageStateId.CAN_ID_STATE_IDLE;

                        PGain = cMCRegisterValue.PGain;
                        IGain = cMCRegisterValue.IGain;
                        Dgain = cMCRegisterValue.DGain;
                        Offset = cMCRegisterValue.Offset;

                        data.UpdateCMCUPIDValues(1, 1, 1, 1, 1, catheterType.ID);
                        localCMCRegisterValues = data.GetCMCRegisterValuesAccordingToCatheterID(catheterType.ID);

                        cmcuRegister = localCMCRegisterValues.Find(a => a.StateID == 1);

                        Assert.AreEqual(cmcuRegister.PGain, 1);
                        Assert.AreEqual(cmcuRegister.IGain, 1);
                        Assert.AreEqual(cmcuRegister.DGain, 1);
                        Assert.AreEqual(cmcuRegister.Offset, 1);

                        //Reset To the good  value;

                        data.UpdateCMCUPIDValues(1, PGain, IGain, Dgain, Offset, catheterType.ID);


                        break;

                    case 2:
                        mid = MessageStateId.CAN_ID_STATE_READY;

                        PGain = cMCRegisterValue.PGain;
                        IGain = cMCRegisterValue.IGain;
                        Dgain = cMCRegisterValue.DGain;
                        Offset = cMCRegisterValue.Offset;

                        data.UpdateCMCUPIDValues(2, 2, 2, 2, 2, catheterType.ID);
                        localCMCRegisterValues = data.GetCMCRegisterValuesAccordingToCatheterID(catheterType.ID);

                        cmcuRegister = localCMCRegisterValues.Find(a => a.StateID == 2);

                        Assert.AreEqual(cmcuRegister.PGain, 2);
                        Assert.AreEqual(cmcuRegister.IGain, 2);
                        Assert.AreEqual(cmcuRegister.DGain, 2);
                        Assert.AreEqual(cmcuRegister.Offset, 2);

                        //Reset To the good  value;

                        data.UpdateCMCUPIDValues(2, PGain, IGain, Dgain, Offset, catheterType.ID);

                        break;

                    case 3:
                        mid = MessageStateId.CAN_ID_STATE_INFLATION;

                        PGain = cMCRegisterValue.PGain;
                        IGain = cMCRegisterValue.IGain;
                        Dgain = cMCRegisterValue.DGain;
                        Offset = cMCRegisterValue.Offset;

                        data.UpdateCMCUPIDValues(3, 3, 3, 3, 3, catheterType.ID);
                        localCMCRegisterValues = data.GetCMCRegisterValuesAccordingToCatheterID(catheterType.ID);

                        cmcuRegister = localCMCRegisterValues.Find(a => a.StateID == 3);

                        Assert.AreEqual(cmcuRegister.PGain, 3);
                        Assert.AreEqual(cmcuRegister.IGain, 3);
                        Assert.AreEqual(cmcuRegister.DGain, 3);
                        Assert.AreEqual(cmcuRegister.Offset, 3);

                        //Reset To the good  value;

                        data.UpdateCMCUPIDValues(3, PGain, IGain, Dgain, Offset, catheterType.ID);

                        break;

                    case 4:
                        mid = MessageStateId.CAN_ID_STATE_TRANSITION;

                        PGain = cMCRegisterValue.PGain;
                        IGain = cMCRegisterValue.IGain;
                        Dgain = cMCRegisterValue.DGain;
                        Offset = cMCRegisterValue.Offset;

                        data.UpdateCMCUPIDValues(4, 4, 4, 4, 4, catheterType.ID);
                        localCMCRegisterValues = data.GetCMCRegisterValuesAccordingToCatheterID(catheterType.ID);

                        cmcuRegister = localCMCRegisterValues.Find(a => a.StateID == 4);

                        Assert.AreEqual(cmcuRegister.PGain, 4);
                        Assert.AreEqual(cmcuRegister.IGain, 4);
                        Assert.AreEqual(cmcuRegister.DGain, 4);
                        Assert.AreEqual(cmcuRegister.Offset, 4);

                        //Reset To the good  value;

                        data.UpdateCMCUPIDValues(4, PGain, IGain, Dgain, Offset, catheterType.ID);

                        break;

                    case 5:
                        mid = MessageStateId.CAN_ID_STATE_ABLATION;

                        PGain = cMCRegisterValue.PGain;
                        IGain = cMCRegisterValue.IGain;
                        Dgain = cMCRegisterValue.DGain;
                        Offset = cMCRegisterValue.Offset;

                        data.UpdateCMCUPIDValues(5, 5, 5, 5, 5, catheterType.ID);
                        localCMCRegisterValues = data.GetCMCRegisterValuesAccordingToCatheterID(catheterType.ID);

                        cmcuRegister = localCMCRegisterValues.Find(a => a.StateID == 5);

                        Assert.AreEqual(cmcuRegister.PGain, 5);
                        Assert.AreEqual(cmcuRegister.IGain, 5);
                        Assert.AreEqual(cmcuRegister.DGain, 5);
                        Assert.AreEqual(cmcuRegister.Offset, 5);

                        //Reset To the good  value;

                        data.UpdateCMCUPIDValues(5, PGain, IGain, Dgain, Offset, catheterType.ID);

                        break;

                    case 6:
                        mid = MessageStateId.CAN_ID_STATE_THAWING;

                        PGain = cMCRegisterValue.PGain;
                        IGain = cMCRegisterValue.IGain;
                        Dgain = cMCRegisterValue.DGain;
                        Offset = cMCRegisterValue.Offset;

                        data.UpdateCMCUPIDValues(6, 6, 6, 6, 6, catheterType.ID);
                        localCMCRegisterValues = data.GetCMCRegisterValuesAccordingToCatheterID(catheterType.ID);

                        cmcuRegister = localCMCRegisterValues.Find(a => a.StateID == 6);

                        Assert.AreEqual(cmcuRegister.PGain, 6);
                        Assert.AreEqual(cmcuRegister.IGain, 6);
                        Assert.AreEqual(cmcuRegister.DGain, 6);
                        Assert.AreEqual(cmcuRegister.Offset, 6);

                        //Reset To the good  value;

                        data.UpdateCMCUPIDValues(6, PGain, IGain, Dgain, Offset, catheterType.ID);

                        break;

                }
            }
            #endregion
        }

        [TestMethod()]
        public void GetCatheterAccordingToCatheterIdTest()
        {
            int CatheterID = 1;
            int CatheterDatabaseId = 1;


            CatheterType catheterType = data.GetCatheterAccordingToCatheterId(CatheterID);

            if (catheterType != null)
            {
                RunAssert(CatheterID, catheterType.CatheterID, "Catheter ID");
                RunAssert(CatheterDatabaseId, catheterType.ID, "Catheter Database ID");
            }
            else
            {
                Assert.Fail("Catheter ID could not be found!");
            }
        }

        [TestMethod()]
        public void GetAllCatheterInformationTest()
        {
            // int catheterInformationId = 1;
            int serialNumber = 255;
            int firmwareVersion = 1;
            DateTime catheterExpirationDate = new DateTime(2019, 1, 1, 0, 0, 0);
            DateTime LastUseDate = new DateTime(2018, 1, 16, 11, 0, 0);
            int numberOfInjection = 0;
            bool IsUsedForEngineering = false;
            int catheterTypeId = 1;
            int lot = 255;
            int OverloadedCatheterID = 1;

            ObservableCollection<CatheterInformation> catheterInformation = data.GetAllCatheterInformation();

            for (int i = 0; i < catheterInformation.Count; i++)
            {
                RunAssert(serialNumber, catheterInformation[i].SerialNumber, "Catheter Serial Number");
                RunAssert(firmwareVersion, catheterInformation[i].FirmwareVersion, "Catheter Firmware Version");
                RunAssert(catheterExpirationDate, catheterInformation[i].CatheterExpirationDate, "Catheter Expiration Date");
                RunAssert(LastUseDate, catheterInformation[i].LastUseDate, "Catheter Last Use Date");
                RunAssert(IsUsedForEngineering, catheterInformation[i].IsUsedForEngineering, "Catheter Last Use Date");
                RunAssert(numberOfInjection, catheterInformation[i].NumberOfInjection, "Catheter's number of injections");
                RunAssert(catheterTypeId, catheterInformation[i].CatheterTypeID, "Catheter Type ID");
                RunAssert(lot, catheterInformation[i].Lot, "Catheter Lot");
                RunAssert(OverloadedCatheterID, catheterInformation[i].OverloadedCatheterID, "OverloadedCatheterID");
            }
        }


        [TestMethod()]
        public void GetCatheterInformationsAccordingToSerialNumberAndLotTest()
        {


            int catheterInformationId = 1;
            int serialNumber = 255;
            int firmwareVersion = 1;
            DateTime catheterExpirationDate = new DateTime(2019, 1, 1, 0, 0, 0);
            DateTime LastUseDate = new DateTime(2018, 1, 16, 11, 0, 0);
            int numberOfInjection = 0;
            int catheterTypeId = 1;
            int lot = 255;
            int CatheterID = 1;
            CatheterInformation catheterInformation = data.GetatheterInformationsAccordingToSerialNumberAndLot(serialNumber, lot, CatheterID, false);
            //this.data.DataAccess.GetatheterInformationsAccordingToSerialNumberAndLot(CatheterSerialNumber, CatheterLot, CatheterID, false)

            if (catheterInformation != null)
            {
                RunAssert(catheterInformationId, catheterInformation.ID, "Catheter Information ID");
                RunAssert(serialNumber, catheterInformation.SerialNumber, "Catheter Serial Number");
                RunAssert(firmwareVersion, catheterInformation.FirmwareVersion, "Catheter Firmware Version");

                RunAssert(catheterExpirationDate.Year, catheterInformation.CatheterExpirationDate.Year, "Catheter Expiration Date");
                RunAssert(catheterExpirationDate.Month, catheterInformation.CatheterExpirationDate.Month, "Catheter Expiration Date");
                RunAssert(catheterExpirationDate.Day, catheterInformation.CatheterExpirationDate.Day, "Catheter Expiration Date");

                RunAssert(LastUseDate.Year, catheterInformation.LastUseDate.Year, "Catheter Last Use Date");
                RunAssert(LastUseDate.Month, catheterInformation.LastUseDate.Month, "Catheter Last Use Date");
                RunAssert(LastUseDate.Day, catheterInformation.LastUseDate.Day, "Catheter Last Use Date");
                RunAssert(LastUseDate.Hour, catheterInformation.LastUseDate.Hour, "Catheter Last Use Date");

                RunAssert(numberOfInjection, catheterInformation.NumberOfInjection, "Catheter's number of injections");
                RunAssert(catheterTypeId, catheterInformation.CatheterTypeID, "Catheter Type ID");
                RunAssert(lot, catheterInformation.Lot, "Catheter Lot");
            }
            else
            {
                Assert.Fail("Catheter Information could not be found!");
            }
        }

        [TestMethod()]
        public void GetCatheterInformationsAccordingToSerialNumberTest()
        {


            int catheterInformationId = 1;
            int serialNumber = 255;
            int firmwareVersion = 1;
            DateTime catheterExpirationDate = new DateTime(2019, 1, 1, 0, 0, 0);
            DateTime LastUseDate = new DateTime(2018, 1, 16, 11, 0, 0);
            int numberOfInjection = 0;
            int catheterTypeId = 1;
            int lot = 255;

            CatheterInformation catheterInformation = data.GetatheterInformationsAccordingToSerialNumber(serialNumber);

            if (catheterInformation != null)
            {
                RunAssert(catheterInformationId, catheterInformation.ID, "Catheter Information ID");
                RunAssert(serialNumber, catheterInformation.SerialNumber, "Catheter Serial Number");
                RunAssert(firmwareVersion, catheterInformation.FirmwareVersion, "Catheter Firmware Version");

                RunAssert(catheterExpirationDate.Year, catheterInformation.CatheterExpirationDate.Year, "Catheter Expiration Date");
                RunAssert(catheterExpirationDate.Month, catheterInformation.CatheterExpirationDate.Month, "Catheter Expiration Date");
                RunAssert(catheterExpirationDate.Day, catheterInformation.CatheterExpirationDate.Day, "Catheter Expiration Date");

                RunAssert(LastUseDate.Year, catheterInformation.LastUseDate.Year, "Catheter Last Use Date");
                RunAssert(LastUseDate.Month, catheterInformation.LastUseDate.Month, "Catheter Last Use Date");
                RunAssert(LastUseDate.Day, catheterInformation.LastUseDate.Day, "Catheter Last Use Date");
                RunAssert(LastUseDate.Hour, catheterInformation.LastUseDate.Hour, "Catheter Last Use Date");

                RunAssert(numberOfInjection, catheterInformation.NumberOfInjection, "Catheter's number of injections");
                RunAssert(catheterTypeId, catheterInformation.CatheterTypeID, "Catheter Type ID");
                RunAssert(lot, catheterInformation.Lot, "Catheter Lot");
            }
            else
            {
                Assert.Fail("Catheter Information could not be found!");
            }
        }

        [TestMethod()]
        public void GetCatheterLastUseDateTest()
        {


            int catheterId = 1;
            DateTime expectedLastUseDate = new DateTime(2018, 1, 16, 11, 0, 0);

            DateTime lastUseDate = data.GetCatheterLastUseDate(catheterId);

            if (lastUseDate != null)
            {

                RunAssert(expectedLastUseDate.Year, lastUseDate.Year, "Catheter Last Use Date");
                RunAssert(expectedLastUseDate.Month, lastUseDate.Month, "Catheter Last Use Date");
                RunAssert(expectedLastUseDate.Day, lastUseDate.Day, "Catheter Last Use Date");
                RunAssert(expectedLastUseDate.Hour, lastUseDate.Hour, "Catheter Last Use Date");

            }
            else
            {
                Assert.Fail("Catheter's last use date could not be found!");
            }
        }

        [TestMethod()]
        public void GetCatheterExpirationDateTest()
        {


            int catheterId = 1;
            DateTime expectedExpirationDate = new DateTime(2019, 1, 1);

            System.Threading.Thread.Sleep(1500);
            DateTime expirationDate = data.GetCatheterExpirationDate(catheterId);

            if (expirationDate != null)
            {
                RunAssert(expectedExpirationDate.Year, expirationDate.Year, "Catheter Expiration Date");
                RunAssert(expectedExpirationDate.Month, expirationDate.Month, "Catheter Expiration Date");
                RunAssert(expectedExpirationDate.Day, expirationDate.Day, "Catheter Expiration Date");
                RunAssert(expectedExpirationDate.Hour, expirationDate.Hour, "Catheter Expiration Date");
            }
            else
            {
                Assert.Fail("Catheter's expiration date could not be found!");
            }
        }

        [TestMethod()]
        public void GetThawingTemperatureSetPointValuesAccordingToCatheterIDTest()
        {

            int catheterIDOne = 1;
            int catheterIDTwo = 2;

            double expectedThawingTemperatureSetPoint = 50;
            double thawingTemperatureSetPoint = 0;

            // Catheter one 
            thawingTemperatureSetPoint = data.GetThawingTemperatureSetPointValuesAccordingToCatheterID(catheterIDOne);

            Assert.AreEqual(thawingTemperatureSetPoint, expectedThawingTemperatureSetPoint);

            //Reset the set point
            thawingTemperatureSetPoint = 0;


            // Catheter two
            thawingTemperatureSetPoint = data.GetThawingTemperatureSetPointValuesAccordingToCatheterID(catheterIDTwo);

            Assert.AreEqual(thawingTemperatureSetPoint, expectedThawingTemperatureSetPoint);

        }

        [TestMethod()]
        public void SetDefalteAfterThawFunctionalityAndIsConsoleUsingDeflateAfterThawFunctionalityTest()
        {
            bool expectedThawFunctionality = true;
            bool thawFunctionality = true;

            //Set the Thaw value
            data.SetDefalteAfterThawFunctionality(true);

            //In these case the expected value is true
            thawFunctionality = data.IsConsoleUsingDeflateAfterThawFunctionality();

            Assert.AreEqual(expectedThawFunctionality, thawFunctionality);

            expectedThawFunctionality = false;

            //Set the Thaw value
            data.SetDefalteAfterThawFunctionality(false);

            //In these case the expected value is true
            thawFunctionality = data.IsConsoleUsingDeflateAfterThawFunctionality();

            Assert.AreEqual(expectedThawFunctionality, thawFunctionality);

        }

        [TestCleanup]
        public void Cleanup()
        {
            string sqlConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=ConsoleDatabase;Data Source=.\SQLEXPRESS;";
            SqlConnection con = new SqlConnection(sqlConnectionString);
            var cmd = new SqlCommand("DataAccessTestCleanup", con);
            con.Open();
            try
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch
            {
                con.Close();
            }
        }


        /*
         
            USE [ConsoleDatabase]
             GO
             
             ALTER TABLE [dbo].[CatheterInformations] DROP CONSTRAINT [FK_CatheterTypeCatheterInformation]
             GO
             

             IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CatheterInformations]') AND type in (N'U'))
             DROP TABLE [dbo].[CatheterInformations]
             GO

             SET ANSI_NULLS ON
             GO
             
             SET QUOTED_IDENTIFIER ON
             GO
             
             CREATE TABLE [dbo].[CatheterInformations](
             	[ID] [int] IDENTITY(1,1) NOT NULL,
             	[SerialNumber] [int] NOT NULL,
             	[FirmwareVersion] [int] NOT NULL,
             	[CatheterExpirationDate] [datetime] NOT NULL,
             	[LastUseDate] [datetime] NOT NULL,
             	[NumberOfInjection] [int] NOT NULL,
             	[Lot] [int] NOT NULL,
             	[IsUsedForEngineering] [bit] NOT NULL,
             	[OverloadedCatheterID] [int] NOT NULL,
             	[CatheterTypeID] [int] NOT NULL,
              CONSTRAINT [PK_CatheterInformations] PRIMARY KEY CLUSTERED 
             (
             	[ID] ASC
             )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
             ) ON [PRIMARY]
             GO
             
             ALTER TABLE [dbo].[CatheterInformations]  WITH CHECK ADD  CONSTRAINT [FK_CatheterTypeCatheterInformation] FOREIGN KEY([CatheterTypeID])
             REFERENCES [dbo].[CatheterTypes] ([ID])
             GO
             
             ALTER TABLE [dbo].[CatheterInformations] CHECK CONSTRAINT [FK_CatheterTypeCatheterInformation]
             GO
           
             
             INSERT INTO [dbo].[CatheterInformations]
                        ([SerialNumber]
                        ,[FirmwareVersion]
                        ,[CatheterExpirationDate]
                        ,[LastUseDate]
                        ,[NumberOfInjection]
                        ,[Lot]
                        ,[IsUsedForEngineering]
                        ,[OverloadedCatheterID]
                        ,[CatheterTypeID])
                  VALUES
                        (255, 1,'2019-01-01 00:00:00.000', '2018-01-16 11:00:00.000',0,255,0,1,1)
         
         */


    }
}