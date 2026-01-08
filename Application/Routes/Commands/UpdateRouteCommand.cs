using MediatR;
using RailwayTicketBooking.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace RailwayTicketBooking.Application.Routes.Commands
{
    public class UpdateRouteCommand : IRequest<bool>
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string SourceStationId { get; set; }

        [Required]
        public string DestinationStationId { get; set; }

        [Required]
        public List<RouteStation> Stations { get; set; } = new List<RouteStation>();

        [Range(0, double.MaxValue, ErrorMessage = "Total distance must be positive")]
        public double TotalDistance { get; set; }

        public bool IsActive { get; set; } = true;
        [Required]
        public List<Train> trains { get; set; } = new List<Train>();

        public List<string> TrainIds { get; set; } = new List<string>();
    }
}

