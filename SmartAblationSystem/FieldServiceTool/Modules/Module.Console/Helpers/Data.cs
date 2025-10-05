using System;
using System.Collections.Generic;
using DataAccessLayer;
using static Communication.CanBusMessageDefinition;

namespace Module.Console.Helpers
{
    /// <summary>
    /// This class is for the Data Model, it allows access to the DataAccess.
    /// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class Data
    {
        public static readonly IDictionary<int, MessageStateId> StateToMessageStateIdDict =
            new Dictionary<int, MessageStateId>()
            {
              {1, MessageStateId.CAN_ID_STATE_IDLE},
              {2, MessageStateId.CAN_ID_STATE_READY},
              {3, MessageStateId.CAN_ID_STATE_INFLATION},
              {4, MessageStateId.CAN_ID_STATE_TRANSITION},
              {5, MessageStateId.CAN_ID_STATE_ABLATION},
              {6, MessageStateId.CAN_ID_STATE_THAWING},
              {7, MessageStateId.CAN_ID_STATE_EXCEPTION}
            };

        public static readonly IDictionary<MessageStateId, int> MessageStateIdToStateDict =
            new Dictionary<MessageStateId, int>()
            {
              {MessageStateId.CAN_ID_STATE_IDLE, 1},
              {MessageStateId.CAN_ID_STATE_READY, 2},
              {MessageStateId.CAN_ID_STATE_INFLATION, 3},
              {MessageStateId.CAN_ID_STATE_TRANSITION, 4},
              {MessageStateId.CAN_ID_STATE_ABLATION, 5},
              {MessageStateId.CAN_ID_STATE_THAWING, 6},
              {MessageStateId.CAN_ID_STATE_EXCEPTION, 7}
            };

        /// <summary>
        /// Initializes a new instance of the Data Model class and its properties
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public Data(ICacheableDataAccess dataAccess)
        {
            this.DataAccess = dataAccess;
        }

        /// <summary>
        /// Gets or sets a DataAccess value.
        /// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public ICacheableDataAccess DataAccess { get; set; } = null;

        internal int CalculateRisk(double weight, double height, int gender, DateTime dateOfBirth)
        {
            throw new NotImplementedException();
        }
    }
}