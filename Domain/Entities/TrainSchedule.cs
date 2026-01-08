using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace RailwayTicketBooking.Domain.Entities
{
    public class TrainSchedule
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string TrainId { get; set; }
        public string RouteId { get; set; }

        // Instead of storing a fixed date, store DAYS the train runs on
        public List<DayOfWeek> RunsOn { get; set; } = new List<DayOfWeek>();

        // Static schedule (no date)
        public List<ScheduleStation> Stations { get; set; } = new List<ScheduleStation>();

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class ScheduleStation
    {
        public string StationId { get; set; }
        public int StationOrder { get; set; }

        public TimeSpan ArrivalTime { get; set; }
        public TimeSpan DepartureTime { get; set; }

        public int DayOffset { get; set; } = 0;

        // Halt in minutes
        public int HaltMinutes { get; set; }

        public bool IsMajorStation { get; set; }
        public string PlatformNumber { get; set; }

        // Optional runtime values (not required for template)
        [BsonIgnore]
        public DateTime? ActualArrival { get; set; }

        [BsonIgnore]
        public DateTime? ActualDeparture { get; set; }
    }

    //public class TrainSchedule
    //{
    //    [BsonId]
    //    [BsonRepresentation(BsonType.ObjectId)]
    //    public string Id { get; set; }

    //    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    //    public Guid ScheduleId { get; set; } = Guid.NewGuid();

    //    [Required]
    //    public string TrainId { get; set; }

    //    [Required]
    //    public string RouteId { get; set; }

    //    public DateTime DepartureDate { get; set; } = DateTime.Now;

    //    public DateTime ArrivalDate { get; set; } = DateTime.Now;

    //    public List<ScheduleStation> Stations { get; set; } = new List<ScheduleStation>();

    //    public bool IsActive { get; set; } = true;

    //    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    //    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    //    // Navigation properties
    //    public List<string> BookingIds { get; set; } = new List<string>();
    //}

    //public class ScheduleStation
    //{
    //    public string StationId { get; set; }

    //    public int StationOrder { get; set; }

    //    public DateTime ScheduledArrival { get; set; } = DateTime.Now;

    //    public DateTime ScheduledDeparture { get; set; } = DateTime.Now; 

    //    public DateTime? ActualArrival { get; set; }

    //    public DateTime? ActualDeparture { get; set; }

    //    public TimeSpan HaltDuration { get; set; }

    //    public bool IsMajorStation { get; set; }

    //    public string PlatformNumber { get; set; }

    //    // Helper property for form binding
    //    [BsonIgnore]
    //    public int HaltMinutes
    //    {
    //        get => (int)HaltDuration.TotalMinutes;
    //        set => HaltDuration = TimeSpan.FromMinutes(value);
    //    }
    //}
}
