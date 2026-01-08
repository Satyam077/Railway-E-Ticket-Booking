using MediatR;
using RailwayTicketBooking.Domain.Entities;

namespace RailwayTicketBooking.Application.Bookings.Queries
{
    public class GetBookingByIdQuery : IRequest<Booking>
    {
        public string Id { get; set; }

        public GetBookingByIdQuery(string id)
        {
            Id = id;
        }
    }
}

