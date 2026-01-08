using MediatR;
using RailwayTicketBooking.Domain.Entities;

namespace RailwayTicketBooking.Application.Account.Queries
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
