using Converters;

namespace ConverterTests;

[TestClass]
public sealed class BytesUnitConvertersTests {
  [TestMethod]
  public void SmallSizeConversion() {
    ulong bytes = 1024;
    string expectedKB = "1.00 KB";
    string actualKB = ByteUnitConverters.ConvertBytesToReadableUnit(bytes);
    Assert.AreEqual(expectedKB, actualKB);
  }

  [TestMethod]
  public void LargeSizeConversion() {
    ulong bytes = 1073741824; // 1 GB
    string expectedGB = "1.00 GB";
    string actualGB = ByteUnitConverters.ConvertBytesToReadableUnit(bytes);
    Assert.AreEqual(expectedGB, actualGB);
  }
}