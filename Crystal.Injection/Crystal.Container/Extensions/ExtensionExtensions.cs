using Crystal.Extension;

namespace Crystal
{
  /// <summary>
  /// Extension class that adds a set of convenience overloads to the
  /// <see cref="ICrystalContainer"/> interface.
  /// </summary>
  public static class ExtensionExtensions
  {
    #region Extension management and configuration

    /// <summary>
    /// Add an extension to the container.
    /// </summary>
    /// <param name="extension"><see cref="CrystalContainerExtension"/> to add.</param>
    /// <returns>The <see cref="ICrystalContainer"/> object that this method was called on (this in C#, Me in Visual Basic).</returns>
    public static ICrystalContainer AddExtension(this ICrystalContainer container, ICrystalContainerExtensionConfigurator extension)
    {
      return ((CrystalContainer)container ?? throw new ArgumentNullException(nameof(container))).AddExtension(extension);
    }

    /// <summary>
    /// Resolve access to a configuration interface exposed by an extension.
    /// </summary>
    /// <remarks>Extensions can expose configuration interfaces as well as adding
    /// strategies and policies to the container. This method walks the list of
    /// added extensions and returns the first one that implements the requested type.
    /// </remarks>
    /// <param name="configurationInterface"><see cref="Type"/> of configuration interface required.</param>
    /// <returns>The requested extension's configuration interface, or null if not found.</returns>
    public static object Configure(this ICrystalContainer container, Type configurationInterface)
    {
      return ((CrystalContainer)container ?? throw new ArgumentNullException(nameof(container))).Configure(configurationInterface);
    }

    /// <summary>
    /// Creates a new extension object and adds it to the container.
    /// </summary>
    /// <typeparam name="TExtension">Type of <see cref="CrystalContainerExtension"/> to add. The extension type
    /// will be resolved from within the supplied <paramref name="container"/>.</typeparam>
    /// <param name="container">Container to add the extension to.</param>
    /// <returns>The <see cref="ICrystalContainer"/> object that this method was called on (this in C#, Me in Visual Basic).</returns>
    public static ICrystalContainer AddNewExtension<TExtension>(this ICrystalContainer container) where TExtension : CrystalContainerExtension
    {
      TExtension newExtension = (container ?? throw new ArgumentNullException(nameof(container))).Resolve<TExtension>();
      return container.AddExtension(newExtension);
    }

    /// <summary>
    /// Resolve access to a configuration interface exposed by an extension.
    /// </summary>
    /// <remarks>Extensions can expose configuration interfaces as well as adding
    /// strategies and policies to the container. This method walks the list of
    /// added extensions and returns the first one that implements the requested type.
    /// </remarks>
    /// <typeparam name="TConfigurator">The configuration interface required.</typeparam>
    /// <param name="container">Container to configure.</param>
    /// <returns>The requested extension's configuration interface, or null if not found.</returns>
    public static TConfigurator Configure<TConfigurator>(this ICrystalContainer container) where TConfigurator : ICrystalContainerExtensionConfigurator
    {
      return (TConfigurator)(container ?? throw new ArgumentNullException(nameof(container))).Configure(typeof(TConfigurator));
    }

    #endregion
  }
}
