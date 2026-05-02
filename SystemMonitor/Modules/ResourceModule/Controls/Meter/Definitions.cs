using System;
using System.Collections.Generic;
using System.Text;

namespace ResourceModule.Controls.Meter;

public enum Unit {
  None,
  Percent,
  Absolute,
  Ghz
}

public static class Definitions {
  public const string PercentageString = "%";
  public const string GhzString = "GHz";
  public const string AbsoluteString = "";
  public const string NoneString = "";
}