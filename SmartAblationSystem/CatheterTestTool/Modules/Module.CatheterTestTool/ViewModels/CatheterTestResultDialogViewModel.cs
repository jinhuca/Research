
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Windows.Input;
using Module.CatheterTestTool.Models;
using Prism.Commands;

namespace Module.CatheterTestTool.ViewModels
{
  public class CatheterTestResultDialogViewModel : BindableBase, IDialogAware
  {
    private DelegateCommand _closeDialogCommand;
    public ICommand CloseDialogCommand => _closeDialogCommand ?? (_closeDialogCommand = new DelegateCommand(CloseDialog));

    public string Title => CatheterTestConstants.CATHETER_TEST_TOOL_TITLE;

    private bool _allTestsPassed = false;
    public bool AllTestsPassed
    {
      get => _allTestsPassed;
      set => SetProperty(ref _allTestsPassed, value); 
    }

    public event Action<IDialogResult> RequestClose;

    public bool CanCloseDialog() => true;

    public void OnDialogClosed()
    {
    }

    public void OnDialogOpened(IDialogParameters parameters)
    {
      AllTestsPassed = parameters.GetValue<bool>(CatheterTestConstants.TEST_RESULT_KEY);
    }

    private void CloseDialog()
    {
      RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
    }

  }
}
