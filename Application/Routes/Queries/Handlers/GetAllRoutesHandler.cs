using MediatR;
using MongoDB.Driver;
using RailwayTicketBooking.Infrastructure;

namespace RailwayTicketBooking.Application.Routes.Queries.Handlers
{
    public class GetAllRoutesHandler : IRequestHandler<GetAllRoutesQuery, List<RailwayTicketBooking.Domain.Entities.Route>>
    {
        private readonly MongoDbContext _context;

        public GetAllRoutesHandler(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<List<RailwayTicketBooking.Domain.Entities.Route>> Handle(GetAllRoutesQuery request, CancellationToken cancellationToken)
        {
            return await _context.Routes.Find(_ => true).ToListAsync(cancellationToken);
        }
    }
}
