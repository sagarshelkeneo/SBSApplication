using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.Payment.Dtos;
using MediatR;

namespace Client.Application.Features.Payment.Queries
{
    public record GetPaymentDetailsQuery(int companyId,int? Id) : IRequest<List<PaymentDetailsDto>>;

}
