
namespace SmartAblationSystem.ViewModels
{
  using System.Collections.Generic;
  using System;
  using System.ComponentModel;
  using System.Reactive.Linq;
  using System.Threading.Tasks;
  using System.Windows.Input;

  using Prism.Commands;
  using Prism.Mvvm;

  using SmartAblationSystem.Helpers;
  using SmartAblationSystem.Views;

  public class MessagePopupViewModel : BindableBase
  {
    public enum DisplayingFormat
    {
      UNKNOWN = 0,
      HOSPITAL= 1,
      BSC = 2
    }

    private static readonly string _systemNotificationMessageTitle = "SYSTEM NOTIFICATION";  
    private static readonly string _systemMessageTitle = "SYSTEM MESSAGE";  
    private static readonly string _warningMessageTitle = "WARNING MESSAGE";
    private static readonly string _defaultMessageId = "Error";

    private static readonly IDictionary<MessagePopup.MessageType, string> _defaultCaptionTitleByType =
      new Dictionary<MessagePopup.MessageType, string>()
        {
          { MessagePopup.MessageType.SystemMessage, _systemMessageTitle}, 
          { MessagePopup.MessageType.ErrorMessage, _systemNotificationMessageTitle },
          { MessagePopup.MessageType.WarningMessage, _warningMessageTitle }
        };

    private static readonly string _yesButtonContent = "YES";
    private static readonly string _noButtonContent = "NO";
    private static readonly string _okButtonContent = "OK";
    private static readonly string _cancelButtonContent = "Cancel";

    private readonly CommonViewModel _commonViewModel;
    private List<Tuple<long, string, string, string, Enumeration.ErrorTypes >> errorList = new List<Tuple<long, string, string, string, Enumeration.ErrorTypes>>();
    private DisplayingFormat _displayingMessageFormat = DisplayingFormat.HOSPITAL;

    private MessagePopup.MessageType _errorListMessageType = MessagePopup.MessageType.SystemMessage;

    private uint _previousVolumeValue = 0;

    private bool _can1WasInError;
    private bool _can2WasInError;

    public MessagePopupViewModel(CommonViewModel commonViewModel)
    {
      _commonViewModel = commonViewModel;
      _previousVolumeValue = this._commonViewModel.RequiredVolume; 

      SwitchEngineeringMessageCommand = new DelegateCommand(SwitchEngineeringMessage, () => true);
      this.NavigatePreviousMessageCommand = new DelegateCommand(this.ExecuteNavigatePreviousMessage).ObservesCanExecute(() => CanNavigatePrevious);
      this.NavigateNextMessageCommand = new DelegateCommand(this.ExecuteNavigateNextMessage).ObservesCanExecute(() => this.CanNavigateNext);
      MuteVolumeCommand = new DelegateCommand(ExecuteMuteVolumeCommand).ObservesCanExecute(()=>CanMuteVolume);

      /** CAN 1 **/
      _can1WasInError = this._commonViewModel.IsCanOneInError;
      if (_can1WasInError)
        _commonViewModel.IsCanOneWasInError = true;

      /** CAN 2 **/
      _can2WasInError = this._commonViewModel.IsCanTwoInError;
      if (_can2WasInError)
        this._commonViewModel.IsCanTwoWasInError = true;

      Observable.FromEventPattern<PropertyChangedEventArgs>(this._commonViewModel, "PropertyChanged")
        .Where(e => e.EventArgs.PropertyName == nameof(CommonViewModel.IsCanOneWasInError))
        .Subscribe(_ => CanMuteVolume = !this._commonViewModel.IsCanOneWasInError);
    }

    #region public methods 
    public void InitializeMessageSettings(string message, 
                                          MessagePopup.MessageType messageType = MessagePopup.MessageType.SystemMessage,
                                          MessagePopup.ButtonType buttonType = MessagePopup.ButtonType.YesNo, 
                                          string messageTitle = "")
    {
      CurrentMessage = message; 
      MessageType = messageType;
      ConfigureButtons(buttonType);
      
      CaptionTitle = GetCaptionTitleByMessageType(messageType, messageTitle);

      HasCryterionMessage = false; 
      IsActionRequired = false; 
      TotalNumOfMessages = 1;
      HasMessageId = false; 
      IsVolumeControlEnabled = messageType == MessagePopup.MessageType.ErrorMessage;
      CurrentMessageId = HasMessageId ? _defaultMessageId : String.Empty;
    }

