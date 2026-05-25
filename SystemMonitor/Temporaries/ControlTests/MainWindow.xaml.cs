using System.Windows;
using System.Windows.Threading;

namespace ControlTests;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window {
  public MainWindow() {
    InitializeComponent();

    Loaded += (_, __) => {
      // start data generation after window loaded to ensure plotter is ready

      // supply real-time data every second
      var timer = new DispatcherTimer {
        Interval = TimeSpan.FromSeconds(1)
      };

      timer.Tick += (_, __) => {
        var y = new Random().NextDouble() * 60;
        plotter?.AddPoint(y);
      };
      timer.Start();
    };
  }
}