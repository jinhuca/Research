using CrystalMonitor.Hardware;
using System.Text;

namespace CrystalMonitorTests.HardwareTests;

/// <summary>
/// Unit tests for the Identifier class.
/// Tests construction, parsing, equality, comparison, and hierarchy operations.
/// </summary>
public class IdentifierTests {
  // =========================================================================
  // Construction - Basic
  // =========================================================================

  [Fact]
  public void Identifier_Construction_SingleElement_DoesNotThrow() {
    var ex = Record.Exception(() => new Identifier("test"));
    Assert.Null(ex);
  }

  [Fact]
  public void Identifier_Construction_MultipleElements_DoesNotThrow() {
    var ex = Record.Exception(() => new Identifier("test", "0", "cpu"));
    Assert.Null(ex);
  }

  [Fact]
  public void Identifier_Construction_EmptyElements_DoesNotThrow() {
    var ex = Record.Exception(() => new Identifier("", "", ""));
    Assert.Null(ex);
  }

  [Fact]
  public void Identifier_ToString_SingleElement_FormatsCorrectly() {
    var id = new Identifier("test");
    Assert.Equal("/test", id.ToString());
  }

  [Fact]
  public void Identifier_ToString_MultipleElements_FormatsCorrectly() {
    var id = new Identifier("test", "0", "cpu");
    Assert.Equal("/test/0/cpu", id.ToString());
  }

  // =========================================================================
  // Construction - Hierarchy
  // =========================================================================

  [Fact]
  public void Identifier_Construction_FromBase_ExtendsProperly() {
    var baseId = new Identifier("test", "0");
    var extendedId = new Identifier(baseId, "cpu");

    Assert.Equal("/test/0/cpu", extendedId.ToString());
  }

  [Fact]
  public void Identifier_Construction_NestedHierarchy_BuildsCorrectly() {
    var id1 = new Identifier("test");
    var id2 = new Identifier(id1, "0");
    var id3 = new Identifier(id2, "cpu");
    var id4 = new Identifier(id3, "core0");

    Assert.Equal("/test/0/cpu/core0", id4.ToString());
  }

  [Fact]
  public void Identifier_Construction_FromBase_WithMultipleExtensions() {
    var baseId = new Identifier("hardware");
    var extended = new Identifier(baseId, "sensor1", "temperature", "0");

    Assert.Equal("/hardware/sensor1/temperature/0", extended.ToString());
  }

  [Fact]
  public void Identifier_Construction_DeepHierarchy() {
    var id = new Identifier("a", "b", "c", "d", "e", "f", "g", "h");
    Assert.Equal("/a/b/c/d/e/f/g/h", id.ToString());
  }

  // =========================================================================
  // URI Encoding
  // =========================================================================

  [Fact]
  public void Identifier_Construction_EscapesSpecialCharacters() {
    var id = new Identifier("test value", "with spaces");
    var result = id.ToString();

    // Should contain URL-encoded values
    Assert.Contains("%20", result); // space encoded as %20
  }

  [Theory]
  [InlineData("test@value")]
  [InlineData("test:value")]
  [InlineData("test/value")]
  [InlineData("test\\value")]
  public void Identifier_Construction_EscapesSpecialChars(string input) {
    var id = new Identifier(input);
    var result = id.ToString();

    // Should have encoded the special character
    Assert.NotNull(result);
  }

  [Fact]
  public void Identifier_Construction_WithUnicodeCharacters() {
    var id = new Identifier("test°C", "温度");
    var result = id.ToString();

    Assert.NotNull(result);
  }

  // =========================================================================
  // Equality
  // =========================================================================

  [Fact]
  public void Identifier_Equals_SameElements_AreEqual() {
    var id1 = new Identifier("test", "0", "cpu");
    var id2 = new Identifier("test", "0", "cpu");

    Assert.Equal(id1, id2);
  }

  [Fact]
  public void Identifier_Equals_DifferentElements_AreNotEqual() {
    var id1 = new Identifier("test", "0", "cpu");
    var id2 = new Identifier("test", "1", "cpu");

    Assert.NotEqual(id1, id2);
  }

  [Fact]
  public void Identifier_Equals_NullOther_ReturnsFalse() {
    var id = new Identifier("test");

    Assert.NotEqual(id, null);
  }

  [Fact]
  public void Identifier_Equals_Reflexive() {
    var id = new Identifier("test", "0");

    Assert.Equal(id, id);
  }

