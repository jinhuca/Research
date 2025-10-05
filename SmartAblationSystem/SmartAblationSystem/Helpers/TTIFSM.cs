using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAblationSystem.Helpers
{
    public static class TTIFSM
    {
        public static bool AreSensorsInPlayBackMode { get; set; }
        public static bool IsUserCancelingTTISettings { get; set; }

        public static bool IsInitializingTTISettings { get; set; }
        public static int StoredRequiredAblationTime { get; set; }

        public static bool IsFixedTimerSelected { get; set; }

        public static bool ISTTIFixedTimerSelected { get; set; }

        public static bool ISTTIDurationTimerSelected { get; set; }

        public static bool ISTTISelected { get; set; }

        public static int AblationTimer { get; set; }

        public static int DurationExpectedVeinIsolationTime { get; set; }

        public static int AblationTimerTTI { get; set; }

        public static int NewAblationTimerTTI { get; set; }

        public static int AblationTimerTTIFixed { get; set; }

        public static int NewAblationTimerTTIFixed { get; set; }

        public static Enumeration.AblationDurationType AblationDurationType { get; set; }

        public static int RequiredAblationTime { get; set; }

        public static bool IsUsingAutoPlayback { get; set; }


    }
}
