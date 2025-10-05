using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS232Communication
{
    /// <summary>
    /// Represents the LSPRO data builder class
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public static class LSPRODataBuilder
    {

        private static byte[] ConsoleStatus = new byte[3] {0x00,0x01, 0x24};
        private static byte[] AblationTime = new byte[3] { 0x00, 0x12, 0x28 };
        private static byte[] NumberOfAblation = new byte[3] { 0x00, 0x21, 0x24 };
        private static byte[] Temperature = new byte[3] { 0x01, 0x21,0x28 };


        /// <summary>
        /// Append data At the beginning of an array
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="firstByteArray">first byte array</param>
        /// <param name="secondByteArray">second byte array</param>
        /// <returns>concatenated data</returns>
        public static byte[] AppendDataAtTheBeginningOfAnArray(byte[] firstByteArray, byte[] secondByteArray)
        {
            List<byte> firstList = new List<byte>(firstByteArray);
            List<byte> secondList = new List<byte>(secondByteArray);

            //start from second
            secondList.AddRange(firstList);

            return secondList.ToArray();


        }


        /// <summary>
        /// Append data at the end of an array
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="firstByteArray">first byte array</param>
        /// <param name="secondByteArray">second byte array</param>
        /// <returns>concatenated data</returns>
        public static byte[] AppendDataAtTheEndOfAnArray(byte[] firstByteArray, byte[] secondByteArray)
        {

            List<byte> firstList = new List<byte>(firstByteArray);
            List<byte> secondList = new List<byte>(secondByteArray);

            //Start from first
            firstList.AddRange(secondList);

            return firstList.ToArray();
        }

        /// <summary>
        /// Format time for LSPRO format
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="time">time to format</param>
        /// <returns>Formated time</returns>
        public static byte[] FormatTime(double time)
        {

            long _time = BitConverter.DoubleToInt64Bits(time);


            byte[] data = new byte[8];
            Array.Clear(data, 0, 8);


            data = BitConverter.GetBytes(_time);

            return AppendDataAtTheEndOfAnArray(AblationTime, data);
        }


        /// <summary>
        /// Format temperature
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="temperature">Temperature to format</param>
        /// <returns>Formated temperature</returns>
        public static byte[] FormatTemperature(double temperature)
        {
            //double test = -40.50277099609375;

            double _temperature = temperature * 1.00000000000000;
            long _temperatureLong = BitConverter.DoubleToInt64Bits(_temperature);

            //long longValue;
            //longValue = BitConverter.DoubleToInt64Bits(test);

            //var bytes = BitConverter.GetBytes(test);
            //var x = BitConverter.ToInt64(bytes, 0);
            //string okoko=  "0x" + x.ToString("X8");

            byte[] data = new byte[8];
            Array.Clear(data, 0, 8);

            
            data = BitConverter.GetBytes(_temperatureLong);

            //byte[] invertedData = new byte[8];
            //Array.Clear(invertedData, 0, 8);

            //for (int i = 0; i <= 7; i++)
            //{
            //    invertedData[8 + i] = data[7 - i];
            //}
            return AppendDataAtTheEndOfAnArray(Temperature, data);

        }

        /// <summary>
        /// Format console status
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="status">Status to format</param>
        /// <returns>Formated status</returns>
        public static byte[] FormatConsoleStatus(int status)
        {
            byte[] data = new byte[4];
            Array.Clear(data, 0, 4);

            data = BitConverter.GetBytes(status);

            //int refernceIndex = Array.FindLastIndex(data, element => element != 0);

            //byte[] invertedData = new byte[4];
            //Array.Clear(invertedData, 0, 4);

            //for (int i = refernceIndex; i >= 0; i--)
            //{
            //    invertedData[invertedData.Length - 1 - (refernceIndex - i)] = data[i];
            //}

            return AppendDataAtTheEndOfAnArray(ConsoleStatus, data);

        }

        /// <summary>
        /// Format number of ablation
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="ablationNumber">Ablation number to format</param>
        /// <returns>Formated ablation number</returns>
        public static byte[] FormatNumberOfAblation(int ablationNumber)
        {
            byte[] data = new byte[4];
            Array.Clear(data, 0, 4);

            data = BitConverter.GetBytes(ablationNumber);

            int refernceIndex = Array.FindLastIndex(data, element => element != 0);

            //byte[] invertedData = new byte[4];
            //Array.Clear(invertedData, 0, 4);

            //for (int i = refernceIndex; i >= 0; i--)
            //{
            //    invertedData[invertedData.Length - 1 - (refernceIndex - i)] = data[i];
            //}

            return AppendDataAtTheEndOfAnArray(NumberOfAblation, data);

        }
    }
}
