using System.Reflection;
using Crystal.Builder;

namespace Crystal.Policy
{
  [Obsolete("IPropertySelectorPolicy has been deprecated, please use IPropertySelectorPolicy instead", true)]
  public interface IPropertySelectorPolicy : ISelect<PropertyInfo>
  {
    IEnumerable<object> SelectProperties(ref BuilderContext context);
  }
}
