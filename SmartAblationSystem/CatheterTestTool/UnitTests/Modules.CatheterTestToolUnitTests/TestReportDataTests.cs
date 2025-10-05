using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.XmlDiffPatch;
using Module.CatheterTestTool.Models;
using TestResult = Module.CatheterTestTool.Models.TestResult;

namespace Module.CatheterTestToolUnitTest
{
    [TestClass]
    public class TestReportDataTests
    {
        private string expectedTestReportData = "<TestReportData><Tester>Clark Zhang</Tester>" +
        "<TestTime>2022-06-22T12:28:30</TestTime>" + 
        "<CatheterInfo><ID>1</ID><SerialNumber>128</SerialNumber><Lot>1</Lot></CatheterInfo>" + 
        "<Results>" +
        "<Result sensor = \"TEMP\" value= \"-35\" expected=\"-30/-40\" result= \"PASS\" />" +
        "<Result sensor= \"IBP\" value= \"2.5\" expected=\"2.3/2.7\" result= \"PASS\" />" +
        "<Result sensor= \"OBP\" value= \"-20\" expected=\"-30/-10\" result= \"PASS\" />" +
        "<Result sensor= \"PT2\" value= \"608\" expected=\"600/800\" result= \"PASS\" />" +
        "<Result sensor= \"PT3\" value= \"34\" expected=\"30/40\" result= \"PASS\" />" +
        "<Result sensor= \"PT4\" value= \"23\" expected=\"20/30\" result= \"PASS\" />" +
        "<Result sensor= \"FM1\" value= \"8088\" expected=\"8100/8500\" result= \"FAIL\" />" +
        "</Results>" +
        "</TestReportData>";

        [TestMethod]
        public void SerializationTests()
        {
            var testReportData = new TestReportData();
            testReportData.TesterName = "Clark Zhang";
            testReportData.TestDate = DateTime.Parse("2022-06-22 12:28:30");
            testReportData.CatheterInfo = new CatheterInfoData() { ID=1, SerialNumber = 128, Lot = 1, CatheterExpirationDate = DateTime.Today + TimeSpan.FromDays(365)};
            testReportData.Results = new List<TestDataValidationResult>()
            {
                new TestDataValidationResult() {Sensor = CatheterTestConstants.SensorNameTC1, Value = -35d, Expected = new []{-30d,-40d}, Result = TestResult.PASS},
                new TestDataValidationResult() {Sensor = CatheterTestConstants.SensorNameIBP, Value = 2.5d, Expected = new []{2.3,2.7}, Result = TestResult.PASS},
                new TestDataValidationResult() {Sensor = CatheterTestConstants.SensorNameOBP, Value = -20d, Expected = new []{-30d,-10d}, Result = TestResult.PASS},
                new TestDataValidationResult() {Sensor = CatheterTestConstants.SensorNamePT2, Value = 608d, Expected = new []{600d,800d}, Result = TestResult.PASS},
                new TestDataValidationResult() {Sensor = CatheterTestConstants.SensorNamePT3, Value = 34d, Expected = new []{30d,40d}, Result = TestResult.PASS},
                new TestDataValidationResult() {Sensor = CatheterTestConstants.SensorNamePT4, Value = 23d, Expected = new []{20d,30d}, Result = TestResult.PASS},
                new TestDataValidationResult() {Sensor = CatheterTestConstants.SensorNameFM1, Value = 8088d, Expected = new []{8100d,8500d}, Result = TestResult.FAIL}
            };

            var catheterInfo = testReportData.CatheterInfo; 
            var serializer = new XmlSerializer(typeof(TestReportData));
            // TextWriter writer = new StreamWriter("MyTest.xml");
            StringWriter writer = new StringWriter();
            serializer.Serialize(writer, testReportData);
            
            var doc = new XmlDocument();
            doc.LoadXml(writer.ToString());

            var expectedDoc = new XmlDocument();
            expectedDoc.LoadXml(expectedTestReportData);

            var xmlDiff = new XmlDiff()
            {
                IgnoreChildOrder = true, IgnoreComments = true, IgnoreNamespaces = true, IgnoreWhitespace = true, 
                IgnoreDtd = true, IgnorePI = true, IgnorePrefixes = true, IgnoreXmlDecl = true
            };

            var diffWriter = new StringWriter(); 
            bool match = xmlDiff.Compare(expectedDoc, doc, XmlWriter.Create(diffWriter));
            if (!match)
            {
                Console.WriteLine(diffWriter.ToString());
            }

            Assert.IsTrue(match);
        }
    }
}
