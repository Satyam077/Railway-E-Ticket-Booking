using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace RailwayTicketBooking.Domain.Entities
{
    public class Station
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid StationId { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(10)]
        public string StationCode { get; set; } // e.g., "NDLS", "PNBE"

        [Required]
        [StringLength(100)]
        public string StationName { get; set; } // e.g., "New Delhi", "Patna Junction"

        [StringLength(50)]
        public string City { get; set; }

        [StringLength(50)]
        public string State { get; set; }

        [StringLength(50)]
        public string Country { get; set; } = "India";

        [Range(-90, 90)]
        public double Latitude { get; set; }

        [Range(-180, 180)]
        public double Longitude { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public List<string> RouteIds { get; set; } = new List<string>();

        // Helper properties
        [BsonIgnore]
        public string DisplayName => $"{StationName} ({StationCode})";

        [BsonIgnore]
        public string StatusText => IsActive ? "Active" : "Inactive";
    }
}