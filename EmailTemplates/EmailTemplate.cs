using Railway_Ticket_Booking.Domain.Entities;
using Railway_Ticket_Booking.Domain.Enums;

namespace Railway_Ticket_Booking.EmailTemplates
{
    public static class EmailTemplate
    {
        public static string GenerateRegistrationEmail(User user, UserRole role)
        {
            var roleName = role switch
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
        .header {{ background-color: #0066cc; color: white; padding: 20px; text-align: center; border-radius: 5px 5px 0 0; }}
        .content {{ background-color: #f9f9f9; padding: 30px; border: 1px solid #ddd; }}
        .footer {{ background-color: #333; color: white; padding: 15px; text-align: center; border-radius: 0 0 5px 5px; font-size: 12px; }}
        .info-table {{ width: 100%; margin: 20px 0; }}
        .info-table td {{ padding: 8px; border-bottom: 1px solid #eee; }}
        .info-table td:first-child {{ font-weight: bold; width: 40%; }}
        .button {{ display: inline-block; padding: 12px 30px; background-color: #0066cc; color: white; text-decoration: none; border-radius: 5px; margin-top: 20px; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>Welcome to Railway Ticket Booking System</h1>
        </div>
        <div class=""content"">
            <h2>Hello {user.FirstName}!</h2>
            <p>Your account has been successfully created. We're excited to have you on board!</p>
            
            <h3>Account Details:</h3>
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
                    <td>Account Status:</td>
                    <td>{(user.IsActive ? "Active" : "Inactive")}</td>
                </tr>
                <tr>
                    <td>Date of Birth:</td>
                    <td>{user.DateOfBirth:dd MMM, yyyy}</td>
                </tr>
                <tr>
                    <td>Gender:</td>
                    <td>{user.Gender ?? "Not specified"}</td>
                </tr>
                <tr>
                    <td>Address:</td>
                    <td>{user.Address ?? "Not provided"}, {user.City ?? ""}, {user.State ?? ""} - {user.PinCode ?? ""}, {user.Country ?? "India"}</td>
                </tr>
                <tr>
                    <td>Account Created:</td>
                    <td>{user.CreatedAt:dd MMM, yyyy 'at' HH:mm} UTC</td>
                </tr>
            </table>

            <p><strong>Important:</strong> Please keep your login credentials secure and do not share them with anyone.</p>
            
            <p>You can now log in to your account and start booking railway tickets.</p>
            
            <a href=""#"" class=""button"">Login to Your Account</a>
            
            <p style=""margin-top: 30px; padding-top: 20px; border-top: 1px solid #ddd; font-size: 12px; color: #666;"">
                If you did not create this account, please contact our support team immediately.
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
