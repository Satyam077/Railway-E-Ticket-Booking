using MediatR;
using RailwayTicketBooking.Domain.Entities;

namespace RailwayTicketBooking.Application.Bookings.Queries
{
    public class GetBookingByPNRQuery : IRequest<Booking>
    {
        public string PNR { get; set; }
    }
}

