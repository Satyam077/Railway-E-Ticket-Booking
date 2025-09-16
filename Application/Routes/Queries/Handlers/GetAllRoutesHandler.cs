using MediatR;
using MongoDB.Driver;
using Railway_Ticket_Booking.Infrastructure;

namespace Railway_Ticket_Booking.Application.Routes.Queries.Handlers
{
    public class GetAllRoutesHandler : IRequestHandler<GetAllRoutesQuery, List<Railway_Ticket_Booking.Domain.Entities.Route>>
    {
        private readonly MongoDbContext _context;

        public GetAllRoutesHandler(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<List<Railway_Ticket_Booking.Domain.Entities.Route>> Handle(GetAllRoutesQuery request, CancellationToken cancellationToken)
        {
            return await _context.Routes.Find(_ => true).ToListAsync(cancellationToken);
        }
    }
}
