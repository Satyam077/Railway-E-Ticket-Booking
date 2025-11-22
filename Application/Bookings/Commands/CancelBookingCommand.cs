using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Railway_Ticket_Booking.Application.Bookings.Commands
{
    public class CancelBookingCommand : IRequest<CancelBookingResult>
    {
        [Required]
        public string BookingId { get; set; }

        [Required]
        public string UserId { get; set; }

        [StringLength(500)]
        public string CancellationReason { get; set; }
    }

    public class CancelBookingResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public decimal RefundAmount { get; set; }
        public int ProcessingDays { get; set; }
        public string RefundTransactionId { get; set; }
    }
}

