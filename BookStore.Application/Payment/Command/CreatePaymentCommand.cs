using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixHub.Application.Payment.Command
{
    public sealed record CreatePaymentCommand(
    Guid OrderId
) : IRequest<CreatePaymentResponse>;
}
