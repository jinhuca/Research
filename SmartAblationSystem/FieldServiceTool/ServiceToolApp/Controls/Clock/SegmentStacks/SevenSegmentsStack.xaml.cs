using System.Collections.ObjectModel;
using System.Windows;

namespace ServiceToolApp.Controls.Clock.SegmentStacks
{
	public partial class SevenSegmentsStack
	{
		/// <summary>
		/// Stores chars from the split value string
		/// </summary>
		private ObservableCollection<CharItem> ValueChars = new ObservableCollection<CharItem>();

		public SevenSegmentsStack()
		{
			InitializeComponent();
		}

		public override void RaisePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			ValueChars = GetCharsArray();
			SegmentsArray.ItemsSource = ValueChars;
		}
	}
}
