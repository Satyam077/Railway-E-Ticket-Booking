using MediatR;
using MongoDB.Driver;
using Railway_Ticket_Booking.Domain.Entities;
using Railway_Ticket_Booking.Domain.Enums;
using Railway_Ticket_Booking.Infrastructure;
using Railway_Ticket_Booking.Infrastructure.Services;

namespace Railway_Ticket_Booking.Application.Bookings.Commands.Handlers
{
    public class CancelBookingHandler : IRequestHandler<CancelBookingCommand, CancelBookingResult>
    {
        private readonly MongoDbContext _context;
        private readonly CancellationService _cancellationService;

        public CancelBookingHandler(MongoDbContext context, CancellationService cancellationService)
        {
            _context = context;
            _cancellationService = cancellationService;
        }

        public async Task<CancelBookingResult> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Get booking
                var booking = await _context.Bookings
                    .Find(b => b.Id == request.BookingId)
                    .FirstOrDefaultAsync(cancellationToken);

                if (booking == null)
                {
                    return new CancelBookingResult
                    {
                        Success = false,
                        Message = "Booking not found."
                    };
                }

                // Verify user owns the booking
                if (booking.UserId != request.UserId)
                {
                    return new CancelBookingResult
                    {
                        Success = false,
                        Message = "You are not authorized to cancel this booking."
                    };
                }

                // Check if booking can be cancelled
                if (!_cancellationService.CanCancel(booking))
                {
                    return new CancelBookingResult
                    {
                        Success = false,
                        Message = "This booking cannot be cancelled. Cancellation is only allowed at least 4 hours before departure."
                    };
                }

                // Calculate refund
                var cancellationResult = _cancellationService.CalculateRefund(booking, DateTime.UtcNow);

                if (!cancellationResult.IsCancellable)
                {
                    return new CancelBookingResult
                    {
                        Success = false,
                        Message = cancellationResult.Reason
                    };
                }

                // Update booking status
                booking.Status = BookingStatus.Cancelled;
                booking.CancellationDate = DateTime.UtcNow;
                booking.CancellationReason = request.CancellationReason;
                booking.RefundAmount = cancellationResult.RefundAmount;
                booking.UpdatedAt = DateTime.UtcNow;

                // Free up seats (mark them as available)
                // Note: In a real system, you might want to update seat availability
                // For now, we'll just update the booking status

                // Update booking in database
                await _context.Bookings.ReplaceOneAsync(
                    b => b.Id == booking.Id,
                    booking,
                    cancellationToken: cancellationToken);

                // Process refund through payment gateway
                var refundTransactionId = await ProcessRefund(booking, cancellationResult.RefundAmount);

                return new CancelBookingResult
                {
                    Success = true,
                    Message = "Booking cancelled successfully. Refund will be processed within 5-7 business days.",
                    RefundAmount = cancellationResult.RefundAmount,
                    ProcessingDays = cancellationResult.ProcessingDays,
                    RefundTransactionId = refundTransactionId
                };
            }
            catch (Exception ex)
            {
                return new CancelBookingResult
                {
                    Success = false,
                    Message = $"Error cancelling booking: {ex.Message}"
                };
            }
        }

        private async Task<string> ProcessRefund(Booking booking, decimal refundAmount)
        {
            try
            {
                // Get payment record
                Payment payment = null;
                if (!string.IsNullOrEmpty(booking.PaymentId))
                {
                    payment = await _context.Payments
                        .Find(p => p.Id == booking.PaymentId)
                        .FirstOrDefaultAsync();
                }

                if (payment == null)
                {
                    // Try to find payment by booking ID
                    payment = await _context.Payments
                        .Find(p => p.BookingId == booking.Id)
                        .FirstOrDefaultAsync();
                }

                if (payment == null || payment.Status != PaymentStatus.Completed)
                {
                    // No payment found or payment not completed - still allow cancellation
                    // but mark refund as pending
                    return $"REFUND-{Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper()}";
                }

                // Create refund details
                var refundDetails = new RefundDetails
                {
                    RefundId = Guid.NewGuid(),
                    RefundTransactionId = $"REFUND-{Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper()}",
                    RefundAmount = refundAmount,
                    RefundStatus = PaymentStatus.Pending, // Will be updated when gateway confirms
                    RefundInitiatedAt = DateTime.UtcNow,
                    RefundReason = booking.CancellationReason,
                    RefundReference = booking.PNR,
                    ProcessingDays = 5
                };

                // Update payment with refund details
                payment.Refund = refundDetails;
                payment.Status = PaymentStatus.PartiallyRefunded; // Will be Refunded when complete
                payment.UpdatedAt = DateTime.UtcNow;

                await _context.Payments.ReplaceOneAsync(
                    p => p.Id == payment.Id,
                    payment);

                // TODO: Integrate with payment gateway (PayU) to process actual refund
                // For now, we'll just mark it as initiated
                // In production, you would call PayU refund API here

                return refundDetails.RefundTransactionId;
            }
            catch (Exception ex)
            {
                // Log error but don't fail cancellation
                Console.WriteLine($"Error processing refund: {ex.Message}");
                return $"REFUND-{Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper()}";
            }
        }
    }
}

