using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Railway_Ticket_Booking.Application.Account.Commands
{
    public class DeleteUserCommand : IRequest<bool>
    {
        [Required]
        public string Id { get; set; }

        public DeleteUserCommand(string id)
        {
            Id = id;
        }
    }
}
