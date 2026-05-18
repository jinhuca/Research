namespace SystemManagementProvider.Interfaces;

public interface ISMQuery {
  public Dictionary<string, (string, string)> Query(string query);
  public Dictionary<string, Dictionary<string, (string, string)>> QueryMultiple(string query);
}
