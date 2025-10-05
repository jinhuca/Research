using System.Xml;
using Log = LogSystem.LogService;

namespace ConsoleCommSimulator.Configuration
{
  public class RTRMessageConfig
  {
    public byte[] RTR1Data { get; set; } = new byte[8];

    public bool Parse(XmlNode node)
    {
      if (node == null) return false;
      XmlNode internalNode;

      try
      {
        internalNode = node.SelectSingleNode("RTRVerification");

      }
      catch (System.Xml.XPath.XPathException xe)
      {
        Log.LogException(xe);
        return false;
      }
      // 

      var byteValue1 = ConfigUtils.ConvertIntStringToByteArray(internalNode?.Attributes?.GetNamedItem("value")?.InnerText, 16);

      if (byteValue1 != null )
      {
        // data should look like 09 6f 00 00 etc
        RTR1Data = byteValue1;

      }
    
      return true;
    }

  }
}
