using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using Log = LogSystem.LogService;

namespace ConsoleCommSimulator.Configuration
{
  public class PTMessageConfig
  {
    public int Interval { get; set; }
    public IDictionary<string, StateToPTValue> StateToIntByteMap { get; } = new Dictionary<string, StateToPTValue>();
    //public byte[] PTdata { get; set; } = new byte[8];

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
      StateToIntByteMap.Clear();

      try
      {
        var nodeOfPtstate = node?.SelectNodes("//Settings/Ptstate");
        if (nodeOfPtstate == null) return false;

        foreach (XmlNode statenode in nodeOfPtstate)
        {
          string state;
          StateToPTValue value = new StateToPTValue();
          var newStateInnerText = statenode?.Attributes?.GetNamedItem("state")?.InnerText;
          if (newStateInnerText != null)
          {
            state = newStateInnerText;
          }
          else
          {
            Log.LogInfo("parsing xml attribute name failed");
            return false;
          }

          foreach (XmlNode setting in statenode.ChildNodes)
          {
            var valueInnertext = setting?.Attributes?.GetNamedItem("value")?.InnerText;
            var nameInnerText = setting?.Attributes?.GetNamedItem("name")?.InnerText;

            if (valueInnertext != null
              && nameInnerText != null)
            {
              try
              {
                var newPTValue = double.Parse(valueInnertext, CultureInfo.InvariantCulture);
                switch (nameInnerText)
                {
                  case "PT1":
                    value.PT1 = newPTValue;
                    break;
                  case "PT2":
                    value.PT2 = newPTValue;
                    break;
                  case "PT3":
                    value.PT3 = newPTValue;
                    break;
                  case "PT4":
                    value.PT4 = newPTValue;
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
            StateToIntByteMap.Add(state, value);
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
