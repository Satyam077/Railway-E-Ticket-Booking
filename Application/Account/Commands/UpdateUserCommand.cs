using MediatR;
using Railway_Ticket_Booking.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Railway_Ticket_Booking.Application.Account.Commands
{
    public class UpdateUserCommand : IRequest<bool>
    {
        [Required]
        public string Id { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        // Optional password update - if empty, don't update password
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }

        public UserRole Role { get; set; } = UserRole.Customer;

        [Range(typeof(DateTime), "01/01/1950", "01/01/2100", ErrorMessage = "Date of Birth must be after 01/01/1950")]
        public DateTime DateOfBirth { get; set; }

        [StringLength(10)]
        public string Gender { get; set; }

        [StringLength(200)]
        public string Address { get; set; }

        [StringLength(50)]
        public string City { get; set; }

        [StringLength(50)]
        public string State { get; set; }

        [StringLength(10)]
        public string PinCode { get; set; }

        [StringLength(50)]
        public string Country { get; set; } = "India";

        public bool IsActive { get; set; } = true;

        public bool IsEmailVerified { get; set; } = false;

        public bool IsPhoneVerified { get; set; } = false;
    }
}
