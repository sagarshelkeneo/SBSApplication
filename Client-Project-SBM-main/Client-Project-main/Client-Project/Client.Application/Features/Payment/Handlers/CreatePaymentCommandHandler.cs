using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.Payment.Commands;
using Client.Application.Features.Payment.Dtos;
using Client.Application.Interfaces;
using MediatR;

namespace Client.Application.Features.Payment.Handlers
{
    public class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, List<PaymentDetailsDto>>
    {
        private readonly IPaymentRepository _repository;

        public CreatePaymentCommandHandler(IPaymentRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<PaymentDetailsDto>> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
        {
            var result = await _repository.CreatePaymentAsync(request.Payment);

            if (result == null || result.Count == 0)
                throw new Exception("Payment insert failed or returned no records.");

            return result;
        }
    }

}
