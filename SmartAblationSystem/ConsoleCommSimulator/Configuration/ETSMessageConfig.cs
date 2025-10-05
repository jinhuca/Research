using System;
using System.Collections.Generic;
using System.Xml;
using Log = LogSystem.LogService;

namespace ConsoleCommSimulator.Configuration
{
  public class ETSMessageConfig
  {
    public int Interval { get; set; }
    public IDictionary<string, StateToETSValue> StateToETSMap { get; } = new Dictionary<string, StateToETSValue>();

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
      StateToETSMap.Clear();

      try
      {
        var ICBstateNode = node?.SelectNodes("//Settings/ETSstate");
        if (ICBstateNode == null) return false;

        foreach (XmlNode statenode in ICBstateNode)
        {
          string state;
          StateToETSValue value = new StateToETSValue();
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
                  case "Channel0":
                    value.Channel0 = int.Parse(valueInnerText);
                    break;
                  case "Channel1":
                    value.Channel1 = int.Parse(valueInnerText);
                    break;
                  case "Channel2":
                    value.Channel2 = int.Parse(valueInnerText);
                    break;
                  case "Channel3":
                    value.Channel3 = int.Parse(valueInnerText);
                    break;
                  case "Channel4":
                    value.Channel4 = int.Parse(valueInnerText);
                    break;
                  case "Channel5":
                    value.Channel5 = int.Parse(valueInnerText);
                    break;
                  case "Channel6":
                    value.Channel6 = int.Parse(valueInnerText);
                    break;
                  case "Channel7":
                    value.Channel7 = int.Parse(valueInnerText);
                    break;
                  case "Channel8":
                    value.Channel8 = int.Parse(valueInnerText);
                    break;
                  case "Channel9":
                    value.Channel9 = int.Parse(valueInnerText);
                    break;
                  case "Channel10":
                    value.Channel10 = int.Parse(valueInnerText);
                    break;
                  case "Channel11":
                    value.Channel11 = int.Parse(valueInnerText);
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
            StateToETSMap.Add(state, value);
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
