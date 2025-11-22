using Microsoft.Extensions.Options;
using Railway_Ticket_Booking.Domain.Entities;
using Railway_Ticket_Booking.WebSettings;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Railway_Ticket_Booking.Infrastructure.Services
{
    public class PayuService
    {
        private readonly PayuOptions _opt;
        private readonly HttpClient _httpClient;

        public PayuService(IOptions<PayuOptions> opt, IHttpClientFactory httpClientFactory)
        {
            _opt = opt.Value;
            _httpClient = httpClientFactory.CreateClient();
        }

        public string GenerateTxnId() =>
            Guid.NewGuid().ToString("N").Substring(0, 20);

        public string GenerateHash(string key, string txnid, string amount,
            string productinfo, string firstname, string email,
            string udf1 = "", string udf2 = "", string udf3 = "",
            string udf4 = "", string udf5 = "")
        {
            // PayU hash formula: sha512(key|txnid|amount|productinfo|firstname|email|udf1|udf2|udf3|udf4|udf5||||||SALT)
            var raw = $"{key}|{txnid}|{amount}|{productinfo}|{firstname}|{email}|{udf1}|{udf2}|{udf3}|{udf4}|{udf5}||||||{_opt.Salt}";
            using var sha = SHA512.Create();
            return string.Concat(
                sha.ComputeHash(Encoding.UTF8.GetBytes(raw))
                   .Select(b => b.ToString("x2"))
            );
        }

        public string GenerateRefundHash(string command, string var1, string hash)
        {
            // PayU refund hash: sha512(command|var1|SALT)
            var raw = $"{command}|{var1}|{_opt.Salt}";
            using var sha = SHA512.Create();
            return string.Concat(
                sha.ComputeHash(Encoding.UTF8.GetBytes(raw))
                   .Select(b => b.ToString("x2"))
            );
        }

        public string BuildForm(Dictionary<string, string> data)
        {
            var sb = new StringBuilder();
            // Use test or production URL based on TestMode
            var payuUrl = _opt.TestMode
                ? "https://test.payu.in/_payment"
                : "https://secure.payu.in/_payment";

            sb.Append($"<form id='payu' method='post' action='{payuUrl}'>");
            foreach (var d in data)
            {
                var encodedValue = WebUtility.HtmlEncode(d.Value);
                sb.Append($"<input type='hidden' name='{d.Key}' value='{encodedValue}' />");
            }

            sb.Append("</form><script>document.getElementById('payu').submit();</script>");
            return sb.ToString();
        }

        /// <summary>
        /// Process refund through PayU API
        /// </summary>
        public async Task<PayuRefundResponse> ProcessRefundAsync(string transactionId, decimal refundAmount, string refundReason = "")
        {
            try
            {
                // PayU Refund API endpoint
                var refundUrl = _opt.TestMode
                    ? "https://test.payu.in/merchant/postservice?form=2"
                    : "https://secure.payu.in/merchant/postservice?form=2";

                // Generate hash for refund
                var command = "cancel_refund_transaction";
                var hash = GenerateRefundHash(command, transactionId, "");

                // Prepare refund request
                var refundData = new Dictionary<string, string>
                {
                    { "key", _opt.Key },
                    { "command", command },
                    { "var1", transactionId },
                    { "var2", refundAmount.ToString("0.00") },
                    { "var3", refundReason },
                    { "hash", hash }
                };

                // Convert to form data
                var formData = new FormUrlEncodedContent(refundData);

                // Send refund request
                var response = await _httpClient.PostAsync(refundUrl, formData);
                var responseContent = await response.Content.ReadAsStringAsync();

                // Parse PayU response
                // PayU returns response in format: status|message|transaction_id
                var responseParts = responseContent.Split('|');
                
                if (responseParts.Length >= 2)
                {
                    var status = responseParts[0].Trim();
                    var message = responseParts.Length > 1 ? responseParts[1].Trim() : "";
                    var refundTxnId = responseParts.Length > 2 ? responseParts[2].Trim() : "";

                    return new PayuRefundResponse
                    {
                        Success = status == "success" || status == "1",
                        Message = message,
                        RefundTransactionId = refundTxnId,
                        OriginalTransactionId = transactionId,
                        RefundAmount = refundAmount
                    };
                }

                return new PayuRefundResponse
                {
                    Success = false,
                    Message = "Invalid response from PayU",
                    OriginalTransactionId = transactionId,
                    RefundAmount = refundAmount
                };
            }
            catch (Exception ex)
            {
                return new PayuRefundResponse
                {
                    Success = false,
                    Message = $"Error processing refund: {ex.Message}",
                    OriginalTransactionId = transactionId,
                    RefundAmount = refundAmount
                };
            }
        }
    }
}
