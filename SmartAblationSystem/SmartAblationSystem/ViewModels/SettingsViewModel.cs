using DataAccessLayer;
using SmartAblationSystem.Helpers;
using SmartAblationSystem.Models;
using SmartAblationSystem.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using FileSerializer;
using Ionic.Zip;
using Prism.Commands;
using Microsoft.Win32;
using Prism.Mvvm;
using Shared;
using static LogSystem.LogService;

namespace SmartAblationSystem.ViewModels
{
	/// <summary>
	/// This class is the Settings View Model
	/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
	/// </summary>
	internal class SettingsViewModel : BindableBase
	{
    private static byte[] updateAppCommandEncryptionCode = { 0xa8, 0xd1, 0x9a, 0x92, 0x6e, 0x28, 0xb3, 0x8f, 0x5, 0xad, 0x3d, 0x1a, 
                                                             0x29, 0x40, 0x1b, 0xa3, 0xd1, 0xf3, 0x85, 0xaa, 0x09, 0x09, 0xd5, 0xa0, 
                                                             0x14, 0xa2, 0x33, 0x18, 0x49, 0x70 };
    private static byte[] serviceToolAppEncryptionCode = { 0xe8, 0xe2, 0xab, 0xd6, 0x9, 0x32, 0xb3, 0x8f, 0x5, 0xad, 0x52, 0x37, 
                                                           0x13, 0x4a, 0x19, 0xb7, 0xdc, 0xd0, 0xa1, 0xab, 0x17, 0x14, 0xbc, 0xce,
                                                           0x4, 0xb5, 0x3e, 0x39, 0x11, 0x24, 0x39, 0x87 };
		private bool isZipingFiles = false;

		int zipProgressEntriesValue = 0;
		int zipMaximumEntriesValue = 0;
		private long minimumSpaceForZiping = 1000;  //1G

		int progressPercentage = 0;

		DispatcherTimer progressBarTimer;

		FileAction fileAction;

		private Language selectedUserManualLanguage;

		private int userManualLanguageId = -1;
		public ICommand ManageUsersCommand { get; private set; }
		public ICommand ServiceToolCommand { get; }

		public ICommand CatheterToolCommand { get; }
		public ICommand UpdateAppCommand { get; private set; }
		public ICommand TimeAndDateCommand { get; private set; }

		public ICommand UserManualCommand { get; private set; }

		public ICommand MaintenanceCommand { get; private set; }

		public ICommand ActionLogCommand { get; private set; }

		private ViewsEventArgs viewsEvent;

		private CommonViewModel localCommonViewModel = CommonViewModel.Current;

		private bool isUserAllowedTochangeDateTime = false;

		public ICommand ViewErrorLogCommand { get; private set; }

		private USBDriveConnectionManager.USBDriveConnectionManager usbDriveConnectionManager;

		private List<DriveInfo> usbDriveList;

		FileSystemWatcher watcher;
		public List<DriveInfo> USBDriveList
		{
			get
			{
				return usbDriveList;
			}
			set
			{
				usbDriveList = value;
				RaisePropertyChanged("USBDriveConnected");
				RaisePropertyChanged("IsServiceToolsAvailable");
				RaisePropertyChanged("IsCatheterToolAvailable");
			}
		}
		public bool USBDriveConnected
		{
			get
			{
				if (USBDriveList != null && USBDriveList.Count != 0)
				{
					var usbRoot = USBDriveList[0].Name;
					return File.Exists(usbRoot + "SystemUpdater.zip");
				}
				return false;
			}
		}
		/// <summary>
		/// Indicate if the service tool is available to use
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsServiceToolsAvailable
		{
			get
			{
				if (USBDriveList != null && USBDriveList.Count != 0)
				{
					var usbRoot = USBDriveList[0].Name;
					return (File.Exists(usbRoot + "ServiceTool.zip"));
					//return (File.Exists(usbRoot + "ServiceTool.zip") && IsNetRightVersion());
				}
				return false;
			}
		}

		/// <summary>
		/// Indicate if the catheter tool is available to use
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsCatheterToolAvailable
		{
			get
			{
				if (USBDriveList != null && USBDriveList.Count != 0)
				{
					var usbRoot = USBDriveList[0].Name;
					return (File.Exists(usbRoot + "CatheterTool.zip"));
					//return (File.Exists(usbRoot + "CatheterTool.zip") && IsNetRightVersion());
				}
				return false;
			}
		}

