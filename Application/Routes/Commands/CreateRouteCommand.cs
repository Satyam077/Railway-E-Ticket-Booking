using MediatR;
using RailwayTicketBooking.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace RailwayTicketBooking.Application.Routes.Commands
{
    public class CreateRouteCommand : IRequest<string>
    {
        [Required]
        public string SourceStationId { get; set; }

        [Required]
        public string DestinationStationId { get; set; }

        [Required]
        public List<RouteStation> Stations { get; set; } = new List<RouteStation>();

        [Required]
        public List<Train> trains { get; set; } = new List<Train>();

        [Range(0, double.MaxValue, ErrorMessage = "Total distance must be positive")]
        public double TotalDistance { get; set; }
        public List<string> TrainIds { get; set; } = new List<string>();
        public bool IsDeleted { get; set; } = false;
    }
}
