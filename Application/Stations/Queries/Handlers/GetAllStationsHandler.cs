using MediatR;
using MongoDB.Driver;
using Railway_Ticket_Booking.Domain.Entities;
using Railway_Ticket_Booking.Infrastructure;

namespace Railway_Ticket_Booking.Application.Stations.Queries.Handlers
{
    public class GetAllStationsHandler : IRequestHandler<GetAllStationsQuery, List<Station>>
    {
        private readonly MongoDbContext _context;

        public GetAllStationsHandler(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<List<Station>> Handle(GetAllStationsQuery request, CancellationToken cancellationToken)
        {
            return await _context.Stations
                .Find(_ => true)
                .SortBy(s => s.StationName)
                .ToListAsync(cancellationToken);
        }
    }
}
