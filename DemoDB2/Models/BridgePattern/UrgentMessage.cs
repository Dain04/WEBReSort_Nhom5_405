using DemoDB2.Models.BridgePattern;
using System;

namespace DemoDB2.Models.BridgePattern
{
    public class UrgentMessage : Message
    {
        public UrgentMessage(IMessageSender messageSender) : base(messageSender) { }

        public override void Send(string message)
        {
            Console.WriteLine("[‼️ KHẨN CẤP]");
            _messageSender.SendMessage(message);
        }
    }
}
