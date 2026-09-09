using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixHub.Application.Payment
{
    public sealed record PaymentCheckoutRequest(
    string InvoiceNumber,
    decimal Amount,
    string Description,
    string SuccessUrl,
    string ErrorUrl,
    string CancelUrl);
}
