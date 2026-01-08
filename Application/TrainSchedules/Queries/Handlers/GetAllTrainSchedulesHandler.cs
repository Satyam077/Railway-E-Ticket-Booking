using MediatR;
using MongoDB.Driver;
using RailwayTicketBooking.Domain.Entities;
using RailwayTicketBooking.Infrastructure;

namespace RailwayTicketBooking.Application.TrainSchedules.Queries.Handlers
{
    public class GetAllTrainSchedulesHandler : IRequestHandler<GetAllTrainSchedulesQuery, List<TrainSchedule>>
    {
        private readonly MongoDbContext _context;

        public GetAllTrainSchedulesHandler(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<List<TrainSchedule>> Handle(GetAllTrainSchedulesQuery request, CancellationToken cancellationToken)
        {
            return await _context.TrainSchedules
                .Find(_ => true).ToListAsync(cancellationToken);
        }
    }
}
