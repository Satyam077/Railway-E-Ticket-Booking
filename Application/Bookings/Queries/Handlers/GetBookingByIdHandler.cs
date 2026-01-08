using MediatR;
using MongoDB.Driver;
using RailwayTicketBooking.Domain.Entities;
using RailwayTicketBooking.Infrastructure;

namespace RailwayTicketBooking.Application.Bookings.Queries.Handlers
{
    public class GetBookingByIdHandler : IRequestHandler<GetBookingByIdQuery, Booking>
    {
        private readonly MongoDbContext _context;

        public GetBookingByIdHandler(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<Booking> Handle(GetBookingByIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.Bookings
                .Find(b => b.Id == request.Id)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}

