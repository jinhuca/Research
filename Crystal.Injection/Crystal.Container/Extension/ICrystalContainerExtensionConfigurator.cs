namespace Crystal.Extension
{
  /// <summary>
  /// Base interface for all extension configuration interfaces.
  /// </summary>
  public interface ICrystalContainerExtensionConfigurator
  {
    /// <summary>
    /// Retrieve the container instance that we are currently configuring.
    /// </summary>
    ICrystalContainer Container { get; }
  }
}
