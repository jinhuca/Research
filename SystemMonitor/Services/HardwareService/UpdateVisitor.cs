using CrystalMonitor.Hardware;

namespace HardwareService;

public sealed class UpdateVisitor : IVisitor {
  public void VisitComputer(IComputer computer) => computer.Traverse(this);
  public void VisitHardware(IHardware hardware) {
    hardware.Update();
    foreach (var sub in hardware.SubHardware) sub.Accept(this);
  }
  public void VisitSensor(ISensor sensor) { }
  public void VisitParameter(IParameter parameter) { }
}

