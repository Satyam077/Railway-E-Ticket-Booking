namespace RailwayTicketBooking.Domain.Entities
{
    public class PayuRefundResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string RefundTransactionId { get; set; }
        public string OriginalTransactionId { get; set; }
        public decimal RefundAmount { get; set; }
    }
}
