using DataAccessLayer;
using System;

namespace SmartAblationSystem.Models
{
    /// <summary>
    /// This class is the Action Log Record Model
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class ActionLogRecord
    {
        private DateTime timestamp;
        private User user;
        private DataAccessLayer.Action action;

        /// <summary>
        /// Initializes a new instance of the Action Log Record Model class
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="timestamp">A DateTime object represengint an action log timestamp.</param>
        /// <param name="user">A User representing the user that performed an action.</param>
        /// <param name="action">An Action representing an action that the user performed.</param>
        public ActionLogRecord(DateTime timestamp, User user, DataAccessLayer.Action action)
        {
            this.timestamp = timestamp;
            this.user = user;
            this.action = action;
        }

        /// <summary>
        /// Returns a string representing the user's username
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string Username
        {
            get { return user?.UserName; }
        }

        /// <summary>
        /// Returns a DateTime representing the action's timestamp
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public DateTime Timestamp
        {
            get { return timestamp; }
        }

        /// <summary>
        /// Returns a string representing the action's description
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string Action
        {
            get
            {
                return action?.Description;
            }
        }
    }
}