
using System;
using System.Xml.Serialization;

namespace Module.CatheterTestTool.Models
{
    public enum TestResult
    {
        None,
        PASS,
        FAIL
    } 

    [Serializable]
    public class TestDataValidationResult
    {
        [XmlAttribute("sensor")]
        public string Sensor { get; set; }

        [XmlAttribute("value")]
        public double Value { get; set; }

        [XmlIgnore]
        public double[] Expected { get; set; }

        [XmlAttribute("expected")]
        public string ExpectedInString
        {
            get => Expected != null && Expected.Length == 2
                ? $"{Expected[0]}/{Expected[1]}"
                : String.Empty;
            set
            {
                // implement setter for serialzation/deserialization 
                var expectedArray = value.Split('/');
                try
                {
                    Expected = new [] { double.Parse(expectedArray[0]), double.Parse(expectedArray[1]) };
                }
                catch
                {
                    // ignored
                }
            }
        }

        [XmlAttribute("result")]
        public TestResult Result { get; set; }
    }

    public class TestDataValidationResults 
    {
        public TestDataValidationResult TC1 { get; set; }
        public TestDataValidationResult IBP { get; set; }
        public TestDataValidationResult OBP { get; set; }
        public TestDataValidationResult PT2 { get; set; }
        public TestDataValidationResult FM1 { get; set; }
        public TestDataValidationResult PT3 { get; set; }
        public TestDataValidationResult PT4 { get; set; }
    }
}
