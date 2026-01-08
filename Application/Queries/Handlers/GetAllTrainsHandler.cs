// Application/Queries/Handlers/GetAllTrainsHandler.cs
using MediatR;
using MongoDB.Driver;
using RailwayTicketBooking.Domain.Entities;
using RailwayTicketBooking.Infrastructure;
namespace RailwayTicketBooking.Application.Queries.Handlers
{
    public class GetAllTrainsHandler : IRequestHandler<GetAllTrainsQuery, List<Train>>
    {
        private readonly MongoDbContext _context;

        public GetAllTrainsHandler(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<List<Train>> Handle(GetAllTrainsQuery request, CancellationToken cancellationToken)
        {
            return await _context.Trains.Find(_ => true).ToListAsync(cancellationToken);
        }
    }
}
