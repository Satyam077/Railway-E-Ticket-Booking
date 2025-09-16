using MediatR;
using Railway_Ticket_Booking.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Railway_Ticket_Booking.Application.TrainSchedules.Commands
{
    public class UpdateTrainScheduleCommand : IRequest<bool>
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string TrainId { get; set; }

        [Required]
        public string RouteId { get; set; }

        [Required]
        public DateTime DepartureDate { get; set; }

        [Required]
        public DateTime ArrivalDate { get; set; }

        public List<ScheduleStation> Stations { get; set; } = new List<ScheduleStation>();

        public bool IsActive { get; set; } = true;
    }
}
