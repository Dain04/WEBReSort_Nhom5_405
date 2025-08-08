namespace DemoDB2.Models.TemplateMethod
{
    public class VIPRoomBooking : RoomBookingTemplate
    {
        protected override void SelectRoom()
        {
            System.Diagnostics.Debug.WriteLine("🏨 Chọn phòng VIP với dịch vụ cao cấp...");
        }

        protected override void ProcessPayment()
        {
            System.Diagnostics.Debug.WriteLine("💰 Thanh toán qua tài khoản thành viên VIP...");
        }

        protected override void SendConfirmation()
        {
            System.Diagnostics.Debug.WriteLine("📱 Gửi SMS xác nhận đặt phòng VIP...");
        }
    }
}
