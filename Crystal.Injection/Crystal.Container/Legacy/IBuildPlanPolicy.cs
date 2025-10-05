using Crystal.Builder;

namespace Crystal.Policy
{
  [Obsolete("IBuildPlanPolicy has been deprecated, please use ResolveDelegateFactory instead", true)]
  public interface IBuildPlanPolicy
  {
    void BuildUp(ref BuilderContext context);
  }
}
