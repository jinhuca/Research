using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAblationSystem.Models
{
    public class CaseCalendar
    {
        public CaseCalendar(string caseMon, string caseMonValue, string caseTue, string caseTueValue, string caseWed, string caseWedValue,
                 string caseThu, string caseThuValue, string caseFri, string caseFriValue, string caseSat, string caseSatValue, string caseSun, string caseSunValue)
        {
            CaseMon = caseMon;
            CaseTue = caseTue;
            CaseWed = caseWed;
            CaseThu = caseThu;
            CaseFri = caseFri;
            CaseSat = caseSat;
            CaseSun = caseSun;
            CaseMonValue = caseMonValue;
            CaseTueValue = caseTueValue;
            CaseWedValue = caseWedValue;
            CaseThuValue = caseThuValue;
            CaseFriValue = caseFriValue;
            CaseSatValue = caseSatValue;
            CaseSunValue = caseSunValue;

        }
        public string CaseMon { get; set; }
        public string CaseTue { get; set; }

        public string CaseWed { get; set; }

        public string CaseThu { get; set; }

        public string CaseFri { get; set; }

        public string CaseSat { get; set; }

        public string CaseSun { get; set; }

        public string CaseMonValue { get; set; }
        public string CaseTueValue { get; set; }

        public string CaseWedValue { get; set; }

        public string CaseThuValue { get; set; }

        public string CaseFriValue { get; set; }

        public string CaseSatValue { get; set; }

        public string CaseSunValue { get; set; }


    }
}
