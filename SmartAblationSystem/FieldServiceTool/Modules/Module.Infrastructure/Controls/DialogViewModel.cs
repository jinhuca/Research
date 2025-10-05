using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using Module.Infrastructure.Constants;


namespace Module.Infrastructure.Controls
{
	public class DialogViewModel : BindableBase, IDialogAware
  {
    private static readonly string DefaultYesButtonText = Strings.YesText;
    private static readonly string DefaultNoButtonText = Strings.NoText;

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

    private string _yesButtonText;
    public string YesButtonText
    {
      get => _yesButtonText;
			set => SetProperty(ref _yesButtonText, value);
    }

    private string _noButtonText;
    public string NoButtonText
    {
      get => _noButtonText;
      set => SetProperty(ref _noButtonText, value);
    }

		public bool CanCloseDialog() => true;

		public void OnDialogClosed()
		{
		}

		public void OnDialogOpened(IDialogParameters parameters)
		{
			Title = parameters.GetValue<string>(Strings.DialogTitleKey);
			Message = parameters.GetValue<string>(Strings.DialogMessageKey); 

			YesButtonText = parameters.ContainsKey(Strings.DialogYesButtonTextKey) 
        ? parameters.GetValue<string>(Strings.DialogYesButtonTextKey) 
        : DefaultYesButtonText;
      NoButtonText = parameters.ContainsKey(Strings.DialogNoButtonTextKey)
        ? parameters.GetValue<string>(Strings.DialogNoButtonTextKey)
        : DefaultNoButtonText; 
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
