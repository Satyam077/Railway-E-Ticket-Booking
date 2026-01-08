using MediatR;
using MongoDB.Driver;
using RailwayTicketBooking.Domain.Entities;
using RailwayTicketBooking.Infrastructure;

namespace RailwayTicketBooking.Application.Bookings.Queries.Handlers
{
    public class GetBookingByPNRHandler : IRequestHandler<GetBookingByPNRQuery, Booking>
    {
        private readonly MongoDbContext _context;

        public GetBookingByPNRHandler(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<Booking> Handle(GetBookingByPNRQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.PNR))
                return null;

            return await _context.Bookings
                .Find(b => b.PNR == request.PNR.ToUpper().Trim())
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}

