using DataAccessLayer;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;
using System;
using System.Management;
using System.IO;
using SmartAblationSystem.Helpers;
using SmartAblationSystem.Views;
using System.Threading.Tasks;
using System.Data.SqlClient;
using FileSerializer;
using System.Data;
using Prism.Commands;

namespace SmartAblationSystem.ViewModels
{
  using System.Windows;
  using System.Windows.Threading;

  public class ConsoleErrorLogViewModel : BindableBase
    {
        public ICommand ReturnToSettingsCommand { get; private set; }
        private List<ErrorLog> errorLogList = new List<ErrorLog>();
        private List<ErrorLog> filtedErrorLogList = new List<ErrorLog>();
    //    public List<CatheterInformation> cathInfo = new List<CatheterInformation>();
        private List<List<ErrorLog>> groupErrorLogListByErrorCode = new List<List<ErrorLog>>();
        private List<List<ErrorLog>> groupErrorLogListByDate = new List<List<ErrorLog>>();
        private USBDriveConnectionManager.USBDriveConnectionManager usbDriveConnectionManager;
        private List<DriveInfo> usbDriveList;
        private string filterErrorNum = "";
        private string filterFrom = "";
        private string filterTo = "";
        private long errorcode = 0;
        private bool isErrorLogSummaryPopup=false;
        private int errorLogNum = 0;
        private ErrorType selectederrortype = new ErrorType();
        private bool saveInProgress;
        private string saveToUSBPath;
        private string filePassword;
        public ICommand ErrorLogSummaryCommand { get; private set; }
        public ICommand FilterCommand { get; private set; }
        public ICommand ClearCommand { get; private set; }
        public ICommand SaveDataToUSBCommand { get; private set; }
        public ConsoleErrorLogViewModel()
        { 
            this.ReturnToSettingsCommand = new DelegateCommand<object>(this.OnReturnToSettingsCommand, this.CanReturnToSettingsCommand);
            CommonViewModel.Current.PropertyChanged += Current_PropertyChanged;
            this.FilterCommand = new DelegateCommand<object>(this.OnFilterCommand, this.CanFilterCommand);
            this.ClearCommand = new DelegateCommand<object>(this.OnClearCommand, this.CanClearCommand);
            this.ErrorLogSummaryCommand = new DelegateCommand<object>(this.OnErrorLogSummaryCommand, this.CanErrorLogSummaryCommand);
            this.SaveDataToUSBCommand = new DelegateCommand<object>(this.OnSaveDataToUSBCommand, this.CanSaveDataToUSBCommand);

            GetErrorLogValue();


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
                                 
        }


        public void GetErrorLogValue()
        {
            ErrorLogList = CommonViewModel.Current.Data.DataAccess.GetErrorLog();
            FiltedErrorLogList = errorLogList;
            var temp1 = errorLogList.OrderBy(x => x.ErrorInformation).ToList();
            var temp2 = errorLogList.OrderByDescending(x => x.ErrorDate).ToList();
            ErrorLogNum = FiltedErrorLogList.Count;
            GroupErrorLogListByErrorCode = temp1.GroupBy(u => u.ErrorCode).Select(grp => grp.ToList()).ToList();
            GroupErrorLogListByDate = temp2.GroupBy(u => u.ErrorDate.ToString("0:MMM dd yyy")).Select(grp => grp.ToList()).ToList();
        }


        /// <summary>
        /// Function that returns if the system can invoke the Return To Settings command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command parameter (not used in this function).</param>
        /// <returns>Boolean value if the system can invoke the command.</returns>
        private bool CanReturnToSettingsCommand(object arg)
        {
            return true;
        }

        /// <summary>
        /// Function/Command that handles the Return To Settings operation when the Return To Settings
        /// command is invoked
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="obj">The command's parameter (not used in this function).</param>
        private void OnReturnToSettingsCommand(object obj)
        {
            ViewsEventArgs viewsEvent = new ViewsEventArgs();
            viewsEvent.ViewName = "BackToSettings";
            CommonViewModel.Current.OnViewchanged(viewsEvent);
        }

        /// <summary>
        /// This function handles the sender's PropertyChanged event
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">The View Model that sent the event.</param>
        /// <param name="e">The property changed arguments.</param>
        private void Current_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
          // CommonViewModel commonviewmodel = sender as CommonViewModel;

