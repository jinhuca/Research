// <copyright file="CanBusCommunication.cs" company="company">
// Copyright (c) Cryterion Medical Inc. All rights reserved.
// </copyright>
// <author>Alex Smail</author>
// <date>01-17-2017</date>
// <summary> Manage CAN 1 and CAN 2 Communication</summary>

using System;
using System.Configuration;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml;
using LogSystem;
using static LogSystem.LogService;

namespace Communication
{
    /// <summary>
    /// Manages CAN 1 and CAN 2 communication
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>

    public class CanBusCommunication : ICanBusCommunication, IDisposable
    {
        private AdvCANIO CanOneDevice = new AdvCANIO();

        //Adding the Can Two device
        private AdvCANIO CanTwoDevice = new AdvCANIO();

        private ThreadStart ReceiveThreadStOne;
        private Thread ReceiveThreadOne;

        //Adding thread for the can Two
        private ThreadStart ReceiveThreadStTwo;

        private Thread ReceiveThreadTwo;

        private uint nMsgCountOne = 0;
        private uint nMsgCountTwo = 0;

        private uint maximumTimeOut = 0;

        private bool m_bRunReceiveOne = false;
        private bool m_bRunReceiveTwo = false;

        // Flag: Has Dispose already been called?
        private bool disposed = false;

        public event EventHandler<CanBusEventArgs> MessageReceivedOne;

        private CanBusEventArgs canBusOneEventArgs = null;

        //Event from Can 2
        public event EventHandler<CanBusEventArgs> MessageReceivedTwo;

        private CanBusEventArgs canBusTwoEventArgs = null;

        /// <summary>
        /// Gets or sets events from CAN 1
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public CanBusEventArgs CanBusOneEventArgs
        {
            get
            {
                return canBusOneEventArgs;
            }

            set
            {
                canBusOneEventArgs = value;
            }
        }

        /// <summary>
        /// Gets or sets events from CAN 1
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public CanBusEventArgs CanBusTwoEventArgs
        {
            get
            {
                return canBusTwoEventArgs;
            }

            set
            {
                canBusTwoEventArgs = value;
            }
        }

        private string cmcuAndPmcu = string.Empty;
        private string connectionBox = string.Empty;

        /// <summary>
        /// Creates CAN bus communication class
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <id>SF-SDS-0070</id>
        public CanBusCommunication()
        {
            cmcuAndPmcu = ConfigurationManager.AppSettings["SBCPMCUCMCU"];
            connectionBox = ConfigurationManager.AppSettings["SBCCONNECTIONBOX"];

            XmlDocument xDoc = new XmlDocument();

            if (xDoc != null)
            {
                xDoc.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream("Communication.configuration.xml"));

                XmlNode GeneralInformationNode = xDoc.SelectSingleNode("/IOs");
                foreach (XmlNode node in GeneralInformationNode)
                {
                    if (node.Name == "TO")
                    {
                        maximumTimeOut = Convert.ToUInt32(node.Attributes.GetNamedItem("Number").Value);
                    }
                    else if (node.Name == "NMC")
                    {
                        uint nmc = Convert.ToUInt32(node.Attributes.GetNamedItem("Number").Value);
                        nMsgCountOne = nmc;
                        nMsgCountTwo = nmc;
                    }
                }
            }

            // CAN 1
            CanBusOneEventArgs = new CanBusEventArgs();
            if (InitializeCanBusOne())
            {
                CanBusOneEnterWorkMode();
                //LogInfo("CanBus One is initialized properly.");
            }

            StartReceiveThreadOne();

            // CAN 2
            CanBusTwoEventArgs = new CanBusEventArgs();
            if (InitializeCanBusTwo())
            {
                CanBusTwoEnterWorkMode();
                //LogInfo("CanBus Two is initialized properly.");
            }

            // the start thread is enabled only when we go on cryoablation
            // there is investigation on how to manage the ecg thread.  we can use a bool to read only when we are
            // in ablation the firmware shall not send any data when we leave the cryo ablation

            StartReceiveThreadTwo();
        }

