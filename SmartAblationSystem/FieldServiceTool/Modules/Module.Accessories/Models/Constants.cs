namespace Module.Accessories.Models
{
	public static class Constants
	{
		public const string TemperatureSeriesName = "temperatureSeries";
		public const string FlowSeriesName = "flowSeries";
		public const string DmsSeriesName = "dmsSeries";
		public const string EtsSeriesName = "etsSeries";

		public const int TemperatureDisplayCount = 240;
		public const int FlowMeterDisplayCount = 20;
		public const int EtsDisplayCount = 60;

		public const double SamplingIntervalInMilliseconds = 1000;

		public const double EsophagusTemperatureMinValue = 0;
		public const double EsophagusTemperatureMaxValue = 50;

		public const double TemperatureMinValue = -80;
		public const double TemperatureMaxValue = 40;
	}
}
