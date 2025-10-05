using DataAccessLayer;
using Prism.Mvvm;
using SmartAblationSystem.Helpers;
using SmartAblationSystem.Models;
using SmartAblationSystem.Validation;
using SmartAblationSystem.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using static SmartAblationSystem.Helpers.Enumeration;
using Microsoft.VisualBasic.FileIO;
using System.Windows;
using System.IO.Ports;
using System.Linq;
using Prism.Commands;

namespace SmartAblationSystem.ViewModels
{
    /// <summary>
    /// This class is the Site Setup View Model.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class SiteSetupViewModel : BindableBase
    {

        [DllImport("Shell32.dll", CharSet = CharSet.Unicode)]
        static extern uint SHEmptyRecycleBin(IntPtr hwnd, string pszRootPath, RecycleFlags dwFlags);

        private string hospitalName;
        private string address;
        private string city;
        private string state;
        private string zIPCode;
        private string country;
        private string phoneNumber;
        private string serialNumber;
        private bool isHospitalInfoValid;
        private bool isPoundUnitActivated;
        // private bool hasPrinter;
        private bool isInchesUnitActivated;
        private bool activateBalloonRampDown;
        private Language selectedLanguage;
        private int originalLanguageId;
        private HospitalInformationValidator hospitalInformationValidator;
        private bool isUsingBloodPressureSensor;

        private const string FILESTORAGE = "FileStore\\";
        private const string ARCHIVESTORAGE = "Archive\\";
        List<string> invalidPortComList = new List<string> { "COM1", "COM2", "COM3", "COM4" };

        private HospitalInformation hospitalInformation;

        private int numberOfFiles = 1000;
        private int numberOfDeletedFile = 0;
        int progressPercentage = 0;
        string action = string.Empty;
        private bool isDeletingCompleted = false;
        private bool isDeletingStarted = false;
        private bool isComPortsAvailable = false;
        private bool isComPortsValide = true;
        private int changeindex = 0;

        //Metal 
        private double tenPoundsMetalWeightValue = 0;

        private double fifteenPoundsMetalWeightValue = 0;
        public ICommand SaveCommand { get; private set; }

        public ICommand PurgeCommand { get; private set; }

        public ICommand DeactivateFeatuersCommand { get; private set; }

        public ICommand OcclusionPressureSensorCommand { get; private set; }

        public ICommand EnableDefalteAfterThawCommand { get; private set; }

        public ICommand DeleteSiteInformarionsCommand { get; private set; }

        public ICommand WeightUnitCommand { get; private set; }

        public ICommand BalloonRampDownCommand { get; private set; }

        public ICommand ReadValuesFromDBCommand { get; private set; }

        //Metal weight command 
        public ICommand TenPoundsMetalWeightValueCommand { get; private set; }

        public ICommand FifteenPoundsMetalWeightValueCommand { get; private set; }

        //LSPRO command
        public ICommand OpenComPortCommand { get; private set; }

        public ICommand CloseComPortCommand { get; private set; }

        public ICommand SaveComPortCommand { get; private set; }
        public ICommand LowFlowCommand { get; private set; }

        public ICommand EnhancedAudioCommand { get; private set; }

        /// <summary>
        /// This constructor initializes the Site Setup properties and commands.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public SiteSetupViewModel()
        {
            HospitalInformation = CommonViewModel.Current.Data.DataAccess.GetHospitalInformation();
            if (HospitalInformation != null)
            {
                HospitalName = HospitalInformation.Name;
                Address = HospitalInformation.Address;
                City = HospitalInformation.City;
                ZIPCode = HospitalInformation.PostalCode;
                PhoneNumber = HospitalInformation.PhoneNumber;
                State = HospitalInformation.State;
                Country = HospitalInformation.Country;
            }

            TenPoundsMetalWeightValue = CommonViewModel.Current.Data.DataAccess.GetTankTypes(1).MetalWeight;

            FifteenPoundsMetalWeightValue = CommonViewModel.Current.Data.DataAccess.GetTankTypes(2).MetalWeight;

            PurgeTheConsole = CommonViewModel.Current.Data.DataAccess.IsConsoleUsingPurgeFunctionality();

            IsUsingBloodPressureSensor = CommonViewModel.Current.Data.DataAccess.IsUsingBloodPressureSensor();

            IsUsingLowFlow = CommonViewModel.Current.Data.DataAccess.IsSystemUsingLowFlow();

            DeactivateFeatuers = CommonViewModel.Current.Data.DataAccess.IsConsoleUsingCatheterDeflateSwitchFunctionality();

            EnableDefalteAfterThaw = CommonViewModel.Current.Data.DataAccess.IsConsoleUsingDeflateAfterThawFunctionality();

            SerialNumber = CommonViewModel.Current.Data.DataAccess.GetConsoleSerialNumber();

            ActivateBalloonRampDown = CommonViewModel.Current.Data.DataAccess.IsConsoleUsingBalloonRampDownFunctionality();
            BalloonRampDown.IsBalloonRampDownActivated = ActivateBalloonRampDown;

            this.SaveCommand = new DelegateCommand<object>(this.OnSaveCommand, this.CanSaveCommand);

            this.PurgeCommand = new DelegateCommand<object>(this.OnPurgeCommand, this.CanPurgeCommand);

            this.DeactivateFeatuersCommand = new DelegateCommand<object>(this.OnDeactivateFeatuersCommand, this.CanDeactivateFeatuersCommand);

            this.OcclusionPressureSensorCommand = new DelegateCommand<object>(this.OnOcclusionPressureSensorCommand, this.CanOcclusionPressureSensorCommand);

            this.EnableDefalteAfterThawCommand = new DelegateCommand<object>(this.OnEnableDefalteAfterThawCommand, this.CanEnableDefalteAfterThawCommand);

            this.DeleteSiteInformarionsCommand = new DelegateCommand<object>(this.OnDeleteSiteInformarionsCommand, this.CanDeleteSiteInformarionsCommand);

            this.WeightUnitCommand = new DelegateCommand<object>(this.OnWeightUnitCommand, this.CanWeightUnitCommand);

            this.BalloonRampDownCommand = new DelegateCommand<object>(this.OnBalloonRampDownCommand, this.CanBalloonRampDownCommand);

            this.BalloonRampDownCommand = new DelegateCommand<object>(this.OnBalloonRampDownCommand, this.CanBalloonRampDownCommand);

            //Metal weight command
            this.TenPoundsMetalWeightValueCommand = new DelegateCommand<object>(this.OnTenPoundsMetalWeightValueCommand, this.CanTenPoundsMetalWeightValueCommand);

            this.SaveComPortCommand = new DelegateCommand<object>(this.OnSaveComPortCommand, this.CanSaveComPortCommand);

            this.FifteenPoundsMetalWeightValueCommand = new DelegateCommand<object>(this.OnFifteenPoundsMetalWeightValueCommand, this.CanFifteenPoundsMetalWeightValueCommand);

            this.OpenComPortCommand = new DelegateCommand<object>(this.OnOpenComPortCommand, this.CanOpenComPortCommand);

            this.CloseComPortCommand = new DelegateCommand<object>(this.OnCloseComPortCommand, this.CanCloseComPortCommand);

            this.ReadValuesFromDBCommand = new DelegateCommand<object>(this.OnReadValuesFromDBCommand, this.CanReadValuesFromDBCommand);

            this.LowFlowCommand = new DelegateCommand<object>(this.OnLowFlowCommand, this.CanLowFlowCommand);

            this.EnhancedAudioCommand = new DelegateCommand<object>(this.OnEnhancedAudioCommand, this.CanEnhancedAudioCommand);

            hospitalInformationValidator = new HospitalInformationValidator();

            originalLanguageId = CommonViewModel.Current.Data.DataAccess.GetCurrentLanguageId();
            if (originalLanguageId > 0 && LanguageList != null && LanguageList.Count > 0)
            {
                foreach (Language language in LanguageList)
                {
                    if (language.Id == originalLanguageId)
                    {
                        SelectedLanguage = language;
                    }
                }
            }

            if (CommonViewModel.Current.Data.DataAccess.GetCurrentWeightUnit() == (int)Enumeration.WeightUnit.Lbs)
            {
                IsPoundUnitActivated = true;
                IsInchesUnitActivated = true;
            }
            else
            {
                IsPoundUnitActivated = false;
                IsInchesUnitActivated = false;
            }

            /*   if (CommonViewModel.Current.Data.DataAccess.GetPrinterStatus() == 1)
                   HasPrinter = true;
               else
                   HasPrinter = false;*/

            IsDeletingCompleted = false;

            PortName = CommonViewModel.Current.Data.DataAccess.GetLSPROComPort();

            IsPortComValid();
        }



        /// <summary>
        /// This property gets/sets the Hospital Name.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// 
        public string HospitalName
        {
            get
            {
                return hospitalName;
            }

            set
            {
                hospitalName = value;
                RaisePropertyChanged("HospitalName");
            }
        }

        /// <summary>
        /// This property gets/sets the Hospital's Address.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string Address
        {
            get
            {
                return address;
            }

            set
            {
                address = value;
                RaisePropertyChanged("Address");
            }
        }

        /// <summary>
        /// This property gets/sets the Hospital's City.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string City
        {
            get
            {
                return city;
            }

            set
            {
                city = value;
                RaisePropertyChanged("City");
            }
        }

        /// <summary>
        /// This property gets/sets the Hospital's ZIP Code.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string ZIPCode
        {
            get
            {
                return zIPCode;
            }

            set
            {
                zIPCode = value;
                RaisePropertyChanged("ZIPCode");
            }
        }

        /// <summary>
        /// This property gets/sets the Hospital's Phone Number.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string PhoneNumber
        {
            get
            {
                return phoneNumber;
            }

            set
            {
                phoneNumber = value;
                RaisePropertyChanged("PhoneNumber");
            }
        }

        /// <summary>
        /// This property gets/sets the Is Hospital Info Valid flag (boolean).
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsHospitalInfoValid
        {
            get
            {
                return isHospitalInfoValid;
            }

            set
            {
                isHospitalInfoValid = value;
                RaisePropertyChanged("IsHospitalInfoValid");
            }
        }

        /// <summary>
        /// This property gets/sets the Hospital's State.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string State
        {
            get
            {
                return state;
            }

            set
            {
                state = value;
                RaisePropertyChanged("State");
            }
        }

        /// <summary>
        /// This property gets/sets the Hospital's Country.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string Country
        {
            get
            {
                return country;
            }

            set
            {
                country = value;
                RaisePropertyChanged("Country");
            }
        }

        /// <summary>
        /// This property gets/sets the Hospital Information object.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public HospitalInformation HospitalInformation
        {
            get
            {
                return hospitalInformation;
            }

            set
            {
                hospitalInformation = value;
            }
        }

        /// <summary>
        /// Function that returns if the system can invoke the Save Command.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command parameter (not used in this function).</param>
        /// <returns>Boolean value if the system can invoke the command.</returns>
        private bool CanSaveCommand(object arg)
        {
            return true;
        }

        /// <summary>
        /// Function/Command that handles the Hospital property save when the Save command is invoke.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="obj">The command parameter (not used in this function).</param>
        private void OnSaveCommand(object obj)
        {
            if (this.HospitalName == null || this.Address == null || this.City == null || this.State == null || this.ZIPCode == null ||
                this.Country == null || this.SelectedLanguage == null)
                return;


            //Verify if, for the selected language, the translation is available.
            if (SelectedLanguage != null && !CommonViewModel.Current.Data.DataAccess.IsLanguageTranslated(SelectedLanguage.Id))
            {
                Tuple<long, string, string, string> genericMessage = Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID84, (int)Enumeration.ErrorTypes.GUI);

                MessagePopup dialogPopup = new MessagePopup(genericMessage, MessagePopup.MessageType.WarningMessage, MessagePopup.ButtonType.Ok);
                dialogPopup.ShowDialog();
                return;
            }

            if (HospitalInformation == null)
            {
                CommonViewModel.Current.Data.DataAccess.AddHospital(this.HospitalName, this.Address, this.City, this.State, this.ZIPCode, this.Country, this.PhoneNumber);
                CommonViewModel.Current.Data.DataAccess.SetConsoleSerialNumber(SerialNumber);

                if (IsPoundUnitActivated)
                {
                    CommonViewModel.Current.Data.DataAccess.SetWeightUnit((short)Enumeration.WeightUnit.Lbs);
                    Scale.CurrentWeightUnit = Enumeration.WeightUnit.Lbs;

                    CommonViewModel.Current.Data.DataAccess.SetLenghtUnit((short)Enumeration.LengthUnit.Inches);
                    Toise.CurrentToiseUnit = Enumeration.LengthUnit.Inches;

                }
                else
                {
                    CommonViewModel.Current.Data.DataAccess.SetWeightUnit((short)Enumeration.WeightUnit.Kg);
                    Scale.CurrentWeightUnit = Enumeration.WeightUnit.Kg;

                    CommonViewModel.Current.Data.DataAccess.SetLenghtUnit((short)Enumeration.LengthUnit.Centimeters);
                    Toise.CurrentToiseUnit = Enumeration.LengthUnit.Centimeters;
                }
                /*  if (HasPrinter)
                  {
                      CommonViewModel.Current.Data.DataAccess.SetPrinterStatus(1);
                  }
                  else
                  {
                      CommonViewModel.Current.Data.DataAccess.SetPrinterStatus(0);
                  }
  */

            }
            else
            {
                HospitalInformation.Name = HospitalName;
                HospitalInformation.Address = Address;
                HospitalInformation.City = City;
                HospitalInformation.PostalCode = ZIPCode;
                HospitalInformation.PhoneNumber = PhoneNumber;
                HospitalInformation.State = State;
                HospitalInformation.Country = Country;

                if (SelectedLanguage.Id != originalLanguageId)
                {
                    Languages.CurrentLanguage = SelectedLanguage;
                    Tuple<long, string, string, string> genericMessage = Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID85, (int)Enumeration.ErrorTypes.GUI);

                    MessagePopup dialogPopup = new MessagePopup(genericMessage, MessagePopup.MessageType.SystemMessage, MessagePopup.ButtonType.Ok);
                    dialogPopup.ShowDialog();
                }


                CommonViewModel.Current.Data.DataAccess.UpdateHospital(HospitalInformation);
                CommonViewModel.Current.Data.DataAccess.SetConsoleSerialNumber(SerialNumber);

                if (IsPoundUnitActivated)
                {
                    CommonViewModel.Current.Data.DataAccess.SetWeightUnit((short)Enumeration.WeightUnit.Lbs);
                    Scale.CurrentWeightUnit = Enumeration.WeightUnit.Lbs;

                    CommonViewModel.Current.Data.DataAccess.SetLenghtUnit((short)Enumeration.LengthUnit.Inches);
                    Toise.CurrentToiseUnit = Enumeration.LengthUnit.Inches;
                }
                else
                {
                    CommonViewModel.Current.Data.DataAccess.SetWeightUnit((short)Enumeration.WeightUnit.Kg);
                    Scale.CurrentWeightUnit = Enumeration.WeightUnit.Kg;

                    CommonViewModel.Current.Data.DataAccess.SetLenghtUnit((short)Enumeration.LengthUnit.Centimeters);
                    Toise.CurrentToiseUnit = Enumeration.LengthUnit.Centimeters;
                }
                /*    if (HasPrinter)
                    {
                        CommonViewModel.Current.Data.DataAccess.SetPrinterStatus(1);
                    }
                    else
                    {
                        CommonViewModel.Current.Data.DataAccess.SetPrinterStatus(0);
                    }
    */
            }

            
        }

        /// <summary>
        /// Returns if the system can invoke the purge command.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private bool CanPurgeCommand(object arg)
        {
            return true;
        }

        /// <summary>
        /// Handles purge command.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void OnPurgeCommand(object obj)
        {
            if (PurgeTheConsole)
            {
                PurgeTheConsole = false;
            }

            else
            {
                PurgeTheConsole = true;
            }

            CommonViewModel.Current.Data.DataAccess.SetPurgeFunctionality(PurgeTheConsole);
        }

        /// <summary>
        /// Returns if the system can invoke the deactivate features command.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private bool CanDeactivateFeatuersCommand(object arg)
        {
            return true;
        }

        /// <summary>
        /// Returns if the system can invoke the occlusion pressure sensor command.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private bool CanOcclusionPressureSensorCommand(object arg)
        {
            return true;
        }


        /// <summary>
        /// Handles occlusion pressure sensor command.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void OnOcclusionPressureSensorCommand(object obj)
        {
            if (IsUsingBloodPressureSensor)
            {
                IsUsingBloodPressureSensor = false;
            }

            else
            {
                IsUsingBloodPressureSensor = true;
            }

            CommonViewModel.Current.Data.DataAccess.SetUsingBloodPressureSensor(IsUsingBloodPressureSensor);
        }


        /// <summary>
        /// Handles deactivate features command.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void OnDeactivateFeatuersCommand(object obj)
        {
            if (DeactivateFeatuers)
            {
                DeactivateFeatuers = false;
            }

            else
            {
                DeactivateFeatuers = true;
            }

            CommonViewModel.Current.Data.DataAccess.SetCatheterDeflateSwitchFunctionality(DeactivateFeatuers);
        }


        /// <summary>
        /// Returns if the system can invoke the deactivate features command.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private bool CanDeleteSiteInformarionsCommand(object arg)
        {
            return true;
        }


        /// <summary>
        /// Handles deactivate features command.
        /// . Safety classification: Death or SERIOUS INJURY is possible (IEC 62304 Class C).
        /// </summary>
        /// <id>SF-SDS-0006</id>
        public void OnEnableDefalteAfterThawCommand(object obj)
        {
            if (EnableDefalteAfterThaw)
            {
                EnableDefalteAfterThaw = false;
                CommonViewModel.Current.Console.DeflateAfterThaw = false;
            }

            else
            {
                EnableDefalteAfterThaw = true;
                CommonViewModel.Current.Console.DeflateAfterThaw = true;
            }

            CommonViewModel.Current.Data.DataAccess.SetDefalteAfterThawFunctionality(EnableDefalteAfterThaw);
        }


        /// <summary>
        /// Returns if the system can invoke the deactivate features command.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private bool CanEnableDefalteAfterThawCommand(object arg)
        {
            return true;
        }

        /// <summary>
        /// Handles deactivate features command.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void OnDeleteSiteInformarionsCommand(object obj)
        {

            IsDeletingStarted = false;

            MessagePopup dialogPopup = new MessagePopup("Are you sure that you want to delete? This will erase all patient, users and procedure data !", messageType: MessagePopup.MessageType.WarningMessage);

            try
            {

                if ((bool)dialogPopup.ShowDialog())
                {
                    NumberOfFiles = 0;
                    NumberOfDeletedFile = 0;
                    ProgressPercentage = 0;

                    IsDeletingStarted = true;

                    string BackupFolder = @"C:\BSC_Backup\";
                    if (Directory.Exists(BackupFolder)) Directory.Delete(BackupFolder, true);


                    var procedureTask = CommonViewModel.Current.Data.DataAccess.DeleteAllProcedures();

                    var filesTask = DeleteAllFiles();

                    var siteSetupTask = DeleteSiteSetupInformation();


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
        }

        #region Power OFF

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

        #endregion


        #region Weight Unit
        /// <summary>
        /// Returns if the system can invoke the deactivate features command.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private bool CanWeightUnitCommand(object arg)
        {
            return true;
        }

        /// <summary>
        /// Handles deactivate features command.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void OnWeightUnitCommand(object obj)
        {

        }



        #endregion


        #region Balloon Ramp up 
        /// <summary>
        /// Returns if the system can invoke the deactivate features command.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private bool CanBalloonRampDownCommand(object arg)
        {
            return true;
        }

        /// <summary>
        /// Handles deactivate features command.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void OnBalloonRampDownCommand(object obj)
        {

        }



        #endregion

        #region Metal weight

        /// <summary>
        /// Can ten pounds metal weight value command.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg"> Argument</param>
        /// <returns>True if we can save</returns>
        private bool CanTenPoundsMetalWeightValueCommand(object arg)
        {
            return true;
        }

        /// <summary>
        /// Handles deactivate features command.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void OnTenPoundsMetalWeightValueCommand(object obj)
        {
            CommonViewModel.Current.Data.DataAccess.ChangeTankMetalWeight(1, TenPoundsMetalWeightValue);
        }

        /// <summary>
        /// Can fifteen pounds metal weight value command.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg"> Argument</param>
        /// <returns>True if we can save</returns>
        private bool CanFifteenPoundsMetalWeightValueCommand(object arg)
        {
            return true;
        }


        /// <summary>
        /// Can save com port command.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg"> Argument</param>
        /// <returns>True if we can save</returns>
        private bool CanSaveComPortCommand(object arg)
        {
            return true;
        }

        /// <summary>
        /// Save com port command.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="obj">Object</param>
        private void OnSaveComPortCommand(object obj)
        {
            if (IsPortComValid())
            {
                CommonViewModel.Current.Data.DataAccess.SetLSPROComPort(PortName);
                CommonViewModel.Current.SpManager.CurrentSerialSettings.PortName = PortName;
            }
                
        }


        /// <summary>
        /// Can close com port command.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg"> Argument</param>
        /// <returns>True if we can save</returns>
        private bool CanCloseComPortCommand(object arg)
        {
            return true;
        }

        /// <summary>
        /// Close com port command.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="obj">Object</param>
        private void OnCloseComPortCommand(object obj)
        {
            if(IsPortComValid())
            CommonViewModel.Current.SpManager.StopListening();
        }

        private bool CanOpenComPortCommand(object arg)
        {
            return true;
        }

        private void OnOpenComPortCommand(object obj)
        {
            if (IsPortComValid())
                CommonViewModel.Current.SpManager.StartListening();
        }

        /// <summary>
        /// Handles deactivate features command.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void OnFifteenPoundsMetalWeightValueCommand(object obj)
        {
            CommonViewModel.Current.Data.DataAccess.ChangeTankMetalWeight(2, FifteenPoundsMetalWeightValue);
        }

        private bool CanReadValuesFromDBCommand(object arg)
        {
            return true;
        }

        /// <summary>
        /// Handles deactivate features command.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void OnReadValuesFromDBCommand(object obj)
        {
            TenPoundsMetalWeightValue = CommonViewModel.Current.Data.DataAccess.GetTankTypes(1).MetalWeight;
            FifteenPoundsMetalWeightValue = CommonViewModel.Current.Data.DataAccess.GetTankTypes(2).MetalWeight;
        }


        private bool CanLowFlowCommand(object arg)
        {
            return true;
        }

        /// <summary>
        /// Handles deactivate features command.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void OnLowFlowCommand(object obj)
        {
            if (IsUsingLowFlow)
            {
                IsUsingLowFlow = false;

            }

            else
            {
                IsUsingLowFlow = true;
            }

            CommonViewModel.Current.Data.DataAccess.SetLowFlowFunctionality(IsUsingLowFlow);
        }


        #endregion

        private bool CanEnhancedAudioCommand(object arg)
        {
            return true;
        }

        /// <summary>
        /// Handles deactivate features command.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void OnEnhancedAudioCommand(object obj)
        {
            if (EnabaleEnhancedAudio)
            {
                EnabaleEnhancedAudio = false;

            }
            else
            {
                EnabaleEnhancedAudio = true;
            }
        }


        /// <summary>
        /// Gets a list of all languages.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public List<Language> LanguageList
        {
            get
            {
                return Languages.GetAllLanguage();
            }
        }

        /// <summary>
        /// Gets/sets the selected language.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public Language SelectedLanguage
        {
            get
            {
                return selectedLanguage;
            }

            set
            {
                selectedLanguage = value;
                RaisePropertyChanged("SelectedLanguage");
            }
        }

        /// <summary>
        /// Gets/sets the min-length of hospital name.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int HospitalNameMinLength
        {
            get
            {
                return HospitalInformationValidator.nameMinLenght;
            }
        }
        /// <summary>
        /// Gets the error message of hospital name min-length.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string HospitalNameMinLengthError
        {
            get
            {
                return "Min " + HospitalNameMinLength + " characters!";
            }
        }
        /// <summary>
        /// Gets min-length of address(hospital).
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int HospitalAddressMinLength
        {
            get
            {
                return HospitalInformationValidator.adressMinLenght;
            }
        }

        /// <summary>
        /// Gets the error message of address(hospital) min-length.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string HospitalAddressMinLengthError
        {
            get
            {
                return "Min " + HospitalAddressMinLength + " characters!";
            }
        }

        /// <summary>
        /// Gets min-length of city name(hospital).
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int HospitalCityMinLength
        {
            get
            {
                return HospitalInformationValidator.cityMinLenght;
            }
        }
        /// <summary>
        /// Gets the error message of city(hospital) min-length.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string HospitalCityMinLengthError
        {
            get
            {
                return "Min " + HospitalCityMinLength + " characters!";
            }
        }
        /// <summary>
        /// Gets min-length of state name(hospital).
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int HospitalStateMinLength
        {
            get
            {
                return HospitalInformationValidator.stateMinLenght;
            }
        }
        /// <summary>
        /// Gets the error message of state(hospital) min-length.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string HospitalStateMinLengthError
        {
            get
            {
                return "Min " + HospitalStateMinLength + " characters!";
            }
        }
        /// <summary>
        /// Gets min-length of ZIP code(hospital).
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int HospitalZIPCodeMinLength
        {
            get
            {
                return HospitalInformationValidator.postalCodeMinLenght;
            }
        }
        /// <summary>
        /// Gets the error message of ZIP code(hospital) min-length.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string HospitalZIPCodeMinLengthError
        {
            get
            {
                return "Min " + HospitalZIPCodeMinLength + " characters!";
            }
        }
        /// <summary>
        /// Gets min-length of country name(hospital).
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int HospitalCountryMinLength
        {
            get
            {
                return HospitalInformationValidator.coutryMinLenght;
            }
        }

        /// <summary>
        /// Gets the error message of country(hospital) min-length.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string HospitalCountryMinLengthError
        {
            get
            {
                return "Min " + HospitalCountryMinLength + " characters!";
            }
        }
        /// <summary>
        /// Gets min-length of phone number(hospital).
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int HospitalPhoneNumberMinLength
        {
            get
            {
                return HospitalInformationValidator.phoneMinLength;
            }
        }
        /// <summary>
        /// Gets the error message of phone(hospital) min-length.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string HospitalPhoneNumberMinLengthError
        {
            get
            {
                return "Min " + HospitalPhoneNumberMinLength + " characters!";
            }
        }
        /// <summary>
        /// Gets/sets the serial number.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string SerialNumber
        {
            get
            {
                return serialNumber;
            }

            set
            {
                serialNumber = value;
                RaisePropertyChanged("SerialNumber");
            }
        }

        /// <summary>
        /// Gets/sets the value indicating whether purge the console.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool PurgeTheConsole
        {
            get
            {
                return CommonViewModel.Current.Console.PurgeTheConsole;
            }

            set
            {
                CommonViewModel.Current.Console.PurgeTheConsole = value;
                RaisePropertyChanged("PurgeTheConsole");
            }

        }

             /// <summary>
        /// Gets/sets the value indicating whether purge the console.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsUsingBloodPressureSensor
        {
            get
            {
                return CommonViewModel.Current.Console.IsUsingBloodPressureSensor;
            }

            set
            {
                CommonViewModel.Current.Console.IsUsingBloodPressureSensor = value;
                RaisePropertyChanged("IsUsingBloodPressureSensor");
                
            }

        }

        /// <summary>
        /// Gets/sets the value indicating whether low flow is using on the console.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsUsingLowFlow
        {
            get
            {
                return CommonViewModel.Current.IsUsingLowFlow;
            }

            set
            {
                CommonViewModel.Current.IsUsingLowFlow = value;
                RaisePropertyChanged("IsUsingLowFlow");

            }

        }



        

        /// <summary>
        /// Gets/sets the value indicating whether deactivate features.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool DeactivateFeatuers
        {
            get
            {
                return CommonViewModel.Current.Console.DeactivateFeatuers;
            }

            set
            {
                CommonViewModel.Current.Console.DeactivateFeatuers = value;
                RaisePropertyChanged("DeactivateFeatuers");
            }

        }

        /// <summary>
        /// Gets/sets the value indicating whether deactivate features.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool EnableDefalteAfterThaw
        {
            get
            {
                return CommonViewModel.Current.Console.EnableDefalteAfterThaw;
            }

            set
            {
                CommonViewModel.Current.Console.EnableDefalteAfterThaw = value;
                RaisePropertyChanged("EnableDefalteAfterThaw");
            }

        }



        /// <summary>
        /// Gets/sets the value indicating whether enabale enhanced audio.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool EnabaleEnhancedAudio
        {
            get
            {
                return CommonViewModel.Current.Console.EnabaleEnhancedAudio;
            }

            set
            {
                CommonViewModel.Current.Console.EnabaleEnhancedAudio = value;
                RaisePropertyChanged("EnabaleEnhancedAudio");
            }

        }

        /// <summary>
        /// Gets/sets the value indicating whether the system is able to ablate.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        //public bool IsAbleToDelete
        //{
        //    get
        //    {
        //        return true;
        //    }
        //    //set
        //    //{

        //    //}
        //} 

        /// <summary>
        /// Gets/sets the value indicating whether is pound unit activated.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsPoundUnitActivated
        {
            get
            {
                return isPoundUnitActivated;
            }
            set
            {
                isPoundUnitActivated = value;
                RaisePropertyChanged("IsPoundUnitActivated");
            }
        }

        /*  public bool HasPrinter
          {
              get
              {
                  return hasPrinter;
              }
              set
              {
                  hasPrinter = value;
                  RaisePropertyChanged("HasPrinter");
              }
          }
  */
        /// <summary>
        /// Gets/sets the value indicating whether is inches unit activated.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsInchesUnitActivated
        {
            get
            {
                return isInchesUnitActivated;
            }

            set
            {
                isInchesUnitActivated = value;
                RaisePropertyChanged("IsInchesUnitActivated");
            }
        }
        /// <summary>
        /// Gets/sets the value indicating whether is balloon ramp down activated.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool ActivateBalloonRampDown
        {
            get
            {
                return activateBalloonRampDown;
            }
            set
            {
                activateBalloonRampDown = value;
                BalloonRampDown.IsBalloonRampDownActivated = activateBalloonRampDown;
                CommonViewModel.Current.Data.DataAccess.SetBalloonRampDownFunctionality(activateBalloonRampDown);
                RaisePropertyChanged("ActivateBalloonRampDown");
            }
        }

        /// <summary>
        /// Gets or sets the ten pounds metal weight value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double TenPoundsMetalWeightValue
        {
            get
            {
                return tenPoundsMetalWeightValue;
            }

            set
            {
                tenPoundsMetalWeightValue = value;
                RaisePropertyChanged("TenPoundsMetalWeightValue");
            }
        }

        /// <summary>
        /// Gets or sets the fifteen pounds metal weight value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double FifteenPoundsMetalWeightValue
        {
            get
            {
                return fifteenPoundsMetalWeightValue;
            }

            set
            {
                fifteenPoundsMetalWeightValue = value;
                RaisePropertyChanged("FifteenPoundsMetalWeightValue");
            }
        }

        /// <summary>
        /// Gets or sets the number of file
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int NumberOfFiles
        {
            get
            {
                return numberOfFiles;
            }
            set
            {

                numberOfFiles = value;
                if (numberOfFiles < 0)
                    numberOfFiles = 0;

                RaisePropertyChanged("NumberOfFiles");
            }
        }

        /// <summary>
        /// Gets or sets the number of deleted file
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int NumberOfDeletedFile
        {
            get
            {
                return numberOfDeletedFile;
            }
            set
            {

                numberOfDeletedFile = value;
                RaisePropertyChanged("NumberOfDeletedFile");
            }
        }

        /// <summary>
        /// Gets or sets the progress percentage
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

        /// <summary>
        /// Gets or sets the user action
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string Action
        {
            get
            {
                return action;
            }
            set
            {
                action = value;
                RaisePropertyChanged("Action");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is deleting completed
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsDeletingCompleted
        {
            get
            {
                return isDeletingCompleted;
            }
            set
            {
                if (!IsDeletingStarted)
                    isDeletingCompleted = true;
                else
                    isDeletingCompleted = value;

                RaisePropertyChanged("IsDeletingCompleted");

                if (isDeletingCompleted && IsDeletingStarted)
                {

#if !DEBUG
                   Task.Delay(1000).ContinueWith(t => PowerOff()); 

#endif
                }
            }
        }


        /// <summary>
        /// Gets or sets a value indicating whether is deleting started
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsDeletingStarted
        {
            get => isDeletingStarted;
            set => isDeletingStarted = value;
        }


        /// <summary>
        /// Gets the Port name list
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string PortName
        {
            get
            {
                return CommonViewModel.Current.PortName;
            }

            set
            {
                CommonViewModel.Current.PortName = value;
                RaisePropertyChanged("PortName");
                IsPortComValid(value);
                changeindex++;


            }

        }

        /// <summary>
        /// Gets the Port name list
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string[] PortNameList
        {
            get
            {

                return CommonViewModel.Current.SpManager.CurrentSerialSettings.PortNameCollection;

            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Com Port is available
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsComPortsAvailable
        {
            get
            {
                return (PortNameList.Length != 0 && PortName != string.Empty);
            }
            set
            {
                isComPortsAvailable = value;
                RaisePropertyChanged("IsComPortsAvailable");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Com Port is valid
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsComPortsValide
        {
            get
            {
                return isComPortsValide;
            }
            set
            {
                isComPortsValide = value;
                RaisePropertyChanged("IsComPortsValide");
            }
        }

        /// <summary>
        /// Gets or sets the invalid Port COM list.
        ///  . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public List<string> InvalidPortComList
        {
            get => invalidPortComList;
            set => invalidPortComList = value;
        }


        /// <summary>
        /// Gets base path.
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
        /// Removes all files in the folders.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private async Task DeleteAllFiles()
        {
            IsDeletingCompleted = false;
            int _numberOfFiles = 0;
            int _numberOfDeletedFile = 0;

            await Task.Run(() =>
            {

                string[] filePaths = Directory.GetFiles(Path.Combine(GetBasePath(), FILESTORAGE));

                NumberOfFiles = filePaths.Length - 1;

                if (NumberOfFiles >= 0)
                {
                    Action = "Deleting File Store...";
                }

                foreach (string filePath in filePaths)
                {
                    DeleteToRecycle(filePath);
                    NumberOfDeletedFile++;

                    if (NumberOfDeletedFile > NumberOfFiles)
                    {
                        NumberOfDeletedFile = NumberOfFiles;
                    }

                    if (NumberOfFiles != 0)
                        ProgressPercentage = (NumberOfDeletedFile * 100 / NumberOfFiles);
                }

                _numberOfFiles = NumberOfFiles;
                _numberOfDeletedFile = NumberOfDeletedFile;

                NumberOfFiles = 0;
                NumberOfDeletedFile = 0;

                // Delete the archives  
                string Dirctorypath = Path.Combine(GetBasePath(), ARCHIVESTORAGE);
                if (Directory.Exists(Dirctorypath))
                {
                    filePaths = Directory.GetFiles(Path.Combine(GetBasePath(), ARCHIVESTORAGE));

                    NumberOfFiles = filePaths.Length - 1;

                    if (NumberOfFiles >= 0)
                    {
                        Action = "Deleting Archives...";
                    }


                    foreach (string filePath in filePaths)
                    {
                        DeleteToRecycle(filePath);
                        NumberOfDeletedFile++;

                        if (NumberOfDeletedFile > NumberOfFiles)
                        {
                            NumberOfDeletedFile = NumberOfFiles;
                        }

                        if (NumberOfFiles != 0)
                            ProgressPercentage = (NumberOfDeletedFile * 100 / NumberOfFiles);
                    }
                }


                //EmptyRecycleBin();
            }
                );

            if (NumberOfFiles == 0 || NumberOfDeletedFile == 0)
            {
                NumberOfFiles = _numberOfFiles;
                NumberOfDeletedFile = _numberOfDeletedFile;
            }
            Action = "The system will shut down in 10sec...";

            await Task.Delay(10000).ContinueWith(t => IsDeletingCompleted = true);

        }
        /// <summary>
        /// Removes the site setup information from DB.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private async Task DeleteSiteSetupInformation()
        {
            await Task.Run(() =>
            {
                //Delete hospital site city..
                HospitalInformation.Name = "";
                HospitalInformation.Address = "";
                HospitalInformation.City = "";
                HospitalInformation.PostalCode = "";
                HospitalInformation.PhoneNumber = "1";
                HospitalInformation.State = "";
                HospitalInformation.Country = "";

                HospitalName = HospitalInformation.Name;
                Address = HospitalInformation.Address;
                City = HospitalInformation.City;
                ZIPCode = HospitalInformation.PostalCode;
                PhoneNumber = HospitalInformation.PhoneNumber;
                State = HospitalInformation.State;
                Country = HospitalInformation.Country;


                CommonViewModel.Current.Data.DataAccess.UpdateHospital(HospitalInformation);
            }
                );


        }

        /// <summary>
        /// Empty recycle bin.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private uint EmptyRecycleBin()
        {
            try
            {
                uint value = SHEmptyRecycleBin(IntPtr.Zero, null, RecycleFlags.SHERB_NOCONFIRMATION);

                return value;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return 8;

            }

        }

        /// <summary>
        /// Delete a file to recycle
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="filePath">file path</param>
        private void DeleteToRecycle(string filePath)
        {

            if (FileSystem.FileExists(filePath))
            {
                FileSystem.DeleteFile(filePath, UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently);
            }
        }


        /// <summary>
        /// Validate the com port
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <returns>true if the port is valide </returns>
        private bool IsPortComValid()
        {
            string[] allPortComList = SerialPort.GetPortNames();
            if (PortName != string.Empty && allPortComList.Contains(PortName) && !InvalidPortComList.Contains(PortName))
            {
                IsComPortsValide = true;
                return true;
            }
            else
            {
                IsComPortsValide = false;
                return false;
            }
        }

        /// <summary>
        /// Validate the com port
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="_comPort">the name of the com port </param>
        /// <returns>true if the com port is valid</returns>
        private bool IsPortComValid(string _comPortName)
        {
            string[] allPortComList = SerialPort.GetPortNames();
            if (PortName != string.Empty && allPortComList.Contains(PortName) && !InvalidPortComList.Contains(_comPortName) && changeindex !=0)
            {
                //DisplayMessage("Save The Com Port Before Using LSPRO.", MessagePopup.MessageType.WarningMessage, MessagePopup.ButtonType.Ok, "SAVE!");
                return (IsComPortsValide = true);
            }
            else
            {
                return (IsComPortsValide = false);
            }
        }

        /// <summary>
        /// Display warning message for port communication saving
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="message">message to display</param>
        /// <param name="messageType">message type</param>
        /// <param name="buttonType">button type</param>
        /// <param name="titleMessage">title message</param>
        internal void DisplayMessage(String message, MessagePopup.MessageType messageType, MessagePopup.ButtonType buttonType, string titleMessage)
        {

            Application.Current.Dispatcher.Invoke((System.Action)delegate
            {

                MessagePopup messagePopup = new MessagePopup(message, messageType, buttonType, titleMessage);
                messagePopup.ShowDialog();
            });

        }

    }
}