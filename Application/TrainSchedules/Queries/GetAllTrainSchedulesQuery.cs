using MediatR;
using RailwayTicketBooking.Domain.Entities;

namespace RailwayTicketBooking.Application.TrainSchedules.Queries
{
    public class GetAllTrainSchedulesQuery : IRequest<List<TrainSchedule>> { }
}
