using Prism.Mvvm;

namespace Module.Console.Helpers
{
  /// <summary>
  /// This class handles the Sensor Reading Manager.
  /// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
  /// </summary>
  public static class SensorReadingManager
  {
    /// <summary>
    /// Gets or sets a value indicating whether the sensors are connected or not.
    /// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public static bool AreSensorsConnected { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the Playback is allowed or not.
    /// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public static bool AllowPlayback { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether the catheter cable is connected.
    /// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public static bool IsCatheterCableConnected { get; set; } = false;

    public static bool AllowRemoteControl { get; set; } = false;

    /// <summary>
    /// Disconnects sensors.
    /// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public static void DisconnectSensors()
    {
      if (AllowPlayback)
        AreSensorsConnected = false;
    }

    /// <summary>
    /// Connects sensors.
    /// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public static void ConnectSensors()
    {
      AreSensorsConnected = true;
    }
  }
}