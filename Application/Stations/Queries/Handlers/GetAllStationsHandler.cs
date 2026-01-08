using MediatR;
using MongoDB.Driver;
using RailwayTicketBooking.Domain.Entities;
using RailwayTicketBooking.Infrastructure;

namespace RailwayTicketBooking.Application.Stations.Queries.Handlers
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
