using DemoDB2.Models.Strategies;
using DemoDB2.Models.StrategyPattern;
using DemoDB2.Models.TemplateMethod;
using System.Web.Mvc;

namespace DemoDB2.Controllers
{
    public class BookingController : Controller
    {

        //Strategy
        public ActionResult CalculatePrice(int roomId, bool isMember, bool isHoliday)
        {
            decimal basePrice = 100m; // Lấy giá từ DB trong thực tế

            IPricingStrategy pricingStrategy;

            if (isHoliday)
            {
                pricingStrategy = new HolidayPricingStrategy();
            }
            else if (isMember)
            {
                pricingStrategy = new MemberPricingStrategy();
            }
            else
            {
                pricingStrategy = new RegularPricingStrategy();
            }

            RoomBookingContext bookingContext = new RoomBookingContext(pricingStrategy);
            decimal finalPrice = bookingContext.GetFinalPrice(basePrice);

            return Json(new { FinalPrice = finalPrice }, JsonRequestBehavior.AllowGet);
        }
        //Template Method 
        public ActionResult BookRoom(bool isVIP)
        {
            RoomBookingTemplate booking;

            if (isVIP)
            {
                booking = new VIPRoomBooking();
            }
            else
            {
                booking = new RegularRoomBooking();
            }

            booking.BookRoom(); // Gọi Template Method

            return Json(new { Message = "Đặt phòng thành công!" }, JsonRequestBehavior.AllowGet);
        }
    }
}
