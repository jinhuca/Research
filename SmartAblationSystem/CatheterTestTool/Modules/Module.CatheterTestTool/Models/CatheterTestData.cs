
using System;
using System.Xml.Serialization;

namespace Module.CatheterTestTool.Models
{
    public class CatheterTestData : ICloneable
    {
        public string TesterName { get; set; }
        public CatheterInfoData CatheterInfo { get; set; }
        public double TC  { get; set; }
        public double FM1 { get; set; }
        public double IBP { get; set; }
        public double OBP { get; set; }
        public double PT2 { get; set; }
        public double PT3 { get; set; }
        public double PT4 { get; set; }

        public object Clone()
        {
            return new CatheterTestData()
            {
                TesterName = this.TesterName, 
                TC = this.TC, FM1 = this.FM1,
                IBP = this.IBP, OBP = this.OBP,
                PT2 = this.PT2, PT3 = this.PT3, PT4 = this.PT4,
                CatheterInfo = new CatheterInfoData()
                {
                    ID = this.CatheterInfo?.ID ?? 0,
                    SerialNumber = this.CatheterInfo?.SerialNumber??0,
                    FirmwareVersion = this.CatheterInfo?.FirmwareVersion??0,
                    CatheterExpirationDate = this.CatheterInfo?.CatheterExpirationDate??DateTime.Today,
                    LastUseDate = this.CatheterInfo?.LastUseDate??DateTime.Today
                }
            };
        }
    }

    public class CatheterInfoData
    {
        [XmlElement]
        public int ID { get; set; }

        [XmlElement]
        public int SerialNumber { get; set; }

        [XmlElement]
        public int Lot { get; set; }

        [XmlIgnore]
        public int FirmwareVersion { get; set; }

        [XmlIgnore]
        public DateTime CatheterExpirationDate { get; set; }

        [XmlIgnore]
        public DateTime LastUseDate { get; set; }
    }
}
