using System.Collections.Generic;

namespace CrystalMonitor.Hardware.Storage;

public interface ISmart {
  /// <summary>
  /// Gets all available smart attributes.
  /// </summary>
  IReadOnlyList<SmartAttribute> Attributes { get; }
}
