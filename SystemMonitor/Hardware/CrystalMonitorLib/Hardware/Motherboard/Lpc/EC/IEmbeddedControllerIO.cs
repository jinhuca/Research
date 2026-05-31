using System;

namespace CrystalMonitor.Hardware.Motherboard.Lpc.EC;

public interface IEmbeddedControllerIO : IDisposable {
  void Read(ushort[] registers, byte[] data);
}