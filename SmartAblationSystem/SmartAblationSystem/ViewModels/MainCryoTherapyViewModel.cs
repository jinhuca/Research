using Communication;
using DataAccessLayer;
using DevExpress.Mvvm.Native;
using MahApps.Metro.Controls;
using PDFReportsGenerator;
using Shared;
using SmartAblationSystem.Helpers;
using SmartAblationSystem.Models;
using SmartAblationSystem.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Management;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UniversalLoginManager;
using static LogSystem.LogService;
using Action = System.Action;
using BindableBase = Prism.Mvvm.BindableBase;
using Patient = SmartAblationSystem.Views.Patient;

namespace SmartAblationSystem.ViewModels
{
	/// <summary>
	/// This class is the Main Cryotherapy View Model
	/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
	/// </summary>
	public class MainCryoTherapyViewModel : BindableBase, IDataExportable
	{
		private UserControl currentMainCryoTherapyView;
		private UserControl cryoTherapyView;
		private UserControl patientView;
		private UserControl reportView;
		private UserControl homeView;
		private ViewsEventArgs viewsEvent;
		private readonly DataAccess _dataAccess;

		public ICommand NavigateToViewCommand { get; private set; }
		public ICommand IncrementSystemStateCommand { get; private set; }
		public ICommand SetUnknownSystemStateCommand { get; private set; }
		public ICommand SetExceptionSystemStateCommand { get; private set; }
		public ICommand IncrementTemperatureCommand { get; private set; }
		public ICommand DecrementTemperatureCommand { get; private set; }
		public ICommand ErrorCommand { get; private set; }
		public ICommand TogglePressureSensorCommand { get; private set; }
		public ICommand IncreaseBloodPressureCommand { get; private set; }
		public ICommand DecreaseBloodPressureCommand { get; private set; }
		public ICommand EndProcedureCommand { get; private set; }
		public ICommand ExportCurrentProcedureCommand { get; }
		public ICommand PrintPDFCommand { get; }
		public ICommand ReturnToProcedureCommand { get; private set; }
		public ICommand CompleteProcedureCommand { get; private set; }
		public ICommand ExitPlayBackCommand { get; private set; }

		public ICommand PlayBackCommand { get; private set; }
		public ICommand ResetTherapyCommand { get; private set; }

		public ICommand ProxIncreaseCommand { get; private set; }
		public ICommand ProxDecreaseCommand { get; private set; }
		public ICommand ETSIncreaseCommand { get; private set; }
		public ICommand ETSDecreaseCommand { get; private set; }

		public ICommand MultSensorCommand { get; private set; }
		public ICommand MultSensorDCommand { get; private set; }
		public ICommand SimulationModeCommand { get; private set; }

		public string ExceptionMessage { get; set; }
		public event EventHandler USBExportProgressEvent;

#if Simulator

		bool valueReseted = false;
		double[] bloodPressureSimValue = { 35, 35, 35, 35 };

#endif

		#region Constants

		private const string DoubleDash = "--";
		private const string DateFormatString = "MM/dd/yyyy";
		private const string Gender = "GENDER";
		private const string Weight = "WEIGHT";
		private const string Height = "HEIGHT";
		private const string BMI = "BMI";
		private const string DoctorTitle = "Dr. ";
		private const string Whitespace = " ";

		#endregion Constants

		private void GetUserType()
		{
			if(CommonViewModel.Current.IsCryterionUser)
			{
				_accessControlType = LoginManager.AccessControlType.CRYTERION;    // BSC
				_userType = UserType.Bsc;
			}
			else if(CommonViewModel.Current.IsBSCADMINUser)
			{
				_accessControlType = LoginManager.AccessControlType.BSCADMIN;
				_userType = UserType.BostonBsc;
			}
			else if(CommonViewModel.Current.IsDoctor)
			{
				_accessControlType = LoginManager.AccessControlType.DOCTOR;
				_userType = UserType.Doctor;
			}
			else if(CommonViewModel.Current.IsAdminUser)
			{
				_accessControlType = LoginManager.AccessControlType.ADMIN;
				_userType = UserType.Admin;
			}
			else
			{
				_accessControlType = LoginManager.AccessControlType.USER;
				_userType = UserType.User;
			}
		}

		/// <summary>
		/// This constructor initializes the Main Cryotherapy View Model's properties and commands
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public MainCryoTherapyViewModel()
		{
			PatientView = new Patient();
			CurrentMainCryoTherapyView = PatientView;

			// Build also the cryo therapy heavy loading
			// this.CryoTherapyView = new CryoTherapy();

			NavigateToViewCommand = new Prism.Commands.DelegateCommand<object>(OnNavigateToView, CanNavigateToView);
			IncrementSystemStateCommand = new Prism.Commands.DelegateCommand<object>(OnIncrementSystemStateCommand, CanIncrementSystemStateCommand);
			SetUnknownSystemStateCommand = new Prism.Commands.DelegateCommand<object>(OnSetUnknownSystemStateCommand, CanSetUnknownSystemStateCommand);
			SetExceptionSystemStateCommand = new Prism.Commands.DelegateCommand<object>(OnSetExceptionSystemStateCommand, CanSetExceptionSystemStateCommand);
			IncrementTemperatureCommand = new Prism.Commands.DelegateCommand<object>(OnIncrementTemperatureCommand, CanChangeTemperatureCommand);
			DecrementTemperatureCommand = new Prism.Commands.DelegateCommand<object>(OnDecrementTemperatureCommand, CanChangeTemperatureCommand);
			ErrorCommand = new Prism.Commands.DelegateCommand<object>(OnErrorCommand, CanErrorCommand);
			TogglePressureSensorCommand = new Prism.Commands.DelegateCommand<object>(OnTogglePressureSensorCommand, CanTogglePressureSensorCommand);
			IncreaseBloodPressureCommand = new Prism.Commands.DelegateCommand<object>(OnIncreaseBloodPressureCommand, CanIncreaseBloodPressureCommand);
			DecreaseBloodPressureCommand = new Prism.Commands.DelegateCommand<object>(OnDecreaseBloodPressureCommand, CanDecreaseBloodPressureCommand);
			EndProcedureCommand = new Prism.Commands.DelegateCommand<object>(OnEndProcedureCommand, CanEndProcedureCommand);
			ReturnToProcedureCommand = new Prism.Commands.DelegateCommand<object>(OnReturnToProcedureCommand, CanReturnToProcedureCommand);
			CompleteProcedureCommand = new Prism.Commands.DelegateCommand<object>(OnCompleteProcedureCommand, CanCompleteProcedureCommand);
			ResetTherapyCommand = new Prism.Commands.DelegateCommand<object>(OnResetTherapyCommand, CanResetTherapyCommand);
			ExportCurrentProcedureCommand = new Prism.Commands.DelegateCommand<object>(OnExportProcedureCommand, CanExportProcedure);
			PrintPDFCommand = new Prism.Commands.DelegateCommand<object>(OnPrintPDFCommand, CanPrintPDF);
			ExitPlayBackCommand = new Prism.Commands.DelegateCommand<object>(OnPlayBackCommand, CanPlayBackCommand);
			PlayBackCommand = new Prism.Commands.DelegateCommand<object>(OnReloadPlayBackCommand, CanReloadPlayBackCommand);
			ProxIncreaseCommand = new Prism.Commands.DelegateCommand<object>(OnProxIncreaseCommand, CanProxIncreaseCommand);
			ProxDecreaseCommand = new Prism.Commands.DelegateCommand<object>(OnProxDecreaseCommand, CanProxDecreaseCommand);
			ETSIncreaseCommand = new Prism.Commands.DelegateCommand<object>(OnETSIncreaseCommand, CanETSIncreaseCommand);
			ETSDecreaseCommand = new Prism.Commands.DelegateCommand<object>(OnETSDecreaseCommand, CanETSDecreaseCommand);
			MultSensorCommand = new Prism.Commands.DelegateCommand<object>(OnMultSensorCommand, CanMultSensorCommand);
			MultSensorDCommand = new Prism.Commands.DelegateCommand<object>(OnMultSensorDCommand, CanMultSensorDCommand);
			SimulationModeCommand = new Prism.Commands.DelegateCommand<object>(OnSimulationModeCommand, CanSimulationModeCommand);
			CommonViewModel.Current.PropertyChanged += Current_PropertyChanged;
			viewsEvent = new ViewsEventArgs();
			_usbDriveConnectionManager = new USBDriveConnectionManager.USBDriveConnectionManager(USBDriveConnection_EventArrived);
			USBDriveList = _usbDriveConnectionManager.GetUSBDriveList();
			IsExportingCurrentProcedure = true;
			_dataAccess = CommonViewModel.Current.Data.DataAccess;
			GetUserType();
		}

		private bool CanETSDecreaseCommand(object arg)
		{
			return true;
		}

		private void OnETSDecreaseCommand(object obj)
		{
#if Simulator
			CommonViewModel.Current.DeacreaeSesnorSimulation();
#endif
		}

