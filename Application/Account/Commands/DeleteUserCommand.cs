using MediatR;
using System.ComponentModel.DataAnnotations;

namespace RailwayTicketBooking.Application.Account.Commands
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
