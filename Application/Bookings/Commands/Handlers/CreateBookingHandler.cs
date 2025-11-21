using MediatR;
using MongoDB.Driver;
using Railway_Ticket_Booking.Domain.Entities;
using Railway_Ticket_Booking.Domain.Enums;
using Railway_Ticket_Booking.Infrastructure;
using System.Text;

namespace Railway_Ticket_Booking.Application.Bookings.Commands.Handlers
{
    public class CreateBookingHandler : IRequestHandler<CreateBookingCommand, string>
    {
        private readonly MongoDbContext _context;

        public CreateBookingHandler(MongoDbContext context)
        {
            _context = context;
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
                Passengers = request.Passengers.Select(p => new Passenger
                {
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    Age = p.Age,
                    Gender = p.Gender,
                    IdProofType = p.IdProofType,
                    IdProofNumber = p.IdProofNumber,
                    BerthPreference = p.BerthPreference,
                    FoodPreference = p.FoodPreference
                }).ToList(),
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

