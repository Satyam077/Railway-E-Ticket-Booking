using MediatR;
using MongoDB.Driver;
using RailwayTicketBooking.Domain.Entities;
using RailwayTicketBooking.Infrastructure;

namespace RailwayTicketBooking.Application.Stations.Commands.Handlers
{
    public class UpdateStationHandler : IRequestHandler<UpdateStationCommand, bool>
    {
        private readonly MongoDbContext _context;

        public UpdateStationHandler(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateStationCommand request, CancellationToken cancellationToken)
        {
            // Check if another station with same code exists (excluding current station)
            var existingStationByCode = await _context.Stations
                .Find(s => s.StationCode.ToUpper() == request.StationCode.ToUpper() && s.Id != request.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (existingStationByCode != null)
            {
                throw new InvalidOperationException("Another station with this code already exists.");
            }

            // Check if another station with same name exists (excluding current station)
            var existingStationByName = await _context.Stations
                .Find(s => s.StationName.ToUpper() == request.StationName.ToUpper() && s.Id != request.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (existingStationByName != null)
            {
                throw new InvalidOperationException("Another station with this name already exists.");
            }

            var update = Builders<Station>.Update
                .Set(s => s.StationCode, request.StationCode.ToUpper())
                .Set(s => s.StationName, request.StationName)
                .Set(s => s.City, request.City)
                .Set(s => s.State, request.State)
                .Set(s => s.Country, request.Country)
                .Set(s => s.Latitude, request.Latitude)
                .Set(s => s.Longitude, request.Longitude)
                .Set(s => s.IsActive, request.IsActive)
                .Set(s => s.RouteIds, request.RouteIds)
                .Set(s => s.UpdatedAt, DateTime.UtcNow);

            var result = await _context.Stations.UpdateOneAsync(
                s => s.Id == request.Id,
                update,
                cancellationToken: cancellationToken);

            return result.ModifiedCount > 0;
        }
    }
}
