using Prism.Mvvm;
using System;
using System.IO;
using System.Management;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using static Communication.CanBusMessageDefinition;
using BootLoader;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections.Generic;
using SmartAblationSystem.Views;
using SmartAblationSystem.Helpers;
using static SmartAblationSystem.Helpers.Enumeration;
using System.Windows.Threading;
using System.Windows;
using System.Collections.ObjectModel;
using SmartAblationSystem.Models;
using DataAccessLayer;
using Prism.Commands;
using Microsoft.VisualBasic.Devices;

namespace SmartAblationSystem.ViewModels
{
    /// <summary>
    /// This class is the System Files View Model
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    internal class SystemFilesViewModel : BindableBase
    {

        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        private static extern long StrFormatKBSize(
         long qdw,
        [MarshalAs(UnmanagedType.LPTStr)] StringBuilder pszBuf,
        int cchBuf);



        private const int CentralMicroControllerFirmwareVersionId = 8;
        private const int PatientMicroControllerFirmwareVersionId = 48;
        private const int CatheterFirmwareVersionId = 56;
        private const int RepeaterFirmwareAndICBFirmwareId = 11;
        private const int RemoteFirmwareId = 24;

        private string FILESTORAGE = string.Empty;

        private int numberOfRetry = 3;

        static private int FileSeize = 0;

        byte[] Initdata = new byte[8];

        private bool isCPLDSelected = false;
        private bool isPMCUSelected = false;
        private bool isRepeaterSelected = false;
        private bool isICBSelected = false;
        private bool isRemoteSelected = false;
        private bool isCatheterSelected = false;
        private bool isCMCUSelected = false;

        private bool isLoadingFirmware = false;
        bool isFirmwareLoadSelected = false;

        public ICommand ReadFirmwareVersionCommand { get; private set; }

        public ICommand LoadFirmwareVersionCommand { get; private set; }

        public ICommand AppModeCommand { get; private set; }

        public ICommand ImportFileFromUSBCommand { get; private set; }


        private List<DriveInfo> usbDriveList;
        private USBDriveConnectionManager.USBDriveConnectionManager usbDriveConnectionManager;

        private FirmwareDescription firmwareDescription;

        private FirmwareDescription previousSelectedFirmwareDescription;

        private List<FirmwareDescription> firmwareDescriptions;

        DispatcherTimer percentageTimer;
        int transmittedDataPercentage = 0;
        int previousTransmittedDataPercentage = 0;
        int timeOutMaximumValue = 10;
        int timeOutCompter = 0;

        const short cMCUIndex = 0;
        const short pMCUIndex = 1;
        const short repeaterIndex = 2;
        const short cPLDIndex = 3;
        const short icbIndex = 4;
        const short remoteIndex = 5;

        byte[] moduleKeysdata = new byte[8];

        private ProcedureSpaceModel procedureSpaceModel;

        private bool isUsingSystemFile = false;

        private ConsoleVersion consoleVersion = null;

        /// <summary>
        /// This constructor initializes CMC, PMC, Catheter Firmware version and Repeater/ICM firmwares values
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public SystemFilesViewModel()
        {
            CommonViewModel.Current.PropertyChanged += Current_PropertyChanged;

            ReadTheFirmwareVersions();
            //Task.Delay(3000).ContinueWith(t => ReadTheFirmwareVersions());

            this.ReadFirmwareVersionCommand = new DelegateCommand<object>(this.OnReadFirmwareVersionCommand, this.CanReadFirmwareVersionCommand);

            this.LoadFirmwareVersionCommand = new DelegateCommand<object>(this.OnLoadFirmwareVersionCommand, this.CanLoadFirmwareVersionCommand);

            this.AppModeCommand = new DelegateCommand<object>(this.OnAppModeCommand, this.CanAppModeCommand);

            this.ImportFileFromUSBCommand = new DelegateCommand<object>(this.OnImportFileFromUSBCommand, this.CanImportFileFromUSBCommand);

            FileSeize = 0;

            FirmwareDescriptions = new List<FirmwareDescription>();
            PreviousSelectedFirmwareDescription = new FirmwareDescription(-1, "Unknown");

            GetFirmwareInformation();

            percentageTimer = new DispatcherTimer();
            percentageTimer.Interval = TimeSpan.FromSeconds(5);
            percentageTimer.Tick += PercentageTimer_Tick;


            //Get notified of each USB drive connection event
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

                    Tuple<long, string, string, string> genericMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID41, (int)Enumeration.ErrorTypes.GUI);
                    Tuple<long, string, string, string> titleMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID42, (int)Enumeration.ErrorTypes.GUI);

