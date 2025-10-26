using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Features.Invoice.Commands;
using Client.Application.Features.Invoice.Dtos;
using Client.Application.Interfaces;
using MediatR;

namespace Client.Application.Features.Invoice.Handlers
{
    public class UpdateInvoiceCommandHandler : IRequestHandler<UpdateInvoiceCommand, List<InvoiceDetailsDto>>
    {
        private readonly IInvoiceRepository _repo;

        public UpdateInvoiceCommandHandler(IInvoiceRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<InvoiceDetailsDto>> Handle(UpdateInvoiceCommand request, CancellationToken cancellationToken)
        {
            return await _repo.UpdateInvoiceAsync(request.Invoice);
        }
    }

}
