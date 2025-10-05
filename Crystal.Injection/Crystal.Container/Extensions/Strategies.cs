using Crystal.Extension;
using Crystal.Policy;

namespace Crystal
{
  /// <summary>
  /// This extension forces the container to only use activated strategies during resolution
  /// </summary>
  /// <remarks>
  /// This extension forces compatibility with systems without support for runtime compilers. 
  /// One of such systems is iOS.
  /// </remarks>
  public class ForceActivation : CrystalContainerExtension
  {
    protected override void Initialize()
    {
      var crystal = (CrystalContainer)Container;
      crystal._buildStrategy = crystal.ResolvingFactory;
      crystal.Defaults.Set(typeof(ResolveDelegateFactory), crystal._buildStrategy);
    }
  }

  /// <summary>
  /// This extension forces the container to only use compiled strategies during resolution
  /// </summary>
  public class ForceCompillation : CrystalContainerExtension
  {
    protected override void Initialize()
    {
      var crystal = (CrystalContainer)Container;
      crystal._buildStrategy = crystal.CompilingFactory;
      crystal.Defaults.Set(typeof(ResolveDelegateFactory), crystal._buildStrategy);
    }
  }
}
