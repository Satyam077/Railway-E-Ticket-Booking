using MediatR;
using RailwayTicketBooking.Domain.Entities;

namespace RailwayTicketBooking.Application.Bookings.Queries
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

