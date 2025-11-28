using MongoDB.Driver;
using Railway_Ticket_Booking.Domain.Entities;
using Railway_Ticket_Booking.EmailServices;
using Railway_Ticket_Booking.EmailTemplates;
using Railway_Ticket_Booking.Infrastructure;

namespace Railway_Ticket_Booking.Infrastructure.Services
{
    public class BookingEmailService
    {
        private readonly MongoDbContext _context;
        private readonly IEmailService _emailService;

        public BookingEmailService(MongoDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task SendBookingConfirmationEmailAsync(string bookingId)
        {
            try
            {
                var booking = await _context.Bookings
                    .Find(b => b.Id == bookingId)
                    .FirstOrDefaultAsync();

                if (booking == null)
                {
                    Console.WriteLine($"Booking not found: {bookingId}");
                    return;
                }

                var paymentTask = LoadPaymentAsync(booking);
                var trainTask = LoadTrainAsync(booking);
                var scheduleTask = LoadScheduleAsync(booking);
                var sourceStationTask = LoadStationAsync(booking.SourceStationId);
                var destinationStationTask = LoadStationAsync(booking.DestinationStationId);

                await Task.WhenAll(paymentTask, trainTask, scheduleTask, sourceStationTask, destinationStationTask);

                var payment = await paymentTask;
                var train = await trainTask;
                var schedule = await scheduleTask;
                var sourceStation = await sourceStationTask;
                var destinationStation = await destinationStationTask;

                var emailHtml = EmailTemplate.GenerateBookingConfirmationEmail(
                    booking,
                    payment,
                    train,
                    schedule,
                    sourceStation,
                    destinationStation);

                // Send email
                var subject = $"Booking Confirmed - PNR: {booking.PNR} | Railway Ticket Booking";
                await _emailService.SendEmailAsync(booking.ContactEmail, subject, emailHtml);

                Console.WriteLine($"Booking confirmation email sent to {booking.ContactEmail} for PNR: {booking.PNR}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending booking confirmation email: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        private async Task<Payment> LoadPaymentAsync(Booking booking)
        {
            if (string.IsNullOrEmpty(booking.PaymentId))
            {
                // Try to find payment by booking ID
                return await _context.Payments
                    .Find(p => p.BookingId == booking.Id)
                    .FirstOrDefaultAsync();
            }

            return await _context.Payments
                .Find(p => p.Id == booking.PaymentId)
                .FirstOrDefaultAsync();
        }

        private async Task<Train> LoadTrainAsync(Booking booking)
        {
            if (string.IsNullOrEmpty(booking.TrainId))
                return null;

            return await _context.Trains
                .Find(t => t.Id == booking.TrainId)
                .FirstOrDefaultAsync();
        }

        private async Task<TrainSchedule> LoadScheduleAsync(Booking booking)
        {
            if (string.IsNullOrEmpty(booking.ScheduleId))
                return null;

            return await _context.TrainSchedules
                .Find(s => s.Id == booking.ScheduleId)
                .FirstOrDefaultAsync();
        }

        private async Task<Station> LoadStationAsync(string stationId)
        {
            if (string.IsNullOrEmpty(stationId))
                return null;

            return await _context.Stations
                .Find(s => s.Id == stationId)
                .FirstOrDefaultAsync();
        }

        public async Task SendCancellationEmailAsync(string bookingId, decimal refundAmount, decimal cancellationCharge)
        {
            try
            {
                var booking = await _context.Bookings
                    .Find(b => b.Id == bookingId)
                    .FirstOrDefaultAsync();

                if (booking == null)
                {
                    Console.WriteLine($"Booking not found: {bookingId}");
                    return;
                }

                var paymentTask = LoadPaymentAsync(booking);
                var trainTask = LoadTrainAsync(booking);
                var scheduleTask = LoadScheduleAsync(booking);
                var sourceStationTask = LoadStationAsync(booking.SourceStationId);
                var destinationStationTask = LoadStationAsync(booking.DestinationStationId);

                await Task.WhenAll(paymentTask, trainTask, scheduleTask, sourceStationTask, destinationStationTask);

                var payment = await paymentTask;
                var train = await trainTask;
                var schedule = await scheduleTask;
                var sourceStation = await sourceStationTask;
                var destinationStation = await destinationStationTask;

                var emailHtml = EmailTemplate.GenerateCancellationEmail(
                    booking,
                    payment,
                    train,
                    schedule,
                    sourceStation,
                    destinationStation,
                    refundAmount,
                    cancellationCharge);

                // Send email
                var subject = $"Booking Cancelled - PNR: {booking.PNR} | Railway Ticket Booking";
                await _emailService.SendEmailAsync(booking.ContactEmail, subject, emailHtml);

                Console.WriteLine($"Cancellation email sent to {booking.ContactEmail} for PNR: {booking.PNR}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending cancellation email: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        public async Task SendRefundEmailAsync(string bookingId, decimal refundAmount, string refundTransactionId)
        {
            try
            {
                var booking = await _context.Bookings
                    .Find(b => b.Id == bookingId)
                    .FirstOrDefaultAsync();

                if (booking == null)
                {
                    Console.WriteLine($"Booking not found: {bookingId}");
                    return;
                }

                var paymentTask = LoadPaymentAsync(booking);
                var trainTask = LoadTrainAsync(booking);
                var sourceStationTask = LoadStationAsync(booking.SourceStationId);
                var destinationStationTask = LoadStationAsync(booking.DestinationStationId);

                await Task.WhenAll(paymentTask, trainTask, sourceStationTask, destinationStationTask);

                var payment = await paymentTask;
                var train = await trainTask;
                var sourceStation = await sourceStationTask;
                var destinationStation = await destinationStationTask;

                var emailHtml = EmailTemplate.GenerateRefundEmail(
                    booking,
                    payment,
                    train,
                    sourceStation,
                    destinationStation,
                    refundAmount,
                    refundTransactionId);

                // Send email
                var subject = $"Refund Processed - PNR: {booking.PNR} | Railway Ticket Booking";
                await _emailService.SendEmailAsync(booking.ContactEmail, subject, emailHtml);

                Console.WriteLine($"Refund email sent to {booking.ContactEmail} for PNR: {booking.PNR}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending refund email: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }
    }
}

