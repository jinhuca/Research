using System.Diagnostics;

namespace LogModule;

public static class TestDebug {
  public static void WriteOut(string msg) {
    Debug.WriteLine(msg);
  }
}
