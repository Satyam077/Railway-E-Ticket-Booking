using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Railway_Ticket_Booking.Application.Routes.Commands
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
