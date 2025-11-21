using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Railway_Ticket_Booking.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Railway_Ticket_Booking.Domain.Entities
{
    public class Payment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid PaymentId { get; set; } = Guid.NewGuid();

        [Required]
        public string BookingId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string TransactionId { get; set; }

        [StringLength(50)]
        public string GatewayTransactionId { get; set; }

        public PaymentMethod Method { get; set; }

        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        public decimal Amount { get; set; }

        public decimal GatewayCharges { get; set; }

        public decimal NetAmount { get; set; }

        [StringLength(10)]
        public string Currency { get; set; } = "INR";

        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        public DateTime? CompletedAt { get; set; }

        public DateTime? FailedAt { get; set; }

        [StringLength(500)]
        public string FailureReason { get; set; }

        [StringLength(100)]
        public string GatewayName { get; set; } // Razorpay, PayU, Paytm, etc.

        [StringLength(200)]
        public string GatewayResponse { get; set; }

        public RefundDetails Refund { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Helper properties
        [BsonIgnore]
        public bool IsSuccessful => Status == PaymentStatus.Completed;

        [BsonIgnore]
        public bool IsRefundable => IsSuccessful && Refund == null;
    }

    public class RefundDetails
    {
        public Guid RefundId { get; set; } = Guid.NewGuid();

        [StringLength(50)]
        public string RefundTransactionId { get; set; }

        public decimal RefundAmount { get; set; }

        public PaymentStatus RefundStatus { get; set; }

        public DateTime RefundInitiatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? RefundCompletedAt { get; set; }

        [StringLength(500)]
        public string RefundReason { get; set; }

        [StringLength(200)]
        public string RefundReference { get; set; }

        public int ProcessingDays { get; set; } = 5; // Default processing time
    }
}
