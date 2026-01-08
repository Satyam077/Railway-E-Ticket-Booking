using RailwayTicketBooking.Domain.Entities;
using RailwayTicketBooking.Domain.Enums;

namespace RailwayTicketBooking.Infrastructure.Services
{
    public static class SeatGenerator
    {
        /// <summary>
        /// Generates seats for a given train and class based on IRCTC patterns
        /// </summary>
        public static List<Seat> GenerateSeatsForClass(string trainId, SeatClass seatClass)
        {
            return seatClass switch
            {
                SeatClass.SleeperClass => GenerateSleeperCoaches(trainId),
                SeatClass.ACThreeTier => GenerateAC3TierCoaches(trainId),
                SeatClass.ACTwoTier => GenerateAC2TierCoaches(trainId),
                SeatClass.ACFirstClass => GenerateAC1TierCoaches(trainId),
                SeatClass.ChairCar => GenerateChairCarCoaches(trainId),
                SeatClass.ACChairCar => GenerateACChairCarCoaches(trainId),
                SeatClass.SecondSitting => GenerateSecondSittingCoaches(trainId),
                _ => GenerateSleeperCoaches(trainId) // Default to sleeper
            };
        }

        /// <summary>
        /// Generates Sleeper Class coaches (72 seats per coach, pattern: LB, MB, UB, LB, MB, UB, SL, SU)
        /// </summary>
        private static List<Seat> GenerateSleeperCoaches(string trainId)
        {
            var seats = new List<Seat>();
            string[] coachNumbers = { "S1", "S2", "S3", "S4", "S5", "S6", "S7", "S8", "S9", "S10" };
            string[] berthTypes = { "LB", "MB", "UB", "LB", "MB", "UB", "SL", "SU" }; // 8-seat cycle

            foreach (var coachNumber in coachNumbers)
            {
                for (int i = 1; i <= 72; i++)
                {
                    int index = (i - 1) % 8;
                    string berthType = berthTypes[index];
                    string seatNumber = $"{i}{berthType}";

                    seats.Add(new Seat
                    {
                        TrainId = trainId,
                        CoachNumber = coachNumber,
                        SeatNumber = seatNumber,
                        Class = SeatClass.SleeperClass,
                        Status = SeatStatus.Available,
                        Price = 0, // Will be set based on route
                        IsLowerBerth = berthType == "LB" || berthType == "SL",
                        IsMiddleBerth = berthType == "MB",
                        IsUpperBerth = berthType == "UB" || berthType == "SU",
                        IsWindowSeat = berthType == "SL" || berthType == "SU",
                        IsAisleSeat = berthType == "SU",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }
            }

            return seats;
        }

        /// <summary>
        /// Generates AC 3 Tier coaches (64 seats per coach, pattern: LB, MB, UB, LB, MB, UB, SL, SU)
        /// </summary>
        private static List<Seat> GenerateAC3TierCoaches(string trainId)
        {
            var seats = new List<Seat>();
            string[] coachNumbers = { "B1", "B2", "B3", "B4", "B5", "B6", "B7", "B8" };
            string[] berthTypes = { "LB", "MB", "UB", "LB", "MB", "UB", "SL", "SU" };

            foreach (var coachNumber in coachNumbers)
            {
                for (int i = 1; i <= 64; i++)
                {
                    int index = (i - 1) % 8;
                    string berthType = berthTypes[index];
                    string seatNumber = $"{i}{berthType}";

                    seats.Add(new Seat
                    {
                        TrainId = trainId,
                        CoachNumber = coachNumber,
                        SeatNumber = seatNumber,
                        Class = SeatClass.ACThreeTier,
                        Status = SeatStatus.Available,
                        Price = 0,
                        IsLowerBerth = berthType == "LB" || berthType == "SL",
                        IsMiddleBerth = berthType == "MB",
                        IsUpperBerth = berthType == "UB" || berthType == "SU",
                        IsWindowSeat = berthType == "SL" || berthType == "SU",
                        IsAisleSeat = berthType == "SU",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }
            }

            return seats;
        }

        /// <summary>
        /// Generates AC 2 Tier coaches (46 seats per coach, pattern: LB, UB, LB, UB, SL, SU)
        /// </summary>
        private static List<Seat> GenerateAC2TierCoaches(string trainId)
        {
            var seats = new List<Seat>();
            string[] coachNumbers = { "A1", "A2", "A3", "A4", "A5", "A6" };
            string[] berthTypes = { "LB", "UB", "LB", "UB", "SL", "SU" }; // 6-seat cycle

            foreach (var coachNumber in coachNumbers)
            {
                for (int i = 1; i <= 46; i++)
                {
                    int index = (i - 1) % 6;
                    string berthType = berthTypes[index];
                    string seatNumber = $"{i}{berthType}";

                    seats.Add(new Seat
                    {
                        TrainId = trainId,
                        CoachNumber = coachNumber,
                        SeatNumber = seatNumber,
                        Class = SeatClass.ACTwoTier,
                        Status = SeatStatus.Available,
                        Price = 0,
                        IsLowerBerth = berthType == "LB" || berthType == "SL",
                        IsUpperBerth = berthType == "UB" || berthType == "SU",
                        IsWindowSeat = berthType == "SL" || berthType == "SU",
                        IsAisleSeat = berthType == "SU",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }
            }

            return seats;
        }

        /// <summary>
        /// Generates AC First Class coaches (18 seats per coach, pattern: LB, UB, LB, UB)
        /// </summary>
        private static List<Seat> GenerateAC1TierCoaches(string trainId)
        {
            var seats = new List<Seat>();
            string[] coachNumbers = { "H1", "H2", "H3" };
            string[] berthTypes = { "LB", "UB", "LB", "UB" }; // 4-seat cycle

            foreach (var coachNumber in coachNumbers)
            {
                for (int i = 1; i <= 18; i++)
                {
                    int index = (i - 1) % 4;
                    string berthType = berthTypes[index];
                    string seatNumber = $"{i}{berthType}";

                    seats.Add(new Seat
                    {
                        TrainId = trainId,
                        CoachNumber = coachNumber,
                        SeatNumber = seatNumber,
                        Class = SeatClass.ACFirstClass,
                        Status = SeatStatus.Available,
                        Price = 0,
                        IsLowerBerth = berthType == "LB",
                        IsUpperBerth = berthType == "UB",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }
            }

            return seats;
        }

        /// <summary>
        /// Generates Chair Car coaches (78 seats per coach, numbered 1-78)
        /// </summary>
        private static List<Seat> GenerateChairCarCoaches(string trainId)
        {
            var seats = new List<Seat>();
            string[] coachNumbers = { "C1", "C2", "C3", "C4", "C5", "C6" };

            foreach (var coachNumber in coachNumbers)
            {
                for (int i = 1; i <= 78; i++)
                {
                    // Window seats: 1, 2, 3, 4, 77, 78 and every 4th seat pattern
                    bool isWindow = (i % 4 == 1 || i % 4 == 0) || i <= 4 || i >= 77;
                    bool isAisle = (i % 4 == 2 || i % 4 == 3);

                    seats.Add(new Seat
                    {
                        TrainId = trainId,
                        CoachNumber = coachNumber,
                        SeatNumber = i.ToString(),
                        Class = SeatClass.ChairCar,
                        Status = SeatStatus.Available,
                        Price = 0,
                        IsWindowSeat = isWindow,
                        IsAisleSeat = isAisle,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }
            }

            return seats;
        }

        /// <summary>
        /// Generates AC Chair Car coaches (78 seats per coach, numbered 1-78)
        /// </summary>
        private static List<Seat> GenerateACChairCarCoaches(string trainId)
        {
            var seats = new List<Seat>();
            string[] coachNumbers = { "CC1", "CC2", "CC3", "CC4", "CC5", "CC6" };

            foreach (var coachNumber in coachNumbers)
            {
                for (int i = 1; i <= 78; i++)
                {
                    bool isWindow = (i % 4 == 1 || i % 4 == 0) || i <= 4 || i >= 77;
                    bool isAisle = (i % 4 == 2 || i % 4 == 3);

                    seats.Add(new Seat
                    {
                        TrainId = trainId,
                        CoachNumber = coachNumber,
                        SeatNumber = i.ToString(),
                        Class = SeatClass.ACChairCar,
                        Status = SeatStatus.Available,
                        Price = 0,
                        IsWindowSeat = isWindow,
                        IsAisleSeat = isAisle,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }
            }

            return seats;
        }

        /// <summary>
        /// Generates Second Sitting coaches (108 seats per coach, numbered 1-108)
        /// </summary>
        private static List<Seat> GenerateSecondSittingCoaches(string trainId)
        {
            var seats = new List<Seat>();
            string[] coachNumbers = { "GS1", "GS2", "GS3", "GS4", "GS5", "GS6", "GS7", "GS8" };

            foreach (var coachNumber in coachNumbers)
            {
                for (int i = 1; i <= 108; i++)
                {
                    bool isWindow = (i % 4 == 1 || i % 4 == 0);
                    bool isAisle = (i % 4 == 2 || i % 4 == 3);

                    seats.Add(new Seat
                    {
                        TrainId = trainId,
                        CoachNumber = coachNumber,
                        SeatNumber = i.ToString(),
                        Class = SeatClass.SecondSitting,
                        Status = SeatStatus.Available,
                        Price = 0,
                        IsWindowSeat = isWindow,
                        IsAisleSeat = isAisle,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }
            }

            return seats;
        }
    }
}

