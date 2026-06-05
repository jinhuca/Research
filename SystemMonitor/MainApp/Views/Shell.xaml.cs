using System.Windows;

namespace MainApp;

public partial class Shell : Window {
  public Shell() {
    InitializeComponent();
    // Hook onto StateChanged to detect native snapping or external maximize actions
    StateChanged += MainWindow_StateChanged;
  }

  // Minimize Event Trigger
  private void MinimizeButton_Click(object sender, RoutedEventArgs e) {
    WindowState = WindowState.Minimized;
  }

  // Maximize and Restore Event Trigger
  private void MaximizeButton_Click(object sender, RoutedEventArgs e) {
    ToggleMaximize();
  }

  // Close Event Trigger
  private void CloseButton_Click(object sender, RoutedEventArgs e) {
    Close();
  }

  // Shared method to switch the Window State
  private void ToggleMaximize() {
    if (WindowState == WindowState.Maximized) {
      WindowState = WindowState.Normal;
    }
    else {
      WindowState = WindowState.Maximized;
    }
  }

  // Ensure the visual glyph button accurately reflects the OS state change
  private void MainWindow_StateChanged(object sender, EventArgs e) {
    switch (WindowState) {
      case WindowState.Maximized:
        TxtMaximize.Text = "\uE923"; // "Restore" glyph icon (overlapping rectangles)
        BtnMaximize.ToolTip = "Restore Down";
        break;
      case WindowState.Normal:
        TxtMaximize.Text = "\uE922"; // "Maximize" glyph icon (single square box)
        BtnMaximize.ToolTip = "Maximize";
        break;
    }
  }
}