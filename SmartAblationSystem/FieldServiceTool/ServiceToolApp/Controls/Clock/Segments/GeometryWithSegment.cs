using System.Windows.Media;

namespace ServiceToolApp.Controls.Clock.Segments
{
	public class GeometryWithSegment
	{
		public PathGeometry Geometry { get; set; }
		public Segments SegmentNumber { get; set; }
		public bool IsSelected { get; set; }

		public GeometryWithSegment(PathGeometry geometry, Segments segment, bool isSelected = false)
		{
			Geometry = geometry;
			SegmentNumber = segment;
			IsSelected = isSelected;
		}
	}
}
