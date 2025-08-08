using DemoDB2.Models.VisitorPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoDB2.Services.VisitorParttern
{
    public interface IPhongVisitor
    {
        void Visit(StandardPhong phong);
        void Visit(VIPPhong phong);
    }
}
