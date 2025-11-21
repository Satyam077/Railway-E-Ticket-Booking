using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Railway_Ticket_Booking.Domain.Entities;
using Railway_Ticket_Booking.Domain.Enums;
using Railway_Ticket_Booking.Infrastructure;
using Railway_Ticket_Booking.PayUServices;
using Railway_Ticket_Booking.WebSettings;
using System.Security.Cryptography;
using System.Text;

namespace Railway_Ticket_Booking.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class PayuController : ControllerBase
    {
        private readonly PayuService _payu;
        private readonly PayuOptions _opt;
        private readonly MongoDbContext _context;

        public PayuController(PayuService payu, IOptions<PayuOptions> opt, MongoDbContext context)
        {
            _payu = payu;
            _opt = opt.Value;
            _context = context;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentRequest request)
        {
            try
            {
                var booking = await _context.Bookings
                    .Find(b => b.Id == request.BookingId)
                    .FirstOrDefaultAsync();

                if (booking == null)
                {
                    return BadRequest(new { error = "Booking not found" });
                }

                var txnid = _payu.GenerateTxnId();

                // Create payment record
                var payment = new Payment
                {
                    BookingId = booking.Id,
                    UserId = booking.UserId,
                    TransactionId = txnid,
                    Method = PaymentMethod.DebitCard,
                    Status = PaymentStatus.Pending,
                    Amount = booking.FinalAmount,
                    NetAmount = booking.FinalAmount,
                    Currency = "INR",
                    GatewayName = "PayU",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _context.Payments.InsertOneAsync(payment);

                // Update booking with payment ID
                booking.PaymentId = payment.Id;
                await _context.Bookings.ReplaceOneAsync(
                    b => b.Id == booking.Id,
                    booking
                );

                // Prepare product info
                var productInfo = $"Railway Ticket - PNR: {booking.PNR}";

                var scheme = Request.Scheme; // http or https
                var host = Request.Host.Value; // localhost:5038 or localhost:7187
                var callbackUrl = $"{scheme}://{host}/api/payu/response";

                // Build payment form fields
                var fields = new Dictionary<string, string>
                {
                    {"key", _opt.Key},
                    {"txnid", txnid},
                    {"amount", booking.FinalAmount.ToString("0.00")},
                    {"productinfo", productInfo},
                    {"firstname", booking.ContactEmail.Split('@')[0]}, // Use email prefix as name
                    {"email", booking.ContactEmail},
                    {"phone", booking.ContactPhone},
                    {"surl", callbackUrl}, // Use dynamic URL
                    {"furl", callbackUrl}, // Use dynamic URL
                    {"udf1", booking.Id}, // Store booking ID for callback
                    {"udf2", payment.Id}  // Store payment ID for callback
                };

                var hash = _payu.GenerateHash(
                    _opt.Key, txnid, booking.FinalAmount.ToString("0.00"),
                    productInfo, fields["firstname"], booking.ContactEmail,
                    booking.Id,  // udf1
                    payment.Id   // udf2
                );

                fields["hash"] = hash;

                var form = _payu.BuildForm(fields);
                return Content(form, "text/html");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Error creating payment: {ex.Message}" });
            }
        }
        [HttpPost("response")]
        [HttpGet("response")]
        public async Task<IActionResult> PaymentResponse()
        {
            try
            {
                string GetValue(string key)
                {
                    if (Request.Form.ContainsKey(key))
                        return Request.Form[key].ToString();

                    if (Request.Query.ContainsKey(key))
                        return Request.Query[key].ToString();

                    return string.Empty;
                }

                var status = GetValue("status");
                var txnid = GetValue("txnid");
                var amount = GetValue("amount");
                var productinfo = GetValue("productinfo");
                var firstname = GetValue("firstname");
                var email = GetValue("email");
                var hash = GetValue("hash");
                var bookingId = GetValue("udf1");
                var paymentId = GetValue("udf2");

                if (string.IsNullOrEmpty(status) || string.IsNullOrEmpty(txnid))
                {
                    return Content("<h2>Payment Error</h2><p>Invalid payment response.</p>", "text/html");
                }

                var payment = await _context.Payments.Find(p => p.TransactionId == txnid).FirstOrDefaultAsync();

                if (payment == null)
                {
                    return Content("<h2>Payment Error</h2><p>No payment found.</p>", "text/html");
                }

                if (status == "success")
                {
                    payment.Status = PaymentStatus.Completed;
                    payment.UpdatedAt = DateTime.UtcNow;

                    var booking = await _context.Bookings.Find(b => b.Id == bookingId).FirstOrDefaultAsync();
                    booking.Status = BookingStatus.Confirmed;

                    await _context.Payments.ReplaceOneAsync(p => p.Id == payment.Id, payment);
                    await _context.Bookings.ReplaceOneAsync(b => b.Id == booking.Id, booking);
                    var redirectUrl = $"{Request.Scheme}://{Request.Host}/booking-confirmation/{booking.Id}";
                    return Redirect(redirectUrl);
                }
                else
                {
                    var booking = await _context.Bookings.Find(b => b.Id == bookingId).FirstOrDefaultAsync();
                    booking.Status = BookingStatus.Failed;
                    await _context.Bookings.ReplaceOneAsync(b => b.Id == booking.Id, booking);
                    var redirectUrl = $"{Request.Scheme}://{Request.Host}/booking/{booking.Id}?payment=failed";
                    return Redirect(redirectUrl);

                }
            }
            catch (Exception ex)
            {
                return Content($"<h2>Error</h2><p>{ex.Message}</p>");
            }
        }
    }

    public class PaymentRequest
    {
        public string BookingId { get; set; }
    }
}
