using System;

namespace SmartAblationSystem
{
    /// <summary>
    /// This class is the Views Event Args.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class ViewsEventArgs : EventArgs
    {
        private string viewName;
        private string viewID;

        /// <summary>
        /// Gets or sets a value indicating the View Name.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string ViewName
        {
            get
            {
                return this.viewName;
            }
            set
            {
                this.viewName = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the View ID.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string ViewID
        {
            get
            {
                return this.viewID;
            }
            set
            {
                this.viewID = value;
            }
        }
    }
}