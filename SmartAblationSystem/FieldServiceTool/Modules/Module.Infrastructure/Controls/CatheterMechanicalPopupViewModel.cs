using System;
using System.Timers;
using System.Windows.Input;
using Module.Infrastructure.Constants;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace Module.Infrastructure.Controls
{
  public class CatheterMechanicalPopupViewModel : BindableBase, IDialogAware
  {
    private static readonly int DEFAULT_VOLUME_VALUE = 50;
    private readonly Timer _connectorImageTimer = new Timer(1000);

    public CatheterMechanicalPopupViewModel()
    {
      _connectorImageTimer.AutoReset = true;
    }

    public string Title => "SYSTEM NOTIFICATION";

    public event Action<IDialogResult> RequestClose;

    private int _currentVolume = 0;
    private Action<int> _updateVolume;

    private bool _showConnectorOn;
    public bool ShowConnectorOn
    {
      get => _showConnectorOn;
      set => SetProperty(ref _showConnectorOn, value);
    }

    public bool _isVolumeOn;
    public bool IsVolumeOn
    {
      get => _isVolumeOn;
      set => SetProperty(ref _isVolumeOn, value);
    }

    public string _message = Strings.ConnectCatheterMechanicallyMessage;

    public string Message
    {
      get => _message;
      set => SetProperty(ref _message, value);
    }

    public ICommand CloseDialogCommand => new DelegateCommand<string>(ExecuteCloseDialog);
    public ICommand MuteVolumeCommand => new DelegateCommand<string>(ExecuteMuteVolume);

    public bool CanCloseDialog() => true;

    public void OnDialogClosed()
    {
      _connectorImageTimer.Elapsed -= ConnectorImageTimeTick;
      _connectorImageTimer.Stop();
    }

    public void OnDialogOpened(IDialogParameters parameters)
    {
      _connectorImageTimer.Elapsed += ConnectorImageTimeTick;
      _connectorImageTimer.Start();

      _currentVolume = parameters.GetValue<int>(Strings.CurrentVolumeParameterKey);
      _updateVolume = parameters.GetValue<Action<int>>(Strings.UpdateVolumeActionParameterKey);

      if (parameters.ContainsKey(Strings.DialogMessageKey))
      {
        Message = parameters.GetValue<string>(Strings.DialogMessageKey);
      }

      IsVolumeOn = _currentVolume > 0;
      _updateVolume?.Invoke(_currentVolume <= 0 ? DEFAULT_VOLUME_VALUE : _currentVolume);
    }

    private void ConnectorImageTimeTick(object sender, EventArgs e)
    {
      ShowConnectorOn = !ShowConnectorOn; 
    }

    private void ExecuteCloseDialog(string closeParam)
    {
      if (Enum.TryParse(closeParam, out ButtonResult result))
      {
        RequestClose?.Invoke(new DialogResult(result));
      }
    }

    private void ExecuteMuteVolume(string muteParam)
    {
      if (muteParam == "MuteVolume")
      {
        IsVolumeOn = false;
        _updateVolume?.Invoke(0);
      }
      else
      {
        IsVolumeOn = true;
        _updateVolume?.Invoke(_currentVolume <= 0 ? DEFAULT_VOLUME_VALUE : _currentVolume);
      }
    }
  }
}
