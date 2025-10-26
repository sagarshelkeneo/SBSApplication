using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.Payment.Dtos;
using MediatR;

namespace Client.Application.Features.Payment.Commands
{
    public record UpdatePaymentCommand(UpdatePaymentDto Payment) : IRequest<List<PaymentDetailsDto>>;

}