		private bool CanETSIncreaseCommand(object arg)
		{
			return true;
		}

		private void OnETSIncreaseCommand(object obj)
		{
#if Simulator
			CommonViewModel.Current.IncreaseSesnorSimulation();
#endif
		}

		private bool CanTogglePressureSensorCommand(object arg)
		{
			return true;
		}

		private void OnTogglePressureSensorCommand(object obj)
		{
#if Simulator
			if(CommonViewModel.Current.IsBloodPressureSensorConnected)
				CommonViewModel.Current.IsBloodPressureSensorConnected = false;
			else
			{
				CommonViewModel.Current.IsBloodPressureSensorConnected = true;
				CommonViewModel.Current.CurrentBloodPressureValue = bloodPressureSimValue;
			}
#endif
		}

		private bool CanIncreaseBloodPressureCommand(object arg)
		{
			return true;
		}

		private void OnIncreaseBloodPressureCommand(object obj)
		{
#if Simulator
			/*double[] _bloodPressureValue = new double[4];
      for (int i = 0; i < 4; i++)
      {
          pres_cnt = ((pres_cnt + 1) % Maxpres);
          _bloodPressureValue[i] = (double)(pres_cnt + 5);
      }
      CommonViewModel.Current.BloodPressureValue = _bloodPressureValue;
      ((CryoTherapyViewModel)cryoTherapyView.DataContext).EcgChannel1And2Reading = _bloodPressureValue[3];*/
			double[] dataBloodPressureValue = bloodPressureSimValue;
			for(int index = 0; index < dataBloodPressureValue.Length; index++)
			{
				if(dataBloodPressureValue[index] < 99)
				{
					dataBloodPressureValue[index] = dataBloodPressureValue[index] + 1;
				}
			}
			CommonViewModel.Current.CurrentBloodPressureValue = dataBloodPressureValue;
			//CommonViewModel.Current.BloodPressureValue = dataBloodPressureValue;
			((CryoTherapyViewModel)cryoTherapyView.DataContext).EcgChannel1And2Reading = dataBloodPressureValue[3];
#endif
		}
		//int pres_cnt = 0;
		//int Maxpres = 80;
		private bool CanDecreaseBloodPressureCommand(object arg)
		{
			return true;
		}

		private void OnDecreaseBloodPressureCommand(object obj)
		{
#if Simulator
			double[] dataBloodPressureValue = bloodPressureSimValue;
			for(int index = 0; index < dataBloodPressureValue.Length; index++)
			{
				if(dataBloodPressureValue[index] > 0)
				{
					dataBloodPressureValue[index] = dataBloodPressureValue[index] - 1;
				}
			}
			CommonViewModel.Current.CurrentBloodPressureValue = dataBloodPressureValue;
			//CommonViewModel.Current.BloodPressureValue = dataBloodPressureValue;
			CommonViewModel.Current.EcgChannel1And2Reading = dataBloodPressureValue[3];
#endif
		}

		private bool CanProxDecreaseCommand(object arg)
		{
			return true;
		}

		private void OnProxDecreaseCommand(object obj)
		{
			CommonViewModel.Current.EcgChannel5And6Reading--;
		}

		private bool CanProxIncreaseCommand(object arg)
		{
			return true;
		}

		private void OnProxIncreaseCommand(object obj)
		{
			CommonViewModel.Current.EcgChannel5And6Reading++;
			// CommonViewModel.Current.RCWarningTimerControlPopupMessage();
		}


		private bool CanMultSensorCommand(object arg)
		{
			return true;
		}
		private void OnMultSensorCommand(object obj)
		{

			CommonViewModel.Current.IsMultiEtsSesnorConnected = true;
		}

		private bool CanMultSensorDCommand(object arg)
		{
			return true;
		}
		private void OnMultSensorDCommand(object obj)
		{
			CommonViewModel.Current.IsMultiEtsSesnorConnected = false;
		}

		private bool CanSimulationModeCommand(object obj)
		{
			return true;
		}

		private void OnSimulationModeCommand(object obj)
		{
			/*SimulationMode simulationMode = new SimulationMode(this);
      simulationMode.Show();
      simulationMode.Topmost = true;*/
		}


		/// <summary>
		/// Function that refreshes the visibility flags
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public void RefreshButtonsVisibility()
		{
			RaisePropertyChanged("IsCompleteProcedureVisible");
			RaisePropertyChanged("IsEndProcedureVisible");
			RaisePropertyChanged("IsReturnToProcedureVisible");
		}

		/// <summary>
		/// Function that returns if the system can invoke the End Procedure command
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="arg">The command parameter (not used in this function).</param>
		/// <returns>Boolean value if the system can invoke the command.</returns>
		private bool CanEndProcedureCommand(object arg)
		{
			return true;
		}

		/// <summary>
		/// Function that returns if the system can invoke the Return To Procedure command
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="arg">The command parameter (not used in this function).</param>
		/// <returns>Boolean value if the system can invoke the command.</returns>
		private bool CanReturnToProcedureCommand(object arg)
		{
			return true;
		}

		/// <summary>
		/// Function that returns if the system can invoke the Complete Procedure command
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="arg">The command parameter (not used in this function).</param>
		/// <returns>Boolean value if the system can invoke the command.</returns>
		private bool CanCompleteProcedureCommand(object arg)
		{
			return true;
		}

		/// <summary>
		/// Function/Command that handles the complete procedure operation when the Complete Procedure
		/// command is invoked
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="obj">The command's parameter (not used in this function).</param>
		private void OnCompleteProcedureCommand(object obj)
		{
			RaisePropertyChanged("IsCompleteProcedureVisible");
			RaisePropertyChanged("IsEndProcedureVisible");
			RaisePropertyChanged("IsReturnToProcedureVisible");
			CommonViewModel.Current.OnViewchanged(viewsEvent);
			CommonViewModel.Current.Console.Disconnect();
			CommonViewModel.Current.IsVacuumDisconnected = true;
		}

		/// <summary>
		/// Function that returns if the system can invoke the Reset Therapy command
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="arg">The command parameter (not used in this function).</param>
		/// <returns>Boolean value if the system can invoke the command.</returns>
		private bool CanResetTherapyCommand(object arg)
		{
			return true;
		}

		/// <summary>
		/// Function/Command that handles the Reset Therapy operations when the Reset Therapy
		/// command is invoked
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="obj">The command's parameter (not used in this function).</param>
		private void OnResetTherapyCommand(object obj)
		{
			try
			{
				((CryoTherapyViewModel)cryoTherapyView?.DataContext)?.ResetCryoTherapy();
			}
			catch(Exception ex)
			{
				LogException(ex);
			}
		}

		/// <summary>
		/// This property gets/sets the IsPlaybackModeDeactivated value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsPlayBackModeDeactivted
		{
			get
			{
				return CommonViewModel.Current.IsPlayBackModeDeactivted;
			}
			set
			{
				CommonViewModel.Current.IsPlayBackModeDeactivted = value;
				RaisePropertyChanged("IsPlayBackModeDeactivted");
			}
		}

		/// <summary>
		/// Function/Command that handles the Playback operations when the Playback
		/// command is invoked
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="arg">The command's parameter (not used in this function).</param>
		private void OnPlayBackCommand(object arg)
		{
			AblationInformation.IsThereAbltionHistoricalData = true;

			SensorReadingMananger.ConnectSensors();
			AreSensorsInPlayBackMode = false;
			TTIFSM.AreSensorsInPlayBackMode = false;
			((CryoTherapyViewModel)cryoTherapyView.DataContext).ResetCryoTherapyPlayBackData();
			((CryoTherapyViewModel)cryoTherapyView.DataContext).RefreshWeightData();

			if(((CryoTherapyViewModel)cryoTherapyView.DataContext).WasAblationTimeManuallyChanged)
			{
				((CryoTherapyViewModel)cryoTherapyView.DataContext).RequiredAblationTime = ((CryoTherapyViewModel)cryoTherapyView.DataContext).TemporaryManualAblationTime;
				((CryoTherapyViewModel)cryoTherapyView.DataContext).ISTTISelected = false;    // IsFixedTimerSelected = false;
			}
			IsPlayBackModeDeactivted = true;
			CommonViewModel.Current.ChangeBalloonTypeFSM.DASBalloonEnabled = false;
			// ((CryoTherapyViewModel)cryoTherapyView.DataContext).DataLoading = false;
		}

		/// <summary>
		/// Function that returns if the system can invoke the Playback command
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="arg">The command parameter (not used in this function).</param>
		/// <returns>Boolean value if the system can invoke the command.</returns>
		private bool CanPlayBackCommand(object arg)
		{

			return true;
			//return IsPlayBackModeDeactivted;   //false;
		}

