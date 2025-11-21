using MediatR;
using MongoDB.Driver;
using Railway_Ticket_Booking.Domain.Entities;
using Railway_Ticket_Booking.Domain.Enums;
using Railway_Ticket_Booking.EmailServices;
using Railway_Ticket_Booking.Infrastructure;
using BCrypt.Net;

namespace Railway_Ticket_Booking.Application.Commands.Handlers
{
    public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, bool>
    {
        private readonly MongoDbContext _context;
        private readonly IEmailService _emailService;

        public UpdateUserHandler(MongoDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            // Get the existing user to compare changes
            var existingUser = await _context.Users
                .Find(u => u.Id == request.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (existingUser == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            // Check if another user with the same email exists (excluding current user)
            var userWithEmail = await _context.Users
                .Find(u => u.Email == request.Email && u.Id != request.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (userWithEmail != null)
            {
                throw new InvalidOperationException("Another user with this email already exists.");
            }

            // Create update definition
            var updateBuilder = Builders<User>.Update
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
            bool passwordChanged = false;
            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
                updateBuilder = updateBuilder.Set(u => u.PasswordHash, hashedPassword);
                passwordChanged = true;
            }

            var result = await _context.Users.UpdateOneAsync(
                u => u.Id == request.Id,
                updateBuilder,
                cancellationToken: cancellationToken);

            if (result.ModifiedCount > 0)
            {
                // Get updated user for email
                var updatedUser = await _context.Users
                    .Find(u => u.Id == request.Id)
                    .FirstOrDefaultAsync(cancellationToken);

                // Send update notification email
                try
                {
                    var emailSubject = "Account Updated - Railway Ticket Booking System";
                    var emailBody = GenerateUpdateEmail(existingUser, updatedUser, request, passwordChanged);
                    await _emailService.SendEmailAsync(updatedUser.Email, emailSubject, emailBody);
                }
                catch (Exception ex)
                {
                    // Log the error but don't fail the update
                    Console.WriteLine($"Failed to send update email: {ex.Message}");
                }
            }

            return result.ModifiedCount > 0;
        }

        private string GenerateUpdateEmail(User oldUser, User newUser, UpdateUserCommand request, bool passwordChanged)
        {
            var roleName = request.Role switch
            {
                UserRole.SuperAdmin => "Super Administrator",
                UserRole.Admin => "Administrator",
                UserRole.ZonalManager => "Zonal Manager",
                UserRole.Customer => "Customer",
                _ => "User"
            };

            // Build changes list
            var changes = new List<string>();
            if (oldUser.FirstName != request.FirstName) changes.Add($"First Name: {oldUser.FirstName} → {request.FirstName}");
            if (oldUser.LastName != request.LastName) changes.Add($"Last Name: {oldUser.LastName} → {request.LastName}");
            if (oldUser.Email != request.Email) changes.Add($"Email: {oldUser.Email} → {request.Email}");
            if (oldUser.PhoneNumber != request.PhoneNumber) changes.Add($"Phone Number: {oldUser.PhoneNumber} → {request.PhoneNumber}");
            if (oldUser.Role != request.Role) changes.Add($"Role: {oldUser.Role} → {roleName}");
            if (oldUser.DateOfBirth != request.DateOfBirth) changes.Add($"Date of Birth: {oldUser.DateOfBirth:dd MMM, yyyy} → {request.DateOfBirth:dd MMM, yyyy}");
            if (oldUser.Gender != request.Gender) changes.Add($"Gender: {oldUser.Gender ?? "Not specified"} → {request.Gender ?? "Not specified"}");
            if (oldUser.Address != request.Address) changes.Add($"Address: {oldUser.Address ?? "Not provided"} → {request.Address ?? "Not provided"}");
            if (oldUser.City != request.City) changes.Add($"City: {oldUser.City ?? "Not provided"} → {request.City ?? "Not provided"}");
            if (oldUser.State != request.State) changes.Add($"State: {oldUser.State ?? "Not provided"} → {request.State ?? "Not provided"}");
            if (oldUser.PinCode != request.PinCode) changes.Add($"Pin Code: {oldUser.PinCode ?? "Not provided"} → {request.PinCode ?? "Not provided"}");
            if (oldUser.Country != request.Country) changes.Add($"Country: {oldUser.Country ?? "India"} → {request.Country ?? "India"}");
            if (oldUser.IsActive != request.IsActive) changes.Add($"Account Status: {(oldUser.IsActive ? "Active" : "Inactive")} → {(request.IsActive ? "Active" : "Inactive")}");
            if (oldUser.IsEmailVerified != request.IsEmailVerified) changes.Add($"Email Verified: {(oldUser.IsEmailVerified ? "Yes" : "No")} → {(request.IsEmailVerified ? "Yes" : "No")}");
            if (oldUser.IsPhoneVerified != request.IsPhoneVerified) changes.Add($"Phone Verified: {(oldUser.IsPhoneVerified ? "Yes" : "No")} → {(request.IsPhoneVerified ? "Yes" : "No")}");
            if (passwordChanged) changes.Add("Password: Changed");

            var changesHtml = changes.Any() 
                ? string.Join("", changes.Select(c => $"<tr><td style='padding: 5px; border-bottom: 1px solid #eee;'>{c}</td></tr>"))
                : "<tr><td style='padding: 5px;'>No changes detected</td></tr>";

            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #ff9800; color: white; padding: 20px; text-align: center; border-radius: 5px 5px 0 0; }}
        .content {{ background-color: #f9f9f9; padding: 30px; border: 1px solid #ddd; }}
        .footer {{ background-color: #333; color: white; padding: 15px; text-align: center; border-radius: 0 0 5px 5px; font-size: 12px; }}
        .info-table {{ width: 100%; margin: 20px 0; border-collapse: collapse; }}
        .info-table td {{ padding: 8px; border-bottom: 1px solid #eee; }}
        .info-table td:first-child {{ font-weight: bold; width: 40%; }}
        .changes-table {{ width: 100%; margin: 20px 0; background-color: #fff3cd; border: 1px solid #ffc107; border-radius: 5px; }}
        .warning {{ background-color: #fff3cd; border-left: 4px solid #ff9800; padding: 15px; margin: 20px 0; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>Account Updated</h1>
        </div>
        <div class=""content"">
            <h2>Hello {newUser.FirstName}!</h2>
            <p>Your account information has been successfully updated. Please review the changes below:</p>
            
            <h3>Updated Account Details:</h3>
            <table class=""info-table"">
                <tr>
                    <td>Full Name:</td>
                    <td>{newUser.FullName}</td>
                </tr>
                <tr>
                    <td>Email:</td>
                    <td>{newUser.Email}</td>
                </tr>
                <tr>
                    <td>Phone Number:</td>
                    <td>{newUser.PhoneNumber}</td>
                </tr>
                <tr>
                    <td>Role:</td>
                    <td>{roleName}</td>
                </tr>
                <tr>
                    <td>Account Status:</td>
                    <td>{(newUser.IsActive ? "Active" : "Inactive")}</td>
                </tr>
                <tr>
                    <td>Date of Birth:</td>
                    <td>{newUser.DateOfBirth:dd MMM, yyyy}</td>
                </tr>
                <tr>
                    <td>Gender:</td>
                    <td>{newUser.Gender ?? "Not specified"}</td>
                </tr>
                <tr>
                    <td>Address:</td>
                    <td>{newUser.Address ?? "Not provided"}, {newUser.City ?? ""}, {newUser.State ?? ""} - {newUser.PinCode ?? ""}, {newUser.Country ?? "India"}</td>
                </tr>
                <tr>
                    <td>Email Verified:</td>
                    <td>{(newUser.IsEmailVerified ? "Yes" : "No")}</td>
                </tr>
                <tr>
                    <td>Phone Verified:</td>
                    <td>{(newUser.IsPhoneVerified ? "Yes" : "No")}</td>
                </tr>
                <tr>
                    <td>Last Updated:</td>
                    <td>{newUser.UpdatedAt:dd MMM, yyyy 'at' HH:mm} UTC</td>
                </tr>
            </table>

            <div class=""warning"">
                <h3 style=""margin-top: 0;"">Changes Made:</h3>
                <table class=""changes-table"">
                    {changesHtml}
                </table>
            </div>

            {(passwordChanged ? @"<div class=""warning"">
                <p><strong>Important:</strong> Your password has been changed. If you did not make this change, please contact our support team immediately and reset your password.</p>
            </div>" : "")}

            <p>If you did not make these changes, please contact our support team immediately.</p>
            
            <p style=""margin-top: 30px; padding-top: 20px; border-top: 1px solid #ddd; font-size: 12px; color: #666;"">
                This is an automated notification email. Please do not reply to this email.
            </p>
        </div>
        <div class=""footer"">
            <p>&copy; {DateTime.Now.Year} Railway Ticket Booking System. All rights reserved.</p>
            <p>This is an automated email, please do not reply.</p>
        </div>
    </div>
</body>
</html>";
        }
    }
}
