using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Railway_Ticket_Booking.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Railway_Ticket_Booking.Domain.Entities
{
    public class Passengers
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid UserId { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(50)]
        public string? FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string? LastName { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [Phone]
        public string? PhoneNumber { get; set; }


        [Required]
        public string? AdharNumber { get; set; }

        [Required]
        public string? PasswordHash { get; set; }

        public UserRole Role { get; set; } = UserRole.Customer;

        public DateTime DateOfBirth { get; set; }

        [StringLength(10)]
        public string? Gender { get; set; } // Male, Female, Other

        [StringLength(200)]
        public string? Address { get; set; }

        [StringLength(50)]
        public string City { get; set; }

        [StringLength(50)]
        public string State { get; set; }

        [StringLength(10)]
        public string PinCode { get; set; }

        [StringLength(50)]
        public string Country { get; set; } = "India";

        public bool IsActive { get; set; } = true;

        public bool IsEmailVerified { get; set; } = false;

        public bool IsPhoneVerified { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastLoginAt { get; set; }

        // Navigation properties
        public List<string> BookingIds { get; set; } = new List<string>();

        // Helper properties
        [BsonIgnore]
        public string FullName => $"{FirstName} {LastName}";

        [BsonIgnore]
        public int Age => DateTime.Now.Year - DateOfBirth.Year;
    }
}
