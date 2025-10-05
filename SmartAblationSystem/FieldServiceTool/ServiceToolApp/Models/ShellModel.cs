using DataAccessLayer;
using Module.Console.Interfaces;
using Module.Infrastructure;
using Module.Infrastructure.AppLog;
using Module.Infrastructure.Helpers;
using Module.Infrastructure.PubSubEvents;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Management;
using System.Reactive.Subjects;
using System.Reflection;
using System.Threading.Tasks;
using static Communication.CanBusMessageDefinition;
using static Communication.CanBusMessageDefinition.MessageStateId;
using static Module.Infrastructure.Constants.Strings;
using static Module.Infrastructure.SessionStatus;

namespace ServiceToolApp.Models
{
	public class ShellModel : BindableBase
	{
		public ShellModel(
			IEventAggregator eventAggregator,
			IMachineModel machineModel,
			DataAccess dataAccess,
			IContainerProvider containerProvider)
		{
			_eventAggregator = eventAggregator;
			_machineModel = machineModel;
			_dataAccess = dataAccess;
			_containerProvider = containerProvider;
			_systemStateObject = new BehaviorSubject<MessageStateId>(CAN_ID_STATE_UNKNOWN);
			_consoleExceptionSubject = new BehaviorSubject<MessageStateId>(CAN_ID_STATE_UNKNOWN);

			_eventAggregator.GetEvent<SessionStatusEvent>().Subscribe(OnReceiveSessionStatusEvent);
			_usbManager = new USBManager(USBArrivedEventHandler);
			SubscribeEvents();
			InitializeConsole();
		}

		#region Fields

		private readonly IContainerProvider _containerProvider;
		private readonly IEventAggregator _eventAggregator;
		private readonly IMachineModel _machineModel;
		private readonly ISubject<MessageStateId> _systemStateObject;
		private readonly ISubject<MessageStateId> _consoleExceptionSubject;
		private readonly USBManager _usbManager;
		private readonly DataAccess _dataAccess;

		#endregion Fields

		#region Properties

		private string _testerFirstName = string.Empty;
		public string TesterFirstName
		{
			get => _testerFirstName;
			set => SetProperty(ref _testerFirstName, value);
		}

		private string _testerLastName = string.Empty;
		public string TesterLastName
		{
			get => _testerLastName;
			set => SetProperty(ref _testerLastName, value);
		}

		private string _hospitalName = string.Empty;
		public string HospitalName
		{
			get => _hospitalName;
			set => SetProperty(ref _hospitalName, value);
		}

		private string _consoleSerialNumber = string.Empty;
		public string ConsoleSerialNumber
		{
			get => _consoleSerialNumber;
			set => SetProperty(ref _consoleSerialNumber, value);
		}

		private string _fstVersion = string.Empty;
		public string FstVersion
		{
			get => _fstVersion;
			set => SetProperty(ref _fstVersion, value);
		}

		private SessionStatus _SessionStatus = Unknown;
		public SessionStatus SessionStatus
		{
			get => _SessionStatus;
			set => SetProperty(ref _SessionStatus, value);
		}

		private MessageStateId _systemStateModel;
		public MessageStateId SystemStateModel
		{
			get => _systemStateModel;
			set => SetProperty(ref _systemStateModel, value);
		}

		private List<DriveInfo> _usbDriveList;
		public List<DriveInfo> USBDriveList
		{
			get => _usbDriveList;
			set
			{
				SetProperty(ref _usbDriveList, value);
				RaisePropertyChanged(nameof(USBDriveConnected));
			}
		}

		private bool _usbDriveConnected;
		public bool USBDriveConnected
		{
			get => _usbDriveConnected;
			set => SetProperty(ref _usbDriveConnected, value);
		}

		private bool _isServiceToolAvailable;
		public bool IsServiceToolAvailable
		{
			get => _isServiceToolAvailable;
			set => SetProperty(ref _isServiceToolAvailable, value);
		}

		private uint _requiredVolume = 50;
		public uint RequiredVolume
		{
			get => _requiredVolume;
			set
			{
				if(value > 100 || value < 0 || _machineModel.IsCanOneInError)
				{
					return;
				}
				_machineModel.Console.SetAudioLevel(RequiredVolume);
				_ = SetProperty(ref _requiredVolume, value);
			}
		}

		private bool _VolumeCtrlEnabled = true;
		public bool VolumeCtrlEnabled
		{
			get => _VolumeCtrlEnabled;
			set => SetProperty(ref _VolumeCtrlEnabled, value);
		}

		#endregion Properties

		#region Event Handlers

		private void _machineModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch(e.PropertyName)
			{
				case nameof(IMachineModel.SystemState):
					SystemStateModel = _machineModel.SystemState;
					break;
			}
		}
		private void UpdateUSBConnectionStatus()
		{
			USBArrivedEventHandler(null, null);
		}

		private void OnReceiveSessionStatusEvent((SessionStatus status, DateTime) sessionEventValue)
		{
			SessionStatus = sessionEventValue.status == Finished ? Ready : sessionEventValue.status;
		}

		private void USBArrivedEventHandler(object sender, EventArrivedEventArgs e)
		{
			USBDriveList = _usbManager.GetUSBDriveList();
			USBDriveConnected = USBDriveList != null && USBDriveList.Count > 0;
			IsServiceToolAvailable = USBDriveConnected && File.Exists(USBDriveList[0].Name + FSTZipName);
			_eventAggregator.GetEvent<USBConnectionEvent>().Publish(USBDriveConnected ? USBDriveList[0].Name : string.Empty);
		}

		#endregion Event Handlers

		#region Methods

		private void SubscribeEvents()
		{
			_machineModel.PropertyChanged += _machineModel_PropertyChanged;
		}

		public async Task TerminateConsole()
		{
			await _machineModel.Terminate();
		}

		private void InitializeConsole()
		{
			try
			{
				ConsoleSerialNumber = _dataAccess.GetConsoleSerialNumber();
				HospitalName = _dataAccess.GetHospitalName();
			}
			catch(Exception e)
			{
				FieldServiceTrace.LogException(e);
			}
			SubscribeEvents();
			FstVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
			_machineModel.SystemState = CAN_ID_STATE_IDLE;
			UpdateUSBConnectionStatus();
		}

		#endregion Methods
	}
}
