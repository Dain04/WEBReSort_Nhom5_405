
using DemoDB2.Models.BridgePattern;
using System.Web.Mvc;

namespace DemoDB2.Controllers
{
    public class MessageController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult SendUrgentEmail()
        {
            Message message = new UrgentMessage(new EmailSender());
            message.Send("Hệ thống gặp lỗi nghiêm trọng!");
            return Content("✅ Gửi email khẩn cấp thành công!");
        }

        public ActionResult SendNormalSms()
        {
            Message message = new NormalMessage(new SmsSender());
            message.Send("Khách hàng mới đã đăng ký.");
            return Content("✅ Gửi SMS thông thường thành công!");
        }

        public ActionResult SendUrgentPush()
        {
            Message message = new UrgentMessage(new PushSender());
            message.Send("Tài khoản của bạn bị đăng nhập từ thiết bị lạ!");
            return Content("✅ Gửi Push Notification khẩn cấp thành công!");
        }
    }
}
