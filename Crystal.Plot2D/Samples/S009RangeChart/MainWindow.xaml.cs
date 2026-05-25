using Crystal.Plot2D;
using Crystal.Plot2D.DataSources.MultiDimensional;
using Crystal.Plot2D.DataSources.OneDimensional;
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

namespace S009RangeChart {
  public partial class MainWindow : Window {
    public MainWindow() {
      InitializeComponent();
      Loaded += MainWindow_Loaded;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e) {
      // Define your data points
      var x = new double[] { 1, 2, 3, 4, 5 };
      var yMin = new double[] { 10, 12, 11, 14, 13 };
      var yMax = new double[] { 20, 22, 21, 24, 23 };

      // Create data sources
      var xSource = x.AsEnumerable();
      var yMinSource = yMin.AsEnumerable();
      var yMaxSource = yMax.AsEnumerable();

      // Composite the sources into a multi-value source
      var dataSource = new CompositeDataSource(xSource, yMinSource, yMaxSource);

      // Add to the plotter (implementation depends on your chosen visual style)
      // For simple boundary lines:
      plotter.AddLineGraph(new CompositeDataSource(xSource, yMinSource, yMaxSource), Colors.Blue, 1, "Min");
      plotter.AddLineGraph(new CompositeDataSource(xSource, yMinSource, yMaxSource), Colors.Red, 1, "Max");

    }
  }
}