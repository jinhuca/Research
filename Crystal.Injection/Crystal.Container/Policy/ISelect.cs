using System.Reflection;

namespace Crystal.Policy
{
  public interface ISelect<TMemberInfo> where TMemberInfo : MemberInfo
  {
    IEnumerable<object> Select(Type type, IPolicySet registration);
  }
}
