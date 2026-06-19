using CrystalMonitor.Hardware;

namespace CrystalMonitorTests.HardwareTests;

/// <summary>
/// Unit tests for the ControlMode enum.
/// Tests enumeration, values, parsing, and switch logic.
/// </summary>
public class ControlModeTests {
  // =========================================================================
  // Ordinal Values
  // =========================================================================

  [Theory]
  [InlineData(ControlMode.Undefined, 0)]
  [InlineData(ControlMode.Software, 1)]
  [InlineData(ControlMode.Default, 2)]
  public void ControlMode_OrdinalValue_IsCorrect(ControlMode mode, int expected) {
    Assert.Equal(expected, (int)mode);
  }

  // =========================================================================
  // Member Count
  // =========================================================================

  [Fact]
  public void ControlMode_HasExactly3Members() {
    Assert.Equal(3, Enum.GetValues<ControlMode>().Length);
  }

  [Fact]
  public void ControlMode_HasUndefined_Software_Default() {
    var values = Enum.GetValues<ControlMode>();
    Assert.Contains(ControlMode.Undefined, values);
    Assert.Contains(ControlMode.Software, values);
    Assert.Contains(ControlMode.Default, values);
  }

  // =========================================================================
  // IsDefined
  // =========================================================================

  [Theory]
  [InlineData(ControlMode.Undefined)]
  [InlineData(ControlMode.Software)]
  [InlineData(ControlMode.Default)]
  public void ControlMode_AllMembers_AreDefined(ControlMode mode) {
    Assert.True(Enum.IsDefined(mode));
  }

  [Fact]
  public void ControlMode_UndefinedValue_IsNotDefined() {
    Assert.False(Enum.IsDefined((ControlMode)999));
  }

  [Fact]
  public void ControlMode_NegativeValue_IsNotDefined() {
    Assert.False(Enum.IsDefined((ControlMode)(-1)));
  }

  // =========================================================================
  // Cast from Int
  // =========================================================================

  [Theory]
  [InlineData(0, ControlMode.Undefined)]
  [InlineData(1, ControlMode.Software)]
  [InlineData(2, ControlMode.Default)]
  public void ControlMode_CastFromInt_ReturnsCorrectMember(int value, ControlMode expected) {
    Assert.Equal(expected, (ControlMode)value);
  }

  [Fact]
  public void ControlMode_CastFromUndefinedInt_DoesNotThrow() {
    var ex = Record.Exception(() => _ = (ControlMode)999);
    Assert.Null(ex);
  }

  // =========================================================================
  // Parsing
  // =========================================================================

  [Theory]
  [InlineData("Undefined", ControlMode.Undefined)]
  [InlineData("Software", ControlMode.Software)]
  [InlineData("Default", ControlMode.Default)]
  public void ControlMode_Parse_ReturnsCorrectMember(string name, ControlMode expected) {
    Assert.Equal(expected, Enum.Parse<ControlMode>(name));
  }

  [Theory]
  [InlineData("undefined", ControlMode.Undefined)]
  [InlineData("software", ControlMode.Software)]
  [InlineData("default", ControlMode.Default)]
  public void ControlMode_Parse_CaseInsensitive_ReturnsCorrectMember(
    string name, ControlMode expected) {
    Assert.Equal(expected, Enum.Parse<ControlMode>(name, ignoreCase: true));
  }

  [Fact]
  public void ControlMode_Parse_InvalidName_Throws() {
    Assert.Throws<ArgumentException>(() => Enum.Parse<ControlMode>("InvalidMode"));
  }

  [Fact]
  public void ControlMode_Parse_CaseSensitive_FailsForWrongCase() {
    Assert.Throws<ArgumentException>(() => 
      Enum.Parse<ControlMode>("software", ignoreCase: false));
  }

  // =========================================================================
  // ToString
  // =========================================================================

  [Theory]
  [InlineData(ControlMode.Undefined, "Undefined")]
  [InlineData(ControlMode.Software, "Software")]
  [InlineData(ControlMode.Default, "Default")]
  public void ControlMode_ToString_ReturnsCorrectName(ControlMode mode, string expected) {
    Assert.Equal(expected, mode.ToString());
  }

  [Fact]
  public void ControlMode_ToString_Consistent() {
    var mode = ControlMode.Software;
    var str1 = mode.ToString();
    var str2 = mode.ToString();
    var str3 = mode.ToString();

    Assert.Equal(str1, str2);
    Assert.Equal(str2, str3);
  }

  // =========================================================================
  // Uniqueness
  // =========================================================================

  [Fact]
  public void ControlMode_AllMembers_HaveUniqueValues() {
    var values = Enum.GetValues<ControlMode>().Select(v => (int)v).ToList();
    Assert.Equal(values.Count, values.Distinct().Count());
  }

