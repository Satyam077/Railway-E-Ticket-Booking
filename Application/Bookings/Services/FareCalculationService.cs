using Railway_Ticket_Booking.Domain.Entities;

namespace Railway_Ticket_Booking.Application.Bookings.Services
{
    public static class FareCalculationService
    {
        // Fare per kilometer for each class
        private static readonly Dictionary<string, decimal> FarePerKm = new Dictionary<string, decimal>
        {
            { "Sleeper", 0.75m },
            { "Second Sitting", 0.45m },
            { "AC Chair Car", 1.28m },
            { "Chair Car", 1.28m },
            { "AC 3 Tier", 1.20m },
            { "AC 2 Tier", 1.80m },
            { "AC First Class", 3.00m },
            { "General", 0.35m },
            { "General Unreserved", 0.35m },
            { "GN/UR", 0.35m },
            { "Executive Chair Car", 2.40m }
        };

        // Minimum fare for each class
        private static readonly Dictionary<string, decimal> MinimumFare = new Dictionary<string, decimal>
        {
            { "Sleeper", 120m },
            { "Second Sitting", 30m },
            { "AC Chair Car", 150m },
            { "Chair Car", 150m },
            { "AC 3 Tier", 300m },
            { "AC 2 Tier", 400m },
            { "AC First Class", 500m },
            { "General", 10m },
            { "General Unreserved", 10m },
            { "GN/UR", 10m },
            { "Executive Chair Car", 150m }
        };

        /// <summary>
        /// Calculates the base fare per passenger based on distance and class
        /// </summary>
        public static decimal CalculateBaseFare(string className, double distance)
        {
            // Get fare per km for the class
            var farePerKm = FarePerKm.ContainsKey(className) ? FarePerKm[className] : 1.00m;
            
            // Calculate fare based on distance
            var calculatedFare = (decimal)distance * farePerKm;
            
            // Get minimum fare for the class
            var minFare = MinimumFare.ContainsKey(className) ? MinimumFare[className] : 100m;
            
            // Return the maximum of calculated fare and minimum fare
            return Math.Max(calculatedFare, minFare);
        }

        /// <summary>
        /// Calculates superfast charge based on distance
        /// </summary>
        public static decimal CalculateSuperfastCharge(bool isSuperfast, double distance)
        {
            if (!isSuperfast)
                return 0m;

            if (distance < 500)
                return 45m;
            else
                return 75m;
        }

        /// <summary>
        /// Calculates total fare including all charges
        /// </summary>
        public static FareBreakdown CalculateTotalFare(
            string className,
            double distance,
            bool isSuperfast,
            int passengerCount,
            decimal? discountAmount = null,
            decimal? discountPercentage = null)
        {
            // Base fare per passenger
            var baseFarePerPassenger = CalculateBaseFare(className, distance);
            
            // Superfast charge per passenger
            var superfastChargePerPassenger = CalculateSuperfastCharge(isSuperfast, distance);
            
            // Total base amount (base fare + superfast charge) for all passengers
            var totalBaseAmount = (baseFarePerPassenger + superfastChargePerPassenger) * passengerCount;
            
            // Apply discount
            decimal discountAmountApplied = 0m;
            if (discountAmount.HasValue && discountAmount.Value > 0)
            {
                discountAmountApplied = Math.Min(discountAmount.Value, totalBaseAmount);
            }
            else if (discountPercentage.HasValue && discountPercentage.Value > 0)
            {
                discountAmountApplied = Math.Round(totalBaseAmount * (discountPercentage.Value / 100m), 2);
            }
            
            var amountAfterDiscount = totalBaseAmount - discountAmountApplied;
            
            // Tax (18% GST)
            var taxAmount = Math.Round(amountAfterDiscount * 0.18m, 2);
            
            // Service charge (fixed)
            var serviceCharge = 20m;
            
            // Final total
            var finalAmount = amountAfterDiscount + taxAmount + serviceCharge;
            
            return new FareBreakdown
            {
                BaseFarePerPassenger = baseFarePerPassenger,
                SuperfastChargePerPassenger = superfastChargePerPassenger,
                TotalBaseAmount = totalBaseAmount,
                DiscountAmount = discountAmountApplied,
                AmountAfterDiscount = amountAfterDiscount,
                TaxAmount = taxAmount,
                ServiceCharge = serviceCharge,
                TotalAmount = finalAmount,
                Distance = distance
            };
        }

        /// <summary>
        /// Calculates distance between two stations from route
        /// </summary>
        public static double CalculateDistanceBetweenStations(
            Railway_Ticket_Booking.Domain.Entities.Route route,
            string sourceStationId,
            string destinationStationId)
        {
            if (route?.Stations == null || !route.Stations.Any())
            {
                // If no stations in route, try to use TotalDistance if source/dest match
                if (route != null && 
                    route.SourceStationId == sourceStationId && 
                    route.DestinationStationId == destinationStationId)
                {
                    return route.TotalDistance;
                }
                return 0;
            }

            var sourceStation = route.Stations.FirstOrDefault(s => s.StationId == sourceStationId);
            var destStation = route.Stations.FirstOrDefault(s => s.StationId == destinationStationId);

            if (sourceStation == null || destStation == null)
            {
                // Fallback: if route source/dest match, use TotalDistance
                if (route.SourceStationId == sourceStationId && 
                    route.DestinationStationId == destinationStationId)
                {
                    return route.TotalDistance;
                }
                return 0;
            }

            // If stations are in order, calculate difference
            if (sourceStation.StationOrder < destStation.StationOrder)
            {
                return Math.Abs(destStation.DistanceFromSource - sourceStation.DistanceFromSource);
            }
            else if (sourceStation.StationOrder > destStation.StationOrder)
            {
                // Reverse direction
                return Math.Abs(sourceStation.DistanceFromSource - destStation.DistanceFromSource);
            }

            return 0;
        }
    } 
}

