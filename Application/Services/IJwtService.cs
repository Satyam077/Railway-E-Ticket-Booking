using Railway_Ticket_Booking.Domain.Entities;
using System.Security.Claims;

namespace Railway_Ticket_Booking.Application.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user);
        ClaimsPrincipal? ValidateToken(string token);
    }
}

