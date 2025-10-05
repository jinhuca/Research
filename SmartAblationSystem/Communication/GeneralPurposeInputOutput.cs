// <copyright file="GeneralPurposeInputOutput.cs" company=" Cryterion Medical Inc.  ">
// Copyright (c) Cryterion Medical Inc. All rights reserved.
// </copyright>
// <author>Alex Smail</author>
// <date>01-02-2017</date>
// <summary> Manage the general purpose input Output</summary>

using Susi4.APIs;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Communication
{
    /// <summary>
    /// Manages the general purpose IO
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class GeneralPurposeInputOutput : IGeneralPurposeInputOutput
    {
        private int MAX_BANK_NUM = 0;

        private List<DeviceInfo> DevList = new List<DeviceInfo>();
        private List<DevPinInfo> DevPinList = new List<DevPinInfo>();

        /// <summary>
        /// Creates general purpose IO class
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public GeneralPurposeInputOutput()
        {
            try
            {
                XmlDocument xDoc = new XmlDocument();

                if (xDoc != null)
                {
                    xDoc.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream("Communication.configuration.xml"));

                    XmlNode GeneralInformationNode = xDoc.SelectSingleNode("/IOs");
                    foreach (XmlNode node in GeneralInformationNode)
                    {
                        MAX_BANK_NUM = Convert.ToInt32(node.Attributes.GetNamedItem("Number").Value);
                    }
                }

                UInt32 Status = SusiLib.SusiLibInitialize();

                if (Status != SusiStatus.SUSI_STATUS_SUCCESS && Status != SusiStatus.SUSI_STATUS_INITIALIZED)
                    return;
            }
            catch
            {
                return;
            }

            InitializeGPIO();
            InitializePins();
            InitializeDirectionAsOutput();
        }

        /// <summary>
        /// Initializes general purpose IO
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void InitializeGPIO()
        {
            UInt32 Status;

            for (int i = 0; i < MAX_BANK_NUM; i++)
            {
                DeviceInfo info = new DeviceInfo(SusiGPIO.SUSI_ID_GPIO_BANK((UInt32)i));

                Status = SusiGPIO.SusiGPIOGetCaps(info.ID, SusiGPIO.SUSI_ID_GPIO_INPUT_SUPPORT, out info.SupportInput);
                if (Status != SusiStatus.SUSI_STATUS_SUCCESS)
                    continue;

                Status = SusiGPIO.SusiGPIOGetCaps(info.ID, SusiGPIO.SUSI_ID_GPIO_OUTPUT_SUPPORT, out info.SupportOutput);
                if (Status != SusiStatus.SUSI_STATUS_SUCCESS)
                    continue;

                DevList.Add(info);
            }
        }

        /// <summary>
        /// Initializes pins
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void InitializePins()
        {
            StringBuilder sb = new StringBuilder(32);
            UInt32 mask;

            for (int i = 0; i < DevList.Count; i++)
            {
                // 32 pins per bank
                for (int j = 0; j < 32; j++)
                {
                    mask = (UInt32)(1 << j);
                    if ((DevList[i].SupportInput & mask) > 0 || (DevList[i].SupportOutput & mask) > 0)
                    {
                        DevPinInfo pinInfo = new DevPinInfo((UInt32)((i << 5) + j));
                        DevPinList.Add(pinInfo);
                    }
                }
            }
        }

        /// <summary>
        /// Sets direction as an output
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void InitializeDirectionAsOutput()
        {
            for (uint Id = 0; Id < MAX_BANK_NUM; Id++)
            {
                SetGPIODirection(Id, 1, 0);
            }
        }

        /// <summary>
        /// Sets the general purpose IO level
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="Id">general purpose IO id </param>
        /// <param name="mask">general purpose IO mask</param>
        /// <param name="level">If 1 we Activate else we deactivate</param>
        public void SetGPIOLevel(uint Id, uint mask, uint level)
        {
            uint Status;

            Status = SusiGPIO.SusiGPIOSetLevel(Id, mask, level);
            if (Status != SusiStatus.SUSI_STATUS_SUCCESS)
            {
                // To do
            }
        }

        /// <summary>
        /// When the level is 0 we are using the Gpio as output
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="mask"> Put the mask to one</param>
        /// <param name="level"> Put the level to 0</param>
        public void SetGPIODirection(uint Id, uint mask, uint level)
        {
            uint Status;
            Status = SusiGPIO.SusiGPIOSetDirection(Id, mask, level);
            if (Status != SusiStatus.SUSI_STATUS_SUCCESS)
            {
                // to do
            }
        }
    }

    /// <summary>
    /// Manages device info
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    internal class DeviceInfo
    {
        public UInt32 ID;
        public UInt32 SupportInput;
        public UInt32 SupportOutput;

        /// <summary>
        /// Creates the device information class
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="DeviceID">Device ID</param>
        public DeviceInfo(UInt32 DeviceID)
        {
            ID = DeviceID;
            SupportInput = 0;
            SupportOutput = 0;
        }
    }


    /// <summary>
    /// Manages device pin info
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    internal class DevPinInfo
    {
        public UInt32 ID;

        private string _Name = "";

        /// <summary>
        /// Creates the device information  calss
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="DeviceID">The device id</param>
        public DevPinInfo(UInt32 DeviceID)
        {
            ID = DeviceID;

            UInt32 Length = 32;
            StringBuilder sb = new StringBuilder((int)Length);
            if (SusiBoard.SusiBoardGetStringA(SusiBoard.SUSI_ID_MAPPING_GET_NAME_GPIO(ID), sb, ref Length) == SusiStatus.SUSI_STATUS_SUCCESS)
            {
                _Name = sb.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the IO name
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string Name
        {
            get { return _Name; }
        }

        /// <summary>
        /// Converts to string
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <returns>A string</returns>
        override public string ToString()
        {
            return String.Format("{0} ({1})", ID, Name);
        }
    }
}