using MediatR;
using RailwayTicketBooking.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace RailwayTicketBooking.Application.TrainSchedules.Commands
{
    public class CreateTrainScheduleCommand : IRequest<string>
    {

        [Required]
        public string TrainId { get; set; }

        [Required]
        public string RouteId { get; set; }

        // Optional: specify weekdays; empty = daily
        public List<DayOfWeek> RunsOn { get; set; } = new List<DayOfWeek>();

        public List<ScheduleStation> Stations { get; set; } = new List<ScheduleStation>();

        public bool IsActive { get; set; } = true;
    }
}
