using MediatR;
using MongoDB.Driver;
using Railway_Ticket_Booking.Domain.Entities;
using Railway_Ticket_Booking.Infrastructure;

namespace Railway_Ticket_Booking.Application.TrainSchedules.Queries.Handlers
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