  // =========================================================================
  // Equality
  // =========================================================================

  [Fact]
  public void ControlMode_SameValue_IsEqual() {
    ControlMode a = ControlMode.Software;
    ControlMode b = ControlMode.Software;
    Assert.Equal(a, b);
  }

  [Fact]
  public void ControlMode_DifferentValues_AreNotEqual() {
    Assert.NotEqual(ControlMode.Software, ControlMode.Default);
    Assert.NotEqual(ControlMode.Undefined, ControlMode.Software);
  }

  // =========================================================================
  // Comparison Operators
  // =========================================================================

  [Fact]
  public void ControlMode_Comparison_LessThan() {
    Assert.True(ControlMode.Undefined < ControlMode.Software);
    Assert.True(ControlMode.Software < ControlMode.Default);
    Assert.False(ControlMode.Default < ControlMode.Undefined);
  }

  [Fact]
  public void ControlMode_Comparison_GreaterThan() {
    Assert.True(ControlMode.Default > ControlMode.Software);
    Assert.True(ControlMode.Software > ControlMode.Undefined);
    Assert.False(ControlMode.Undefined > ControlMode.Default);
  }

  [Fact]
  public void ControlMode_Comparison_LessThanOrEqual() {
    Assert.True(ControlMode.Undefined <= ControlMode.Undefined);
    Assert.True(ControlMode.Undefined <= ControlMode.Software);
    Assert.False(ControlMode.Default <= ControlMode.Undefined);
  }

  [Fact]
  public void ControlMode_Comparison_GreaterThanOrEqual() {
    Assert.True(ControlMode.Default >= ControlMode.Default);
    Assert.True(ControlMode.Default >= ControlMode.Software);
    Assert.False(ControlMode.Undefined >= ControlMode.Default);
  }

  // =========================================================================
  // Iteration and Enumeration
  // =========================================================================

  [Fact]
  public void ControlMode_GetValues_ReturnsAllMembers() {
    var values = Enum.GetValues<ControlMode>();
    Assert.Equal(3, values.Length);
    Assert.Contains(ControlMode.Undefined, values);
    Assert.Contains(ControlMode.Software, values);
    Assert.Contains(ControlMode.Default, values);
  }

  [Fact]
  public void ControlMode_GetNames_ReturnsCorrectCount() {
    var names = Enum.GetNames<ControlMode>();
    Assert.Equal(3, names.Length);
    Assert.Contains("Undefined", names);
    Assert.Contains("Software", names);
    Assert.Contains("Default", names);
  }

  [Fact]
  public void ControlMode_CanEnumerateAllValues() {
    var allModes = Enum.GetValues<ControlMode>().ToList();
    var stringRepresentations = allModes.Select(m => m.ToString()).ToList();

    Assert.Equal(3, stringRepresentations.Count);
    Assert.All(stringRepresentations, s => Assert.NotEmpty(s));
  }

  // =========================================================================
  // Switch Pattern Matching
  // =========================================================================

  [Theory]
  [InlineData(ControlMode.Undefined, "uninitialized")]
  [InlineData(ControlMode.Software, "controlled")]
  [InlineData(ControlMode.Default, "automatic")]
  public void ControlMode_SwitchPattern_ReturnsExpectedCategory(
    ControlMode mode, string expectedCategory) {
    string category = mode switch {
      ControlMode.Undefined => "uninitialized",
      ControlMode.Software => "controlled",
      ControlMode.Default => "automatic",
      _ => throw new ArgumentOutOfRangeException(nameof(mode))
    };

    Assert.Equal(expectedCategory, category);
  }

  [Fact]
  public void ControlMode_SwitchPattern_HasNoCatchAllNeeded() {
    // Verify all values are handled
    foreach (var mode in Enum.GetValues<ControlMode>()) {
      var handled = mode switch {
        ControlMode.Undefined => true,
        ControlMode.Software => true,
        ControlMode.Default => true
      };

      Assert.True(handled);
    }
  }

  // =========================================================================
  // Specific Semantics
  // =========================================================================

  [Fact]
  public void ControlMode_Undefined_IsZero() {
    Assert.Equal(0, (int)ControlMode.Undefined);
  }

  [Fact]
  public void ControlMode_Software_IsOne() {
    Assert.Equal(1, (int)ControlMode.Software);
  }

  [Fact]
  public void ControlMode_Default_IsTwo() {
    Assert.Equal(2, (int)ControlMode.Default);
  }

  [Fact]
  public void ControlMode_Undefined_IsDefaultInstance() {
    var defaultMode = default(ControlMode);
    Assert.Equal(ControlMode.Undefined, defaultMode);
  }

  // =========================================================================
  // Sorting and Ordering
  // =========================================================================

