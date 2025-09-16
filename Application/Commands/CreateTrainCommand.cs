using MediatR;
using Railway_Ticket_Booking.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Railway_Ticket_Booking.Application.Commands
{
    public class CreateTrainCommand : IRequest<string>
    {
        [Required]
        [StringLength(10)]
        public string TrainNumber { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(50)]
        public string TrainType { get; set; }

        public List<string> RouteIds { get; set; } = new List<string>();

        public List<TrainClass> Classes { get; set; } = new List<TrainClass>();

        public int TotalSeats { get; set; }
    }
}
