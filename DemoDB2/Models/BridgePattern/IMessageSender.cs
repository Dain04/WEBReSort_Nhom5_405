using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoDB2.Models.BridgePattern
{
     public interface IMessageSender
    {
        //Gửi giao diện thông báo
       void SendMessage(string message);
    }
}
