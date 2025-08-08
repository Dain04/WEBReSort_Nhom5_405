using System;

namespace DemoDB2.Models.BridgePattern
{
    public class NormalMessage : Message
    {
        public NormalMessage(IMessageSender messageSender) : base(messageSender) { }

        public override void Send(string message)
        {
            Console.WriteLine("[ℹ️ THÔNG BÁO]");
            _messageSender.SendMessage(message);
        }
    }
}
