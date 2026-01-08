using MediatR;
using MongoDB.Driver;
using RailwayTicketBooking.Domain.Entities;
using RailwayTicketBooking.Domain.Enums;
using RailwayTicketBooking.Infrastructure;

namespace RailwayTicketBooking.Application.Bookings.Queries.Handlers
{
    public class GetAllBookingsHandler : IRequestHandler<GetAllBookingsQuery, List<Booking>>
    {
        private readonly MongoDbContext _context;

        public GetAllBookingsHandler(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<List<Booking>> Handle(GetAllBookingsQuery request, CancellationToken cancellationToken)
        {
            var filterBuilder = Builders<Booking>.Filter;
            var filter = filterBuilder.Empty;

            // Apply IsActive filter if provided
            if (request.IsActive.HasValue)
            {
                filter = filter & filterBuilder.Eq(b => b.IsActive, request.IsActive.Value);
            }

            // Apply Status filter if provided
            if (request.Status.HasValue)
            {
                filter = filter & filterBuilder.Eq(b => b.Status, request.Status.Value);
            }

            return await _context.Bookings
                .Find(filter)
                .SortByDescending(b => b.BookingDate)
                .ToListAsync(cancellationToken);
        }
    }
}

