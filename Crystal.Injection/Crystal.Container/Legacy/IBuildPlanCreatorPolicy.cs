using Crystal.Builder;

namespace Crystal.Policy
{
  [Obsolete("This interface has been replaced with Crystal.Policy.ResolveDelegateFactory delegate", true)]
  public interface IBuildPlanCreatorPolicy
  {
    /// <summary>
    /// Create a build plan using the given context and build key.
    /// </summary>
    /// <param name="context">Current build context.</param>
    /// <param name="type"></param>
    /// <param name="name"></param>
    /// <returns>The build plan.</returns>
    IBuildPlanPolicy CreatePlan(ref BuilderContext context, Type type, string name);
  }
}
