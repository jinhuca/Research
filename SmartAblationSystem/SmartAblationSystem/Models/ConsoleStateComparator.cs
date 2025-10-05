using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAblationSystem.Models
{
    public class ConsoleStateComparator
    {
        private Data Data = new Data();
        public ConsoleStateComparator(Data data)
        {
            this.Data = data;
        }

        public ConsoleStateComparator( )
        {

        }

        public bool IsConsoleStateChanged(ConsoleVersion consoleVersion)
        {
            ConsoleVersion currentConsoleVersion = this.Data.DataAccess.GetLatestVersion();

            if (currentConsoleVersion == null)
            {
                return true;
            }
            else
            {
                if (currentConsoleVersion.Software != consoleVersion.Software)
                {

                }

                else if (currentConsoleVersion.ControlFirmware != consoleVersion.ControlFirmware)
                {

                }
                else if(currentConsoleVersion.ControlFirmwareBootLoader != consoleVersion.ControlFirmwareBootLoader)
                {

                }
                else if(currentConsoleVersion.CPLDFirmware != consoleVersion.CPLDFirmware)
                {

                }
                else if(currentConsoleVersion.RemoteFirmware != consoleVersion.RemoteFirmware)
                {

                }
                else if(currentConsoleVersion.RemoteFirmwareBootLoader != consoleVersion.RemoteFirmwareBootLoader)
                {

                }
                else if(currentConsoleVersion.PatientFirmware != consoleVersion.PatientFirmware)
                {

                }
                else if(currentConsoleVersion.PatientFirmwareBootLoader != consoleVersion.PatientFirmwareBootLoader)
                {

                }
                else if(currentConsoleVersion.RepeaterFirmware != consoleVersion.RepeaterFirmware)
                {

                }
                else if(currentConsoleVersion.RepeaterFirmwareBootLoader != consoleVersion.RepeaterFirmwareBootLoader)
                {

                }
                else if(currentConsoleVersion.ICBFirmware != consoleVersion.ICBFirmware)
                {

                }
                else if(currentConsoleVersion.ICBFirmwareBootLoader != consoleVersion.ICBFirmwareBootLoader)
                {

                }
                else if(currentConsoleVersion.CatheterFirmware != consoleVersion.CatheterFirmware)
                {

                }

            }
            return false;
        }

        public void VerifyAndUpdateStatiqueDevices(ConsoleVersion consoleVersion)
        {
            bool isAnyConsoleStateChanged = false;

            ConsoleVersion currentConsoleVersion = this.Data.DataAccess.GetLatestVersion();

            if (currentConsoleVersion == null)
            {
                this.Data.DataAccess.AddVersion(consoleVersion);
                return;
            }

            else
            {

                if (currentConsoleVersion.Software != consoleVersion.Software)
                {
                    currentConsoleVersion.Software = consoleVersion.Software;
                    isAnyConsoleStateChanged = true;
                }

                if (currentConsoleVersion.ControlFirmware != consoleVersion.ControlFirmware && consoleVersion.ControlFirmware != "0")
                {
                    currentConsoleVersion.ControlFirmware = consoleVersion.ControlFirmware;
                    isAnyConsoleStateChanged = true;
                }
                if (currentConsoleVersion.ControlFirmwareBootLoader != consoleVersion.ControlFirmwareBootLoader && consoleVersion.ControlFirmwareBootLoader != "0")
                {
                    currentConsoleVersion.ControlFirmwareBootLoader = consoleVersion.ControlFirmwareBootLoader;
                    isAnyConsoleStateChanged = true;
                }
                if (currentConsoleVersion.CPLDFirmware != consoleVersion.CPLDFirmware && consoleVersion.CPLDFirmware != "0")
                {
                    currentConsoleVersion.CPLDFirmware = consoleVersion.CPLDFirmware;
                    isAnyConsoleStateChanged = true;
                }
                if (currentConsoleVersion.PatientFirmware != consoleVersion.PatientFirmware && consoleVersion.PatientFirmware != "0")
                {
                    currentConsoleVersion.PatientFirmware = consoleVersion.PatientFirmware;
                    isAnyConsoleStateChanged = true;
                }
                if (currentConsoleVersion.PatientFirmwareBootLoader != consoleVersion.PatientFirmwareBootLoader && consoleVersion.PatientFirmwareBootLoader != "0")
                {
                    currentConsoleVersion.PatientFirmwareBootLoader = consoleVersion.PatientFirmwareBootLoader;
                    isAnyConsoleStateChanged = true;
                }
                if (currentConsoleVersion.RepeaterFirmware != consoleVersion.RepeaterFirmware && consoleVersion.RepeaterFirmware != "0")
                {
                    currentConsoleVersion.RepeaterFirmware = consoleVersion.RepeaterFirmware;
                    isAnyConsoleStateChanged = true;
                }
                if (currentConsoleVersion.RepeaterFirmwareBootLoader != consoleVersion.RepeaterFirmwareBootLoader && consoleVersion.RepeaterFirmwareBootLoader != "0")
                {
                    currentConsoleVersion.RepeaterFirmwareBootLoader = consoleVersion.RepeaterFirmwareBootLoader;
                    isAnyConsoleStateChanged = true;
                }
                if (currentConsoleVersion.ICBFirmware != consoleVersion.ICBFirmware && consoleVersion.ICBFirmware != "0")
                {
                    currentConsoleVersion.ICBFirmware = consoleVersion.ICBFirmware;
                    isAnyConsoleStateChanged = true;
                }
                if (currentConsoleVersion.ICBFirmwareBootLoader != consoleVersion.ICBFirmwareBootLoader && consoleVersion.ICBFirmwareBootLoader != "0")
                {
                    currentConsoleVersion.ICBFirmwareBootLoader = consoleVersion.ICBFirmwareBootLoader;
                    isAnyConsoleStateChanged = true;
                }
                if (currentConsoleVersion.CatheterFirmware != consoleVersion.CatheterFirmware && consoleVersion.CatheterFirmware != "0")
                {
                    currentConsoleVersion.CatheterFirmware = consoleVersion.CatheterFirmware;
                    isAnyConsoleStateChanged = true;
                }
                if (currentConsoleVersion.RemoteFirmware != consoleVersion.RemoteFirmware && consoleVersion.RemoteFirmware != "0")
                {
                    currentConsoleVersion.RemoteFirmware = consoleVersion.RemoteFirmware;
                    isAnyConsoleStateChanged = true;
                }

                if (isAnyConsoleStateChanged)
                {
                    currentConsoleVersion.StartDate = DateTime.Now;
                    this.Data.DataAccess.AddVersion(currentConsoleVersion);
                }
            }
        }


    }
}
