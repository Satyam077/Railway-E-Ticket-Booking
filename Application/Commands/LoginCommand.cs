using MediatR;
using Railway_Ticket_Booking.Application.DTOs;
using System.ComponentModel.DataAnnotations;

namespace Railway_Ticket_Booking.Application.Commands
{
    public class LoginCommand : IRequest<LoginResponseDTO>
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; }
    }
}

