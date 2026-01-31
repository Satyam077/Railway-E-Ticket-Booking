using MediatR;
using RailwayTicketBooking.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using RailwayTicketBooking.Domain.DTOs;

namespace RailwayTicketBooking.Application.Account.Commands
{
    public class CreateUserCommand : IRequest<RegistrationResponseDTO>
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        //[Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }

        public UserRole Role { get; set; } = UserRole.Customer;

        [Range(typeof(DateTime), "01/01/1950", "01/01/2100", ErrorMessage = "Date of Birth must be after 01/01/1950")]
        public DateTime DateOfBirth { get; set; } = DateTime.Now;

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
    }
}
