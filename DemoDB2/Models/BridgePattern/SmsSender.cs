using System;

namespace DemoDB2.Models.BridgePattern
{
    public class SmsSender : IMessageSender
    {
        // Gửi sms
        public void SendMessage(string message)
        {
            Console.WriteLine("📱 Gửi SMS: " + message);
        }
    }
}
