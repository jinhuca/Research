using System.Collections.Generic;
using System.Xml;

namespace ConsoleCommSimulator.Interfaces
{
  public interface ISimulatorConfiguration
  {
    void LoadConfiguration();
    XmlNode LoadConfigurationSection(string sectionId, string name = "default");
    List<string> GetTCDataList();
  }
}
