using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace RailwayTicketBooking.Domain.Entities
{
    public class Notification
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public Guid NotificationId { get; set; } = Guid.NewGuid();

        [Required]
        public string UserId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        [StringLength(1000)]
        public string Message { get; set; }

        [StringLength(50)]
        public string Type { get; set; } // Booking, Payment, Cancellation, General

        [StringLength(20)]
        public string Priority { get; set; } = "Normal"; // High, Normal, Low

        public bool IsRead { get; set; } = false;

        public bool IsEmailSent { get; set; } = false;

        public bool IsSMSSent { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ReadAt { get; set; }

        public string RelatedEntityId { get; set; } // BookingId, PaymentId, etc.

        [StringLength(50)]
        public string RelatedEntityType { get; set; } // Booking, Payment, etc.

        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
    }
}
