using Crystal.Extension;
using Crystal.Lifetime;

namespace Crystal
{
  public class DefaultLifetime : CrystalContainerExtension
  {
    protected override void Initialize()
    {
    }

    #region Public Members

    public ITypeLifetimeManager TypeDefaultLifetime
    {
      get => (ITypeLifetimeManager)((CrystalContainer)Container).TypeLifetimeManager;
      set => ((CrystalContainer)Container).TypeLifetimeManager = (LifetimeManager)value ?? throw new ArgumentNullException("Type Lifetime Manager can not be null");
    }

    public IInstanceLifetimeManager InstanceDefaultLifetime
    {
      get => (IInstanceLifetimeManager)((CrystalContainer)Container).InstanceLifetimeManager;
      set => ((CrystalContainer)Container).InstanceLifetimeManager = (LifetimeManager)value ?? throw new ArgumentNullException("Instance Lifetime Manager can not be null");
    }

    public IFactoryLifetimeManager FactoryDefaultLifetime
    {
      get => (IFactoryLifetimeManager)((CrystalContainer)Container).FactoryLifetimeManager;
      set => ((CrystalContainer)Container).FactoryLifetimeManager = (LifetimeManager)value ?? throw new ArgumentNullException("Factory Lifetime Manager can not be null");
    }

    #endregion
  }
}
