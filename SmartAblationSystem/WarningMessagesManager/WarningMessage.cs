namespace WarningMessagesManager
{
    /// <summary>
    /// This class represents a warning message.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class WarningMessage
    {
        private string message;
        private string crtMessage;
        private WarningMessagesManagerEnumeration.MessageType type;

        /// <summary>
        /// Constructor that receives a string message.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public WarningMessage(string message, string crtMessage, WarningMessagesManagerEnumeration.MessageType type = WarningMessagesManagerEnumeration.MessageType.SYSTEM, string errorType = "")
        {
            string[] _cryterionMessage = new string[] { string.Empty };

            if (crtMessage?.Length > 0)
            {
                if (crtMessage.Contains("-"))
                {
                    _cryterionMessage = crtMessage.Split('-');

                    int errorCodeLenght = _cryterionMessage[1].Length;

                    string errorCode = _cryterionMessage[1].Substring(0, errorCodeLenght);
                    string errorMessage = _cryterionMessage[1].Substring(errorCodeLenght);

                    this.CrtMessage = _cryterionMessage[0] + errorCode  + errorType + " " + errorMessage;
                }
            }
            else
            {
                this.CrtMessage = crtMessage;
            }



            this.message = message;
            this.type = type;

           
       }

        /// <summary>
        /// Property that sets and gets a message string.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string Message
        {
            get
            {
                return message;
            }
            set
            {
                message = value;
            }
        }

        /// <summary>
        /// Property that sets and gets the warning message's type.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public WarningMessagesManagerEnumeration.MessageType Type
        {
            get
            {
                return this.type;
            }
            set
            {
                this.type = value;
            }
        }
        /// <summary>
        /// Property that sets and gets the warning message's type.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string CrtMessage { get => crtMessage; set => crtMessage = value; }
    }
}