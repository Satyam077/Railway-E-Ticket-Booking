using Railway_Ticket_Booking.Domain.Entities;
using Railway_Ticket_Booking.Domain.Enums;
using MongoDB.Driver;

namespace Railway_Ticket_Booking.Infrastructure.Services
{
    public class SeatAllocationService
    {
        private readonly MongoDbContext _context;

        public SeatAllocationService(MongoDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Allocates seats for passengers based on IRCTC-style allocation logic
        /// </summary>
        public async Task<List<BookedSeat>> AllocateSeatsAsync(
            string trainId,
            SeatClass seatClass,
            DateTime journeyDate,
            List<Passenger> passengers,
            List<BookedSeat> existingBookedSeats)
        {
            var allocatedSeats = new List<BookedSeat>();

            // Get all available seats for the train and class
            var allSeats = await _context.Seats
                .Find(s => s.TrainId == trainId && s.Class == seatClass && s.Status == SeatStatus.Available)
                .ToListAsync();

            if (!allSeats.Any())
            {
                // If no seats exist, generate them (first time allocation)
                await GenerateSeatsIfNotExists(trainId, seatClass);
                allSeats = await _context.Seats
                    .Find(s => s.TrainId == trainId && s.Class == seatClass && s.Status == SeatStatus.Available)
                    .ToListAsync();
            }

            // Get already booked seats for the journey date
            var bookedSeatNumbers = existingBookedSeats?
                .Select(bs => $"{bs.CoachNumber}-{bs.SeatNumber}")
                .ToHashSet() ?? new HashSet<string>();

            // Filter available seats
            var availableSeats = allSeats
                .Where(s => !bookedSeatNumbers.Contains($"{s.CoachNumber}-{s.SeatNumber}"))
                .OrderBy(s => s.CoachNumber)
                .ThenBy(s => ExtractSeatNumber(s.SeatNumber))
                .ToList();

            // Group passengers by preference for better allocation
            var passengersByPreference = passengers
                .Select((p, index) => new { Passenger = p, Index = index })
                .OrderByDescending(x => GetPreferencePriority(x.Passenger.BerthPreference))
                .ThenBy(x => x.Passenger.Age) // Prioritize seniors and children
                .ToList();

            // Try to allocate seats together for same booking
            var allocatedSeatNumbers = new HashSet<string>();

            foreach (var passengerGroup in passengersByPreference)
            {
                var passenger = passengerGroup.Passenger;
                var allocatedSeat = AllocateSeatForPassenger(
                    availableSeats,
                    allocatedSeatNumbers,
                    passenger,
                    seatClass);

                if (allocatedSeat != null)
                {
                    var bookedSeat = new BookedSeat
                    {
                        SeatId = allocatedSeat.Id,
                        PassengerId = passenger.PassengerId.ToString(),
                        CoachNumber = allocatedSeat.CoachNumber,
                        SeatNumber = allocatedSeat.SeatNumber,
                        Class = seatClass,
                        Price = allocatedSeat.Price
                    };

                    allocatedSeats.Add(bookedSeat);
                    allocatedSeatNumbers.Add($"{allocatedSeat.CoachNumber}-{allocatedSeat.SeatNumber}");

                    // Update seat status to Booked
                    allocatedSeat.Status = SeatStatus.Booked;
                    await _context.Seats.ReplaceOneAsync(
                        s => s.Id == allocatedSeat.Id,
                        allocatedSeat);
                }
                else
                {
                    // If no seat available, still create booking but without seat assignment
                    // This allows waitlist functionality in future
                    var bookedSeat = new BookedSeat
                    {
                        PassengerId = passenger.PassengerId.ToString(),
                        CoachNumber = "WL", // Waitlist
                        SeatNumber = "0",
                        Class = seatClass,
                        Price = 0
                    };
                    allocatedSeats.Add(bookedSeat);
                }
            }

            return allocatedSeats;
        }

        /// <summary>
        /// Allocates a single seat for a passenger based on preferences
        /// </summary>
        private Seat AllocateSeatForPassenger(
            List<Seat> availableSeats,
            HashSet<string> alreadyAllocated,
            Passenger passenger,
            SeatClass seatClass)
        {
            var preference = passenger.BerthPreference?.ToLower() ?? "";

            // Filter seats that are not already allocated in this booking
            var candidateSeats = availableSeats
                .Where(s => !alreadyAllocated.Contains($"{s.CoachNumber}-{s.SeatNumber}"))
                .ToList();

            // Try to match preference
            Seat selectedSeat = null;

            if (!string.IsNullOrEmpty(preference))
            {
                if (preference.Contains("lower"))
                {
                    selectedSeat = candidateSeats
                        .FirstOrDefault(s => s.IsLowerBerth || s.SeatNumber.EndsWith("LB"));
                }
                else if (preference.Contains("middle"))
                {
                    selectedSeat = candidateSeats
                        .FirstOrDefault(s => s.IsMiddleBerth || s.SeatNumber.EndsWith("MB"));
                }
                else if (preference.Contains("upper"))
                {
                    selectedSeat = candidateSeats
                        .FirstOrDefault(s => s.IsUpperBerth || s.SeatNumber.EndsWith("UB"));
                }
                else if (preference.Contains("window"))
                {
                    selectedSeat = candidateSeats
                        .FirstOrDefault(s => s.IsWindowSeat || s.SeatNumber.EndsWith("SL") || s.SeatNumber.EndsWith("SU"));
                }
                else if (preference.Contains("aisle"))
                {
                    selectedSeat = candidateSeats
                        .FirstOrDefault(s => s.IsAisleSeat || s.SeatNumber.EndsWith("SU"));
                }
            }

            // If preference not found or no preference, allocate any available seat
            if (selectedSeat == null)
            {
                selectedSeat = candidateSeats.FirstOrDefault();
            }

            return selectedSeat;
        }

        /// <summary>
        /// Generates seats for a coach if they don't exist
        /// </summary>
        private async Task GenerateSeatsIfNotExists(string trainId, SeatClass seatClass)
        {
            var existingSeats = await _context.Seats
                .Find(s => s.TrainId == trainId && s.Class == seatClass)
                .AnyAsync();

            if (existingSeats)
                return;

            var seats = SeatGenerator.GenerateSeatsForClass(trainId, seatClass);
            if (seats.Any())
            {
                await _context.Seats.InsertManyAsync(seats);
            }
        }

        /// <summary>
        /// Extracts numeric part from seat number (e.g., "12LB" -> 12)
        /// </summary>
        private int ExtractSeatNumber(string seatNumber)
        {
            if (string.IsNullOrEmpty(seatNumber))
                return 0;

            var numericPart = new string(seatNumber.TakeWhile(char.IsDigit).ToArray());
            return int.TryParse(numericPart, out var number) ? number : 0;
        }

        /// <summary>
        /// Gets priority for berth preference (higher number = higher priority)
        /// </summary>
        private int GetPreferencePriority(string preference)
        {
            if (string.IsNullOrEmpty(preference))
                return 0;

            var pref = preference.ToLower();
            if (pref.Contains("lower")) return 5; // Highest priority
            if (pref.Contains("window")) return 4;
            if (pref.Contains("middle")) return 3;
            if (pref.Contains("upper")) return 2;
            if (pref.Contains("aisle")) return 1;
            return 0;
        }
    }
}

