using System;

namespace Module.Infrastructure.Constants
{
	public static class ReadyStateConstants
	{
		public const double IBP_INFLATION_STABLE_VALUE = 2.5;
		public const double IBP_INFLATION_STABLE_VALUE_VARIANT = 0.3;
		public const double IBPDelta = 2.0d;

		public const int CHECK_IDLE_STATE_TIMEOUT_IN_SEC = 3000;
		public const int CHECK_CATHETER_READY_TIMEOUT_IN_MILISEC = 8000;
		public const int IDLE_TO_READY_STATE_TIMEOUT_IN_SEC = 10000;
		public const int READY_TO_INFLATION_STATE_TIMEOUT_IN_SEC = 30000;
		public const int READY_TO_ABLATION_STATE_TIMEOUT_IN_SEC = 40000;
		public const int IBP_STABLIZATION_TIME_IN_SEC = 5;
		public const int IBP_STABLIZATION_TIMEOUT_IN_SEC = 25;
		public const int RECORDING_TEST_DATA_TIME_IN_SEC = 10;

		public const string SensorNameTC1 = "TEMP";
		public const string SensorNameIBP = "IBP";
		public const string SensorNameOBP = "OBP";
		public const string SensorNamePT2 = "PT2";
		public const string SensorNamePT3 = "PT3";
		public const string SensorNamePT4 = "PT4";
		public const string SensorNameFM1 = "FM1";

		public const string STATE_UNKNOWN = "UNKNOWN";
		public const string STATE_IDLE = "IDLE";
		public const string STATE_READY = "READY";
		public const string STATE_INFLATION = "INFLATION";
		public const string STATE_ABLATION = "ABLATION";
		public const string STATE_TRANSITION = "TRANSITION";
		public const string STATE_THAWING = "THAWING";
		public const string STATE_EXCEPTION = "EXCEPTION";

		public const double FM1AvgThreshold = 40.0;
		public const string FM1Identity = "Measured FM1 = ";
		public const string FlowMeterSymbol = " sccm";
		public const string FM1RangeMessage = "The average flow is too high (greater than 40 sccm).\n";

		public const double PT1AvgThreshold = 700.0;
		public const string PT1Identity = "Measured PT1 = ";
		public const string PressureUnitSymbol = " psig";
		public const string PT1RangeMessage = "The tank pressure is too low (less than 700 psig). There isn't enough pressure to properly complete the test. Consider changing the refrigerant tank and restart the test.\n";

		public const double LC1AvgThreshold = 4.5;
		public const string LC1Identity = "Measured LC1 = ";
		public const string WeightPoundSymbol = " lbs";
		public const string LC1RangeMessage = "The amount of remaining N2O is too low (less than 4.5 lbs). There isn't enough gas to properly complete the test. Consider changing the refrigerant tank and restart the test.\n";

		public const string CelsiusSymbol = " °C";

		public const string IBPIdentity = "Measured IBP = ";
		public const string IBPRangeMessage = "The inner balloon pressure is too high (greater than  ";
		
		public const string OBPIdentity = "Measured OBP = ";
		public const string OBPRangeMessage = "The outer balloon pressure is too high (greater than  ";

		public static bool AreDoubleValuesEqual(this double initialValue, double value, double bias)
		{
			return Math.Abs(initialValue - value) <= bias;
		}
	}
}
