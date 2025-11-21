using MediatR;
using Railway_Ticket_Booking.Domain.Entities;

namespace Railway_Ticket_Booking.Application.Bookings.Queries
{
    public class GetUserBookingsQuery : IRequest<List<Booking>>
    {
        public string UserId { get; set; }

        public GetUserBookingsQuery(string userId)
        {
            UserId = userId;
        }
    }
}

