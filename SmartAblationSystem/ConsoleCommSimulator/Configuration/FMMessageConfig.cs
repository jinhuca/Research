using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using Log = LogSystem.LogService;

namespace ConsoleCommSimulator.Configuration
{
  public class FMMessageConfig
  {
    public int Interval { get; set; }
    public double TargetFM { get; set; }
    public List<int> HiFlowCatheters { get; set; } = new List<int>();
    public IDictionary<string, StateToFMValue> StateToFMMap { get; } = new Dictionary<string, StateToFMValue>();

    public bool Parse(XmlNode node)
    {
      if (node == null) return false;
      XmlNode internalNode;
      XmlNode internalFMNode;
      try
      {
        internalNode = node.SelectSingleNode("UpdateInterval");
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
      try
      {
        internalFMNode = node.SelectSingleNode("TargetFM");
      }
      catch (System.Xml.XPath.XPathException xe)
      {
        Log.LogException(xe);
        return false;
      }

      if (internalFMNode != null && double.TryParse(internalFMNode.InnerText, out double fm))
      {
        TargetFM = fm;
      }
      return ParseSettingsByState(node);
    }
    private bool ParseSettingsByState(XmlNode node)
    {
      // reset the dictionary, prevent adding the same keys again
      StateToFMMap.Clear();

      try
      {
        var FMStateNode = node?.SelectNodes("//Settings/FMstate");
        if (FMStateNode == null) return false;

        foreach (XmlNode statenode in FMStateNode)
        {
          string state;
          StateToFMValue value = new StateToFMValue();
          var stateInnerText = statenode?.Attributes?.GetNamedItem("state")?.InnerText;
          if (stateInnerText != null)
          {
            state = stateInnerText;
          }
          else
          {
            Log.LogInfo("parsing xml attribute name failed");
            return false;
          }

          foreach (XmlNode setting in statenode.ChildNodes)
          {
            var valueInnerText = setting?.Attributes?.GetNamedItem("value")?.InnerText;
            var nameInnerText = setting?.Attributes?.GetNamedItem("name")?.InnerText;
            if (valueInnerText != null
              && nameInnerText != null)
            {
              try
              {
                switch (nameInnerText)
                {
                  case "FM1":
                    value.FM1 = double.Parse(valueInnerText, CultureInfo.InvariantCulture);
                    break;
                  case "PT5":
                    value.PT5 = double.Parse(valueInnerText, CultureInfo.InvariantCulture);
                    break;
                  case "PID":
                    value.PID = double.Parse(valueInnerText, CultureInfo.InvariantCulture);
                    break;
                }
              }
              catch (FormatException fe)
              {
                Log.LogException(fe);
                return false;
              }
              catch (OverflowException oe)
              {
                Log.LogException(oe);
                return false;
              }
              
            }
            else
            {
              Log.LogInfo("parsing xml attribute value failed");
              return false;
            }
          }
          try
          {
            StateToFMMap.Add(state, value);
          }
          catch (ArgumentException ae)
          {
            Log.LogException(ae);
            return false;
          }
        }
      }
      catch (System.Xml.XPath.XPathException xe)
      {
        Log.LogException(xe);
        return false;
      }

      return ParseHiFlowCatheters(node);
    }
    private bool ParseHiFlowCatheters(XmlNode node)
    {
      try
      {
        var HiFlowNode = node?.SelectNodes("//HiFlowCatheters");
        if (HiFlowNode == null) return false;

        foreach (XmlNode catheternode in HiFlowNode)
        {
          if (catheternode != null && int.TryParse(catheternode.InnerText, out int catheterId))
          {
            HiFlowCatheters.Add(catheterId);
          }

        }

      }
      catch (System.Xml.XPath.XPathException xe)
      {
        Log.LogException(xe);
        return false;
      }

      return true;
    }

  }
}
