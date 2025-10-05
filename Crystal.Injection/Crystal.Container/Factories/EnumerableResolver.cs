using System.Reflection;
using Crystal.Builder;
using Crystal.Policy;
using Crystal.Resolution;

namespace Crystal.Factories
{
  public class EnumerableResolver
  {
    #region Fields

    internal static MethodInfo EnumerableMethod = typeof(EnumerableResolver)
      .GetTypeInfo()
      .GetDeclaredMethod(nameof(EnumerableResolver.Resolver));

    internal static MethodInfo EnumerableFactory = typeof(EnumerableResolver)
      .GetTypeInfo()
      .GetDeclaredMethod(nameof(EnumerableResolver.ResolverFactory));

    #endregion

    #region ResolveDelegateFactory

    public static ResolveDelegateFactory Factory = (ref BuilderContext context) =>
    {
      var typeArgument = context.Type.GenericTypeArguments.First();
      if (typeArgument.IsGenericType)
      {
        return ((EnumerableFactoryDelegate)EnumerableFactory
          .MakeGenericMethod(typeArgument)
          .CreateDelegate(typeof(EnumerableFactoryDelegate)))();
      }

      return (ResolveDelegate<BuilderContext>)EnumerableMethod
        .MakeGenericMethod(typeArgument)
        .CreateDelegate(typeof(ResolveDelegate<BuilderContext>));
    };

    #endregion

    #region Implementation

    private static object Resolver<TElement>(ref BuilderContext context)
    {
      return ((CrystalContainer)context.Container).ResolveEnumerable<TElement>(context.Resolve, context.Name);
    }

    private static ResolveDelegate<BuilderContext> ResolverFactory<TElement>()
    {
      Type type = typeof(TElement).GetGenericTypeDefinition();
      return (ref BuilderContext c) => ((CrystalContainer)c.Container).ResolveEnumerable<TElement>(c.Resolve, type, c.Name);
    }


    internal static object DiagnosticResolver<TElement>(ref BuilderContext context)
    {
      return ((CrystalContainer)context.Container).ResolveEnumerable<TElement>(context.Resolve, context.Name).ToArray();
    }

    internal static ResolveDelegate<BuilderContext> DiagnosticResolverFactory<TElement>()
    {
      Type type = typeof(TElement).GetGenericTypeDefinition();
      return (ref BuilderContext c) => ((CrystalContainer)c.Container).ResolveEnumerable<TElement>(c.Resolve, type, c.Name).ToArray();
    }

    #endregion

    #region Nested Types

    private delegate ResolveDelegate<BuilderContext> EnumerableFactoryDelegate();

    #endregion
  }
}
