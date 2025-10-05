using System.Windows;
using System.Windows.Controls;

namespace ServiceToolApp.Controls.Clock
{
	internal class ArrangedPanel : Panel
	{
		protected override Size ArrangeOverride(Size finalSize)
		{
			double x = 0;
			const double y = 0;
			var w = finalSize.Width / InternalChildren.Count;
			var h = finalSize.Height;

			foreach (UIElement child in InternalChildren)
			{
				child.Arrange(new Rect(new Point(x, y), new Size(w, h)));
				x += w;
			}
			return finalSize;
		}
	}
}
