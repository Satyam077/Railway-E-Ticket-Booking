using MediatR;
using MongoDB.Driver;
using RailwayTicketBooking.Domain.Entities;
using RailwayTicketBooking.Domain.Enums;
using RailwayTicketBooking.Infrastructure;
using RailwayTicketBooking.Infrastructure.Services;
using System.Text;

namespace RailwayTicketBooking.Application.Bookings.Commands.Handlers
{
    public class CreateBookingHandler : IRequestHandler<CreateBookingCommand, string>
    {
        private readonly MongoDbContext _context;
        private readonly SeatAllocationService _seatAllocationService;

        public CreateBookingHandler(MongoDbContext context, SeatAllocationService seatAllocationService)
        {
            _context = context;
            _seatAllocationService = seatAllocationService;
        }

        public async Task<string> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
        {
            // Generate PNR
            var pnr = GeneratePNR();

            // Calculate amounts
            var baseAmount = request.ClassFare * request.Passengers.Count;
            var taxAmount = baseAmount * 0.18m; // 18% GST
            var serviceCharge = 20m; // Fixed service charge
            var finalAmount = baseAmount + taxAmount + serviceCharge;

            // Convert passengers
            var passengers = request.Passengers.Select(p => new Passenger
            {
                FirstName = p.FirstName,
                LastName = p.LastName,
                Age = p.Age,
                Gender = p.Gender,
                IdProofType = p.IdProofType,
                IdProofNumber = p.IdProofNumber,
                BerthPreference = p.BerthPreference,
                FoodPreference = p.FoodPreference
            }).ToList();

            // Parse seat class from SelectedClass string
            var seatClass = ParseSeatClass(request.SelectedClass);

            // Get existing booked seats for the same train and journey date
            var existingBookings = await _context.Bookings
                .Find(b => b.TrainId == request.TrainId &&
                           b.JourneyDate.Date == request.JourneyDate.Date &&
                           b.Status == BookingStatus.Confirmed &&
                           b.IsActive)
                .ToListAsync(cancellationToken);

            var existingBookedSeats = existingBookings
                .SelectMany(b => b.Seats ?? new List<BookedSeat>())
                .ToList();

            // Allocate seats using IRCTC-style allocation
            var allocatedSeats = await _seatAllocationService.AllocateSeatsAsync(
                request.TrainId,
                seatClass,
                request.JourneyDate,
                passengers,
                existingBookedSeats);

            // Create booking
            var booking = new Booking
            {
                PNR = pnr,
                UserId = request.UserId,
                TrainId = request.TrainId,
                ScheduleId = request.ScheduleId,
                SourceStationId = request.SourceStationId,
                DestinationStationId = request.DestinationStationId,
                JourneyDate = request.JourneyDate,
                Status = BookingStatus.Pending,
                Passengers = passengers,
                Seats = allocatedSeats,
                TotalAmount = baseAmount,
                TaxAmount = taxAmount,
                ServiceCharge = serviceCharge,
                FinalAmount = finalAmount,
                ContactEmail = request.ContactEmail,
                ContactPhone = request.ContactPhone,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.Bookings.InsertOneAsync(booking, cancellationToken: cancellationToken);
            return booking.Id;
        }

        /// <summary>
        /// Parses seat class string to SeatClass enum
        /// Handles various formats like "AC 3 Tier", "3A", "Sleeper Class", etc.
        /// </summary>
        private SeatClass ParseSeatClass(string selectedClass)
        {
            if (string.IsNullOrEmpty(selectedClass))
                return SeatClass.SleeperClass;

            var classLower = selectedClass.ToLower().Trim();
            
            // Try exact enum match first
            if (Enum.TryParse<SeatClass>(selectedClass, true, out var parsedClass))
                return parsedClass;
            
            // Handle common class name formats
            if (classLower.Contains("sleeper") || classLower.Contains("sl") || classLower == "sl")
                return SeatClass.SleeperClass;
            if (classLower.Contains("ac 3") || classLower.Contains("3a") || classLower.Contains("3 tier") || 
                classLower.Contains("ac3") || classLower == "3a" || classLower.Contains("ac iii"))
                return SeatClass.ACThreeTier;
            if (classLower.Contains("ac 2") || classLower.Contains("2a") || classLower.Contains("2 tier") ||
                classLower.Contains("ac2") || classLower == "2a" || classLower.Contains("ac ii"))
                return SeatClass.ACTwoTier;
            if (classLower.Contains("ac 1") || classLower.Contains("1a") || classLower.Contains("first") ||
                classLower.Contains("ac1") || classLower == "1a" || classLower.Contains("ac i"))
                return SeatClass.ACFirstClass;
            if (classLower.Contains("chair") || classLower.Contains("cc") || classLower == "cc")
                return SeatClass.ACChairCar;
            if (classLower.Contains("second") || classLower.Contains("2s") || classLower == "2s" ||
                classLower.Contains("general"))
                return SeatClass.SecondSitting;
            if (classLower.Contains("executive") || classLower.Contains("ec"))
                return SeatClass.ExecutiveClass;

            // Default to Sleeper
            return SeatClass.SleeperClass;
        }

        private string GeneratePNR()
        {
            // Generate 10-character PNR: 3 letters + 7 digits
            var random = new Random();
            var letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var digits = "0123456789";

            var pnr = new StringBuilder();
            
            // Add 3 random letters
            for (int i = 0; i < 3; i++)
            {
                pnr.Append(letters[random.Next(letters.Length)]);
            }

            // Add 7 random digits
            for (int i = 0; i < 7; i++)
            {
                pnr.Append(digits[random.Next(digits.Length)]);
            }

            return pnr.ToString();
        }
    }
}

