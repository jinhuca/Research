using static Communication.CanBusMessageDefinition;

namespace Console
{
    /// <summary>
    /// Represents the console finite state machine
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class ConsoleFiniteStateMachine
    {
        private static ConsoleFiniteStateMachine instance;

        private static MessageStateId currentState = MessageStateId.CAN_ID_STATE_IDLE;

        /// <summary>
        /// Creates console finite state machine class
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private ConsoleFiniteStateMachine()
        {
        }

        /// <summary>
        /// Gets the console state machine
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static ConsoleFiniteStateMachine Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ConsoleFiniteStateMachine();
                }

                return instance;
            }
        }

        /// <summary>
        /// Gets or sets the current state
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static MessageStateId CurrentState
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