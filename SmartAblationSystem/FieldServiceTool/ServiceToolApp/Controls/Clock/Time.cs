using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ServiceToolApp.Controls.Clock
{
	public class Time : INotifyPropertyChanged
	{
		private string _displayDateTime = string.Empty;
		public string DisplayDateTime
		{
			get => _displayDateTime;
			set
			{
				if (_displayDateTime == value) return;
				_displayDateTime = value;
				RaisePropertyChanged();
			}
		}

		private string _timeZone = string.Empty;
		public string TimeZone
		{
			get => _timeZone;
			set
			{
				if (_timeZone == value) return;
				_timeZone = value;
				RaisePropertyChanged();
			}
		}

		private string _timeZoneDaylight = string.Empty;
		public string TimeZoneDaylight
		{
			get => _timeZoneDaylight;
			set
			{
				if (_timeZoneDaylight == value) return;
				_timeZoneDaylight = value;
				RaisePropertyChanged();
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}