    public void InitializeMessageSettings(List<Tuple<long, string, string, string>> errors,
                                          MessagePopup.MessageType messageType = MessagePopup.MessageType.SystemMessage,
                                          MessagePopup.ButtonType buttonType = MessagePopup.ButtonType.YesNo,
                                          string messageTitle = "",
                                          bool isActionRequired = false, 
                                          Enumeration.ErrorTypes errorType = Enumeration.ErrorTypes.Unknown)
    {
      IsActionRequired = isActionRequired;

      if (errors == null || errors.Count == 0)
        return;

      errorList = ConvertToFiveElementTupe(errors, errorType); 

      TotalNumOfMessages = errors.Count;
      CurrentMessageIndex = 1; 

      ConfigureButtons(buttonType);
      HasMessageId = IsErrorOrWarningMessage(messageType); 
      IsVolumeControlEnabled = messageType == MessagePopup.MessageType.ErrorMessage;

      _errorListMessageType = messageType;
      // MessageType = messageType;
      CaptionTitle = GetCaptionTitleByMessageType(messageType, messageTitle);

      _displayingMessageFormat = DisplayingFormat.HOSPITAL; 
      
      UpdateCurrentMessage(CurrentMessageIndex);
    }

    public void ResetVolumeValue(bool muteVolume)
    {
      if (!this._can1WasInError && !this._commonViewModel.IsCanOneWasInError && !this._commonViewModel.IsCanOneReseted)
      {
        var requiredVolumeValue = muteVolume ? 0 : this._previousVolumeValue;  
        Task.Delay(1000).ContinueWith(_ => _commonViewModel.RequiredVolume = requiredVolumeValue);
      } 
    }

    public void CleanUpForClose()
    {
      if (_commonViewModel.IsCanOneWasInError)
      {
        _commonViewModel.IsCanOneReseted = true;
        
        // Reset CAN1
        _commonViewModel.ResetCanOneStopWatch();

        // Prevents CAN1 Error Message to start stacking the same error message
        if (_commonViewModel.ErrorIdMessageAndSolutionList != null && _commonViewModel.ErrorIdMessageAndSolutionList.Count != 0)
        {
          _commonViewModel.ErrorIdMessageAndSolutionList.Clear();
        }
                
        Task.Delay(3000).ContinueWith(t => _commonViewModel.IsCanOneWasInError = false);
      }

      if (!_commonViewModel.IsCanOneWasInError && _commonViewModel.IsCanOneReseted)
      {
        _commonViewModel.IsCanOneReseted = false;
      }

      /** CAN2 **/
      if (_commonViewModel.IsCanTwoWasInError)
      {
        if(_commonViewModel.ErrorIdMessageAndSolutionList != null && _commonViewModel.ErrorIdMessageAndSolutionList.Count != 0)
        {
          // Prevents CAN2 Error Message to start stacking the same error message
          _commonViewModel.ErrorIdMessageAndSolutionList.Clear();
        }
                
        Task.Delay(3000).ContinueWith(t => _commonViewModel.IsCanTwoWasInError = false);
      }
    }
    #endregion public methods 

    #region properties 
    public ICommand NavigatePreviousMessageCommand { get; }
    public ICommand NavigateNextMessageCommand { get; }
    public ICommand SwitchEngineeringMessageCommand { get; }
    public ICommand MuteVolumeCommand { get; }

    private bool _canNavigatePrevious = false;

    public bool CanNavigatePrevious
    {
      get => this._canNavigatePrevious;
      set => SetProperty(ref this._canNavigatePrevious, value);
    }

    private bool _canNavigateNext = true;

    public bool CanNavigateNext
    {
      get => this._canNavigateNext;
      set => SetProperty(ref this._canNavigateNext, value);
    }
    
    private string _captionTitle = _systemNotificationMessageTitle;

    public string CaptionTitle
    {
      get => this._captionTitle;
      set => SetProperty(ref this._captionTitle, value);
    }

