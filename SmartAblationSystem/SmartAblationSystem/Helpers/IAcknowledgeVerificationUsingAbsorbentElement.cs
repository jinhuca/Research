using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAblationSystem.Helpers
{
    /// <summary>
    ///Interface of AcknowledgeVerificationUsingAbsorbentElement
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    internal interface IAcknowledgeVerificationUsingAbsorbentElement
    {
        bool IsResulutNull(List<int> results);
    }
}
