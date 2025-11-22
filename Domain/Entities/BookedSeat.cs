using MongoDB.Bson.Serialization.Attributes;
using Railway_Ticket_Booking.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Railway_Ticket_Booking.Domain.Entities
{
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
