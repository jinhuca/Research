// <copyright file="BuildDataBytes.cs" company=" Cryterion Medical Inc.  ">
// Copyright (c) Cryterion Medical Inc. All rights reserved.
// </copyright>
// <author>Alex Smail</author>
// <date>01-31-2017</date>
// <summary> Build data used in can bus communication</summary>

using Console.Configurations;
using System;

namespace Console.Helpers
{
    /// <summary>
    /// Build can one and can two data bytes
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public static class BuildDataBytes
    {
        /// <summary>
        /// Convert pressure transducer data to bytes
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="pressureTrnasducer">pressure trnasducer</param>
        /// <param name="valuesType">values type</param>
        /// <returns>Data as byte array</returns>
        public static byte[] ConvertPressureTransducerDataToBytes(IPressureTransducer pressureTrnasducer, string valuesType = "Threshold")
        {
            byte[] data = new byte[8];
            Array.Clear(data, 0, 8);
            Type type = pressureTrnasducer.GetType();
            if (type.Equals(typeof(PressureTransducerOne)))
            {
                PressureTransducerOne PT1 = pressureTrnasducer as PressureTransducerOne;

                if (valuesType == "Threshold")
                {
                    if (PT1.PressureThresholdHighLimit > 0)
                    {
                        data[0] = (byte)(FactorOfTenInt(PT1.PressureThresholdHighLimit) >> 8);
                        data[1] = (byte)(FactorOfTenInt(PT1.PressureThresholdHighLimit));
                    }
                    else
                    {
                        data[1] = (byte)(FactorOfTenInt(PT1.PressureThresholdHighLimit) >> 8);
                        data[0] = (byte)(FactorOfTenInt(PT1.PressureThresholdHighLimit));
                    }

                    if (PT1.TankPressureTooHigh > 0)
                    {
                        data[2] = (byte)(FactorOfTenInt(PT1.TankPressureTooHigh) >> 8);
                        data[3] = (byte)(FactorOfTenInt(PT1.TankPressureTooHigh));
                    }
                    else
                    {
                        data[3] = (byte)(FactorOfTenInt(PT1.TankPressureTooHigh) >> 8);
                        data[2] = (byte)(FactorOfTenInt(PT1.TankPressureTooHigh));
                    }

                    if (PT1.TankPressureLow > 0)
                    {
                        data[4] = (byte)(FactorOfTenInt(PT1.TankPressureLow) >> 8);
                        data[5] = (byte)(FactorOfTenInt(PT1.TankPressureLow));
                    }
                    else
                    {
                        data[5] = (byte)(FactorOfTenInt(PT1.TankPressureLow) >> 8);
                        data[4] = (byte)(FactorOfTenInt(PT1.TankPressureLow));
                    }
                }
                else
                {
                    if (PT1.PressureLowRangeLimit > 0)
                    {
                        data[0] = (byte)(FactorOfTenInt(PT1.PressureLowRangeLimit) >> 8);
                        data[1] = (byte)(FactorOfTenInt(PT1.PressureLowRangeLimit));
                    }
                    else
                    {
                        data[1] = (byte)(FactorOfTenInt(PT1.PressureLowRangeLimit) >> 8);
                        data[0] = (byte)(FactorOfTenInt(PT1.PressureLowRangeLimit));
                    }

                    if (PT1.PressureHighRangeLimit > 0)
                    {
                        data[2] = (byte)(FactorOfTenInt(PT1.PressureHighRangeLimit) >> 8);
                        data[3] = (byte)(FactorOfTenInt(PT1.PressureHighRangeLimit));
                    }
                    else
                    {
                        data[3] = (byte)(FactorOfTenInt(PT1.PressureHighRangeLimit) >> 8);
                        data[2] = (byte)(FactorOfTenInt(PT1.PressureHighRangeLimit));
                    }
                }
            }
            else
            {
                // X= 2, 3, 4

                if (valuesType == "Threshold")
                {
                    if (pressureTrnasducer.PressureThresholdHighLimit > 0)
                    {
                        data[0] = (byte)(FactorOfTenInt(pressureTrnasducer.PressureThresholdHighLimit) >> 8);
                        data[1] = (byte)(FactorOfTenInt(pressureTrnasducer.PressureThresholdHighLimit));
                    }
                    else
                    {
                        data[1] = (byte)(FactorOfTenInt(pressureTrnasducer.PressureThresholdHighLimit) >> 8);
                        data[0] = (byte)(FactorOfTenInt(pressureTrnasducer.PressureThresholdHighLimit));
                    }
                }
                else
                {
                    if (pressureTrnasducer.PressureLowRangeLimit > 0)
                    {
                        data[0] = (byte)(FactorOfTenInt(pressureTrnasducer.PressureLowRangeLimit) >> 8);
                        data[1] = (byte)(FactorOfTenInt(pressureTrnasducer.PressureLowRangeLimit));
                    }
                    else
                    {
                        data[1] = (byte)(FactorOfTenInt(pressureTrnasducer.PressureLowRangeLimit) >> 8);
                        data[0] = (byte)(FactorOfTenInt(pressureTrnasducer.PressureLowRangeLimit));
                    }

                    if (pressureTrnasducer.PressureHighRangeLimit > 0)
                    {
                        data[2] = (byte)(FactorOfTenInt(pressureTrnasducer.PressureHighRangeLimit) >> 8);
                        data[3] = (byte)(FactorOfTenInt(pressureTrnasducer.PressureHighRangeLimit));
                    }
                    else
                    {
                        data[3] = (byte)(FactorOfTenInt(pressureTrnasducer.PressureHighRangeLimit) >> 8);
                        data[2] = (byte)(FactorOfTenInt(pressureTrnasducer.PressureHighRangeLimit));
                    }
                }
            }

            return data;
        }

        /// <summary>
        /// convert pressure switch data to bytes
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="pressureSwitch">Pressure switch</param>
        /// <param name="valuesType">Values type</param>
        /// <returns>Data as byte array</returns>
        public static byte[] ConvertPressureSwitchDataToBytes(IPressureSwitch pressureSwitch, string valuesType = "Threshold")
        {
            byte[] data = new byte[8];
            Array.Clear(data, 0, 8);

            if (valuesType == "Threshold")
            {
                if (pressureSwitch.PressureThresholdHighLimit > 0)
                {
                    data[0] = (byte)(FactorOfTenInt(pressureSwitch.PressureThresholdHighLimit) >> 8);
                    data[1] = (byte)(FactorOfTenInt(pressureSwitch.PressureThresholdHighLimit));
                }
                else
                {
                    data[1] = (byte)(FactorOfTenInt(pressureSwitch.PressureThresholdHighLimit) >> 8);
                    data[0] = (byte)(FactorOfTenInt(pressureSwitch.PressureThresholdHighLimit));
                }
            }
            else
            {
                if (pressureSwitch.PressureLowRangeLimit > 0)
                {
                    data[0] = (byte)(FactorOfTenInt(pressureSwitch.PressureLowRangeLimit) >> 8);
                    data[1] = (byte)(FactorOfTenInt(pressureSwitch.PressureLowRangeLimit));
                }
                else
                {
                    data[1] = (byte)(FactorOfTenInt(pressureSwitch.PressureLowRangeLimit) >> 8);
                    data[0] = (byte)(FactorOfTenInt(pressureSwitch.PressureLowRangeLimit));
                }

                if (pressureSwitch.PressureHighRangeLimit > 0)
                {
                    data[2] = (byte)(FactorOfTenInt(pressureSwitch.PressureHighRangeLimit) >> 8);
                    data[3] = (byte)(FactorOfTenInt(pressureSwitch.PressureHighRangeLimit));
                }
                else
                {
                    data[3] = (byte)(FactorOfTenInt(pressureSwitch.PressureHighRangeLimit) >> 8);
                    data[2] = (byte)(FactorOfTenInt(pressureSwitch.PressureHighRangeLimit));
                }
            }

            return data;
        }

        /// <summary>
        /// Convert temperature sensor  Data to bytes
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="temperatureSensorOne"></param>
        /// <param name="valuesType"></param>
        /// <returns>Data as byte array</returns>
        public static byte[] ConvertTemperatureSensorOneDataToBytes(TemperatureSensorOne temperatureSensorOne, string valuesType = "Threshold")
        {
            byte[] data = new byte[8];
            Array.Clear(data, 0, 8);

            if (valuesType == "Threshold")
            {
                if (temperatureSensorOne.TemperatureThresholdHighLimit > 0)
                {
                    data[0] = (byte)(FactorOfTenInt(temperatureSensorOne.TemperatureThresholdHighLimit) >> 8);
                    data[1] = (byte)(FactorOfTenInt(temperatureSensorOne.TemperatureThresholdHighLimit));
                }
                else
                {
                    data[0] = (byte)(FactorOfTenInt(temperatureSensorOne.TemperatureThresholdHighLimit) >> 8);
                    data[1] = (byte)(FactorOfTenInt(temperatureSensorOne.TemperatureThresholdHighLimit));
                }
            }
            else
            {
                if (temperatureSensorOne.TemperatureLowRangeLimit > 0)
                {
                    data[0] = (byte)(FactorOfTenInt(temperatureSensorOne.TemperatureLowRangeLimit) >> 8);
                    data[1] = (byte)(FactorOfTenInt(temperatureSensorOne.TemperatureLowRangeLimit));
                }
                else
                {
                    data[0] = (byte)(FactorOfTenInt(temperatureSensorOne.TemperatureLowRangeLimit) >> 8);
                    data[1] = (byte)(FactorOfTenInt(temperatureSensorOne.TemperatureLowRangeLimit));
                }

                if (temperatureSensorOne.TemperatureHighRangeLimit > 0)
                {
                    data[2] = (byte)(FactorOfTenInt(temperatureSensorOne.TemperatureHighRangeLimit) >> 8);
                    data[3] = (byte)(FactorOfTenInt(temperatureSensorOne.TemperatureHighRangeLimit));
                }
                else
                {
                    data[2] = (byte)(FactorOfTenInt(temperatureSensorOne.TemperatureHighRangeLimit) >> 8);
                    data[3] = (byte)(FactorOfTenInt(temperatureSensorOne.TemperatureHighRangeLimit));
                }
            }

            return data;
        }

        /// <summary>
        /// Convert flow meter data to bytes
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="flowMeterOne">Flow Meter</param>
        /// <param name="valuesType">Values type</param>
        /// <returns>Data as byte array</returns>
        public static byte[] ConvertFlowMeterOneDataToBytes(FlowMeterOne flowMeterOne, string valuesType = "Threshold")
        {
            byte[] data = new byte[8];
            Array.Clear(data, 0, 8);

            if (valuesType == "Threshold")
            {

                    data[0] = (byte)(FactorOfTenIntFM1(flowMeterOne.FlowMeterThresholLowlimit) >> 8);
                    data[1] = (byte)(FactorOfTenIntFM1(flowMeterOne.FlowMeterThresholLowlimit));

                    data[2] = (byte)(FactorOfTenIntFM1(flowMeterOne.FlowMeterThresholHighlimit) >> 8);
                    data[3] = (byte)(FactorOfTenIntFM1(flowMeterOne.FlowMeterThresholHighlimit));
                
 
            }
            else
            {

                    data[0] = (byte)(FactorOfTenIntFM1(flowMeterOne.FlowMeterLowRangeLimit) >> 8);
                    data[1] = (byte)(FactorOfTenIntFM1(flowMeterOne.FlowMeterLowRangeLimit));
                
  
                    data[2] = (byte)(FactorOfTenIntFM1(flowMeterOne.FlowMeterHighRangelimit) >> 8);
                    data[3] = (byte)(FactorOfTenIntFM1(flowMeterOne.FlowMeterHighRangelimit));
                

            }

            return data;
        }

        /// <summary>
        /// Convert load cell data to bytes
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="loadCellOne">Load cell</param>
        /// <param name="valuesType">Values type</param>
        /// <returns>Data as byte array</returns>
        public static byte[] ConvertLoadCellOneDataToBytes(LoadCellOne loadCellOne, string valuesType = "Threshold")
        {
            byte[] data = new byte[8];
            Array.Clear(data, 0, 8);

            if (valuesType == "Threshold")
            {
       
                
                    data[0] = (byte)(FactorOfTenInt(loadCellOne.LoadCellThresholdWarning) >> 8);
                    data[1] = (byte)(FactorOfTenInt(loadCellOne.LoadCellThresholdWarning));
                

                    data[2] = (byte)(FactorOfTenInt(loadCellOne.LoadCellThresholdFail) >> 8);
                    data[3] = (byte)(FactorOfTenInt(loadCellOne.LoadCellThresholdFail));
           

            }
            else
            {
 
                    data[0] = (byte)(FactorOfTenInt(loadCellOne.LoadCellLowRangeLimit) >> 8);
                    data[1] = (byte)(FactorOfTenInt(loadCellOne.LoadCellLowRangeLimit));
                

                    data[2] = (byte)(FactorOfTenInt(loadCellOne.LoadCellHighRangeLimit) >> 8);
                    data[3] = (byte)(FactorOfTenInt(loadCellOne.LoadCellHighRangeLimit));
         
            }

            return data;
        }

        /// <summary>
        /// Convert patient pressure transducer one data to bytes
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="patientPressureTransducerOne">Patient pressure transducer one</param>
        /// <returns>Data as byte array</returns>
        public static byte[] ConvertPatientPressureTransducerOneDataToBytes(PatientPressureTransducerOne patientPressureTransducerOne)
        {
            byte[] data = new byte[8];
            Array.Clear(data, 0, 8);

            //Threshold cPInner High

            data[0] = (byte)(FactorOfTenInt(patientPressureTransducerOne.PressureThresholdHighLimit) >> 8);
            data[1] = (byte)(FactorOfTenInt(patientPressureTransducerOne.PressureThresholdHighLimit));

            //cPOuter High

            data[2] = (byte)(FactorOfTenInt(patientPressureTransducerOne.PressureLowRangeLimit) >> 8);
            data[3] = (byte)(FactorOfTenInt(patientPressureTransducerOne.PressureLowRangeLimit));

            //cPTip High

            data[4] = (byte)(FactorOfTenInt(patientPressureTransducerOne.PressureHighRangeLimit) >> 8);
            data[5] = (byte)(FactorOfTenInt(patientPressureTransducerOne.PressureHighRangeLimit));

            return data;
        }

        /// <summary>
        /// Convert ballon data to byte
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="ballon">Ballon</param>
        /// <returns>Data as byte array</returns>
        public static byte[] ConvertBallonDataToByte(PatientMicroControllerBalloonPressureRegulator ballon)
        {
            byte[] data = new byte[8];
            Array.Clear(data, 0, 8);

            data[0] = (byte)(FactorOfTenInt(ballon.TargetBalloonPressure) >> 8);
            data[1] = (byte)(FactorOfTenInt(ballon.TargetBalloonPressure));

            //data[2] = (byte)(FactorOfTenInt(ballon.TargetBalloonFlow) >> 8);
            //data[3] = (byte)(FactorOfTenInt(ballon.TargetBalloonFlow));

            return data;
        }

        /// <summary>
        /// Convert PID data to byte
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="pid">PID object</param>
        /// <returns>Data as byte array</returns>
        public static byte[] ConvertPIDDataToByte(IPID pid)
        {
            byte[] data = new byte[8];
            Array.Clear(data, 0, 8);

            data[0] = (byte)(FactorOfTenInt(pid.PGain) >> 8);
            data[1] = (byte)(FactorOfTenInt(pid.PGain));

            data[2] = (byte)(FactorOfTenInt(pid.IGain) >> 8);
            data[3] = (byte)(FactorOfTenInt(pid.IGain));

            data[4] = (byte)(FactorOfTenInt(pid.DGain) >> 8);
            data[5] = (byte)(FactorOfTenInt(pid.DGain));

            data[6] = (byte)(FactorOfTenInt(pid.Offset) >> 8);
            data[7] = (byte)(FactorOfTenInt(pid.Offset));

            return data;
        }


        /// <summary>
        /// Convert balloon size data to byte
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="cryoBalloonConfiguration">The cryoballoon configuration</param>
        /// <returns>Data as byte array</returns>
        public static byte[] ConvertBalloonSizeDataToByte(CryoBalloonConfiguration cryoBalloonConfiguration)
        {
            byte[] data = new byte[8];
            Array.Clear(data, 0, 8);

            data[0] = (byte)(((int)cryoBalloonConfiguration.RampUpTimeByStep) >> 8);
            data[1] = (byte)((cryoBalloonConfiguration.RampUpTimeByStep));

            data[2] = (byte)(FactorOfTenInt(cryoBalloonConfiguration.PressureRampUpValue) >> 8);
            data[3] = (byte)(FactorOfTenInt(cryoBalloonConfiguration.PressureRampUpValue));

            data[4] = (byte)((int)(cryoBalloonConfiguration.RampDownTimeByStep) >> 8);
            data[5] = (byte)((cryoBalloonConfiguration.RampDownTimeByStep));

            data[6] = (byte)(FactorOfTenInt(cryoBalloonConfiguration.PressureRampDownValue) >> 8);
            data[7] = (byte)(FactorOfTenInt(cryoBalloonConfiguration.PressureRampDownValue));

            return data;
        }

        /// <summary>
        /// Convert thermocouple one data to bytes
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="thermocoupleOne">Thermocouple one</param>
        /// <returns>Data as byte array</returns>
        public static byte[] ConvertThermocoupleOneAndBloodDetectorDataToBytes(ThermocoupleOne thermocoupleOne,  BloodDetector bloodDetector)
        {
            byte[] data = new byte[8];
            Array.Clear(data, 0, 8);

            data[0] = (byte)(FactorOfTenInt(thermocoupleOne.ThawingTemperature) >> 8);
            data[1] = (byte)(FactorOfTenInt(thermocoupleOne.ThawingTemperature));

            data[2] = (byte)(FactorOfTenInt(thermocoupleOne.ThawingTemperatureSetPoint) >> 8);
            data[3] = (byte)(FactorOfTenInt(thermocoupleOne.ThawingTemperatureSetPoint));

            data[4] = (byte)(FactorOfTenInt(bloodDetector.LowerBloodThreshold) >> 8);
            data[5] = (byte)(FactorOfTenInt(bloodDetector.LowerBloodThreshold));

            data[6] = (byte)(FactorOfTenInt(bloodDetector.UpperBloodThreshold) >> 8);
            data[7] = (byte)(FactorOfTenInt(bloodDetector.UpperBloodThreshold));

            return data;
        }

        /// <summary>
        /// Convert injection flow data to Bytes
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="injectionFlow">Injection flow</param>
        /// <param name="injectionPressure">Injection pressure</param>
        /// <returns>Data as byte array</returns>
        public static byte[] ConvertInjectionFlowDataToByte(CentralMicroControllerFlowAndPressureRegulator injectionFlow, CentralMicroControllerFlowAndPressureRegulator injectionPressure)
        {
            byte[] data = new byte[8];
            Array.Clear(data, 0, 8);

            data[0] = (byte)(FactorOfTenIntFM1(injectionFlow.TargetInjectionFlow) >> 8);
            data[1] = (byte)(FactorOfTenIntFM1(injectionFlow.TargetInjectionFlow));

            data[2] = (byte)(FactorOfTenInt(injectionPressure.TargetInjectionPressure) >> 8);
            data[3] = (byte)(FactorOfTenInt(injectionPressure.TargetInjectionPressure));

            return data;
        }

        /// <summary>
        /// Convert catheter iD , serial number, and expiration date data to byte
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="catheter">Catheter object</param>
        /// <returns>Data as byte array</returns>
        public static byte[] ConvertCatheterIDSerialNumberExpirationDateDataToByte(Catheter catheter)
        {
            byte[] data = new byte[8];
            Array.Clear(data, 0, 8);

            data[0] = (byte)(FactorOfTenInt(catheter.CatheterID) >> 8);
            data[1] = (byte)(FactorOfTenInt(catheter.CatheterID));

            data[2] = (byte)(FactorOfTenInt(catheter.SerialNumber) >> 8);
            data[3] = (byte)(FactorOfTenInt(catheter.SerialNumber));

            data[4] = (byte)FactorOfTenInt(catheter.CatheterExpirationMonth);

            data[5] = (byte)FactorOfTenInt(catheter.CatheterExpirationDay);

            data[6] = (byte)(FactorOfTenInt(catheter.CatheterExpirationYear) >> 8);
            data[7] = (byte)(FactorOfTenInt(catheter.CatheterExpirationYear));

            return data;
        }

        /// <summary>
        /// Convert catheter validation data to byte
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="catheter">Catheter object</param>
        /// <param name="isCatethervaild">Is catether vaild</param>
        /// <returns>Data as byte array</returns>
        public static byte[] ConvertCatheterValidationDataToByte(Catheter catheter, bool isCatethervaild = false)
        {
            byte[] data = new byte[8];
            Array.Clear(data, 0, 8);
            if (isCatethervaild)
            {
                data[0] = (byte)(FactorOfTenInt(255) >> 8);
                data[1] = (byte)(FactorOfTenInt(255));
            }
            else
            {
                data[0] = (byte)(FactorOfTenInt(0) >> 8);
                data[1] = (byte)(FactorOfTenInt(0));
            }

            data[2] = (byte)(FactorOfTenInt(catheter.SerialNumber) >> 8);
            data[3] = (byte)(FactorOfTenInt(catheter.SerialNumber));

            data[4] = (byte)FactorOfTenInt(catheter.CatheterExpirationMonth);

            data[5] = (byte)FactorOfTenInt(catheter.CatheterExpirationDay);

            data[6] = (byte)(FactorOfTenInt(catheter.CatheterExpirationYear) >> 8);
            data[7] = (byte)(FactorOfTenInt(catheter.CatheterExpirationYear));

            return data;
        }

        /// <summary>
        /// Convert Catheter last use date number of injections data to byte
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="catheter">Catheter object</param>
        /// <returns>Data as byte array</returns>
        public static byte[] ConvertCatheterLastUseDateNumberOfInjectionsDataToByte(Catheter catheter)
        {
            byte[] data = new byte[8];
            Array.Clear(data, 0, 8);

            data[0] = (byte)(catheter.CatheterLastUseHour);

            data[1] = (byte)(catheter.CatheterLastUseDay);

            data[2] = (byte)(catheter.CatheterLastUseMonth);

            data[3] = (byte)((catheter.CatheterLastUseYear) >> 8);
            data[4] = (byte)((catheter.CatheterLastUseYear));

            data[5] = (byte)((catheter.NumberOfInjections) >> 8);
            data[6] = (byte)((catheter.NumberOfInjections));

            return data;
        }

        /// <summary>
        /// Convert calibration component data to byte
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="ComponentValue"> Component value</param>
        /// <returns>Data as byte array</returns>
        public static byte[] ConvertCalibrationComponentDataToByte(int ComponentValue)
        {
            byte[] data = new byte[8];
            Array.Clear(data, 0, 8);

            data[0] = (byte)((ComponentValue) >> 8);
            data[1] = (byte)((ComponentValue));


            return data;
        }

        public static byte[] ConvertCalibrationComponentDataToByte(int ComponentValue, int calibrationFactor)
        {
            byte[] data = new byte[8];
            Array.Clear(data, 0, 8);

            data[0] = (byte)((ComponentValue) >> 8);
            data[1] = (byte)((ComponentValue));

            data[2] = (byte)((calibrationFactor) >> 8);
            data[3] = (byte)((calibrationFactor));
            return data;
        }

        /// <summary>
        /// Convert audio component data to byte
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="ComponentValue">Component value</param>
        /// <returns>Data as byte array</returns>
        public static byte[] ConvertAudioComponentDataToByte(int ComponentValue)
        {
            byte[] data = new byte[8];
            Array.Clear(data, 0, 8);

            data[1] = (byte)((ComponentValue) >> 8);
            data[0] = (byte)((ComponentValue));

            return data;
        }

        /// <summary>
        /// Convert heart beat status data to byte
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="HeartbeatStatusValue">Heart beat status value</param>
        /// <returns>Data as byte array</returns>
        public static byte[] ConvertHeartbeatStatusDataToByte(int HeartbeatStatusValue)
        {
            byte[] data = new byte[8];
            Array.Clear(data, 0, 8);

            data[1] = (byte)((HeartbeatStatusValue) >> 8);
            data[0] = (byte)((HeartbeatStatusValue));

            return data;
        }

        /// <summary>
        /// Convert heart beat status data to byte
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="HeartbeatStatusValue">Heart beat status value</param>
        /// <returns>Data as byte array</returns>
        public static byte[] ConvertHeartbeatStatusDataToByte(int HeartbeatStatusValue, double dmsDetectionThreshold)
        {
            byte[] data = new byte[8];
            Array.Clear(data, 0, 8);

            data[1] = (byte)((HeartbeatStatusValue) >> 8);
            data[0] = (byte)((HeartbeatStatusValue));


            data[2] = (byte)(FactorOfThousandInt(dmsDetectionThreshold) >> 8);
            data[3] = (byte)(FactorOfThousandInt(dmsDetectionThreshold));

            return data;
        }


        /// <summary>
        /// Convert console state to byte
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="isInablationState">Is console In Ablation state</param>
        /// <returns>Data as byte array</returns>
        public static byte[] ConvertConsoleStateToByte(int isInablationState)
        {
            byte[] data = new byte[8];
            Array.Clear(data, 0, 8);

            data[0] = (byte)((isInablationState));

            return data;
        }

        /// <summary>
        /// value =  value *10
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="valueToConvert"></param>
        /// <returns>value factor ten</returns>
        private static int FactorOfTenInt(double valueToConvert)
        {
            return (int)(valueToConvert * 10);
        }

        /// <summary>
        /// Value =  value * X
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="valueToConvert"></param>
        /// <returns>FM1 Factor it can be one or ten</returns>
        private static int FactorOfTenIntFM1(double valueToConvert)
        {
            return (int)(valueToConvert);
        }

        /// <summary>
        /// convert a value to thousand base 
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="HeartbeatStatusValue">Heart beat status value</param>
        /// <returns>Data as byte array</returns>
        private static int FactorOfThousandInt(double valueToConvert)
        {
            return (int)(valueToConvert * 1000);
        }

        /// <summary>
        /// convert a value to ten base 
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="valueToConvert">value to convert</param>
        /// <returns>A value divided by ten</returns>
        private static int FactorOfTenDivision(double valueToConvert)
        {
            return (int)(valueToConvert / 10);
        }

    }
}