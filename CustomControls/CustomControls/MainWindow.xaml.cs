using System.ComponentModel;
using System.Windows;

namespace CustomControls; 
public partial class MainWindow : Window, INotifyPropertyChanged {
  public MainWindow() {
    InitializeComponent();
    DataContext = this;
    MyValue = 2.67;
    MyMin = -50;
    MyMax = 50;
    MyUnit = Unit.Percent;
  }

  private Unit _myUnit = Unit.Percent;
  public Unit MyUnit {
    get => _myUnit;
    set {
      if (_myUnit != value) {
        _myUnit = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MyUnit)));
      }
    }
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