using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using Log = LogSystem.LogService;

namespace ConsoleCommSimulator.Configuration
{
  public class TCMessageConfig
  {
    public int Interval { get; set; }
    public IDictionary<string, StateToTCValue> StateToTCMap { get; } = new Dictionary<string, StateToTCValue>();
    public IDictionary<string, List<double>> ThawingTCMap { get; } = new Dictionary<string, List<double>>();
    // contains at least default curve, thawing initial, thwing plateau and thawing end deltas
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
      return ParseSettingsByState(node) && ParseTCSettings(node);
    }

    private bool ParseTCSettings(XmlNode node)
    {
      ThawingTCMap.Clear();
      try
      {
        // adds the thawing deltas
        XmlNode tcThawingDataNode = node.SelectSingleNode("//TCThawingData");

        var InitialData = GetDoubleListFromValueAttribute(tcThawingDataNode, "_initial");
        var PlateauData = GetDoubleListFromValueAttribute(tcThawingDataNode, "_plateau");
        var EndData = GetDoubleListFromValueAttribute(tcThawingDataNode, "_end");

        ThawingTCMap["_initial"] = InitialData;
        ThawingTCMap["_plateau"] = PlateauData;
        ThawingTCMap["_end"] = EndData;
        return true;
      }
      catch (Exception e)
      {
        Log.LogException(e);
      }
      return false;
    }
    private List<double> GetDoubleListFromValueAttribute(XmlNode parentNode, string attributeName)
    {
      List<double> doubleList = new List<double>();

      XmlNode dataNode = parentNode.SelectSingleNode($"*[contains(@name, '{attributeName}')]");
      if (dataNode != null && dataNode.Attributes["value"] != null)
      {
        string valueAttribute = dataNode.Attributes["value"].Value;
        doubleList = valueAttribute.Split(',')
          .Select(str => double.TryParse(str, out double value) ? value : 0)
          .ToList();
      }

      return doubleList;
    }
    private bool ParseSettingsByState(XmlNode node)
    {
      // reset the dictionary, prevent adding the same keys again
      StateToTCMap.Clear();

      try
      {
        var TCStateNode = node?.SelectNodes("//Settings/TCstate");
        if (TCStateNode == null) return false;

        foreach (XmlNode statenode in TCStateNode)
        {
          string state;
          StateToTCValue value = new StateToTCValue();
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
                  case "TC1":
                    // all 3 values could be negative
                    value.TC1 = double.Parse(valueInnerText, CultureInfo.InvariantCulture);
                    break;
                  case "TC2":
                    value.TC2 = double.Parse(valueInnerText, CultureInfo.InvariantCulture);
                    break;
                  case "PMCUCJ":
                    value.PMCUCJ = double.Parse(valueInnerText, CultureInfo.InvariantCulture);
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
            StateToTCMap.Add(state, value);
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
    public int LoadTCInterval(XmlNode dataIntervalNode)
    {
      try
      {

        if (dataIntervalNode != null && int.TryParse(dataIntervalNode.InnerText, out int dataInterval))
        {
          return dataInterval;
        }
        else
        {
          throw new Exception("Error: DataInterval not found or invalid in the XML.");
        }
      }
      catch (Exception ex)
      {
        // Handle any exceptions that might occur during XML parsing or node selection.
        Log.LogException(ex);
        return -1;
      }
    }
    public List<double> LoadTCXML(XmlNode dataNode)
    {
      try
      {
        string dataString = dataNode.InnerText;
        // Parse the data into a double array
        string[] dataValues = dataString.Split(',');
        var TCXmlData = new List<double>();

        for (int i = 0; i < dataValues.Length; i++)
        {
          if (double.TryParse(dataValues[i], out double value))
          {

              TCXmlData.Add(value);

          }
          else
          {
            // Handle invalid data (not a double)
            Log.LogInfo("Invalid data value: " + dataValues[i]);
                      
          }
        }

        return TCXmlData;
      }
      catch (Exception e)
      {
        Log.LogException(e);
        return null;
      }
      

    }

  }
}
