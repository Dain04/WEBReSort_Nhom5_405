using DemoDB2.Models.BridgePattern;

namespace DemoDB2.Models.BridgePattern
{
    public abstract class Message
    {
        protected IMessageSender _messageSender;

        public Message(IMessageSender messageSender)
        {
            _messageSender = messageSender;
        }

        public abstract void Send(string message);
    }
}
