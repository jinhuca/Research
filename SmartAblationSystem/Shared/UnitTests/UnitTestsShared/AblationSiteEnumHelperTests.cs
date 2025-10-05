
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared;

using static Shared.AblationSiteEnum; 

namespace SharedUnitTests
{
  [TestClass]
  public class AblationSiteEnumHelperTests
  {
    [TestMethod]
    public void GetSortedAblationSiteEnums_Test()
    {
      var ablationSites = AblationSiteEnumHelper.GetSortedAblationSiteEnums();
      var expectedResult = new List<AblationSiteEnum> { LSPV, LIPV, RIPV, RSPV, LCPV, RMPV, OTHER };
      Assert.IsTrue(expectedResult.SequenceEqual(ablationSites));
    }

    [TestMethod]
    public void GetAblationSiteGroupNames_Test()
    {
      var groupNames = AblationSiteEnumHelper.GetAblationSiteGroupNames();
      var expectedResult = new List<string> { "LSPV", "LIPV", "RIPV", "RSPV", "LCPV", "RMPV", "OTHER" };
      Assert.IsTrue(expectedResult.SequenceEqual(groupNames));
    }

    [TestMethod]
    public void GetDescription_Test()
    {
      var values = Enum.GetValues(typeof(AblationSiteEnum)).OfType<AblationSiteEnum>(); 
      Assert.IsTrue(values.All( s=> Enum.GetName(typeof(AblationSiteEnum), s).Equals(s.GetDescription())));
    }

    [TestMethod]
    public void GetGroupName_Test()
    {
      Assert.AreEqual(nameof(AblationSiteEnum.LIPV), AblationSiteEnum.LIPV.GetGroupName());
      Assert.AreEqual(nameof(AblationSiteEnum.LSPV), AblationSiteEnum.LSPV.GetGroupName());
      Assert.AreEqual(nameof(AblationSiteEnum.RSPV), AblationSiteEnum.RSPV.GetGroupName());
      Assert.AreEqual(nameof(AblationSiteEnum.RIPV), AblationSiteEnum.RIPV.GetGroupName());
      Assert.AreEqual(nameof(AblationSiteEnum.LCPV), AblationSiteEnum.LCPV.GetGroupName());
      Assert.AreEqual(nameof(AblationSiteEnum.RMPV), AblationSiteEnum.RMPV.GetGroupName());
      Assert.AreEqual(nameof(AblationSiteEnum.OTHER), AblationSiteEnum.OTHER.GetGroupName());
      Assert.AreEqual(nameof(AblationSiteEnum.OTHER), AblationSiteEnum.UNKNOWN.GetGroupName());
    }
  }
}
