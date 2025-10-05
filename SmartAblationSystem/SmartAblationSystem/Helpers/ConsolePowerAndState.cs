using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAblationSystem.Helpers
{
    /// <summary>
    /// This class handles console power and state
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public static class ConsolePowerAndState
    {
        static private bool isConsoleStarted = false;

        static private int numberOfLogingToTherapy = 0;

        static private double voltageValue = 0;

        static private ConsoleVersion consoleVersionReference = new ConsoleVersion();

        static int catheterID = 0;

        static int systemStatesID = 0;

        static int userID = 0;

        static bool isUsingICB = false;

        static bool isUsingRemote = false;



        /// <summary>
        /// Gets/sets the value of IsConsoleStarted
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static bool IsConsoleStarted
        {
            get => isConsoleStarted;
            set => isConsoleStarted = value;
        }

        /// <summary>
        /// Gets/sets the value of NumberOfLogingToTherapy
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static int NumberOfLogingToTherapy
        {
            get => numberOfLogingToTherapy;
            set => numberOfLogingToTherapy = value;
        }

        /// <summary>
        /// Gets/sets the value of Voltage
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static double VoltageValue
        {
            get => voltageValue;
            set => voltageValue = value;
        }

        /// <summary>
        /// Gets/sets the value of ConsoleVersionReference
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static ConsoleVersion ConsoleVersionReference
        {
            get => consoleVersionReference;
            set => consoleVersionReference = value;
        }
        public static int CatheterID
        {
            get => catheterID;
            set => catheterID = value;
        }
        public static int SystemStatesID
        {
            get => systemStatesID;
            set => systemStatesID = value;
        }
        public static int UserID
        {
            get => userID;
            set => userID = value;
        }
        public static bool IsUsingICB
        {
            get => isUsingICB;
            set => isUsingICB = value;
        }
        public static bool IsUsingRemote
        {
            get => isUsingRemote;
            set => isUsingRemote = value;
        }
    }
}
