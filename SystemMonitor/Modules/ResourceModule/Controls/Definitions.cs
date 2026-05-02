using System;
using System.Collections.Generic;
using System.Text;

namespace ResourceModule.Controls;

public enum Unit {
  None,
  Percent,
  Absolute
}

public static class Definitions {
  public const string PercentageString = "%";
  public const string AbsoluteString = "";
  public const string NoneString = "";
}