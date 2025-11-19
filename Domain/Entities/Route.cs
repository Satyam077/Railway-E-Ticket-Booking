using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace Railway_Ticket_Booking.Domain.Entities
{
    public class Route
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid RouteId { get; set; } = Guid.NewGuid();

        [Required]
        public string SourceStationId { get; set; }

        [Required]
        public string DestinationStationId { get; set; }

        [Required]
        public List<RouteStation> Stations { get; set; } = new List<RouteStation>();

        public double TotalDistance { get; set; } // in kilometers

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public List<string> TrainIds { get; set; } = new List<string>();

        // Helper properties
        [BsonIgnore]
        public string DisplayName => $"{SourceStationId} → {DestinationStationId}";

        [BsonIgnore]
        public string StatusText => IsActive ? "Active" : "Inactive";

        [BsonIgnore]
        public int StationCount => Stations?.Count ?? 0;

        public bool IsDeleted { get; set; } = false;
    }

    public class RouteStation
    {
        public string StationId { get; set; }

        public int StationOrder { get; set; }
        public string PlatformNumber { get; set; }
        public int DayOffset { get; set; }

        public double DistanceFromSource { get; set; } // in kilometers

        public TimeSpan ArrivalTime { get; set; } = TimeSpan.Zero;

        public TimeSpan DepartureTime { get; set; } = TimeSpan.Zero;

        public TimeSpan HaltDuration { get; set; } = TimeSpan.Zero;

        public bool IsMajorStation { get; set; } = false;
        [BsonIgnore]
        public int HaltMinutes
        {
            get => (int)HaltDuration.TotalMinutes;
            set => HaltDuration = TimeSpan.FromMinutes(value);
        }

    }
}