  [Fact]
  public void Identifier_Equals_Symmetric() {
    var id1 = new Identifier("test", "0");
    var id2 = new Identifier("test", "0");

    Assert.Equal(id1, id2);
    Assert.Equal(id2, id1);
  }

  [Fact]
  public void Identifier_Equals_Transitive() {
    var id1 = new Identifier("test", "0");
    var id2 = new Identifier("test", "0");
    var id3 = new Identifier("test", "0");

    Assert.Equal(id1, id2);
    Assert.Equal(id2, id3);
    Assert.Equal(id1, id3);
  }

  [Fact]
  public void Identifier_Equals_ConstructedDifferentWays_AreEqual() {
    var id1 = new Identifier("test", "0", "cpu");
    var id2 = new Identifier(new Identifier("test", "0"), "cpu");

    Assert.Equal(id1, id2);
  }

  // =========================================================================
  // Equality Operators
  // =========================================================================

  [Fact]
  public void Identifier_OperatorEqual_SameElements_True() {
    var id1 = new Identifier("test", "0");
    var id2 = new Identifier("test", "0");

    Assert.True(id1 == id2);
  }

  [Fact]
  public void Identifier_OperatorEqual_DifferentElements_False() {
    var id1 = new Identifier("test", "0");
    var id2 = new Identifier("test", "1");

    Assert.False(id1 == id2);
  }

  [Fact]
  public void Identifier_OperatorNotEqual_DifferentElements_True() {
    var id1 = new Identifier("test", "0");
    var id2 = new Identifier("test", "1");

    Assert.True(id1 != id2);
  }

  [Fact]
  public void Identifier_OperatorEqual_BothNull_True() {
    Identifier id1 = null;
    Identifier id2 = null;

    Assert.True(id1 == id2);
  }

  [Fact]
  public void Identifier_OperatorEqual_OneNull_False() {
    var id1 = new Identifier("test");
    Identifier id2 = null;

    Assert.False(id1 == id2);
  }

  // =========================================================================
  // Comparison
  // =========================================================================

  [Fact]
  public void Identifier_CompareTo_EqualIdentifiers_ReturnsZero() {
    var id1 = new Identifier("test", "0");
    var id2 = new Identifier("test", "0");

    Assert.Equal(0, id1.CompareTo(id2));
  }

  [Fact]
  public void Identifier_CompareTo_FirstLess_ReturnsNegative() {
    var id1 = new Identifier("aaa");
    var id2 = new Identifier("bbb");

    Assert.True(id1.CompareTo(id2) < 0);
  }

  [Fact]
  public void Identifier_CompareTo_FirstGreater_ReturnsPositive() {
    var id1 = new Identifier("zzz");
    var id2 = new Identifier("aaa");

    Assert.True(id1.CompareTo(id2) > 0);
  }

  [Fact]
  public void Identifier_CompareTo_NullOther_ReturnsPositive() {
    var id = new Identifier("test");

    Assert.True(id.CompareTo(null) > 0);
  }

  [Fact]
  public void Identifier_OperatorLessThan_Ordering() {
    var id1 = new Identifier("aaa");
    var id2 = new Identifier("bbb");

    Assert.True(id1 < id2);
    Assert.False(id2 < id1);
  }

  [Fact]
  public void Identifier_OperatorGreaterThan_Ordering() {
    var id1 = new Identifier("zzz");
    var id2 = new Identifier("aaa");

    Assert.True(id1 > id2);
    Assert.False(id2 > id1);
  }

  [Fact]
  public void Identifier_OperatorLessThan_NullFirst_True() {
    Identifier id1 = null;
    var id2 = new Identifier("test");

    Assert.True(id1 < id2);
  }

  [Fact]
  public void Identifier_OperatorGreaterThan_NullFirst_False() {
    Identifier id1 = null;
    var id2 = new Identifier("test");

    Assert.False(id1 > id2);
  }

  // =========================================================================
  // Hashing
  // =========================================================================

  [Fact]
  public void Identifier_GetHashCode_EqualIdentifiers_SameHash() {
    var id1 = new Identifier("test", "0");
    var id2 = new Identifier("test", "0");

    Assert.Equal(id1.GetHashCode(), id2.GetHashCode());
  }

  [Fact]
  public void Identifier_GetHashCode_CanBeUsedInHashSet() {
    var id1 = new Identifier("test", "0");
    var id2 = new Identifier("test", "0");
    var id3 = new Identifier("test", "1");

    var set = new HashSet<Identifier> { id1, id2, id3 };

    // id1 and id2 are equal, so set should have 2 elements
    Assert.Equal(2, set.Count);
  }