        /// <summary>
        /// Initializes CAN 1
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <returns>true if success</returns>
        private bool InitializeCanBusOne()
        {
            string CanPortName = cmcuAndPmcu;
            UInt16 BaudRateValue;
            int nRet = 0;
            bool result = true;

            //Open CAN port
            nRet = CanOneDevice.acCanOpen(CanPortName, false, 1000, 1000);
            if (nRet < 0)
            {
                result = false;
            }

            //Enter reset mode
            nRet = CanOneDevice.acEnterResetMode();
            if (nRet < 0)
            {
                result = false;
            }

            //Set Baud Rate
            BaudRateValue = 125;
            nRet = CanOneDevice.acSetBaud(BaudRateValue);
            if (nRet < 0)
            {
                CanOneDevice.acCanClose();
                result = false;
            }

            nRet = CanOneDevice.acSetTimeOut(maximumTimeOut, maximumTimeOut);

            if (nRet < 0)
            {
                CanOneDevice.acCanClose();
                result = false;
            }

            ReceiveThreadStOne = new ThreadStart(ReceiveThreadMethodOne);            //Create a new thread
            ReceiveThreadOne = new Thread(ReceiveThreadStOne);
            ReceiveThreadOne.Priority = ThreadPriority.Normal;

            return result;
        }

        /// <summary>
        /// Initializes CAN 2
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <returns>true if success</returns>
        private bool InitializeCanBusTwo()
        {
            string CanPortName = connectionBox;
            UInt16 BaudRateValue;
            int nRet = 0;
            bool result = true;

            //Open CAN port
            nRet = CanTwoDevice.acCanOpen(CanPortName, false, 1000, 1000);
            if (nRet < 0)
            {
                result = false;
            }

            //Enter reset mode
            nRet = CanTwoDevice.acEnterResetMode();
            if (nRet < 0)
            {
                result = false;
            }

            //Set Baud Rate
            BaudRateValue = 125;
            nRet = CanTwoDevice.acSetBaud(BaudRateValue);
            if (nRet < 0)
            {
                CanTwoDevice.acCanClose();
                result = false;
            }

            //set timeOut
            nRet = CanTwoDevice.acSetTimeOut(maximumTimeOut, maximumTimeOut);

            if (nRet < 0)
            {
                CanTwoDevice.acCanClose();
                result = false;
            }

            ReceiveThreadStTwo = new ThreadStart(ReceiveThreadMethodTwo);            //Create a new thread for the can 2
            ReceiveThreadTwo = new Thread(ReceiveThreadStTwo);
            ReceiveThreadTwo.Priority = ThreadPriority.Normal;

            return result;
        }

