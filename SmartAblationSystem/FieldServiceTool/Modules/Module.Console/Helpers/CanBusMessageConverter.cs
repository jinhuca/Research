using System;

namespace Module.Console.Helpers
{
  /// <summary>
  /// This class handles the Can Bus messages converion
  /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
  /// </summary>
  public static class CanBusMessageConverter
  {
    /// <summary>
    /// Converts Can Bus message to Decimal Data
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="data">A bytes array representing the Can Bus data to convert.</param>
    /// <param name="index">An integer representing an index.</param>
    /// <returns>A double representing the converted value.</returns>
    public static double ConverteDecimalData(byte[] data, int index)
    {
      /********************************************************
      When the 16 bit value is left shifted, its MSB is moved to
      the MSB of the 32 bit signed value.When the right shift is performed,
      sign extension is performed - i.e.the right hand bits are filled with the sign bit.

          00000000 00000000 11110000 11110000 << 16
        = 11110000 11110000 00000000 00000000

        11110000 11110000 00000000 00000000 >> 16
      = 11111111 11111111 11110000 11110000

      *******************************************************/
      return (((data[index] * 256 + data[index + 1]))) / 10.0;
      // these code wase used to calculate the negativ values
      // return (((data[index] * 256 + data[index + 1] << 16)) >> 16) / 10.0;
    }

    /// <summary>
    /// Converts Can Bus message to ECG Decimal Data
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="data">A bytes array representing the Can Bus data to convert.</param>
    /// <param name="index">An integer representing an index.</param>
    /// <param name="multiplicationFactor">A double representing a multiplication factor.</param>
    /// <returns>A double representing the converted value.</returns>
    public static double ConverteECGDecimalData(byte[] data, int index, double multiplicationFactor = 10.0)
    {
      /********************************************************
      When the 16 bit value is left shifted, its MSB is moved to
      the MSB of the 32 bit signed value.When the right shift is performed,
      sign extension is performed - i.e.the right hand bits are filled with the sign bit.

          00000000 00000000 11110000 11110000 << 16
        = 11110000 11110000 00000000 00000000

        11110000 11110000 00000000 00000000 >> 16
      = 11111111 11111111 11110000 11110000

      *******************************************************/

      // these code wase used to calculate the negativ values
      return (((data[index] * 256 + data[index + 1] << 16)) >> 16) / multiplicationFactor;
    }

    /// <summary>
    /// Converts Can Bus message to Blood Pressure Data
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="data">A bytes array representing the Can Bus data to convert.</param>
    /// <param name="convertedData">double array representing converted data.</param>
    /// <param name="multiplicationFactor">A double representing a multiplication factor.</param>
    /// <id>SF-SDS-0101</id>
    public static void ConverteBloodPressureData(byte[] data, out double[] convertedData, double multiplicationFactor = 100.0)
    {
      convertedData = new double[4];

      /********************************************************
      When the 16 bit value is left shifted, its MSB is moved to
      the MSB of the 32 bit signed value.When the right shift is performed,
      sign extension is performed - i.e.the right hand bits are filled with the sign bit.

          00000000 00000000 11110000 11110000 << 16
        = 11110000 11110000 00000000 00000000

        11110000 11110000 00000000 00000000 >> 16
      = 11111111 11111111 11110000 11110000

      *******************************************************/

      // these code wase used to calculate the negativ values

      convertedData[0] = (((data[0] * 256 + data[1] << 16)) >> 16) / multiplicationFactor;
      convertedData[1] = (((data[2] * 256 + data[3] << 16)) >> 16) / multiplicationFactor;
      convertedData[2] = (((data[4] * 256 + data[5] << 16)) >> 16) / multiplicationFactor;
      convertedData[3] = (((data[6] * 256 + data[7] << 16)) >> 16) / multiplicationFactor;
    }

    /// <summary>
    /// Converts Can Bus message to Negative Decimal Data
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="data">A bytes array representing the Can Bus data to convert.</param>
    /// <param name="index">An integer representing an index.</param>
    /// <returns>A double representing the converted value.</returns>
    public static double ConverteNegativDecimalData(byte[] data, int index)
    {
      /********************************************************
      When the 16 bit value is left shifted, its MSB is moved to
      the MSB of the 32 bit signed value.When the right shift is performed,
      sign extension is performed - i.e.the right hand bits are filled with the sign bit.

          00000000 00000000 11110000 11110000 << 16
        = 11110000 11110000 00000000 00000000

        11110000 11110000 00000000 00000000 >> 16
      = 11111111 11111111 11110000 11110000

      *******************************************************/

      // these code wase used to calculate the negativ values
      return (((data[index] * 256 + data[index + 1] << 16)) >> 16) / 10.0;
    }

