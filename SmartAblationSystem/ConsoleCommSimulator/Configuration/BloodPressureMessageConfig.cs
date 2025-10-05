using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using Log = LogSystem.LogService;

namespace ConsoleCommSimulator.Configuration
{
  public class BloodPressureMessageConfig
  {
    public int ECGInterval { get; set; }
    public IDictionary<string, StateToICBValue> StateToBloodPressureMap { get; } = new Dictionary<string, StateToICBValue>();

    public bool Parse(XmlNode node)
    {
      if (node == null) return false;
      XmlNode internalNode;
      try
      {
        internalNode = node.SelectSingleNode("ICBUpdateInterval");

      }
      catch (System.Xml.XPath.XPathException xe)
      {
        Log.LogException(xe);
        return false;
      }

      if (internalNode != null && int.TryParse(internalNode.InnerText, out int interval))
      {
        ECGInterval = interval;
      }

      return ParseSettingsByState(node);
    }

    private bool ParseSettingsByState(XmlNode node)
    {
      // reset the dictionary, prevent adding the same keys again
      StateToBloodPressureMap.Clear();

      try
      {
        var ICBstateNode = node?.SelectNodes("//Settings/BloodPressurestate");
        if (ICBstateNode == null) return false;

        foreach (XmlNode statenode in ICBstateNode)
        {
          string state;
          StateToICBValue value = new StateToICBValue();
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
                  case "Pressure01":
                    value.Pressure01 = double.Parse(valueInnerText, CultureInfo.InvariantCulture);
                    break;
                  case "Pressure23":
                    value.Pressure23 = double.Parse(valueInnerText, CultureInfo.InvariantCulture);
                    break;
                  case "Pressure45":
                    value.Pressure45 = double.Parse(valueInnerText, CultureInfo.InvariantCulture);
                    break;
                  case "Pressure67":
                    value.Pressure67 = double.Parse(valueInnerText, CultureInfo.InvariantCulture);
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
            StateToBloodPressureMap.Add(state, value);
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