		public bool _isWaitingUnzip = false;
		public bool IsWaitingUnzip
		{
			get
			{
				return _isWaitingUnzip;
			}
			set
			{
				_isWaitingUnzip = value;
				RaisePropertyChanged("IsWaitingUnzip");
			}
		}
		public bool CanUpdate
		{
			get
			{
				return CommonViewModel.Current.IsCryterionUser || CommonViewModel.Current.IsBSCADMINUser;
			}
		}
		/// <summary>
		/// Indicate if the service tool can be used by the user
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool CanUseServiceTool
		{
			get
			{
				return (CommonViewModel.Current.IsCryterionUser || CommonViewModel.Current.IsBSCADMINUser);
			}
		}

		/// <summary>
		/// Indicate if the catheter tool can be used by the user
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool CanUseCatheterTool
		{
			get
			{
				return (CommonViewModel.Current.IsCryterionUser || CommonViewModel.Current.IsBSCADMINUser);
			}
		}

		/// <summary>
		/// This constructor initializes the Settings commands and properties
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public SettingsViewModel()
		{
      _hospitalName = CommonViewModel.Current.Data.DataAccess.GetHospitalName();
      IsWaitingUnzip = false;
			ManageUsersCommand = new DelegateCommand<object>(OnManageUsersCommand, CanManageUsersCommand);
			TimeAndDateCommand = new DelegateCommand<object>(OnTimeAndDateCommand, CanTimeAndDateCommand);
			UserManualCommand = new DelegateCommand<object>(OnUserManualCommand, CanUserManualCommand);
			UpdateAppCommand = new DelegateCommand<object>(OnUpdateAppCommand, CanUserManualCommand);
			ServiceToolCommand = new DelegateCommand<object>(OnServiceToolCommand).ObservesCanExecute(() => IsServiceToolsAvailable);
			CatheterToolCommand = new DelegateCommand<object>(OnCatheterToolCommand).ObservesCanExecute(() => IsCatheterToolAvailable);
			MaintenanceCommand = new DelegateCommand<object>(OnMaintenanceCommand, CanMaintenanceCommand);
			ActionLogCommand = new DelegateCommand<object>(OnActionLogCommand, CanActionLogCommand);
			ViewErrorLogCommand = new DelegateCommand<object>(OnViewErrorLogCommand, CanViewErrorLogCommand);
			localCommonViewModel.PropertyChanged += Current_PropertyChanged;

			viewsEvent = new ViewsEventArgs();

			progressBarTimer = new DispatcherTimer();
			progressBarTimer.Interval = TimeSpan.FromMilliseconds(500);
			progressBarTimer.Tick += ProgressBarTimer_Tick;

			var originalUserManualLanguageId = CommonViewModel.Current.Data.DataAccess.GetSelectedUserManualLanguageId();
			UserManualLanguages = new ObservableCollection<Language>(Languages.GetAllUserManualLanguage());
			if (originalUserManualLanguageId > 0 && UserManualLanguages?.Count > 0)
			{
				foreach (Language language in UserManualLanguages)
				{
					if (language.Id == originalUserManualLanguageId)
					{
						selectedUserManualLanguage = language;
					}
				}
			}

			usbDriveConnectionManager = new USBDriveConnectionManager.USBDriveConnectionManager(USBDriveConnection_EventArrived);

			if (usbDriveConnectionManager != null)
			{
				try
				{
					USBDriveList = usbDriveConnectionManager.GetUSBDriveList();
				}
				catch (Exception ex)
				{
					// TODO
					ex.ToString();
				}
			}

		}

