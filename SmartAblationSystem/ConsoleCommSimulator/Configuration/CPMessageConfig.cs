using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using Log = LogSystem.LogService;

namespace ConsoleCommSimulator.Configuration
{
  public class CPMessageConfig
  {
    public int Interval { get; set; }
    public IDictionary<string, StateToCPValue> StateToCPMap { get; } = new Dictionary<string, StateToCPValue>();

    public bool Parse(XmlNode node)
    {
      if (node == null) return false;
      XmlNode internalNode;
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
      return ParseSettingsByState(node);
    }

    private bool ParseSettingsByState(XmlNode node)
    {
      // reset the dictionary, prevent adding the same keys again
      StateToCPMap.Clear();

      try
      {
        var CPStateNode = node?.SelectNodes("//Settings/CPstate");
        if (CPStateNode == null) return false;

        foreach (XmlNode statenode in CPStateNode)
        {
          string state;
          StateToCPValue value = new StateToCPValue();
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
                  case "CP1":
                    value.CP1 = double.Parse(valueInnerText, CultureInfo.InvariantCulture);
                    break;
                  case "CP2":
                    value.CP2 = double.Parse(valueInnerText, CultureInfo.InvariantCulture);
                    break;
                  case "CTIP":
                    value.CTIP = double.Parse(valueInnerText, CultureInfo.InvariantCulture);
                    break;
                  case "PIDDUTY":
                    value.PIDDUTY = double.Parse(valueInnerText, CultureInfo.InvariantCulture);
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
            StateToCPMap.Add(state, value);
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

      return true;
    }

  }
}
