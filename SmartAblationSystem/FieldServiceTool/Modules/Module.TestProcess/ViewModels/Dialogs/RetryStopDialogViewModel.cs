using Module.Infrastructure.Constants;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using static Module.Infrastructure.Constants.Strings;

namespace Module.TestProcess.ViewModels.Dialogs
{
	public class RetryStopDialogViewModel : BindableBase, IDialogAware
	{
		public string StopText => Strings.StopText;
		public string StopParameter => ButtonResult.Abort.ToString();

		private DelegateCommand<string> _closeDialogCommand;
		public DelegateCommand<string> CloseDialogCommand => _closeDialogCommand ?? (_closeDialogCommand = new DelegateCommand<string>(CloseDialog));

		private string _retryText = Strings.RetryText;

    public string RetryText
    {
			get => _retryText;
			set => SetProperty(ref _retryText, value);
    }

    public string RetryParameter => ButtonResult.Retry.ToString();

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

		public void OnDialogClosed()
		{
		}

		public void OnDialogOpened(IDialogParameters parameters)
		{
			Title = parameters.GetValue<string>(DialogTitleKey);
			Message = parameters.GetValue<string>(DialogMessageKey);

      RetryText = parameters.ContainsKey(RetryButtonTextKey) 
        ? parameters.GetValue<string>(RetryButtonTextKey)
        : Strings.RetryText;
    }

    protected virtual void CloseDialog(string parameter)
		{
			ButtonResult result = ParseStringToButtonResult(parameter);
			RaiseRequestClose(new DialogResult(result));
		}

		public event Action<IDialogResult> RequestClose;
		public virtual void RaiseRequestClose(DialogResult dialogResult)
		{
			RequestClose?.Invoke(dialogResult);
		}

		private ButtonResult ParseStringToButtonResult(string parameter)
		{
			return Enum.TryParse(parameter, out ButtonResult result) ? result : throw new ArgumentException();
		}
	}
}
