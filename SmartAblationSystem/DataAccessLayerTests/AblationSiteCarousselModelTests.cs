
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared;
using SmartAblationSystem.Models;

namespace DataAccessLayerTests
{
  [TestClass]
  public class AblationSiteCarousselModelTests
  {
    [TestMethod]
    public void MoveAblationSiteToTheLeft_Test()
    {
      AblationSiteCarousselModel.CurrentAblationSite = AblationSiteEnum.OTHER;
      var ablationSiteList = AblationSiteEnumHelper.GetSortedAblationSiteEnums(); 
      var count = ablationSiteList.Count;
      var curIndex = ablationSiteList.IndexOf(AblationSiteCarousselModel.CurrentAblationSite);
      Trace.WriteLine(AblationSiteCarousselModel.CurrentAblationSite);

      for (int i = 0; i < count*3; ++i)
      {
        AblationSiteCarousselModel.MoveAblationSiteToTheLeft();
        Trace.WriteLine(AblationSiteCarousselModel.CurrentAblationSite);
        curIndex = (--curIndex + count) % count;
        Assert.AreEqual(ablationSiteList[curIndex], AblationSiteCarousselModel.CurrentAblationSite); 
      }
    }

    [TestMethod]
    public void MoveAblationSiteToTheRight_Test()
    {
      AblationSiteCarousselModel.CurrentAblationSite = AblationSiteEnum.OTHER;
      var ablationSiteList = AblationSiteEnumHelper.GetSortedAblationSiteEnums();
      var count = ablationSiteList.Count;
      var curIndex = ablationSiteList.IndexOf(AblationSiteCarousselModel.CurrentAblationSite);
      Trace.WriteLine(AblationSiteCarousselModel.CurrentAblationSite);

      for (int i = 0; i < count * 3; ++i)
      {
        AblationSiteCarousselModel.MoveAblationSiteToTheRight();
        Trace.WriteLine(AblationSiteCarousselModel.CurrentAblationSite);
        curIndex = ++curIndex% count;
        Assert.AreEqual(ablationSiteList[curIndex], AblationSiteCarousselModel.CurrentAblationSite);
      }
    }
  }
}
