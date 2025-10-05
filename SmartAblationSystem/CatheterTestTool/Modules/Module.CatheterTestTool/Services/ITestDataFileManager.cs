using System.Collections.Generic;
using Module.CatheterTestTool.Models;

namespace Module.CatheterTestTool.Services
{
  public interface ITestDataFileManager
  {
    bool SaveTestData(TestReportData testReportData, IList<CatheterTestData> detailData); 
    IList<string> SearchTestResultFiles();
    bool MoveTestDataFiles(IEnumerable<string> fileNameList, string targetDrive); 
  }
}