            switch (e.PropertyName)
            {
                case "Login":
                    RaisePropertyChanged("Login");
                    break;
            }
        }

        /// <summary>
        /// gets/sets a list of error log.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public List<ErrorLog> ErrorLogList
        {
            get
            {
                return errorLogList;
            }
            set
            {
                errorLogList = value;
                RaisePropertyChanged("ErrorLogList");
            }

        }
        /// <summary>
        /// gets/sets a list of filteed error log.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public List<ErrorLog> FiltedErrorLogList
        {
            get
            {
                 return filtedErrorLogList;
            }
            set
            {
                filtedErrorLogList = value;
                RaisePropertyChanged("FiltedErrorLogList");
            }

        }

        /// <summary>
        /// gets/sets filter error number value.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string FilterErrorNum
        {
            get
            {
                return filterErrorNum;
            }
            set
            {
                filterErrorNum = value;
                RaisePropertyChanged("FilterErrorNum");
            }
            
        }

        /// <summary>
        /// gets/sets a list of group error code.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public List<List<ErrorLog>> GroupErrorLogListByErrorCode
        {
            get
            {
                return groupErrorLogListByErrorCode;
            }
            set
            {
                groupErrorLogListByErrorCode = value;
                RaisePropertyChanged("GroupErrorLogListByErrorCode");

            }
        }

        /// <summary>
        /// gets/sets a list of Group Error Log List By Date.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public List<List<ErrorLog>> GroupErrorLogListByDate
        {
            get
            {
                return groupErrorLogListByDate;
            }
            set
            {
                groupErrorLogListByDate = value;
                RaisePropertyChanged("GroupErrorLogListByDate");

            }
        }

        /// <summary>
        /// Gets a list of ErrorTypelist.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public List<ErrorType> ErrorTypeList
        {
            
            get
            {
                List<ErrorType> errorTypeList =null;

                errorTypeList = CommonViewModel.Current.Data.DataAccess.GetErrorType().ToList();
                ErrorType defaultitem = new ErrorType();
                defaultitem.Description = "All";
                defaultitem.Type = 0;
                errorTypeList.Add(defaultitem);               
                return errorTypeList;
            }
        }



        /// <summary>
        /// get/set FilterFrom value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string FilterFrom
        {
            get
            {
                return filterFrom;
            }
            set
            {
                filterFrom = value;
                RaisePropertyChanged("FilterFrom");
            }

        }
        /// <summary>
        /// get/set SelectErrortype value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public ErrorType SelectErrortype
        {
            get
            {
                return selectederrortype;
            }
            set
            {
                selectederrortype = value;
                RaisePropertyChanged("SelectErrortype");
            }
        }


        /// <summary>
        /// get/set FilterTo value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string FilterTo
        {
            get
            {
                return filterTo;
            }
            set
            {
                filterTo = value;
                RaisePropertyChanged("FilterTo");
            }

        }


        /// <summary>
        /// get/set ErrorLoCode value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public long ErrorCode
        {
            get
            {
                return errorcode;
            }
            set
            {
                errorcode = value;
                RaisePropertyChanged("ErrorCode");
            }

        }
        /// <summary>
        /// get/set ErrorLogNum value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
      
        public int ErrorLogNum
        {
            get
            {
                return errorLogNum;
            }
            set
            {
                errorLogNum = value;
                RaisePropertyChanged("ErrorLogNum");
            }
        }

        /// <summary>
        /// Function/Command that handles the Filter command is invoked
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command's parameter (not used in this function).</param>
        private bool CanFilterCommand(object arg)
        {
            return true;
        }

