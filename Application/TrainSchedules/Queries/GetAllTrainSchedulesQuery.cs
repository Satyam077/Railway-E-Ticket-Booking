using MediatR;
using Railway_Ticket_Booking.Domain.Entities;

namespace Railway_Ticket_Booking.Application.TrainSchedules.Queries
{
    public class GetAllTrainSchedulesQuery : IRequest<List<TrainSchedule>> { }
}
