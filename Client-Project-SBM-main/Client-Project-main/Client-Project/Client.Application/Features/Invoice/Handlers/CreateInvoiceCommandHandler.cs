using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Client.Application.Features.Invoice.Commands;
using Client.Application.Features.Invoice.Dtos;
using Client.Application.Interfaces;
using MediatR;

namespace Client.Application.Features.Invoice.Handlers
{
    public class CreateInvoiceCommandHandler : IRequestHandler<CreateInvoiceCommand,List<InvoiceDetailsDto>>
    {
        private readonly IInvoiceRepository _invoiceRepository;
        public CreateInvoiceCommandHandler(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }
        public async Task<List<InvoiceDetailsDto>> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
        {
            return await _invoiceRepository.CreateInvoiceAsync(request.Invoice);
        }
    }
}
