using System.Management;
using SystemManagementProvider.Constants;
using SystemManagementProvider.Queries;

namespace SystemManagementProviderTests; 
[TestClass]
public sealed class QueryOSTest {
  ManagementObjectSearcher? _searcher;

  [ClassInitialize]
  public static void ClassInitialize(TestContext context) {
  }

  [TestInitialize]
  public void TestInitialize() {
    _searcher = new ManagementObjectSearcher(Win32_OperatingSystem.QueryString);
    if (_searcher == null) {
      throw new InvalidDataException();
    }
  }

  [TestCleanup]
  public void TestCleanup() {
    _searcher?.Dispose();
  }

  [TestMethod]
  public void TestQueryOSTest() {
    QueryOperatingSystem queryOS = new();
    var result_ = queryOS.GetInfo();
    Assert.IsNotEmpty(result_);
  }
}
