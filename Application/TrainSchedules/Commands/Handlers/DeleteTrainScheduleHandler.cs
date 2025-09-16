using MediatR;
using MongoDB.Driver;
using Railway_Ticket_Booking.Infrastructure;

namespace Railway_Ticket_Booking.Application.TrainSchedules.Commands.Handlers
{
    public class DeleteTrainScheduleHandler : IRequestHandler<DeleteTrainScheduleCommand, bool>
    {
        private readonly MongoDbContext _context;

        public DeleteTrainScheduleHandler(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteTrainScheduleCommand request, CancellationToken cancellationToken)
        {
            var filter = Builders<Railway_Ticket_Booking.Domain.Entities.TrainSchedule>.Filter.Eq(ts => ts.Id, request.Id);
            var result = await _context.TrainSchedules.DeleteOneAsync(filter, cancellationToken: cancellationToken);
            return result.DeletedCount > 0;
        }
    }
}
