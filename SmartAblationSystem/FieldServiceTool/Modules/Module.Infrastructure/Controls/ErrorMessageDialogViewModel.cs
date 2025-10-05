using Module.Infrastructure.Constants;
using Module.Infrastructure.PubSubEvents;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Module.Infrastructure.Controls
{
	public enum MessageCategory
  {
    Information,
    Warning,
    Error
  }

  public class ErrorMessageExtender : Tuple<long, string, string, string, int>
  {
    public ErrorMessageExtender(long t1, string t2, string t3, string t4, int t5, MessageCategory category) : base(t1, t2, t3, t4, t5)
    {
      MessageCategory = category;
    }

    public ErrorMessageExtender(Tuple<long, string, string, string, int> tuple, MessageCategory category) :
      this(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, category)
    {
    }

    public MessageCategory MessageCategory { get; }
  }

  public class ErrorMessageDialogViewModel : BindableBase, IDialogAware
  {
    private static readonly string MESSAGE_DIALOG_TITLE = "System Message";
    private static readonly string INVALID_SOLUTION_MESSAGE = "Solution";
    private static readonly int DEFAULT_VOLUME_VALUE = 50; 

    private readonly IEventAggregator _eventAggregator;
    private int _currentVolume = 0; 
    private Action<int> _updateVolume;

    public ErrorMessageDialogViewModel(IEventAggregator eventAggregator)
    {
      _eventAggregator = eventAggregator;
      _eventAggregator.GetEvent<ErrorListUpdateEvent>().Subscribe(HandleErrorListUpdate); 

      GoToPreviousCommand = new DelegateCommand(GoToPreviousErrorMessage, () => CurrentMessageIndex > 1);
      GoToNextCommand = new DelegateCommand(GoToNextErrorMessage, () => CurrentMessageIndex < ErrorList?.Count);

      MuteVolumeCommand = new DelegateCommand(ExecuteMuteVolume, () => true);
      EnableVolumeCommand = new DelegateCommand(ExecuteEnableVolume, () => true);
      CloseDialogCommand = new DelegateCommand<string>(ExecuteCloseDialog);
      ToggleErrorMessageCommand = new DelegateCommand(ExecuteToggleErrorMessage);
    }

    // public ICommand WarningCommand { get; set; }
    public DelegateCommand GoToPreviousCommand { get; set; }
    public DelegateCommand GoToNextCommand { get; set; }
    public ICommand UpdateVolumeCommand { get; set; }

    public ICommand MuteVolumeCommand { get; set; }
    public ICommand EnableVolumeCommand { get; set; }
    public ICommand CloseDialogCommand { get; set; }
    public DelegateCommand ToggleErrorMessageCommand { get; set; }

    private bool _displayEngineeringMessage = true;
    public bool DisplayEngineeringMessage
    {
      get => _displayEngineeringMessage;
      set => SetProperty(ref _displayEngineeringMessage, value);
    }

    private bool _isActionRequired;

    public bool IsActionRequired
    {
      get => _isActionRequired; 
      set => SetProperty(ref _isActionRequired, value);
    }

    private string _currentActionDescription;
    public string CurrentActionDescription
    {
      get => _currentActionDescription; 
      set => SetProperty(ref _currentActionDescription, value);
    }

    public bool _isVolumeOn;
    public bool IsVolumeOn
    {
      get => _isVolumeOn; 
      set => SetProperty(ref _isVolumeOn, value);
    }

    public bool HasErrorMessages => _errorList != null && _errorList.Count > 0;

    private string _messageTitle;
    public string MessageTitle
    {
      get => _messageTitle; 
      set => SetProperty(ref _messageTitle, value);
    }

    private string _currentMessage;
    public string CurrentMessage
    {
      get => _currentMessage; 
      set => SetProperty(ref _currentMessage, value);
    }

    private bool _canNavigateErrorList;
    public bool CanNavigateErrorList
    {
      get => _canNavigateErrorList; 
      set => SetProperty(ref _canNavigateErrorList, value);
    }

    private int _currentErrorIndex = 1;
    public int CurrentMessageIndex
    {
      get => _currentErrorIndex;
      set
      {
        SetProperty(ref _currentErrorIndex, value);
        GoToPreviousCommand.RaiseCanExecuteChanged();
        GoToNextCommand.RaiseCanExecuteChanged();
      }
    }

    public int TotalMessageCount => _errorList?.Count ?? 0;

    private List<ErrorMessageExtender> _errorList = new List<ErrorMessageExtender>();
    public List<ErrorMessageExtender> ErrorList => _errorList;

    public string Title => String.Empty;
    public string MessageDialogTitle => MESSAGE_DIALOG_TITLE;

    private bool _isErrorMessage;
    public bool IsErrorMessage
    {
      get => _isErrorMessage;
      set => SetProperty(ref _isErrorMessage, value);
    }

    public event Action<IDialogResult> RequestClose;

    public bool CanCloseDialog() => true;

    public void OnDialogClosed()
    {
    }

    public void OnDialogOpened(IDialogParameters parameters)
    {
      // We shall receive parameters: 
      // "ErrorList" -> IList<Tuple<long, string, string, string, int>>, error message list  
      // "CurrentVolume" -> int, the current volume value 
      // "UpdateVolumeAction" -> Action<int>, the action that adjust volume setting

      HandleErrorListUpdate(parameters.GetValue<IList<ErrorMessageExtender>>(Strings.ErrorListParameterKey));
      _currentVolume = parameters.GetValue<int>(Strings.CurrentVolumeParameterKey);
      _updateVolume = parameters.GetValue<Action<int>>(Strings.UpdateVolumeActionParameterKey); 
 
      Initialize();
    }

    private void Initialize()
    {
      NotifyErrorListUpdate();

      CurrentMessageIndex = 1;
      UpdateCurrentErrorMessage(CurrentMessageIndex - 1);

      IsVolumeOn = _currentVolume > 0;
    }

    private void GoToPreviousErrorMessage()
    {
      if (CurrentMessageIndex > 1)
      {
        CurrentMessageIndex--; 
        UpdateCurrentErrorMessage(CurrentMessageIndex - 1);
      } 
    }

    private void GoToNextErrorMessage()
    {
      if (CurrentMessageIndex < ErrorList.Count)
      {
        CurrentMessageIndex++;
        UpdateCurrentErrorMessage(CurrentMessageIndex - 1);
      }
    }

    private void ExecuteMuteVolume()
    {
      IsVolumeOn = false;
      _updateVolume?.Invoke(0);
    }

    private void ExecuteEnableVolume()
    {
      IsVolumeOn = true;
      _updateVolume?.Invoke(_currentVolume <=0 ? DEFAULT_VOLUME_VALUE : _currentVolume);
    }

    private void ExecuteCloseDialog(string closeParameter)
    {
      if (Enum.TryParse(closeParameter, out ButtonResult result))
      {
        RequestClose?.Invoke(new DialogResult(result)); 
      }
    }

    private void ExecuteToggleErrorMessage()
    {
      DisplayEngineeringMessage = !DisplayEngineeringMessage;

      var errorMessage = _errorList[CurrentMessageIndex-1];

      CurrentMessage = DisplayEngineeringMessage
        ? errorMessage.Item4
        : errorMessage.Item2;
    }

    private void UpdateCurrentErrorMessage(int index)
    {
      if (_errorList == null || _errorList.Count == 0) 
        return;

      // Tuple<ErrorCode, message, solutionMessage, CryterionMessage, errorType> 
      var errorMessage = _errorList[index];

      MessageTitle = CreateMessageTitle(errorMessage.Item4, errorMessage.Item1, errorMessage.Item5);
      CurrentMessage = DisplayEngineeringMessage 
          ? errorMessage.Item4
          : errorMessage.Item2; 

      IsActionRequired = !string.IsNullOrEmpty(errorMessage.Item3) && errorMessage.Item3 != INVALID_SOLUTION_MESSAGE;
      CurrentActionDescription = errorMessage.Item3; 

      IsErrorMessage = errorMessage.MessageCategory == MessageCategory.Error;
    }

    private string CreateMessageTitle(string cryterionMessage, long errorCode, int errorType)
    {
      var errorCodeHex = errorCode.ToString("X8"); 
      if (string.IsNullOrEmpty(cryterionMessage) || !cryterionMessage.Contains("-")) 
        return $"Error - {errorCodeHex} - {errorType}";
      
      return $"{cryterionMessage.Split('-')[0]} - {errorCodeHex} - {errorType}";
    }

    private void HandleErrorListUpdate(IList<ErrorMessageExtender> newErrorList)
    {
      if (newErrorList == null || newErrorList.Count == 0) return;

      lock (_errorList)
      {
        var distinctNewErrorList = newErrorList.Where(error => !ErrorExist(error)).ToList();
        if (distinctNewErrorList.Any())
        {
          _errorList.AddRange(distinctNewErrorList);
          NotifyErrorListUpdate();
        }
      }
    }

    private bool ErrorExist(ErrorMessageExtender newError)
    {
      return _errorList.Any(e => e.Item1 == newError.Item1 && e.Item5 == newError.Item5);
    }

    private void NotifyErrorListUpdate()
    {
      RaisePropertyChanged(nameof(ErrorList));
      RaisePropertyChanged(nameof(HasErrorMessages));
      RaisePropertyChanged(nameof(TotalMessageCount));
      CanNavigateErrorList = ErrorList.Count > 1;
      GoToPreviousCommand.RaiseCanExecuteChanged();
      GoToNextCommand.RaiseCanExecuteChanged();
    }
  }
}
