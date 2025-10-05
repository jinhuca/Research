using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Communication.CanBusMessageDefinition;

namespace Module.Console.Helpers
{
  public class RemoteControlFSM
  {
    private SwitchState currrentSwitchState;

    public SwitchState CurrrentSwitchState
    {
      get => currrentSwitchState;
      set => currrentSwitchState = value;
    }

    public RemoteControlFSM()
    {
      CurrrentSwitchState = new SwitchState();
    }

    public void MembraneSwitchStateLogic(byte[] data, int ID)
    {
      switch (ID)
      {
        case 26:
          DataArbitration(data[0]);
          break;
        default:
          break;
      }
    }

    private void DataArbitration(int MembraneByte)
    {
      switch (MembraneByte)
      {
        // string LogPathName = @"C:\Remote_log.txt";
        // File.AppendAllText(LogPathName, "Data=" + MembraneByte.ToString( ) + " " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss fff") + Environment.NewLine);
        case (int)SwitchState.StopButton:
          currrentSwitchState = SwitchState.StopButton;
          break;
        case (int)SwitchState.SwitchStateDeactivated:
          currrentSwitchState = SwitchState.SwitchStateDeactivated;
          break;
        case (int)SwitchState.StartButton:
          currrentSwitchState = SwitchState.StartButton;
          break;
        case (int)SwitchState.AblationTimerDecrement:
          currrentSwitchState = SwitchState.AblationTimerDecrement;
          break;
        case (int)SwitchState.AblationTimerIncrement:
          currrentSwitchState = SwitchState.AblationTimerIncrement;
          break;
        case (int)SwitchState.AblationSiteLeft:
          currrentSwitchState = SwitchState.AblationSiteLeft;
          break;
        case (int)SwitchState.AblationSiteRight:
          currrentSwitchState = SwitchState.AblationSiteRight;
          break;
        case (int)SwitchState.BalloonDiameterIncrease:
          currrentSwitchState = SwitchState.BalloonDiameterIncrease;
          break;
        case (int)SwitchState.BalloonDiameterDecrease:
          currrentSwitchState = SwitchState.BalloonDiameterDecrease;
          break;
        default:
          currrentSwitchState = SwitchState.Unknown;
          break;
      }
    }
  }
}
