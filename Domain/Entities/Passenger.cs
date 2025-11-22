using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Railway_Ticket_Booking.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Railway_Ticket_Booking.Domain.Entities
{
    public class Passenger
    {
        [BsonRepresentation(BsonType.String)]
        public Guid PassengerId { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        public int Age { get; set; }

        [StringLength(10)]
        public string Gender { get; set; } // Male, Female, Other

        [StringLength(50)]
        public string IdProofType { get; set; } // Aadhar, PAN, Passport, etc.

        [StringLength(50)]
        public string IdProofNumber { get; set; }

        [StringLength(20)]
        public string BerthPreference { get; set; } // Upper, Middle, Lower, Window, Aisle

        [StringLength(50)]
        public string FoodPreference { get; set; } // Veg, Non-Veg, Jain, etc.

        public bool IsChild => Age < 12;

        public bool IsSenior => Age >= 60;

        [BsonIgnore]
        public string FullName => $"{FirstName} {LastName}";
    }
}
