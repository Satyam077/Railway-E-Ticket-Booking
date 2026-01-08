using MediatR;
using MongoDB.Driver;
using RailwayTicketBooking.Domain.Entities;
using RailwayTicketBooking.Infrastructure;

namespace RailwayTicketBooking.Application.TrainSchedules.Queries.Handlers
{
    public class GetTrainScheduleByIdHandler : IRequestHandler<GetTrainScheduleByIdQuery, TrainSchedule>
    {
        private readonly MongoDbContext _context;

        public GetTrainScheduleByIdHandler(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<TrainSchedule> Handle(GetTrainScheduleByIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.TrainSchedules
                .Find(ts => ts.Id == request.Id)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
