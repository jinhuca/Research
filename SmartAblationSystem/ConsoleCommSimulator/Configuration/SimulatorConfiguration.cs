using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using ConsoleCommSimulator.Interfaces;
using Log = LogSystem.LogService;

namespace ConsoleCommSimulator.Configuration
{
  public class SimulatorConfiguration : ISimulatorConfiguration
  {
  private static readonly string _configDirectory = "Configuration";
  private static readonly string _configurationPath = Path.Combine(_configDirectory, "CanBusSimulatorConfig.xml");
  private static readonly string _TCconfigurationPath = Path.Combine(_configDirectory, "TCDataConfiguration.xml");
  private static readonly string _rootNode = "CanBusSimulatorConfiguration";
  private static readonly string _TCrootNode = "TCSimulatorConfiguration";

  private XmlDocument _configurationDocument;
    private XmlDocument _TCconfigurationDocument;

    private Dictionary<string, XmlNode> _loadedSections = new Dictionary<string, XmlNode>();

    public void LoadConfiguration()
    {
      _configurationDocument = LoadConfigurationFromFile(_configurationPath);
      _TCconfigurationDocument = LoadConfigurationFromFile(_TCconfigurationPath);
    }

    public List<string> GetTCDataList()
    {
      // this function goes through all TC datasets in the TCDataConfiguration.xml file and displays their name attribute 
      List<string> tcDataConfigNames = new List<string>();

      try
      {
        XmlNodeList tcDataConfigNodes = _TCconfigurationDocument.SelectNodes($"{_TCrootNode}/TCDataConfig");

        foreach (XmlNode tcDataConfigNode in tcDataConfigNodes)
        {
          XmlAttribute nameAttribute = tcDataConfigNode.Attributes["name"];
          if (nameAttribute != null)
          {
            tcDataConfigNames.Add(nameAttribute.Value);
          }
        }
      }
      catch (Exception ex)
      {
        // Handle any exceptions that might occur during XML parsing or node selection.
        Log.LogException(ex);
        return new List<string>();
      }

      return tcDataConfigNames;

    }

    public XmlNode LoadConfigurationSection(string sectionId, string name = "default")
    {
      if (_configurationDocument == null) return null;

      try
      {
        var sectionKey = $"{sectionId}:{name}";

        if (_loadedSections.TryGetValue(sectionKey, out var cachedNode))
        {
          return cachedNode;
        }
        else
        {
          XmlNode node;

          if (sectionId == "TCData" || sectionId == "TCFITData" || sectionId == "TCDataInterval")
          {
            var TCNode = _TCconfigurationDocument.SelectSingleNode($"{_TCrootNode}/TCDataConfig[@name='{name}']/{sectionId}");
            node = TCNode;
          }
          else
          {
            node = _configurationDocument.SelectSingleNode($"{_rootNode}/{sectionId}");
          }

          _loadedSections[sectionKey] = node;
          return node;
        }
      }
      catch (Exception ex)
      {
        Log.LogException(ex);
        Log.LogInfo("SECTION ID: "+sectionId);
      }

      return null;
    }
    private XmlDocument LoadConfigurationFromFile(string path)
    {
      var document = new XmlDocument();
      try
      {
        document.Load(path);
      }
      catch (Exception ex)
      {
        Log.LogException(ex);
        Log.LogInfo("Exception occured when loading file:"+path);
      }

      return document;
    }
  }
}
