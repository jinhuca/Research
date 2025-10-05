namespace WarningMessagesManager
{
    public class WarningMessagesManagerEnumeration
    {

        /// <summary>
        /// Defines message type
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public enum MessageType
        {
            SYSTEM = 0,
            WARNING = 1,
            ERROR = 2
        }
    }
}