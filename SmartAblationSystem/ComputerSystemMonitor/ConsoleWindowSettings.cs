using System;
using System.Runtime.InteropServices;

namespace ComputerSystemMonitor
{
  public static class ConsoleWindowSettings
  {
    public static void SetEditMode(bool Enable)
    {
      IntPtr consoleHandle = NativeFunctions.GetStdHandle((int)NativeFunctions.StdHandle.STD_INPUT_HANDLE);
      uint consoleMode;

      NativeFunctions.GetConsoleMode(consoleHandle, out consoleMode);
      if (Enable)
      {
        consoleMode |= (uint)ConsoleMode.ENABLE_QUICK_EDIT_MODE;
      }
      else
      {
        consoleMode &= ~(uint)ConsoleMode.ENABLE_QUICK_EDIT_MODE;
      }

      consoleMode |= (uint)ConsoleMode.ENABLE_EXTENDED_FLAGS;
      NativeFunctions.SetConsoleMode(consoleHandle, consoleMode);
    }

    public static void SetWindowSize(int width, int height)
    {
      Console.SetWindowSize(width, height);
      IntPtr sysMenu = NativeFunctions.GetSystemMenu(NativeFunctions.ConsoleWindow, false);

      if (NativeFunctions.ConsoleWindow != IntPtr.Zero)
      {
        NativeFunctions.DeleteMenu(sysMenu, NativeFunctions.SC_MINIMIZE, NativeFunctions.MF_BYCOMMAND);
        NativeFunctions.DeleteMenu(sysMenu, NativeFunctions.SC_MAXIMIZE, NativeFunctions.MF_BYCOMMAND);
        NativeFunctions.DeleteMenu(sysMenu, NativeFunctions.SC_SIZE, NativeFunctions.MF_BYCOMMAND);
      }
    }

    public static void SetWindowPosition(int xPosition, int yPosition)
    {
      NativeFunctions.SetWindowPos(NativeFunctions.ConsoleWindow, 0, xPosition, yPosition, 0, 0, NativeFunctions.SWP_NOSIZE);
    }

    private enum ConsoleMode : uint
    {
      ENABLE_ECHO_INPUT = 0x0004,
      ENABLE_EXTENDED_FLAGS = 0x0080,
      ENABLE_INSERT_MODE = 0x0020,
      ENABLE_LINE_INPUT = 0x0002,
      ENABLE_MOUSE_INPUT = 0x0010,
      ENABLE_PROCESSED_INPUT = 0x0001,
      ENABLE_QUICK_EDIT_MODE = 0x0040,
      ENABLE_WINDOW_INPUT = 0x0008,
      ENABLE_VIRTUAL_TERMINAL_INPUT = 0x0200,

      ENABLE_PROCESSED_OUTPUT = 0x0001,
      ENABLE_WRAP_AT_EOL_OUTPUT = 0x0002,
      ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004,
      DISABLE_NEWLINE_AUTO_RETURN = 0x0008,
      ENABLE_LVB_GRID_WORLDWIDE = 0x0010
    }

    private static class NativeFunctions
    {
      public const int SWP_NOSIZE = 0x0001;
      public const int MF_BYCOMMAND = 0x00000000;
      public const int SC_CLOSE = 0xF060;
      public const int SC_MINIMIZE = 0xF020;
      public const int SC_MAXIMIZE = 0xF030;
      public const int SC_SIZE = 0xF000;

      public enum StdHandle : int
      {
        STD_INPUT_HANDLE = -10,
        STD_OUTPUT_HANDLE = -11,
        STD_ERROR_HANDLE = -12,
      }

      [DllImport("kernel32.dll", SetLastError = true)]
      public static extern IntPtr GetStdHandle(int nStdHandle);

      [DllImport("kernel32.dll", SetLastError = true)]
      public static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

      [DllImport("kernel32.dll", SetLastError = true)]
      public static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

      [DllImport("kernel32.dll", ExactSpelling = true)]
      public static extern IntPtr GetConsoleWindow();

      [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
      public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

      [DllImport("user32.dll")]
      public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

      [DllImport("user32.dll")]
      public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

      public static IntPtr ConsoleWindow = GetConsoleWindow();
    }
  }
}