        /// <summary>
        /// Frees the unmanaged resources
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="disposing">bool for disposing</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;
            if (disposing)
            {
                m_bRunReceiveOne = false;

                CanOneDevice.acCanClose();

                CanTwoDevice.acCanClose();

                try
                {
                    if ((ReceiveThreadOne.ThreadState & (ThreadState.Stopped | ThreadState.Unstarted)) == 0)
                    {
                        ReceiveThreadOne.Abort();

                        // i have to put again the join function. i commented because when there is no
                        // Can connected it freez
                        //ReceiveThreadOne.Join();                                                       //Thread stops
                    }

                    if ((ReceiveThreadTwo.ThreadState & (ThreadState.Stopped | ThreadState.Unstarted)) == 0)
                    {
                        ReceiveThreadTwo.Abort();

                        // i have to put again the join function. i commented because when there is no
                        // Can connected it freez
                        //ReceiveThreadTwo.Join();                                                       //Thread stops
                    }

                    disposed = true;
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Frees the unmanaged resources.
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// A thread that receives data from CAN 1
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void ReceiveThreadMethodOne()
        {
            try
            {
                //string ReceiveStatus;
                int nRet;
                uint nReadCount = nMsgCountOne;
                uint pulNumberofRead = 0;
                uint ReceiveIndex = 0;

                AdvCan.canmsg_t[] msgRead = new AdvCan.canmsg_t[nMsgCountOne];
                for (int i = 0; i < nMsgCountOne; i++)
                {
                    msgRead[i].data = new byte[8];
                }

                ReceiveIndex = 0;
                m_bRunReceiveOne = true;
                CanOneDevice.acClearRxFifo(); 

                while (m_bRunReceiveOne)
                {
                    nRet = CanOneDevice.acCanRead(msgRead, nReadCount, ref pulNumberofRead); //Receiving frames
                    if (nRet == AdvCANIO.TIME_OUT)
                    {
                        // To do
                        // Log.Error("CanBusOne read TIMEOUT.");
                        throw new System.ArgumentException("AdvCANIO.TIME_OUT");
                    }
                    
                    if (nRet == AdvCANIO.OPERATION_ERROR)
                    {
                        // To do
                        // Log.Error("CanBusOne read OPERATION_ERROR.");
                        throw new System.ArgumentException("AdvCANIO.OPERATION_ERROR");
                    }
                    
                    // if (pulNumberofRead > 1)
                    // {
                    //   LogInfo($"Read more than one package from CanBusOne : {pulNumberofRead}.");
                    // }

                    for (int j = 0; j < pulNumberofRead; j++)
                    {
                        if (msgRead[j].id == AdvCan.ERRORID)
                        {
                          LogInfo($"CanBusOne read error. ErrorId = {msgRead[j].id}", LogLevel.Error);
                        }
                        else
                        {
                            NotifyCanOneMessageReceived(ref msgRead[j]);
                        }
                    }

                    Thread.Sleep(0);
                }
            }
            catch (Exception e)
            {
              LogException(e);
                //Log.Error("CanBusOne Stop with exception {@exception}.", e);
            }
        }

        /// <summary>
        /// A thread that receives data from CAN 2
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void ReceiveThreadMethodTwo()
        {
            try
            {
                //string ReceiveStatus;
                int nRet;
                uint nReadCount = nMsgCountTwo;
                uint pulNumberofRead = 0;
                uint ReceiveIndex = 0;

                AdvCan.canmsg_t[] msgRead = new AdvCan.canmsg_t[nMsgCountTwo];
                for (int i = 0; i < nMsgCountTwo; i++)
                {
                    msgRead[i].data = new byte[8];
                }

                ReceiveIndex = 0;
                m_bRunReceiveTwo = true;
                CanTwoDevice.acClearRxFifo();

                while (m_bRunReceiveTwo)
                {
                    nRet = CanTwoDevice.acCanRead(msgRead, nReadCount, ref pulNumberofRead); //Receiving frames
                    if (nRet == AdvCANIO.TIME_OUT)
                    {
                        // To do
                        // Log.Error("CanBusTwo read TIMEOUT.");

                        throw new System.ArgumentException("AdvCANIO.TIME_OUT");
                    }
                    else if (nRet == AdvCANIO.OPERATION_ERROR)
                    {
                        // Log.Error("CanBusTwo read OPERATION_ERROR.");
                        // To do
                        throw new System.ArgumentException("AdvCANIO.OPERATION_ERROR");
                    }
                    else
                    {
                        for (int j = 0; j < pulNumberofRead; j++)
                        {
                            //ReceiveStatus = "Package ";
                            //ReceiveStatus += Convert.ToString(ReceiveIndex + j + 1) + " is ";
                            if (msgRead[j].id == AdvCan.ERRORID)
                            {
                                // To do
                                //throw new System.ArgumentException("AdvCan.ERRORID");
                                LogInfo($"CanBusTwo read error. ErrorId = {msgRead[j].id}");
                            }
                            else
                            {
                                NotifyCanTwoMessageReceived(ref msgRead[j]);
                            }
                        }
                    }

                    // ReceiveIndex += pulNumberofRead;
                    Thread.Sleep(0);
                }
            }
            catch (Exception e)
            {
              LogException(e);
            }
        }


        /// <summary>
        /// Sets CanBusTwoEventArgs properties 
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void NotifyCanOneMessageReceived(ref AdvCan.canmsg_t msgRead)
        {
          CanBusOneEventArgs.Falgs = msgRead.flags;
          CanBusOneEventArgs.Cob = msgRead.cob;
          CanBusOneEventArgs.Id = msgRead.id;
          CanBusOneEventArgs.Length = msgRead.length;
          CanBusOneEventArgs.Data = msgRead.data;

          OnMessageReceivedOne(this, CanBusOneEventArgs);
        }

        /// <summary>
        /// Sets CanBusTwoEventArgs properties 
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void NotifyCanTwoMessageReceived(ref AdvCan.canmsg_t msgRead)
        {
            CanBusTwoEventArgs.Falgs = msgRead.flags;
            CanBusTwoEventArgs.Cob = msgRead.cob;
            CanBusTwoEventArgs.Id = msgRead.id;
            CanBusTwoEventArgs.Length = msgRead.length;
            CanBusTwoEventArgs.Data = msgRead.data;

            OnMessageReceivedTwo(this, CanBusTwoEventArgs);
        }

        /// <summary>
        /// Enters CAN 1 in work mode
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <returns></returns>

        private bool CanBusOneEnterWorkMode()
        {
            int nRet = -1;
            bool result = true;

            nRet = CanOneDevice.acEnterWorkMode();                                     //Enter work mdoe
            if (nRet < 0)
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Enters CAN 2 in work mode
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <returns>True success </returns>
        private bool CanBusTwoEnterWorkMode()
        {
            int nRet = -1;
            bool result = true;

            nRet = CanTwoDevice.acEnterWorkMode();                                     //Enter work mdoe
            if (nRet < 0)
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Starts ReceiveThreadOne
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public void StartReceiveThreadOne()
        {
            ReceiveThreadOne.Start();
        }

        /// <summary>
        /// Starts ReceiveThreadTwo
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public void StartReceiveThreadTwo()
        {
            ReceiveThreadTwo.Start();
        }

        /// <summary>
        /// Stops ReceiveThreadOne
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public void StopReceiveThreadOne()
        {
            ReceiveThreadOne.Abort();
            ReceiveThreadOne.Join();
        }

        /// <summary>
        /// Stops ReceiveThreadTwo
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public void StopReceiveThreadTwo()
        {
            ReceiveThreadTwo.Abort();
            ReceiveThreadTwo.Join();
        }

        /// <summary>
        /// Receives events from CAN 1
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">Refrence to the sender</param>
        /// <param name="e">Represents the base class for classes that contain can bus event data</param>
        protected virtual void OnMessageReceivedOne(object sender, CanBusEventArgs e)
        {
            MessageReceivedOne?.Invoke(sender, e);
        }

        /// <summary>
        /// Handles events from the CAN 2
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Represents the base class for classes that contain can bus event data</param>
        protected virtual void OnMessageReceivedTwo(object sender, CanBusEventArgs e)
        {
            MessageReceivedTwo?.Invoke(sender, e);
        }

        /// <summary>
        /// Sends data to CAN 1
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="messageId">message id</param>
        /// <param name="dataToSend">data to send</param>
        /// <param name="messageframeformats">message frame formats</param>

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SendDataToCanBus(uint messageId, byte[] dataToSend, bool messageframeformats, bool ReadingFirmwareVersion = false)
        {
            int nRet;
            uint nMsgCount = 1;
            AdvCan.canmsg_t[] msgWrite = new AdvCan.canmsg_t[nMsgCount];                 //Package for write
            AdvCan.canmsg_t[] tmp_msgWrite = new AdvCan.canmsg_t[nMsgCount];

            uint nWriteCount = 0;
            uint pulNumberofWritten = 0;
            uint SendIndex = 0;

            char[] data = new char[8];

            //Initialize msg
            for (int j = 0; j < 1; j++)
            {
                msgWrite[j].flags = 0;
                msgWrite[j].cob = 0;
                msgWrite[j].id = messageId; //FFFFFAAA
                msgWrite[j].length = (short)dataToSend.Length; //(short)AdvCan.DATALENGTH;
                msgWrite[j].data = dataToSend; //new byte[AdvCan.DATALENGTH];
                if (messageframeformats)
                {
                    msgWrite[j].flags += AdvCan.MSG_RTR;
                    msgWrite[j].length = 0;
                }
            }

            if (nWriteCount > 0)
            {
                if (nMsgCount - nWriteCount > 0 && nMsgCount - nWriteCount <= tmp_msgWrite.Length && nMsgCount - nWriteCount <= msgWrite.Length)
                {
                    Array.Copy(msgWrite, nWriteCount, tmp_msgWrite, 0, nMsgCount - nWriteCount);
                }
            }
            else
            {
                nWriteCount = nMsgCount;
                if (nMsgCount > 0 && nMsgCount <= msgWrite.Length && nMsgCount <= tmp_msgWrite.Length)
                {
                    Array.Copy(msgWrite, 0, tmp_msgWrite, 0, nMsgCount);
                }
            }

            /**********************************************************************************************
              *  NOTE: acCanWrite usage
              *
              *    Description£º
              *       Users can use this interface to send data to CAN port which was opened.
              *       One or more frames can be selected each time.
              *
              *    Parameters:
              *       msgWrite                - managed buffer to write
              *       nWriteCount             - CAN frame number want to write each time
              *       pulNumberofWritten      - Real number of frames sent to driver.
              *
              *    In this example, we send 100 CAN frames defined by 'nMsgCount' each time by default.
              *    If user want to send one or more frames eache time, user can also change it as follows:
              *    Firstly, open CAN port and pass the value of 'MsgNumberOfReadBuffer' and 'MsgNumberOfWriteBuffer'arguments.
              *    About 'MsgNumberOfReadBuffer' and 'MsgNumberOfWriteBuffer', please see 'acCanPort' usage above.
              *    Secondly, define the msgWrite according to the frame number user want to send each time.
              *    Thirdly, define the value of 'nWriteCount'according to the frame number user want to send each time.
              *    In this examples, user can only change the value of 'nMsgCount' to change the count of frame to send each time.
             /**********************************************************************************************/

            if (tmp_msgWrite[0].length > 1 && !ReadingFirmwareVersion)
            {
                nRet = CanOneDevice.acCanWrite(tmp_msgWrite, nWriteCount, ref pulNumberofWritten); //Send frames

                if (nRet == AdvCANIO.TIME_OUT)
                {
                    return;
                }
                else if (nRet == AdvCANIO.OPERATION_ERROR)
                {
                    return;
                }
                else
                {
                    nWriteCount -= pulNumberofWritten;
                    SendIndex += pulNumberofWritten;
                }
            }
            else if (ReadingFirmwareVersion)
            {
                nRet = CanOneDevice.acCanWrite(tmp_msgWrite, nWriteCount, ref pulNumberofWritten); //Send frames
                if (nRet == AdvCANIO.TIME_OUT)
                {
                    return;
                }
                else if (nRet == AdvCANIO.OPERATION_ERROR)
                {
                    return;
                }
                else
                {
                    nWriteCount -= pulNumberofWritten;
                    SendIndex += pulNumberofWritten;
                }
            }
        }

        /// <summary>
        /// Sends data to CAN 2
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="messageId">message Id</param>
        /// <param name="dataToSend">data to send</param>
        /// <param name="messageframeformats">message frame formats</param>
        public void SendDataToCanBusTwo(uint messageId, byte[] dataToSend, bool messageframeformats)
        {
            int nRet;
            uint nMsgCount = 1;
            AdvCan.canmsg_t[] msgWrite = new AdvCan.canmsg_t[nMsgCount];                 //Package for write
            AdvCan.canmsg_t[] tmp_msgWrite = new AdvCan.canmsg_t[nMsgCount];

            uint nWriteCount = 0;
            uint pulNumberofWritten = 0;
            uint SendIndex = 0;

            char[] data = new char[8];

            //Initialize msg
            for (int j = 0; j < 1; j++)
            {
                msgWrite[j].flags = 0;
                msgWrite[j].cob = 0;
                msgWrite[j].id = messageId; //FFFFFAAA
                msgWrite[j].length = (short)dataToSend.Length; //(short)AdvCan.DATALENGTH;
                msgWrite[j].data = dataToSend; //new byte[AdvCan.DATALENGTH];
                if (messageframeformats)
                {
                    msgWrite[j].flags += AdvCan.MSG_RTR;
                    msgWrite[j].length = 0;
                }
            }

            //if (!messageframeformats)
            //{
            if (nWriteCount > 0)
            {
                Array.Copy(msgWrite, nWriteCount, tmp_msgWrite, 0, nMsgCount - nWriteCount);
            }
            else
            {
                //for (int j = 0; j < nMsgCount; j++)
                //{
                //strTemp = Convert.ToString(SendIndex + 1 + j);
                //data = strTemp.ToCharArray();
                //for (i = 0; i < data.Length; i++)
                //{
                //    msgWrite[j].data[i] = Convert.ToByte(data[i] - 48);
                //}
                //msgWrite[j].length = (short)data.Length;
                nWriteCount = nMsgCount;
                Array.Copy(msgWrite, 0, tmp_msgWrite, 0, nMsgCount);
                //}
            }
            //}

            /**********************************************************************************************
              *  NOTE: acCanWrite usage
              *
              *    Description£º
              *       Users can use this interface to send data to CAN port which was opened.
              *       One or more frames can be selected each time.
              *
              *    Parameters:
              *       msgWrite                - managed buffer to write
              *       nWriteCount             - CAN frame number want to write each time
              *       pulNumberofWritten      - Real number of frames sent to driver.
              *
              *    In this example, we send 100 CAN frames defined by 'nMsgCount' each time by default.
              *    If user want to send one or more frames eache time, user can also change it as follows:
              *    Firstly, open CAN port and pass the value of 'MsgNumberOfReadBuffer' and 'MsgNumberOfWriteBuffer'arguments.
              *    About 'MsgNumberOfReadBuffer' and 'MsgNumberOfWriteBuffer', please see 'acCanPort' usage above.
              *    Secondly, define the msgWrite according to the frame number user want to send each time.
              *    Thirdly, define the value of 'nWriteCount'according to the frame number user want to send each time.
              *    In this examples, user can only change the value of 'nMsgCount' to change the count of frame to send each time.
             /**********************************************************************************************/
            nRet = CanTwoDevice.acCanWrite(tmp_msgWrite, nWriteCount, ref pulNumberofWritten); //Send frames
            if (nRet == AdvCANIO.TIME_OUT)
            {
            }
            else if (nRet == AdvCANIO.OPERATION_ERROR)
            {
            }
            else
            {
                nWriteCount -= pulNumberofWritten;
                SendIndex += pulNumberofWritten;
            }

            Thread.Sleep(2);
        }
    }
}