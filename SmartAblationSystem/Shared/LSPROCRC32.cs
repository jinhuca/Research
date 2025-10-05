using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS232Communication
{
    /// <summary>
    /// Represents the LSPRO CRC32 calculator class
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public partial class LSPROCRC32
    {
        private static bool s_bInitialized = false;
        private static uint[] s_dwaLookupTable = new uint[256];


        /// <summary>
        /// Initialize the LSPRO CRC32 class
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public void Initialize()
        { //Initialize the Lookup Table with values used by the CRC-32 standard
            s_bInitialized = true;
            const uint dwPolynomial = 0x04c11db7; //Official polynomial for Winzip
            for (uint nValue = 0; nValue < 256; nValue++)
            { //For All bytes values
                s_dwaLookupTable[nValue] = (uint)(Reflect((uint)nValue, 8) << 24); //Fast Reflect 32 bits
                for (int j = 0; j < 8; j++)
                { //For 8 bits
                    s_dwaLookupTable[nValue] = (uint)((s_dwaLookupTable[nValue] << 1) ^ ((s_dwaLookupTable[nValue] & (1 << 31)) != 0 ? dwPolynomial : 0));
                }
                s_dwaLookupTable[nValue] = Reflect(s_dwaLookupTable[nValue], 32);
            }
        }

        /// <summary>
        /// Reflect the data
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="dwData">Data</param>
        /// <param name="byLength">Data length</param>
        /// <returns></returns>
        public uint Reflect(uint dwData, byte byLength)
        { //invert bits order of dwData for bits 0 to byLength - ex: if byLength=8,exchange bits 0&7, 1&6, 2&5, 3&4
            uint dwSource = dwData;
            Debug.Assert(s_bInitialized); //Must Call Initialize() before use
            int dwReflectedData = 0; //All Starting bits == 0;
            for (int i = 0; i < byLength; i++)
            {
                if ((dwSource & 1) != 0) // if bit[0] of wData == 1
                {
                    dwReflectedData |= 1 << (byLength - 1 - i); //Sets bit[byLength-1-i] of wReflectedData to 1;
                }
                dwSource >>= 1; //Shift bits to the right to process next bit
            }
            return (uint)dwReflectedData;
        }

        /// <summary>
        /// Get the CRC value 
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="abyData"> Data used to calculate the CRC</param>
        /// <returns>CRC value</returns>
        public uint GetValue(byte[] abyData)
        { //Return CRC32 Value of the Byte Array
            Debug.Assert(s_bInitialized); //Must Call Initialize() before use
            uint dwCRC32Value = 0xFFFFFFFF;
            for (int i = 0; i < abyData.Length; i++)
            {
                dwCRC32Value = (uint)((dwCRC32Value >> 8) ^ s_dwaLookupTable[(dwCRC32Value & 0xFF) ^ abyData[i]]);
            }
            return (~dwCRC32Value);
        }
    }
}
