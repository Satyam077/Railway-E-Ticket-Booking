using MediatR;
using Railway_Ticket_Booking.Domain.Entities;

namespace Railway_Ticket_Booking.Application.Bookings.Queries
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

