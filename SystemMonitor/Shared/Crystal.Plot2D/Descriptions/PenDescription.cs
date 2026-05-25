using Crystal.Plot2D.Charts;
using Crystal.Plot2D.Graphs;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Crystal.Plot2D.Descriptions;

public sealed class PenDescription : StandardDescription {
  /// <summary>
  /// Initializes a new instance of the <see cref="PenDescription"/> class.
  /// </summary>
  public PenDescription() { }

  /// <summary>
  /// Initializes a new instance of the <see cref="PenDescription"/> class.
  /// </summary>
  /// <param name="description">Custom description.</param>
  public PenDescription(string description) : base(description: description) { }

  protected override LegendItem CreateLegendItemCore() {
    return new LineLegendItem(description: this);
  }

  protected override void AttachCore(UIElement graph) {
    base.AttachCore(element: graph);
    if(graph is LineGraph g) {
      // Bind to the LinePen's Brush and Thickness. Use the property names on the types
      // to build the path (e.g. "LinePen.Brush") because nameof(g.LinePen.Brush) yields
      // only the last identifier ("Brush") which tries to resolve a "Brush" property
      // on the LineGraph and causes the binding error.
      SetBinding(dp: StrokeProperty, binding: new Binding(path: nameof(LineGraph.LinePen) + "." + nameof(Pen.Brush)) { Source = g });
      SetBinding(dp: StrokeThicknessProperty, binding: new Binding(path: nameof(LineGraph.LinePen) + "." + nameof(Pen.Thickness)) { Source = g });
    }
  }

  public Brush Stroke {
    get => (Brush)GetValue(dp: StrokeProperty);
    set => SetValue(dp: StrokeProperty, value: value);
  }

  public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(
    name: nameof(Stroke),
    propertyType: typeof(Brush),
    ownerType: typeof(PenDescription),
    typeMetadata: new FrameworkPropertyMetadata(propertyChangedCallback: null));

  public double StrokeThickness {
    get => (double)GetValue(dp: StrokeThicknessProperty);
    set => SetValue(dp: StrokeThicknessProperty, value: value);
  }

  public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register(
    name: nameof(StrokeThickness),
    propertyType: typeof(double),
    ownerType: typeof(PenDescription),
    typeMetadata: new FrameworkPropertyMetadata(defaultValue: 0.0));

}
