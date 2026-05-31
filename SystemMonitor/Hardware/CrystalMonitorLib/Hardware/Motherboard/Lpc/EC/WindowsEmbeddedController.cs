using System.Collections.Generic;

namespace CrystalMonitor.Hardware.Motherboard.Lpc.EC;

public class WindowsEmbeddedController : EmbeddedController {
  public WindowsEmbeddedController(IEnumerable<EmbeddedControllerSource> sources, ISettings settings) : base(sources, settings) { }

  protected override IEmbeddedControllerIO AcquireIOInterface() {
    return new WindowsEmbeddedControllerIO();
  }
}