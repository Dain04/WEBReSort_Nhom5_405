namespace DemoDB2.Models.TemplateMethod
{
    public abstract class RoomBookingTemplate
    {
        public void BookRoom()
        {
            SelectRoom();
            ProcessPayment();
            SendConfirmation();
        }

        protected abstract void SelectRoom();
        protected abstract void ProcessPayment();
        protected virtual void SendConfirmation()
        {
            System.Diagnostics.Debug.WriteLine("🔔 Gửi email xác nhận đặt phòng...");
        }
    }
}
