using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS232Communication
{
    /// <summary>
    /// Represents the lSPRO Enumeartion class
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class LSPROEnumeartion
    {
        public byte[] GUID = new byte[] { 0x7b, 0x59, 0x37, 0x71, 0x1d, 0x63, 0xd3, 0x44, 0x8e, 0x08, 0x3b, 0xb5, 0x2d, 0x51, 0x8b, 0xd2 };

        public byte[] Count = new byte[1] { 0x04 };

        public byte Version = 1;

        public byte[] CCMPAndversion = new byte[] { 0x43, 0x43, 0x4d, 0x50, 0x01 };

        private byte[] consoleStatus;

        private byte[] ablationTime;

        private byte[] numberOfAblation;

        private byte[] temperature;

        /// <summary>
        /// Initialize LSPRO enumeartion class
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public LSPROEnumeartion()
        {
            ConsoleStatus = new byte[2] {0x00,0x01 };
            AblationTime = new byte[2] { 0x00, 0x12 };
            NumberOfAblation = new byte[2] { 0x00, 0x21 };
            Temperature = new byte[2] { 0x01, 0x21 };
        }


        /// <summary>
        /// Gets or sets the console status
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public byte[] ConsoleStatus
        {
            get => consoleStatus;
            set => consoleStatus = value;
        }


        /// <summary>
        /// Gets or sets the ablation time
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public byte[] AblationTime
        {
            get => ablationTime;
            set => ablationTime = value;
        }


        /// <summary>
        /// Gets or sets the number of ablation
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public byte[] NumberOfAblation
        {
            get => numberOfAblation;
            set => numberOfAblation = value;
        }


        /// <summary>
        /// Gets or sets the temperature
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public byte[] Temperature
        {
            get => temperature;
            set => temperature = value;
        }
    }

    /// <summary>
    /// CCMP command enumeration
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public enum CCMPCommand
    {
        CCMP_SERVICE_UNAVAILABLE = 0,
        CCMP_AUTHENTICATE = 1,
        CCMP_GET_VALUES = 2,
        CCMP_GET_TIMED_VALUES = 3,
        CCMP_SET_VALUES = 63,
        CCMP_TRAP_EVENT = 127,
    }

    /// <summary>
    /// LSPRO request enumeration
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public enum LSPRORequest
    {
        Authenticate_Request = 129, //0x81
        Get_Timed_Value_Request = 131, // 0x83

    }

    /// <summary>
    /// CCMP control bytes enumeration
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public enum CCMPControlBytes
    {
        CTRL_SEQ = 0x0F, 	//Identify that what the following byte is a control sequence
        CTRL_SEQ_IGNORE = 0x00,	//Identify that the control sequence detected is to be ignored
        CTRL_SEQ_PACKET_BEGIN = 0xFE,	//Identify the beginning of a packet
        CTRL_SEQ_PACKET_END = 0xFF	//Identify the end of a packet
    }

    /// <summary>
    /// Type numbering enumeration
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public enum TypeNumbering
    {
        CCMPT_INVALID = 0,	// 0x00 invalid data (0 byte)
        CCMPT_ARRAY	= 64,	//0x array
        CCMPT_UCHAR = 1,	// 0x01 unsigned char (1 byte)
        CCMPT_USHORT = 2,	// 0x02 unsigned short (2 bytes)
        CCMPT_UINT	= 4,	//0x04 unsigned int (4 bytes)
        CCMPT_ULONG	 =68,	//0x44 unsigned long (4 bytes)
        CCMPT_CHAR	= 33,	//0x21 signed char (1 byte)
        CCMPT_SHORT =34,	//0x22 signed short (2 bytes)
        CCMPT_INT = 36,	    //0x24 signed int (4 bytes)
        CCMPT_FLOAT	= 164, 	//0xA4signed float (4 bytes)
        CCMPT_LONG	= 36,	//0x24 signed long (4 bytes)
        CCMPT_DOUBLE =40	//0x28 signed double (8 bytes)

    }


}
