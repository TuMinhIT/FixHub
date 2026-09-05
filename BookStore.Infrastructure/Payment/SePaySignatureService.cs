
using System.Security.Cryptography;
using System.Text;

namespace FixHub.Infrastructure.Payment
{
    public static class SePaySignatureService
    {
        public static string Generate(
            IReadOnlyDictionary<string, string> fields,
            string secretKey)
        {
            string[] allowedFields =
            [
            "order_amount",
            "merchant",
            "currency",
            "operation",
            "order_description",
            "order_invoice_number",
            "customer_id",
            "payment_method",
            "success_url",
            "error_url",
            "cancel_url"
            ];

            var signedFields = new List<string>();

            foreach (var field in allowedFields)
            {
                if (!fields.TryGetValue(field, out var value))
                    continue;

                signedFields.Add($"{field}={value}");
            }

            var signedString = string.Join(",", signedFields);

            using var hmac = new HMACSHA256(
                Encoding.UTF8.GetBytes(secretKey));

            var hash = hmac.ComputeHash(
                Encoding.UTF8.GetBytes(signedString));

            return Convert.ToBase64String(hash);
        }
    }
}
