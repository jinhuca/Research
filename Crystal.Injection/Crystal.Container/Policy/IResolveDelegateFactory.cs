using Crystal.Builder;
using Crystal.Resolution;

namespace Crystal.Policy
{
  public delegate ResolveDelegate<BuilderContext> ResolveDelegateFactory(ref BuilderContext context);

  public interface IResolveDelegateFactory
  {
    ResolveDelegate<BuilderContext> GetResolver(ref BuilderContext context);
  }
}
