using System;
using BootLoader;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Windows;
using Console;
using System.Windows.Threading;
using System.Threading;
using Communication;
using Ionic.Zip;
//using Ionic.Zip;
//using SystemUpdate.DBAction;
//using UniversalLoginManager;
//using SmartAblationSystem.Helpers;
using static Communication.CanBusMessageDefinition;

namespace SystemUpdate
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string PCUpdatesPathZip = @"C:\UpdateFiles";
        protected void RaisePropertyChanged(string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private USBDriveConnectionManager.USBDriveConnectionManager usbDriveConnectionManager;

        public List<DriveInfo> USBDriveList { get; set; }

        public string _currentProcess = "Starting...";
        public string CurrentProcess
        {
            get
            {
                return _currentProcess;
            }
            set
            {
                _currentProcess = value;
                RaisePropertyChanged("CurrentProcess");
            }
        }
        public int _maxStep;
        public int MaxStep
        {
            get
            {
                return _maxStep;
            }
            set
            {
                _maxStep = value;
                RaisePropertyChanged("MaxStep");
            }
        }

        public int _currentStep;
        public int CurrentStep
        {
            get
            {
                return _currentStep;
            }
            set
            {
                if (value <= MaxStep)
                {
                    _currentStep = value;
                }
                RaisePropertyChanged("CurrentStep");
            }
        }
        byte[] Initdata = new byte[8];
        byte[] moduleKeysdata = new byte[8];
        public byte[] ModuleKeysdata
        {
            get => moduleKeysdata;
            set => moduleKeysdata = value;
        }
        public bool IsUpdatingFirmware { get; private set; }
        public bool updateDone = false;
        private string FILESTORAGE = string.Empty;
        Machine Console;
        ASCIIToByteConverter Converter;
        DispatcherTimer percentageTimer;
        uint packetNumber = 0;
        string path = string.Empty;
        string consoleID = string.Empty;
        //DBBase dbbase = new DBBase();
        //Stopwatch stopWatchCan1 = new Stopwatch();
        //Stopwatch stopWatchCan2 = new Stopwatch();
        public MainWindow()
        {

            consoleID = "1";
            InitializeComponent();
            Console = new Machine(new CanBusCommunication(), new GeneralPurposeInputOutput());
            Converter = new ASCIIToByteConverter();
            Console.FailResetEnable();
            Thread.Sleep(10);
            Console.FailResetDisable();
            Thread.Sleep(10);
            Console.Disconnect();
            //Subscribe  to the registers
            Console.registerEvent += new EventHandler<RegisterValuesEventArgs>(RegisterChanged);

            //Subscribe to Connection box register
            Console.canTwoRegisterEvent += new EventHandler<RegisterValuesEventArgs>(CanTwoRegisterChanged);
            //percentageTimer = new DispatcherTimer();
            //percentageTimer.Interval = TimeSpan.FromSeconds(5);
            //percentageTimer.Tick += PercentageTimer_Tick;
            //CommonViewModel currentViewModel = new CommonViewModel();


            usbDriveConnectionManager = new USBDriveConnectionManager.USBDriveConnectionManager(USBDriveConnection_EventArrived);


            try
            {
                USBDriveList = usbDriveConnectionManager.GetUSBDriveList();
                path = USBDriveList[0].Name + "updateLog_" + consoleID + ".txt";
            }
            catch (Exception ex)
            {
                // TODO
                ex.ToString();
            }
            //TODO: CREATE TEMP FILE FOR DATABASE (FILESTORAGE)
        }

        private void RegisterChanged(object sender, RegisterValuesEventArgs e)
        {

            //pbStatus.Value = Converter.DataTransmissionPercenatge;
            var communicationData = sender as ICanBusCommunication;
            byte[] data = null;

            if (communicationData != null && communicationData.CanBusOneEventArgs.Data != null)
            {
                data = communicationData.CanBusOneEventArgs.Data;
            }

            if (communicationData.CanBusOneEventArgs.Falgs != (int)FrameType.Remote && data != null)
            {

                // Register values Main Microcontroller

                switch (e.ID)
                {
                    case 8:
                        //VERSIONS
                        var cmcu = ((data[0] * 256 + data[0 + 1] << 16)) >> 16;
                        var cpld = ((data[2] * 256 + data[2 + 1] << 16)) >> 16;
                        var boot = ((data[4] * 256 + data[4 + 1] << 16)) >> 16;
                        CurrentProcess = "CMCU:" + cmcu + " CPLD:" + cpld + " BOOT:" + boot;
                        break;
                    case 59:
                        //SendInit();
                        CurrentProcess = "Send init...";
                        Console.SendBootMessage(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE, (int)CanBusMessageDefinition.BootLoaderID.CAN_ID_BOOT_INIT, Converter.Initdata);
                        break;
                    case 60:
                        //FIRMWARE UPDATE END MSG
                        //ValidateFirmwareUpgrade((((data[3] * 256 + data[2]))));
                        //PASS = 44033, 44034, 44035
                        //FAIL = 48385, 48386 ,48387
                        var oh = data[3] * 256 + data[2];
                        CurrentProcess = "Finished firmware upgrade with message " + oh;
                        IsUpdatingFirmware = false;
                        //stopWatchCan1.Restart();
                        break;
                }
            }
            else
            {
                if (e != null && e.ID == 50)
                {


                }

                if (e != null && e.ID == 51)
                {
                }


                if (e != null && e.ID == 58)
                {


                    //AnswerRTRBootData(communicationData);
                    if (Converter.CanSendEndTransmission)
                    {
                        Console.SendBootMessage(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE, (int)CanBusMessageDefinition.BootLoaderID.CAN_ID_BOOT_END, Converter.Initdata);

                    }

                    else
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            //CurrentProcess = "Answering with data loop no."+i;
                            packetNumber = 0;
                            var bootdata = new byte[8];
                            bootdata = Converter.GetPacket(out packetNumber);
                            Console.AnswerRTRBootMessage(packetNumber, (int)communicationData.CanBusOneEventArgs.Id, bootdata);
                            Array.Clear(bootdata, 0, 8);

                            var percentage = Converter.DataTransmissionPercenatge;
                        }
                    }



                }
            }
        }
        private void CanTwoRegisterChanged(object sender, RegisterValuesEventArgs e)
        {
            //pbStatus.Value = Converter.DataTransmissionPercenatge;
            var communicationData = sender as ICanBusCommunication;
            byte[] data = null;

            if (communicationData != null && communicationData.CanBusTwoEventArgs.Data != null)
            {
                data = communicationData.CanBusTwoEventArgs.Data;
            }

            switch (e.ID)
            {


                case 58:
                    if (Converter.CanSendEndTransmission)
                    {
                        Console.SendBootMessageForICBOrReapeter(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE, (int)CanBusMessageDefinition.BootLoaderID.CAN_ID_BOOT_END, Converter.Initdata);

                    }

                    else
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            packetNumber = 0;
                            var bootdata = new byte[8];
                            bootdata = Converter.GetPacket(out packetNumber);
                            Console.AnswerRTRBootMessageForICBOrReapeter(packetNumber, (int)communicationData.CanBusTwoEventArgs.Id, Converter.GetPacket(out packetNumber));
                            Array.Clear(bootdata, 0, 8);
                        }
                    }
                    break;

                case 59:
                    CurrentProcess = "Send init...";
                    Console.SendBootMessageForICBOrReapeter(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE, (int)CanBusMessageDefinition.BootLoaderID.CAN_ID_BOOT_INIT, Converter.Initdata);
                    break;

                case 60:
                    //ValidateFirmwareUpgrade((((data[3] * 256 + data[2]))));
                    //PASS = 44033, 44034, 44035
                    //FAIL = 48385, 48386 ,48387
                    CurrentProcess = "Finished firmware upgrade";
                    var oh = data[3] * 256 + data[2];
                    IsUpdatingFirmware = false;
                    //stopWatchCan2.Restart();
                    break;
            }
        }

        private void CancelFirmwareUpdate()
        {
            Converter.ResetPackets();
            Array.Clear(ModuleKeysdata, 0, 8);
            ModuleKeysdata = BitConverter.GetBytes((long)CanBusMessageDefinition.ModuleKeys.CMCUREBOOT);
            if (FILESTORAGE.Contains("RMCU") || FILESTORAGE.Contains("ICB"))
            {
                //FOR REPEATER AND ICB
                Console.SendBootMessageForICBOrReapeter(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE, (int)CanBusMessageDefinition.BootLoaderID.CAN_ID_BOOT_INIT, ModuleKeysdata);

            }
            else
            {
                //FOR REST
                Console.SendBootMessage(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE, (int)CanBusMessageDefinition.BootLoaderID.CAN_ID_BOOT_INIT, ModuleKeysdata);

            }
            Converter.DataTransmissionPercenatge = 0;
            IsUpdatingFirmware = false;
        }
        //SEPARATE GUI THREAD AND PROCESS THREAD
        private void Window_ContentRendered(object sender, EventArgs e)
        {

            //LOAD CHECKBOXES

            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;

            worker.RunWorkerAsync();

            Hide();
            //BackgroundWorker workerBack = new BackgroundWorker();
            //workerBack.WorkerReportsProgress = true;
            //workerBack.WorkerSupportsCancellation = true;
            //workerBack.DoWork += worker_BackupAll;
            //workerBack.ProgressChanged += worker_ProgressChanged;

            //worker.RunWorkerAsync();


        }

        //private void worker_BackupAll(object sender, DoWorkEventArgs e)
        //{
        //    //DB
        //    Directory.CreateDirectory(@"C:\BSC_WinUpgradeTemp\");
        //    dbbase.DBBackupFileGen(@"C:\BSC_WinUpgradeTemp\", "BSC_ConsoleDatabase.BAK");
        //    var zipFile = "BSCSFD.zip";
        //    ZipFile ziping = new ZipFile();
        //    ziping.Password = "Eh*203.0%$!!azXcw@)";
        //    ziping.AddDirectory(@"C:\BSC_WinUpgradeTemp\");
        //    ziping.Save(zipFile);

        //    //FILESTORE
        //    string filestorePath = @"C:\Program Files\BSC\Smart Ablation System\FileStore";
        //    ziping = new ZipFile();
        //    ziping.Password = "Eh$dege*8778uBYRkijh)";
        //    ziping.AddDirectory(filestorePath);
        //    ziping.Save(zipFile);
        //}

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            string userState = e.UserState as string;
            if (userState == null)
            {
                StepPerc.Visibility = Visibility.Hidden;

                StepStatus.IsIndeterminate = true;
                pbStatus.Value = e.ProgressPercentage;
            }
            else
            {
                StepPerc.Visibility = Visibility.Visible;
                StepStatus.IsIndeterminate = false;
                StepStatus.Value = e.ProgressPercentage;
            }
            if (e.ProgressPercentage == 100)
            {
                DemoScreen demoScreen = new DemoScreen();
                demoScreen.Show();
                Close();
            }

        }
        private void USBDriveConnection_EventArrived(object sender, EventArrivedEventArgs e)
        {
            try
            {
                USBDriveList = usbDriveConnectionManager.GetUSBDriveList();
                if (IsUpdatingFirmware && USBDriveList.Count == 0)
                {
                    CancelFirmwareUpdate();
                }

            }
            catch (Exception ex)
            {
                ex.ToString();

            }
        }

        private void KillAllOtherProcesses()
        {
            Process current = Process.GetCurrentProcess();
            // get all the processes with currnent process name
            Process[] processes = Process.GetProcessesByName("SmartAblationSystem");

            foreach (Process process in processes)
            {
                //Ignore the current process  
                if (process.Id != current.Id)
                {
                    process.Kill();
                }
            }
        }
        private void Log(string log)
        {
            var time = DateTime.Now.ToString();
            var logString = time + " : " + log;
            if (!File.Exists(path) && path != string.Empty)
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine(logString);
                }
            }
            else
            {
                using (StreamWriter writer = new StreamWriter(path, true))
                {
                    writer.WriteLine(logString);
                }
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (!updateDone)
            {
                MessagePopup confirmMessage = new MessagePopup("Cancelling the update process will cause the SMARTFREEZE system to reboot. Continue?");
                if ((bool)confirmMessage.ShowDialog())
                {
                    System.Diagnostics.Process.Start("shutdown.exe", "-r -t 0");
                }
                else
                {
                    e.Cancel = true;
                }

            }
        }
        //CODE TO EXECUTE IN WORKER THREAD
        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            //TODO: MOVE UPDATE FOLDERS TO DESKTOP AND UNZIP THEM
            //MessagePopup confirmMessage = new MessagePopup("Cancelling the update process will cause the SMARTFREEZE system to reboot. Continue?");
            //confirmMessage.ShowDialog();
            Log("Starting Update");
            try
            {
                USBDriveList = usbDriveConnectionManager.GetUSBDriveList();
            }
            catch (Exception ex)
            {
                // TODO
                Log(ex.ToString());

            }
            //updateFirmware(Path.Combine(USBDriveList[0].Name, "CMCU\\"));

            var USBRoot = USBDriveList[0].Name;
            //Directory.CreateDirectory("C:\\SystemUpdates");
            //File.Copy(Path.Combine(USBRoot,"SystemUpdater/UpdateFiles.zip"), PCUpdatesPathZip, true);
            //string x = Path.Combine(USBRoot, @"SystemUpdater\UpdateFiles");
            //try
            //{
            //    if (Directory.Exists(x))
            //    {

            //        Directory.Move(x, PCUpdatesPathZip);
            //    }
            //}
            //catch (Exception exx)
            //{
            //    System.Console.WriteLine(exx.Message);
            //}

            //NO PW
            using (ZipFile zip = ZipFile.Read(Path.Combine(USBRoot, "SystemUpdater/UpdateFiles.zip")))
            {
                zip.ExtractAll("C:\\", ExtractExistingFileAction.OverwriteSilently);
            }
            //Directory.Delete("C:\\SystemUpdates", true);

            //Read me file
            //read line
            List<UpdateObj> allUpdates = new List<UpdateObj>();
            try
            {
                string readMe = PCUpdatesPathZip + "\\readme.txt";
                if (File.Exists(readMe))
                {
                    CurrentProcess = "Getting all updates from read me file...";
                    Dispatcher.Invoke(delegate ()
                    {
                        UpdateSelection Selector = new UpdateSelection(getUpdatesFromTxt());
                        Selector.ShowDialog();
                        allUpdates = Selector.Updates;
                        Show();
                    });
                    //TODO: DELETE READ ME AFTER SUCCESSFUL READING
                }
                else
                {
                    Log("ReadMe file not found aborting update");
                    MessageBox.Show("Could not find readme file", "Message", MessageBoxButton.OK, MessageBoxImage.Error);
                    //return; put that back
                }

            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }

            MaxStep = allUpdates.Count;
            //MaxStep = 5;
            CurrentStep = 0;
            int progress = 0;
            //loop through all contents of readme file
            if (allUpdates.Count > 0)
            {
                //StepStatus.IsIndeterminate = false;
                foreach (UpdateObj update in allUpdates)
                //for (int i = 0; i < 5; i++)
                {

                    //TODO: Write to log file update title and timestamp
                    //DateTime.Now.ToString("HH:mm:ss");
                    if (update.type.ToLower() == "gui")
                    {
                        uninstallOldGUI();
                        UpdateGUI(update);
                        CurrentStep++;
                    }
                    else if (update.type.ToLower() == "firmware")//update firmwares
                    {
                        IsUpdatingFirmware = true;
                        Log("Updating " + update.title + " to version " + update.version);
                        CurrentProcess = "Updating " + update.title + " to version " + update.version + " ...";
                        updateFirmware(update.path, update.title);

                        while (IsUpdatingFirmware)
                        {

                        }
                        CurrentStep++;
                    }
                    else if (update.type.ToLower() == "other")//Run custom .exe files
                    {
                        UpdateOtherEXE(update);
                        CurrentStep++;
                    }
                    Thread.Sleep(1000);

                    progress += 100 / MaxStep;
                    CurrentStep++;
                    //(sender as BackgroundWorker).ReportProgress(progress,"firmware");//FOR FIRMWARE UPDATE
                    (sender as BackgroundWorker).ReportProgress(progress);
                }
                updateDone = true;
                if (progress != 100)
                {
                    (sender as BackgroundWorker).ReportProgress(100);
                }

                //HIDE AND OPEN DEMO SCREEN FOR REBOOT
                //TODO: Delete UpdateFiles from root folder
            }
        }
        public void UpdateOtherEXE(UpdateObj update)
        {
            Log("Updating " + update.title);
            CurrentProcess = "Updating " + update.title + " ...";

            Process OtherProcess = new Process();
            OtherProcess.StartInfo.FileName = update.path;

            OtherProcess.Start();
            OtherProcess.WaitForExit();

        }
        public void UpdateGUI(UpdateObj update)
        {
            Log("Updating to GUI v." + update.version);
            CurrentProcess = "Updating to GUI v." + update.version + " ...";

            ////Install new GUI
            Process GUIprocess = new Process();
            GUIprocess.StartInfo.FileName = "msiexec.exe";
            GUIprocess.StartInfo.Arguments = string.Format("/i {0} /qb+ /promptrestart", update.path);

            GUIprocess.Start();
            GUIprocess.WaitForExit();

        }

        public void uninstallOldGUI()
        {
            CurrentProcess = "Deleting old GUI...";
            var oldGUIPath = @"C:\UpdateFiles\oldGUI\Setup.msi";
            //Remove old GUI
            Process deleteOldGUI = new Process();
            deleteOldGUI.StartInfo.FileName = "msiexec.exe";
            //TODO: Resolve Path
            deleteOldGUI.StartInfo.Arguments = string.Format("/x {0} /qb+!", oldGUIPath);

            deleteOldGUI.Start();
            deleteOldGUI.WaitForExit();
        }

        //This function reads the updates from the specified read me file
        public List<UpdateObj> getUpdatesFromTxt()
        {
            var PCRoot = @"C:\";
            var USBRoot = USBDriveList[0].Name;
            List<UpdateObj> allUpdates = new List<UpdateObj>();
            string line;

            System.IO.StreamReader file = new System.IO.StreamReader(PCUpdatesPathZip + @"\readme.txt");
            while ((line = file.ReadLine()) != null)
            {

                string[] obj = line.Split(' ');
                string test = Path.Combine(PCRoot, obj[1]);
                obj[1] = Path.Combine(PCRoot, obj[1]);
                allUpdates.Add(new UpdateObj(obj[0], obj[1], obj[2], obj[3]));

            }

            file.Close();
            //DELETE readme after reading
            //File.Delete(USBRoot + "/SystemUpdater/readme.txt");
            return allUpdates;
        }
        public void updateFirmware(string path, string title)
        {
            if (Directory.Exists(path))
            {
                CurrentProcess = "Updating Firmware...";

                //Put Console in safe mode (all LEDs on)
                Console.GUIIsReady = false;
                System.Threading.Thread.Sleep(1000);
                Console.GUIInMaintenanceMode = true;
                Console.HeartbeatActivated = false;

                Converter.ResetPackets();
                Converter.DataTransmissionPercenatge = 0;

                Array.Clear(ModuleKeysdata, 0, 8);
                byte[] fileSizedata = new byte[8];
                Array.Clear(fileSizedata, 0, 8);
                Array.Clear(Initdata, 0, 8);
                Converter.ClearInitData();

                FILESTORAGE = path;
                SetBoardType(title);
                Converter.BoardType = FirmwareBootLoader.Helpers.Definitions.Board.CPLD;
                ModuleKeysdata = BitConverter.GetBytes((long)CanBusMessageDefinition.ModuleKeys.CMCUKey);

                string[] filePaths = Directory.GetFiles(FILESTORAGE);
                Converter.ReadFile(filePaths[0]);
                long length = new FileInfo(filePaths[0]).Length;
                fileSizedata = BitConverter.GetBytes((long)length);

                Initdata[0] = ModuleKeysdata[0];
                Initdata[1] = ModuleKeysdata[1];
                Initdata[2] = fileSizedata[0];
                Initdata[3] = fileSizedata[1];
                Initdata[4] = fileSizedata[2];
                Initdata[5] = fileSizedata[3];
                Converter.Initdata = Initdata;

                if (title.ToUpper() == "RMCU" || title.ToUpper() == "ICB")
                {
                    //FOR REPEATER AND ICB (CAN 2)
                    Console.SendBootMessageForICBOrReapeter(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE, (int)CanBusMessageDefinition.BootLoaderID.CAN_ID_BOOT_START, ModuleKeysdata);
                    //stopWatchCan2.Start();
                    //stopWatchCan1.Reset();
                }
                else
                {
                    //FOR REST (CAN 1)
                    Console.SendBootMessage(CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE, (int)CanBusMessageDefinition.BootLoaderID.CAN_ID_BOOT_START, ModuleKeysdata);
                    //stopWatchCan1.Start();
                    //stopWatchCan2.Reset();
                }
            }
            else
            {
                Log(title + " update file not found");
            }
        }
        /// <summary>
        /// Sets Board types for communication to firmwares using CANBUS
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public void SetBoardType(string title)
        {
            if (title.ToUpper() == "CPLD")
            {
                Converter.BoardType = FirmwareBootLoader.Helpers.Definitions.Board.CPLD;
                ModuleKeysdata = BitConverter.GetBytes((long)CanBusMessageDefinition.ModuleKeys.CMCUKey);  //When we are programming CPLD we first CMCU key
            }

            else if (title.ToUpper() == "PMCU")
            {
                Converter.BoardType = FirmwareBootLoader.Helpers.Definitions.Board.PMCU;
                ModuleKeysdata = BitConverter.GetBytes((long)CanBusMessageDefinition.ModuleKeys.PMCUKey);
            }

            else if (title.ToUpper() == "RMCU")
            {
                Converter.BoardType = FirmwareBootLoader.Helpers.Definitions.Board.Repeater;
                ModuleKeysdata = BitConverter.GetBytes((long)CanBusMessageDefinition.ModuleKeys.RMCUKey);
            }

            else if (title.ToUpper() == "ICB")
            {
                Converter.BoardType = FirmwareBootLoader.Helpers.Definitions.Board.ICB;
                ModuleKeysdata = BitConverter.GetBytes((long)CanBusMessageDefinition.ModuleKeys.BMCUKey);
            }

            else if (title.ToUpper() == "CATHETER")
            {
                Converter.BoardType = FirmwareBootLoader.Helpers.Definitions.Board.Catheter;
                // ModuleKeysdata = BitConverter.GetBytes((long)ModuleKeys.CMCUKey); TODO
            }

            else if (title.ToUpper() == "CMCU")
            {
                Converter.BoardType = FirmwareBootLoader.Helpers.Definitions.Board.CMCU;
                ModuleKeysdata = BitConverter.GetBytes((long)CanBusMessageDefinition.ModuleKeys.CMCUKey);
            }
        }
    }

    public class UpdateObj
    {

        public string title;
        public string path;
        public string version;
        public string type;
        public UpdateObj(string title, string path, string version, string type)
        {
            this.title = title;
            this.path = path;
            this.version = version;
            this.type = type;
        }
    }
}