		private void USBDriveConnection_EventArrived(object sender, EventArrivedEventArgs e)
		{
			try
			{
				USBDriveList = usbDriveConnectionManager.GetUSBDriveList();
			}
			catch (Exception ex)
			{
				ex.ToString();
			}
		}
		/// <summary>
		/// Occurs when the progress bar Timer Tick event is raised
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="sender">The object that sent the event.</param>
		/// <param name="e">The progress event</param>
		private void ProgressBarTimer_Tick(object sender, EventArgs e)
		{
			if (fileAction.ZipMaximumEntriesValue != 0)
			{
				ZipMaximumEntriesValue = fileAction.ZipMaximumEntriesValue;
				ZipProgressEntriesValue = fileAction.ZipProgressEntriesValue;

				ProgressPercentage = (ZipProgressEntriesValue * 100 / ZipMaximumEntriesValue);
			}
		}
		/// <summary>
		/// Command that close current app and start Service Tool app 
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		private void OnServiceToolCommand(object obj)
		{
			var genericMessage = new Tuple<long, string, string, string>(26089, "Are you sure you want to start the Service Tool application? (This closes the application)", "Solution", "N/A");
			var dialogPopup = new MessagePopup(genericMessage, messageType: MessagePopup.MessageType.SystemMessage);

			if (dialogPopup != null && !(bool)dialogPopup.ShowDialog()) return;

			// (1) Zip to Console drive
			var zipFile = USBDriveList[0].Name + "ServiceTool.zip";
			var extractPopup = new MessagePopup(new Tuple<long, string, string, string>(1, "Creating Service Tool ...", "Solution", ""), MessagePopup.MessageType.SystemMessage, MessagePopup.ButtonType.Ok, "");
			extractPopup.Show();
			IsWaitingUnzip = true;
			ZipFile zip = ZipFile.Read(zipFile);   

			//#if DEBUG || Simulator
			//			string serviceToolPath = Path.Combine(Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName, "ServiceTool");
			//#else
			string serviceToolPath = ConfigurationManager.AppSettings["ServiceToolPath"];
//#endif
			zip.Password = new string(PasswordUtils.DecryptPasscode(serviceToolAppEncryptionCode));
			zip.ExtractAll(serviceToolPath, ExtractExistingFileAction.OverwriteSilently);
			extractPopup.Close();
			IsWaitingUnzip = false;

			// (2) Exit SmartFreeze
			localCommonViewModel.Console.PowerOffMessage();
			Thread.Sleep(500);
			localCommonViewModel.Console.DeactivateAllIOS();
			Thread.Sleep(500);
			localCommonViewModel.Console.CanBusCommunication.Dispose();
			Thread.Sleep(1000);

			// (3) Start Service Tool
			try
			{
				using (Process myProcess = new Process())
				{
					var programPath = Path.Combine(serviceToolPath, ConfigurationManager.AppSettings["ServiceToolFileName"]);
					if (File.Exists(programPath))
					{
						myProcess.StartInfo.FileName = programPath;
						myProcess.StartInfo.WorkingDirectory = Path.GetDirectoryName(programPath) ?? string.Empty;
						myProcess.StartInfo.CreateNoWindow = false;
						myProcess.Start();
					}
					else
					{
						var exceptionPopup = new MessagePopup(new Tuple<long, string, string, string>(2, $"Service Tool does not exist. {programPath}", "Solution", ""), MessagePopup.MessageType.ErrorMessage, MessagePopup.ButtonType.Ok);
						exceptionPopup.ShowDialog();
					}
				}
			}
			catch (Exception e)
			{
				LogException(e);
				throw;
			}
			Environment.Exit(0);
		}


