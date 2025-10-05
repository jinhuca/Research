using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using Log = LogSystem.LogService;

namespace ConsoleCommSimulator.Configuration
{
  public class DMSSensorMessageConfig
  {
    public int ECGInterval { get; set; }
    public byte[] DMSSetting { get; set; } = new byte[8];
/*    public bool HiResDMS { get; set; }*/
    public IDictionary<string, StateToDMSValue> StateToDMSMap { get; } = new Dictionary<string, StateToDMSValue>();

    public bool Parse(XmlNode node)
    {
      if (node == null) return false;
      XmlNode internalNode;
      XmlNode DMS1;
      XmlNode DMS2;
      XmlNode DMS3;
      XmlNode DMS4;
      XmlNode HiResDMSnode;
      try
      {
        internalNode = node.SelectSingleNode("ECGUpdateInterval");
        DMS1 = node.SelectSingleNode("DMSConnected");
        DMS2 = node.SelectSingleNode("PressureConnected");
        DMS3 = node.SelectSingleNode("Series400ETSConnected");
        DMS4 = node.SelectSingleNode("CircaETSConnected");
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
      if (DMS1 != null && int.TryParse(DMS1.InnerText, out int dms1))
      { // DMS: should always be 1 
        if (dms1 == 1)
        {
          DMSSetting[0] = (byte)(DMSSetting[0] | 0x01);
        }
      }
      if (DMS2 != null && int.TryParse(DMS2.InnerText, out int dms2))
      { // Pressure
        if (dms2 == 1)
        {
          DMSSetting[0] = (byte)(DMSSetting[0] | 0x02);
        }
      }
      if (DMS3 != null && int.TryParse(DMS3.InnerText, out int dms3))
      { // 400 ETS
        if (dms3 == 1)
        {
          DMSSetting[0] = (byte)(DMSSetting[0] | 0x04);
        }
      }
      if (DMS4 != null && int.TryParse(DMS4.InnerText, out int dms4))
      { // circa ETS
        if (dms4 == 1)
        {
          DMSSetting[0] = (byte)(DMSSetting[0] | 0x08);
        }
      }
      DMSSetting[0] = (byte)(DMSSetting[0] & 0xff);
      /*if (HiResDMSnode != null && int.TryParse(HiResDMSnode.InnerText, out int hiResDMS))
      {
        HiResDMS = (hiResDMS == 1);
      }*/
      return ParseSettingsByState(node);
    }

    private bool ParseSettingsByState(XmlNode node)
    {
      // reset the dictionary, prevent adding the same keys again
      StateToDMSMap.Clear();
      try
      {
        
        XmlNodeList DMSstateNode = node.SelectNodes("//DMSstate");
        //if (DMSstateNode == null) return false;
        foreach (XmlNode statenode in DMSstateNode)
        {
          string state;
          StateToDMSValue value = new StateToDMSValue();
          //StateToHiResDMSValue hiResValue = new StateToHiResDMSValue();
          var stateInnerText = statenode.Attributes.GetNamedItem("state").InnerText;
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
                  case "ECG12":
                    value.ECG12 = double.Parse(valueInnerText, CultureInfo.InvariantCulture);
                    break;
                  case "DiaphragmGraph":
                    value.DiaphragmGraph = double.Parse(valueInnerText, CultureInfo.InvariantCulture);
                    break;
                  case "ESOTEMP":
                    value.ESOTEMP = double.Parse(valueInnerText, CultureInfo.InvariantCulture);
                    break;
                  case "ECG78":
                    value.ECG78 = double.Parse(valueInnerText, CultureInfo.InvariantCulture);
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
            StateToDMSMap.Add(state, value);
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
