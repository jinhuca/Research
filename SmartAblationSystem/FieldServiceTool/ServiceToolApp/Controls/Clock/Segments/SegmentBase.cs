using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ServiceToolApp.Controls.Clock.Segments
{
	[DesignTimeVisible(true)]
	public class SegmentBase : UserControl, ISegment
	{
		protected event PropertyChangedCallback PropertyChanged = (sender, e) => { };
		protected static double defVertDividerSixteen = 7.5;
		protected static double defHorizDividerSixteen = 11.5;

		public Color PenColor
		{
			get => (Color)GetValue(PenColorProperty);
			set => SetValue(PenColorProperty, value);
		}

		public static DependencyProperty PenColorProperty = DependencyProperty.Register(
			nameof(PenColor),
			typeof(Color),
			typeof(SegmentBase),
			new PropertyMetadata(Colors.Transparent, VisualChanged));

		public double PenThickness
		{
			get => (double)GetValue(PenThicknessProperty);
			set => SetValue(PenThicknessProperty, value);
		}

		public static DependencyProperty PenThicknessProperty = DependencyProperty.Register(
			nameof(PenThickness),
			typeof(double),
			typeof(SegmentBase),
			new PropertyMetadata(1.0, VisualChanged));

		public Color SelectedPenColor
		{
			get => (Color)GetValue(SelectedPenColorProperty);
			set => SetValue(SelectedPenColorProperty, value);
		}

		public static DependencyProperty SelectedPenColorProperty = DependencyProperty.Register(
			nameof(SelectedPenColor),
			typeof(Color),
			typeof(SegmentBase),
			new PropertyMetadata(Colors.Black, VisualChanged));

		public Brush FillBrush
		{
			get => (Brush)GetValue(FillBrushProperty);
			set => SetValue(FillBrushProperty, value);
		}

		public static DependencyProperty FillBrushProperty = DependencyProperty.Register(
			nameof(FillBrush),
			typeof(Brush),
			typeof(SegmentBase),
			new PropertyMetadata(new SolidColorBrush(Colors.Transparent), VisualChanged));

		public Brush SelectedFillBrush
		{
			get => (Brush)GetValue(SelectedFillBrushProperty);
			set => SetValue(SelectedFillBrushProperty, value);
		}

		public static DependencyProperty SelectedFillBrushProperty = DependencyProperty.Register(
			nameof(SelectedFillBrush),
			typeof(Brush),
			typeof(SegmentBase),
			new PropertyMetadata(new SolidColorBrush(Colors.Green), VisualChanged));

		public string Value
		{
			get => (string)GetValue(ValueProperty);
			set => SetValue(ValueProperty, value);
		}

		public static DependencyProperty ValueProperty = DependencyProperty.Register(
			nameof(Value),
			typeof(string),
			typeof(SegmentBase),
			new PropertyMetadata(string.Empty, VisualChanged));

		public double GapWidth
		{
			get => (double)GetValue(GapWidthProperty);
			set => SetValue(GapWidthProperty, value);
		}

		public static DependencyProperty GapWidthProperty = DependencyProperty.Register(
			nameof(GapWidth),
			typeof(double),
			typeof(SegmentBase),
			new PropertyMetadata(3.0, VisualChanged));

		public bool ShowDot
		{
			get => (bool)GetValue(ShowDotProperty);
			set => SetValue(ShowDotProperty, value);
		}

		public static DependencyProperty ShowDotProperty = DependencyProperty.Register(
			nameof(ShowDot),
			typeof(bool),
			typeof(SegmentBase),
			new PropertyMetadata(false, VisualChanged));

		public bool OnDot
		{
			get => (bool)GetValue(OnDotProperty);
			set => SetValue(OnDotProperty, value);
		}

		public static DependencyProperty OnDotProperty = DependencyProperty.Register(
			nameof(OnDot),
			typeof(bool),
			typeof(SegmentBase),
			new PropertyMetadata(false, VisualChanged));

		public bool ShowColon
		{
			get => (bool)GetValue(ShowColonProperty);
			set => SetValue(ShowColonProperty, value);
		}

		public static DependencyProperty ShowColonProperty = DependencyProperty.Register(
			nameof(ShowColon),
			typeof(bool),
			typeof(SegmentBase),
			new PropertyMetadata(false, VisualChanged));

		public bool OnColon
		{
			get => (bool)GetValue(OnColonProperty);
			set => SetValue(OnColonProperty, value);
		}

		public static DependencyProperty OnColonProperty = DependencyProperty.Register(
			nameof(OnColon),
			typeof(bool),
			typeof(SegmentBase),
			new PropertyMetadata(false, VisualChanged));

		public double TiltAngle
		{
			get => (double)GetValue(TiltAngleProperty);
			set => SetValue(TiltAngleProperty, value);
		}

		public static DependencyProperty TiltAngleProperty = DependencyProperty.Register(
			nameof(TiltAngle),
			typeof(double),
			typeof(SegmentBase),
			new PropertyMetadata(10.0, VisualChanged));

		public bool RoundedCorners
		{
			get => (bool)GetValue(RoundedCornersProperty);
			set => SetValue(RoundedCornersProperty, value);
		}

		public static DependencyProperty RoundedCornersProperty = DependencyProperty.Register(
			nameof(RoundedCorners),
			typeof(bool),
			typeof(SegmentBase),
			new PropertyMetadata(false, VisualChanged));

		public List<int> SelectedSegments
		{
			get => (List<int>)GetValue(SelectedSegmentsProperty);
			set => SetValue(SelectedSegmentsProperty, value);
		}

		public static DependencyProperty SelectedSegmentsProperty = DependencyProperty.Register(
			nameof(SelectedSegments),
			typeof(List<int>),
			typeof(SegmentBase),
			new PropertyMetadata(new List<int>(), VisualChanged));

		public List<Tuple<int, Brush, Color>> SegmentsBrush
		{
			get => (List<Tuple<int, Brush, Color>>)GetValue(SegmentsBrushProperty);
			set => SetValue(SegmentsBrushProperty, value);
		}

		public static DependencyProperty SegmentsBrushProperty = DependencyProperty.Register(
			nameof(SegmentsBrush),
			typeof(List<Tuple<int, Brush, Color>>),
			typeof(SegmentBase),
			new PropertyMetadata(new List<Tuple<int, Brush, Color>>(), VisualChanged));

		public double VerticalSegmentDivider
		{
			get => (double)GetValue(VerticalSegmentDividerProperty);
			set => SetValue(VerticalSegmentDividerProperty, value);
		}

		public static DependencyProperty VerticalSegmentDividerProperty = DependencyProperty.Register(
			nameof(VerticalSegmentDivider),
			typeof(double),
			typeof(SegmentBase),
			new PropertyMetadata(5.0, VisualChanged));

		public double HorizontalSegmentDivider
		{
			get => (double)GetValue(HorizontalSegmentDividerProperty);
			set => SetValue(HorizontalSegmentDividerProperty, value);
		}

		public static DependencyProperty HorizontalSegmentDividerProperty = DependencyProperty.Register(
			nameof(HorizontalSegmentDivider),
			typeof(double),
			typeof(SegmentBase),
			new PropertyMetadata(9.0, VisualChanged));

		private static void VisualChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			SegmentBase segments = (SegmentBase)sender;
			segments.PropertyChanged(sender, e);
		}
	}
}
