using MediatR;
using MongoDB.Driver;
using Railway_Ticket_Booking.Domain.Entities;
using Railway_Ticket_Booking.Infrastructure;

namespace Railway_Ticket_Booking.Application.Stations.Commands.Handlers
{
    public class CreateStationHandler : IRequestHandler<CreateStationCommand, string>
    {
        private readonly MongoDbContext _context;

        public CreateStationHandler(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<string> Handle(CreateStationCommand request, CancellationToken cancellationToken)
        {
            // Check if station with same code already exists
            var existingStationByCode = await _context.Stations
                .Find(s => s.StationCode.ToUpper() == request.StationCode.ToUpper())
                .FirstOrDefaultAsync(cancellationToken);

            if (existingStationByCode != null)
            {
                throw new InvalidOperationException("Station with this code already exists.");
            }

            // Check if station with same name already exists
            var existingStationByName = await _context.Stations
                .Find(s => s.StationName.ToUpper() == request.StationName.ToUpper())
                .FirstOrDefaultAsync(cancellationToken);

            if (existingStationByName != null)
            {
                throw new InvalidOperationException("Station with this name already exists.");
            }

            var station = new Station
            {
                StationCode = request.StationCode.ToUpper(),
                StationName = request.StationName,
                City = request.City,
                State = request.State,
                Country = request.Country,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.Stations.InsertOneAsync(station, cancellationToken: cancellationToken);
            return station.Id;
        }
    }
}