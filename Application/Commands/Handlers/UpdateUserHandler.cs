using MediatR;
using MongoDB.Driver;
using Railway_Ticket_Booking.Infrastructure;
using BCrypt.Net;

namespace Railway_Ticket_Booking.Application.Commands.Handlers
{
    public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, bool>
    {
        private readonly MongoDbContext _context;

        public UpdateUserHandler(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            // Check if another user with the same email exists (excluding current user)
            var existingUser = await _context.Users
                .Find(u => u.Email == request.Email && u.Id != request.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (existingUser != null)
            {
                throw new InvalidOperationException("Another user with this email already exists.");
            }

            // Create update definition
            var updateBuilder = Builders<Railway_Ticket_Booking.Domain.Entities.User>.Update
                .Set(u => u.FirstName, request.FirstName)
                .Set(u => u.LastName, request.LastName)
                .Set(u => u.Email, request.Email)
                .Set(u => u.PhoneNumber, request.PhoneNumber)
                .Set(u => u.Role, request.Role)
                .Set(u => u.DateOfBirth, request.DateOfBirth)
                .Set(u => u.Gender, request.Gender)
                .Set(u => u.Address, request.Address)
                .Set(u => u.City, request.City)
                .Set(u => u.State, request.State)
                .Set(u => u.PinCode, request.PinCode)
                .Set(u => u.Country, request.Country)
                .Set(u => u.IsActive, request.IsActive)
                .Set(u => u.IsEmailVerified, request.IsEmailVerified)
                .Set(u => u.IsPhoneVerified, request.IsPhoneVerified)
                .Set(u => u.UpdatedAt, DateTime.UtcNow);

            // Update password only if provided
            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
                updateBuilder = updateBuilder.Set(u => u.PasswordHash, hashedPassword);
            }

            var result = await _context.Users.UpdateOneAsync(
                u => u.Id == request.Id,
                updateBuilder,
                cancellationToken: cancellationToken);

            return result.ModifiedCount > 0;
        }
    }
}
