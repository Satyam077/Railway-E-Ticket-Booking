using MediatR;
using MongoDB.Driver;
using Railway_Ticket_Booking.Domain.Entities;
using Railway_Ticket_Booking.Infrastructure;

namespace Railway_Ticket_Booking.Application.TrainSchedules.Queries.Handlers
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
                .Find(_ => true)
                .SortByDescending(ts => ts.DepartureDate)
                .ToListAsync(cancellationToken);
        }
    }
}
