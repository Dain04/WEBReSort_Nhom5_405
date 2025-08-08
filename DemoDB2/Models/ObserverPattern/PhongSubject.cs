using System.Collections.Generic;

namespace DemoDB2.Models.ObserverPattern
{
    public class PhongSubject
    {
        private List<IPhongObserver> observers = new List<IPhongObserver>(); // Khai báo danh sách observer

        public void Attach(IPhongObserver observer)
        {
            observers.Add(observer);
        }

        public void Detach(IPhongObserver observer)
        {
            observers.Remove(observer);
        }

        public void Notify(Phong phong)
        {
            foreach (var observer in observers)
            {
                observer.Update(phong);
            }
        }
    }
}