  [Fact]
  public void ControlMode_CanBeSorted() {
    var modes = new[] {
      ControlMode.Default,
      ControlMode.Undefined,
      ControlMode.Software
    };

    var sorted = modes.OrderBy(m => m).ToArray();

    Assert.Equal(ControlMode.Undefined, sorted[0]);
    Assert.Equal(ControlMode.Software, sorted[1]);
    Assert.Equal(ControlMode.Default, sorted[2]);
  }

  [Fact]
  public void ControlMode_CanBeSortedDescending() {
    var modes = new[] {
      ControlMode.Undefined,
      ControlMode.Default,
      ControlMode.Software
    };

    var sorted = modes.OrderByDescending(m => m).ToArray();

    Assert.Equal(ControlMode.Default, sorted[0]);
    Assert.Equal(ControlMode.Software, sorted[1]);
    Assert.Equal(ControlMode.Undefined, sorted[2]);
  }

  // =========================================================================
  // Boundary Cases
  // =========================================================================

  [Fact]
  public void ControlMode_Undefined_IsMinValue() {
    var minValue = Enum.GetValues<ControlMode>().Min();
    Assert.Equal(ControlMode.Undefined, minValue);
  }

  [Fact]
  public void ControlMode_Default_IsMaxValue() {
    var maxValue = Enum.GetValues<ControlMode>().Max();
    Assert.Equal(ControlMode.Default, maxValue);
  }

  [Fact]
  public void ControlMode_ValueBeyondRange_IsNotDefined() {
    Assert.False(Enum.IsDefined((ControlMode)100));
  }

  // =========================================================================
  // Collection Behavior
  // =========================================================================

  [Fact]
  public void ControlMode_CanBeAddedToList() {
    var modes = new List<ControlMode> {
      ControlMode.Undefined,
      ControlMode.Software,
      ControlMode.Default
    };

    Assert.Equal(3, modes.Count);
  }

  [Fact]
  public void ControlMode_CanBeUsedInHashSet() {
    var set = new HashSet<ControlMode> {
      ControlMode.Software,
      ControlMode.Software,
      ControlMode.Default
    };

    // Duplicates eliminated
    Assert.Equal(2, set.Count);
  }

  [Fact]
  public void ControlMode_CanBeUsedAsDictionaryKey() {
    var dict = new Dictionary<ControlMode, string> {
      { ControlMode.Undefined, "Not Set" },
      { ControlMode.Software, "Manual" },
      { ControlMode.Default, "Automatic" }
    };

    Assert.Equal("Manual", dict[ControlMode.Software]);
  }

  [Fact]
  public void ControlMode_CanBeQueriedWithLinq() {
    var modes = Enum.GetValues<ControlMode>();
    var softwareModes = modes.Where(m => m == ControlMode.Software).ToArray();

    Assert.Single(softwareModes);
    Assert.Contains(ControlMode.Software, softwareModes);
  }

  // =========================================================================
  // Practical Control Scenarios
  // =========================================================================

  [Fact]
  public void ControlMode_DefaultInstanceUninitialized() {
    var mode = default(ControlMode);
    Assert.Equal(ControlMode.Undefined, mode);
  }

  [Fact]
  public void ControlMode_CanToggleBetweenSoftwareAndDefault() {
    var current = ControlMode.Software;
    var next = current == ControlMode.Software ? ControlMode.Default : ControlMode.Software;

    Assert.Equal(ControlMode.Default, next);
  }

  [Fact]
  public void ControlMode_TransitionFromUndefined() {
    var state = ControlMode.Undefined;

    state = ControlMode.Software;
    Assert.Equal(ControlMode.Software, state);

    state = ControlMode.Default;
    Assert.Equal(ControlMode.Default, state);
  }

  [Fact]
  public void ControlMode_InitiallyUndefinedThenConfigured() {
    var fanMode = ControlMode.Undefined;

    Assert.Equal(ControlMode.Undefined, fanMode);

    fanMode = ControlMode.Software;

    Assert.NotEqual(ControlMode.Undefined, fanMode);
    Assert.Equal(ControlMode.Software, fanMode);
  }

  // =========================================================================
  // String Representation Scenarios
  // =========================================================================

  [Fact]
  public void ControlMode_ToString_UsefulForLogging() {
    var mode = ControlMode.Software;
    var logMessage = $"Control mode is: {mode}";

    Assert.Contains("Software", logMessage);
  }

  [Fact]
  public void ControlMode_Parse_ReversesOfToString() {
    foreach (var originalMode in Enum.GetValues<ControlMode>()) {
      var stringForm = originalMode.ToString();
      var parsedMode = Enum.Parse<ControlMode>(stringForm);

      Assert.Equal(originalMode, parsedMode);
    }
  }
}
