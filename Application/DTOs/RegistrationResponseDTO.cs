namespace Railway_Ticket_Booking.Application.DTOs
{
    public class RegistrationResponseDTO
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}

