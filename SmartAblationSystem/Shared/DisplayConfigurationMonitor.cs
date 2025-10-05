using System;
using System.Management;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Shared
{
  public class DisplayConfigurationMonitor : IDisplayConfigurationMonitor
  {
    // Enumeration for Display Configuration Flags
    [Flags]
    public enum SetDisplayConfigFlags : uint
    {
      SDC_TOPOLOGY_INTERNAL = 0x00000001,
      SDC_TOPOLOGY_CLONE = 0x00000002,
      SDC_TOPOLOGY_EXTEND = 0x00000004,
      SDC_TOPOLOGY_EXTERNAL = 0x00000008,
      SDC_APPLY = 0x00000080
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct DEVMODE
    {
      public const int CCHDEVICENAME = 32;
      public const int CCHFORMNAME = 32;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHDEVICENAME)]
      public string dmDeviceName;
      public short dmSpecVersion;
      public short dmDriverVersion;
      public short dmSize;
      public short dmDriverExtra;
      public int dmFields;
      public int dmPositionX;
      public int dmPositionY;
      public int dmDisplayOrientation;
      public int dmDisplayFixedOutput;
      public short dmColor;
      public short dmDuplex;
      public short dmYResolution;
      public short dmTTOption;
      public short dmCollate;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHFORMNAME)]
      public string dmFormName;
      public short dmLogPixels;
      public int dmBitsPerPel;
      public int dmPelsWidth;
      public int dmPelsHeight;
      public int dmDisplayFlags;
      public int dmDisplayFrequency;
      public int dmICMMethod;
      public int dmICMIntent;
      public int dmMediaType;
      public int dmDitherType;
      public int dmReserved1;
      public int dmReserved2;
      public int dmPanningWidth;
      public int dmPanningHeight;
    }


    public void DisplayMonitoringSubscription()
    {
      SystemEvents.DisplaySettingsChanged += new EventHandler(this.SystemEvents_DisplaySettingsChanged);
      // Check and set correct display mode upon bootup if screen is already connected
      this.SystemEvents_DisplaySettingsChanged(this, null);
    }

    // Import DLL function. This allows to change the display mode without Windows popup screen displaying.
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern long SetDisplayConfig(uint numPathArrayElements,
    IntPtr pathArray, uint numModeArrayElements, IntPtr modeArray, SetDisplayConfigFlags flags);
    [DllImport("user32.dll")]
    // A signature for ChangeDisplaySettingsEx with a DEVMODE struct as the second parameter won't allow you to pass in IntPtr.Zero, so create an overload
    public static extern int ChangeDisplaySettingsEx(string lpszDeviceName, IntPtr lpDevMode, IntPtr hwnd, uint dwflags, IntPtr lParam);

    [DllImport("user32.dll")]
    public static extern int ChangeDisplaySettingsEx(string lpszDeviceName, ref DEVMODE lpDevMode, IntPtr hwnd, uint dwflags, IntPtr lParam);
    [DllImport("user32.dll")]
    public static extern bool EnumDisplaySettings(string deviceName, int modeNum, ref DEVMODE devMode);
    [DllImport("user32.dll")]
    public static extern bool EnumDisplayDevices(string lpDevice, uint iDevNum, ref DISPLAY_DEVICE lpDisplayDevice, uint dwFlags);


    public const int CDS_NONE = 0;
    public const int CDS_UPDATEREGISTRY = 0x01;
    public const int CDS_SET_PRIMARY = 0x10;
    public const int CDS_RESET = 0x40000000;
    public const int CDS_NORESET = 0x10000000;
    public const int DISP_CHANGE_SUCCESSFUL = 0;
    [Flags()]
    public enum DisplayDeviceStateFlags : int
    {
      /// <summary>The device is part of the desktop.</summary>
      AttachedToDesktop = 0x1,
      MultiDriver = 0x2,
      /// <summary>The device is part of the desktop.</summary>
      PrimaryDevice = 0x4,
      /// <summary>Represents a pseudo device used to mirror application drawing for remoting or other purposes.</summary>
      MirroringDriver = 0x8,
      /// <summary>The device is VGA compatible.</summary>
      VGACompatible = 0x10,
      /// <summary>The device is removable; it cannot be the primary display.</summary>
      Removable = 0x20,
      /// <summary>The device has more display modes than its output devices support.</summary>
      ModesPruned = 0x8000000,
      Remote = 0x4000000,
      Disconnect = 0x2000000,
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct DISPLAY_DEVICE
    {
      [MarshalAs(UnmanagedType.U4)]
      public int cb;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
      public string DeviceName;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
      public string DeviceString;
      [MarshalAs(UnmanagedType.U4)]
      public DisplayDeviceStateFlags StateFlags;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
      public string DeviceID;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
      public string DeviceKey;
    }
    /// <summary>
    /// Sets the correct display mode when the display mode state changes, including external screen connection/disconnection.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="sender">The component that raised the event.</param>
    /// <param name="e">Represents the base class for classes that contain user control event data.</param>
    private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
    {
      try
      {

        // Check number of physical display devices connected
        if (this.GetConnectedScreensCount() > 1)
        {

          // Set the display mode to Clone if multiple screens
          SetDisplayConfig(0, IntPtr.Zero, 0, IntPtr.Zero, SetDisplayConfigFlags.SDC_TOPOLOGY_CLONE | SetDisplayConfigFlags.SDC_APPLY);
          SetAsPrimaryMonitor();
        }
        else
        {
          // Set the display mode to Extend if single screen
          SetDisplayConfig(0, IntPtr.Zero, 0, IntPtr.Zero, SetDisplayConfigFlags.SDC_TOPOLOGY_EXTEND | SetDisplayConfigFlags.SDC_APPLY);
        }

      }
      catch (Exception ex)
      {
        ex.ToString();
      }
    }

    /// <summary>
    /// Fetches the number of physical display devices connected, regardless of current Display mode.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    int GetConnectedScreensCount()
    {
      int connectedScreens = 0;

      try
      {
        ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE PNPClass = 'Monitor'");

        foreach (ManagementObject queryObj in searcher.Get())
        {
          connectedScreens++;
        }
      }
      catch (Exception ex)
      {
        ex.ToString();
      }

      return connectedScreens;
    }
    public static void SetAsPrimaryMonitor()
    {
      DISPLAY_DEVICE device = new DISPLAY_DEVICE();
      DEVMODE deviceMode = new DEVMODE();
      device.cb = Marshal.SizeOf(device);

      EnumDisplayDevices(null, 1, ref device, 0);
      int offsetx = deviceMode.dmPositionX;
      int offsety = deviceMode.dmPositionY;
      deviceMode.dmPositionX = 0;
      deviceMode.dmPositionY = 0;
      deviceMode.dmDisplayFrequency = 60;

      // set \\\\.\\DISPLAY1 as main (VGA)
      if (false != EnumDisplaySettings(device.DeviceName, -1, ref deviceMode)) {
        var result = ChangeDisplaySettingsEx(
            device.DeviceName,
            ref deviceMode,
            (IntPtr)null,
            (CDS_SET_PRIMARY | CDS_UPDATEREGISTRY | CDS_NORESET),
            IntPtr.Zero
        );
      }

      device = new DISPLAY_DEVICE();
      device.cb = Marshal.SizeOf(device);
      DEVMODE otherDeviceMode = new DEVMODE();

      EnumDisplayDevices(null, 0, ref device, 0);
      // set \\\\.\\DISPLAY2 (HDMI)
      otherDeviceMode.dmPositionX -= offsetx;
      otherDeviceMode.dmPositionY -= offsety;
      otherDeviceMode.dmDisplayFrequency = 60;
      if (false != EnumDisplaySettings(device.DeviceName, -1, ref otherDeviceMode))
      {
        var result2 = ChangeDisplaySettingsEx(
            device.DeviceName,
            ref otherDeviceMode,
            (IntPtr)null,
            (CDS_UPDATEREGISTRY | CDS_NORESET),
            IntPtr.Zero
        );
      }

      // Apply settings
      ChangeDisplaySettingsEx(null, IntPtr.Zero, (IntPtr)null, CDS_NONE, (IntPtr)null);
    }
    
  }
}
