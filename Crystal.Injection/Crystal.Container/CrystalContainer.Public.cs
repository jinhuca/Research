using System.Reflection;
using Crystal.Builder;
using Crystal.Extension;
using Crystal.Factories;
using Crystal.Lifetime;
using Crystal.Policy;
using Crystal.Registration;
using Crystal.Resolution;
using Crystal.Storage;
using Crystal.Strategies;

namespace Crystal
{
  public partial class CrystalContainer
  {
    #region Constructors

    /// <summary>
    /// Create a default <see cref="CrystalContainer"/>.
    /// </summary>
    public CrystalContainer()
    {
      _root = this;

      // WithLifetime
      LifetimeContainer = new LifetimeContainer(this);
      _typeLifetimeManager = TransientLifetimeManager.Instance;
      _factoryLifetimeManager = TransientLifetimeManager.Instance;
      _instanceLifetimeManager = new ContainerControlledLifetimeManager();

      // Registrations
      _registrations = new Registrations(ContainerInitialCapacity);

      // Context
      _context = new ContainerContext(this);

      // Methods
      _get = Get;
      _getGenericRegistration = GetOrAddGeneric;
      _isExplicitlyRegistered = IsExplicitlyRegisteredLocally;
      IsTypeExplicitlyRegistered = IsTypeTypeExplicitlyRegisteredLocally;

      GetRegistration = GetOrAdd;
      Register = AddOrUpdate;
      GetPolicy = Get;
      SetPolicy = Set;
      ClearPolicy = Clear;

      // Build Strategies
      _strategies = new StagedStrategyChain<BuilderStrategy, CrystalBuildStage>
            {
                {   // Array
                    new ArrayResolveStrategy(
                        typeof(CrystalContainer).GetTypeInfo().GetDeclaredMethod(nameof(ResolveArray)),
                        typeof(CrystalContainer).GetTypeInfo().GetDeclaredMethod(nameof(ResolveGenericArray))),
                    CrystalBuildStage.Enumerable
                },
                {new BuildKeyMappingStrategy(), CrystalBuildStage.TypeMapping},   // Mapping
                {new LifetimeStrategy(), CrystalBuildStage.Lifetime},             // WithLifetime
                {new BuildPlanStrategy(), CrystalBuildStage.Creation}             // Build
            };

      // Update on change
      _strategies.Invalidated += OnStrategiesChanged;
      _strategiesChain = _strategies.ToArray();


      // Default Policies and Strategies
      SetDefaultPolicies(this);

      Set(typeof(Func<>), All, typeof(LifetimeManager), new PerResolveLifetimeManager());
      Set(typeof(Func<>), All, typeof(ResolveDelegateFactory), (ResolveDelegateFactory)DeferredFuncResolverFactory.DeferredResolveDelegateFactory);
      Set(typeof(Lazy<>), All, typeof(ResolveDelegateFactory), (ResolveDelegateFactory)GenericLazyResolverFactory.GetResolver);
      Set(typeof(IEnumerable<>), All, typeof(ResolveDelegateFactory), EnumerableResolver.Factory);

      // Register this instance
      ((ICrystalContainer)this).RegisterInstance(typeof(ICrystalContainer), null, this, _containerManager);
    }

    #endregion

    /// <inheritdoc />
    public IEnumerable<IContainerRegistration> Registrations
    {
      get
      {
        var set = new QuickSet<Type>();
        set.Add(NamedType.GetHashCode(typeof(ICrystalContainer), null), typeof(ICrystalContainer));

        // ICrystalContainer
        yield return new ContainerRegistrationStruct
        {
          RegisteredType = typeof(ICrystalContainer),
          MappedToType = typeof(CrystalContainer),
          LifetimeManager = _containerManager
        };

        // Scan containers for explicit registrations
        for (var container = this; null != container; container = container._parent)
        {
          // Skip to parent if no registrations
          if (null == container._registrations) continue;

          // Hold on to registries
          var registrations = container._registrations;

          for (var i = 0; i < registrations.Count; i++)
          {
            var registry = registrations.Entries[i].Value;
            Type type = registrations.Entries[i].Key;

            switch (registry)
            {
              case LinkedRegistry linkedRegistry:
                for (var node = (LinkedNode<string, IPolicySet>)linkedRegistry; null != node; node = node.Next)
                {
                  if (node.Value is ContainerRegistration containerRegistration &&
                      set.Add(NamedType.GetHashCode(type, node.Key), type))
                  {
                    yield return new ContainerRegistrationStruct
                    {
                      RegisteredType = type,
                      Name = node.Key,
                      LifetimeManager = containerRegistration.LifetimeManager,
                      MappedToType = containerRegistration.Type,
                    };
                  }
                }
                break;

              case HashRegistry hashRegistry:
                var count = hashRegistry.Count;
                var nodes = hashRegistry.Entries;
                for (var j = 0; j < count; j++)
                {
                  var name = nodes[j].Key;

                  if (nodes[j].Value is ContainerRegistration containerRegistration &&
                      set.Add(NamedType.GetHashCode(type, name), type))
                  {
                    yield return new ContainerRegistrationStruct
                    {
                      RegisteredType = type,
                      Name = name,
                      LifetimeManager = containerRegistration.LifetimeManager,
                      MappedToType = containerRegistration.Type,
                    };
                  }
                }
                break;

              default:
                yield break;
            }
          }
        }
      }
    }


    #region Extension Management

    /// <summary>
    /// Add an extension to the container.
    /// </summary>
    /// <param name="extension"><see cref="CrystalContainerExtension"/> to add.</param>
    /// <returns>The <see cref="ICrystalContainer"/> object that this method was called on (this in C#, Me in Visual Basic).</returns>
    public ICrystalContainer AddExtension(ICrystalContainerExtensionConfigurator extension)
    {
      lock (LifetimeContainer)
      {
        if (null == _extensions)
          _extensions = new List<ICrystalContainerExtensionConfigurator>();

        _extensions.Add(extension ?? throw new ArgumentNullException(nameof(extension)));
      }
        (extension as CrystalContainerExtension)?.InitializeExtension(_context);

      return this;
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
    public object Configure(Type configurationInterface)
    {
#if NETSTANDARD1_0 || NETCOREAPP1_0
            return _extensions?.FirstOrDefault(ex => configurationInterface.GetTypeInfo()
                                                                           .IsAssignableFrom(ex.GetType()
                                                                           .GetTypeInfo()));
#else
      return _extensions?.FirstOrDefault(ex => configurationInterface.IsAssignableFrom(ex.GetType()));
#endif
    }

    #endregion


    #region IDisposable Implementation

    /// <summary>
    /// Dispose this container instance.
    /// </summary>
    /// <remarks>
    /// Disposing the container also disposes any child containers,
    /// and disposes any instances whose lifetimes are managed
    /// by the container.
    /// </remarks>
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    #endregion
  }
}
