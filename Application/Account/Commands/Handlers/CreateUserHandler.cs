using MediatR;
using RailwayTicketBooking.Domain.Entities;
using RailwayTicketBooking.Domain.Enums;
using RailwayTicketBooking.EmailServices;
using RailwayTicketBooking.Infrastructure;
using BCrypt.Net;
using MongoDB.Driver;
using RailwayTicketBooking.EmailTemplates;
using RailwayTicketBooking.Domain.DTOs;
using RailwayTicketBooking.Application.Account.Commands;

namespace RailwayTicketBooking.Application.Account.Commands.Handlers
{
    public class CreateUserHandler : IRequestHandler<CreateUserCommand, RegistrationResponseDTO>
    {
        private readonly MongoDbContext _context;
        private readonly IEmailService _emailService;

        public CreateUserHandler(MongoDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
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

            // Send welcome email
            try
            {
                var emailSubject = $"Welcome to Railway Ticket Booking - Account Created Successfully";
                var emailBody = EmailTemplate.GenerateRegistrationEmail(user, request.Role);
                await _emailService.SendEmailAsync(user.Email, emailSubject, emailBody);
            }
            catch (Exception ex)
            {
                // Log the error but don't fail the registration
                Console.WriteLine($"Failed to send registration email: {ex.Message}");
            }

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
