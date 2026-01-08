using MediatR;
using MongoDB.Driver;
using RailwayTicketBooking.Domain.Entities;
using RailwayTicketBooking.Infrastructure;

namespace RailwayTicketBooking.Application.TrainSchedules.Commands.Handlers
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
            if (string.IsNullOrWhiteSpace(request.TrainId))
                throw new ArgumentException("TrainId is required", nameof(request));
            
            if (string.IsNullOrWhiteSpace(request.RouteId))
                throw new ArgumentException("RouteId is required", nameof(request));

            var schedule = new TrainSchedule
            {
                TrainId = request.TrainId,
                RouteId = request.RouteId,
                RunsOn = request.RunsOn ?? new List<DayOfWeek>(),
                Stations = request.Stations ?? new List<ScheduleStation>(),
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.TrainSchedules.InsertOneAsync(schedule, cancellationToken: cancellationToken);
            return schedule.Id;
        }
    }
}
