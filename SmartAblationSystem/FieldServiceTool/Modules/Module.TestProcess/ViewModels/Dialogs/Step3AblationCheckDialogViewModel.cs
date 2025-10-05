using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Module.Infrastructure.Constants;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using static Module.Infrastructure.Constants.Strings;

namespace Module.TestProcess.ViewModels.Dialogs
{
	public class Step3AblationCheckDialogViewModel : BindableBase, IDialogAware
	{
		private string _title = string.Empty;
		public string Title
		{
			get => _title;
			set => SetProperty(ref _title, value);
		}

		public string AblationFailureMessage => AblationFailureMsg;
		public string ContinueText => Strings.ContinueText;
		public string ContinueParameter { get; set; } = ButtonResult.Ignore.ToString();

		public string RetryText => Strings.RetryText;
		public string RetryParameter => ButtonResult.Retry.ToString();

		public string StopText => Strings.StopText;
		public string StopParameter => ButtonResult.Abort.ToString();

		private string _message = string.Empty;
		public string Message
		{
			get => _message;
			set => SetProperty(ref _message, value);
		}

		private DelegateCommand<string> _closeDialogCommand;
		public DelegateCommand<string> CloseDialogCommand => _closeDialogCommand ?? (_closeDialogCommand = new DelegateCommand<string>(CloseDialog));

		public bool CanCloseDialog() => true;

		public void OnDialogClosed() { }

		public void OnDialogOpened(IDialogParameters parameters)
		{
			Title = parameters.GetValue<string>(DialogTitleKey);
			Message = parameters.GetValue<string>(DialogMessageKey);
		}

		public event Action<IDialogResult> RequestClose;

		protected virtual void CloseDialog(string parameter)
		{
			ButtonResult result = ParseStringToButtonResult(parameter);
			RaiseRequestClose(new DialogResult(result));
		}

		private ButtonResult ParseStringToButtonResult(string parameter)
		{
			return Enum.TryParse(parameter, out ButtonResult result) ? result : throw new ArgumentException();
		}

		public virtual void RaiseRequestClose(DialogResult dialogResult)
		{
			RequestClose?.Invoke(dialogResult);
		}
	}
}
