using Module.Infrastructure.Constants;
using Module.Infrastructure.PubSubEvents;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using ServiceToolApp.Models;
using ServiceToolApp.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Module.Infrastructure.AppLog;
using Module.Infrastructure.Helpers;
using static System.DateTime;
using static ServiceToolApp.Definitions.Constants;

namespace ServiceToolApp
{
	public class LogonViewModel : BindableBase, IDialogAware, INotifyDataErrorInfo
	{
		public string ApplicationTitle => Resources.ApplicationTitle;
		public string OkText => Resources.OKText;
		public string OkButtonCommandParameter => Resources.YesText;
		public string QuitText => Resources.QuitText;
		public string QuitButtonCommandParameter => Resources.QuitText;

		private readonly ShellModel _shellModel;

		private bool _IsCloseCommandEnabled;
		public bool IsCloseCommandEnabled
		{
			get => _IsCloseCommandEnabled;
			set => SetProperty(ref _IsCloseCommandEnabled, value);
		}

		private bool _IsQuitCommandEnabled = true;
		public bool IsQuitCommandEnabled
		{
			get => _IsQuitCommandEnabled;
			set => SetProperty(ref _IsQuitCommandEnabled, value);
		}

		public string FirstNameTitle { get; } = Resources.FirstNameTitle;
		private string _FirstName = string.Empty;
		public string FirstName
		{
			get => _FirstName;
			set
			{
				if(_FirstName == value) return;
				ValidateFirstName(value);
				SetProperty(ref _FirstName, value);
				_shellModel.TesterFirstName = value;
				IsCloseCommandEnabled = !HasErrors && !string.IsNullOrEmpty(_FirstName) && !string.IsNullOrEmpty(_LastName);
			}
		}

		public string LastNameTitle { get; } = Resources.LastNameTitle;
		private string _LastName = string.Empty;
		public string LastName
		{
			get => _LastName;
			set
			{
				if(_LastName == value) return;
				ValidateLastName(value);
				SetProperty(ref _LastName, value);
				_shellModel.TesterLastName = value;
				IsCloseCommandEnabled = !HasErrors && !string.IsNullOrEmpty(_FirstName) && !string.IsNullOrEmpty(_FirstName);
			}
		}

		private readonly IEventAggregator _eventAggregator;
		public LogonViewModel(IEventAggregator eventAggregator, ShellModel shellModel)
		{
			_eventAggregator = eventAggregator;
			_shellModel = shellModel;

			ValidateFirstName(FirstName);
			ValidateLastName(LastName);
			IsCloseCommandEnabled = !HasErrors && !string.IsNullOrEmpty(_FirstName) && !string.IsNullOrEmpty(_FirstName);
		}

		#region IDialogAware

		private DelegateCommand<string> _closeLogonCommand;
		public DelegateCommand<string> CloseLogonCommand
			=> _closeLogonCommand ?? (_closeLogonCommand = new DelegateCommand<string>(OnCloseLogonCommand).ObservesCanExecute(() => IsCloseCommandEnabled));

		private DelegateCommand<string> _quitLogonCommand;
		public DelegateCommand<string> QuitLogonCommand =>
			_quitLogonCommand ?? (_quitLogonCommand = new DelegateCommand<string>(OnQuitLogonCommand).ObservesCanExecute(() => IsQuitCommandEnabled));

		private void OnCloseLogonCommand(string parameter)
		{
			RaiseRequestClose(new DialogResult(ParseStringToButtonResult(parameter)));
			_eventAggregator.GetEvent<TesterInfoEvent>().Publish((FirstName, LastName, Now));
			_eventAggregator.GetEvent<ConsoleSerialNumberEvent>().Publish(_shellModel.ConsoleSerialNumber);
			_eventAggregator.GetEvent<HospitalNameEvent>().Publish(_shellModel.HospitalName);
			_eventAggregator.GetEvent<FstVersionEvent>().Publish(_shellModel.FstVersion);
		}

		private async void OnQuitLogonCommand(string parameter)
		{
			IsQuitCommandEnabled = false;
			CreateOnHomeBatchFile();
			_eventAggregator.GetEvent<UserCommandEvent>().Publish((UserCommand.GoSmartFreeze, Now));
			await InvokeSmartFreeze();
			await TerminateServiceToolApp();
		}

