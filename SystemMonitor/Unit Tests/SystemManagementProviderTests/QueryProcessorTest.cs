using System.Management;
using SystemManagementProvider.Constants;
using SystemManagementProvider.Queries;

namespace SystemManagementProviderTests;

[TestClass]
public sealed class QueryProcessorTest {
  ManagementObjectSearcher? _searcher;

  [ClassInitialize]
  public static void ClassInitialize(TestContext context) {
  }

  [TestInitialize]
  public void TestInitialize() {
    _searcher = new ManagementObjectSearcher(Win32_Processor.QueryString);
    if (_searcher == null) {
      throw new InvalidDataException();
    }
  }

  [TestCleanup]
  public void Cleanup() {
    _searcher?.Dispose();
  }

  //[TestMethod]
  //public void TestAddressWidth() {
  //  QueryProcessors queryProcessors = new();
  //  var result_ = queryProcessors.Query(Win32_Processor.AddressWidthKey);
  //  var expect1_ = (Win32_Processor.AddressWidthKey, "64");
  //  var expect2_ = (Win32_Processor.AddressWidthKey, "32");
  //  Assert.IsTrue(result_.Equals(expect1_) || result_.Equals(expect2_));
  //}

  [TestMethod]
  public void TestQueryProcessorInfo() {
    QueryProcessors queryProcessors = new();
    var result_ = queryProcessors.GetInfo();
    Assert.IsNotEmpty(result_);
  }
}