		/// <summary>
		/// Command that close current app and start Catheter Tool app 
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		private void OnCatheterToolCommand(object obj)
		{
			var genericMessage = new Tuple<long, string, string, string>(26089, "Are you sure you want to start the Catheter Tool application? (This closes the application)", "Solution", "N/A");
			var dialogPopup = new MessagePopup(genericMessage, messageType: MessagePopup.MessageType.SystemMessage);

			if (dialogPopup != null && !(bool)dialogPopup.ShowDialog()) return;

			// (1) Zip to Console drive
			var zipFile = USBDriveList[0].Name + "CatheterTool.zip";
			var extractPopup = new MessagePopup(new Tuple<long, string, string, string>(1, "Creating Catheter Tool ...", "Solution", ""), MessagePopup.MessageType.SystemMessage, MessagePopup.ButtonType.Ok, "");
			extractPopup.Show();
			IsWaitingUnzip = true;
			ZipFile zip = ZipFile.Read(zipFile);
			string serviceToolPath = ConfigurationManager.AppSettings["CatheterToolPath"];
			//#endif
			zip.Password = "OIJGaqdf&(!NcYSQEHk76ONPM$JZM@aAWCfggfg";
			zip.ExtractAll(serviceToolPath, ExtractExistingFileAction.OverwriteSilently);
			extractPopup.Close();
			IsWaitingUnzip = false;

			// (2) Exit SmartFreeze
			localCommonViewModel.Console.PowerOffMessage();
			Thread.Sleep(500);
			localCommonViewModel.Console.DeactivateAllIOS();
			Thread.Sleep(500);
			localCommonViewModel.Console.CanBusCommunication.Dispose();
			Thread.Sleep(1000);

			// (3) Start Tool
			try
			{
				using (Process myProcess = new Process())
				{
					var programPath = Path.Combine(serviceToolPath, ConfigurationManager.AppSettings["CatheterToolFileName"]);
					if (File.Exists(programPath))
					{
						myProcess.StartInfo.FileName = programPath;
						myProcess.StartInfo.WorkingDirectory = Path.GetDirectoryName(programPath) ?? string.Empty;
						myProcess.StartInfo.CreateNoWindow = false;
						myProcess.Start();
					}
					else
					{
						var exceptionPopup = new MessagePopup(new Tuple<long, string, string, string>(2, $"Catheter Tool does not exist. {programPath}", "Solution", ""), MessagePopup.MessageType.ErrorMessage, MessagePopup.ButtonType.Ok);
						exceptionPopup.ShowDialog();
					}
				}
			}
			catch (Exception e)
			{
				LogException(e);
				throw;
			}
			Environment.Exit(0);
		}




		private void OnUpdateAppCommand(object obj)
		{
			var USBRoot = USBDriveList[0].Name;
      var password = PasswordUtils.DecryptPasscode(updateAppCommandEncryptionCode); 

			Tuple<long, string, string, string> genericMessage = new Tuple<long, string, string, string>(1, "Continuing with the update process will close the SmartAblation application. Continue with the update of the SMARTFREEZE System?", "Solution", "");

			MessagePopup MessagePopup = new MessagePopup(genericMessage, MessagePopup.MessageType.SystemMessage, MessagePopup.ButtonType.YesNo, "");
			var zipFile = USBRoot + "SystemUpdater.zip";
			try
			{
				if ((bool)MessagePopup.ShowDialog())
				{
					if (File.Exists(zipFile) && ZipFile.CheckZipPassword(zipFile, new string(password)) && !ZipFile.CheckZipPassword(zipFile, "1"))//Call System Updater
					{
						MessagePopup ExtractPopup = new MessagePopup(new Tuple<long, string, string, string>(1, "Extracting Update script...", "Solution", ""), MessagePopup.MessageType.SystemMessage, MessagePopup.ButtonType.Ok, "");
						ExtractPopup.Show();
						IsWaitingUnzip = true;
						ZipFile zip = ZipFile.Read(zipFile);
						zip.Password = new string(password);
						zip.ExtractAll(USBRoot + "SystemUpdater/", ExtractExistingFileAction.OverwriteSilently);
						//ExtractPopup.Hide();
						ExtractPopup.Close();
						IsWaitingUnzip = false;

						Process UpdateProcess = new Process();
						UpdateProcess.StartInfo.FileName = Path.Combine(USBRoot, "SystemUpdater/SystemUpdate.exe");
						UpdateProcess.Start();
						// Environment.Exit(0);
						localCommonViewModel.Console.PowerOffMessage();
						System.Threading.Thread.Sleep(500);
						localCommonViewModel.Console.DeactivateAllIOS();
						System.Threading.Thread.Sleep(500);
						localCommonViewModel.Console.CanBusCommunication.Dispose();
						System.Threading.Thread.Sleep(1000);
						Environment.Exit(0);
						UpdateProcess.BeginOutputReadLine();

						UpdateProcess.WaitForExit();


						//                        Directory.Delete("D:/SystemUpdater/");
					}
					else
					{
						Tuple<long, string, string, string> newgenericMessage = new Tuple<long, string, string, string>(2, "The files required for the update were not found on the inserted USB.", "Please verify that the necessary files are correctly placed inside the USB drive.", "");

						MessagePopup newMessagePopup = new MessagePopup(newgenericMessage, MessagePopup.MessageType.ErrorMessage, MessagePopup.ButtonType.Ok, "", true);
						newMessagePopup.ShowDialog();
					}

				}
				else
				{
					return;
				}
			}
			catch (Exception ex)
			{
				ex.ToString();
      }
      finally
      {
        Array.Clear(password, 0, password.Length);
			}
		}

