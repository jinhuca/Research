using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Module.CatheterTestTool.Models;
using Module.Infrastructure.AppLog;

namespace Module.CatheterTestTool.Services
{
  public class TestDataFileManager : ITestDataFileManager
  {
    private static readonly string TIME_STAMP_FORMAT= "yyyyMMdd-HHmmss";
    private static readonly string FILENAME_PREFIX = "POLARxTest_";
    private static readonly string RESULT_FILE_EXT = "xml";
    private static readonly string DETAIL_FILE_EXT = "csv";


    private readonly string _sourceReportFileRootFolder;
    private readonly string _targetFileRootFolder;

    public TestDataFileManager()
    {
      _sourceReportFileRootFolder = Properties.Settings.Default.TestReportRootFolder;
      _targetFileRootFolder = Properties.Settings.Default.TargetRootFolder;
    }

    public bool SaveTestData(TestReportData testReportData, IList<CatheterTestData> detailData)
    {
      bool saveFileSucceed = false;

      try
      {
        CreateFolderIfNotExist(_sourceReportFileRootFolder);

        var timeStamp = DateTime.Now.ToString(TIME_STAMP_FORMAT);
        var fileNames = CreateFileNames(testReportData.CatheterInfo.Lot, testReportData.CatheterInfo.SerialNumber, timeStamp); 

        SaveReportData(testReportData, fileNames.Item1);
        SaveDetailDataFile(detailData, fileNames.Item2); 

        saveFileSucceed = true;
      }
      catch (IOException ex)
      {
        saveFileSucceed = false;
      }

      return saveFileSucceed;
    }

    public IList<string> SearchTestResultFiles()
    {
      IList<string> fileList = new List<string>();

      try
      {
        fileList = Directory.GetFiles(_sourceReportFileRootFolder, $"{FILENAME_PREFIX}*.{RESULT_FILE_EXT}")
          .Select(Path.GetFileNameWithoutExtension)
          .Distinct()
          .ToList();
      }
      catch (Exception ex)
      {
        FieldServiceTrace.LogException(ex);
      }

      return fileList;
    }

    public bool MoveTestDataFiles(IEnumerable<string> fileNameList, string targetDrive)
    {
      var targetRootPath = Path.Combine(targetDrive, _targetFileRootFolder); 
      CreateFolderIfNotExist(targetRootPath);

      bool succeeded = true; 
      foreach (var fileName in fileNameList)
      {
        succeeded &= MoveTestResultFiles(fileName, targetRootPath);
      }

      return succeeded; 
    }

    private bool MoveTestResultFiles(string sourceFile, string targetFolder)
    {
      bool succeeded = true;
      try
      {
        var sourceFiles = Directory.GetFiles(_sourceReportFileRootFolder, sourceFile + ".*");
        foreach (var filePath in sourceFiles)
        {
          succeeded &= MoveTestResultFile(filePath, Path.Combine(targetFolder, Path.GetFileName(filePath)));
        }
      }
      catch (Exception ex)
      {
        succeeded = false;
        FieldServiceTrace.LogException(ex);
      }

      return succeeded; 
    }

    private bool MoveTestResultFile(string sourcePath, string destPath)
    {
      try
      {
        // Move file: Copy with overwrite = true, then delete source
        File.Copy(sourcePath, destPath, true);
        File.Delete(sourcePath);
        return true;
      }
      catch (Exception ex)
      {
        FieldServiceTrace.LogException(ex);
        return false;
      }
    }

    private Tuple<string, string> CreateFileNames(int catheterLotNum, int catheterSerialNum, string timeStamp)
    {
      var resultFileName = FILENAME_PREFIX + $"{catheterLotNum}_{catheterSerialNum}_{timeStamp}.{RESULT_FILE_EXT}"; 
      var detailFileName = FILENAME_PREFIX + $"{catheterLotNum}_{catheterSerialNum}_{timeStamp}.{DETAIL_FILE_EXT}";

      return Tuple.Create(resultFileName, detailFileName);
    }

    private bool SaveReportData(TestReportData testReportData, string resultFileName)
    {
      var fileFullPath = Path.Combine(_sourceReportFileRootFolder, resultFileName);

      var serializer = new XmlSerializer(typeof(TestReportData));
      TextWriter writer = new StreamWriter(fileFullPath);
      serializer.Serialize(writer, testReportData);
      writer.Close();

      return true;
    }

    private bool SaveDetailDataFile(IList<CatheterTestData> detailData, string detailFileName)
    {
      var fileFullPath = Path.Combine(_sourceReportFileRootFolder, detailFileName);
      var header = string.Join(",", "Temperature", "FM1", "IBP", "OBP", "PT2", "PT3", "PT4");

      TextWriter writer = new StreamWriter(fileFullPath);
      writer.WriteLine(header);

      foreach (var s in detailData
                 .Select(data => $"{data.TC},{data.FM1},{data.IBP},{data.OBP},{data.PT2},{data.PT3},{data.PT4}"))
      {
        writer.WriteLine(s);    
      }

      writer.Close();

      return true; 
    }

    private void CreateFolderIfNotExist(string rootPath)
    {
      if (!string.IsNullOrEmpty(rootPath) && !Directory.Exists(rootPath))
      {
        Directory.CreateDirectory(rootPath);
      }
    }
  }
}
