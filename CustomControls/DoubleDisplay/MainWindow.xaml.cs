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

namespace DoubleDisplay; 

public partial class MainWindow : Window, INotifyPropertyChanged {

  private double _mMyDoubleValue = 1234.56;

  public event PropertyChangedEventHandler? PropertyChanged;

  public double MyDoubleValue
  {
    get => _mMyDoubleValue;
    set
    {
      _mMyDoubleValue = value;
      OnPropertyChanged();
    }
  }

  private void OnPropertyChanged() {
    // Attempt to infer the calling property's name from the call stack (e.g. "set_MyDoubleValue")
    string? propertyName = null;
    try {
      var stack = new System.Diagnostics.StackTrace();
      // Get the immediate caller frame
      var frame = stack.GetFrame(1);
      var method = frame?.GetMethod();
      if (method != null) {
        var name = method.Name;
        // If caller is a property setter/getter, strip the "set_"/"get_" prefix
        if (name.StartsWith("set_", StringComparison.Ordinal) || name.StartsWith("get_", StringComparison.Ordinal)) {
          propertyName = name.Substring(4);
        }
        else {
          propertyName = name;
        }
      }
    }
    catch {
      // If anything goes wrong while inspecting the stack, fall back to empty (meaning "all properties" for some listeners)
      propertyName = string.Empty;
    }

    if (string.IsNullOrEmpty(propertyName)) {
      propertyName = string.Empty;
    }

    // Raise the PropertyChanged event if there are subscribers
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }

  public MainWindow() {
    InitializeComponent();
    DataContext = this;
  }
}