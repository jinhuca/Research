using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAblationSystem.Helpers
{
    /// <summary>
    /// This class for fetch system drives info
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public static class DrivesInformation
    {
        private const long OneKb = 1024;
        private const long OneMb = OneKb * 1024;
        private const long OneGb = OneMb * 1024;
        private const long OneTb = OneGb * 1024;

        private static long warningThreshold = 500;
        private static long failureThreshold = 100;
        private static long hardDriveTotalSpace = 0;


        /// <summary>
        /// Gets/sets the free sapce in MB
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static long FreeSapceInMB
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets warning threshold value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static long WarningThreshold
        {
            get
            {
                return warningThreshold;
            }

            set
            {
                warningThreshold = value;
                
            }
        }

        /// <summary>
        /// Gets/sets failure threshold value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static long FailureThreshold
        {
            get
            {
               return failureThreshold;
            }
            set
            {
                failureThreshold = value;
            }
        }

        /// <summary>
        /// Gets/sets hard drive total space value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static long HardDriveTotalSpace
        {
            get => hardDriveTotalSpace;
            set => hardDriveTotalSpace = value;
        }

        public static HealthStatus HardDiskHealthStatus = HealthStatus.Unknown;

        /// <summary>
        /// Gets total free space
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <id>SF-SDS-0100</id>
        public static double GetTotalFreeSpace()
        {
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady)
                {
                    HardDriveTotalSpace = (long)Math.Round((double)drive.TotalSize / OneGb, 0);

                    FreeSapceInMB = (long)Math.Round((double)drive.TotalFreeSpace / OneMb, 0);

                    if (FreeSapceInMB < FailureThreshold)
                    {
                        HardDiskHealthStatus = HealthStatus.Fail;
                    }
                    else if (FreeSapceInMB < WarningThreshold)
                    {
                        HardDiskHealthStatus = HealthStatus.Warning;
                    }
                    else if (FreeSapceInMB > WarningThreshold)
                    {
                        HardDiskHealthStatus = HealthStatus.Pass;
                    }
                    return FreeSapceInMB;
                }
            }
            return 0;
        }
        /// <summary>
        /// Define health status value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public enum HealthStatus
        {
            Unknown = 0,
            Pass = 1,
            Warning = 2,
            Fail = 3,
        }
    }
}