                    MessagePopup messagePopup = new MessagePopup(genericMessage, MessagePopup.MessageType.WarningMessage, MessagePopup.ButtonType.Ok, titleMessage.Item2);
                    messagePopup.ShowDialog();
                }
            }

            CreateBooTLoaderDataStorage();

            procedureSpaceModel = new ProcedureSpaceModel();

            consoleVersion = new ConsoleVersion();

            ConsoleVersion = CommonViewModel.Current.Data.DataAccess.GetLatestVersion();
        }

        /// <summary>
        /// Create boot loader data storage
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void CreateBooTLoaderDataStorage()
        {
            try
            {
                string directoryName = Path.Combine(GetBasePath(), "CPLD");
                System.IO.Directory.CreateDirectory(directoryName);

                directoryName = Path.Combine(GetBasePath(), "PMCU");
                System.IO.Directory.CreateDirectory(directoryName);

                directoryName = Path.Combine(GetBasePath(), "RMCU");
                System.IO.Directory.CreateDirectory(directoryName);

                directoryName = Path.Combine(GetBasePath(), "BMCU");
                System.IO.Directory.CreateDirectory(directoryName);

                directoryName = Path.Combine(GetBasePath(), "Catheter");
                System.IO.Directory.CreateDirectory(directoryName);

                directoryName = Path.Combine(GetBasePath(), "CMCU");
                System.IO.Directory.CreateDirectory(directoryName);

                directoryName = Path.Combine(GetBasePath(), "RCMCU");
                System.IO.Directory.CreateDirectory(directoryName);
            }

            catch (Exception ex)
            {
                //TODO
            }

        }

        /// <summary>
        /// This function is called at each Timer percentage timer's Tick
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">Sent event.</param>
        /// <param name="e">The property changed arguments (not used in this function).</param>
        private void PercentageTimer_Tick(object sender, EventArgs e)
        {

            IsBootLoaderUpdatingFirmware = true;

            if (IsCPLDSelected)
            {
                if (TransmittedDataPercentage > 90 && TransmittedDataPercentage < 100 && CommonViewModel.Current.UpgradeStatus == (double)CPLDStatusKey.CMCUPASSINTERMEDAIREITERMEDIARYPASS)
                {
                    TransmittedDataPercentage = 100;
                    return;
                }

                else if (TransmittedDataPercentage > 90 && TransmittedDataPercentage < 100 && CommonViewModel.Current.UpgradeStatus != (double)CPLDStatusKey.CMCUPASSINTERMEDAIREITERMEDIARYPASS)
                {
                    TimeOutCompter++;
                    if (TimeOutCompter >= timeOutMaximumValue)
                    {
                        IsLoadingFirmware = false;
                        TransmittedDataPercentage = 0;
                        IsBootLoaderUpdatingFirmware = false;
                        CommonViewModel.Current.ASCIIToByteConverter.DataTransmissionPercenatge = 0;
                        percentageTimer.Stop();

                        DisplayErrorMessage("Firmware update failed. Please try again. If the problem persists, reset the system and try again.", MessagePopup.MessageType.WarningMessage, MessagePopup.ButtonType.Ok, "Update Error");
                    }

                    return;
                }

              
            }

            TransmittedDataPercentage = CommonViewModel.Current.ASCIIToByteConverter.DataTransmissionPercenatge;

            if (TransmittedDataPercentage != PreviousTransmittedDataPercentage)
            {
                PreviousTransmittedDataPercentage = TransmittedDataPercentage;
            }
            else
            {

                TimeOutCompter++;
                if (TimeOutCompter >= timeOutMaximumValue)
                {
                    IsLoadingFirmware = false;
                    TransmittedDataPercentage = 0;
                    IsBootLoaderUpdatingFirmware = false;
                    CommonViewModel.Current.ASCIIToByteConverter.DataTransmissionPercenatge = 0;
                    percentageTimer.Stop();

                    DisplayErrorMessage("Firmware update failed. Please try again. If the problem persists, reset the system and try again.", MessagePopup.MessageType.WarningMessage, MessagePopup.ButtonType.Ok, "Update Error");
                }
            }

            if (TransmittedDataPercentage >= 100)
            {
                IsLoadingFirmware = false;
                TransmittedDataPercentage = 0;
                //IsBootLoaderUpdatingFirmware = false;
                CommonViewModel.Current.ASCIIToByteConverter.DataTransmissionPercenatge = 0;
                percentageTimer.Stop();
            }
        }

        /// <summary>
        /// This property gets the firmware list
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public List<Firmware> FirmareFileList
        {
            get
            {
                int id = 0;

                List<Firmware> firmwareList = new List<Firmware>();
                Firmware[] firmwares = (Firmware[])Enum.GetValues(typeof(Firmware));

                foreach (Firmware element in firmwares)
                {
                    if (element.ToString() != "Unknown")
                    {
                        firmwareList.Add(element);

                    }

                    id++;
                }

                return firmwareList;
            }

        }

        /// <summary>
        /// Get firmware information
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void GetFirmwareInformation()
        {
            int id = 0;

            List<Firmware> firmwareList = new List<Firmware>();
            Firmware[] firmwares = (Firmware[])Enum.GetValues(typeof(Firmware));

            foreach (Firmware element in firmwares)
            {
                if (element.ToString() != "Unknown")
                {
                    firmwareList.Add(element);

                    FirmwareDescription _firmwareDescription = new FirmwareDescription(id, element.ToString());
                    _firmwareDescription.PropertyChanged += _firmwareDescription_PropertyChanged;

                    FirmwareDescriptions.Add(_firmwareDescription);
                }

                id++;
            }
        }

        /// <summary>
        /// This function handles the firmware description property changed event
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="e">The property changed arguments</param>
        private void _firmwareDescription_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            FirmwareDescription selecetdFirmwareDescription = sender as FirmwareDescription;

            if (PreviousSelectedFirmwareDescription != selecetdFirmwareDescription)
            {
                if (PreviousSelectedFirmwareDescription.Id != -1)
                {
                    PreviousSelectedFirmwareDescription.Update = false;
                }
                PreviousSelectedFirmwareDescription = selecetdFirmwareDescription;
            }
        }

        /// <summary>
        /// Convert the bytes to string
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="byteCount">The byte number count</param>
        /// <returns></returns>
        private static string BytesToString(long byteCount)
        {
            var sb = new StringBuilder(32);
            StrFormatKBSize(byteCount, sb, sb.Capacity);
            return sb.ToString();
        }

        /// <summary>
        /// Set file seize
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="byteCount">The byte number count</param>
        private static void SetFileSeize(long byteCount)
        {
            var sb = new StringBuilder(32);
            StrFormatKBSize(byteCount, sb, sb.Capacity);

            string newValue = sb.ToString().Replace("KB", "");

            FileSeize = Convert.ToInt32(newValue);
        }

        /// <summary>
        /// Function that returns if the system can invoke the Read Firmware Version command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command parameter (not used in this function).</param>
        /// <returns>Boolean value if the system can invoke the command.</returns>
        private bool CanReadFirmwareVersionCommand(object arg)
        {
            return true;
        }

        /// <summary>
        /// Function/Command that handles the reading of firmware version when the Read Firmware Version
        /// command is invoked
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command's parameter (not used in this function).</param>
        private void OnReadFirmwareVersionCommand(object arg)
        {
            // Reset the display before requesting the versions.
            ResetTheFirmwareVersions();
            ReadTheFirmwareVersions();
        }

        /// <summary>
        /// Function that returns if the system can invoke the load firmware version command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command parameter (not used in this function).</param>
        /// <returns>Boolean value if the system can invoke the command.</returns>
        private bool CanLoadFirmwareVersionCommand(object arg)
        {
            return true;
        }


        /// <summary>
        /// Function/Command that handles the reading of firmware version when the Read Firmware Version
        /// command is invoked
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command's parameter (not used in this function).</param>
        private void OnLoadFirmwareVersionCommand(object arg)
        {
               CommonViewModel.Current.LogUserAction(Enumeration.Actions.LoadFirmwareVersionCommand);


            // Put the system in safe state 
            CommonViewModel.Current.Console.GUIIsReady = false;
            System.Threading.Thread.Sleep(1000);
            for (int i = 0; i < 3; i++)

            {
                CommonViewModel.Current.RequiredVolume = 0;
                System.Threading.Thread.Sleep(20);
            }

            //Set the maintenance Mode  and Reset
            CommonViewModel.Current.Console.GUIInMaintenanceMode = true;
            CommonViewModel.Current.Console.HeartbeatActivated = false;
            CommonViewModel.Current.ASCIIToByteConverter.ResetPackets();
            TransmittedDataPercentage = 0;
            CommonViewModel.Current.ASCIIToByteConverter.DataTransmissionPercenatge = 0;


            Array.Clear(ModuleKeysdata, 0, 8);

            byte[] fileSeizedata = new byte[8];
            Array.Clear(fileSeizedata, 0, 8);

            Array.Clear(Initdata, 0, 8);

            CommonViewModel.Current.ASCIIToByteConverter.ClearInitData();

            //Deceide the Key according to the user Decision 

            SetFILESTORAGE();

            if (FILESTORAGE == string.Empty)
            {
                Tuple<long, string, string, string> genericMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID95, (int)Enumeration.ErrorTypes.GUI);
                Tuple<long, string, string, string> selectionMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID96, (int)Enumeration.ErrorTypes.GUI);

                MessagePopup MessagePopup = new Views.MessagePopup(genericMessage, Views.MessagePopup.MessageType.WarningMessage, Views.MessagePopup.ButtonType.Ok, selectionMessage.Item2);
                MessagePopup.ShowDialog();

                //MessagePopup messagePopup = new MessagePopup("Select a firmware to Load", MessagePopup.MessageType.WarningMessage, MessagePopup.ButtonType.Ok, "Selection Error");
                //messagePopup.ShowDialog();
                return;
            }


            string[] filePaths = Directory.GetFiles(Path.Combine(GetBasePath(), FILESTORAGE));

            if (filePaths.Length <= 0)
            {
                Tuple<long, string, string, string> genericMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID97, (int)Enumeration.ErrorTypes.GUI);
                Tuple<long, string, string, string> fileMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID98, (int)Enumeration.ErrorTypes.GUI);

                MessagePopup MessagePopup = new Views.MessagePopup(genericMessage, Views.MessagePopup.MessageType.WarningMessage, Views.MessagePopup.ButtonType.Ok, fileMessage.Item2);
                MessagePopup.ShowDialog();

                //MessagePopup messagePopup = new MessagePopup("Hex File Not Found!", MessagePopup.MessageType.WarningMessage, MessagePopup.ButtonType.Ok, "File Error");
                //messagePopup.ShowDialog();
                return;
            }

            CommonViewModel.Current.ASCIIToByteConverter.ReadFile(filePaths[0]);

            long length = new System.IO.FileInfo(filePaths[0]).Length;

            //SetFileSeize(length);


            //Get the file size
            fileSeizedata = BitConverter.GetBytes((long)length);



            Initdata[0] = ModuleKeysdata[0];
            Initdata[1] = ModuleKeysdata[1];
            Initdata[2] = fileSeizedata[0];
            Initdata[3] = fileSeizedata[1];
            Initdata[4] = fileSeizedata[2];
            Initdata[5] = fileSeizedata[3];


            CommonViewModel.Current.ASCIIToByteConverter.Initdata = this.Initdata;

            if (IsRepeaterSelected || IsICBSelected || IsRemoteSelected)
            {
                CommonViewModel.Current.Console.SendBootMessageForICBOrReapeter(MessageStateId.CAN_ID_STATE_IDLE, (int)BootLoaderID.CAN_ID_BOOT_START, ModuleKeysdata);
            }
            else
            {
                CommonViewModel.Current.Console.SendBootMessage(MessageStateId.CAN_ID_STATE_IDLE, (int)BootLoaderID.CAN_ID_BOOT_START, ModuleKeysdata);
            }


            IsLoadingFirmware = true;

            percentageTimer.Start();

            IsBootLoaderUpdatingFirmware = true;

            TimeOutCompter = 0;
        }


        /// <summary>
        /// Function that returns if the system can invoke the app mode command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command parameter (not used in this function).</param>
        /// <returns>Boolean value if the system can invoke the command.</returns>
        private bool CanAppModeCommand(object arg)
        {
            return true;
        }



        /// <summary>
        /// Function/Command that handles the application mode
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command's parameter (not used in this function).</param>
        private void OnAppModeCommand(object arg)
        {

            CommonViewModel.Current.LogUserAction(Enumeration.Actions.AppModeCommand);

            //Set the maintenance Mode 
            //CommonViewModel.Current.Console.GUIInMaintenanceMode = false;
            CommonViewModel.Current.Console.HeartbeatActivated = true;

            CommonViewModel.Current.ASCIIToByteConverter.ResetPackets();


            Array.Clear(ModuleKeysdata, 0, 8);


            ModuleKeysdata = BitConverter.GetBytes((long)ModuleKeys.CMCUREBOOT);

            if (IsRepeaterSelected || IsICBSelected || IsRemoteSelected)
            {
                CommonViewModel.Current.Console.SendBootMessageForICBOrReapeter(MessageStateId.CAN_ID_STATE_IDLE, (int)BootLoaderID.CAN_ID_BOOT_INIT, ModuleKeysdata);
            }
            else
            {

                CommonViewModel.Current.Console.SendBootMessage(MessageStateId.CAN_ID_STATE_IDLE, (int)BootLoaderID.CAN_ID_BOOT_INIT, ModuleKeysdata);
            }

            IsLoadingFirmware = false;

            percentageTimer.Stop();

            
            TransmittedDataPercentage = 0;
            CommonViewModel.Current.ASCIIToByteConverter.DataTransmissionPercenatge = 0;
            percentageTimer.Stop();

            IsFirmwareLoadSelected = false;
            Task.Delay(6000).ContinueWith(t => QuitSafeState());

            TimeOutCompter = 0;

        }


        /// <summary>
        /// Function that returns if the system can import file from USB Command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command parameter (not used in this function).</param>
        /// <returns>Boolean value if the system can invoke the command.</returns>
        private bool CanImportFileFromUSBCommand(object arg)
        {
            return true;
        }



        /// <summary>
        /// Function/Command that handles the import file from USB command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command's parameter (not used in this function).</param>
        private void OnImportFileFromUSBCommand(object arg)
        {

            FirmwareSelector firmwareSelector = new FirmwareSelector(this);


            if ((bool)firmwareSelector.ShowDialog())
            {
                List<FirmwareDescription> firmwareToCopy = new List<FirmwareDescription>();

                firmwareToCopy = FirmwareDescriptions.FindAll(f => f.Update == true);

                foreach (FirmwareDescription element in firmwareToCopy)
                {
                    CopyFile(element.Name);
                }

            }

        }

        /// <summary>
        /// Import file from USB
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="obj">object. Not used</param>
        private void ImportFileFromUSB(object obj)
        {
            List<FirmwareDescription> firmwareToCopy = new List<FirmwareDescription>();

            firmwareToCopy = FirmwareDescriptions.FindAll(f => f.Update == true);

            foreach (FirmwareDescription element in firmwareToCopy)
            {
                CopyFile(element.Name);
            }
        }

        /// <summary>
        /// Copy a file
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="name">Name of the file to copy</param>
        private void CopyFile(string name)
        {
            string destPath = Path.Combine(GetBasePath(), name);
            string sourcePath = "";
            string fileNamePath = "";

            if (USBDriveList.Count > 0)
            {
                //sourcePath = Path.Combine(USBDriveList[0].Name, name);
                sourcePath = USBDriveList[0].Name;
            }

            else
            {
                IsPMCUSelected = false;
                IsCMCUSelected = false;
                IsCPLDSelected = false;
                IsRepeaterSelected = false;
                IsICBSelected = false;
                IsRemoteSelected = false;
                IsFirmwareLoadSelected = false;

                Tuple<long, string, string, string> genericMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID100, (int)Enumeration.ErrorTypes.GUI);

                MessagePopup MessagePopup = new Views.MessagePopup(genericMessage, Views.MessagePopup.MessageType.WarningMessage, Views.MessagePopup.ButtonType.Ok, "USB Error");
                MessagePopup.ShowDialog();



                return;
            }


            try
            {
                if (Directory.Exists(sourcePath))
                {
                    string[] arrayOfFile = Directory.GetFiles(sourcePath);

                    switch (name)
                    {
                        case "CMCU":
                            fileNamePath = Path.Combine(USBDriveList[0].Name, "cMcuMain.hex");
                            break;
                        case "CPLD":
                            fileNamePath = Path.Combine(USBDriveList[0].Name, "cMcuMain.hex");
                            break;
                        case "PMCU":
                            fileNamePath = Path.Combine(USBDriveList[0].Name, "pMcuMain.hex");
                            break;
                        case "RMCU":
                            fileNamePath = Path.Combine(USBDriveList[0].Name, "rMcuMain.hex");
                            break;
                        case "BMCU":
                            fileNamePath = Path.Combine(USBDriveList[0].Name, "bMcuMain.hex");
                            break;
                        case "RCMCU":
                            fileNamePath = Path.Combine(USBDriveList[0].Name, "rcMcuMain.hex");
                            break;
                    }

                    if (arrayOfFile.Length > 0)
                    {
                        bool fileFound = false;
                        foreach (string files in arrayOfFile)
                        {
                            if(files == fileNamePath)
                            {
                                // First Delete the Old file
                                string[] filePaths = Directory.GetFiles(destPath);
                                foreach (string filePath in filePaths)
                                    File.Delete(filePath);

                                FileInfo fileInfo = new FileInfo(files);
                                fileInfo.CopyTo(string.Format(@"{0}\{1}", destPath, fileInfo.Name), true);
                                fileFound = true;
                            }   
                        }
                        if (!fileFound)
                        {
                            IsPMCUSelected = false;
                            IsCMCUSelected = false;
                            IsCPLDSelected = false;
                            IsRepeaterSelected = false;
                            IsICBSelected = false;
                            IsRemoteSelected = false;
                            IsFirmwareLoadSelected = false;

                            Tuple<long, string, string, string> genericMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID97, (int)Enumeration.ErrorTypes.GUI);
                            Tuple<long, string, string, string> fileMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID98, (int)Enumeration.ErrorTypes.GUI);

                            MessagePopup MessagePopup = new Views.MessagePopup(genericMessage, Views.MessagePopup.MessageType.WarningMessage, Views.MessagePopup.ButtonType.Ok, fileMessage.Item2);
                            MessagePopup.ShowDialog();
                        }
                    }
                    else
                    {
                        IsPMCUSelected = false;
                        IsCMCUSelected = false;
                        IsCPLDSelected = false;
                        IsRepeaterSelected = false;
                        IsICBSelected = false;
                        IsRemoteSelected = false;
                        IsFirmwareLoadSelected = false;

                        Tuple<long, string, string, string> genericMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID97, (int)Enumeration.ErrorTypes.GUI);
                        Tuple<long, string, string, string> fileMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID98, (int)Enumeration.ErrorTypes.GUI);

                        MessagePopup MessagePopup = new Views.MessagePopup(genericMessage, Views.MessagePopup.MessageType.WarningMessage, Views.MessagePopup.ButtonType.Ok, fileMessage.Item2);
                        MessagePopup.ShowDialog();


                    }


                }
                else
                {
                    IsPMCUSelected = false;
                    IsCMCUSelected = false;
                    IsCPLDSelected = false;
                    IsRepeaterSelected = false;
                    IsICBSelected = false;
                    IsRemoteSelected = false;
                    IsFirmwareLoadSelected = false;

                    Tuple<long, string, string, string> genericMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID99, (int)Enumeration.ErrorTypes.GUI);
                    Tuple<long, string, string, string> fileMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID98, (int)Enumeration.ErrorTypes.GUI);

                    MessagePopup MessagePopup = new Views.MessagePopup(genericMessage, Views.MessagePopup.MessageType.WarningMessage, Views.MessagePopup.ButtonType.Ok, fileMessage.Item2);
                    MessagePopup.ShowDialog();

                 }

            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// Set file storage
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void SetFILESTORAGE()
        {
            if (IsCPLDSelected)
            {
                FILESTORAGE = "CPLD\\";
                CommonViewModel.Current.ASCIIToByteConverter.BoardType = FirmwareBootLoader.Helpers.Definitions.Board.CPLD;
                ModuleKeysdata = BitConverter.GetBytes((long)ModuleKeys.CMCUKey);  //When we are programming CPLD we first CMCU key
            }

            else if (IsPMCUSelected)
            {
                FILESTORAGE = "PMCU\\";
                CommonViewModel.Current.ASCIIToByteConverter.BoardType = FirmwareBootLoader.Helpers.Definitions.Board.PMCU;
                ModuleKeysdata = BitConverter.GetBytes((long)ModuleKeys.PMCUKey);
            }

            else if (IsRepeaterSelected)
            {
                FILESTORAGE = "RMCU\\";
                CommonViewModel.Current.ASCIIToByteConverter.BoardType = FirmwareBootLoader.Helpers.Definitions.Board.Repeater;
                ModuleKeysdata = BitConverter.GetBytes((long)ModuleKeys.RMCUKey);
            }

            else if (IsICBSelected)
            {
                FILESTORAGE = "BMCU\\";
                CommonViewModel.Current.ASCIIToByteConverter.BoardType = FirmwareBootLoader.Helpers.Definitions.Board.ICB;
                ModuleKeysdata = BitConverter.GetBytes((long)ModuleKeys.BMCUKey);
            }

            else if (IsRemoteSelected)
            {
                FILESTORAGE = "RCMCU\\";
                CommonViewModel.Current.ASCIIToByteConverter.BoardType = FirmwareBootLoader.Helpers.Definitions.Board.Remote;
                ModuleKeysdata = BitConverter.GetBytes((long)ModuleKeys.RCMCUKey);
            }

            else if (IsCatheterSelected)
            {
                FILESTORAGE = "Catheter\\";
                CommonViewModel.Current.ASCIIToByteConverter.BoardType = FirmwareBootLoader.Helpers.Definitions.Board.Catheter;
                // ModuleKeysdata = BitConverter.GetBytes((long)ModuleKeys.CMCUKey); TODO
            }

            else if (IsCMCUSelected)
            {
                FILESTORAGE = "CMCU\\";
                CommonViewModel.Current.ASCIIToByteConverter.BoardType = FirmwareBootLoader.Helpers.Definitions.Board.CMCU;
                ModuleKeysdata = BitConverter.GetBytes((long)ModuleKeys.CMCUKey);
            }


        }

        /// <summary>
        /// This property gets/sets the Central Micro Controller (CMC) firmware version
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int CentralMicroControllerFirmwareVersion
        {
            get
            {
                return CommonViewModel.Current.CentralMicroControllerFirmwareVersion;
            }

            set
            {
                try
                {
                    CommonViewModel.Current.CentralMicroControllerFirmwareVersion = value;
                    RaisePropertyChanged("CentralMicroControllerFirmwareVersion");
                }
                catch { }
            }
        }

        /// <summary>
        /// This property gets/sets the CPLD firmware version
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int CpldFirmwareVersion
        {
            get
            {
                return CommonViewModel.Current.CpldFirmwareVersion;
            }

            set
            {
                try
                {
                    CommonViewModel.Current.CpldFirmwareVersion = value;
                    RaisePropertyChanged("CpldFirmwareVersion");
                }
                catch { }
            }
        }

        /// <summary>
        /// This property gets/sets the Patient Micro Controller (PMC) firmware version
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int PatientMicroControllerFirmwareVersion
        {
            get
            {
                return CommonViewModel.Current.PatientMicroControllerFirmwareVersion;
            }

            set
            {
                try
                {
                    CommonViewModel.Current.PatientMicroControllerFirmwareVersion = value;
                    RaisePropertyChanged("PatientMicroControllerFirmwareVersion");
                }
                catch { }
            }
        }

        /// <summary>
        /// This property gets/sets the Repeater firmware value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int RepeaterFirmware
        {
            get
            {
                return CommonViewModel.Current.RepeaterFirmware;
            }

            set
            {
                try
                {
                    CommonViewModel.Current.RepeaterFirmware = value;
                    RaisePropertyChanged("RepeaterFirmware");
                }
                catch { }
            }
        }

        /// <summary>
        /// This property gets/sets the ICB firmware value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int ICBFirmware
        {
            get
            {
                return CommonViewModel.Current.ICBFirmware;
            }

            set
            {
                try
                {
                    CommonViewModel.Current.ICBFirmware = value;
                    RaisePropertyChanged("ICBFirmware");
                }
                catch { }
            }
        }

        /// <summary>
        /// This property gets/sets the remote control firmware value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>

        public int RemoteControlFirmware
        {
            get
            {
                return CommonViewModel.Current.RemoteControlFirmware;
            }

            set
            {
                try
                {
                    CommonViewModel.Current.RemoteControlFirmware = value;
                    RaisePropertyChanged("RemoteControlFirmware");
                }
                catch { }
            }
        }

        /// <summary>
        /// This property gets/sets the Catheter firmware version
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int CatheterFirmwareVersion
        {
            get
            {
                return CommonViewModel.Current.CatheterFirmwareVersion;
            }

            set
            {
                try
                {
                    CommonViewModel.Current.CatheterFirmwareVersion = value;
                    RaisePropertyChanged("CatheterFirmwareVersion");
                }
                catch { }
            }
        }

        #region  Boot loader Versions

        /// <summary>
        /// Gets or sets  the central microController bootLoader firmware version
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int CentralMicroControllerBootLoaderFirmwareVersion
        {
            get
            {
                return CommonViewModel.Current.CentralMicroControllerBootLoaderFirmwareVersion;
            }

            set
            {
                try
                {
                    CommonViewModel.Current.CentralMicroControllerBootLoaderFirmwareVersion = value;
                    RaisePropertyChanged("CentralMicroControllerBootLoaderFirmwareVersion");
                }
                catch { }
            }
        }

        /// <summary>
        /// Gets or sets  the CPLD bootLoader firmware version
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int CpldBootLoaderFirmwareVersion
        {
            get
            {
                return CommonViewModel.Current.CpldBootLoaderFirmwareVersion;
            }

            set
            {
                try
                {
                    CommonViewModel.Current.CpldBootLoaderFirmwareVersion = value;
                    RaisePropertyChanged("CpldBootLoaderFirmwareVersion");
                }
                catch { }
            }
        }

        /// <summary>
        /// Gets or sets  the repeater bootLoader firmware version
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int RepeaterBootLoaderFirmware
        {
            get
            {
                return CommonViewModel.Current.RepeaterBootLoaderFirmware;
            }

            set
            {
                try
                {
                    CommonViewModel.Current.RepeaterBootLoaderFirmware = value;
                    RaisePropertyChanged("RepeaterBootLoaderFirmware");
                }
                catch { }
            }
        }

        /// <summary>
        /// Gets or sets  the patient bootLoader firmware version
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int PatientMicroControllerBootLoaderFirmwareVersion
        {
            get
            {
                return CommonViewModel.Current.PatientMicroControllerBootLoaderFirmwareVersion;
            }

            set
            {
                try
                {
                    CommonViewModel.Current.PatientMicroControllerBootLoaderFirmwareVersion = value;
                    RaisePropertyChanged("PatientMicroControllerBootLoaderFirmwareVersion");
                }
                catch { }
            }
        }

        /// <summary>
        /// This property gets/sets the remote control boot loader firmware value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>

        public int RemoteControlBootLoaderFirmwareVersion
        {
            get
            {
                return CommonViewModel.Current.RemoteControlBootLoaderFirmwareVersion;
            }

            set
            {
                try
                {
                    CommonViewModel.Current.RemoteControlBootLoaderFirmwareVersion = value;
                    RaisePropertyChanged("RemoteControlBootLoaderFirmwareVersion");
                }
                catch { }
            }
        }

        /// <summary>
        /// This property gets/sets the ICB boot loader firmware value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int ICBBootLoaderFirmwareVersion
        {
            get
            {
                return CommonViewModel.Current.ICBBootLoaderFirmwareVersion;
            }

            set
            {
                try
                {
                    CommonViewModel.Current.ICBBootLoaderFirmwareVersion = value;
                    RaisePropertyChanged("ICBBootLoaderFirmwareVersion");
                }
                catch { }
            }
        }
        #endregion

        /// <summary>
        /// This read-only property returns the console's Minutes of Use
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
        /// Gets or sets  a value indicating whether the boot loader updating firmware
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsBootLoaderUpdatingFirmware
        {
            get
            {
                return CommonViewModel.Current.IsBootLoaderUpdatingFirmware;
            }
            set
            {
                CommonViewModel.Current.IsBootLoaderUpdatingFirmware = value;
                RaisePropertyChanged("IsBootLoaderUpdatingFirmware");
            }
        }

        /// <summary>
        /// This read-only property returns the current Date and Time
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public DateTime CurrentDateTime
        {
            get { return DateTime.Now; }
        }

        /// <summary>
        /// This read-only property returns the system's amount of RAM
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string RAM
        {
            get
            {
                // Fetches total physical memory in bytes
                double totalPhysicalMemory = (double)new ComputerInfo().TotalPhysicalMemory;
                // Factor to convert from bytes to gigabytes
                double factor = Math.Pow(1024, 3);
                // Offset to include inaccessible memory
                double offset = 0.5;
                // Unit label
                string unit = " GB";

                return ((totalPhysicalMemory / factor) + offset).ToString("0") + unit;
            }
        }

        /// <summary>
        /// Get the Drive information in Gigabyte
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string OSDisk
        {
            get
            {
                return DrivesInformation.HardDriveTotalSpace + " GB";
            }
        }

        /// <summary>
        /// Get the estimated remaining procedure.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int EstimatedRemainingProcedure
        {
            get
            {
                return procedureSpaceModel.RemainingProcedure;
            }
        }

        /// <summary>
        /// This read-only property returns the system's MAC Address
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        //public string MACAddress
        //{
        //    get
        //    {
        //        ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
        //        ManagementObjectCollection moc = mc.GetInstances();
        //        string MACAddress = String.Empty;
        //        foreach (ManagementObject mo in moc)
        //        {
        //            if (MACAddress == String.Empty)
        //            {
        //                if ((bool)mo["IPEnabled"] == true) MACAddress = mo["MacAddress"].ToString();
        //            }
        //            mo.Dispose();
        //        }

        //        MACAddress = MACAddress.Replace(":", "");
        //        return MACAddress;
        //    }
        //}

        /// <summary>
        /// This read-only property returns the system's board maker.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string BoardMaker
        {
            get
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT Manufacturer FROM Win32_BaseBoard");
                foreach (ManagementObject wmi in searcher.Get())
                {
                    try
                    {
                        return wmi.GetPropertyValue("Manufacturer").ToString();
                    }
                    catch (Exception e)
                    {
                        return "Board Maker: Unknown";
                    }
                }
                return "Board Maker: Unknown";
            }
        }

        /// <summary>
        /// This read-only property returns the system's Board Product ID.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string BoardProductId
        {
            get
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT Product FROM Win32_BaseBoard");

                foreach (ManagementObject wmi in searcher.Get())
                {
                    try
                    {
                        return wmi.GetPropertyValue("Product").ToString();
                    }
                    catch { }
                }
                return "Product: Unknown";
            }
        }

        /// <summary>
        /// This read-only property returns the system's BIOS maker
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string GetBIOSmaker
        {
            get
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT Manufacturer FROM Win32_BIOS");

                foreach (ManagementObject wmi in searcher.Get())
                {
                    try
                    {
                        return wmi.GetPropertyValue("Manufacturer").ToString();
                    }
                    catch { }
                }
                return "BIOS Maker: Unknown";
            }
        }

        /// <summary>
        /// this read-only property returns the system's CPU Manufacturer
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string CPUManufacturer
        {
            get
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT Manufacturer FROM Win32_Processor");

                foreach (ManagementObject wmi in searcher.Get())
                {
                    try
                    {
                        return wmi.GetPropertyValue("Manufacturer").ToString();
                    }
                    catch { }
                }
                return "CPU Manufacturer: Unknown";
            }
        }

        /// <summary>
        /// This read-only property returns the system's CPU Speed in GHz
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string CpuSpeedInGHz
        {
            get
            {
                double GHz;
                string _GHz = string.Empty;

                ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT CurrentClockSpeed FROM Win32_Processor");

                foreach (ManagementObject wmi in searcher.Get())
                {
                    try
                    {
                        GHz = 0.001 * (UInt32)wmi.Properties["CurrentClockSpeed"].Value;
                        _GHz = String.Format("{0:0.00}", GHz);
                        break;
                    }
                    catch { }
                }
                return _GHz + " GHz";
            }
        }

        /// <summary>
        /// This read-only property returns the system's BIOS Version
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string BIOSVersion
        {
            get
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT BIOSVersion FROM Win32_BIOS");

                foreach (ManagementObject wmi in searcher.Get())
                {
                    try
                    {
                        if (((string[])wmi["BIOSVersion"]).Length > 1)
                            return ((string[])wmi["BIOSVersion"])[0] + " - " + ((string[])wmi["BIOSVersion"])[1];
                        else
                            return ((string[])wmi["BIOSVersion"])[0];
                    }
                    catch { }
                }
                return "Unknown";
            }
        }

        /// <summary>
        /// This read-only property returns the OS name
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string OSName
        {
            get
            {
                return new ComputerInfo().OSFullName;
            }
        }

        /// <summary>
        // This read-only property returns the OS version
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string OSVersion
        {
            get
            {
                return new ComputerInfo().OSVersion;
            }
        }

        /// <summary>
        /// This read-only property returns the database version
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int DatabaseVersion
        {
            get
            {
                return CommonViewModel.Current.DatabaseVersion;
            }
        }

        /// <summary>
        /// Gets or sets  a value indicating whether the CPLD update is selected
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsCPLDSelected
        {
            get
            {
                return isCPLDSelected;
            }
            set
            {
                isCPLDSelected = value;
                RaisePropertyChanged("IsCPLDSelected");

                if (value)
                {

                    IsPMCUSelected = false;
                    IsRepeaterSelected = false;
                    IsICBSelected = false;
                    IsCatheterSelected = false;
                    IsCMCUSelected = false;
                    IsRemoteSelected = false;

                    IsFirmwareLoadSelected = true;

                    FirmwareDescriptions[cPLDIndex].Update = true;
                    ImportFileFromUSB(FirmwareDescriptions[cPLDIndex]);
                }

            }
        }

        /// <summary>
        /// Gets or sets  a value indicating whether the PMCU update is selected
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsPMCUSelected
        {
            get
            {
                return isPMCUSelected;
            }
            set
            {
                isPMCUSelected = value;
                RaisePropertyChanged("IsPMCUSelected");

                if (value)
                {
                    IsCPLDSelected = false;
                    IsRepeaterSelected = false;
                    IsICBSelected = false;
                    IsCatheterSelected = false;
                    IsCMCUSelected = false;
                    IsRemoteSelected = false;

                    IsFirmwareLoadSelected = true;

                    FirmwareDescriptions[pMCUIndex].Update = true;
                    ImportFileFromUSB(FirmwareDescriptions[pMCUIndex]);
                }
            }
        }

        /// <summary>
        /// Gets or sets  a value indicating whether the repeater update is selected
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsRepeaterSelected
        {
            get
            {
                return isRepeaterSelected;
            }
            set
            {
                isRepeaterSelected = value;

                RaisePropertyChanged("IsRepeaterSelected");

                if (value)
                {
                    IsCPLDSelected = false;
                    IsPMCUSelected = false;
                    IsICBSelected = false;
                    IsCatheterSelected = false;
                    IsCMCUSelected = false;
                    IsRemoteSelected = false;

                    IsFirmwareLoadSelected = true;

                    FirmwareDescriptions[repeaterIndex].Update = true;
                    ImportFileFromUSB(FirmwareDescriptions[repeaterIndex]);
                }
            }
        }

        /// <summary>
        /// Gets or sets  a value indicating whether the ICB update is selected
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsICBSelected
        {
            get
            {
                return isICBSelected;
            }
            set
            {
                isICBSelected = value;
                RaisePropertyChanged("IsICBSelected");

                if (value)
                {
                    IsCPLDSelected = false;
                    IsPMCUSelected = false;
                    IsRepeaterSelected = false;
                    IsCatheterSelected = false;
                    IsCMCUSelected = false;
                    IsRemoteSelected = false;

                    IsFirmwareLoadSelected = true;

                    FirmwareDescriptions[icbIndex].Update = true;
                    ImportFileFromUSB(FirmwareDescriptions[icbIndex]);
                }
            }
        }

        /// <summary>
        /// Gets or sets  a value indicating whether the catheter update is selected
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsCatheterSelected
        {
            get
            {
                return isCatheterSelected;
            }
            set
            {
                isCatheterSelected = value;
                RaisePropertyChanged("IsCatheterSelected");

                if (value)
                {
                    IsCPLDSelected = false;
                    IsPMCUSelected = false;
                    IsRepeaterSelected = false;
                    IsICBSelected = false;
                    IsCMCUSelected = false;
                    IsRemoteSelected = false;
                }
            }
        }

        /// <summary>
        /// Gets or sets  a value indicating whether the CMCU update is selected
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsCMCUSelected
        {
            get
            {
                return isCMCUSelected;
            }
            set
            {
                isCMCUSelected = value;
                RaisePropertyChanged("IsCMCUSelected");

                if (value)
                {
                    IsCPLDSelected = false;
                    IsPMCUSelected = false;
                    IsRepeaterSelected = false;
                    IsICBSelected = false;
                    IsCatheterSelected = false;
                    IsRemoteSelected = false;

                    IsFirmwareLoadSelected = true;

                    FirmwareDescriptions[cMCUIndex].Update = true;
                    ImportFileFromUSB(FirmwareDescriptions[cMCUIndex]);

                }
            }
        }

        // <summary>
        /// Gets or sets  a value indicating whether the RCMCU update is selected
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsRemoteSelected
        {
            get
            {
                return isRemoteSelected;
            }
            set
            {
                isRemoteSelected = value;
                RaisePropertyChanged("IsRemoteSelected");

                if (value)
                {
                    IsCPLDSelected = false;
                    IsPMCUSelected = false;
                    IsRepeaterSelected = false;
                    IsICBSelected = false;
                    IsCatheterSelected = false;
                    IsCMCUSelected = false;

                    IsFirmwareLoadSelected = true;

                    FirmwareDescriptions[remoteIndex].Update = true;
                    ImportFileFromUSB(FirmwareDescriptions[remoteIndex]);
                }
            }
        }

        /// <summary>
        /// Gets or sets  a value indicating whether the software is loading the firmware
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsLoadingFirmware
        {
            get
            {
                return isLoadingFirmware;
            }

            set
            {
                isLoadingFirmware = value;
                RaisePropertyChanged("IsLoadingFirmware");
            }
        }

        /// <summary>
        /// Gets or sets  a value indicating the firmware module key
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double ModuleKey
        {
            get
            {
                return CommonViewModel.Current.ModuleKey;
            }
            set
            {
                CommonViewModel.Current.ModuleKey = value;
                RaisePropertyChanged("ModuleKey");
            }
        }

        /// <summary>
        /// Gets or sets  a value indicating the upgrade status
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double UpgradeStatus
        {
            get
            {
                return CommonViewModel.Current.UpgradeStatus;
            }
            set
            {
                CommonViewModel.Current.UpgradeStatus = value;
                RaisePropertyChanged("UpgradeStatus");
            }
        }

        /// <summary>
        /// This resets the firmware versions
        /// . Safety clsassification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public void ResetTheFirmwareVersions()
        {
            try
            {
                CentralMicroControllerFirmwareVersion = 0;
                CentralMicroControllerBootLoaderFirmwareVersion = 0;
                CpldFirmwareVersion = 0;
                PatientMicroControllerFirmwareVersion = 0;
                PatientMicroControllerBootLoaderFirmwareVersion = 0;
                RepeaterFirmware = 0;
                RepeaterBootLoaderFirmware = 0;
                CatheterFirmwareVersion = 0;
                ICBFirmware = 0;
                ICBBootLoaderFirmwareVersion = 0;
                RemoteControlFirmware = 0;
                RemoteControlBootLoaderFirmwareVersion = 0;
            }

            catch (Exception ex)
            {
                // TODO
                ex.ToString();

            }
        }

        /// <summary>
        /// This reads the firm ware version
        /// . Safety clsassification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public void ReadTheFirmwareVersions()
        {
            for (int i = 0; i < numberOfRetry; i++)
            {
                try
                {
                    CommonViewModel.Current.Console.ReadFromMicroController(MessageStateId.CAN_ID_STATE_IDLE, CentralMicroControllerFirmwareVersionId);
                    System.Threading.Thread.Sleep(10);

                    CommonViewModel.Current.Console.ReadFromMicroController(MessageStateId.CAN_ID_STATE_IDLE, PatientMicroControllerFirmwareVersionId);
                    System.Threading.Thread.Sleep(10);

                    CommonViewModel.Current.Console.ReadFromMicroController(MessageStateId.CAN_ID_STATE_IDLE, CatheterFirmwareVersionId);
                    System.Threading.Thread.Sleep(10);

                    CommonViewModel.Current.Console.ReadFromMicroControllerOnCanTwo(MessageStateId.CAN_ID_STATE_IDLE, RepeaterFirmwareAndICBFirmwareId);
                    System.Threading.Thread.Sleep(10);

                    CommonViewModel.Current.Console.ReadFromMicroControllerOnCanTwo(MessageStateId.CAN_ID_STATE_IDLE, RemoteFirmwareId);
                    System.Threading.Thread.Sleep(10);
                }

                catch (Exception ex)
                {
                    // TODO
                    ex.ToString();

                }
            }
        }

        /// <summary>
        /// Gets file base path
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private string GetBasePath()
        {
            string thePath = "";

            String path = AppDomain.CurrentDomain.BaseDirectory;
            String[] extract = Regex.Split(path, "bin");  //split it in bin
            thePath = extract[0];

            return thePath;
        }

        /// <summary>
        /// Gets or sets  the USB drive list
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
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
            }
        }

        /// <summary>
        /// Gets or sets  a value indicating whether the USB drive connected
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public Boolean USBDriveConnected
        {
            get
            {
                bool _uSBDriveConnected = USBDriveList != null && USBDriveList.Count != 0;

                if (!_uSBDriveConnected)
                {
                    IsPMCUSelected = false;
                    IsCMCUSelected = false;
                    IsCPLDSelected = false;
                    IsRepeaterSelected = false;
                    IsICBSelected = false;
                    IsRemoteSelected = false;
                    IsFirmwareLoadSelected = false;
                }


                if (IsLoadingFirmware && !_uSBDriveConnected)
                {
                    OnAppModeCommand(null);
                }

                return _uSBDriveConnected;
            }
        }

        /// <summary>
        /// Gets or sets  a value indicating the firmware description
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        internal FirmwareDescription FirmwareDescription
        {
            get => firmwareDescription;
            set => firmwareDescription = value;
        }

        /// <summary>
        /// Gets or sets  a value indicating the list firmware description
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public List<FirmwareDescription> FirmwareDescriptions
        {
            get
            {
                return firmwareDescriptions;
            }
            set
            {
                firmwareDescriptions = value;

            }
        }

        /// <summary>
        /// Gets or sets  a value indicating the transmitted data percentage
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int TransmittedDataPercentage
        {
            get
            {
                return transmittedDataPercentage;
            }
            set
            {
                if (value > 100)
                    value = 100;
                transmittedDataPercentage = value;
                RaisePropertyChanged("TransmittedDataPercentage");
            }

        }

        /// <summary>
        /// Gets or sets  an array of module keys data
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public byte[] ModuleKeysdata
        {
            get => moduleKeysdata;
            set => moduleKeysdata = value;
        }

        /// <summary>
        /// Gets or sets  a value indicating the time out compter.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int TimeOutCompter
        {
            get => timeOutCompter;
            set => timeOutCompter = value;
        }

        /// <summary>
        /// Gets or sets  a value indicating the previous selected firmware description
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public FirmwareDescription PreviousSelectedFirmwareDescription
        {
            get => previousSelectedFirmwareDescription;
            set => previousSelectedFirmwareDescription = value;
        }
        /// <summary>
        /// Gets or sets  a value indicating the previous transmitted data percentage
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int PreviousTransmittedDataPercentage
        {
            get => previousTransmittedDataPercentage;
            set => previousTransmittedDataPercentage = value;
        }

        /// <summary>
        /// Gets or sets  a value indicating whether a firmware is selected
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsFirmwareLoadSelected
        {
            get
            {
              return isFirmwareLoadSelected;
            }
            set
            {
                isFirmwareLoadSelected = value;
                RaisePropertyChanged("IsFirmwareLoadSelected");
            }
        }

        /// <summary>
        /// Gets or sets  a value indicating whether we using the system files
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsUsingSystemFile
        {
            get => isUsingSystemFile;
            set => isUsingSystemFile = value;
        }
        public ConsoleVersion ConsoleVersion { get => consoleVersion; set => consoleVersion = value; }

        /// <summary>
        /// This function handles the USB connection PropertyChanged event
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">USB sender.</param>
        /// <param name="e">The USB event.</param>
        private void USBDriveConnection_EventArrived(object sender, EventArrivedEventArgs e)
        {
            try
            {
                USBDriveList = usbDriveConnectionManager.GetUSBDriveList();

            }
            catch (Exception ex)
            {
                // TODO
                ex.ToString();

                Tuple<long, string, string, string> genericMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID41, (int)Enumeration.ErrorTypes.GUI);
                Tuple<long, string, string, string> titleMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID42, (int)Enumeration.ErrorTypes.GUI);

                MessagePopup messagePopup = new MessagePopup(genericMessage, MessagePopup.MessageType.WarningMessage, MessagePopup.ButtonType.Ok, titleMessage.Item2);
            }
        }


        /// <summary>
        /// This function validate the firmware upgrade
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void ValidateFirmwareUpgrade()
        {
            if (IsCPLDSelected)
            {
                if (CommonViewModel.Current.UpgradeStatus == (double)CPLDStatusKey.CMCUPASS)
                {
                    CommonViewModel.Current.ASCIIToByteConverter.BoardType = FirmwareBootLoader.Helpers.Definitions.Board.CPLD;
                    ModuleKeysdata = BitConverter.GetBytes((long)ModuleKeys.CPLDKey);

                    Array.Clear(Initdata, 0, 8);

                    Initdata[0] = ModuleKeysdata[0];
                    Initdata[1] = ModuleKeysdata[1];
                    CommonViewModel.Current.Console.SendBootMessage(MessageStateId.CAN_ID_STATE_IDLE, (int)BootLoaderID.CAN_ID_BOOT_START, ModuleKeysdata);
                }

                else if (CommonViewModel.Current.UpgradeStatus == (double)CPLDStatusKey.CMCUANDCPLDPASS)
                {
                    IsLoadingFirmware = false;
                    TransmittedDataPercentage = 0;

                    CommonViewModel.Current.ASCIIToByteConverter.DataTransmissionPercenatge = 0;
                    percentageTimer.Stop();

                    IsFirmwareLoadSelected = false;
                    Task.Delay(6000).ContinueWith(t => QuitSafeState(true));
                   
                }

             
            }

            if (IsCMCUSelected || IsPMCUSelected || IsRepeaterSelected || IsICBSelected || IsRemoteSelected)
            {
                if (CommonViewModel.Current.UpgradeStatus == (double)CPLDStatusKey.CMCUPASS)
                {
                    IsLoadingFirmware = false;
                    TransmittedDataPercentage = 0;

                    CommonViewModel.Current.ASCIIToByteConverter.DataTransmissionPercenatge = 0;
                    percentageTimer.Stop();

                    IsFirmwareLoadSelected = false;
                    Task.Delay(6000).ContinueWith(t => QuitSafeState(true));

                }

            }

            if (CommonViewModel.Current.UpgradeStatus == (double)CPLDStatusKey.CMCUFAIL ||
                CommonViewModel.Current.UpgradeStatus == (double)CPLDStatusKey.INTERMEDAIREITERMEDIARYFAIL ||
                CommonViewModel.Current.UpgradeStatus == (double)CPLDStatusKey.CMCUANDCPLDFAIL)
            {
                IsLoadingFirmware = false;
                TransmittedDataPercentage = 0;

                CommonViewModel.Current.ASCIIToByteConverter.DataTransmissionPercenatge = 0;
                percentageTimer.Stop();

                IsFirmwareLoadSelected = false;

                Task.Delay(6000).ContinueWith(t => QuitSafeState(false));
                
            }


 
        }

        /// <summary>
        /// Display boot loader messages
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="message">message to display</param>
        /// <param name="messageType">message type</param>
        /// <param name="buttonType">button type</param>
        /// <param name="titleMessage">title message</param>
        internal void DisplayErrorMessage(String message, MessagePopup.MessageType messageType, MessagePopup.ButtonType buttonType, string titleMessage)
        {

            Application.Current.Dispatcher.Invoke((System.Action)delegate
            {

                MessagePopup messagePopup = new MessagePopup(message, messageType, buttonType, titleMessage);
                messagePopup.ShowDialog();
            });

        }

        /// <summary>
        /// Quit safe state and allow the user to update the firmware
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public void QuitSafeState()
        {
 
            IsFirmwareLoadSelected = true;
            IsBootLoaderUpdatingFirmware = false;
        }

        /// <summary>
        /// Quit safe state and allow the user to update the firmware
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="isBootSuccess">Update status</param>
        public void QuitSafeState(bool isBootSuccess)
        {

            if (isBootSuccess)
            {
                DisplayErrorMessage("Firmware update successful!", MessagePopup.MessageType.SystemMessage, MessagePopup.ButtonType.Ok, "Firmware update");
            }

            else
            {
                DisplayErrorMessage("Firmware update failed. Please try again. If the problem persists, reset the system and try again.", MessagePopup.MessageType.WarningMessage, MessagePopup.ButtonType.Ok, "Update Error");
            }

            IsFirmwareLoadSelected = true;
            IsBootLoaderUpdatingFirmware = false;
        }


        /// <summary>
        /// This function handles the sender's PropertyChanged event
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">The View Model that sent the event.</param>
        /// <param name="e">The parameter's name that has changed.</param>
        private void Current_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            CommonViewModel commonviewmodel = sender as CommonViewModel;

            if (IsUsingSystemFile)
            {
                switch (e.PropertyName)
                {
                    case "MinutesOfUse":
                        RaisePropertyChanged("MinutesOfUse");
                        break;

                    case "RepeaterFirmware":
                        RaisePropertyChanged("RepeaterFirmware");
                        break;

                    case "ICBFirmware":
                        RaisePropertyChanged("ICBFirmware");
                        break;
                    case "RemoteControlFirmware":
                        RaisePropertyChanged("RemoteControlFirmware");
                        break;

                    case "PatientMicroControllerFirmwareVersion":
                        RaisePropertyChanged("PatientMicroControllerFirmwareVersion");
                        break;

                    case "PatientMicroControllerBootLoaderFirmwareVersion":
                        RaisePropertyChanged("PatientMicroControllerBootLoaderFirmwareVersion");
                        break;

                    case "RepeaterBootLoaderFirmware":
                        RaisePropertyChanged("RepeaterBootLoaderFirmware");
                        break;

                    case "CpldFirmwareVersion":
                        RaisePropertyChanged("CpldFirmwareVersion");
                        break;

                    case "CentralMicroControllerFirmwareVersion":
                        RaisePropertyChanged("CentralMicroControllerFirmwareVersion");
                        break;

                    case "CentralMicroControllerBootLoaderFirmwareVersion":
                        RaisePropertyChanged("CentralMicroControllerBootLoaderFirmwareVersion");
                        break;

                    case "CatheterFirmwareVersion":
                        RaisePropertyChanged("CatheterFirmwareVersion");
                        break;

                    case "DatabaseVersion":
                        RaisePropertyChanged("DatabaseVersion");
                        break;

                    case "ModuleKey":
                        //RaisePropertyChanged("ModuleKey");
                        break;

                    case "UpgradeStatus":
                        //RaisePropertyChanged("UpgradeStatus");
                        ValidateFirmwareUpgrade();
                        break;

                    case "RemoteControlBootLoaderFirmwareVersion":
                        RaisePropertyChanged("RemoteControlBootLoaderFirmwareVersion");
                        break;

                    case "ICBBootLoaderFirmwareVersion":
                        RaisePropertyChanged("ICBBootLoaderFirmwareVersion");
                        break;
                }
            }
        }
    }
}