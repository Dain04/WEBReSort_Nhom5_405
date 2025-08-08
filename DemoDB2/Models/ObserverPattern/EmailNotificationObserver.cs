using DemoDB2.Models.ObserverPattern;
using DemoDB2.Models;
using System;
public class EmailNotificationObserver : IPhongObserver
{
    public void Update(Phong phong)
    {
        Console.WriteLine($"[Email] Gửi thông báo về trạng thái mới của phòng {phong.PhongID} đến khách hàng.");
    }
}
