using System;

namespace DemoDB2.Models.BridgePattern
{
    public class PushSender : IMessageSender
    {
        public void SendMessage(string message)
        {
            Console.WriteLine("🔔 Gửi Push Notification: " + message);
        }
    }
}
