using MediatR;
using Railway_Ticket_Booking.Domain.Entities;

namespace Railway_Ticket_Booking.Application.TrainSchedules.Queries
{
    public class GetTrainScheduleByIdQuery : IRequest<TrainSchedule>
    {
        public string Id { get; set; }

        public GetTrainScheduleByIdQuery(string id)
        {
            Id = id;
        }
    }
}
