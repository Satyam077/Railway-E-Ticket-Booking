using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Railway_Ticket_Booking.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Railway_Ticket_Booking.Domain.Entities
{
    public class Booking
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid BookingId { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(20)]
        public string PNR { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string TrainId { get; set; }

        [Required]
        public string ScheduleId { get; set; }

        [Required]
        public string SourceStationId { get; set; }

        [Required]
        public string DestinationStationId { get; set; }

        public DateTime JourneyDate { get; set; }

        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        public List<Passenger> Passengers { get; set; } = new List<Passenger>();

        public List<BookedSeat> Seats { get; set; } = new List<BookedSeat>();

        public decimal TotalAmount { get; set; }

        public decimal TaxAmount { get; set; }

        public decimal ServiceCharge { get; set; }

        public decimal FinalAmount { get; set; }

        public DateTime BookingDate { get; set; } = DateTime.UtcNow;

        public DateTime? CancellationDate { get; set; }

        [StringLength(500)]
        public string CancellationReason { get; set; }

        public decimal RefundAmount { get; set; }

        [StringLength(200)]
        public string ContactEmail { get; set; }

        [StringLength(15)]
        public string ContactPhone { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public string PaymentId { get; set; }

        // Helper methods
        [BsonIgnore]
        public int PassengerCount => Passengers?.Count ?? 0;

        [BsonIgnore]
        public bool IsCancellable => Status == BookingStatus.Confirmed && 
                                   JourneyDate > DateTime.Now.AddHours(4);
    }

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

    public class BookedSeat
    {
        public string SeatId { get; set; }

        public string PassengerId { get; set; }

        [StringLength(10)]
        public string CoachNumber { get; set; }

        [StringLength(10)]
        public string SeatNumber { get; set; }

        public SeatClass Class { get; set; }

        public decimal Price { get; set; }

        [BsonIgnore]
        public string FullSeatNumber => $"{CoachNumber}-{SeatNumber}";
    }
}
