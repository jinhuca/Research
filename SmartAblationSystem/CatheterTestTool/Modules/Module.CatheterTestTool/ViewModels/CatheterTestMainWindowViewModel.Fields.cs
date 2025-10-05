
using System.Collections.Generic;
using System.Reactive.Disposables;
using Communication;
using Module.CatheterTestTool.Models;
using Module.CatheterTestTool.Services;
using Module.Console.Interfaces;
using Prism.Events;
using Prism.Ioc;
using Prism.Services.Dialogs;

namespace Module.CatheterTestTool.ViewModels
{
  public partial class CatheterTestMainWindowViewModel
  {
    private static string _indentationSpace = "\t";
    private static IDictionary<CanBusMessageDefinition.MessageStateId, string> _systemStateToStringDict =
        new Dictionary<CanBusMessageDefinition.MessageStateId, string>()
        {
                { CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_UNKNOWN, CatheterTestConstants.STATE_UNKNOWN},
                { CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE, CatheterTestConstants.STATE_IDLE},
                { CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY, CatheterTestConstants.STATE_READY},
                { CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_INFLATION, CatheterTestConstants.STATE_INFLATION},
                { CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_ABLATION, CatheterTestConstants.STATE_ABLATION},
                { CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION, CatheterTestConstants.STATE_ABLATION},
                { CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_THAWING, CatheterTestConstants.STATE_THAWING},
                { CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_EXCEPTION, CatheterTestConstants.STATE_EXCEPTION}
        };

    private readonly IContainerProvider _containerProvider;
    private readonly IEventAggregator _eventAggregator;
    private readonly IDialogService _dialogService;
    private readonly IMachineModel _machineModel;
    private readonly ICatheterVisualTestService _catheterVisualService;
    private readonly ICatheterTestService _catheterTestService;
    private USBDriveConnectionManager.USBDriveConnectionManager _usbDriveConnectionManager;

    private readonly SerialDisposable _testStatusSubscriptionDisposable = new SerialDisposable();

#if DEBUG
    private long _cmcuAllWarningFlag = (long)(CanBusMessageDefinition.CMCUStatusError.LoadCellWeightWarning |
                                              CanBusMessageDefinition.CMCUStatusError.PressureInTankIsHighFanToBeOn |
                                              CanBusMessageDefinition.CMCUStatusError.PressurePT1InTankIsLow |
                                              CanBusMessageDefinition.CMCUStatusError.SubCoolerTemperatureIsHigh |
                                              CanBusMessageDefinition.CMCUStatusError.InjectionVentPressureOutOfRange
                                              );
#endif
  }
}
