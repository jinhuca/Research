using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAblationSystem.Models
{
    /// <summary>
    /// This class is for log the action.
    /// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class ActionLog
    {
        private DateTime timestamp;
        private string username;
        private string action;

        /// <summary>
        /// Default Cosntructor
        /// </summary>
        public ActionLog()
        {

        }
        /// <summary>
        /// Constructor, initialize parameters
        /// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public ActionLog(DateTime timestamp, string username, string action)
        {
            this.timestamp = timestamp;
            this.username = username;
            this.action = action;
        }
        /// <summary>
        /// Gets/sets user name.
        /// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>      
        public string Username
        {
            get { return username; }
        }
        /// <summary>
        /// Gets time stamp.
        /// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public DateTime Timestamp
        {
            get { return timestamp; }
        }
        /// <summary>
        /// Gets value of action.
        /// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string Action
        {
            get
            {
                return action;
            }
        }
    }
}