		/// Function/Command that handles the Playback operations when the reload Playback
		/// command is invoked
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="arg">The command's parameter (not used in this function).</param>
		private void OnReloadPlayBackCommand(object arg)
		{
			// Re-activate the Blood Pressure graph if system is in ready state and occlusion pressure sensor is connected and enabled.
			if(CommonViewModel.Current.SystemState == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY && ((CryoTherapyViewModel)cryoTherapyView.DataContext).EnabledIsBloodPressureSensorConnected)
			{
				((CryoTherapyViewModel)cryoTherapyView.DataContext).IsMonitoringBloodPressure = false;
				RaisePropertyChanged("EnabledIsBloodPressureSensorConnected");
			}

			// CommonViewModel.Current.IsUsingAutoPlayback = true;
			// canReloadPlayBack = false;
			SensorReadingMananger.DisconnectSensors();
			AreSensorsInPlayBackMode = true;
			//((CryoTherapyViewModel)cryoTherapyView.DataContext).ResetCryoTherapyPlayBackData();
			IsPlayBackModeDeactivted = false;
			// ((CryoTherapyViewModel)cryoTherapyView.DataContext).DataLoading = true;

			((CryoTherapyViewModel)cryoTherapyView.DataContext).LastAblationCommand.Execute("OnReloadPlayBackCommand");
			//canReloadPlayBack = true;
		}

		//  private bool canReloadPlayBack = true;
		/// <summary>
		/// Function that returns if the system can invoke the  Reload Playback command
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="arg">The command parameter (not used in this function).</param>
		/// <returns>Boolean value if the system can invoke the command.</returns>
		private bool CanReloadPlayBackCommand(object arg)
		{
			//  return canReloadPlayBack;
			return true;
		}

		/// <summary>
		/// Function/Command that handles the End Procedure operations (flags, console, view change, reset)
		/// when the End Procedure command is invoked
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="obj">The command's parameter (not used in this function).</param>
		private void OnEndProcedureCommand(object obj)
		{
			if(obj.ToString() == "HomeAndEndingProcedure")
			{
				CommonViewModel.Current.Console.Disconnect();
				CommonViewModel.Current.Console.AblateDisable();

				CommonViewModel.Current.IsAblationProcedureEnded = false; // we have to stop the timer and put the data in the database
				CommonViewModel.Current.CanStartTherapy = false;
				CommonViewModel.Current.SkinToSkinDuration = 0;

				CurrentMainCryoTherapyView = PatientView;

				viewsEvent.ViewName = "Home";
				CommonViewModel.Current.OnViewchanged(viewsEvent);

				//If anything goes wrong, we must not use the previous patient information and/or procedure info.
				CommonViewModel.Current.CurrentPatient = null;
				CommonViewModel.Current.CurrentProcedure = null;
				((PatientViewModel)PatientView.DataContext).ResetPatientInfo();

				RaisePropertyChanged("IsCompleteProcedureVisible");
				RaisePropertyChanged("IsEndProcedureVisible");
				RaisePropertyChanged("IsReturnToProcedureVisible");
				ReadyToExport = false;
				return;
			}
			else if(obj.ToString() == "ExportedAndReturned")
			{
				CommonViewModel.Current.Console.Disconnect();
				CommonViewModel.Current.Console.AblateDisable();

				CommonViewModel.Current.IsAblationProcedureEnded = false; // we have to stop the timer and put the data in the database
				CommonViewModel.Current.CanStartTherapy = false;
				CommonViewModel.Current.SkinToSkinDuration = 0;

				CurrentMainCryoTherapyView = PatientView;
				(CurrentMainCryoTherapyView.DataContext as PatientViewModel).IsTherePatient = false;
				ProcedureLogModel.PreviousLogedPatient = null;

				viewsEvent.ViewName = "Home";
				CommonViewModel.Current.OnViewchanged(viewsEvent);

				//If anything goes wrong, we must not use the previous patient information and/or procedure info.
				CommonViewModel.Current.CurrentPatient = null;
				CommonViewModel.Current.CurrentProcedure = null;
				((PatientViewModel)PatientView.DataContext).ResetPatientInfo();

				RaisePropertyChanged("IsCompleteProcedureVisible");
				RaisePropertyChanged("IsEndProcedureVisible");
				RaisePropertyChanged("IsReturnToProcedureVisible");
				ReadyToExport = false;
				return;
			}

			Tuple<long, string, string, string> genericMessage = Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID11, (int)Enumeration.ErrorTypes.GUI);

			MessagePopup dialogPopup = new MessagePopup(genericMessage)
      {
        WindowStartupLocation = WindowStartupLocation.Manual,
        Left = 601,
        Top = 490
      };

			if((bool)dialogPopup.ShowDialog())
			{
				ExitToHome();
				ReadyToExport = false;
			}
			else
			{
				ReadyToExport = true;
			}

			void ExitToHome()
			{
				CommonViewModel.Current.Console.Disconnect();
				CommonViewModel.Current.Console.AblateDisable();

				CommonViewModel.Current.IsAblationProcedureEnded =
					false; // we have to stop the timer and put the data in the database
				CommonViewModel.Current.CanStartTherapy = false;
				CPUTimeWatchdog.IsTimerStarted = false;

				CurrentMainCryoTherapyView = PatientView;

				viewsEvent.ViewName = "Home";
				CommonViewModel.Current.OnViewchanged(viewsEvent);

				//If anything goes wrong, we must not use the previous patient information and/or procedure info.

				//Save the skin to skin time
				if(CommonViewModel.Current.SkinToSkinDuration != 0 && CommonViewModel.Current.CurrentProcedure != null)
				{
					short skinToSkinDuration = (short)CommonViewModel.Current.SkinToSkinDuration;

					if(skinToSkinDuration > 0)
					{
						ProcedureLogModel.SkinToSkinDuration = skinToSkinDuration;
						CommonViewModel.Current.SkinToSkinAblationTimer.Start();
					}

					CommonViewModel.Current.CurrentProcedure.SkinToSkinDuration =
						(short)ProcedureLogModel.SkinToSkinDurationBeforeLeavingTheCryoScreen; //skinToSkinDuration;
					CommonViewModel.Current.Data.DataAccess.UpdateProcedure(CommonViewModel.Current.CurrentProcedure);
				}

				CommonViewModel.Current.CurrentPatient = null;
				CommonViewModel.Current.CurrentProcedure = null;
				((PatientViewModel)PatientView.DataContext).ResetPatientInfo();

				RaisePropertyChanged("IsCompleteProcedureVisible");
				RaisePropertyChanged("IsEndProcedureVisible");
				RaisePropertyChanged("IsReturnToProcedureVisible");
			}
		}

		/// <summary>
		/// Function/Command that handles the Return To Procedure operations
		/// when the Return to Procedure command is invoked
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="obj">The command's parameter (not used in this function).</param>
		private void OnReturnToProcedureCommand(object obj)
		{
			CommonViewModel.Current.OnViewchanged(viewsEvent);
			RaisePropertyChanged("IsCompleteProcedureVisible");
			RaisePropertyChanged("IsEndProcedureVisible");
			RaisePropertyChanged("IsReturnToProcedureVisible");
			CommonViewModel.Current.ScreenName = "Cryo Therapy";
		}

		/// <summary>
		/// Function that returns if the system can invoke the Increment System State command
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="arg">The command parameter (not used in this function).</param>
		/// <returns>Boolean value if the system can invoke the command.</returns>
		private bool CanIncrementSystemStateCommand(object arg)
		{
			return true;
		}

		/// <summary>
		/// Function that returns if the system can invoke the Change Temperature command
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="arg">The command parameter (not used in this function).</param>
		/// <returns>Boolean value if the system can invoke the command.</returns>
		private bool CanChangeTemperatureCommand(object arg)
		{
			return true;
		}

		/// <summary>
		/// Function/Command that handles the System State Incrementation when the Increment System State
		/// command is invoked.  This command shall only be invoked by the DebugWithSimulator solution
		/// if configuration is selected
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="obj">The command's parameter (not used in this function).</param>
		private void OnIncrementSystemStateCommand(object obj)
		{

			if(CommonViewModel.Current.SystemState == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_UNKNOWN)
			{
				CommonViewModel.Current.SystemState = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE;
			}
			else if(CommonViewModel.Current.SystemState == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE)
			{
				CommonViewModel.Current.SystemState = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY;
			}
			else if(CommonViewModel.Current.SystemState == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY)
			{
				CommonViewModel.Current.SystemState = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_INFLATION;
			}
			else if(CommonViewModel.Current.SystemState == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_INFLATION)
			{
				CommonViewModel.Current.SystemState = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION;
			}
			else if(CommonViewModel.Current.SystemState == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION)
			{
				CommonViewModel.Current.SystemState = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_ABLATION;
			}
			else if(CommonViewModel.Current.SystemState == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_ABLATION)
			{
				CommonViewModel.Current.SystemState = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING;
			}
			else if(CommonViewModel.Current.SystemState == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING)
			{
				//CommonViewModel.Current.SystemState = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION;
				CommonViewModel.Current.SystemState = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE;
				Task.Delay(3000).ContinueWith(t => CommonViewModel.Current.SystemState = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY);

			}
			else if(CommonViewModel.Current.SystemState == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_EXCEPTION)
			{
				CommonViewModel.Current.SystemState = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE;
			}

			//CommonViewModel.Current.IsCMCULoadCellWeightFail = true;
			//CommonViewModel.Current.IsUserAllowedToChangeTank = true;

			//CommonViewModel.Current.WarningMessageManager.AddMessage("This is a SYSTEM message triggered in simulator mode.  Nothing really happenned!", WarningMessagesManager.WarningMessagesManagerEnumeration.MessageType.SYSTEM);
			//CommonViewModel.Current.WarningMessageManager.AddMessage("This is a WARNING message triggered in simulator mode.  Nothing really happenned!", WarningMessagesManager.WarningMessagesManagerEnumeration.MessageType.WARNING);
			//CommonViewModel.Current.WarningMessageManager.AddMessage("This is an ERROR message triggered in simulator mode.  Nothing really happenned!", WarningMessagesManager.WarningMessagesManagerEnumeration.MessageType.ERROR);
		}