    /// <summary>
    /// Converts Can Bus FM1 message to Negative Decimal Data
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="data">A bytes array representing the Can Bus data to convert.</param>
    /// <param name="index">An integer representing an index.</param>
    /// <returns>A double representing the converted value.</returns>
    public static double ConverteFM1NegativDecimalData(byte[] data, int index)
    {
      /********************************************************
      When the 16 bit value is left shifted, its MSB is moved to
      the MSB of the 32 bit signed value.When the right shift is performed,
      sign extension is performed - i.e.the right hand bits are filled with the sign bit.

          00000000 00000000 11110000 11110000 << 16
        = 11110000 11110000 00000000 00000000

        11110000 11110000 00000000 00000000 >> 16
      = 11111111 11111111 11110000 11110000

      *******************************************************/

      // these code wase used to calculate the negativ values
      return (((data[index] * 256 + data[index + 1] << 16)) >> 16);
    }

    /// <summary>
    /// Converts Can Bus message to Info Data
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="data">A bytes array representing the Can Bus data to convert.</param>
    /// <param name="index">An integer representing an index.</param>
    /// <returns>An integer representing the converted value.</returns>
    public static int ConverteInfoData(byte[] data, int index)
    {
      /********************************************************
      When the 16 bit value is left shifted, its MSB is moved to
      the MSB of the 32 bit signed value.When the right shift is performed,
      sign extension is performed - i.e.the right hand bits are filled with the sign bit.

          00000000 00000000 11110000 11110000 << 16
        = 11110000 11110000 00000000 00000000

        11110000 11110000 00000000 00000000 >> 16
      = 11111111 11111111 11110000 11110000

      *******************************************************/
      return ((data[index] * 256 + data[index + 1] << 16)) >> 16;
    }

    /// <summary>
    /// Converts Can Bus message to Info Data
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="data">A bytes array representing the Can Bus data to convert.</param>
    /// <returns>An integer representing the converted value.</returns>
    public static Int64 ConvertStatusErrorData(byte[] data)
    {
      return (UInt32)(data[0] << 24) | (UInt32)(data[1] & 0xFF) << 16 | (UInt32)(data[2] & 0xFF) << 8 | (UInt32)(data[3] & 0xFF);
    }

    /// <summary>
    /// Converts Can Bus message to Decimal Data FM1
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="data"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static double ConverteDecimalDataFM1(byte[] data, int index)
    {
      /********************************************************
      When the 16 bit value is left shifted, its MSB is moved to
      the MSB of the 32 bit signed value.When the right shift is performed,
      sign extension is performed - i.e.the right hand bits are filled with the sign bit.

          00000000 00000000 11110000 11110000 << 16
        = 11110000 11110000 00000000 00000000

        11110000 11110000 00000000 00000000 >> 16
      = 11111111 11111111 11110000 11110000

      *******************************************************/
      return (((data[index] * 256 + data[index + 1])));
      // these code wase used to calculate the negativ values
      // return (((data[index] * 256 + data[index + 1] << 16)) >> 16) / 10.0;
    }

    /// <summary>
    /// Converts catheter info data
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="data"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static int ConverteCatheterInfoData(byte[] data, int index)
    {
      return (((data[index] * 256 + data[index + 1])));
    }
    /// <summary>
    /// Converts valves status data
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public static int ConvertValvesStatusData(byte[] data)
    {
      byte[] localData = new byte[8];
      Array.Clear(localData, 0, 8);

      localData[0] = data[3];
      localData[1] = data[4];

      return (((localData[1] * 256 + localData[0])));
    }

    /// <summary>
    /// Converts ramp up and ramp down time value to double
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public static double ConvertRampUpTimeAndRampDownTimeByStepData(byte[] data, int index)
    {
      /********************************************************
      When the 16 bit value is left shifted, its MSB is moved to
      the MSB of the 32 bit signed value.When the right shift is performed,
      sign extension is performed - i.e.the right hand bits are filled with the sign bit.

          00000000 00000000 11110000 11110000 << 16
        = 11110000 11110000 00000000 00000000

        11110000 11110000 00000000 00000000 >> 16
      = 11111111 11111111 11110000 11110000

      *******************************************************/

      // these code wase used to calculate the negativ values
      return (((data[index] * 256 + data[index + 1] << 16)) >> 16); // * 10.0;
    }

    /// <summary>
    /// Converts ramp up and ramp down pressure value to double
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public static double ConvertRampUpPressureAndRampDownPressureByStepData(byte[] data, int index)
    {
      /********************************************************
      When the 16 bit value is left shifted, its MSB is moved to
      the MSB of the 32 bit signed value.When the right shift is performed,
      sign extension is performed - i.e.the right hand bits are filled with the sign bit.

          00000000 00000000 11110000 11110000 << 16
        = 11110000 11110000 00000000 00000000

        11110000 11110000 00000000 00000000 >> 16
      = 11111111 11111111 11110000 11110000

      *******************************************************/

      // these code wase used to calculate the negativ values
      return (((data[index] * 256 + data[index + 1] << 16)) >> 16) / 10.0;
    }

    /// <summary>
    /// Converts module key value to int
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public static int ConvertModuleKeyData(byte[] data)
    {

      return (((data[1] * 256 + data[0])));
    }
    /// <summary>
    /// Converts upgreade status data to int
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public static int ConvertUpgradeStatusData(byte[] data)
    {

      return (((data[3] * 256 + data[2])));
    }
  }
}