		/// <summary>
		/// Function that returns if the system can invoke the Manage Users command
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="arg">The command parameter (not used in this function).</param>
		/// <returns>Boolean value if the system can invoke the command.</returns>
		private bool CanManageUsersCommand(object arg)
		{
			return true;
		}

		/// <summary>
		/// Function/Command that handles the Manage User command
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="arg">Command parameter (not used in this function).</param>
		private void OnManageUsersCommand(object arg)
		{
			if (ZiPStates.IsZipingFiles)
				return;

			viewsEvent.ViewName = "ManageUsers";
			CommonViewModel.Current.OnViewchanged(viewsEvent);
		}

		/// <summary>
		/// Function that returns if the system can invoke the Time and Date command
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="arg">The command parameter (not used in this function).</param>
		/// <returns>Boolean value if the system can invoke the command.</returns>
		private bool CanTimeAndDateCommand(object arg)
		{
			return true;
		}

		/// <summary>
		/// Function/Command that handles the Time And Date command
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="arg">Command parameter (not used in this function).</param>
		private void OnTimeAndDateCommand(object arg)
		{
			if (ZiPStates.IsZipingFiles)
				return;

			viewsEvent.ViewName = "TimeAndDate";
			CommonViewModel.Current.OnViewchanged(viewsEvent);
		}

		/// <summary>
		/// Function that returns if the system can invoke the User Manual command
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="arg">The command parameter (not used in this function).</param>
		/// <returns>Boolean value if the system can invoke the command.</returns>
		private bool CanUserManualCommand(object arg)
		{
			return true;
		}

		/// <summary>
		/// Function/Command that handles the User Manual command
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="arg">Command parameter (not used in this function).</param>
		private void OnUserManualCommand(object arg)
		{
			viewsEvent.ViewName = "UserManual";
			CommonViewModel.Current.OnViewchanged(viewsEvent);
		}

		///// <summary>
		///// Get the drive total free space.   
		///// . Safety classification: Death or SERIOUS INJURY is possible (IEC 62304 Class C). (unused)
		///// </summary>
		///// <param name="driveName">the drive name</param>
		///// <returns>The available free space</returns>
		private long GetTotalFreeSpace(string driveName)
		{
			foreach (DriveInfo drive in DriveInfo.GetDrives())
			{
				if (drive.IsReady && drive.Name == driveName)
				{
					// return drive.TotalFreeSpace;
					return drive.AvailableFreeSpace;
				}
			}
			return -1;
		}


		/// <summary>
		/// Function that dele file.
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="filepath">The file path </param>
		private void DeleteTmpFile(string filepath)
		{

			DirectoryInfo di = new DirectoryInfo(@filepath);
			FileInfo[] files = di.GetFiles("*.tmp")
													 .Where(p => p.Extension == ".tmp").ToArray();
			foreach (FileInfo file in files)
				try
				{
					if (File.Exists(file.FullName)) File.Delete(file.FullName);
				}
				catch { }
		}


