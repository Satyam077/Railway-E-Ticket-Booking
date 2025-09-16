using MediatR;
using MongoDB.Driver;
using Railway_Ticket_Booking.Domain.Entities;
using Railway_Ticket_Booking.Infrastructure;

namespace Railway_Ticket_Booking.Application.TrainSchedules.Commands.Handlers
{
    public class CreateTrainScheduleHandler : IRequestHandler<CreateTrainScheduleCommand, string>
    {
        private readonly MongoDbContext _context;

        public CreateTrainScheduleHandler(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<string> Handle(CreateTrainScheduleCommand request, CancellationToken cancellationToken)
        {
            var trainSchedule = new TrainSchedule
            {
                TrainId = request.TrainId,
                RouteId = request.RouteId,
                DepartureDate = request.DepartureDate,
                ArrivalDate = request.ArrivalDate,
                Stations = request.Stations ?? new List<ScheduleStation>(),
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.TrainSchedules.InsertOneAsync(trainSchedule, cancellationToken: cancellationToken);
            return trainSchedule.Id;
        }
    }
}
