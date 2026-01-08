namespace RailwayTicketBooking.Domain.Entities
{
    public class CancellationResult
    {
        public string BookingId { get; set; }
        public DateTime CancellationTime { get; set; }
        public bool IsCancellable { get; set; }
        public string Reason { get; set; }
        public decimal OriginalAmount { get; set; }
        public decimal BaseFare { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal ServiceCharge { get; set; }
        public decimal CancellationCharge { get; set; }
        public decimal RefundAmount { get; set; }
        public decimal RefundPercentage { get; set; }
        public int ProcessingDays { get; set; }
        public RefundBreakdown RefundBreakdown { get; set; }
    }

    public class RefundBreakdown
    {
        public decimal BaseFare { get; set; }
        public decimal CancellationCharge { get; set; }
        public decimal RefundableFare { get; set; }
        public decimal TaxRefund { get; set; }
        public decimal ServiceChargeRefund { get; set; }
        public decimal TotalRefund { get; set; }
    }
}
