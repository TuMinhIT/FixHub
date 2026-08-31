using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixHub.Application.Payment.Command
{
    public sealed record CreatePaymentResponse(
     Guid PaymentId,
     string CheckoutUrl,
     IReadOnlyDictionary<string, string> Fields);
}
