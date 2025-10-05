using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using FirmwareBootLoader.Helpers;
using static FirmwareBootLoader.Helpers.Definitions;


namespace BootLoader
{
    /// <summary>
    /// This class handles ASCII characters conversion
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class ASCIIToByteConverter
    {
        string[] genericData;
        string[] cMCUData;
        string[] pMCUData;
        string[] repeaterData;
        string[] iCBData;
        string[] catheterData;
        string[] remoteData;

        List<byte> cMCURS232Data;
        List<byte> pMCURS232Data;
        List<byte> repeaterRS232Data;
        List<byte> iCBRS232Data;
        List<byte> catheterRS232Data;
        List<byte> cPLDRS232Data;
        List<byte> remoteRS232Data;

        byte[] rS232DataArray;
        byte[] pMCURS232DataArray;
        byte[] repeaterRS232DataArray;
        byte[] iCBRS232DataArray;
        byte[] catheterRS232DataArray;
        byte[] remoteRS232DataArray;

        int dataTransmissionPercenatge = 0;

        List<byte[]> rS232DataConvertedBuffer;

        //packet

        byte[] previousPacket;
        byte[] currentPacket;
        byte[] nextPacket;

        private string endOfLine = "0D0A"; // CR
        private string endOfFile = "3b"; //;

        private int DCharchter = 13;
        private int ACharchter = 10;
        private byte[] EndOfLineCharchter;
        private bool isEndOfFileReached = false;

        private bool canSendEndTransmission = false;

        long sourceIndex = 0;
        int lenght = 8;
        int rS232DataArrayLength = 0;

        uint packetNumber = 0;

        byte[] initdata;

        Board boardType;

        private object _sync = new object();

        /// <summary>
        /// Default constructor
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public ASCIIToByteConverter()
        {
            CMCURS232Data = new List<byte>();
            PMCURS232Data = new List<byte>();
            RepeaterRS232Data = new List<byte>();
            ICBRS232Data = new List<byte>();
            CatheterRS232Data = new List<byte>();
            CPLDRS232Data = new List<byte>();
            RemoteRS232Data = new List<byte>();

            RS232DataConvertedBuffer = new List<byte[]>();

            previousPacket = new byte[8];
            currentPacket = new byte[8];
            nextPacket = new byte[8];
            initdata = new byte[8];

            Array.Clear(Initdata, 0, 8);

            EndOfLineCharchter = new byte[2];

            byte[] buffre = new byte[8];

            buffre = BitConverter.GetBytes(DCharchter);
            EndOfLineCharchter[0] = buffre[0];
            buffre = BitConverter.GetBytes(ACharchter);
            EndOfLineCharchter[1] = buffre[0];

        }

        /// <summary>
        /// Gets or sets the CMCU data
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string[] CMCUData
        {
            get => cMCUData;
            set => cMCUData = value;
        }

        /// <summary>
        /// Gets or sets the PMCU data
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string[] PMCUData
        {
            get => pMCUData;
            set => pMCUData = value;
        }

        /// <summary>
        /// Gets or sets the Repeater data
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string[] RepeaterData
        {
            get => repeaterData;
            set => repeaterData = value;
        }

        /// <summary>
        /// Gets or sets the ICB data
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string[] ICBData
        {
            get => iCBData;
            set => iCBData = value;
        }

        /// <summary>
        /// Gets or sets the Catheter data
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string[] CatheterData
        {
            get => catheterData;
            set => catheterData = value;
        }

        /// <summary>
        /// Gets or sets the Remote Control data
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string[] RemoteData
        {
            get => remoteData;
            set => remoteData = value;
        }

        /// <summary>
        /// Gets or sets the CMCU data list using RS232 format
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public List<byte> CMCURS232Data
        {
            get => cMCURS232Data;
            set => cMCURS232Data = value;
        }

        /// <summary>
        /// Gets or sets the PMCU data list using RS232 format
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public List<byte> PMCURS232Data
        {
            get => pMCURS232Data;
            set => pMCURS232Data = value;
        }

        /// <summary>
        /// Gets or sets the Repeater data list using RS232 format
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public List<byte> RepeaterRS232Data
        {
            get => repeaterRS232Data;
            set => repeaterRS232Data = value;
        }

        /// <summary>
        /// Gets or sets the ICB data list using RS232 format
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public List<byte> ICBRS232Data
        {
            get => iCBRS232Data;
            set => iCBRS232Data = value;
        }

        /// <summary>
        /// Gets or sets the Catheter data list using RS232 format
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public List<byte> CatheterRS232Data
        {
            get => catheterRS232Data;
            set => catheterRS232Data = value;
        }

        /// <summary>
        /// Gets or sets the Remote Control data list using RS232 format
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public List<byte> RemoteRS232Data
        {
            get => remoteRS232Data;
            set => remoteRS232Data = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the end of file is reached
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsEndOfFileReached
        {
            get => isEndOfFileReached;
            set => isEndOfFileReached = value;
        }

        /// <summary>
        /// Gets or sets the RS232 data converted buffer
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public List<byte[]> RS232DataConvertedBuffer
        {
            get => rS232DataConvertedBuffer;
            set => rS232DataConvertedBuffer = value;
        }

        /// <summary>
        /// Gets or sets the source index
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public long SourceIndex
        {
            get => sourceIndex;
            set => sourceIndex = value;
        }

        /// <summary>
        /// Gets or sets the packet number
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public uint PacketNumber
        {
            get => packetNumber;
            set => packetNumber = value;
        }

        /// <summary>
        /// Gets or sets the initialization data
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public byte[] Initdata
        {
            get => initdata;
            set => initdata = value;
        }

        /// <summary>
        /// Gets or sets the generic data
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string[] GenericData
        {
            get => genericData;
            set => genericData = value;
        }

        /// <summary>
        /// Gets or sets the board type
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public Board BoardType
        {
            get => boardType;
            set => boardType = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether we can send end transmission
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool CanSendEndTransmission
        {
            get => canSendEndTransmission;
            set => canSendEndTransmission = value;
        }

        /// <summary>
        /// Gets or sets the data transmission percentage
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int DataTransmissionPercenatge
        {
            get
            {
                lock (_sync)
                {
                    return dataTransmissionPercenatge;
                }
            }

            set
            {
                lock (_sync)
                {
                    dataTransmissionPercenatge = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the RS232 data array length
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int RS232DataArrayLength
        {
            get
            {

                return rS232DataArrayLength;


            }
            set
            {

                rS232DataArrayLength = value;

            }
        }

        /// <summary>
        /// Gets or sets the CPLD data list using RS232 format
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public List<byte> CPLDRS232Data
        {
            get => cPLDRS232Data;
            set => cPLDRS232Data = value;
        }

        /// <summary>
        /// Clears initialization data
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public void ClearInitData()
        {
            Array.Clear(Initdata, 0, 8);
        }

        /// <summary>
        /// Gets file from USB
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="path">file path</param>
        public void GetFileFromUSB(string path)
        {

        }

        /// <summary>
        /// Resets data packets
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public void ResetPackets()
        {
            Array.Clear(previousPacket, 0, 8);
            Array.Clear(currentPacket, 0, 8);
            Array.Clear(nextPacket, 0, 8);

            if (rS232DataArray != null)
                Array.Clear(rS232DataArray, 0, rS232DataArray.Length);

            if (CMCURS232Data != null && CMCURS232Data.Count > 0)
            {
                CMCURS232Data.Clear();
            }

            if (PMCURS232Data != null && PMCURS232Data.Count > 0)
            {
                PMCURS232Data.Clear();
            }

            if (RepeaterRS232Data != null && RepeaterRS232Data.Count > 0)
            {
                RepeaterRS232Data.Clear();
            }

            if (ICBRS232Data != null && ICBRS232Data.Count > 0)
            {
                ICBRS232Data.Clear();
            }

            if (CatheterRS232Data != null && CatheterRS232Data.Count > 0)
            {
                CatheterRS232Data.Clear();
            }

            if (CPLDRS232Data != null && CPLDRS232Data.Count > 0)
            {
                CPLDRS232Data.Clear();
            }

            if (RemoteRS232Data != null && RemoteRS232Data.Count > 0)
            {
                RemoteRS232Data.Clear();
            }

            if (genericData != null)
                Array.Clear(genericData, 0, genericData.Length);

            SourceIndex = 0;
            PacketNumber = 0;
            RS232DataArrayLength = 0;
            IsEndOfFileReached = false;
            CanSendEndTransmission = false;
        }

        /// <summary>
        /// Gets packet number
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="packetNumber">The packet number </param>
        /// <returns>Pakets</returns>
        public byte[] GetPacket(out uint packetNumber)
        {
            RS232DataArrayLength = rS232DataArray.Length;


            Array.Clear(currentPacket, 0, 8);

            if (PacketNumber == lenght)
            {
                PacketNumber = 0;
            }



            if (((SourceIndex + lenght) > RS232DataArrayLength) || (SourceIndex > RS232DataArrayLength))
            {

                lenght = rS232DataArray.Length - (int)SourceIndex;
                Array.Clear(currentPacket, 0, 8);
                Array.Copy(rS232DataArray, SourceIndex, currentPacket, 0, lenght);
                lenght = 8;
                CanSendEndTransmission = true;

            }


            Array.Copy(rS232DataArray, SourceIndex, currentPacket, 0, lenght);
            SourceIndex = SourceIndex + lenght;
            packetNumber = PacketNumber;
            PacketNumber++;

            if (RS232DataArrayLength != 0)
                DataTransmissionPercenatge = (int)((100 * (SourceIndex + lenght)) / RS232DataArrayLength);

            return currentPacket;

        }

        /// <summary>
        /// Initializes packets
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public void InitialiazePackets()
        {
            Array.Copy(rS232DataArray, SourceIndex, currentPacket, 0, lenght);
            SourceIndex = SourceIndex + lenght;

        }

        /// <summary>
        /// Converts data to array
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void ConvertDataToArray()
        {


            switch (BoardType)
            {
                case Board.CMCU:
                    rS232DataArray = CMCURS232Data?.ToArray();

                    break;

                case Board.PMCU:
                    rS232DataArray = PMCURS232Data?.ToArray();

                    break;

                case Board.Repeater:

                    rS232DataArray = RepeaterRS232Data?.ToArray();
                    break;

                case Board.ICB:
                    rS232DataArray = ICBRS232Data?.ToArray();

                    break;

                case Board.Catheter:
                    rS232DataArray = CatheterRS232Data?.ToArray();
                    break;

                case Board.CPLD:
                    rS232DataArray = CPLDRS232Data?.ToArray();
                    break;

                case Board.Remote:
                    rS232DataArray = RemoteRS232Data?.ToArray();
                    break;
            }
        }



        /// <summary>
        /// Formats line to RS232 format
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="line">The line to format</param>
        private void FormatLineToRS232Format(string line)
        {
            RS232DataConvertedBuffer.Clear();

            switch (BoardType)
            {
                case Board.CMCU:

                    if (IsEndOfFileReached)
                        return;

                    if (line.Contains(endOfFile))
                    {
                        RS232DataConvertedBuffer.Add(Encoding.Default.GetBytes((endOfFile)));
                        IsEndOfFileReached = true;

                        foreach (byte cmcu in RS232DataConvertedBuffer[0])
                        {
                            CMCURS232Data.Add(cmcu);
                        }
                    }
                    else
                    {
                        RS232DataConvertedBuffer.Add(Encoding.Default.GetBytes((line)));

                        foreach (byte cmcu in RS232DataConvertedBuffer[0])
                        {
                            CMCURS232Data.Add(cmcu);
                        }

                        CMCURS232Data.Add(EndOfLineCharchter[0]);
                        CMCURS232Data.Add(EndOfLineCharchter[1]);
                    }

                    break;

                case Board.PMCU:
                    if (IsEndOfFileReached)
                        return;

                    if (line.Contains(endOfFile))
                    {
                        RS232DataConvertedBuffer.Add(Encoding.Default.GetBytes((endOfFile)));
                        IsEndOfFileReached = true;

                        foreach (byte pmcu in RS232DataConvertedBuffer[0])
                        {
                            PMCURS232Data.Add(pmcu);
                        }
                    }
                    else
                    {
                        RS232DataConvertedBuffer.Add(Encoding.Default.GetBytes((line)));

                        foreach (byte pmcu in RS232DataConvertedBuffer[0])
                        {
                            PMCURS232Data.Add(pmcu);
                        }

                        PMCURS232Data.Add(EndOfLineCharchter[0]);
                        PMCURS232Data.Add(EndOfLineCharchter[1]);
                    }

                    break;

                case Board.Repeater:
                    if (IsEndOfFileReached)
                        return;

                    if (line.Contains(endOfFile))
                    {
                        RS232DataConvertedBuffer.Add(Encoding.Default.GetBytes((endOfFile)));
                        IsEndOfFileReached = true;

                        foreach (byte repeater in RS232DataConvertedBuffer[0])
                        {
                            RepeaterRS232Data.Add(repeater);
                        }
                    }
                    else
                    {
                        RS232DataConvertedBuffer.Add(Encoding.Default.GetBytes((line)));

                        foreach (byte repeater in RS232DataConvertedBuffer[0])
                        {
                            RepeaterRS232Data.Add(repeater);
                        }

                        RepeaterRS232Data.Add(EndOfLineCharchter[0]);
                        RepeaterRS232Data.Add(EndOfLineCharchter[1]);
                    }

                    break;

                case Board.ICB:
                    if (IsEndOfFileReached)
                        return;

                    if (line.Contains(endOfFile))
                    {
                        RS232DataConvertedBuffer.Add(Encoding.Default.GetBytes((endOfFile)));
                        IsEndOfFileReached = true;

                        foreach (byte icb in RS232DataConvertedBuffer[0])
                        {
                            ICBRS232Data.Add(icb);
                        }
                    }
                    else
                    {
                        RS232DataConvertedBuffer.Add(Encoding.Default.GetBytes((line)));

                        foreach (byte icb in RS232DataConvertedBuffer[0])
                        {
                            ICBRS232Data.Add(icb);
                        }

                        ICBRS232Data.Add(EndOfLineCharchter[0]);
                        ICBRS232Data.Add(EndOfLineCharchter[1]);
                    }

                    break;

                case Board.Catheter:
                    if (IsEndOfFileReached)
                        return;

                    if (line.Contains(endOfFile))
                    {
                        RS232DataConvertedBuffer.Add(Encoding.Default.GetBytes((endOfFile)));
                        IsEndOfFileReached = true;

                        foreach (byte catheter in RS232DataConvertedBuffer[0])
                        {
                            CatheterRS232Data.Add(catheter);
                        }
                    }
                    else
                    {
                        RS232DataConvertedBuffer.Add(Encoding.Default.GetBytes((line)));

                        foreach (byte catheter in RS232DataConvertedBuffer[0])
                        {
                            CatheterRS232Data.Add(catheter);
                        }

                        CatheterRS232Data.Add(EndOfLineCharchter[0]);
                        CatheterRS232Data.Add(EndOfLineCharchter[1]);
                    }

                    break;

                case Board.CPLD:
                    if (IsEndOfFileReached)
                        return;

                    if (line.Contains(endOfFile))
                    {
                        RS232DataConvertedBuffer.Add(Encoding.Default.GetBytes((endOfFile)));
                        IsEndOfFileReached = true;

                        foreach (byte cpld in RS232DataConvertedBuffer[0])
                        {
                            CPLDRS232Data.Add(cpld);
                        }
                    }
                    else
                    {
                        RS232DataConvertedBuffer.Add(Encoding.Default.GetBytes((line)));

                        foreach (byte cpld in RS232DataConvertedBuffer[0])
                        {
                            CPLDRS232Data.Add(cpld);
                        }

                        CPLDRS232Data.Add(EndOfLineCharchter[0]);
                        CPLDRS232Data.Add(EndOfLineCharchter[1]);
                    }

                    break;

                case Board.Remote:
                    if (IsEndOfFileReached)
                        return;

                    if (line.Contains(endOfFile))
                    {
                        RS232DataConvertedBuffer.Add(Encoding.Default.GetBytes((endOfFile)));
                        IsEndOfFileReached = true;

                        foreach (byte icb in RS232DataConvertedBuffer[0])
                        {
                            RemoteRS232Data.Add(icb);
                        }
                    }
                    else
                    {
                        RS232DataConvertedBuffer.Add(Encoding.Default.GetBytes((line)));

                        foreach (byte icb in RS232DataConvertedBuffer[0])
                        {
                            RemoteRS232Data.Add(icb);
                        }

                        RemoteRS232Data.Add(EndOfLineCharchter[0]);
                        RemoteRS232Data.Add(EndOfLineCharchter[1]);
                    }

                    break;
            }
        }


        /// <summary>
        /// Reads file
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="path">The path of the file</param>
        public void ReadFile(string path)
        {
            var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);

            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                try
                {
                    // stopWatch.Start();


                    genericData = File.ReadAllLines(path, Encoding.UTF8);


                    foreach (string line in genericData)
                    {
                        FormatLineToRS232Format(line);
                    }

                    ConvertDataToArray();

                    // InitialiazePackets();

                    //stopWatch.Stop();

                    //time = stopWatch.Elapsed.TotalMilliseconds.ToString();
                    //Test = CMCURS232Data;



                }

                catch (Exception ex)
                {
                    ex.ToString();

                }
            }

        }

    }

}
