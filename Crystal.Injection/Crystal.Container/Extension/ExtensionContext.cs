using Crystal.Builder;
using Crystal.Events;
using Crystal.Injection;
using Crystal.Lifetime;
using Crystal.Policy;
using Crystal.Processors;
using Crystal.Storage;
using Crystal.Strategies;

namespace Crystal.Extension
{
  /// <summary>
  /// The <see cref="ExtensionContext"/> class provides the means for extension objects
  /// to manipulate the internal state of the <see cref="ICrystalContainer"/>.
  /// </summary>
  public abstract class ExtensionContext
  {
    #region Container

    /// <summary>
    /// The container that this context is associated with.
    /// </summary>
    /// <value>The <see cref="ICrystalContainer"/> object.</value>
    public abstract ICrystalContainer Container { get; }

    /// <summary>
    /// The <see cref="ILifetimeContainer"/> that this container uses.
    /// </summary>
    /// <value>The <see cref="ILifetimeContainer"/> is used to manage <see cref="IDisposable"/> objects that the container is managing.</value>
    public abstract ILifetimeContainer Lifetime { get; }

    #endregion

    #region Strategies

    /// <summary>
    /// The strategies this container uses.
    /// </summary>
    /// <value>The <see cref="IStagedStrategyChain{TStrategyType,TStageEnum}"/> that the container uses to build objects.</value>
    public abstract IStagedStrategyChain<BuilderStrategy, CrystalBuildStage> Strategies { get; }

    /// <summary>
    /// The strategies this container uses to construct build plans.
    /// </summary>
    /// <value>The <see cref="IStagedStrategyChain{TStrategyType,TStageEnum}"/> that this container uses when creating
    /// build plans.</value>
    public abstract IStagedStrategyChain<MemberProcessor, BuilderStage> BuildPlanStrategies { get; }

    #endregion

    #region Policy Lists

    /// <summary>
    /// The policies this container uses.
    /// </summary>
    /// <remarks>The <see cref="IPolicyList"/> the that container uses to build objects.</remarks>
    public abstract IPolicyList Policies { get; }

    #endregion

    #region Events

    /// <summary>
    /// This event is raised when the 
    /// <see cref="ICrystalContainer.RegisterType(Type,Type,string,LifetimeManager, InjectionMember[])"/> 
    /// method, or one of its overloads, is called.
    /// </summary>
    public abstract event EventHandler<RegisterEventArgs> Registering;

    /// <summary>
    /// This event is raised when the <see cref="ICrystalContainer.RegisterInstance(Type,string,object,LifetimeManager)"/> method,
    /// or one of its overloads, is called.
    /// </summary>
    public abstract event EventHandler<RegisterInstanceEventArgs> RegisteringInstance;

    /// <summary>
    /// This event is raised when the <see cref="ICrystalContainer.CreateChildContainer"/> method is called, providing 
    /// the newly created child container to extensions to act on as they see fit.
    /// </summary>
    public abstract event EventHandler<ChildContainerCreatedEventArgs> ChildContainerCreated;

    #endregion
  }
}
