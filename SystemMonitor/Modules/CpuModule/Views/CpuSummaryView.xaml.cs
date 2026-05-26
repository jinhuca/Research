using System.Windows.Controls;

namespace CpuModule.Views;

public partial class CpuSummaryView : UserControl {
  public CpuSummaryView() {
    InitializeComponent();
    // Ensure this control stretches to fill its parent when loaded.
    Loaded += (s, e) => {
      HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
      VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
      if (Parent is System.Windows.FrameworkElement) {
        Width = double.NaN; // Auto
        Height = double.NaN; // Auto
      }
    };
  }
}
