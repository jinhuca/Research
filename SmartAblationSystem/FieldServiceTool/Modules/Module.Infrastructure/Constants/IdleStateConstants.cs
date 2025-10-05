namespace Module.Infrastructure.Constants
{
	public class IdleStateConstants
	{
		public const string FM1ReadingAverage = "FM1 = ";
		public const string PT1ReadingAverage = "PT1 = ";
		public const string LC1ReadingAverage = "LC1 = ";
		public const string PT3ReadingAverage = "PT3 = ";
		public const string TS1ReadingAverage = "TS1 = ";

		public const long RecordingTS1PeriodInSecond = 16;
    public const long WaitingTS1TimeoutInSecond = 600;

		public const double FM1AvgThreshold = 40.0;
		public const double PT1AvgThreshold = 700.0;
		public const double LC1AvgThreshold = 4.5;
		public const double TS1AvgThreshold = -25.0;
		public const string ErrorMessage = " reading out of range.";

		public const string FM1Identity = "Measured FM1 = ";
		public const string FlowMeterSymbol = " sccm";
		public const string FM1RangeMessage = "The average flow is too high (greater than 40 sccm).\n";

		public const string PT1Identity = "Measured PT1 = ";
		public const string PressureUnitSymbol = " psig";
		public const string PT1RangeMessage = "The tank pressure is too low (less than 700 psig). There isn't enough pressure to properly complete the test. Consider changing the refrigerant tank and restart the test.\n";

		public const string LC1Identity = "Measured LC1 = ";
		public const string WeightPoundSymbol = " lbs";
		public const string LC1RangeMessage = "The amount of remaining N2O is too low (less than 4.5 lbs). There isn't enough gas to properly complete the test. Consider changing the refrigerant tank and restart the test.\n";

		public const string TS1Identity = "Measured TS1 = ";
		public const string CelsiusSymbol = " °C";
		public const string TS1RangeMessage = "The sub-cooler temperature is too warm (greater than -25 °C) to start the test.\n";
	}
}
