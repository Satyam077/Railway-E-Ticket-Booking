using MediatR;

namespace Railway_Ticket_Booking.Application.TrainSchedules.Commands
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
