namespace Railway_Ticket_Booking.Domain.Entities
{
    public class FareBreakdown
    {
        public decimal BaseFarePerPassenger { get; set; }
        public decimal SuperfastChargePerPassenger { get; set; }
        public decimal TotalBaseAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal AmountAfterDiscount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal ServiceCharge { get; set; }
        public decimal TotalAmount { get; set; }
        public double Distance { get; set; }
    }
}
