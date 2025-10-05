using SmartFreezeApp.Services.Interfaces;

namespace SmartFreezeApp.Services
{
    public class MessageService : IMessageService
    {
        public string GetMessage()
        {
            return "Hello from the Message Service";
        }
    }
}
