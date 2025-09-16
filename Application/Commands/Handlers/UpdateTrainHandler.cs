using MediatR;
using MongoDB.Driver;
using Railway_Ticket_Booking.Infrastructure;

namespace Railway_Ticket_Booking.Application.Commands.Handlers
{
    public class UpdateTrainHandler : IRequestHandler<UpdateTrainCommand, bool>
    {
        private readonly MongoDbContext _context;

        public UpdateTrainHandler(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateTrainCommand request, CancellationToken cancellationToken)
        {
            // Check if another train with the same number exists (excluding current train)
            var existingTrain = await _context.Trains
                .Find(t => t.TrainNumber == request.TrainNumber && t.Id != request.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (existingTrain != null)
            {
                throw new InvalidOperationException("Another train with this number already exists.");
            }

            // Create update definition
            var update = Builders<Railway_Ticket_Booking.Domain.Entities.Train>.Update
                .Set(t => t.TrainNumber, request.TrainNumber)
                .Set(t => t.Name, request.Name)
                .Set(t => t.TrainType, request.TrainType)
                .Set(t => t.RouteIds, request.RouteIds)
                .Set(t => t.Classes, request.Classes)
                .Set(t => t.TotalSeats, request.TotalSeats)
                .Set(t => t.IsActive, request.IsActive)
                .Set(t => t.UpdatedAt, DateTime.UtcNow);

            var result = await _context.Trains.UpdateOneAsync(
                t => t.Id == request.Id,
                update,
                cancellationToken: cancellationToken);

            return result.ModifiedCount > 0;
        }
    }
}
