using static SmartAblationSystem.Helpers.Enumeration;

namespace SmartAblationSystem.Models
{
    /// <summary>
    /// This class is the Change Tank FSM Model
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class ChangeTankFSM
    {
        private static ChangeTankFSM instance;
        private static TankStates currentState = TankStates.Tank_Opened;

        /// <summary>
        /// Initializes a new instance of the Change Tank FSM Model class
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private ChangeTankFSM()
        {
        }

        /// <summary>
        /// Returns a ChangeTankFSM instance representing the Change Tank FSM
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static ChangeTankFSM Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ChangeTankFSM();
                }

                return instance;
            }
        }

        /// <summary>
        /// Returns a TankStates representing the current tank's state
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static TankStates CurrentState
        {
            get
            {
                return currentState;
            }
            set
            {
                currentState = value;
            }
        }
    }
}