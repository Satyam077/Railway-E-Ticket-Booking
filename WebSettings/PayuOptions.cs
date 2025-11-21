namespace Railway_Ticket_Booking.WebSettings
{
    public class PayuOptions
    {
        public string Key { get; set; }
        public string Salt { get; set; }
        public string SuccessUrl { get; set; }
        public string FailureUrl { get; set; }
        public bool TestMode { get; set; } = true;
    }

}
