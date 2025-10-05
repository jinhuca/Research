using MahApps.Metro.Controls;
using Prism.Commands;
using Prism.Mvvm;
using SmartAblationSystem.Views;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace SmartAblationSystem.ViewModels
{
	public class FileExportCancellationPopupViewModel : BindableBase
	{
		private readonly IDataExportable _context;
		private readonly CancellationTokenSource _cancelTokenSource;
		public ICommand CancelCommand { get; }
		public EventHandler CloseWindowRequested;

		private bool _isExporting;
		public bool IsExporting
		{
			get => _isExporting;
			set => SetProperty(ref _isExporting, value);
		}

		private bool _isCanceled;
		private bool IsCanceled
		{
			get => _isCanceled;
			set => SetProperty(ref _isCanceled, value);
		}

		private int _procRecCount;
		public int ProcedureRecordsCount
		{
			get => _procRecCount;
			set => SetProperty(ref _procRecCount, value);
		}

		private int _progressBarValue;
		public int ProgressBarValue
		{
			get => _progressBarValue;
			set => SetProperty(ref _progressBarValue, value);
		}

		private int _logProgressBarValue;
		public int LogProgressBarValue
		{
			get => _logProgressBarValue;
			set
			{
				SetProperty(ref _logProgressBarValue, value);
				UsbExportProgressEventHandler(this, EventArgs.Empty);
			}
		}

		private string _logExportMessage;

		public string LogExportMessage
		{
			get=> _logExportMessage;
			set
			{
				SetProperty(ref _logExportMessage, value);
				UsbExportProgressEventHandler(this, EventArgs.Empty);
			}
		}

		private bool _isExportingCurrentProcedure;
		public bool IsExportingCurrentProcedure
		{
			get => _isExportingCurrentProcedure;
			set => SetProperty(ref _isExportingCurrentProcedure, value);
		}

		private string _errorMessage = string.Empty;
		public string ErrorMessage
		{
			get => _errorMessage;
			set => SetProperty(ref _errorMessage, value);
		}

		private bool _saveLogSelected;
		public bool SaveLogSelected
		{
			get => _saveLogSelected;
			set => SetProperty(ref _saveLogSelected, value);
		}

		public FileExportCancellationPopupViewModel(CancellationTokenSource cancellation, IDataExportable context)
		{
			CancelCommand = new DelegateCommand<object>(OnCancelCommand, obj => true);
			_context = context;
			_cancelTokenSource = cancellation;
			IsExporting = false;
			IsCanceled = false;
			ProcedureRecordsCount = context.ProcedureRecordsCount;
			SaveLogSelected = context.SaveLogSelected;
			LogProgressBarValue = 0;
			IsExportingCurrentProcedure = context.IsExportingCurrentProcedure;
			context.USBExportProgressEvent += UsbExportProgressEventHandler;
			context.PropertyChanged += context_PropertyChanged;
		}

		private void context_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			switch(e.PropertyName)
			{
				case nameof(_context.ExceptionMessage):
					ErrorMessage = _context.ExceptionMessage;
					break;
				case nameof(_context.SaveLogSelected):
					SaveLogSelected = _context.SaveLogSelected;
					break;
				case nameof(_context.LogProgressBarValue):
					LogProgressBarValue = _context.LogProgressBarValue;
					break;
				case nameof(_context.LogMessage):
					LogExportMessage = _context.LogMessage;
					break;
			}
		}

		private void Close()
		{
			_context.USBExportProgressEvent -= UsbExportProgressEventHandler;
			_context.PropertyChanged -= context_PropertyChanged;
			CloseWindowRequested?.Invoke(this, EventArgs.Empty);
		}

		private void UsbExportProgressEventHandler(object sender, EventArgs e)
		{
			Application.Current.Dispatcher.Invoke(() =>
			{
				ProgressBarValue = _context.ProgressBarValue;
				IsExporting = _context.IsExportingFiles;
				IsCanceled = _context.IsCanceled;
			});
		}

		private void OnCancelCommand(object arg)
		{
			if(IsCanceled == false && IsExporting)
			{
				var dialogConfirmationPopup_ = new MessagePopup(
					message: "Are you sure you want to cancel?",
					messageType: MessagePopup.MessageType.WarningMessage,
					buttonType: MessagePopup.ButtonType.OkCancel,
					messageTitle: "Confirmation Message"
				);

				var dialogResult_ = dialogConfirmationPopup_.ShowDialog();
        if (dialogResult_.HasValue && dialogResult_.Value && !_cancelTokenSource.IsCancellationRequested)
        {
          _cancelTokenSource.Cancel();
          IsCanceled = true;
          IsExporting = false;
        }
			}
			else if(IsCanceled == false && IsExporting == false)
			{
				Close();
				if(_context.DeletionSelected)
				{
					Application.Current.BeginInvoke(() =>
          {
						var popup_ = new FileDeletionPopup(_context);
						popup_.ShowDialog();
          });
				}
			}
			else
			{
				Close();
			}
		}
	}
}