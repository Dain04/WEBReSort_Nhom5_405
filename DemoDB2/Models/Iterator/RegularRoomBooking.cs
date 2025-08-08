namespace DemoDB2.Models.TemplateMethod
{
    public class RegularRoomBooking : RoomBookingTemplate
    {
        protected override void SelectRoom()
        {
            System.Diagnostics.Debug.WriteLine("🛏 Chọn phòng tiêu chuẩn...");
        }

        protected override void ProcessPayment()
        {
            System.Diagnostics.Debug.WriteLine("💳 Thanh toán qua thẻ ngân hàng...");
        }
    }
}
