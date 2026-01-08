using MediatR;

namespace RailwayTicketBooking.Application.TrainSchedules.Commands
{
    public class DeleteTrainScheduleCommand : IRequest<bool>
    {
        public string Id { get; set; }

        public DeleteTrainScheduleCommand(string id)
        {
            Id = id;
        }
    }
}
