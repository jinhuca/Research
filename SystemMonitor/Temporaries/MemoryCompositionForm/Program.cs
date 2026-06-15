using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using LibreHardwareMonitor.Hardware;
using Vortice.DXGI;

/// <summary>
/// Mimics Task Manager's "Memory composition" stacked bar for a GPU:
/// shows Dedicated (used/total) and Shared (used/total) memory as a
/// segmented horizontal bar with a tooltip-style label.
/// </summary>
public class MemoryCompositionForm : Form {
  private readonly Computer _computer;
  private readonly System.Windows.Forms.Timer _timer;
  private IHardware _gpu;

  private float _dedicatedUsed, _dedicatedTotal;
  private float _sharedUsed, _sharedTotal;
  private string _adapterName;

  public MemoryCompositionForm() {
    Text = "GPU Memory Composition";
    Width = 500;
    Height = 160;
    DoubleBuffered = true;

    _computer = new Computer { IsGpuEnabled = true };
    _computer.Open();

    _gpu = _computer.Hardware.FirstOrDefault(h =>
        h.HardwareType == HardwareType.GpuNvidia ||
        h.HardwareType == HardwareType.GpuAmd ||
        h.HardwareType == HardwareType.GpuIntel);

    _timer = new System.Windows.Forms.Timer { Interval = 1000 };
    _timer.Tick += (s, e) => { RefreshValues(); Invalidate(); };
    _timer.Start();

    FetchDxgiTotals();
    RefreshValues();
  }

  /// <summary>
  /// Gets static Dedicated/Shared memory TOTAL capacity via DXGI - this is what
  /// Task Manager itself uses, and is far more reliable than LHM's SmallData totals
  /// (especially for "D3D Shared Memory Total" which LHM often reports as 0).
  /// Matches the GPU by name to the LHM hardware entry where possible.
  /// </summary>
  private void FetchDxgiTotals() {
    try {
      using var factory = DXGI.CreateDXGIFactory1<IDXGIFactory1>();

      for (uint i = 0; factory.EnumAdapters1(i, out var adapter).Success; i++) {
        var desc = adapter.Description1;
        adapter.Dispose();

        // Skip the "Microsoft Basic Render Driver" software adapter
        if (desc.Flags.HasFlag(AdapterFlags.Software))
          continue;

        bool nameMatches = _gpu != null &&
            desc.Description.IndexOf(_gpu.Name, StringComparison.OrdinalIgnoreCase) >= 0;

        // Prefer a name match; otherwise fall back to the first hardware adapter found
        if (nameMatches || (_dedicatedTotal == 0 && _sharedTotal == 0)) {
          _dedicatedTotal = desc.DedicatedVideoMemory / (1024f * 1024f);
          _sharedTotal = desc.SharedSystemMemory / (1024f * 1024f);
          _adapterName = desc.Description;

          if (nameMatches)
            break; // good match found, stop looking
        }
      }
    }
    catch (Exception ex) {
      // DXGI may be unavailable in some sandboxed/RDP contexts - degrade gracefully
      Console.WriteLine($"DXGI query failed: {ex.Message}");
    }
  }

  private void RefreshValues() {
    if (_gpu == null) return;
    _gpu.Update();

    float Get(string name) =>
        _gpu.Sensors.FirstOrDefault(s =>
            s.SensorType == SensorType.SmallData &&
            s.Name.Equals(name, StringComparison.OrdinalIgnoreCase))?.Value ?? 0f;

    _dedicatedUsed = Get("D3D Dedicated Memory Used");
    _sharedUsed = Get("D3D Shared Memory Used");

    // Totals come from DXGI (FetchDxgiTotals), not LHM - LHM's "D3D Shared
    // Memory Total" is frequently 0 on many driver versions.
  }

  protected override void OnPaint(PaintEventArgs e) {
    base.OnPaint(e);
    var g = e.Graphics;
    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

    if (_gpu == null && _adapterName == null) {
      g.DrawString("No GPU detected.", Font, Brushes.Black, 10, 10);
      return;
    }

    const int barX = 10;
    const int barY = 50;
    const int barHeight = 24;
    int barWidth = ClientSize.Width - 20;

    float totalCapacity = _dedicatedTotal + _sharedTotal;
    if (totalCapacity <= 0) totalCapacity = 1; // avoid div-by-zero

    // Proportional widths for each segment
    int dedicatedWidth = (int)(barWidth * (_dedicatedTotal / totalCapacity));
    int sharedWidth = barWidth - dedicatedWidth;

    int dedicatedUsedWidth = (int)(dedicatedWidth * SafeRatio(_dedicatedUsed, _dedicatedTotal));
    int sharedUsedWidth = (int)(sharedWidth * SafeRatio(_sharedUsed, _sharedTotal));

    // --- Title ---
    using (var titleFont = new Font(Font.FontFamily, 10, FontStyle.Bold))
      g.DrawString($"{_gpu?.Name ?? _adapterName} - Memory composition", titleFont, Brushes.Black, barX, 10);

    // --- Outer bar background (capacity outline) ---
    var dedicatedRect = new Rectangle(barX, barY, dedicatedWidth, barHeight);
    var sharedRect = new Rectangle(barX + dedicatedWidth, barY, sharedWidth, barHeight);

    g.FillRectangle(new SolidBrush(Color.FromArgb(230, 230, 230)), dedicatedRect);
    g.FillRectangle(new SolidBrush(Color.FromArgb(245, 245, 245)), sharedRect);

    // --- Used portions ---
    g.FillRectangle(new SolidBrush(Color.FromArgb(0, 120, 215)),   // blue = dedicated used
        new Rectangle(barX, barY, dedicatedUsedWidth, barHeight));

    g.FillRectangle(new SolidBrush(Color.FromArgb(0, 200, 160)),   // teal = shared used
        new Rectangle(barX + dedicatedWidth, barY, sharedUsedWidth, barHeight));

    // --- Divider line between dedicated and shared segments ---
    using (var pen = new Pen(Color.Gray, 1))
      g.DrawLine(pen, barX + dedicatedWidth, barY - 2, barX + dedicatedWidth, barY + barHeight + 2);

    // --- Outline ---
    g.DrawRectangle(Pens.Gray, barX, barY, barWidth - 1, barHeight - 1);

    // --- Labels below bar ---
    int labelY = barY + barHeight + 10;

    g.DrawString("■ Dedicated GPU memory", Font, new SolidBrush(Color.FromArgb(0, 120, 215)), barX, labelY);
    g.DrawString($"{FormatMb(_dedicatedUsed)} / {FormatMb(_dedicatedTotal)}", Font, Brushes.Black, barX + 220, labelY);

    g.DrawString("■ Shared GPU memory", Font, new SolidBrush(Color.FromArgb(0, 200, 160)), barX, labelY + 20);
    g.DrawString($"{FormatMb(_sharedUsed)} / {FormatMb(_sharedTotal)}", Font, Brushes.Black, barX + 220, labelY + 20);
  }

  private static float SafeRatio(float used, float total) =>
      total <= 0 ? 0 : Math.Min(used / total, 1f);

  private static string FormatMb(float mb) =>
      mb >= 1024 ? $"{mb / 1024f:F1} GB" : $"{mb:F0} MB";

  protected override void OnFormClosed(FormClosedEventArgs e) {
    _timer.Stop();
    _computer.Close();
    base.OnFormClosed(e);
  }

  [STAThread]
  public static void Main() {
    Application.SetHighDpiMode(HighDpiMode.SystemAware);
    Application.EnableVisualStyles();
    Application.Run(new MemoryCompositionForm());
  }
}