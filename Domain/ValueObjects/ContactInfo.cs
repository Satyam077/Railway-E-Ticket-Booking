using System.ComponentModel.DataAnnotations;

namespace RailwayTicketBooking.Domain.ValueObjects
{
    public class ContactInfo
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        [StringLength(15)]
        public string AlternatePhone { get; set; }

        public bool IsEmailVerified { get; set; } = false;

        public bool IsPhoneVerified { get; set; } = false;

        public ContactInfo() { }

        public ContactInfo(string email, string phoneNumber, string alternatePhone = null)
        {
            Email = email;
            PhoneNumber = phoneNumber;
            AlternatePhone = alternatePhone;
        }

        public override bool Equals(object obj)
        {
            if (obj is not ContactInfo contact) return false;
            
            return Email == contact.Email &&
                   PhoneNumber == contact.PhoneNumber &&
                   AlternatePhone == contact.AlternatePhone;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Email, PhoneNumber, AlternatePhone);
        }
    }
}
