using DemoDB2.Models.VisitorPattern;
using Newtonsoft.Json;

namespace DemoDB2.Services.VisitorParttern
{
    public class JsonExportVisitor : IPhongVisitor
    {
        public string JsonResult { get; private set; }

        public void Visit(StandardPhong phong)
        {
            JsonResult = JsonConvert.SerializeObject(phong);
        }

        public void Visit(VIPPhong phong)
        {
            JsonResult = JsonConvert.SerializeObject(phong);
        }
    }
}
