using System;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

using static Module.CatheterTestTool.Models.CatheterTestConstants;

namespace Module.CatheterTestTool.ViewModels
{
  public class CatheterTestPopupMessageViewModel : BindableBase, IDialogAware
  {
    private string _okButtonText;
    public string OkButtonText
    {
      get => _okButtonText; 
      set => SetProperty(ref _okButtonText, value);
    }

    private string _cancelButtonText;

    public string CancelButtonText
    {
      get => _cancelButtonText; 
      set => SetProperty(ref _cancelButtonText, value);
    }

    private bool _isPassFailMessageDialog;
    public bool IsPassFailMessageDialog
    {
      get => _isPassFailMessageDialog;
      set => SetProperty(ref _isPassFailMessageDialog, value);
    }

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

    public void OnDialogClosed()
    {
    }

    public void OnDialogOpened(IDialogParameters parameters)
    {
      Title = parameters.GetValue<string>(POPUP_DIALOG_TITLE_KEY);
      Message = parameters.GetValue<string>(POPUP_DIALOG_MESSAGE_KEY);
      OkButtonText = parameters.GetValue<string>(POPUP_DIALOG_OKBUTTON_TEXT_KEY);
      CancelButtonText = parameters.GetValue<string>(POPUP_DIALOG_CANCELBUTTON_TEXT_KEY);
      IsPassFailMessageDialog = parameters.GetValue<bool>(POPUP_DIALOG_ISPASSFAIL_DIALOG_KEY);
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
