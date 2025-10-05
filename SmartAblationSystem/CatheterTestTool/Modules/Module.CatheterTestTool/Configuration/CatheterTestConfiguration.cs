using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Xml;
using Module.Infrastructure.AppLog;

namespace Module.CatheterTestTool.Configuration
{
  public class CatheterTestConfiguration : ICatheterTestConfiguration
  {
    private const string ConfigFileName = "Configuration/CatheterTestConfig.xml";
    private const string CatheterTestConfigurationNodeName = "Configuration/CatheterTestConfig";
    private const string CatheterIdAttributeName = "catheterId";
    private const string IBPInflationSettingNodeName = "IBPInflationSetting";
    private const string IBPStableValueAttributeName = "stableValue";
    private const string IBPOffsetValueAttributeName = "offset";

    private const string TestCriteriaConfigNodeName = "TestCriteriaConfig";
    private const string TestCriteriaNodeName = "TestCriteria";
    private const string SensorAttributeName = "Sensor";
    private const string MinValueAttributeName = "Min";
    private const string MaxValueAttributeName = "Max";

    private IDictionary<int, IDictionary<string, ValidationCriteria>> _criteriaMap = new Dictionary<int, IDictionary<string, ValidationCriteria>>();

    private IDictionary<int, InflationIBPSetting> _inflationIbpSettingMap = new ConcurrentDictionary<int, InflationIBPSetting>();

    public CatheterTestConfiguration()
    {
      FieldServiceTrace.Log("Loading CatheterTestConfiguration ...");
      LoadConfiguration();
      FieldServiceTrace.Log("CatheterTestConfiguration Loaded.");
    }

    public void LoadConfiguration(string configPath = null)
    {

      XmlDocument xDoc = new XmlDocument();

      try
      {
        xDoc.Load(ConfigFileName);

        var configNodes = xDoc.SelectNodes(CatheterTestConfigurationNodeName);
        foreach (XmlNode configurationNode in configNodes)
        {
          var catheterId = Convert.ToInt32(configurationNode.Attributes?.GetNamedItem(CatheterIdAttributeName).Value);

          var ibpInflationSettingNode = configurationNode.SelectSingleNode(IBPInflationSettingNodeName);

          var targetValue = Convert.ToDouble(ibpInflationSettingNode?.Attributes?.GetNamedItem(IBPStableValueAttributeName).Value);
          var offset = Convert.ToDouble(ibpInflationSettingNode?.Attributes?.GetNamedItem(IBPOffsetValueAttributeName).Value);

          _inflationIbpSettingMap.Add(catheterId, new InflationIBPSetting(targetValue, offset));

          var testCriteriaConfigNode = configurationNode.SelectSingleNode(TestCriteriaConfigNodeName);

          var validationCriteria = ParseTestCriteriaConfiguation(testCriteriaConfigNode);
          _criteriaMap.Add(catheterId, validationCriteria);
        }
      }
      catch (Exception ex)
      {

      }
    }

    public IDictionary<string, ValidationCriteria> GetValidationCriteriaMap(int catheterId)
    {
      return _criteriaMap.ContainsKey(catheterId)
        ? _criteriaMap[catheterId]
        : new Dictionary<string, ValidationCriteria>();
    }

    public InflationIBPSetting GetInflationIBPSetting(int catheterId)
    {
      return _inflationIbpSettingMap.ContainsKey(catheterId)
        ? _inflationIbpSettingMap[catheterId]
        : new InflationIBPSetting(0, 0);
    }

    private IDictionary<string, ValidationCriteria> ParseTestCriteriaConfiguation(XmlNode testCriteriaConfigNode)
    {

      var validationCriteria = new Dictionary<string, ValidationCriteria>();

      if (testCriteriaConfigNode != null)
      {
        try
        {

          foreach (XmlNode node in testCriteriaConfigNode)
          {
            if (node.Name == TestCriteriaNodeName)
            {
              string sensor = node.Attributes?.GetNamedItem(SensorAttributeName).Value;
              double minValue = Convert.ToDouble(node.Attributes?.GetNamedItem(MinValueAttributeName).Value);
              double maxValue = Convert.ToDouble(node.Attributes?.GetNamedItem(MaxValueAttributeName).Value);

              validationCriteria.Add(sensor, new ValidationCriteria(sensor, minValue, maxValue));
            }
          }
        }
        catch (Exception ex)
        {
          FieldServiceTrace.LogException(ex);
        }
      }

      return validationCriteria;
    }

  }
}
