using System;
using System.Timers;
using static System.DateTime;

namespace ServiceToolApp.Controls.Clock
{
	public partial class DigitalClock
	{
		public Time Time { get; set; }

		public DigitalClock()
		{
			InitializeComponent();
			InitializeTimeNow();
			DataContext = Time;
		}

		private void InitializeTimeNow()
		{
			Time = new Time();
			var timer = new Timer
			{
				AutoReset = true,
				Enabled = true,
				Interval = 100
			};

			timer.Elapsed += (s, e) =>
			{
				Time.DisplayDateTime = Now.ToString("yyyy-MM-dd HH:mm:ss");
				Time.TimeZone = TimeZoneInfo.Local.DisplayName;

				var daylightOffset = DateTimeOffset.Now.Offset;
				var format = daylightOffset.Hours == 0 ? "00" : daylightOffset.Hours > 0 ? "+00" : "00";
				Time.TimeZoneDaylight = $"(UTC{daylightOffset.Hours.ToString(format)}:{daylightOffset.Minutes:00}) {TimeZoneInfo.Local.DaylightName}";
				TimeZoneInfo.ClearCachedData();
			};
		}
	}
}
