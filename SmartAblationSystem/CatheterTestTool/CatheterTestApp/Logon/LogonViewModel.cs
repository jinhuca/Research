using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using CatheterTestApp.Properties;
using Module.CatheterTestTool.Models;
using Module.Infrastructure.Constants;
using Module.Infrastructure.PubSubEvents;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace CatheterTestlApp
{
	public class LogonViewModel : BindableBase, IDialogAware, INotifyDataErrorInfo
	{
        public string ApplicationTitle => CatheterTestConstants.CATHETER_TEST_TOOL_TITLE;
        public string OkText => "OK";
        public string OkButtonCommandParameter => "Yes";
        public const string FirstNameEmptyErrorMessage = "First name cannot be empty.";
        public const string FirstNameInvalidMessage = "First name must have 2 to 10 characters. \nstarted with letter, followed by letter, numeric, space, period, or underscore.";
        public const string LastNameEmptyErrorMessage = "Last name cannot be empty.";
        public const string LastNameInvalidMessage = "Last name must have 2 to 10 characters, \nstarted with letter, followed by letter, numeric, space, period, or underscore.";

        private bool _IsCloseCommandEnabled;
		public bool IsCloseCommandEnabled
		{
			get => _IsCloseCommandEnabled;
			set => SetProperty(ref _IsCloseCommandEnabled, value);
		}

		public string FirstNameTitle { get; } = Resources.FirstNameTitle;
		private string _FirstName = string.Empty;
		public string FirstName
		{
			get => _FirstName;
			set
			{
				if (_FirstName == value) return;
				ValidateFirstName(value);
				SetProperty(ref _FirstName, value);
				IsCloseCommandEnabled = !HasErrors && !string.IsNullOrEmpty(_FirstName) && !string.IsNullOrEmpty(_LastName);
			}
		}

        public string DBVersionDisplay => string.Empty;

		public string LastNameTitle { get; } = Resources.LastNameTitle;
		private string _LastName = string.Empty;
		public string LastName
		{
			get => _LastName;
			set
			{
				if (_LastName == value) return;
				ValidateLastName(value);
				SetProperty(ref _LastName, value);
				IsCloseCommandEnabled = !HasErrors && !string.IsNullOrEmpty(_FirstName) && !string.IsNullOrEmpty(_FirstName);
			}
		}

		private readonly IEventAggregator _eventAggregator;

		public LogonViewModel(IEventAggregator eventAggregator)
		{
			_eventAggregator = eventAggregator;

			ValidateFirstName(FirstName);
			ValidateLastName(LastName);
			IsCloseCommandEnabled = !HasErrors && !string.IsNullOrEmpty(_FirstName) && !string.IsNullOrEmpty(_FirstName);
		}

		private DelegateCommand<string> _closeLogonCommand;

		public DelegateCommand<string> CloseLogonCommand
			=> _closeLogonCommand ?? (_closeLogonCommand = new DelegateCommand<string>(OnCloseLogonCommand).ObservesCanExecute(() => IsCloseCommandEnabled));

		private void OnCloseLogonCommand(string parameter)
		{
			RaiseRequestClose(new DialogResult(ParseStringToButtonResult(parameter)));
			_eventAggregator.GetEvent<TesterInfoEvent>().Publish((FirstName, LastName, DateTime.Now));
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

		private readonly Dictionary<string, List<string>> _errorsByPropertyName = new Dictionary<string, List<string>>();
		private readonly Regex nameValidationRegex = new Regex(@"^[A-Za-z][A-Za-z0-9\s_.]{1,9}$");
		public IEnumerable GetErrors(string propertyName) => _errorsByPropertyName.ContainsKey(propertyName) ? _errorsByPropertyName[propertyName] : null;
		public bool HasErrors => _errorsByPropertyName.Any();
		public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
		private void RaiseErrorsChanged(string propertyName) => ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));

		private void ValidateFirstName(string first)
		{
			ClearErrors(nameof(FirstName));
			if (string.IsNullOrEmpty(first))
			{
				AddError(nameof(FirstName), FirstNameEmptyErrorMessage);
				return;
			}
			if (!nameValidationRegex.Match(first).Success)
			{
				AddError(nameof(FirstName), FirstNameInvalidMessage);
			}
		}

		private void ValidateLastName(string last)
		{
			ClearErrors(nameof(LastName));
			if (string.IsNullOrEmpty(last))
			{
				AddError(nameof(LastName), LastNameEmptyErrorMessage);
				return;
			}
			if (!nameValidationRegex.Match(last).Success)
			{
				AddError(nameof(LastName), LastNameInvalidMessage);
			}
		}

		private void AddError(string propertyName, string error)
		{
			if (!_errorsByPropertyName.ContainsKey(propertyName))
			{
				_errorsByPropertyName[propertyName] = new List<string>();
			}
			if (!_errorsByPropertyName[propertyName].Contains(error))
			{
				_errorsByPropertyName[propertyName].Add(error);
				RaiseErrorsChanged(propertyName);
			}
		}

		private void ClearErrors(string propertyName)
		{
			if (_errorsByPropertyName.ContainsKey(propertyName))
			{
				_errorsByPropertyName.Remove(propertyName);
				RaiseErrorsChanged(propertyName);
			}
		}
	}
}
