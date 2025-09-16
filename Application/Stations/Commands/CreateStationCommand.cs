using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Railway_Ticket_Booking.Application.Stations.Commands
{
    public class CreateStationCommand : IRequest<string>
    {
        [Required]
        [StringLength(10)]
        public string StationCode { get; set; }

        [Required]
        [StringLength(100)]
        public string StationName { get; set; }

        [StringLength(50)]
        public string City { get; set; }

        [StringLength(50)]
        public string State { get; set; }

        [StringLength(50)]
        public string Country { get; set; } = "India";

        [Range(-90, 90)]
        public double Latitude { get; set; }

        [Range(-180, 180)]
        public double Longitude { get; set; }
    }
}