		/// <summary>
		/// Function that returns if the system can invoke the Set Unknown System State command
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="arg">The command parameter (not used in this function).</param>
		/// <returns>Boolean value if the system can invoke the command.</returns>
		private bool CanSetUnknownSystemStateCommand(object arg)
		{
			return true;
		}

		/// <summary>
		/// Function/Command that handles the Unknown System State setting when the
		/// Unknown System State command is invoked
		/// This command shall only be invoked the DebugWithSimulator solution configuration is selected.
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="obj">The command's parameter (not used in this function).</param>
		private void OnSetUnknownSystemStateCommand(object obj)
		{
			// CommonViewModel.Current.SystemState = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_UNKNOWN;
		}

		/// <summary>
		/// Function that returns if the system can invoke the Set Exception System State command
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="arg">The command parameter (not used in this function).</param>
		/// <returns>Boolean value if the system can invoke the command.</returns>
		private bool CanSetExceptionSystemStateCommand(object arg)
		{
			return true;
		}

		/// <summary>
		/// Function/Command that handles the Exception System State setting when the
		/// Exception System State command is invoked
		/// This command shall only be invoked the DebugWithSimulator solution configuration is selected.
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="obj">The command's parameter (not used in this function).</param>
		private void OnSetExceptionSystemStateCommand(object obj)
		{
			//CommonViewModel.Current.GetCMCUStatusError(8);
			//CommonViewModel.Current.SystemState = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_EXCEPTION;
		}

		/// <summary>
		/// Function/Command that handles the Temperature incrementation when the Increment Temperature
		/// command is invoked.  This command shall only be invoked the DebugWithSimulator solution configuration is selected
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="obj">The command's parameter (not used in this function).</param>
		private void OnIncrementTemperatureCommand(object obj)
		{
			CommonViewModel.Current.TC1Reading += 5;  //Tank weight
			CommonViewModel.Current.LC1Reading += 5;  //Tank gage

			//Tip Pressure, 0 to 15
			if(CommonViewModel.Current.EcgChannel1And2Reading < 240)
				CommonViewModel.Current.EcgChannel1And2Reading += 10;

			//Diaphragm amplitude goes between -2 and 2 G
			if(CommonViewModel.Current.EcgChannel3And4Reading < 2)
				CommonViewModel.Current.EcgChannel3And4Reading += 0.1;

			//Esophagus temperature, set 30 value by default when in simulator mode
			if(CommonViewModel.Current.EcgChannel5And6Reading == 0)
			{
				CommonViewModel.Current.EcgChannel5And6Reading = 35;
			}

			//Esophagus temperature
			//if (CommonViewModel.Current.EcgChannel5And6Reading + 1 < 50)
			CommonViewModel.Current.EcgChannel5And6Reading += 1;

			//Diaphragm movement, set 90 value by default when in simulator mode
			if(CommonViewModel.Current.EcgChannel7And8Reading == 0)
			{
				CommonViewModel.Current.EcgChannel7And8Reading = 90;
			}

			//Diaphragm movement
			if(CommonViewModel.Current.EcgChannel7And8Reading < 100)
				CommonViewModel.Current.EcgChannel7And8Reading += 1;

			//Balloon Pressure, 0 to 10
			if(CommonViewModel.Current.CP1Reading < 10)
				CommonViewModel.Current.CP1Reading += 0.5;

			CommonViewModel.Current.LC1Reading = +1;
			CommonViewModel.Current.BloodDetecorImValue++;

			//CommonViewModel.Current.IsVeinIsolated = true;

			// CommonViewModel.Current.IsDiaphragmMovementDetected = false;

			//CommonViewModel.Current.test();



#if Simulator
			//CommonViewModel.Current.testForSimulation();


			if(!valueReseted)
			{
				ResetDataForSimulation();
				valueReseted = true;
			}
#endif
		}

		/// <summary>
		/// Function/Command that handles the Temperature decrementation when the Decrement Temperature
		/// command is invoked.  This command shall only be invoked the DebugWithSimulator solution configuration is selected
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="obj">The command's parameter (not used in this function).</param>
		private void OnDecrementTemperatureCommand(object obj)
		{
			CommonViewModel.Current.TC1Reading -= 5; //Tank weight
			CommonViewModel.Current.CatheterTemperature -= 5;
			//Tip Pressure, 0 to 15
			if(CommonViewModel.Current.EcgChannel1And2Reading > 0)
				CommonViewModel.Current.EcgChannel1And2Reading -= 10;

			//Diaphragm amplitude goes between -2 and 2 G
			if(CommonViewModel.Current.EcgChannel3And4Reading > -2)
				CommonViewModel.Current.EcgChannel3And4Reading -= 0.1;

			//Esophagus temperature, set 30 value by default when in simulator mode
			if(CommonViewModel.Current.EcgChannel5And6Reading == 0)
			{
				CommonViewModel.Current.EcgChannel5And6Reading = 30;
			}

			//Esophagus temperature
			//if (CommonViewModel.Current.EcgChannel5And6Reading > 0)
			CommonViewModel.Current.EcgChannel5And6Reading -= 1;

			//Diaphragm movement, set 90 value by default when in simulator mode
			if(CommonViewModel.Current.EcgChannel7And8Reading == 0)
			{
				CommonViewModel.Current.EcgChannel7And8Reading = 90;
			}

			//Diaphragm movement
			if(CommonViewModel.Current.EcgChannel7And8Reading >= 0)
				CommonViewModel.Current.EcgChannel7And8Reading -= 1;

			//Balloon Pressure, 0 to 10
			if(CommonViewModel.Current.CP1Reading > 0)
				CommonViewModel.Current.CP1Reading -= 0.5;

			CommonViewModel.Current.LC1Reading = -1;
			CommonViewModel.Current.BloodDetecorImValue--;

			//CommonViewModel.Current.DeacreaeSesnorSimulation();
		}

		int errorNumber = 0;

		/// <summary>
		/// Function/Command that handles the Error display.
		/// command is invoked.  This command shall only be invoked the DebugWithSimulator solution configuration is selected
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="obj">The command's parameter (not used in this function).</param>
		private void OnErrorCommand(object obj)
		{

			//CommonViewModel.Current.DisplayException5Message();

			//CommonViewModel.Current.GetCMCUStatusError(544); //544 //512

			Task.Delay(10000).ContinueWith(t => CommonViewModel.Current.GetCMCUStatusError(8));

			//CommonViewModel.Current.GetCMCUStatusError(1568);
			//List<Tuple<long, string, string, string>> errors = new List<Tuple<long, string, string, string>>();

			//errors.Add(new Tuple<long, string, string, string>(1, "Cable not connected!", "Connect Cable!", "A generic Cryterion technical system notification"));
			//errors.Add(new Tuple<long, string, string, string>(2, "Catheter connection lost!", "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.", ""));
			//errors.Add(new Tuple<long, string, string, string>(3, "Generic issue 1", "Fix generic issue 1!  No video available.  Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.", ""));

			//MessagePopup messagePopup = new MessagePopup(errors,
			//                                             MessagePopup.MessageType.ErrorMessage,
			//                                             MessagePopup.ButtonType.Ok,
			//                                             IsActionRequired: true);

			//if ((bool)messagePopup.ShowDialog())
			//{
			//}

			//messagePopup = new MessagePopup("Test Single message", MessagePopup.MessageType.SystemMessage);
			//if ((bool)messagePopup.ShowDialog())
			//{
			//}

			switch(errorNumber)
			{
				case 0:

					//CommonViewModel.Current.GetCMCUStatusError(512);
					break;

				case 1:

					// CommonViewModel.Current.GetPMCUStatusError(16384);
					break;

				case 2:

					//CommonViewModel.Current.GetPMCUStatusError(16);
					break;

				case 3:

					//CommonViewModel.Current.GetPMCUStatusError(32);
					break;

				case 4:

					//   CommonViewModel.Current.GetPMCUStatusError(64);
					break;

				case 5:

					//  CommonViewModel.Current.GetPMCUStatusError(128);
					break;

				case 6:

					//  CommonViewModel.Current.GetPMCUStatusError(256);
					break;

				case 7:

					// CommonViewModel.Current.GetPMCUStatusError(512);
					break;

				case 8:

					// CommonViewModel.Current.GetPMCUStatusError(1024);
					break;

				case 9:

					//CommonViewModel.Current.GetPMCUStatusError(2048);
					break;

				case 10:

					//CommonViewModel.Current.GetPMCUStatusError(4096);
					break;

				case 11:

					//CommonViewModel.Current.GetPMCUStatusError(16384);
					errorNumber = 0;
					break;
			}

			errorNumber++;


		}

