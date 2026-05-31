using System;

namespace CrystalMonitor.Hardware.Motherboard.Lpc;

internal interface IGigabyteController : IDisposable {
  bool Enable(bool enabled);

  void Restore();
}
