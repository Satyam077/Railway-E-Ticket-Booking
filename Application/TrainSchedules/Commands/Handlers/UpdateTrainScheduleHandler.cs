using MediatR;
using MongoDB.Driver;
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
            var filter = Builders<Railway_Ticket_Booking.Domain.Entities.TrainSchedule>.Filter.Eq(ts => ts.Id, request.Id);
            
            var update = Builders<Railway_Ticket_Booking.Domain.Entities.TrainSchedule>.Update
                .Set(ts => ts.TrainId, request.TrainId)
                .Set(ts => ts.RouteId, request.RouteId)
                .Set(ts => ts.DepartureDate, request.DepartureDate)
                .Set(ts => ts.ArrivalDate, request.ArrivalDate)
                .Set(ts => ts.Stations, request.Stations)
                .Set(ts => ts.IsActive, request.IsActive)
                .Set(ts => ts.UpdatedAt, DateTime.UtcNow);

            var result = await _context.TrainSchedules.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
            return result.ModifiedCount > 0;
        }
    }
}