		private void ResetDataForSimulation()
		{
			CommonViewModel.Current.EcgChannel5And6Reading = 37;

			CommonViewModel.Current.EtsSesnor1 = 37;
			CommonViewModel.Current.EtsSesnor2 = 37;
			CommonViewModel.Current.EtsSesnor3 = 37;
			CommonViewModel.Current.EtsSesnor4 = 37;
			CommonViewModel.Current.EtsSesnor5 = 37;
			CommonViewModel.Current.EtsSesnor6 = 37;
			CommonViewModel.Current.EtsSesnor7 = 37;
			CommonViewModel.Current.EtsSesnor8 = 37;
			CommonViewModel.Current.EtsSesnor9 = 37;
			CommonViewModel.Current.EtsSesnor10 = 37;
			CommonViewModel.Current.EtsSesnor11 = 37;
			CommonViewModel.Current.EtsSesnor12 = 37;
			CommonViewModel.Current.EtsSesnor13 = 37;
			CommonViewModel.Current.TIP = 37;

			CommonViewModel.Current.MinimumTemperature = 37;


		}

		/// <summary>
		/// Function that returns if the system can invoke the Error command
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="arg">The command parameter (not used in this function).</param>
		/// <returns>Boolean value if the system can invoke the command.</returns>
		private bool CanErrorCommand(object arg)
		{
			return true;
		}

		/// <summary>
		/// This property gets/sets CryoTherapy View value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public UserControl CryoTherapyView
		{
			get
			{
				return cryoTherapyView;
			}

			set
			{
				SetProperty(ref cryoTherapyView, value);
			}
		}

		/// <summary>
		/// This property gets/sets the Current Main CryoTherapy View value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public UserControl CurrentMainCryoTherapyView
		{
			get
			{
				return currentMainCryoTherapyView;
			}

			set
			{
				SetProperty(ref currentMainCryoTherapyView, value);
			}
		}

		/// <summary>
		/// This property gets/sets the Patient View value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public UserControl PatientView
		{
			get
			{
				return patientView;
			}

			set
			{
				SetProperty(ref patientView, value);
			}
		}

		/// <summary>
		/// This property gets/sets Report View value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public UserControl ReportView
		{
			get
			{
				return reportView;
			}

			set
			{
				SetProperty(ref reportView, value);
			}
		}

		/// <summary>
		/// This property gets/sets the Home View value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public UserControl HomeView
		{
			get
			{
				return homeView;
			}

			set
			{
				SetProperty(ref homeView, value);
			}
		}

		/// <summary>
		/// This property gets/sets the System State value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public CanBusMessageDefinition.MessageStateId SystemState
		{
			get
			{
				//RaisePropertyChanged("IsEndProcedureVisible");
				return CommonViewModel.Current.SystemState;
			}
			set
			{
				RaisePropertyChanged("SystemState");
			}
		}

		public bool PatientInfoAnonymousVisible => (SaveToCSVSelected || SaveToPDFSelected)
																							 && (_userType == UserType.Admin || _userType == UserType.Doctor || _userType == UserType.User);

		private bool _patientInfoAnonymized;
		public bool IsPatientInfoAnonymized
		{
			get => _patientInfoAnonymized;
			set => SetProperty(ref _patientInfoAnonymized, value);
		}

		/// <summary>
		/// Function/Command that handles the View change when the Navigate To View
		/// command is invoked
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="arg">The command's parameter (view to display).</param>
		private void OnNavigateToView(object arg)
		{
			if(arg.ToString() == "PatientInfo")
			{
        PatientView = new Patient();

        CurrentMainCryoTherapyView = PatientView;
				ReadyToExport = false;
			}
			else if(arg.ToString() == "Therapy")
			{
				if(CryoTherapyView == null)
					CryoTherapyView = new CryoTherapy();

				CurrentMainCryoTherapyView = CryoTherapyView;

				//Set the Fixed Timer each time entering the Cryotherapy view.
				CryoTherapyViewModel cryoViewModel = (CryoTherapyViewModel)CryoTherapyView.DataContext;
				cryoViewModel.IsPatientNameVisible = NotificationModel.Instance.CurrentPhysician.preference.IsShowPatientInfo;

				if(!ProcedureLogModel.CanReloadProcudreInformation)
					cryoViewModel.IsFixedTimerSelected = true;

				CommonViewModel.Current.ScreenName = "Cryo Therapy";
				ReadyToExport = false;
			}
			else if(arg.ToString() == "Report")
			{
				if(ReportView == null)
					ReportView = new Report();

				CurrentMainCryoTherapyView = ReportView;
				if(ReportView.DataContext != null && CryoTherapyView.DataContext != null)
				{
					var reportViewModel_ = (ReportViewModel)ReportView.DataContext;
					reportViewModel_.IsPatientInfoVisibilityMutable = true;

					var therapyViewModel_ = (CryoTherapyViewModel)CryoTherapyView.DataContext;
					reportViewModel_.IsPatientInfoVisible = therapyViewModel_.IsPatientNameVisible;
          reportViewModel_.InBodyTime = therapyViewModel_.InBodyTime;
        }
				viewsEvent.ViewName = "Summary Report";
				ReadyToExport = true;
				OnCompleteProcedureCommand(arg);
			}
			else if(arg.ToString() == "Home")
			{
				CommonViewModel.Current.AblationSummary.ClearAblationSummary();
				OnEndProcedureCommand(arg);
			}
			else if(arg.ToString() == "HomeAndEndingProcedure")
			{
				CommonViewModel.Current.AblationSummary.ClearAblationSummary();
				OnEndProcedureCommand(arg);
			}
			else if(arg.ToString() == "ReturnToProcedure")
			{
				if(CryoTherapyView == null)
					CryoTherapyView = new CryoTherapy();

				CurrentMainCryoTherapyView = CryoTherapyView;
				var therapyViewModel_ = cryoTherapyView.DataContext as CryoTherapyViewModel;
				therapyViewModel_.IsPatientNameVisible = (reportView.DataContext as ReportViewModel).IsPatientInfoVisible;
				therapyViewModel_.IsFromReturnToProcedure = true;
        therapyViewModel_.CryoTherapyTime = 0;
				viewsEvent.ViewName = "MainCryoTherapy";
				OnReturnToProcedureCommand(arg);
				ReadyToExport = false;
			}
			else if(arg.ToString() == "ExportedAndReturned")
			{
				CommonViewModel.Current.AblationSummary.ClearAblationSummary();
				OnEndProcedureCommand(arg);
			}
		}

		/// <summary>
		/// This property gets/sets the Return to Procedure Visible value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public Visibility IsReturnToProcedureVisible
		{
			get
			{
				Visibility visibility = Visibility.Collapsed;

				if(currentMainCryoTherapyView == ReportView &&
						(CommonViewModel.Current.SystemState == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE ||
						 CommonViewModel.Current.SystemState == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY))
				{
					visibility = Visibility.Visible;
				}

				return visibility;
			}
			set
			{
				RaisePropertyChanged("IsReturnToProcedureVisible");
			}
		}

		/// <summary>
		/// This property gets/sets the End Procedure Visible value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public Visibility IsEndProcedureVisible
		{
			get
			{
				Visibility visibility = Visibility.Collapsed;

				if(currentMainCryoTherapyView == ReportView &&
						(CommonViewModel.Current.SystemState == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE ||
						 CommonViewModel.Current.SystemState == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY))
				{
					visibility = Visibility.Visible;
				}

				return visibility;
			}

			set
			{
				RaisePropertyChanged("IsEndProcedureVisible");
			}
		}

		/// <summary>
		/// This property gets/sets the Complete Procedure Visible value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public Visibility IsCompleteProcedureVisible
		{
			get
			{
				Visibility visibility = Visibility.Collapsed;

				if(currentMainCryoTherapyView == CryoTherapyView && CommonViewModel.Current.CurrentAblation != null &&
						(CommonViewModel.Current.SystemState == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE ||
						 CommonViewModel.Current.SystemState == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY))
				{
					visibility = Visibility.Visible;
				}

				return visibility;
			}

			set
			{
				RaisePropertyChanged("IsCompleteProcedureVisible");
			}
		}

