using MediatR;
using MongoDB.Driver;
using RailwayTicketBooking.Infrastructure;

namespace RailwayTicketBooking.Application.Routes.Commands.Handlers
{
    public class DeleteRouteHandler : IRequestHandler<DeleteRouteCommand, bool>
    {
        private readonly MongoDbContext _context;

        public DeleteRouteHandler(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteRouteCommand request, CancellationToken cancellationToken)
        {
            // Check if route is being used by any trains
            var trainsUsingRoute = await _context.Trains
                .Find(t => t.RouteIds.Contains(request.Id))
                .CountDocumentsAsync(cancellationToken);

            if (trainsUsingRoute > 0)
            {
                throw new InvalidOperationException("Cannot delete route that is being used by trains. Please remove the route from all trains first.");
            }

            var result = await _context.Routes.DeleteOneAsync(
                r => r.Id == request.Id,
                cancellationToken: cancellationToken);

            return result.DeletedCount > 0;
        }
    }
}
