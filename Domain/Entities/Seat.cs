using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using RailwayTicketBooking.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace RailwayTicketBooking.Domain.Entities
{
    public class Seat
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid SeatId { get; set; } = Guid.NewGuid();

        [Required]
        public string TrainId { get; set; }

        [Required]
        public string CoachNumber { get; set; } // e.g., "A1", "B2"

        [Required]
        [StringLength(10)]
        public string SeatNumber { get; set; } // e.g., "1", "2A", "3B"

        public SeatClass Class { get; set; }

        public SeatStatus Status { get; set; } = SeatStatus.Available;

        public decimal Price { get; set; }

        public bool IsWindowSeat { get; set; } = false;

        public bool IsAisleSeat { get; set; } = false;

        public bool IsUpperBerth { get; set; } = false;

        public bool IsLowerBerth { get; set; } = false;

        public bool IsMiddleBerth { get; set; } = false;

        [StringLength(200)]
        public string Amenities { get; set; } // JSON string of amenities

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Helper properties
        [BsonIgnore]
        public string FullSeatNumber => $"{CoachNumber}-{SeatNumber}";
    }
}