		/// <summary>
		/// This property gets/sets the Sensors In Playback mode flag
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool AreSensorsInPlayBackMode
		{
			get
			{
				return CommonViewModel.Current.AreSensorsInPlayBackMode;
			}

			set
			{
				CommonViewModel.Current.AreSensorsInPlayBackMode = value;
				RaisePropertyChanged("AreSensorsInPlayBackMode");
			}
		}

		/// <summary>
		/// This property gets/sets the CanStartDiaphragmMovementMonitoring value
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool CanstartDiaphragmMovementMonitoring
		{
			get
			{
				return CommonViewModel.Current.CanstartDiaphragmMovementMonitoring;
			}

			set
			{
				CommonViewModel.Current.CanstartDiaphragmMovementMonitoring = value;
				RaisePropertyChanged("CanstartDiaphragmMovementMonitoring");
			}
		}

		/// <summary>
		/// Function that returns if the system can invoke the Navigate to View command
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="arg">The command parameter (not used in this function).</param>
		/// <returns>Boolean value if the system can invoke the command.</returns>
		private bool CanNavigateToView(object arg)
		{
			return true;
		}

		/// <summary>
		/// This function handles the sender's PropertyChanged event
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="sender">The View Model that sent the event.</param>
		/// <param name="e">The property changed arguments.</param>
		private void Current_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			CommonViewModel commonviewmodel = sender as CommonViewModel;

