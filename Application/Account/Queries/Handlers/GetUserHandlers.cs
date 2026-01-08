using MediatR;
using MongoDB.Driver;
using RailwayTicketBooking.Domain.Entities;
using RailwayTicketBooking.Infrastructure;

namespace RailwayTicketBooking.Application.Account.Queries.Handlers
{
    public class GetAllUsersHandler : IRequestHandler<GetAllUsersQuery, List<User>>
    {
        private readonly MongoDbContext _context;

        public GetAllUsersHandler(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<List<User>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            return await _context.Users
                .Find(_ => true)
                .SortByDescending(u => u.CreatedAt)
                .ToListAsync(cancellationToken);
        }
    }

    public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, User>
    {
        private readonly MongoDbContext _context;

        public GetUserByIdHandler(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<User> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.Users
                .Find(u => u.Id == request.UserId)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }

    public class GetUserByEmailHandler : IRequestHandler<GetUserByEmailQuery, User>
    {
        private readonly MongoDbContext _context;

        public GetUserByEmailHandler(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<User> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
        {
            return await _context.Users
                .Find(u => u.Email == request.Email && u.IsActive)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