        /// <summary>
        /// Function/Command that handles the Filter command is invoked
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command's parameter (not used in this function).</param>
        private void OnFilterCommand(object arg)
        {
            string errorNum = "";    //FilterErrorNum;
            ErrorType selecterrorType = SelectErrortype;

            string errorDateFrom =  "1/1/1900 00:00:00 AM";
            string errorDateTo = "1/1/2900 00:00:00 AM";
            if (!String.IsNullOrEmpty(filterFrom))
            {
                string[] sub1 = filterFrom.Split(' ');
                errorDateFrom = sub1[0] + " 00:00:00 AM" ;
            }   

            if (!String.IsNullOrEmpty(filterTo))
            {
                string[] sub2 = filterTo.Split(' ');
                errorDateTo = sub2[0]  +" 11:59:59 PM";
            }
                

            var temp = errorLogList.Where(p=>p.ErrorDate>=DateTime.Parse(errorDateFrom) && p.ErrorDate<= DateTime.Parse(errorDateTo)).ToList();
            
            if (FilterErrorNum != "")
            { 
                errorNum = FilterErrorNum;
                temp = temp.Where(p => p.ErrorInformation.ToLower().Contains(errorNum.ToLower())).ToList();
            }

            if(selecterrorType!=null && selecterrorType.Type > 0)
            {
                temp = temp.Where(p => p.ErrorType == int.Parse(selecterrorType.Type.ToString())).ToList();
            }

            FiltedErrorLogList = temp;
            ErrorLogNum = temp.Count;
            var temp1 = temp.OrderBy(x => x.ErrorInformation).ToList();
            var temp2 = temp.OrderByDescending(x => x.ErrorDate).ToList();
            GroupErrorLogListByErrorCode = temp1.GroupBy(u => u.ErrorCode).Select(grp => grp.ToList()).ToList();
            GroupErrorLogListByDate = temp2.GroupBy(u => u.ErrorDate.ToString("0:MMM dd yyy")).Select(grp => grp.ToList()).ToList();
        }



        /// <summary>
        /// Function/Command that handles the SaveDataToUSB command is invoked
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command's parameter (not used in this function).</param>
        private bool CanSaveDataToUSBCommand(object arg)
        {
            return true;
        }

