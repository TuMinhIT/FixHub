using FixHub.Application.Common.Models;
using FixHub.Infrastructure.Payment;
using Microsoft.AspNetCore.Mvc;

namespace FixHub.API.Controllers
{
    [ApiController]
    [Route("api/sepay")]
    public class SePayIPNendpoint : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public SePayIPNendpoint(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("ipn")]
        public async Task<IActionResult> ReceiveIpn([FromForm] Dictionary<string, string> formData)
        {
            if (formData.Count == 0)
            {
                return BadRequest(new ApiResponse<string>(null!, "No payload received") { Success = false });
            }

            var secretKey = _configuration["SePay:SecretKey"] ?? string.Empty;
            var providedSignature = formData.GetValueOrDefault("signature");

            if (string.IsNullOrWhiteSpace(secretKey) || string.IsNullOrWhiteSpace(providedSignature))
            {
                return BadRequest(new ApiResponse<string>(null!, "Invalid SePay callback") { Success = false });
            }

            var expectedSignature = SePaySignatureService.Generate(formData, secretKey);
            if (!string.Equals(expectedSignature, providedSignature, StringComparison.Ordinal))
            {
                return BadRequest(new ApiResponse<string>(null!, "SePay signature mismatch") { Success = false });
            }

            var orderIdRaw = formData.GetValueOrDefault("order_id") ?? formData.GetValueOrDefault("order_invoice_number");
            if (string.IsNullOrWhiteSpace(orderIdRaw))
            {
                return BadRequest(new ApiResponse<string>(null!, "Missing order reference") { Success = false });
            }

            Console.WriteLine($"SePay IPN received for Order ID: {orderIdRaw}");
            return Ok(new ApiResponse<string>("OK", "SePay callback processed successfully"));
        }
    }
}
