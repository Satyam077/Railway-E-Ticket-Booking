using MediatR;
using Railway_Ticket_Booking.Domain.Entities;

namespace Railway_Ticket_Booking.Application.Account.Queries
{
    public class GetAllUsersQuery : IRequest<List<User>> { }

    public class GetUserByIdQuery : IRequest<User>
    {
        public string UserId { get; set; }

        public GetUserByIdQuery(string userId)
        {
            UserId = userId;
        }
    }

    public class GetUserByEmailQuery : IRequest<User>
    {
        public string Email { get; set; }

        public GetUserByEmailQuery(string email)
        {
            Email = email;
        }
    }
}
