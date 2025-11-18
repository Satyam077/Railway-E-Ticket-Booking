using MediatR;
using Railway_Ticket_Booking.Application.DTOs;
using Railway_Ticket_Booking.Domain.Entities;
using Railway_Ticket_Booking.Infrastructure;
using BCrypt.Net;
using MongoDB.Driver;

namespace Railway_Ticket_Booking.Application.Commands.Handlers
{
    public class CreateUserHandler : IRequestHandler<CreateUserCommand, RegistrationResponseDTO>
    {
        private readonly MongoDbContext _context;

        public CreateUserHandler(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<RegistrationResponseDTO> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            // Normalize email to lowercase for comparison
            var normalizedEmail = request.Email.ToLowerInvariant();
            
            // Check if user with email already exists (case-insensitive)
            var existingUser = await _context.Users
                .Find(u => u.Email == normalizedEmail)
                .FirstOrDefaultAsync(cancellationToken);

            if (existingUser != null)
            {
                return new RegistrationResponseDTO
                {
                    IsSuccess = false,
                    Message = "User with this email already exists."
                };
            }

            // Hash the password
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = normalizedEmail,
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

            return new RegistrationResponseDTO
            {
                UserId = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role.ToString(),
                IsSuccess = true,
                Message = "User registered successfully!"
            };
        }
    }
}
