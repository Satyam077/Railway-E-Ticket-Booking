using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Railway_Ticket_Booking.Domain.Entities;
using Railway_Ticket_Booking.Domain.Enums;
using Railway_Ticket_Booking.Infrastructure;
using Railway_Ticket_Booking.Infrastructure.Services;
using Railway_Ticket_Booking.WebSettings;

namespace Railway_Ticket_Booking.Application.Bookings.Commands.Handlers
{
    public class CancelBookingHandler : IRequestHandler<CancelBookingCommand, CancelBookingResult>
    {
        private readonly MongoDbContext _context;
        private readonly CancellationService _cancellationService;
        private readonly BookingEmailService _bookingEmailService;
        private readonly PayuService _payuService;

        public CancelBookingHandler(
            MongoDbContext context, 
            CancellationService cancellationService,
            BookingEmailService bookingEmailService,
            PayuService payuService)
        {
            _context = context;
            _cancellationService = cancellationService;
            _bookingEmailService = bookingEmailService;
            _payuService = payuService;
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

                // Send cancellation email asynchronously (fire and forget)
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _bookingEmailService.SendCancellationEmailAsync(
                            booking.Id, 
                            cancellationResult.RefundAmount, 
                            cancellationResult.CancellationCharge);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error sending cancellation email: {ex.Message}");
                    }
                });

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

                // Process refund through PayU
                string refundTransactionId;
                PaymentStatus refundStatus = PaymentStatus.Pending;

                if (!string.IsNullOrEmpty(payment.GatewayTransactionId) || !string.IsNullOrEmpty(payment.TransactionId))
                {
                    // Use GatewayTransactionId if available, otherwise use TransactionId
                    var txnId = !string.IsNullOrEmpty(payment.GatewayTransactionId) 
                        ? payment.GatewayTransactionId 
                        : payment.TransactionId;

                    Console.WriteLine($"Processing refund for transaction: {txnId}, Amount: {refundAmount}");

                    // Call PayU refund API
                    var refundResponse = await _payuService.ProcessRefundAsync(
                        txnId, 
                        refundAmount, 
                        booking.CancellationReason ?? "Booking cancellation");

                    Console.WriteLine($"PayU refund response - Success: {refundResponse.Success}, Message: {refundResponse.Message}, RefundTxnId: {refundResponse.RefundTransactionId}");

                    if (refundResponse.Success)
                    {
                        refundTransactionId = !string.IsNullOrEmpty(refundResponse.RefundTransactionId) 
                            ? refundResponse.RefundTransactionId 
                            : $"REFUND-{Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper()}";
                        refundStatus = PaymentStatus.Refunded;
                        
                        Console.WriteLine($"PayU refund successful: {refundTransactionId} for transaction: {txnId}");
                    }
                    else
                    {
                        // Refund initiated but pending confirmation
                        refundTransactionId = $"REFUND-{Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper()}";
                        refundStatus = PaymentStatus.Pending;
                        
                        Console.WriteLine($"PayU refund pending: {refundResponse.Message}");
                    }
                }
                else
                {
                    // No gateway transaction ID, create internal refund reference
                    refundTransactionId = $"REFUND-{Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper()}";
                    refundStatus = PaymentStatus.Pending;
                    Console.WriteLine($"No gateway transaction ID found, creating internal refund reference: {refundTransactionId}");
                }

                // Create refund details
                var refundDetails = new RefundDetails
                {
                    RefundId = Guid.NewGuid(),
                    RefundTransactionId = refundTransactionId,
                    RefundAmount = refundAmount,
                    RefundStatus = refundStatus,
                    RefundInitiatedAt = DateTime.UtcNow,
                    RefundReason = booking.CancellationReason,
                    RefundReference = booking.PNR,
                    ProcessingDays = 5
                };

                // Update refund completed date if refund was successful
                if (refundStatus == PaymentStatus.Refunded)
                {
                    refundDetails.RefundCompletedAt = DateTime.UtcNow;
                }

                // Update payment with refund details
                payment.Refund = refundDetails;
                
                // Update payment status based on refund amount
                if (refundAmount >= payment.Amount)
                {
                    payment.Status = PaymentStatus.Refunded;
                }
                else
                {
                    payment.Status = PaymentStatus.PartiallyRefunded;
                }
                
                payment.UpdatedAt = DateTime.UtcNow;

                Console.WriteLine($"Updating payment with refund details - PaymentId: {payment.Id}, RefundAmount: {refundAmount}, RefundStatus: {refundStatus}");

                await _context.Payments.ReplaceOneAsync(
                    p => p.Id == payment.Id,
                    payment);

                Console.WriteLine($"Payment updated successfully with refund information");

                // If refund was successful, send refund email
                if (refundStatus == PaymentStatus.Refunded)
                {
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await _bookingEmailService.SendRefundEmailAsync(
                                booking.Id, 
                                refundAmount, 
                                refundTransactionId);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error sending refund email: {ex.Message}");
                        }
                    });
                }

                return refundTransactionId;
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

