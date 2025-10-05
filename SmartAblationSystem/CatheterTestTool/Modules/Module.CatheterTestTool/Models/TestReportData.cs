using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Module.CatheterTestTool.Models
{
    [XmlRoot("TestReportData")]
    public class TestReportData
    {
        [XmlElement("Tester")]
        public string TesterName { get; set; }
        
        [XmlElement("TestTime")]
        public DateTime TestDate { get; set; }

        [XmlElement("CatheterInfo")]
        public CatheterInfoData CatheterInfo { get; set; }

        [XmlArray("Results")]
        [XmlArrayItem("Result")]
        public List<TestDataValidationResult> Results { get; set; }
    }
}
