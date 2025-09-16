using MediatR;
using MongoDB.Driver;
using Railway_Ticket_Booking.Infrastructure;

namespace Railway_Ticket_Booking.Application.Stations.Commands.Handlers
{
    public class DeleteStationHandler : IRequestHandler<DeleteStationCommand, bool>
    {
        private readonly MongoDbContext _context;

        public DeleteStationHandler(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteStationCommand request, CancellationToken cancellationToken)
        {
            // Check if station is being used in any routes
            var routesUsingStation = await _context.Routes
                .Find(r => r.SourceStationId == request.Id || 
                          r.DestinationStationId == request.Id ||
                          r.Stations.Any(s => s.StationId == request.Id))
                .CountDocumentsAsync(cancellationToken);

            if (routesUsingStation > 0)
            {
                throw new InvalidOperationException("Cannot delete station that is being used in routes. Please remove the station from all routes first.");
            }

            var result = await _context.Stations.DeleteOneAsync(
                s => s.Id == request.Id,
                cancellationToken: cancellationToken);

            return result.DeletedCount > 0;
        }
    }
}
