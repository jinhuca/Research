using System.Management;
using SystemManagementProvider;
using SystemManagementProvider.Constants;
using SystemManagementProvider.Queries;

namespace SystemManagementProviderTests; 
[TestClass]
public sealed class QueryProcessorTest {

  [TestMethod]
  public void TestAddressWidth() {
    ManagementObjectSearcher searcher = new(Win32_Processor.Query_String);
    QueryProcessors queryProcessors = new(searcher);
    var result_ = queryProcessors.Query(Win32_Processor.AddressWidthKey);
    var expect1_ = (Win32_Processor.AddressWidthKey, "64");
    var expect2_ = (Win32_Processor.AddressWidthKey, "32");
    Assert.IsTrue(result_.Equals(expect1_) || result_.Equals(expect2_));
  }

  [TestMethod]
  public void TestQueryProcessorInfo() {
    ManagementObjectSearcher searcher = new(Win32_Processor.Query_String);
    QueryProcessors queryProcessors = new(searcher);
    var result_ = queryProcessors.GetInfo();
    Assert.IsNotEmpty(result_);
  }
}
