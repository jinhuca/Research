using ServiceToolApp.Controls.Clock.Segments;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace ServiceToolApp.Controls.Clock.SegmentStacks
{
	/// <summary>
	/// A base class for stack of segment controls
	/// </summary>
	[DesignTimeVisible(true)]
	public class SegmentsStackBase : SegmentBase
	{
		public int ElementsCount
		{
			get => (int)GetValue(ElementsCountProperty);
			set => SetValue(ElementsCountProperty, value);
		}

		public static DependencyProperty ElementsCountProperty = DependencyProperty.Register(
			nameof(ElementsCount),
			typeof(int),
			typeof(SegmentsStackBase),
			new PropertyMetadata(1, CountChanged));

		public SegmentsStackBase() => PropertyChanged += RaisePropertyChanged;

		public virtual void RaisePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
		}

		private static void CountChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			var segments = (SegmentsStackBase)sender;
			segments.RaisePropertyChanged(sender, e);
		}

		public ObservableCollection<CharItem> GetCharsArray()
		{
			if (Value is null) return new ObservableCollection<CharItem>();
			// converts value to char array
			char[] charArray = Value.ToCharArray();
			// the dots count
			var dotCount = charArray.Count(c => c == '.');
			// the colons count
			var colonCount = charArray.Count(c => c == ':');

			// the chars count without dots and colons
			var charCount = charArray.Count() - dotCount;

			var valueChars = new ObservableCollection<CharItem>();
			int index = 0;

			if (!charArray.Any()) return valueChars;
			for (int i = 0; i < ElementsCount; i++)
			{
				// sets properties for the each seven segment item
				var item = new CharItem
				{
					ShowDot = ShowDot,
					ShowColon = ShowColon,
					FillBrush = FillBrush,
					SelectedFillBrush = SelectedFillBrush,
					PenColor = PenColor,
					SelectedPenColor = SelectedPenColor,
					PenThickness = PenThickness,
					GapWidth = GapWidth,
					RoundedCorners = RoundedCorners,
					TiltAngle = TiltAngle,
					VerticalSegmentDivider = VerticalSegmentDivider,
					HorizontalSegmentDivider = HorizontalSegmentDivider
				};

				valueChars.Add(item);

				if (i < ElementsCount - charCount) continue;
				if (index <= charArray.Count() - 1)
				{
					// sets char for the element
					if (charArray[index] != '.' && charArray[index] != ':')
					{
						valueChars[i].Item = charArray[index];
					}

					// sets ":" for the element
					if (charArray[index] == ':')
					{
						valueChars[i].OnColon = true;
					}

					// sets dot for the element
					if (charArray[index] == '.')
					{
						valueChars[i - 1].OnDot = true;
						valueChars[i].Item = charArray[index + 1];
						index++;
					}
				}
				index++;
			}
			if (ElementsCount >= charCount)     // sets dot for the last element if required
			{
				if (charArray[charArray.Count() - 1] == '.')
				{
					var item = valueChars.Last();
					item.OnDot = true;
				}
			}
			else
			{
				if (charArray[index] == '.')
				{
					var item = valueChars[ElementsCount - 1];
					item.OnDot = true;
				}
			}
			return valueChars;
		}
	}
}
