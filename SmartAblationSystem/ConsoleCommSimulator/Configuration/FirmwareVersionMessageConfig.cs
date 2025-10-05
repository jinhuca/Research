using System;
using System.Linq;
using System.Xml;
using Log = LogSystem.LogService;

namespace ConsoleCommSimulator.Configuration
{
  public class FirmwareVersionMessageConfig
  {
    public byte[] PMCUFirmwareData { get; set; } = new byte[8];
    public byte[] CMCUFirmwareData { get; set; } = new byte[8];
    public byte[] CatheterFirmwareData { get; set; } = new byte[8];
    public byte[] RepeaterICBFirmwareData { get; set; } = new byte[8];
    public byte[] RemoteFirmwareData { get; set; } = new byte[8];

    public bool Parse(XmlNode node)
    {
      if (node == null) return false;
      XmlNode internalNode;
      XmlNode internalNode3;
      XmlNode internalNode4;
      //pmcu
      XmlNode internalNode2;
      XmlNode internalNode5;

      // catheter firmware
      XmlNode internalNode6;
      //ICB
      XmlNode internalNode7;
      XmlNode internalNode8;
      XmlNode internalNode9;
      XmlNode internalNode10;
      // remote
      XmlNode internalNode11;
      XmlNode internalNode12;
      XmlNode internalNode13;
      XmlNode internalNode14;
      try
      {
        internalNode = node.SelectSingleNode("CMCUFirmwareVersion");
        internalNode3 = node.SelectSingleNode("CPLDVersion");
        internalNode4 = node.SelectSingleNode("CMCUBootloaderFirmwareVersion");
      }
      catch (System.Xml.XPath.XPathException xe)
      {
        Log.LogException(xe);
        return false;
      }
      // 

      var byteValue1 = ConfigUtils.ConvertIntStringToByteArray(internalNode?.Attributes?.GetNamedItem("value")?.InnerText, 16);
      var byteValue3 = ConfigUtils.ConvertIntStringToByteArray(internalNode3?.Attributes?.GetNamedItem("value")?.InnerText, 16);
      var byteValue4 = ConfigUtils.ConvertIntStringToByteArray(internalNode4?.Attributes?.GetNamedItem("value")?.InnerText, 16);
      if (byteValue1 != null && byteValue3 != null && byteValue4 != null)
      {
        // data will look like 00 00 16 01 which is 1.0.0.1 so we take last 2 
        CMCUFirmwareData[0] = byteValue1[2];
        CMCUFirmwareData[1] = byteValue1[3];
        CMCUFirmwareData[2] = byteValue3[2];
        CMCUFirmwareData[3] = byteValue3[3];
        CMCUFirmwareData[4] = byteValue4[2];
        CMCUFirmwareData[5] = byteValue4[3];
      }
      else
      {
        Log.LogInfo("getting xml node value failed");
        return false;
      }

      try
      {
        internalNode2 = node.SelectSingleNode("PMCUFirmwareVersion");
        internalNode5 = node.SelectSingleNode("PMCUBootloaderFirmwareVersion");
      }
      catch (System.Xml.XPath.XPathException xe)
      {
        Log.LogException(xe);
        return false;
      }
      var byteValue2 = ConfigUtils.ConvertIntStringToByteArray(internalNode2?.Attributes?.GetNamedItem("value")?.InnerText, 16);
      var byteValue5 = ConfigUtils.ConvertIntStringToByteArray(internalNode5?.Attributes?.GetNamedItem("value")?.InnerText, 16);
      if (byteValue2 != null && byteValue5 != null)
      {
        PMCUFirmwareData[0] = byteValue2[2];
        PMCUFirmwareData[1] = byteValue2[3];
        PMCUFirmwareData[2] = byteValue5[2];
        PMCUFirmwareData[3] = byteValue5[3];
      }
      else
      {
        Log.LogInfo("getting xml node value failed");
        return false;
      }

      try
      {
        internalNode6 = node.SelectSingleNode("CatheterFirmwareVersion");
      }
      catch (System.Xml.XPath.XPathException xe)
      {
        Log.LogException(xe);
        return false;
      }
      var byteValue6 = ConfigUtils.ConvertIntStringToByteArray(internalNode6?.Attributes?.GetNamedItem("value")?.InnerText, 16);
      if (byteValue6 != null)
      {
        // use last 2
        CatheterFirmwareData[0] = byteValue6[2];

        CatheterFirmwareData[1] = byteValue6[3];
      }
      else
      {
        Log.LogInfo("getting xml node value failed");
        return false;
      }
      try
      {
        internalNode7 = node.SelectSingleNode("RepeaterFirmwareVersion");
        internalNode8 = node.SelectSingleNode("ICBFirmwareVersion");
        internalNode9 = node.SelectSingleNode("RepeaterBootloaderFirmwareVersion");
        internalNode10 = node.SelectSingleNode("ICBBootloaderFirmwareVersion");
      }
      catch (System.Xml.XPath.XPathException xe)
      {
        Log.LogException(xe);
        return false;
      }
      var byteValue7 = ConfigUtils.ConvertIntStringToByteArray(internalNode7?.Attributes?.GetNamedItem("value")?.InnerText, 16);
      var byteValue8 = ConfigUtils.ConvertIntStringToByteArray(internalNode8?.Attributes?.GetNamedItem("value")?.InnerText, 16);
      var byteValue9 = ConfigUtils.ConvertIntStringToByteArray(internalNode9?.Attributes?.GetNamedItem("value")?.InnerText, 16);
      var byteValue10 = ConfigUtils.ConvertIntStringToByteArray(internalNode10?.Attributes?.GetNamedItem("value")?.InnerText, 16);
      if (byteValue7 == null || byteValue8 == null || byteValue9 == null || byteValue10 == null)
      {
        Log.LogInfo("getting xml node value failed");
        return false;
      }
      else
      {
        // use last 2
        RepeaterICBFirmwareData[0] = byteValue7[2];
        RepeaterICBFirmwareData[1] = byteValue7[3];
        RepeaterICBFirmwareData[2] = byteValue8[2];
        RepeaterICBFirmwareData[3] = byteValue8[3];
        RepeaterICBFirmwareData[4] = byteValue9[2];
        RepeaterICBFirmwareData[5] = byteValue9[3];
        RepeaterICBFirmwareData[6] = byteValue10[2];
        RepeaterICBFirmwareData[7] = byteValue10[3];

      }
      try
      {
        internalNode11 = node.SelectSingleNode("RemoteFirmwareVersion");
        internalNode12 = node.SelectSingleNode("RemoteBootloaderFirmwareVersion");
        // those are unknown data
        internalNode13 = node.SelectSingleNode("RemoteFirmwareVersion3");
        internalNode14 = node.SelectSingleNode("RemoteFirmwareVersion4");
      }
      catch (System.Xml.XPath.XPathException xe)
      {
        Log.LogException(xe);
        return false;
      }
      var byteValue11 = ConfigUtils.ConvertIntStringToByteArray(internalNode11?.Attributes?.GetNamedItem("value")?.InnerText, 16);
      var byteValue12 = ConfigUtils.ConvertIntStringToByteArray(internalNode12?.Attributes?.GetNamedItem("value")?.InnerText, 16);
      var byteValue13 = ConfigUtils.ConvertIntStringToByteArray(internalNode13?.Attributes?.GetNamedItem("value")?.InnerText, 16);
      var byteValue14 = ConfigUtils.ConvertIntStringToByteArray(internalNode14?.Attributes?.GetNamedItem("value")?.InnerText, 16);
      if (byteValue11 == null || byteValue12 == null || byteValue13 == null || byteValue14 == null)
      {
        Log.LogInfo("getting xml node value failed");
        return false;
      }
      else
      {
        // use last 2
        RemoteFirmwareData[0] = byteValue11[2];
        RemoteFirmwareData[1] = byteValue11[3];
        RemoteFirmwareData[2] = byteValue12[2]; // 60
        RemoteFirmwareData[3] = byteValue12[3]; // 61
        RemoteFirmwareData[4] = byteValue13[2];
        RemoteFirmwareData[5] = byteValue13[3];
        RemoteFirmwareData[6] = byteValue14[2];
        RemoteFirmwareData[7] = byteValue14[3];

      }
      // add the 56 24 and 11 configs and XML
      // 56: 4002
      // 24: 1002 3C3D B2F43C3D length 8 or 4?
      // 11: 2006 1401 1305 0000 (8)
      return true;
    }

  }
}
