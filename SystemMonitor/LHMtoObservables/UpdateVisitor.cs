namespace LHMtoObservables;

using CrystalMonitor.Hardware;
// ── Visitor (required by CrystalHardwareMonitor's update model) ──────────────────────────────────
public sealed class UpdateVisitor : IVisitor {
  public void VisitComputer(IComputer computer) => computer.Traverse(this);
  public void VisitHardware(IHardware hardware) {
    hardware.Update();
    foreach (var sub in hardware.SubHardware) sub.Accept(this);
  }
  public void VisitSensor(ISensor sensor) { }
  public void VisitParameter(IParameter parameter) { }
}
