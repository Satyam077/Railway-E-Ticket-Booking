using MediatR;
using System.ComponentModel.DataAnnotations;

namespace RailwayTicketBooking.Application.Stations.Commands
{
    public class DeleteStationCommand : IRequest<bool>
    {
        [Required]
        public string? Id { get; set; }

        public DeleteStationCommand(string id)
        {
            Id = id;
        }
    }
}