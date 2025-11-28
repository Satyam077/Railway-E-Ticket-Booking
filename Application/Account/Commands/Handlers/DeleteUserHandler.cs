using MediatR;
using MongoDB.Driver;
using Railway_Ticket_Booking.Domain.Entities;
using Railway_Ticket_Booking.Domain.Enums;
using Railway_Ticket_Booking.EmailServices;
using Railway_Ticket_Booking.Infrastructure;

namespace Railway_Ticket_Booking.Application.Account.Commands.Handlers
{
    public class DeleteUserHandler : IRequestHandler<DeleteUserCommand, bool>
    {
        private readonly MongoDbContext _context;
        private readonly IEmailService _emailService;

        public DeleteUserHandler(MongoDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            // Get user details before deletion for email notification
            var user = await _context.Users
                .Find(u => u.Id == request.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (user == null)
            {
                return false;
            }

            // Check if user has any active bookings before deletion
            var userBookings = await _context.Bookings
                .Find(b => b.UserId == request.Id)
                .CountDocumentsAsync(cancellationToken);

            if (userBookings > 0)
            {
                throw new InvalidOperationException("Cannot delete user with existing bookings. Please cancel all bookings first.");
            }

            // Send account deletion notification email before deleting
            try
            {
                var emailSubject = "Account Deleted - Railway Ticket Booking System";
                var emailBody = GenerateDeletionEmail(user);
                await _emailService.SendEmailAsync(user.Email, emailSubject, emailBody);
            }
            catch (Exception ex)
            {
                // Log the error but don't fail the deletion
                Console.WriteLine($"Failed to send deletion email: {ex.Message}");
            }

            var result = await _context.Users.DeleteOneAsync(
                u => u.Id == request.Id,
                cancellationToken: cancellationToken);

            return result.DeletedCount > 0;
        }

        private string GenerateDeletionEmail(User user)
        {
            var roleName = user.Role switch
            {
                UserRole.SuperAdmin => "Super Administrator",
                UserRole.Admin => "Administrator",
                UserRole.ZonalManager => "Zonal Manager",
                UserRole.Customer => "Customer",
                _ => "User"
            };

            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #dc3545; color: white; padding: 20px; text-align: center; border-radius: 5px 5px 0 0; }}
        .content {{ background-color: #f9f9f9; padding: 30px; border: 1px solid #ddd; }}
        .footer {{ background-color: #333; color: white; padding: 15px; text-align: center; border-radius: 0 0 5px 5px; font-size: 12px; }}
        .info-table {{ width: 100%; margin: 20px 0; border-collapse: collapse; }}
        .info-table td {{ padding: 8px; border-bottom: 1px solid #eee; }}
        .info-table td:first-child {{ font-weight: bold; width: 40%; }}
        .warning {{ background-color: #f8d7da; border-left: 4px solid #dc3545; padding: 15px; margin: 20px 0; }}
        .notice {{ background-color: #d1ecf1; border-left: 4px solid #0c5460; padding: 15px; margin: 20px 0; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>Account Deletion Notification</h1>
        </div>
        <div class=""content"">
            <h2>Hello {user.FirstName}!</h2>
            <p>This email confirms that your account has been permanently deleted from the Railway Ticket Booking System.</p>
            
            <div class=""warning"">
                <p><strong>Important:</strong> Your account and all associated data have been permanently removed from our system.</p>
            </div>

            <h3>Deleted Account Information:</h3>
            <table class=""info-table"">
                <tr>
                    <td>Full Name:</td>
                    <td>{user.FullName}</td>
                </tr>
                <tr>
                    <td>Email:</td>
                    <td>{user.Email}</td>
                </tr>
                <tr>
                    <td>Phone Number:</td>
                    <td>{user.PhoneNumber}</td>
                </tr>
                <tr>
                    <td>Role:</td>
                    <td>{roleName}</td>
                </tr>
                <tr>
                    <td>Account Created:</td>
                    <td>{user.CreatedAt:dd MMM, yyyy 'at' HH:mm} UTC</td>
                </tr>
                <tr>
                    <td>Account Deleted:</td>
                    <td>{DateTime.UtcNow:dd MMM, yyyy 'at' HH:mm} UTC</td>
                </tr>
            </table>

            <div class=""notice"">
                <h3 style=""margin-top: 0;"">What This Means:</h3>
                <ul>
                    <li>Your account has been permanently removed from our database</li>
                    <li>You will no longer be able to log in to the system</li>
                    <li>All your personal information has been deleted</li>
                    <li>If you had any bookings, they have been cancelled (if applicable)</li>
                </ul>
            </div>

            <p><strong>If you did not request this deletion:</strong> Please contact our support team immediately if you believe this account deletion was made in error or without your authorization.</p>
            
            <p>If you wish to use our services again in the future, you will need to create a new account.</p>
            
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
