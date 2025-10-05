namespace SmartAblationSystem.Models
{
    /// <summary>
    /// This class is the Engineering Data File Model
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class EngineeringDataFile
    {
        private string filename;
        private bool selected;

        /// <summary>
        /// Initializes a new instance of the Engineering Data File Model class and its properties
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public EngineeringDataFile()
        {
            filename = string.Empty;
            selected = false;
        }

        /// <summary>
        /// Initializes a new instance of the Engineering Data File Model class and its properties
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="filename">A string representing a filename.</param>
        /// <param name="selected">A boolean representing if a file is selected or not.</param>
        public EngineeringDataFile(string filename, bool selected)
        {
            this.filename = filename;
            this.selected = selected;
        }

        /// <summary>
        /// Gets or sets a FileName value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string Filename
        {
            get { return filename; }
            set { filename = value; }
        }

        /// <summary>
        /// Gets or sets a Selected value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool Selected
        {
            get { return selected; }
            set { selected = value; }
        }
    }
}