		/// <summary>
		/// Asynchronous task that archive files
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <returns></returns>
		private async Task ArchiveFiles()
		{

			fileAction = new FileAction();

			Procedure procedure = new Procedure();
			List<Procedure> procedureList = null;
			string basePath = fileAction.GetBasePath();
			if (!Directory.Exists(basePath + "Archive")) fileAction.CreateNewFolder(basePath + "Archive");

			DeleteTmpFile(basePath + "Archive");

			string PW = "Ehdege*787)";  //ConfigurationManager.AppSettings["BSCSystemZip"];
			procedureList = CommonViewModel.Current?.Data?.DataAccess?.GetAllProcedures().FindAll(a => a.Ablations.Count != 0);
			if (procedureList.Count > 0)
			{
				List<Procedure> t = procedureList.OrderBy(n => n.Id).ToList();
				Procedure firstRecord = t[0];
				Procedure lastRecord = t[t.Count - 1];
				int Min = firstRecord.Id;

				int Max = lastRecord.Id;
				string jsonfilename = Min.ToString() + "-" + Max.ToString() + "_" + DateTime.Now.ToString("yyyy'-'dd'-'M'--'HH'-'mm'-'ss") + ".zip";
				try
				{

					await Task.Run(() =>
					{

						fileAction.ZipFileWithPW(basePath + "FileStore", PW, basePath + "Archive\\" + jsonfilename);
						fileAction.CreateNewFolderWithPermission(basePath, "FileStore");
						CommonViewModel.Current.Data.DataAccess.UpdateProceduresArchived();
						IsZipingFiles = false;
						progressBarTimer.Stop();

					});



					Tuple<long, string, string, string> genericMessage = Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID107, (int)Enumeration.ErrorTypes.GUI);

					MessagePopup MessagePopup = new MessagePopup(genericMessage, MessagePopup.MessageType.SystemMessage, MessagePopup.ButtonType.Ok, "");
					MessagePopup.ShowDialog();

#if !DEBUG
                    PowerOff();
#endif
				}
				catch (Exception ex)
				{
					IsZipingFiles = false;
					progressBarTimer.Stop();

					Tuple<long, string, string, string> genericMessage = Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID106, (int)Enumeration.ErrorTypes.GUI);

					MessagePopup MessagePopup = new MessagePopup(genericMessage, MessagePopup.MessageType.ErrorMessage, MessagePopup.ButtonType.Ok, "");
					MessagePopup.ShowDialog();

				}
			}
			else
			{
				IsZipingFiles = false;
				progressBarTimer.Stop();

				Tuple<long, string, string, string> genericMessage = Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID105, (int)Enumeration.ErrorTypes.GUI);