        /// <summary>
        /// Function/Command that handles the SaveDataToUSB command is invoked
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command's parameter (not used in this function).</param>
        private async void OnSaveDataToUSBCommand(object sender)
        {
            bool operationCompleted = false;
            string USBPath = "";
            if (USBDriveList != null && USBDriveList.Count > 0 )
            {
                try
                {

                    USBPath = USBDriveList[0].Name + "BSCErrorLog" + Path.DirectorySeparatorChar;
                    saveToUSBPath = "File(s) will be saved in " + " " + USBDriveList[0].Name + "BSCErrorLog" +
                                    Path.DirectorySeparatorChar;

         
                    SaveEngineeringDataToUSB saveengineeringData = new SaveEngineeringDataToUSB(this);

                    if ((bool)saveengineeringData.ShowDialog())
                    {
                        SaveInProgress = true;

                        
                        operationCompleted = await Task.Run(() => SaveToUSB());
                    }
                }
                catch (IOException ex)
                {

                    ex.ToString();
                    Tuple<long, string, string, string> genericMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID43, (int)Enumeration.ErrorTypes.GUI);
                    Tuple<long, string, string, string> titleMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID44, (int)Enumeration.ErrorTypes.GUI);

                    MessagePopup dialogPopup = new MessagePopup(genericMessage, MessagePopup.MessageType.ErrorMessage, MessagePopup.ButtonType.Ok, messageTitle: titleMessage.Item2);
                    dialogPopup.ShowDialog();
                }
                catch (UnauthorizedAccessException ex)
                {
                    ex.ToString();

                    Tuple<long, string, string, string> genericMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID45, (int)Enumeration.ErrorTypes.GUI);
                    Tuple<long, string, string, string> titleMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID44, (int)Enumeration.ErrorTypes.GUI);

                    MessagePopup dialogPopup = new MessagePopup(genericMessage, MessagePopup.MessageType.ErrorMessage, MessagePopup.ButtonType.Ok, messageTitle: titleMessage.Item2);
                    dialogPopup.ShowDialog();
                }
                catch (ArgumentException ex)
                {
                    ex.ToString();

                    //Path is null
                    Tuple<long, string, string, string> genericMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID46, (int)Enumeration.ErrorTypes.GUI);
                    Tuple<long, string, string, string> titleMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID44, (int)Enumeration.ErrorTypes.GUI);

                    MessagePopup dialogPopup = new MessagePopup(genericMessage, MessagePopup.MessageType.ErrorMessage, MessagePopup.ButtonType.Ok, messageTitle: titleMessage.Item2);
                    dialogPopup.ShowDialog();
                }
                catch (NotSupportedException ex)
                {
                    ex.ToString();

                    //The path contains : sign that is invalid for the Drive (ex: C:/)

                    Tuple<long, string, string, string> genericMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID47, (int)Enumeration.ErrorTypes.GUI);
                    Tuple<long, string, string, string> titleMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID44, (int)Enumeration.ErrorTypes.GUI);

                    MessagePopup dialogPopup = new MessagePopup(genericMessage, MessagePopup.MessageType.ErrorMessage, MessagePopup.ButtonType.Ok, messageTitle: titleMessage.Item2);
                    dialogPopup.ShowDialog();
                }
                catch (ObjectDisposedException ex)
                {
                    // TODO
                    ex.ToString();

                    //The target file/directory does not exist anymore (ex: file deleted, drive removed)

                    Tuple<long, string, string, string> genericMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID48, (int)Enumeration.ErrorTypes.GUI);
                    Tuple<long, string, string, string> titleMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID44, (int)Enumeration.ErrorTypes.GUI);

                    MessagePopup dialogPopup = new MessagePopup(genericMessage, MessagePopup.MessageType.ErrorMessage, MessagePopup.ButtonType.Ok, messageTitle: titleMessage.Item2);
                    dialogPopup.ShowDialog();
                }
                catch (Exception ex)
                {
                    // TODO
                    ex.ToString();


                    Tuple<long, string, string, string> genericMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID49, (int)Enumeration.ErrorTypes.GUI);
                    Tuple<long, string, string, string> titleMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID44, (int)Enumeration.ErrorTypes.GUI);

                    MessagePopup dialogPopup = new MessagePopup(genericMessage, MessagePopup.MessageType.ErrorMessage, MessagePopup.ButtonType.Ok, messageTitle: titleMessage.Item2);
                    dialogPopup.ShowDialog();
                }
                finally
                {
                   // SaveInProgress = false;
                }
            }

            if (operationCompleted)
            {
                Tuple<long, string, string, string> genericMessage51 = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID51, (int)Enumeration.ErrorTypes.GUI);
                Tuple<long, string, string, string> genericMessage52 = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID52, (int)Enumeration.ErrorTypes.GUI);

                MessagePopup dialogPopup = new MessagePopup(genericMessage51.Item2, MessagePopup.MessageType.SystemMessage, MessagePopup.ButtonType.Ok, messageTitle: genericMessage52.Item2);
                dialogPopup.ShowDialog();
            }

        }

        /// <summary>
        /// Property that handles the Save to USB boolean.  It tells if the operation is in progress
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool SaveInProgress
        {
            get
            {
                return saveInProgress;
            }
            set
            {
                saveInProgress = value;
                RaisePropertyChanged("SaveInProgress");
            }
        }


        /// <summary>
        /// This property gets/sets the available/connected USB Drive List
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
        /// This function handles the USB Drive connection EventArrived event.
        /// When a USB Drive is connected, the USB Drive list is repopulated
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">The object that sent the event.</param>
        /// <param name="e">The event argument.</param>
        private void USBDriveConnection_EventArrived(object sender, EventArrivedEventArgs e)
        {
            try
            {
                USBDriveList = usbDriveConnectionManager.GetUSBDriveList();
            }
            catch (Exception ex)
            {
                ex.ToString();
                Tuple<long, string, string, string> genericMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID41, (int)Enumeration.ErrorTypes.GUI);
                Tuple<long, string, string, string> titleMessage = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID42, (int)Enumeration.ErrorTypes.GUI);
                Tuple<long, string, string, string> genericMessage53 = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID53, (int)Enumeration.ErrorTypes.GUI);
                Tuple<long, string, string, string> genericMessage54 = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID54, (int)Enumeration.ErrorTypes.GUI);

                DispatcherBeginInvoke(() =>
                    {
                      MessagePopup messagePopup = new MessagePopup(genericMessage53.Item2, MessagePopup.MessageType.WarningMessage, 
                        MessagePopup.ButtonType.Ok, genericMessage54.Item2);
                      messagePopup.ShowDialog();
                    });
            }
        }

        private void DispatcherBeginInvoke(System.Action action)
        {
          Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, action); 
        }

        /// <summary>
        /// Function that returns if the system can invoke the Clear command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command parameter (not used in this function).</param>
        /// <returns>Boolean value if the system can invoke the command.</returns>
        private bool CanClearCommand(object arg)
        {
            return true;
        }

        /// <summary>
        /// Function/Command that handles the Notification window display when the Clear command is invoked
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command's parameter (not used in this function).</param>
        private void OnClearCommand(object arg)
        {
            string errorNum = "";    
            FiltedErrorLogList = ErrorLogList;
            if (FilterErrorNum != "")
            {
                errorNum = FilterErrorNum;
                FiltedErrorLogList = FiltedErrorLogList.Where(p => p.ErrorInformation.Contains(errorNum)).ToList();
            }
          

            //ErrorTypeList.
            ErrorLogNum = FiltedErrorLogList.Count;
            var temp1 = FiltedErrorLogList.OrderBy(x => x.ErrorInformation).ToList();
            var temp2 = FiltedErrorLogList.OrderByDescending(x => x.ErrorDate).ToList();
            GroupErrorLogListByErrorCode = temp1.GroupBy(u => u.ErrorCode).Select(grp => grp.ToList()).ToList();
            GroupErrorLogListByDate = temp2.GroupBy(u => u.ErrorDate.ToString("0:MMM dd yyy")).Select(grp => grp.ToList()).ToList();


        }




        /// <summary>
        /// Function/Command that handles the Notification window display when the ErrorLogSummary command is invoked
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command's parameter (not used in this function).</param>
        private void OnErrorLogSummaryCommand(object arg)
        {
            IsErrorLogSummaryPopup = true;
            //IsSavedToDB = false;
        }

        /// <summary>
        /// Function that returns if the system can invoke the ErrorLogSummary command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command parameter (not used in this function).</param>
        /// <returns>Boolean value if the system can invoke the command.</returns>
        private bool CanErrorLogSummaryCommand(object arg)
        {
            return true;
        }

        public bool IsErrorLogSummaryPopup
        {
            get
            {
                return isErrorLogSummaryPopup;
            }
            set
            {
                isErrorLogSummaryPopup = value;
                RaisePropertyChanged("IsErrorLogSummaryPopup");
            }

        }

        /// <summary>
        /// This function handles the Engineering files saving on a USB drive.  It allows file selection, conversion to JSON/CSV
        /// and saving on a USB Drive
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <returns>Boolean if the operation was successfull.</returns>
        private bool SaveToUSB()
        {
            //EngineeringData engineeringData = null;
            //string saveToUSBPath = "";
            bool operationCompleted = false;

            if (USBDriveList != null && USBDriveList.Count > 0)
            {
                try
                {
                    ExcelManager excelManger = new ExcelManager();
                    DBBase dbBase = new DBBase();
                    DataSet errorLogData = dbBase.GetErrorLogExcelData();
                    if (errorLogData!=null)
                    {
                        string FileName = "ErrorLogDetails" + "_"+ DateTime.Now.ToString("MMddyyyy_hhmmss");
                        excelManger.GenerateErrorLogExcelFile(errorLogData, FileName, USBDriveList[0].Name + "BSCErrorLog", FilePassword);
                    }

                   
                    operationCompleted = true;
                }
                catch (Exception ex)
                {
                    // TODO
                    ex.ToString();

                    throw;
                }
            }

            return operationCompleted;
        }

        /// <summary>
        /// Gets/sets File password string
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string FilePassword
        {
            get { return filePassword; }
            set
            {
                filePassword = value;
            }
        }

        /// <summary>
        /// This read-only property returns if a USB Drive is connected
        /// to the console
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public Boolean USBDriveConnected
        {
            get
            {
                return USBDriveList != null && USBDriveList.Count != 0 ;
            }
        }

        /// <summary>
        /// This property gets/sets the path where to save the data
        /// on a USB Drive
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string SaveToUSBPath
        {
            get { return saveToUSBPath; }
            set
            {
                saveToUSBPath = value;
                RaisePropertyChanged("SaveToUSBPath");
            }
        }
    }

}
