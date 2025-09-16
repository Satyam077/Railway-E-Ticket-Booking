using MediatR;
using Railway_Ticket_Booking.Domain.Entities;

namespace Railway_Ticket_Booking.Application.TrainSchedules.Queries
{
    public class SearchTrainsQuery : IRequest<List<TrainSearchResult>>
    {
        public string FromStationId { get; set; } = string.Empty;
        public string ToStationId { get; set; } = string.Empty;
        public DateTime TravelDate { get; set; }
        public string Class { get; set; } = "All Classes";
        public string Quota { get; set; } = "GENERAL";
        public bool PersonWithDisability { get; set; } = false;
        public bool FlexibleWithDate { get; set; } = false;
        public bool TrainWithAvailableBerth { get; set; } = false;
        public bool RailwayPassConcession { get; set; } = false;
    }

    public class TrainSearchResult
    {
        public string TrainId { get; set; } = string.Empty;
        public string TrainNumber { get; set; } = string.Empty;
        public string TrainName { get; set; } = string.Empty;
        public string TrainType { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; } = DateTime.Now;
        public DateTime ArrivalTime { get; set; } = DateTime.Now;

        //public TimeSpan DepartureTime { get; set; }
        //public TimeSpan ArrivalTime { get; set; }
        public TimeSpan Duration { get; set; }
        public string FromStation { get; set; } = string.Empty;
        public string ToStation { get; set; } = string.Empty;
        public string FromStationCode { get; set; } = string.Empty;
        public string ToStationCode { get; set; } = string.Empty;
        public List<string> RunsOn { get; set; } = new List<string>(); // Days of week
        public List<TrainClassInfo> Classes { get; set; } = new List<TrainClassInfo>();
        public bool IsActive { get; set; } = true;
        public string ScheduleId { get; set; } = string.Empty;
    }

    public class TrainClassInfo
    {
        public string ClassName { get; set; } = string.Empty;
        public decimal Fare { get; set; }
        public int AvailableSeats { get; set; }
        public string Status { get; set; } = "Available"; // Available, Waiting List, RAC
    }
}
