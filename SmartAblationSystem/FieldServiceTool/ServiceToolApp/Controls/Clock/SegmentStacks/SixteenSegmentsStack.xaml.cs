using System.Collections.ObjectModel;
using System.Windows;

namespace ServiceToolApp.Controls.Clock.SegmentStacks
{
	public partial class SixteenSegmentsStack
	{
		/// <summary>
		/// Stores chars from the split value string
		/// </summary>
		private ObservableCollection<CharItem> ValueChars;

		public SixteenSegmentsStack()
		{
			InitializeComponent();

			VerticalSegmentDivider = defVertDividerSixteen;
			HorizontalSegmentDivider = defHorizDividerSixteen;
		}

		public override void RaisePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			ValueChars = GetCharsArray();
			SegmentsArray.ItemsSource = ValueChars;
		}
	}
}
