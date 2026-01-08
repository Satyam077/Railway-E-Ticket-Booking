using MediatR;
using MongoDB.Driver;
using RailwayTicketBooking.Infrastructure;

namespace RailwayTicketBooking.Application.Routes.Commands.Handlers
{
    public class UpdateRouteHandler : IRequestHandler<UpdateRouteCommand, bool>
    {
        private readonly MongoDbContext _context;

        public UpdateRouteHandler(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateRouteCommand request, CancellationToken cancellationToken)
        {
            // Validate that source and destination are different
            if (request.SourceStationId == request.DestinationStationId)
            {
                throw new InvalidOperationException("Source and destination stations cannot be the same.");
            }

            // Check if another route with same source/destination exists (excluding current route)
            var existingRoute = await _context.Routes
                .Find(r => r.SourceStationId == request.SourceStationId && 
                          r.DestinationStationId == request.DestinationStationId &&
                          r.Id != request.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (existingRoute != null)
            {
                throw new InvalidOperationException("Another route between these stations already exists.");
            }

            var update = Builders<RailwayTicketBooking.Domain.Entities.Route>.Update
                .Set(r => r.SourceStationId, request.SourceStationId)
                .Set(r => r.DestinationStationId, request.DestinationStationId)
                .Set(r => r.Stations, request.Stations)
                .Set(r => r.TotalDistance, request.TotalDistance)
                .Set(r => r.IsActive, request.IsActive)
                .Set(r => r.TrainIds, request.TrainIds)
                .Set(r => r.UpdatedAt, DateTime.UtcNow);

            var result = await _context.Routes.UpdateOneAsync(
                r => r.Id == request.Id,
                update,
                cancellationToken: cancellationToken);

            return result.ModifiedCount > 0;
        }
    }
}
