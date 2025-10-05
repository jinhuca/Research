using Communication;
using ConsoleCommSimulator.Configuration;
using ConsoleCommSimulator.Interfaces;
using ConsoleCommSimulator.MessageProviders;
using ConsoleCommSimulator;
using Prism.Ioc;
using System;
using System.Linq;
using Shared;

namespace SmartAblationSystem
{
  using ConsoleCommSimulator.Helper;

  public partial class App
  {

    private bool IsInSimulationMode()
    {
      return true;
      var arguments = Environment.GetCommandLineArgs();
      return arguments.Any(a => a.TrimStart(new[] { '-', '/' }).ToUpper() == _simulationModeParameter);
    }

    private void RegisterSimulatorTypes(IContainerRegistry containerRegistry)
    {
      containerRegistry.Register<IDisplayConfigurationMonitor, DisplayConfigurationMonitorSim>();
      // Register CanBus Message Providers by name
      RegisterCanBusMessageProviders(containerRegistry);

      // Register Simulator
      containerRegistry.RegisterSingleton<ISimulatorConfiguration, SimulatorConfiguration>();

      var consoleSimulator = Container.Resolve<ConsoleSimulator>();
      containerRegistry.RegisterInstance<ICanBusCommunication>(consoleSimulator);
      containerRegistry.RegisterInstance<IGeneralPurposeInputOutput>(consoleSimulator);
    }

    private void RegisterCanBusMessageProviders(IContainerRegistry containerRegistry)
    {
      containerRegistry.Register<ICanBusMessageProvider, CmcuStatusMessageProvider>(nameof(CmcuStatusMessageProvider));
      containerRegistry.Register<ICanBusMessageProvider, PmcuStatusMessageProvider>(nameof(PmcuStatusMessageProvider));
      containerRegistry.Register<ICanBusMessageProvider, CatheterInfoMessageProvider>(nameof(CatheterInfoMessageProvider));
      containerRegistry.Register<ICanBusMessageProvider, PTMessageProvider>(nameof(PTMessageProvider));
      containerRegistry.Register<ICanBusMessageProvider, PSMessageProvider>(nameof(PSMessageProvider));
      containerRegistry.Register<ICanBusMessageProvider, TCMessageProvider>(nameof(TCMessageProvider));
      containerRegistry.Register<ICanBusMessageProvider, FMMessageProvider>(nameof(FMMessageProvider));
      containerRegistry.Register<ICanBusMessageProvider, LCMessageProvider>(nameof(LCMessageProvider));
      containerRegistry.Register<ICanBusMessageProvider, TSMessageProvider>(nameof(TSMessageProvider));
      containerRegistry.Register<ICanBusMessageProvider, CPMessageProvider>(nameof(CPMessageProvider));
      containerRegistry.Register<ICanBusMessageProvider, RTRMessageProvider>(nameof(RTRMessageProvider));
      containerRegistry.Register<ICanBusMessageProvider, cIMPMessageProvider>(nameof(cIMPMessageProvider));
      containerRegistry.Register<ICanBusMessageProvider, FirmwareVersionMessageProvider>(nameof(FirmwareVersionMessageProvider));
      containerRegistry.Register<ICanBusMessageProvider, CanBus2SensorStatusMessageProvider>(nameof(CanBus2SensorStatusMessageProvider));
      containerRegistry.Register<ICanBusMessageProvider, DMSSensorMessageProvider>(nameof(DMSSensorMessageProvider));
      containerRegistry.Register<ICanBusMessageProvider, ETSMessageProvider>(nameof(ETSMessageProvider));
      containerRegistry.Register<ICanBusMessageProvider, BloodPressureMessageProvider>(nameof(BloodPressureMessageProvider));
      containerRegistry.Register<ICanBusMessageProvider, HighResolutionDMSMessageProvider>(nameof(HighResolutionDMSMessageProvider));
      containerRegistry.Register<ICanBusMessageProvider, LCCalibrationMessageProvider>(nameof(LCCalibrationMessageProvider));
    }
  }
}