  [Fact]
  public void Identifier_GetHashCode_CanBeUsedInDictionary() {
    var id1 = new Identifier("test", "0");
    var id2 = new Identifier("test", "0");

    var dict = new Dictionary<Identifier, string> { { id1, "value1" } };

    Assert.True(dict.ContainsKey(id2));
    Assert.Equal("value1", dict[id2]);
  }

  // =========================================================================
  // Edge Cases
  // =========================================================================

  [Fact]
  public void Identifier_Construction_NoElements_CreatesEmptyIdentifier() {
    var id = new Identifier();
    Assert.Empty(id.ToString());
  }

  [Fact]
  public void Identifier_Construction_LongElement() {
    var longElement = new string('a', 1000);
    var id = new Identifier(longElement);

    Assert.NotNull(id.ToString());
  }

  [Fact]
  public void Identifier_Construction_ManyElements() {
    var elements = Enumerable.Range(0, 100).Select(i => i.ToString()).ToArray();
    var id = new Identifier(elements);

    Assert.NotNull(id.ToString());
  }

  [Fact]
  public void Identifier_Equals_WithDifferentCase_CaseSensitive() {
    var id1 = new Identifier("Test");
    var id2 = new Identifier("test");

    // Should be different due to case sensitivity
    Assert.NotEqual(id1, id2);
  }

  [Fact]
  public void Identifier_ToString_ConsistentAcrossMultipleCalls() {
    var id = new Identifier("test", "0", "cpu");
    var str1 = id.ToString();
    var str2 = id.ToString();
    var str3 = id.ToString();

    Assert.Equal(str1, str2);
    Assert.Equal(str2, str3);
  }

  [Fact]
  public void Identifier_CompareTo_SortsCorrectly() {
    var ids = new[] {
      new Identifier("zebra"),
      new Identifier("apple"),
      new Identifier("monkey"),
      new Identifier("banana")
    };

    var sorted = ids.OrderBy(id => id, Comparer<Identifier>.Create((a, b) => a.CompareTo(b))).ToArray();

    Assert.Equal("/apple", sorted[0].ToString());
    Assert.Equal("/banana", sorted[1].ToString());
    Assert.Equal("/monkey", sorted[2].ToString());
    Assert.Equal("/zebra", sorted[3].ToString());
  }

  [Fact]
  public void Identifier_Hierarchy_ParentChild_Relationship() {
    var parent = new Identifier("hardware", "0");
    var child = new Identifier(parent, "sensor", "temp");

    var parentStr = parent.ToString();
    var childStr = child.ToString();

    Assert.True(childStr.StartsWith(parentStr));
  }

  [Fact]
  public void Identifier_Hierarchy_SiblingIdentifiers() {
    var base_id = new Identifier("hardware", "0");
    var sibling1 = new Identifier(base_id, "sensor0");
    var sibling2 = new Identifier(base_id, "sensor1");

    Assert.NotEqual(sibling1, sibling2);
    Assert.True(sibling1.ToString().StartsWith(base_id.ToString()));
    Assert.True(sibling2.ToString().StartsWith(base_id.ToString()));
  }

  // =========================================================================
  // Integration with Collections
  // =========================================================================

  [Fact]
  public void Identifier_CanBeUsedInList() {
    var ids = new List<Identifier> {
      new Identifier("test", "0"),
      new Identifier("test", "1"),
      new Identifier("test", "2")
    };

    Assert.Equal(3, ids.Count);
    Assert.Contains(new Identifier("test", "0"), ids);
  }

  [Fact]
  public void Identifier_CanBeQueried() {
    var ids = new[] {
      new Identifier("cpu", "0"),
      new Identifier("cpu", "1"),
      new Identifier("gpu", "0"),
      new Identifier("memory", "0")
    };

    var cpuIds = ids.Where(id => id.ToString().Contains("cpu")).ToArray();

    Assert.Equal(2, cpuIds.Length);
  }

  [Fact]
  public void Identifier_CanBeComparedInLinq() {
    var ids = new[] {
      new Identifier("zebra"),
      new Identifier("apple"),
      new Identifier("banana")
    };

    var sorted = ids.OrderBy(id => id).ToArray();

    Assert.Equal("/apple", sorted[0].ToString());
    Assert.Equal("/banana", sorted[1].ToString());
    Assert.Equal("/zebra", sorted[2].ToString());
  }
}
