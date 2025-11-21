using MediatR;
using MongoDB.Driver;
using Railway_Ticket_Booking.Domain.Entities;
using Railway_Ticket_Booking.Infrastructure;

namespace Railway_Ticket_Booking.Application.Bookings.Queries.Handlers
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

