using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using Log = LogSystem.LogService;

namespace ConsoleCommSimulator.Configuration
{
  public class LCMessageConfig
  {
    public int Interval { get; set; }
    public int LCInterval { get; set; }
    public double LC1Value { get; set; }
    public bool Parse(XmlNode node)
    {
      if (node == null) return false;
      XmlNode internalNode;
      XmlNode lc1Node;
      XmlNode lc1IntervalNode;
      try
      {
        internalNode = node.SelectSingleNode("UpdateInterval");
        lc1Node = node.SelectSingleNode("LC1");
        lc1IntervalNode = node.SelectSingleNode("LC1Interval");
      }
      catch (System.Xml.XPath.XPathException xe)
      {
        Log.LogException(xe);
        return false;
      }

      if (internalNode != null && int.TryParse(internalNode.InnerText, out int interval))
      {
        Interval = interval;
      }
      if (lc1Node != null && double.TryParse(lc1Node.InnerText, out double lc1Value))
      {
        LC1Value = lc1Value;
      }
      if (lc1IntervalNode != null && int.TryParse(lc1IntervalNode.InnerText, out int lc1Interval))
      {
        LCInterval = lc1Interval;
      }

      return true;
    }

  }
}
