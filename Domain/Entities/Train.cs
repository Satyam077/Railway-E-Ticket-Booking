using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace RailwayTicketBooking.Domain.Entities
{
    public class Train
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid TrainId { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(10)]
        public string TrainNumber { get; set; } // e.g., "12043"

        [Required]
        [StringLength(100)]
        public string Name { get; set; } // e.g., "Shatabdi Express"

        [StringLength(50)]
        public string TrainType { get; set; } // Express, Superfast, Passenger, etc.

        public List<string> RouteIds { get; set; } = new List<string>();

        public List<TrainClass> Classes { get; set; } = new List<TrainClass>();

        public int TotalSeats { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public List<string> ScheduleIds { get; set; } = new List<string>();

        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid CreatedBy { get; set; } = Guid.NewGuid();

        // Helper properties
        [BsonIgnore]
        public string DisplayName => $"{TrainNumber} - {Name}";

        [BsonIgnore]
        public string StatusText => IsActive ? "Active" : "Inactive";
    }

    public class TrainClass
    {
        public string ClassName { get; set; } // AC First Class, AC 2 Tier, etc.

        public int SeatCount { get; set; }

        public decimal BasePrice { get; set; }

        public List<string> Amenities { get; set; } = new List<string>();
    }
}
