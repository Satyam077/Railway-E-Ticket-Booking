using MediatR;
using RailwayTicketBooking.Domain.Entities;

namespace RailwayTicketBooking.Application.TrainSchedules.Queries
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
