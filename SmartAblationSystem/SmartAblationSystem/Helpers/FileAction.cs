using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SmartAblationSystem.Helpers
{
  /// <summary>
  /// This class manage file action
  /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
  /// </summary>
  public class FileAction
  {
    int zipProgressEntriesValue = 0;
    int zipMaximumEntriesValue = 0;

    /// <summary>
    /// Gets/set Zip Progress Entries Value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>   
    public int ZipProgressEntriesValue
    {
      get => zipProgressEntriesValue;
      set => zipProgressEntriesValue = value;
    }
    /// <summary>
    /// Gets/set Zip Maximum Entries Value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public int ZipMaximumEntriesValue
    {
      get => zipMaximumEntriesValue;
      set => zipMaximumEntriesValue = value;
    }

    /// <summary>
    /// This public function removes pdf file from given file path and name
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public void DeleteCurrentPDFs(string filePathName)
    {
      string currentDoctorPDFName = GetBasePath() + "PDFFiles\\Doctor_" + filePathName + ".pdf";

      if(File.Exists(currentDoctorPDFName))
      {
        File.Delete(currentDoctorPDFName);
      }

      string currentBostonPDFName = GetBasePath() + "PDFFiles\\Boston_" + filePathName + ".pdf";

      if(File.Exists(currentBostonPDFName))
      {
        File.Delete(currentBostonPDFName);
      }

      string currentBostonBSCPDFName = GetBasePath() + "PDFFiles\\BostonBSC_" + filePathName + ".pdf";

      if(File.Exists(currentBostonBSCPDFName))
      {
        File.Delete(currentBostonBSCPDFName);
      }
    }
    /// <summary>
    /// Gets file path
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public string GetBasePath()
    {
      string thePath = "";

      String path = AppDomain.CurrentDomain.BaseDirectory;
      String[] extract = Regex.Split(path, "bin");  //split it in bin
      thePath = extract[0];
      return thePath;
    }

    /// <summary>
    /// Create folder with passed name
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public void CreateNewFolder(string FolderName)
    {
      //  string FolderPath;
      //  FolderPath = Path + FolderName;
      try
      {
        if(!Directory.Exists(FolderName))
        {
          Directory.CreateDirectory(FolderName);
        }
      }
      catch(Exception ex)
      {

      }
    }

    /// <summary>
    /// Set permission for a folder
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public void SetFolderPermission(string folderPath, int userType)
    {
      var directoryInfo = new DirectoryInfo(folderPath);
      var directorySecurity = directoryInfo.GetAccessControl();
      FileSystemAccessRule fileSystemRule;
      String UserAccount = "";

      if(userType == 1)
        UserAccount = WindowsIdentity.GetCurrent().Name;
      else
        UserAccount = "SMARTFREEZE\\Hospital";

      fileSystemRule = new FileSystemAccessRule(UserAccount, FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit,
                                                    PropagationFlags.None, AccessControlType.Allow);
      directorySecurity.AddAccessRule(fileSystemRule);
      directoryInfo.SetAccessControl(directorySecurity);
    }
    /// <summary>
    /// Create a folder with permission
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public void CreateNewFolderWithPermission(string Path, string FolderName)
    {
      string FolderPath;
      FolderPath = Path + FolderName;
      try
      {
        if(!Directory.Exists(FolderPath))
        {
          Directory.CreateDirectory(FolderPath);
          SetFolderPermission(FolderPath, 1);
          SetFolderPermission(FolderPath, 2);
        }
      }
      catch(Exception ex)
      {

      }
    }

    /// <summary>
    /// Zip file with password
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public void ZipFileWithPW(string Path, string PW, string outputPath)
    {
      //Reset the Progress values:
      ZipMaximumEntriesValue = 0;
      ZipProgressEntriesValue = 0;

      using(ZipFile zip = new ZipFile())
      {
        zip.Password = PW;
        DirectoryInfo dir = new DirectoryInfo(Path);
        try
        {
          zip.SaveProgress += Zip_SaveProgress;

          FileInfo[] files = dir.GetFiles();
          foreach(FileInfo file in files)
          {
            zip.AddFile(file.FullName);
          }
          zip.Save(outputPath);
          Directory.Delete(Path, true);
        }
        catch(Exception ex)
        {

        }
      }
    }

    public void ZipFilesWithPassword(string sourceFolder, string password, string targetFolder)
    {
      using(var zip_ = new ZipFile())
      {
				zip_.Password = password;
				var dir_ = new DirectoryInfo(sourceFolder);
        try
        {
          zip_.SaveProgress += Zip_SaveProgress;
          FileInfo[] files_ = dir_.GetFiles();
          foreach(var file_ in files_)
          {
						zip_.AddFile(file_.FullName, string.Empty);
          }

          foreach(var dir in dir_.GetDirectories())
          {
            foreach(var file_ in dir.GetFiles())
            {
							zip_.AddFile(file_.FullName, String.Empty);
            }
          }
					zip_.Save(targetFolder);
        }
        catch(Exception e)
        {
          LogSystem.LogService.LogException(e);
        }
      }
    }

    public void ZipSingleFileWithPassword(string sourceName, string password, string targetFolder)
    {
      using(var zip_ = new ZipFile())
      {
        try
        {
          zip_.Password = password;
          zip_.AddFile(sourceName);
          zip_.Save(targetFolder);
        }
        catch(Exception e)
        {
          LogSystem.LogService.LogException(e);
        }
      }
    }

    /// <summary>
    /// Set zip maximum entries value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private void Zip_SaveProgress(object sender, SaveProgressEventArgs e)
    {
      if(e?.EntriesTotal != 0)
      {
        ZipMaximumEntriesValue = e.EntriesTotal;

      }

      if(e?.EntriesSaved != 0)
      {
        ZipProgressEntriesValue = e.EntriesSaved;
      }

    }

    /// <summary>
    /// Zip a file
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public void ZipFile(string Path, string PW, string outputPath)
    {
      using(ZipFile zip = new ZipFile())
      {
        DirectoryInfo dir = new DirectoryInfo(Path);
        try
        {
          FileInfo[] files = dir.GetFiles();
          foreach(FileInfo file in files)
          {
            zip.AddFile(file.FullName);

          }
          zip.Save(outputPath);

        }
        catch(Exception ex)
        {

        }
      }
    }
    /// <summary>
    /// Remove the .pdf files in in FDFFiles folder. 
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public void RemovePDFFile()
    {

      try
      {
        string[] files = System.IO.Directory.GetFiles(GetBasePath() + "PDFFiles", "*.pdf");

        foreach(string file in files)
        {
          File.Delete(file);
        }
      }
      catch(Exception ex)
      {
      }


    }



    /// <summary>
    /// Regular expression for input string
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool InputRegularExpression(string inputstring)
    {
      bool isValid = false;
      try
      {
        Regex objAlphaNumericPattern = new Regex("^[a-zA-Z0-9 _,-]*$");
        if(objAlphaNumericPattern.IsMatch(inputstring))
          isValid = true;
      }
      catch(Exception ex)
      {
      }
      return isValid;

    }


    public void SaveImage(Canvas canvas, int width, int height, string filePath, double DPI)
    {
      Rect bounds = VisualTreeHelper.GetDescendantBounds(canvas);

      double dpi = DPI;
      RenderTargetBitmap rtb = new RenderTargetBitmap(width, height, dpi, dpi, System.Windows.Media.PixelFormats.Default);

      DrawingVisual dv = new DrawingVisual();
      using(DrawingContext dc = dv.RenderOpen())
      {
        VisualBrush vb = new VisualBrush(canvas);

        dc.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
      }

      rtb.Render(dv);

      BmpBitmapEncoder image = new BmpBitmapEncoder();
      image.Frames.Add(BitmapFrame.Create(rtb));

      try
      {
	      using (Stream fs = File.Create(filePath))
	      {
		      image.Save(fs);
		      fs.Close();
	      }
      }
      catch (Exception ex)
      {
	      LogSystem.LogService.LogException(ex);
      }

    }

  }
}
