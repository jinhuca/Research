using SystemManagementProvider.Constants;

namespace SystemManagementProvider.Interfaces; 
public interface ISMQuery {
  public Dictionary<string, (string, string)> Query(string query);
}
