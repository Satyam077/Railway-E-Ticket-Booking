using MediatR;
using Railway_Ticket_Booking.Domain.Entities;
using Railway_Ticket_Booking.Domain.Enums;

namespace Railway_Ticket_Booking.Application.Bookings.Queries
{
    public class GetAllBookingsQuery : IRequest<List<Booking>>
    {
        // Optional filter parameters
        public bool? IsActive { get; set; }
        public BookingStatus? Status { get; set; }

        public GetAllBookingsQuery(bool? isActive = null, BookingStatus? status = null)
        {
            IsActive = isActive;
            Status = status;
        }
    }
}

