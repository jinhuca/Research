using System.Windows.Media;

namespace ServiceToolApp.Controls.Clock.Segments
{
	internal interface ISegment
	{
		/// <summary>
		/// A brush for not selected elements
		/// </summary>
		Brush FillBrush { get; set; }

		/// <summary>
		/// A brush for selected elements
		/// </summary>
		Brush SelectedFillBrush { get; set; }

		/// <summary>
		/// A brush for not selected elements
		/// </summary>
		Color PenColor { get; set; }

		/// <summary>
		/// A pen color for selected elements
		/// </summary>
		Color SelectedPenColor { get; set; }

		/// <summary>
		/// A pen thickness of elements
		/// </summary>
		double PenThickness { get; set; }

		/// <summary>
		/// A value for segments control
		/// </summary>
		string Value { get; set; }

		/// <summary>
		/// A gap  between segments 
		/// </summary>
		double GapWidth { get; set; }

		/// <summary>
		/// Checks whether or not the corners are rounded 
		/// </summary>
		bool RoundedCorners { get; set; }

		/// <summary>
		/// A tilt angle (in degrees)
		/// </summary>
		double TiltAngle { get; set; }

		/// <summary>
		/// Shows/Hides dot for segments control
		/// </summary>
		bool ShowDot { get; set; }

		/// <summary>
		/// Shows/Hides colon for segments control
		/// </summary>
		bool ShowColon { get; set; }

		/// <summary>
		/// On/Off dot for segments control
		/// </summary>
		bool OnDot { get; set; }

		/// <summary>
		/// On/Off colon for segments control
		/// </summary>
		bool OnColon { get; set; }

		/// <summary>
		/// A divider for vertical segments width
		/// </summary>
		double VerticalSegmentDivider { get; set; }

		/// <summary>
		/// A divider for horizontal. segments height
		/// </summary>
		double HorizontalSegmentDivider { get; set; }
	}
}
