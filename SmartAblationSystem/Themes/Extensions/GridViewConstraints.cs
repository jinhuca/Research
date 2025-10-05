using System.Windows;
using System.Windows.Controls;

namespace CustomControls.Extensions
{
	public static class GridViewConstraints
	{
		public static readonly DependencyProperty MinColumnWidthProperty = DependencyProperty.RegisterAttached(
			"MinColumnWidth", 
			typeof(double), 
			typeof(GridViewConstraints), 
			new PropertyMetadata(75d, (s, e) =>
			{
				if(s is ListView listView)
				{
					listView.Loaded += (lvs, lve) =>
					{
						if(listView.View is GridView view)
						{
							foreach(var column in view.Columns)
							{
								SetMinWidth(listView, column);
								((System.ComponentModel.INotifyPropertyChanged)column).PropertyChanged += (cs, ce) =>
								{
									if(ce.PropertyName == nameof(GridViewColumn.ActualWidth))
									{
										SetMinWidth(listView, column);
									}
								};
							}
						}
					};
				}
			}));

		private static void SetMinWidth(ListView listView, GridViewColumn column)
		{
			var minWidth = (double)listView.GetValue(MinColumnWidthProperty);
			if(column.Width < minWidth)
			{
				column.Width = minWidth;
			}
		}

		public static double GetMinColumnWidth(DependencyObject obj) => (double)obj.GetValue(MinColumnWidthProperty);

		public static void SetMinColumnWidth(DependencyObject obj, double value) => obj.SetValue(MinColumnWidthProperty, value);

		public static readonly DependencyProperty MaxColumnWidthProperty = DependencyProperty.RegisterAttached(
			"MaxColumnWidth",
			typeof(double),
			typeof(GridViewConstraints),
			new PropertyMetadata(75d, (s, e) =>
			{
				if(s is ListView listView)
				{
					listView.Loaded += (lvs, lve) =>
					{
						if(listView.View is GridView view)
						{
							foreach(var column in view.Columns)
							{
								SetMaxWidth(listView, column);
								((System.ComponentModel.INotifyPropertyChanged)column).PropertyChanged += (cs, ce) =>
								{
									if(ce.PropertyName == nameof(GridViewColumn.ActualWidth))
									{
										SetMaxWidth(listView, column);
									}
								};
							}
						}
					};
				}
			}));

		private static void SetMaxWidth(ListView listView, GridViewColumn column)
		{
			var maxWidth = (double)listView.GetValue(MaxColumnWidthProperty);
			if (column.Width > maxWidth)
			{
				column.Width = maxWidth;
			}
		}

		public static double GetMaxColumnWidth(DependencyObject obj) => (double)obj.GetValue(MaxColumnWidthProperty);

		public static void SetMaxColumnWidth(DependencyObject obj, double value) => obj.SetValue(MaxColumnWidthProperty, value);
	}
}
