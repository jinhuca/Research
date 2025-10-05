using Crystal.Builder;

namespace Crystal.Policy
{
  [Obsolete("IMethodSelectorPolicy has been deprecated, please use ISelectMembers<MethodInfo> instead", true)]
  public interface IMethodSelectorPolicy
  {
    IEnumerable<object> SelectMethods(ref BuilderContext context);
  }
}
