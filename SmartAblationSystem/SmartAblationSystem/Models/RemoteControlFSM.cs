using SmartAblationSystem.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Communication.CanBusMessageDefinition;

namespace SmartAblationSystem.Models
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

           // string LogPathName = @"C:\Remote_log.txt";
           // File.AppendAllText(LogPathName, "Data=" + MembraneByte.ToString( ) + " " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss fff") + Environment.NewLine);
            if (MembraneByte == (int)SwitchState.StopButton)
            {
                currrentSwitchState = SwitchState.StopButton;
            }
            else if (MembraneByte == (int)SwitchState.SwitchStateDeactivated)
            {
                currrentSwitchState = SwitchState.SwitchStateDeactivated;
            }
            else if (MembraneByte == (int)SwitchState.StartButton)
            {
                currrentSwitchState = SwitchState.StartButton;
            }
            else if (MembraneByte == (int)SwitchState.AblationTimerDecrement)
            {
                currrentSwitchState = SwitchState.AblationTimerDecrement;
            }
            else if ( MembraneByte  == (int)SwitchState.AblationTimerIncrement)
            {
                currrentSwitchState = SwitchState.AblationTimerIncrement;
            }
            else if (MembraneByte  == (int)SwitchState.AblationSiteLeft)
            {
                currrentSwitchState = SwitchState.AblationSiteLeft;
            }
            else if (MembraneByte == (int)SwitchState.AblationSiteRight)
            {
                currrentSwitchState = SwitchState.AblationSiteRight;
            }
            else if (MembraneByte  == (int)SwitchState.BalloonDiameterIncrease)
            {
                currrentSwitchState = SwitchState.BalloonDiameterIncrease;
            }
            else if ( MembraneByte  == (int)SwitchState.BalloonDiameterDecrease)
            {
                currrentSwitchState = SwitchState.BalloonDiameterDecrease;
            }
            else
            {
                currrentSwitchState = SwitchState.Unknown;
            }
        }


        //private void DataArbitration(int MembraneByte)
        //{

        //    if ((MembraneByte | (int)SwitchState.StopButton) == (int)SwitchState.StopButton)
        //    {
        //        currrentSwitchState = SwitchState.StopButton;
        //    }

        //    else if ((MembraneByte | (int)SwitchState.StartButton) == (int)SwitchState.StartButton)
        //    {
        //        currrentSwitchState = SwitchState.StartButton;
        //    }

        //    else if ((MembraneByte | (int)SwitchState.AblationTimerDecrement) == (int)SwitchState.AblationTimerDecrement)
        //    {
        //        currrentSwitchState = SwitchState.AblationTimerDecrement;
        //    }

        //    else if ((MembraneByte | (int)SwitchState.AblationTimerIncrement) == (int)SwitchState.AblationTimerIncrement)
        //    {
        //        currrentSwitchState = SwitchState.AblationTimerIncrement;
        //    }

        //    else if ((MembraneByte | (int)SwitchState.AblationSiteLeft) == (int)SwitchState.AblationSiteLeft)
        //    {
        //        currrentSwitchState = SwitchState.AblationSiteLeft;
        //    }

        //    else if ((MembraneByte | (int)SwitchState.AblationSiteRight) == (int)SwitchState.AblationSiteRight)
        //    {
        //        currrentSwitchState = SwitchState.AblationSiteRight;
        //    }

        //    else if ((MembraneByte | (int)SwitchState.BalloonDiameterIncrease) == (int)SwitchState.BalloonDiameterIncrease)
        //    {
        //        currrentSwitchState = SwitchState.BalloonDiameterIncrease;
        //    }

        //    else if ((MembraneByte | (int)SwitchState.BalloonDiameterDecrease) == (int)SwitchState.BalloonDiameterDecrease)
        //    {
        //        currrentSwitchState = SwitchState.BalloonDiameterDecrease;
        //    }

        //    else if ((MembraneByte | (int)SwitchState.SwitchStateDeactivated) == (int)SwitchState.SwitchStateDeactivated)
        //    {
        //        currrentSwitchState = SwitchState.SwitchStateDeactivated;
        //    }
        //}

    }
}
