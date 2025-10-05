using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SmartAblationSystem.Helpers
{

    /// <summary>
    /// This class handles the time Zone
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public static class TimeZoneManager
    {

        /// <summary>
        /// [Win32 API call]
        /// The GetTimeZoneInformation function retrieves the current time-zone parameters. 
        /// These parameters control the translations between Coordinated Universal Time (UTC) 
        /// and local time.
        /// </summary>
        /// <param name="lpTimeZoneInformation">[out] Pointer to a TIME_ZONE_INFORMATION structure to receive the current time-zone parameters.</param>
        /// <returns>
        /// If the function succeeds, the return value is one of the following values.
        /// <list type="table">
        /// <listheader>
        /// <term>Return code/value</term>
        /// <description>Description</description>
        /// </listheader>
        /// <item>
        /// <term>TIME_ZONE_ID_UNKNOWN == 0</term>
        /// <description>
        /// The system cannot determine the current time zone. This error is also returned if you call the SetTimeZoneInformation function and supply the bias values but no transition dates. 
        /// This value is returned if daylight saving time is not used in the current time zone, because there are no transition dates.
        /// </description>
        /// </item>
        /// <item>
        /// <term>TIME_ZONE_ID_STANDARD == 1</term>
        /// <description>
        /// The system is operating in the range covered by the StandardDate member of the TIME_ZONE_INFORMATION structure.
        /// </description>
        /// </item>
        /// <item>
        /// <term>TIME_ZONE_ID_DAYLIGHT == 2</term>
        /// <description>
        /// The system is operating in the range covered by the DaylightDate member of the TIME_ZONE_INFORMATION structure.
        /// </description>
        /// </item>
        /// </list>
        /// If the function fails, the return value is TIME_ZONE_ID_INVALID. To get extended error information, call GetLastError.
        /// </returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern int GetTimeZoneInformation(out TimeZoneInformation lpTimeZoneInformation);

        /// <summary>
        /// [Win32 API call]
        /// The SetTimeZoneInformation function sets the current time-zone parameters. 
        /// These parameters control translations from Coordinated Universal Time (UTC) 
        /// to local time.
        /// </summary>
        /// <param name="lpTimeZoneInformation">[in] Pointer to a TIME_ZONE_INFORMATION structure that contains the time-zone parameters to set.</param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero.
        /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
        /// </returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern bool SetTimeZoneInformation([In] ref TimeZoneInformation lpTimeZoneInformation);

        /// <summary>
        /// The SystemTime structure represents a date and time using individual members 
        /// for the month, day, year, weekday, hour, minute, second, and millisecond
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct SystemTime
        {
            public short year;
            public short month;
            public short dayOfWeek;
            public short day;
            public short hour;
            public short minute;
            public short second;
            public short milliseconds;
        }

        /// <summary>
        /// The TimeZoneInformation structure specifies information specific to the time zone.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct TimeZoneInformation
        {
            /// <summary>
            /// Current bias for local time translation on this computer, in minutes. The bias is the difference, in minutes, between Coordinated Universal Time (UTC) and local time. All translations between UTC and local time are based on the following formula: 
            /// <para>UTC = local time + bias</para>
            /// <para>This member is required.</para>
            /// </summary>
            public int bias;
            /// <summary>
            /// Pointer to a null-terminated string associated with standard time. For example, "EST" could indicate Eastern Standard Time. The string will be returned unchanged by the GetTimeZoneInformation function. This string can be empty
            /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string standardName;
            /// <summary>
            /// A SystemTime structure that contains a date and local time when the transition from daylight saving time to standard time occurs on this operating system. If the time zone does
            /// not support daylight saving time or if the caller needs to disable daylight saving time, the wMonth member in the SystemTime structure must be zero. If this date is specified, 
            /// the DaylightDate value in the TimeZoneInformation structure must also be specified. Otherwise, the system assumes the time zone data is invalid and no changes will be applied.
            /// <para>To select the correct day in the month, set the wYear member to zero, the wHour and wMinute members to the transition time, the wDayOfWeek member to the appropriate 
            /// weekday, and the wDay member to indicate the occurence of the day of the week within the month (first through fifth).</para>
            /// <para>Using this notation, specify the 2:00a.m. on the first Sunday in April as follows: wHour = 2, wMonth = 4, wDayOfWeek = 0, wDay = 1. Specify 2:00a.m. on the last Thursday in October as follows: wHour = 2, wMonth = 10, wDayOfWeek = 4, wDay = 5.</para>
            /// </summary>
            public SystemTime standardDate;
            /// <summary>
            /// Bias value to be used during local time translations that occur during standard time. This member is ignored if a value for the StandardDate member is not supplied. 
            /// <para>This value is added to the value of the Bias member to form the bias used during standard time. In most time zones, the value of this member is zero.</para>
            /// </summary>
            public int standardBias;
            /// <summary>
            /// Pointer to a null-terminated string associated with daylight saving time. For example, "PDT" could indicate Pacific Daylight Time. The string will be returned unchanged by the GetTimeZoneInformation function. This string can be empty.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string daylightName;
            /// <summary>
            /// A SystemTime structure that contains a date and local time when the transition from standard time to daylight saving time occurs on this operating system. If the time zone does not support daylight saving time or if the caller needs to disable daylight saving time, the wMonth member in the SystemTime structure must be zero. If this date is specified, the StandardDate value in the TimeZoneInformation structure must also be specified. Otherwise, the system assumes the time zone data is invalid and no changes will be applied.
            /// <para>To select the correct day in the month, set the wYear member to zero, the wHour and wMinute members to the transition time, the wDayOfWeek member to the appropriate weekday, and the wDay member to indicate the occurence of the day of the week within the month (first through fifth).</para>
            /// </summary>
            public SystemTime daylightDate;
            /// <summary>
            /// Bias value to be used during local time translations that occur during daylight saving time. This member is ignored if a value for the DaylightDate member is not supplied. 
            /// <para>This value is added to the value of the Bias member to form the bias used during daylight saving time. In most time zones, the value of this member is –60.</para>
            /// </summary>
            public int daylightBias;
        }


        /// <summary>
        /// Sets new time-zone information for the local system.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="tzi">Struct containing the time-zone parameters to set.</param>
        public static void SetTimeZone(TimeZoneInformation tzi)
        {
            bool rs = false;
            try
            {
                // set local system timezone
                rs = SetTimeZoneInformation(ref tzi);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }
        /// <summary>
        /// Sets time-zone information .
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static void SetTimeZone(System.TimeZoneInfo tzi)
        {
            var process = Process.Start(new ProcessStartInfo
            {
                FileName = "tzutil.exe",
                Arguments = "/s \"" + tzi.Id + "\"",
                UseShellExecute = false,
                CreateNoWindow = true
            });

            if (process != null)
            {
                process.WaitForExit();
                TimeZoneInfo.ClearCachedData();
            }


            // set local system timezone
            // SetTimeZoneInformation(ref tzi);
        }

        public static void LoadQuickAssist()
        {
            var process = Process.Start(new ProcessStartInfo
            {
                FileName = "quickassist.exe",
             
                UseShellExecute = false,
                CreateNoWindow = true
            });

            if (process != null)
            {
                process.WaitForExit();
            
            }


            // set local system timezone
            // SetTimeZoneInformation(ref tzi);
        }

        /// <summary>
        /// Gets current timezone information for the local system.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <returns>Struct containing the current time-zone parameters.</returns>
        public static TimeZoneInformation GetTimeZone()
        {
            // create struct instance
            TimeZoneInformation tzi;

            // retrieve timezone info
            int currentTimeZone = GetTimeZoneInformation(out tzi);

            return tzi;
        }
        /// <summary>
        /// Gets system time zones.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static ReadOnlyCollection<TimeZoneInfo> TimeZoneInfoCollection
        {
            get
            {
                return TimeZoneInfo.GetSystemTimeZones(); ;

            }
        }
        /// <summary>
        /// Gets local time zone info
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static TimeZoneInfo GetLocalTimeZone
        {
            get
            {
                return TimeZoneInfo.Local;
            }
        }

        /// <summary>
        /// Convert datatime to specific time zone
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static DateTime ConvertToSpecificTimeZone(this DateTime source, TimeZoneInfo timeZone)
        {
            
            var offset = timeZone.GetUtcOffset(source);
            var newDate = source + offset;
            return newDate;
        }
        

    }
    /// <summary>
    /// This class adjusts token privileges functionality
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public static class AdjustTokenPrivilegesFunctionality
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct LUID
        {
            public uint LowPart;
            public int HighPart;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct LUID_AND_ATTRIBUTES
        {
            public LUID Luid;
            public UInt32 Attributes;
        }

        private struct TOKEN_PRIVILEGES
        {
            public UInt32 PrivilegeCount;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
            public LUID_AND_ATTRIBUTES[] Privileges;
        }

        public const string SE_TIME_ZONE_NAME = "SeTimeZonePrivilege";
        public const int TOKEN_ADJUST_PRIVILEGES = 0x00000020;
        public const int TOKEN_QUERY = 0x00000008;
        public const int SE_PRIVILEGE_ENABLED = 0x00000002;

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AdjustTokenPrivileges(IntPtr TokenHandle, bool DisableAllPrivileges, ref TOKEN_PRIVILEGES NewState, UInt32 Zero, IntPtr Null1, IntPtr Null2);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int OpenProcessToken(int ProcessHandle, int DesiredAccess, out IntPtr tokenhandle);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern int GetCurrentProcess();

        [DllImport("advapi32.dll", CharSet = CharSet.Auto)]
        private static extern int LookupPrivilegeValue(string lpsystemname, string lpname, [MarshalAs(UnmanagedType.Struct)] ref LUID lpLuid);

        /// <summary>
        /// Enable set time zone privileges
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static void EnableSetTimeZonePrivileges()
        {
            // We must add the set timezone privilege to the process token or SetTimeZoneInformation will fail
            IntPtr token;
            int retval = OpenProcessToken(GetCurrentProcess(), TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, out token);
            //Assert.IsTrue(retval != 0, String.Format("OpenProcessToken failed. GetLastError: {0}", Marshal.GetLastWin32Error()));

            LUID luid = new LUID();
            retval = LookupPrivilegeValue(null, SE_TIME_ZONE_NAME, ref luid);
            //Assert.IsTrue(retval != 0, String.Format("LookupPrivilegeValue failed. GetLastError: {0}", Marshal.GetLastWin32Error()));

            TOKEN_PRIVILEGES tokenPrivs = new TOKEN_PRIVILEGES();
            tokenPrivs.PrivilegeCount = 1;
            tokenPrivs.Privileges = new LUID_AND_ATTRIBUTES[1];
            tokenPrivs.Privileges[0].Luid = luid;
            tokenPrivs.Privileges[0].Attributes = SE_PRIVILEGE_ENABLED;
            //Assert.IsTrue(AdjustTokenPrivileges(token, false, ref tokenPrivs, 0, IntPtr.Zero, IntPtr.Zero), String.Format("AdjustTokenPrivileges failed. GetLastError: {0}", Marshal.GetLastWin32Error()));
        }
    }
}
