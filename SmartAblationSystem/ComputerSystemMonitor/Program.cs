using System;

namespace ComputerSystemMonitor
{
  internal class Program
  {
    internal static SystemMonitor monitor = new SystemMonitor();
    public static void Main()
    {
      ConsoleWindowSettings.SetEditMode(false);
      ConsoleWindowSettings.SetWindowPosition(0,0);
      ConsoleWindowSettings.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
      SystemMonitor.Start();
    }
  }
}
