using System;
using System.Xml;
using Log = LogSystem.LogService;

namespace ConsoleCommSimulator.Configuration
{
  public class CatheterInfoMessageConfig
  {
    // add constants
    private string CATHETER_WAIT = "CatheterWait";
    private string CATHETER_ID = "CatheterId";
    private string CATHETER_SN = "CatheterSn";
    private string CATHETER_MONTH = "CatheterMonth";
    private string CATHETER_LOT = "CatheterLot";
    private string CATHETER_DAY = "CatheterDay";
    private string CATHETER_YEAR = "CatheterYear";
    private string FIRST_CATHETER_HOUR = "FirstUseCatheterHour";
    private string FIRST_CATHETER_MONTH = "FirstUseCatheterMonth";
    private string FIRST_CATHETER_DAY = "FirstUseCatheterDay";
    private string FIRST_CATHETER_YEAR = "FirstUseCatheterYear";
    private string CATHETER_INJECTIONS = "CatheterInjections";

    //private int _catheterId;
    private int _catheterSn;
    private int _catheterLot;
    private int _catheterMonth;
    private int _catheterDay;
    private int _catheterYear;
    private int _firstCatheterHour;
    private int _firstCatheterMonth;
    private int _firstCatheterDay;
    private int _firstCatheterYear;
    private int _catheterInjections;

    public int CatheterWait { get; set; } = 100; // default is 100 mili sec
    public byte[] CatheterData { get; set; } = new byte[8];
    public byte[] FirstUseCatheterData { get; set; } = new byte[8];
    public int CatheterId { get; set; } // public to tell FM if it's hiflow or not

    public int CatheterSn => _catheterSn;
    public int CatheterLot => _catheterLot;

    public bool IsNewCatheter => (_firstCatheterHour == 0 && _firstCatheterMonth == 0 && _firstCatheterDay == 0 && _firstCatheterYear == 0);

    public bool Parse(XmlNode node)
    {
      if (node == null) return false;

      return ParseSettings(node);
    }
    // function returns false if any exception occurs, only return true if values are valid
    private bool ParseSettings(XmlNode node)
    {
      try
      {
        if (node?.SelectSingleNode(CATHETER_WAIT) != null)
        {
          XmlNode internalNode = node?.SelectSingleNode(CATHETER_WAIT);
          int.TryParse(internalNode.InnerText, out int value);
          CatheterWait = value;
        }
        if (node?.SelectSingleNode(CATHETER_ID) != null)
        {
          XmlNode internalNode = node?.SelectSingleNode(CATHETER_ID);
          int.TryParse(internalNode.InnerText, out int value);
          CatheterId = value;
           
        }
        if (node?.SelectSingleNode(CATHETER_SN) != null)
        {
          XmlNode internalNode = node?.SelectSingleNode(CATHETER_SN);
          int.TryParse(internalNode.InnerText, out int value);
          _catheterSn = value;
        }
        if (node?.SelectSingleNode(CATHETER_LOT) != null)
        {
          XmlNode internalNode = node?.SelectSingleNode(CATHETER_LOT);
          int.TryParse(internalNode.InnerText, out int value);
          _catheterLot = value;
        }
        if (node?.SelectSingleNode(CATHETER_MONTH) != null)
        {
          XmlNode internalNode = node?.SelectSingleNode(CATHETER_MONTH);
          int.TryParse(internalNode.InnerText, out int value);
          _catheterMonth = value;
        }
        if (node?.SelectSingleNode(CATHETER_DAY) != null)
        {
          XmlNode internalNode = node?.SelectSingleNode(CATHETER_DAY);
          int.TryParse(internalNode.InnerText, out int value);
          _catheterDay = value;
        }
        if (node?.SelectSingleNode(CATHETER_YEAR) != null)
        {
          XmlNode internalNode = node?.SelectSingleNode(CATHETER_YEAR);
          int.TryParse(internalNode.InnerText, out int value);
          _catheterYear = value;
        }
        if (node?.SelectSingleNode(FIRST_CATHETER_HOUR) != null)
        {
          XmlNode internalNode = node?.SelectSingleNode(FIRST_CATHETER_HOUR);
          int.TryParse(internalNode.InnerText, out int value);
          _firstCatheterHour = value;
        }
        if (node?.SelectSingleNode(FIRST_CATHETER_MONTH) != null)
        {
          XmlNode internalNode = node?.SelectSingleNode(FIRST_CATHETER_MONTH);
          int.TryParse(internalNode.InnerText, out int value);
          _firstCatheterMonth = value;
        }
        if (node?.SelectSingleNode(FIRST_CATHETER_DAY) != null)
        {
          XmlNode internalNode = node?.SelectSingleNode(FIRST_CATHETER_DAY);
          int.TryParse(internalNode.InnerText, out int value);
          _firstCatheterDay = value;
        }
        if (node?.SelectSingleNode(FIRST_CATHETER_YEAR) != null)
        {
          XmlNode internalNode = node?.SelectSingleNode(FIRST_CATHETER_YEAR);
          int.TryParse(internalNode.InnerText, out int value);
          _firstCatheterYear = value;
        }
        if (node?.SelectSingleNode(CATHETER_INJECTIONS) != null)
        {
          XmlNode internalNode = node?.SelectSingleNode(CATHETER_INJECTIONS);
          int.TryParse(internalNode.InnerText, out int value);
          _catheterInjections = value;
        }

      }
      catch (System.Xml.XPath.XPathException xe)
      {
        Log.LogException(xe);
        return false;
      }
      catch (OverflowException oe)
      {
        Log.LogException(oe);
        return false;
      }
      catch (FormatException fe1)
      {
        Log.LogException(fe1);
      }

      CatheterData[0] = (byte) (CatheterId & 0xFF);
      CatheterData[1] = (byte) (_catheterSn & 0xFF);
      CatheterData[2] = (byte) ((_catheterLot >> 8) & 0xFF);
      CatheterData[3] = (byte) (_catheterLot & 0xFF);
      CatheterData[4] = (byte) (_catheterMonth & 0xFF);
      CatheterData[5] = (byte) (_catheterDay  & 0xFF);
      CatheterData[6] = (byte) ((_catheterYear >> 8) & 0xFF);
      CatheterData[7] = (byte) (_catheterYear & 0xFF);
      FirstUseCatheterData[0] = (byte) (_firstCatheterHour & 0xFF);
      FirstUseCatheterData[1] = (byte) (_firstCatheterMonth & 0xFF);
      FirstUseCatheterData[2] = (byte) (_firstCatheterDay & 0xFF);
      FirstUseCatheterData[3] = (byte) ((_firstCatheterYear >> 8) & 0xFF);
      FirstUseCatheterData[4] = (byte) (_firstCatheterYear & 0xFF);
      FirstUseCatheterData[5] = (byte) ((_catheterInjections >> 8) & 0xFF);
      FirstUseCatheterData[6] = (byte) (_catheterInjections & 0xFF);
      
      return true;
    }

  }
}
