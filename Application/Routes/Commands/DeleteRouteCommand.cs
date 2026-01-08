using MediatR;
using System.ComponentModel.DataAnnotations;

namespace RailwayTicketBooking.Application.Routes.Commands
{
    public class DeleteRouteCommand : IRequest<bool>
    {
        [Required]
        public string Id { get; set; }

        public DeleteRouteCommand(string id)
        {
            Id = id;
        }
    }
}
