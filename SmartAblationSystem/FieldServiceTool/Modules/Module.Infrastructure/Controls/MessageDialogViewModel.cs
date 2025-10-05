using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using static Module.Infrastructure.Constants.Strings;

namespace Module.Infrastructure.Controls
{
  public enum MessageDialogType
  {
		Information, 
		Warning,
		Error
  }

	public class MessageDialogViewModel : BindableBase, IDialogAware
	{
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

    private MessageDialogType _mssageDialogType;

    public MessageDialogType MessageDialogType
    {
      get => _mssageDialogType;
			set => SetProperty(ref _mssageDialogType, value);
    }

		public bool CanCloseDialog() => true;

		public void OnDialogClosed()
		{
		}

		public void OnDialogOpened(IDialogParameters parameters)
		{
			Title = parameters.GetValue<string>(DialogTitleKey);
			Message = parameters.GetValue<string>(DialogMessageKey);
      MessageDialogType = parameters.GetValue<MessageDialogType>(MessageDialogTypeKey);
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
