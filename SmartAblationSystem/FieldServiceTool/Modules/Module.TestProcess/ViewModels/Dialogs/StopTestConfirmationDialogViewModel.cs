using System;
using Module.Infrastructure;
using Module.Infrastructure.Constants;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace Module.TestProcess.ViewModels.Dialogs
{
  public class StopTestConfirmationDialogViewModel : BindableBase, IDialogAware
  {
		private static readonly string DefaultYesButtonText = Strings.YesText;
		private static readonly string DefaultNoButtonText = Strings.NoText;

		private DelegateCommand<string> _continueTestCommand;
		public DelegateCommand<string> ContinueTestCommand => _continueTestCommand ?? (_continueTestCommand = new DelegateCommand<string>(CloseDialog).ObservesCanExecute(() => CanContinueTest));

    private DelegateCommand<string> _stopTestCommand;
    public DelegateCommand<string> StopTestCommand => _stopTestCommand ?? (_stopTestCommand = new DelegateCommand<string>(CloseDialog));

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

    private bool _canContinueTest = true;
    public bool CanContinueTest
    {
      get => _canContinueTest;
			set => SetProperty(ref _canContinueTest, value);
    }

		public bool CanCloseDialog() => true;

		public void OnDialogClosed()
		{
      _sessionStatusDisposable?.Dispose();
		}

    // private IObservable<SessionStatus> _sessionStatusObservable;  
    private IDisposable _sessionStatusDisposable = null; 

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

      var sessionStatusObservable = parameters.ContainsKey(Strings.SessionStatusParameterKey) 
        ? parameters.GetValue<IObservable<SessionStatus>>(Strings.SessionStatusParameterKey)
				: null;

      if (sessionStatusObservable != null)
      {
        _sessionStatusDisposable = sessionStatusObservable.Subscribe(
          status_ => CanContinueTest = status_ != SessionStatus.Finished &&
                                                                        status_ != SessionStatus.Exception &&
                                                                        status_ != SessionStatus.Ready && 
                                                                        status_ != SessionStatus.Stopped);
      }
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
