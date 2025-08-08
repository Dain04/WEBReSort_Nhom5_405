using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DemoDB2.Models;

namespace DemoDB2.Models.ObserverPattern
{
    public interface IPhongObserver
    {
        void Update(Phong phong);
    }
}
