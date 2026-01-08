using MediatR;
using RailwayTicketBooking.Domain.Entities;
using RailwayTicketBooking.Domain.Enums;

namespace RailwayTicketBooking.Application.Bookings.Queries
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

