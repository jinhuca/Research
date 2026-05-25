using Crystal.Plot2D;
using Crystal.Plot2D.Axes.Numeric;
using Crystal.Plot2D.DataSources.OneDimensional;
using Crystal.Plot2D.Graphs;
using Crystal.Plot2D.Navigation;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace S007DynamicPointAddSample;

public partial class MainWindow
{
  public MainWindow()
  {
    InitializeComponent();
    
    double xMin = 0;
    double xMax = 60;
    double yMin = 0;
    double yMax = 100;

    plotter.Viewport.Visible = new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
    plotter.Visible = new Rect(xMin, yMin, xMax - xMin, yMax - yMin);

    plotter.AxisGrid.GridPath.Stroke = Brushes.Gray;
    plotter.AxisGrid.IsVisibleChanged += (s, e) => {
        //plotter.AxisGrid.GridPath.Stroke = Brushes.Transparent;
    };

    // adding line chart to plotter
    xLineGraph = plotter.AddLineGraph(_data.AsDataSource());
    xLineGraph.LinePen = new Pen(Brushes.Black, 1);

    for (int i = 0; i < 60; i++) {
      //AddNextPoint();
      Add60Point();
    }

    Loaded += MainWindow_Loaded;
  }

  private DispatcherTimer _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
  private ObservableCollection<Point> _data = new ObservableCollection<Point>();
  private int x = 0;
  private Random rnd = new Random();
  LineGraph xLineGraph;

  private void MainWindow_Loaded(object? sender, RoutedEventArgs e)
  {
    //for (int i = 0; i < 60; i++) {
    //  //AddNextPoint();
    //  Add60Point();
    //}

    // switching off approximate content bound's comparison, as this can cause improper behavior.
    plotter.Viewport.UseApproximateContentBoundsComparison = false;

    plotter.MainHorizontalAxisVisibility = Visibility.Visible;
    plotter.MainVerticalAxisVisibility = Visibility.Visible;
    plotter.NewLegendVisible = false;

    var nav = plotter.Children.OfType<MouseNavigation>().FirstOrDefault();
    if (nav != null) {
      plotter.Children.Remove(nav);
    }

    // adding line chart to plotter
    //var line_ = plotter.AddLineGraph(_data.AsDataSource());
    //line_.LinePen = new Pen(Brushes.White, 1);

    Viewport2D.SetUsesApproximateContentBoundsComparison(xLineGraph, true);

    _timer.Tick += timer_tick;
    _timer.Start();
  }
    
  void timer_tick(object? sender, EventArgs e)
  {
    AddNextPoint();
    _data.RemoveAt(0);
    xLineGraph.LinePen = new Pen(Brushes.Black, 1);
    plotter.FitToView();
  }

  private void AddNextPoint()
  {
    var p = new Point {
      X = x++,
      //Y = 0.1 * Math.Sqrt(x) * Math.Cos(x)
      Y = rnd.Next(0, 51)
    };
    _data.Add(p);
  }

  private void Add60Point() {
    
    var p = new Point {
      X = x++,
      Y = 0
    };
    _data.Add(p);
  }
}