    private MessagePopup.MessageType _messageType = MessagePopup.MessageType.ErrorMessage;

    public MessagePopup.MessageType MessageType
    {
      get => _messageType;
      set => SetProperty(ref _messageType, value);
    }

    private string _currentMessageId = string.Empty;
    public string CurrentMessageId 
    {
      get => _currentMessageId;
      set => SetProperty(ref this._currentMessageId, value);
    }

    private string _currentMessage = string.Empty;
    public string CurrentMessage 
    {
      get => _currentMessage;
      set => SetProperty(ref this._currentMessage, value);
    }

    private string _currentSolution = string.Empty;

    public string CurrentSolution
    {
      get => this._currentSolution;
      set => SetProperty(ref this._currentSolution, value);
    }

    private int _currentMessageIndex = 0;

    public int CurrentMessageIndex
    {
      get => _currentMessageIndex;
      set => SetProperty(ref this._currentMessageIndex, value); 
    }

    private int _totalNumOfMessages = 0;

    public int TotalNumOfMessages
    {
      get => _totalNumOfMessages;
      set
      {
        this.SetProperty(ref this._totalNumOfMessages, value);
        this.RaisePropertyChanged(nameof(HasMoreThan1Messages));
      }
    }

    private bool _hasMoreThan1Messages = true;

    public bool HasMoreThan1Messages
    {
      get => TotalNumOfMessages > 1;
    }

    private bool _isActionRequired = true;

    public bool IsActionRequired
    {
      get => this._isActionRequired;
      set => SetProperty(ref this._isActionRequired, value);
    }

    private bool _isVolumeMuted = false;

    public bool IsVolumeMuted
    {
      get => _isVolumeMuted;
      set => SetProperty(ref this._isVolumeMuted, value);
    }

    private bool _canMuteVolume = true;

    public bool CanMuteVolume
    {
      get => _canMuteVolume;
      set => SetProperty(ref this._canMuteVolume, value);
    }

    private string _leftButtonText = _yesButtonContent;

    public string LeftButtonText
    {
      get => this._leftButtonText; 
      set => this.SetProperty(ref this._leftButtonText, value);
    }

    private string _rightButtonText = _noButtonContent;

    public string RightButtonText
    {
      get => this._rightButtonText; 
      set => this.SetProperty(ref this._rightButtonText, value);
    }

    private bool _hasTwoButtons = true;

    public bool HasTwoButtons
    {
      get => this._hasTwoButtons;
      set => SetProperty(ref this._hasTwoButtons, value);
    }

    private bool _isVolumeControlEnabled = true;

    public bool IsVolumeControlEnabled
    {
      get => this._isVolumeControlEnabled;
      set => SetProperty(ref this._isVolumeControlEnabled, value);
    }

    private bool _hasCryterionMessage = true;

    public bool HasCryterionMessage
    {
      get => _hasCryterionMessage;
      set => SetProperty(ref this._hasCryterionMessage, value); 
    }

    private bool _hasMessageId = true;

    public bool HasMessageId
    {
      get => _hasMessageId;
      set => SetProperty(ref this._hasMessageId, value); 
    }
    #endregion properties 

    #region private methods
    private void ExecuteMuteVolumeCommand()
    {
      this.ResetVolumeValue(IsVolumeMuted); 
    }

    private void UpdateCanNavigateMessage()
    {
      CanNavigatePrevious = CurrentMessageIndex > 1; 
      CanNavigateNext = CurrentMessageIndex < TotalNumOfMessages;
    }

    private void ExecuteNavigatePreviousMessage()
    {
      --CurrentMessageIndex;
      UpdateNavigateMessage(); 
    }

    private void ExecuteNavigateNextMessage()
    {
      ++CurrentMessageIndex;
      UpdateNavigateMessage(); 
    }

    private void UpdateNavigateMessage()
    {
      UpdateCanNavigateMessage();
      UpdateCurrentMessage(CurrentMessageIndex); 
    }
    private void SwitchEngineeringMessage()
    {
      if (this._displayingMessageFormat == DisplayingFormat.HOSPITAL)
        this._displayingMessageFormat = DisplayingFormat.BSC;
      else
        this._displayingMessageFormat = DisplayingFormat.HOSPITAL; 

      UpdateCurrentMessage(CurrentMessageIndex);
    }

