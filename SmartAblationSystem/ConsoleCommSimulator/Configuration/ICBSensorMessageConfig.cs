using System;
using System.Collections.Generic;
using System.Xml;
using Log = Serilog.Log;

namespace ConsoleCommSimulator.Configuration
{
  public class ICBSensorMessageConfig
  {
    public int ECGInterval { get; set; }
    public IDictionary<string, StateToICBValue> StateToICBMap { get; } = new Dictionary<string, StateToICBValue>();

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
        Log.Error(xe.Message);
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
      StateToICBMap.Clear();

      try
      {
        var ICBstateNode = node?.SelectNodes("//Settings/ICBPressurestate");
        if (ICBstateNode == null) return false;

        foreach (XmlNode statenode in ICBstateNode)
        {
          string state;
          StateToICBValue value = new StateToICBValue();
          var stateInnerText = statenode?.Attributes?.GetNamedItem("state")?.InnerText;
          if (stateInnerText != null)
          {
            state = stateInnerText;
            //Console.WriteLine(value: setting.Attributes.GetNamedItem("state").InnerText);
          }
          else
          {
            Log.Error("parsing xml attribute name failed");
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
                    value.Pressure01 = double.Parse(valueInnerText);
                    break;
                  case "Pressure23":
                    value.Pressure23 = double.Parse(valueInnerText);
                    break;
                  case "Pressure45":
                    value.Pressure45 = double.Parse(valueInnerText);
                    break;
                  case "Pressure67":
                    value.Pressure67 = double.Parse(valueInnerText);
                    break;
                }
              }
              catch (FormatException fe)
              {
                Log.Error(fe.Message);
                return false;
              }
              catch (OverflowException oe)
              {
                Log.Error(oe.Message);
                return false;
              }
              
            }
            else
            {
              Log.Error("parsing xml attribute value failed");
              return false;
            }
          }
          try
          {
            StateToICBMap.Add(state, value);
          }
          catch (ArgumentException ae)
          {
            Log.Error(ae.Message);
            return false;
          }
        }
      }
      catch (System.Xml.XPath.XPathException xe)
      {
        Log.Error(xe.Message);
        return false;
      }

      return true;
    }

  }
}
