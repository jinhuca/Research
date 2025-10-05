using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;

namespace USBDriveConnectionManager
{
    /// <summary>
    /// This class manages USB drive connection and disconnection.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class USBDriveConnectionManager
    {
        /// <summary>
        /// This property gets/sets DriveInfos value.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public List<DriveInfo> DriveInfos { get; set; }

        private ManagementEventWatcher managementEventWatcher;

        /// <summary>
        /// Default class constructor.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public USBDriveConnectionManager()
        {
        }

        /// <summary>
        /// Constructor that receives an event handler as paramater allowing the caller to be notified
        /// when the event is invoked.
        /// Loads USB drive informations to get a list of currently connected drives.
        /// Registers and starts a Windows event watcher to receive notification when a volume changed event arrives.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="eventHandler">Event handler that will be notified by the management event watcher when an
        /// event arrived.</param>
        public USBDriveConnectionManager(EventArrivedEventHandler eventHandler)
        {
            //USB drive detector
            DriveInfos = new List<DriveInfo>();
            ReloadDriveInfos();
            RegisterManagementEventWatching();
            managementEventWatcher.EventArrived += eventHandler;
        }

        /// <summary>
        /// Function that refreshes the USB Drive connected to the system and returns it to the caller.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <returns>List of DriveInfo.</returns>
        public List<DriveInfo> GetUSBDriveList()
        {
            ReloadDriveInfos();
            return DriveInfos;
        }

        /// <summary>
        /// Stops the system's manager event watcher.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public void Dispose()
        {
            if (managementEventWatcher != null)
            {
                managementEventWatcher.Stop();
            }
        }

        /// <summary>
        /// Sets up and starts the Management Event Watcher that listens to any Windows volume change event.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void RegisterManagementEventWatching()
        {
            managementEventWatcher = new ManagementEventWatcher();
            var query = new WqlEventQuery("SELECT * FROM Win32_VolumeChangeEvent");
            managementEventWatcher.Query = query;
            managementEventWatcher.Start();
        }

        /// <summary>
        /// Refreshes the list of connected USB Drive.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void ReloadDriveInfos()
        {
            var usbDrives = GetAllRemovableDrives();

            DriveInfos.Clear();

            foreach (var usbDrive in usbDrives)
            {
                DriveInfos.Add(usbDrive);
            }
        }

        /// <summary>
        /// Function that gathers and returns all the "removable" drives connected to the system.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <returns>List of DriveInfo object.</returns>
        private static IEnumerable<DriveInfo> GetAllRemovableDrives()
        {
            var driveInfos = DriveInfo.GetDrives().AsEnumerable();
            driveInfos = driveInfos.Where(drive => drive.DriveType == DriveType.Removable && drive.IsReady==true && drive.VolumeLabel !="DWA-171");
            return driveInfos;
        }
    }
}