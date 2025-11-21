using MediatR;
using MongoDB.Driver;
using Railway_Ticket_Booking.Domain.Entities;
using Railway_Ticket_Booking.Infrastructure;

namespace Railway_Ticket_Booking.Application.Bookings.Queries.Handlers
{
    public class GetUserBookingsHandler : IRequestHandler<GetUserBookingsQuery, List<Booking>>
    {
        private readonly MongoDbContext _context;

        public GetUserBookingsHandler(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<List<Booking>> Handle(GetUserBookingsQuery request, CancellationToken cancellationToken)
        {
            return await _context.Bookings
                .Find(b => b.UserId == request.UserId && b.IsActive)
                .SortByDescending(b => b.BookingDate)
                .ToListAsync(cancellationToken);
        }
    }
}

