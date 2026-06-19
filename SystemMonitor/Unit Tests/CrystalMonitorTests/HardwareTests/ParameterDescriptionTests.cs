using CrystalMonitor.Hardware;

namespace CrystalMonitorTests.HardwareTests;

/// <summary>
/// Unit tests for the ParameterDescription struct.
/// Tests construction, property access, equality, and edge cases.
/// </summary>
public class ParameterDescriptionTests {
  // =========================================================================
  // Construction
  // =========================================================================

  [Fact]
  public void ParameterDescription_Construction_WithValidValues_DoesNotThrow() {
    var ex = Record.Exception(() => 
      new ParameterDescription("Temperature", "CPU Temperature", 25.5f));
    Assert.Null(ex);
  }

  [Fact]
  public void ParameterDescription_Construction_WithEmptyName_DoesNotThrow() {
    var ex = Record.Exception(() => 
      new ParameterDescription("", "Description", 0f));
    Assert.Null(ex);
  }

  [Fact]
  public void ParameterDescription_Construction_WithNullName_DoesNotThrow() {
    var ex = Record.Exception(() => 
      new ParameterDescription(null, "Description", 0f));
    Assert.Null(ex);
  }

  [Fact]
  public void ParameterDescription_Construction_WithNullDescription_DoesNotThrow() {
    var ex = Record.Exception(() => 
      new ParameterDescription("Name", null, 0f));
    Assert.Null(ex);
  }

  [Theory]
  [InlineData(0f)]
  [InlineData(1f)]
  [InlineData(-1f)]
  [InlineData(100.5f)]
  [InlineData(float.MinValue)]
  [InlineData(float.MaxValue)]
  public void ParameterDescription_Construction_WithVariousDefaultValues_DoesNotThrow(float defaultValue) {
    var ex = Record.Exception(() => 
      new ParameterDescription("Name", "Desc", defaultValue));
    Assert.Null(ex);
  }

  // =========================================================================
  // Property Access
  // =========================================================================

  [Fact]
  public void ParameterDescription_Name_IsSetInConstructor() {
    var param = new ParameterDescription("TestName", "TestDesc", 10f);
    Assert.Equal("TestName", param.Name);
  }

  [Fact]
  public void ParameterDescription_Description_IsSetInConstructor() {
    var param = new ParameterDescription("TestName", "TestDesc", 10f);
    Assert.Equal("TestDesc", param.Description);
  }

  [Fact]
  public void ParameterDescription_DefaultValue_IsSetInConstructor() {
    var param = new ParameterDescription("TestName", "TestDesc", 42.5f);
    Assert.Equal(42.5f, param.DefaultValue);
  }

  [Fact]
  public void ParameterDescription_Properties_CanBeReadMultipleTimes() {
    var param = new ParameterDescription("Name", "Desc", 15f);

    var name1 = param.Name;
    var name2 = param.Name;
    var desc1 = param.Description;
    var desc2 = param.Description;
    var val1 = param.DefaultValue;
    var val2 = param.DefaultValue;

    Assert.Equal(name1, name2);
    Assert.Equal(desc1, desc2);
    Assert.Equal(val1, val2);
  }

  // =========================================================================
  // Equality
  // =========================================================================

  [Fact]
  public void ParameterDescription_Equality_SameValues_AreEqual() {
    var param1 = new ParameterDescription("Name", "Desc", 10f);
    var param2 = new ParameterDescription("Name", "Desc", 10f);

    Assert.Equal(param1, param2);
  }

  [Fact]
  public void ParameterDescription_Equality_DifferentNames_AreNotEqual() {
    var param1 = new ParameterDescription("Name1", "Desc", 10f);
    var param2 = new ParameterDescription("Name2", "Desc", 10f);

    Assert.NotEqual(param1, param2);
  }

  [Fact]
  public void ParameterDescription_Equality_DifferentDescriptions_AreNotEqual() {
    var param1 = new ParameterDescription("Name", "Desc1", 10f);
    var param2 = new ParameterDescription("Name", "Desc2", 10f);

    Assert.NotEqual(param1, param2);
  }

  [Fact]
  public void ParameterDescription_Equality_DifferentDefaultValues_AreNotEqual() {
    var param1 = new ParameterDescription("Name", "Desc", 10f);
    var param2 = new ParameterDescription("Name", "Desc", 20f);

    Assert.NotEqual(param1, param2);
  }

  [Fact]
  public void ParameterDescription_Equality_Reflexive() {
    var param = new ParameterDescription("Name", "Desc", 10f);

    Assert.Equal(param, param);
  }

  [Fact]
  public void ParameterDescription_Equality_Symmetric() {
    var param1 = new ParameterDescription("Name", "Desc", 10f);
    var param2 = new ParameterDescription("Name", "Desc", 10f);

    Assert.Equal(param1, param2);
    Assert.Equal(param2, param1);
  }

