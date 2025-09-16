using MediatR;
using MongoDB.Driver;
using Railway_Ticket_Booking.Infrastructure;

namespace Railway_Ticket_Booking.Application.Routes.Commands.Handlers
{
    public class CreateRouteHandler : IRequestHandler<CreateRouteCommand, string>
    {
        private readonly MongoDbContext _context;

        public CreateRouteHandler(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<string> Handle(CreateRouteCommand request, CancellationToken cancellationToken)
        {
            // Validate that source and destination are different
            if (request.SourceStationId == request.DestinationStationId)
            {
                throw new InvalidOperationException("Source and destination stations cannot be the same.");
            }

            // Check if route already exists
            var existingRoute = await _context.Routes
                .Find(r => r.SourceStationId == request.SourceStationId && 
                          r.DestinationStationId == request.DestinationStationId)
                .FirstOrDefaultAsync(cancellationToken);

            if (existingRoute != null)
            {
                throw new InvalidOperationException("Route between these stations already exists.");
            }

            // Validate stations exist
            var sourceStation = await _context.Stations
                .Find(s => s.Id == request.SourceStationId)
                .FirstOrDefaultAsync(cancellationToken);

            var destinationStation = await _context.Stations
                .Find(s => s.Id == request.DestinationStationId)
                .FirstOrDefaultAsync(cancellationToken);

            if (sourceStation == null)
                throw new InvalidOperationException("Source station not found.");

            if (destinationStation == null)
                throw new InvalidOperationException("Destination station not found.");

            var route = new Railway_Ticket_Booking.Domain.Entities.Route
            {
                SourceStationId = request.SourceStationId,
                DestinationStationId = request.DestinationStationId,
                Stations = request.Stations,
                TotalDistance = request.TotalDistance,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.Routes.InsertOneAsync(route, cancellationToken: cancellationToken);
            return route.Id;
        }
    }
}
