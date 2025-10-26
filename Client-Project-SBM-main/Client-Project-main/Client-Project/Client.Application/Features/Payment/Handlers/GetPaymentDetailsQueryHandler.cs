using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.Payment.Dtos;
using Client.Application.Features.Payment.Queries;
using Client.Application.Interfaces;
using MediatR;

namespace Client.Application.Features.Payment.Handlers
{
    public class GetPaymentDetailsQueryHandler : IRequestHandler<GetPaymentDetailsQuery, List<PaymentDetailsDto>>
    {
        private readonly IPaymentRepository _repository;

        public GetPaymentDetailsQueryHandler(IPaymentRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<PaymentDetailsDto>> Handle(GetPaymentDetailsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetPaymentsAsync(request.companyId,request.Id);
        }
    }

}
