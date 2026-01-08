using MediatR;
using RailwayTicketBooking.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace RailwayTicketBooking.Application.Bookings.Commands
{
    public class CreateBookingCommand : IRequest<string>
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string TrainId { get; set; }

        [Required]
        public string ScheduleId { get; set; }

        [Required]
        public string SourceStationId { get; set; }

        [Required]
        public string DestinationStationId { get; set; }

        [Required]
        public DateTime JourneyDate { get; set; }

        [Required]
        public List<PassengerInfo> Passengers { get; set; } = new List<PassengerInfo>();

        [Required]
        public string SelectedClass { get; set; }

        [Required]
        public decimal ClassFare { get; set; }

        [Required]
        public string ContactEmail { get; set; }

        [Required]
        public string ContactPhone { get; set; }

        public string Quota { get; set; } = "GENERAL";
    }

    public class PassengerInfo
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
        [Range(1, 120)]
        public int Age { get; set; }

        [Required]
        [StringLength(10)]
        public string Gender { get; set; }

        [StringLength(50)]
        public string IdProofType { get; set; }

        [StringLength(50)]
        public string IdProofNumber { get; set; }

        [StringLength(20)]
        public string BerthPreference { get; set; }

        [StringLength(50)]
        public string FoodPreference { get; set; }
    }
}

