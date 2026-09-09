using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixHub.Application.Payment
{
    public sealed record PaymentCheckoutResult(
     string CheckoutUrl,
     IReadOnlyDictionary<string, string> Fields);
}
