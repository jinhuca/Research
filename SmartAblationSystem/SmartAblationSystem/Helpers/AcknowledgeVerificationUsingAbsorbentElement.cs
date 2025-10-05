using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAblationSystem.Helpers
{
    /// <summary>
    /// This class acknowledge verification using absorbent element
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class AcknowledgeVerificationUsingAbsorbentElement : IAcknowledgeVerificationUsingAbsorbentElement
    {
        /// <summary>
        /// Constructor that initialize the acknowledge verification using absorbent element.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public AcknowledgeVerificationUsingAbsorbentElement()
        {
        }
        /// <summary>
        /// return a value indicating whether is result null
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsResulutNull(List<int> results)
        {
            int product = 1; 

            if (results != null)
            {
                foreach (int i in results)
                {
                    product = product * i;
                }
            }
            else
            {
                product = 0;
            }

            return (product == 0);
        }
    }
}