				MessagePopup MessagePopup = new MessagePopup(genericMessage, MessagePopup.MessageType.SystemMessage, MessagePopup.ButtonType.Ok, "");
				MessagePopup.ShowDialog();

			}

		}

		/// <summary>
		/// Function that returns if the system can invoke the archive command
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="arg">The command parameter (not used in this function).</param>
		/// <returns>Boolean value if the system can invoke the command.</returns>
		private bool CanArchiveCommand(object arg)
		{
			return true;
		}

		/// <summary>
		/// Function that returns if the system can invoke the Maintenance command
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="arg">The command parameter (not used in this function).</param>
		/// <returns>Boolean value if the system can invoke the command.</returns>
		private bool CanMaintenanceCommand(object arg)
		{
			return true;
		}

		/// <summary>
		/// Function/Command that handles the Maintenance command
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="arg">Command parameter (not used in this function).</param>
		private void OnMaintenanceCommand(object arg)
		{
			if (ZiPStates.IsZipingFiles)
				return;

			viewsEvent.ViewName = "Service";
			CommonViewModel.Current.OnViewchanged(viewsEvent);
		}

		/// <summary>
		/// Function that returns if the system can invoke the Action Log command
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="arg">The command parameter (not used in this function).</param>
		/// <returns>Boolean value if the system can invoke the command.</returns>
		private bool CanActionLogCommand(object arg)
		{
			return true;
		}

		/// <summary>
		/// Function/Command that handles the Action Log command
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="arg">Command parameter (not used in this function).</param>
		private void OnActionLogCommand(object arg)
		{
			if (ZiPStates.IsZipingFiles)
				return;

			viewsEvent.ViewName = "ActionLog";
			CommonViewModel.Current.OnViewchanged(viewsEvent);
		}

		//private


		/// <summary>
		/// Function that returns if the system can invoke the Action Log command
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="arg">The command parameter (not used in this function).</param>
		/// <returns>Boolean value if the system can invoke the command.</returns>
		private bool CanViewErrorLogCommand(object arg)
		{
			return true;
		}

		/// <summary>
		/// Function/Command that handles the Action Log command
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="arg">Command parameter (not used in this function).</param>
		private void OnViewErrorLogCommand(object arg)
		{
			if (ZiPStates.IsZipingFiles)
				return;

			viewsEvent.ViewName = "ViewErrorLog";
			CommonViewModel.Current.OnViewchanged(viewsEvent);
		}






		/// <summary>
		/// This read-only property returns if the current user has Cryterion Type
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsCryterionUser
		{
			get
			{
				if (CommonViewModel.Current.IsBSCADMINUser || CommonViewModel.Current.IsCryterionUser)
					return true;
				else
					return false; //CommonViewModel.Current.IsCryterionUser;
			}
		}

		public bool IsBSCADMINUser
		{
			get
			{
				return CommonViewModel.Current.IsBSCADMINUser;
			}
		}

		/// <summary>
		/// This read-only property returns if the current user is admin
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsAdminUser
		{
			get
			{
				return CommonViewModel.Current.IsAdminUser;
			}
		}

		/// <summary>
		/// This read-only property returns if the current user has User Type
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsUser
		{
			get
			{
				return CommonViewModel.Current.IsUser;
			}
		}

		/// <summary>
		/// This read-only property returns the system's Minutes of User
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public long MinutesOfUse
		{
			get
			{
				return CommonViewModel.Current.MinutesOfUse;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the software is ziping files 
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsZipingFiles
		{
			get
			{
				return isZipingFiles;
			}

			set
			{
				isZipingFiles = value;
				ZiPStates.IsZipingFiles = value;
				RaisePropertyChanged("IsZipingFiles");
			}
		}


		/// <summary>
		/// Gets or sets the Zip progress entries value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public int ZipProgressEntriesValue
		{
			get
			{
				return zipProgressEntriesValue;
			}

			set
			{
				zipProgressEntriesValue = value;
				RaisePropertyChanged("ZipProgressEntriesValue");
			}
		}


		/// <summary>
		/// Gets or sets the Zip maximum entries value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public int ZipMaximumEntriesValue
		{
			get
			{
				return zipMaximumEntriesValue;
			}

			set
			{
				zipMaximumEntriesValue = value;
				RaisePropertyChanged("ZipMaximumEntriesValue");
			}
		}

		/// <summary>
		/// Gets or sets the Zip progress percentage value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public int ProgressPercentage
		{
			get
			{
				return progressPercentage;
			}

			set
			{
				progressPercentage = value;
				RaisePropertyChanged("ProgressPercentage");
			}
		}

    private string _hospitalName;
    public string HospitalName
    {
			get=>_hospitalName;
      set => SetProperty(ref _hospitalName, value);
    }

		/// <summary>
		/// Gets or sets the selected user manual Language value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public Language SelectedUserManualLanguage
		{
			get
			{
				return selectedUserManualLanguage;
			}

			set
			{
				selectedUserManualLanguage = value;
				RaisePropertyChanged("SelectedUserManualLanguage");
				CommonViewModel.Current.Data.DataAccess.SetCurrentUserManual(selectedUserManualLanguage.Id);
				Languages.SelectedUserManualLanguage = selectedUserManualLanguage;

			}
		}

		/// <summary>
		/// This function handles the sender's PropertyChanged event
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="sender">The View Model that sent the event.</param>
		/// <param name="e">The parameter's name that has changed.</param>
		private void Current_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case "CurrentUser":
					RaisePropertyChanged("IsCryterionUser");
					RaisePropertyChanged("IsUser");
					RaisePropertyChanged("IsAdminUser");
					RaisePropertyChanged("IsBSCADMINUser");
					RaisePropertyChanged("CanUpdate");
					RaisePropertyChanged("CanUseServiceTool");
					RaisePropertyChanged("CanUseCatheterTool");
					break;

				case "MinutesOfUse":
					RaisePropertyChanged("MinutesOfUse");
					break;
			}
		}

		/// <summary>
		/// Power off the SBC
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		private void PowerOff()
		{

			CommonViewModel.Current.Console.PowerOffMessage();
			System.Threading.Thread.Sleep(3000);
			Process.Start("shutdown", "/s /t 0");
		}

		/// <summary>
		/// Gets all user manual languages
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public ObservableCollection<Language> UserManualLanguages { get; }

		/// <summary>
		/// Gets or sets a value indicating whether if a user allowed to change Date Time
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsUserAllowedTochangeDateTime
		{
			get
			{
				return (IsAdminUser || IsCryterionUser || IsBSCADMINUser);
			}

		}


		/// <summary>
		/// Reset the selected user manual language
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public void ResetSelectedUserManualLanguage()
		{

			RaisePropertyChanged("SelectedUserManualLanguage");
		}
	}
}