  [Fact]
  public void ParameterDescription_Equality_Transitive() {
    var param1 = new ParameterDescription("Name", "Desc", 10f);
    var param2 = new ParameterDescription("Name", "Desc", 10f);
    var param3 = new ParameterDescription("Name", "Desc", 10f);

    Assert.Equal(param1, param2);
    Assert.Equal(param2, param3);
    Assert.Equal(param1, param3);
  }

  // =========================================================================
  // Comparison via Equals (ParameterDescription struct does not override operators)
  // =========================================================================

  [Fact]
  public void ParameterDescription_CompareSameValues_ViaEquals() {
    var param1 = new ParameterDescription("Name", "Desc", 10f);
    var param2 = new ParameterDescription("Name", "Desc", 10f);

    Assert.True(param1.Equals(param2));
  }

  [Fact]
  public void ParameterDescription_CompareDifferentValues_ViaEquals() {
    var param1 = new ParameterDescription("Name1", "Desc", 10f);
    var param2 = new ParameterDescription("Name2", "Desc", 10f);

    Assert.False(param1.Equals(param2));
  }

  // =========================================================================
  // GetHashCode
  // =========================================================================

  [Fact]
  public void ParameterDescription_GetHashCode_EqualInstances_SameHash() {
    var param1 = new ParameterDescription("Name", "Desc", 10f);
    var param2 = new ParameterDescription("Name", "Desc", 10f);

    Assert.Equal(param1.GetHashCode(), param2.GetHashCode());
  }

  [Fact]
  public void ParameterDescription_GetHashCode_CanBeUsedInHashSet() {
    var param1 = new ParameterDescription("Name", "Desc", 10f);
    var param2 = new ParameterDescription("Name", "Desc", 10f);
    var param3 = new ParameterDescription("Name", "Desc", 20f);

    var set = new HashSet<ParameterDescription> { param1, param2, param3 };

    // param1 and param2 are equal, so set should have 2 elements
    Assert.Equal(2, set.Count);
  }

  [Fact]
  public void ParameterDescription_GetHashCode_CanBeUsedInDictionary() {
    var key1 = new ParameterDescription("Name", "Desc", 10f);
    var key2 = new ParameterDescription("Name", "Desc", 10f);

    var dict = new Dictionary<ParameterDescription, string> { { key1, "value1" } };

    Assert.True(dict.ContainsKey(key2));
    Assert.Equal("value1", dict[key2]);
  }

  // =========================================================================
  // ToString
  // =========================================================================

  [Fact]
  public void ParameterDescription_ToString_ReturnsNonEmptyString() {
    var param = new ParameterDescription("Name", "Desc", 10f);
    var str = param.ToString();

    Assert.NotEmpty(str);
  }

  [Fact]
  public void ParameterDescription_ToString_Consistent() {
    var param = new ParameterDescription("Name", "Desc", 10f);
    var str1 = param.ToString();
    var str2 = param.ToString();

    Assert.Equal(str1, str2);
  }

  // =========================================================================
  // Special Values
  // =========================================================================

  [Fact]
  public void ParameterDescription_ZeroDefaultValue() {
    var param = new ParameterDescription("Name", "Desc", 0f);
    Assert.Equal(0f, param.DefaultValue);
  }

  [Fact]
  public void ParameterDescription_NegativeDefaultValue() {
    var param = new ParameterDescription("Name", "Desc", -100f);
    Assert.Equal(-100f, param.DefaultValue);
  }

  [Fact]
  public void ParameterDescription_LargeDefaultValue() {
    var param = new ParameterDescription("Name", "Desc", 1_000_000.5f);
    Assert.Equal(1_000_000.5f, param.DefaultValue);
  }

  [Fact]
  public void ParameterDescription_VerySmallDefaultValue() {
    var param = new ParameterDescription("Name", "Desc", 0.0001f);
    Assert.Equal(0.0001f, param.DefaultValue);
  }

  // =========================================================================
  // String Edge Cases
  // =========================================================================

  [Fact]
  public void ParameterDescription_EmptyName_IsAllowed() {
    var param = new ParameterDescription("", "Description", 10f);
    Assert.Equal("", param.Name);
  }

  [Fact]
  public void ParameterDescription_EmptyDescription_IsAllowed() {
    var param = new ParameterDescription("Name", "", 10f);
    Assert.Equal("", param.Description);
  }

  [Fact]
  public void ParameterDescription_VeryLongName() {
    var longName = new string('a', 1000);
    var param = new ParameterDescription(longName, "Desc", 10f);
    Assert.Equal(longName, param.Name);
  }

  [Fact]
  public void ParameterDescription_VeryLongDescription() {
    var longDesc = new string('b', 1000);
    var param = new ParameterDescription("Name", longDesc, 10f);
    Assert.Equal(longDesc, param.Description);
  }

  [Fact]
  public void ParameterDescription_NameWithSpecialCharacters() {
    var param = new ParameterDescription("Name@#$%^&*()", "Desc", 10f);
    Assert.Equal("Name@#$%^&*()", param.Name);
  }

