using MediatR;
using MongoDB.Driver;
using RailwayTicketBooking.Infrastructure;
using BCrypt.Net;
using RailwayTicketBooking.Infrastructure.Services;
using RailwayTicketBooking.Domain.DTOs;
using RailwayTicketBooking.Application.Account.Commands;

namespace RailwayTicketBooking.Application.Account.Commands.Handlers
{
    public class LoginHandler : IRequestHandler<LoginCommand, LoginResponseDTO>
    {
        private readonly MongoDbContext _context;
        private readonly IJwtService _jwtService;
        private readonly IConfiguration _configuration;

        public LoginHandler(MongoDbContext context, IJwtService jwtService, IConfiguration configuration)
        {
            _context = context;
            _jwtService = jwtService;
            _configuration = configuration;
        }

        public async Task<LoginResponseDTO> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // Normalize email to lowercase for comparison
            var normalizedEmail = request.Email.ToLowerInvariant();

            // Find user by email
            var user = await _context.Users
                .Find(u => u.Email == normalizedEmail)
                .FirstOrDefaultAsync(cancellationToken);

            if (user == null)
            {
                return new LoginResponseDTO
                {
                    IsSuccess = false,
                    Message = "Invalid email or password."
                };
            }

            // Check if user is active
            if (!user.IsActive)
            {
                return new LoginResponseDTO
                {
                    IsSuccess = false,
                    Message = "Your account has been deactivated. Please contact support."
                };
            }

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return new LoginResponseDTO
                {
                    IsSuccess = false,
                    Message = "Invalid email or password."
                };
            }

            // Update last login time
            var update = Builders<Domain.Entities.User>.Update
                .Set(u => u.LastLoginAt, DateTime.UtcNow);

            await _context.Users.UpdateOneAsync(
                u => u.Id == user.Id,
                update,
                cancellationToken: cancellationToken);

            // Generate JWT token
            var token = _jwtService.GenerateToken(user);
            var expirationMinutes = int.Parse(_configuration["Jwt:ExpirationMinutes"] ?? "60");

            return new LoginResponseDTO
            {
                Token = token,
                UserId = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role.ToString(),
                ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes),
                IsSuccess = true,
                Message = "Login successful!"
            };
        }
    }
}

