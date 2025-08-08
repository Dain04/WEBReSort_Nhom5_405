using DemoDB2.Models.ObserverPattern;
using DemoDB2.Models;
using System;
public class PhongLoggerObserver : IPhongObserver
{
    public void Update(Phong phong)
    {
        Console.WriteLine($"[Logger] Phòng {phong.PhongID} đã cập nhật trạng thái: {phong.TinhTrang}");
    }
}
