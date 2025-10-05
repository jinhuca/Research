using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAblationSystem.Models
{
    public class CaseReport 
    {
        public CaseReport()
        {
        }
        public int caseyear { get; set; }
        public int casemonth { get; set; }
        public int caseday { get; set; }
        public int caseprocedureId { get; set; }
        public int caseablations { get; set; }

    }
}
