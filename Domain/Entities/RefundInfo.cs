namespace RailwayTicketBooking.Domain.Entities
{
    public class RefundInfo
    {
        public string PNR { get; set; }
        public decimal RefundAmount { get; set; }
        public int ProcessingDays { get; set; }
    }

    public class RecommendedTrip
    {
        public string FromStation { get; set; }
        public string FromStationId { get; set; }
        public string ToStation { get; set; }
        public string ToStationId { get; set; }
    }
}