			switch(e.PropertyName)
			{
				case "CanStartTherapy":

					if(CommonViewModel.Current.CanStartTherapy)
					{
						OnNavigateToView("Therapy");
						if(ResetTherapyCommand.CanExecute(null))
						{
							ResetTherapyCommand.Execute(null);
						}
					}
					RaisePropertyChanged("IsEndProcedureVisible");
					RaisePropertyChanged("IsCompleteProcedureVisible");
					RaisePropertyChanged("IsReturnToProcedureVisible");
					break;

				case "SystemState":
					RaisePropertyChanged("IsEndProcedureVisible");
					RaisePropertyChanged("IsCompleteProcedureVisible");
					RaisePropertyChanged("IsReturnToProcedureVisible");
					RaisePropertyChanged("SystemState");
					break;

				case "AreSensorsInPlayBackMode":
					RaisePropertyChanged("AreSensorsInPlayBackMode");
					break;

				case "IsPlayBackModeDeactivted":
					RaisePropertyChanged("IsPlayBackModeDeactivted");
					break;

				case "CanstartDiaphragmMovementMonitoring":
					RaisePropertyChanged("CanstartDiaphragmMovementMonitoring");
					break;
			}
		}

		private readonly USBDriveConnectionManager.USBDriveConnectionManager _usbDriveConnectionManager;

		private void USBDriveConnection_EventArrived(object sender, EventArrivedEventArgs e)
		{
			USBDriveList = _usbDriveConnectionManager.GetUSBDriveList();
			RaisePropertyChanged(nameof(USBDriveConnected));
			RaisePropertyChanged(nameof(ReadyToExport));
			RaisePropertyChanged(nameof(CanExportProcedure));
		}

		private List<DriveInfo> _usbDriveList;
		public List<DriveInfo> USBDriveList
		{
			get => _usbDriveList;
			set => SetProperty(ref _usbDriveList, value);
		}

		public bool USBDriveConnected => USBDriveList != null && USBDriveList.Count != 0;

		private bool _readyToExport;

		public bool ReadyToExport
		{
			get => _readyToExport && CurrentProcedureReady;
			set
			{
				SetProperty(ref _readyToExport, value);
				RaisePropertyChanged(nameof(CanExportProcedure));
			}
		}

		private ProcedureRecords _currentProcedureRecords;

		private bool CurrentProcedureReady => CommonViewModel.Current.CurrentProcedure != null;

		private bool isPrinterAvailable = true;
    public bool IsPrinterAvailable
    {
      get => isPrinterAvailable;
      set => SetProperty(ref isPrinterAvailable, value);
    }

		private string _usbPath = string.Empty;
		public string USBPath
		{
			get => _usbPath;
			set => SetProperty(ref _usbPath, value);
		}

		private bool CanExportProcedure(object obj) => true;

		private bool CanPrintPDF(object obj) => true;

		private const string ExportFolder = "PatientRecord";
		private const string Underscore = "_";
		private UserType _userType = UserType.Unknown;
		public string HospitalName => _dataAccess.GetHospitalName() ?? "";

		private async void OnPrintPDFCommand(object obj)
		{
			var currentProcedureRecords_ = CreateCurrentProcedureRecords();
			if(currentProcedureRecords_ == null)
			{
				return;
			}

      GetUserType();

			IsPrinterAvailable = false;
			using(var service_ = new DataExportService(_userType, currentProcedureRecords_, null, IsPatientInfoAnonymized, string.Empty))
			{
				service_.SelectedProcedureRecordsList.Add(currentProcedureRecords_);
				await service_.PrintPdfReport();
			}
			IsPrinterAvailable = true;
		}

		private CancellationTokenSource _cancellationTokenSource;

		private async void OnExportProcedureCommand(object obj)
		{
            GetUserType();
			USBPath = USBDriveList[0]?.Name + ExportFolder + Path.DirectorySeparatorChar;
            var saveNotifDialog_ = new SaveToUSBNotification();
            var notifResult_ = saveNotifDialog_.ShowDialog();
            if (notifResult_.HasValue)
            {
				if (!notifResult_.Value)
				{
					return;
				}
			}
			else
			{
				return;
			}
            var saveProcedureDialog_ = new SaveProcedureToUSB(this);
      currentMainCryoTherapyView.Opacity = 0.1;
      var dialogResult_ = saveProcedureDialog_.ShowDialog();
      currentMainCryoTherapyView.Opacity = 1.0;
      var procedureSaved_ = false;

			if(dialogResult_.HasValue && dialogResult_.Value)
			{
                _cancellationTokenSource = new CancellationTokenSource();
				var exportDialog_ = new FileExportCancellationPopup(_cancellationTokenSource, this);
				IsExportingFiles = true;
				IsCanceled = false;

				var exportDataTask_ = Task.Run(() =>
				{
					if(SaveLogSelected)
					{
						var dir_ = new DirectoryInfo(USBPath);
						ZipLogsToUsb(dir_, _cancellationTokenSource.Token);
					}

					try
					{
						procedureSaved_ = ExportCurrentProcedureDataToUsb();
					}
					catch(Exception e)
					{
						LogException(e);
						procedureSaved_ = false;
					}

					if(SaveToReportSelected)
					{
						SaveCaseReportToUSB();
					}
				});
				Application.Current.Dispatcher.BeginInvoke((Action)(() =>
				{
					_ = exportDialog_.ShowDialog();
				}));

				await exportDataTask_;

				if(procedureSaved_)
				{
					IsExportingFiles = false;
					IsCanceled = false;
				}
				else
				{
					IsExportingFiles = false;
					IsCanceled = true;
				}

				OnNavigateToView("ExportedAndReturned");
			}
		}

		private void ZipLogsToUsb(DirectoryInfo directoryInfo, CancellationToken cancellationToken)
		{
			try
			{
				var csn_ = _dataAccess?.GetConsoleSerialNumber() ?? string.Empty;
				_currentProcedureRecords = CreateCurrentProcedureRecords();
				var lst_ = new List<ProcedureRecords> { _currentProcedureRecords };
				using(var service_ = new DataExportService(UserType.Bsc, directoryInfo, lst_, csn_))
				{
					var result_ = service_.ExportLogFile(this, cancellationToken);
					if(!File.Exists(result_?.FullName) && cancellationToken.IsCancellationRequested)
					{
						LogMessage = "Export log files cancelled.";
					}
				}
			}
			catch(Exception e)
			{
				LogException(e);
			}
		}

		private bool ExportCurrentProcedureDataToUsb()
		{
			_currentProcedureRecords = CreateCurrentProcedureRecords();
			var currentProcedureSaved_ = false;
			var dest_ = new DirectoryInfo(USBPath);

			using(var dataExportService_ = new DataExportService(_userType, _currentProcedureRecords, dest_, IsPatientInfoAnonymized, FilePassword))
			{
				if(SaveToJSONSelected)
				{
					try
					{
						currentProcedureSaved_ = File.Exists(dataExportService_.ExportJsonFile()?.FullName);
					}
					catch(Exception e)
					{
						LogException(e);
						currentProcedureSaved_ = false;
					}
				}

				if(SaveToCSVSelected)
				{
					try
					{
						currentProcedureSaved_ = File.Exists(dataExportService_.ExportExcelFile()?.FullName);
					}
					catch(Exception e)
					{
						LogException(e);
						currentProcedureSaved_ = false;
					}
				}

				if(SaveToPDFSelected)
				{
					try
					{
						currentProcedureSaved_ = File.Exists(dataExportService_.ExportPdfFile()?.FullName);
					}
					catch(Exception e)
					{
						LogException(e);
						currentProcedureSaved_ = false;
					}
				}
			}

			return currentProcedureSaved_;
		}

		private LoginManager.AccessControlType _accessControlType = LoginManager.AccessControlType.USER;

		private string GetBasePath()
		{
			var thePath_ = string.Empty;
			var path_ = AppDomain.CurrentDomain.BaseDirectory;
			var extractedStrings_ = Regex.Split(path_, "bin");  //split it in bin
			thePath_ = extractedStrings_[0];
			return thePath_;
		}

		private string _fileToExport = string.Empty;
		public string FileToExport
		{
			get => _fileToExport;
			set => SetProperty(ref _fileToExport, value);
		}

		private ProcedureRecords CreateCurrentProcedureRecords()
		{
			var currentProcedure_ = CommonViewModel.Current.CurrentProcedure;
			var allPatient_ = _dataAccess.GetAllPatient();
			var allAblations_ = _dataAccess.GetAllAblationByProcedureId(currentProcedure_.Id);
			currentProcedure_.Ablations = allAblations_;

			ProcedureRecords procedureRecords_ = new ProcedureRecords
			{
				Procedure = currentProcedure_,
			};
			procedureRecords_.Procedure.Patient = allPatient_.Find(patient_ => patient_.ID == currentProcedure_.PatientID);
			return procedureRecords_;
		}

		private bool _saveToCSVSelected;
		public bool IsCanceled { get; set; }

		public bool SaveToCSVSelected
		{
			get => _saveToCSVSelected;
			set
			{
				SetProperty(ref _saveToCSVSelected, value);
				RaisePropertyChanged(nameof(IsPasswordVisible));
				RaisePropertyChanged(nameof(FilePassword));
				RaisePropertyChanged(nameof(ConfirmPassword));
				RaisePropertyChanged(nameof(IsOkEnabled));
				RaisePropertyChanged(nameof(PatientInfoAnonymousVisible));
				if(!value && !SaveToPDFSelected)
					IsPatientInfoAnonymized = false;
			}
		}

		private bool _saveToJSONSelected;
		public bool SaveToJSONSelected
		{
			get => _saveToJSONSelected;
			set
			{
				SetProperty(ref _saveToJSONSelected, value);
				RaisePropertyChanged(nameof(IsPasswordVisible));
				RaisePropertyChanged(nameof(FilePassword));
				RaisePropertyChanged(nameof(ConfirmPassword));
				RaisePropertyChanged(nameof(IsOkEnabled));
				if(!value)
				{
					DeletionSelected = false;
				}
			}
		}

		private bool _saveToPDFSelected;
		public bool SaveToPDFSelected
		{
			get => _saveToPDFSelected;
			set
			{
				SetProperty(ref _saveToPDFSelected, value);
				RaisePropertyChanged(nameof(IsPasswordVisible));
				RaisePropertyChanged(nameof(FilePassword));
				RaisePropertyChanged(nameof(ConfirmPassword));
				RaisePropertyChanged(nameof(IsOkEnabled));
				RaisePropertyChanged(nameof(PatientInfoAnonymousVisible));
				if(!value && !SaveToCSVSelected)
					IsPatientInfoAnonymized = false;
			}
		}

		private bool _saveToReportSelected;
		public bool SaveToReportSelected
		{
			get => _saveToReportSelected;
			set
			{
				SetProperty(ref _saveToReportSelected, value);
				RaisePropertyChanged(nameof(IsPasswordVisible));
				RaisePropertyChanged(nameof(IsOkEnabled));
				RaisePropertyChanged(nameof(FilePassword));
				RaisePropertyChanged(nameof(ConfirmPassword));
			}
		}

		private bool _saveLogSelected;
		public bool SaveLogSelected
		{
			get => _saveLogSelected;
			set
			{
				SetProperty(ref _saveLogSelected, value);
				RaisePropertyChanged(nameof(IsPasswordVisible));
				RaisePropertyChanged(nameof(IsOkEnabled));
				RaisePropertyChanged(nameof(FilePassword));
				RaisePropertyChanged(nameof(ConfirmPassword));
			}
		}

		public bool ActionLogExported { get; set; }
		public bool ErrorLogExported { get; set; }
		public bool SmartFreezeLogExported { get; set; }
		public bool WinEventLogExported { get; set; }
		private string _logMessage = string.Empty;
		public string LogMessage
		{
			get => _logMessage;
			set => SetProperty(ref _logMessage, value);
		}
		public int LogFileCount => 4;

		private int _logProgressBarValue;
		public int LogProgressBarValue
		{
			get => _logProgressBarValue;
			set
			{
				SetProperty(ref _logProgressBarValue, value);
				USBExportProgressEvent?.Invoke(this, EventArgs.Empty);
			}
		}

		public int ProcedureRecordsCount { get; }

		private int _progressBarValue;
		public int ProgressBarValue
		{
			get => _progressBarValue;
			set
			{
				SetProperty(ref _progressBarValue, value);
				USBExportProgressEvent?.Invoke(this, EventArgs.Empty);
			}
		}

		private bool _isExportingFiles;
		public bool IsExportingFiles
		{
			get => _isExportingFiles;
			set
			{
				SetProperty(ref _isExportingFiles, value);
				USBExportProgressEvent?.Invoke(this, EventArgs.Empty);
			}
		}

		#region Password

		private bool _isPasswordValid;
		public bool IsPasswordValid
		{
			get => _isPasswordValid;
			set
			{
				SetProperty(ref _isPasswordValid, value);
				RaisePropertyChanged(nameof(IsPasswordConfirmed));
				RaisePropertyChanged(nameof(IsOkEnabled));
			}
		}

		private bool _isPasswordConfirmed;
		public bool IsPasswordConfirmed
		{
			get => _isPasswordConfirmed;
			set
			{
				SetProperty(ref _isPasswordConfirmed, value);
				RaisePropertyChanged(nameof(IsOkEnabled));
			}
		}

		public bool IsOkEnabled => IsPasswordValid
															 && IsPasswordConfirmed
															 && (SaveToCSVSelected || SaveToPDFSelected || SaveToJSONSelected || SaveLogSelected || SaveToReportSelected)
															 || SaveToJSONSelected && !SaveToCSVSelected && !SaveToPDFSelected && (_userType == UserType.Bsc || _userType == UserType.BostonBsc)
															 || (!SaveToCSVSelected && !SaveToPDFSelected && !SaveToJSONSelected && SaveLogSelected);

		public bool IsPasswordVisible => SaveToJSONSelected && _userType != UserType.Bsc && _userType != UserType.BostonBsc
																		 || SaveToCSVSelected
																		 || SaveToPDFSelected
																		 || SaveToReportSelected;

		private string _filePassword = string.Empty;
		public string FilePassword
		{
			get => _filePassword;
      set
			{
				ValidatePassword(value);
        IsPasswordValid = GetErrors(nameof(FilePassword)) == null;
				ValidateConfirmPassword(ConfirmPassword);
				SetProperty(ref _filePassword, value);
			}
		}

		private string _confirmPassword = string.Empty;
		public string ConfirmPassword
		{
			get => _confirmPassword;
      set
			{
				ValidateConfirmPassword(value);
				IsPasswordConfirmed = GetErrors(nameof(ConfirmPassword)) == null;
				SetProperty(ref _confirmPassword, value);
			}
		}

		#endregion Password

		#region Deletion

		private bool _deletionSelected;

		public bool DeletionSelected
		{
			get => _deletionSelected;
			set => SetProperty(ref _deletionSelected, value);
		}

		public async Task OnDeleteDataFiles(bool? delete)
		{
			await DeleteCurrentProcedureDataFileAsync();
			await ArchiveCurrentProcedureOnDBAsync();
			await LogDeleteActionAsync();
			DeletionSelected = false;
		}

		private async Task DeleteCurrentProcedureDataFileAsync()
		{
			var currentProcedure_ = _currentProcedureRecords.Procedure;
			var files_ = currentProcedure_.Ablations.Select(a => a.DataFile);

			await Task.Run(() =>
			{
				foreach(var f_ in files_)
				{
					try
					{
						File.Delete(f_);
					}
					catch(Exception e)
					{
						LogException(e);
						Application.Current.BeginInvoke(() =>
						{
							var errorPopup_ = new MessagePopup("Error in deleting data file.", MessagePopup.MessageType.ErrorMessage, MessagePopup.ButtonType.Ok);
							errorPopup_.ShowDialog();
						});
					}
				}
			});
		}

		private async Task ArchiveCurrentProcedureOnDBAsync()
		{
			var currentProcedure_ = _currentProcedureRecords.Procedure;
			await Task.Run(() => _dataAccess.ArchiveProcedure(currentProcedure_));
		}

		private async Task LogDeleteActionAsync()
		{
			await Task.Run(() =>
			{
				try
				{
					var currentProcedure_ = _currentProcedureRecords.Procedure; // CommonViewModel.Current.CurrentProcedure;
					var message_ = "ID: " + currentProcedure_.Id + ", " + "Date: " + currentProcedure_.ProcedureStartDateTime;
					CommonViewModel.Current.LogUserAction(Enumeration.Actions.DeleteProcedure, message_);
				}
				catch(Exception e)
				{
					LogException(e);
				}
			});
		}

		public string ProcedureStartTime { get; set; } = string.Empty;

		public string ProcedureEndTime { get; set; } = string.Empty;

		private ObservableCollection<ProcedureRecords> _filteredProceduresList = new ObservableCollection<ProcedureRecords>();
		public ObservableCollection<ProcedureRecords> FilteredProcedureRecordsList
		{
			get => _filteredProceduresList;
			set => SetProperty(ref _filteredProceduresList, value);
		}

		private readonly PDFCaseReport _PDFCaseReport = new PDFCaseReport();
		private readonly PDFConversion _PDFConversion = new PDFConversion();
		public bool IsCryterionUser => CommonViewModel.Current.IsCryterionUser || CommonViewModel.Current.IsBSCADMINUser;
		public bool IsBSCADMINUser => CommonViewModel.Current.IsBSCADMINUser;
		public bool IsDoctor => CommonViewModel.Current.IsDoctor;
		public bool IsAdminUser => CommonViewModel.Current.IsAdminUser;

		private bool _isExportingCurrentProcedure;

		public bool IsExportingCurrentProcedure
		{
			get => _isExportingCurrentProcedure;
			set => SetProperty(ref _isExportingCurrentProcedure, value);
		}

		private List<ProcedureRecords> GetAllProcedures()
		{
			var allProcedures_ = new List<ProcedureRecords>();
			try
			{
				var allPatient_ = _dataAccess.GetAllPatient();
				List<Procedure> procedureList = null;
				if(CommonViewModel.Current.IsDoctor)
				{
					var userId_ = CommonViewModel.Current.CurrentUser.Id;
					var procedureList_ = _dataAccess.GetProceduresByPhysician(userId_);
					procedureList = FilterProcedures(procedureList_);
					var physician = _dataAccess.GetphysicianByID(userId_);
					allPatient_.ForEach(x => x.Physician = physician);
					foreach(var procedure in procedureList)
					{
						var procRecord = new ProcedureRecords { Procedure = procedure };
						procRecord.Procedure.Patient = allPatient_.Find(patient_ => patient_.ID == procedure.PatientID);
						allProcedures_.Add(procRecord);
					}
				}
				else if(IsCryterionUser || IsBSCADMINUser || IsAdminUser)
				{
					var procedureList_ = _dataAccess.GetAllProcedures();
					procedureList = FilterProcedures(procedureList_);
					var physicianList_ = _dataAccess.GetAllPhysicians();
					allPatient_.ForEach(x => x.Physician = physicianList_.Find(physician_ => physician_.ID == x.PhysicianID));
					foreach(var procedure in procedureList)
					{
						var procRecord = new ProcedureRecords { Procedure = procedure };
						procRecord.Procedure.Patient = allPatient_.Find(x => x.ID == procedure.PatientID);
						if(IsCryterionUser || IsBSCADMINUser)
						{
							procRecord.Procedure.Patient.FirstName = procRecord.Procedure.Patient.LastName = "-";
						}
						allProcedures_.Add(procRecord);
					}
				}
			}
			catch(Exception ex)
			{
				LogException(ex);
				return null;
			}
			return allProcedures_;
		}

		private List<Procedure> FilterProcedures(List<Procedure> proceduresList)
		{
			var list_ = new List<Procedure>();
			foreach(var procedure_ in proceduresList)
			{
				var ablationFilter = new List<Ablation>();
				if(procedure_.Ablations.Count > 0)
				{
					foreach(var ablation in procedure_.Ablations)
					{
						if(ablation?.DataFile?.Length > 10 && File.Exists(ablation.DataFile))
						{
							ablationFilter.Add(ablation);
						}
					}
					if(ablationFilter.Count > 0)
					{
						procedure_.Ablations = null;
						procedure_.Ablations = ablationFilter;
						list_.Add(procedure_);
					}
				}
			}
			return list_;
		}

		private bool SaveCaseReportToUSB()
		{
			var result_ = true;

			string procedureStartDate = "";
			string procedureEndDate = "";
			string caseReportName = "CaseReport";
			List<ProcedureRecords> procedureRecordsListWhereFrom = new List<ProcedureRecords>();
			if(ProcedureStartTime == "0") procedureStartDate = "1900-01-01";
			else
			{
				caseReportName += ProcedureStartTime;
				procedureStartDate = ProcedureStartTime + "-01-01";
			}

			if(ProcedureEndTime == "0") procedureEndDate = "2900-12-31";
			else
			{
				if(ProcedureStartTime != ProcedureEndTime) caseReportName += "-" + ProcedureEndTime;
				procedureEndDate = ProcedureEndTime + "-12-31";
			}

			var allProcedures_ = GetAllProcedures();
			FilteredProcedureRecordsList = new ObservableCollection<ProcedureRecords>(allProcedures_.OrderBy(pr_ =>
				pr_.Procedure.ProcedureStartDateTime, ListSortDirection.Descending));

			procedureRecordsListWhereFrom = FilteredProcedureRecordsList.Where(p =>
				p.ProcedureDate >= DateTime.Parse(procedureStartDate) &&
				p.ProcedureDate <= DateTime.Parse(procedureEndDate)).ToList();

			Application.Current.BeginInvoke(() =>
			{
				CaseSummaryReport caseSummaryReport = new CaseSummaryReport(procedureRecordsListWhereFrom);
				caseSummaryReport.Visibility = Visibility.Collapsed;
				caseSummaryReport.Show();
			});

			try
			{
				_PDFCaseReport.GeneratePDFCaseReport(procedureRecordsListWhereFrom, caseReportName, HospitalName);

				string sourceFilePath = "";

				string mysavePath = USBDriveList[0].Name + "PatientRecord\\" + caseReportName + ".pdf";
				sourceFilePath = getCaseFilePath(mysavePath, caseReportName) + ".pdf";
				if(File.Exists(mysavePath))
				{
					File.Delete(mysavePath);
				}

				File.Copy(sourceFilePath, mysavePath);
				_PDFConversion.Protect(sourceFilePath, mysavePath, FilePassword);
				File.Delete(sourceFilePath);
			}
			catch(Exception e)
			{
				LogException(e);
				result_ = false;
			}
			return result_;
		}

		private string getCaseFilePath(string path, string caseFileName)
		{
			var basePath_ = GetBasePath() + "PDFFiles\\";
			var temppath_ = basePath_ + caseFileName;
			return temppath_;
		}

    #endregion Deletion

    #region INotifyDataErrorInfo Interface

    public IEnumerable GetErrors(string propertyName)
      => _errorsByPropertyName.ContainsKey(propertyName) ? _errorsByPropertyName[propertyName] : null;

    public bool HasErrors => _errorsByPropertyName.Any();

    public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

    #endregion

    #region INotifyDataErrorInfo Implementation

    private readonly Dictionary<string, List<string>> _errorsByPropertyName = new Dictionary<string, List<string>>();
    private readonly Regex _passwordValidationRegex = new Regex("^(?=.*[0-9]).{8,16}$", RegexOptions.Compiled);
    private void AddError(string propertyName, string error)
    {
      if(!_errorsByPropertyName.ContainsKey(propertyName))
      {
        _errorsByPropertyName[propertyName] = new List<string>();
      }
      if(!_errorsByPropertyName[propertyName].Contains(error))
      {
        _errorsByPropertyName[propertyName].Add(error);
        RaiseErrorsChanged(propertyName);
      }
    }

    public void ClearErrors(string propertyName)
    {
      if(_errorsByPropertyName.ContainsKey(propertyName))
      {
        _errorsByPropertyName.Remove(propertyName);
        RaiseErrorsChanged(propertyName);
      }
    }

    private void RaiseErrorsChanged(string propertyName)
      => ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));

    private void ValidatePassword(string pw)
    {
      ClearErrors(nameof(FilePassword));
      if(string.IsNullOrEmpty(pw))
      {
        AddError(nameof(FilePassword), UIConstants.PasswordEmptyErrorMessage);
        return;
      }
      if(!_passwordValidationRegex.IsMatch(pw))
      {
        AddError(nameof(FilePassword), UIConstants.PasswordInvalidMessage);
        IsPasswordValid = false;
      }
      else
      {
        IsPasswordValid = true;
      }
    }

    private void ValidateConfirmPassword(string cpw)
    {
      ClearErrors(nameof(ConfirmPassword));
      if(cpw != FilePassword || !_passwordValidationRegex.IsMatch(FilePassword))
      {
        AddError(nameof(ConfirmPassword), UIConstants.PasswordNotMatchMessage);
        IsPasswordConfirmed = false;
      }
      else
      {
        IsPasswordConfirmed = true;
      }
    }

    #endregion
  }
}