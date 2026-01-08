using MediatR;
using MongoDB.Driver;
using RailwayTicketBooking.Domain.Entities;
using RailwayTicketBooking.Infrastructure;

namespace RailwayTicketBooking.Application.Commands.Handlers
{
    public class CreateTrainHandler : IRequestHandler<CreateTrainCommand, string>
    {
        private readonly MongoDbContext _context;

        public CreateTrainHandler(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<string> Handle(CreateTrainCommand request, CancellationToken cancellationToken)
        {
            // Check if train with same number already exists
            var existingTrain = await _context.Trains
                .Find(t => t.TrainNumber == request.TrainNumber)
                .FirstOrDefaultAsync(cancellationToken);

            if (existingTrain != null)
            {
                throw new InvalidOperationException("Train with this number already exists.");
            }

            var train = new Train
            {
                TrainNumber = request.TrainNumber,
                Name = request.Name,
                TrainType = request.TrainType,
                RouteIds = request.RouteIds,
                Classes = request.Classes,
                TotalSeats = request.TotalSeats,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.Trains.InsertOneAsync(train, cancellationToken: cancellationToken);
            return train.Id;
        }
    }
}
