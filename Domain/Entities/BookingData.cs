namespace RailwayTicketBooking.Domain.Entities
{
    public class BookingData
    {
        public string TrainId { get; set; }
        public string ScheduleId { get; set; }
        public string TrainName { get; set; }
        public string TrainNumber { get; set; }
        public string SourceStationId { get; set; }
        public string DestinationStationId { get; set; }
        public string SourceStationName { get; set; }
        public string DestinationStationName { get; set; }
        public DateTime JourneyDate { get; set; }
        public string SelectedClass { get; set; }
        public decimal ClassFare { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public string Quota { get; set; }
    }
}
