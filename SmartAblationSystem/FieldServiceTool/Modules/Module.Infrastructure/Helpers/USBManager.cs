using System;
using Prism.Mvvm;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;

namespace Module.Infrastructure.Helpers
{
	public class USBManager : BindableBase, IDisposable
	{
		private ManagementEventWatcher managementEventWatcher;

		private List<DriveInfo> _driveInfos;
		public List<DriveInfo> DriveInfos
		{
			get => _driveInfos;
			set => SetProperty(ref _driveInfos, value);
		}
		
		public USBManager(EventArrivedEventHandler eventHandler)
		{
			DriveInfos = new List<DriveInfo>();
			ReloadDriveInfos();
			RegisterManagementEventWatching();
			managementEventWatcher.EventArrived += eventHandler;
		}

		public List<DriveInfo> GetUSBDriveList()
		{
			ReloadDriveInfos();
			return DriveInfos;
		}

		public void Dispose() => managementEventWatcher?.Stop();

		private void RegisterManagementEventWatching()
		{
			managementEventWatcher = new ManagementEventWatcher();
			var query = new WqlEventQuery("SELECT * FROM Win32_VolumeChangeEvent");
			managementEventWatcher.Query = query;
			managementEventWatcher.Start();
		}

		private void ReloadDriveInfos()
		{
			var usbDrives = GetAllRemovableDrives();
			DriveInfos?.Clear();
			if (usbDrives == null) return;
			foreach (var usbDrive in usbDrives)
			{
				DriveInfos?.Add(usbDrive);
			}
		}

		private static IEnumerable<DriveInfo> GetAllRemovableDrives()
		{
			var driveInfos = DriveInfo.GetDrives().AsEnumerable();
			driveInfos = driveInfos.Where(drive => drive.DriveType == DriveType.Removable && drive.IsReady == true && drive.VolumeLabel != "DWA-171");
			return driveInfos;
		}
	}
}
