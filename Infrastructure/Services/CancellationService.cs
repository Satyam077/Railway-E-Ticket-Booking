using RailwayTicketBooking.Domain.Entities;
using RailwayTicketBooking.Domain.Enums;

namespace RailwayTicketBooking.Infrastructure.Services
{
    public class CancellationService
    {
        public CancellationResult CalculateRefund(Booking booking, DateTime cancellationTime)
        {
            if (booking == null)
                throw new ArgumentNullException(nameof(booking));

            var result = new CancellationResult
            {
                BookingId = booking.Id,
                CancellationTime = cancellationTime,
                OriginalAmount = booking.FinalAmount,
                BaseFare = booking.TotalAmount,
                TaxAmount = booking.TaxAmount,
                ServiceCharge = booking.ServiceCharge
            };

            // Check if booking is cancellable
            if (booking.Status != BookingStatus.Confirmed)
            {
                result.IsCancellable = false;
                result.Reason = $"Booking status is {booking.Status}, only Confirmed bookings can be cancelled.";
                return result;
            }

            // Calculate time before departure
            var departureTime = booking.JourneyDate;
            var timeBeforeDeparture = departureTime - cancellationTime;
            var hoursBeforeDeparture = timeBeforeDeparture.TotalHours;

            // IRCTC Rule: Cannot cancel less than 4 hours before departure
            if (hoursBeforeDeparture < 4)
            {
                result.IsCancellable = false;
                result.Reason = "Cancellation not allowed less than 4 hours before departure as per IRCTC rules.";
                return result;
            }

            result.IsCancellable = true;

            // Calculate cancellation charges based on IRCTC rules
            decimal cancellationCharge = 0;
            decimal refundPercentage = 0;

            if (hoursBeforeDeparture > 48)
            {
                // More than 48 hours before departure: Flat cancellation charges based on class
                cancellationCharge = GetFlatCancellationCharge(booking.Seats?.FirstOrDefault()?.Class ?? SeatClass.SleeperClass);
                refundPercentage = 100; // Full refund after flat charge
            }
            else if (hoursBeforeDeparture > 12)
            {
                // Between 48 and 12 hours: 25% of fare deducted (minimum cancellation charge applies)
                var chargePercentage = 0.25m;
                var calculatedCharge = booking.TotalAmount * chargePercentage;
                var minCharge = GetFlatCancellationCharge(booking.Seats?.FirstOrDefault()?.Class ?? SeatClass.SleeperClass);
                cancellationCharge = Math.Max(calculatedCharge, minCharge);
                refundPercentage = 100; // Full refund after charge
            }
            else if (hoursBeforeDeparture > 4)
            {
                // Between 12 and 4 hours: 50% of fare deducted (minimum cancellation charge applies)
                var chargePercentage = 0.50m;
                var calculatedCharge = booking.TotalAmount * chargePercentage;
                var minCharge = GetFlatCancellationCharge(booking.Seats?.FirstOrDefault()?.Class ?? SeatClass.SleeperClass);
                cancellationCharge = Math.Max(calculatedCharge, minCharge);
                refundPercentage = 100; // Full refund after charge
            }

            // Calculate refund amounts
            // Service charge and tax are not refunded
            var refundableAmount = booking.TotalAmount - cancellationCharge;
            
            // Ensure refund is not negative
            if (refundableAmount < 0)
                refundableAmount = 0;

            result.CancellationCharge = cancellationCharge;
            result.RefundAmount = refundableAmount;
            result.RefundPercentage = refundPercentage;
            result.ProcessingDays = 5; // Standard IRCTC refund processing time

            // Calculate breakdown
            result.RefundBreakdown = new RefundBreakdown
            {
                BaseFare = booking.TotalAmount,
                CancellationCharge = cancellationCharge,
                RefundableFare = refundableAmount,
                TaxRefund = 0, // Tax is not refunded
                ServiceChargeRefund = 0, // Service charge is not refunded
                TotalRefund = refundableAmount
            };

            return result;
        }

        /// <summary>
        /// Gets flat cancellation charges based on seat class (IRCTC rules)
        /// </summary>
        private decimal GetFlatCancellationCharge(SeatClass seatClass)
        {
            return seatClass switch
            {
                SeatClass.ACFirstClass => 200m,
                SeatClass.ACTwoTier => 180m,
                SeatClass.ACThreeTier => 120m,
                SeatClass.ACChairCar => 120m,
                SeatClass.ExecutiveClass => 120m,
                SeatClass.SleeperClass => 60m,
                SeatClass.SecondSitting => 30m,
                SeatClass.ChairCar => 60m,
                _ => 60m // Default to Sleeper class charge
            };
        }

        /// <summary>
        /// Checks if a booking can be cancelled based on IRCTC rules
        /// </summary>
        public bool CanCancel(Booking booking)
        {
            if (booking == null)
                return false;

            if (booking.Status != BookingStatus.Confirmed)
                return false;

            // Must be at least 4 hours before departure
            var departureTime = booking.JourneyDate;
            var timeBeforeDeparture = departureTime - DateTime.Now;
            
            return timeBeforeDeparture.TotalHours >= 4;
        }

        /// <summary>
        /// Gets cancellation charges information for display
        /// </summary>
        public string GetCancellationChargesInfo(Booking booking)
        {
            if (!CanCancel(booking))
                return "Cancellation not allowed.";

            var departureTime = booking.JourneyDate;
            var timeBeforeDeparture = departureTime - DateTime.Now;
            var hoursBeforeDeparture = timeBeforeDeparture.TotalHours;

            if (hoursBeforeDeparture > 48)
            {
                var flatCharge = GetFlatCancellationCharge(booking.Seats?.FirstOrDefault()?.Class ?? SeatClass.SleeperClass);
                return $"Flat cancellation charge: ₹{flatCharge} per passenger (Full refund after charges)";
            }
            else if (hoursBeforeDeparture > 12)
            {
                return "25% of fare deducted (minimum cancellation charge applies). Full refund after charges.";
            }
            else if (hoursBeforeDeparture > 4)
            {
                return "50% of fare deducted (minimum cancellation charge applies). Full refund after charges.";
            }

            return "Cancellation not allowed less than 4 hours before departure.";
        }
    }
}

