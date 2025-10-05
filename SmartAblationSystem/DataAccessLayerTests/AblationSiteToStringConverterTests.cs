
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared;
using SmartAblationSystem.Converters;

namespace DataAccessLayerTests
{
  [TestClass]
  public class AblationSiteToStringConverterTests
  {
    [TestMethod]
    public void ConvertTo_Typeof_AblationSiteEnum_Test()
    {
      var ablationSiteList = AblationSiteEnumHelper.GetSortedAblationSiteEnums();
      var converter = new AblationSiteToStringConverter();
      foreach (var ablationSite in ablationSiteList)
      {
        var result = converter.Convert(ablationSite, typeof(AblationSiteEnum), null, CultureInfo.InvariantCulture);
        var expectedResult = ablationSite.GetDescription(); 
        Assert.AreEqual(expectedResult, result);
      }

      Assert.AreEqual("-", converter.Convert(AblationSiteEnum.UNKNOWN, typeof(AblationSiteEnum), null, CultureInfo.InvariantCulture));
    }

    [TestMethod]
    public void ConvertTo_Typeof_Int_Test()
    {
      var ablationSiteList = AblationSiteEnumHelper.GetSortedAblationSiteEnums();
      var converter = new AblationSiteToStringConverter();
      foreach (var ablationSite in ablationSiteList)
      {
        var result = converter.Convert((int)ablationSite, typeof(int), null, CultureInfo.InvariantCulture);
        var expectedResult = ablationSite.GetDescription();
        Assert.AreEqual(expectedResult, result);
      }

      Assert.AreEqual("-", converter.Convert(100, typeof(int), null, CultureInfo.InvariantCulture));
    }

    [TestMethod]
    public void ConvertTo_Typeof_IntString_Test()
    {
      var ablationSiteList = AblationSiteEnumHelper.GetSortedAblationSiteEnums();
      var converter = new AblationSiteToStringConverter();
      foreach (var ablationSite in ablationSiteList)
      {
        var result = converter.Convert(((int)ablationSite).ToString(), typeof(string), null, CultureInfo.InvariantCulture);
        var expectedResult = ablationSite.GetDescription();
        Assert.AreEqual(expectedResult, result);
      }

      Assert.AreEqual("-", converter.Convert("100", typeof(string), null, CultureInfo.InvariantCulture));

    }

    [TestMethod]
    public void ConvertTo_Typeof_EnumNameString_Test()
    {
      var ablationSiteList = AblationSiteEnumHelper.GetSortedAblationSiteEnums();
      var converter = new AblationSiteToStringConverter();
      foreach (var ablationSite in ablationSiteList)
      {
        var result = converter.Convert(ablationSite.ToString(), typeof(string), null, CultureInfo.InvariantCulture);
        var expectedResult = ablationSite.GetDescription();
        Assert.AreEqual(expectedResult, result);
      }

      Assert.AreEqual("-", converter.Convert("unknown", typeof(int), null, CultureInfo.InvariantCulture));
      Assert.AreEqual("-", converter.Convert("hello world", typeof(int), null, CultureInfo.InvariantCulture));
    }

  }
}
