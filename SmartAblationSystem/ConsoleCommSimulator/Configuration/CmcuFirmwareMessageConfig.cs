using System;
using System.Xml;
using Log = Serilog.Log;

namespace ConsoleCommSimulator.Configuration
{
  public class CmcuFirmwareMessageConfig
  {
    public byte[] Firmware { get; set; } = new byte[8];

    public bool Parse(XmlNode node)
    {
      if (node == null) return false;

      return ParseSettingsByState(node);
    }
    // function returns false if any exception occurs, only return true if values are valid
    private bool ParseSettingsByState(XmlNode node)
    {
      try
      {
        var settingNode = node?.SelectNodes("FirmwareVersion");
        if (settingNode == null) return false;

        foreach (XmlNode setting in settingNode)
        {
          var stateInnerText = setting?.Attributes?.GetNamedItem("value")?.InnerText;
          if (stateInnerText != null)
          {
            // firmware is larger than int so we use int64
            Int64.TryParse(stateInnerText, out Int64 value);
            Firmware[0] = (byte) ((value >> 24) & 0xFF );
            Firmware[1] = (byte) ((value >> 20) & 0xFF );
            Firmware[2] = (byte) ((value >> 16) & 0xFF );
            Firmware[3] = (byte) ((value >> 12) & 0xFF );
            Firmware[4] = (byte) ((value >> 8) & 0xFF );
            Firmware[5] = (byte) ((value) & 0xFF );
          }
          else
          {
            Log.Error("getting xml node key failed");
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
