using System.ComponentModel;
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

namespace CustomControls {
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window, INotifyPropertyChanged {
    public MainWindow() {
      InitializeComponent();
      this.DataContext = this;
      MyValue = -1.6;
      MyMin = -10;
      MyMax = 0;
    }

    private double _myValue;
    public double MyValue {
      get => _myValue;
      set {
        if (_myValue != value) {
          _myValue = value;
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MyValue)));
        }
      }
    }

    private double _myMin = 0;
    public double MyMin {
      get => _myMin;
      set {
        if (_myMin != value) {
          _myMin = value;
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MyMin)));
        }
      }
    }

    private double _myMax = 100;
    public double MyMax {
      get => _myMax;
      set {
        if (_myMax != value) {
          _myMax = value;
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MyMax)));
        }
      }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
  }
}