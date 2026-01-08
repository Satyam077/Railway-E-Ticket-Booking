using MediatR;
using MongoDB.Driver;
using RailwayTicketBooking.Infrastructure;

namespace RailwayTicketBooking.Application.TrainSchedules.Commands.Handlers
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
            var filter = Builders<RailwayTicketBooking.Domain.Entities.TrainSchedule>.Filter.Eq(ts => ts.Id, request.Id);
            var result = await _context.TrainSchedules.DeleteOneAsync(filter, cancellationToken: cancellationToken);
            return result.DeletedCount > 0;
        }
    }
}
