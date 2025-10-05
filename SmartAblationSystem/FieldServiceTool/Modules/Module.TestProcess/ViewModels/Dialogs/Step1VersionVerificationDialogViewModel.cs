using Module.Infrastructure.Constants;
using Module.Infrastructure.TestResults.Interfaces;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;

namespace Module.TestProcess.ViewModels.Tests
{
	public class Step1VersionVerificationDialogViewModel : BindableBase, IDialogAware
	{
		#region Text for versions

		public string BootloaderText => "Bootloader";
		public string ApplicationText => "Application";
		public string FirmwareText => "Firmware";
		public string CMCUBootText => "Control:";
		public string CMCUBooTVersionText => "Control:";
		public string CMCUVersionText => "Control:";
		public string CPLDVersionText => "CPLD:";
		public string DBVersionText => "Database:";
		public string GUIVersionText => "SmartFreeze:";
		public string ICBBootVersionText => "ICB:";
		public string ICBVersionText => "ICB:";
		public string PMCUBootVersionText => "Patient:";
		public string PMCUBootText => "Patient:";
		public string PMCUVersionText => "Patient:";
		public string RCMCUBootVersionText => "Remote:";
		public string RCMCUVersionText => "Remote:";
		public string RMCUBootVersionText => "Repeater:";
		public string RMCUVersionText => "Repeater:";

    public string RemindMessage => "Verify the displayed software and firmware versions match those allowed in SB-901.";

    #endregion Text for versions

		#region Values for versions

		private IVersionTestResult _versionParams;
		public IVersionTestResult VersionParams
		{
			get => _versionParams;
			set => SetProperty(ref _versionParams, value);
		}

		#endregion Values for versions

		private DelegateCommand<string> _closeDialogCommand;
		public DelegateCommand<string> CloseDialogCommand => _closeDialogCommand ?? (_closeDialogCommand = new DelegateCommand<string>(CloseDialog));

		private string _title = string.Empty;
		public string Title
		{
			get => _title;
			set => SetProperty(ref _title, value);
		}

		private string _message = string.Empty;
		public string Message
		{
			get => _message;
			set => SetProperty(ref _message, value);
		}

		public bool CanCloseDialog() => true;

		public void OnDialogClosed() { }

		public void OnDialogOpened(IDialogParameters parameters)
		{
			Title = parameters.GetValue<string>(Strings.DialogTitleKey);
			Message = parameters.GetValue<string>(Strings.DialogMessageKey);
			VersionParams = parameters.GetValue<IVersionTestResult>(Strings.VersionParameters);
		}

		protected virtual void CloseDialog(string parameter)
		{
			ButtonResult result = ParseStringToButtonResult(parameter);
			RaiseRequestClose(new DialogResult(result));
		}

		public virtual void RaiseRequestClose(DialogResult dialogResult)
		{
			RequestClose?.Invoke(dialogResult);
		}

		public event Action<IDialogResult> RequestClose;

		private ButtonResult ParseStringToButtonResult(string parameter)
		{
			return Enum.TryParse(parameter, out ButtonResult result) ? result : throw new ArgumentException();
		}
	}
}
