using Microsoft.Extensions.Options;
using Railway_Ticket_Booking.WebSettings;
using System.Net;
using System.Security.Cryptography;
using System.Text;
namespace Railway_Ticket_Booking.Infrastructure.Services
{
    public class PayuService
    {
        private readonly PayuOptions _opt;

        public PayuService(IOptions<PayuOptions> opt)
        {
            _opt = opt.Value;
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
    }
}
