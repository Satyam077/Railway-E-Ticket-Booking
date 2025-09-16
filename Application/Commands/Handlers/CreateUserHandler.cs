using MediatR;
using Railway_Ticket_Booking.Domain.Entities;
using Railway_Ticket_Booking.Infrastructure;
using BCrypt.Net;
using MongoDB.Driver;

namespace Railway_Ticket_Booking.Application.Commands.Handlers
{
    public class CreateUserHandler : IRequestHandler<CreateUserCommand, string>
    {
        private readonly MongoDbContext _context;

        public CreateUserHandler(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<string> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            // Check if user with email already exists
            var existingUser = await _context.Users
                .Find(u => u.Email == request.Email)
                .FirstOrDefaultAsync(cancellationToken);

            if (existingUser != null)
            {
                throw new InvalidOperationException("User with this email already exists.");
            }

            // Hash the password
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                PasswordHash = hashedPassword,
                Role = request.Role,
                DateOfBirth = request.DateOfBirth,
                Gender = request.Gender,
                Address = request.Address,
                City = request.City,
                State = request.State,
                PinCode = request.PinCode,
                Country = request.Country,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.Users.InsertOneAsync(user, cancellationToken: cancellationToken);
            return user.Id;
        }
    }
}
