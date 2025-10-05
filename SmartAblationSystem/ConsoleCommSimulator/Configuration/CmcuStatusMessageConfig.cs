using System;
using System.Collections.Generic;
using System.Xml;
using Log = LogSystem.LogService;

namespace ConsoleCommSimulator.Configuration
{
  public class CmcuStatusMessageConfig
  {
    public int Interval { get; set; }
    public IDictionary<string, byte[]> StateToMessageByteMap { get; } = new Dictionary<string, byte[]>();

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
    // function returns false if any exception occurs, only return true if values are valid
    private bool ParseSettingsByState(XmlNode node)
    {
      // reset the dictionary, prevent adding the same keys again
      StateToMessageByteMap.Clear();

      try
      {
        var settingNode = node?.SelectNodes("Settings/Setting");
        if (settingNode == null) return false;

        foreach (XmlNode setting in settingNode)
        {
          string statelocation;
          byte[] valuelocation;
          var stateInnerText = setting?.Attributes?.GetNamedItem("state")?.InnerText;
          if (stateInnerText != null)
          {
            statelocation = stateInnerText;
          }
          else
          {
            Log.LogInfo("getting xml node key failed");
            return false;
          }
          var byteValue = ConfigUtils.ConvertIntStringToByteArray(setting?.Attributes?.GetNamedItem("value")?.InnerText, 16);
          if (byteValue != null)
          {
            valuelocation = byteValue;
          }
          else
          {
            Log.LogInfo("getting xml node value failed");
            return false;
          }
          try
          {
            StateToMessageByteMap.Add(statelocation, valuelocation);
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
