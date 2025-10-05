using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Module.CatheterTestTool.Models;
using Module.CatheterTestTool.PubSubEvents;
using Module.CatheterTestTool.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace Module.CatheterTestTool.ViewModels
{
  public class TestResultFileSelectorViewModel : BindableBase, IDialogAware
  {
    private DelegateCommand _okCommand;
    private DelegateCommand _cancelCommand;
    private bool _usbConnected;
    private string _usbDriveName;

    private readonly ITestDataFileManager _testDataFileManager;
    private readonly IEventAggregator _eventAggregator; 

    public event Action<IDialogResult> RequestClose;

    public TestResultFileSelectorViewModel(ITestDataFileManager testDataFileManager, IEventAggregator eventAggregator)
    {
      _testDataFileManager = testDataFileManager;
      _eventAggregator = eventAggregator;
      _eventAggregator.GetEvent<USBConnectionEvent>().Subscribe(UpdateUSBDriveInfo); 
      UpdateFileList();
    }

    public ICommand OkCommand
    {
      get => _okCommand = _okCommand ?? new DelegateCommand(ExecuteOkCommand, CanExecuteOkCommand);
    }

    public ICommand CancelCommand
    {
      get => _cancelCommand = _cancelCommand ?? new DelegateCommand(ExecuteCancelCommand, ()=>true);
    }

    private IList<CatheterTestResultFile> _testResultFileList;

    public IList<CatheterTestResultFile> TestResultFileList
    {
      get => _testResultFileList; 
      set => SetProperty(ref _testResultFileList, value);
    }

    public string Title => string.Empty;

    public string USBDriveName
    {
      get => _usbDriveName;
      set => SetProperty(ref _usbDriveName, value); 
    }

    public bool USBConnected
    {
      get => _usbConnected;
      set
      {
        SetProperty(ref _usbConnected, value);
        _okCommand?.RaiseCanExecuteChanged();
      }
    }

    public bool CanCloseDialog() => true;

    public void OnDialogClosed()
    {
      UnsubscribeSelectionChanged();
    }

    public void OnDialogOpened(IDialogParameters parameters)
    {
      UpdateUSBDriveInfo(parameters?.GetValue<string>(CatheterTestConstants.USB_DRIVE_NAME_PARAM) ?? string.Empty);
    }

    private void UpdateFileList()
    {
      var fileList = _testDataFileManager?.SearchTestResultFiles();
      UnsubscribeSelectionChanged();

      TestResultFileList = fileList != null && fileList.Count > 0 
                         ? fileList.Select(f => new CatheterTestResultFile(f)).ToList()
                         : null;

      SubscribeSelectionChanged();

      _okCommand?.RaiseCanExecuteChanged();
    }

    private void FileSelectionChanged(object sender, EventArgs args)
    {
      _okCommand?.RaiseCanExecuteChanged();
    }

    private void SubscribeSelectionChanged()
    {
      if (TestResultFileList == null)
        return;

      foreach (var f in TestResultFileList)
      {
        f.PropertyChanged += FileSelectionChanged;
      }
    }

    private void UnsubscribeSelectionChanged()
    {
      if (TestResultFileList == null)
        return;
      foreach (var f in TestResultFileList)
      {
        f.PropertyChanged -= FileSelectionChanged;
      }
    }

    private void ExecuteOkCommand()
    {
      var moveFileList = TestResultFileList
        .Where(f => f.Selected)
        .Select(f => f.FileName);

      _testDataFileManager.MoveTestDataFiles(moveFileList, USBDriveName); 
      RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
    }

    private bool CanExecuteOkCommand()
    {
      return USBConnected
             && TestResultFileList != null
             && TestResultFileList.Any(f=>f.Selected); 
    }

    private void ExecuteCancelCommand()
    {
      RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
    }

    private void UpdateUSBDriveInfo(string usbDriveName)
    {
      USBDriveName = usbDriveName;
      USBConnected = !string.IsNullOrEmpty(usbDriveName);
    }
  }
}
