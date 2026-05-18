using SystemManagementProvider.Constants;

namespace SystemManagementProvider.Interfaces;  
public interface ISMProvider {
  public ISMQuery GetQueryProvider(SMCategories category);
}
