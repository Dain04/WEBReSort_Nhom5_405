
using System.Web.Mvc;
namespace DemoDB2.Controllers
{
    public class NhanVienController : Controller
    {
        private readonly NhanVienFactory _nhanVienFactory = new NhanVienFactory();

        public ActionResult Index(int id)
        {
            var nhanVien = _nhanVienFactory.GetCustomer(id);

            return View(nhanVien);

        }
    }

}
