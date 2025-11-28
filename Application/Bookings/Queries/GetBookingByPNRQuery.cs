using MediatR;
using Railway_Ticket_Booking.Domain.Entities;

namespace Railway_Ticket_Booking.Application.Bookings.Queries
{
    public class GetBookingByPNRQuery : IRequest<Booking>
    {
        public string PNR { get; set; }
    }
}

