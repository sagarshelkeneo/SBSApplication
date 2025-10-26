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
    public class UpdatePaymentCommandHandler : IRequestHandler<UpdatePaymentCommand, List<PaymentDetailsDto>>
    {
        private readonly IPaymentRepository _repository;

        public UpdatePaymentCommandHandler(IPaymentRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<PaymentDetailsDto>> Handle(UpdatePaymentCommand request, CancellationToken cancellationToken)
        {
            return await _repository.UpdatePaymentAsync(request.Payment);
        }
    }
}
