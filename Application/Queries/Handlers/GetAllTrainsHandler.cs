// Application/Queries/Handlers/GetAllTrainsHandler.cs
using MediatR;
using MongoDB.Driver;
using Railway_Ticket_Booking.Domain.Entities;
using Railway_Ticket_Booking.Infrastructure;
namespace Railway_Ticket_Booking.Application.Queries.Handlers
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