  [Fact]
  public void ParameterDescription_DescriptionWithSpecialCharacters() {
    var param = new ParameterDescription("Name", "Desc with\nnewline\ttab", 10f);
    Assert.Equal("Desc with\nnewline\ttab", param.Description);
  }

  [Fact]
  public void ParameterDescription_NameWithUnicodeCharacters() {
    var param = new ParameterDescription("温度", "Description", 10f);
    Assert.Equal("温度", param.Name);
  }

  // =========================================================================
  // Struct Semantics
  // =========================================================================

  [Fact]
  public void ParameterDescription_IsValueType() {
    Assert.True(typeof(ParameterDescription).IsValueType);
  }

  [Fact]
  public void ParameterDescription_CopySemantics() {
    var original = new ParameterDescription("Name", "Desc", 10f);
    var copy = original;

    // Modifying field through reflection would only affect copy
    // But since ParameterDescription is immutable, we verify equality
    Assert.Equal(original, copy);
  }

  [Fact]
  public void ParameterDescription_DefaultInstance_HasDefaultValues() {
    var param = default(ParameterDescription);

    Assert.Null(param.Name);
    Assert.Null(param.Description);
    Assert.Equal(0f, param.DefaultValue);
  }

  // =========================================================================
  // Collection Behavior
  // =========================================================================

  [Fact]
  public void ParameterDescription_CanBeAddedToList() {
    var params_list = new List<ParameterDescription> {
      new ParameterDescription("Param1", "Desc1", 10f),
      new ParameterDescription("Param2", "Desc2", 20f),
      new ParameterDescription("Param3", "Desc3", 30f)
    };

    Assert.Equal(3, params_list.Count);
  }

  [Fact]
  public void ParameterDescription_CanBeQueriedInLinq() {
    var params_list = new[] {
      new ParameterDescription("High", "Desc1", 100f),
      new ParameterDescription("Low", "Desc2", 10f),
      new ParameterDescription("Medium", "Desc3", 50f)
    };

    var highValue = params_list.Where(p => p.DefaultValue > 75f).ToArray();

    Assert.Single(highValue);
    Assert.Equal("High", highValue[0].Name);
  }

  [Fact]
  public void ParameterDescription_CanBeSorted() {
    var params_list = new[] {
      new ParameterDescription("Zebra", "Z", 10f),
      new ParameterDescription("Apple", "A", 20f),
      new ParameterDescription("Monkey", "M", 30f)
    };

    var sorted = params_list.OrderBy(p => p.Name).ToArray();

    Assert.Equal("Apple", sorted[0].Name);
    Assert.Equal("Monkey", sorted[1].Name);
    Assert.Equal("Zebra", sorted[2].Name);
  }

  [Fact]
  public void ParameterDescription_CanBeGrouped() {
    var params_list = new[] {
      new ParameterDescription("Param1", "TypeA", 10f),
      new ParameterDescription("Param2", "TypeA", 20f),
      new ParameterDescription("Param3", "TypeB", 30f)
    };

    var grouped = params_list.GroupBy(p => p.Description).ToArray();

    Assert.Equal(2, grouped.Length);
  }

  // =========================================================================
  // Practical Scenarios
  // =========================================================================

  [Fact]
  public void ParameterDescription_TemperatureSensor_CommonUsage() {
    var param = new ParameterDescription(
      "CPU Temperature",
      "Temperature of the main processor",
      25.0f);

    Assert.Equal("CPU Temperature", param.Name);
    Assert.Equal("Temperature of the main processor", param.Description);
    Assert.Equal(25.0f, param.DefaultValue);
  }

  [Fact]
  public void ParameterDescription_VoltageMonitor_CommonUsage() {
    var param = new ParameterDescription(
      "12V Rail",
      "Voltage on the 12V power rail",
      12.0f);

    Assert.True(param.DefaultValue > 11.5f && param.DefaultValue < 12.5f);
  }

  [Fact]
  public void ParameterDescription_MultipleParameters_Consistency() {
    var temps = new[] {
      new ParameterDescription("CPU Temp", "Core temperature", 25.0f),
      new ParameterDescription("GPU Temp", "GPU temperature", 35.0f),
      new ParameterDescription("System Temp", "System temperature", 30.0f)
    };

    Assert.Equal(3, temps.Length);
    Assert.All(temps, p => Assert.NotEmpty(p.Name));
  }

  [Fact]
  public void ParameterDescription_DefaultValueRange_ValidTemperatures() {
    var validTemps = new[] {
      new ParameterDescription("Temp1", "Desc", -40.0f),  // Valid min for sensors
      new ParameterDescription("Temp2", "Desc", 25.0f),   // Room temp
      new ParameterDescription("Temp3", "Desc", 125.0f)   // Valid max for sensors
    };

    Assert.All(validTemps, p => Assert.NotNull(p));
  }
}
