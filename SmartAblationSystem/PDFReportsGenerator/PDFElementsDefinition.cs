using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDFReportsGenerator
{
    /// <summary>
    /// Represents the PDF elements definition class
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class PDFElementsDefinition
    {
        /// <summary>
        /// Gets or sets element type.
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string ElementType { get; set; }

        /// <summary>
        /// Gets or sets element display name.
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string ElementDispalyName { get; set; }

        /// <summary>
        /// Gets or sets element value.
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public List<string> ElementValue { get; set; }
    }


    /// <summary>
    /// Represents the PDF items definition class
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class PDFItemsDefinition
    {

        /// <summary>
        /// Gets or sets item type.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string ItemType { get; set; }

        /// <summary>
        /// Gets or sets item name.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// Gets or sets item value.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public List<int> ItemValue { get; set; }
    }

    /// <summary>
    /// Represents the PDF elements table class
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class PDFElementsTable
    {
        /// <summary>
        /// Gets or sets element type
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string ElementType { get; set; }

        /// <summary>
        /// Gets or sets element display name
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string ElementDispalyName { get; set; }

        /// <summary>
        /// Gets or sets element value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string[][] ElementValue { get; set; }
    }
}
