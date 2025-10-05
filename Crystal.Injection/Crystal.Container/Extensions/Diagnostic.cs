using System.Diagnostics;
using System.Reflection;
using Crystal.Extension;
using Crystal.Factories;

namespace Crystal
{
  /// <summary>
  /// Diagnostic extension implements validating when calling <see cref="ICrystalContainer.RegisterType"/>, 
  /// <see cref="ICrystalContainer.Resolve"/>, and <see cref="ICrystalContainer.BuildUp"/> methods. When executed 
  /// these methods provide extra layer of verification and validation as well 
  /// as more detailed reporting of error conditions.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Crystal uses reflection to gather information about types, members, and parameters. 
  /// It is quite obvious that it takes significant amount of time during execution. So, 
  /// to optimize performance all these verifications where moved to the Diagnostic
  /// extension. It is recommended to include this extension only during 
  /// development cycle and refrain from executing it in production 
  /// environment.
  /// </para>
  /// <para>
  /// This extension can be registered in two ways: by adding an extension or by calling
  /// <c>EnableDiagnostic()</c> extension method on container. 
  /// Adding extension to container will work in any build, where <c>EnableDiagnostic()</c>
  /// will only enable it in DEBUG mode. 
  /// </para>
  /// </remarks>
  /// <example>
  /// <code>
  ///     var container = new CrystalContainer();
  /// #if DEBUG
  ///     container.AddExtension(new Diagnostic());
  /// #endif
  /// </code>
  /// <code>
  /// var container = new CrystalContainer();
  /// container.EnableDiagnostic();
  /// </code>
  /// </example>
  public class Diagnostic : CrystalContainerExtension
  {
    protected override void Initialize()
    {
      ((CrystalContainer)Container).SetDefaultPolicies = CrystalContainer.SetDiagnosticPolicies;
      ((CrystalContainer)Container).SetDefaultPolicies((CrystalContainer)Container);
      EnumerableResolver.EnumerableMethod = typeof(EnumerableResolver).GetTypeInfo().GetDeclaredMethod(nameof(EnumerableResolver.DiagnosticResolver));
      EnumerableResolver.EnumerableFactory = typeof(EnumerableResolver).GetTypeInfo().GetDeclaredMethod(nameof(EnumerableResolver.DiagnosticResolverFactory));
    }
  }

  public static class DiagnosticExtensions
  {
    /// <summary>
    /// Enables diagnostic validations on the container built in DEBUG mode.
    /// </summary>
    /// <remarks>
    /// <para>This extension method adds <see cref="Diagnostic"/> extension to the 
    /// container and enables extended validation for all container's operations.</para>
    /// <para>This method will only work if the calling code is built with DEBUG
    /// symbol defined. In other word in you building in Debug mode. Conditional 
    /// methods can not return any values, so fluent notation can not be used with 
    /// this method.</para>
    /// </remarks>
    /// <example>
    /// This is how you could call this method to enable diagnostics:
    /// <code>
    /// var container = new CrystalContainer();
    /// container.EnableDebugDiagnostic();
    /// ...
    /// </code>
    /// </example>
    /// <param name="container">The Crystal Container instance</param>
    [Conditional("DEBUG")]
    public static void EnableDebugDiagnostic(this CrystalContainer container)
    {
      if (null == container) throw new ArgumentNullException(nameof(container));
      container.AddExtension(new Diagnostic());
    }

    /// <summary>
    /// Enables diagnostic validations on the container.
    /// </summary>
    /// <remarks>
    /// <para>This extension method adds <see cref="Diagnostic"/> extension to the 
    /// container and enables extended validation for all container's operations.</para>
    /// <para>This method works regardless of the build mode. In other word, it will 
    /// always enable validation. This method could be used with fluent notation.</para>
    /// </remarks>
    /// <example>
    /// This is how you could call this method to enable diagnostics:
    /// <code>
    /// var container = new CrystalContainer().EnableDebugDiagnostic();
    /// ...
    /// </code>
    /// </example>
    /// <param name="container">The Crystal Container instance</param>
    /// <returns></returns>
    public static CrystalContainer EnableDiagnostic(this CrystalContainer container)
    {
      if (null == container) throw new ArgumentNullException(nameof(container));
      container.AddExtension(new Diagnostic());
      return container;
    }
  }
}
