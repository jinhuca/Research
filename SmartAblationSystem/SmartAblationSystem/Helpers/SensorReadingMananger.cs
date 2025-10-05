namespace SmartAblationSystem.Helpers
{
    /// <summary>
    /// This class handles the Sensor Reading Manager
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public static class SensorReadingMananger
    {
        private static bool areSensorsConnected = true;
        private static bool allowPlayback = false;
        private static bool isCatheterCableConnected = false;
        private static bool allowRemoteControl = false;

        /// <summary>
        /// Gets or sets a value indicating whether the sensors are connected or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static bool AreSensorsConnected
        {
            get
            {
                return areSensorsConnected;
            }
            set
            {
                if (value != areSensorsConnected)
                    areSensorsConnected = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Playback is allowed or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static bool AllowPlayback
        {
            get
            {
                return allowPlayback;
            }
            set
            {
                if (value != allowPlayback)
                    allowPlayback = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the catheter cable is connected
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static bool IsCatheterCableConnected
        {
            get
            {
                return isCatheterCableConnected;
            }
            set
            {
                isCatheterCableConnected = value;
            }
        }

        public static bool AllowRemoteControl
        {
            get => allowRemoteControl;
            set => allowRemoteControl = value;
        }

        /// <summary>
        /// Disconnects sensors
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static void DisconnectSensors()
        {
            if (AllowPlayback)
                AreSensorsConnected = false;
        }

        /// <summary>
        /// Connects sensors
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static void ConnectSensors()
        {
            AreSensorsConnected = true;
        }
    }
}