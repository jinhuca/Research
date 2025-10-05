using System;

namespace Module.Console.Helpers
{
	/// <summary>
	/// This class is intended to define the  outer balloon pressure threshold.
	/// Safety classification: Death or SERIOUS INJURY is possible (IEC 62304 Class C).
	/// </summary>
	public class OuterBalloonPressureThreshold
	{
		/// <summary>
		/// Calculate OBP threshold
		/// </summary>
		/// <param name="PT3Value">PT3 value</param>
		/// <param name="PSIGReference">PSIG reference</param>
		/// <returns>OBP threshold</returns>
		/// <id>SF-SDS-0103</id>
		public static double GetThreshold(double PT3Value, double PSIGReference = 4.7)
		{
			return Math.Round(PSIGReference - PT3Value, 1);
		}
	}
}
