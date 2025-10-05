using Module.Infrastructure.PubSubEvents;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using static Module.Infrastructure.Constants.Strings;

namespace Module.TestProcess.ViewModels.Dialogs
{
	public class RationaleDialogViewModel : BindableBase, IDialogAware
  {
    private static readonly int DefaultMaxTextLength = 500;  
		public string OkText => "OK";
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

		private string _rationaleMsg = string.Empty;
		public string RationaleMsg
		{
			get => _rationaleMsg;
			set => SetProperty(ref _rationaleMsg, value);
		}

    private int _maxTextLength = DefaultMaxTextLength; 
		public int MaxTextLength
    {
      get => _maxTextLength;
			set => SetProperty(ref _maxTextLength, value);
    }

		public bool CanCloseDialog() => true;

		private readonly IEventAggregator _eventAggregator;
		public RationaleDialogViewModel(IEventAggregator eventAggregator)
		{
			_eventAggregator = eventAggregator;
		}

		public void OnDialogClosed()
		{
			_eventAggregator.GetEvent<RetryRationaleEvent>().Publish((RationaleMsg, Message));
		}

		public void OnDialogOpened(IDialogParameters parameters)
		{
			Title = parameters.GetValue<string>(DialogTitleKey);
			Message = parameters.GetValue<string>(DialogMessageKey);
			RationaleMsg = parameters.GetValue<string>(ParamIdKey);
      
      MaxTextLength = parameters.ContainsKey(MaxTextLengthInTextBoxKey) 
                      ? parameters.GetValue<int>(MaxTextLengthInTextBoxKey) 
                      : DefaultMaxTextLength;
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
