using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace LogModule;  
public static class TestDebug {
  public static void WriteOut(string msg) {
    Debug.WriteLine(msg);
  }
}
