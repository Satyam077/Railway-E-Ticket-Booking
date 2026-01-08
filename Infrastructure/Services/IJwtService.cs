using RailwayTicketBooking.Domain.Entities;
using System.Security.Claims;

namespace RailwayTicketBooking.Infrastructure.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user);
        ClaimsPrincipal? ValidateToken(string token);
    }
}

