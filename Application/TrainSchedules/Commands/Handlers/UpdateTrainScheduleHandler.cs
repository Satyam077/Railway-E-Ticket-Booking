using MediatR;
using MongoDB.Driver;
using Railway_Ticket_Booking.Domain.Entities;
using Railway_Ticket_Booking.Infrastructure;

namespace Railway_Ticket_Booking.Application.TrainSchedules.Commands.Handlers
{
    public class UpdateTrainScheduleHandler : IRequestHandler<UpdateTrainScheduleCommand, bool>
    {
        private readonly MongoDbContext _context;

        public UpdateTrainScheduleHandler(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateTrainScheduleCommand request, CancellationToken cancellationToken)
        {
            var filter = Builders<TrainSchedule>.Filter.Eq(s => s.Id, request.Id);

            var update = Builders<TrainSchedule>.Update
                .Set(s => s.TrainId, request.TrainId)
                .Set(s => s.RouteId, request.RouteId)
                .Set(s => s.RunsOn, request.RunsOn ?? new List<DayOfWeek>())
                .Set(s => s.Stations, request.Stations ?? new List<ScheduleStation>())
                .Set(s => s.IsActive, request.IsActive)
                .Set(s => s.UpdatedAt, DateTime.UtcNow);

            var res = await _context.TrainSchedules.UpdateOneAsync(
                filter,
                update,
                cancellationToken: cancellationToken);

            return res.ModifiedCount > 0;
        }
    }
}