    private void ConfigureButtons(MessagePopup.ButtonType buttonType)
    {
      switch (buttonType)
      {
        case MessagePopup.ButtonType.YesNo:
          LeftButtonText = _yesButtonContent;
          RightButtonText = _noButtonContent;
          HasTwoButtons = true;
          break;
        case MessagePopup.ButtonType.OkCancel:
          LeftButtonText = _okButtonContent;
          RightButtonText = _cancelButtonContent;
          HasTwoButtons = true;
          break;
        case MessagePopup.ButtonType.Ok:
          LeftButtonText = _okButtonContent;
          RightButtonText = string.Empty;
          HasTwoButtons = false;
          break;
        default:
          LeftButtonText = _yesButtonContent;
          RightButtonText = _noButtonContent;
          HasTwoButtons = true;
          break;
      }
    }

    private void UpdateCurrentMessage(int messageIndex)
    {
      if (messageIndex < 1 || messageIndex > TotalNumOfMessages)
        return;

      var errorTuple = this.errorList[messageIndex - 1];
      HasCryterionMessage = _errorListMessageType == MessagePopup.MessageType.ErrorMessage 
                            && !string.IsNullOrEmpty(errorTuple.Item4);

      CurrentMessage = this._displayingMessageFormat == DisplayingFormat.BSC && HasCryterionMessage
                         ? errorTuple.Item4
                         : errorTuple.Item2;

      CurrentSolution = errorTuple.Item3;

      var isWarningMessage = (_errorListMessageType == MessagePopup.MessageType.ErrorMessage
                              && !string.IsNullOrEmpty(errorTuple.Item4) && errorTuple.Item4.Contains("Warning")) 
                             || _errorListMessageType == MessagePopup.MessageType.WarningMessage;

      MessageType = isWarningMessage ? MessagePopup.MessageType.WarningMessage : _errorListMessageType;
      CurrentMessageId = ConfigureMessageId(MessageType == MessagePopup.MessageType.ErrorMessage, errorTuple.Item1, errorTuple.Item4, errorTuple.Item5);
    }

    #endregion private methods

    #region private static methods
    private static string GetCaptionTitleByMessageType(MessagePopup.MessageType messageType, string title = "")
    {
      return string.IsNullOrEmpty(title) 
               ? _defaultCaptionTitleByType[messageType]
               : title;
    }

    private static List<Tuple<long, string, string, string, Enumeration.ErrorTypes>> ConvertToFiveElementTupe(List<Tuple<long, string, string, string>> errors, Enumeration.ErrorTypes errorType)
    {
      var errorListWithType = new List<Tuple<long, string, string, string, Enumeration.ErrorTypes>>();

      foreach (Tuple<long, string, string, string> error in errors)
      {
        errorListWithType.Add(Tuple.Create(error.Item1, error.Item2, error.Item3, error.Item4, errorType));
      }


      return errorListWithType;
    }

    private string ConfigureMessageId(bool isErrorMessage, long errorCode, string cryterionMessage, Enumeration.ErrorTypes errorType)
    {
      var messageId = isErrorMessage ? _defaultMessageId : string.Empty;
      if (!string.IsNullOrEmpty(cryterionMessage))
      {
        if (cryterionMessage.Contains("-"))
        {
          messageId = cryterionMessage.Split('-')[0];
        }
      }

      messageId = $"{messageId} - {errorCode:X8}{BoardTypeToString(errorType)}";
      return messageId; 
    } 

    private static string BoardTypeToString(Enumeration.ErrorTypes errorType)
    {
      string boardType = string.Empty;

      switch (errorType)
      {
        case Enumeration.ErrorTypes.CMCU:

          boardType = "-1";
          break;
        case Enumeration.ErrorTypes.PMCU:
          boardType = "-2";
          break;                  
      }

      return boardType; 
    }

    private bool IsErrorOrWarningMessage(MessagePopup.MessageType messageType)
    {
      return messageType == MessagePopup.MessageType.ErrorMessage
             || messageType == MessagePopup.MessageType.WarningMessage;
    }
    #endregion private static methods
  }
}