		private async Task InvokeSmartFreeze()
		{
			var appLocation_ = ConfigurationManager.AppSettings[SmartFreezeAppPath];
			var appName_ = ConfigurationManager.AppSettings[SmartFreezeFileName];
			var smartFreezeApp_ = Path.Combine(appLocation_, appName_);

			if(!File.Exists(smartFreezeApp_))
			{
				return;
			}

			try
			{
				using(var smProcess_ = new Process())
				{
					smProcess_.StartInfo.FileName = smartFreezeApp_;
					smProcess_.StartInfo.WorkingDirectory = Path.GetDirectoryName(smartFreezeApp_) ?? string.Empty;
					smProcess_.StartInfo.CreateNoWindow = false;
					await Task.Run(() => smProcess_.Start());
				}
			}
			catch(Exception ex)
			{
				FieldServiceTrace.LogException(ex);
			}
		}

		private async Task TerminateServiceToolApp()
		{
			await _shellModel.TerminateConsole();
			await ThreadHelpers.WaitForAsync(1);
			Environment.Exit(0);
		}

		private void CreateOnHomeBatchFile()
		{
			using(var sw = File.CreateText(MonitorConstants.OnHomeBatchPath))
			{
				sw.WriteLine(MonitorConstants.DeleteFSTCmd);
				sw.WriteLine(MonitorConstants.DeleteOnHomeBatchCmd);
			}
		}

		public bool CanCloseDialog() => true;
		public void OnDialogClosed() { }
		public void OnDialogOpened(IDialogParameters parameters) => Title = parameters.GetValue<string>(Strings.DialogTitleKey);

		private string _title = string.Empty;
		public string Title
		{
			get => _title;
			set => SetProperty(ref _title, value);
		}

		private ButtonResult ParseStringToButtonResult(string parameter) => Enum.TryParse(parameter, out ButtonResult result) ? result : throw new ArgumentException();
		public virtual void RaiseRequestClose(DialogResult dialogResult) => RequestClose?.Invoke(dialogResult);
		public event Action<IDialogResult> RequestClose;

		#endregion IDialogAware

		#region INotifyDataErrorInfo

		private readonly Dictionary<string, List<string>> _errorsByPropertyName = new Dictionary<string, List<string>>();
		private readonly Regex nameValidationRegex = new Regex(@"^[A-Za-z][A-Za-z0-9\s_.]{1,19}$");
		public IEnumerable GetErrors(string propertyName) => _errorsByPropertyName.ContainsKey(propertyName) ? _errorsByPropertyName[propertyName] : null;
		public bool HasErrors => _errorsByPropertyName.Any();
		public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
		private void RaiseErrorsChanged(string propertyName) => ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));

		private void ValidateFirstName(string first)
		{
			ClearErrors(nameof(FirstName));
			if(string.IsNullOrEmpty(first))
			{
				AddError(nameof(FirstName), FirstNameEmptyErrorMessage);
				return;
			}
			if(!nameValidationRegex.Match(first).Success)
			{
				AddError(nameof(FirstName), FirstNameInvalidMessage);
			}
		}

		private void ValidateLastName(string last)
		{
			ClearErrors(nameof(LastName));
			if(string.IsNullOrEmpty(last))
			{
				AddError(nameof(LastName), LastNameEmptyErrorMessage);
				return;
			}
			if(!nameValidationRegex.Match(last).Success)
			{
				AddError(nameof(LastName), LastNameInvalidMessage);
			}
		}

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

		private void ClearErrors(string propertyName)
		{
			if(_errorsByPropertyName.ContainsKey(propertyName))
			{
				_errorsByPropertyName.Remove(propertyName);
				RaiseErrorsChanged(propertyName);
			}
		}

		private string _DBVersionDisplay;
		public string DBVersionDisplay
		{
			get => _DBVersionDisplay;
			set => SetProperty(ref _DBVersionDisplay, value);
		}

		#endregion INotifyDataErrorInfo
	}
}
