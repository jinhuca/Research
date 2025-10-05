using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAblationSystem.Helpers
{
    public static class CPUTimeWatchdog
    {
        private static bool isTimerStarted = false;
        private static Stopwatch stopwatchVerificator = new Stopwatch();
        private static double totalMillisconds = 0;

        public static bool IsTimerStarted
        {
            get => isTimerStarted;
            set => isTimerStarted = value;
        }
        public static Stopwatch StopwatchVerificator
        {
            get => stopwatchVerificator;
            set => stopwatchVerificator = value;
        }
        public static double TotalMillisconds
        {
            get => totalMillisconds;
            set => totalMillisconds = value;
        }

        public static void StartTimeMonitoring()
        {
            TotalMillisconds = 0;
            StopwatchVerificator.Start();
        }

        public static void StopTimeMonitoring()
        {
            TotalMillisconds = 0;
            StopwatchVerificator.Stop();
        }
    }
}
