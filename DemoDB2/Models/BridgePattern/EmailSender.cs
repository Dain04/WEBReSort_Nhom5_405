using System;

namespace DemoDB2.Models.BridgePattern
{
    public class EmailSender : IMessageSender
    {
        //gửi Email
        public void SendMessage(string message)
        {
            Console.WriteLine("📧 Gửi Email: " + message);
        }
    }
}
