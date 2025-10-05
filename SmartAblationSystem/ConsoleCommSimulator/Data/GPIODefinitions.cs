namespace ConsoleCommSimulator.Data
{
  public enum GPIODefinitions
  {
    StopGPIOID = 0,
    WatchdogResetGPIOID = 1,
    SystemResetGPIOID = 2,
    FailResetGPIOID = 3,
    InjectionGPIOID = 4,
    AblateGPIOID = 5, 
    VacuumGPIOID = 6, 
    ChangeTankGPIOID = 7
  }

  public enum ActiveLevel
  {
    DeactivateLevel = 0,
    ActivateLevel = 1
  }

  public class GPIOState
  {
    public bool StopGPIO { get; set; }
    public bool AblateGPIO { get; set; }
    public bool SystemResetGPIO { get; set; }
    public bool WatchdogResetGPIO { get; set; }
    public bool VacuumResetGPIO { get; set; }
    public bool FailResetStatus { get; set; }
    public bool InjectionResetGPIO { get; set; }
    public bool ChangeTankGPIO { get; set; }
  } 

}
