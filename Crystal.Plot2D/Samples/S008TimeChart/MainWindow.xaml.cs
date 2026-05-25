using Crystal.Plot2D;
using Crystal.Plot2D.DataSources.OneDimensional;
using Crystal.Plot2D.Graphs;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace S008TimeChart; 

public partial class MainWindow : Window {
  LineGraph graph;

  public MainWindow() {
    InitializeComponent();
    InitializeGraph();
  }

  private void InitializeGraph() {
    LineGraph lineGraph = plotter.AddLineGraph(new[] {
      new Point(0, 0),
      new Point(1, 2),
      new Point(2, 1),
      new Point(3, 3),
      new Point(4, 2)
    }.AsDataSource());
  }
}